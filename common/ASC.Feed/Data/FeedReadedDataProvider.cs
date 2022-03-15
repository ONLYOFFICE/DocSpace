namespace ASC.Feed.Data;

public class FeedReadedDataProvider
{
    private FeedDbContext FeedDbContext => _lazyFeedDbContext.Value;

    private const string _dbId = Constants.FeedDbId;

    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly Lazy<FeedDbContext> _lazyFeedDbContext;

    public FeedReadedDataProvider(AuthContext authContext, TenantManager tenantManager, DbContextManager<FeedDbContext> dbContextManager)
    {
        _authContext = authContext;
        _tenantManager = tenantManager;
        _lazyFeedDbContext = new Lazy<FeedDbContext>(() => dbContextManager.Get(_dbId));
    }

    public DateTime GetTimeReaded()
    {
        return GetTimeReaded(GetUser(), "all", GetTenant());
    }

    public DateTime GetTimeReaded(string module)
    {
        return GetTimeReaded(GetUser(), module, GetTenant());
    }

    public DateTime GetTimeReaded(Guid user, string module, int tenant)
    {
        return FeedDbContext.FeedReaded
            .Where(r => r.Tenant == tenant)
            .Where(r => r.UserId == user)
            .Where(r => r.Module == module)
            .Max(r => r.TimeStamp);
    }

    public void SetTimeReaded()
    {
        SetTimeReaded(GetUser(), DateTime.UtcNow, "all", GetTenant());
    }

    public void SetTimeReaded(string module)
    {
        SetTimeReaded(GetUser(), DateTime.UtcNow, module, GetTenant());
    }

    public void SetTimeReaded(Guid user)
    {
        SetTimeReaded(user, DateTime.UtcNow, "all", GetTenant());
    }

    public void SetTimeReaded(Guid user, DateTime time, string module, int tenant)
    {
        if (string.IsNullOrEmpty(module))
        {
            return;
        }

        var feedReaded = new FeedReaded
        {
            UserId = user,
            TimeStamp = time,
            Module = module,
            Tenant = tenant
        };

        FeedDbContext.AddOrUpdate(r => r.FeedReaded, feedReaded);
        FeedDbContext.SaveChanges();
    }

    public IEnumerable<string> GetReadedModules(DateTime fromTime)
    {
        return GetReadedModules(GetUser(), GetTenant(), fromTime);
    }

    public IEnumerable<string> GetReadedModules(Guid user, int tenant, DateTime fromTime)
    {
        return FeedDbContext.FeedReaded
            .Where(r => r.Tenant == tenant)
            .Where(r => r.UserId == user)
            .Where(r => r.TimeStamp >= fromTime)
            .Select(r => r.Module)
            .ToList();
    }

    private int GetTenant()
    {
        return _tenantManager.GetCurrentTenant().Id;
    }

    private Guid GetUser()
    {
        return _authContext.CurrentAccount.ID;
    }
}
