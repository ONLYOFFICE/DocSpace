namespace ASC.Core.Common.EF.Context;

public class MySqlAuditTrailContext : AuditTrailContext { }
public class PostgreSqlAuditTrailContext : AuditTrailContext { }
public class AuditTrailContext : BaseDbContext
{
    public DbSet<AuditEvent> AuditEvents { get; set; }
    public DbSet<User> Users { get; set; }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext =>
        new Dictionary<Provider, Func<BaseDbContext>>()
        {
                    { Provider.MySql, () => new MySqlAuditTrailContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlAuditTrailContext() } ,
        };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
        .From(modelBuilder, Provider)
        .AddAuditEvent()
        .AddUser()
        .AddDbFunction();
    }
}

public static class AuditTrailContextExtension
{
    public static DIHelper AddAuditTrailContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<AuditTrailContext>();
    }
}
