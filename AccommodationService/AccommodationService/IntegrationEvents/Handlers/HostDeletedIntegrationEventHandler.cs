using AccommodationService.Common.Events;
using AccommodationService.Common.Events.Received;
using AccommodationService.Services.Interfaces;

public sealed class HostDeletedIntegrationEventHandler
    : IIntegrationEventHandler<HostDeletedIntegrationEvent>
{
    private readonly ILogger<HostDeletedIntegrationEventHandler> _logger;
    private readonly IAccommodationService _accommodationService;

    public HostDeletedIntegrationEventHandler(ILogger<HostDeletedIntegrationEventHandler> logger, IAccommodationService accommodationService)
    {
        _accommodationService = accommodationService;
        _logger = logger;
    }

    public async Task Handle(HostDeletedIntegrationEvent @event, CancellationToken ct)
    {
        await _accommodationService.DeleteHostAccommodationsAsync(@event.UserId, ct);
        _logger.LogInformation("Handling HostDeletedIntegrationEventHandler for UserId={UserId}",
            @event.UserId);
    }
}
