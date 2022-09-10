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
class DbQuotaService : IQuotaService
{
    private readonly IDbContextFactory<CoreDbContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public DbQuotaService(IDbContextFactory<CoreDbContext> dbContextManager, IMapper mapper)
    {
        _dbContextFactory = dbContextManager;
        _mapper = mapper;
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        return coreDbContext.Quotas
            .ProjectTo<TenantQuota>(_mapper.ConfigurationProvider)
            .ToList();
    }

    public TenantQuota GetTenantQuota(int id)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        return coreDbContext.Quotas
            .Where(r => r.Tenant == id)
            .ProjectTo<TenantQuota>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        ArgumentNullException.ThrowIfNull(quota);

        using var coreDbContext = _dbContextFactory.CreateDbContext();
        coreDbContext.AddOrUpdate(r => r.Quotas, _mapper.Map<TenantQuota, DbQuota>(quota));
        coreDbContext.SaveChanges();

        return quota;
    }

    public void RemoveTenantQuota(int id)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        var strategy = coreDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var coreDbContext = _dbContextFactory.CreateDbContext();
            using var tr = coreDbContext.Database.BeginTransaction();
            var d = coreDbContext.Quotas
                 .Where(r => r.Tenant == id)
                 .SingleOrDefault();

            if (d != null)
            {
                coreDbContext.Quotas.Remove(d);
                coreDbContext.SaveChanges();
            }

            tr.Commit();
        });
    }


    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        ArgumentNullException.ThrowIfNull(row);

        using var coreDbContext = _dbContextFactory.CreateDbContext();
        var strategy = coreDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var coreDbContext = _dbContextFactory.CreateDbContext();
            using var tx = coreDbContext.Database.BeginTransaction();

            var counter = coreDbContext.QuotaRows
                .Where(r => r.Path == row.Path && r.Tenant == row.Tenant)
                .Select(r => r.Counter)
                .Take(1)
                .FirstOrDefault();

            var dbQuotaRow = _mapper.Map<TenantQuotaRow, DbQuotaRow>(row);
            dbQuotaRow.Counter = exchange ? counter + row.Counter : row.Counter;
            dbQuotaRow.LastModified = DateTime.UtcNow;

            coreDbContext.AddOrUpdate(r => r.QuotaRows, dbQuotaRow);
            coreDbContext.SaveChanges();

            tx.Commit();
        });
    }

    public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        IQueryable<DbQuotaRow> q = coreDbContext.QuotaRows;

        if (tenantId != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenantId);
        }

        return q.ProjectTo<TenantQuotaRow>(_mapper.ConfigurationProvider).ToList();
    }

    public void SetUserQuotaRow(UserQuotaRow row, bool exchange)
    {
        ArgumentNullException.ThrowIfNull(row);

        using var coreDbContext = _dbContextFactory.CreateDbContext();
        var strategy = coreDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var coreDbContext = _dbContextFactory.CreateDbContext();
            using var tx = coreDbContext.Database.BeginTransaction();

            var counter = coreDbContext.UserQuotaRows
                .Where(r => r.Path == row.Path && r.Tenant == row.Tenant && r.UserId == row.UserId)
                .Select(r => r.Counter)
                .Take(1)
                .FirstOrDefault();

            var dbUserQuotaRow = _mapper.Map<UserQuotaRow, DbUsersQuotaRow>(row);
            dbUserQuotaRow.Counter = exchange ? counter + row.Counter : row.Counter;

            coreDbContext.AddOrUpdate(r => r.UserQuotaRows, dbUserQuotaRow);
            coreDbContext.SaveChanges();

            tx.Commit();
        });
    }

    public IEnumerable<UserQuotaRow> FindUserQuotaRows(int tenantId, Guid userId)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        IQueryable<DbUsersQuotaRow> q = coreDbContext.UserQuotaRows;

        if (tenantId != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenantId && r.UserId == userId);
        }

        return q.ProjectTo<UserQuotaRow>(_mapper.ConfigurationProvider).ToList();
    }
}
