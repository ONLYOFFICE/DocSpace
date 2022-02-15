namespace ASC.Core.Common.EF.Context;

public class MySqlAccountLinkContext : AccountLinkContext { }
public class PostgreSqlAccountLinkContext : AccountLinkContext { }
public class AccountLinkContext : BaseDbContext
{
    public DbSet<AccountLinks> AccountLinks { get; set; }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlAccountLinkContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlAccountLinkContext() } ,
                };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
           .From(modelBuilder, Provider)
           .AddAccountLinks();
    }
}

public static class AccountLinkContextExtension
{
    public static DIHelper AddAccountLinkContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<AccountLinkContext>();
    }
}
