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

using Document = ASC.ElasticSearch.Document;

namespace ASC.Files.Core.Data;

[Scope]
internal class FileDao : AbstractDao, IFileDao<int>
{
    private static readonly object _syncRoot = new object();
    private readonly ILogger<FileDao> _logger;
    private readonly FactoryIndexerFile _factoryIndexer;
    private readonly GlobalStore _globalStore;
    private readonly GlobalSpace _globalSpace;
    private readonly GlobalFolder _globalFolder;
    private readonly Global _global;
    private readonly IDaoFactory _daoFactory;
    private readonly ChunkedUploadSessionHolder _chunkedUploadSessionHolder;
    private readonly ProviderFolderDao _providerFolderDao;
    private readonly CrossDao _crossDao;
    private readonly Settings _settings;
    private readonly IMapper _mapper;
    private readonly ThumbnailSettings _thumbnailSettings;

    public FileDao(
        ILogger<FileDao> logger,
        FactoryIndexerFile factoryIndexer,
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticProvider,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache,
        GlobalStore globalStore,
        GlobalSpace globalSpace,
        GlobalFolder globalFolder,
        Global global,
        IDaoFactory daoFactory,
        ChunkedUploadSessionHolder chunkedUploadSessionHolder,
        ProviderFolderDao providerFolderDao,
        CrossDao crossDao,
        Settings settings,
        IMapper mapper,
        ThumbnailSettings thumbnailSettings)
        : base(
              dbContextManager,
              userManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              tenantExtra,
              tenantStatisticProvider,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache)
    {
        _logger = logger;
        _factoryIndexer = factoryIndexer;
        _globalStore = globalStore;
        _globalSpace = globalSpace;
        _globalFolder = globalFolder;
        _global = global;
        _daoFactory = daoFactory;
        _chunkedUploadSessionHolder = chunkedUploadSessionHolder;
        _providerFolderDao = providerFolderDao;
        _crossDao = crossDao;
        _settings = settings;
        _mapper = mapper;
        _thumbnailSettings = thumbnailSettings;
    }

    public Task InvalidateCacheAsync(int fileId)
    {
        return Task.CompletedTask;
    }

    public async Task<File<int>> GetFileAsync(int fileId)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        var query = GetFileQuery(filesDbContext, r => r.Id == fileId && r.CurrentVersion).AsNoTracking();

        var dbFile = await FromQueryWithShared(filesDbContext, query)
            .SingleOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public async Task<File<int>> GetFileAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        var query = GetFileQuery(filesDbContext, r => r.Id == fileId && r.Version == fileVersion).AsNoTracking();

        var dbFile = await FromQueryWithShared(filesDbContext, query)
            .SingleOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public Task<File<int>> GetFileAsync(int parentId, string title)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(title);

