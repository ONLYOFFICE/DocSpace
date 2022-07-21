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

namespace ASC.Files.Thirdparty.OneDrive;

[Scope]
internal class OneDriveFileDao : OneDriveDaoBase, IFileDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly OneDriveDaoSelector _oneDriveDaoSelector;
    private readonly IFileDao<int> _fileDao;

    public OneDriveFileDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<OneDriveFileDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        OneDriveDaoSelector oneDriveDaoSelector,
        IFileDao<int> fileDao,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
        _crossDao = crossDao;
        _oneDriveDaoSelector = oneDriveDaoSelector;
        _fileDao = fileDao;
    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        var onedriveFileId = MakeOneDriveId(fileId);
        await ProviderInfo.CacheResetAsync(onedriveFileId).ConfigureAwait(false);

        var onedriveFile = await GetOneDriveItemAsync(fileId).ConfigureAwait(false);
        var parentId = GetParentFolderId(onedriveFile);
        if (parentId != null)
        {
            await ProviderInfo.CacheResetAsync(parentId).ConfigureAwait(false);
        }
    }

    public Task<File<string>> GetFileAsync(string fileId)
    {
        return GetFileAsync(fileId, 1);
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return ToFile(await GetOneDriveItemAsync(fileId).ConfigureAwait(false));
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var items = await GetOneDriveItemsAsync(parentId, false).ConfigureAwait(false);

        return ToFile(items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase) && item.File != null));
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return ToFile(await GetOneDriveItemAsync(fileId).ConfigureAwait(false));
    }

    public IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
    {
        return GetFileAsync(fileId).ToAsyncEnumerable();
    }

    public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
    {
        var list = new List<File<string>>();

        if (fileIds == null || !fileIds.Any())
        {
            return AsyncEnumerable.Empty<File<string>>();
        }

        var result = fileIds.ToAsyncEnumerable().SelectAwait(async e => ToFile(await GetOneDriveItemAsync(e).ConfigureAwait(false)));

        return result;
    }

    public IAsyncEnumerable<File<string>> GetFilesFilteredAsync(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
    {
        if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly)
        {
            return AsyncEnumerable.Empty<File<string>>();
        }

        var files = GetFilesAsync(fileIds);

        //Filter
        if (subjectID != Guid.Empty)
        {
            files = files.Where(x => subjectGroup
                                         ? _userManager.IsUserInGroup(x.CreateBy, subjectID)
                                         : x.CreateBy == subjectID);
        }

        switch (filterType)
        {
            case FilterType.FoldersOnly:
                return AsyncEnumerable.Empty<File<string>>();
            case FilterType.DocumentsOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Document);
                break;
            case FilterType.PresentationsOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Presentation);
                break;
            case FilterType.SpreadsheetsOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Spreadsheet);
                break;
            case FilterType.ImagesOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Image);
                break;
            case FilterType.ArchiveOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Archive);
                break;
            case FilterType.MediaOnly:
                files = files.Where(x =>
                {
                    var fileType = FileUtility.GetFileTypeByFileName(x.Title);

                    return fileType == FileType.Audio || fileType == FileType.Video;
                });
                break;
            case FilterType.ByExtension:
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.Trim().ToLower();
                    files = files.Where(x => FileUtility.GetFileExtension(x.Title).Equals(searchText));
                }
                break;
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
        }

        return files;
    }


    public async Task<List<string>> GetFilesAsync(string parentId)
    {
        var items = await GetOneDriveItemsAsync(parentId, false).ConfigureAwait(false);

        return items.Select(entry => MakeId(entry.Id)).ToList();
    }


    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        //Get only files
        var items = await GetOneDriveItemsAsync(parentId, false).ConfigureAwait(false);
        var files = items.Select(ToFile);

        //Filter
        if (subjectID != Guid.Empty)
        {
            files = files.Where(x => subjectGroup
                                         ? _userManager.IsUserInGroup(x.CreateBy, subjectID)
                                         : x.CreateBy == subjectID);
        }

        switch (filterType)
        {
            case FilterType.FoldersOnly:
                yield break;
            case FilterType.DocumentsOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Document);
                break;
            case FilterType.PresentationsOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Presentation);
                break;
            case FilterType.SpreadsheetsOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Spreadsheet);
                break;
            case FilterType.ImagesOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Image);
                break;
            case FilterType.ArchiveOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Archive);
                break;
            case FilterType.MediaOnly:
                files = files.Where(x =>
                {
                    var fileType = FileUtility.GetFileTypeByFileName(x.Title);

                    return fileType == FileType.Audio || fileType == FileType.Video;
                });
                break;
            case FilterType.ByExtension:
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.Trim().ToLower();
                    files = files.Where(x => FileUtility.GetFileExtension(x.Title).Equals(searchText));
                }
                break;
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
        }

        if (orderBy == null)
        {
            orderBy = new OrderBy(SortedByType.DateAndTime, false);
        }

        files = orderBy.SortedBy switch
        {
            SortedByType.Author => orderBy.IsAsc ? files.OrderBy(x => x.CreateBy) : files.OrderByDescending(x => x.CreateBy),
            SortedByType.AZ => orderBy.IsAsc ? files.OrderBy(x => x.Title) : files.OrderByDescending(x => x.Title),
            SortedByType.DateAndTime => orderBy.IsAsc ? files.OrderBy(x => x.ModifiedOn) : files.OrderByDescending(x => x.ModifiedOn),
            SortedByType.DateAndTimeCreation => orderBy.IsAsc ? files.OrderBy(x => x.CreateOn) : files.OrderByDescending(x => x.CreateOn),
            _ => orderBy.IsAsc ? files.OrderBy(x => x.Title) : files.OrderByDescending(x => x.Title),
        };

        foreach (var f in files)
        {
            yield return f;
        }
    }

    public override Task<Stream> GetFileStreamAsync(File<string> file)
    {
        return GetFileStreamAsync(file, 0);
    }

    public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
    {
        var onedriveFileId = MakeOneDriveId(file.Id);
        await ProviderInfo.CacheResetAsync(onedriveFileId).ConfigureAwait(false);

        var onedriveFile = await GetOneDriveItemAsync(file.Id).ConfigureAwait(false);
        if (onedriveFile == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }
        if (onedriveFile is ErrorItem errorItem)
        {
            throw new Exception(errorItem.Error);
        }

        var storage = await ProviderInfo.StorageAsync;
        var fileStream = await storage.DownloadStreamAsync(onedriveFile, (int)offset).ConfigureAwait(false);

        return fileStream;
    }

    public Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
    {
        throw new NotSupportedException();
    }

    public Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
    {
        return Task.FromResult(false);
    }

    public Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(fileStream);

        return InternalSaveFileAsync(file, fileStream);
    }

    private async Task<File<string>> InternalSaveFileAsync(File<string> file, Stream fileStream)
    {
        Item newOneDriveFile = null;
        var storage = await ProviderInfo.StorageAsync;

        if (file.Id != null)
        {
            newOneDriveFile = await storage.SaveStreamAsync(MakeOneDriveId(file.Id), fileStream).ConfigureAwait(false);
            if (!newOneDriveFile.Name.Equals(file.Title))
            {
                file.Title = await GetAvailableTitleAsync(file.Title, GetParentFolderId(newOneDriveFile), IsExistAsync).ConfigureAwait(false);
                newOneDriveFile = await storage.RenameItemAsync(newOneDriveFile.Id, file.Title).ConfigureAwait(false);
            }
        }
        else if (file.ParentId != null)
        {
            var folderId = MakeOneDriveId(file.ParentId);
            var folder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);
            file.Title = await GetAvailableTitleAsync(file.Title, folderId, IsExistAsync).ConfigureAwait(false);
            newOneDriveFile = await storage.CreateFileAsync(fileStream, file.Title, MakeOneDrivePath(folder)).ConfigureAwait(false);
        }

        if (newOneDriveFile != null)
        {
            await ProviderInfo.CacheResetAsync(newOneDriveFile.Id).ConfigureAwait(false);
        }

        var parentId = GetParentFolderId(newOneDriveFile);
        if (parentId != null)
        {
            await ProviderInfo.CacheResetAsync(parentId).ConfigureAwait(false);
        }

        return ToFile(newOneDriveFile);
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var onedriveFile = await GetOneDriveItemAsync(fileId).ConfigureAwait(false);
        if (onedriveFile == null)
        {
            return;
        }

        var id = MakeId(onedriveFile.Id);

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

        if (onedriveFile is not ErrorItem)
        {
            var storage = await ProviderInfo.StorageAsync;
            await storage.DeleteItemAsync(onedriveFile);
        }

        await ProviderInfo.CacheResetAsync(onedriveFile.Id).ConfigureAwait(false);
        var parentFolderId = GetParentFolderId(onedriveFile);
        if (parentFolderId != null)
        {
            await ProviderInfo.CacheResetAsync(parentFolderId).ConfigureAwait(false);
        }
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var items = await GetOneDriveItemsAsync(folderId.ToString(), false).ConfigureAwait(false);

        return items.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<TTo> MoveFileAsync<TTo>(string fileId, TTo toFolderId)
    {
        if (toFolderId is int tId)
        {
            return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tId).ConfigureAwait(false), typeof(TTo));
        }

        if (toFolderId is string tsId)
        {
            return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tsId).ConfigureAwait(false), typeof(TTo));
        }

        throw new NotImplementedException();
    }

    public async Task<int> MoveFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _oneDriveDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            true)
            .ConfigureAwait(false);

        return moved.Id;
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var onedriveFile = await GetOneDriveItemAsync(fileId).ConfigureAwait(false);
        if (onedriveFile is ErrorItem errorItem)
        {
            throw new Exception(errorItem.Error);
        }

        var toOneDriveFolder = await GetOneDriveItemAsync(toFolderId).ConfigureAwait(false);
        if (toOneDriveFolder is ErrorItem errorItem1)
        {
            throw new Exception(errorItem1.Error);
        }

        var fromFolderId = GetParentFolderId(onedriveFile);

        var newTitle = await GetAvailableTitleAsync(onedriveFile.Name, toOneDriveFolder.Id, IsExistAsync).ConfigureAwait(false);
        var storage = await ProviderInfo.StorageAsync;
        onedriveFile = await storage.MoveItemAsync(onedriveFile.Id, newTitle, toOneDriveFolder.Id).ConfigureAwait(false);

        await ProviderInfo.CacheResetAsync(onedriveFile.Id).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(fromFolderId).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(toOneDriveFolder.Id).ConfigureAwait(false);

        return MakeId(onedriveFile.Id);
    }

    public async Task<File<TTo>> CopyFileAsync<TTo>(string fileId, TTo toFolderId)
    {
        if (toFolderId is int tId)
        {
            return await CopyFileAsync(fileId, tId).ConfigureAwait(false) as File<TTo>;
        }

        if (toFolderId is string tsId)
        {
            return await CopyFileAsync(fileId, tsId).ConfigureAwait(false) as File<TTo>;
        }

        throw new NotImplementedException();
    }


    public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, _oneDriveDaoSelector.ConvertId,
                toFolderId, _fileDao, r => r,
                false)
            .ConfigureAwait(false);

        return moved;
    }


    public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var onedriveFile = await GetOneDriveItemAsync(fileId).ConfigureAwait(false);
        if (onedriveFile is ErrorItem errorItem)
        {
            throw new Exception(errorItem.Error);
        }

        var toOneDriveFolder = await GetOneDriveItemAsync(toFolderId).ConfigureAwait(false);
        if (toOneDriveFolder is ErrorItem errorItem1)
        {
            throw new Exception(errorItem1.Error);
        }

        var newTitle = await GetAvailableTitleAsync(onedriveFile.Name, toOneDriveFolder.Id, IsExistAsync).ConfigureAwait(false);
        var storage = await ProviderInfo.StorageAsync;
        var newOneDriveFile = await storage.CopyItemAsync(onedriveFile.Id, newTitle, toOneDriveFolder.Id).ConfigureAwait(false);

        await ProviderInfo.CacheResetAsync(newOneDriveFile.Id).ConfigureAwait(false);
        await ProviderInfo.CacheResetAsync(toOneDriveFolder.Id).ConfigureAwait(false);

        return ToFile(newOneDriveFile);
    }


    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var onedriveFile = await GetOneDriveItemAsync(file.Id).ConfigureAwait(false);
        newTitle = await GetAvailableTitleAsync(newTitle, GetParentFolderId(onedriveFile), IsExistAsync).ConfigureAwait(false);

        var storage = await ProviderInfo.StorageAsync;
        onedriveFile = await storage.RenameItemAsync(onedriveFile.Id, newTitle).ConfigureAwait(false);

        await ProviderInfo.CacheResetAsync(onedriveFile.Id).ConfigureAwait(false);
        var parentId = GetParentFolderId(onedriveFile);
        if (parentId != null)
        {
            await ProviderInfo.CacheResetAsync(parentId).ConfigureAwait(false);
        }

        return MakeId(onedriveFile.Id);
    }


    public Task<string> UpdateCommentAsync(string fileId, int fileVersion, string comment)
    {
        return Task.FromResult(string.Empty);
    }

    public Task CompleteVersionAsync(string fileId, int fileVersion)
    {
        return Task.CompletedTask;
    }

    public Task ContinueVersionAsync(string fileId, int fileVersion)
    {
        return Task.FromResult(0);
    }

    public bool UseTrashForRemove(File<string> file)
    {
        return false;
    }

    public override Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        var oneDriveId = MakeOneDriveId(_oneDriveDaoSelector.ConvertId(fileId));
        return ProviderInfo.GetThumbnailAsync(oneDriveId, width, height);
    }

    #region chunking

    private File<string> RestoreIds(File<string> file)
    {
        if (file == null)
        {
            return null;
        }

        if (file.Id != null)
        {
            file.Id = MakeId(file.Id);
        }

        if (file.ParentId != null)
        {
            file.ParentId = MakeId(file.ParentId);
        }

        return file;
    }

    public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        if (_setupInfo.ChunkUploadSize > contentLength)
        {
            return Task.FromResult(new ChunkedUploadSession<string>(RestoreIds(file), contentLength) { UseChunks = false });
        }

        return InternalCreateUploadSessionAsync(file, contentLength);
    }

    private async Task<ChunkedUploadSession<string>> InternalCreateUploadSessionAsync(File<string> file, long contentLength)
    {
        var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

        Item onedriveFile;
        if (file.Id != null)
        {
            onedriveFile = await GetOneDriveItemAsync(file.Id).ConfigureAwait(false);
        }
        else
        {
            var folder = await GetOneDriveItemAsync(file.ParentId).ConfigureAwait(false);
            onedriveFile = new Item { Name = file.Title, ParentReference = new ItemReference { Id = folder.Id } };
        }

        var storage = await ProviderInfo.StorageAsync;
        var onedriveSession = await storage.CreateResumableSessionAsync(onedriveFile, contentLength).ConfigureAwait(false);
        if (onedriveSession != null)
        {
            uploadSession.Items["OneDriveSession"] = onedriveSession;
        }
        else
        {
            uploadSession.Items["TempPath"] = _tempPath.GetTempFileName();
        }

        uploadSession.File = RestoreIds(uploadSession.File);

        return uploadSession;
    }

    public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength)
    {
        if (!uploadSession.UseChunks)
        {
            if (uploadSession.BytesTotal == 0)
            {
                uploadSession.BytesTotal = chunkLength;
            }

            uploadSession.File = await SaveFileAsync(uploadSession.File, stream).ConfigureAwait(false);
            uploadSession.BytesUploaded = chunkLength;

            return uploadSession.File;
        }

        if (uploadSession.Items.ContainsKey("OneDriveSession"))
        {
            var oneDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("OneDriveSession");
            var storage = await ProviderInfo.StorageAsync;
            await storage.TransferAsync(oneDriveSession, stream, chunkLength).ConfigureAwait(false);
        }
        else
        {
            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            using var fs = new FileStream(tempPath, FileMode.Append);
            await stream.CopyToAsync(fs).ConfigureAwait(false);
        }

        uploadSession.BytesUploaded += chunkLength;

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
        {
            uploadSession.File = await FinalizeUploadSessionAsync(uploadSession).ConfigureAwait(false);
        }
        else
        {
            uploadSession.File = RestoreIds(uploadSession.File);
        }

        return uploadSession.File;
    }

    private async Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("OneDriveSession"))
        {
            var oneDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("OneDriveSession");

            await ProviderInfo.CacheResetAsync(oneDriveSession.FileId).ConfigureAwait(false);
            var parentDriveId = oneDriveSession.FolderId;
            if (parentDriveId != null)
            {
                await ProviderInfo.CacheResetAsync(parentDriveId).ConfigureAwait(false);
            }

            return ToFile(await GetOneDriveItemAsync(oneDriveSession.FileId).ConfigureAwait(false));
        }

        using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"), FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);

        return await SaveFileAsync(uploadSession.File, fs).ConfigureAwait(false);
    }

    public async Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("OneDriveSession"))
        {
            var oneDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("OneDriveSession");

            if (oneDriveSession.Status != ResumableUploadSessionStatus.Completed)
            {
                var storage = await ProviderInfo.StorageAsync;
                await storage.CancelTransferAsync(oneDriveSession).ConfigureAwait(false);

                oneDriveSession.Status = ResumableUploadSessionStatus.Aborted;
            }
        }
        else if (uploadSession.Items.ContainsKey("TempPath"))
        {
            System.IO.File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
        }
    }
    #endregion
}
