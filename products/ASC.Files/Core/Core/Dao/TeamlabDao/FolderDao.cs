/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Files.Core.EF;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Files.Core.Thirdparty;
using ASC.Files.Thirdparty.ProviderDao;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Search;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.Data
{
    [Scope]
    internal class FolderDao : AbstractDao, IFolderDao<int>
    {
        private const string my = "my";
        private const string common = "common";
        private const string share = "share";
        private const string recent = "recent";
        private const string favorites = "favorites";
        private const string templates = "templates";
        private const string privacy = "privacy";
        private const string trash = "trash";
        private const string projects = "projects";

        private FactoryIndexerFolder FactoryIndexer { get; }
        private GlobalSpace GlobalSpace { get; }
        private IDaoFactory DaoFactory { get; }
        private ProviderFolderDao ProviderFolderDao { get; }
        private CrossDao CrossDao { get; }

        public FolderDao(
            FactoryIndexerFolder factoryIndexer,
            UserManager userManager,
            DbContextManager<EF.FilesDbContext> dbContextManager,
            DbContextManager<TenantDbContext> dbContextManager1,
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
            CrossDao crossDao)
            : base(
                  dbContextManager,
                  dbContextManager1,
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
            FactoryIndexer = factoryIndexer;
            GlobalSpace = globalSpace;
            DaoFactory = daoFactory;
            ProviderFolderDao = providerFolderDao;
            CrossDao = crossDao;
        }

        public Folder<int> GetFolder(int folderId)
        {
            return GetFolderAsync(folderId).Result;
        }

        public async Task<Folder<int>> GetFolderAsync(int folderId)
        {
            var query = GetFolderQuery(r => r.Id == folderId).AsNoTracking();
            return ToFolder(await FromQueryWithShared(query).Take(1).SingleOrDefaultAsync().ConfigureAwait(false));
        }

        public Folder<int> GetFolder(string title, int parentId)
        {
            return GetFolderAsync(title, parentId).Result;
        }

        public async Task<Folder<int>> GetFolderAsync(string title, int parentId)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(title);

            var query = GetFolderQuery(r => r.Title == title && r.ParentId == parentId).AsNoTracking()
                .OrderBy(r => r.CreateOn);

            return ToFolder(await FromQueryWithShared(query).Take(1).FirstOrDefaultAsync().ConfigureAwait(false));
        }

        public Folder<int> GetRootFolder(int folderId)
        {
            return GetRootFolderAsync(folderId).Result;
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

            return ToFolder(await FromQueryWithShared(query).SingleOrDefaultAsync().ConfigureAwait(false));
        }

        public Folder<int> GetRootFolderByFile(int fileId)
        {
            return GetRootFolderByFileAsync(fileId).Result;
        }

        public async Task<Folder<int>> GetRootFolderByFileAsync(int fileId)
        {
            var subq = Query(FilesDbContext.Files).AsNoTracking()
                .Where(r => r.Id == fileId && r.CurrentVersion)
                .Select(r => r.FolderId)
                .Distinct();

            var q = await FilesDbContext.Tree.AsNoTracking()
                .Where(r => subq.Contains(r.FolderId))
                .OrderByDescending(r => r.Level)
                .Select(r => r.ParentId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var query = GetFolderQuery(r => r.Id == q).AsNoTracking();

            return ToFolder(await FromQueryWithShared(query).SingleOrDefaultAsync().ConfigureAwait(false));
        }

        public List<Folder<int>> GetFolders(int parentId)
        {
            return GetFoldersAsync(parentId).ToListAsync().Result;
        }

        public IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId)
        {
            return GetFoldersAsync(parentId, default, default, false, default, string.Empty);
        }

        public List<Folder<int>> GetFolders(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            return GetFoldersAsync(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, withSubfolders).ToListAsync().Result;
        }

        public IAsyncEnumerable<Folder<int>> GetFoldersAsync(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return AsyncEnumerable.Empty<Folder<int>>();

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var q = GetFolderQuery(r => r.ParentId == parentId).AsNoTracking();

            if (withSubfolders)
            {
                q = GetFolderQuery().AsNoTracking()
                    .Join(FilesDbContext.Tree, r => r.Id, a => a.FolderId, (folder, tree) => new { folder, tree })
                    .Where(r => r.tree.ParentId == parentId && r.tree.Level != 0)
                    .Select(r => r.folder);
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                if (FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var searchIds))
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
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
                    q = q.Where(r => users.Contains(r.CreateBy));
                }
                else
                {
                    q = q.Where(r => r.CreateBy == subjectID);
                }
            }

            return FromQueryWithShared(q).AsAsyncEnumerable().Select(ToFolder);
        }

        public List<Folder<int>> GetFolders(IEnumerable<int> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
        {
            return GetFoldersAsync(folderIds, filterType, subjectGroup, subjectID, searchText, searchSubfolders, checkShare).ToListAsync().Result;
        }

        public IAsyncEnumerable<Folder<int>> GetFoldersAsync(IEnumerable<int> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
        {
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return AsyncEnumerable.Empty<Folder<int>>();

            var q = GetFolderQuery(r => folderIds.Contains(r.Id)).AsNoTracking();

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
                if (FactoryIndexer.TrySelectIds(s =>
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
                    var users = UserManager.GetUsersByGroup(subjectID.Value).Select(u => u.ID).ToArray();
                    q = q.Where(r => users.Contains(r.CreateBy));
                }
                else
                {
                    q = q.Where(r => r.CreateBy == subjectID);
                }
            }

            return (checkShare ? FromQueryWithShared(q) : FromQuery(q)).AsAsyncEnumerable().Select(e => ToFolder(e)).Distinct();
        }

        public List<Folder<int>> GetParentFolders(int folderId)
        {
            return GetParentFoldersAsync(folderId).Result;
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
            return query.ConvertAll(e => ToFolder(e));
        }

        public int SaveFolder(Folder<int> folder)
        {
            return SaveFolder(folder, null);
        }

        public async Task<int> SaveFolderAsync(Folder<int> folder)
        {
            return await SaveFolderAsync(folder, null).ConfigureAwait(false);
        }

        public int SaveFolder(Folder<int> folder, IDbContextTransaction transaction)
        {
            return SaveFolderAsync(folder, transaction).Result;
        }

        public async Task<int> SaveFolderAsync(Folder<int> folder, IDbContextTransaction transaction)
        {
            if (folder == null) throw new ArgumentNullException("folder");

            folder.Title = Global.ReplaceInvalidCharsAndTruncate(folder.Title);

            folder.ModifiedOn = TenantUtil.DateTimeNow();
            folder.ModifiedBy = AuthContext.CurrentAccount.ID;

            if (folder.CreateOn == default) folder.CreateOn = TenantUtil.DateTimeNow();
            if (folder.CreateBy == default) folder.CreateBy = AuthContext.CurrentAccount.ID;

            var isnew = false;

            var tx = transaction ?? FilesDbContext.Database.BeginTransaction();

            if (folder.ID != default && await IsExistAsync(folder.ID).ConfigureAwait(false))
            {
                var toUpdate = await Query(FilesDbContext.Folders)
                    .Where(r => r.Id == folder.ID)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                toUpdate.Title = folder.Title;
                toUpdate.CreateBy = folder.CreateBy;
                toUpdate.ModifiedOn = TenantUtil.DateTimeToUtc(folder.ModifiedOn);
                toUpdate.ModifiedBy = folder.ModifiedBy;

                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                if (folder.FolderType == FolderType.DEFAULT || folder.FolderType == FolderType.BUNCH)
                {
                    FactoryIndexer.IndexAsync(toUpdate).ConfigureAwait(false);
                }
            }
            else
            {
                isnew = true;
                var newFolder = new DbFolder
                {
                    Id = 0,
                    ParentId = folder.FolderID,
                    Title = folder.Title,
                    CreateOn = TenantUtil.DateTimeToUtc(folder.CreateOn),
                    CreateBy = folder.CreateBy,
                    ModifiedOn = TenantUtil.DateTimeToUtc(folder.ModifiedOn),
                    ModifiedBy = folder.ModifiedBy,
                    FolderType = folder.FolderType,
                    TenantId = TenantID
                };

                var entityEntry = await FilesDbContext.Folders.AddAsync(newFolder).ConfigureAwait(false);
                newFolder = entityEntry.Entity;
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
                if (folder.FolderType == FolderType.DEFAULT || folder.FolderType == FolderType.BUNCH)
                {
                    FactoryIndexer.IndexAsync(newFolder).ConfigureAwait(false);
                }
                folder.ID = newFolder.Id;

                //itself link
                var newTree = new DbFolderTree
                {
                    FolderId = folder.ID,
                    ParentId = folder.ID,
                    Level = 0
                };

                await FilesDbContext.Tree.AddAsync(newTree).ConfigureAwait(false);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                //full path to root
                var oldTree = FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => r.FolderId == folder.FolderID);

                foreach (var o in oldTree)
                {
                    var treeToAdd = new DbFolderTree
                    {
                        FolderId = folder.ID,
                        ParentId = o.ParentId,
                        Level = o.Level + 1
                    };

                    await FilesDbContext.Tree.AddAsync(treeToAdd).ConfigureAwait(false);
                }

                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            if (transaction == null)
            {
                await tx.CommitAsync().ConfigureAwait(false);
                tx.Dispose();
            }

            if (isnew)
            {
                await RecalculateFoldersCountAsync(folder.ID).ConfigureAwait(false);
            }

            //FactoryIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, folder));
            return folder.ID;
        }

        private bool IsExist(int folderId)
        {
            return IsExistAsync(folderId).Result;
        }

        private async Task<bool> IsExistAsync(int folderId)
        {
            return await Query(FilesDbContext.Folders).AsNoTracking()
                .AnyAsync(r => r.Id == folderId).ConfigureAwait(false);
        }

        public void DeleteFolder(int id)
        {
            DeleteFolderAsync(id).Wait();
        }

        public async Task DeleteFolderAsync(int id)
        {
            if (id == default) throw new ArgumentNullException("folderId");

            using var tx = FilesDbContext.Database.BeginTransaction();
            var subfolders =
                await FilesDbContext.Tree
                .AsQueryable()
                .Where(r => r.ParentId == id)
                .Select(r => r.FolderId)
                .ToListAsync().ConfigureAwait(false);

            if (!subfolders.Contains(id)) subfolders.Add(id); // chashed folder_tree

            var parent = await Query(FilesDbContext.Folders)
                .Where(r => r.Id == id)
                .Select(r => r.ParentId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            var folderToDelete = Query(FilesDbContext.Folders).Where(r => subfolders.Contains(r.Id));
            FilesDbContext.Folders.RemoveRange(folderToDelete);

            foreach (var f in folderToDelete)
            {
                await FactoryIndexer.DeleteAsync(f).ConfigureAwait(false);
            }

            var treeToDelete = FilesDbContext.Tree.AsQueryable().Where(r => subfolders.Contains(r.FolderId));
            FilesDbContext.Tree.RemoveRange(treeToDelete);

            var subfoldersStrings = subfolders.Select(r => r.ToString()).ToList();
            var linkToDelete = Query(FilesDbContext.TagLink)
                .Where(r => subfoldersStrings.Contains(r.EntryId))
                .Where(r => r.EntryType == FileEntryType.Folder);
            FilesDbContext.TagLink.RemoveRange(linkToDelete);

            var tagsToRemove = Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Any(a => a.TagId == r.Id));

            FilesDbContext.Tag.RemoveRange(tagsToRemove);

            var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => subfoldersStrings.Contains(r.EntryId))
                    .Where(r => r.EntryType == FileEntryType.Folder);

            FilesDbContext.Security.RemoveRange(securityToDelete);
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            var bunchToDelete = Query(FilesDbContext.BunchObjects)
                .Where(r => r.LeftNode == id.ToString());

            FilesDbContext.RemoveRange(bunchToDelete);
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await tx.CommitAsync().ConfigureAwait(false);

            await RecalculateFoldersCountAsync(parent).ConfigureAwait(false);

            //FactoryIndexer.DeleteAsync(new FoldersWrapper { Id = id });
        }

        public TTo MoveFolder<TTo>(int folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
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

        public int MoveFolder(int folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<int> MoveFolderAsync(int folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var folder = await GetFolderAsync(folderId).ConfigureAwait(false);

                if (folder.FolderType != FolderType.DEFAULT)
                    throw new ArgumentException("It is forbidden to move the System folder.", "folderId");

                var recalcFolders = new List<int> { toFolderId };
                var parent = await FilesDbContext.Folders
                    .AsQueryable()
                    .Where(r => r.Id == folderId)
                    .Select(r => r.ParentId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (parent != 0 && !recalcFolders.Contains(parent)) recalcFolders.Add(parent);

                var toUpdate = await Query(FilesDbContext.Folders)
                    .Where(r => r.Id == folderId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                toUpdate.ParentId = toFolderId;
                toUpdate.ModifiedOn = DateTime.UtcNow;
                toUpdate.ModifiedBy = AuthContext.CurrentAccount.ID;

                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var subfolders = await FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => r.ParentId == folderId)
                    .ToDictionaryAsync(r => r.FolderId, r => r.Level)
                    .ConfigureAwait(false);

                var toDelete = FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => subfolders.Keys.Contains(r.FolderId) && !subfolders.Keys.Contains(r.ParentId));

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

                recalcFolders.ForEach(async e => await RecalculateFoldersCountAsync(e).ConfigureAwait(false));
                recalcFolders.ForEach(async fid => await GetRecalculateFilesCountUpdateAsync(fid).ConfigureAwait(false));
            }
            return folderId;
        }

        public string MoveFolder(int folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<string> MoveFolderAsync(int folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var toSelector = ProviderFolderDao.GetSelector(toFolderId);

            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, DaoFactory.GetFileDao<int>(), r => r,
                toFolderId, toSelector.GetFolderDao(toFolderId), toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                true, cancellationToken)
                .ConfigureAwait(false);

            return moved.ID;
        }


        public Folder<TTo> CopyFolder<TTo>(int folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
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

        public Folder<int> CopyFolder(int folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<int>> CopyFolderAsync(int folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var folder = await GetFolderAsync(folderId).ConfigureAwait(false);

            var toFolder = await GetFolderAsync(toFolderId).ConfigureAwait(false);

            if (folder.FolderType == FolderType.BUNCH)
                folder.FolderType = FolderType.DEFAULT;

            var copy = ServiceProvider.GetService<Folder<int>>();
            copy.FolderID = toFolderId;
            copy.RootFolderId = toFolder.RootFolderId;
            copy.RootFolderCreator = toFolder.RootFolderCreator;
            copy.RootFolderType = toFolder.RootFolderType;
            copy.Title = folder.Title;
            copy.FolderType = folder.FolderType;

            copy = await GetFolderAsync(await SaveFolderAsync(copy).ConfigureAwait(false)).ConfigureAwait(false);

            //FactoryIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, copy));
            return copy;
        }

        public Folder<string> CopyFolder(int folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<string>> CopyFolderAsync(int folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var toSelector = ProviderFolderDao.GetSelector(toFolderId);

            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, DaoFactory.GetFileDao<int>(), r => r,
                toFolderId, toSelector.GetFolderDao(toFolderId), toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                false, cancellationToken)
                .ConfigureAwait(false);

            return moved;
        }

        public IDictionary<int, string> CanMoveOrCopy<TTo>(int[] folderIds, TTo to)
        {
            if (to is int tId)
            {
                return CanMoveOrCopy(folderIds, tId);
            }

            if (to is string tsId)
            {
                return CanMoveOrCopy(folderIds, tsId);
            }

            throw new NotImplementedException();
        }

        public async Task<IDictionary<int, string>> CanMoveOrCopyAsync<TTo>(int[] folderIds, TTo to)
        {
            if (to is int tId)
            {
                return await CanMoveOrCopyAsync(folderIds, tId).ConfigureAwait(false);
            }

            if (to is string tsId)
            {
                return await CanMoveOrCopyAsync(folderIds, tsId).ConfigureAwait(false);
            }

            throw new NotImplementedException();
        }

        public IDictionary<int, string> CanMoveOrCopy(int[] folderIds, string to)
        {
            return new Dictionary<int, string>();
        }

        public Task<IDictionary<int, string>> CanMoveOrCopyAsync(int[] folderIds, string to)
        {
            return Task.FromResult((IDictionary<int, string>)new Dictionary<int, string>());
        }

        public IDictionary<int, string> CanMoveOrCopy(int[] folderIds, int to)
        {
            var result = new Dictionary<int, string>();

            foreach (var folderId in folderIds)
            {
                var exists = FilesDbContext.Tree
                    .AsNoTracking()
                    .Any(r => r.ParentId == folderId && r.FolderId == to);

                if (exists)
                {
                    throw new InvalidOperationException(FilesCommonResource.ErrorMassage_FolderCopyError);
                }

                var title = Query(FilesDbContext.Folders)
                    .AsNoTracking()
                    .Where(r => r.Id == folderId)
                    .Select(r => r.Title.ToLower())
                    .FirstOrDefault();

                var conflict = Query(FilesDbContext.Folders)
                    .AsNoTracking()
                    .Where(r => r.Title.ToLower() == title)
                    .Where(r => r.ParentId == to)
                    .Select(r => r.Id)
                    .FirstOrDefault();

                if (conflict != 0)
                {
                    FilesDbContext.Files
                        .AsNoTracking()
                        .Join(FilesDbContext.Files, f1 => f1.Title.ToLower(), f2 => f2.Title.ToLower(), (f1, f2) => new { f1, f2 })
                        .Where(r => r.f1.TenantId == TenantID && r.f1.CurrentVersion && r.f1.FolderId == folderId)
                        .Where(r => r.f2.TenantId == TenantID && r.f2.CurrentVersion && r.f2.FolderId == conflict)
                        .Select(r => r.f1)
                        .ToList()
                        .ForEach(r => result[r.Id] = r.Title);

                    var childs = Query(FilesDbContext.Folders)
                        .AsNoTracking()
                        .Where(r => r.ParentId == folderId)
                        .Select(r => r.Id);

                    foreach (var pair in CanMoveOrCopy(childs.ToArray(), conflict))
                    {
                        result.Add(pair.Key, pair.Value);
                    }
                }
            }

            return result;
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
                        .Where(r => r.f1.TenantId == TenantID && r.f1.CurrentVersion && r.f1.FolderId == folderId)
                        .Where(r => r.f2.TenantId == TenantID && r.f2.CurrentVersion && r.f2.FolderId == conflict)
                        .Select(r => r.f1)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    files.ForEach(r => result[r.Id] = r.Title);

                    var childs = Query(FilesDbContext.Folders)
                        .AsNoTracking()
                        .Where(r => r.ParentId == folderId)
                        .Select(r => r.Id);

                    foreach (var pair in await CanMoveOrCopyAsync(childs.ToArray(), conflict).ConfigureAwait(false))
                    {
                        result.Add(pair.Key, pair.Value);
                    }
                }
            }

            return result;
        }

        public int RenameFolder(Folder<int> folder, string newTitle)
        {
            return RenameFolderAsync(folder, newTitle).Result;
        }

        public async Task<int> RenameFolderAsync(Folder<int> folder, string newTitle)
        {
            var toUpdate = await Query(FilesDbContext.Folders)
                .Where(r => r.Id == folder.ID)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            toUpdate.Title = Global.ReplaceInvalidCharsAndTruncate(newTitle);
            toUpdate.ModifiedOn = DateTime.UtcNow;
            toUpdate.ModifiedBy = AuthContext.CurrentAccount.ID;

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            FactoryIndexer.IndexAsync(toUpdate).ConfigureAwait(false);

            return folder.ID;
        }


        public int GetItemsCount(int folderId)
        {
            return GetItemsCountAsync(folderId).Result;
        }

        public async Task<int> GetItemsCountAsync(int folderId)
        {
            return await GetFoldersCountAsync(folderId).ConfigureAwait(false) +
                   await GetFilesCountAsync(folderId).ConfigureAwait(false);
        }

        private int GetFoldersCount(int parentId)
        {
            return GetFoldersCountAsync(parentId).Result;
        }

        private async Task<int> GetFoldersCountAsync(int parentId)
        {
            var count = await FilesDbContext.Tree
                .AsQueryable()
                .CountAsync(r => r.ParentId == parentId && r.Level > 0)
                .ConfigureAwait(false);

            return count;
        }

        private int GetFilesCount(int folderId)
        {
            return GetFilesCountAsync(folderId).Result;
        }

        private async Task<int> GetFilesCountAsync(int folderId)
        {
            var count = await Query(FilesDbContext.Files)
                .Join(FilesDbContext.Tree, r => r.FolderId, r => r.FolderId, (file, tree) => new { tree, file })
                .Where(r => r.tree.ParentId == folderId)
                .Select(r => r.file.Id)
                .Distinct()
                .CountAsync()
                .ConfigureAwait(false);

            return count;
        }

        public bool IsEmpty(int folderId)
        {
            return GetItemsCount(folderId) == 0;
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

        public long GetMaxUploadSize(int folderId, bool chunkedUpload)
        {
            return GetMaxUploadSizeAsync(folderId, chunkedUpload).Result;
        }

        public Task<long> GetMaxUploadSizeAsync(int folderId, bool chunkedUpload)
        {
            var tmp = long.MaxValue;

            if (CoreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
                tmp = CoreConfiguration.PersonalMaxSpace(SettingsManager) - GlobalSpace.GetUserUsedSpace();

            return Task.FromResult(Math.Min(tmp, chunkedUpload ? SetupInfo.MaxChunkedUploadSize(TenantExtra, TenantStatisticProvider) : SetupInfo.MaxUploadSize(TenantExtra, TenantStatisticProvider)));
        }

        private void RecalculateFoldersCount(int id)
        {
            RecalculateFoldersCountAsync(id).Wait();
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

        public void ReassignFolders(int[] folderIds, Guid newOwnerId)
        {
            var toUpdate = Query(FilesDbContext.Folders)
                .Where(r => folderIds.Contains(r.Id));

            foreach (var f in toUpdate)
            {
                f.CreateBy = newOwnerId;
            }

            FilesDbContext.SaveChanges();
        }

        public async Task ReassignFoldersAsync(int[] folderIds, Guid newOwnerId)
        {
            var toUpdate = Query(FilesDbContext.Folders)
                .Where(r => folderIds.Contains(r.Id));

            foreach (var f in toUpdate)
            {
                f.CreateBy = newOwnerId;
            }

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
        }


        public IEnumerable<Folder<int>> SearchFolders(string text, bool bunch)
        {
            return Search(text).Where(f => bunch
                                               ? f.RootFolderType == FolderType.BUNCH
                                               : (f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON)).ToList();
        }

        private IEnumerable<Folder<int>> Search(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<Folder<int>>();

            if (FactoryIndexer.TrySelectIds(s => s.MatchAll(text), out var ids))
            {
                var q1 = GetFolderQuery(r => ids.Contains(r.Id));
                return FromQueryWithShared(q1).Select(ToFolder).ToList();
            }

            var q = BuildSearch(GetFolderQuery(), text, SearhTypeEnum.Any);
            return FromQueryWithShared(q).Select(ToFolder).ToList();
        }

        public IEnumerable<int> GetFolderIDs(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(bunch)) throw new ArgumentNullException("bunch");

            var keys = data.Select(id => string.Format("{0}/{1}/{2}", module, bunch, id)).ToArray();

            var folderIdsDictionary = Query(FilesDbContext.BunchObjects)
                .AsNoTracking()
                .Where(r => keys.Length > 1 ? keys.Any(a => a == r.RightNode) : r.RightNode == keys[0])
                .ToDictionary(r => r.RightNode, r => r.LeftNode);

            var folderIds = new List<int>();

            foreach (var key in keys)
            {
                var newFolderId = 0;
                if (createIfNotExists && !folderIdsDictionary.TryGetValue(key, out var folderId))
                {
                    var folder = ServiceProvider.GetService<Folder<int>>();
                    switch (bunch)
                    {
                        case my:
                            folder.FolderType = FolderType.USER;
                            folder.Title = my;
                            break;
                        case common:
                            folder.FolderType = FolderType.COMMON;
                            folder.Title = common;
                            break;
                        case trash:
                            folder.FolderType = FolderType.TRASH;
                            folder.Title = trash;
                            break;
                        case share:
                            folder.FolderType = FolderType.SHARE;
                            folder.Title = share;
                            break;
                        case recent:
                            folder.FolderType = FolderType.Recent;
                            folder.Title = recent;
                            break;
                        case favorites:
                            folder.FolderType = FolderType.Favorites;
                            folder.Title = favorites;
                            break;
                        case templates:
                            folder.FolderType = FolderType.Templates;
                            folder.Title = templates;
                            break;
                        case privacy:
                            folder.FolderType = FolderType.Privacy;
                            folder.Title = privacy;
                            break;
                        case projects:
                            folder.FolderType = FolderType.Projects;
                            folder.Title = projects;
                            break;
                        default:
                            folder.FolderType = FolderType.BUNCH;
                            folder.Title = key;
                            break;
                    }
                    using var tx = FilesDbContext.Database.BeginTransaction();//NOTE: Maybe we shouldn't start transaction here at all

                    newFolderId = SaveFolder(folder, tx); //Save using our db manager

                    var newBunch = new DbFilesBunchObjects
                    {
                        LeftNode = newFolderId.ToString(),
                        RightNode = key,
                        TenantId = TenantID
                    };

                    FilesDbContext.AddOrUpdate(r => r.BunchObjects, newBunch);
                    FilesDbContext.SaveChanges();

                    tx.Commit(); //Commit changes
                }
                folderIds.Add(newFolderId);
            }
            return folderIds;
        }

        public async Task<IEnumerable<int>> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(bunch)) throw new ArgumentNullException("bunch");

            var keys = data.Select(id => string.Format("{0}/{1}/{2}", module, bunch, id)).ToArray();

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
                    var folder = ServiceProvider.GetService<Folder<int>>();
                    switch (bunch)
                    {
                        case my:
                            folder.FolderType = FolderType.USER;
                            folder.Title = my;
                            break;
                        case common:
                            folder.FolderType = FolderType.COMMON;
                            folder.Title = common;
                            break;
                        case trash:
                            folder.FolderType = FolderType.TRASH;
                            folder.Title = trash;
                            break;
                        case share:
                            folder.FolderType = FolderType.SHARE;
                            folder.Title = share;
                            break;
                        case recent:
                            folder.FolderType = FolderType.Recent;
                            folder.Title = recent;
                            break;
                        case favorites:
                            folder.FolderType = FolderType.Favorites;
                            folder.Title = favorites;
                            break;
                        case templates:
                            folder.FolderType = FolderType.Templates;
                            folder.Title = templates;
                            break;
                        case privacy:
                            folder.FolderType = FolderType.Privacy;
                            folder.Title = privacy;
                            break;
                        case projects:
                            folder.FolderType = FolderType.Projects;
                            folder.Title = projects;
                            break;
                        default:
                            folder.FolderType = FolderType.BUNCH;
                            folder.Title = key;
                            break;
                    }
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
                }
                folderIds.Add(newFolderId);
            }
            return folderIds;
        }

        public int GetFolderID(string module, string bunch, string data, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(bunch)) throw new ArgumentNullException("bunch");

            var key = string.Format("{0}/{1}/{2}", module, bunch, data);
            var folderId = Query(FilesDbContext.BunchObjects)
                .Where(r => r.RightNode == key)
                .Select(r => r.LeftNode)
                .FirstOrDefault();

            if (folderId != null)
            {
                return Convert.ToInt32(folderId);
            }

            var newFolderId = 0;
            if (createIfNotExists)
            {
                var folder = ServiceProvider.GetService<Folder<int>>();
                folder.FolderID = 0;
                switch (bunch)
                {
                    case my:
                        folder.FolderType = FolderType.USER;
                        folder.Title = my;
                        folder.CreateBy = new Guid(data.ToString());
                        break;
                    case common:
                        folder.FolderType = FolderType.COMMON;
                        folder.Title = common;
                        break;
                    case trash:
                        folder.FolderType = FolderType.TRASH;
                        folder.Title = trash;
                        folder.CreateBy = new Guid(data.ToString());
                        break;
                    case share:
                        folder.FolderType = FolderType.SHARE;
                        folder.Title = share;
                        break;
                    case recent:
                        folder.FolderType = FolderType.Recent;
                        folder.Title = recent;
                        break;
                    case favorites:
                        folder.FolderType = FolderType.Favorites;
                        folder.Title = favorites;
                        break;
                    case templates:
                        folder.FolderType = FolderType.Templates;
                        folder.Title = templates;
                        break;
                    case privacy:
                        folder.FolderType = FolderType.Privacy;
                        folder.Title = privacy;
                        folder.CreateBy = new Guid(data);
                        break;
                    case projects:
                        folder.FolderType = FolderType.Projects;
                        folder.Title = projects;
                        break;
                    default:
                        folder.FolderType = FolderType.BUNCH;
                        folder.Title = key;
                        break;
                }
                using var tx = FilesDbContext.Database.BeginTransaction(); //NOTE: Maybe we shouldn't start transaction here at all
                newFolderId = SaveFolder(folder, tx); //Save using our db manager
                var toInsert = new DbFilesBunchObjects
                {
                    LeftNode = newFolderId.ToString(),
                    RightNode = key,
                    TenantId = TenantID
                };

                FilesDbContext.AddOrUpdate(r => r.BunchObjects, toInsert);
                FilesDbContext.SaveChanges();

                tx.Commit(); //Commit changes
            }



            return newFolderId;
        }

        public async Task<int> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(bunch)) throw new ArgumentNullException("bunch");

            var key = string.Format("{0}/{1}/{2}", module, bunch, data);
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
                var folder = ServiceProvider.GetService<Folder<int>>();
                folder.FolderID = 0;
                switch (bunch)
                {
                    case my:
                        folder.FolderType = FolderType.USER;
                        folder.Title = my;
                        folder.CreateBy = new Guid(data.ToString());
                        break;
                    case common:
                        folder.FolderType = FolderType.COMMON;
                        folder.Title = common;
                        break;
                    case trash:
                        folder.FolderType = FolderType.TRASH;
                        folder.Title = trash;
                        folder.CreateBy = new Guid(data.ToString());
                        break;
                    case share:
                        folder.FolderType = FolderType.SHARE;
                        folder.Title = share;
                        break;
                    case recent:
                        folder.FolderType = FolderType.Recent;
                        folder.Title = recent;
                        break;
                    case favorites:
                        folder.FolderType = FolderType.Favorites;
                        folder.Title = favorites;
                        break;
                    case templates:
                        folder.FolderType = FolderType.Templates;
                        folder.Title = templates;
                        break;
                    case privacy:
                        folder.FolderType = FolderType.Privacy;
                        folder.Title = privacy;
                        folder.CreateBy = new Guid(data);
                        break;
                    case projects:
                        folder.FolderType = FolderType.Projects;
                        folder.Title = projects;
                        break;
                    default:
                        folder.FolderType = FolderType.BUNCH;
                        folder.Title = key;
                        break;
                }
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
            }



            return newFolderId;
        }

        int IFolderDao<int>.GetFolderIDProjects(bool createIfNotExists)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, projects, null, createIfNotExists);
        }

        async Task<int> IFolderDao<int>.GetFolderIDProjectsAsync(bool createIfNotExists)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, projects, null, createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDTrash(bool createIfNotExists, Guid? userId = null)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, trash, (userId ?? AuthContext.CurrentAccount.ID).ToString(), createIfNotExists);
        }

        public async Task<int> GetFolderIDTrashAsync(bool createIfNotExists, Guid? userId = null)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, trash, (userId ?? AuthContext.CurrentAccount.ID).ToString(), createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDCommon(bool createIfNotExists)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, common, null, createIfNotExists);
        }

        public async Task<int> GetFolderIDCommonAsync(bool createIfNotExists)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, common, null, createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDUser(bool createIfNotExists, Guid? userId = null)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, my, (userId ?? AuthContext.CurrentAccount.ID).ToString(), createIfNotExists);
        }

        public async Task<int> GetFolderIDUserAsync(bool createIfNotExists, Guid? userId = null)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, my, (userId ?? AuthContext.CurrentAccount.ID).ToString(), createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDShare(bool createIfNotExists)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, share, null, createIfNotExists);
        }

        public async Task<int> GetFolderIDShareAsync(bool createIfNotExists)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, share, null, createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDRecent(bool createIfNotExists)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, recent, null, createIfNotExists);
        }

        public async Task<int> GetFolderIDRecentAsync(bool createIfNotExists)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, recent, null, createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDFavorites(bool createIfNotExists)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, favorites, null, createIfNotExists);
        }

        public async Task<int> GetFolderIDFavoritesAsync(bool createIfNotExists)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, favorites, null, createIfNotExists).ConfigureAwait(false);
        }

        public int GetFolderIDTemplates(bool createIfNotExists)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, templates, null, createIfNotExists);
        }

        public async Task<int> GetFolderIDTemplatesAsync(bool createIfNotExists)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, templates, null, createIfNotExists).ConfigureAwait(false);
        }
        public int GetFolderIDPrivacy(bool createIfNotExists, Guid? userId = null)
        {
            return (this as IFolderDao<int>).GetFolderID(FileConstant.ModuleId, privacy, (userId ?? AuthContext.CurrentAccount.ID).ToString(), createIfNotExists);
        }

        public async Task<int> GetFolderIDPrivacyAsync(bool createIfNotExists, Guid? userId = null)
        {
            return await (this as IFolderDao<int>).GetFolderIDAsync(FileConstant.ModuleId, privacy, (userId ?? AuthContext.CurrentAccount.ID).ToString(), createIfNotExists).ConfigureAwait(false);
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

        public Folder<int> ToFolder(DbFolderQuery r)
        {
            if (r == null) return null;
            var result = ServiceProvider.GetService<Folder<int>>();
            result.ID = r.Folder.Id;
            result.FolderID = r.Folder.ParentId;
            result.Title = r.Folder.Title;
            result.CreateOn = TenantUtil.DateTimeFromUtc(r.Folder.CreateOn);
            result.CreateBy = r.Folder.CreateBy;
            result.ModifiedOn = TenantUtil.DateTimeFromUtc(r.Folder.ModifiedOn);
            result.ModifiedBy = r.Folder.ModifiedBy;
            result.FolderType = r.Folder.FolderType;
            result.TotalSubFolders = r.Folder.FoldersCount;
            result.TotalFiles = r.Folder.FilesCount;
            result.RootFolderType = r.Root?.FolderType ?? default;
            result.RootFolderCreator = r.Root?.CreateBy ?? default;
            result.RootFolderId = r.Root?.Id ?? default;
            result.Shared = r.Shared;

            switch (result.FolderType)
            {
                case FolderType.COMMON:
                    result.Title = FilesUCResource.CorporateFiles;
                    break;
                case FolderType.USER:
                    result.Title = FilesUCResource.MyFiles;
                    break;
                case FolderType.SHARE:
                    result.Title = FilesUCResource.SharedForMe;
                    break;
                case FolderType.Recent:
                    result.Title = FilesUCResource.Recent;
                    break;
                case FolderType.Favorites:
                    result.Title = FilesUCResource.Favorites;
                    break;
                case FolderType.TRASH:
                    result.Title = FilesUCResource.Trash;
                    break;
                case FolderType.Privacy:
                    result.Title = FilesUCResource.PrivacyRoom;
                    break;
                case FolderType.Projects:
                    result.Title = FilesUCResource.ProjectFiles;
                    break;
                case FolderType.BUNCH:
                    try
                    {
                        result.Title = GetProjectTitle(result.ID);
                    }
                    catch (Exception)
                    {
                        //Global.Logger.Error(e);
                    }
                    break;
            }

            if (result.FolderType != FolderType.DEFAULT && 0.Equals(result.FolderID)) result.RootFolderType = result.FolderType;
            if (result.FolderType != FolderType.DEFAULT && result.RootFolderCreator == default) result.RootFolderCreator = result.CreateBy;
            if (result.FolderType != FolderType.DEFAULT && 0.Equals(result.RootFolderId)) result.RootFolderId = result.ID;

            return result;
        }

        public (Folder<int>, SmallShareRecord) ToFolderWithShare(DbFolderQueryWithSecurity r)
        {
            var file = ToFolder(r.DbFolderQuery);
            var record = r.Security != null
                ? new SmallShareRecord
                {
                    ShareOn = r.Security.TimeStamp,
                    ShareBy = r.Security.Owner,
                    ShareTo = r.Security.Subject
                }
                : null;

            return (file, record);
        }

        public string GetBunchObjectID(int folderID)
        {
            return Query(FilesDbContext.BunchObjects)
                .Where(r => r.LeftNode == (folderID).ToString())
                .Select(r => r.RightNode)
                .FirstOrDefault();
        }

        public async Task<string> GetBunchObjectIDAsync(int folderID)
        {
            return await Query(FilesDbContext.BunchObjects)
                .Where(r => r.LeftNode == (folderID).ToString())
                .Select(r => r.RightNode)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public Dictionary<string, string> GetBunchObjectIDs(List<int> folderIDs)
        {
            var folderSIds = folderIDs.Select(r => r.ToString()).ToList();

            return Query(FilesDbContext.BunchObjects)
                .Where(r => folderSIds.Any(a => a == r.LeftNode))
                .ToDictionary(r => r.LeftNode, r => r.RightNode);
        }

        public async Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<int> folderIDs)
        {
            var folderSIds = folderIDs.Select(r => r.ToString()).ToList();

            return await Query(FilesDbContext.BunchObjects)
                .Where(r => folderSIds.Any(a => a == r.LeftNode))
                .ToDictionaryAsync(r => r.LeftNode, r => r.RightNode)
                .ConfigureAwait(false);
        }

        public IEnumerable<(Folder<int>, SmallShareRecord)> GetFeedsForFolders(int tenant, DateTime from, DateTime to)
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
                .Where(r => r.Security.Security == FileShare.Restrict)
                .Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to);

            return q2.Select(ToFolderWithShare).ToList().Union(q4.Select(ToFolderWithShare).ToList());
        }

        public IEnumerable<int> GetTenantsWithFeedsForFolders(DateTime fromTime)
        {
            var q1 = FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.ModifiedOn > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Count() > 0)
                .Select(r => r.Key)
                .ToList();

            var q2 = FilesDbContext.Security
                .AsQueryable()
                .Where(r => r.TimeStamp > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Count() > 0)
                .Select(r => r.Key)
                .ToList();

            return q1.Union(q2);
        }

        public async Task<IEnumerable<int>> GetTenantsWithFeedsForFoldersAsync(DateTime fromTime)
        {
            var q1 = await FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.ModifiedOn > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Count() > 0)
                .Select(r => r.Key)
                .ToListAsync()
                .ConfigureAwait(false);

            var q2 = await FilesDbContext.Security
                .AsQueryable()
                .Where(r => r.TimeStamp > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Count() > 0)
                .Select(r => r.Key)
                .ToListAsync()
                .ConfigureAwait(false);

            return q1.Union(q2);
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
}