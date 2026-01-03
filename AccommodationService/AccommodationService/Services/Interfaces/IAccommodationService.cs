using AccommodationService.Domain.DTOs;

namespace AccommodationService.Services.Interfaces;

public interface IAccommodationService
{
    Task Create(AccommodationRequest request);
    Task DeleteHostAccommodationsAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<HostAccommodationListItemDto>> GetMyAsync(CancellationToken ct);
    Task<GetAccommodationResponse> Get(Guid id, CancellationToken ct);
    Task<ICollection<AmenityResponseDto>> GetAmenities(CancellationToken ct);
	Task<AccommodationReservationInfoResponseDto> GetReservationInfoAsync(Guid id, DateOnly start, DateOnly end, int guests, CancellationToken ct);
}