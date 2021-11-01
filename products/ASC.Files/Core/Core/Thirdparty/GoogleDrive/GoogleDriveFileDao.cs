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

using Microsoft.Extensions.Options;

using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace ASC.Files.Thirdparty.GoogleDrive
{
    [Scope]
    internal class GoogleDriveFileDao : GoogleDriveDaoBase, IFileDao<string>
    {
        private CrossDao CrossDao { get; }
        private GoogleDriveDaoSelector GoogleDriveDaoSelector { get; }
        private IFileDao<int> FileDao { get; }

        public GoogleDriveFileDao(
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
            TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            GoogleDriveDaoSelector = googleDriveDaoSelector;
            FileDao = fileDao;
        }

        public void InvalidateCache(string fileId)
        {
            var driveId = MakeDriveId(fileId);
            ProviderInfo.CacheReset(driveId, true);

            var driveFile = GetDriveEntry(fileId);
            var parentDriveId = GetParentDriveId(driveFile);
            if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId);
        }

        public File<string> GetFile(string fileId)
        {
            return GetFile(fileId, 1);
        }

        public File<string> GetFile(string fileId, int fileVersion)
        {
            return ToFile(GetDriveEntry(fileId));
        }

        public File<string> GetFile(string parentId, string title)
        {
            return ToFile(GetDriveEntries(parentId, false)
                              .FirstOrDefault(file => file.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
        }

        public File<string> GetFileStable(string fileId, int fileVersion)
        {
            return ToFile(GetDriveEntry(fileId));
        }

        public List<File<string>> GetFileHistory(string fileId)
        {
            return new List<File<string>> { GetFile(fileId) };
        }

        public List<File<string>> GetFiles(IEnumerable<string> fileIds)
        {
            if (fileIds == null || !fileIds.Any()) return new List<File<string>>();
            return fileIds.Select(GetDriveEntry).Select(ToFile).ToList();
        }

        public List<File<string>> GetFilesFiltered(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
        {
            if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly) return new List<File<string>>();

            var files = GetFiles(fileIds).AsEnumerable();

            //Filter
            if (subjectID != Guid.Empty)
            {
                files = files.Where(x => subjectGroup
                                             ? UserManager.IsUserInGroup(x.CreateBy, subjectID)
                                             : x.CreateBy == subjectID);
            }

            switch (filterType)
            {
                case FilterType.FoldersOnly:
                    return new List<File<string>>();
                case FilterType.DocumentsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Document);
                    break;
                case FilterType.PresentationsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Presentation);
                    break;
                case FilterType.SpreadsheetsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Spreadsheet);
                    break;
                case FilterType.ImagesOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Image);
                    break;
                case FilterType.ArchiveOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Archive);
                    break;
                case FilterType.MediaOnly:
                    files = files.Where(x =>
                        {
                            FileType fileType;
                            return (fileType = FileUtility.GetFileTypeByFileName(x.Title)) == FileType.Audio || fileType == FileType.Video;
                        });
                    break;
                case FilterType.ByExtension:
                    if (!string.IsNullOrEmpty(searchText))
                        files = files.Where(x => FileUtility.GetFileExtension(x.Title).Contains(searchText));
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            return files.ToList();
        }

        public List<string> GetFiles(string parentId)
        {
            return GetDriveEntries(parentId, false).Select(entry => MakeId(entry.Id)).ToList();
        }

        public List<File<string>> GetFiles(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<string>>();

            //Get only files
            var files = GetDriveEntries(parentId, false).Select(ToFile);

            //Filter
            if (subjectID != Guid.Empty)
            {
                files = files.Where(x => subjectGroup
                                             ? UserManager.IsUserInGroup(x.CreateBy, subjectID)
                                             : x.CreateBy == subjectID);
            }

            switch (filterType)
            {
                case FilterType.FoldersOnly:
                    return new List<File<string>>();
                case FilterType.DocumentsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Document);
                    break;
                case FilterType.PresentationsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Presentation);
                    break;
                case FilterType.SpreadsheetsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Spreadsheet);
                    break;
                case FilterType.ImagesOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Image);
                    break;
                case FilterType.ArchiveOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Archive);
                    break;
                case FilterType.MediaOnly:
                    files = files.Where(x =>
                    {
                        FileType fileType;
                        return (fileType = FileUtility.GetFileTypeByFileName(x.Title)) == FileType.Audio || fileType == FileType.Video;
                    });
                    break;
                case FilterType.ByExtension:
                    if (!string.IsNullOrEmpty(searchText))
                        files = files.Where(x => FileUtility.GetFileExtension(x.Title).Contains(searchText));
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            files = orderBy.SortedBy switch
            {
                SortedByType.Author => orderBy.IsAsc ? files.OrderBy(x => x.CreateBy) : files.OrderByDescending(x => x.CreateBy),
                SortedByType.AZ => orderBy.IsAsc ? files.OrderBy(x => x.Title) : files.OrderByDescending(x => x.Title),
                SortedByType.DateAndTime => orderBy.IsAsc ? files.OrderBy(x => x.ModifiedOn) : files.OrderByDescending(x => x.ModifiedOn),
                SortedByType.DateAndTimeCreation => orderBy.IsAsc ? files.OrderBy(x => x.CreateOn) : files.OrderByDescending(x => x.CreateOn),
                _ => orderBy.IsAsc ? files.OrderBy(x => x.Title) : files.OrderByDescending(x => x.Title),
            };
            return files.ToList();
        }

        public override Stream GetFileStream(File<string> file)
        {
            return GetFileStream(file, 0);
        }

        public Stream GetFileStream(File<string> file, long offset)
        {
            var driveId = MakeDriveId(file.ID);
            ProviderInfo.CacheReset(driveId, true);
            var driveFile = GetDriveEntry(file.ID);
            if (driveFile == null) throw new ArgumentNullException("file", FilesCommonResource.ErrorMassage_FileNotFound);
            if (driveFile is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var fileStream = ProviderInfo.Storage.DownloadStream(driveFile, (int)offset);

            if (!driveFile.Size.HasValue && fileStream != null && fileStream.CanSeek)
            {
                file.ContentLength = fileStream.Length; // hack for google drive
            }

            return fileStream;
        }

        public Uri GetPreSignedUri(File<string> file, TimeSpan expires)
        {
            throw new NotSupportedException();
        }

        public bool IsSupportedPreSignedUri(File<string> file)
        {
            return false;
        }

        public File<string> SaveFile(File<string> file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (fileStream == null) throw new ArgumentNullException("fileStream");

            DriveFile newDriveFile = null;

            if (file.ID != null)
            {
                newDriveFile = ProviderInfo.Storage.SaveStream(MakeDriveId(file.ID), fileStream, file.Title);
            }
            else if (file.FolderID != null)
            {
                newDriveFile = ProviderInfo.Storage.InsertEntry(fileStream, file.Title, MakeDriveId(file.FolderID));
            }

            ProviderInfo.CacheReset(newDriveFile);
            var parentDriveId = GetParentDriveId(newDriveFile);
            if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, false);

            return ToFile(newDriveFile);
        }

        public File<string> ReplaceFileVersion(File<string> file, Stream fileStream)
        {
            return SaveFile(file, fileStream);
        }

        public void DeleteFile(string fileId)
        {
            var driveFile = GetDriveEntry(fileId);
            if (driveFile == null) return;
            var id = MakeId(driveFile.Id);

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

            if (!(driveFile is ErrorDriveEntry))
            {
                ProviderInfo.Storage.DeleteEntry(driveFile.Id);
            }

            ProviderInfo.CacheReset(driveFile.Id);
            var parentDriveId = GetParentDriveId(driveFile);
            if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, false);
        }

        public bool IsExist(string title, object folderId)
        {
            return GetDriveEntries(folderId, false)
                .Any(file => file.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public TTo MoveFile<TTo>(string fileId, TTo toFolderId)
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

        public int MoveFile(string fileId, int toFolderId)
        {
            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, GoogleDriveDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                true);

            return moved.ID;
        }

        public string MoveFile(string fileId, string toFolderId)
        {
            var driveFile = GetDriveEntry(fileId);
            if (driveFile is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var toDriveFolder = GetDriveEntry(toFolderId);
            if (toDriveFolder is ErrorDriveEntry errorDriveEntry1) throw new Exception(errorDriveEntry1.Error);

            var fromFolderDriveId = GetParentDriveId(driveFile);

            driveFile = ProviderInfo.Storage.InsertEntryIntoFolder(driveFile, toDriveFolder.Id);
            if (fromFolderDriveId != null)
            {
                ProviderInfo.Storage.RemoveEntryFromFolder(driveFile, fromFolderDriveId);
            }

            ProviderInfo.CacheReset(driveFile.Id);
            ProviderInfo.CacheReset(fromFolderDriveId, false);
            ProviderInfo.CacheReset(toDriveFolder.Id, false);

            return MakeId(driveFile.Id);
        }

        public File<TTo> CopyFile<TTo>(string fileId, TTo toFolderId)
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

        public File<int> CopyFile(string fileId, int toFolderId)
        {
            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, GoogleDriveDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false);

            return moved;
        }

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            var driveFile = GetDriveEntry(fileId);
            if (driveFile is ErrorDriveEntry errorDriveEntry) throw new Exception(errorDriveEntry.Error);

            var toDriveFolder = GetDriveEntry(toFolderId);
            if (toDriveFolder is ErrorDriveEntry errorDriveEntry1) throw new Exception(errorDriveEntry1.Error);

            var newDriveFile = ProviderInfo.Storage.CopyEntry(toDriveFolder.Id, driveFile.Id);

            ProviderInfo.CacheReset(newDriveFile);
            ProviderInfo.CacheReset(toDriveFolder.Id, false);

            return ToFile(newDriveFile);
        }

        public string FileRename(File<string> file, string newTitle)
        {
            var driveFile = GetDriveEntry(file.ID);
            driveFile.Name = newTitle;

            driveFile = ProviderInfo.Storage.RenameEntry(driveFile.Id, driveFile.Name);

            ProviderInfo.CacheReset(driveFile);
            var parentDriveId = GetParentDriveId(driveFile);
            if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, false);

            return MakeId(driveFile.Id);
        }

        public string UpdateComment(string fileId, int fileVersion, string comment)
        {
            return string.Empty;
        }

        public void CompleteVersion(string fileId, int fileVersion)
        {
        }

        public void ContinueVersion(string fileId, int fileVersion)
        {
        }

        public bool UseTrashForRemove(File<string> file)
        {
            return false;
        }

        #region chunking

        private File<string> RestoreIds(File<string> file)
        {
            if (file == null) return null;

            if (file.ID != null)
                file.ID = MakeId(file.ID);

            if (file.FolderID != null)
                file.FolderID = MakeId(file.FolderID);

            return file;
        }

        public ChunkedUploadSession<string> CreateUploadSession(File<string> file, long contentLength)
        {
            if (SetupInfo.ChunkUploadSize > contentLength)
                return new ChunkedUploadSession<string>(RestoreIds(file), contentLength) { UseChunks = false };

            var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

            DriveFile driveFile;
            if (file.ID != null)
            {
                driveFile = GetDriveEntry(file.ID);
            }
            else
            {
                var folder = GetDriveEntry(file.FolderID);
                driveFile = ProviderInfo.Storage.FileConstructor(file.Title, null, folder.Id);
            }

            var googleDriveSession = ProviderInfo.Storage.CreateResumableSession(driveFile, contentLength);
            if (googleDriveSession != null)
            {
                uploadSession.Items["GoogleDriveSession"] = googleDriveSession;
            }
            else
            {
                uploadSession.Items["TempPath"] = TempPath.GetTempFileName();
            }

            uploadSession.File = RestoreIds(uploadSession.File);
            return uploadSession;
        }

        public File<string> UploadChunk(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength)
        {
            if (!uploadSession.UseChunks)
            {
                if (uploadSession.BytesTotal == 0)
                    uploadSession.BytesTotal = chunkLength;

                uploadSession.File = SaveFile(uploadSession.File, stream);
                uploadSession.BytesUploaded = chunkLength;
                return uploadSession.File;
            }

            if (uploadSession.Items.ContainsKey("GoogleDriveSession"))
            {
                var googleDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("GoogleDriveSession");
                ProviderInfo.Storage.Transfer(googleDriveSession, stream, chunkLength);
            }
            else
            {
                var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
                using var fs = new FileStream(tempPath, FileMode.Append);
                stream.CopyTo(fs);
            }

            uploadSession.BytesUploaded += chunkLength;

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                uploadSession.File = FinalizeUploadSession(uploadSession);
            }
            else
            {
                uploadSession.File = RestoreIds(uploadSession.File);
            }

            return uploadSession.File;
        }

        public File<string> FinalizeUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("GoogleDriveSession"))
            {
                var googleDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("GoogleDriveSession");

                ProviderInfo.CacheReset(googleDriveSession.FileId);
                var parentDriveId = googleDriveSession.FolderId;
                if (parentDriveId != null) ProviderInfo.CacheReset(parentDriveId, false);

                return ToFile(GetDriveEntry(googleDriveSession.FileId));
            }

            using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"), FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);
            return SaveFile(uploadSession.File, fs);
        }

        public void AbortUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("GoogleDriveSession"))
            {
                var googleDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("GoogleDriveSession");

                if (googleDriveSession.Status != ResumableUploadSessionStatus.Completed)
                {
                    googleDriveSession.Status = ResumableUploadSessionStatus.Aborted;
                }
            }
            else if (uploadSession.Items.ContainsKey("TempPath"))
            {
                File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
            }
        }

        #endregion
    }
}