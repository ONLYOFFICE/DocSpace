namespace ASC.ActiveDirectory.Base;
public class ActiveDirectoryDbContext : BaseDbContext
{
    public DbSet<DbTenant> Tenants { get; set; }
    public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddDbTenant()
            .AddWebstudioSettings()
            .AddDbFunction();
    }

}

public static class ActiveDirectoryDbContextExtention
{
    public static DIHelper AddCRMDbContextService(this DIHelper services)
    {
        return services.AddDbContextManagerService<ActiveDirectoryDbContext>();
    }
}
