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
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly ILogger<FileDao> _logger;
    private readonly FactoryIndexerFile _factoryIndexer;
    private readonly GlobalStore _globalStore;
    private readonly GlobalSpace _globalSpace;
    private readonly GlobalFolder _globalFolder;
    private readonly Global _global;
    private readonly IDaoFactory _daoFactory;
    private readonly ChunkedUploadSessionHolder _chunkedUploadSessionHolder;
    private readonly SelectorFactory _selectorFactory;
    private readonly CrossDao _crossDao;
    private readonly Settings _settings;
    private readonly IMapper _mapper;
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly IQuotaService _quotaService;

    public FileDao(
        ILogger<FileDao> logger,
        FactoryIndexerFile factoryIndexer,
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        MaxTotalSizeStatistic maxTotalSizeStatistic,
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
        SelectorFactory selectorFactory,
        CrossDao crossDao,
        Settings settings,
        IMapper mapper,
        ThumbnailSettings thumbnailSettings,
        IQuotaService quotaService)
        : base(
              dbContextManager,
              userManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              maxTotalSizeStatistic,
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
        _selectorFactory = selectorFactory;
        _crossDao = crossDao;
        _settings = settings;
        _mapper = mapper;
        _thumbnailSettings = thumbnailSettings;
        _quotaService = quotaService;
    }

    public Task InvalidateCacheAsync(int fileId)
    {
        return Task.CompletedTask;
    }

    public async Task<File<int>> GetFileAsync(int fileId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var query = GetFileQuery(filesDbContext, r => r.Id == fileId && r.CurrentVersion).AsNoTracking();

        var dbFile = await FromQuery(filesDbContext, query)
            .SingleOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public async Task<File<int>> GetFileAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var query = GetFileQuery(filesDbContext, r => r.Id == fileId && r.Version == fileVersion).AsNoTracking();

        var dbFile = await FromQuery(filesDbContext, query)
            .SingleOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public async Task<File<int>> GetFileAsync(int parentId, string title)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(title);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => r.Title == title && r.CurrentVersion && r.ParentId == parentId)
            .AsNoTracking()
            .OrderBy(r => r.CreateOn);

        var dbFile = await FromQuery(filesDbContext, query).FirstOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public async Task<File<int>> GetFileStableAsync(int fileId, int fileVersion = -1)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var query = GetFileQuery(filesDbContext, r => r.Id == fileId && r.Forcesave == ForcesaveType.None)
            .AsNoTracking();

        if (fileVersion >= 0)
        {
            query = query.Where(r => r.Version <= fileVersion);
        }

        query = query.OrderByDescending(r => r.Version);

        var dbFile = await FromQuery(filesDbContext, query).FirstOrDefaultAsync();

        return _mapper.Map<DbFileQuery, File<int>>(dbFile);
    }

    public async IAsyncEnumerable<File<int>> GetFileHistoryAsync(int fileId)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => r.Id == fileId).OrderByDescending(r => r.Version).AsNoTracking();

        await foreach (var e in FromQuery(filesDbContext, query).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQuery, File<int>>(e);
        }
    }

    public async IAsyncEnumerable<File<int>> GetFilesAsync(IEnumerable<int> fileIds)
    {
        if (fileIds == null || !fileIds.Any())
        {
            yield break;
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => fileIds.Contains(r.Id) && r.CurrentVersion).AsNoTracking();

        await foreach (var e in FromQuery(filesDbContext, query).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQuery, File<int>>(e);
        }
    }

    public async IAsyncEnumerable<File<int>> GetFilesFilteredAsync(IEnumerable<int> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
    {
        if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = GetFileQuery(filesDbContext, r => fileIds.Contains(r.Id) && r.CurrentVersion).AsNoTracking();

        if (!string.IsNullOrEmpty(searchText))
        {
            var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

            (var succ, var searchIds) = await _factoryIndexer.TrySelectIdsAsync(s => func(s).In(r => r.Id, fileIds.ToArray()));
            if (succ)
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
                var users = (await _userManager.GetUsersByGroupAsync(subjectID)).Select(u => u.Id).ToArray();
                query = query.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                query = query.Where(r => r.CreateBy == subjectID);
            }
        }

        switch (filterType)
        {
            case FilterType.OFormOnly:
            case FilterType.OFormTemplateOnly:
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

        await foreach (var e in (checkShared ? FromQuery(filesDbContext, query) : FromQuery(filesDbContext, query)).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQuery, File<int>>(e);
        }
    }

    public async IAsyncEnumerable<int> GetFilesAsync(int parentId)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();

        var query = Query(filesDbContext.Files)
            .AsNoTracking()
            .Where(r => r.ParentId == parentId && r.CurrentVersion)
            .Select(r => r.Id);

        await foreach (var e in query.AsAsyncEnumerable())
        {
            yield return e;
        }
    }

    public async IAsyncEnumerable<File<int>> GetFilesAsync(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false, bool excludeSubject = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
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

            (var succ, var searchIds) = await _factoryIndexer.TrySelectIdsAsync(expression);
            if (succ)
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
                var users = (await _userManager.GetUsersByGroupAsync(subjectID)).Select(u => u.Id).ToArray();
                q = q.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                q = excludeSubject ? q.Where(r => r.CreateBy != subjectID) : q.Where(r => r.CreateBy == subjectID);
            }
        }

        switch (filterType)
        {
            case FilterType.OFormOnly:
            case FilterType.OFormTemplateOnly:
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
        
        await foreach (var e in FromQuery(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQuery, File<int>>(e);
        }
    }

    public async Task<Stream> GetFileStreamAsync(File<int> file, long offset)
    {
        return await (await _globalStore.GetStoreAsync()).GetReadStreamAsync(string.Empty, GetUniqFilePath(file), offset);
    }

    public async Task<Uri> GetPreSignedUriAsync(File<int> file, TimeSpan expires)
    {
        return await (await _globalStore.GetStoreAsync()).GetPreSignedUriAsync(string.Empty, GetUniqFilePath(file), expires,
                                                 new List<string>
                                                     {
                                                             string.Concat("Content-Disposition:", ContentDispositionUtil.GetHeaderValue(file.Title, withoutBase: true))
                                                     });
    }

    public async Task<bool> IsSupportedPreSignedUriAsync(File<int> file)
    {
        return (await _globalStore.GetStoreAsync()).IsSupportedPreSignedUri;
    }

    public async Task<Stream> GetFileStreamAsync(File<int> file)
    {
        return await (await _globalStore.GetStoreAsync()).GetReadStreamAsync(string.Empty, GetUniqFilePath(file), 0);
    }

    public async Task<File<int>> SaveFileAsync(File<int> file, Stream fileStream)
    {
        return await SaveFileAsync(file, fileStream, true);
    }

    public async Task<File<int>> SaveFileAsync(File<int> file, Stream fileStream, bool checkQuota = true, ChunkedUploadSession<int> uploadSession = null)
    {
        ArgumentNullException.ThrowIfNull(file);

        var maxChunkedUploadSize = await _setupInfo.MaxChunkedUploadSize(_tenantManager, _maxTotalSizeStatistic);
        if (checkQuota && maxChunkedUploadSize < file.ContentLength)
        {
            throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
        }

        if (checkQuota && _coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            var personalMaxSpace = await _coreConfiguration.PersonalMaxSpaceAsync(_settingsManager);
            if (personalMaxSpace - await _globalSpace.GetUserUsedSpaceAsync(file.Id == default ? _authContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
            {
                throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
            }
        }

        var quotaSettings = await _settingsManager.LoadAsync<TenantUserQuotaSettings>();

        if (quotaSettings.EnableUserQuota)
        {
            var user = await _userManager.GetUsersAsync(file.Id == default ? _authContext.CurrentAccount.ID : file.CreateBy);
            var userQuotaSettings = await _settingsManager.LoadAsync<UserQuotaSettings>(user);
            var quotaLimit = userQuotaSettings.UserQuota;

            if (quotaLimit != -1)
            {
                var userUsedSpace = Math.Max(0, (await _quotaService.FindUserQuotaRowsAsync(TenantID, user.Id)).Where(r => !string.IsNullOrEmpty(r.Tag)).Sum(r => r.Counter));

                if (quotaLimit - userUsedSpace < file.ContentLength)
                {
                    throw FileSizeComment.GetPersonalFreeSpaceException(quotaLimit);
                }
            }
        }

        var isNew = false;
        List<int> parentFoldersIds;
        DbFile toInsert = null;

        await _semaphore.WaitAsync();
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();

            if (file.Id == default)
            {
                file.Id = await filesDbContext.Files.AnyAsync() ? await filesDbContext.Files.MaxAsync(r => r.Id) + 1 : 1;
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

            var tenantId = TenantID;
            await filesDbContext.Files
                .Where(r => r.Id == file.Id && r.CurrentVersion && r.TenantId == tenantId)
                .ExecuteUpdateAsync(f => f.SetProperty(p => p.CurrentVersion, false));

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

            await filesDbContext.AddOrUpdateAsync(r => r.Files, toInsert);
            await filesDbContext.SaveChangesAsync();

            await tx.CommitAsync();
        });

        file.PureTitle = file.Title;

        var parentFolders = await
            filesDbContext.Tree
            .Where(r => r.FolderId == file.ParentId)
            .OrderByDescending(r => r.Level)
            .ToListAsync();

        parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

        if (parentFoldersIds.Count > 0)
        {
            await filesDbContext.Folders
                .Where(r => parentFoldersIds.Contains(r.Id))
                .ExecuteUpdateAsync(f => f
                .SetProperty(p => p.ModifiedOn, _tenantUtil.DateTimeToUtc(file.ModifiedOn))
                .SetProperty(p => p.ModifiedBy, file.ModifiedBy)
                );
            }

        toInsert.Folders = parentFolders;

        if (isNew)
        {
            await RecalculateFilesCountAsync(file.ParentId);
        }

        _semaphore.Release();

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
                        var stored = await (await _globalStore.GetStoreAsync()).IsDirectoryAsync(GetUniqFileDirectory(file.Id));
                        await DeleteFileAsync(file.Id, stored);
                    }
                    else if (!await IsExistOnStorageAsync(file))
                    {
                        await DeleteVersionAsync(file);
                    }
                }
                catch (Exception deleteException)
                {
                    throw new Exception(saveException.Message, deleteException);
                }
                throw;
            }
        }
        else
        {
            if(uploadSession != null)
            {
                await _chunkedUploadSessionHolder.MoveAsync(uploadSession, GetUniqFilePath(file));
            }
        }

        _ = _factoryIndexer.IndexAsync(await InitDocumentAsync(toInsert));

        return await GetFileAsync(file.Id);
    }

    public async Task<File<int>> ReplaceFileVersionAsync(File<int> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Id == default)
        {
            throw new ArgumentException("No file id or folder id toFolderId determine provider");
        }

        var maxChunkedUploadSize = await _setupInfo.MaxChunkedUploadSize(_tenantManager, _maxTotalSizeStatistic);

        if (maxChunkedUploadSize < file.ContentLength)
        {
            throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
        }

        if (_coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            var personalMaxSpace = await _coreConfiguration.PersonalMaxSpaceAsync(_settingsManager);
            if (personalMaxSpace - await _globalSpace.GetUserUsedSpaceAsync(file.Id == default ? _authContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
            {
                throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
            }
        }

        DbFile toUpdate = null;

        List<int> parentFoldersIds;
        await _semaphore.WaitAsync();
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();

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

            toUpdate = await filesDbContext.Files
                .FirstOrDefaultAsync(r => r.Id == file.Id && r.Version == file.Version && r.TenantId == TenantID);

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

            await filesDbContext.SaveChangesAsync();

            await tx.CommitAsync();
        });

        file.PureTitle = file.Title;

        var parentFolders = await filesDbContext.Tree
            .Where(r => r.FolderId == file.ParentId)
            .OrderByDescending(r => r.Level)
            .ToListAsync();

        parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

        if (parentFoldersIds.Count > 0)
        {
            await filesDbContext.Folders
                .Where(r => parentFoldersIds.Contains(r.Id))
                .ExecuteUpdateAsync(f => f
                .SetProperty(p => p.ModifiedOn, _tenantUtil.DateTimeToUtc(file.ModifiedOn))
                .SetProperty(p => p.ModifiedBy, file.ModifiedBy)
                );
            }

        toUpdate.Folders = parentFolders;
        _semaphore.Release();

        if (fileStream != null)
        {
            try
            {
                await DeleteVersionStreamAsync(file);
                await SaveFileStreamAsync(file, fileStream);
            }
            catch
            {
                if (!await IsExistOnStorageAsync(file))
                {
                    await DeleteVersionAsync(file);
                }

                throw;
            }
        }

        _ = _factoryIndexer.IndexAsync(await InitDocumentAsync(toUpdate));

        return await GetFileAsync(file.Id);
    }

    private async ValueTask DeleteVersionAsync(File<int> file)
    {
        if (file == null
            || file.Id == default
            || file.Version <= 1)
        {
            return;
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Query(filesDbContext.Files)
            .Where(r => r.Id == file.Id && r.Version == file.Version)
            .ExecuteDeleteAsync();

        await Query(filesDbContext.Files)
            .Where(r => r.Id == file.Id && r.Version == file.Version - 1)
            .ExecuteUpdateAsync(f => f.SetProperty(p => p.CurrentVersion, true));
    }

    private async Task DeleteVersionStreamAsync(File<int> file)
    {
        await (await _globalStore.GetStoreAsync()).DeleteDirectoryAsync(GetUniqFileVersionPath(file.Id, file.Version));
    }

    private async Task SaveFileStreamAsync(File<int> file, Stream stream)
    {
        await (await _globalStore.GetStoreAsync()).SaveAsync(string.Empty, GetUniqFilePath(file), stream, file.Title);
    }

    public async Task DeleteFileAsync(int fileId)
    {
        await DeleteFileAsync(fileId, true);
    }

    private async ValueTask DeleteFileAsync(int fileId, bool deleteFolder)
    {
        if (fileId == default)
        {
            return;
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();

            var fromFolders = await Query(filesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Select(a => a.ParentId)
                .Distinct()
                .ToListAsync();

            await Query(filesDbContext.TagLink)
            .Where(r => r.EntryId == fileId.ToString() && r.EntryType == FileEntryType.File)
            .ExecuteDeleteAsync();

            var toDeleteFiles = Query(filesDbContext.Files).Where(r => r.Id == fileId);
            var toDeleteFile = await toDeleteFiles.FirstOrDefaultAsync(r => r.CurrentVersion);

            foreach (var d in toDeleteFiles)
            {
                await _factoryIndexer.DeleteAsync(d);
            }

            await toDeleteFiles.ExecuteDeleteAsync();

            await Query(filesDbContext.Tag)
                .Where(r => !Query(filesDbContext.TagLink).Any(a => a.TagId == r.Id))
                .ExecuteDeleteAsync();

            await Query(filesDbContext.Security)
                .Where(r => r.EntryId == fileId.ToString())
                .Where(r => r.EntryType == FileEntryType.File)
                .ExecuteDeleteAsync();

            await tx.CommitAsync();

            foreach (var folderId in fromFolders)
            {
                await RecalculateFilesCountAsync(folderId);
            }

            if (deleteFolder)
            {
                await DeleteFolderAsync(fileId);
            }

            if (toDeleteFile != null)
            {
                await _factoryIndexer.DeleteAsync(toDeleteFile);
            }
        });
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        if (folderId is int fId)
        {
            return await IsExistAsync(title, fId);
        }

        throw new NotImplementedException();
    }

    public async Task<bool> IsExistAsync(string title, int folderId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
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
            return IdConverter.Convert<TTo>(await MoveFileAsync(fileId, tId));
        }

        if (toFolderId is string tsId)
        {
            return IdConverter.Convert<TTo>(await MoveFileAsync(fileId, tsId));
        }

        throw new NotImplementedException();
    }

    public async Task<int> MoveFileAsync(int fileId, int toFolderId)
    {
        if (fileId == default)
        {
            return default;
        }

        var trashIdTask = _globalFolder.GetFolderTrashAsync(_daoFactory);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var fromFolders = await Query(filesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Select(a => a.ParentId)
                .Distinct()
                .ToListAsync();

            var q = Query(filesDbContext.Files).Where(r => r.Id == fileId);

            using (var tx = await filesDbContext.Database.BeginTransactionAsync())
            {
                var trashId = await trashIdTask;
                var oldParentId = (await q.FirstOrDefaultAsync())?.ParentId;
                
                    if (trashId.Equals(toFolderId))
                    {
                    await q.ExecuteUpdateAsync(f => f
                    .SetProperty(p => p.ParentId, toFolderId)
                    .SetProperty(p => p.ModifiedBy, _authContext.CurrentAccount.ID)
                    .SetProperty(p => p.ModifiedOn, DateTime.UtcNow));
                }
                else
                {
                    await q.ExecuteUpdateAsync(f => f.SetProperty(p => p.ParentId, toFolderId));
                }

                var tagDao = _daoFactory.GetTagDao<int>();

                if (toFolderId == trashId && oldParentId.HasValue)
                {
                    var origin = Tag.Origin(fileId, FileEntryType.File, oldParentId.Value, _authContext.CurrentAccount.ID);
                    await tagDao.SaveTagsAsync(origin);
                }
                else if (oldParentId == trashId)
                {
                    await tagDao.RemoveTagLinksAsync(fileId, FileEntryType.File, TagType.Origin);
                }

                foreach (var f in fromFolders)
                {
                    await RecalculateFilesCountAsync(f);
                }

                await RecalculateFilesCountAsync(toFolderId);

                await tx.CommitAsync();
            }

            var toUpdateFile = await q.FirstOrDefaultAsync(r => r.CurrentVersion);

            if (toUpdateFile != null)
            {
                toUpdateFile.Folders = await filesDbContext.Tree
                .Where(r => r.FolderId == toFolderId)
                .OrderByDescending(r => r.Level)
                .ToListAsync();

                _factoryIndexer.Update(toUpdateFile, UpdateAction.Replace, w => w.Folders);
            }
        });

        return fileId;
    }

    public async Task<string> MoveFileAsync(int fileId, string toFolderId)
    {
        var toSelector = _selectorFactory.GetSelector(toFolderId);

        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, r => r,
            toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            true);

        return moved.Id;
    }

    public async Task<File<TTo>> CopyFileAsync<TTo>(int fileId, TTo toFolderId)
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

    public async Task<File<int>> CopyFileAsync(int fileId, int toFolderId)
    {
        var file = await GetFileAsync(fileId);
        if (file != null)
        {
            var copy = _serviceProvider.GetService<File<int>>();
            copy.FileStatus = file.FileStatus;
            copy.ParentId = toFolderId;
            copy.Title = file.Title;
            copy.ConvertedType = file.ConvertedType;
            copy.Comment = FilesCommonResource.CommentCopy;
            copy.Encrypted = file.Encrypted;
            copy.ThumbnailStatus = file.ThumbnailStatus == Thumbnail.Created ? Thumbnail.Creating : Thumbnail.Waiting;

            using (var stream = await GetFileStreamAsync(file))
            {
                copy.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                copy = await SaveFileAsync(copy, stream);
            }

            if (file.ThumbnailStatus == Thumbnail.Created)
            {
                foreach (var size in _thumbnailSettings.Sizes)
                {
                    await (await _globalStore.GetStoreAsync()).CopyAsync(String.Empty,
                                         GetUniqThumbnailPath(file, size.Width, size.Height),
                                         String.Empty,
                                         GetUniqThumbnailPath(copy, size.Width, size.Height));                 
                }

                await SetThumbnailStatusAsync(copy, Thumbnail.Created);

                copy.ThumbnailStatus = Thumbnail.Created;
            }

            return copy;
        }
        return null;
    }

    public async Task<File<string>> CopyFileAsync(int fileId, string toFolderId)
    {
        var toSelector = _selectorFactory.GetSelector(toFolderId);

        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, r => r,
            toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            false);

        return moved;
    }

    public async Task<int> FileRenameAsync(File<int> file, string newTitle)
    {
        newTitle = Global.ReplaceInvalidCharsAndTruncate(newTitle);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var toUpdate = await Query(filesDbContext.Files)
            .Where(r => r.Id == file.Id)
            .Where(r => r.CurrentVersion)
            .FirstOrDefaultAsync();

        toUpdate.Title = newTitle;
        toUpdate.ModifiedOn = DateTime.UtcNow;
        toUpdate.ModifiedBy = _authContext.CurrentAccount.ID;

        await filesDbContext.SaveChangesAsync();

        await _factoryIndexer.UpdateAsync(toUpdate, true, r => r.Title, r => r.ModifiedBy, r => r.ModifiedOn);

        return file.Id;
    }

    public async Task<string> UpdateCommentAsync(int fileId, int fileVersion, string comment)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        comment ??= string.Empty;
        comment = comment.Substring(0, Math.Min(comment.Length, 255));

        await Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Version == fileVersion)
            .ExecuteUpdateAsync(f => f.SetProperty(p => p.Comment, comment));

        return comment;
    }

    public async Task CompleteVersionAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Version > fileVersion)
            .ExecuteUpdateAsync(f => f.SetProperty(p => p.VersionGroup, p => p.VersionGroup + 1));
        }

    public async Task ContinueVersionAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var versionGroup = await Query(filesDbContext.Files)
                 .AsNoTracking()
                 .Where(r => r.Id == fileId)
                 .Where(r => r.Version == fileVersion)
                 .Select(r => r.VersionGroup)
                 .FirstOrDefaultAsync();

        await Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Version > fileVersion)
            .Where(r => r.VersionGroup > versionGroup)
            .ExecuteUpdateAsync(f => f.SetProperty(p => p.VersionGroup, p => p.VersionGroup - 1));
    }

    public bool UseTrashForRemove(File<int> file)
    {
        if (file.Encrypted && file.RootFolderType == FolderType.VirtualRooms)
        {
            return false;
        }

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

    private async Task RecalculateFilesCountAsync(int folderId)
    {
        await GetRecalculateFilesCountUpdateAsync(folderId);
    }

    #region chunking

    public async Task<ChunkedUploadSession<int>> CreateUploadSessionAsync(File<int> file, long contentLength)
    {
        return await _chunkedUploadSessionHolder.CreateUploadSessionAsync(file, contentLength);
    }

    public async Task<File<int>> UploadChunkAsync(ChunkedUploadSession<int> uploadSession, Stream stream, long chunkLength)
    {
        if (!uploadSession.UseChunks)
        {
            using var streamToSave = await _chunkedUploadSessionHolder.UploadSingleChunkAsync(uploadSession, stream, chunkLength);
            if (streamToSave != Stream.Null)
            {
                uploadSession.File = await SaveFileAsync(await GetFileForCommitAsync(uploadSession), streamToSave);
            }

            return uploadSession.File;
        }

        await _chunkedUploadSessionHolder.UploadChunkAsync(uploadSession, stream, chunkLength);

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal || uploadSession.LastChunk)
        {
            uploadSession.BytesTotal = uploadSession.BytesUploaded;
            uploadSession.File = await FinalizeUploadSessionAsync(uploadSession);
        }

        return uploadSession.File;
    }

    public async Task<File<int>> FinalizeUploadSessionAsync(ChunkedUploadSession<int> uploadSession)
    {
        await _chunkedUploadSessionHolder.FinalizeUploadSessionAsync(uploadSession);

        var file = await GetFileForCommitAsync(uploadSession);
        await SaveFileAsync(file, null, uploadSession.CheckQuota, uploadSession);

        return file;
    }

    public async Task AbortUploadSessionAsync(ChunkedUploadSession<int> uploadSession)
    {
        await _chunkedUploadSessionHolder.AbortUploadSessionAsync(uploadSession);
    }

    private async Task<File<int>> GetFileForCommitAsync(ChunkedUploadSession<int> uploadSession)
    {
        if (uploadSession.File.Id != default)
        {
            var file = await GetFileAsync(uploadSession.File.Id);
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
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Query(filesDbContext.Files)
            .Where(r => r.CurrentVersion)
            .Where(r => fileIds.Contains(r.Id))
            .ExecuteUpdateAsync(p => p.SetProperty(f => f.CreateBy, newOwnerId));
        }

    public IAsyncEnumerable<File<int>> GetFilesAsync(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
    {
        if (parentIds == null || !parentIds.Any() || filterType == FilterType.FoldersOnly)
        {
            return AsyncEnumerable.Empty<File<int>>();
        }

        return InternalGetFilesAsync(parentIds, filterType, subjectGroup, subjectID, searchText, searchInContent);
    }

    private async IAsyncEnumerable<File<int>> InternalGetFilesAsync(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();

        var q = GetFileQuery(filesDbContext, r => r.CurrentVersion)
            .AsNoTracking()
            .Join(filesDbContext.Tree, a => a.ParentId, t => t.FolderId, (file, tree) => new { file, tree })
            .Where(r => parentIds.Contains(r.tree.ParentId))
            .Select(r => r.file);

        if (!string.IsNullOrEmpty(searchText))
        {
            var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

            (var succ, var searchIds) = await _factoryIndexer.TrySelectIdsAsync(s => func(s));
            if (succ)
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
                var users = (await _userManager.GetUsersByGroupAsync(subjectID)).Select(u => u.Id).ToArray();
                q = q.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                q = q.Where(r => r.CreateBy == subjectID);
            }
        }

        switch (filterType)
        {
            case FilterType.OFormOnly:
            case FilterType.OFormTemplateOnly:
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

        await foreach (var e in FromQuery(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQuery, File<int>>(e);
        }
    }

    public async IAsyncEnumerable<File<int>> SearchAsync(string searchText, bool bunch = false)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        (var succ, var ids) = await _factoryIndexer.TrySelectIdsAsync(s => s.MatchAll(searchText));
        if (succ)
        {
            var query = GetFileQuery(filesDbContext, r => r.CurrentVersion && ids.Contains(r.Id)).AsNoTracking();

            var files = FromQuery(filesDbContext, query).AsAsyncEnumerable()
                .Select(e => _mapper.Map<DbFileQuery, File<int>>(e))
                .Where(
                    f =>
                    bunch
                        ? f.RootFolderType == FolderType.BUNCH
                        : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON);
            await foreach(var file in files)
            {
                yield return file;
            }
        }
        else
        {
            var query = BuildSearch(GetFileQuery(filesDbContext, r => r.CurrentVersion).AsNoTracking(), searchText, SearhTypeEnum.Any);

            var files = FromQuery(filesDbContext, query)
                .AsAsyncEnumerable()
                .Select(e => _mapper.Map<DbFileQuery, File<int>>(e))
                .Where(f =>
                       bunch
                            ? f.RootFolderType == FolderType.BUNCH
                            : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON);
            await foreach (var file in files)
            {
                yield return file;
            }
        }
    }

    private async Task DeleteFolderAsync(int fileId)
    {
        await (await _globalStore.GetStoreAsync()).DeleteDirectoryAsync(GetUniqFileDirectory(fileId));
    }

    public async Task<bool> IsExistOnStorageAsync(File<int> file)
    {
        return await (await _globalStore.GetStoreAsync()).IsFileAsync(string.Empty, GetUniqFilePath(file));
    }

    private const string DiffTitle = "diff.zip";

    public async Task SaveEditHistoryAsync(File<int> file, string changes, Stream differenceStream)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(changes);
        ArgumentNullException.ThrowIfNull(differenceStream);

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Query(filesDbContext.Files)
            .Where(r => r.Id == file.Id)
            .Where(r => r.Version == file.Version)
            .ExecuteUpdateAsync(f => f.SetProperty(p => p.Changes, changes.Trim()));

        await (await _globalStore.GetStoreAsync()).SaveAsync(string.Empty, GetUniqFilePath(file, DiffTitle), differenceStream, DiffTitle);
    }

    public async IAsyncEnumerable<EditHistory> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, int fileId, int fileVersion = 0)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var query = Query(filesDbContext.Files)
            .Where(r => r.Id == fileId)
            .Where(r => r.Forcesave == ForcesaveType.None);

        if (fileVersion > 0)
        {
            query = query.Where(r => r.Version == fileVersion);
        }

        query = query.OrderBy(r => r.Version);

        await foreach (var r in query.AsAsyncEnumerable())
        {
            var item = _serviceProvider.GetService<EditHistory>();

            item.ID = r.Id;
            item.Version = r.Version;
            item.VersionGroup = r.VersionGroup;
            item.ModifiedOn = _tenantUtil.DateTimeFromUtc(r.ModifiedOn);
            item.ModifiedBy = r.ModifiedBy;
            item.ChangesString = r.Changes;
            item.Key = await documentServiceHelper.GetDocKeyAsync(item.ID, item.Version, _tenantUtil.DateTimeFromUtc(r.CreateOn));

            yield return item;
        }
    }

    public async Task<Stream> GetDifferenceStreamAsync(File<int> file)
    {
        return await (await _globalStore.GetStoreAsync()).GetReadStreamAsync(string.Empty, GetUniqFilePath(file, DiffTitle), 0);
    }

    public async Task<bool> ContainChangesAsync(int fileId, int fileVersion)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        return await Query(filesDbContext.Files)
            .AnyAsync(r => r.Id == fileId &&
                      r.Version == fileVersion &&
                      r.Changes != null)