        return InternalGetFileAsync(parentId, title);
    }

    private async Task<File<int>> InternalGetFileAsync(int parentId, string title)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var query = GetFileQuery(filesDbContext, r => r.Title == title && r.CurrentVersion && r.ParentId == parentId)
            .AsNoTracking()
            .OrderBy(r => r.CreateOn);

        var dbFile = await FromQueryWithShared(filesDbContext, query).FirstOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public async Task<File<int>> GetFileStableAsync(int fileId, int fileVersion = -1)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        var query = GetFileQuery(filesDbContext, r => r.Id == fileId && r.Forcesave == ForcesaveType.None)
            .AsNoTracking();

        if (fileVersion >= 0)
        {
            query = query.Where(r => r.Version <= fileVersion);
        }

        query = query.OrderByDescending(r => r.Version);

        var dbFile = await FromQueryWithShared(filesDbContext, query).FirstOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public IAsyncEnumerable<File<int>> GetFileHistoryAsync(int fileId)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => r.Id == fileId).OrderByDescending(r => r.Version).AsNoTracking();

        return FromQueryWithShared(filesDbContext, query).AsAsyncEnumerable()
            .Select(_mapper.Map<DbFileQuery, File<int>>);
    }

    public IAsyncEnumerable<File<int>> GetFilesAsync(IEnumerable<int> fileIds)
    {
        if (fileIds == null || !fileIds.Any())
        {
            return AsyncEnumerable.Empty<File<int>>();
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => fileIds.Contains(r.Id) && r.CurrentVersion)
            .AsNoTracking();

        return FromQueryWithShared(filesDbContext, query).AsAsyncEnumerable()
            .Select(e => _mapper.Map<DbFileQuery, File<int>>(e));
    }

    public IAsyncEnumerable<File<int>> GetFilesFilteredAsync(IEnumerable<int> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
    {
        if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly)
        {
            return AsyncEnumerable.Empty<File<int>>();
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => fileIds.Contains(r.Id) && r.CurrentVersion).AsNoTracking();

        if (!string.IsNullOrEmpty(searchText))
        {
            var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

            if (_factoryIndexer.TrySelectIds(s => func(s).In(r => r.Id, fileIds.ToArray()), out var searchIds))
            {
                query = query.Where(r => searchIds.Contains(r.Id));
            }
            else
            {
                query = BuildSearch(query, searchText, SearhTypeEnum.Any);
            }
        }

        if (subjectID != Guid.Empty)
        {
            if (subjectGroup)
            {
                var users = _userManager.GetUsersByGroup(subjectID).Select(u => u.Id).ToArray();
                query = query.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                query = query.Where(r => r.CreateBy == subjectID);
            }
        }

        switch (filterType)
        {
            case FilterType.DocumentsOnly:
            case FilterType.ImagesOnly:
            case FilterType.PresentationsOnly:
            case FilterType.SpreadsheetsOnly:
            case FilterType.ArchiveOnly:
            case FilterType.MediaOnly:
                query = query.Where(r => r.Category == (int)filterType);
                break;
            case FilterType.ByExtension:
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = BuildSearch(query, searchText, SearhTypeEnum.End);
                }
                break;
        }

        return (checkShared ? FromQueryWithShared(filesDbContext, query) : FromQuery(filesDbContext, query)).AsAsyncEnumerable()
            .Select(e => _mapper.Map<DbFileQuery, File<int>>(e));
    }

    public async Task<List<int>> GetFilesAsync(int parentId)
    {
        var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        return await Query(filesDbContext.Files)
            .AsNoTracking()
            .Where(r => r.ParentId == parentId && r.CurrentVersion)
            .Select(r => r.Id)
            .ToListAsync()
;
    }

    public IAsyncEnumerable<File<int>> GetFilesAsync(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            return AsyncEnumerable.Empty<File<int>>();
        }

        if (orderBy == null)
        {
            orderBy = new OrderBy(SortedByType.DateAndTime, false);
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetFileQuery(filesDbContext, r => r.ParentId == parentId && r.CurrentVersion).AsNoTracking();


        if (withSubfolders)
        {
            q = GetFileQuery(filesDbContext, r => r.CurrentVersion)
                .AsNoTracking()
                .Join(filesDbContext.Tree, r => r.ParentId, a => a.FolderId, (file, tree) => new { file, tree })
                .Where(r => r.tree.ParentId == parentId)
                .Select(r => r.file);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            var func = GetFuncForSearch(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders);

            Expression<Func<Selector<DbFile>, Selector<DbFile>>> expression = s => func(s);

            if (_factoryIndexer.TrySelectIds(expression, out var searchIds))
            {
                q = q.Where(r => searchIds.Contains(r.Id));
            }
            else
            {
                q = BuildSearch(q, searchText, SearhTypeEnum.Any);
            }
        }

        q = orderBy.SortedBy switch
        {
            SortedByType.Author => orderBy.IsAsc ? q.OrderBy(r => r.CreateBy) : q.OrderByDescending(r => r.CreateBy),
            SortedByType.Size => orderBy.IsAsc ? q.OrderBy(r => r.ContentLength) : q.OrderByDescending(r => r.ContentLength),
            SortedByType.AZ => orderBy.IsAsc ? q.OrderBy(r => r.Title) : q.OrderByDescending(r => r.Title),
            SortedByType.DateAndTime => orderBy.IsAsc ? q.OrderBy(r => r.ModifiedOn) : q.OrderByDescending(r => r.ModifiedOn),
            SortedByType.DateAndTimeCreation => orderBy.IsAsc ? q.OrderBy(r => r.CreateOn) : q.OrderByDescending(r => r.CreateOn),
            _ => q.OrderBy(r => r.Title),
        };
        if (subjectID != Guid.Empty)
        {
            if (subjectGroup)
            {
                var users = _userManager.GetUsersByGroup(subjectID).Select(u => u.Id).ToArray();
                q = q.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                q = q.Where(r => r.CreateBy == subjectID);
            }
        }

        switch (filterType)
        {
            case FilterType.DocumentsOnly:
            case FilterType.ImagesOnly:
            case FilterType.PresentationsOnly:
            case FilterType.SpreadsheetsOnly:
            case FilterType.ArchiveOnly:
            case FilterType.MediaOnly:
                q = q.Where(r => r.Category == (int)filterType);
                break;
            case FilterType.ByExtension:
                if (!string.IsNullOrEmpty(searchText))
                {
                    q = BuildSearch(q, searchText, SearhTypeEnum.End);
                }
                break;
        }

        return FromQueryWithShared(filesDbContext, q).AsAsyncEnumerable()
            .Select(_mapper.Map<DbFileQuery, File<int>>);
    }

    public Task<Stream> GetFileStreamAsync(File<int> file, long offset)
    {
        return _globalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file), (int)offset);
    }

    public Task<Uri> GetPreSignedUriAsync(File<int> file, TimeSpan expires)
    {
        return _globalStore.GetStore().GetPreSignedUriAsync(string.Empty, GetUniqFilePath(file), expires,
                                                 new List<string>
                                                     {
                                                             string.Concat("Content-Disposition:", ContentDispositionUtil.GetHeaderValue(file.Title, withoutBase: true))
                                                     });
    }

    public Task<bool> IsSupportedPreSignedUriAsync(File<int> file)
    {
        return Task.FromResult(_globalStore.GetStore().IsSupportedPreSignedUri);
    }

    public Task<Stream> GetFileStreamAsync(File<int> file)
    {
        return _globalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file), 0);
    }

    public Task<File<int>> SaveFileAsync(File<int> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream, true);
    }

    public Task<File<int>> SaveFileAsync(File<int> file, Stream fileStream, bool checkQuota = true)
    {
        ArgumentNullException.ThrowIfNull(file);

        var maxChunkedUploadSize = _setupInfo.MaxChunkedUploadSize(_tenantExtra, _tenantStatisticProvider);
        if (checkQuota && maxChunkedUploadSize < file.ContentLength)
        {
            throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
        }

        return InternalSaveFileAsync(file, fileStream, checkQuota);
    }

    private async Task<File<int>> InternalSaveFileAsync(File<int> file, Stream fileStream, bool checkQuota = true)
    {
        if (checkQuota && _coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            var personalMaxSpace = _coreConfiguration.PersonalMaxSpace(_settingsManager);
            if (personalMaxSpace - await _globalSpace.GetUserUsedSpaceAsync(file.Id == default ? _authContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
            {
                throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
            }
        }

        var isNew = false;
        List<int> parentFoldersIds;
        DbFile toInsert = null;

        lock (_syncRoot)
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var filesDbContext = _dbContextFactory.CreateDbContext();
                using var tx = filesDbContext.Database.BeginTransaction();

                if (file.Id == default)
                {
                    file.Id = filesDbContext.Files.Any() ? filesDbContext.Files.Max(r => r.Id) + 1 : 1;
                    file.Version = 1;
                    file.VersionGroup = 1;
                    isNew = true;
                }

                file.Title = Global.ReplaceInvalidCharsAndTruncate(file.Title);
                //make lowerCase
                file.Title = FileUtility.ReplaceFileExtension(file.Title, FileUtility.GetFileExtension(file.Title));

                file.ModifiedBy = _authContext.CurrentAccount.ID;
                file.ModifiedOn = _tenantUtil.DateTimeNow();
                if (file.CreateBy == default)
                {
                    file.CreateBy = _authContext.CurrentAccount.ID;
                }

                if (file.CreateOn == default)
                {
                    file.CreateOn = _tenantUtil.DateTimeNow();
                }

                var toUpdate = filesDbContext.Files
                    .FirstOrDefault(r => r.Id == file.Id && r.CurrentVersion && r.TenantId == TenantID);

                if (toUpdate != null)
                {
                    toUpdate.CurrentVersion = false;
                    filesDbContext.SaveChanges();
                }

                toInsert = new DbFile
                {
                    Id = file.Id,
                    Version = file.Version,
                    VersionGroup = file.VersionGroup,
                    CurrentVersion = true,
                    ParentId = file.ParentId,
                    Title = file.Title,
                    ContentLength = file.ContentLength,
                    Category = (int)file.FilterType,
                    CreateBy = file.CreateBy,
                    CreateOn = _tenantUtil.DateTimeToUtc(file.CreateOn),
                    ModifiedBy = file.ModifiedBy,
                    ModifiedOn = _tenantUtil.DateTimeToUtc(file.ModifiedOn),
                    ConvertedType = file.ConvertedType,
                    Comment = file.Comment,
                    Encrypted = file.Encrypted,
                    Forcesave = file.Forcesave,
                    ThumbnailStatus = file.ThumbnailStatus,
                    TenantId = TenantID
                };

                filesDbContext.AddOrUpdate(r => r.Files, toInsert);
                filesDbContext.SaveChanges();

                tx.Commit();
            });

            file.PureTitle = file.Title;

            var parentFolders =
                filesDbContext.Tree
                .AsQueryable()
                .Where(r => r.FolderId == file.ParentId)
                .OrderByDescending(r => r.Level)
                .ToList();

            parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

            if (parentFoldersIds.Count > 0)
            {
                var folderToUpdate = filesDbContext.Folders
                    .AsQueryable()
                    .Where(r => parentFoldersIds.Contains(r.Id));

                foreach (var f in folderToUpdate)
                {
                    f.ModifiedOn = _tenantUtil.DateTimeToUtc(file.ModifiedOn);
                    f.ModifiedBy = file.ModifiedBy;
                }

                filesDbContext.SaveChanges();
            }

            toInsert.Folders = parentFolders;

            if (isNew)
            {
                RecalculateFilesCountAsync(file.ParentId).Wait();
            }
        }

        if (fileStream != null)
        {
            try
            {
                await SaveFileStreamAsync(file, fileStream);
            }
            catch (Exception saveException)
            {
                try
                {
                    if (isNew)
                    {
                        var stored = await _globalStore.GetStore().IsDirectoryAsync(GetUniqFileDirectory(file.Id));
                        await DeleteFileAsync(file.Id, stored).ConfigureAwait(false);
                    }
                    else if (!await IsExistOnStorageAsync(file))
                    {
                        await DeleteVersionAsync(file).ConfigureAwait(false);
                    }
                }
                catch (Exception deleteException)
                {
                    throw new Exception(saveException.Message, deleteException);
                }
                throw;
            }
        }

        _ = _factoryIndexer.IndexAsync(await InitDocumentAsync(toInsert).ConfigureAwait(false));

        return await GetFileAsync(file.Id).ConfigureAwait(false);
    }

    public Task<File<int>> ReplaceFileVersionAsync(File<int> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Id == default)
        {
            throw new ArgumentException("No file id or folder id toFolderId determine provider");
        }

        var maxChunkedUploadSize = _setupInfo.MaxChunkedUploadSize(_tenantExtra, _tenantStatisticProvider);

        if (maxChunkedUploadSize < file.ContentLength)
        {
            throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
        }

        return InternalReplaceFileVersionAsync(file, fileStream);
    }

    private async Task<File<int>> InternalReplaceFileVersionAsync(File<int> file, Stream fileStream)
    {
        if (_coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            var personalMaxSpace = _coreConfiguration.PersonalMaxSpace(_settingsManager);
            if (personalMaxSpace - await _globalSpace.GetUserUsedSpaceAsync(file.Id == default ? _authContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
            {
                throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
            }
        }

        DbFile toUpdate = null;

        List<int> parentFoldersIds;
        lock (_syncRoot)
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var filesDbContext = _dbContextFactory.CreateDbContext();
                using var tx = filesDbContext.Database.BeginTransaction();

                file.Title = Global.ReplaceInvalidCharsAndTruncate(file.Title);
                //make lowerCase
                file.Title = FileUtility.ReplaceFileExtension(file.Title, FileUtility.GetFileExtension(file.Title));

                file.ModifiedBy = _authContext.CurrentAccount.ID;
                file.ModifiedOn = _tenantUtil.DateTimeNow();
                if (file.CreateBy == default)
                {
                    file.CreateBy = _authContext.CurrentAccount.ID;
                }

                if (file.CreateOn == default)
                {
                    file.CreateOn = _tenantUtil.DateTimeNow();
                }

                toUpdate = filesDbContext.Files
                    .FirstOrDefault(r => r.Id == file.Id && r.Version == file.Version && r.TenantId == TenantID);

                toUpdate.Version = file.Version;
                toUpdate.VersionGroup = file.VersionGroup;
                toUpdate.ParentId = file.ParentId;
                toUpdate.Title = file.Title;
                toUpdate.ContentLength = file.ContentLength;
                toUpdate.Category = (int)file.FilterType;
                toUpdate.CreateBy = file.CreateBy;
                toUpdate.CreateOn = _tenantUtil.DateTimeToUtc(file.CreateOn);
                toUpdate.ModifiedBy = file.ModifiedBy;
                toUpdate.ModifiedOn = _tenantUtil.DateTimeToUtc(file.ModifiedOn);
                toUpdate.ConvertedType = file.ConvertedType;
                toUpdate.Comment = file.Comment;
                toUpdate.Encrypted = file.Encrypted;
                toUpdate.Forcesave = file.Forcesave;
                toUpdate.ThumbnailStatus = file.ThumbnailStatus;

                filesDbContext.SaveChanges();

                tx.Commit();
            });

            file.PureTitle = file.Title;

            var parentFolders = filesDbContext.Tree
                .AsQueryable()
                .Where(r => r.FolderId == file.ParentId)
                .OrderByDescending(r => r.Level)
                .ToList();

            parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

            if (parentFoldersIds.Count > 0)
            {
                var folderToUpdate = filesDbContext.Folders
                    .AsQueryable()
                    .Where(r => parentFoldersIds.Contains(r.Id));

                foreach (var f in folderToUpdate)
                {
                    f.ModifiedOn = _tenantUtil.DateTimeToUtc(file.ModifiedOn);
                    f.ModifiedBy = file.ModifiedBy;
                }

                filesDbContext.SaveChanges();
            }

            toUpdate.Folders = parentFolders;
        }

        if (fileStream != null)
        {
            try
            {
                await DeleteVersionStreamAsync(file);
                await SaveFileStreamAsync(file, fileStream);
            }
            catch
            {
                if (!await IsExistOnStorageAsync(file).ConfigureAwait(false))
                {
                    await DeleteVersionAsync(file).ConfigureAwait(false);
                }

                throw;
            }
        }

        _ = _factoryIndexer.IndexAsync(await InitDocumentAsync(toUpdate).ConfigureAwait(false));

        return await GetFileAsync(file.Id).ConfigureAwait(false);
    }

    private Task DeleteVersionAsync(File<int> file)
    {
        if (file == null
            || file.Id == default
            || file.Version <= 1)
        {
            return Task.CompletedTask;
        }

        return InternalDeleteVersionAsync(file);
    }

    private async Task InternalDeleteVersionAsync(File<int> file)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var toDelete = await Query(filesDbContext.Files)
            .FirstOrDefaultAsync(r => r.Id == file.Id && r.Version == file.Version)
            .ConfigureAwait(false);

        if (toDelete != null)
        {
            filesDbContext.Files.Remove(toDelete);
        }

        await filesDbContext.SaveChangesAsync();

        var toUpdate = await Query(filesDbContext.Files)
            .FirstOrDefaultAsync(r => r.Id == file.Id && r.Version == file.Version - 1)
            .ConfigureAwait(false);

        toUpdate.CurrentVersion = true;
        await filesDbContext.SaveChangesAsync();
    }

    private async Task DeleteVersionStreamAsync(File<int> file)
    {
        await _globalStore.GetStore().DeleteDirectoryAsync(GetUniqFileVersionPath(file.Id, file.Version));
    }

    private async Task SaveFileStreamAsync(File<int> file, Stream stream)
    {
        await _globalStore.GetStore().SaveAsync(string.Empty, GetUniqFilePath(file), stream, file.Title);
    }

    public Task DeleteFileAsync(int fileId)
    {
        return DeleteFileAsync(fileId, true);
    }

    private Task DeleteFileAsync(int fileId, bool deleteFolder)
    {
        if (fileId == default)
        {
            return Task.CompletedTask;
        }

        return internalDeleteFileAsync(fileId, deleteFolder);
    }

    private async Task internalDeleteFileAsync(int fileId, bool deleteFolder)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
            using var tx = await filesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false);

            var fromFolders = Query(filesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Select(a => a.ParentId)
                .Distinct()
                .AsAsyncEnumerable();

            var toDeleteLinks = Query(filesDbContext.TagLink).Where(r => r.EntryId == fileId.ToString() && r.EntryType == FileEntryType.File);
            filesDbContext.RemoveRange(await toDeleteLinks.ToListAsync());

            var toDeleteFiles = Query(filesDbContext.Files).Where(r => r.Id == fileId);
            var toDeleteFile = await toDeleteFiles.FirstOrDefaultAsync(r => r.CurrentVersion);

            foreach (var d in toDeleteFiles)
            {
                await _factoryIndexer.DeleteAsync(d).ConfigureAwait(false);
            }

            filesDbContext.RemoveRange(await toDeleteFiles.ToListAsync());

            var tagsToRemove = Query(filesDbContext.Tag)
                .Where(r => !Query(filesDbContext.TagLink).Any(a => a.TagId == r.Id));

            filesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

            var securityToDelete = Query(filesDbContext.Security)
                .Where(r => r.EntryId == fileId.ToString())
                .Where(r => r.EntryType == FileEntryType.File);

            filesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
            await filesDbContext.SaveChangesAsync();

            await tx.CommitAsync();

            var forEachTask = fromFolders.ForEachAwaitAsync(async folderId => await RecalculateFilesCountAsync(folderId));

            if (deleteFolder)
            {
                await DeleteFolderAsync(fileId);
            }

            if (toDeleteFile != null)
            {
                await _factoryIndexer.DeleteAsync(toDeleteFile).ConfigureAwait(false);
            }

            await forEachTask;
        });
    }

    public Task<bool> IsExistAsync(string title, object folderId)
    {
        if (folderId is int fId)
        {
            return IsExistAsync(title, fId);
        }

        throw new NotImplementedException();
    }

    public async Task<bool> IsExistAsync(string title, int folderId)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        return await Query(filesDbContext.Files)
            .AsNoTracking()
            .AnyAsync(r => r.Title == title &&
                      r.ParentId == folderId &&
                      r.CurrentVersion)
