namespace AccommodationService.Common.Events.Published
{
    public record AvailabilityUpsertedIntegrationEvent(Guid AccommodationId, Guid AvailabilityId, DateTimeOffset StartDate, DateTimeOffset EndDate, decimal Price) : IIntegrationEvent;
}
