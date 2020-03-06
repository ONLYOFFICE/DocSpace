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
using System.Threading;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Web.Studio.Core;

namespace ASC.Files.Thirdparty.Dropbox
{
    internal class DropboxFolderDao : DropboxDaoBase, IFolderDao<string>
    {
        public DropboxFolderDao(IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo)
        {
        }

        public DropboxFolderDao(DropboxDaoSelector.DropboxInfo dropboxInfo, DropboxDaoSelector dropboxDaoSelector)
            : base(dropboxInfo, dropboxDaoSelector)
        {
        }

        public Folder<string> GetFolder(string folderId)
        {
            return ToFolder(GetDropboxFolder(folderId));
        }

        public Folder<string> GetFolder(string title, string parentId)
        {
            var metadata = GetDropboxItems(parentId, true)
                .FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));

            return metadata == null
                       ? null
                       : ToFolder(metadata.AsFolder);
        }

        public Folder<string> GetRootFolderByFile(string fileId)
        {
            return GetRootFolder(fileId);
        }

        public List<Folder<string>> GetFolders(string parentId)
        {
            return GetDropboxItems(parentId, true).Select(item => ToFolder(item.AsFolder)).ToList();
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

            switch (orderBy.SortedBy)
            {
                case SortedByType.Author:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.CreateBy) : folders.OrderByDescending(x => x.CreateBy);
                    break;
                case SortedByType.AZ:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title);
                    break;
                case SortedByType.DateAndTime:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.ModifiedOn) : folders.OrderByDescending(x => x.ModifiedOn);
                    break;
                case SortedByType.DateAndTimeCreation:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.CreateOn) : folders.OrderByDescending(x => x.CreateOn);
                    break;
                default:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title);
                    break;
            }

            return folders.ToList();
        }

        public List<Folder<string>> GetFolders(string[] folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
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
                var dropboxFolder = GetDropboxFolder(folderId);

                if (dropboxFolder is ErrorFolder)
                {
                    folderId = null;
                }
                else
                {
                    path.Add(ToFolder(dropboxFolder));
                    folderId = GetParentFolderPath(dropboxFolder);
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

            if (folder.ParentFolderID != null)
            {
                var dropboxFolderPath = MakeDropboxPath(folder.ParentFolderID);

                folder.Title = GetAvailableTitle(folder.Title, dropboxFolderPath, IsExist);

                var dropboxFolder = DropboxProviderInfo.Storage.CreateFolder(folder.Title, dropboxFolderPath);

                DropboxProviderInfo.CacheReset(dropboxFolder);
                var parentFolderPath = GetParentFolderPath(dropboxFolder);
                if (parentFolderPath != null) DropboxProviderInfo.CacheReset(parentFolderPath);

                return MakeId(dropboxFolder);
            }
            return null;
        }

        public bool IsExist(string title, string folderId)
        {
            return GetDropboxItems(folderId, true)
                .Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public void DeleteFolder(string folderId)
        {
            var dropboxFolder = GetDropboxFolder(folderId);
            var id = MakeId(dropboxFolder);

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

                var tagsToRemove = Query(FilesDbContext.Tag)
                    .Where(r => !Query(FilesDbContext.TagLink).Where(a => a.TagId == r.Id).Any());

                FilesDbContext.Tag.RemoveRange(tagsToRemove);

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

            if (!(dropboxFolder is ErrorFolder))
                DropboxProviderInfo.Storage.DeleteItem(dropboxFolder);

            DropboxProviderInfo.CacheReset(MakeDropboxPath(dropboxFolder), true);
            var parentFolderPath = GetParentFolderPath(dropboxFolder);
            if (parentFolderPath != null) DropboxProviderInfo.CacheReset(parentFolderPath);
        }

        public string MoveFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var dropboxFolder = GetDropboxFolder(folderId);
            if (dropboxFolder is ErrorFolder) throw new Exception(((ErrorFolder)dropboxFolder).Error);

            var toDropboxFolder = GetDropboxFolder(toFolderId);
            if (toDropboxFolder is ErrorFolder) throw new Exception(((ErrorFolder)toDropboxFolder).Error);

            var fromFolderPath = GetParentFolderPath(dropboxFolder);

            dropboxFolder = DropboxProviderInfo.Storage.MoveFolder(MakeDropboxPath(dropboxFolder), MakeDropboxPath(toDropboxFolder), dropboxFolder.Name);

            DropboxProviderInfo.CacheReset(MakeDropboxPath(dropboxFolder), false);
            DropboxProviderInfo.CacheReset(fromFolderPath);
            DropboxProviderInfo.CacheReset(MakeDropboxPath(toDropboxFolder));

            return MakeId(dropboxFolder);
        }

        public Folder<string> CopyFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var dropboxFolder = GetDropboxFolder(folderId);
            if (dropboxFolder is ErrorFolder) throw new Exception(((ErrorFolder)dropboxFolder).Error);

            var toDropboxFolder = GetDropboxFolder(toFolderId);
            if (toDropboxFolder is ErrorFolder) throw new Exception(((ErrorFolder)toDropboxFolder).Error);

            var newDropboxFolder = DropboxProviderInfo.Storage.CopyFolder(MakeDropboxPath(dropboxFolder), MakeDropboxPath(toDropboxFolder), dropboxFolder.Name);

            DropboxProviderInfo.CacheReset(newDropboxFolder);
            DropboxProviderInfo.CacheReset(MakeDropboxPath(newDropboxFolder), false);
            DropboxProviderInfo.CacheReset(MakeDropboxPath(toDropboxFolder));

            return ToFolder(newDropboxFolder);
        }

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, string to)
        {
            return new Dictionary<string, string>();
        }

        public string RenameFolder(Folder<string> folder, string newTitle)
        {
            var dropboxFolder = GetDropboxFolder(folder.ID);
            var parentFolderPath = GetParentFolderPath(dropboxFolder);

            if (IsRoot(dropboxFolder))
            {
                //It's root folder
                DropboxDaoSelector.RenameProvider(DropboxProviderInfo, newTitle);
                //rename provider customer title
            }
            else
            {
                newTitle = GetAvailableTitle(newTitle, parentFolderPath, IsExist);

                //rename folder
                dropboxFolder = DropboxProviderInfo.Storage.MoveFolder(MakeDropboxPath(dropboxFolder), parentFolderPath, newTitle);
            }

            DropboxProviderInfo.CacheReset(dropboxFolder);
            if (parentFolderPath != null) DropboxProviderInfo.CacheReset(parentFolderPath);

            return MakeId(dropboxFolder);
        }

        public int GetItemsCount(string folderId)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty(string folderId)
        {
            var dropboxFolderPath = MakeDropboxPath(folderId);
            //note: without cache
            return DropboxProviderInfo.Storage.GetItems(dropboxFolderPath).Count == 0;
        }

        public bool UseTrashForRemove(Folder<string> folder)
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

        public long GetMaxUploadSize(string folderId, bool chunkedUpload)
        {
            var storageMaxUploadSize = DropboxProviderInfo.Storage.MaxChunkedUploadFileSize;

            return chunkedUpload ? storageMaxUploadSize : Math.Min(storageMaxUploadSize, SetupInfo.AvailableFileSize);
        }

        #region Only for TMFolderDao

        public void ReassignFolders(string[] folderIds, Guid newOwnerId)
        {
        }

        public IEnumerable<Folder<string>> Search(string text, bool bunch)
        {
            return null;
        }

        public string GetFolderID(string module, string bunch, string data, bool createIfNotExists)
        {
            return null;
        }

        public IEnumerable<string> GetFolderIDs(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
        {
            return new List<string>();
        }

        public string GetFolderIDCommon(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDUser(bool createIfNotExists, Guid? userId)
        {
            return null;
        }

        public string GetFolderIDShare(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDTrash(bool createIfNotExists, Guid? userId)
        {
            return null;
        }


        public string GetFolderIDPhotos(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDProjects(bool createIfNotExists)
        {
            return null;
        }

        public string GetBunchObjectID(string folderID)
        {
            return null;
        }

        public Dictionary<string, string> GetBunchObjectIDs(List<string> folderIDs)
        {
            return null;
        }

        #endregion
    }
}