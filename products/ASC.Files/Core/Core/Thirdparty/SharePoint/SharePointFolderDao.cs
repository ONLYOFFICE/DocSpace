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
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharePointFolderDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        SharePointDaoSelector sharePointDaoSelector,
        IFileDao<int> fileDao,
        IFolderDao<int> folderDao,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
        _crossDao = crossDao;
        _sharePointDaoSelector = sharePointDaoSelector;
        _fileDao = fileDao;
        _folderDao = folderDao;
    }

    public async Task<Folder<string>> GetFolderAsync(string folderId)
    {
        return ProviderInfo.ToFolder(await ProviderInfo.GetFolderByIdAsync(folderId));
    }

    public async Task<Folder<string>> GetFolderAsync(string title, string parentId)
    {
        var folderFolders = await ProviderInfo.GetFolderFoldersAsync(parentId);

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

    public async IAsyncEnumerable<Folder<string>> GetRoomsAsync(IEnumerable<string> parentsIds, IEnumerable<string> roomsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        if (CheckInvalidFilter(filterType) || (provider != ProviderFilter.None && provider != ProviderFilter.SharePoint))
        {
            yield break;
        }

        var rooms = roomsIds.ToAsyncEnumerable().SelectAwait(async e => await GetFolderAsync(e).ConfigureAwait(false));

        rooms = FilterByRoomType(rooms, filterType);
        rooms = FilterBySubject(rooms, subjectId, excludeSubject, subjectFilter, subjectEntriesIds);

        if (!string.IsNullOrEmpty(searchText))
        {
            rooms = rooms.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        rooms = FilterByTags(rooms, withoutTags, tags, filesDbContext);

        await foreach (var room in rooms)
        {
            yield return room;
        }
    }

    public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
    {
        var folderFolders = await ProviderInfo.GetFolderFoldersAsync(parentId);

        foreach (var i in folderFolders)
        {
            yield return ProviderInfo.ToFolder(i);
        }
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText,
        bool withSubfolders = false, bool excludeSubject = false, int offset = 0, int count = -1)
    {
        if (CheckInvalidFilter(filterType))
        {
            return AsyncEnumerable.Empty<Folder<string>>();
        }

        var folders = GetFoldersAsync(parentId);

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

        orderBy ??= new OrderBy(SortedByType.DateAndTime, false);

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

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true, bool excludeSubject = false)
    {
        if (CheckInvalidFilter(filterType))
        {
            return AsyncEnumerable.Empty<Folder<string>>();
        }

        var folders = folderIds.ToAsyncEnumerable().SelectAwait(async e => await GetFolderAsync(e));

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

        return folders;
    }

    public async IAsyncEnumerable<Folder<string>> GetParentFoldersAsync(string folderId)
    {
        var path = new List<Folder<string>>();
        var folder = await ProviderInfo.GetFolderByIdAsync(folderId);
        if (folder != null)
        {
            do
            {
                path.Add(ProviderInfo.ToFolder(folder));
            } while (folder != ProviderInfo.RootFolder && folder is not SharePointFolderErrorEntry &&
                     (folder = await ProviderInfo.GetParentFolderAsync(folder.ServerRelativeUrl)) != null);
        }
        path.Reverse();

        await foreach (var p in path.ToAsyncEnumerable())
        {
            yield return p;
        }
    }

    public async Task<string> SaveFolderAsync(Folder<string> folder)
    {
        if (folder.Id != null)
        {
            //Create with id
            var savedfolder = await ProviderInfo.CreateFolderAsync(folder.Id);

            return ProviderInfo.ToFolder(savedfolder).Id;
        }

        if (folder.ParentId != null)
        {
            var parentFolder = await ProviderInfo.GetFolderByIdAsync(folder.ParentId);

            folder.Title = await GetAvailableTitleAsync(folder.Title, parentFolder, IsExistAsync);

            var newFolder = await ProviderInfo.CreateFolderAsync(parentFolder.ServerRelativeUrl + "/" + folder.Title);

            return ProviderInfo.ToFolder(newFolder).Id;
        }

        return null;
    }

    public async Task<bool> IsExistAsync(string title, Microsoft.SharePoint.Client.Folder folder)
    {
        var folderFolders = await ProviderInfo.GetFolderFoldersAsync(folder.ServerRelativeUrl);

        return folderFolders.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task DeleteFolderAsync(string folderId)
    {
        var folder = await ProviderInfo.GetFolderByIdAsync(folderId);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using (var tx = await filesDbContext.Database.BeginTransactionAsync())
            {
                var hashIDs = await Query(filesDbContext.ThirdpartyIdMapping)
               .Where(r => r.Id.StartsWith(folder.ServerRelativeUrl))
               .Select(r => r.HashId)
               .ToListAsync()
               ;

                var link = await Query(filesDbContext.TagLink)
                .Where(r => hashIDs.Any(h => h == r.EntryId))
                .ToListAsync()
                ;

                filesDbContext.TagLink.RemoveRange(link);
                await filesDbContext.SaveChangesAsync();

                var tagsToRemove = from ft in filesDbContext.Tag
                                   join ftl in filesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                                   where ftl == null
                                   select ft;

                filesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

                var securityToDelete = Query(filesDbContext.Security)
                .Where(r => hashIDs.Any(h => h == r.EntryId));

                filesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
                await filesDbContext.SaveChangesAsync();

                var mappingToDelete = Query(filesDbContext.ThirdpartyIdMapping)
                .Where(r => hashIDs.Any(h => h == r.HashId));

                filesDbContext.ThirdpartyIdMapping.RemoveRange(await mappingToDelete.ToListAsync());
                await filesDbContext.SaveChangesAsync();

                await tx.CommitAsync();
            }
        });


        await ProviderInfo.DeleteFolderAsync(folderId);
    }

    public async Task<TTo> MoveFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tId, cancellationToken), typeof(TTo));
        }

        if (toFolderId is string tsId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tsId, cancellationToken), typeof(TTo));
        }

        throw new NotImplementedException();
    }

    public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, _sharePointDaoSelector.GetFileDao(folderId), _sharePointDaoSelector.ConvertId,
                toFolderId, _folderDao, _fileDao, r => r,
                true, cancellationToken)
            ;

        return moved.Id;
    }

    public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var newFolderId = await ProviderInfo.MoveFolderAsync(folderId, toFolderId);
        await UpdatePathInDBAsync(ProviderInfo.MakeId(folderId), newFolderId);

        return newFolderId;
    }

    public async Task<Folder<TTo>> CopyFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return await CopyFolderAsync(folderId, tId, cancellationToken) as Folder<TTo>;
        }

        if (toFolderId is string tsId)
        {
            return await CopyFolderAsync(folderId, tsId, cancellationToken) as Folder<TTo>;
        }

        throw new NotImplementedException();
    }

    public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        return ProviderInfo.ToFolder(await ProviderInfo.CopyFolderAsync(folderId, toFolderId));
    }

    public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
            folderId, this, _sharePointDaoSelector.GetFileDao(folderId), _sharePointDaoSelector.ConvertId,
            toFolderId, _folderDao, _fileDao, r => r,
            false, cancellationToken)
            ;

        return moved;
    }

    public async Task<IDictionary<string, TTo>> CanMoveOrCopyAsync<TTo>(string[] folderIds, TTo to)
    {
        if (to is int tId)
        {
            return await CanMoveOrCopyAsync<TTo>(folderIds, tId);
        }

        if (to is string tsId)
        {
            return await CanMoveOrCopyAsync<TTo>(folderIds, tsId);
        }

        throw new NotImplementedException();
    }

    public Task<IDictionary<string, TTo>> CanMoveOrCopyAsync<TTo>(string[] folderIds, string to)
    {
        return Task.FromResult((IDictionary<string, TTo>)new Dictionary<string, TTo>());
    }

    public Task<IDictionary<string, TTo>> CanMoveOrCopyAsync<TTo>(string[] folderIds, int to)
    {
        return Task.FromResult((IDictionary<string, TTo>)new Dictionary<string, TTo>());
    }

    public async Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
    {
        var oldId = ProviderInfo.MakeId(folder.Id);
        var newFolderId = oldId;
        if (ProviderInfo.SpRootFolderId.Equals(folder.Id))
        {
            //It's root folder
            await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle);
            //rename provider customer title
        }
        else
        {
            newFolderId = (string)await ProviderInfo.RenameFolderAsync(folder.Id, newTitle);

            if (DocSpaceHelper.IsRoom(ProviderInfo.FolderType) && ProviderInfo.FolderId != null)
            {
                await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle);

                if (ProviderInfo.FolderId == oldId)
                {
                    await DaoSelector.UpdateProviderFolderId(ProviderInfo, newFolderId);
                }
            }
        }

        await UpdatePathInDBAsync(oldId, newFolderId);

        return newFolderId;
    }


    public Task<int> GetItemsCountAsync(string folderId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsEmptyAsync(string folderId)
    {
        var folder = await ProviderInfo.GetFolderByIdAsync(folderId);

        return folder.ItemCount == 0;
    }

    public bool UseTrashForRemoveAsync(Folder<string> folder)
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

    public IDataWriteOperator CreateDataWriteOperator(
            string folderId,
            CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder)
    {
        return null;
    }
}
