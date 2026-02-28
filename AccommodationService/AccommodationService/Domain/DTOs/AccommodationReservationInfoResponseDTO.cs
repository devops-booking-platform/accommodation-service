namespace AccommodationService.Domain.DTOs;

public sealed record AccommodationReservationInfoResponseDto(
    Guid HostId,
    string Name,
    int MaxGuests,
    decimal TotalPrice,
    bool IsAutoAcceptEnabled
);