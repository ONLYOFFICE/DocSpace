namespace ASC.IPSecurity;

[Scope]
public class IPRestrictionsRepository
{
    private TenantDbContext TenantDbContext => _lazyTenantDbContext.Value;

    private const string DbId = "core";

    private readonly Lazy<TenantDbContext> _lazyTenantDbContext;
    private readonly IMapper _mapper;

    public IPRestrictionsRepository(DbContextManager<TenantDbContext> dbContextManager, IMapper mapper)
    {
        _lazyTenantDbContext = new Lazy<TenantDbContext>(() => dbContextManager.Get(DbId));
        _mapper = mapper;
    }

    public List<IPRestriction> Get(int tenant)
    {
        return TenantDbContext.TenantIpRestrictions
            .Where(r => r.Tenant == tenant)
            .ProjectTo<IPRestriction>(_mapper.ConfigurationProvider)
            .ToList();
    }

    public List<string> Save(IEnumerable<string> ips, int tenant)
    {
        using var tx = TenantDbContext.Database.BeginTransaction();

        var restrictions = TenantDbContext.TenantIpRestrictions.Where(r => r.Tenant == tenant).ToList();
        TenantDbContext.TenantIpRestrictions.RemoveRange(restrictions);

        var ipsList = ips.Select(r => new TenantIpRestrictions
        {
            Tenant = tenant,
            Ip = r
        });

        TenantDbContext.TenantIpRestrictions.AddRange(ipsList);

        tx.Commit();

        return ips.ToList();
    }
}
