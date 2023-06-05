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

namespace ASC.Feed.Data;

[Scope]
public class FeedReadedDataProvider
{
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly IDbContextFactory<FeedDbContext> _dbContextFactory;

    public FeedReadedDataProvider(AuthContext authContext, TenantManager tenantManager, IDbContextFactory<FeedDbContext> dbContextFactory)
    {
        _authContext = authContext;
        _tenantManager = tenantManager;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<DateTime> GetTimeReadedAsync()
    {
        return await GetTimeReadedAsync(GetUser(), "all", await GetTenantAsync());
    }

    public async Task<DateTime> GetTimeReadedAsync(string module)
    {
        return await GetTimeReadedAsync(GetUser(), module, await GetTenantAsync());
    }

    public async Task<DateTime> GetTimeReadedAsync(Guid user, string module, int tenant)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        return await feedDbContext.FeedReaded
            .Where(r => r.Tenant == tenant)
            .Where(r => r.UserId == user)
            .Where(r => r.Module == module)
            .MaxAsync(r => r.TimeStamp);
    }

    public async Task SetTimeReadedAsync()
    {
        await SetTimeReadedAsync(GetUser(), DateTime.UtcNow, "all", await GetTenantAsync());
    }

    public async Task SetTimeReadedAsync(string module)
    {
        await SetTimeReadedAsync(GetUser(), DateTime.UtcNow, module, await GetTenantAsync());
    }

    public async Task SetTimeReadedAsync(Guid user)
    {
        await SetTimeReadedAsync(user, DateTime.UtcNow, "all", await GetTenantAsync());
    }

    public async Task SetTimeReadedAsync(Guid user, DateTime time, string module, int tenant)
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

        using var feedDbContext = _dbContextFactory.CreateDbContext();
        await feedDbContext.AddOrUpdateAsync(q => q.FeedReaded, feedReaded);
        await feedDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetReadedModulesAsync(DateTime fromTime)
    {
        return await GetReadedModulesAsync(GetUser(), await GetTenantAsync(), fromTime);
    }

    public async Task<IEnumerable<string>> GetReadedModulesAsync(Guid user, int tenant, DateTime fromTime)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        return await feedDbContext.FeedReaded
            .Where(r => r.Tenant == tenant)
            .Where(r => r.UserId == user)
            .Where(r => r.TimeStamp >= fromTime)
            .Select(r => r.Module)
            .ToListAsync();
    }

    private async Task<int> GetTenantAsync()
    {
        return await _tenantManager.GetCurrentTenantIdAsync();
    }

    private Guid GetUser()
    {
        return _authContext.CurrentAccount.ID;
    }
}
