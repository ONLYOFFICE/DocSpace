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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Thirdparty;
using ASC.Web.Core.Files;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.GoogleDrive
{
    [Scope]
    internal class GoogleDriveFolderDao : GoogleDriveDaoBase, IFolderDao<string>
    {
        private CrossDao CrossDao { get; }
        private GoogleDriveDaoSelector GoogleDriveDaoSelector { get; }
        private IFileDao<int> FileDao { get; }
        private IFolderDao<int> FolderDao { get; }

        public GoogleDriveFolderDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            CrossDao crossDao,
            GoogleDriveDaoSelector googleDriveDaoSelector,
            IFileDao<int> fileDao,
            IFolderDao<int> folderDao,
            TempPath tempPath
            ) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            GoogleDriveDaoSelector = googleDriveDaoSelector;
            FileDao = fileDao;
            FolderDao = folderDao;
        }

        public Folder<string> GetFolder(string folderId)
        {
            return GetFolderAsync(folderId).Result;
        }

        public async Task<Folder<string>> GetFolderAsync(string folderId)
        {
            return ToFolder(await GetDriveEntryAsync(folderId).ConfigureAwait(false));
        }

        public Folder<string> GetFolder(string title, string parentId)
        {
            return GetFolderAsync(title, parentId).Result;
        }

        public async Task<Folder<string>> GetFolderAsync(string title, string parentId)
        {
            var entries = await GetDriveEntriesAsync(parentId, true).ConfigureAwait(false);
            return ToFolder(entries.FirstOrDefault(folder => folder.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Folder<string> GetRootFolderByFile(string fileId)
        {
            return GetRootFolderByFileAsync(fileId).Result;
        }

        public async Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
        {
            return await GetRootFolderAsync("").ConfigureAwait(false);
        }

        public List<Folder<string>> GetFolders(string parentId)
        {
            return GetFoldersAsync(parentId).ToListAsync().Result;
        }

        public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
        {
            var entries = await GetDriveEntriesAsync(parentId, true).ConfigureAwait(false);

            foreach (var i in entries)
            {
                yield return ToFolder(i);
            }
        }

        public List<Folder<string>> GetFolders(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            return GetFoldersAsync(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, withSubfolders).ToListAsync().Result;
        }

        public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return AsyncEnumerable.Empty<Folder<string>>();

            var folders = GetFoldersAsync(parentId); //TODO:!!!

            if (subjectID != Guid.Empty)
            {
                folders = folders.Where(x => subjectGroup
                                                 ? UserManager.IsUserInGroup(x.CreateBy, subjectID)
                                                 : x.CreateBy == subjectID);
            }

            if (!string.IsNullOrEmpty(searchText))
                folders = folders.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            folders = orderBy.SortedBy switch
            {
                SortedByType.Author => orderBy.IsAsc ? folders.OrderBy(x => x.CreateBy) : folders.OrderByDescending(x => x.CreateBy),
                SortedByType.AZ => orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title),
                SortedByType.DateAndTime => orderBy.IsAsc ? folders.OrderBy(x => x.ModifiedOn) : folders.OrderByDescending(x => x.ModifiedOn),
                SortedByType.DateAndTimeCreation => orderBy.IsAsc ? folders.OrderBy(x => x.CreateOn) : folders.OrderByDescending(x => x.CreateOn),
                _ => orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title),
            };

            return folders;
        }

        public List<Folder<string>> GetFolders(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
        {
            return GetFoldersAsync(folderIds, filterType, subjectGroup, subjectID, searchText, searchSubfolders, checkShare).ToListAsync().Result;
        }

        public IAsyncEnumerable<Folder<string>> GetFoldersAsync(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
        {
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return AsyncEnumerable.Empty<Folder<string>>();

            var folders = folderIds.ToAsyncEnumerable().SelectAwait(async e => await GetFolderAsync(e).ConfigureAwait(false));

            if (subjectID.HasValue && subjectID != Guid.Empty)
            {
                folders = folders.Where(x => subjectGroup
                                                 ? UserManager.IsUserInGroup(x.CreateBy, subjectID.Value)
                                                 : x.CreateBy == subjectID);
            }

            if (!string.IsNullOrEmpty(searchText))
                folders = folders.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            return folders;
        }

        public List<Folder<string>> GetParentFolders(string folderId)
        {
            return GetParentFoldersAsync(folderId).Result;
        }

        public async Task<List<Folder<string>>> GetParentFoldersAsync(string folderId)
        {
            var path = new List<Folder<string>>();

            while (folderId != null)
            {
                var driveFolder = await GetDriveEntryAsync(folderId).ConfigureAwait(false);

                if (driveFolder is ErrorDriveEntry)
                {
                    folderId = null;
                }
                else
                {
                    path.Add(ToFolder(driveFolder));
                    folderId = GetParentDriveId(driveFolder);
                }
            }

            path.Reverse();
            return path;
        }

        public string SaveFolder(Folder<string> folder)
        {
            return SaveFolderAsync(folder).Result;
        }

        public async Task<string> SaveFolderAsync(Folder<string> folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            if (folder.ID != null)
            {
                return await RenameFolderAsync(folder, folder.Title).ConfigureAwait(false);
            }

            if (folder.FolderID != null)
            {
                var driveFolderId = MakeDriveId(folder.FolderID);

                var driveFolder = await ProviderInfo.Storage.InsertEntryAsync(null, folder.Title, driveFolderId, true).ConfigureAwait(false);

                await ProviderInfo.CacheResetAsync(driveFolder).ConfigureAwait(false);
                var parentDriveId = GetParentDriveId(driveFolder);
                if (parentDriveId != null) await ProviderInfo.CacheResetAsync(parentDriveId, true).ConfigureAwait(false);

                return MakeId(driveFolder);
            }
            return null;
        }

        public void DeleteFolder(string folderId)
        {
            DeleteFolderAsync(folderId).Wait();
        }

        public async Task DeleteFolderAsync(string folderId)
        {
            var driveFolder = await GetDriveEntryAsync(folderId).ConfigureAwait(false);
            var id = MakeId(driveFolder);

            using (var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var hashIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                   .Where(r => r.Id.StartsWith(id))
                   .Select(r => r.HashId)
                   .ToListAsync()
                   .ConfigureAwait(false);

                var link = await Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId))
                    .ToListAsync()
                    .ConfigureAwait(false);

                FilesDbContext.TagLink.RemoveRange(link);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var tagsToRemove = Query(FilesDbContext.Tag)
                    .Where(r => !Query(FilesDbContext.TagLink).Where(a => a.TagId == r.Id).Any());

                FilesDbContext.Tag.RemoveRange(tagsToRemove);

                var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.Security.RemoveRange(securityToDelete);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var mappingToDelete = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => hashIDs.Any(h => h == r.HashId));

                FilesDbContext.ThirdpartyIdMapping.RemoveRange(mappingToDelete);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false);
            }

            if (!(driveFolder is ErrorDriveEntry))
                await ProviderInfo.Storage.DeleteEntryAsync(driveFolder.Id).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(driveFolder.Id).ConfigureAwait(false);
            var parentDriveId = GetParentDriveId(driveFolder);
            if (parentDriveId != null) await ProviderInfo.CacheResetAsync(parentDriveId, true).ConfigureAwait(false);
        }


        public int MoveFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, GoogleDriveDaoSelector.GetFileDao(folderId), GoogleDriveDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                true, cancellationToken)
                .ConfigureAwait(false);

            return moved.ID;
        }

        public TTo MoveFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<TTo> MoveFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
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

        public string MoveFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var driveFolder = await GetDriveEntryAsync(folderId).ConfigureAwait(false);
            if (driveFolder is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var toDriveFolder = await GetDriveEntryAsync(toFolderId).ConfigureAwait(false);
            if (toDriveFolder is ErrorDriveEntry errorDriveEntry1) throw new Exception(errorDriveEntry1.Error);

            var fromFolderDriveId = GetParentDriveId(driveFolder);

            driveFolder = await ProviderInfo.Storage.InsertEntryIntoFolderAsync(driveFolder, toDriveFolder.Id).ConfigureAwait(false);
            if (fromFolderDriveId != null)
            {
                await ProviderInfo.Storage.RemoveEntryFromFolderAsync(driveFolder, fromFolderDriveId).ConfigureAwait(false);
            }

            await ProviderInfo.CacheResetAsync(driveFolder.Id).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(fromFolderDriveId, true).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toDriveFolder.Id, true).ConfigureAwait(false);

            return MakeId(driveFolder.Id);
        }

        public Folder<TTo> CopyFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<TTo>> CopyFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
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

        public Folder<int> CopyFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, GoogleDriveDaoSelector.GetFileDao(folderId), GoogleDriveDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                false, cancellationToken)
                .ConfigureAwait(false);

            return moved;
        }

        public Folder<string> CopyFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var driveFolder = await GetDriveEntryAsync(folderId).ConfigureAwait(false);
            if (driveFolder is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var toDriveFolder = await GetDriveEntryAsync(toFolderId).ConfigureAwait(false);
            if (toDriveFolder is ErrorDriveEntry errorDriveEntry1) throw new Exception(errorDriveEntry1.Error);

            var newDriveFolder = await ProviderInfo.Storage.InsertEntryAsync(null, driveFolder.Name, toDriveFolder.Id, true).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(newDriveFolder).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toDriveFolder.Id, true).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toDriveFolder.Id).ConfigureAwait(false);

            return ToFolder(newDriveFolder);
        }

        public IDictionary<string, string> CanMoveOrCopy<TTo>(string[] folderIds, TTo to)
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

        public async Task<IDictionary<string, string>> CanMoveOrCopyAsync<TTo>(string[] folderIds, TTo to)
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

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, string to)
        {
            return new Dictionary<string, string>();
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, string to)
        {
            return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
        }

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, int to)
        {
            return new Dictionary<string, string>();
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, int to)
        {
            return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
        }

        public string RenameFolder(Folder<string> folder, string newTitle)
        {
            return RenameFolderAsync(folder, newTitle).Result;
        }

        public async Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
        {
            var driveFolder = await GetDriveEntryAsync(folder.ID).ConfigureAwait(false);

            if (IsRoot(driveFolder))
            {
                //It's root folder
                await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle).ConfigureAwait(false);
                //rename provider customer title
            }
            else
            {
                //rename folder
                driveFolder.Name = newTitle;
                driveFolder = await ProviderInfo.Storage.RenameEntryAsync(driveFolder.Id, driveFolder.Name).ConfigureAwait(false);
            }

            await ProviderInfo.CacheResetAsync(driveFolder).ConfigureAwait(false);
            var parentDriveId = GetParentDriveId(driveFolder);
            if (parentDriveId != null) await ProviderInfo.CacheResetAsync(parentDriveId, true).ConfigureAwait(false);

            return MakeId(driveFolder.Id);
        }

        public int GetItemsCount(string folderId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetItemsCountAsync(string folderId)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty(string folderId)
        {
            var driveId = MakeDriveId(folderId);
            //note: without cache
            return ProviderInfo.Storage.GetEntries(driveId).Count == 0;
        }

        public async Task<bool> IsEmptyAsync(string folderId)
        {
            var driveId = MakeDriveId(folderId);
            //note: without cache
            var entries = await ProviderInfo.Storage.GetEntriesAsync(driveId).ConfigureAwait(false);
            return entries.Count == 0;
        }

        public bool UseTrashForRemove(Folder<string> folder)
        {
            return false;
        }

        public bool UseRecursiveOperation(string folderId, string toRootFolderId)
        {
            return false;
        }

        public bool UseRecursiveOperation<TTo>(string folderId, TTo toRootFolderId)
        {
            return true;
        }

        public bool UseRecursiveOperation(string folderId, int toRootFolderId)
        {
            return true;
        }

        public bool CanCalculateSubitems(string entryId)
        {
            return false;
        }

        public long GetMaxUploadSize(string folderId, bool chunkedUpload)
        {
            return GetMaxUploadSizeAsync(folderId, chunkedUpload).Result;
        }

        public async Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload)
        {
            var storageMaxUploadSize = await ProviderInfo.Storage.GetMaxUploadSizeAsync().ConfigureAwait(false);

            return chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, SetupInfo.AvailableFileSize);
        }
    }
}