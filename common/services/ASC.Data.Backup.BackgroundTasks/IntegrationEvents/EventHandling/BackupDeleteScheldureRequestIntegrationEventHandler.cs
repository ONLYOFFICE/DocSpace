using ASC.EventBus.Events;

namespace ASC.Data.Backup.IntegrationEvents.EventHandling;

[Scope]
public class BackupDeleteScheldureRequestIntegrationEventHandler : IIntegrationEventHandler<IntegrationEvent>
{
    private readonly ILog _logger;
    private readonly BackupService _backupService;

    public BackupDeleteScheldureRequestIntegrationEventHandler(
        IOptionsMonitor<ILog> logger,
        BackupService backupService)        
    {       
        _logger = logger.CurrentValue;
        _backupService = backupService;
    }
    
    public async Task Handle(IntegrationEvent @event)
    {
        _logger.InfoFormat("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        _backupService.DeleteSchedule(@event.TenantId);

        await Task.CompletedTask;
    }
}