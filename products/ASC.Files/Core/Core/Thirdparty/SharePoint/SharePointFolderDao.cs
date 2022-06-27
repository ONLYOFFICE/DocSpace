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

namespace ASC.Files.Thirdparty.SharePoint;

[Scope]
internal class SharePointFolderDao : SharePointDaoBase, IFolderDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly SharePointDaoSelector _sharePointDaoSelector;
    private readonly IFileDao<int> _fileDao;
    private readonly IFolderDao<int> _folderDao;

    public SharePointFolderDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharePointFolderDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        SharePointDaoSelector sharePointDaoSelector,
        IFileDao<int> fileDao,
        IFolderDao<int> folderDao,
        TempPath tempPath)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
    {
        _crossDao = crossDao;
        _sharePointDaoSelector = sharePointDaoSelector;
        _fileDao = fileDao;
        _folderDao = folderDao;
    }

    public async Task<Folder<string>> GetFolderAsync(string folderId)
    {
        return ProviderInfo.ToFolder(await ProviderInfo.GetFolderByIdAsync(folderId).ConfigureAwait(false));
    }

    public async Task<Folder<string>> GetFolderAsync(string title, string parentId)
    {
        var folderFolders = await ProviderInfo.GetFolderFoldersAsync(parentId).ConfigureAwait(false);

        return ProviderInfo.ToFolder(folderFolders
                    .FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
    }

    public Task<Folder<string>> GetRootFolderAsync(string folderId)
    {
        return Task.FromResult(ProviderInfo.ToFolder(ProviderInfo.RootFolder));
    }

    public Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
    {
        return Task.FromResult(ProviderInfo.ToFolder(ProviderInfo.RootFolder));
    }

    public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
    {
        var folderFolders = await ProviderInfo.GetFolderFoldersAsync(parentId).ConfigureAwait(false);

        foreach (var i in folderFolders)
        {
            yield return ProviderInfo.ToFolder(i);
        }
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false,
        IEnumerable<string> tagNames = null)
    {
        return GetFoldersAsync(parentId, orderBy, new[] { filterType }, subjectGroup, subjectID, searchText, withSubfolders, tagNames);
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, IEnumerable<FilterType> filterTypes, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false, 
        IEnumerable<string> tagNames = null)
    {
        if (!CheckForInvalidFilters(filterTypes))
        {
            return AsyncEnumerable.Empty<Folder<string>>();
        }

        var folders = GetFoldersAsync(parentId);

        folders = SetFilterByTypes(folders, filterTypes);

        //Filter
        if (subjectID != Guid.Empty)
        {
            folders = folders.Where(x => subjectGroup
                                             ? _userManager.IsUserInGroup(x.CreateBy, subjectID)
                                             : x.CreateBy == subjectID);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            folders = folders.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
        }

        folders = FilterByTags(folders, tagNames);

        if (orderBy == null)
        {
            orderBy = new OrderBy(SortedByType.DateAndTime, false);
        }

        folders = orderBy.SortedBy switch
        {
            SortedByType.Author => orderBy.IsAsc ? folders.OrderBy(x => x.CreateBy) : folders.OrderByDescending(x => x.CreateBy),
            SortedByType.AZ => orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title),
            SortedByType.DateAndTime => orderBy.IsAsc ? folders.OrderBy(x => x.ModifiedOn) : folders.OrderByDescending(x => x.ModifiedOn),
            SortedByType.DateAndTimeCreation => orderBy.IsAsc ? folders.OrderBy(x => x.CreateOn) : folders.OrderByDescending(x => x.CreateOn),
            _ => orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title),
        };

        return folders;
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true,
        IEnumerable<string> tagNames = null)
    {
        return GetFoldersAsync(folderIds, new[] { filterType }, subjectGroup, subjectID, searchText, searchSubfolders, checkShare, tagNames);
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(IEnumerable<string> folderIds, IEnumerable<FilterType> filterTypes, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true,
        IEnumerable<string> tagNames = null)
    {
        if (!CheckForInvalidFilters(filterTypes))
        {
            return AsyncEnumerable.Empty<Folder<string>>();
        }

        var folders = folderIds.ToAsyncEnumerable().SelectAwait(async e => await GetFolderAsync(e).ConfigureAwait(false));

        folders = SetFilterByTypes(folders, filterTypes);

        if (subjectID.HasValue && subjectID != Guid.Empty)
        {
            folders = folders.Where(x => subjectGroup
                                             ? _userManager.IsUserInGroup(x.CreateBy, subjectID.Value)
                                             : x.CreateBy == subjectID);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            folders = folders.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
        }

        folders = FilterByTags(folders, tagNames);

        return folders;
    }

    public async Task<List<Folder<string>>> GetParentFoldersAsync(string folderId)
    {
        var path = new List<Folder<string>>();
        var folder = await ProviderInfo.GetFolderByIdAsync(folderId).ConfigureAwait(false);
        if (folder != null)
        {
            do
            {
                path.Add(ProviderInfo.ToFolder(folder));
            } while (folder != ProviderInfo.RootFolder && folder is not SharePointFolderErrorEntry &&
                     (folder = await ProviderInfo.GetParentFolderAsync(folder.ServerRelativeUrl).ConfigureAwait(false)) != null);
        }
        path.Reverse();

        return path;
    }

    public async Task<string> SaveFolderAsync(Folder<string> folder)
    {
        if (folder.Id != null)
        {
            //Create with id
            var savedfolder = await ProviderInfo.CreateFolderAsync(folder.Id).ConfigureAwait(false);

            return ProviderInfo.ToFolder(savedfolder).Id;
        }

        if (folder.ParentId != null)
        {
            var parentFolder = await ProviderInfo.GetFolderByIdAsync(folder.ParentId).ConfigureAwait(false);

            folder.Title = await GetAvailableTitleAsync(folder.Title, parentFolder, IsExistAsync).ConfigureAwait(false);

            var newFolder = await ProviderInfo.CreateFolderAsync(parentFolder.ServerRelativeUrl + "/" + folder.Title).ConfigureAwait(false);

            return ProviderInfo.ToFolder(newFolder).Id;
        }

        return null;
    }

    public async Task<bool> IsExistAsync(string title, Microsoft.SharePoint.Client.Folder folder)
    {
        var folderFolders = await ProviderInfo.GetFolderFoldersAsync(folder.ServerRelativeUrl).ConfigureAwait(false);

        return folderFolders.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task DeleteFolderAsync(string folderId)
    {
        var folder = await ProviderInfo.GetFolderByIdAsync(folderId).ConfigureAwait(false);

        var strategy = FilesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using (var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var hashIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                   .Where(r => r.Id.StartsWith(folder.ServerRelativeUrl))
                   .Select(r => r.HashId)
                   .ToListAsync()
                   .ConfigureAwait(false);

                var link = await Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId))
                    .ToListAsync()
                    .ConfigureAwait(false);

                FilesDbContext.TagLink.RemoveRange(link);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var tagsToRemove = from ft in FilesDbContext.Tag
                                   join ftl in FilesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                                   where ftl == null
                                   select ft;

                FilesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

                var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var mappingToDelete = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => hashIDs.Any(h => h == r.HashId));

                FilesDbContext.ThirdpartyIdMapping.RemoveRange(await mappingToDelete.ToListAsync());
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false);
            }
        });


        await ProviderInfo.DeleteFolderAsync(folderId).ConfigureAwait(false);
    }

    public async Task<TTo> MoveFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tId, cancellationToken).ConfigureAwait(false), typeof(TTo));
        }

        if (toFolderId is string tsId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tsId, cancellationToken).ConfigureAwait(false), typeof(TTo));
        }

        throw new NotImplementedException();
    }

    public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, _sharePointDaoSelector.GetFileDao(folderId), _sharePointDaoSelector.ConvertId,
                toFolderId, _folderDao, _fileDao, r => r,
                true, cancellationToken)
            .ConfigureAwait(false);

        return moved.Id;
    }

    public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var newFolderId = await ProviderInfo.MoveFolderAsync(folderId, toFolderId).ConfigureAwait(false);
        await UpdatePathInDBAsync(ProviderInfo.MakeId(folderId), newFolderId).ConfigureAwait(false);

        return newFolderId;
    }

    public async Task<Folder<TTo>> CopyFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return await CopyFolderAsync(folderId, tId, cancellationToken).ConfigureAwait(false) as Folder<TTo>;
        }

        if (toFolderId is string tsId)
        {
            return await CopyFolderAsync(folderId, tsId, cancellationToken).ConfigureAwait(false) as Folder<TTo>;
        }

        throw new NotImplementedException();
    }

    public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        return ProviderInfo.ToFolder(await ProviderInfo.CopyFolderAsync(folderId, toFolderId).ConfigureAwait(false));
    }

    public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
            folderId, this, _sharePointDaoSelector.GetFileDao(folderId), _sharePointDaoSelector.ConvertId,
            toFolderId, _folderDao, _fileDao, r => r,
            false, cancellationToken)
            .ConfigureAwait(false);

        return moved;
    }

    public Task<IDictionary<string, string>> CanMoveOrCopyAsync<TTo>(string[] folderIds, TTo to)
    {
        if (to is int tId)
        {
            return CanMoveOrCopyAsync(folderIds, tId);
        }

        if (to is string tsId)
        {
            return CanMoveOrCopyAsync(folderIds, tsId);
        }

        throw new NotImplementedException();
    }

    public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, string to)
    {
        return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
    }

    public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, int to)
    {
        return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
    }

    public async Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
    {
        var oldId = ProviderInfo.MakeId(folder.Id);
        var newFolderId = oldId;
        if (ProviderInfo.SpRootFolderId.Equals(folder.Id))
        {
            //It's root folder
            await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle).ConfigureAwait(false);
            //rename provider customer title
        }
        else
        {
            newFolderId = (string)await ProviderInfo.RenameFolderAsync(folder.Id, newTitle).ConfigureAwait(false);
        }
        await UpdatePathInDBAsync(oldId, newFolderId).ConfigureAwait(false);

        return newFolderId;
    }


    public Task<int> GetItemsCountAsync(string folderId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsEmptyAsync(string folderId)
    {
        var folder = await ProviderInfo.GetFolderByIdAsync(folderId).ConfigureAwait(false);

        return folder.ItemCount == 0;
    }

    public bool UseTrashForRemove(Folder<string> folder)
    {
        return false;
    }

    public bool UseRecursiveOperation<TTo>(string folderId, TTo toRootFolderId)
    {
        return false;
    }

    public bool UseRecursiveOperation(string folderId, int toRootFolderId)
    {
        return false;
    }

    public bool UseRecursiveOperation(string folderId, string toRootFolderId)
    {
        return false;
    }

    public bool CanCalculateSubitems(string entryId)
    {
        return false;
    }

    public Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload = false)
    {
        return Task.FromResult(2L * 1024L * 1024L * 1024L);
    }
}
