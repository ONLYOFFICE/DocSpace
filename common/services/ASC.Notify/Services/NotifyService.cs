namespace ASC.Notify.Services;

[Singletone]
public class NotifyService : IHostedService
{
    private readonly DbWorker _db;
    private readonly ICacheNotify<NotifyInvoke> _cacheInvoke;
    private readonly ICacheNotify<NotifyMessage> _cacheNotify;
    private readonly ILog _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly NotifyConfiguration _notifyConfiguration;
    private readonly NotifyServiceCfg _notifyServiceCfg;

    public NotifyService(
        IOptions<NotifyServiceCfg> notifyServiceCfg,
        DbWorker db,
        ICacheNotify<NotifyInvoke> cacheInvoke,
        ICacheNotify<NotifyMessage> cacheNotify,
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory serviceScopeFactory,
        NotifyConfiguration notifyConfiguration)
    {
        _cacheInvoke = cacheInvoke;
        _cacheNotify = cacheNotify;
        _db = db;
        _logger = options.Get("ASC.NotifyService");
        _notifyConfiguration = notifyConfiguration;
        _notifyServiceCfg = notifyServiceCfg.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Notify Service running.");

        _cacheNotify.Subscribe((n) => SendNotifyMessage(n), CacheNotifyAction.InsertOrUpdate);
        _cacheInvoke.Subscribe((n) => InvokeSendMethod(n), CacheNotifyAction.InsertOrUpdate);

        if (0 < _notifyServiceCfg.Schedulers.Count)
        {
            InitializeNotifySchedulers();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Notify Service is stopping.");

        _cacheNotify.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
        _cacheInvoke.Unsubscribe(CacheNotifyAction.InsertOrUpdate);

        return Task.CompletedTask;
    }

    private void SendNotifyMessage(NotifyMessage notifyMessage)
    {
        try
        {
            _db.SaveMessage(notifyMessage);
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }

    private void InvokeSendMethod(NotifyInvoke notifyInvoke)
    {
        var service = notifyInvoke.Service;
        var method = notifyInvoke.Method;
        var tenant = notifyInvoke.Tenant;
        var parameters = notifyInvoke.Parameters;

        var serviceType = Type.GetType(service, true);

        using var scope = _serviceScopeFactory.CreateScope();

        var instance = scope.ServiceProvider.GetService(serviceType);
        if (instance == null)
        {
            throw new Exception("Service instance not found.");
        }

        var methodInfo = serviceType.GetMethod(method);
        if (methodInfo == null)
        {
            throw new Exception("Method not found.");
        }

        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        var tenantWhiteLabelSettingsHelper = scope.ServiceProvider.GetService<TenantWhiteLabelSettingsHelper>();
        var settingsManager = scope.ServiceProvider.GetService<SettingsManager>();

        tenantManager.SetCurrentTenant(tenant);
        tenantWhiteLabelSettingsHelper.Apply(settingsManager.Load<TenantWhiteLabelSettings>(), tenant);
        methodInfo.Invoke(instance, parameters.ToArray());
    }

    private void InitializeNotifySchedulers()
    {
        _notifyConfiguration.Configure();
        foreach (var pair in _notifyServiceCfg.Schedulers.Where(r => r.MethodInfo != null))
        {
            _logger.DebugFormat("Start scheduler {0} ({1})", pair.Name, pair.MethodInfo);
            pair.MethodInfo.Invoke(null, null);
        }
    }
}
