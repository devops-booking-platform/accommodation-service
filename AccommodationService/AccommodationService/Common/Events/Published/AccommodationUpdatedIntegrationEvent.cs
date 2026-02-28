using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Enums;

namespace AccommodationService.Common.Events.Published;

public record AccommodationUpdatedIntegrationEvent(
    Guid AccommodationId,
    string Name,
    int MaxGuest,
    int MinGuest,
    List<AmenityDTO> Amenities,
    LocationDTO? Location,
    PriceType PriceType) : IIntegrationEvent;