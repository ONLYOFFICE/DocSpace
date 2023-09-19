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

namespace ASC.IPSecurity;

[Scope]
public class IPRestrictionsRepository
{
    private readonly IDbContextFactory<TenantDbContext> _dbContextManager;
    private readonly IMapper _mapper;

    public IPRestrictionsRepository(IDbContextFactory<TenantDbContext> dbContextManager, IMapper mapper)
    {
        _dbContextManager = dbContextManager;
        _mapper = mapper;
    }

    public async Task<List<IPRestriction>> GetAsync(int tenant)
    {
        await using var tenantDbContext = _dbContextManager.CreateDbContext();
        return await tenantDbContext.TenantIpRestrictions
            .Where(r => r.TenantId == tenant)
            .ProjectTo<IPRestriction>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<IpRestrictionBase>> SaveAsync(IEnumerable<IpRestrictionBase> ips, int tenant)
    {
        await using var tenantDbContext = _dbContextManager.CreateDbContext();
        var strategy = tenantDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextManager.CreateDbContext();
            using var tr = await tenantDbContext.Database.BeginTransactionAsync();

            await Queries.DeleteTenantIpRestrictionsAsync(tenantDbContext, tenant);

            var ipsList = ips.Select(r => new TenantIpRestrictions
            {
                TenantId = tenant,
                Ip = r.Ip,
                ForAdmin = r.ForAdmin
            });
            tenantDbContext.TenantIpRestrictions.AddRange(ipsList);

            await tenantDbContext.SaveChangesAsync();
            await tr.CommitAsync();
        });
        return ips.ToList();
    }
}

static file class Queries
{
    public static readonly Func<TenantDbContext, int, Task<int>>
        DeleteTenantIpRestrictionsAsync = EF.CompileAsyncQuery(
            (TenantDbContext ctx, int tenantId) =>
                ctx.TenantIpRestrictions.Where(r => r.TenantId == tenantId)
                        .ExecuteDelete());
}