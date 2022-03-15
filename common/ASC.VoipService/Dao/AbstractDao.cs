namespace ASC.VoipService.Dao;

public class AbstractDao
{
    private readonly string _dbid = "default";
    private readonly Lazy<VoipDbContext> _lazyVoipDbContext;

    protected VoipDbContext VoipDbContext { get => _lazyVoipDbContext.Value; }
    protected int TenantID
    {
        get;
        private set;
    }

    protected AbstractDao(DbContextManager<VoipDbContext> dbOptions, TenantManager tenantManager)
    {
        _lazyVoipDbContext = new Lazy<VoipDbContext>(() => dbOptions.Get(_dbid));
        TenantID = tenantManager.GetCurrentTenant().Id;
    }

    protected string GetTenantColumnName(string table)
    {
        const string tenant = "tenant_id";
        if (!table.Contains(' ')) return tenant;
        return table.Substring(table.IndexOf(" ", StringComparison.Ordinal)).Trim() + "." + tenant;
    }


    protected static Guid ToGuid(object guid)
    {
        var str = guid as string;
        return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
    }
}
