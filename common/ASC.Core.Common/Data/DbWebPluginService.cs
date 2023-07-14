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

        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.AddOrUpdateAsync(q => q.WebPlugins, webPlugin);
        await dbContext.SaveChangesAsync();

        return webPlugin;
    }

    public async Task<DbWebPlugin> GetByIdAsync(int tenantId, int id)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var webPlugin = await dbContext.WebPlugins.FindAsync(id);

        return webPlugin.TenantId == tenantId ? webPlugin : null;
    }

    public async Task<List<DbWebPlugin>> GetAsync(int tenantId, bool? enabled = null)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return await dbContext.WebPlugins
            .Where(r => r.TenantId == tenantId && (!enabled.HasValue || r.Enabled == enabled.Value))
            .OrderByDescending(r => r.Id)
            .ToListAsync();
    }

    public async Task UpdateAsync(int tenantId, int id, bool enabled)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        await dbContext.WebPlugins
           .Where(r => r.Id == id && r.TenantId == tenantId)
           .ExecuteUpdateAsync(r => r.SetProperty(p => p.Enabled, enabled));
    }

    public async Task DeleteAsync(int tenantId, int id)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        await dbContext.WebPlugins
            .Where(r => r.Id == id && r.TenantId == tenantId)
            .ExecuteDeleteAsync();
    }
}
