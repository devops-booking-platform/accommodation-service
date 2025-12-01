namespace AccommodationService.Common.Events
{
    public interface IIntegrationEvent { }
    public record UserDeletedIntegrationEvent(Guid UserId, string Role) : IIntegrationEvent;
    public record ReservationCreatedIntegrationEvent(Guid UserId, string Role, string check) : IIntegrationEvent;
}
