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

namespace ASC.Files.Core.Core.Thirdparty;
internal class ThirdPartyFileDao<TFile, TFolder, TItem>: IFileDao<string>
    where TFile : class, TItem
    where TFolder : class, TItem
    where TItem: class
{
    private readonly IDaoBase<TFile, TFolder, TItem> _dao;
    private readonly IProviderInfo<TFile, TFolder, TItem> _providerInfo;

    private readonly UserManager _userManager;
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly IDaoSelector _daoSelector;
    private readonly CrossDao _crossDao;
    private readonly IFileDao<int> _fileDao;
    private readonly SetupInfo _setupInfo;
    private readonly TempPath _tempPath;

    public ThirdPartyFileDao(UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        IDaoSelector daoSelector,
        CrossDao crossDao,
        IFileDao<int> fileDao, 
        SetupInfo setupInfo,
        TempPath tempPath)
    {
        _userManager = userManager;
        _dbContextFactory = dbContextFactory;
        _daoSelector = daoSelector;
        _crossDao = crossDao;
        _fileDao = fileDao;
        _setupInfo = setupInfo;
        _tempPath = tempPath;
    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        var thirdFileId = _dao.MakeId(fileId);
        await _providerInfo.CacheResetAsync(thirdFileId, true);

        var thirdFile = await _dao.GetFileAsync(fileId);
        var parentId = _dao.GetParentFolderId(thirdFile);

        if (parentId != null)
        {
            await _providerInfo.CacheResetAsync(parentId);
        }
    }

    public Task<File<string>> GetFileAsync(string fileId)
    {
        return GetFileAsync(fileId, 1);
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return _dao.ToFile(await _dao.GetFileAsync(fileId));
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var items = await _dao.GetItemsAsync(parentId, false);

        return _dao.ToFile(items.FirstOrDefault(item => _dao.GetName(item).Equals(title, StringComparison.InvariantCultureIgnoreCase)) as TFile);
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return _dao.ToFile(await _dao.GetFileAsync(fileId));
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
            yield return _dao.ToFile(await _dao.GetFileAsync(fileId));
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
        var items = await _dao.GetItemsAsync(parentId, false);

        foreach (var item in items)
        {
            yield return _dao.MakeId(_dao.GetId(item));
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false, bool excludeSubject = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        //Get only files
        var filesWait = await _dao.GetItemsAsync(parentId, false);
        var files = filesWait.Select(item => _dao.ToFile(item as TFile));

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

    public Task<Stream> GetFileStreamAsync(File<string> file)
    {
        return GetFileStreamAsync(file, 0);
    }

    public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
    {
        var fileId = _dao.MakeId(file.Id);
        await _providerInfo.CacheResetAsync(fileId, true);

        var thirdFile = await _dao.GetFileAsync(file.Id);
        if (thirdFile == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (thirdFile is IErrorItem errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var storage = await _providerInfo.StorageAsync;
        var fileStream = await storage.DownloadStreamAsync(thirdFile, (int)offset);

        return fileStream;
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
        TFile newFile = null;
        var storage = await _providerInfo.StorageAsync;

        if (file.Id != null)
        {
            var fileId = _dao.MakeId(file.Id);
            newFile = await storage.SaveStreamAsync(fileId, fileStream);

            if (!_dao.GetName(newFile).Equals(file.Title))
            {
                var folderId = _dao.GetParentFolderId(await _dao.GetFileAsync(fileId));
                file.Title = await _dao.GetAvailableTitleAsync(file.Title, folderId, IsExistAsync);
                newFile = await storage.RenameFileAsync(fileId, file.Title);
            }
        }
        else if (file.ParentId != null)
        {
            var folderId = _dao.MakeId(file.ParentId);
            file.Title = await _dao.GetAvailableTitleAsync(file.Title, folderId, IsExistAsync);
            newFile = await storage.CreateFileAsync(fileStream, file.Title, folderId);
        }

        await _providerInfo.CacheResetAsync(_dao.GetId(newFile));
        var parentId = _dao.GetParentFolderId(newFile);
        if (parentId != null)
        {
            await _providerInfo.CacheResetAsync(parentId);
        }

        return _dao.ToFile(newFile);
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var item = await _dao.GetItemsAsync(folderId.ToString(), false);

        return item.Any(item => _dao.GetName(item).Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var file = await _dao.GetFileAsync(fileId);
        if (file == null)
        {
            return;
        }

        var id = _dao.MakeId(_dao.GetId(file));

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using (var tx = await filesDbContext.Database.BeginTransactionAsync())
            {
                var hashIDs = _dao.Query(filesDbContext.ThirdpartyIdMapping)
                .Where(r => r.Id.StartsWith(id))
                .Select(r => r.HashId);

                var link = await _dao.Query(filesDbContext.TagLink)
                .Where(r => hashIDs.Any(h => h == r.EntryId))
                .ToListAsync();

                filesDbContext.TagLink.RemoveRange(link);
                await filesDbContext.SaveChangesAsync();

                var tagsToRemove = from ft in filesDbContext.Tag
                                   join ftl in filesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                                   where ftl == null
                                   select ft;

                filesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

                var securityToDelete = _dao.Query(filesDbContext.Security)
                .Where(r => hashIDs.Any(h => h == r.EntryId));

                filesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
                await filesDbContext.SaveChangesAsync();

                var mappingToDelete = _dao.Query(filesDbContext.ThirdpartyIdMapping)
                .Where(r => hashIDs.Any(h => h == r.HashId));

                filesDbContext.ThirdpartyIdMapping.RemoveRange(await mappingToDelete.ToListAsync());
                await filesDbContext.SaveChangesAsync();

                await tx.CommitAsync();
            }
        });

        if (file is not IErrorItem)
        {
            var storage = await _providerInfo.StorageAsync;
            await storage.DeleteItemAsync(file);
        }

        await _providerInfo.CacheResetAsync(_dao.GetId(file), true);
        var parentFolderId = _dao.GetParentFolderId(file);
        if (parentFolderId != null)
        {
            await _providerInfo.CacheResetAsync(parentFolderId);
        }
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
            fileId, this, _daoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            true);

        return moved.Id;
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var file = await _dao.GetFileAsync(fileId);
        if (file is IErrorItem errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var toFolder = await _dao.GetFolderAsync(toFolderId);
        if (toFolder is IErrorItem errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var fromFolderId = _dao.GetParentFolderId(file);

        var newTitle = await _dao.GetAvailableTitleAsync(_dao.GetName(file), _dao.GetId(toFolder), IsExistAsync);
        var storage = await _providerInfo.StorageAsync;
        file = await storage.MoveFileAsync(_dao.GetId(file), newTitle, _dao.GetId(toFolder));

        await _providerInfo.CacheResetAsync(_dao.GetId(file), true);
        await _providerInfo.CacheResetAsync(_dao.GetId(toFolder));
        await _providerInfo.CacheResetAsync(_dao.GetId(toFolder));

        return _dao.MakeId(_dao.GetId(file));
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

    public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var file = await _dao.GetFileAsync(fileId);
        if (file is IErrorItem errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var toFolder = await _dao.GetFolderAsync(toFolderId);
        if (toFolder is IErrorItem errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var newTitle = await _dao.GetAvailableTitleAsync(_dao.GetName(file), _dao.GetId(toFolder), IsExistAsync);
        var storage = await _providerInfo.StorageAsync;
        var newFile = await storage.CopyFileAsync(_dao.GetId(file), newTitle, _dao.GetId(toFolder));

        await _providerInfo.CacheResetAsync(_dao.GetId(newFile));
        await _providerInfo.CacheResetAsync(_dao.GetId(toFolder));

        return _dao.ToFile(newFile);
    }

    public Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        var moved = _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _daoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            false);

        return moved;
    }

    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var thirdFile = await _dao.GetFileAsync(file.Id);
        newTitle = await _dao.GetAvailableTitleAsync(newTitle, _dao.GetParentFolderId(thirdFile), IsExistAsync);

        var storage = await _providerInfo.StorageAsync;
        thirdFile = await storage.RenameFileAsync(_dao.GetId(thirdFile), newTitle);

        await _providerInfo.CacheResetAsync(_dao.GetId(thirdFile));
        var parentId = _dao.GetParentFolderId(thirdFile);
        if (parentId != null)
        {
            await _providerInfo.CacheResetAsync(parentId);
        }

        return _dao.MakeId(_dao.GetId(thirdFile));
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

    public async Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        var thirdFileId = _dao.MakeId(_daoSelector.ConvertId(fileId));

        var storage = await _providerInfo.StorageAsync;
        return await storage.GetThumbnailAsync(thirdFileId, width, height);
    }
    
    private File<string> RestoreIds(File<string> file)
    {
        if (file == null)
        {
            return null;
        }

        if (file.Id != null)
        {
            file.Id = _dao.MakeId(file.Id);
        }

        if (file.ParentId != null)
        {
            file.ParentId = _dao.MakeId(file.ParentId);
        }

        return file;
    }

    public virtual Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        throw new NotImplementedException();
    }

    public virtual Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength)
    {
        throw new NotImplementedException();
    }

    public virtual Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        throw new NotImplementedException();
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("TempPath"))
        {
            System.IO.File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
        }

        return Task.CompletedTask;
    }



    public Task ReassignFilesAsync(string[] fileIds, Guid newOwnerId)
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
    {
        return AsyncEnumerable.Empty<File<string>>();
    }

    public IAsyncEnumerable<File<string>> SearchAsync(string text, bool bunch)
    {
        return null;
    }

    public Task<bool> IsExistOnStorageAsync(File<string> file)
    {
        return Task.FromResult(true);
    }

    public Task SaveEditHistoryAsync(File<string> file, string changes, Stream differenceStream)
    {
        //Do nothing
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<EditHistory> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, string fileId, int fileVersion)
    {
        return null;
    }

    public Task<Stream> GetDifferenceStreamAsync(File<string> file)
    {
        return null;
    }

    public Task<bool> ContainChangesAsync(string fileId, int fileVersion)
    {
        return Task.FromResult(false);
    }

    public string GetUniqThumbnailPath(File<string> file, int width, int height)
    {
        //Do nothing
        return null;
    }

    public Task SetThumbnailStatusAsync(File<string> file, Thumbnail status)
    {
        return Task.CompletedTask;
    }

    public Task<Stream> GetThumbnailAsync(File<string> file, int width, int height)
    {
        return GetThumbnailAsync(file.Id, width, height);
    }

    public Task<EntryProperties> GetProperties(string fileId)
    {
        return Task.FromResult<EntryProperties>(null);
    }

    public Task SaveProperties(string fileId, EntryProperties entryProperties)
    {
        return null;
    }
    
    public string GetUniqFilePath(File<string> file, string fileTitle)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FileWithShare> GetFeedsAsync(int tenant, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<int> GetTenantsWithFeedsAsync(DateTime fromTime, bool includeSecurity)
    {
        throw new NotImplementedException();
    }

    public Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
    {
        throw new NotImplementedException();
    }
}
