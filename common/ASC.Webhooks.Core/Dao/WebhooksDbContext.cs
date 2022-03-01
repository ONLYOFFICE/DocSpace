namespace ASC.Webhooks.Core.Dao;
public class MySqlWebhooksDbContext : WebhooksDbContext { }
public class PostgreSqlWebhooksDbContext : WebhooksDbContext { }
public partial class WebhooksDbContext : BaseDbContext
{
    public WebhooksDbContext() { }

    public WebhooksDbContext(DbContextOptions<WebhooksDbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<WebhooksConfig> WebhooksConfigs { get; set; }
    public virtual DbSet<WebhooksLog> WebhooksLogs { get; set; }
    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlWebhooksDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlWebhooksDbContext() } ,
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
        .From(modelBuilder, Provider)
        .AddWebhooksConfig()
        .AddWebhooksLog();
    }
}

public static class WebhooksDbExtension
{
    public static DIHelper AddWebhooksDbContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<TenantDbContext>();
    }
}
