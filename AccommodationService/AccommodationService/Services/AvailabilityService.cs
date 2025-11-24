using AccommodationService.Common.Exceptions;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.Repositories.Interfaces;
using AccommodationService.Services.Interfaces;

namespace AccommodationService.Services;

public class AvailabilityService(
    IRepository<Availability> availabilityRepository,
    IRepository<Accommodation> accommodationRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IAvailabilityService
{
    public async Task CreateOrUpdate(AvailabilityRequest request)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("You don't have access to this action.");
        }

        var accommodation = await accommodationRepository
            .GetByIdAsync(request.AccommodationId);
        ValidateAccommodation(accommodation, userId.Value);

        ValidateAvailability(request);
        
        if (request.Id.HasValue)
        {
            var availability = await availabilityRepository
                .GetByIdAsync(request.Id.Value) ?? throw new NotFoundException("Availability does not exist.");

            availability.Update(request);
            return;
        }

        var newAvailability = Availability.Create(request);
        await availabilityRepository.AddAsync(newAvailability);

        await unitOfWork.SaveChangesAsync();
    }

    private void ValidateAvailability(AvailabilityRequest request)
    {
        var isOverlapping = availabilityRepository
            .Query()
            .Where(a => a.AccommodationId == request.AccommodationId)
            .Where(a => request.Id == null || a.Id != request.Id)
            .Any(a => (request.StartDate < a.EndDate && request.EndDate > a.StartDate));

        if (isOverlapping)
        {
            throw new InvalidOperationException("Availability overlaps with existing intervals.");
        }
    }

    private static void ValidateAccommodation(Accommodation? accommodation, Guid userId)
    {
        if (accommodation == null)
        {
            throw new NotFoundException("Accommodation does not exist.");
        }

        if (accommodation.HostId != userId)
        {
            throw new UnauthorizedAccessException("You don't have access to this action.");
        }
    }
}