;
    }

    public async Task<TTo> MoveFileAsync<TTo>(int fileId, TTo toFolderId)
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

    public Task<int> MoveFileAsync(int fileId, int toFolderId)
    {
        if (fileId == default)
        {
            return Task.FromResult<int>(default);
        }

        return InternalMoveFileAsync(fileId, toFolderId);
    }

    private async Task<int> InternalMoveFileAsync(int fileId, int toFolderId)
    {
        List<DbFile> toUpdate;

        var trashIdTask = _globalFolder.GetFolderTrashAsync<int>(_daoFactory);

        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        var fromFolders = await Query(filesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Select(a => a.ParentId)
                .Distinct()
                .AsAsyncEnumerable()
                .ToListAsync()
                .ConfigureAwait(false);

        toUpdate = await Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .ToListAsync()
            .ConfigureAwait(false);


        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
            using (var tx = await filesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {

                foreach (var f in toUpdate)
                {
                    f.ParentId = toFolderId;

                    var trashId = await trashIdTask;
                    if (trashId.Equals(toFolderId))
                    {
                        f.ModifiedBy = _authContext.CurrentAccount.ID;
                        f.ModifiedOn = DateTime.UtcNow;
                    }
                }

                await filesDbContext.SaveChangesAsync().ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);

                foreach (var f in fromFolders)
                {
                    await RecalculateFilesCountAsync(f).ConfigureAwait(false);
                }

                await RecalculateFilesCountAsync(toFolderId).ConfigureAwait(false);
            }
        });

        var parentFoldersTask =
            filesDbContext.Tree
            .AsQueryable()
            .Where(r => r.FolderId == toFolderId)
            .OrderByDescending(r => r.Level)
            .ToListAsync()
            .ConfigureAwait(false);

        var toUpdateFile = toUpdate.FirstOrDefault(r => r.CurrentVersion);

        if (toUpdateFile != null)
        {
            toUpdateFile.Folders = await parentFoldersTask;
            _factoryIndexer.Update(toUpdateFile, UpdateAction.Replace, w => w.Folders);
        }

        return fileId;
    }

    public async Task<string> MoveFileAsync(int fileId, string toFolderId)
    {
        var toSelector = _providerFolderDao.GetSelector(toFolderId);

        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, r => r,
            toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            true)
            .ConfigureAwait(false);

        return moved.Id;
    }

    public async Task<File<TTo>> CopyFileAsync<TTo>(int fileId, TTo toFolderId)
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

    public async Task<File<int>> CopyFileAsync(int fileId, int toFolderId)
    {
        var file = await GetFileAsync(fileId).ConfigureAwait(false);
        if (file != null)
        {
            var copy = _serviceProvider.GetService<File<int>>();
            copy.FileStatus = file.FileStatus;
            copy.ParentId = toFolderId;
            copy.Title = file.Title;
            copy.ConvertedType = file.ConvertedType;
            copy.Comment = FilesCommonResource.CommentCopy;
            copy.Encrypted = file.Encrypted;

            using (var stream = await GetFileStreamAsync(file))
            {
                copy.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                copy = await SaveFileAsync(copy, stream).ConfigureAwait(false);
            }

            if (file.ThumbnailStatus == Thumbnail.Created)
            {
                foreach (var size in _thumbnailSettings.Sizes)
                {
                    using (var thumbnail = await GetThumbnailAsync(file, size.Width, size.Height))
                    {
                        await SaveThumbnailAsync(copy, thumbnail, size.Width, size.Height);
                    }
                    copy.ThumbnailStatus = Thumbnail.Created;
                }
            }

            return copy;
        }
        return null;
    }

    public async Task<File<string>> CopyFileAsync(int fileId, string toFolderId)
    {
        var toSelector = _providerFolderDao.GetSelector(toFolderId);

        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, r => r,
            toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            false);

        return moved;
    }

    public async Task<int> FileRenameAsync(File<int> file, string newTitle)
    {
        newTitle = Global.ReplaceInvalidCharsAndTruncate(newTitle);

        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var toUpdate = await Query(filesDbContext.Files)
            .Where(r => r.Id == file.Id)
            .Where(r => r.CurrentVersion)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        toUpdate.Title = newTitle;
        toUpdate.ModifiedOn = DateTime.UtcNow;
        toUpdate.ModifiedBy = _authContext.CurrentAccount.ID;

        await filesDbContext.SaveChangesAsync().ConfigureAwait(false);

        await _factoryIndexer.UpdateAsync(toUpdate, true, r => r.Title, r => r.ModifiedBy, r => r.ModifiedOn);

        return file.Id;
    }

    public async Task<string> UpdateCommentAsync(int fileId, int fileVersion, string comment)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var toUpdate = await Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Version == fileVersion)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        comment ??= string.Empty;
        comment = comment.Substring(0, Math.Min(comment.Length, 255));

        toUpdate.Comment = comment;

        await filesDbContext.SaveChangesAsync();

        return comment;
    }

    public async Task CompleteVersionAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var toUpdate = Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Version > fileVersion);

        foreach (var f in toUpdate)
        {
            f.VersionGroup += 1;
        }

        await filesDbContext.SaveChangesAsync();
    }

    public async Task ContinueVersionAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
            using var tx = await filesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false);

            var versionGroup = await Query(filesDbContext.Files)
                .AsNoTracking()
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .Select(r => r.VersionGroup)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var toUpdate = Query(filesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version > fileVersion)
                .Where(r => r.VersionGroup > versionGroup);

            foreach (var f in toUpdate)
            {
                f.VersionGroup -= 1;
            }

            await filesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await tx.CommitAsync().ConfigureAwait(false);
        });
    }

    public bool UseTrashForRemove(File<int> file)
    {
        return file.RootFolderType != FolderType.TRASH && file.RootFolderType != FolderType.Privacy;
    }

    public string GetUniqFileDirectory(int fileId)
    {
        if (fileId == 0)
        {
            throw new ArgumentNullException("fileIdObject");
        }

        return string.Format("folder_{0}/file_{1}", (fileId / 1000 + 1) * 1000, fileId);
    }

    public string GetUniqFilePath(File<int> file)
    {
        return file != null
                   ? GetUniqFilePath(file, "content" + FileUtility.GetFileExtension(file.PureTitle))
                   : null;
    }

    public string GetUniqFilePath(File<int> file, string fileTitle)
    {
        return file != null
                   ? $"{GetUniqFileVersionPath(file.Id, file.Version)}/{fileTitle}"
                   : null;
    }

    public string GetUniqFileVersionPath(int fileId, int version)
    {
        return fileId != 0
                   ? string.Format("{0}/v{1}", GetUniqFileDirectory(fileId), version)
                   : null;
    }

    private Task RecalculateFilesCountAsync(int folderId)
    {
        return GetRecalculateFilesCountUpdateAsync(folderId);
    }

    #region chunking

    public Task<ChunkedUploadSession<int>> CreateUploadSessionAsync(File<int> file, long contentLength)
    {
        return _chunkedUploadSessionHolder.CreateUploadSessionAsync(file, contentLength);
    }

    public async Task<File<int>> UploadChunkAsync(ChunkedUploadSession<int> uploadSession, Stream stream, long chunkLength)
    {
        if (!uploadSession.UseChunks)
        {
            using var streamToSave = await _chunkedUploadSessionHolder.UploadSingleChunkAsync(uploadSession, stream, chunkLength).ConfigureAwait(false);
            if (streamToSave != Stream.Null)
            {
                uploadSession.File = await SaveFileAsync(await GetFileForCommitAsync(uploadSession).ConfigureAwait(false), streamToSave).ConfigureAwait(false);
            }

            return uploadSession.File;
        }

        await _chunkedUploadSessionHolder.UploadChunkAsync(uploadSession, stream, chunkLength);

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
        {
            uploadSession.File = await FinalizeUploadSessionAsync(uploadSession).ConfigureAwait(false);
        }

        return uploadSession.File;
    }

    private async Task<File<int>> FinalizeUploadSessionAsync(ChunkedUploadSession<int> uploadSession)
    {
        await _chunkedUploadSessionHolder.FinalizeUploadSessionAsync(uploadSession);

        var file = await GetFileForCommitAsync(uploadSession).ConfigureAwait(false);
        await SaveFileAsync(file, null, uploadSession.CheckQuota).ConfigureAwait(false);
        await _chunkedUploadSessionHolder.MoveAsync(uploadSession, GetUniqFilePath(file));

        return file;
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<int> uploadSession)
    {
        return _chunkedUploadSessionHolder.AbortUploadSessionAsync(uploadSession);
    }

    private async Task<File<int>> GetFileForCommitAsync(ChunkedUploadSession<int> uploadSession)
    {
        if (uploadSession.File.Id != default)
        {
            var file = await GetFileAsync(uploadSession.File.Id).ConfigureAwait(false);
            if (!uploadSession.KeepVersion)
            {
                file.Version++;
            }
            file.ContentLength = uploadSession.BytesTotal;
            file.ConvertedType = null;
            file.Comment = FilesCommonResource.CommentUpload;
            file.Encrypted = uploadSession.Encrypted;
            file.ThumbnailStatus = Thumbnail.Waiting;

            return file;
        }

        var result = _serviceProvider.GetService<File<int>>();
        result.ParentId = uploadSession.File.ParentId;
        result.Title = uploadSession.File.Title;
        result.ContentLength = uploadSession.BytesTotal;
        result.Comment = FilesCommonResource.CommentUpload;
        result.Encrypted = uploadSession.Encrypted;
        result.CreateOn = uploadSession.File.CreateOn;

        return result;
    }

    #endregion

    #region Only in TMFileDao

    public async Task ReassignFilesAsync(int[] fileIds, Guid newOwnerId)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var toUpdate = Query(filesDbContext.Files)
            .Where(r => r.CurrentVersion)
            .Where(r => fileIds.Contains(r.Id));

        foreach (var f in toUpdate)
        {
            f.CreateBy = newOwnerId;
        }

        await filesDbContext.SaveChangesAsync();
    }

    public Task<List<File<int>>> GetFilesAsync(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
    {
        if (parentIds == null || !parentIds.Any() || filterType == FilterType.FoldersOnly)
        {
            return Task.FromResult(new List<File<int>>());
        }

        return InternalGetFilesAsync(parentIds, filterType, subjectGroup, subjectID, searchText, searchInContent);
    }

    private async Task<List<File<int>>> InternalGetFilesAsync(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        var q = GetFileQuery(filesDbContext, r => r.CurrentVersion)
            .AsNoTracking()
            .Join(filesDbContext.Tree, a => a.ParentId, t => t.FolderId, (file, tree) => new { file, tree })
            .Where(r => parentIds.Contains(r.tree.ParentId))
            .Select(r => r.file);

        if (!string.IsNullOrEmpty(searchText))
        {
            var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

            if (_factoryIndexer.TrySelectIds(s => func(s), out var searchIds))
            {
                q = q.Where(r => searchIds.Contains(r.Id));
            }
            else
            {
                q = BuildSearch(q, searchText, SearhTypeEnum.Any);
            }
        }

        if (subjectID != Guid.Empty)
        {
            if (subjectGroup)
            {
                var users = _userManager.GetUsersByGroup(subjectID).Select(u => u.Id).ToArray();
                q = q.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                q = q.Where(r => r.CreateBy == subjectID);
            }
        }

        switch (filterType)
        {
            case FilterType.DocumentsOnly:
            case FilterType.ImagesOnly:
            case FilterType.PresentationsOnly:
            case FilterType.SpreadsheetsOnly:
            case FilterType.ArchiveOnly:
            case FilterType.MediaOnly:
                q = q.Where(r => r.Category == (int)filterType);
                break;
            case FilterType.ByExtension:
                if (!string.IsNullOrEmpty(searchText))
                {
                    q = BuildSearch(q, searchText, SearhTypeEnum.End);
                }
                break;
        }

        var query = await FromQueryWithShared(filesDbContext, q).ToListAsync().ConfigureAwait(false);

        return query.ConvertAll(e => _mapper.Map<DbFileQuery, File<int>>(e));
    }

    public IAsyncEnumerable<File<int>> SearchAsync(string searchText, bool bunch = false)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        if (_factoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var ids))
        {
            var query = GetFileQuery(filesDbContext, r => r.CurrentVersion && ids.Contains(r.Id)).AsNoTracking();

            return FromQueryWithShared(filesDbContext, query).AsAsyncEnumerable()
                .Select(e => _mapper.Map<DbFileQuery, File<int>>(e))
                .Where(
                    f =>
                    bunch
                        ? f.RootFolderType == FolderType.BUNCH
                        : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON)
                ;
        }
        else
        {
            var query = BuildSearch(GetFileQuery(filesDbContext, r => r.CurrentVersion).AsNoTracking(), searchText, SearhTypeEnum.Any);

            return FromQueryWithShared(filesDbContext, query)
                .AsAsyncEnumerable()
                .Select(e => _mapper.Map<DbFileQuery, File<int>>(e))
                .Where(f =>
                       bunch
                            ? f.RootFolderType == FolderType.BUNCH
                            : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON);
        }
    }

    private async Task DeleteFolderAsync(int fileId)
    {
        await _globalStore.GetStore().DeleteDirectoryAsync(GetUniqFileDirectory(fileId));
    }

    public Task<bool> IsExistOnStorageAsync(File<int> file)
    {
        return _globalStore.GetStore().IsFileAsync(string.Empty, GetUniqFilePath(file));
    }

    private const string DiffTitle = "diff.zip";

    public Task SaveEditHistoryAsync(File<int> file, string changes, Stream differenceStream)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(changes);

        ArgumentNullException.ThrowIfNull(differenceStream);

        return InternalSaveEditHistoryAsync(file, changes, differenceStream);
    }

    private async Task InternalSaveEditHistoryAsync(File<int> file, string changes, Stream differenceStream)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var toUpdateTask = Query(filesDbContext.Files)
            .Where(r => r.Id == file.Id)
            .Where(r => r.Version == file.Version)
            .ToListAsync()
            .ConfigureAwait(false);

        changes = changes.Trim();

        foreach (var f in await toUpdateTask)
        {
            f.Changes = changes;
        }

        await filesDbContext.SaveChangesAsync();

        await _globalStore.GetStore().SaveAsync(string.Empty, GetUniqFilePath(file, DiffTitle), differenceStream, DiffTitle);
    }

    public async Task<List<EditHistory>> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, int fileId, int fileVersion = 0)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Forcesave == ForcesaveType.None);

        if (fileVersion > 0)
        {
            query = query.Where(r => r.Version == fileVersion);
        }

        query = query.OrderBy(r => r.Version);
        var dbFiles = await query.ToListAsync().ConfigureAwait(false);

        return dbFiles
                .Select(r =>
                    {
                        var item = _serviceProvider.GetService<EditHistory>();

                        item.ID = r.Id;
                        item.Version = r.Version;
                        item.VersionGroup = r.VersionGroup;
                        item.ModifiedOn = _tenantUtil.DateTimeFromUtc(r.ModifiedOn);
                        item.ModifiedBy = r.ModifiedBy;
                        item.ChangesString = r.Changes;
                        item.Key = documentServiceHelper.GetDocKey(item.ID, item.Version, _tenantUtil.DateTimeFromUtc(r.CreateOn));

                        return item;
                    })
                .ToList();
    }

    public Task<Stream> GetDifferenceStreamAsync(File<int> file)
    {
        return _globalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file, DiffTitle), 0);
    }

    public async Task<bool> ContainChangesAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        return await Query(filesDbContext.Files)
            .AnyAsync(r => r.Id == fileId &&
                      r.Version == fileVersion &&
                      r.Changes != null)
