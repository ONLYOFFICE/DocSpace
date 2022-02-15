namespace ASC.Core.Common.EF.Context;

public class MySqlMailDbContext : MailDbContext { }
public class PostgreSqlMailDbContext : MailDbContext { }
public class MailDbContext : BaseDbContext
{
    public DbSet<MailboxServer> MailboxServer { get; set; }
    public DbSet<ServerServer> ServerServer { get; set; }
    public DbSet<MailboxProvider> MailboxProvider { get; set; }
    public DbSet<Mailbox> Mailbox { get; set; }
    public DbSet<ApiKeys> ApiKeys { get; set; }
    public DbSet<GreyListingWhiteList> GreyListingWhiteList { get; set; }

    public MailDbContext() { }
    public MailDbContext(DbContextOptions options) : base(options) { }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlMailDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlMailDbContext() } ,
            };
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddMailbox()
            .AddMailboxProvider()
            .AddServerServer()
            .AddGreyListingWhiteList();
    }
}
public static class MailDbExtension
{
    public static DIHelper AddMailDbContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<MailDbContext>();
    }
}
