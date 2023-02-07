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

using DriveFile = Google.Apis.Drive.v3.Data.File;
using File = System.IO.File;

namespace ASC.Files.Thirdparty.GoogleDrive;

[Scope]
internal class GoogleDriveFileDao : GoogleDriveDaoBase, IFileDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly GoogleDriveDaoSelector _googleDriveDaoSelector;
    private readonly IFileDao<int> _fileDao;

    public GoogleDriveFileDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<GoogleDriveFileDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        GoogleDriveDaoSelector googleDriveDaoSelector,
        IFileDao<int> fileDao,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
        _crossDao = crossDao;
        _googleDriveDaoSelector = googleDriveDaoSelector;
        _fileDao = fileDao;
    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        var driveId = MakeDriveId(fileId);
        await ProviderInfo.CacheResetAsync(driveId, true);

        var driveFile = await GetDriveEntryAsync(fileId);
        var parentDriveId = GetParentDriveId(driveFile);
        if (parentDriveId != null)
        {
            await ProviderInfo.CacheResetAsync(parentDriveId);
        }
    }

    public Task<File<string>> GetFileAsync(string fileId)
    {
        return GetFileAsync(fileId, 1);
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return ToFile(await GetDriveEntryAsync(fileId));
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var data = await GetDriveEntriesAsync(parentId, false);

        return ToFile(data.FirstOrDefault(file => file.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return ToFile(await GetDriveEntryAsync(fileId));
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
            yield return ToFile(await GetDriveEntryAsync(fileId));
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
        var entries = await GetDriveEntriesAsync(parentId, false);

        foreach (var entry in entries)
        {
            yield return MakeId(entry.Id);
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false, bool excludeSubject = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        //Get only files
        var entries = await GetDriveEntriesAsync(parentId, false);
        var files = entries.Select(ToFile);

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
        var driveId = MakeDriveId(file.Id);
        await ProviderInfo.CacheResetAsync(driveId, true);
        var driveFile = await GetDriveEntryAsync(file.Id);
        if (driveFile == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (driveFile is ErrorDriveEntry errorDriveEntry)
        {
            throw new Exception(errorDriveEntry.Error);
        }

        var storage = await ProviderInfo.StorageAsync;
        var fileStream = await storage.DownloadStreamAsync(driveFile, (int)offset);

        if (!driveFile.Size.HasValue && fileStream != null && fileStream.CanSeek)
        {
            file.ContentLength = fileStream.Length; // hack for google drive
        }

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

    public async Task<File<string>> InternalSaveFileAsync(File<string> file, Stream fileStream)
    {
        DriveFile newDriveFile = null;
        var storage = await ProviderInfo.StorageAsync;

        if (file.Id != null)
        {
            newDriveFile = await storage.SaveStreamAsync(MakeDriveId(file.Id), fileStream, file.Title);
        }
        else if (file.ParentId != null)
        {
            newDriveFile = await storage.InsertEntryAsync(fileStream, file.Title, MakeDriveId(file.ParentId));
        }

        await ProviderInfo.CacheResetAsync(newDriveFile);
        var parentDriveId = GetParentDriveId(newDriveFile);
        if (parentDriveId != null)
        {
            await ProviderInfo.CacheResetAsync(parentDriveId, false);
        }

        return ToFile(newDriveFile);
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var driveFile = await GetDriveEntryAsync(fileId);
        if (driveFile == null)
        {
            return;
        }

        var id = MakeId(driveFile.Id);

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

        if (driveFile is not ErrorDriveEntry)
        {
            var storage = await ProviderInfo.StorageAsync;
            await storage.DeleteEntryAsync(driveFile.Id);
        }

        await ProviderInfo.CacheResetAsync(driveFile.Id);
        var parentDriveId = GetParentDriveId(driveFile);
        if (parentDriveId != null)
        {
            await ProviderInfo.CacheResetAsync(parentDriveId, false);
        }
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var entries = await GetDriveEntriesAsync(folderId, false);

        return entries.Any(file => file.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
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

    public async Task<int> MoveFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _googleDriveDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            true)
            ;

        return moved.Id;
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var driveFile = await GetDriveEntryAsync(fileId);
        if (driveFile is ErrorDriveEntry errorDriveEntry)
        {
            throw new Exception(errorDriveEntry.Error);
        }

        var toDriveFolder = await GetDriveEntryAsync(toFolderId);
        if (toDriveFolder is ErrorDriveEntry errorDriveEntry1)
        {
            throw new Exception(errorDriveEntry1.Error);
        }

        var fromFolderDriveId = GetParentDriveId(driveFile);

        var storage = await ProviderInfo.StorageAsync;
        driveFile = await storage.InsertEntryIntoFolderAsync(driveFile, toDriveFolder.Id);
        if (fromFolderDriveId != null)
        {
            await storage.RemoveEntryFromFolderAsync(driveFile, fromFolderDriveId);
        }

        await ProviderInfo.CacheResetAsync(driveFile.Id);
        await ProviderInfo.CacheResetAsync(fromFolderDriveId, false);
        await ProviderInfo.CacheResetAsync(toDriveFolder.Id, false);

        return MakeId(driveFile.Id);
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

    public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _googleDriveDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            false)
            ;

        return moved;
    }

    public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var driveFile = await GetDriveEntryAsync(fileId);
        if (driveFile is ErrorDriveEntry errorDriveEntry)
        {
            throw new Exception(errorDriveEntry.Error);
        }

        var toDriveFolder = await GetDriveEntryAsync(toFolderId);
        if (toDriveFolder is ErrorDriveEntry errorDriveEntry1)
        {
            throw new Exception(errorDriveEntry1.Error);
        }

        var storage = await ProviderInfo.StorageAsync;
        var newDriveFile = await storage.CopyEntryAsync(toDriveFolder.Id, driveFile.Id);

        await ProviderInfo.CacheResetAsync(newDriveFile);
        await ProviderInfo.CacheResetAsync(toDriveFolder.Id, false);

        return ToFile(newDriveFile);
    }

    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var driveFile = await GetDriveEntryAsync(file.Id);
        driveFile.Name = newTitle;

        var storage = await ProviderInfo.StorageAsync;
        driveFile = await storage.RenameEntryAsync(driveFile.Id, driveFile.Name);

        await ProviderInfo.CacheResetAsync(driveFile);
        var parentDriveId = GetParentDriveId(driveFile);
        if (parentDriveId != null)
        {
            await ProviderInfo.CacheResetAsync(parentDriveId, false);
        }

        return MakeId(driveFile.Id);
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
        return ProviderInfo.GetThumbnail(MakeDriveId(_googleDriveDaoSelector.ConvertId(fileId)), width, height);
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


    public async Task<ChunkedUploadSession<string>> InternalCreateUploadSessionAsync(File<string> file, long contentLength)
    {
        var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

        DriveFile driveFile;
        var storageTask = ProviderInfo.StorageAsync;
        GoogleDriveStorage storage;

        if (file.Id != null)
        {
            driveFile = await GetDriveEntryAsync(file.Id);
        }
        else
        {
            var folder = await GetDriveEntryAsync(file.ParentId);
            storage = await storageTask;
            driveFile = storage.FileConstructor(file.Title, null, folder.Id);
        }

        storage = await storageTask;
        var googleDriveSession = await storage.CreateResumableSessionAsync(driveFile, contentLength);
        if (googleDriveSession != null)
        {
            uploadSession.Items["GoogleDriveSession"] = googleDriveSession;
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

        if (uploadSession.Items.ContainsKey("GoogleDriveSession"))
        {
            var googleDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("GoogleDriveSession");
            var storage = await ProviderInfo.StorageAsync;
            storage = await ProviderInfo.StorageAsync;
            await storage.TransferAsync(googleDriveSession, stream, chunkLength, uploadSession.LastChunk);
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
        if (uploadSession.Items.ContainsKey("GoogleDriveSession"))
        {
            var googleDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("GoogleDriveSession");

            await ProviderInfo.CacheResetAsync(googleDriveSession.FileId);
            var parentDriveId = googleDriveSession.FolderId;
            if (parentDriveId != null)
            {
                await ProviderInfo.CacheResetAsync(parentDriveId, false);
            }

            return ToFile(await GetDriveEntryAsync(googleDriveSession.FileId));
        }

        using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"), FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);

        return await SaveFileAsync(uploadSession.File, fs);
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("GoogleDriveSession"))
        {
            var googleDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("GoogleDriveSession");

            if (googleDriveSession.Status != ResumableUploadSessionStatus.Completed)
            {
                googleDriveSession.Status = ResumableUploadSessionStatus.Aborted;
            }

            return Task.CompletedTask;
        }
        else if (uploadSession.Items.ContainsKey("TempPath"))
        {
            File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));

            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    #endregion
}
