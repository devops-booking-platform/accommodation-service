namespace AccommodationService.Common.Events.Received
{
    public record HostDeletedIntegrationEvent(Guid UserId) : IIntegrationEvent;
}
