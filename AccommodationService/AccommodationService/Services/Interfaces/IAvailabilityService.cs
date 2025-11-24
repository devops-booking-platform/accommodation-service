using AccommodationService.Domain.DTOs;

namespace AccommodationService.Services.Interfaces;

public interface IAvailabilityService
{
    Task CreateOrUpdate(AvailabilityRequest request);
}