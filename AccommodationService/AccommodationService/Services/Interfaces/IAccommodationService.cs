using AccommodationService.Domain.DTOs;

namespace AccommodationService.Services.Interfaces;

public interface IAccommodationService
{
    Task Create(AccommodationRequest request);
}