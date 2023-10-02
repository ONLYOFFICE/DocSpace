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
public class DbWebPluginService
{
    private readonly IDbContextFactory<WebPluginDbContext> _dbContextFactory;

    public DbWebPluginService(IDbContextFactory<WebPluginDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<DbWebPlugin> SaveAsync(DbWebPlugin webPlugin)
    {
        ArgumentNullException.ThrowIfNull(webPlugin);

        await using var dbContext = _dbContextFactory.CreateDbContext();

        await dbContext.AddOrUpdateAsync(q => q.WebPlugins, webPlugin);
        await dbContext.SaveChangesAsync();

        return webPlugin;
    }

    public async Task<DbWebPlugin> GetByIdAsync(int tenantId, int id)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        return await Queries.WebPluginByIdAsync(dbContext, tenantId, id);
    }

    public async Task<DbWebPlugin> GetByNameAsync(int tenantId, string name)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        return await Queries.WebPluginByNameAsync(dbContext, tenantId, name);
    }

    public async Task<List<DbWebPlugin>> GetAsync(int tenantId)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        return await Queries.WebPluginsAsync(dbContext, tenantId).ToListAsync();
    }

    public async Task UpdateAsync(int tenantId, int id, bool enabled)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        await Queries.UpdateWebPluginStatusAsync(dbContext, tenantId, id, enabled);
    }

    public async Task DeleteAsync(int tenantId, int id)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        await Queries.DeleteWebPluginByIdAsync(dbContext, tenantId, id);
    }
}

static file class Queries
{
    public static readonly Func<WebPluginDbContext, int, IAsyncEnumerable<DbWebPlugin>>
        WebPluginsAsync = EF.CompileAsyncQuery(
            (WebPluginDbContext ctx, int tenantId) =>
                ctx.WebPlugins
                    .AsNoTracking()
                    .Where(r => r.TenantId == tenantId));

    public static readonly Func<WebPluginDbContext, int, int, Task<DbWebPlugin>>
        WebPluginByIdAsync = EF.CompileAsyncQuery(
            (WebPluginDbContext ctx, int tenantId, int id) =>
                ctx.WebPlugins
                    .AsNoTracking()
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .FirstOrDefault());

    public static readonly Func<WebPluginDbContext, int, string, Task<DbWebPlugin>>
        WebPluginByNameAsync = EF.CompileAsyncQuery(
            (WebPluginDbContext ctx, int tenantId, string name) =>
                ctx.WebPlugins
                    .AsNoTracking()
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Name == name)
                    .FirstOrDefault());

    public static readonly Func<WebPluginDbContext, int, int, bool, Task<int>>
        UpdateWebPluginStatusAsync = EF.CompileAsyncQuery(
            (WebPluginDbContext ctx, int tenantId, int id, bool enabled) =>
                ctx.WebPlugins
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .ExecuteUpdate(toUpdate => toUpdate
                        .SetProperty(p => p.Enabled, enabled)
                    ));

    public static readonly Func<WebPluginDbContext, int, int, Task<int>>
        DeleteWebPluginByIdAsync = EF.CompileAsyncQuery(
            (WebPluginDbContext ctx, int tenantId, int id) =>
                ctx.WebPlugins
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .ExecuteDelete());
}