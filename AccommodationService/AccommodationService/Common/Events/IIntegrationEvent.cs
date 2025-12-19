using AccommodationService.Domain.DTOs;

namespace AccommodationService.Common.Events
{
    public interface IIntegrationEvent { }
    public record UserDeletedIntegrationEvent(Guid UserId, string Role) : IIntegrationEvent;
    public record ReservationCreatedIntegrationEvent(Guid UserId, string Role, string check) : IIntegrationEvent;
    public record AccommodationCreatedEvent(Guid AccommodationId, string Name, int MaxGuest, int MinGuest, List<AmenityDTO> Amenities, LocationDTO? Location) : IIntegrationEvent;
}
