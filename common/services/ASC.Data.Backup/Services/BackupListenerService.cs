namespace ASC.Data.Backup.Services;

[Singletone]
internal sealed class BackupListenerService : IHostedService
{
    private readonly IEventBus<DeleteSchedule> _cacheDeleteSchedule;
    private readonly IServiceScopeFactory _scopeFactory;

    public BackupListenerService(IEventBus<DeleteSchedule> cacheDeleteSchedule,
        IServiceScopeFactory scopeFactory)
    {
        _cacheDeleteSchedule = cacheDeleteSchedule;
        _scopeFactory = scopeFactory;
    }

    public void DeleteScheldure(DeleteSchedule deleteSchedule)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var backupService = scope.ServiceProvider.GetService<BackupService>();

            backupService.DeleteSchedule(deleteSchedule.TenantId);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cacheDeleteSchedule.Subscribe((n) => DeleteScheldure(n), Common.Caching.EventType.Insert);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cacheDeleteSchedule.Unsubscribe(Common.Caching.EventType.Insert);

        return Task.CompletedTask;
    }
}