;
    }

    public async IAsyncEnumerable<FileWithShare> GetFeedsAsync(int tenant, DateTime from, DateTime to)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var q1 = filesDbContext.Files
            .Where(r => r.TenantId == tenant)
            .Where(r => r.CurrentVersion)
            .Where(r => r.ModifiedOn >= from && r.ModifiedOn <= to);

        var q2 = FromQuery(filesDbContext, q1)
            .Select(r => new DbFileQueryWithSecurity() { DbFileQuery = r, Security = null });

        var q3 = filesDbContext.Files
            .Where(r => r.TenantId == tenant)
            .Where(r => r.CurrentVersion);

        var q4 = FromQuery(filesDbContext, q3)
            .Join(filesDbContext.Security.DefaultIfEmpty(), r => r.File.Id.ToString(), s => s.EntryId, (f, s) => new DbFileQueryWithSecurity { DbFileQuery = f, Security = s })
            .Where(r => r.Security.TenantId == tenant)
            .Where(r => r.Security.EntryType == FileEntryType.File)
            .Where(r => r.Security.Share == FileShare.Restrict);
        //.Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to);

        await foreach (var e in q2.AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQueryWithSecurity, FileWithShare>(e);
        }

        await foreach (var e in q4.AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFileQueryWithSecurity, FileWithShare>(e);
        }
    }

    public async IAsyncEnumerable<int> GetTenantsWithFeedsAsync(DateTime fromTime, bool includeSecurity)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var q1Task = filesDbContext.Files
            .Where(r => r.ModifiedOn > fromTime)
            .Select(r => r.TenantId)
            .Distinct();

        await foreach (var q in q1Task.AsAsyncEnumerable())
        {
            yield return q;
        }

        if (includeSecurity)
        {
            var q2Task = filesDbContext.Security
            .Where(r => r.TimeStamp > fromTime)
            .Select(r => r.TenantId)
            .Distinct();

            await foreach (var q in q2Task.AsAsyncEnumerable())
            {
                yield return q;
            }
        }
    }

    private const string ThumbnailTitle = "thumb";


    public async Task SetThumbnailStatusAsync(File<int> file, Thumbnail status)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var (id, tenantId, version) = (file.Id, TenantID, file.Version);

        await filesDbContext.Files
                            .Where(r => r.Id == id && r.Version == version && r.TenantId == tenantId)
                            .ExecuteUpdateAsync(f => f.SetProperty(p => p.ThumbnailStatus, status));
    }


    public string GetUniqThumbnailPath(File<int> file, int width, int height)
    {
        var thumnailName = GetThumnailName(width, height);

        return GetUniqFilePath(file, thumnailName);
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
        var storage = await _globalStore.GetStoreAsync();
        var isFile = await storage.IsFileAsync(string.Empty, path);

        if (!isFile)
        {
            throw new FileNotFoundException();
        }

        return await storage.GetReadStreamAsync(string.Empty, path, 0);
    }

    private string GetThumnailName(int width, int height)
    {
        return $"{ThumbnailTitle}.{width}x{height}.{_global.ThumbnailExtension}";
    }

    public async Task<EntryProperties> GetProperties(int fileId)
    {
        var entryId = fileId.ToString();
        var tenantId = TenantID;

        using var filesDbContext = _dbContextFactory.CreateDbContext();
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

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        if (entryProperties == null || string.IsNullOrEmpty(data = EntryProperties.Serialize(entryProperties, _logger)))
        {
            await filesDbContext.FilesProperties
               .Where(r => r.TenantId == tenantId)
               .Where(r => r.EntryId == entryId)
               .ExecuteDeleteAsync();

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
                   var users = _userManager.GetUsersByGroupAsync(subjectID).Result.Select(u => u.Id).ToArray();
                   result.In(r => r.CreateBy, users);
               }
               else
               {
                   result.Where(r => r.CreateBy, subjectID);
               }
           }

           switch (filterType)
           {
               case FilterType.OFormOnly:
               case FilterType.OFormTemplateOnly:
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
        var fileType = FileEntryType.File;
        var denyArray = new[] { FileConstant.DenyDownloadId, FileConstant.DenySharingId };

        return from r in dbFiles
               select new DbFileQuery
               {
                   File = r,
                   Root = (from f in filesDbContext.Folders
                           where f.Id ==
                           (from t in filesDbContext.Tree
                            where t.FolderId == r.ParentId
                            orderby t.Level descending
                            select t.ParentId
                            ).FirstOrDefault()
                           where f.TenantId == r.TenantId
                           select f
                          ).FirstOrDefault(),
                   Shared = (from f in filesDbContext.Security
                             where f.EntryType == fileType && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId && !denyArray.Contains(f.Subject)
                             select f
                             ).Any()
               };
    }

    protected IQueryable<DbFileQuery> FromQuery(FilesDbContext filesDbContext, IQueryable<DbFile> dbFiles)
    {
        return dbFiles
            .Select(r => new DbFileQuery
            {
                File = r,
                Root = (from f in filesDbContext.Folders
                        where f.Id ==
                        (from t in filesDbContext.Tree
                         where t.FolderId == r.ParentId
                         orderby t.Level descending
                         select t.ParentId
                         ).FirstOrDefault()
                        where f.TenantId == r.TenantId
                        select f
                          ).FirstOrDefault()
            });
    }

    protected internal async Task<DbFile> InitDocumentAsync(DbFile dbFile)
    {
        if (!await _factoryIndexer.CanIndexByContentAsync(dbFile))
        {
            dbFile.Document = new Document
            {
                Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(""))
            };

            return dbFile;
        }

        return await InernalInitDocumentAsync(dbFile);
    }

    private async Task<DbFile> InernalInitDocumentAsync(DbFile dbFile)
    {
        var file = _serviceProvider.GetService<File<int>>();
        file.Id = dbFile.Id;
        file.Title = dbFile.Title;
        file.Version = dbFile.Version;
        file.ContentLength = dbFile.ContentLength;

        if (!await IsExistOnStorageAsync(file) || file.ContentLength > _settings.MaxContentLength)
        {
            return dbFile;
        }

        using var stream = await GetFileStreamAsync(file);

        if (stream == null)
        {
            return dbFile;
        }

        using (var ms = new MemoryStream())
        {
            await stream.CopyToAsync(ms);
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