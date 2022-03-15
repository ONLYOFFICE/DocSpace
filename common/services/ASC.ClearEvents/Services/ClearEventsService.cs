namespace ASC.ClearEvents.Services;

[Scope]
public class ClearEventsService : IHostedService, IDisposable
{
    private readonly ILog _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer _timer;

    public ClearEventsService(IOptionsMonitor<ILog> options, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = options.CurrentValue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Timer Clear Events Service running.");

        _timer = new Timer(DeleteOldEvents, null, TimeSpan.Zero,
            TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Timed Clear Events Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_timer == null)
        {
            return;
        }

        var handle = new AutoResetEvent(false);

        if (!_timer.Dispose(handle))
        {
            throw new Exception("Timer already disposed");
        }

        handle.WaitOne();
    }

    private void DeleteOldEvents(object state)
    {
        try
        {
            GetOldEvents(r => r.LoginEvents, "LoginHistoryLifeTime");
            GetOldEvents(r => r.AuditEvents, "AuditTrailLifeTime");
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }
    }

    private void GetOldEvents<T>(Expression<Func<MessagesContext, DbSet<T>>> func, string settings) where T : MessageEvent
    {
        List<T> ids;
        var compile = func.Compile();
        do
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var ef = scope.ServiceProvider.GetService<DbContextManager<MessagesContext>>().Get("messages");
            var table = compile.Invoke(ef);

            var ae = table
                .Join(ef.Tenants, r => r.TenantId, r => r.Id, (audit, tenant) => audit)
                .Select(r => new
                {
                    r.Id,
                    r.Date,
                    r.TenantId,
                    ef = r
                })
                .Where(r => r.Date < DateTime.UtcNow.AddDays(-Convert.ToDouble(
                    ef.WebstudioSettings
                    .Where(a => a.TenantId == r.TenantId && a.Id == TenantAuditSettings.Guid)
                    .Select(r => JsonExtensions.JsonValue(nameof(r.Data).ToLower(), settings))
                    .FirstOrDefault() ?? TenantAuditSettings.MaxLifeTime.ToString())))
                .Take(1000);

            ids = ae.Select(r => r.ef).ToList();

            if (!ids.Any())
            {
                return;
            }

            table.RemoveRange(ids);
            ef.SaveChanges();

        } while (ids.Any());
    }
}