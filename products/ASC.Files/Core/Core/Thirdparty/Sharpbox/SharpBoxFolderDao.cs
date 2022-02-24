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
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Thirdparty;
using ASC.Web.Core.Files;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Sharpbox
{
    [Scope]
    internal class SharpBoxFolderDao : SharpBoxDaoBase, IFolderDao<string>
    {
        private CrossDao CrossDao { get; }
        private SharpBoxDaoSelector SharpBoxDaoSelector { get; }
        private IFileDao<int> FileDao { get; }
        private IFolderDao<int> FolderDao { get; }

        public SharpBoxFolderDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            CrossDao crossDao,
            SharpBoxDaoSelector sharpBoxDaoSelector,
            IFileDao<int> fileDao,
            IFolderDao<int> folderDao,
            TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            SharpBoxDaoSelector = sharpBoxDaoSelector;
            FileDao = fileDao;
            FolderDao = folderDao;
        }

        public Task<Folder<string>> GetFolderAsync(string folderId)
        {
            return Task.FromResult(ToFolder(GetFolderById(folderId)));
        }

        public Task<Folder<string>> GetFolderAsync(string title, string parentId)
        {
            var parentFolder = ProviderInfo.Storage.GetFolder(MakePath(parentId));
            return Task.FromResult(ToFolder(parentFolder.OfType<ICloudDirectoryEntry>().FirstOrDefault(x => x.Name.Equals(title, StringComparison.OrdinalIgnoreCase))));
        }

        public Task<Folder<string>> GetRootFolderAsync(string folderId)
        {
            return Task.FromResult(ToFolder(RootFolder()));
        }

        public Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
        {
            return Task.FromResult(ToFolder(RootFolder()));
        }

        public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
        {
            var parentFolder = ProviderInfo.Storage.GetFolder(MakePath(parentId));
            return parentFolder.OfType<ICloudDirectoryEntry>().Select(ToFolder).ToAsyncEnumerable();
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
        public Task<List<Folder<string>>> GetParentFoldersAsync(string folderId)
        {
            var path = new List<Folder<string>>();
            var folder = GetFolderById(folderId);
            if (folder != null)
            {
                do
                {
                    path.Add(ToFolder(folder));
                } while ((folder = folder.Parent) != null);
            }
            path.Reverse();
            return Task.FromResult(path);
        }


        public async Task<string> SaveFolderAsync(Folder<string> folder)
        {
            try
            {
                if (folder.ID != null)
                {
                    //Create with id
                    var savedfolder = ProviderInfo.Storage.CreateFolder(MakePath(folder.ID));
                    return MakeId(savedfolder);
                }
                if (folder.FolderID != null)
                {
                    var parentFolder = GetFolderById(folder.FolderID);

                    folder.Title = await GetAvailableTitleAsync(folder.Title, parentFolder, IsExistAsync).ConfigureAwait(false);

                    var newFolder = ProviderInfo.Storage.CreateFolder(folder.Title, parentFolder);
                    return MakeId(newFolder);
                }
            }
            catch (SharpBoxException e)
            {
                var webException = (WebException)e.InnerException;
                if (webException != null)
                {
                    var response = (HttpWebResponse)webException.Response;
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
                        }
                    }
                    throw;
                }
            }
            return null;
        }

        public async Task DeleteFolderAsync(string folderId)
        {
            var folder = GetFolderById(folderId);
            var id = MakeId(folder);

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

                var tagsToRemove = from ft in FilesDbContext.Tag
                                   join ftl in FilesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                                   where ftl == null
                                   select ft;

                FilesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

                var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var mappingToDelete = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => hashIDs.Any(h => h == r.HashId));

                FilesDbContext.ThirdpartyIdMapping.RemoveRange(await mappingToDelete.ToListAsync());
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false);
            }

            if (!(folder is ErrorEntry))
                ProviderInfo.Storage.DeleteFileSystemEntry(folder);
        }

        public Task<bool> IsExistAsync(string title, ICloudDirectoryEntry folder)
        {
            try
            {
                return Task.FromResult(ProviderInfo.Storage.GetFileSystemObject(title, folder) != null);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {

            }
            return Task.FromResult(false);
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

        public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, SharpBoxDaoSelector.GetFileDao(folderId), SharpBoxDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                true, cancellationToken)
                .ConfigureAwait(false);

            return moved.ID;
        }

        public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var entry = GetFolderById(folderId);
            var folder = GetFolderById(toFolderId);

            var oldFolderId = MakeId(entry);

            if (!ProviderInfo.Storage.MoveFileSystemEntry(entry, folder))
                throw new Exception("Error while moving");

            var newFolderId = MakeId(entry);

            await UpdatePathInDBAsync(oldFolderId, newFolderId).ConfigureAwait(false);

            return newFolderId;
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

        public async Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = await CrossDao.PerformCrossDaoFolderCopyAsync(
                folderId, this, SharpBoxDaoSelector.GetFileDao(folderId), SharpBoxDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                false, cancellationToken)
                .ConfigureAwait(false);

            return moved;
        }

        public Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var folder = GetFolderById(folderId);
            if (!ProviderInfo.Storage.CopyFileSystemEntry(MakePath(folderId), MakePath(toFolderId)))
                throw new Exception("Error while copying");

            return Task.FromResult(ToFolder(GetFolderById(toFolderId).OfType<ICloudDirectoryEntry>().FirstOrDefault(x => x.Name == folder.Name)));
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync<TTo>(string[] folderIds, TTo to)
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

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, string to)
        {
            return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, int to)
        {
            return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
        }

        public async Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
        {
            var entry = GetFolderById(folder.ID);

            var oldId = MakeId(entry);
            var newId = oldId;

            if ("/".Equals(MakePath(folder.ID)))
            {
                //It's root folder
                await DaoSelector.RenameProviderAsync(ProviderInfo, newTitle).ConfigureAwait(false);
                //rename provider customer title
            }
            else
            {
                var parentFolder = GetFolderById(folder.FolderID);
                newTitle = await GetAvailableTitleAsync(newTitle, parentFolder, IsExistAsync).ConfigureAwait(false);

                //rename folder
                if (ProviderInfo.Storage.RenameFileSystemEntry(entry, newTitle))
                {
                    //Folder data must be already updated by provider
                    //We can't search google folders by title because root can have multiple folders with the same name
                    //var newFolder = SharpBoxProviderInfo.Storage.GetFileSystemObject(newTitle, folder.Parent);
                    newId = MakeId(entry);
                }
            }

            await UpdatePathInDBAsync(oldId, newId).ConfigureAwait(false);

            return newId;
        }

        public Task<int> GetItemsCountAsync(string folderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmptyAsync(string folderId)
        {
            return Task.FromResult(GetFolderById(folderId).Count == 0);
        }

        public bool UseTrashForRemove(Folder<string> folder)
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

        public bool UseRecursiveOperation(string folderId, string toRootFolderId)
        {
            return false;
        }

        public bool CanCalculateSubitems(string entryId)
        {
            return false;
        }

        public Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload = false)
        {
            var storageMaxUploadSize =
                chunkedUpload
                    ? ProviderInfo.Storage.CurrentConfiguration.Limits.MaxChunkedUploadFileSize
                    : ProviderInfo.Storage.CurrentConfiguration.Limits.MaxUploadFileSize;

            if (storageMaxUploadSize == -1)
                storageMaxUploadSize = long.MaxValue;

            return Task.FromResult(chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, SetupInfo.AvailableFileSize));
        }
    }
}