;
    }

    public async Task<IEnumerable<FileWithShare>> GetFeedsAsync(int tenant, DateTime from, DateTime to)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var q1 = filesDbContext.Files
            .AsQueryable()
            .Where(r => r.TenantId == tenant)
            .Where(r => r.CurrentVersion)
            .Where(r => r.ModifiedOn >= from && r.ModifiedOn <= to);

        var q2 = FromQuery(filesDbContext, q1)
            .Select(r => new DbFileQueryWithSecurity() { DbFileQuery = r, Security = null });

        var q3 = filesDbContext.Files
            .AsQueryable()
            .Where(r => r.TenantId == tenant)
            .Where(r => r.CurrentVersion);

        var q4Task = FromQuery(filesDbContext, q3)
            .Join(filesDbContext.Security.AsQueryable().DefaultIfEmpty(), r => r.File.Id.ToString(), s => s.EntryId, (f, s) => new DbFileQueryWithSecurity { DbFileQuery = f, Security = s })
            .Where(r => r.Security.TenantId == tenant)
            .Where(r => r.Security.EntryType == FileEntryType.File)
            .Where(r => r.Security.Share == FileShare.Restrict)
            //.Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to)
            .ToListAsync();

        var fileWithShare = await q2.Select(e => _mapper.Map<DbFileQueryWithSecurity, FileWithShare>(e))
            .ToListAsync().ConfigureAwait(false);
        var q4 = await q4Task.ConfigureAwait(false);

        return fileWithShare.Union(_mapper.Map<IEnumerable<DbFileQueryWithSecurity>, IEnumerable<FileWithShare>>(q4));
    }

    public async Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime fromTime)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var q1Task = filesDbContext.Files
            .AsQueryable()
            .Where(r => r.ModifiedOn > fromTime)
            .GroupBy(r => r.TenantId)
            .Where(r => r.Any())
            .Select(r => r.Key)
            .ToListAsync()
            .ConfigureAwait(false);

        var q2Task = filesDbContext.Security
            .AsQueryable()
            .Where(r => r.TimeStamp > fromTime)
            .GroupBy(r => r.TenantId)
            .Where(r => r.Any())
            .Select(r => r.Key)
            .ToListAsync()
            .ConfigureAwait(false);

        var q1 = await q1Task;
        var q2 = await q2Task;

        return q1.Union(q2);
    }

    private const string ThumbnailTitle = "thumb";

    public Task SaveThumbnailAsync(File<int> file, Stream thumbnail, int width, int height)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        return InternalSaveThumbnailAsync(file, thumbnail, width, height);
    }

    private async Task InternalSaveThumbnailAsync(File<int> file, Stream thumbnail, int width, int height)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        var toUpdate = await filesDbContext.Files
            .AsQueryable()
            .FirstOrDefaultAsync(r => r.Id == file.Id && r.Version == file.Version && r.TenantId == TenantID)
            .ConfigureAwait(false);

        if (toUpdate != null)
        {
            toUpdate.ThumbnailStatus = thumbnail != null ? Thumbnail.Created : file.ThumbnailStatus;
            await filesDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        if (thumbnail == null)
        {
            return;
        }

        var thumnailName = GetThumnailName(width, height);
        await _globalStore.GetStore().SaveAsync(string.Empty, GetUniqFilePath(file, thumnailName), thumbnail, thumnailName);
    }

    public async Task<Stream> GetThumbnailAsync(int fileId, int width, int height)
    {
        var file = await GetFileAsync(fileId);
        return await GetThumbnailAsync(file, width, height);
    }

    public async Task<Stream> GetThumbnailAsync(File<int> file, int width, int height)
    {
        var thumnailName = GetThumnailName(width, height);
        var path = GetUniqFilePath(file, thumnailName);
        var storage = _globalStore.GetStore();
        var isFile = await storage.IsFileAsync(string.Empty, path).ConfigureAwait(false);

        if (!isFile)
        {
            throw new FileNotFoundException();
        }

        return await storage.GetReadStreamAsync(string.Empty, path, 0).ConfigureAwait(false);
    }

    private string GetThumnailName(int width, int height)
    {
        return $"{ThumbnailTitle}.{width}x{height}.{_global.ThumbnailExtension}";
    }

    public async Task<EntryProperties> GetProperties(int fileId)
    {
        var entryId = fileId.ToString();
        var tenantId = TenantID;

        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();
        return EntryProperties.Deserialize(await
                filesDbContext.FilesProperties
               .Where(r => r.TenantId == tenantId)
               .Where(r => r.EntryId == entryId)
               .Select(r => r.Data)
               .FirstOrDefaultAsync(), _logger);
    }

    public async Task SaveProperties(int fileId, EntryProperties entryProperties)
    {
        var entryId = fileId.ToString();
        var tenantId = TenantID;
        string data;

        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        if (entryProperties == null || string.IsNullOrEmpty(data = EntryProperties.Serialize(entryProperties, _logger)))
        {
            var props = filesDbContext.FilesProperties
               .Where(r => r.TenantId == tenantId)
               .Where(r => r.EntryId == entryId);

            filesDbContext.FilesProperties.RemoveRange(await props.ToListAsync());
            await filesDbContext.SaveChangesAsync();
            return;
        }

        await filesDbContext.AddOrUpdateAsync(r => r.FilesProperties, new DbFilesProperties { TenantId = tenantId, EntryId = entryId, Data = data });
        await filesDbContext.SaveChangesAsync();
    }
    #endregion

    private Func<Selector<DbFile>, Selector<DbFile>> GetFuncForSearch(int? parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
    {
        return s =>
       {
           var result = !searchInContent || filterType == FilterType.ByExtension
               ? s.Match(r => r.Title, searchText)
               : s.MatchAll(searchText);

           if (parentId != null)
           {
               if (withSubfolders)
               {
                   result.In(a => a.Folders.Select(r => r.ParentId), new[] { parentId });
               }
               else
               {
                   result.InAll(a => a.Folders.Select(r => r.ParentId), new[] { parentId });
               }
           }

           if (orderBy != null)
           {
               switch (orderBy.SortedBy)
               {
                   case SortedByType.Author:
                       result.Sort(r => r.CreateBy, orderBy.IsAsc);
                       break;
                   case SortedByType.Size:
                       result.Sort(r => r.ContentLength, orderBy.IsAsc);
                       break;
                   //case SortedByType.AZ:
                   //    result.Sort(r => r.Title, orderBy.IsAsc);
                   //    break;
                   case SortedByType.DateAndTime:
                       result.Sort(r => r.ModifiedOn, orderBy.IsAsc);
                       break;
                   case SortedByType.DateAndTimeCreation:
                       result.Sort(r => r.CreateOn, orderBy.IsAsc);
                       break;
               }
           }

           if (subjectID != Guid.Empty)
           {
               if (subjectGroup)
               {
                   var users = _userManager.GetUsersByGroup(subjectID).Select(u => u.Id).ToArray();
                   result.In(r => r.CreateBy, users);
               }
               else
               {
                   result.Where(r => r.CreateBy, subjectID);
               }
           }

           switch (filterType)
           {
               case FilterType.DocumentsOnly:
               case FilterType.ImagesOnly:
               case FilterType.PresentationsOnly:
               case FilterType.SpreadsheetsOnly:
               case FilterType.ArchiveOnly:
               case FilterType.MediaOnly:
                   result.Where(r => r.Category, (int)filterType);
                   break;
           }

           return result;
       };
    }

    protected IQueryable<DbFileQuery> FromQueryWithShared(FilesDbContext filesDbContext, IQueryable<DbFile> dbFiles)
    {
        var cId = _authContext.CurrentAccount.ID;

        return from r in dbFiles
               select new DbFileQuery
               {
                   File = r,
                   Root = (from f in filesDbContext.Folders.AsQueryable()
                           where f.Id ==
                           (from t in filesDbContext.Tree.AsQueryable()
                            where t.FolderId == r.ParentId
                            orderby t.Level descending
                            select t.ParentId
                            ).FirstOrDefault()
                           where f.TenantId == r.TenantId
                           select f
                          ).FirstOrDefault(),
                   Shared = (from f in filesDbContext.Security.AsQueryable()
                             where f.EntryType == FileEntryType.File && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId && !(new[] { FileConstant.DenyDownloadId, FileConstant.DenySharingId }).Contains(f.Subject)
                             select f
                             ).Any(),
                   IsFillFormDraft = (from f in filesDbContext.FilesLink
                                      where f.TenantId == r.TenantId && f.LinkedId == r.Id.ToString() && f.LinkedFor == cId
                                      select f)
                             .Any(),
                   Deny = (from f in filesDbContext.Security.AsQueryable()
                           where f.EntryType == FileEntryType.File && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId && (new[] { FileConstant.DenyDownloadId, FileConstant.DenySharingId }).Contains(f.Subject)
                           select f
                            ).GroupBy(a => a.EntryId,
                            (a, b) =>
                            new DbFileDeny
                            {
                                DenyDownload = b.Any(c => c.Subject == FileConstant.DenyDownloadId),
                                DenySharing = b.Any(c => c.Subject == FileConstant.DenySharingId)
                            })
                            .FirstOrDefault(),
               };
    }

    protected IQueryable<DbFileQuery> FromQuery(FilesDbContext filesDbContext, IQueryable<DbFile> dbFiles)
    {
        var cId = _authContext.CurrentAccount.ID;

        return dbFiles
            .Select(r => new DbFileQuery
            {
                File = r,
                Root = (from f in filesDbContext.Folders.AsQueryable()
                        where f.Id ==
                        (from t in filesDbContext.Tree.AsQueryable()
                         where t.FolderId == r.ParentId
                         orderby t.Level descending
                         select t.ParentId
                         ).FirstOrDefault()
                        where f.TenantId == r.TenantId
                        select f
                          ).FirstOrDefault(),
                Shared = true,
                IsFillFormDraft = (from f in filesDbContext.FilesLink
                                   where f.TenantId == r.TenantId && f.LinkedId == r.Id.ToString() && f.LinkedFor == cId
                                   select f)
                             .Any(),
                Deny = (from f in filesDbContext.Security.AsQueryable()
                        where f.EntryType == FileEntryType.File && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId && (new[] { FileConstant.DenyDownloadId, FileConstant.DenySharingId }).Contains(f.Subject)
                        select f
                            ).GroupBy(a => a.EntryId,
                            (a, b) =>
                            new DbFileDeny
                            {
                                DenyDownload = b.Any(c => c.Subject == FileConstant.DenyDownloadId),
                                DenySharing = b.Any(c => c.Subject == FileConstant.DenySharingId)
                            })
                            .FirstOrDefault(),
            });
    }

    protected internal Task<DbFile> InitDocumentAsync(DbFile dbFile)
    {
        if (!_factoryIndexer.CanIndexByContent(dbFile))
        {
            dbFile.Document = new Document
            {
                Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(""))
            };

            return Task.FromResult(dbFile);
        }

        return InernalInitDocumentAsync(dbFile);
    }

    private async Task<DbFile> InernalInitDocumentAsync(DbFile dbFile)
    {
        var file = _serviceProvider.GetService<File<int>>();
        file.Id = dbFile.Id;
        file.Title = dbFile.Title;
        file.Version = dbFile.Version;
        file.ContentLength = dbFile.ContentLength;

        if (!await IsExistOnStorageAsync(file).ConfigureAwait(false) || file.ContentLength > _settings.MaxContentLength)
        {
            return dbFile;
        }

        using var stream = await GetFileStreamAsync(file).ConfigureAwait(false);

        if (stream == null)
        {
            return dbFile;
        }

        using (var ms = new MemoryStream())
        {
            await stream.CopyToAsync(ms).ConfigureAwait(false);
            dbFile.Document = new Document
            {
                Data = Convert.ToBase64String(ms.GetBuffer())
            };
        }

        return dbFile;
    }
}

public class DbFileQuery
{
    public DbFile File { get; set; }
    public DbFolder Root { get; set; }
    public bool Shared { get; set; }
    public bool IsFillFormDraft { get; set; }
    public DbFileDeny Deny { get; set; }
}

public class DbFileDeny
{
    public bool DenyDownload { get; set; }
    public bool DenySharing { get; set; }
}

public class DbFileQueryWithSecurity
{
    public DbFileQuery DbFileQuery { get; set; }
    public DbFilesSecurity Security { get; set; }
}