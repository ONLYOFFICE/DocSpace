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

namespace ASC.Files.Core.Data;

[Scope]
internal class FolderDao : AbstractDao, IFolderDao<int>
{
    private const string My = "my";
    private const string Common = "common";
    private const string Share = "share";
    private const string Recent = "recent";
    private const string Favorites = "favorites";
    private const string Templates = "templates";
    private const string Privacy = "privacy";
    private const string Trash = "trash";
    private const string Projects = "projects";
    private const string VirtualRooms = "virtualrooms";
    private const string Archive = "archive";

    private readonly FactoryIndexerFolder _factoryIndexer;
    private readonly GlobalSpace _globalSpace;
    private readonly IDaoFactory _daoFactory;
    private readonly ProviderFolderDao _providerFolderDao;
    private readonly CrossDao _crossDao;
    private readonly IMapper _mapper;
    private readonly GlobalFolder _globalFolder;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly GlobalStore _globalStore;

    public FolderDao(
        FactoryIndexerFolder factoryIndexer,
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
        GlobalSpace globalSpace,
        IDaoFactory daoFactory,
        ProviderFolderDao providerFolderDao,
        CrossDao crossDao,
        IMapper mapper,
        GlobalStore globalStore,
        GlobalFolder globalFolder)
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
        _factoryIndexer = factoryIndexer;
        _globalSpace = globalSpace;
        _daoFactory = daoFactory;
        _providerFolderDao = providerFolderDao;
        _crossDao = crossDao;
        _mapper = mapper;
        _globalStore = globalStore;
        _globalFolder = globalFolder;
    }

    public async Task<Folder<int>> GetFolderAsync(int folderId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var query = GetFolderQuery(filesDbContext, r => r.Id == folderId).AsNoTracking();

        var dbFolder = await FromQueryWithShared(filesDbContext, query).Take(1).SingleOrDefaultAsync();

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public Task<Folder<int>> GetFolderAsync(string title, int parentId)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(title);

        return InternalGetFolderAsync(title, parentId);
    }

    private async Task<Folder<int>> InternalGetFolderAsync(string title, int parentId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var query = GetFolderQuery(filesDbContext, r => r.Title == title && r.ParentId == parentId).AsNoTracking()
            .OrderBy(r => r.CreateOn);

        var dbFolder = await FromQueryWithShared(filesDbContext, query).Take(1).FirstOrDefaultAsync();

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public async Task<Folder<int>> GetRootFolderAsync(int folderId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var id = await filesDbContext.Tree
            .AsNoTracking()
            .Where(r => r.FolderId == folderId)
            .OrderByDescending(r => r.Level)
            .Select(r => r.ParentId)
            .FirstOrDefaultAsync();

        var query = GetFolderQuery(filesDbContext, r => r.Id == id).AsNoTracking();

        var dbFolder = await FromQueryWithShared(filesDbContext, query).SingleOrDefaultAsync();

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public async Task<Folder<int>> GetRootFolderByFileAsync(int fileId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var subq = Query(filesDbContext.Files).AsNoTracking()
            .Where(r => r.Id == fileId && r.CurrentVersion)
            .Select(r => r.ParentId)
            .Distinct();

        var q = await filesDbContext.Tree.AsNoTracking()
            .Where(r => subq.Contains(r.FolderId))
            .OrderByDescending(r => r.Level)
            .Select(r => r.ParentId)
            .FirstOrDefaultAsync()
            ;

        var query = GetFolderQuery(filesDbContext, r => r.Id == q).AsNoTracking();

        var dbFolder = await FromQueryWithShared(filesDbContext, query).SingleOrDefaultAsync();

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId)
    {
        return GetFoldersAsync(parentId, default, FilterType.None, false, default, string.Empty);
    }

    public async IAsyncEnumerable<Folder<int>> GetRoomsAsync(IEnumerable<int> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter,
        IEnumerable<string> subjectEntriesIds)
    {
        if (CheckInvalidFilter(filterType) || provider != ProviderFilter.None)
        {
            yield break;
        }

        var filter = GetRoomTypeFilter(filterType);

        var searchByTags = tags != null && tags.Any() && !withoutTags;
        var searchByTypes = filterType != FilterType.None && filterType != FilterType.FoldersOnly;

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetFolderQuery(filesDbContext, r => parentsIds.Contains(r.ParentId)).AsNoTracking();

        q = !withSubfolders ? BuildRoomsQuery(filesDbContext, q, filter, tags, subjectId, searchByTags, withoutTags, searchByTypes, false, excludeSubject, subjectFilter, subjectEntriesIds)
            : BuildRoomsWithSubfoldersQuery(filesDbContext, parentsIds, filter, tags, searchByTags, searchByTypes, withoutTags, excludeSubject, subjectId, subjectFilter, subjectEntriesIds);

        if (!string.IsNullOrEmpty(searchText))
        {
            q = _factoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var searchIds)
                ? q.Where(r => searchIds.Contains(r.Id)) : BuildSearch(q, searchText, SearhTypeEnum.Any);
        }

        await foreach (var e in FromQueryWithShared(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
        }
    }

    public async IAsyncEnumerable<Folder<int>> GetRoomsAsync(IEnumerable<int> parentsIds, IEnumerable<int> roomsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        if (CheckInvalidFilter(filterType) || provider != ProviderFilter.None)
        {
            yield break;
        }

        var filter = GetRoomTypeFilter(filterType);

        var searchByTags = tags != null && tags.Any() && !withoutTags;
        var searchByTypes = filterType != FilterType.None && filterType != FilterType.FoldersOnly;

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetFolderQuery(filesDbContext, f => roomsIds.Contains(f.Id) || (f.CreateBy == _authContext.CurrentAccount.ID && parentsIds.Contains(f.ParentId))).AsNoTracking();

        q = !withSubfolders ? BuildRoomsQuery(filesDbContext, q, filter, tags, subjectId, searchByTags, withoutTags, searchByTypes, false, excludeSubject, subjectFilter, subjectEntriesIds)
            : BuildRoomsWithSubfoldersQuery(filesDbContext, roomsIds, filter, tags, searchByTags, searchByTypes, withoutTags, excludeSubject, subjectId, subjectFilter, subjectEntriesIds);

        if (!string.IsNullOrEmpty(searchText))
        {
            q = _factoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var searchIds)
                ? q.Where(r => searchIds.Contains(r.Id)) : BuildSearch(q, searchText, SearhTypeEnum.Any);
        }

        await foreach (var e in FromQueryWithShared(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
        }
    }

    public async Task<int> GetFoldersCountAsync(int parentId, FilterType filterType, bool subjectGroup, Guid subjectId, string searchText,
        bool withSubfolders = false, bool excludeSubject = false)
    {
        if (CheckInvalidFilter(filterType))
        {
            return 0;
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();

        if (filterType == FilterType.None && subjectId == default && string.IsNullOrEmpty(searchText) && !withSubfolders && !excludeSubject)
        {
            return await filesDbContext.Tree.CountAsync(r => r.ParentId == parentId && r.Level == 1);
        }

        var q = GetFoldersQueryWithFilters(parentId, null, subjectGroup, subjectId, searchText, withSubfolders, excludeSubject, filesDbContext);

        return await q.CountAsync();
    }

    public async IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false,
        bool excludeSubject = false, int offset = 0, int count = -1)
    {
        if (CheckInvalidFilter(filterType) || count == 0)
        {
            yield break;
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();

        var q = GetFoldersQueryWithFilters(parentId, orderBy, subjectGroup, subjectID, searchText, withSubfolders, excludeSubject, filesDbContext);

        q = q.Skip(offset);

        if (count > 0)
        {
            q = q.Take(count);
        }

        await foreach (var e in FromQueryWithShared(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
        }
    }

    public async IAsyncEnumerable<Folder<int>> GetFoldersAsync(IEnumerable<int> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true, bool excludeSubject = false)
    {
        if (CheckInvalidFilter(filterType))
        {
            yield break;
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetFolderQuery(filesDbContext, r => folderIds.Contains(r.Id)).AsNoTracking();

        if (searchSubfolders)
        {
            q = GetFolderQuery(filesDbContext).AsNoTracking()
                .Join(filesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
                .Where(r => folderIds.Contains(r.tree.ParentId))
                .Select(r => r.folder);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            if (_factoryIndexer.TrySelectIds(s =>
                                                searchSubfolders
                                                    ? s.MatchAll(searchText)
                                                    : s.MatchAll(searchText).In(r => r.Id, folderIds.ToArray()),
                                                out var searchIds))
            {
                q = q.Where(r => searchIds.Contains(r.Id));
            }
            else
            {
                q = BuildSearch(q, searchText, SearhTypeEnum.Any);
            }
        }


        if (subjectID.HasValue && subjectID != Guid.Empty)
        {
            if (subjectGroup)
            {
                var users = _userManager.GetUsersByGroup(subjectID.Value).Select(u => u.Id).ToArray();
                q = q.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                q = excludeSubject ? q.Where(r => r.CreateBy != subjectID) : q.Where(r => r.CreateBy == subjectID);
            }
        }

        await foreach (var e in (checkShare ? FromQueryWithShared(filesDbContext, q) : FromQuery(filesDbContext, q)).AsAsyncEnumerable().Distinct())
        {
            yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
        }
    }

    public async IAsyncEnumerable<Folder<int>> GetParentFoldersAsync(int folderId)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();

        var q = GetFolderQuery(filesDbContext)
            .AsNoTracking()
            .Join(filesDbContext.Tree, r => r.Id, a => a.ParentId, (folder, tree) => new { folder, tree })
            .Where(r => r.tree.FolderId == folderId)
            .OrderByDescending(r => r.tree.Level)
            .Select(r => r.folder);

        await foreach (var e in FromQueryWithShared(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
        }
    }

    public async IAsyncEnumerable<ParentRoomPair> GetParentRoomsAsync(IEnumerable<int> foldersIds)
    {
        var roomTypes = new List<FolderType> { FolderType.CustomRoom, FolderType.ReviewRoom, FolderType.FillingFormsRoom, FolderType.EditingRoom, FolderType.ReadOnlyRoom };

        var filesDbContext = _dbContextFactory.CreateDbContext();

        var q = GetFolderQuery(filesDbContext)
            .AsNoTracking()
            .Join(filesDbContext.Tree, r => r.Id, a => a.ParentId, (folder, tree) => new { folder, tree })
            .Where(r => foldersIds.Contains(r.tree.FolderId))
            .OrderByDescending(r => r.tree.Level)
            .Where(r => roomTypes.Contains(r.folder.FolderType))
            .Select(r => new ParentRoomPair { FolderId = r.tree.FolderId, ParentRoomId = r.folder.Id });

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return e;
        }
    }

    public Task<int> SaveFolderAsync(Folder<int> folder)
    {
        return SaveFolderAsync(folder, null);
    }

    private Task<int> SaveFolderAsync(Folder<int> folder, IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return InternalSaveFolderAsync(folder, transaction);
    }

    private async Task<int> InternalSaveFolderAsync(Folder<int> folder, IDbContextTransaction transaction)
    {
        var folderId = folder.Id;

        if (transaction == null)
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var filesDbContext = _dbContextFactory.CreateDbContext();
                using var tx = await filesDbContext.Database.BeginTransactionAsync();

                folderId = await InternalSaveFolderToDbAsync(folder);

                await tx.CommitAsync();
            });
        }
        else
        {
            folderId = await InternalSaveFolderToDbAsync(folder);
        }

        //FactoryIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, folder));
        return folderId;
    }

    public async Task<int> InternalSaveFolderToDbAsync(Folder<int> folder)
    {
        folder.Title = Global.ReplaceInvalidCharsAndTruncate(folder.Title);

        folder.ModifiedOn = _tenantUtil.DateTimeNow();
        folder.ModifiedBy = _authContext.CurrentAccount.ID;

        if (folder.CreateOn == default)
        {
            folder.CreateOn = _tenantUtil.DateTimeNow();
        }
        if (folder.CreateBy == default)
        {
            folder.CreateBy = _authContext.CurrentAccount.ID;
        }

        var isnew = false;

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        if (folder.Id != default && await IsExistAsync(folder.Id))
        {
            var toUpdate = await Query(filesDbContext.Folders)
                .Where(r => r.Id == folder.Id)
                .FirstOrDefaultAsync();

            toUpdate.Title = folder.Title;
            toUpdate.CreateBy = folder.CreateBy;
            toUpdate.ModifiedOn = _tenantUtil.DateTimeToUtc(folder.ModifiedOn);
            toUpdate.ModifiedBy = folder.ModifiedBy;
            toUpdate.HasLogo = folder.HasLogo;

            await filesDbContext.SaveChangesAsync();

            if (folder.FolderType == FolderType.DEFAULT || folder.FolderType == FolderType.BUNCH)
            {
                _ = _factoryIndexer.IndexAsync(toUpdate);
            }
        }
        else
        {
            isnew = true;
            var newFolder = new DbFolder
            {
                Id = 0,
                ParentId = folder.ParentId,
                Title = folder.Title,
                CreateOn = _tenantUtil.DateTimeToUtc(folder.CreateOn),
                CreateBy = folder.CreateBy,
                ModifiedOn = _tenantUtil.DateTimeToUtc(folder.ModifiedOn),
                ModifiedBy = folder.ModifiedBy,
                FolderType = folder.FolderType,
                Private = folder.Private,
                TenantId = TenantID
            };

            var entityEntry = await filesDbContext.Folders.AddAsync(newFolder);
            newFolder = entityEntry.Entity;
            await filesDbContext.SaveChangesAsync();

            if (folder.FolderType == FolderType.DEFAULT || folder.FolderType == FolderType.BUNCH)
            {
                _ = _factoryIndexer.IndexAsync(newFolder);
            }

            folder.Id = newFolder.Id;

            //itself link
            var newTree = new DbFolderTree
            {
                FolderId = folder.Id,
                ParentId = folder.Id,
                Level = 0
            };

            await filesDbContext.Tree.AddAsync(newTree);
            await filesDbContext.SaveChangesAsync();

            //full path to root
            var oldTree = filesDbContext.Tree
                .Where(r => r.FolderId == folder.ParentId);

            foreach (var o in oldTree)
            {
                var treeToAdd = new DbFolderTree
                {
                    FolderId = folder.Id,
                    ParentId = o.ParentId,
                    Level = o.Level + 1
                };

                await filesDbContext.Tree.AddAsync(treeToAdd);
            }

            await filesDbContext.SaveChangesAsync();
        }

        if (isnew)
        {
            await RecalculateFoldersCountAsync(folder.Id);
        }

        return folder.Id;

    }

    private async Task<bool> IsExistAsync(int folderId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        return await Query(filesDbContext.Folders).AsNoTracking()
            .AnyAsync(r => r.Id == folderId);
    }

    public Task DeleteFolderAsync(int folderId)
    {
        if (folderId == default)
        {
            throw new ArgumentNullException(nameof(folderId));
        }

        return InternalDeleteFolderAsync(folderId);
    }

    private async Task InternalDeleteFolderAsync(int id)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();
            var subfolders =
                await filesDbContext.Tree
                .Where(r => r.ParentId == id)
                .Select(r => r.FolderId)
                .ToListAsync();

            if (!subfolders.Contains(id))
            {
                subfolders.Add(id); // chashed folder_tree
            }

            var parent = await Query(filesDbContext.Folders)
                .Where(r => r.Id == id)
                .Select(r => r.ParentId)
                .FirstOrDefaultAsync();

            var folderToDelete = Query(filesDbContext.Folders).Where(r => subfolders.Contains(r.Id));

            foreach (var f in folderToDelete)
            {
                await _factoryIndexer.DeleteAsync(f);
            }

            await folderToDelete.ExecuteDeleteAsync();

            await filesDbContext.Tree
            .Where(r => subfolders.Contains(r.FolderId))
            .ExecuteDeleteAsync();

            var subfoldersStrings = subfolders.Select(r => r.ToString()).ToList();

            await Query(filesDbContext.TagLink)
                .Where(r => subfoldersStrings.Contains(r.EntryId))
                .Where(r => r.EntryType == FileEntryType.Folder)
                .ExecuteDeleteAsync();

            await Query(filesDbContext.Tag)
                .Where(r => !Query(filesDbContext.TagLink).Any(a => a.TagId == r.Id))
                .ExecuteDeleteAsync();

            var originToDelete = Query(filesDbContext.Tag)
                .Where(t => t.Name == id.ToString() || subfoldersStrings.Contains(t.Name));

            await Query(filesDbContext.TagLink)
                .Where(l => originToDelete.Select(t => t.Id).Contains(l.TagId))
                .ExecuteDeleteAsync();

            await originToDelete.ExecuteDeleteAsync();

            await Query(filesDbContext.Security)
                .Where(r => subfoldersStrings.Contains(r.EntryId))
                .Where(r => r.EntryType == FileEntryType.Folder)
                .ExecuteDeleteAsync();

            await Query(filesDbContext.BunchObjects)
                .Where(r => r.LeftNode == id.ToString())
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
            await RecalculateFoldersCountAsync(parent);
        });

        //FactoryIndexer.DeleteAsync(new FoldersWrapper { Id = id });
    }

    public async Task<TTo> MoveFolderAsync<TTo>(int folderId, TTo toFolderId, CancellationToken? cancellationToken)
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

    public async Task<int> MoveFolderAsync(int folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();
        var trashIdTask = _globalFolder.GetFolderTrashAsync<int>(_daoFactory);

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using (var tx = await filesDbContext.Database.BeginTransactionAsync())
            {
                var folder = await GetFolderAsync(folderId);
                var oldParentId = folder.ParentId;

                if (folder.FolderType != FolderType.DEFAULT && !DocSpaceHelper.IsRoom(folder.FolderType))
                {
                    throw new ArgumentException("It is forbidden to move the System folder.", nameof(folderId));
                }

                var recalcFolders = new List<int> { toFolderId };
                var parent = await filesDbContext.Folders
                    .Where(r => r.Id == folderId)
                    .Select(r => r.ParentId)
                    .FirstOrDefaultAsync();

                if (parent != 0 && !recalcFolders.Contains(parent))
                {
                    recalcFolders.Add(parent);
                }

                await Query(filesDbContext.Folders)
                    .Where(r => r.Id == folderId)
                    .ExecuteUpdateAsync(toUpdate => toUpdate
                    .SetProperty(p => p.ParentId, toFolderId)
                    .SetProperty(p => p.ModifiedOn, DateTime.UtcNow)
                    .SetProperty(p => p.ModifiedBy, _authContext.CurrentAccount.ID)
                    );

                var subfolders = await filesDbContext.Tree
                    .Where(r => r.ParentId == folderId)
                    .ToDictionaryAsync(r => r.FolderId, r => r.Level);

#pragma warning disable CA1841 // Prefer Dictionary.Contains methods
                await filesDbContext.Tree
                    .Where(r => subfolders.Keys.Contains(r.FolderId) && !subfolders.Keys.Contains(r.ParentId))
                    .ExecuteDeleteAsync();
#pragma warning restore CA1841 // Prefer Dictionary.Contains methods

                var toInsert = await filesDbContext.Tree
                    .Where(r => r.FolderId == toFolderId)
                    .OrderBy(r => r.Level)
                    .ToListAsync();

                foreach (var subfolder in subfolders)
                {
                    foreach (var f in toInsert)
                    {
                        var newTree = new DbFolderTree
                        {
                            FolderId = subfolder.Key,
                            ParentId = f.ParentId,
                            Level = subfolder.Value + 1 + f.Level
                        };
                        await filesDbContext.AddOrUpdateAsync(r => r.Tree, newTree);
                    }
                }

                var trashId = await trashIdTask;
                var tagDao = _daoFactory.GetTagDao<int>();

                if (toFolderId == trashId)
                {
                    var origin = Tag.Origin(folderId, FileEntryType.Folder, oldParentId, _authContext.CurrentAccount.ID);
                    await tagDao.SaveTags(origin);
                }
                else if (oldParentId == trashId)
                {
                    await tagDao.RemoveTagLinksAsync(folderId, FileEntryType.Folder, TagType.Origin);
                }

                await filesDbContext.SaveChangesAsync();
                await tx.CommitAsync();

                foreach (var e in recalcFolders)
                {
                    await RecalculateFoldersCountAsync(e);
                }
                foreach (var e in recalcFolders)
                {
                    await GetRecalculateFilesCountUpdateAsync(e);
                }
            }
        });

        return folderId;
    }

    public async Task<string> MoveFolderAsync(int folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var toSelector = _providerFolderDao.GetSelector(toFolderId);

        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
            folderId, this, _daoFactory.GetFileDao<int>(), r => r,
            toFolderId, toSelector.GetFolderDao(toFolderId), toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            true, cancellationToken)
            ;

        return moved.Id;
    }

    public async Task<Folder<TTo>> CopyFolderAsync<TTo>(int folderId, TTo toFolderId, CancellationToken? cancellationToken)
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

    public async Task<Folder<int>> CopyFolderAsync(int folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var folder = await GetFolderAsync(folderId);

        var toFolder = await GetFolderAsync(toFolderId);

        if (folder.FolderType == FolderType.BUNCH)
        {
            folder.FolderType = FolderType.DEFAULT;
        }

        var copy = _serviceProvider.GetService<Folder<int>>();
        copy.ParentId = toFolderId;
        copy.RootId = toFolder.RootId;
        copy.RootCreateBy = toFolder.RootCreateBy;
        copy.RootFolderType = toFolder.RootFolderType;
        copy.Title = folder.Title;
        copy.FolderType = folder.FolderType;

        copy = await GetFolderAsync(await SaveFolderAsync(copy));

        //FactoryIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, copy));
        return copy;
    }

    public async Task<Folder<string>> CopyFolderAsync(int folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var toSelector = _providerFolderDao.GetSelector(toFolderId);

        var moved = await _crossDao.PerformCrossDaoFolderCopyAsync(
            folderId, this, _daoFactory.GetFileDao<int>(), r => r,
            toFolderId, toSelector.GetFolderDao(toFolderId), toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            false, cancellationToken)
            ;

        return moved;
    }

    public async Task<IDictionary<int, TTo>> CanMoveOrCopyAsync<TTo>(int[] folderIds, TTo to)
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

    public Task<IDictionary<int, TTo>> CanMoveOrCopyAsync<TTo>(int[] folderIds, string to)
    {
        return Task.FromResult((IDictionary<int, TTo>)new Dictionary<int, TTo>());
    }

    public async Task<IDictionary<int, TTo>> CanMoveOrCopyAsync<TTo>(int[] folderIds, int to)
    {
        if (typeof(TTo) == typeof(string))
        {
            return new Dictionary<int, TTo>();
        }

        var result = new Dictionary<int, TTo>();

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        foreach (var folderId in folderIds)
        {
            var exists = await filesDbContext.Tree
                .AsNoTracking()
                .AnyAsync(r => r.ParentId == folderId && r.FolderId == to)
                ;

            if (exists)
            {
                throw new InvalidOperationException(FilesCommonResource.ErrorMassage_FolderCopyError);
            }

            var title = await Query(filesDbContext.Folders)
                .AsNoTracking()
                .Where(r => r.Id == folderId)
                .Select(r => r.Title.ToLower())
                .FirstOrDefaultAsync();

            var conflict = await Query(filesDbContext.Folders)
                .AsNoTracking()
                .Where(r => r.Title.ToLower() == title)
                .Where(r => r.ParentId == to)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (conflict != 0)
            {
                var files = await filesDbContext.Files
                    .AsNoTracking()
                    .Join(filesDbContext.Files, f1 => f1.Title.ToLower(), f2 => f2.Title.ToLower(), (f1, f2) => new { f1, f2 })
                    .Where(r => r.f1.TenantId == TenantID && r.f1.CurrentVersion && r.f1.ParentId == folderId)
                    .Where(r => r.f2.TenantId == TenantID && r.f2.CurrentVersion && r.f2.ParentId == conflict)
                    .Select(r => r.f1)
                    .ToListAsync();

                foreach (var file in files)
                {
                    result[file.Id] = (TTo)Convert.ChangeType(conflict, typeof(TTo));
                }

                var childs = await Query(filesDbContext.Folders)
                    .AsNoTracking()
                    .Where(r => r.ParentId == folderId)
                    .Select(r => r.Id)
                    .ToArrayAsync();

                foreach (var pair in await CanMoveOrCopyAsync(childs, conflict))
                {
                    result.Add(pair.Key, (TTo)Convert.ChangeType(pair.Value, typeof(TTo)));
                }
            }
        }

        return result;
    }

    public async Task<int> RenameFolderAsync(Folder<int> folder, string newTitle)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var toUpdate = await Query(filesDbContext.Folders)
            .Where(r => r.Id == folder.Id)
            .FirstOrDefaultAsync();

        toUpdate.Title = Global.ReplaceInvalidCharsAndTruncate(newTitle);
        toUpdate.ModifiedOn = DateTime.UtcNow;
        toUpdate.ModifiedBy = _authContext.CurrentAccount.ID;

        await filesDbContext.SaveChangesAsync();

        _ = _factoryIndexer.IndexAsync(toUpdate);

        return folder.Id;
    }

    public async Task<int> GetItemsCountAsync(int folderId)
    {
        return await GetFoldersCountAsync(folderId) +
               await GetFilesCountAsync(folderId);
    }

    private async Task<int> GetFoldersCountAsync(int parentId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var count = await filesDbContext.Tree
            .CountAsync(r => r.ParentId == parentId && r.Level > 0);

        return count;
    }

    private async Task<int> GetFilesCountAsync(int folderId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        return await Query(filesDbContext.Files)
            .Join(filesDbContext.Tree, r => r.ParentId, r => r.FolderId, (file, tree) => new { tree, file })
            .Where(r => r.tree.ParentId == folderId)
            .Select(r => r.file.Id)
            .Distinct()
            .CountAsync();
    }

    public async Task<bool> IsEmptyAsync(int folderId)
    {
        return await GetItemsCountAsync(folderId) == 0;
    }

    public bool UseTrashForRemoveAsync(Folder<int> folder)
    {
        return folder.RootFolderType != FolderType.TRASH && folder.RootFolderType != FolderType.Privacy && folder.FolderType != FolderType.BUNCH && !folder.Private;
    }

    public bool UseRecursiveOperation(int folderId, string toRootFolderId)
    {
        return true;
    }

    public bool UseRecursiveOperation(int folderId, int toRootFolderId)
    {
        return true;
    }

    public bool UseRecursiveOperation<TTo>(int folderId, TTo toRootFolderId)
    {
        return true;
    }

    public bool CanCalculateSubitems(int entryId)
    {
        return true;
    }

    public async Task<long> GetMaxUploadSizeAsync(int folderId, bool chunkedUpload = false)
    {
        var tmp = long.MaxValue;

        if (_coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            tmp = _coreConfiguration.PersonalMaxSpace(_settingsManager) - await _globalSpace.GetUserUsedSpaceAsync();
        }

        return Math.Min(tmp, chunkedUpload ?
            await _setupInfo.MaxChunkedUploadSize(_tenantManager, _maxTotalSizeStatistic) :
            await _setupInfo.MaxUploadSize(_tenantManager, _maxTotalSizeStatistic));
    }

    private async Task RecalculateFoldersCountAsync(int id)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        //ExecuteUpdate
        var toUpdate = await Query(filesDbContext.Folders)
            .Join(filesDbContext.Tree, r => r.Id, r => r.ParentId, (file, tree) => new { file, tree })
            .Where(r => r.tree.FolderId == id)
            .Select(r => r.file)
            .ToListAsync();

        foreach (var f in toUpdate)
        {
            var count = await filesDbContext.Tree.Where(r => r.ParentId == f.Id).CountAsync() - 1;
            f.FoldersCount = count;
        }

        await filesDbContext.SaveChangesAsync();
    }

    #region Only for TMFolderDao

    public async Task ReassignFoldersAsync(int[] folderIds, Guid newOwnerId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Query(filesDbContext.Folders)
            .Where(r => folderIds.Contains(r.Id))
            .ExecuteUpdateAsync(f => f.SetProperty(p => p.CreateBy, newOwnerId));
    }

    public async IAsyncEnumerable<Folder<int>> SearchFoldersAsync(string text, bool bunch = false)
    {
        var folders = SearchAsync(text);

        await foreach (var f in folders)
        {
            if (bunch ? f.RootFolderType == FolderType.BUNCH
            : (f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON))
            {
                yield return f;
            }
        }
    }

    private async IAsyncEnumerable<Folder<int>> SearchAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();

        if (_factoryIndexer.TrySelectIds(s => s.MatchAll(text), out var ids))
        {
            var q1 = GetFolderQuery(filesDbContext, r => ids.Contains(r.Id));

            await foreach (var e in FromQueryWithShared(filesDbContext, q1).AsAsyncEnumerable())
            {
                yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
            }

            yield break;
        }

        var q = BuildSearch(GetFolderQuery(filesDbContext), text, SearhTypeEnum.Any);

        await foreach (var e in FromQueryWithShared(filesDbContext, q).AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQuery, Folder<int>>(e);
        }
    }

    public IAsyncEnumerable<int> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(module);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(bunch);

        return InternalGetFolderIDsAsync(module, bunch, data, createIfNotExists);
    }

    private async IAsyncEnumerable<int> InternalGetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
    {
        var keys = data.Select(id => $"{module}/{bunch}/{id}").ToArray();

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var folderIdsDictionary = await Query(filesDbContext.BunchObjects)
            .AsNoTracking()
            .Where(r => keys.Length > 1 ? keys.Any(a => a == r.RightNode) : r.RightNode == keys[0])
            .ToDictionaryAsync(r => r.RightNode, r => r.LeftNode);

        foreach (var key in keys)
        {
            var newFolderId = 0;
            if (createIfNotExists && !folderIdsDictionary.TryGetValue(key, out var folderId))
            {
                var folder = _serviceProvider.GetService<Folder<int>>();
                switch (bunch)
                {
                    case My:
                        folder.FolderType = FolderType.USER;
                        folder.Title = My;
                        break;
                    case Common:
                        folder.FolderType = FolderType.COMMON;
                        folder.Title = Common;
                        break;
                    case Trash:
                        folder.FolderType = FolderType.TRASH;
                        folder.Title = Trash;
                        break;
                    case Share:
                        folder.FolderType = FolderType.SHARE;
                        folder.Title = Share;
                        break;
                    case Recent:
                        folder.FolderType = FolderType.Recent;
                        folder.Title = Recent;
                        break;
                    case Favorites:
                        folder.FolderType = FolderType.Favorites;
                        folder.Title = Favorites;
                        break;
                    case Templates:
                        folder.FolderType = FolderType.Templates;
                        folder.Title = Templates;
                        break;
                    case Privacy:
                        folder.FolderType = FolderType.Privacy;
                        folder.Title = Privacy;
                        break;
                    case Projects:
                        folder.FolderType = FolderType.Projects;
                        folder.Title = Projects;
                        break;
                    case VirtualRooms:
                        folder.FolderType = FolderType.VirtualRooms;
                        folder.Title = VirtualRooms;
                        break;
                    case Archive:
                        folder.FolderType = FolderType.Archive;
                        folder.Title = Archive;
                        break;
                    default:
                        folder.FolderType = FolderType.BUNCH;
                        folder.Title = key;
                        break;
                }

                var strategy = filesDbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var filesDbContext = _dbContextFactory.CreateDbContext();
                    using var tx = await filesDbContext.Database.BeginTransactionAsync();//NOTE: Maybe we shouldn't start transaction here at all

                    newFolderId = await SaveFolderAsync(folder, tx); //Save using our db manager

                    var newBunch = new DbFilesBunchObjects
                    {
                        LeftNode = newFolderId.ToString(),
                        RightNode = key,
                        TenantId = TenantID
                    };

                    await filesDbContext.AddOrUpdateAsync(r => r.BunchObjects, newBunch);
                    await filesDbContext.SaveChangesAsync();

                    await tx.CommitAsync(); //Commit changes
                });
            }

            yield return newFolderId;
        }
    }

    public Task<int> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(module);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(bunch);

        return InternalGetFolderIDAsync(module, bunch, data, createIfNotExists);
    }

    private async Task<int> InternalGetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var key = $"{module}/{bunch}/{data}";
        var folderId = await InternalGetFolderIDAsync(key);

        if (folderId != null)
        {
            return Convert.ToInt32(folderId);
        }

        var newFolderId = 0;
        if (createIfNotExists)
        {
            await _semaphore.WaitAsync();
            folderId = await InternalGetFolderIDAsync(key);

            if (folderId != null)
            {
                _semaphore.Release();
                return Convert.ToInt32(folderId);
            }

            var folder = _serviceProvider.GetService<Folder<int>>();
            folder.ParentId = 0;
            switch (bunch)
            {
                case My:
                    folder.FolderType = FolderType.USER;
                    folder.Title = My;
                    folder.CreateBy = new Guid(data);
                    break;
                case Common:
                    folder.FolderType = FolderType.COMMON;
                    folder.Title = Common;
                    break;
                case Trash:
                    folder.FolderType = FolderType.TRASH;
                    folder.Title = Trash;
                    folder.CreateBy = new Guid(data);
                    break;
                case Share:
                    folder.FolderType = FolderType.SHARE;
                    folder.Title = Share;
                    break;
                case Recent:
                    folder.FolderType = FolderType.Recent;
                    folder.Title = Recent;
                    break;
                case Favorites:
                    folder.FolderType = FolderType.Favorites;
                    folder.Title = Favorites;
                    break;
                case Templates:
                    folder.FolderType = FolderType.Templates;
                    folder.Title = Templates;
                    break;
                case Privacy:
                    folder.FolderType = FolderType.Privacy;
                    folder.Title = Privacy;
                    folder.CreateBy = new Guid(data);
                    break;
                case Projects:
                    folder.FolderType = FolderType.Projects;
                    folder.Title = Projects;
                    break;
                case VirtualRooms:
                    folder.FolderType = FolderType.VirtualRooms;
                    folder.Title = VirtualRooms;
                    break;
                case Archive:
                    folder.FolderType = FolderType.Archive;
                    folder.Title = Archive;
                    break;
                default:
                    folder.FolderType = FolderType.BUNCH;
                    folder.Title = key;
                    break;
            }

            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var filesDbContext = _dbContextFactory.CreateDbContext();
                using var tx = await filesDbContext.Database.BeginTransactionAsync(); //NOTE: Maybe we shouldn't start transaction here at all
                newFolderId = await SaveFolderAsync(folder, tx); //Save using our db manager
                var toInsert = new DbFilesBunchObjects
                {
                    LeftNode = newFolderId.ToString(),
                    RightNode = key,
                    TenantId = TenantID
                };

                await filesDbContext.AddOrUpdateAsync(r => r.BunchObjects, toInsert);
                await filesDbContext.SaveChangesAsync();

                await tx.CommitAsync(); //Commit changes
            });
            _semaphore.Release();
        }

        return newFolderId;
    }

    private async Task<string> InternalGetFolderIDAsync(string key)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        return await Query(filesDbContext.BunchObjects)
            .Where(r => r.RightNode == key)
            .Select(r => r.LeftNode)
            .FirstOrDefaultAsync();
    }

    Task<int> IFolderDao<int>.GetFolderIDProjectsAsync(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Projects, null, createIfNotExists);
    }

    public Task<int> GetFolderIDTrashAsync(bool createIfNotExists, Guid? userId = null)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Trash, (userId ?? _authContext.CurrentAccount.ID).ToString(), createIfNotExists);
    }

    public Task<int> GetFolderIDCommonAsync(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Common, null, createIfNotExists);
    }

    public Task<int> GetFolderIDUserAsync(bool createIfNotExists, Guid? userId = null)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, My, (userId ?? _authContext.CurrentAccount.ID).ToString(), createIfNotExists);
    }

    public Task<int> GetFolderIDShareAsync(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Share, null, createIfNotExists);
    }

    public Task<int> GetFolderIDRecentAsync(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Recent, null, createIfNotExists);
    }

    public Task<int> GetFolderIDFavoritesAsync(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Favorites, null, createIfNotExists);
    }

    public Task<int> GetFolderIDTemplatesAsync(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Templates, null, createIfNotExists);
    }

    public Task<int> GetFolderIDPrivacyAsync(bool createIfNotExists, Guid? userId = null)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Privacy, (userId ?? _authContext.CurrentAccount.ID).ToString(), createIfNotExists);
    }

    public Task<int> GetFolderIDVirtualRooms(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, VirtualRooms, null, createIfNotExists);
    }

    public Task<int> GetFolderIDArchive(bool createIfNotExists)
    {
        return (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, Archive, null, createIfNotExists);
    }

    public IAsyncEnumerable<OriginData> GetOriginsDataAsync(IEnumerable<int> entriesIds)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();

        return _getOriginsDataQuery(filesDbContext, entriesIds, TenantID);
    }

    #endregion

    protected internal IQueryable<DbFolder> GetFolderQuery(FilesDbContext filesDbContext, Expression<Func<DbFolder, bool>> where = null)
    {
        var q = Query(filesDbContext.Folders);
        if (where != null)
        {
            q = q.Where(where);
        }

        return q;
    }

    protected IQueryable<DbFolderQuery> FromQueryWithShared(FilesDbContext filesDbContext, IQueryable<DbFolder> dbFiles)
    {
        var e = from r in dbFiles
                select new DbFolderQuery
                {
                    Folder = r,
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
                };

        return e;
    }

    protected IQueryable<DbFolderQuery> FromQuery(FilesDbContext filesDbContext, IQueryable<DbFolder> dbFiles)
    {
        var FilesDbContext = filesDbContext;

        return dbFiles
            .Select(r => new DbFolderQuery
            {
                Folder = r,
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
            });
    }

    public async Task<string> GetBunchObjectIDAsync(int folderID)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        return await Query(filesDbContext.BunchObjects)
            .Where(r => r.LeftNode == folderID.ToString())
            .Select(r => r.RightNode)
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<int> folderIDs)
    {
        var folderSIds = folderIDs.Select(r => r.ToString()).ToList();

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        return await Query(filesDbContext.BunchObjects)
            .Where(r => folderSIds.Any(a => a == r.LeftNode))
            .ToDictionaryAsync(r => r.LeftNode, r => r.RightNode);
    }

    public async IAsyncEnumerable<FolderWithShare> GetFeedsForRoomsAsync(int tenant, DateTime from, DateTime to)
    {
        var roomTypes = new List<FolderType> { FolderType.CustomRoom, FolderType.ReviewRoom, FolderType.FillingFormsRoom, FolderType.EditingRoom, FolderType.ReadOnlyRoom };
        Expression<Func<DbFolder, bool>> filter = f => roomTypes.Contains(f.FolderType);

        await foreach (var e in GetFeedsInternalAsync(tenant, from, to, filter, null))
        {
            yield return e;
        }
    }

    public async IAsyncEnumerable<FolderWithShare> GetFeedsForFoldersAsync(int tenant, DateTime from, DateTime to)
    {
        Expression<Func<DbFolder, bool>> foldersFilter = f => f.FolderType == FolderType.DEFAULT;
        Expression<Func<DbFolderQueryWithSecurity, bool>> securityFilter = f => f.Security.Share == FileShare.Restrict;


        await foreach (var e in GetFeedsInternalAsync(tenant, from, to, foldersFilter, securityFilter))
        {
            yield return e;
        }
    }

    public async IAsyncEnumerable<FolderWithShare> GetFeedsInternalAsync(int tenant, DateTime from, DateTime to, Expression<Func<DbFolder, bool>> foldersFilter,
        Expression<Func<DbFolderQueryWithSecurity, bool>> securityFilter)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var q1 = filesDbContext.Folders
            .Where(r => r.TenantId == tenant)
            .Where(foldersFilter)
            .Where(r => r.CreateOn >= from && r.ModifiedOn <= to);

        var q2 = FromQuery(filesDbContext, q1)
            .Select(r => new DbFolderQueryWithSecurity() { DbFolderQuery = r, Security = null });

        var q3 = filesDbContext.Folders
            .Where(r => r.TenantId == tenant)
            .Where(foldersFilter);

        var q4 = FromQuery(filesDbContext, q3)
            .Join(filesDbContext.Security.DefaultIfEmpty(), r => r.Folder.Id.ToString(), s => s.EntryId, (f, s) => new DbFolderQueryWithSecurity { DbFolderQuery = f, Security = s })
            .Where(r => r.Security.TenantId == tenant)
            .Where(r => r.Security.EntryType == FileEntryType.Folder)
            .Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to);

        if (securityFilter != null)
        {
            q4 = q4.Where(securityFilter);
        }

        await foreach (var e in q2.AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQueryWithSecurity, FolderWithShare>(e);
        }

        await foreach (var e in q4.AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFolderQueryWithSecurity, FolderWithShare>(e);
        }
    }

    public async IAsyncEnumerable<int> GetTenantsWithFoldersFeedsAsync(DateTime fromTime)
    {
        Expression<Func<DbFolder, bool>> filter = f => f.FolderType == FolderType.DEFAULT;

        await foreach (var q in GetTenantsWithFeeds(fromTime, filter, false))
        {
            yield return q;
        }
    }

    public async IAsyncEnumerable<int> GetTenantsWithRoomsFeedsAsync(DateTime fromTime)
    {
        var roomTypes = new List<FolderType> { FolderType.CustomRoom, FolderType.ReviewRoom, FolderType.FillingFormsRoom, FolderType.EditingRoom, FolderType.ReadOnlyRoom };
        Expression<Func<DbFolder, bool>> filter = f => roomTypes.Contains(f.FolderType);

        await foreach (var q in GetTenantsWithFeeds(fromTime, filter, true))
        {
            yield return q;
        }
    }

    public IAsyncEnumerable<Folder<int>> GetFakeRoomsAsync(IEnumerable<int> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText,
        bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        return AsyncEnumerable.Empty<Folder<int>>();
    }

    public IAsyncEnumerable<Folder<int>> GetFakeRoomsAsync(IEnumerable<int> parentsIds, IEnumerable<int> roomsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId,
        string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        return AsyncEnumerable.Empty<Folder<int>>();
    }

    public async Task<(int RoomId, string RoomTitle)> GetParentRoomInfoFromFileEntryAsync<TTo>(FileEntry<TTo> fileEntry)
    {
        var rootFolderType = fileEntry.RootFolderType;

        if (rootFolderType != FolderType.VirtualRooms && rootFolderType != FolderType.Archive)
        {
            return (-1, "");
        }

        var rootFolderId = Convert.ToInt32(fileEntry.RootId);
        var entryId = Convert.ToInt32(fileEntry.Id);

        if (rootFolderId == entryId)
        {
            return (-1, "");
        }

        var folderId = Convert.ToInt32(fileEntry.ParentId);

        if (rootFolderId == folderId)
        {
            return (entryId, fileEntry.Title);
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var parentFolders = await filesDbContext.Tree
            .Join(filesDbContext.Folders, r => r.ParentId, s => s.Id, (t, f) => new { Tree = t, Folders = f })
            .Where(r => r.Tree.FolderId == folderId)
            .OrderByDescending(r => r.Tree.Level)
            .Select(r => new { r.Tree.ParentId, r.Folders.Title })
            .ToListAsync();

        if (parentFolders.Count > 1)
        {
            return (parentFolders[1].ParentId, parentFolders[1].Title);
        }

        return (parentFolders[0].ParentId, parentFolders[0].Title);
    }

    private async IAsyncEnumerable<int> GetTenantsWithFeeds(DateTime fromTime, Expression<Func<DbFolder, bool>> filter, bool includeSecurity)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var q1 = filesDbContext.Folders
            .Where(r => r.ModifiedOn > fromTime)
            .Where(filter)
            .Select(r => r.TenantId).Distinct();

        await foreach (var q in q1.AsAsyncEnumerable())
        {
            yield return q;
        }

        if (includeSecurity)
        {
            var q2 = filesDbContext.Security.Where(r => r.TimeStamp > fromTime).Select(r => r.TenantId).Distinct();

            await foreach (var q in q2.AsAsyncEnumerable())
            {
                yield return q;
            }
        }
    }

    private IQueryable<DbFolder> BuildRoomsQuery(FilesDbContext filesDbContext, IQueryable<DbFolder> query, FolderType filterByType, IEnumerable<string> tags, Guid subjectId, bool searchByTags, bool withoutTags,
        bool searchByFilter, bool withSubfolders, bool excludeSubject, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        if (subjectId != Guid.Empty)
        {
            if (subjectFilter == SubjectFilter.Owner)
            {
                query = excludeSubject ? query.Where(f => f.CreateBy != subjectId) : query.Where(f => f.CreateBy == subjectId);
            }
            else if (subjectFilter == SubjectFilter.Member)
            {
                query = excludeSubject ? query.Where(f => f.CreateBy != subjectId && !subjectEntriesIds.Contains(f.Id.ToString()))
                    : query.Where(f => f.CreateBy == subjectId || subjectEntriesIds.Contains(f.Id.ToString()));
            }
        }

        if (searchByFilter)
        {
            query = query.Where(f => f.FolderType == filterByType);
        }

        if (withoutTags)
        {
            query = query.Where(f => !filesDbContext.TagLink.Join(filesDbContext.Tag, l => l.TagId, t => t.Id, (link, tag) => new { link.EntryId, tag })
                .Where(r => r.tag.Type == TagType.Custom).Any(t => t.EntryId == f.Id.ToString()));
        }

        if (searchByTags && !withSubfolders)
        {
            query = query.Join(filesDbContext.TagLink, f => f.Id.ToString(), t => t.EntryId, (folder, tag) => new { folder, tag.TagId })
                .Join(filesDbContext.Tag, r => r.TagId, t => t.Id, (result, tagInfo) => new { result.folder, result.TagId, tagInfo.Name })
                .Where(r => tags.Contains(r.Name))
                .Select(r => r.folder).Distinct();
        }

        return query;
    }

    private IQueryable<DbFolder> BuildRoomsWithSubfoldersQuery(FilesDbContext filesDbContext, int parentId, FolderType filterByType, IEnumerable<string> tags, bool searchByTags, bool searchByFilter, bool withoutTags,
        bool excludeSubject, Guid subjectId, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        var q1 = GetFolderQuery(filesDbContext, r => r.ParentId == parentId).AsNoTracking();

        q1 = BuildRoomsQuery(filesDbContext, q1, filterByType, tags, subjectId, searchByTags, withoutTags, searchByFilter, true, excludeSubject, subjectFilter, subjectEntriesIds);

        if (searchByTags)
        {
            var q2 = q1.Join(filesDbContext.TagLink, f => f.Id.ToString(), t => t.EntryId, (folder, tagLink) => new { folder, tagLink.TagId })
                .Join(filesDbContext.Tag, r => r.TagId, t => t.Id, (result, tag) => new { result.folder, tag.Name })
                .Where(r => tags.Contains(r.Name))
                .Select(r => r.folder.Id).Distinct();

            return GetFolderQuery(filesDbContext).AsNoTracking().Join(filesDbContext.Tree, f => f.Id, t => t.FolderId, (folder, tree) => new { folder, tree })
                .Where(r => q2.Contains(r.tree.ParentId))
                .Select(r => r.folder);
        }

        if (!searchByFilter && !searchByTags && !withoutTags && !excludeSubject)
        {
            return GetFolderQuery(filesDbContext).AsNoTracking()
                .Join(filesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
                .Where(r => r.tree.ParentId == parentId && r.tree.Level != 0)
                .Select(r => r.folder);
        }

        return GetFolderQuery(filesDbContext).AsNoTracking().Join(filesDbContext.Tree, f => f.Id, t => t.FolderId, (folder, tree) => new { folder, tree })
                    .Where(r => q1.Select(f => f.Id).Contains(r.tree.ParentId))
                    .Select(r => r.folder);
    }

    private IQueryable<DbFolder> BuildRoomsWithSubfoldersQuery(FilesDbContext filesDbContext, IEnumerable<int> roomsIds, FolderType filterByType, IEnumerable<string> tags, bool searchByTags, bool searchByFilter, bool withoutTags,
        bool withoutMe, Guid ownerId, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        var q1 = GetFolderQuery(filesDbContext, f => roomsIds.Contains(f.Id)).AsNoTracking();

        q1 = BuildRoomsQuery(filesDbContext, q1, filterByType, tags, ownerId, searchByTags, withoutTags, searchByFilter, true, withoutMe, subjectFilter, subjectEntriesIds);

        if (searchByTags)
        {
            var q2 = q1.Join(filesDbContext.TagLink, f => f.Id.ToString(), t => t.EntryId, (folder, tagLink) => new { folder, tagLink.TagId })
                .Join(filesDbContext.Tag, r => r.TagId, t => t.Id, (result, tag) => new { result.folder, tag.Name })
                .Where(r => tags.Contains(r.Name))
                .Select(r => r.folder.Id).Distinct();

            return GetFolderQuery(filesDbContext).AsNoTracking()
                .Join(filesDbContext.Tree, f => f.Id, t => t.FolderId, (folder, tree) => new { folder, tree })
                .Where(r => q2.Contains(r.tree.ParentId))
                .Select(r => r.folder);
        }

        if (!searchByFilter && !searchByTags && !withoutTags && !withoutMe)
        {
            return GetFolderQuery(filesDbContext).AsNoTracking()
                .Join(filesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
                .Where(r => roomsIds.Contains(r.tree.ParentId))
                .Select(r => r.folder);
        }

        return GetFolderQuery(filesDbContext).AsNoTracking().Join(filesDbContext.Tree, f => f.Id, t => t.FolderId, (folder, tree) => new { folder, tree })
                    .Where(r => q1.Select(f => f.Id).Contains(r.tree.ParentId))
                    .Select(r => r.folder);
    }

    private bool CheckInvalidFilter(FilterType filterType)
    {
        return filterType is
            FilterType.FilesOnly or
            FilterType.ByExtension or
            FilterType.DocumentsOnly or
            FilterType.OFormOnly or
            FilterType.OFormTemplateOnly or
            FilterType.ImagesOnly or
            FilterType.PresentationsOnly or
            FilterType.SpreadsheetsOnly or
            FilterType.ArchiveOnly or
            FilterType.MediaOnly;
    }

    private FolderType GetRoomTypeFilter(FilterType filterType)
    {
        return filterType switch
        {
            FilterType.FillingFormsRooms => FolderType.FillingFormsRoom,
            FilterType.EditingRooms => FolderType.EditingRoom,
            FilterType.ReviewRooms => FolderType.ReviewRoom,
            FilterType.ReadOnlyRooms => FolderType.ReadOnlyRoom,
            FilterType.CustomRooms => FolderType.CustomRoom,
            _ => FolderType.CustomRoom,
        };
    }

    public IDataWriteOperator CreateDataWriteOperator(
           int folderId,
           CommonChunkedUploadSession chunkedUploadSession,
           CommonChunkedUploadSessionHolder sessionHolder)
    {
        return _globalStore.GetStore().CreateDataWriteOperator(chunkedUploadSession, sessionHolder);
    }

    private IQueryable<DbFolder> GetFoldersQueryWithFilters(int parentId, OrderBy orderBy, bool subjectGroup, Guid subjectId, string searchText, bool withSubfolders, bool excludeSubject,
        FilesDbContext filesDbContext)
    {
        var q = GetFolderQuery(filesDbContext, r => r.ParentId == parentId).AsNoTracking();

        if (withSubfolders)
        {
            q = GetFolderQuery(filesDbContext).AsNoTracking()
                .Join(filesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
                .Where(r => r.tree.ParentId == parentId && r.tree.Level != 0)
                .Select(r => r.folder);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            if (_factoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var searchIds))
            {
                q = q.Where(r => searchIds.Contains(r.Id));
            }
            else
            {
                q = BuildSearch(q, searchText, SearhTypeEnum.Any);
            }
        }

        q = orderBy == null ? q : orderBy.SortedBy switch
        {
            SortedByType.Author => orderBy.IsAsc ? q.OrderBy(r => r.CreateBy) : q.OrderByDescending(r => r.CreateBy),
            SortedByType.AZ => orderBy.IsAsc ? q.OrderBy(r => r.Title) : q.OrderByDescending(r => r.Title),
            SortedByType.DateAndTime => orderBy.IsAsc ? q.OrderBy(r => r.ModifiedOn) : q.OrderByDescending(r => r.ModifiedOn),
            SortedByType.DateAndTimeCreation => orderBy.IsAsc ? q.OrderBy(r => r.CreateOn) : q.OrderByDescending(r => r.CreateOn),
            _ => q.OrderBy(r => r.Title),
        };

        if (subjectId != Guid.Empty)
        {
            if (subjectGroup)
            {
                var users = _userManager.GetUsersByGroup(subjectId).Select(u => u.Id).ToArray();
                q = q.Where(r => users.Contains(r.CreateBy));
            }
            else
            {
                q = excludeSubject ? q.Where(r => r.CreateBy != subjectId) : q.Where(r => r.CreateBy == subjectId);
            }
        }

        return q;
    }

    private static readonly Func<FilesDbContext, IEnumerable<int>, int, IAsyncEnumerable<OriginData>> _getOriginsDataQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext filesDbContext, IEnumerable<int> entriesIds, int tenantId) =>
            filesDbContext.TagLink.AsNoTracking()
                .Where(l => l.TenantId == tenantId)
                .Where(l => entriesIds.Contains(Convert.ToInt32(l.EntryId)))
                .Join(filesDbContext.Tag.AsNoTracking()
                    .Where(t => t.Type == TagType.Origin), l => l.TagId, t => t.Id, (l, t) => new { t.Name, t.Type, l.EntryType, l.EntryId })
                .GroupBy(r => r.Name, r => new { r.EntryId, r.EntryType })
                .Select(r => new OriginData
                {
                    OriginRoom = filesDbContext.Folders.AsNoTracking().FirstOrDefault(f => f.TenantId == tenantId &&
                                                                            f.Id == filesDbContext.Tree.AsNoTracking()
                                                                                .Where(t => t.FolderId == Convert.ToInt32(r.Key))
                                                                                .OrderByDescending(t => t.Level)
                                                                                .Select(t => t.ParentId)
                                                                                .Skip(1)
                                                                                .FirstOrDefault()),
                    OriginFolder = filesDbContext.Folders.AsNoTracking().FirstOrDefault(f => f.TenantId == tenantId && f.Id == Convert.ToInt32(r.Key)),
                    Entries = r.Select(e => new KeyValuePair<string, FileEntryType>(e.EntryId, e.EntryType)).ToHashSet()
                }));

    private string GetProjectTitle(object folderID)
    {
        return "";
        //if (!ApiServer.Available)
        //{
        //    return string.Empty;
        //}

        //var cacheKey = "documents/folders/" + folderID.ToString();

        //var projectTitle = Convert.ToString(cache.Get<string>(cacheKey));

        //if (!string.IsNullOrEmpty(projectTitle)) return projectTitle;

        //var bunchObjectID = GetBunchObjectID(folderID);

        //if (string.IsNullOrEmpty(bunchObjectID))
        //    throw new Exception("Bunch Object id is null for " + folderID);

        //if (!bunchObjectID.StartsWith("projects/project/"))
        //    return string.Empty;

        //var bunchObjectIDParts = bunchObjectID.Split('/');

        //if (bunchObjectIDParts.Length < 3)
        //    throw new Exception("Bunch object id is not supported format");

        //var projectID = Convert.ToInt32(bunchObjectIDParts[bunchObjectIDParts.Length - 1]);

        //if (HttpContext.Current == null || !SecurityContext.IsAuthenticated)
        //    return string.Empty;

        //var apiServer = new ApiServer();

        //var apiUrl = string.Format("{0}project/{1}.json?fields=id,title", SetupInfo.WebApiBaseUrl, projectID);

        //var responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))))["response"];

        //if (responseApi != null && responseApi.HasValues)
        //{
        //    projectTitle = Global.ReplaceInvalidCharsAndTruncate(responseApi["title"].Value<string>());
        //}
        //else
        //{
        //    return string.Empty;
        //}
        //if (!string.IsNullOrEmpty(projectTitle))
        //{
        //    cache.Insert(cacheKey, projectTitle, TimeSpan.FromMinutes(15));
        //}
        //return projectTitle;
    }
}

public class DbFolderQuery
{
    public DbFolder Folder { get; set; }
    public DbFolder Root { get; set; }
    public bool Shared { get; set; }
}

public class DbFolderQueryWithSecurity
{
    public DbFolderQuery DbFolderQuery { get; set; }
    public DbFilesSecurity Security { get; set; }
}

public class ParentRoomPair
{
    public int FolderId { get; set; }
    public int ParentRoomId { get; set; }
}

public class OriginData
{
    public DbFolder OriginRoom { get; set; }
    public DbFolder OriginFolder { get; set; }
    public HashSet<KeyValuePair<string, FileEntryType>> Entries { get; set; }
}