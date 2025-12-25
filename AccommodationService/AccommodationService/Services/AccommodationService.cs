using AccommodationService.Common.Events;
using AccommodationService.Common.Events.Published;
using AccommodationService.Common.Exceptions;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.Domain.Enums;
using AccommodationService.Repositories.Interfaces;
using AccommodationService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AccommodationService.Services;

public class AccommodationService(
    IRepository<Accommodation> accommodationRepository,
    IRepository<Amenity> amenityRepository,
    IRepository<Photo> photoRepository,
    IRepository<Availability> availabilityRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IEventBus eventBus
) : IAccommodationService
{
    public async Task Create(AccommodationRequest request)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("You don't have access to this action.");

        var accommodation = Accommodation.Create(request, userId.Value);

        await GetAmenities(request, accommodation);

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

    private async Task GetAmenities(AccommodationRequest request, Accommodation accommodation)
    {
        var amenities = await amenityRepository.Query()
            .Where(x => request.Amenities.Contains(x.Id))
            .ToListAsync();

        accommodation.Amenities = amenities;
    }

    public async Task<AccommodationReservationInfoResponseDTO> GetReservationInfoAsync(
        Guid accommodationId,
        DateTimeOffset start,
        DateTimeOffset end,
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

        return new AccommodationReservationInfoResponseDTO(
            Name: accommodation.Name,
            HostId: accommodation.HostId,
            MaxGuests: accommodation.MaximumNumberOfGuests,
            IsAutoAcceptEnabled: accommodation.IsAutoConfirm,
            TotalPrice: totalPrice
        );
    }

    private static (DateTimeOffset StartUtc, DateTimeOffset EndUtc) NormalizeToUtcMidnights(
        DateTimeOffset start,
        DateTimeOffset end)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();

        var startMidnightUtc = new DateTimeOffset(
            startUtc.Year, startUtc.Month, startUtc.Day, 0, 0, 0, TimeSpan.Zero);

        var endMidnightUtc = new DateTimeOffset(
            endUtc.Year, endUtc.Month, endUtc.Day, 0, 0, 0, TimeSpan.Zero);

        if (endMidnightUtc <= startMidnightUtc)
            throw new ArgumentOutOfRangeException(nameof(end), "End date must be after start date.");

        return (startMidnightUtc, endMidnightUtc);
    }

    private async Task<decimal> CalculateTotalPriceAsync(
        Guid accommodationId,
        PriceType priceType,
        DateTimeOffset start,
        DateTimeOffset end,
        int guests,
        CancellationToken ct)
    {
        if (guests <= 0)
            throw new ArgumentOutOfRangeException(nameof(guests), "Guests must be positive.");

        var (startUtc, endUtc) = NormalizeToUtcMidnights(start, end);

        var intervals = await availabilityRepository.Query()
            .AsNoTracking()
            .Where(a => a.AccommodationId == accommodationId)
            .Where(a => a.EndDate > startUtc && a.StartDate < endUtc) 
            .Select(a => new { a.StartDate, a.EndDate, a.Price })
            .ToListAsync(ct);

        if (intervals.Count == 0)
            throw new ConflictException("Accommodation is not available for the selected dates.");

        var startDay = DateOnly.FromDateTime(startUtc.UtcDateTime);
        var endDay = DateOnly.FromDateTime(endUtc.UtcDateTime);

        var dayIntervals = intervals
            .Select(x => new
            {
                StartDay = DateOnly.FromDateTime(x.StartDate.UtcDateTime),
                EndDay = DateOnly.FromDateTime(x.EndDate.UtcDateTime),
                x.Price
            })
            .Where(x => x.EndDay > startDay && x.StartDay < endDay)
            .OrderBy(x => x.StartDay.DayNumber)
            .ToList();

        if (dayIntervals.Count == 0)
            throw new ConflictException("Accommodation is not available for the selected dates.");

        var points = new List<DateOnly> { startDay, endDay };
        foreach (var it in dayIntervals)
        {
            if (it.StartDay > startDay && it.StartDay < endDay) points.Add(it.StartDay);
            if (it.EndDay > startDay && it.EndDay < endDay) points.Add(it.EndDay);
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


    public async Task<IReadOnlyList<HostAccommodationListItemDTO>> GetMyAsync(CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("You don't have access to this action.");

        return await accommodationRepository.Query()
            .Where(x => x.HostId == userId.Value)
            .Select(x => new HostAccommodationListItemDTO
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