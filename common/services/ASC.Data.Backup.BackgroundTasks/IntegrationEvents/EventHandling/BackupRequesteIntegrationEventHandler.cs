using ASC.Core;
using ASC.Data.Backup.Core.IntegrationEvents.Events;
using ASC.EventBus.Exceptions;

namespace ASC.Data.Backup.IntegrationEvents.EventHandling;

[Scope]
public class BackupRequesteIntegrationEventHandler : IIntegrationEventHandler<BackupRequestIntegrationEvent>
{
    private readonly BackupAjaxHandler _backupAjaxHandler;
    private readonly ILog _logger;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly AuthManager _authManager;

    public BackupRequesteIntegrationEventHandler(
        BackupAjaxHandler backupAjaxHandler,
        IOptionsMonitor<ILog> logger,
        TenantManager tenantManager,
        SecurityContext securityContext,
        AuthManager authManager
        )
    {       
        _tenantManager = tenantManager;
        _authManager = authManager;
        _securityContext = securityContext;
        _backupAjaxHandler = backupAjaxHandler;
        _logger = logger.CurrentValue;            
    }
    
    public async Task Handle(BackupRequestIntegrationEvent @event)
    {
        _logger.InfoFormat("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        _tenantManager.SetCurrentTenant(@event.TenantId);

        _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(@event.TenantId, @event.CreateBy));
    
        _backupAjaxHandler.StartBackup(@event.StorageType, @event.StorageParams, @event.BackupMail);

        throw new IntegrationEventRejectExeption("Backup service is very busy");

        await Task.CompletedTask;

    }
}