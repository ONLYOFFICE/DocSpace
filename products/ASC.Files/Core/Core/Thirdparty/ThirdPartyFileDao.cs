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
[Scope]
internal abstract class ThirdPartyFileDao<TFile, TFolder, TItem> : IFileDao<string>
    where TFile : class, TItem
    where TFolder : class, TItem
    where TItem : class
{
    internal IDaoBase<TFile, TFolder, TItem> Dao { get; }
    internal IProviderInfo<TFile, TFolder, TItem> ProviderInfo { get; private set; }

    private readonly UserManager _userManager;
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly IDaoSelector<TFile, TFolder, TItem> _daoSelector;
    private readonly CrossDao _crossDao;
    private readonly IFileDao<int> _fileDao;
    private readonly int _tenantId;

    public ThirdPartyFileDao(UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        IDaoSelector<TFile, TFolder, TItem> daoSelector,
        CrossDao crossDao,
        IFileDao<int> fileDao,
        IDaoBase<TFile, TFolder, TItem> dao,
        TenantManager tenantManager)
    {
        _userManager = userManager;
        _dbContextFactory = dbContextFactory;
        _daoSelector = daoSelector;
        _crossDao = crossDao;
        _fileDao = fileDao;
        Dao = dao;
        _tenantId = tenantManager.GetCurrentTenant().Id;
    }

    public void Init(string pathPrefix, IProviderInfo<TFile, TFolder, TItem> providerInfo)
    {
        Dao.Init(pathPrefix, providerInfo);
        ProviderInfo = providerInfo;
    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        var thirdFileId = Dao.MakeThirdId(fileId);
        await ProviderInfo.CacheResetAsync(thirdFileId, true);

        var thirdFile = await Dao.GetFileAsync(fileId);
        var parentId = Dao.GetParentFolderId(thirdFile);

        if (parentId != null)
        {
            await ProviderInfo.CacheResetAsync(parentId);
        }
    }

    public Task<File<string>> GetFileAsync(string fileId)
    {
        return GetFileAsync(fileId, 1);
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return Dao.ToFile(await Dao.GetFileAsync(fileId));
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var items = await Dao.GetItemsAsync(parentId, false);

        return Dao.ToFile(items.FirstOrDefault(item => Dao.GetName(item).Equals(title, StringComparison.InvariantCultureIgnoreCase)) as TFile);
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return Dao.ToFile(await Dao.GetFileAsync(fileId));
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
            yield return Dao.ToFile(await Dao.GetFileAsync(fileId));
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
        var items = await Dao.GetItemsAsync(parentId, false);

        foreach (var item in items)
        {
            yield return Dao.MakeId(Dao.GetId(item));
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
        var filesWait = await Dao.GetItemsAsync(parentId, false);
        var files = filesWait.Select(item => Dao.ToFile(item as TFile));

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
        var fileId = Dao.MakeThirdId(file.Id);
        await ProviderInfo.CacheResetAsync(fileId, true);

        var thirdFile = await Dao.GetFileAsync(file.Id);
        if (thirdFile == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (thirdFile is IErrorItem errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var storage = await ProviderInfo.StorageAsync;
        var fileStream = await storage.DownloadStreamAsync(thirdFile, (int)offset);

        return fileStream;
    }

    public Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
    {
        return Task.FromResult(false);
    }

    public async Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(fileStream);

        TFile newFile = null;
        var storage = await ProviderInfo.StorageAsync;

        if (file.Id != null)
        {
            var fileId = Dao.MakeThirdId(file.Id);
            newFile = await storage.SaveStreamAsync(fileId, fileStream);

            if (!Dao.GetName(newFile).Equals(file.Title))
            {
                var folderId = Dao.GetParentFolderId(await Dao.GetFileAsync(fileId));
                file.Title = await Dao.GetAvailableTitleAsync(file.Title, folderId, IsExistAsync);
                newFile = await storage.RenameFileAsync(fileId, file.Title);
            }
        }
        else if (file.ParentId != null)
        {
            var folderId = Dao.MakeThirdId(file.ParentId);
            file.Title = await Dao.GetAvailableTitleAsync(file.Title, folderId, IsExistAsync);
            newFile = await storage.CreateFileAsync(fileStream, file.Title, folderId);
        }

        await ProviderInfo.CacheResetAsync(Dao.GetId(newFile));
        var parentId = Dao.GetParentFolderId(newFile);
        if (parentId != null)
        {
            await ProviderInfo.CacheResetAsync(parentId);
        }

        return Dao.ToFile(newFile);
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var item = await Dao.GetItemsAsync(folderId.ToString(), false);

        return item.Any(item => Dao.GetName(item).Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var file = await Dao.GetFileAsync(fileId);
        if (file == null)
        {
            return;
        }

        var id = Dao.MakeId(Dao.GetId(file));

        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var filesDbContext = _dbContextFactory.CreateDbContext();
            await using (var tx = await filesDbContext.Database.BeginTransactionAsync())
            {
                await Queries.DeleteTagLinksAsync(filesDbContext, _tenantId, id);
                await Queries.DeleteTagsAsync(filesDbContext);
                await Queries.DeleteFilesSecuritiesAsync(filesDbContext, _tenantId, id);
                await Queries.DeleteThirdpartyIdMappingsAsync(filesDbContext, _tenantId, id);

                await tx.CommitAsync();
            }
        });

        if (file is not IErrorItem)
        {
            var storage = await ProviderInfo.StorageAsync;
            await storage.DeleteItemAsync(file);
        }

        await ProviderInfo.CacheResetAsync(Dao.GetId(file), true);
        var parentFolderId = Dao.GetParentFolderId(file);
        if (parentFolderId != null)
        {
            await ProviderInfo.CacheResetAsync(parentFolderId);
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
        var file = await Dao.GetFileAsync(fileId);
        if (file is IErrorItem errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var toFolder = await Dao.GetFolderAsync(toFolderId);
        if (toFolder is IErrorItem errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var fromFolderId = Dao.GetParentFolderId(file);

        var newTitle = await Dao.GetAvailableTitleAsync(Dao.GetName(file), Dao.GetId(toFolder), IsExistAsync);
        var storage = await ProviderInfo.StorageAsync;
        file = await storage.MoveFileAsync(Dao.GetId(file), newTitle, Dao.GetId(toFolder));

        await ProviderInfo.CacheResetAsync(Dao.GetId(file), true);
        await ProviderInfo.CacheResetAsync(Dao.GetId(toFolder));
        await ProviderInfo.CacheResetAsync(Dao.GetId(toFolder));

        return Dao.MakeId(Dao.GetId(file));
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
        var file = await Dao.GetFileAsync(fileId);
        if (file is IErrorItem errorFile)
        {
            throw new Exception(errorFile.Error);
        }

        var toFolder = await Dao.GetFolderAsync(toFolderId);
        if (toFolder is IErrorItem errorFolder)
        {
            throw new Exception(errorFolder.Error);
        }

        var newTitle = await Dao.GetAvailableTitleAsync(Dao.GetName(file), Dao.GetId(toFolder), IsExistAsync);
        var storage = await ProviderInfo.StorageAsync;
        var newFile = await storage.CopyFileAsync(Dao.GetId(file), newTitle, Dao.GetId(toFolder));

        await ProviderInfo.CacheResetAsync(Dao.GetId(newFile));
        await ProviderInfo.CacheResetAsync(Dao.GetId(toFolder));

        return Dao.ToFile(newFile);
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
        var thirdFile = await Dao.GetFileAsync(file.Id);
        newTitle = await Dao.GetAvailableTitleAsync(newTitle, Dao.GetParentFolderId(thirdFile), IsExistAsync);

        var storage = await ProviderInfo.StorageAsync;
        thirdFile = await storage.RenameFileAsync(Dao.GetId(thirdFile), newTitle);

        await ProviderInfo.CacheResetAsync(Dao.GetId(thirdFile));
        var parentId = Dao.GetParentFolderId(thirdFile);
        if (parentId != null)
        {
            await ProviderInfo.CacheResetAsync(parentId);
        }

        return Dao.MakeId(Dao.GetId(thirdFile));
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
        var thirdFileId = Dao.MakeThirdId(_daoSelector.ConvertId(fileId));

        var storage = await ProviderInfo.StorageAsync;
        return await storage.GetThumbnailAsync(thirdFileId, width, height);
    }

    internal File<string> RestoreIds(File<string> file)
    {
        if (file == null)
        {
            return null;
        }

        if (file.Id != null)
        {
            file.Id = Dao.MakeId(file.Id);
        }

        if (file.ParentId != null)
        {
            file.ParentId = Dao.MakeId(file.ParentId);
        }

        return file;
    }

    public abstract Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength);

    public abstract Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength);

    public abstract Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession);

    public abstract Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession);

    public Task ReassignFilesAsync(Guid oldOwner, Guid newOwnerId, IEnumerable<string> exceptFolderIds)
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

    public Task<int> GetFilesCountAsync(string parentId, FilterType filterType, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders = false, 
        bool excludeSubject = false, string roomId = default)
    {
        throw new NotImplementedException();
    }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, Task<int>> DeleteTagsAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
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

    public static readonly Func<FilesDbContext, int, string, Task<int>> DeleteTagLinksAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string idStart) =>
                ctx.TagLink
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => ctx.ThirdpartyIdMapping
                        .Where(t => t.TenantId == tenantId)
                        .Where(t => t.Id.StartsWith(idStart))
                        .Select(t => t.HashId).Any(h => h == r.EntryId))
                        .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, string, Task<int>> DeleteFilesSecuritiesAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string idStart) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => ctx.ThirdpartyIdMapping
                        .Where(t => t.TenantId == tenantId)
                        .Where(t => t.Id.StartsWith(idStart))
                        .Select(t => t.HashId).Any(h => h == r.EntryId))
                        .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, string, Task<int>>
        DeleteThirdpartyIdMappingsAsync =
            Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
                (FilesDbContext ctx, int tenantId, string idStart) =>
                    ctx.ThirdpartyIdMapping
                        .Where(r => r.TenantId == tenantId)
                        .Where(t => t.Id.StartsWith(idStart))
                        .ExecuteDelete());
}
