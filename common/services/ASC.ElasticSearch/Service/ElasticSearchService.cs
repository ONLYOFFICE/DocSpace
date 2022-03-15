namespace ASC.ElasticSearch.Service;

[Singletone(Additional = typeof(ServiceExtension))]
public class ElasticSearchService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheNotify<ReIndexAction> _cacheNotify;

    public ElasticSearchService(IServiceProvider serviceProvider, ICacheNotify<ReIndexAction> cacheNotify)
    {
        _serviceProvider = serviceProvider;
        _cacheNotify = cacheNotify;
    }

    public void Subscribe()
    {
        _cacheNotify.Subscribe((a) =>
        {
            ReIndex(a.Names.ToList(), a.Tenant);
        }, CacheNotifyAction.Any);
    }

    public bool Support(string table)
    {
        return _serviceProvider.GetService<IEnumerable<IFactoryIndexer>>().Any(r => r.IndexName == table);
    }

    public void ReIndex(List<string> toReIndex, int tenant)
    {
        var allItems = _serviceProvider.GetService<IEnumerable<IFactoryIndexer>>().ToList();
        var tasks = new List<Task>(toReIndex.Count);

        foreach (var item in toReIndex)
        {
            var index = allItems.FirstOrDefault(r => r.IndexName == item);
            if (index == null)
            {
                continue;
            }

            var generic = typeof(BaseIndexer<>);
            var instance = (IIndexer)Activator.CreateInstance(generic.MakeGenericType(index.GetType()), index);
            tasks.Add(instance.ReIndex());
        }

        if (tasks.Count == 0)
        {
            return;
        }

        Task.WhenAll(tasks).ContinueWith(r =>
        {
            using var scope = _serviceProvider.CreateScope();

            var scopeClass = scope.ServiceProvider.GetService<ServiceScope>();
            var (tenantManager, settingsManager) = scopeClass;
            tenantManager.SetCurrentTenant(tenant);
            settingsManager.ClearCache<SearchSettings>();
        });
    }
    //public State GetState()
    //{
    //    return new State
    //    {
    //        Indexing = Launcher.Indexing,
    //        LastIndexed = Launcher.LastIndexed
    //    };
    //}
}

[Scope]
public class ServiceScope
{
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;

    public ServiceScope(TenantManager tenantManager, SettingsManager settingsManager)
    {
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
    }

    public void Deconstruct(out TenantManager tenantManager, out SettingsManager settingsManager)
    {
        tenantManager = _tenantManager;
        settingsManager = _settingsManager;
    }
}

internal static class ServiceExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<ServiceScope>();
    }
}
