namespace ASC.Core.Common.EF;

public class BaseDbContextManager<T> : OptionsManager<T>, IDisposable where T : class, IDisposable, IAsyncDisposable, new()
{
    private Dictionary<string, T> _pairs;
    private MigrationHistory _migrationHistory;
    private List<T> _asyncList;
    private IOptionsFactory<T> _factory;
    private IConfiguration _configuration;

    public BaseDbContextManager(IOptionsFactory<T> factory, IConfiguration configuration,
        MigrationHistory migrationHistory) : base(factory)
    {
        _pairs = new Dictionary<string, T>();
        _asyncList = new List<T>();
        _factory = factory;
        _configuration = configuration;
        _migrationHistory = migrationHistory;
    }

    public override T Get(string name)
    {
        if (!_pairs.ContainsKey(name))
        {
            var t = base.Get(name);
            _pairs.Add(name, t);

            if (t is BaseDbContext dbContext)
            {
                if (_configuration["migration:enabled"] == "true"
                    && _migrationHistory.TryAddMigratedContext(t.GetType()))
                {
                    dbContext.Migrate();
                }
            }
        }

        return _pairs[name];
    }

    public T GetNew(string name = "default")
    {
        var result = _factory.Create(name);

        _asyncList.Add(result);

        return result;
    }

    public void Dispose()
    {
        foreach (var v in _pairs)
        {
            v.Value.Dispose();
        }

        foreach (var v in _asyncList)
        {
            v.Dispose();
        }
    }
}

[Scope(typeof(ConfigureDbContext<>))]
public class DbContextManager<T> : BaseDbContextManager<T> where T : BaseDbContext, new()
{
    public DbContextManager(IOptionsFactory<T> factory, IConfiguration configuration,
        MigrationHistory migrationHistory) : base(factory, configuration, migrationHistory)
    {
    }
}

public class MultiRegionalDbContextManager<T> : BaseDbContextManager<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
{
    public MultiRegionalDbContextManager(IOptionsFactory<MultiRegionalDbContext<T>> factory, IConfiguration configuration,
        MigrationHistory migrationHistory) : base(factory, configuration, migrationHistory)
    {
    }
}

public static class DbContextManagerExtension
{
    public static DIHelper AddDbContextManagerService<T>(this DIHelper services) where T : BaseDbContext, new()
    {
        //TODO
        //services.TryAddScoped<MultiRegionalDbContextManager<T>>();
        //services.TryAddScoped<IConfigureOptions<MultiRegionalDbContext<T>>, ConfigureMultiRegionalDbContext<T>>();
        return services;
    }
}
