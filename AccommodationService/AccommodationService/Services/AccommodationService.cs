using AccommodationService.Common.Exceptions;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
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
    IUnitOfWork unitOfWork) : IAccommodationService
{
    public async Task Create(AccommodationRequest request)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("You don't have access to this action.");
        }

        var accommodation = Accommodation.Create(request, userId.Value);

        await GetAmenities(request, accommodation);

        var photos = CreatePhotos(request, accommodation);

        await accommodationRepository.AddAsync(accommodation);
        await photoRepository.AddRangeAsync(photos);
        await unitOfWork.SaveChangesAsync();
    }

    private static List<Photo> CreatePhotos(AccommodationRequest request, Accommodation accommodation)
        => request.Photos.Select(photo => Photo.Create(photo, accommodation.Id)).ToList();

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

        var accommodation = await accommodationRepository.GetByIdAsync(accommodationId) ?? throw new NotFoundException("Accommodation not found.");

        if (guests > accommodation.MaximumNumberOfGuests)
            throw new ConflictException("Guests exceed max capacity.");

        var isCoveredByAvailability = await IsIntervalAvailableAsync(accommodationId, start, end, ct);
        if (!isCoveredByAvailability)
            throw new ConflictException("Accommodation is not available for the selected dates.");

        var totalPrice = 100;

        return new AccommodationReservationInfoResponseDTO(
            Name: accommodation.Name,
            HostId: accommodation.HostId,
            MaxGuests: accommodation.MaximumNumberOfGuests,
            IsAutoAcceptEnabled: accommodation.IsAutoConfirm,
            TotalPrice: totalPrice
        );
    }
    private Task<bool> IsIntervalAvailableAsync(Guid accommodationId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken ct = default)
    {
        return availabilityRepository.Query()
            .AnyAsync(a =>
                a.AccommodationId == accommodationId &&
                a.StartDate <= startDate &&
                a.EndDate >= endDate,
                ct);
    }

	public async Task<IReadOnlyList<HostAccommodationListItemDTO>> GetMyAsync(CancellationToken ct)
	{
		var userId = currentUserService.UserId;
		if (!userId.HasValue)
		{
			throw new UnauthorizedAccessException("You don't have access to this action.");
		}
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
}