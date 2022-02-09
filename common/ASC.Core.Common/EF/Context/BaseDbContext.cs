﻿using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace ASC.Core.Common.EF;

public enum Provider
{
    PostgreSql,
    MySql
}

public class BaseDbContext : DbContext
{
    public ConnectionStringSettings ConnectionStringSettings { get; set; }
    internal string MigrateAssembly { get; set; }
    internal ILoggerFactory LoggerFactory { get; set; }
    protected internal Provider Provider { get; set; }
    protected virtual Dictionary<Provider, Func<BaseDbContext>> ProviderContext => null;

    public string baseName;
    public static ServerVersion serverVersion = ServerVersion.Parse("8.0.25");

    public BaseDbContext() { }

    public BaseDbContext(DbContextOptions options) : base(options) { }

    public void Migrate()
    {
        if (ProviderContext != null)
        {
            var provider = GetProviderByConnectionString();

            using var sqlProvider = ProviderContext[provider]();
            sqlProvider.ConnectionStringSettings = ConnectionStringSettings;
            sqlProvider.LoggerFactory = LoggerFactory;
            sqlProvider.MigrateAssembly = MigrateAssembly;

            sqlProvider.Database.Migrate();
        }
        else
        {
            Database.Migrate();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(LoggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        Provider = GetProviderByConnectionString();
        switch (Provider)
        {
            case Provider.MySql:
                optionsBuilder.UseMySql(ConnectionStringSettings.ConnectionString, serverVersion, r =>
                {
                    if (!string.IsNullOrEmpty(MigrateAssembly))
                        r = r.MigrationsAssembly(MigrateAssembly);
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
}

public abstract class BaseEntity
{
    public abstract object[] GetKeys();
}

public class MultiRegionalDbContext<T> : IDisposable, IAsyncDisposable where T : BaseDbContext, new()
{
    internal List<T> Context { get; set; }

    public MultiRegionalDbContext() { }

    public void Dispose()
    {
        if (Context == null)
        {
            return;
        }

        foreach (var c in Context)
        {
            if (c != null)
            {
                c.Dispose();
            }
        }
    }
    public async ValueTask DisposeAsync()
    {
        if (Context == null)
        {
            return;
        }

        foreach (var c in Context)
        {
            if (c != null)
            {
                await c.DisposeAsync();
            }
        }
    }
}
