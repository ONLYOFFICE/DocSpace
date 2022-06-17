// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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
        var strategy = CoreDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
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
        });
    }


    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        ArgumentNullException.ThrowIfNull(row);

        var strategy = CoreDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
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
        });
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
