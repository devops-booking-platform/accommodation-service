namespace AccommodationService.Domain.DTOs
{
    public sealed record AccommodationReservationInfoResponseDTO(
    Guid HostId,
    string Name,
    int MaxGuests,
    decimal TotalPrice,
    bool IsAutoAcceptEnabled
);
}
