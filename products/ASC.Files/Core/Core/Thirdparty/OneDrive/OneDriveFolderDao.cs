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

namespace ASC.Files.Thirdparty.OneDrive
{
    [Scope]
    internal class OneDriveFolderDao : OneDriveDaoBase, IFolderDao<string>
    {
        private CrossDao CrossDao { get; }
        private OneDriveDaoSelector OneDriveDaoSelector { get; }
        private IFileDao<int> FileDao { get; }
        private IFolderDao<int> FolderDao { get; }

        public OneDriveFolderDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            CrossDao crossDao,
            OneDriveDaoSelector oneDriveDaoSelector,
            IFileDao<int> fileDao,
            IFolderDao<int> folderDao,
            TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            OneDriveDaoSelector = oneDriveDaoSelector;
            FileDao = fileDao;
            FolderDao = folderDao;
        }

        public Folder<string> GetFolder(string folderId)
        {
            return GetFolderAsync(folderId).Result;
        }

        public async Task<Folder<string>> GetFolderAsync(string folderId)
        {
            return ToFolder(await GetOneDriveItemAsync(folderId).ConfigureAwait(false));
        }

        public Folder<string> GetFolder(string title, string parentId)
        {
            return GetFolderAsync(title, parentId).Result;
        }

        public async Task<Folder<string>> GetFolderAsync(string title, string parentId)
        {
            var items = await GetOneDriveItemsAsync(parentId, true).ConfigureAwait(false);
            return ToFolder(items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase) && item.Folder != null));
        }

        public Folder<string> GetRootFolderByFile(string fileId)
        {
            return GetRootFolderByFileAsync(fileId).Result;
        }

        public async Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
        {
            return await GetRootFolderAsync(fileId).ConfigureAwait(false);
        }

        public List<Folder<string>> GetFolders(string parentId)
        {
            return GetFoldersAsync(parentId).ToListAsync().Result;
        }

        public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
        {
            var items = await GetOneDriveItemsAsync(parentId, true).ConfigureAwait(false);

            foreach (var i in items)
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
            //Filter
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
                var onedriveFolder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);

                if (onedriveFolder is ErrorItem)
                {
                    folderId = null;
                }
                else
                {
                    path.Add(ToFolder(onedriveFolder));
                    folderId = GetParentFolderId(onedriveFolder);
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
                var onedriveFolderId = MakeOneDriveId(folder.FolderID);

                folder.Title = await GetAvailableTitleAsync(folder.Title, onedriveFolderId, IsExistAsync).ConfigureAwait(false);

                var onedriveFolder = await ProviderInfo.Storage.CreateFolderAsync(folder.Title, onedriveFolderId).ConfigureAwait(false);

                await ProviderInfo.CacheResetAsync(onedriveFolder.Id).ConfigureAwait(false);
                var parentFolderId = GetParentFolderId(onedriveFolder);
                if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId).ConfigureAwait(false);

                return MakeId(onedriveFolder);
            }
            return null;
        }

        public bool IsExist(string title, string folderId)
        {
            return IsExistAsync(title, folderId).Result;
        }

        public async Task<bool> IsExistAsync(string title, string folderId)
        {
            var items = await GetOneDriveItemsAsync(folderId, true).ConfigureAwait(false);
            return items.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public void DeleteFolder(string folderId)
        {
            DeleteFolderAsync(folderId).Wait();
        }

        public async Task DeleteFolderAsync(string folderId)
        {
            var onedriveFolder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);
            var id = MakeId(onedriveFolder);

            using (var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var hashIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                   .Where(r => r.Id.StartsWith(id))
                   .Select(r => r.HashId)
                   .ToListAsync()
                   .ConfigureAwait(false);

                var link = await Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId))
                    .ToListAsync().ConfigureAwait(false);

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

            if (!(onedriveFolder is ErrorItem))
                await ProviderInfo.Storage.DeleteItemAsync(onedriveFolder).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(onedriveFolder.Id).ConfigureAwait(false);
            var parentFolderId = GetParentFolderId(onedriveFolder);
            if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId).ConfigureAwait(false);
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

        public int MoveFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, OneDriveDaoSelector.GetFileDao(folderId), OneDriveDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                true, cancellationToken)
                .ConfigureAwait(false);

            return moved.ID;
        }

        public string MoveFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var onedriveFolder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);
            if (onedriveFolder is ErrorItem errorItem) throw new Exception(errorItem.Error);

            var toOneDriveFolder = await GetOneDriveItemAsync(toFolderId).ConfigureAwait(false);
            if (toOneDriveFolder is ErrorItem errorItem1) throw new Exception(errorItem1.Error);

            var fromFolderId = GetParentFolderId(onedriveFolder);

            var newTitle = await GetAvailableTitleAsync(onedriveFolder.Name, toOneDriveFolder.Id, IsExistAsync).ConfigureAwait(false);
            onedriveFolder = await ProviderInfo.Storage.MoveItemAsync(onedriveFolder.Id, newTitle, toOneDriveFolder.Id).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(onedriveFolder.Id).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(fromFolderId).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toOneDriveFolder.Id).ConfigureAwait(false);

            return MakeId(onedriveFolder.Id);
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
                folderId, this, OneDriveDaoSelector.GetFileDao(folderId), OneDriveDaoSelector.ConvertId,
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
            var onedriveFolder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);
            if (onedriveFolder is ErrorItem errorItem) throw new Exception(errorItem.Error);

            var toOneDriveFolder = await GetOneDriveItemAsync(toFolderId).ConfigureAwait(false);
            if (toOneDriveFolder is ErrorItem errorItem1) throw new Exception(errorItem1.Error);

            var newTitle = await GetAvailableTitleAsync(onedriveFolder.Name, toOneDriveFolder.Id, IsExistAsync).ConfigureAwait(false);
            var newOneDriveFolder = await ProviderInfo.Storage.CopyItemAsync(onedriveFolder.Id, newTitle, toOneDriveFolder.Id).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(newOneDriveFolder.Id).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toOneDriveFolder.Id).ConfigureAwait(false);

            return ToFolder(newOneDriveFolder);
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
            var onedriveFolder = await GetOneDriveItemAsync(folder.ID).ConfigureAwait(false);
            var parentFolderId = GetParentFolderId(onedriveFolder);

            if (IsRoot(onedriveFolder))
            {
                //It's root folder
                await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle).ConfigureAwait(false);
                //rename provider customer title
            }
            else
            {
                newTitle = await GetAvailableTitleAsync(newTitle, parentFolderId, IsExistAsync).ConfigureAwait(false);

                //rename folder
                onedriveFolder = await ProviderInfo.Storage.RenameItemAsync(onedriveFolder.Id, newTitle).ConfigureAwait(false);
            }

            await ProviderInfo.CacheResetAsync(onedriveFolder.Id).ConfigureAwait(false);
            if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId).ConfigureAwait(false);

            return MakeId(onedriveFolder.Id);
        }

        public int GetItemsCount(string folderId)
        {
            return GetItemsCountAsync(folderId).Result;
        }

        public async Task<int> GetItemsCountAsync(string folderId)
        {
            var onedriveFolder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);
            return (onedriveFolder == null
                    || onedriveFolder.Folder == null
                    || !onedriveFolder.Folder.ChildCount.HasValue)
                       ? 0
                       : onedriveFolder.Folder.ChildCount.Value;
        }

        public bool IsEmpty(string folderId)
        {
            return IsEmptyAsync(folderId).Result;
        }

        public async Task<bool> IsEmptyAsync(string folderId)
        {
            var onedriveFolder = await GetOneDriveItemAsync(folderId).ConfigureAwait(false);
            return onedriveFolder == null
                   || onedriveFolder.Folder == null
                   || onedriveFolder.Folder.ChildCount == 0;
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
            return false;
        }

        public bool UseRecursiveOperation(string folderId, int toRootFolderId)
        {
            return false;
        }

        public bool CanCalculateSubitems(string entryId)
        {
            return true;
        }

        public long GetMaxUploadSize(string folderId, bool chunkedUpload)
        {
            return GetMaxUploadSizeAsync(folderId, chunkedUpload).Result;
        }

        public Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload)
        {
            var storageMaxUploadSize = ProviderInfo.Storage.MaxChunkedUploadFileSize;

            return Task.FromResult(chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, SetupInfo.AvailableFileSize));
        }
    }
}