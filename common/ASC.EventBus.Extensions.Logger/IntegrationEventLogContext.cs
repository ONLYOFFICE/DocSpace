namespace ASC.EventBus.Extensions.Logger;

public class MySqlIntegrationEventLogContext : IntegrationEventLogContext { }
public class PostgreSqlIntegrationEventLogContext : IntegrationEventLogContext { }

public class IntegrationEventLogContext : BaseDbContext
{
    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper.From(modelBuilder, Provider)
                           .AddIntegrationEventLog();

    }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlIntegrationEventLogContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlIntegrationEventLogContext() } ,
                };
        }
    }
}


