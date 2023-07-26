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

using Microsoft.EntityFrameworkCore;

namespace ASC.ApiCache.Helpers;

[Scope]
public class PortalControllerHelper
{
    private readonly ILogger<PortalControllerHelper> _log;
    private readonly TenantDomainValidator _tenantDomainValidator;
    private readonly IDbContextFactory<TeamlabSiteContext> _teamlabSiteContext;

    public PortalControllerHelper(ILogger<PortalControllerHelper> log, TenantDomainValidator tenantDomainValidator, IDbContextFactory<TeamlabSiteContext> teamlabSiteContext)
    {
        _log = log;
        _tenantDomainValidator = tenantDomainValidator;
        _teamlabSiteContext = teamlabSiteContext;
    }

    public async Task AddTenantToCacheAsync(string tenant)
    {
        var cache = new DbCache()
        {
            TenantAlias = tenant.ToLowerInvariant()
        };

        await using var context = _teamlabSiteContext.CreateDbContext();
        await context.Cache.AddAsync(cache);
        await context.SaveChangesAsync();
    }

    public async Task RemoveTenantFromCacheAsync(string domain)
    {
        domain = domain.ToLowerInvariant();
        await using var context = _teamlabSiteContext.CreateDbContext();
        var cache = await Queries.DbCacheAsync(context, domain);
        context.Cache.Remove(cache);
        await context.SaveChangesAsync();
    }

    public async Task<List<string>> FindTenantsInCacheAsync(string portalName)
    {
        //return tenants starts like current
        _log.LogDebug("FindTenantsInCache method");

        portalName = (portalName ?? "").Trim().ToLowerInvariant();

        // forbidden or exists
        await using var context = _teamlabSiteContext.CreateDbContext();
        var exists = await Queries.AnyDbCacheAsync(context, portalName);

        if (exists)
        {
            // cut number suffix
            while (true)
            {
                if (_tenantDomainValidator.MinLength < portalName.Length && char.IsNumber(portalName, portalName.Length - 1))
                {
                    portalName = portalName[0..^1];
                }
                else
                {
                    break;
                }
            }

            return await Queries.TenantAliasesAsync(context, portalName).ToListAsync();
        }
        return null;
    }
}

static file class Queries
{
    public static readonly Func<TeamlabSiteContext, string, Task<DbCache>> DbCacheAsync =
        EF.CompileAsyncQuery(
            (TeamlabSiteContext ctx, string portalName) =>
                ctx.Cache.SingleOrDefault(q => q.TenantAlias == portalName));

    public static readonly Func<TeamlabSiteContext, string, Task<bool>> AnyDbCacheAsync =
        EF.CompileAsyncQuery(
            (TeamlabSiteContext ctx, string portalName) =>
                ctx.Cache
                    .Any(q => q.TenantAlias.Equals(portalName)));

    public static readonly Func<TeamlabSiteContext, string, IAsyncEnumerable<string>> TenantAliasesAsync =
        EF.CompileAsyncQuery(
            (TeamlabSiteContext ctx, string portalName) =>
                ctx.Cache
                    .Select(q => q.TenantAlias)
                    .Where(q => q.StartsWith(portalName)));
}
