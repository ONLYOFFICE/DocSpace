namespace ASC.Core.Common.EF.Context;

public class MySqlVoipDbContext : VoipDbContext { }
public class PostgreSqlVoipDbContext : VoipDbContext { }
public class VoipDbContext : BaseDbContext
{
    public DbSet<VoipNumber> VoipNumbers { get; set; }
    public DbSet<DbVoipCall> VoipCalls { get; set; }
    public DbSet<CrmContact> CrmContact { get; set; }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlVoipDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlVoipDbContext() } ,
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
           .From(modelBuilder, Provider)
           .AddVoipNumber()
           .AddDbVoipCall()
           .AddCrmContact();
    }
}

public static class VoipDbExtension
{
    public static DIHelper AddVoipDbContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<VoipDbContext>();
    }
}
