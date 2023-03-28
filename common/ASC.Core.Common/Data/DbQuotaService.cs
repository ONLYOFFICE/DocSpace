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

    public async Task<IEnumerable<TenantQuota>> GetTenantQuotasAsync()
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        return await coreDbContext.Quotas
            .ProjectTo<TenantQuota>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TenantQuota> GetTenantQuotaAsync(int id)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        return await coreDbContext.Quotas
            .Where(r => r.Tenant == id)
            .ProjectTo<TenantQuota>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<TenantQuota> SaveTenantQuotaAsync(TenantQuota quota)
    {
        ArgumentNullException.ThrowIfNull(quota);

        using var coreDbContext = _dbContextFactory.CreateDbContext();
        await coreDbContext.AddOrUpdateAsync(q => q.Quotas, _mapper.Map<TenantQuota, DbQuota>(quota));
        await coreDbContext.SaveChangesAsync();

        return quota;
    }

    public async Task RemoveTenantQuotaAsync(int id)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        await coreDbContext.Quotas.Where(r => r.Tenant == id).ExecuteDeleteAsync();
    }


    public async Task SetTenantQuotaRowAsync(TenantQuotaRow row, bool exchange)
    {
        ArgumentNullException.ThrowIfNull(row);

        using var coreDbContext = _dbContextFactory.CreateDbContext();

        await AddQuotaAsync(coreDbContext, row.UserId);

        async Task AddQuotaAsync(CoreDbContext coreDbContext, Guid userId)
        {
            var dbTenantQuotaRow = _mapper.Map<TenantQuotaRow, DbQuotaRow>(row);
            dbTenantQuotaRow.UserId = userId;

            if (exchange)
            {
                var counter = await coreDbContext.QuotaRows
                .Where(r => r.Path == row.Path && r.Tenant == row.Tenant && r.UserId == userId)
                .Select(r => r.Counter)
                .Take(1)
                .FirstOrDefaultAsync();

                dbTenantQuotaRow.Counter = counter + row.Counter;
            }
            
            await coreDbContext.AddOrUpdateAsync(q => q.QuotaRows, dbTenantQuotaRow);
            await coreDbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TenantQuotaRow>> FindTenantQuotaRowsAsync(int tenantId)
    {
        return await FindUserQuotaRowsAsync(tenantId, Guid.Empty);
    }

    public async Task<IEnumerable<TenantQuotaRow>> FindUserQuotaRowsAsync(int tenantId, Guid userId)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        var q = coreDbContext.QuotaRows.Where(r => r.UserId == userId);

        if (tenantId != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenantId);
        }

        return await q.ProjectTo<TenantQuotaRow>(_mapper.ConfigurationProvider).ToListAsync();
    }
}
