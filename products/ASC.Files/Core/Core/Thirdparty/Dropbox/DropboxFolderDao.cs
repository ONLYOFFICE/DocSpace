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

namespace ASC.Files.Thirdparty.Dropbox;

[Scope]
internal class DropboxFolderDao : DropboxDaoBase, IFolderDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly DropboxDaoSelector _dropboxDaoSelector;
    private readonly IFileDao<int> _fileDao;
    private readonly IFolderDao<int> _folderDao;

    public DropboxFolderDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<DropboxFolderDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        DropboxDaoSelector dropboxDaoSelector,
        IFileDao<int> fileDao,
        IFolderDao<int> folderDao,
        TempPath tempPath)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
    {
        _crossDao = crossDao;
        _dropboxDaoSelector = dropboxDaoSelector;
        _fileDao = fileDao;
        _folderDao = folderDao;
    }

    public async Task<Folder<string>> GetFolderAsync(string folderId)
    {
        return ToFolder(await GetDropboxFolderAsync(folderId).ConfigureAwait(false));
    }

    public async Task<Folder<string>> GetFolderAsync(string title, string parentId)
    {
        var items = await GetDropboxItemsAsync(parentId, true).ConfigureAwait(false);
        var metadata = items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));

        return metadata == null
                   ? null
                   : ToFolder(metadata.AsFolder);
    }

    public Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
    {
        return GetRootFolderAsync(fileId);
    }

    public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
    {
        var items = await GetDropboxItemsAsync(parentId, true).ConfigureAwait(false);
        foreach (var i in items)
        {
            yield return ToFolder(i.AsFolder);
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

        var folders = GetFoldersAsync(parentId); //TODO:!!!

        folders = SetFilterByTypes(folders, filterTypes);

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

        while (folderId != null)
        {
            var dropboxFolder = await GetDropboxFolderAsync(folderId).ConfigureAwait(false);

            if (dropboxFolder is ErrorFolder)
            {
                folderId = null;
            }
            else
            {
                path.Add(ToFolder(dropboxFolder));
                folderId = GetParentFolderPath(dropboxFolder);
            }
        }

        path.Reverse();

        return path;
    }

    public Task<string> SaveFolderAsync(Folder<string> folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return InternalSaveFolderAsync(folder);
    }

    public async Task<string> InternalSaveFolderAsync(Folder<string> folder)
    {
        if (folder.Id != null)
        {
            return await RenameFolderAsync(folder, folder.Title).ConfigureAwait(false);
        }

        if (folder.ParentId != null)
        {
            var dropboxFolderPath = MakeDropboxPath(folder.ParentId);

            folder.Title = await GetAvailableTitleAsync(folder.Title, dropboxFolderPath, IsExistAsync).ConfigureAwait(false);

            var dropboxFolder = await (await ProviderInfo.StorageAsync).CreateFolderAsync(folder.Title, dropboxFolderPath).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(dropboxFolder).ConfigureAwait(false);
            var parentFolderPath = GetParentFolderPath(dropboxFolder);
            if (parentFolderPath != null)
            {
                await ProviderInfo.CacheResetAsync(parentFolderPath).ConfigureAwait(false);
            }

            return MakeId(dropboxFolder);
        }

        return null;
    }

    public async Task<bool> IsExistAsync(string title, string folderId)
    {
        var items = await GetDropboxItemsAsync(folderId, true).ConfigureAwait(false);

        return items.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task DeleteFolderAsync(string folderId)
    {
        var dropboxFolder = await GetDropboxFolderAsync(folderId).ConfigureAwait(false);
        var id = MakeId(dropboxFolder);

        var strategy = FilesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using (var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var hashIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                   .Where(r => r.Id.StartsWith(id))
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

        if (dropboxFolder is not ErrorFolder)
        {
            await (await ProviderInfo.StorageAsync).DeleteItemAsync(dropboxFolder).ConfigureAwait(false);
        }

        await ProviderInfo.CacheResetAsync(MakeDropboxPath(dropboxFolder), true).ConfigureAwait(false);
        var parentFolderPath = GetParentFolderPath(dropboxFolder);
        if (parentFolderPath != null)
        {
            await ProviderInfo.CacheResetAsync(parentFolderPath).ConfigureAwait(false);
        }
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
            folderId, this, _dropboxDaoSelector.GetFileDao(folderId), _dropboxDaoSelector.ConvertId,
            toFolderId, _folderDao, _fileDao, r => r,
            true, cancellationToken)
            .ConfigureAwait(false);

        return moved.Id;
    }

    public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var dropboxFolder = await GetDropboxFolderAsync(folderId).ConfigureAwait(false);
        if (dropboxFolder is ErrorFolder errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var toDropboxFolder = await GetDropboxFolderAsync(toFolderId).ConfigureAwait(false);
        if (toDropboxFolder is ErrorFolder errorFolder1)
        {
            throw new Exception(errorFolder1.Error);
        }

        var fromFolderPath = GetParentFolderPath(dropboxFolder);

        dropboxFolder = await (await ProviderInfo.StorageAsync).MoveFolderAsync(MakeDropboxPath(dropboxFolder), MakeDropboxPath(toDropboxFolder), dropboxFolder.Name).ConfigureAwait(false);

        await ProviderInfo.CacheResetAsync(MakeDropboxPath(dropboxFolder), false).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(fromFolderPath).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(MakeDropboxPath(toDropboxFolder)).ConfigureAwait(false);

        return MakeId(dropboxFolder);
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

    public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
            folderId, this, _dropboxDaoSelector.GetFileDao(folderId), _dropboxDaoSelector.ConvertId,
            toFolderId, _folderDao, _fileDao, r => r,
            false, cancellationToken)
            .ConfigureAwait(false);

        return moved;
    }

    public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var dropboxFolder = await GetDropboxFolderAsync(folderId).ConfigureAwait(false);
        if (dropboxFolder is ErrorFolder folder)
        {
            throw new Exception(folder.Error);
        }

        var toDropboxFolder = await GetDropboxFolderAsync(toFolderId).ConfigureAwait(false);
        if (toDropboxFolder is ErrorFolder errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var newDropboxFolder = await (await ProviderInfo.StorageAsync).CopyFolderAsync(MakeDropboxPath(dropboxFolder), MakeDropboxPath(toDropboxFolder), dropboxFolder.Name).ConfigureAwait(false);

        await ProviderInfo.CacheResetAsync(newDropboxFolder).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(MakeDropboxPath(newDropboxFolder), false).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(MakeDropboxPath(toDropboxFolder)).ConfigureAwait(false);

        return ToFolder(newDropboxFolder);
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
        var dropboxFolder = await GetDropboxFolderAsync(folder.Id).ConfigureAwait(false);
        var parentFolderPath = GetParentFolderPath(dropboxFolder);

        if (IsRoot(dropboxFolder))
        {
            //It's root folder
            await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle).ConfigureAwait(false);
            //rename provider customer title
        }
        else
        {
            newTitle = await GetAvailableTitleAsync(newTitle, parentFolderPath, IsExistAsync).ConfigureAwait(false);

            //rename folder
            dropboxFolder = await (await ProviderInfo.StorageAsync).MoveFolderAsync(MakeDropboxPath(dropboxFolder), parentFolderPath, newTitle).ConfigureAwait(false);
        }

        await ProviderInfo.CacheResetAsync(dropboxFolder).ConfigureAwait(false);
        if (parentFolderPath != null)
        {
            await ProviderInfo.CacheResetAsync(parentFolderPath).ConfigureAwait(false);
        }

        return MakeId(dropboxFolder);
    }

    public Task<int> GetItemsCountAsync(string folderId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsEmptyAsync(string folderId)
    {
        var dropboxFolderPath = MakeDropboxPath(folderId);
        //note: without cache
        var items = await (await ProviderInfo.StorageAsync).GetItemsAsync(dropboxFolderPath).ConfigureAwait(false);

        return items.Count == 0;
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

    public async Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload = false)
    {
        var storageMaxUploadSize = (await ProviderInfo.StorageAsync).MaxChunkedUploadFileSize;

        return chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, _setupInfo.AvailableFileSize);
    }
}
