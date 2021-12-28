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
            return ToFolder(GetDriveEntry(folderId));
        }

        public Folder<string> GetFolder(string title, string parentId)
        {
            return ToFolder(GetDriveEntries(parentId, true)
                                .FirstOrDefault(folder => folder.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Folder<string> GetRootFolderByFile(string fileId)
        {
            return GetRootFolder("");
        }

        public List<Folder<string>> GetFolders(string parentId)
        {
            return GetDriveEntries(parentId, true).Select(ToFolder).ToList();
        }

        public List<Folder<string>> GetFolders(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return new List<Folder<string>>();

            var folders = GetFolders(parentId).AsEnumerable(); //TODO:!!!

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
            if (filterType == FilterType.FilesOnly || filterType == FilterType.ByExtension
                || filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly
                || filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly
                || filterType == FilterType.ArchiveOnly || filterType == FilterType.MediaOnly)
                return new List<Folder<string>>();

            var folders = folderIds.Select(GetFolder);

            if (subjectID.HasValue && subjectID != Guid.Empty)
            {
                folders = folders.Where(x => subjectGroup
                                                 ? UserManager.IsUserInGroup(x.CreateBy, subjectID.Value)
                                                 : x.CreateBy == subjectID);
            }

            if (!string.IsNullOrEmpty(searchText))
                folders = folders.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            return folders.ToList();
        }

        public List<Folder<string>> GetParentFolders(string folderId)
        {
            var path = new List<Folder<string>>();

            while (folderId != null)
            {
                var driveFolder = GetDriveEntry(folderId);

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
            if (folder == null) throw new ArgumentNullException("folder");
            if (folder.ID != null)
            {
                return RenameFolder(folder, folder.Title);
            }

            if (folder.FolderID != null)
            {
                var driveFolderId = MakeDriveId(folder.FolderID);

                var driveFolder = ProviderInfo.Storage.InsertEntry(null, folder.Title, driveFolderId, true);

                ProviderInfo.CacheReset(driveFolder);
                var parentDriveId = GetParentDriveId(driveFolder);
                if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, true);

                return MakeId(driveFolder);
            }
            return null;
        }

        public void DeleteFolder(string folderId)
        {
            var driveFolder = GetDriveEntry(folderId);
            var id = MakeId(driveFolder);

            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var hashIDs = Query(FilesDbContext.ThirdpartyIdMapping)
                   .Where(r => r.Id.StartsWith(id))
                   .Select(r => r.HashId)
                   .ToList();

                var link = Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId))
                    .ToList();

                FilesDbContext.TagLink.RemoveRange(link);
                FilesDbContext.SaveChanges();

                var tagsToRemove = from ft in FilesDbContext.Tag
                                   join ftl in FilesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                                   where ftl == null
                                   select ft;

                FilesDbContext.Tag.RemoveRange(tagsToRemove.ToList());

                var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.Security.RemoveRange(securityToDelete);
                FilesDbContext.SaveChanges();

                var mappingToDelete = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => hashIDs.Any(h => h == r.HashId));

                FilesDbContext.ThirdpartyIdMapping.RemoveRange(mappingToDelete);
                FilesDbContext.SaveChanges();

                tx.Commit();
            }

            if (!(driveFolder is ErrorDriveEntry))
                ProviderInfo.Storage.DeleteEntry(driveFolder.Id);

            ProviderInfo.CacheReset(driveFolder.Id);
            var parentDriveId = GetParentDriveId(driveFolder);
            if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, true);
        }


        public int MoveFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = CrossDao.PerformCrossDaoFolderCopy(
                folderId, this, GoogleDriveDaoSelector.GetFileDao(folderId), GoogleDriveDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                true, cancellationToken);

            return moved.ID;
        }

        public TTo MoveFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            if (toFolderId is int tId)
            {
                return (TTo)Convert.ChangeType(MoveFolder(folderId, tId, cancellationToken), typeof(TTo));
            }

            if (toFolderId is string tsId)
            {
                return (TTo)Convert.ChangeType(MoveFolder(folderId, tsId, cancellationToken), typeof(TTo));
            }

            throw new NotImplementedException();
        }

        public string MoveFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var driveFolder = GetDriveEntry(folderId);
            if (driveFolder is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var toDriveFolder = GetDriveEntry(toFolderId);
            if (toDriveFolder is ErrorDriveEntry errorDriveEntry1) throw new Exception(errorDriveEntry1.Error);

            var fromFolderDriveId = GetParentDriveId(driveFolder);

            driveFolder = ProviderInfo.Storage.InsertEntryIntoFolder(driveFolder, toDriveFolder.Id);
            if (fromFolderDriveId != null)
            {
                ProviderInfo.Storage.RemoveEntryFromFolder(driveFolder, fromFolderDriveId);
            }

            ProviderInfo.CacheReset(driveFolder.Id);
            ProviderInfo.CacheReset(fromFolderDriveId, true);
            ProviderInfo.CacheReset(toDriveFolder.Id, true);

            return MakeId(driveFolder.Id);
        }

        public Folder<TTo> CopyFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            if (toFolderId is int tId)
            {
                return CopyFolder(folderId, tId, cancellationToken) as Folder<TTo>;
            }

            if (toFolderId is string tsId)
            {
                return CopyFolder(folderId, tsId, cancellationToken) as Folder<TTo>;
            }

            throw new NotImplementedException();
        }

        public Folder<int> CopyFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var moved = CrossDao.PerformCrossDaoFolderCopy(
                folderId, this, GoogleDriveDaoSelector.GetFileDao(folderId), GoogleDriveDaoSelector.ConvertId,
                toFolderId, FolderDao, FileDao, r => r,
                false, cancellationToken);

            return moved;
        }

        public Folder<string> CopyFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var driveFolder = GetDriveEntry(folderId);
            if (driveFolder is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var toDriveFolder = GetDriveEntry(toFolderId);
            if (toDriveFolder is ErrorDriveEntry errorDriveEntry1) throw new Exception(errorDriveEntry1.Error);

            var newDriveFolder = ProviderInfo.Storage.InsertEntry(null, driveFolder.Name, toDriveFolder.Id, true);

            ProviderInfo.CacheReset(newDriveFolder);
            ProviderInfo.CacheReset(toDriveFolder.Id, true);
            ProviderInfo.CacheReset(toDriveFolder.Id);

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
            var driveFolder = GetDriveEntry(folder.ID);

            if (IsRoot(driveFolder))
            {
                //It's root folder
                DaoSelector.RenameProvider(ProviderInfo, newTitle);
                //rename provider customer title
            }
            else
            {
                //rename folder
                driveFolder.Name = newTitle;
                driveFolder = ProviderInfo.Storage.RenameEntry(driveFolder.Id, driveFolder.Name);
            }

            ProviderInfo.CacheReset(driveFolder);
            var parentDriveId = GetParentDriveId(driveFolder);
            if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, true);

            return MakeId(driveFolder.Id);
        }

        public int GetItemsCount(string folderId)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty(string folderId)
        {
            var driveId = MakeDriveId(folderId);
            //note: without cache
            return ProviderInfo.Storage.GetEntries(driveId).Count == 0;
        }

        public bool UseTrashForRemove(Folder<string> folder)
        {
            return false;
        }

        public bool UseRecursiveOperation(string folderId, string toRootFolderId)
        {
            return true;
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
            var storageMaxUploadSize = ProviderInfo.Storage.GetMaxUploadSize();

            return chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, SetupInfo.AvailableFileSize);
        }
    }
}