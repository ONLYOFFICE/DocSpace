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

        public void InvalidateCache(int fileId)
        {
        }

        public File<int> GetFile(int fileId)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.CurrentVersion).AsNoTracking();
            return ToFile(
                    FromQueryWithShared(query)
                    .SingleOrDefault());
        }

        public File<int> GetFile(int fileId, int fileVersion)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.Version == fileVersion).AsNoTracking();
            return ToFile(FromQueryWithShared(query)
                        .SingleOrDefault());
        }

        public File<int> GetFile(int parentId, string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(title);

            var query = GetFileQuery(r => r.Title == title && r.CurrentVersion == true && r.FolderId == parentId)
                .AsNoTracking()
                .OrderBy(r => r.CreateOn);

            return ToFile(FromQueryWithShared(query).FirstOrDefault());
        }

        public File<int> GetFileStable(int fileId, int fileVersion = -1)
        {
            var query = GetFileQuery(r => r.Id == fileId && r.Forcesave == ForcesaveType.None)
                .AsNoTracking();

            if (fileVersion >= 0)
            {
                query = query.Where(r => r.Version <= fileVersion);
            }

            query = query.OrderByDescending(r => r.Version);

            return ToFile(FromQueryWithShared(query).FirstOrDefault());
        }

        public List<File<int>> GetFileHistory(int fileId)
        {
            var query = GetFileQuery(r => r.Id == fileId).OrderByDescending(r => r.Version).AsNoTracking();

            return FromQueryWithShared(query).Select(ToFile).ToList();
        }

        public List<File<int>> GetFiles(IEnumerable<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any()) return new List<File<int>>();

            var query = GetFileQuery(r => fileIds.Contains(r.Id) && r.CurrentVersion)
                .AsNoTracking();

            return FromQueryWithShared(query).Select(ToFile).ToList();
        }

        public List<File<int>> GetFilesFiltered(IEnumerable<int> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly) return new List<File<int>>();

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

            return FromQuery(query).Select(ToFile).ToList();
        }


        public List<int> GetFiles(int parentId)
        {
            return Query(FilesDbContext.Files)
                .AsNoTracking()
                .Where(r => r.FolderId == parentId && r.CurrentVersion)
                .Select(r => r.Id)
                .ToList();
        }

        public List<File<int>> GetFiles(int parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<int>>();

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

            return FromQueryWithShared(q).Select(ToFile).ToList();
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
        public async Task<Stream> GetFileStreamAsync(File<int> file)
        {
            return await GlobalStore.GetStore().GetReadStreamAsync(string.Empty, GetUniqFilePath(file), 0);
        }

        public File<int> SaveFile(File<int> file, Stream fileStream)
        {
            return SaveFile(file, fileStream, true);
        }

        public File<int> SaveFile(File<int> file, Stream fileStream, bool checkQuota = true)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var maxChunkedUploadSize = SetupInfo.MaxChunkedUploadSize(TenantExtra, TenantStatisticProvider);
            if (checkQuota && maxChunkedUploadSize < file.ContentLength)
            {
                throw FileSizeComment.GetFileSizeException(maxChunkedUploadSize);
            }

            if (checkQuota && CoreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
            {
                var personalMaxSpace = CoreConfiguration.PersonalMaxSpace(SettingsManager);
                if (personalMaxSpace - GlobalSpace.GetUserUsedSpace(file.ID == default ? AuthContext.CurrentAccount.ID : file.CreateBy) < file.ContentLength)
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

                FilesDbContext.Files.Add(toInsert);
                FilesDbContext.SaveChanges();

                tx.Commit();

                file.PureTitle = file.Title;

                var parentFolders =
                    FilesDbContext.Tree
                    .Where(r => r.FolderId == file.FolderID)
                    .OrderByDescending(r => r.Level)
                    .ToList();

                parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

                if (parentFoldersIds.Any())
                {
                    var folderToUpdate = FilesDbContext.Folders
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
                    RecalculateFilesCount(file.FolderID);
                }
            }

            if (fileStream != null)
            {
                try
                {
                    SaveFileStream(file, fileStream);
                }
                catch (Exception saveException)
                {
                    try
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
                    }
                    catch (Exception deleteException)
                    {
                        throw new Exception(saveException.Message, deleteException);
                    }
                    throw;
                }
            }

            FactoryIndexer.IndexAsync(InitDocument(toInsert));

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
                    .Where(r => r.FolderId == file.FolderID)
                    .OrderByDescending(r => r.Level)
                    .ToList();

                parentFoldersIds = parentFolders.Select(r => r.ParentId).ToList();

                if (parentFoldersIds.Any())
                {
                    var folderToUpdate = FilesDbContext.Folders
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

            FactoryIndexer.IndexAsync(InitDocument(toUpdate));

            return GetFile(file.ID);
        }

        private void DeleteVersion(File<int> file)
        {
            if (file == null
                || file.ID == default
                || file.Version <= 1) return;

            var toDelete = Query(FilesDbContext.Files)
                .FirstOrDefault(r => r.Id == file.ID && r.Version == file.Version);

            if (toDelete != null)
            {
                FilesDbContext.Files.Remove(toDelete);
            }
            FilesDbContext.SaveChanges();

            var toUpdate = Query(FilesDbContext.Files)
                .FirstOrDefault(r => r.Id == file.ID && r.Version == file.Version - 1);

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

            var fromFolders = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Select(a => a.FolderId)
                .Distinct()
                .ToList();

            var toDeleteFiles = Query(FilesDbContext.Files).Where(r => r.Id == fileId);
            FilesDbContext.RemoveRange(toDeleteFiles);

            foreach (var d in toDeleteFiles)
            {
                FactoryIndexer.DeleteAsync(d);
            }

            var toDeleteLinks = Query(FilesDbContext.TagLink).Where(r => r.EntryId == fileId.ToString() && r.EntryType == FileEntryType.File);
            FilesDbContext.RemoveRange(toDeleteFiles);

            var tagsToRemove = Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Any(a => a.TagId == r.Id));

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

            var toDeleteFile = toDeleteFiles.FirstOrDefault(r => r.CurrentVersion);
            if (toDeleteFile != null)
            {
                FactoryIndexer.DeleteAsync(toDeleteFile);
            }
        }

        public bool IsExist(string title, object folderId)
        {
            if (folderId is int fId) return IsExist(title, fId);

            throw new NotImplementedException();
        }

        public bool IsExist(string title, int folderId)
        {
            return Query(FilesDbContext.Files)
                .AsNoTracking()
                .Any(r => r.Title == title &&
                          r.FolderId == folderId &&
                          r.CurrentVersion);
        }

        public TTo MoveFile<TTo>(int fileId, TTo toFolderId)
        {
            if (toFolderId is int tId)
            {
                return (TTo)Convert.ChangeType(MoveFile(fileId, tId), typeof(TTo));
            }

            if (toFolderId is string tsId)
            {
                return (TTo)Convert.ChangeType(MoveFile(fileId, tsId), typeof(TTo));
            }

            throw new NotImplementedException();
        }

        public int MoveFile(int fileId, int toFolderId)
        {
            if (fileId == default) return default;

            List<DbFile> toUpdate;

            var trashId = GlobalFolder.GetFolderTrash<int>(DaoFactory);

            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var fromFolders = Query(FilesDbContext.Files)
                    .Where(r => r.Id == fileId)
                    .Select(a => a.FolderId)
                    .Distinct()
                    .ToList();

                toUpdate = Query(FilesDbContext.Files)
                    .Where(r => r.Id == fileId)
                    .ToList();

                foreach (var f in toUpdate)
                {
                    f.FolderId = toFolderId;

                    if (trashId.Equals(toFolderId))
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

            var parentFolders =
                FilesDbContext.Tree
                .Where(r => r.FolderId == toFolderId)
                .OrderByDescending(r => r.Level)
                .ToList();

            var toUpdateFile = toUpdate.FirstOrDefault(r => r.CurrentVersion);

            if (toUpdateFile != null)
            {
                toUpdateFile.Folders = parentFolders;
                FactoryIndexer.Update(toUpdateFile, UpdateAction.Replace, w => w.Folders);
            }

            return fileId;
        }

        public string MoveFile(int fileId, string toFolderId)
        {
            var toSelector = ProviderFolderDao.GetSelector(toFolderId);

            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, r => r,
                toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                true);

            return moved.ID;
        }

        public File<TTo> CopyFile<TTo>(int fileId, TTo toFolderId)
        {
            if (toFolderId is int tId)
            {
                return CopyFile(fileId, tId) as File<TTo>;
            }

            if (toFolderId is string tsId)
            {
                return CopyFile(fileId, tsId) as File<TTo>;
            }

            throw new NotImplementedException();
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

                if (file.ThumbnailStatus == Thumbnail.Created)
                {
                    using (var thumbnail = GetThumbnail(file))
                    {
                        SaveThumbnail(copy, thumbnail);
                    }
                    copy.ThumbnailStatus = Thumbnail.Created;
                }

                return copy;
            }
            return null;
        }

        public File<string> CopyFile(int fileId, string toFolderId)
        {
            var toSelector = ProviderFolderDao.GetSelector(toFolderId);

            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, r => r,
                toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                false);

            return moved;
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

            FactoryIndexer.UpdateAsync(toUpdate, true, r => r.Title, r => r.ModifiedBy, r => r.ModifiedOn);

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
                .Where(r => r.Version > fileVersion);

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
                .AsNoTracking()
                .Where(r => r.Id == fileId)
                .Where(r => r.Version == fileVersion)
                .Select(r => r.VersionGroup)
                .FirstOrDefault();

            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.Id == fileId)
                .Where(r => r.Version > fileVersion)
                .Where(r => r.VersionGroup > versionGroup);

            foreach (var f in toUpdate)
            {
                f.VersionGroup -= 1;
            }

            FilesDbContext.SaveChanges();

            tx.Commit();
        }

        public bool UseTrashForRemove(File<int> file)
        {
            return file.RootFolderType != FolderType.TRASH && file.RootFolderType != FolderType.Privacy;
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

        private void RecalculateFilesCount(int folderId)
        {
            GetRecalculateFilesCountUpdate(folderId);
        }

        #region chunking

        public ChunkedUploadSession<int> CreateUploadSession(File<int> file, long contentLength)
        {
            return ChunkedUploadSessionHolder.CreateUploadSession(file, contentLength);
        }

        public File<int> UploadChunk(ChunkedUploadSession<int> uploadSession, Stream stream, long chunkLength)
        {
            if (!uploadSession.UseChunks)
            {
                using var streamToSave = ChunkedUploadSessionHolder.UploadSingleChunk(uploadSession, stream, chunkLength);
                if (streamToSave != Stream.Null)
                {
                    uploadSession.File = SaveFile(GetFileForCommit(uploadSession), streamToSave);
                }

                return uploadSession.File;
            }

            ChunkedUploadSessionHolder.UploadChunk(uploadSession, stream, chunkLength);

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                uploadSession.File = FinalizeUploadSession(uploadSession);
            }

            return uploadSession.File;
        }

        private File<int> FinalizeUploadSession(ChunkedUploadSession<int> uploadSession)
        {
            ChunkedUploadSessionHolder.FinalizeUploadSession(uploadSession);

            var file = GetFileForCommit(uploadSession);
            SaveFile(file, null, uploadSession.CheckQuota);
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

        public void ReassignFiles(int[] fileIds, Guid newOwnerId)
        {
            var toUpdate = Query(FilesDbContext.Files)
                .Where(r => r.CurrentVersion)
                .Where(r => fileIds.Contains(r.Id));

            foreach (var f in toUpdate)
            {
                f.CreateBy = newOwnerId;
            }

            FilesDbContext.SaveChanges();
        }

        public List<File<int>> GetFiles(IEnumerable<int> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            if (parentIds == null || !parentIds.Any() || filterType == FilterType.FoldersOnly) return new List<File<int>>();

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

            return FromQueryWithShared(q).Select(ToFile).ToList();
        }

        public IEnumerable<File<int>> Search(string searchText, bool bunch)
        {
            if (FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out var ids))
            {
                var query = GetFileQuery(r => r.CurrentVersion && ids.Contains(r.Id)).AsNoTracking();
                return FromQueryWithShared(query).Select(ToFile)
                    .Where(
                        f =>
                        bunch
                            ? f.RootFolderType == FolderType.BUNCH
                            : f.RootFolderType == FolderType.USER || f.RootFolderType == FolderType.COMMON)
                    .ToList();
            }
            else
            {
                var query = BuildSearch(GetFileQuery(r => r.CurrentVersion).AsNoTracking(), searchText, SearhTypeEnum.Any);
                return FromQueryWithShared(query).Select(ToFile)
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

        public async Task<bool> IsExistOnStorageAsync(File<int> file)
        {
            return await GlobalStore.GetStore().IsFileAsync(string.Empty, GetUniqFilePath(file));
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
                .Any(r => r.Id == fileId &&
                          r.Version == fileVersion &&
                          r.Changes != null);
        }

        public IEnumerable<(File<int>, SmallShareRecord)> GetFeeds(int tenant, DateTime from, DateTime to)
        {
            var q1 = FilesDbContext.Files
                .Where(r => r.TenantId == tenant)
                .Where(r => r.CurrentVersion)
                .Where(r => r.ModifiedOn >= from && r.ModifiedOn <= to);

            var q2 = FromQuery(q1)
                .Select(r => new DbFileQueryWithSecurity() { DbFileQuery = r, Security = null });

            var q3 = FilesDbContext.Files
                .Where(r => r.TenantId == tenant)
                .Where(r => r.CurrentVersion);

            var q4 = FromQuery(q3)
                .Join(FilesDbContext.Security.DefaultIfEmpty(), r => r.File.Id.ToString(), s => s.EntryId, (f, s) => new DbFileQueryWithSecurity { DbFileQuery = f, Security = s })
                .Where(r => r.Security.TenantId == tenant)
                .Where(r => r.Security.EntryType == FileEntryType.File)
                .Where(r => r.Security.Security == Security.FileShare.Restrict)
                .Where(r => r.Security.TimeStamp >= from && r.Security.TimeStamp <= to);

            return q2.Select(ToFileWithShare).ToList().Union(q4.Select(ToFileWithShare).ToList());
        }

        public IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            var q1 = FilesDbContext.Files
                .Where(r => r.ModifiedOn > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Count() > 0)
                .Select(r => r.Key)
                .ToList();

            var q2 = FilesDbContext.Security
                .Where(r => r.TimeStamp > fromTime)
                .GroupBy(r => r.TenantId)
                .Where(r => r.Count() > 0)
                .Select(r => r.Key)
                .ToList();

            return q1.Union(q2);
        }

        private const string ThumbnailTitle = "thumb";

        public void SaveThumbnail(File<int> file, Stream thumbnail)
        {
            if (file == null) throw new ArgumentNullException("file");

            var toUpdate = FilesDbContext.Files
                .FirstOrDefault(r => r.Id == file.ID && r.Version == file.Version && r.TenantId == TenantID);

            if (toUpdate != null)
            {
                toUpdate.Thumb = thumbnail != null ? Thumbnail.Created : file.ThumbnailStatus;
                FilesDbContext.SaveChanges();
            }

            if (thumbnail == null) return;

            var thumnailName = ThumbnailTitle + "." + Global.ThumbnailExtension;
            GlobalStore.GetStore().Save(string.Empty, GetUniqFilePath(file, thumnailName), thumbnail, thumnailName);
        }

        public Stream GetThumbnail(File<int> file)
        {
            var thumnailName = ThumbnailTitle + "." + Global.ThumbnailExtension;
            var path = GetUniqFilePath(file, thumnailName);
            var storage = GlobalStore.GetStore();
            if (!storage.IsFile(string.Empty, path)) throw new FileNotFoundException();
            return storage.GetReadStream(string.Empty, path);
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
            return from r in dbFiles
                   select new DbFileQuery
                   {
                       File = r,
                       Root = (from f in FilesDbContext.Folders
                               where f.Id ==
                               (from t in FilesDbContext.Tree
                                where t.FolderId == r.FolderId
                                orderby t.Level descending
                                select t.ParentId
                                ).FirstOrDefault()
                               where f.TenantId == r.TenantId
                               select f
                              ).FirstOrDefault(),
                       Shared = (from f in FilesDbContext.Security
                                 where f.EntryType == FileEntryType.File && f.EntryId == r.Id.ToString() && f.TenantId == r.TenantId
                                 select f
                                 ).Any()
                   };
        }

        protected IQueryable<DbFileQuery> FromQuery(IQueryable<DbFile> dbFiles)
        {
            return dbFiles
                .Select(r => new DbFileQuery
                {
                    File = r,
                    Root = (from f in FilesDbContext.Folders
                            where f.Id ==
                            (from t in FilesDbContext.Tree
                             where t.FolderId == r.FolderId
                             orderby t.Level descending
                             select t.ParentId
                             ).FirstOrDefault()
                            where f.TenantId == r.TenantId
                            select f
                              ).FirstOrDefault(),
                    Shared = true
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

        internal protected DbFile InitDocument(DbFile dbFile)
        {
            if (!FactoryIndexer.CanIndexByContent())
            {
                dbFile.Document = new Document
                {
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(""))
                };
                return dbFile;
            }

            var file = ServiceProvider.GetService<File<int>>();
            file.ID = dbFile.Id;
            file.Title = dbFile.Title;
            file.Version = dbFile.Version;
            file.ContentLength = dbFile.ContentLength;

            if (!IsExistOnStorage(file) || file.ContentLength > Settings.MaxContentLength) return dbFile;

            using var stream = GetFileStream(file);
            if (stream == null) return dbFile;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                dbFile.Document = new Document
                {
                    Data = Convert.ToBase64String(ms.GetBuffer())
                };
            }

            return dbFile;
        }

        internal protected async Task<DbFile> InitDocumentAsync(DbFile dbFile)
        {
            if (!FactoryIndexer.CanIndexByContent())
            {
                dbFile.Document = new Document
                {
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(""))
                };
                return dbFile;
            }

            var file = ServiceProvider.GetService<File<int>>();
            file.ID = dbFile.Id;
            file.Title = dbFile.Title;
            file.Version = dbFile.Version;
            file.ContentLength = dbFile.ContentLength;

            if (!await IsExistOnStorageAsync(file) || file.ContentLength > Settings.MaxContentLength) return dbFile;

            using var stream = await GetFileStreamAsync(file);
            if (stream == null) return dbFile;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
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
    }

    public class DbFileQueryWithSecurity
    {
        public DbFileQuery DbFileQuery { get; set; }
        public DbFilesSecurity Security { get; set; }
    }
}