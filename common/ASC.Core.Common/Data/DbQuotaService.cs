using AutoMapper.QueryableExtensions;

namespace ASC.Core.Data;

[Scope]
class ConfigureDbQuotaService : IConfigureNamedOptions<DbQuotaService>
{
    private readonly DbContextManager<CoreDbContext> _dbContextManager;
    public string DbId { get; set; }

    public ConfigureDbQuotaService(DbContextManager<CoreDbContext> dbContextManager)
    {
        _dbContextManager = dbContextManager;
    }

    public void Configure(string name, DbQuotaService options)
    {
        options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => _dbContextManager.Get(name));
    }

    public void Configure(DbQuotaService options)
    {
        options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => _dbContextManager.Value);
    }
}

[Scope]
class DbQuotaService : IQuotaService
{
    internal CoreDbContext CoreDbContext => LazyCoreDbContext.Value;
    internal Lazy<CoreDbContext> LazyCoreDbContext;
    private readonly IMapper _mapper;

    public DbQuotaService(DbContextManager<CoreDbContext> dbContextManager, IMapper mapper)
    {
        LazyCoreDbContext = new Lazy<CoreDbContext>(() => dbContextManager.Value);
        _mapper = mapper;
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        return CoreDbContext.Quotas
            .ProjectTo<TenantQuota>(_mapper.ConfigurationProvider)
            .ToList();
    }

    public TenantQuota GetTenantQuota(int id)
    {
        return CoreDbContext.Quotas
            .Where(r => r.Tenant == id)
            .ProjectTo<TenantQuota>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        ArgumentNullException.ThrowIfNull(quota);

        CoreDbContext.AddOrUpdate(r => r.Quotas, _mapper.Map<TenantQuota, DbQuota>(quota));
        CoreDbContext.SaveChanges();

        return quota;
    }

    public void RemoveTenantQuota(int id)
    {
        using var tr = CoreDbContext.Database.BeginTransaction();
        var d = CoreDbContext.Quotas
             .Where(r => r.Tenant == id)
             .SingleOrDefault();

        if (d != null)
        {
            CoreDbContext.Quotas.Remove(d);
            CoreDbContext.SaveChanges();
        }

        tr.Commit();
    }


    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        ArgumentNullException.ThrowIfNull(row);

        using var tx = CoreDbContext.Database.BeginTransaction();

        var counter = CoreDbContext.QuotaRows
            .Where(r => r.Path == row.Path && r.Tenant == row.Tenant)
            .Select(r => r.Counter)
            .Take(1)
            .FirstOrDefault();

        var dbQuotaRow = _mapper.Map<TenantQuotaRow, DbQuotaRow>(row);
        dbQuotaRow.Counter = exchange ? counter + row.Counter : row.Counter;
        dbQuotaRow.LastModified = DateTime.UtcNow;

        CoreDbContext.AddOrUpdate(r => r.QuotaRows, dbQuotaRow);
        CoreDbContext.SaveChanges();

        tx.Commit();
    }

    public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        IQueryable<DbQuotaRow> q = CoreDbContext.QuotaRows;

        if (tenantId != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenantId);
        }

        return q.ProjectTo<TenantQuotaRow>(_mapper.ConfigurationProvider).ToList();
    }
}
