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

namespace ASC.Files.Thirdparty.Sharpbox;

[Scope]
internal class SharpBoxFileDao : SharpBoxDaoBase, IFileDao<string>
{
    private readonly TempStream _tempStream;
    private readonly CrossDao _crossDao;
    private readonly SharpBoxDaoSelector _sharpBoxDaoSelector;
    private readonly IFileDao<int> _fileDao;

    public SharpBoxFileDao(
        IServiceProvider serviceProvider,
        TempStream tempStream,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharpBoxDaoBase> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        SharpBoxDaoSelector sharpBoxDaoSelector,
        IFileDao<int> fileDao,
        TempPath tempPath,
        AuthContext authContext,
        RegexDaoSelectorBase<ICloudFileSystemEntry, ICloudDirectoryEntry, ICloudFileSystemEntry> regexDaoSelectorBase)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext, regexDaoSelectorBase)
    {
        _tempStream = tempStream;
        _crossDao = crossDao;
        _sharpBoxDaoSelector = sharpBoxDaoSelector;
        _fileDao = fileDao;
    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        await SharpBoxProviderInfo.InvalidateStorageAsync();
    }

    public async Task<File<string>> GetFileAsync(string fileId)
    {
        return await GetFileAsync(fileId, 1);
    }

    public Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return Task.FromResult(ToFile(GetFileById(fileId)));
    }

    public Task<File<string>> GetFileAsync(string parentId, string title)
    {
        return Task.FromResult(ToFile(GetFolderFiles(parentId).FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase))));
    }

    public Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return Task.FromResult(ToFile(GetFileById(fileId)));
    }

    public async IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
    {
        var file = await GetFileAsync(fileId);
        yield return file;
    }

    public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
    {
        return fileIds.Select(fileId => ToFile(GetFileById(fileId))).ToAsyncEnumerable();
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
            files = files.WhereAwait(async x => subjectGroup
                                         ? await _userManager.IsUserInGroupAsync(x.CreateBy, subjectID)
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
        var folder = GetFolderById(parentId).ToAsyncEnumerable();

        await foreach (var entry in folder.Where(x => x is not ICloudDirectoryEntry))
        {
            yield return MakeId(entry);
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, 
        bool searchInContent, bool withSubfolders = false, bool excludeSubject = false, int offset = 0, int count = -1, string roomId = default)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        //Get only files
        var files = GetFolderById(parentId).Where(x => x is not ICloudDirectoryEntry).Select(ToFile).ToAsyncEnumerable();

        //Filter
        if (subjectID != Guid.Empty)
        {
            files = files.WhereAwait(async x => subjectGroup
                                         ? await _userManager.IsUserInGroupAsync(x.CreateBy, subjectID)
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

        //hack
        await Task.Delay(1);

        await foreach (var f in files)
        {
            yield return f;
        }
    }

    public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
    {
        var fileToDownload = GetFileById(file.Id);

        if (fileToDownload == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (fileToDownload is ErrorEntry errorEntry)
        {
            throw new Exception(errorEntry.Error);
        }

        var fileStream = fileToDownload.GetDataTransferAccessor().GetDownloadStream();

        if (fileStream != null && offset > 0)
        {
            if (!fileStream.CanSeek)
            {
                var tempBuffer = _tempStream.Create();

                await fileStream.CopyToAsync(tempBuffer);
                await tempBuffer.FlushAsync();
                tempBuffer.Seek(offset, SeekOrigin.Begin);

                await fileStream.DisposeAsync();

                return tempBuffer;
            }

            fileStream.Seek(offset, SeekOrigin.Begin);
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

    public override Task<Stream> GetFileStreamAsync(File<string> file)
    {
        return GetFileStreamAsync(file, 0);
    }

    public async Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        ICloudFileSystemEntry entry = null;
        if (file.Id != null)
        {
            entry = SharpBoxProviderInfo.Storage.GetFile(MakePath(file.Id), null);
        }
        else if (file.ParentId != null)
        {
            var folder = GetFolderById(file.ParentId);
            file.Title = await GetAvailableTitleAsync(file.Title, folder, IsExistAsync);
            entry = SharpBoxProviderInfo.Storage.CreateFile(folder, file.Title);
        }

        if (entry == null)
        {
            return null;
        }

        try
        {
            entry.GetDataTransferAccessor().Transfer(_tempStream.GetBuffered(fileStream), nTransferDirection.nUpload);
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

        if (file.Id != null && !entry.Name.Equals(file.Title))
        {
            file.Title = await GetAvailableTitleAsync(file.Title, entry.Parent, IsExistAsync);
            SharpBoxProviderInfo.Storage.RenameFileSystemEntry(entry, file.Title);
        }

        return ToFile(entry);
    }

    public async Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return await SaveFileAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var file = GetFileById(fileId);
        if (file == null)
        {
            return;
        }

        var id = MakeId(file);

        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var filesDbContext = _dbContextFactory.CreateDbContext();
            await using (var tx = await filesDbContext.Database.BeginTransactionAsync())
            {
                await Queries.DeleteTagLinksAsync(filesDbContext, _tenantId, id);
                await Queries.DeleteTagsAsync(filesDbContext);
                await Queries.DeleteSecuritiesAsync(filesDbContext, _tenantId, id);
                await Queries.DeleteThirdpartyIdMappingsAsync(filesDbContext, _tenantId, id);
                await tx.CommitAsync();
            }
        });

        if (file is not ErrorEntry)
        {
            SharpBoxProviderInfo.Storage.DeleteFileSystemEntry(file);
        }
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var folder = SharpBoxProviderInfo.Storage.GetFolder(MakePath(folderId));

        return await IsExistAsync(title, folder);
    }

    public Task<bool> IsExistAsync(string title, ICloudDirectoryEntry folder)
    {
        try
        {
            return Task.FromResult(SharpBoxProviderInfo.Storage.GetFileSystemObject(title, folder) != null);
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

    public async Task<TTo> MoveFileAsync<TTo>(string fileId, TTo toFolderId)
    {
        if (toFolderId is int tId)
        {
            return IdConverter.Convert<TTo>(await MoveFileAsync(fileId, tId));
        }

        if (toFolderId is string tsId)
        {
            return IdConverter.Convert<TTo>(await MoveFileAsync(fileId, tsId));
        }

        throw new NotImplementedException();
    }

    public async Task<int> MoveFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _sharpBoxDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            true)
            ;

        return moved.Id;
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var entry = GetFileById(fileId);
        var folder = GetFolderById(toFolderId);

        var oldFileId = MakeId(entry);

        if (!SharpBoxProviderInfo.Storage.MoveFileSystemEntry(entry, folder))
        {
            throw new Exception("Error while moving");
        }

        var newFileId = MakeId(entry);

        await UpdatePathInDBAsync(oldFileId, newFileId);

        return newFileId;
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

    public Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var file = GetFileById(fileId);
        if (!SharpBoxProviderInfo.Storage.CopyFileSystemEntry(MakePath(fileId), MakePath(toFolderId)))
        {
            throw new Exception("Error while copying");
        }

        return Task.FromResult(ToFile(GetFolderById(toFolderId).FirstOrDefault(x => x.Name == file.Name)));
    }

    public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _sharpBoxDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            false)
            ;

        return moved;
    }

    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var entry = GetFileById(file.Id);

        if (entry == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        var oldFileId = MakeId(entry);
        var newFileId = oldFileId;

        var folder = GetFolderById(file.ParentId);
        newTitle = await GetAvailableTitleAsync(newTitle, folder, IsExistAsync);

        if (SharpBoxProviderInfo.Storage.RenameFileSystemEntry(entry, newTitle))
        {
            //File data must be already updated by provider
            //We can't search google files by title because root can have multiple folders with the same name
            //var newFile = SharpBox_providerInfo.Storage.GetFileSystemObject(newTitle, file.Parent);
            newFileId = MakeId(entry);
        }

        await UpdatePathInDBAsync(oldFileId, newFileId);

        return newFileId;
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

    #region chunking

    public async Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        if (_setupInfo.ChunkUploadSize > contentLength && contentLength != -1)
        {
            return new ChunkedUploadSession<string>(MakeId(file), contentLength) { UseChunks = false };
        }

        var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

        var isNewFile = false;

        ICloudFileSystemEntry sharpboxFile;
        if (file.Id != null)
        {
            sharpboxFile = GetFileById(file.Id);
        }
        else
        {
            var folder = GetFolderById(file.ParentId);
            sharpboxFile = SharpBoxProviderInfo.Storage.CreateFile(folder, await GetAvailableTitleAsync(file.Title, folder, IsExistAsync));
            isNewFile = true;
        }

        var sharpboxSession = sharpboxFile.GetDataTransferAccessor().CreateResumableSession(contentLength);
        if (sharpboxSession != null)
        {
            uploadSession.Items["SharpboxSession"] = sharpboxSession;
            uploadSession.Items["IsNewFile"] = isNewFile;
        }
        else
        {
            uploadSession.Items["TempPath"] = _tempPath.GetTempFileName();
        }

        uploadSession.File = MakeId(uploadSession.File);

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

        if (uploadSession.Items.ContainsKey("SharpboxSession"))
        {
            var sharpboxSession =
                uploadSession.GetItemOrDefault<AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects.ResumableUploadSession>("SharpboxSession");

            var isNewFile = uploadSession.Items.ContainsKey("IsNewFile") && uploadSession.GetItemOrDefault<bool>("IsNewFile");
            var sharpboxFile =
                isNewFile
                    ? SharpBoxProviderInfo.Storage.CreateFile(GetFolderById(sharpboxSession.ParentId), sharpboxSession.FileName)
                    : GetFileById(sharpboxSession.FileId);

            sharpboxFile.GetDataTransferAccessor().Transfer(sharpboxSession, stream, chunkLength);
        }
        else
        {
            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            await using var fs = new FileStream(tempPath, FileMode.Append);
            await stream.CopyToAsync(fs);
        }

        uploadSession.BytesUploaded += chunkLength;

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal || uploadSession.LastChunk)
        {
            uploadSession.File = await FinalizeUploadSessionAsync(uploadSession);
        }
        else
        {
            uploadSession.File = MakeId(uploadSession.File);
        }

        return uploadSession.File;
    }

    public async Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("SharpboxSession"))
        {
            var sharpboxSession =
                uploadSession.GetItemOrDefault<AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects.ResumableUploadSession>("SharpboxSession");

            return ToFile(GetFileById(sharpboxSession.FileId));
        }

        await using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"), FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);

        return await SaveFileAsync(uploadSession.File, fs);
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("SharpboxSession"))
        {
            var sharpboxSession =
                uploadSession.GetItemOrDefault<AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects.ResumableUploadSession>("SharpboxSession");

            var isNewFile = uploadSession.Items.ContainsKey("IsNewFile") && uploadSession.GetItemOrDefault<bool>("IsNewFile");
            var sharpboxFile =
                isNewFile
                    ? SharpBoxProviderInfo.Storage.CreateFile(GetFolderById(sharpboxSession.ParentId), sharpboxSession.FileName)
                    : GetFileById(sharpboxSession.FileId);
            sharpboxFile.GetDataTransferAccessor().AbortResumableSession(sharpboxSession);

            return Task.FromResult(0);
        }
        else if (uploadSession.Items.ContainsKey("TempPath"))
        {
            File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));

            return Task.FromResult(0);
        }

        return Task.FromResult(0);
    }

    private File<string> MakeId(File<string> file)
    {
        if (file.Id != null)
        {
            file.Id = PathPrefix + "-" + file.Id;
        }

        if (file.ParentId != null)
        {
            file.ParentId = PathPrefix + "-" + file.ParentId;
        }

        return file;
    }
    #endregion
}

