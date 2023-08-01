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

namespace ASC.Files.Thirdparty.Sharpbox;

[Scope]
internal class SharpBoxFolderDao : SharpBoxDaoBase, IFolderDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly SharpBoxDaoSelector _sharpBoxDaoSelector;
    private readonly IFileDao<int> _fileDao;
    private readonly IFolderDao<int> _folderDao;

    public SharpBoxFolderDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharpBoxFolderDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        SharpBoxDaoSelector sharpBoxDaoSelector,
        IFileDao<int> fileDao,
        IFolderDao<int> folderDao,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
        _crossDao = crossDao;
        _sharpBoxDaoSelector = sharpBoxDaoSelector;
        _fileDao = fileDao;
        _folderDao = folderDao;
    }

    public Task<Folder<string>> GetFolderAsync(string folderId)
    {
        return Task.FromResult(ToFolder(GetFolderById(folderId)));
    }

    public Task<Folder<string>> GetFolderAsync(string title, string parentId)
    {
        var parentFolder = ProviderInfo.Storage.GetFolder(MakePath(parentId));

        return Task.FromResult(ToFolder(parentFolder.OfType<ICloudDirectoryEntry>().FirstOrDefault(x => x.Name.Equals(title, StringComparison.OrdinalIgnoreCase))));
    }

    public Task<Folder<string>> GetRootFolderAsync(string folderId)
    {
        return Task.FromResult(ToFolder(RootFolder()));
    }

    public Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
    {
        return Task.FromResult(ToFolder(RootFolder()));
    }

    public async IAsyncEnumerable<Folder<string>> GetRoomsAsync(IEnumerable<string> parentsIds, IEnumerable<string> roomsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider,
        SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        if (CheckInvalidFilter(filterType) || (provider != ProviderFilter.None && provider != ProviderFilter.kDrive && provider != ProviderFilter.WebDav && provider != ProviderFilter.Yandex))
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

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
    {
        var parentFolder = ProviderInfo.Storage.GetFolder(MakePath(parentId));

        return parentFolder.OfType<ICloudDirectoryEntry>().Select(ToFolder).ToAsyncEnumerable();
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText,
        bool withSubfolders = false, bool excludeSubject = false, int offset = 0, int count = -1)
    {
        if (CheckInvalidFilter(filterType))
        {
            return AsyncEnumerable.Empty<Folder<string>>();
        }

        var folders = GetFoldersAsync(parentId); //TODO:!!!

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

    public IAsyncEnumerable<Folder<string>> GetParentFoldersAsync(string folderId)
    {
        var path = new List<Folder<string>>();
        var folder = GetFolderById(folderId);
        if (folder != null)
        {
            do
            {
                path.Add(ToFolder(folder));
            } while ((folder = folder.Parent) != null);
        }

        path.Reverse();

        return path.ToAsyncEnumerable();
    }


    public async Task<string> SaveFolderAsync(Folder<string> folder)
    {
        try
        {
            if (folder.Id != null)
            {
                //Create with id
                var savedfolder = ProviderInfo.Storage.CreateFolder(MakePath(folder.Id));

                return MakeId(savedfolder);
            }
            if (folder.ParentId != null)
            {
                var parentFolder = GetFolderById(folder.ParentId);

                folder.Title = await GetAvailableTitleAsync(folder.Title, parentFolder, IsExistAsync);

                var newFolder = ProviderInfo.Storage.CreateFolder(folder.Title, parentFolder);

                return MakeId(newFolder);
            }
        }
        catch (SharpBoxException e)
        {
            var webException = (WebException)e.InnerException;
            if (webException != null)
            {
                var response = (HttpWebResponse)webException.Response;
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
                    }
                }
                throw;
            }
        }

        return null;
    }

    public async Task DeleteFolderAsync(string folderId)
    {
        var folder = GetFolderById(folderId);
        var id = MakeId(folder);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using (var tx = await filesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var hashIDs = await Query(filesDbContext.ThirdpartyIdMapping)
               .Where(r => r.Id.StartsWith(id))
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




        if (folder is not ErrorEntry)
        {
            ProviderInfo.Storage.DeleteFileSystemEntry(folder);
        }
    }

    public Task<bool> IsExistAsync(string title, ICloudDirectoryEntry folder)
    {
        try
        {
            return Task.FromResult(ProviderInfo.Storage.GetFileSystemObject(title, folder) != null);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception)
        {

        }

        return Task.FromResult(false);
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
            folderId, this, _sharpBoxDaoSelector.GetFileDao(folderId), _sharpBoxDaoSelector.ConvertId,
            toFolderId, _folderDao, _fileDao, r => r,
            true, cancellationToken)
            ;

        return moved.Id;
    }

    public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var entry = GetFolderById(folderId);
        var folder = GetFolderById(toFolderId);

        var oldFolderId = MakeId(entry);

        if (!ProviderInfo.Storage.MoveFileSystemEntry(entry, folder))
        {
            throw new Exception("Error while moving");
        }

        var newFolderId = MakeId(entry);

        await UpdatePathInDBAsync(oldFolderId, newFolderId);

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

    public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
            folderId, this, _sharpBoxDaoSelector.GetFileDao(folderId), _sharpBoxDaoSelector.ConvertId,
            toFolderId, _folderDao, _fileDao, r => r,
            false, cancellationToken)
            ;

        return moved;
    }

    public Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var folder = GetFolderById(folderId);
        if (!ProviderInfo.Storage.CopyFileSystemEntry(MakePath(folderId), MakePath(toFolderId)))
        {
            throw new Exception("Error while copying");
        }

        return Task.FromResult(ToFolder(GetFolderById(toFolderId).OfType<ICloudDirectoryEntry>().FirstOrDefault(x => x.Name == folder.Name)));
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
        var entry = GetFolderById(folder.Id);

        var oldId = MakeId(entry);
        var newId = oldId;

        if ("/".Equals(MakePath(folder.Id)))
        {
            //It's root folder
            await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle);
            //rename provider customer title
        }
        else
        {
            var parentFolder = GetFolderById(folder.ParentId);
            newTitle = await GetAvailableTitleAsync(newTitle, parentFolder, IsExistAsync);

            //rename folder
            if (ProviderInfo.Storage.RenameFileSystemEntry(entry, newTitle))
            {
                //Folder data must be already updated by provider
                //We can't search google folders by title because root can have multiple folders with the same name
                //var newFolder = SharpBoxProviderInfo.Storage.GetFileSystemObject(newTitle, folder.Parent);
                newId = MakeId(entry);

                if (DocSpaceHelper.IsRoom(ProviderInfo.FolderType) && ProviderInfo.FolderId != null)
                {
                    await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle);

                    if (ProviderInfo.FolderId == oldId)
                    {
                        await DaoSelector.UpdateProviderFolderId(ProviderInfo, newId);
                    }
                }
            }
        }

        await UpdatePathInDBAsync(oldId, newId);

        return newId;
    }

    public Task<int> GetItemsCountAsync(string folderId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsEmptyAsync(string folderId)
    {
        return Task.FromResult(GetFolderById(folderId).Count == 0);
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
        var storageMaxUploadSize =
            chunkedUpload
                ? ProviderInfo.Storage.CurrentConfiguration.Limits.MaxChunkedUploadFileSize
                : ProviderInfo.Storage.CurrentConfiguration.Limits.MaxUploadFileSize;

        if (storageMaxUploadSize == -1)
        {
            storageMaxUploadSize = long.MaxValue;
        }

        return Task.FromResult(chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, _setupInfo.AvailableFileSize));
    }

    public IDataWriteOperator CreateDataWriteOperator(
            string folderId,
            CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder)
    {
        return null;
    }
}
