using ASC.EventBus.Exceptions;

namespace ASC.Data.Backup.IntegrationEvents.EventHandling;

[Scope]
public class BackupRestoreRequestIntegrationEventHandler : IIntegrationEventHandler<BackupRestoreRequestIntegrationEvent>
{
    private readonly BackupAjaxHandler _backupAjaxHandler;
    private readonly ILog _logger;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly AuthManager _authManager;
    private readonly BackupWorker _backupWorker;

    public BackupRestoreRequestIntegrationEventHandler(
        BackupAjaxHandler backupAjaxHandler,
        IOptionsMonitor<ILog> logger,
        TenantManager tenantManager,
        SecurityContext securityContext,
        AuthManager authManager,
        BackupWorker backupWorker)        
    {       
        _tenantManager = tenantManager;
        _authManager = authManager;
        _securityContext = securityContext;
        _backupAjaxHandler = backupAjaxHandler;
        _logger = logger.CurrentValue;
        _backupWorker = backupWorker;
    }
    
    public async Task Handle(BackupRestoreRequestIntegrationEvent @event)
    {
        _logger.InfoFormat("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        if (!@event.Redelivered)
        {
            if (_backupWorker.HaveBackupRestoreRequestWaitingTasks())
            {
                throw new IntegrationEventRejectExeption(@event.Id);
            }
        }

        _tenantManager.SetCurrentTenant(@event.TenantId);
        _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(@event.TenantId, @event.CreateBy));

        _backupAjaxHandler.StartRestore(@event.BackupId,
                                        @event.StorageType,
                                        @event.StorageParams,
                                        @event.Notify);

        await Task.CompletedTask;
    }
}