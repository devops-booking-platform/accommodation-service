using AccommodationService.Common.Events;

namespace AccommodationService.IntegrationEvents.Handlers
{
    public sealed class ReservationCreatedIntegrationEventHandler
    : IIntegrationEventHandler<ReservationCreatedIntegrationEvent>
    {
        private readonly ILogger<ReservationCreatedIntegrationEventHandler> _logger;

        public ReservationCreatedIntegrationEventHandler(ILogger<ReservationCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ReservationCreatedIntegrationEvent @event, CancellationToken ct)
        {
            _logger.LogInformation("Handling ReservationCreatedIntegrationEvent for UserId={UserId}, Role={Role}",
                @event.UserId, @event.Role);
            await Task.CompletedTask;
        }
    }
}
