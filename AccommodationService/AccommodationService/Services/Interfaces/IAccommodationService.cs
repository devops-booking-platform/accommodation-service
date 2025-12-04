using AccommodationService.Domain.DTOs;

namespace AccommodationService.Services.Interfaces;

public interface IAccommodationService
{
    Task Create(AccommodationRequest request);
	Task<IReadOnlyList<HostAccommodationListItemDTO>> GetMyAsync(CancellationToken ct);
	Task<AccommodationReservationInfoResponseDTO> GetReservationInfoAsync(Guid id, DateTimeOffset start, DateTimeOffset end, int guests, CancellationToken ct);
}