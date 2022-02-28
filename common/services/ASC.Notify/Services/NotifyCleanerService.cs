namespace ASC.Notify.Services;

[Singletone]
public class NotifyCleanerService : BackgroundService
{
    private readonly ILog _logger;
    private readonly NotifyServiceCfg _notifyServiceCfg;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _waitingPeriod = TimeSpan.FromHours(8);

    public NotifyCleanerService(IOptions<NotifyServiceCfg> notifyServiceCfg, IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<ILog> options)
    {
        _logger = options.Get("ASC.NotifyCleaner");
        _notifyServiceCfg = notifyServiceCfg.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Info("Notify Cleaner Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            Clear();

            await Task.Delay(_waitingPeriod, stoppingToken);
        }

        _logger.Info("Notify Cleaner Service is stopping.");
    }

    private void Clear()
    {
        try
        {
            var date = DateTime.UtcNow.AddDays(-_notifyServiceCfg.StoreMessagesDays);

            using var scope = _serviceScopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(_notifyServiceCfg.ConnectionStringName);
            using var tx = dbContext.Database.BeginTransaction();

            var info = dbContext.NotifyInfo.Where(r => r.ModifyDate < date && r.State == 4).ToList();
            var queue = dbContext.NotifyQueue.Where(r => r.CreationDate < date).ToList();
            dbContext.NotifyInfo.RemoveRange(info);
            dbContext.NotifyQueue.RemoveRange(queue);

            dbContext.SaveChanges();
            tx.Commit();

            _logger.InfoFormat("Clear notify messages: notify_info({0}), notify_queue ({1})", info.Count, queue.Count);

        }
        catch (ThreadAbortException)
        {
            // ignore
        }
        catch (Exception err)
        {
            _logger.Error(err);
        }
    }
}
