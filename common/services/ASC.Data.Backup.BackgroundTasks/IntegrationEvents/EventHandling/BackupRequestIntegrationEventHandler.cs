namespace ASC.Data.Backup.IntegrationEvents.EventHandling;

[Scope]
public class BackupRequestIntegrationEventHandler : IIntegrationEventHandler<BackupRequestIntegrationEvent>
{
    private readonly BackupAjaxHandler _backupAjaxHandler;
    private readonly ILog _logger;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly AuthManager _authManager;
    private readonly BackupWorker _backupWorker;

    public BackupRequestIntegrationEventHandler(
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
    
    public async Task Handle(BackupRequestIntegrationEvent @event)
    {
        _logger.InfoFormat("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        _tenantManager.SetCurrentTenant(@event.TenantId);

        _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(@event.TenantId, @event.CreateBy));

        if (@event.IsScheduled)
        {
            _backupWorker.StartScheduledBackup(new EF.Model.BackupSchedule
            {
                 BackupMail = @event.BackupMail,
                 BackupsStored = @event.BackupsStored,
                 StorageBasePath = @event.StorageBasePath,
                 StorageParams  = JsonConvert.SerializeObject(@event.StorageParams),
                 StorageType = @event.StorageType,
                 TenantId = @event.TenantId
            });
        }
        else
        {
            _backupAjaxHandler.StartBackup(@event.StorageType, @event.StorageParams, @event.BackupMail);
        }

        await Task.CompletedTask;
    }
}