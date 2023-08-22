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
        MaxTotalSizeStatistic maxTotalSizeStatistic,
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
              maxTotalSizeStatistic,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache)
    { }

    public async Task AddLinkAsync(string sourceId, string linkedId)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        await filesDbContext.AddOrUpdateAsync(r => r.FilesLink, new DbFilesLink()
        {
            TenantId = TenantID,
            SourceId = (await MappingIDAsync(sourceId)).ToString(),
            LinkedId = (await MappingIDAsync(linkedId)).ToString(),
            LinkedFor = _authContext.CurrentAccount.ID
        });

        await filesDbContext.SaveChangesAsync();
    }

    public async Task<string> GetSourceAsync(string linkedId)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        linkedId = (await MappingIDAsync(linkedId)).ToString();

        var sourceId = await Queries.SourceIdAsync(filesDbContext, TenantID, linkedId, _authContext.CurrentAccount.ID);

        return (await MappingIDAsync(sourceId))?.ToString();
    }

    public async Task<string> GetLinkedAsync(string sourceId)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        sourceId = (await MappingIDAsync(sourceId)).ToString();

        var linkedId = await Queries.LinkedIdAsync(filesDbContext, TenantID, sourceId, _authContext.CurrentAccount.ID);

        return (await MappingIDAsync(linkedId))?.ToString();
    }

    public async Task DeleteLinkAsync(string sourceId)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        sourceId = (await MappingIDAsync(sourceId)).ToString();

        var link = await Queries.FileLinkAsync(filesDbContext, TenantID, sourceId, _authContext.CurrentAccount.ID);

        filesDbContext.FilesLink.Remove(link);

        await filesDbContext.SaveChangesAsync();
    }

    public async Task DeleteAllLinkAsync(string fileId)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        fileId = (await MappingIDAsync(fileId)).ToString();

        await Queries.DeleteFileLinks(filesDbContext, TenantID, fileId);
    }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, int, string, Guid, Task<string>> SourceIdAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string linkedId, Guid id) =>
                ctx.FilesLink
                    .Where(r => r.TenantId == tenantId && r.LinkedId == linkedId && r.LinkedFor == id)
                    .Select(r => r.SourceId)
                    .SingleOrDefault());

    public static readonly Func<FilesDbContext, int, string, Guid, Task<string>> LinkedIdAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string sourceId, Guid id) =>
                ctx.FilesLink
                    .Where(r => r.TenantId == tenantId && r.SourceId == sourceId && r.LinkedFor == id)
                    .Select(r => r.LinkedId)
                    .SingleOrDefault());

    public static readonly Func<FilesDbContext, int, string, Guid, Task<DbFilesLink>> FileLinkAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string sourceId, Guid id) =>
                ctx.FilesLink
                    .SingleOrDefault(r => r.TenantId == tenantId && r.SourceId == sourceId && r.LinkedFor == id));

    public static readonly Func<FilesDbContext, int, string, Task<int>> DeleteFileLinks =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string fileId) =>
                ctx.FilesLink
                    .Where(r => r.TenantId == tenantId && (r.SourceId == fileId || r.LinkedId == fileId))
                    .ExecuteDelete());
}
