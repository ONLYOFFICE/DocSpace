namespace ASC.Core.Common.EF.Context;

public class MySqlNotifyDbContext : NotifyDbContext { }
public class PostgreSqlNotifyDbContext : NotifyDbContext { }
public class NotifyDbContext : BaseDbContext
{
    public DbSet<NotifyInfo> NotifyInfo { get; set; }
    public DbSet<NotifyQueue> NotifyQueue { get; set; }
    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlNotifyDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlNotifyDbContext() } ,
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddNotifyInfo()
            .AddNotifyQueue();
    }
}

public static class NotifyDbExtension
{
    public static DIHelper AddNotifyDbContext(this DIHelper services)
    {
        return services.AddDbContextManagerService<NotifyDbContext>();
    }
}
