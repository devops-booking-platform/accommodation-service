namespace AccommodationService.Common.Events.Received
{
    public record ReservationCreatedIntegrationEvent(Guid UserId, string Role, string Check) : IIntegrationEvent;
}
