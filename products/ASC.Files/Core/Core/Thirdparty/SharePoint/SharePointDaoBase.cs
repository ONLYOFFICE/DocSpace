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

using File = Microsoft.SharePoint.Client.File;
using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint;

internal class SharePointDaoBase : ThirdPartyProviderDao<File, Folder, ClientObject>
{
    internal SharePointProviderInfo SharePointProviderInfo { get; private set; }

    public SharePointDaoBase(IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager, 
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        SetupInfo setupInfo,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext, 
        RegexDaoSelectorBase<File, Folder, ClientObject> regexDaoSelectorBase) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextFactory, setupInfo, fileUtility, tempPath, authContext, regexDaoSelectorBase)
    {
    }

    public void Init(string pathPrefix, IProviderInfo<File, Folder, ClientObject> providerInfo)
    {
        PathPrefix = pathPrefix;
        ProviderInfo = providerInfo;
        SharePointProviderInfo = providerInfo as SharePointProviderInfo;
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

    protected async ValueTask UpdatePathInDBAsync(string oldValue, string newValue)
    {
        if (oldValue.Equals(newValue))
        {
            return;
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();
            var oldIDs = await Query(filesDbContext.ThirdpartyIdMapping)
            .Where(r => r.Id.StartsWith(oldValue))
            .Select(r => r.Id)
            .ToListAsync();

            foreach (var oldID in oldIDs)
            {
                var oldHashID = await MappingIDAsync(oldID);
                var newID = oldID.Replace(oldValue, newValue);
                var newHashID = await MappingIDAsync(newID);

                var mappingForDelete = await Query(filesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.HashId == oldHashID).ToListAsync();

                var mappingForInsert = mappingForDelete.Select(m => new DbFilesThirdpartyIdMapping
                {
                    TenantId = m.TenantId,
                    Id = newID,
                    HashId = newHashID
                });

                filesDbContext.RemoveRange(mappingForDelete);
                await filesDbContext.AddRangeAsync(mappingForInsert);

                var securityForDelete = await Query(filesDbContext.Security)
                    .Where(r => r.EntryId == oldHashID).ToListAsync();

                var securityForInsert = securityForDelete.Select(s => new DbFilesSecurity
                {
                    TenantId = s.TenantId,
                    TimeStamp = DateTime.Now,
                    EntryId = newHashID,
                    Share = s.Share,
                    Subject = s.Subject,
                    EntryType = s.EntryType,
                    Owner = s.Owner
                });

                filesDbContext.RemoveRange(securityForDelete);
                await filesDbContext.AddRangeAsync(securityForInsert);

                var linkForDelete = await Query(filesDbContext.TagLink)
                    .Where(r => r.EntryId == oldHashID).ToListAsync();

                var linkForInsert = linkForDelete.Select(l => new DbFilesTagLink
                {
                    EntryId = newHashID,
                    Count = l.Count,
                    CreateBy = l.CreateBy,
                    CreateOn = l.CreateOn,
                    EntryType = l.EntryType,
                    TagId = l.TagId,
                    TenantId = l.TenantId
                });

                filesDbContext.RemoveRange(linkForDelete);
                await filesDbContext.AddRangeAsync(linkForInsert);

                await filesDbContext.SaveChangesAsync();
            }

            await tx.CommitAsync();
        });
    }

    public override string MakeId(string path = null)
    {
        return path;
    }

    public override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var folders = await SharePointProviderInfo.GetFolderFoldersAsync(folderId);
        var subFolders = folders.Select(x => SharePointProviderInfo.MakeId(x.ServerRelativeUrl));

        var folderFiles = await SharePointProviderInfo.GetFolderFilesAsync(folderId);
        var files = folderFiles.Select(x => SharePointProviderInfo.MakeId(x.ServerRelativeUrl));

        return subFolders.Concat(files);
    }
}
