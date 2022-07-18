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

    private static readonly List<FilterType> _constraintFolderFilters =
        new List<FilterType> { FilterType.FilesOnly, FilterType.ByExtension, FilterType.DocumentsOnly, FilterType.ImagesOnly,
            FilterType.PresentationsOnly, FilterType.SpreadsheetsOnly, FilterType.ArchiveOnly, FilterType.MediaOnly };

    public FolderDao(
        FactoryIndexerFolder factoryIndexer,
        UserManager userManager,
        DbContextManager<FilesDbContext> dbContextManager,
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
        GlobalSpace globalSpace,
        IDaoFactory daoFactory,
        ProviderFolderDao providerFolderDao,
        CrossDao crossDao,
        IMapper mapper)
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
        _factoryIndexer = factoryIndexer;
        _globalSpace = globalSpace;
        _daoFactory = daoFactory;
        _providerFolderDao = providerFolderDao;
        _crossDao = crossDao;
        _mapper = mapper;
    }

    public async Task<Folder<int>> GetFolderAsync(int folderId)
    {
        var query = GetFolderQuery(r => r.Id == folderId).AsNoTracking();

        var dbFolder = await FromQueryWithShared(query).Take(1).SingleOrDefaultAsync().ConfigureAwait(false);

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public Task<Folder<int>> GetFolderAsync(string title, int parentId)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(title);

        return InternalGetFolderAsync(title, parentId);
    }

    private async Task<Folder<int>> InternalGetFolderAsync(string title, int parentId)
    {
        var query = GetFolderQuery(r => r.Title == title && r.ParentId == parentId).AsNoTracking()
            .OrderBy(r => r.CreateOn);

        var dbFolder = await FromQueryWithShared(query).Take(1).FirstOrDefaultAsync().ConfigureAwait(false);

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public async Task<Folder<int>> GetRootFolderAsync(int folderId)
    {
        var id = await FilesDbContext.Tree
            .AsNoTracking()
            .Where(r => r.FolderId == folderId)
            .OrderByDescending(r => r.Level)
            .Select(r => r.ParentId)
            .FirstOrDefaultAsync().ConfigureAwait(false);

        var query = GetFolderQuery(r => r.Id == id).AsNoTracking();

        var dbFolder = await FromQueryWithShared(query).SingleOrDefaultAsync().ConfigureAwait(false);

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public async Task<Folder<int>> GetRootFolderByFileAsync(int fileId)
    {
        var subq = Query(FilesDbContext.Files).AsNoTracking()
            .Where(r => r.Id == fileId && r.CurrentVersion)
            .Select(r => r.ParentId)
            .Distinct();

        var q = await FilesDbContext.Tree.AsNoTracking()
            .Where(r => subq.Contains(r.FolderId))
            .OrderByDescending(r => r.Level)
            .Select(r => r.ParentId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        var query = GetFolderQuery(r => r.Id == q).AsNoTracking();

        var dbFolder = await FromQueryWithShared(query).SingleOrDefaultAsync().ConfigureAwait(false);

        return _mapper.Map<DbFolderQuery, Folder<int>>(dbFolder);
    }

    public IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId)
    {
        return GetFoldersAsync(parentId, default, FilterType.None, false, default, string.Empty);
    }

    public IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false,
        IEnumerable<string> tagNames = null)
    {
        return GetFoldersAsync(parentId, orderBy, new[] { filterType }, subjectGroup, subjectID, searchText, withSubfolders, tagNames);
    }

    public IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId, OrderBy orderBy, IEnumerable<FilterType> filterTypes, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false,
        IEnumerable<string> tagNames = null)
    {
        if (!CheckForInvalidFilters(filterTypes))
        {
            return AsyncEnumerable.Empty<Folder<int>>();
        }

        if (orderBy == null)
        {
            orderBy = new OrderBy(SortedByType.DateAndTime, false);
        }

        var q = GetFolderQuery(r => r.ParentId == parentId).AsNoTracking();

        q = SetFilterByTypes(q, filterTypes);

        if (withSubfolders)
        {
            q = GetFolderQuery().AsNoTracking()
                .Join(FilesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
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

        q = orderBy.SortedBy switch
        {
            SortedByType.Author => orderBy.IsAsc ? q.OrderBy(r => r.CreateBy) : q.OrderByDescending(r => r.CreateBy),
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

        if (tagNames != null && tagNames.Any())
        {
            q = q.Join(FilesDbContext.TagLink, f => f.Id.ToString(), t => t.EntryId, (folder, tag) => new { folder, tag.TagId })
                .Join(FilesDbContext.Tag, r => r.TagId, t => t.Id, (result, tagInfo) => new { result.folder, result.TagId, tagInfo.Name })
                .Where(r => tagNames.Contains(r.Name))
                .Select(r => r.folder).Distinct();
        }

        var dbFolders = FromQueryWithShared(q).AsAsyncEnumerable();

        return dbFolders.Select(_mapper.Map<DbFolderQuery, Folder<int>>);
    }

    public IAsyncEnumerable<Folder<int>> GetFoldersAsync(IEnumerable<int> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true,
        IEnumerable<string> tagNames = null)
    {
        return GetFoldersAsync(folderIds, new[] { filterType }, subjectGroup, subjectID, searchText, searchSubfolders, checkShare, tagNames);
    }

    public IAsyncEnumerable<Folder<int>> GetFoldersAsync(IEnumerable<int> folderIds, IEnumerable<FilterType> filterTypes, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true,
        IEnumerable<string> tagNames = null)
    {
        if (!CheckForInvalidFilters(filterTypes))
        {
            return AsyncEnumerable.Empty<Folder<int>>();
        }

        var q = GetFolderQuery(r => folderIds.Contains(r.Id)).AsNoTracking();

        q = SetFilterByTypes(q, filterTypes);

        if (searchSubfolders)
        {
            q = GetFolderQuery()
                .AsNoTracking()
                .Join(FilesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
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
                q = q.Where(r => r.CreateBy == subjectID);
            }
        }

        if (tagNames != null && tagNames.Any())
        {
            q = q.Join(FilesDbContext.TagLink, f => f.Id.ToString(), t => t.EntryId, (folder, tag) => new { folder, tag.TagId })
                .Join(FilesDbContext.Tag, r => r.TagId, t => t.Id, (result, tagInfo) => new { result.folder, result.TagId, tagInfo.Name })
                .Where(r => tagNames.Contains(r.Name))
                .Select(r => r.folder).Distinct();
        }

        var dbFolders = (checkShare ? FromQueryWithShared(q) : FromQuery(q)).AsAsyncEnumerable();

        return dbFolders.Select(_mapper.Map<DbFolderQuery, Folder<int>>).Distinct();
    }

    public async Task<List<Folder<int>>> GetParentFoldersAsync(int folderId)
    {
        var q = GetFolderQuery()
            .AsNoTracking()
            .Join(FilesDbContext.Tree, r => r.Id, a => a.ParentId, (folder, tree) => new { folder, tree })
            .Where(r => r.tree.FolderId == folderId)
            .OrderByDescending(r => r.tree.Level)
            .Select(r => r.folder);

        var query = await FromQueryWithShared(q).ToListAsync().ConfigureAwait(false);

        return _mapper.Map<List<DbFolderQuery>, List<Folder<int>>>(query);
    }

    public Task<int> SaveFolderAsync(Folder<int> folder)
    {
        return SaveFolderAsync(folder, null);
    }

    public Task<int> SaveFolderAsync(Folder<int> folder, IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return InternalSaveFolderAsync(folder, transaction);
    }

    public async Task<int> InternalSaveFolderAsync(Folder<int> folder, IDbContextTransaction transaction)
    {
        var folderId = folder.Id;

        if (transaction == null)
        {
            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var tx = await FilesDbContext.Database.BeginTransactionAsync();

                folderId = await InternalSaveFolderToDbAsync(folder);

                await tx.CommitAsync().ConfigureAwait(false);
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

        if (folder.Id != default && await IsExistAsync(folder.Id).ConfigureAwait(false))
        {
            var toUpdate = await Query(FilesDbContext.Folders)
                .Where(r => r.Id == folder.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            toUpdate.Title = folder.Title;
            toUpdate.CreateBy = folder.CreateBy;
            toUpdate.ModifiedOn = _tenantUtil.DateTimeToUtc(folder.ModifiedOn);
            toUpdate.ModifiedBy = folder.ModifiedBy;

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            if (folder.FolderType == FolderType.DEFAULT || folder.FolderType == FolderType.BUNCH)
            {
                _ = _factoryIndexer.IndexAsync(toUpdate).ConfigureAwait(false);
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
                TenantId = TenantID
            };

            var entityEntry = await FilesDbContext.Folders.AddAsync(newFolder).ConfigureAwait(false);
            newFolder = entityEntry.Entity;
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            if (folder.FolderType == FolderType.DEFAULT || folder.FolderType == FolderType.BUNCH)
            {
                _ = _factoryIndexer.IndexAsync(newFolder).ConfigureAwait(false);
            }

            folder.Id = newFolder.Id;

            //itself link
            var newTree = new DbFolderTree
            {
                FolderId = folder.Id,
                ParentId = folder.Id,
                Level = 0
            };

            await FilesDbContext.Tree.AddAsync(newTree).ConfigureAwait(false);
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            //full path to root
            var oldTree = FilesDbContext.Tree
                .AsQueryable()
                .Where(r => r.FolderId == folder.ParentId);

            foreach (var o in oldTree)
            {
                var treeToAdd = new DbFolderTree
                {
                    FolderId = folder.Id,
                    ParentId = o.ParentId,
                    Level = o.Level + 1
                };

                await FilesDbContext.Tree.AddAsync(treeToAdd).ConfigureAwait(false);
            }

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        if (isnew)
        {
            await RecalculateFoldersCountAsync(folder.Id).ConfigureAwait(false);
        }

        return folder.Id;

    }

    private Task<bool> IsExistAsync(int folderId)
    {
        return Query(FilesDbContext.Folders).AsNoTracking()
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
        var strategy = FilesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var tx = await FilesDbContext.Database.BeginTransactionAsync();
            var subfolders =
                await FilesDbContext.Tree
                .AsQueryable()
                .Where(r => r.ParentId == id)
                .Select(r => r.FolderId)
                .ToListAsync().ConfigureAwait(false);

            if (!subfolders.Contains(id))
            {
                subfolders.Add(id); // chashed folder_tree
            }

            var parent = await Query(FilesDbContext.Folders)
                .Where(r => r.Id == id)
                .Select(r => r.ParentId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            var folderToDelete = await Query(FilesDbContext.Folders).Where(r => subfolders.Contains(r.Id)).ToListAsync();
            FilesDbContext.Folders.RemoveRange(folderToDelete);

            foreach (var f in folderToDelete)
            {
                await _factoryIndexer.DeleteAsync(f).ConfigureAwait(false);
            }

            var treeToDelete = await FilesDbContext.Tree.AsQueryable().Where(r => subfolders.Contains(r.FolderId)).ToListAsync();
            FilesDbContext.Tree.RemoveRange(treeToDelete);

            var subfoldersStrings = subfolders.Select(r => r.ToString()).ToList();
            var linkToDelete = await Query(FilesDbContext.TagLink)
                .Where(r => subfoldersStrings.Contains(r.EntryId))
                .Where(r => r.EntryType == FileEntryType.Folder)
                .ToListAsync();
            FilesDbContext.TagLink.RemoveRange(linkToDelete);

            var tagsToRemove = await Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Any(a => a.TagId == r.Id))
                .ToListAsync();

            FilesDbContext.Tag.RemoveRange(tagsToRemove);

            var securityToDelete = await Query(FilesDbContext.Security)
                    .Where(r => subfoldersStrings.Contains(r.EntryId))
                    .Where(r => r.EntryType == FileEntryType.Folder)
                    .ToListAsync();

            FilesDbContext.Security.RemoveRange(securityToDelete);
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            var bunchToDelete = await Query(FilesDbContext.BunchObjects)
                .Where(r => r.LeftNode == id.ToString())
                .ToListAsync();

            FilesDbContext.RemoveRange(bunchToDelete);
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await tx.CommitAsync().ConfigureAwait(false);
            await RecalculateFoldersCountAsync(parent).ConfigureAwait(false);
        });

        //FactoryIndexer.DeleteAsync(new FoldersWrapper { Id = id });
    }

    public async Task<TTo> MoveFolderAsync<TTo>(int folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tId, cancellationToken).ConfigureAwait(false), typeof(TTo));
        }

        if (toFolderId is string tsId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tsId, cancellationToken).ConfigureAwait(false), typeof(TTo));
        }

        throw new NotImplementedException();
    }

    public async Task<int> MoveFolderAsync(int folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var strategy = FilesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using (var tx = await FilesDbContext.Database.BeginTransactionAsync())
            {
                var folder = await GetFolderAsync(folderId).ConfigureAwait(false);

                if (folder.FolderType != FolderType.DEFAULT && !DocSpaceHelper.IsRoom(folder.FolderType))
                {
                    throw new ArgumentException("It is forbidden to move the System folder.", nameof(folderId));
                }

                var recalcFolders = new List<int> { toFolderId };
                var parent = await FilesDbContext.Folders
                    .AsQueryable()
                    .Where(r => r.Id == folderId)
                    .Select(r => r.ParentId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (parent != 0 && !recalcFolders.Contains(parent))
                {
                    recalcFolders.Add(parent);
                }

                var toUpdate = await Query(FilesDbContext.Folders)
                    .Where(r => r.Id == folderId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                toUpdate.ParentId = toFolderId;
                toUpdate.ModifiedOn = DateTime.UtcNow;
                toUpdate.ModifiedBy = _authContext.CurrentAccount.ID;

                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var subfolders = await FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => r.ParentId == folderId)
                    .ToDictionaryAsync(r => r.FolderId, r => r.Level)
                    .ConfigureAwait(false);

#pragma warning disable CA1841 // Prefer Dictionary.Contains methods
                var toDelete = await FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => subfolders.Keys.Contains(r.FolderId) && !subfolders.Keys.Contains(r.ParentId))
                    .ToListAsync();
#pragma warning restore CA1841 // Prefer Dictionary.Contains methods

                FilesDbContext.Tree.RemoveRange(toDelete);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var toInsert = await FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => r.FolderId == toFolderId)
                    .OrderBy(r => r.Level)
                    .ToListAsync()
                    .ConfigureAwait(false);

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
                        await FilesDbContext.AddOrUpdateAsync(r => r.Tree, newTree).ConfigureAwait(false);
                    }
                }

                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);

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
            .ConfigureAwait(false);

        return moved.Id;
    }

    public async Task<Folder<TTo>> CopyFolderAsync<TTo>(int folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return await CopyFolderAsync(folderId, tId, cancellationToken).ConfigureAwait(false) as Folder<TTo>;
        }

        if (toFolderId is string tsId)
        {
            return await CopyFolderAsync(folderId, tsId, cancellationToken).ConfigureAwait(false) as Folder<TTo>;
        }

        throw new NotImplementedException();
    }

    public async Task<Folder<int>> CopyFolderAsync(int folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var folder = await GetFolderAsync(folderId).ConfigureAwait(false);

        var toFolder = await GetFolderAsync(toFolderId).ConfigureAwait(false);

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

        copy = await GetFolderAsync(await SaveFolderAsync(copy).ConfigureAwait(false)).ConfigureAwait(false);

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
            .ConfigureAwait(false);

        return moved;
    }

    public Task<IDictionary<int, string>> CanMoveOrCopyAsync<TTo>(int[] folderIds, TTo to)
    {
        if (to is int tId)
        {
            return CanMoveOrCopyAsync(folderIds, tId);
        }

        if (to is string tsId)
        {
            return CanMoveOrCopyAsync(folderIds, tsId);
        }

        throw new NotImplementedException();
    }

    public Task<IDictionary<int, string>> CanMoveOrCopyAsync(int[] folderIds, string to)
    {
        return Task.FromResult((IDictionary<int, string>)new Dictionary<int, string>());
    }

    public async Task<IDictionary<int, string>> CanMoveOrCopyAsync(int[] folderIds, int to)
    {
        var result = new Dictionary<int, string>();

        foreach (var folderId in folderIds)
        {
            var exists = await FilesDbContext.Tree
                .AsNoTracking()
                .AnyAsync(r => r.ParentId == folderId && r.FolderId == to)
                .ConfigureAwait(false);

            if (exists)
            {
                throw new InvalidOperationException(FilesCommonResource.ErrorMassage_FolderCopyError);
            }

            var title = await Query(FilesDbContext.Folders)
                .AsNoTracking()
                .Where(r => r.Id == folderId)
                .Select(r => r.Title.ToLower())
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var conflict = await Query(FilesDbContext.Folders)
                .AsNoTracking()
                .Where(r => r.Title.ToLower() == title)
                .Where(r => r.ParentId == to)
                .Select(r => r.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (conflict != 0)
            {
                var files = await FilesDbContext.Files
                    .AsNoTracking()
                    .Join(FilesDbContext.Files, f1 => f1.Title.ToLower(), f2 => f2.Title.ToLower(), (f1, f2) => new { f1, f2 })
                    .Where(r => r.f1.TenantId == TenantID && r.f1.CurrentVersion && r.f1.ParentId == folderId)
                    .Where(r => r.f2.TenantId == TenantID && r.f2.CurrentVersion && r.f2.ParentId == conflict)
                    .Select(r => r.f1)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var file in files)
                {
                    result[file.Id] = file.Title;
                }

                var childs = await Query(FilesDbContext.Folders)
                    .AsNoTracking()
                    .Where(r => r.ParentId == folderId)
                    .Select(r => r.Id)
                    .ToArrayAsync();

                foreach (var pair in await CanMoveOrCopyAsync(childs, conflict).ConfigureAwait(false))
                {
                    result.Add(pair.Key, pair.Value);
                }
            }
        }

        return result;
    }

    public async Task<int> RenameFolderAsync(Folder<int> folder, string newTitle)
    {
        var toUpdate = await Query(FilesDbContext.Folders)
            .Where(r => r.Id == folder.Id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        toUpdate.Title = Global.ReplaceInvalidCharsAndTruncate(newTitle);
        toUpdate.ModifiedOn = DateTime.UtcNow;
        toUpdate.ModifiedBy = _authContext.CurrentAccount.ID;

        await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

        _ = _factoryIndexer.IndexAsync(toUpdate).ConfigureAwait(false);

        return folder.Id;
    }

    public async Task<int> GetItemsCountAsync(int folderId)
    {
        return await GetFoldersCountAsync(folderId).ConfigureAwait(false) +
               await GetFilesCountAsync(folderId).ConfigureAwait(false);
    }

    private async Task<int> GetFoldersCountAsync(int parentId)
    {
        var count = await FilesDbContext.Tree
            .AsQueryable()
            .CountAsync(r => r.ParentId == parentId && r.Level > 0)
            .ConfigureAwait(false);

        return count;
    }

    private async Task<int> GetFilesCountAsync(int folderId)
    {
        var count = await Query(FilesDbContext.Files)
            .Join(FilesDbContext.Tree, r => r.ParentId, r => r.FolderId, (file, tree) => new { tree, file })
            .Where(r => r.tree.ParentId == folderId)
            .Select(r => r.file.Id)
            .Distinct()
            .CountAsync()
            .ConfigureAwait(false);

        return count;
    }

    public async Task<bool> IsEmptyAsync(int folderId)
    {
        return await GetItemsCountAsync(folderId).ConfigureAwait(false) == 0;
    }

    public bool UseTrashForRemove(Folder<int> folder)
    {
        return folder.RootFolderType != FolderType.TRASH && folder.RootFolderType != FolderType.Privacy && folder.FolderType != FolderType.BUNCH;
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

        return Math.Min(tmp, chunkedUpload ? _setupInfo.MaxChunkedUploadSize(_tenantExtra, _tenantStatisticProvider) : _setupInfo.MaxUploadSize(_tenantExtra, _tenantStatisticProvider));
    }

    private async Task RecalculateFoldersCountAsync(int id)
    {
        var toUpdate = await Query(FilesDbContext.Folders)
            .Join(FilesDbContext.Tree, r => r.Id, r => r.ParentId, (file, tree) => new { file, tree })
            .Where(r => r.tree.FolderId == id)
            .Select(r => r.file)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var f in toUpdate)
        {
            var count = await FilesDbContext.Tree.AsQueryable().Where(r => r.ParentId == f.Id).CountAsync().ConfigureAwait(false) - 1;
            f.FoldersCount = count;
        }

        await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    #region Only for TMFolderDao

    public Task ReassignFoldersAsync(int[] folderIds, Guid newOwnerId)
    {
        var toUpdate = Query(FilesDbContext.Folders)
            .Where(r => folderIds.Contains(r.Id));

        foreach (var f in toUpdate)
        {
            f.CreateBy = newOwnerId;
        }

        return FilesDbContext.SaveChangesAsync();
    }

    public IAsyncEnumerable<Folder<int>> SearchFoldersAsync(string text, bool bunch = false)
    {
        var folders = SearchAsync(text);

        return folders.Where(f => bunch ? f.RootFolderType == FolderType.BUNCH
            : (f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON));
    }

    private IAsyncEnumerable<Folder<int>> SearchAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return AsyncEnumerable.Empty<Folder<int>>();
        }

        if (_factoryIndexer.TrySelectIds(s => s.MatchAll(text), out var ids))
        {
            var q1 = GetFolderQuery(r => ids.Contains(r.Id));
            var fromQuery1 = FromQueryWithShared(q1).AsAsyncEnumerable();

            return fromQuery1.Select(_mapper.Map<DbFolderQuery, Folder<int>>);
        }

        var q = BuildSearch(GetFolderQuery(), text, SearhTypeEnum.Any);
        var fromQuery = FromQueryWithShared(q).AsAsyncEnumerable();

        return fromQuery.Select(_mapper.Map<DbFolderQuery, Folder<int>>);
    }

    public Task<IEnumerable<int>> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(module);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(bunch);

        return InternalGetFolderIDsAsync(module, bunch, data, createIfNotExists);
    }

    private async Task<IEnumerable<int>> InternalGetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
    {
        var keys = data.Select(id => $"{module}/{bunch}/{id}").ToArray();

        var folderIdsDictionary = await Query(FilesDbContext.BunchObjects)
            .AsNoTracking()
            .Where(r => keys.Length > 1 ? keys.Any(a => a == r.RightNode) : r.RightNode == keys[0])
            .ToDictionaryAsync(r => r.RightNode, r => r.LeftNode)
            .ConfigureAwait(false);

        var folderIds = new List<int>();

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

                var strategy = FilesDbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false);//NOTE: Maybe we shouldn't start transaction here at all

                    newFolderId = await SaveFolderAsync(folder, tx).ConfigureAwait(false); //Save using our db manager

                    var newBunch = new DbFilesBunchObjects
                    {
                        LeftNode = newFolderId.ToString(),
                        RightNode = key,
                        TenantId = TenantID
                    };

                    await FilesDbContext.AddOrUpdateAsync(r => r.BunchObjects, newBunch).ConfigureAwait(false);
                    await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                    await tx.CommitAsync().ConfigureAwait(false); //Commit changes
                });
            }

            folderIds.Add(newFolderId);
        }

        return folderIds;
    }

    public Task<int> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(module);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(bunch);

        return InternalGetFolderIDAsync(module, bunch, data, createIfNotExists);
    }

    private async Task<int> InternalGetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
    {
        var key = $"{module}/{bunch}/{data}";
        var folderId = await Query(FilesDbContext.BunchObjects)
            .Where(r => r.RightNode == key)
            .Select(r => r.LeftNode)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (folderId != null)
        {
            return Convert.ToInt32(folderId);
        }

        var newFolderId = 0;
        if (createIfNotExists)
        {
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

            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false); //NOTE: Maybe we shouldn't start transaction here at all
                newFolderId = await SaveFolderAsync(folder, tx).ConfigureAwait(false); //Save using our db manager
                var toInsert = new DbFilesBunchObjects
                {
                    LeftNode = newFolderId.ToString(),
                    RightNode = key,
                    TenantId = TenantID
                };

                await FilesDbContext.AddOrUpdateAsync(r => r.BunchObjects, toInsert).ConfigureAwait(false);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false); //Commit changes
            });
        }

        return newFolderId;
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

    #endregion

    protected internal IQueryable<DbFolder> GetFolderQuery(Expression<Func<DbFolder, bool>> where = null)
    {
        var q = Query(FilesDbContext.Folders);
        if (where != null)
        {
            q = q.Where(where);
        }

        return q;
    }

    protected IQueryable<DbFolderQuery> FromQueryWithShared(IQueryable<DbFolder> dbFiles)
    {
        var e = from r in dbFiles
                select new DbFolderQuery
                {
                    Folder = r,
                    Root = (from f in FilesDbContext.Folders.AsQueryable()
                            where f.Id ==
                             (from t in FilesDbContext.Tree.AsQueryable()
                              where t.FolderId == r.ParentId
                              orderby t.Level descending
                              select t.ParentId
                             ).FirstOrDefault()
                            where f.TenantId == r.TenantId
                            select f
                           ).FirstOrDefault(),
                    Shared = (from f in FilesDbContext.Security.AsQueryable()
                              where f.EntryType == FileEntryType.Folder && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId
                              select f
                              ).Any()
                };

        return e;
    }

    protected IQueryable<DbFolderQuery> FromQuery(IQueryable<DbFolder> dbFiles)
    {
        return dbFiles
            .Select(r => new DbFolderQuery
            {
                Folder = r,
                Root = (from f in FilesDbContext.Folders.AsQueryable()
                        where f.Id ==
                        (from t in FilesDbContext.Tree.AsQueryable()
                         where t.FolderId == r.ParentId
                         orderby t.Level descending
                         select t.ParentId
                         ).FirstOrDefault()
                        where f.TenantId == r.TenantId
                        select f
                          ).FirstOrDefault(),
                Shared = true
            });
    }

    public Task<string> GetBunchObjectIDAsync(int folderID)
    {
        return Query(FilesDbContext.BunchObjects)
            .Where(r => r.LeftNode == folderID.ToString())
            .Select(r => r.RightNode)
            .FirstOrDefaultAsync();
    }

    public Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<int> folderIDs)
    {
        var folderSIds = folderIDs.Select(r => r.ToString()).ToList();

        return Query(FilesDbContext.BunchObjects)
            .Where(r => folderSIds.Any(a => a == r.LeftNode))
            .ToDictionaryAsync(r => r.LeftNode, r => r.RightNode);
    }

    public async Task<IEnumerable<FolderWithShare>> GetFeedsForFoldersAsync(int tenant, DateTime from, DateTime to)
    {
        var q1 = FilesDbContext.Folders
            .AsQueryable()
            .Where(r => r.TenantId == tenant)
            .Where(r => r.FolderType == FolderType.DEFAULT)
            .Where(r => r.CreateOn >= from && r.ModifiedOn <= to);

        var q2 = FromQuery(q1)
            .Select(r => new DbFolderQueryWithSecurity() { DbFolderQuery = r, Security = null });

        var q3 = FilesDbContext.Folders
            .AsQueryable()
            .Where(r => r.TenantId == tenant)
            .Where(r => r.FolderType == FolderType.DEFAULT);

        var q4 = FromQuery(q3)
            .Join(FilesDbContext.Security.AsQueryable().DefaultIfEmpty(), r => r.Folder.Id.ToString(), s => s.EntryId, (f, s) => new DbFolderQueryWithSecurity { DbFolderQuery = f, Security = s })
            .Where(r => r.Security.TenantId == tenant)
            .Where(r => r.Security.EntryType == FileEntryType.Folder)
            .Where(r => r.Security.Share == FileShare.Restrict)
            .Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to);

        var firstQuery = await q2.ToListAsync().ConfigureAwait(false);
        var secondQuery = await q4.ToListAsync().ConfigureAwait(false);

        //return firstQuery.Select(ToFolderWithShare).Union(secondQuery.Select(ToFolderWithShare));

        return _mapper.Map<IEnumerable<DbFolderQueryWithSecurity>, IEnumerable<FolderWithShare>>(firstQuery)
            .Union(_mapper.Map<IEnumerable<DbFolderQueryWithSecurity>, IEnumerable<FolderWithShare>>(secondQuery));
    }

    public async Task<IEnumerable<int>> GetTenantsWithFeedsForFoldersAsync(DateTime fromTime)
    {
        var q1 = await FilesDbContext.Files
            .AsQueryable()
            .Where(r => r.ModifiedOn > fromTime)
            .GroupBy(r => r.TenantId)
            .Where(r => r.Any())
            .Select(r => r.Key)
            .ToListAsync()
            .ConfigureAwait(false);

        var q2 = await FilesDbContext.Security
            .AsQueryable()
            .Where(r => r.TimeStamp > fromTime)
            .GroupBy(r => r.TenantId)
            .Where(r => r.Any())
            .Select(r => r.Key)
            .ToListAsync()
            .ConfigureAwait(false);

        return q1.Union(q2);
    }

    private bool CheckForInvalidFilters(IEnumerable<FilterType> filterTypes)
    {
        var intersection = filterTypes.Intersect(_constraintFolderFilters);

        return !intersection.Any();
    }

    private IQueryable<DbFolder> SetFilterByTypes(IQueryable<DbFolder> q, IEnumerable<FilterType> filterTypes)
    {
        if (filterTypes.Any() && !filterTypes.Contains(FilterType.None) && !filterTypes.Contains(FilterType.FoldersOnly))
        {
            var filter = filterTypes.Select(f => f switch
            {
                FilterType.FillingFormsRooms => FolderType.FillingFormsRoom,
                FilterType.EditingRooms => FolderType.EditingRoom,
                FilterType.ReviewRooms => FolderType.ReviewRoom,
                FilterType.ReadOnlyRooms => FolderType.ReadOnlyRoom,
                FilterType.CustomRooms => FolderType.CustomRoom,
                _ => FolderType.CustomRoom
            }).ToHashSet();

            q = q.Where(r => filter.Contains(r.FolderType));
        }

        return q;
    }

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