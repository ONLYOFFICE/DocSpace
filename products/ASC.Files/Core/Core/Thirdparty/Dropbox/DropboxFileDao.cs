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

using File = System.IO.File;

namespace ASC.Files.Thirdparty.Dropbox;

[Scope]
internal class DropboxFileDao : DropboxDaoBase, IFileDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly DropboxDaoSelector _dropboxDaoSelector;
    private readonly IFileDao<int> _fileDao;

    public DropboxFileDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<DropboxFileDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        DropboxDaoSelector dropboxDaoSelector,
        IFileDao<int> fileDao,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
        _crossDao = crossDao;
        _dropboxDaoSelector = dropboxDaoSelector;
        _fileDao = fileDao;
    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        var dropboxFilePath = MakeDropboxPath(fileId);
        await ProviderInfo.CacheResetAsync(dropboxFilePath, true);

        var dropboxFile = await GetDropboxFileAsync(fileId);
        var parentPath = GetParentFolderPath(dropboxFile);
        if (parentPath != null)
        {
            await ProviderInfo.CacheResetAsync(parentPath);
        }
    }

    public Task<File<string>> GetFileAsync(string fileId)
    {
        return GetFileAsync(fileId, 1);
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return ToFile(await GetDropboxFileAsync(fileId));
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var items = await GetDropboxItemsAsync(parentId, false);
        var metadata = items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));

        return metadata == null
                   ? null
                   : ToFile(metadata.AsFile);
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return ToFile(await GetDropboxFileAsync(fileId));
    }

    public async IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
    {
        var file = await GetFileAsync(fileId);
        yield return file;
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
    {
        if (fileIds == null || !fileIds.Any())
        {
            yield break;
        }

        foreach (var fileId in fileIds)
        {
            yield return ToFile(await GetDropboxFileAsync(fileId));
        }
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
            case FilterType.OFormOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.OForm);
                break;
            case FilterType.OFormTemplateOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.OFormTemplate);
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

    public async IAsyncEnumerable<string> GetFilesAsync(string parentId)
    {
        var items = await GetDropboxItemsAsync(parentId, false);

        foreach (var entry in items)
        {
            yield return MakeId(entry);
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false, bool excludeSubject = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        //Get only files

        var items = await GetDropboxItemsAsync(parentId, false);
        var files = items.Select(item => ToFile(item.AsFile));

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
            case FilterType.OFormOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.OForm);
                break;
            case FilterType.OFormTemplateOnly:
                files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.OFormTemplate);
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
        var dropboxFilePath = MakeDropboxPath(file.Id);
        await ProviderInfo.CacheResetAsync(dropboxFilePath, true);

        var dropboxFile = await GetDropboxFileAsync(file.Id);
        if (dropboxFile == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (dropboxFile is ErrorFile errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var fileStream = await (await ProviderInfo.StorageAsync).DownloadStreamAsync(MakeDropboxPath(dropboxFile), (int)offset);

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
        FileMetadata newDropboxFile = null;

        if (file.Id != null)
        {
            var filePath = MakeDropboxPath(file.Id);
            newDropboxFile = await (await ProviderInfo.StorageAsync).SaveStreamAsync(filePath, fileStream);
            if (!newDropboxFile.Name.Equals(file.Title))
            {
                var parentFolderPath = GetParentFolderPath(newDropboxFile);
                file.Title = await GetAvailableTitleAsync(file.Title, parentFolderPath, IsExistAsync);
                newDropboxFile = await (await ProviderInfo.StorageAsync).MoveFileAsync(filePath, parentFolderPath, file.Title);
            }
        }
        else if (file.ParentId != null)
        {
            var folderPath = MakeDropboxPath(file.ParentId);
            file.Title = await GetAvailableTitleAsync(file.Title, folderPath, IsExistAsync);
            newDropboxFile = await (await ProviderInfo.StorageAsync).CreateFileAsync(fileStream, file.Title, folderPath);
        }

        await ProviderInfo.CacheResetAsync(newDropboxFile);
        var parentPath = GetParentFolderPath(newDropboxFile);
        if (parentPath != null)
        {
            await ProviderInfo.CacheResetAsync(parentPath);
        }

        return ToFile(newDropboxFile);
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var dropboxFile = await GetDropboxFileAsync(fileId);
        if (dropboxFile == null)
        {
            return;
        }

        var id = MakeId(dropboxFile);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using (var tx = await filesDbContext.Database.BeginTransactionAsync())
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


        if (dropboxFile is not ErrorFile)
        {
            await (await ProviderInfo.StorageAsync).DeleteItemAsync(dropboxFile);
        }

        await ProviderInfo.CacheResetAsync(MakeDropboxPath(dropboxFile), true);
        var parentFolderPath = GetParentFolderPath(dropboxFile);
        if (parentFolderPath != null)
        {
            await ProviderInfo.CacheResetAsync(parentFolderPath);
        }
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var items = await GetDropboxItemsAsync(folderId, false);

        return items.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<TTo> MoveFileAsync<TTo>(string fileId, TTo toFolderId)
    {
        if (toFolderId is int tId)
        {
            return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tId), typeof(TTo));
        }

        if (toFolderId is string tsId)
        {
            return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tsId), typeof(TTo));
        }

        throw new NotImplementedException();
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var dropboxFile = await GetDropboxFileAsync(fileId);
        if (dropboxFile is ErrorFile errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var toDropboxFolder = await GetDropboxFolderAsync(toFolderId);
        if (toDropboxFolder is ErrorFolder errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var fromFolderPath = GetParentFolderPath(dropboxFile);

        dropboxFile = await (await ProviderInfo.StorageAsync).MoveFileAsync(MakeDropboxPath(dropboxFile), MakeDropboxPath(toDropboxFolder), dropboxFile.Name);

        await ProviderInfo.CacheResetAsync(MakeDropboxPath(dropboxFile), true);
        await ProviderInfo.CacheResetAsync(fromFolderPath);
        await ProviderInfo.CacheResetAsync(MakeDropboxPath(toDropboxFolder));

        return MakeId(dropboxFile);
    }

    public async Task<int> MoveFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _dropboxDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            true)
            ;

        return moved.Id;
    }

    public async Task<File<TTo>> CopyFileAsync<TTo>(string fileId, TTo toFolderId)
    {
        if (toFolderId is int tId)
        {
            return await CopyFileAsync(fileId, tId) as File<TTo>;
        }

        if (toFolderId is string tsId)
        {
            return await CopyFileAsync(fileId, tsId) as File<TTo>;
        }

        throw new NotImplementedException();
    }

    public Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        var moved = _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _dropboxDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            false);

        return moved;
    }

    public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var dropboxFile = await GetDropboxFileAsync(fileId);
        if (dropboxFile is ErrorFile errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var toDropboxFolder = await GetDropboxFolderAsync(toFolderId);
        if (toDropboxFolder is ErrorFolder errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var newDropboxFile = await (await ProviderInfo.StorageAsync).CopyFileAsync(MakeDropboxPath(dropboxFile), MakeDropboxPath(toDropboxFolder), dropboxFile.Name);

        await ProviderInfo.CacheResetAsync(newDropboxFile);
        await ProviderInfo.CacheResetAsync(MakeDropboxPath(toDropboxFolder));

        return ToFile(newDropboxFile);
    }

    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var dropboxFile = await GetDropboxFileAsync(file.Id);
        var parentFolderPath = GetParentFolderPath(dropboxFile);
        newTitle = await GetAvailableTitleAsync(newTitle, parentFolderPath, IsExistAsync);

        dropboxFile = await (await ProviderInfo.StorageAsync).MoveFileAsync(MakeDropboxPath(dropboxFile), parentFolderPath, newTitle);

        await ProviderInfo.CacheResetAsync(dropboxFile);
        var parentPath = GetParentFolderPath(dropboxFile);
        if (parentPath != null)
        {
            await ProviderInfo.CacheResetAsync(parentPath);
        }

        return MakeId(dropboxFile);
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
        return Task.CompletedTask;
    }

    public bool UseTrashForRemove(File<string> file)
    {
        return false;
    }

    public override Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        return ProviderInfo.GetThumbnailsAsync(_dropboxDaoSelector.ConvertId(fileId), width, height);
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

        var dropboxSession = await (await ProviderInfo.StorageAsync).CreateResumableSessionAsync();
        if (dropboxSession != null)
        {
            uploadSession.Items["DropboxSession"] = dropboxSession;
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

            uploadSession.File = await SaveFileAsync(uploadSession.File, stream);
            uploadSession.BytesUploaded = chunkLength;

            return uploadSession.File;
        }

        if (uploadSession.Items.ContainsKey("DropboxSession"))
        {
            var dropboxSession = uploadSession.GetItemOrDefault<string>("DropboxSession");
            await (await ProviderInfo.StorageAsync).TransferAsync(dropboxSession, uploadSession.BytesUploaded, stream);
        }
        else
        {
            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            using var fs = new FileStream(tempPath, FileMode.Append);
            await stream.CopyToAsync(fs);
        }

        uploadSession.BytesUploaded += chunkLength;

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal || uploadSession.LastChunk)
        {
            uploadSession.File = await FinalizeUploadSessionAsync(uploadSession);
        }
        else
        {
            uploadSession.File = RestoreIds(uploadSession.File);
        }

        return uploadSession.File;
    }

    public async Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("DropboxSession"))
        {
            var dropboxSession = uploadSession.GetItemOrDefault<string>("DropboxSession");

            Metadata dropboxFile;
            var file = uploadSession.File;
            if (file.Id != null)
            {
                var dropboxFilePath = MakeDropboxPath(file.Id);
                dropboxFile = await (await ProviderInfo.StorageAsync).FinishResumableSessionAsync(dropboxSession, dropboxFilePath, uploadSession.BytesUploaded);
            }
            else
            {
                var folderPath = MakeDropboxPath(file.ParentId);
                var title = await GetAvailableTitleAsync(file.Title, folderPath, IsExistAsync);
                dropboxFile = await (await ProviderInfo.StorageAsync).FinishResumableSessionAsync(dropboxSession, folderPath, title, uploadSession.BytesUploaded);
            }

            await ProviderInfo.CacheResetAsync(MakeDropboxPath(dropboxFile));
            await ProviderInfo.CacheResetAsync(GetParentFolderPath(dropboxFile), false);

            return ToFile(dropboxFile.AsFile);
        }

        using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"),
                                       FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);

        return await SaveFileAsync(uploadSession.File, fs);
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("TempPath"))
        {
            File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
        }

        return Task.CompletedTask;
    }
    #endregion
}
