namespace AccommodationService.Common.Events.Published
{
    public record AvailabilityUpsertedIntegrationEvent(Guid AccommodationId, Guid AvailabilityId, DateOnly StartDate, DateOnly EndDate, decimal Price) : IIntegrationEvent;
}
