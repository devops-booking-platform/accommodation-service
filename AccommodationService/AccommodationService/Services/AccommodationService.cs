using AccommodationService.Common.Events;
using AccommodationService.Common.Events.Published;
using AccommodationService.Common.Exceptions;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.Domain.Enums;
using AccommodationService.Repositories.Interfaces;
using AccommodationService.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AccommodationService.Services;

public class AccommodationService(
    IRepository<Accommodation> accommodationRepository,
    IRepository<Amenity> amenityRepository,
    IRepository<Photo> photoRepository,
    IRepository<Availability> availabilityRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEventBus eventBus
) : IAccommodationService
{
    public async Task Create(AccommodationRequest request)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("You don't have access to this action.");

        var accommodation = Accommodation.Create(request, userId.Value);

        await GetAmenitiesForAccommodation(request, accommodation);

        var photos = CreatePhotos(request, accommodation);

        await accommodationRepository.AddAsync(accommodation);
        await photoRepository.AddRangeAsync(photos);

        await unitOfWork.SaveChangesAsync();

        var @event = new AccommodationCreatedIntegrationEvent(
            accommodation.Id,
            accommodation.Name,
            accommodation.MaximumNumberOfGuests,
            accommodation.MinimumNumberOfGuests,
            MapAmenities(accommodation),
            MapLocation(accommodation),
            accommodation.PriceType
        );

        await eventBus.PublishAsync(@event);
    }

    private static LocationDTO? MapLocation(Accommodation accommodation)
        => accommodation.Location is null
            ? null
            : new LocationDTO
            {
                Address = accommodation.Location.Address,
                City = accommodation.Location.City,
                Country = accommodation.Location.Country,
                PostalCode = accommodation.Location.PostalCode
            };

    private static List<AmenityDTO> MapAmenities(Accommodation accommodation)
        => accommodation.Amenities
            .Select(a => new AmenityDTO
            {
                Name = a.Name,
                Description = a.Description
            })
            .ToList();

    private static List<Photo> CreatePhotos(AccommodationRequest request, Accommodation accommodation)
        => request.Photos
            .Select(photo => Photo.Create(photo, accommodation.Id))
            .ToList();

    private async Task GetAmenitiesForAccommodation(AccommodationRequest request, Accommodation accommodation)
    {
        var amenities = await amenityRepository.Query()
            .Where(x => request.Amenities.Contains(x.Id))
            .ToListAsync();

        accommodation.Amenities = amenities;
    }
    
    public async Task<GetAccommodationResponse> Get(Guid id, CancellationToken ct)
    {
        var accommodation = await accommodationRepository.Query()
            .Where(x => x.Id == id)
            .Include(a => a.Location)
            .Include(a => a.Photos)
            .Include(a => a.Amenities)
            .Include(a => a.Availabilities)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (accommodation is null)
        {
            throw new NotFoundException("Accommodation not found.");
        }

        return mapper.Map<GetAccommodationResponse>(accommodation);
    }

    public async Task<ICollection<AmenityResponseDto>> GetAmenities(CancellationToken ct)
    {
        var amenities = await amenityRepository.Query()
            .ToListAsync(ct);
        return mapper.Map<ICollection<AmenityResponseDto>>(amenities);
    }

    public async Task<AccommodationReservationInfoResponseDto> GetReservationInfoAsync(
        Guid accommodationId,
        DateOnly start,
        DateOnly end,
        int guests,
        CancellationToken ct = default)
    {
        if (end <= start)
            throw new ArgumentOutOfRangeException(nameof(end), "End date must be after start date.");

        if (guests <= 0)
            throw new ArgumentOutOfRangeException(nameof(guests), "Guests must be positive.");

        var accommodation = await accommodationRepository.GetByIdAsync(accommodationId)
            ?? throw new NotFoundException("Accommodation not found.");

        if (guests > accommodation.MaximumNumberOfGuests)
            throw new ConflictException("Guests exceed max capacity.");

        var totalPrice = await CalculateTotalPriceAsync(accommodationId, accommodation.PriceType, start, end, guests, ct);

        return new AccommodationReservationInfoResponseDto(
            Name: accommodation.Name,
            HostId: accommodation.HostId,
            MaxGuests: accommodation.MaximumNumberOfGuests,
            IsAutoAcceptEnabled: accommodation.IsAutoConfirm,
            TotalPrice: totalPrice
        );
    }

    private async Task<decimal> CalculateTotalPriceAsync(
        Guid accommodationId,
        PriceType priceType,
        DateOnly start,
        DateOnly end,
        int guests,
        CancellationToken ct)
    {
        if (guests <= 0)
            throw new ArgumentOutOfRangeException(nameof(guests), "Guests must be positive.");

        var intervals = await availabilityRepository.Query()
            .AsNoTracking()
            .Where(a => a.AccommodationId == accommodationId)
            .Where(a => a.EndDate > start && a.StartDate < end) 
            .Select(a => new { a.StartDate, a.EndDate, a.Price })
            .ToListAsync(ct);

        if (intervals.Count == 0)
            throw new ConflictException("Accommodation is not available for the selected dates.");

        var dayIntervals = intervals
            .Select(x => new
            {
                StartDay = x.StartDate,
                EndDay = x.EndDate,
                x.Price
            })
            .Where(x => x.EndDay > start && x.StartDay < end)
            .OrderBy(x => x.StartDay.DayNumber)
            .ToList();

        if (dayIntervals.Count == 0)
            throw new ConflictException("Accommodation is not available for the selected dates.");

        var points = new List<DateOnly> { start, end };
        foreach (var it in dayIntervals)
        {
            if (it.StartDay > start && it.StartDay < end) points.Add(it.StartDay);
            if (it.EndDay > start && it.EndDay < end) points.Add(it.EndDay);
        }

        points = points
            .Distinct()
            .OrderBy(x => x.DayNumber)
            .ToList();

        decimal total = 0m;

        for (int i = 0; i < points.Count - 1; i++)
        {
            var segStart = points[i];
            var segEnd = points[i + 1];

            if (segEnd <= segStart)
                continue;

            var covering = dayIntervals.FirstOrDefault(a => a.StartDay <= segStart && a.EndDay >= segEnd);
            if (covering is null)
                throw new ConflictException("Accommodation is not available for the selected dates.");

            var nights = segEnd.DayNumber - segStart.DayNumber;
            if (nights <= 0)
                continue;

            var perNight = priceType == PriceType.PerGuest
                ? covering.Price * guests
                : covering.Price;

            total += perNight * nights;
        }

        return total;
    }


    public async Task<IReadOnlyList<HostAccommodationListItemDto>> GetMyAsync(CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("You don't have access to this action.");

        return await accommodationRepository.Query()
            .Where(x => x.HostId == userId.Value)
            .Select(x => new HostAccommodationListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Location == null
                    ? ""
                    : $"{x.Location.City}, {x.Location.Country}, {x.Location.Address}",
                MinGuests = x.MinimumNumberOfGuests,
                MaxGuests = x.MaximumNumberOfGuests
            })
            .ToListAsync(ct);
    }

    public async Task DeleteHostAccommodationsAsync(Guid hostId, CancellationToken ct)
    {

        var accommodationIds = await accommodationRepository
        .Query()
        .Where(a => a.HostId == hostId)
        .Select(a => a.Id)
        .ToListAsync(ct);

        if (accommodationIds.Count == 0)
            return;

        await accommodationRepository
            .Query()
            .Where(a => a.HostId == hostId)
            .ExecuteDeleteAsync(ct);

        await eventBus.PublishAsync(
            new HostAccommodationsDeletedIntegrationEvent(accommodationIds),
            ct);
    }
}