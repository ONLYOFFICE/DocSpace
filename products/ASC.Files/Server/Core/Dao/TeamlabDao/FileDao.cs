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

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Files.Core.EF;
using ASC.Files.Resources;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Search;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.Data
{
    public class FileDao : AbstractDao, IFileDao<int>
    {
        private static readonly object syncRoot = new object();
        public FactoryIndexer<FilesWrapper> FactoryIndexer { get; }
        public GlobalStore GlobalStore { get; }
        public GlobalSpace GlobalSpace { get; }
        public GlobalFolder GlobalFolder { get; }
        public IDaoFactory DaoFactory { get; }
        public ChunkedUploadSessionHolder ChunkedUploadSessionHolder { get; }

        public FileDao(
            FactoryIndexer<FilesWrapper> factoryIndexer,
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
            GlobalStore globalStore,
            GlobalSpace globalSpace,
            GlobalFolder globalFolder,
            IDaoFactory daoFactory,
            ChunkedUploadSessionHolder chunkedUploadSessionHolder)
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
                  serviceProvider)
        {
            FactoryIndexer = factoryIndexer;
            GlobalStore = globalStore;
            GlobalSpace = globalSpace;
            GlobalFolder = globalFolder;
            DaoFactory = daoFactory;
            ChunkedUploadSessionHolder = chunkedUploadSessionHolder;
        }

        public void InvalidateCache(int fileId)
        {
        }

        public File<int> GetFile(int fileId)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.CurrentVersion);
            return FromQueryWithShared(query).SingleOrDefault();
        }

        public File<int> GetFile(int fileId, int fileVersion)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.Version == fileVersion);
            return FromQueryWithShared(query).SingleOrDefault();
        }

        public File<int> GetFile(int parentId, string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(title);

            var query = GetFileQuery(r => r.Title == title && r.CurrentVersion == true && r.FolderId == parentId)
                .OrderBy(r => r.CreateOn);

            return FromQueryWithShared(query).FirstOrDefault();
        }

        public File<int> GetFileStable(int fileId, int fileVersion = -1)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.Forcesave == ForcesaveType.None);

            if (fileVersion >= 0)
            {
                query = query.Where(r => r.Version <= fileVersion);
            }

            query = query.OrderByDescending(r => r.Version);

            return FromQueryWithShared(query).SingleOrDefault();
        }

        public List<File<int>> GetFileHistory(int fileId)
        {
            var query = GetFileQuery(r => r.Id == fileId).OrderByDescending(r => r.Version);

            return FromQueryWithShared(query);
        }

        public List<File<int>> GetFiles(int[] fileIds)
        {
            if (fileIds == null || fileIds.Length == 0) return new List<File<int>>();

            var query = GetFileQuery(r => fileIds.Any(a => a == r.Id) && r.CurrentVersion);

            return FromQueryWithShared(query);
        }

        public List<File<int>> GetFilesForShare(int[] fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            if (fileIds == null || fileIds.Length == 0 || filterType == FilterType.FoldersOnly) return new List<File<int>>();

            var query = GetFileQuery(r => fileIds.Any(a => a == r.Id) && r.CurrentVersion);

            if (!string.IsNullOrEmpty(searchText))
            {
                var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

                if (FactoryIndexer.TrySelectIds(s => func(s).In(r => r.Id, fileIds), out var searchIds))
                {
                    query = query.Where(r => searchIds.Any(b => b == r.Id));
                }
                else
                {
                    query = query.Where(r => BuildSearch(r, searchText, SearhTypeEnum.Any));
                }
            }

            if (subjectID != Guid.Empty)
            {
                if (subjectGroup)
                {
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
                    query = query.Where(r => users.Any(b => b == r.CreateBy));
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
                        query = query.Where(r => BuildSearch(r, searchText, SearhTypeEnum.End));
                    }
                    break;
            }

            return FromQuery(query);
        }


        public List<int> GetFiles(int parentId)
        {
            var query = GetFileQuery(r => r.FolderId == parentId && r.CurrentVersion).Select(r => r.Id);

            return Query(FilesDbContext.Files)
                .Where(r => r.FolderId == parentId && r.CurrentVersion)
                .Select(r => r.Id)
                .ToList();
        }

        public List<File<int>> GetFiles(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<int>>();

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var q = GetFileQuery(r => r.FolderId == parentId && r.CurrentVersion);


            if (withSubfolders)
            {
                q = GetFileQuery(r => r.CurrentVersion)
                    .Join(FilesDbContext.Tree, r => r.FolderId, a => a.FolderId, (file, tree) => new { file, tree })
                    .Where(r => r.tree.ParentId == parentId)
                    .Select(r => r.file);
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                var func = GetFuncForSearch(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders);

                Expression<Func<Selector<FilesWrapper>, Selector<FilesWrapper>>> expression = s => func(s);

                if (FactoryIndexer.TrySelectIds(expression, out var searchIds))
                {
                    q = q.Where(r => searchIds.Any(a => a == r.Id));
                }
                else
                {
                    q = q.Where(r => BuildSearch(r, searchText, SearhTypeEnum.Any));
                }
            }

            switch (orderBy.SortedBy)
            {
                case SortedByType.Author:
                    q = orderBy.IsAsc ? q.OrderBy(r => r.CreateBy) : q.OrderByDescending(r => r.CreateBy);
                    break;
                case SortedByType.Size:
                    q = orderBy.IsAsc ? q.OrderBy(r => r.ContentLength) : q.OrderByDescending(r => r.ContentLength);
                    break;
                case SortedByType.AZ:
                    q = orderBy.IsAsc ? q.OrderBy(r => r.Title) : q.OrderByDescending(r => r.Title);
                    break;
                case SortedByType.DateAndTime:
                    q = orderBy.IsAsc ? q.OrderBy(r => r.ModifiedOn) : q.OrderByDescending(r => r.ModifiedOn);
                    break;
                case SortedByType.DateAndTimeCreation:
                    q = orderBy.IsAsc ? q.OrderBy(r => r.CreateOn) : q.OrderByDescending(r => r.CreateOn);
                    break;
                default:
                    q = q.OrderBy(r => r.Title);
                    break;
            }

            if (subjectID != Guid.Empty)
            {
                if (subjectGroup)
                {
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
                    q = q.Where(r => users.Any(a => a == r.CreateBy));
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
                        q = q.Where(r => BuildSearch(r, searchText, SearhTypeEnum.End));
                    }
                    break;
            }

            return FromQueryWithShared(q);
        }

        public Stream GetFileStream(File<int> file, long offset)
        {
            return GlobalStore.GetStore().GetReadStream(string.Empty, GetUniqFilePath(file), (int)offset);
        }

        public Uri GetPreSignedUri(File<int> file, TimeSpan expires)
        {
            return GlobalStore.GetStore().GetPreSignedUri(string.Empty, GetUniqFilePath(file), expires,
                                                     new List<string>
                                                         {
                                                             string.Concat("Content-Disposition:", ContentDispositionUtil.GetHeaderValue(file.Title, withoutBase: true))
                                                         });
        }

        public bool IsSupportedPreSignedUri(File<int> file)
        {
            return GlobalStore.GetStore().IsSupportedPreSignedUri;
        }

        public Stream GetFileStream(File<int> file)
        {
            return GetFileStream(file, 0);
        }

        public File<int> SaveFile(File<int> file, Stream fileStream)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var maxChunkedUploadSize = SetupInfo.MaxChunkedUploadSize(TenantExtra, TenantStatisticProvider);
            if (maxChunkedUploadSize < file.ContentLength)
            {
                throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
            }

            if (CoreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
            {
                var personalMaxSpace = CoreConfiguration.PersonalMaxSpace(SettingsManager);
                if (personalMaxSpace - GlobalSpace.GetUserUsedSpace(file.ID == default ? AuthContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
                {
                    throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
                }
            }

            var isNew = false;
            List<int> parentFoldersIds;
            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();

                if (file.ID == default)
                {
                    file.ID = FilesDbContext.Files.Max(r => r.Id) + 1;
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
                    .Where(r => r.Id == file.ID && r.CurrentVersion && r.TenantId == TenantID)
                    .FirstOrDefault();

                if (toUpdate != null)
                {
                    toUpdate.CurrentVersion = false;
                    FilesDbContext.SaveChanges();
                }

                var toInsert = new DbFile
                {
                    Id = file.ID,
                    Version = file.Version,
                    VersionGroup = file.VersionGroup,
                    CurrentVersion = true,
                    FolderId = (int)file.FolderID,
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
                    TenantId = TenantID
                };

                FilesDbContext.Files.Add(toInsert);
                FilesDbContext.SaveChanges();

                tx.Commit();

                file.PureTitle = file.Title;

                parentFoldersIds =
                    FilesDbContext.Tree
                    .Where(r => r.FolderId == (int)file.FolderID)
                    .OrderByDescending(r => r.Level)
                    .Select(r => r.ParentId)
                    .ToList();

                if (parentFoldersIds.Count > 0)
                {
                    var folderToUpdate = FilesDbContext.Folders
                        .Where(r => parentFoldersIds.Any(a => a == r.Id));

                    foreach (var f in folderToUpdate)
                    {
                        f.ModifiedOn = TenantUtil.DateTimeToUtc(file.ModifiedOn);
                        f.ModifiedBy = file.ModifiedBy;
                    }

                    FilesDbContext.SaveChanges();
                }

                if (isNew)
                {
                    RecalculateFilesCount(file.FolderID);
                }
            }

            if (fileStream != null)
            {
                try
                {
                    SaveFileStream(file, fileStream);
                }
                catch
                {
                    if (isNew)
                    {
                        var stored = GlobalStore.GetStore().IsDirectory(GetUniqFileDirectory(file.ID));
                        DeleteFile(file.ID, stored);
                    }
                    else if (!IsExistOnStorage(file))
                    {
                        DeleteVersion(file);
                    }
                    throw;
                }
            }

            FactoryIndexer.IndexAsync(FilesWrapper.GetFilesWrapper(ServiceProvider, file, parentFoldersIds));

            return GetFile(file.ID);
        }

        public File<int> ReplaceFileVersion(File<int> file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (file.ID == default) throw new ArgumentException("No file id or folder id toFolderId determine provider");

            var maxChunkedUploadSize = SetupInfo.MaxChunkedUploadSize(TenantExtra, TenantStatisticProvider);

            if (maxChunkedUploadSize < file.ContentLength)
            {
                throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
            }

            if (CoreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
            {
                var personalMaxSpace = CoreConfiguration.PersonalMaxSpace(SettingsManager);
                if (personalMaxSpace - GlobalSpace.GetUserUsedSpace(file.ID == default ? AuthContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
                {
                    throw FileSizeComment.GetPersonalFreeSpaceException(personalMaxSpace);
                }
            }

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

                var toUpdate = FilesDbContext.Files
                    .Where(r => r.Id == file.ID && r.Version == file.Version)
                    .FirstOrDefault();

                toUpdate.Version = file.Version;
                toUpdate.VersionGroup = file.VersionGroup;
                toUpdate.FolderId = (int)file.FolderID;
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

                FilesDbContext.SaveChanges();

                tx.Commit();

                file.PureTitle = file.Title;

                parentFoldersIds = FilesDbContext.Tree
                    .Where(r => r.FolderId == (int)file.FolderID)
                    .OrderByDescending(r => r.Level)
                    .Select(r => r.ParentId)
                    .ToList();

                if (parentFoldersIds.Count > 0)
                {
                    var folderToUpdate = FilesDbContext.Folders
                        .Where(r => parentFoldersIds.Any(a => a == r.Id));

                    foreach (var f in folderToUpdate)
                    {
                        f.ModifiedOn = TenantUtil.DateTimeToUtc(file.ModifiedOn);
                        f.ModifiedBy = file.ModifiedBy;
                    }

                    FilesDbContext.SaveChanges();
                }
            }

            if (fileStream != null)
            {
                try
                {
                    DeleteVersionStream(file);
                    SaveFileStream(file, fileStream);
                }
                catch
                {
                    if (!IsExistOnStorage(file))
                    {
                        DeleteVersion(file);
                    }
                    throw;
                }
            }

            FactoryIndexer.IndexAsync(FilesWrapper.GetFilesWrapper(ServiceProvider, file, parentFoldersIds));

            return GetFile(file.ID);
        }

        private void DeleteVersion(File<int> file)
        {
            if (file == null
                || file.ID == default
                || file.Version <= 1) return;

            var toDelete = Query(FilesDbContext.Files)
                .Where(r => r.Id == file.ID)
                .Where(r => r.Version == file.Version)
                .FirstOrDefault();

            if (toDelete != null)
            {
                FilesDbContext.Files.Remove(toDelete);
            }
            FilesDbContext.SaveChanges();

            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == file.ID)
                .Where(r => r.Version == file.Version - 1)
                .FirstOrDefault();

            toUpdate.CurrentVersion = true;
            FilesDbContext.SaveChanges();
        }

        private void DeleteVersionStream(File<int> file)
        {
            GlobalStore.GetStore().DeleteDirectory(GetUniqFileVersionPath(file.ID, file.Version));
        }

        private void SaveFileStream(File<int> file, Stream stream)
        {
            GlobalStore.GetStore().Save(string.Empty, GetUniqFilePath(file), stream, file.Title);
        }

        public void DeleteFile(int fileId)
        {
            DeleteFile(fileId, true);
        }

        private void DeleteFile(int fileId, bool deleteFolder)
        {
            if (fileId == default) return;
            using var tx = FilesDbContext.Database.BeginTransaction();

            var fromFolders = Query(FilesDbContext.Files).Where(r => r.Id == fileId).GroupBy(r => r.Id).SelectMany(r => r.Select(a => a.FolderId)).Distinct().ToList();

            var toDeleteFiles = Query(FilesDbContext.Files).Where(r => r.Id == fileId);
            FilesDbContext.RemoveRange(toDeleteFiles);

            var toDeleteLinks = Query(FilesDbContext.TagLink).Where(r => r.EntryId == fileId.ToString()).Where(r => r.EntryType == FileEntryType.File);
            FilesDbContext.RemoveRange(toDeleteFiles);

            var tagsToRemove = Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Where(a => a.TagId == r.Id).Any());

            FilesDbContext.Tag.RemoveRange(tagsToRemove);

            var securityToDelete = Query(FilesDbContext.Security)
                .Where(r => r.EntryId == fileId.ToString())
                .Where(r => r.EntryType == FileEntryType.File);

            FilesDbContext.Security.RemoveRange(securityToDelete);
            FilesDbContext.SaveChanges();

            tx.Commit();

            fromFolders.ForEach(folderId => RecalculateFilesCount(folderId));

            if (deleteFolder)
                DeleteFolder(fileId);

            var wrapper = ServiceProvider.GetService<FilesWrapper>();
            wrapper.Id = (int)fileId;
            FactoryIndexer.DeleteAsync(wrapper);
        }

        public bool IsExist(string title, object folderId)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(string title, int folderId)
        {
            return Query(FilesDbContext.Files)
                .Where(r => r.Title == title)
                .Where(r => r.FolderId == folderId)
                .Where(r => r.CurrentVersion)
                .Any();
        }

        public int MoveFile(int fileId, int toFolderId)
        {
            if (fileId == default) return default;

            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var fromFolders = Query(FilesDbContext.Files)
                    .Where(r => r.Id == fileId)
                    .GroupBy(r => r.Id)
                    .SelectMany(r => r.Select(a => a.FolderId))
                    .Distinct()
                    .ToList();

                var toUpdate = Query(FilesDbContext.Files)
                    .Where(r => r.Id == fileId);

                foreach (var f in toUpdate)
                {
                    f.FolderId = toFolderId;

                    if (GlobalFolder.GetFolderTrash<int>(DaoFactory).Equals(toFolderId))
                    {
                        f.ModifiedBy = AuthContext.CurrentAccount.ID;
                        f.ModifiedOn = DateTime.UtcNow;
                    }
                }

                FilesDbContext.SaveChanges();
                tx.Commit();

                fromFolders.ForEach(folderId => RecalculateFilesCount(folderId));
                RecalculateFilesCount(toFolderId);
            }

            var parentFoldersIds =
                FilesDbContext.Tree
                .Where(r => r.FolderId == (int)toFolderId)
                .OrderByDescending(r => r.Level)
                .Select(r => r.ParentId)
                .ToList();

            var wrapper = ServiceProvider.GetService<FilesWrapper>();
            wrapper.Id = fileId;
            wrapper.Folders = parentFoldersIds.Select(r => new FilesFoldersWrapper() { FolderId = r.ToString() }).ToList();

            FactoryIndexer.Update(wrapper,
                UpdateAction.Replace,
                w => w.Folders);

            return fileId;
        }

        public File<int> CopyFile(int fileId, int toFolderId)
        {
            var file = GetFile(fileId);
            if (file != null)
            {
                var copy = ServiceProvider.GetService<File<int>>();
                copy.FileStatus = file.FileStatus;
                copy.FolderID = toFolderId;
                copy.Title = file.Title;
                copy.ConvertedType = file.ConvertedType;
                copy.Comment = FilesCommonResource.CommentCopy;
                copy.Encrypted = file.Encrypted;

                using (var stream = GetFileStream(file))
                {
                    copy.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                    copy = SaveFile(copy, stream);
                }

                return copy;
            }
            return null;
        }

        public int FileRename(File<int> file, string newTitle)
        {
            newTitle = Global.ReplaceInvalidCharsAndTruncate(newTitle);
            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == file.ID)
                .Where(r => r.CurrentVersion)
                .FirstOrDefault();

            toUpdate.Title = newTitle;
            toUpdate.ModifiedOn = DateTime.UtcNow;
            toUpdate.ModifiedBy = AuthContext.CurrentAccount.ID;

            FilesDbContext.SaveChanges();

            return file.ID;
        }

        public string UpdateComment(int fileId, int fileVersion, string comment)
        {
            comment ??= string.Empty;
            comment = comment.Substring(0, Math.Min(comment.Length, 255));

            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .FirstOrDefault();

            toUpdate.Comment = comment;

            FilesDbContext.SaveChanges();

            return comment;
        }

        public void CompleteVersion(int fileId, int fileVersion)
        {
            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version >= fileVersion);

            foreach (var f in toUpdate)
            {
                f.VersionGroup += 1;
            }

            FilesDbContext.SaveChanges();
        }

        public void ContinueVersion(int fileId, int fileVersion)
        {
            using var tx = FilesDbContext.Database.BeginTransaction();

            var versionGroup = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .Select(r => r.VersionGroup)
                .FirstOrDefault();

            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version >= fileVersion)
                .Where(r => r.VersionGroup >= versionGroup);

            foreach (var f in toUpdate)
            {
                f.VersionGroup -= 1;
            }

            FilesDbContext.SaveChanges();

            tx.Commit();
        }

        public bool UseTrashForRemove(File<int> file)
        {
            return file.RootFolderType != FolderType.TRASH;
        }

        public string GetUniqFileDirectory(int fileId)
        {
            if (fileId == 0) throw new ArgumentNullException("fileIdObject");
            return string.Format("folder_{0}/file_{1}", (Convert.ToInt32(fileId) / 1000 + 1) * 1000, fileId);
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
                       ? string.Format("{0}/{1}", GetUniqFileVersionPath(file.ID, file.Version), fileTitle)
                       : null;
        }

        public string GetUniqFileVersionPath(int fileId, int version)
        {
            return fileId != 0
                       ? string.Format("{0}/v{1}", GetUniqFileDirectory(fileId), version)
                       : null;
        }

        private void RecalculateFilesCount(object folderId)
        {
            GetRecalculateFilesCountUpdate(folderId);
        }

        #region chunking

        public ChunkedUploadSession<int> CreateUploadSession(File<int> file, long contentLength)
        {
            return ChunkedUploadSessionHolder.CreateUploadSession(file, contentLength);
        }

        public void UploadChunk(ChunkedUploadSession<int> uploadSession, Stream stream, long chunkLength)
        {
            if (!uploadSession.UseChunks)
            {
                using (var streamToSave = ChunkedUploadSessionHolder.UploadSingleChunk(uploadSession, stream, chunkLength))
                {
                    if (streamToSave != Stream.Null)
                    {
                        uploadSession.File = SaveFile(GetFileForCommit(uploadSession), streamToSave);
                    }
                }

                return;
            }

            ChunkedUploadSessionHolder.UploadChunk(uploadSession, stream, chunkLength);

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                uploadSession.File = FinalizeUploadSession(uploadSession);
            }
        }

        private File<int> FinalizeUploadSession(ChunkedUploadSession<int> uploadSession)
        {
            ChunkedUploadSessionHolder.FinalizeUploadSession(uploadSession);

            var file = GetFileForCommit(uploadSession);
            SaveFile(file, null);
            ChunkedUploadSessionHolder.Move(uploadSession, GetUniqFilePath(file));

            return file;
        }

        public void AbortUploadSession(ChunkedUploadSession<int> uploadSession)
        {
            ChunkedUploadSessionHolder.AbortUploadSession(uploadSession);
        }

        private File<int> GetFileForCommit(ChunkedUploadSession<int> uploadSession)
        {
            if (uploadSession.File.ID != default)
            {
                var file = GetFile(uploadSession.File.ID);
                file.Version++;
                file.ContentLength = uploadSession.BytesTotal;
                file.ConvertedType = null;
                file.Comment = FilesCommonResource.CommentUpload;
                file.Encrypted = uploadSession.Encrypted;
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

        public void ReassignFiles(int[] fileIds, Guid newOwnerId)
        {
            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.CurrentVersion)
                .Where(r => fileIds.Any(a => a == r.Id));

            foreach (var f in toUpdate)
            {
                f.CreateBy = newOwnerId;
            }

            FilesDbContext.SaveChanges();
        }

        public List<File<int>> GetFiles(int[] parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            if (parentIds == null || parentIds.Length == 0 || filterType == FilterType.FoldersOnly) return new List<File<int>>();

            var q = GetFileQuery(r => r.CurrentVersion)
                .Join(FilesDbContext.Tree, a => a.FolderId, t => t.FolderId, (file, tree) => new { file, tree })
                .Where(r => parentIds.Any(a => a == r.tree.ParentId))
                .Select(r => r.file);

            if (!string.IsNullOrEmpty(searchText))
            {
                var func = GetFuncForSearch(null, null, filterType, subjectGroup, subjectID, searchText, searchInContent, false);

                if (FactoryIndexer.TrySelectIds(s => func(s), out var searchIds))
                {
                    q = q.Where(r => searchIds.Any(b => b == r.Id));
                }
                else
                {
                    q = q.Where(r => BuildSearch(r, searchText, SearhTypeEnum.Any));
                }
            }

            if (subjectID != Guid.Empty)
            {
                if (subjectGroup)
                {
                    var users = UserManager.GetUsersByGroup(subjectID).Select(u => u.ID).ToArray();
                    q = q.Where(r => users.Any(u => u == r.CreateBy));
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
                        q = q.Where(r => BuildSearch(r, searchText, SearhTypeEnum.End));
                    }
                    break;
            }

            return FromQueryWithShared(q);
        }

        public IEnumerable<File<int>> Search(string searchText, bool bunch)
        {
            if (FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var ids))
            {
                var query = GetFileQuery(r => r.CurrentVersion && ids.Any(i => i == r.Id));
                return FromQueryWithShared(query)
                    .Where(
                        f =>
                        bunch
                            ? f.RootFolderType == FolderType.BUNCH
                            : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON)
                    .ToList();
            }
            else
            {
                var query = GetFileQuery(r => r.CurrentVersion && BuildSearch(r, searchText, SearhTypeEnum.Any));
                return FromQueryWithShared(query)
                    .Where(f =>
                           bunch
                                ? f.RootFolderType == FolderType.BUNCH
                                : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON)
                    .ToList();
            }
        }

        private void DeleteFolder(int fileId)
        {
            GlobalStore.GetStore().DeleteDirectory(GetUniqFileDirectory(fileId));
        }

        public bool IsExistOnStorage(File<int> file)
        {
            return GlobalStore.GetStore().IsFile(GetUniqFilePath(file));
        }

        private const string DiffTitle = "diff.zip";

        public void SaveEditHistory(File<int> file, string changes, Stream differenceStream)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (string.IsNullOrEmpty(changes)) throw new ArgumentNullException("changes");
            if (differenceStream == null) throw new ArgumentNullException("differenceStream");

            changes = changes.Trim();

            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == file.ID)
                .Where(r => r.Version == file.Version);

            foreach (var f in toUpdate)
            {
                f.Changes = changes;
            }

            FilesDbContext.SaveChanges();

            GlobalStore.GetStore().Save(string.Empty, GetUniqFilePath(file, DiffTitle), differenceStream, DiffTitle);
        }

        public List<EditHistory> GetEditHistory(DocumentServiceHelper documentServiceHelper, int fileId, int fileVersion = 0)
        {
            var query = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Forcesave == ForcesaveType.None);

            if (fileVersion > 0)
            {
                query = query.Where(r => r.Version == fileVersion);
            }

            query = query.OrderBy(r => r.Version);

            return query
                    .ToList()
                    .Select(r =>
                        {
                            var item = ServiceProvider.GetService<EditHistory>();
                            var editHistoryAuthor = ServiceProvider.GetService<EditHistoryAuthor>();

                            editHistoryAuthor.Id = r.ModifiedBy;
                            item.ID = r.Id;
                            item.Version = r.Version;
                            item.VersionGroup = r.VersionGroup;
                            item.ModifiedOn = TenantUtil.DateTimeFromUtc(r.ModifiedOn);
                            item.ModifiedBy = editHistoryAuthor;
                            item.ChangesString = r.Changes;
                            item.Key = documentServiceHelper.GetDocKey(item.ID, item.Version, TenantUtil.DateTimeFromUtc(r.CreateOn));

                            return item;
                        })
                    .ToList();
        }

        public Stream GetDifferenceStream(File<int> file)
        {
            return GlobalStore.GetStore().GetReadStream(string.Empty, GetUniqFilePath(file, DiffTitle));
        }

        public bool ContainChanges(int fileId, int fileVersion)
        {
            return Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .Where(r => r.Changes != null)
                .Any();
        }

        #endregion

        private Func<Selector<FilesWrapper>, Selector<FilesWrapper>> GetFuncForSearch(object parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
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
                       result.In(a => a.Folders.Select(r => r.FolderId), new[] { parentId.ToString() });
                   }
                   else
                   {
                       result.InAll(a => a.Folders.Select(r => r.FolderId), new[] { parentId.ToString() });
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
                           result.Sort(r => r.LastModifiedOn, orderBy.IsAsc);
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

        protected List<File<int>> FromQueryWithShared(IQueryable<DbFile> dbFiles)
        {
            return dbFiles
                .Select(r => new DbFileQuery
                {
                    file = r,
                    root =
                    FilesDbContext.Folders
                        .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                        .Where(x => x.folder.TenantId == r.TenantId)
                        .Where(x => x.tree.FolderId == r.FolderId)
                        .OrderByDescending(r => r.tree.Level)
                        .Select(r => r.folder)
                        .FirstOrDefault(),
                    shared =
                     FilesDbContext.Security
                        .Where(x => x.EntryType == FileEntryType.File)
                        .Where(x => x.EntryId == r.Id.ToString())
                        .Any()
                })
                .ToList()
                .Select(ToFile)
                .ToList();
        }

        protected List<File<int>> FromQuery(IQueryable<DbFile> dbFiles)
        {
            return dbFiles
                .Select(r => new DbFileQuery
                {
                    file = r,
                    root = FilesDbContext.Folders
                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                            .Where(x => x.folder.TenantId == r.TenantId)
                            .Where(x => x.tree.FolderId == r.FolderId)
                            .OrderByDescending(r => r.tree.Level)
                            .Select(r => r.folder)
                            .FirstOrDefault(),
                    shared = true
                })
                .ToList()
                .Select(ToFile)
                .ToList();
        }

        public File<int> ToFile(DbFileQuery r)
        {
            var file = ServiceProvider.GetService<File<int>>();
            file.ID = r.file.Id;
            file.Title = r.file.Title;
            file.FolderID = r.file.FolderId;
            file.CreateOn = TenantUtil.DateTimeFromUtc(r.file.CreateOn);
            file.CreateBy = r.file.CreateBy;
            file.Version = r.file.Version;
            file.VersionGroup = r.file.VersionGroup;
            file.ContentLength = r.file.ContentLength;
            file.ModifiedOn = TenantUtil.DateTimeFromUtc(r.file.ModifiedOn);
            file.ModifiedBy = r.file.ModifiedBy;
            file.RootFolderType = r.root?.FolderType ?? default;
            file.RootFolderCreator = r.root?.CreateBy ?? default;
            file.RootFolderId = r.root?.Id ?? default;
            file.Shared = r.shared;
            file.ConvertedType = r.file.ConvertedType;
            file.Comment = r.file.Comment;
            file.Encrypted = r.file.Encrypted;
            file.Forcesave = r.file.Forcesave;
            return file;
        }
    }

    public class DbFileQuery
    {
        public DbFile file { get; set; }
        public DbFolder root { get; set; }
        public bool shared { get; set; }
    }

    public static class FileDaoExtention
    {
        public static DIHelper AddFileDaoService(this DIHelper services)
        {
            services.TryAddScoped<IFileDao<int>, FileDao>();
            services.TryAddTransient<File<int>>();

            return services
                .AddFilesDbContextService()
                .AddUserManagerService()
                .AddTenantManagerService()
                .AddTenantUtilService()
                .AddSetupInfo()
                .AddTenantExtraService()
                .AddTenantStatisticsProviderService()
                .AddCoreBaseSettingsService()
                .AddCoreConfigurationService()
                .AddSettingsManagerService()
                .AddAuthContextService()
                .AddGlobalStoreService()
                .AddGlobalSpaceService()
                .AddFactoryIndexerService<FilesWrapper>()
                .AddGlobalFolderService()
                .AddChunkedUploadSessionHolderService()
                .AddFolderDaoService();
        }
    }
}