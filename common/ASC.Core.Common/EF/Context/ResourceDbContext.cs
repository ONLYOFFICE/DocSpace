namespace ASC.Core.Common.EF.Context;

public class MySqlResourceDbContext : ResourceDbContext { }
public class PostgreSqlResourceDbContext : ResourceDbContext { }
public class ResourceDbContext : BaseDbContext
{
    public DbSet<ResAuthors> Authors { get; set; }
    public DbSet<ResAuthorsFile> ResAuthorsFiles { get; set; }
    public DbSet<ResAuthorsLang> ResAuthorsLang { get; set; }
    public DbSet<ResCultures> ResCultures { get; set; }
    public DbSet<ResData> ResData { get; set; }
    public DbSet<ResFiles> ResFiles { get; set; }
    public DbSet<ResReserve> ResReserve { get; set; }
    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlResourceDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlResourceDbContext() } ,
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddResAuthorsLang()
            .AddResAuthorsFile()
            .AddResCultures()
            .AddResFiles()
            .AddResData()
            .AddResAuthors()
            .AddResReserve();
    }
}

public static class ResourceDbExtension
{
    public static DIHelper AddResourceDbService(this DIHelper services)
    {
        return services.AddDbContextManagerService<ResourceDbContext>();
    }
}
