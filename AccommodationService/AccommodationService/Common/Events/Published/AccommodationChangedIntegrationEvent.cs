using AccommodationService.Domain.DTOs;

namespace AccommodationService.Common.Events.Published
{
    public record AccommodationChangedIntegrationEvent(Guid AccommodationId, string Name, int MaxGuest, int MinGuest, List<AmenityDTO> Amenities, LocationDTO? Location) : IIntegrationEvent;
}
