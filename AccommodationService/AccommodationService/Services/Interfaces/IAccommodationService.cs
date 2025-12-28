using AccommodationService.Domain.DTOs;

namespace AccommodationService.Services.Interfaces;

public interface IAccommodationService
{
    Task Create(AccommodationRequest request);
    Task DeleteHostAccommodationsAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<HostAccommodationListItemDTO>> GetMyAsync(CancellationToken ct);
	Task<AccommodationReservationInfoResponseDTO> GetReservationInfoAsync(Guid id, DateOnly start, DateOnly end, int guests, CancellationToken ct);
}