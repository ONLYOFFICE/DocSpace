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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Service;
using ASC.Files.Core.EF;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Files.Core.Thirdparty;
using ASC.Files.Thirdparty.ProviderDao;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Search;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.Data
{
    [Scope]
    internal class FileDao : AbstractDao, IFileDao<int>
    {
        private static readonly object syncRoot = new object();
        private FactoryIndexerFile FactoryIndexer { get; }
        private GlobalStore GlobalStore { get; }
        private GlobalSpace GlobalSpace { get; }
        private GlobalFolder GlobalFolder { get; }
        private Global Global { get; }
        private IDaoFactory DaoFactory { get; }
        private ChunkedUploadSessionHolder ChunkedUploadSessionHolder { get; }
        private ProviderFolderDao ProviderFolderDao { get; }
        private CrossDao CrossDao { get; }
        private Settings Settings { get; }

        public FileDao(
            FactoryIndexerFile factoryIndexer,
            UserManager userManager,
            DbContextManager<EF.FilesDbContext> dbContextManager,
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
            Settings settings)
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
            FactoryIndexer = factoryIndexer;
            GlobalStore = globalStore;
            GlobalSpace = globalSpace;
            GlobalFolder = globalFolder;
            Global = global;
            DaoFactory = daoFactory;
            ChunkedUploadSessionHolder = chunkedUploadSessionHolder;
            ProviderFolderDao = providerFolderDao;
            CrossDao = crossDao;
            Settings = settings;
        }

        public Task InvalidateCacheAsync(int fileId)
        {
            return Task.CompletedTask;
        }

        public async Task<File<int>> GetFileAsync(int fileId)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.CurrentVersion).AsNoTracking();

            return ToFile(await
                FromQueryWithShared(query)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false));
        }

        public async Task<File<int>> GetFileAsync(int fileId, int fileVersion)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.Version == fileVersion).AsNoTracking();
            return ToFile(await
                FromQueryWithShared(query)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false));
        }

        public Task<File<int>> GetFileAsync(int parentId, string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(title);

            return InternalGetFileAsync(parentId, title);
        }

        private async Task<File<int>> InternalGetFileAsync(int parentId, string title)
        {
            var query = GetFileQuery(r => r.Title == title && r.CurrentVersion && r.FolderId == parentId)
                .AsNoTracking()
                .OrderBy(r => r.CreateOn);

            return ToFile(await FromQueryWithShared(query).FirstOrDefaultAsync().ConfigureAwait(false));
        }


        public async Task<File<int>> GetFileStableAsync(int fileId, int fileVersion = -1)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.Forcesave == ForcesaveType.None)
                .AsNoTracking();

            if (fileVersion >= 0)
            {
                query = query.Where(r => r.Version <= fileVersion);
            }

            query = query.OrderByDescending(r => r.Version);

            return ToFile(await FromQueryWithShared(query).FirstOrDefaultAsync().ConfigureAwait(false));
        }

        public IAsyncEnumerable<File<int>> GetFileHistoryAsync(int fileId)
        {
            var query = GetFileQuery(r => r.Id == fileId).OrderByDescending(r => r.Version).AsNoTracking();

            return FromQueryWithShared(query).AsAsyncEnumerable().Select(ToFile);
        }

        public IAsyncEnumerable<File<int>> GetFilesAsync(IEnumerable<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any()) return AsyncEnumerable.Empty<File<int>>();

            var query = GetFileQuery(r => fileIds.Contains(r.Id) && r.CurrentVersion)
                .AsNoTracking();

            return FromQueryWithShared(query).AsAsyncEnumerable().Select(e => ToFile(e));
        }

        public IAsyncEnumerable<File<int>> GetFilesFilteredAsync(IEnumerable<int> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
        {
            if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly) return AsyncEnumerable.Empty<File<int>>();

            var query = GetFileQuery(r => fileIds.Contains(r.Id) && r.CurrentVersion).AsNoTracking();

            if (!string.IsNullOrEmpty(searchText))
            {
                var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

                if (FactoryIndexer.TrySelectIds(s => func(s).In(r => r.Id, fileIds.ToArray()), out var searchIds))
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
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
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

            return (checkShared ? FromQueryWithShared(query) : FromQuery(query)).AsAsyncEnumerable().Select(e => ToFile(e));
        }

        public Task<List<int>> GetFilesAsync(int parentId)
        {
            return Query(FilesDbContext.Files)
                .AsNoTracking()
                .Where(r => r.FolderId == parentId && r.CurrentVersion)
                .Select(r => r.Id)
                .ToListAsync()
;
        }

        public IAsyncEnumerable<File<int>> GetFilesAsync(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return AsyncEnumerable.Empty<File<int>>();

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var q = GetFileQuery(r => r.FolderId == parentId && r.CurrentVersion).AsNoTracking();


            if (withSubfolders)
            {
                q = GetFileQuery(r => r.CurrentVersion)
                    .AsNoTracking()
                    .Join(FilesDbContext.Tree, r => r.FolderId, a => a.FolderId, (file, tree) => new { file, tree })
                    .Where(r => r.tree.ParentId == parentId)
                    .Select(r => r.file);
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                var func = GetFuncForSearch(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders);

                Expression<Func<Selector<DbFile>, Selector<DbFile>>> expression = s => func(s);

                if (FactoryIndexer.TrySelectIds(expression, out var searchIds))
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
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
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

            return FromQueryWithShared(q).AsAsyncEnumerable().Select(ToFile);
        }

        public Task<Stream> GetFileStreamAsync(File<int> file, long offset)
        {
            return GlobalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file), (int)offset);
        }

        public Task<Uri> GetPreSignedUriAsync(File<int> file, TimeSpan expires)
        {
            return GlobalStore.GetStore().GetPreSignedUriAsync(string.Empty, GetUniqFilePath(file), expires,
                                                     new List<string>
                                                         {
                                                             string.Concat("Content-Disposition:", ContentDispositionUtil.GetHeaderValue(file.Title, withoutBase: true))
                                                         });
        }

        public Task<bool> IsSupportedPreSignedUriAsync(File<int> file)
        {
            return Task.FromResult(GlobalStore.GetStore().IsSupportedPreSignedUri);
        }

        public Task<Stream> GetFileStreamAsync(File<int> file)
        {
            return GlobalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file), 0);
        }

        public Task<File<int>> SaveFileAsync(File<int> file, Stream fileStream)
        {
            return SaveFileAsync(file, fileStream, true);
        }

        public Task<File<int>> SaveFileAsync(File<int> file, Stream fileStream, bool checkQuota = true)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var maxChunkedUploadSize = SetupInfo.MaxChunkedUploadSize(TenantExtra, TenantStatisticProvider);
            if (checkQuota && maxChunkedUploadSize < file.ContentLength)
            {
                throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
            }
            return InternalSaveFileAsync(file, fileStream, checkQuota);
        }

        private async Task<File<int>> InternalSaveFileAsync(File<int> file, Stream fileStream, bool checkQuota = true)
        {
            if (checkQuota && CoreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
            {
                var personalMaxSpace = CoreConfiguration.PersonalMaxSpace(SettingsManager);
                if (personalMaxSpace - await GlobalSpace.GetUserUsedSpaceAsync(file.ID == default ? AuthContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
                {
                    throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
                }
            }

            var isNew = false;
            List<int> parentFoldersIds;
            DbFile toInsert = null;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();

                if (file.ID == default)
                {
                    file.ID = FilesDbContext.Files.Any() ? FilesDbContext.Files.Max(r => r.Id) + 1 : 1;
                    file.Version = 1;
                    file.VersionGroup = 1;
                    isNew = true;
                }

                file.Title = Global.ReplaceInvalidCharsAndTruncate(file.Title);
                //make lowerCase
                file.Title = FileUtility.ReplaceFileExtension(file.Title, FileUtility.GetFileExtension(file.Title));

                file.ModifiedBy = AuthContext.CurrentAccount.ID;
                file.ModifiedOn = TenantUtil.DateTimeNow();
                if (file.CreateBy == default) file.CreateBy = AuthContext.CurrentAccount.ID;
                if (file.CreateOn == default) file.CreateOn = TenantUtil.DateTimeNow();

                var toUpdate = FilesDbContext.Files
                    .FirstOrDefault(r => r.Id == file.ID && r.CurrentVersion && r.TenantId == TenantID);

                if (toUpdate != null)
                {
                    toUpdate.CurrentVersion = false;
                    FilesDbContext.SaveChanges();
                }

                toInsert = new DbFile
                {
                    Id = file.ID,
                    Version = file.Version,
                    VersionGroup = file.VersionGroup,
                    CurrentVersion = true,
                    FolderId = file.FolderID,
                    Title = file.Title,
                    ContentLength = file.ContentLength,
                    Category = (int)file.FilterType,
                    CreateBy = file.CreateBy,
                    CreateOn = TenantUtil.DateTimeToUtc(file.CreateOn),
                    ModifiedBy = file.ModifiedBy,
                    ModifiedOn = TenantUtil.DateTimeToUtc(file.ModifiedOn),
                    ConvertedType = file.ConvertedType,
                    Comment = file.Comment,
                    Encrypted = file.Encrypted,
                    Forcesave = file.Forcesave,
                    Thumb = file.ThumbnailStatus,
                    TenantId = TenantID
                };

                FilesDbContext.AddOrUpdate(r => r.Files, toInsert);
                FilesDbContext.SaveChanges();

                tx.Commit();

                file.PureTitle = file.Title;

                var parentFolders =
                    FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => r.FolderId == file.FolderID)
                    .OrderByDescending(r => r.Level)
                    .ToList();

                parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

                if (parentFoldersIds.Count > 0)
                {
                    var folderToUpdate = FilesDbContext.Folders
                        .AsQueryable()
                        .Where(r => parentFoldersIds.Contains(r.Id));

                    foreach (var f in folderToUpdate)
                    {
                        f.ModifiedOn = TenantUtil.DateTimeToUtc(file.ModifiedOn);
                        f.ModifiedBy = file.ModifiedBy;
                    }

                    FilesDbContext.SaveChanges();
                }

                toInsert.Folders = parentFolders;

                if (isNew)
                {
                    RecalculateFilesCountAsync(file.FolderID).Wait();
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
                            var stored = await GlobalStore.GetStore().IsDirectoryAsync(GetUniqFileDirectory(file.ID));
                            await DeleteFileAsync(file.ID, stored).ConfigureAwait(false);
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

            _ = FactoryIndexer.IndexAsync(await InitDocumentAsync(toInsert).ConfigureAwait(false));

            return await GetFileAsync(file.ID).ConfigureAwait(false);
        }

        public Task<File<int>> ReplaceFileVersionAsync(File<int> file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (file.ID == default) throw new ArgumentException("No file id or folder id toFolderId determine provider");

            var maxChunkedUploadSize = SetupInfo.MaxChunkedUploadSize(TenantExtra, TenantStatisticProvider);

            if (maxChunkedUploadSize < file.ContentLength)
            {
                throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
            }
            return InternalReplaceFileVersionAsync(file, fileStream);
        }

        private async Task<File<int>> InternalReplaceFileVersionAsync(File<int> file, Stream fileStream)
        {
            if (CoreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
            {
                var personalMaxSpace = CoreConfiguration.PersonalMaxSpace(SettingsManager);
                if (personalMaxSpace - await GlobalSpace.GetUserUsedSpaceAsync(file.ID == default ? AuthContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
                {
                    throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
                }
            }

            DbFile toUpdate = null;

            List<int> parentFoldersIds;
            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();

                file.Title = Global.ReplaceInvalidCharsAndTruncate(file.Title);
                //make lowerCase
                file.Title = FileUtility.ReplaceFileExtension(file.Title, FileUtility.GetFileExtension(file.Title));

                file.ModifiedBy = AuthContext.CurrentAccount.ID;
                file.ModifiedOn = TenantUtil.DateTimeNow();
                if (file.CreateBy == default) file.CreateBy = AuthContext.CurrentAccount.ID;
                if (file.CreateOn == default) file.CreateOn = TenantUtil.DateTimeNow();

                toUpdate = FilesDbContext.Files
                    .FirstOrDefault(r => r.Id == file.ID && r.Version == file.Version && r.TenantId == TenantID);

                toUpdate.Version = file.Version;
                toUpdate.VersionGroup = file.VersionGroup;
                toUpdate.FolderId = file.FolderID;
                toUpdate.Title = file.Title;
                toUpdate.ContentLength = file.ContentLength;
                toUpdate.Category = (int)file.FilterType;
                toUpdate.CreateBy = file.CreateBy;
                toUpdate.CreateOn = TenantUtil.DateTimeToUtc(file.CreateOn);
                toUpdate.ModifiedBy = file.ModifiedBy;
                toUpdate.ModifiedOn = TenantUtil.DateTimeToUtc(file.ModifiedOn);
                toUpdate.ConvertedType = file.ConvertedType;
                toUpdate.Comment = file.Comment;
                toUpdate.Encrypted = file.Encrypted;
                toUpdate.Forcesave = file.Forcesave;
                toUpdate.Thumb = file.ThumbnailStatus;

                FilesDbContext.SaveChanges();

                tx.Commit();

                file.PureTitle = file.Title;

                var parentFolders = FilesDbContext.Tree
                    .AsQueryable()
                    .Where(r => r.FolderId == file.FolderID)
                    .OrderByDescending(r => r.Level)
                    .ToList();

                parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

                if (parentFoldersIds.Count > 0)
                {
                    var folderToUpdate = FilesDbContext.Folders
                        .AsQueryable()
                        .Where(r => parentFoldersIds.Contains(r.Id));

                    foreach (var f in folderToUpdate)
                    {
                        f.ModifiedOn = TenantUtil.DateTimeToUtc(file.ModifiedOn);
                        f.ModifiedBy = file.ModifiedBy;
                    }

                    FilesDbContext.SaveChanges();
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

            _ = FactoryIndexer.IndexAsync(await InitDocumentAsync(toUpdate).ConfigureAwait(false));

            return await GetFileAsync(file.ID).ConfigureAwait(false);
        }

        private Task DeleteVersionAsync(File<int> file)
        {
            if (file == null
                || file.ID == default
                || file.Version <= 1) return Task.CompletedTask;

            return InternalDeleteVersionAsync(file);
        }

        private async Task InternalDeleteVersionAsync(File<int> file)
        {
            var toDelete = await Query(FilesDbContext.Files)
                .FirstOrDefaultAsync(r => r.Id == file.ID && r.Version == file.Version)
                .ConfigureAwait(false);

            if (toDelete != null)
            {
                FilesDbContext.Files.Remove(toDelete);
            }
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            var toUpdate = await Query(FilesDbContext.Files)
                .FirstOrDefaultAsync(r => r.Id == file.ID && r.Version == file.Version - 1)
                .ConfigureAwait(false);

            toUpdate.CurrentVersion = true;
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task DeleteVersionStreamAsync(File<int> file)
        {
            await GlobalStore.GetStore().DeleteDirectoryAsync(GetUniqFileVersionPath(file.ID, file.Version));
        }

        private async Task SaveFileStreamAsync(File<int> file, Stream stream)
        {
            await GlobalStore.GetStore().SaveAsync(string.Empty, GetUniqFilePath(file), stream, file.Title);
        }

        public Task DeleteFileAsync(int fileId)
        {
            return DeleteFileAsync(fileId, true);
        }

        private Task DeleteFileAsync(int fileId, bool deleteFolder)
        {
            if (fileId == default) return Task.CompletedTask;

            return internalDeleteFileAsync(fileId, deleteFolder);
        }

        private async Task internalDeleteFileAsync(int fileId, bool deleteFolder)
        {
            using var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false);

            var fromFolders = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Select(a => a.FolderId)
                .Distinct()
                .AsAsyncEnumerable();

            var toDeleteLinks = Query(FilesDbContext.TagLink).Where(r => r.EntryId == fileId.ToString() && r.EntryType == FileEntryType.File);
            FilesDbContext.RemoveRange(await toDeleteLinks.ToListAsync());

            var toDeleteFiles = Query(FilesDbContext.Files).Where(r => r.Id == fileId);
            var toDeleteFile = await toDeleteFiles.FirstOrDefaultAsync(r => r.CurrentVersion).ConfigureAwait(false);

            foreach (var d in toDeleteFiles)
            {
                await FactoryIndexer.DeleteAsync(d).ConfigureAwait(false);
            }

            FilesDbContext.RemoveRange(await toDeleteFiles.ToListAsync());

            var tagsToRemove = Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Any(a => a.TagId == r.Id));

            FilesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

            var securityToDelete = Query(FilesDbContext.Security)
                .Where(r => r.EntryId == fileId.ToString())
                .Where(r => r.EntryType == FileEntryType.File);

            FilesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await tx.CommitAsync().ConfigureAwait(false);

            var forEachTask = fromFolders.ForEachAwaitAsync(async folderId => await RecalculateFilesCountAsync(folderId).ConfigureAwait(false)).ConfigureAwait(false);

            if (deleteFolder)
                await DeleteFolderAsync(fileId);

            if (toDeleteFile != null)
            {
                await FactoryIndexer.DeleteAsync(toDeleteFile).ConfigureAwait(false);
            }

            await forEachTask;
        }

        public Task<bool> IsExistAsync(string title, object folderId)
        {
            if (folderId is int fId) return IsExistAsync(title, fId);

            throw new NotImplementedException();
        }

        public Task<bool> IsExistAsync(string title, int folderId)
        {
            return Query(FilesDbContext.Files)
                .AsNoTracking()
                .AnyAsync(r => r.Title == title &&
                          r.FolderId == folderId &&
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
            if (fileId == default) return Task.FromResult<int>(default);

            return InternalMoveFileAsync(fileId, toFolderId);
        }

        private async Task<int> InternalMoveFileAsync(int fileId, int toFolderId)
        {
            List<DbFile> toUpdate;

            var trashIdTask = GlobalFolder.GetFolderTrashAsync<int>(DaoFactory);

            using (var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var fromFolders = await Query(FilesDbContext.Files)
                    .Where(r => r.Id == fileId)
                    .Select(a => a.FolderId)
                    .Distinct()
                    .AsAsyncEnumerable()
                    .ToListAsync()
                    .ConfigureAwait(false);

                toUpdate = await Query(FilesDbContext.Files)
                    .Where(r => r.Id == fileId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var f in toUpdate)
                {
                    f.FolderId = toFolderId;

                    var trashId = await trashIdTask;
                    if (trashId.Equals(toFolderId))
                    {
                        f.ModifiedBy = AuthContext.CurrentAccount.ID;
                        f.ModifiedOn = DateTime.UtcNow;
                    }
                }

                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);

                foreach (var f in fromFolders)
                {
                    await RecalculateFilesCountAsync(f).ConfigureAwait(false);
                }

                await RecalculateFilesCountAsync(toFolderId).ConfigureAwait(false);
            }

            var parentFoldersTask =
                FilesDbContext.Tree
                .AsQueryable()
                .Where(r => r.FolderId == toFolderId)
                .OrderByDescending(r => r.Level)
                .ToListAsync()
                .ConfigureAwait(false);

            var toUpdateFile = toUpdate.FirstOrDefault(r => r.CurrentVersion);

            if (toUpdateFile != null)
            {
                toUpdateFile.Folders = await parentFoldersTask;
                FactoryIndexer.Update(toUpdateFile, UpdateAction.Replace, w => w.Folders);
            }

            return fileId;
        }

        public async Task<string> MoveFileAsync(int fileId, string toFolderId)
        {
            var toSelector = ProviderFolderDao.GetSelector(toFolderId);

            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, r => r,
                toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                true)
                .ConfigureAwait(false);

            return moved.ID;
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
                var copy = ServiceProvider.GetService<File<int>>();
                copy.FileStatus = file.FileStatus;
                copy.FolderID = toFolderId;
                copy.Title = file.Title;
                copy.ConvertedType = file.ConvertedType;
                copy.Comment = FilesCommonResource.CommentCopy;
                copy.Encrypted = file.Encrypted;

                using (var stream = await GetFileStreamAsync(file).ConfigureAwait(false))
                {
                    copy.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                    copy = await SaveFileAsync(copy, stream).ConfigureAwait(false);
                }

                if (file.ThumbnailStatus == Thumbnail.Created)
                {
                    using (var thumbnail = await GetThumbnailAsync(file).ConfigureAwait(false))
                    {
                        await SaveThumbnailAsync(copy, thumbnail).ConfigureAwait(false);
                    }
                    copy.ThumbnailStatus = Thumbnail.Created;
                }

                return copy;
            }
            return null;
        }

        public async Task<File<string>> CopyFileAsync(int fileId, string toFolderId)
        {
            var toSelector = ProviderFolderDao.GetSelector(toFolderId);

            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, r => r,
                toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                false)
                .ConfigureAwait(false);

            return moved;
        }

        public async Task<int> FileRenameAsync(File<int> file, string newTitle)
        {
            newTitle = Global.ReplaceInvalidCharsAndTruncate(newTitle);
            var toUpdate = await Query(FilesDbContext.Files)
                .Where(r => r.Id == file.ID)
                .Where(r => r.CurrentVersion)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            toUpdate.Title = newTitle;
            toUpdate.ModifiedOn = DateTime.UtcNow;
            toUpdate.ModifiedBy = AuthContext.CurrentAccount.ID;

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await FactoryIndexer.UpdateAsync(toUpdate, true, r => r.Title, r => r.ModifiedBy, r => r.ModifiedOn).ConfigureAwait(false);

            return file.ID;
        }

        public async Task<string> UpdateCommentAsync(int fileId, int fileVersion, string comment)
        {
            var toUpdate = await Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            comment ??= string.Empty;
            comment = comment.Substring(0, Math.Min(comment.Length, 255));

            toUpdate.Comment = comment;

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            return comment;
        }

        public Task CompleteVersionAsync(int fileId, int fileVersion)
        {
            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version > fileVersion);

            foreach (var f in toUpdate)
            {
                f.VersionGroup += 1;
            }

            return FilesDbContext.SaveChangesAsync();
        }

        public async Task ContinueVersionAsync(int fileId, int fileVersion)
        {
            using var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false);

            var versionGroup = await Query(FilesDbContext.Files)
                .AsNoTracking()
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .Select(r => r.VersionGroup)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version > fileVersion)
                .Where(r => r.VersionGroup > versionGroup);

            foreach (var f in toUpdate)
            {
                f.VersionGroup -= 1;
            }

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await tx.CommitAsync().ConfigureAwait(false);
        }

        public bool UseTrashForRemove(File<int> file)
        {
            return file.RootFolderType != FolderType.TRASH && file.RootFolderType != FolderType.Privacy;
        }

        public string GetUniqFileDirectory(int fileId)
        {
            if (fileId == 0) throw new ArgumentNullException("fileIdObject");
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
                       ? $"{GetUniqFileVersionPath(file.ID, file.Version)}/{fileTitle}"
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
            return ChunkedUploadSessionHolder.CreateUploadSessionAsync(file, contentLength);
        }

        public async Task<File<int>> UploadChunkAsync(ChunkedUploadSession<int> uploadSession, Stream stream, long chunkLength)
        {
            if (!uploadSession.UseChunks)
            {
                using var streamToSave = await ChunkedUploadSessionHolder.UploadSingleChunkAsync(uploadSession, stream, chunkLength).ConfigureAwait(false);
                if (streamToSave != Stream.Null)
                {
                    uploadSession.File = await SaveFileAsync(await GetFileForCommitAsync(uploadSession).ConfigureAwait(false), streamToSave).ConfigureAwait(false);
                }

                return uploadSession.File;
            }

            await ChunkedUploadSessionHolder.UploadChunkAsync(uploadSession, stream, chunkLength);

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                uploadSession.File = await FinalizeUploadSessionAsync(uploadSession).ConfigureAwait(false);
            }

            return uploadSession.File;
        }

        private async Task<File<int>> FinalizeUploadSessionAsync(ChunkedUploadSession<int> uploadSession)
        {
            await ChunkedUploadSessionHolder.FinalizeUploadSessionAsync(uploadSession);

            var file = await GetFileForCommitAsync(uploadSession).ConfigureAwait(false);
            await SaveFileAsync(file, null, uploadSession.CheckQuota).ConfigureAwait(false);
            await ChunkedUploadSessionHolder.MoveAsync(uploadSession, GetUniqFilePath(file));

            return file;
        }

        public Task AbortUploadSessionAsync(ChunkedUploadSession<int> uploadSession)
        {
            return ChunkedUploadSessionHolder.AbortUploadSessionAsync(uploadSession);
        }

        private async Task<File<int>> GetFileForCommitAsync(ChunkedUploadSession<int> uploadSession)
        {
            if (uploadSession.File.ID != default)
            {
                var file = await GetFileAsync(uploadSession.File.ID).ConfigureAwait(false);
                file.Version++;
                file.ContentLength = uploadSession.BytesTotal;
                file.ConvertedType = null;
                file.Comment = FilesCommonResource.CommentUpload;
                file.Encrypted = uploadSession.Encrypted;
                file.ThumbnailStatus = Thumbnail.Waiting;

                return file;
            }
            var result = ServiceProvider.GetService<File<int>>();
            result.FolderID = uploadSession.File.FolderID;
            result.Title = uploadSession.File.Title;
            result.ContentLength = uploadSession.BytesTotal;
            result.Comment = FilesCommonResource.CommentUpload;
            result.Encrypted = uploadSession.Encrypted;

            return result;
        }

        #endregion

        #region Only in TMFileDao

        public Task ReassignFilesAsync(int[] fileIds, Guid newOwnerId)
        {
            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.CurrentVersion)
                .Where(r => fileIds.Contains(r.Id));

            foreach (var f in toUpdate)
            {
                f.CreateBy = newOwnerId;
            }

            return FilesDbContext.SaveChangesAsync();
        }

        public Task<List<File<int>>> GetFilesAsync(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            if (parentIds == null || !parentIds.Any() || filterType == FilterType.FoldersOnly) return Task.FromResult(new List<File<int>>());

            return InternalGetFilesAsync(parentIds, filterType, subjectGroup, subjectID, searchText, searchInContent);
        }

        private async Task<List<File<int>>> InternalGetFilesAsync(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            var q = GetFileQuery(r => r.CurrentVersion)
                .AsNoTracking()
                .Join(FilesDbContext.Tree, a => a.FolderId, t => t.FolderId, (file, tree) => new { file, tree })
                .Where(r => parentIds.Contains(r.tree.ParentId))
                .Select(r => r.file);

            if (!string.IsNullOrEmpty(searchText))
            {
                var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

                if (FactoryIndexer.TrySelectIds(s => func(s), out var searchIds))
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
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
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

            var query = await FromQueryWithShared(q).ToListAsync().ConfigureAwait(false);
            return query.ConvertAll(e => ToFile(e));
        }

        public IAsyncEnumerable<File<int>> SearchAsync(string searchText, bool bunch = false)
        {
            if (FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var ids))
            {
                var query = GetFileQuery(r => r.CurrentVersion && ids.Contains(r.Id)).AsNoTracking();
                return FromQueryWithShared(query).AsAsyncEnumerable().Select(e => ToFile(e))
                    .Where(
                        f =>
                        bunch
                            ? f.RootFolderType == FolderType.BUNCH
                            : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON)
                    ;
            }
            else
            {
                var query = BuildSearch(GetFileQuery(r => r.CurrentVersion).AsNoTracking(), searchText, SearhTypeEnum.Any);
                return FromQueryWithShared(query).AsAsyncEnumerable().Select(e => ToFile(e))
                    .Where(f =>
                           bunch
                                ? f.RootFolderType == FolderType.BUNCH
                                : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON);
            }
        }

        private async Task DeleteFolderAsync(int fileId)
        {
            await GlobalStore.GetStore().DeleteDirectoryAsync(GetUniqFileDirectory(fileId));
        }

        public Task<bool> IsExistOnStorageAsync(File<int> file)
        {
            return GlobalStore.GetStore().IsFileAsync(string.Empty, GetUniqFilePath(file));
        }

        private const string DiffTitle = "diff.zip";

        public Task SaveEditHistoryAsync(File<int> file, string changes, Stream differenceStream)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (string.IsNullOrEmpty(changes)) throw new ArgumentNullException(nameof(changes));
            if (differenceStream == null) throw new ArgumentNullException(nameof(differenceStream));

            return InternalSaveEditHistoryAsync(file, changes, differenceStream);
        }

        private async Task InternalSaveEditHistoryAsync(File<int> file, string changes, Stream differenceStream)
        {
            var toUpdateTask = Query(FilesDbContext.Files)
                .Where(r => r.Id == file.ID)
                .Where(r => r.Version == file.Version)
                .ToListAsync()
                .ConfigureAwait(false);

            changes = changes.Trim();

            foreach (var f in await toUpdateTask)
            {
                f.Changes = changes;
            }

            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            await GlobalStore.GetStore().SaveAsync(string.Empty, GetUniqFilePath(file, DiffTitle), differenceStream, DiffTitle);
        }

        public async Task<List<EditHistory>> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, int fileId, int fileVersion = 0)
        {
            var query = Query(FilesDbContext.Files)
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
                            var item = ServiceProvider.GetService<EditHistory>();

                            item.ID = r.Id;
                            item.Version = r.Version;
                            item.VersionGroup = r.VersionGroup;
                            item.ModifiedOn = TenantUtil.DateTimeFromUtc(r.ModifiedOn);
                            item.ModifiedBy = r.ModifiedBy;
                            item.ChangesString = r.Changes;
                            item.Key = documentServiceHelper.GetDocKey(item.ID, item.Version, TenantUtil.DateTimeFromUtc(r.CreateOn));

                            return item;
                        })
                    .ToList();
        }

        public Task<Stream> GetDifferenceStreamAsync(File<int> file)
        {
            return GlobalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file, DiffTitle), 0);
        }

        public Task<bool> ContainChangesAsync(int fileId, int fileVersion)
        {
            return Query(FilesDbContext.Files)
                .AnyAsync(r => r.Id == fileId &&
                          r.Version == fileVersion &&
                          r.Changes != null)
;
        }

        public async Task<IEnumerable<(File<int>, SmallShareRecord)>> GetFeedsAsync(int tenant, DateTime from, DateTime to)
        {
            var q1 = FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.TenantId == tenant)
                .Where(r => r.CurrentVersion)
                .Where(r => r.ModifiedOn >= from && r.ModifiedOn <= to);

            var q2 = FromQuery(q1)
                .Select(r => new DbFileQueryWithSecurity() { DbFileQuery = r, Security = null });

            var q3 = FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.TenantId == tenant)
                .Where(r => r.CurrentVersion);

            var q4Task = FromQuery(q3)
                .Join(FilesDbContext.Security.AsQueryable().DefaultIfEmpty(), r => r.File.Id.ToString(), s => s.EntryId, (f, s) => new DbFileQueryWithSecurity { DbFileQuery = f, Security = s })
                .Where(r => r.Security.TenantId == tenant)
                .Where(r => r.Security.EntryType == FileEntryType.File)
                .Where(r => r.Security.Security == Security.FileShare.Restrict)
                .Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to)
                .ToListAsync();

            var fileWithShare = await q2.Select(e => ToFileWithShare(e)).ToListAsync().ConfigureAwait(false);
            var q4 = await q4Task.ConfigureAwait(false);

            return fileWithShare.Union(q4.Select(ToFileWithShare));
        }

        public async Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime fromTime)
        {
            var q1Task = FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.ModifiedOn > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Any())
                .Select(r => r.Key)
                .ToListAsync()
                .ConfigureAwait(false);

            var q2Task = FilesDbContext.Security
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

        public Task SaveThumbnailAsync(File<int> file, Stream thumbnail)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return InternalSaveThumbnailAsync(file, thumbnail);
        }

        private async Task InternalSaveThumbnailAsync(File<int> file, Stream thumbnail)
        {
            var toUpdate = await FilesDbContext.Files
                .AsQueryable()
                .FirstOrDefaultAsync(r => r.Id == file.ID && r.Version == file.Version && r.TenantId == TenantID)
                .ConfigureAwait(false);

            if (toUpdate != null)
            {
                toUpdate.Thumb = thumbnail != null ? Thumbnail.Created : file.ThumbnailStatus;
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            if (thumbnail == null) return;

            var thumnailName = ThumbnailTitle + "." + Global.ThumbnailExtension;
            await GlobalStore.GetStore().SaveAsync(string.Empty, GetUniqFilePath(file, thumnailName), thumbnail, thumnailName);
        }

        public async Task<Stream> GetThumbnailAsync(File<int> file)
        {
            var thumnailName = ThumbnailTitle + "." + Global.ThumbnailExtension;
            var path = GetUniqFilePath(file, thumnailName);
            var storage = GlobalStore.GetStore();
            var isFile = await storage.IsFileAsync(string.Empty, path).ConfigureAwait(false);
            if (!isFile) throw new FileNotFoundException();
            return await storage.GetReadStreamAsync(string.Empty, path, 0).ConfigureAwait(false);
        }

        #endregion

        private Func<Selector<DbFile>, Selector<DbFile>> GetFuncForSearch(object parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
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
                       var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
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

        protected IQueryable<DbFileQuery> FromQueryWithShared(IQueryable<DbFile> dbFiles)
        {
            var cId = AuthContext.CurrentAccount.ID;
            return from r in dbFiles
                   select new DbFileQuery
                   {
                       File = r,
                       Root = (from f in FilesDbContext.Folders.AsQueryable()
                               where f.Id ==
                               (from t in FilesDbContext.Tree.AsQueryable()
                                where t.FolderId == r.FolderId
                                orderby t.Level descending
                                select t.ParentId
                                ).FirstOrDefault()
                               where f.TenantId == r.TenantId
                               select f
                              ).FirstOrDefault(),
                       Shared = (from f in FilesDbContext.Security.AsQueryable()
                                 where f.EntryType == FileEntryType.File && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId
                                 select f
                                 ).Any(),
                       Linked = (from f in FilesDbContext.FilesLink
                                 where f.TenantId == r.TenantId && f.LinkedId == r.Id.ToString() && f.LinkedFor == cId
                                 select f)
                                 .Any()
                   };
        }

        protected IQueryable<DbFileQuery> FromQuery(IQueryable<DbFile> dbFiles)
        {
            var cId = AuthContext.CurrentAccount.ID;
            return dbFiles
                .Select(r => new DbFileQuery
                {
                    File = r,
                    Root = (from f in FilesDbContext.Folders.AsQueryable()
                            where f.Id ==
                            (from t in FilesDbContext.Tree.AsQueryable()
                             where t.FolderId == r.FolderId
                             orderby t.Level descending
                             select t.ParentId
                             ).FirstOrDefault()
                            where f.TenantId == r.TenantId
                            select f
                              ).FirstOrDefault(),
                    Shared = true,
                    Linked = (from f in FilesDbContext.FilesLink
                              where f.TenantId == r.TenantId && f.LinkedId == r.Id.ToString() && f.LinkedFor == cId
                              select f)
                                 .Any()
                });
        }

        public File<int> ToFile(DbFileQuery r)
        {
            var file = ServiceProvider.GetService<File<int>>();
            if (r == null) return null;
            file.ID = r.File.Id;
            file.Title = r.File.Title;
            file.FolderID = r.File.FolderId;
            file.CreateOn = TenantUtil.DateTimeFromUtc(r.File.CreateOn);
            file.CreateBy = r.File.CreateBy;
            file.Version = r.File.Version;
            file.VersionGroup = r.File.VersionGroup;
            file.ContentLength = r.File.ContentLength;
            file.ModifiedOn = TenantUtil.DateTimeFromUtc(r.File.ModifiedOn);
            file.ModifiedBy = r.File.ModifiedBy;
            file.RootFolderType = r.Root?.FolderType ?? default;
            file.RootFolderCreator = r.Root?.CreateBy ?? default;
            file.RootFolderId = r.Root?.Id ?? default;
            file.Shared = r.Shared;
            file.ConvertedType = r.File.ConvertedType;
            file.Comment = r.File.Comment;
            file.Encrypted = r.File.Encrypted;
            file.Forcesave = r.File.Forcesave;
            file.ThumbnailStatus = r.File.Thumb;
            file.IsFillFormDraft = r.Linked;
            return file;
        }

        public (File<int>, SmallShareRecord) ToFileWithShare(DbFileQueryWithSecurity r)
        {
            var file = ToFile(r.DbFileQuery);
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

        internal protected Task<DbFile> InitDocumentAsync(DbFile dbFile)
        {
            if (!FactoryIndexer.CanIndexByContent(dbFile))
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
            var file = ServiceProvider.GetService<File<int>>();
            file.ID = dbFile.Id;
            file.Title = dbFile.Title;
            file.Version = dbFile.Version;
            file.ContentLength = dbFile.ContentLength;

            if (!await IsExistOnStorageAsync(file).ConfigureAwait(false) || file.ContentLength > Settings.MaxContentLength) return dbFile;

            using var stream = await GetFileStreamAsync(file).ConfigureAwait(false);
            if (stream == null) return dbFile;

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
        public bool Linked { get; set; }
    }

    public class DbFileQueryWithSecurity
    {
        public DbFileQuery DbFileQuery { get; set; }
        public DbFilesSecurity Security { get; set; }
    }
}