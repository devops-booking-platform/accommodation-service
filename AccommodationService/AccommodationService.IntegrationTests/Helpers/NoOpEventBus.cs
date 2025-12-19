using AccommodationService.Common.Events;

namespace AccommodationService.IntegrationTests.Helpers
{
    public sealed class NoOpEventBus : IEventBus
    {
        public Task PublishAsync<T>(T @event, CancellationToken ct = default)
            where T : IIntegrationEvent
            => Task.CompletedTask;
    }
}
