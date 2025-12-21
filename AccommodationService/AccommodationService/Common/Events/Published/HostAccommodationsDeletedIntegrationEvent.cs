namespace AccommodationService.Common.Events.Published
{
    public record HostAccommodationsDeletedIntegrationEvent(IReadOnlyList<Guid> AccommodationIds) : IIntegrationEvent;
}