static file class Queries
{
    public static readonly Func<FilesDbContext, int, string, Task<int>>
        DeleteTagLinksAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string idStart) =>
                ctx.TagLink
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => ctx.ThirdpartyIdMapping
                        .Where(m => m.TenantId == tenantId)
                        .Where(m => m.Id.StartsWith(idStart))
                        .Select(m => m.HashId).Any(h => h == r.EntryId))
                        .ExecuteDelete());

    public static readonly Func<FilesDbContext, Task<int>> DeleteTagsAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx) =>
                (from ft in ctx.Tag
                join ftl in ctx.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new
                {
                    TenantId = ftl.TenantId,
                    Id = ftl.TagId
                }
                where ftl == null
                select ft)
                .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, string, Task<int>> DeleteSecuritiesAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string idStart) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => ctx.ThirdpartyIdMapping
                        .Where(m => m.TenantId == tenantId)
                        .Where(m => m.Id.StartsWith(idStart))
                        .Select(m => m.HashId).Any(h => h == r.EntryId))
                        .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, string, Task<int>>
        DeleteThirdpartyIdMappingsAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string idStart) =>
                ctx.ThirdpartyIdMapping
                    .Where(r => r.TenantId == tenantId)
                    .Where(m => m.Id.StartsWith(idStart))
                    .ExecuteDelete());
}
