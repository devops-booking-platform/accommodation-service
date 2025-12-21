using AccommodationService.Domain.DTOs;

namespace AccommodationService.Common.Events.Published
{
    public record AccommodationCreatedIntegrationEvent(Guid AccommodationId, string Name, int MaxGuest, int MinGuest, List<AmenityDTO> Amenities, LocationDTO? Location) : IIntegrationEvent;
}
