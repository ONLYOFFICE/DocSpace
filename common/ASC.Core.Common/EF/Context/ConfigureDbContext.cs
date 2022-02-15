namespace ASC.Core.Common.EF;

[Scope]
public class ConfigureDbContext<T> : IConfigureNamedOptions<T> where T : BaseDbContext, new()
{
    public const string BaseName = "default";
    private readonly EFLoggerFactory _loggerFactory;
    private readonly ConfigurationExtension _configuration;
    private readonly string _migrateAssembly;

    public ConfigureDbContext(EFLoggerFactory loggerFactory, ConfigurationExtension configuration)
    {
        _loggerFactory = loggerFactory;
        _configuration = configuration;
        _migrateAssembly = _configuration["testAssembly"];
    }

    public void Configure(string name, T context)
    {
        context.LoggerFactory = _loggerFactory;
        context.ConnectionStringSettings = _configuration.GetConnectionStrings(name) ?? _configuration.GetConnectionStrings(BaseName);
        context.MigrateAssembly = _migrateAssembly;
    }

    public void Configure(T context)
    {
        Configure(BaseName, context);
    }
}

public class ConfigureMultiRegionalDbContext<T> : IConfigureNamedOptions<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
{
    private readonly string _baseName = "default";
    private readonly ConfigurationExtension _configuration;
    private readonly DbContextManager<T> _dbContext;

    public ConfigureMultiRegionalDbContext(ConfigurationExtension configuration, DbContextManager<T> dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public void Configure(string name, MultiRegionalDbContext<T> context)
    {
        context.Context = new List<T>();

        const StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

        foreach (var c in _configuration.GetConnectionStrings().Where(r =>
        r.Name.Equals(name, cmp) || r.Name.StartsWith(name + ".", cmp) ||
        r.Name.Equals(_baseName, cmp) || r.Name.StartsWith(_baseName + ".", cmp)
        ))
        {
            context.Context.Add(_dbContext.Get(c.Name));
        }
    }

    public void Configure(MultiRegionalDbContext<T> context)
    {
        Configure(_baseName, context);
    }
}
