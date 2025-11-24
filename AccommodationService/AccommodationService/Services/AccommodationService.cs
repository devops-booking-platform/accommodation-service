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
}