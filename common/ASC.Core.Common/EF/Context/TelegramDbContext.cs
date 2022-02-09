namespace ASC.Core.Common.EF.Context;

public class MySqlTelegramDbContext : TelegramDbContext { }
public class PostgreSqlTelegramDbContext : TelegramDbContext { }
public class TelegramDbContext : BaseDbContext
{
    public DbSet<TelegramUser> Users { get; set; }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext =>
       new Dictionary<Provider, Func<BaseDbContext>>()
       {
                { Provider.MySql, () => new MySqlTelegramDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlTelegramDbContext() } ,
       };

    public TelegramDbContext() { }

    public TelegramDbContext(DbContextOptions<TelegramDbContext> options)
        : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddTelegramUsers();
    }
}

public static class TelegramDbContextExtension
{
    public static DIHelper AddTelegramDbContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<TelegramDbContext>();
    }
}
