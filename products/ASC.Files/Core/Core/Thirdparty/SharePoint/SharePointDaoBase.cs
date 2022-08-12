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

using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint;

internal class SharePointDaoBase : ThirdPartyProviderDao<SharePointProviderInfo>
{
    protected override string Id => "spoint";

    public SharePointDaoBase(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharePointDaoBase> monitor,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
    }

    protected string GetAvailableTitle(string requestTitle, Folder parentFolderID, Func<string, Folder, bool> isExist)
    {
        if (!isExist(requestTitle, parentFolderID))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf('.') != -1)
            {
                insertIndex = requestTitle.LastIndexOf('.');
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (isExist(requestTitle, parentFolderID))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    protected async Task<string> GetAvailableTitleAsync(string requestTitle, Folder parentFolderID, Func<string, Folder, Task<bool>> isExist)
    {
        if (!await isExist(requestTitle, parentFolderID))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf(".", StringComparison.Ordinal) != -1)
            {
                insertIndex = requestTitle.LastIndexOf(".", StringComparison.Ordinal);
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (await isExist(requestTitle, parentFolderID))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    private string MatchEvaluator(Match match)
    {
        var index = Convert.ToInt32(match.Groups[2].Value);
        var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);

        return string.Format(" ({0}){1}", index + 1, staticText);
    }

    protected Task UpdatePathInDBAsync(string oldValue, string newValue)
    {
        if (oldValue.Equals(newValue))
        {
            return Task.CompletedTask;
        }

        return InternalUpdatePathInDBAsync(oldValue, newValue);
    }

    private async Task InternalUpdatePathInDBAsync(string oldValue, string newValue)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = filesDbContext.Database.BeginTransaction();
            var oldIDs = await Query(filesDbContext.ThirdpartyIdMapping)
            .Where(r => r.Id.StartsWith(oldValue))
            .Select(r => r.Id)
            .ToListAsync();

        foreach (var oldID in oldIDs)
        {
            var oldHashID = await MappingIDAsync(oldID);
            var newID = oldID.Replace(oldValue, newValue);
            var newHashID = await MappingIDAsync(newID);

                var mappingForUpdate = await Query(filesDbContext.ThirdpartyIdMapping)
                .Where(r => r.HashId == oldHashID)
                .ToListAsync();

            foreach (var m in mappingForUpdate)
            {
                m.Id = newID;
                m.HashId = newHashID;
            }

                await filesDbContext.SaveChangesAsync();

                var securityForUpdate = await Query(filesDbContext.Security)
                .Where(r => r.EntryId == oldHashID)
                .ToListAsync();

            foreach (var s in securityForUpdate)
            {
                s.EntryId = newHashID;
                    s.TimeStamp = DateTime.Now;
            }

                await filesDbContext.SaveChangesAsync();

                var linkForUpdate = await Query(filesDbContext.TagLink)
                .Where(r => r.EntryId == oldHashID)
                .ToListAsync();

            foreach (var l in linkForUpdate)
            {
                l.EntryId = newHashID;
            }

                await filesDbContext.SaveChangesAsync();
        }

        await tx.CommitAsync();
        });
    }

    protected Task<string> MappingIDAsync(string id)
    {
        return MappingIDAsync(id, false);
    }

    protected override string MakeId(string path = null)
    {
        return path;
    }

    protected override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var folders = await ProviderInfo.GetFolderFoldersAsync(folderId);
        var subFolders = folders.Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl));

        var folderFiles = await ProviderInfo.GetFolderFilesAsync(folderId);
        var files = folderFiles.Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl));

        return subFolders.Concat(files);
    }
}
