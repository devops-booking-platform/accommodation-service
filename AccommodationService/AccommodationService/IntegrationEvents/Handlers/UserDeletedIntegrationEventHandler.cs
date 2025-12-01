using AccommodationService.Common.Events;

public sealed class UserDeletedIntegrationEventHandler
    : IIntegrationEventHandler<UserDeletedIntegrationEvent>
{
    private readonly ILogger<UserDeletedIntegrationEventHandler> _logger;

    public UserDeletedIntegrationEventHandler(ILogger<UserDeletedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserDeletedIntegrationEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Handling UserDeletedIntegrationEvent for UserId={UserId}, Role={Role}",
            @event.UserId, @event.Role);
        await Task.CompletedTask;
    }
}
