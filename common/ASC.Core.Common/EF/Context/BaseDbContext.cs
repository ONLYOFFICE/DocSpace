using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace ASC.Core.Common.EF;

public enum Provider
{
    PostgreSql,
    MySql
}

public class BaseDbContext : DbContext
{
    public BaseDbContext() { }
    public BaseDbContext(DbContextOptions options) : base(options) { }

    internal string _migrateAssembly;
    internal ILoggerFactory _loggerFactory;
    public ConnectionStringSettings ConnectionStringSettings { get; set; }
    protected internal Provider Provider;

    public static readonly ServerVersion ServerVersion = ServerVersion.Parse("8.0.25");
    protected virtual Dictionary<Provider, Func<BaseDbContext>> ProviderContext => null;

    public void Migrate()
    {
        if (ProviderContext != null)
        {
            var provider = GetProviderByConnectionString();

            using var sqlProvider = ProviderContext[provider]();
            sqlProvider.ConnectionStringSettings = ConnectionStringSettings;
            sqlProvider._loggerFactory = _loggerFactory;
            sqlProvider._migrateAssembly = _migrateAssembly;

            sqlProvider.Database.Migrate();
        }
        else
        {
            Database.Migrate();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_loggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        Provider = GetProviderByConnectionString();
        switch (Provider)
        {
            case Provider.MySql:
                optionsBuilder.UseMySql(ConnectionStringSettings.ConnectionString, ServerVersion, r =>
                {
                    if (!string.IsNullOrEmpty(_migrateAssembly))
                    {
                        r.MigrationsAssembly(_migrateAssembly);
                    }
                });
                break;
            case Provider.PostgreSql:
                optionsBuilder.UseNpgsql(ConnectionStringSettings.ConnectionString);
                break;
        }
    }

    public Provider GetProviderByConnectionString()
    {
        switch (ConnectionStringSettings.ProviderName)
        {
            case "MySql.Data.MySqlClient":
                return Provider.MySql;
            case "Npgsql":
                return Provider.PostgreSql;
            default:
                break;
        }

        return Provider.MySql;
    }
}

public static class BaseDbContextExtension
{
    public static T AddOrUpdate<T, TContext>(this TContext b, Expression<Func<TContext, DbSet<T>>> expressionDbSet, T entity) where T : BaseEntity where TContext : BaseDbContext
    {
        var dbSet = expressionDbSet.Compile().Invoke(b);
        var existingBlog = dbSet.Find(entity.GetKeys());
        if (existingBlog == null)
        {
            return dbSet.Add(entity).Entity;
        }
        else
        {
            b.Entry(existingBlog).CurrentValues.SetValues(entity);

            return entity;
        }
    }

    public static async Task<T> AddOrUpdateAsync<T, TContext>(this TContext b, Expression<Func<TContext, DbSet<T>>> expressionDbSet, T entity) where T : BaseEntity where TContext : BaseDbContext
    {
        var dbSet = expressionDbSet.Compile().Invoke(b);
        var existingBlog = await dbSet.FindAsync(entity.GetKeys());
        if (existingBlog == null)
        {
            var entityEntry = await dbSet.AddAsync(entity);

            return entityEntry.Entity;
        }
        else
        {
            b.Entry(existingBlog).CurrentValues.SetValues(entity);

            return entity;
        }
    }
}

public abstract class BaseEntity
{
    public abstract object[] GetKeys();
}

public class MultiRegionalDbContext<T> : IDisposable, IAsyncDisposable where T : BaseDbContext, new()
{
    public MultiRegionalDbContext() { }

    internal List<T> _context;

    public void Dispose()
    {
        if (_context == null)
        {
            return;
        }

        foreach (var c in _context)
        {
            if (c != null)
            {
                c.Dispose();
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        return _context == null ? ValueTask.CompletedTask : InternalDisposeAsync();
    }

    private async ValueTask InternalDisposeAsync()
    {
        foreach (var c in _context)
        {
            if (c != null)
            {
                await c.DisposeAsync();
            }
        }
    }
}
