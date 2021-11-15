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

using Box.V2.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Box
{
    [Scope]
    internal class BoxFolderDao : BoxDaoBase, IFolderDao<string>
    {
        private CrossDao CrossDao { get; }
        private BoxDaoSelector BoxDaoSelector { get; }
        private IFileDao<int> FileDao { get; }
        private IFolderDao<int> FolderDao { get; }

        public BoxFolderDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            CrossDao crossDao,
            BoxDaoSelector boxDaoSelector,
            IFileDao<int> fileDao,
            IFolderDao<int> folderDao,
            TempPath tempPath
            )
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            BoxDaoSelector = boxDaoSelector;
            FileDao = fileDao;
            FolderDao = folderDao;
        }

        public Folder<string> GetFolder(string folderId)
        {
            return GetFolderAsync(folderId).Result;
        }

        public async Task<Folder<string>> GetFolderAsync(string folderId)
        {
            return ToFolder(await GetBoxFolderAsync(folderId));
        }

        public Folder<string> GetFolder(string title, string parentId)
        {
            return GetFolderAsync(title, parentId).Result;
        }

        public async Task<Folder<string>> GetFolderAsync(string title, string parentId)
        {
            var items = await GetBoxItemsAsync(parentId, true);
            return ToFolder(items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)) as BoxFolder);
        }

        public Folder<string> GetRootFolderByFile(string fileId)
        {
            return GetRootFolderByFileAsync(fileId).Result;
        }

        public async Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
        {
            return await GetRootFolderAsync(fileId);
        }

        public List<Folder<string>> GetFolders(string parentId)
        {
            return GetFoldersAsync(parentId).Result;
        }

        public async Task<List<Folder<string>>> GetFoldersAsync(string parentId)
        {
            var items =  await GetBoxItemsAsync(parentId, true);
            return items.Select(item => ToFolder(item as BoxFolder)).ToList();
        }

        public List<Folder<string>> GetFolders(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            return GetFoldersAsync(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, withSubfolders).Result;
        }

        public async Task<List<Folder<string>>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return new List<Folder<string>>();

            var foldersList = await GetFoldersAsync(parentId);
            var folders = foldersList.AsEnumerable(); //TODO:!!!

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
            return folders.ToList();
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

            var folders = folderIds.ToAsyncEnumerable().SelectAwait(async e => await GetFolderAsync(e));

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
                var boxFolder = await GetBoxFolderAsync(folderId);

                if (boxFolder is ErrorFolder)
                {
                    folderId = null;
                }
                else
                {
                    path.Add(ToFolder(boxFolder));
                    folderId = GetParentFolderId(boxFolder);
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
                return await RenameFolderAsync(folder, folder.Title);
            }

            if (folder.FolderID != null)
            {
                var boxFolderId = MakeBoxId(folder.FolderID);

                folder.Title = await GetAvailableTitleAsync(folder.Title, boxFolderId, IsExistAsync);

                var boxFolder = await ProviderInfo.Storage.CreateFolderAsync(folder.Title, boxFolderId);

                await ProviderInfo.CacheResetAsync(boxFolder);
                var parentFolderId = GetParentFolderId(boxFolder);
                if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId);

                return MakeId(boxFolder);
            }
            return null;
        }

        public bool IsExist(string title, string folderId)
        {
            return GetBoxItems(folderId, true)
                .Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> IsExistAsync(string title, string folderId)
        {
            var items = await GetBoxItemsAsync(folderId, true);
            return items.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public void DeleteFolder(string folderId)
        {
            DeleteFolderAsync(folderId).Wait();
        }

        public async Task DeleteFolderAsync(string folderId)
        {
            var boxFolder = await GetBoxFolderAsync(folderId);
            var id = MakeId(boxFolder);

            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var hashIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                   .Where(r => r.Id.StartsWith(id))
                   .Select(r => r.HashId)
                   .ToListAsync();

                var link = await Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId))
                    .ToListAsync();

                FilesDbContext.TagLink.RemoveRange(link);
                await FilesDbContext.SaveChangesAsync();

                var tagsToRemove = Query(FilesDbContext.Tag)
                    .Where(r => !Query(FilesDbContext.TagLink).Where(a => a.TagId == r.Id).Any());

                FilesDbContext.Tag.RemoveRange(tagsToRemove);

                var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.Security.RemoveRange(securityToDelete);
                await FilesDbContext.SaveChangesAsync();

                var mappingToDelete = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => hashIDs.Any(h => h == r.HashId));

                FilesDbContext.ThirdpartyIdMapping.RemoveRange(mappingToDelete);
                await FilesDbContext .SaveChangesAsync();

                tx.Commit();
            }

            if (!(boxFolder is ErrorFolder))
            {
                await ProviderInfo.Storage.DeleteItemAsync(boxFolder);
            }

            await ProviderInfo.CacheResetAsync(boxFolder.Id, true);
            var parentFolderId = GetParentFolderId(boxFolder);
            if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId);
        }

        public string MoveFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var boxFolder = await GetBoxFolderAsync(folderId);
            if (boxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var toBoxFolder = await GetBoxFolderAsync(toFolderId);
            if (toBoxFolder is ErrorFolder errorFolder1) throw new Exception(errorFolder1.Error);

            var fromFolderId = GetParentFolderId(boxFolder);

            var newTitle = await GetAvailableTitleAsync(boxFolder.Name, toBoxFolder.Id, IsExistAsync);
            boxFolder = ProviderInfo.Storage.MoveFolder(boxFolder.Id, newTitle, toBoxFolder.Id);

            await ProviderInfo.CacheResetAsync(boxFolder.Id, false);
            await ProviderInfo.CacheResetAsync(fromFolderId);
            await ProviderInfo.CacheResetAsync(toBoxFolder.Id);

            return MakeId(boxFolder.Id);
        }

        public TTo MoveFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<TTo> MoveFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
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

        public int MoveFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return MoveFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                    folderId, this, BoxDaoSelector.GetFileDao(folderId), BoxDaoSelector.ConvertId,
                    toFolderId, FolderDao, FileDao, r => r,
                    true, cancellationToken);

            return moved.ID;
        }

        public Folder<TTo> CopyFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<TTo>> CopyFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
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

        public Folder<string> CopyFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var boxFolder = await GetBoxFolderAsync(folderId);
            if (boxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var toBoxFolder = await GetBoxFolderAsync(toFolderId);
            if (toBoxFolder is ErrorFolder errorFolder1) throw new Exception(errorFolder1.Error);

            var newTitle = await GetAvailableTitleAsync(boxFolder.Name, toBoxFolder.Id, IsExistAsync);
            var newBoxFolder = await ProviderInfo.Storage.CopyFolderAsync(boxFolder.Id, newTitle, toBoxFolder.Id);

            await ProviderInfo.CacheResetAsync(newBoxFolder);
            await ProviderInfo.CacheResetAsync(newBoxFolder.Id, false);
            await ProviderInfo.CacheResetAsync(toBoxFolder.Id);

            return ToFolder(newBoxFolder);
        }

        public Folder<int> CopyFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return CopyFolderAsync(folderId, toFolderId, cancellationToken).Result;
        }

        public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, BoxDaoSelector.GetFileDao(folderId), BoxDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                false, cancellationToken);

            return moved;
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

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, string to)
        {
            return new Dictionary<string, string>();
        }

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, int to)
        {
            return new Dictionary<string, string>();
        }

        public string RenameFolder(Folder<string> folder, string newTitle)
        {
            return RenameFolderAsync(folder, newTitle).Result;
        }

        public async Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
        {
            var boxFolder = await GetBoxFolderAsync(folder.ID);
            var parentFolderId = GetParentFolderId(boxFolder);

            if (IsRoot(boxFolder))
            {
                //It's root folder
                await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle);
                //rename provider customer title
            }
            else
            {
                newTitle = await GetAvailableTitleAsync(newTitle, parentFolderId, IsExistAsync);

                //rename folder
                boxFolder = await ProviderInfo.Storage.RenameFolderAsync(boxFolder.Id, newTitle);
            }

            await ProviderInfo.CacheResetAsync(boxFolder);
            if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId);

            return MakeId(boxFolder.Id);
        }

        public int GetItemsCount(string folderId)
        {
            return GetItemsCountAsync(folderId).Result;
        }

        public Task<int> GetItemsCountAsync(string folderId)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty(string folderId)
        {
            return IsEmptyAsync(folderId).Result;
        }

        public async Task<bool> IsEmptyAsync(string folderId)
        {
            var boxFolderId = MakeBoxId(folderId);
            //note: without cache
            var items = await ProviderInfo.Storage.GetItemsAsync(boxFolderId, 1);
            return items.Count == 0;
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
            return false;
        }

        public long GetMaxUploadSize(string folderId, bool chunkedUpload)
        {
            return GetMaxUploadSizeAsync(folderId, chunkedUpload).Result;
        }

        public async Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload)
        {
            var storageMaxUploadSize = await ProviderInfo.Storage.GetMaxUploadSizeAsync();

            return chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, SetupInfo.AvailableFileSize);
        }
    }
}