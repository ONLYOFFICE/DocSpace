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

namespace ASC.Files.Core.Data;

[Scope]
internal class LinkDao : AbstractDao, ILinkDao
{
    public LinkDao(
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticProvider,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache)
        : base(
              dbContextManager,
              userManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              tenantExtra,
              tenantStatisticProvider,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache)
    { }

    public async Task AddLinkAsync(string sourceId, string linkedId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await filesDbContext.AddOrUpdateAsync(r => r.FilesLink, new DbFilesLink()
        {
            TenantId = TenantID,
            SourceId = sourceId,
            LinkedId = linkedId,
            LinkedFor = _authContext.CurrentAccount.ID
        });

        await filesDbContext.SaveChangesAsync();
    }

    public async Task<string> GetSourceAsync(string linkedId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        return await filesDbContext.FilesLink
            .Where(r => r.TenantId == TenantID && r.LinkedId == linkedId && r.LinkedFor == _authContext.CurrentAccount.ID)
            .Select(r => r.SourceId)
            .SingleOrDefaultAsync();
    }

    public async Task<string> GetLinkedAsync(string sourceId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        return await filesDbContext.FilesLink
            .Where(r => r.TenantId == TenantID && r.SourceId == sourceId && r.LinkedFor == _authContext.CurrentAccount.ID)
            .Select(r => r.LinkedId)
            .SingleOrDefaultAsync();
    }

    public async Task DeleteLinkAsync(string sourceId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var link = await filesDbContext.FilesLink
            .Where(r => r.TenantId == TenantID && r.SourceId == sourceId && r.LinkedFor == _authContext.CurrentAccount.ID)
            .SingleOrDefaultAsync();

        filesDbContext.FilesLink.Remove(link);

        await filesDbContext.SaveChangesAsync();
    }

    public async Task DeleteAllLinkAsync(string fileId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var link = await filesDbContext.FilesLink.Where(r => r.TenantId == TenantID && (r.SourceId == fileId || r.LinkedId == fileId)).ToListAsync();

        filesDbContext.FilesLink.RemoveRange(link);

        await filesDbContext.SaveChangesAsync();
    }
}
