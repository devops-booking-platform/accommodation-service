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
    public Task CreateOrUpdate(AvailabilityRequest request)
    {
        throw new NotImplementedException();
    }
}