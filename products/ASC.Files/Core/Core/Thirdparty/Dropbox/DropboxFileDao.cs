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

using Dropbox.Api.Files;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Dropbox
{
    [Scope]
    internal class DropboxFileDao : DropboxDaoBase, IFileDao<string>
    {
        private CrossDao CrossDao { get; }
        private DropboxDaoSelector DropboxDaoSelector { get; }
        private IFileDao<int> FileDao { get; }

        public DropboxFileDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            CrossDao crossDao,
            DropboxDaoSelector dropboxDaoSelector,
            IFileDao<int> fileDao,
            TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            DropboxDaoSelector = dropboxDaoSelector;
            FileDao = fileDao;
        }

        public void InvalidateCache(string fileId)
        {
            var dropboxFilePath = MakeDropboxPath(fileId);
            ProviderInfo.CacheReset(dropboxFilePath, true);

            var dropboxFile = GetDropboxFile(fileId);
            var parentPath = GetParentFolderPath(dropboxFile);
            if (parentPath != null) ProviderInfo.CacheReset(parentPath);
        }

        public File<string> GetFile(string fileId)
        {
            return GetFile(fileId, 1);
        }

        public File<string> GetFile(string fileId, int fileVersion)
        {
            return ToFile(GetDropboxFile(fileId));
        }

        public File<string> GetFile(string parentId, string title)
        {
            var metadata = GetDropboxItems(parentId, false)
                .FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
            return metadata == null
                       ? null
                       : ToFile(metadata.AsFile);
        }

        public File<string> GetFileStable(string fileId, int fileVersion = -1)
        {
            return ToFile(GetDropboxFile(fileId));
        }

        public List<File<string>> GetFileHistory(string fileId)
        {
            return new List<File<string>> { GetFile(fileId) };
        }

        public List<File<string>> GetFiles(IEnumerable<string> fileIds)
        {
            if (fileIds == null || !fileIds.Any()) return new List<File<string>>();
            return fileIds.Select(GetDropboxFile).Select(ToFile).ToList();
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
                            FileType fileType = FileUtility.GetFileTypeByFileName(x.Title);
                            return fileType == FileType.Audio || fileType == FileType.Video;
                        });
                    break;
                case FilterType.ByExtension:
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        searchText = searchText.Trim().ToLower();
                        files = files.Where(x => FileUtility.GetFileExtension(x.Title).Equals(searchText));
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            return files.ToList();
        }

        public List<string> GetFiles(string parentId)
        {
            return GetDropboxItems(parentId, false).Select(entry => MakeId(entry)).ToList();
        }

        public List<File<string>> GetFiles(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<string>>();

            //Get only files
            var files = GetDropboxItems(parentId, false).Select(item => ToFile(item.AsFile));

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
                            FileType fileType = FileUtility.GetFileTypeByFileName(x.Title);
                            return fileType == FileType.Audio || fileType == FileType.Video;
                        });
                    break;
                case FilterType.ByExtension:
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        searchText = searchText.Trim().ToLower();
                        files = files.Where(x => FileUtility.GetFileExtension(x.Title).Equals(searchText));
                    }
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
            var dropboxFilePath = MakeDropboxPath(file.ID);
            ProviderInfo.CacheReset(dropboxFilePath, true);

            var dropboxFile = GetDropboxFile(file.ID);
            if (dropboxFile == null) throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            if (dropboxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var fileStream = ProviderInfo.Storage.DownloadStream(MakeDropboxPath(dropboxFile), (int)offset);

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
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

            FileMetadata newDropboxFile = null;

            if (file.ID != null)
            {
                var filePath = MakeDropboxPath(file.ID);
                newDropboxFile = ProviderInfo.Storage.SaveStream(filePath, fileStream);
                if (!newDropboxFile.Name.Equals(file.Title))
                {
                    var parentFolderPath = GetParentFolderPath(newDropboxFile);
                    file.Title = GetAvailableTitle(file.Title, parentFolderPath, IsExist);
                    newDropboxFile = ProviderInfo.Storage.MoveFile(filePath, parentFolderPath, file.Title);
                }
            }
            else if (file.FolderID != null)
            {
                var folderPath = MakeDropboxPath(file.FolderID);
                file.Title = GetAvailableTitle(file.Title, folderPath, IsExist);
                newDropboxFile = ProviderInfo.Storage.CreateFile(fileStream, file.Title, folderPath);
            }

            ProviderInfo.CacheReset(newDropboxFile);
            var parentPath = GetParentFolderPath(newDropboxFile);
            if (parentPath != null) ProviderInfo.CacheReset(parentPath);

            return ToFile(newDropboxFile);
        }

        public File<string> ReplaceFileVersion(File<string> file, Stream fileStream)
        {
            return SaveFile(file, fileStream);
        }

        public void DeleteFile(string fileId)
        {
            var dropboxFile = GetDropboxFile(fileId);
            if (dropboxFile == null) return;
            var id = MakeId(dropboxFile);

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

            if (!(dropboxFile is ErrorFile))
            {
                ProviderInfo.Storage.DeleteItem(dropboxFile);
            }

            ProviderInfo.CacheReset(MakeDropboxPath(dropboxFile), true);
            var parentFolderPath = GetParentFolderPath(dropboxFile);
            if (parentFolderPath != null) ProviderInfo.CacheReset(parentFolderPath);
        }

        public bool IsExist(string title, object folderId)
        {
            return GetDropboxItems(folderId, false)
                .Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
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

        public string MoveFile(string fileId, string toFolderId)
        {
            var dropboxFile = GetDropboxFile(fileId);
            if (dropboxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toDropboxFolder = GetDropboxFolder(toFolderId);
            if (toDropboxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var fromFolderPath = GetParentFolderPath(dropboxFile);

            dropboxFile = ProviderInfo.Storage.MoveFile(MakeDropboxPath(dropboxFile), MakeDropboxPath(toDropboxFolder), dropboxFile.Name);

            ProviderInfo.CacheReset(MakeDropboxPath(dropboxFile), true);
            ProviderInfo.CacheReset(fromFolderPath);
            ProviderInfo.CacheReset(MakeDropboxPath(toDropboxFolder));

            return MakeId(dropboxFile);
        }

        public int MoveFile(string fileId, int toFolderId)
        {
            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, DropboxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                true);

            return moved.ID;
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
                fileId, this, DropboxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false);

            return moved;
        }

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            var dropboxFile = GetDropboxFile(fileId);
            if (dropboxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toDropboxFolder = GetDropboxFolder(toFolderId);
            if (toDropboxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var newDropboxFile = ProviderInfo.Storage.CopyFile(MakeDropboxPath(dropboxFile), MakeDropboxPath(toDropboxFolder), dropboxFile.Name);

            ProviderInfo.CacheReset(newDropboxFile);
            ProviderInfo.CacheReset(MakeDropboxPath(toDropboxFolder));

            return ToFile(newDropboxFile);
        }

        public string FileRename(File<string> file, string newTitle)
        {
            var dropboxFile = GetDropboxFile(file.ID);
            var parentFolderPath = GetParentFolderPath(dropboxFile);
            newTitle = GetAvailableTitle(newTitle, parentFolderPath, IsExist);

            dropboxFile = ProviderInfo.Storage.MoveFile(MakeDropboxPath(dropboxFile), parentFolderPath, newTitle);

            ProviderInfo.CacheReset(dropboxFile);
            var parentPath = GetParentFolderPath(dropboxFile);
            if (parentPath != null) ProviderInfo.CacheReset(parentPath);

            return MakeId(dropboxFile);
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

            var dropboxSession = ProviderInfo.Storage.CreateResumableSession();
            if (dropboxSession != null)
            {
                uploadSession.Items["DropboxSession"] = dropboxSession;
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

            if (uploadSession.Items.ContainsKey("DropboxSession"))
            {
                var dropboxSession = uploadSession.GetItemOrDefault<string>("DropboxSession");
                ProviderInfo.Storage.Transfer(dropboxSession, uploadSession.BytesUploaded, stream);
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
            if (uploadSession.Items.ContainsKey("DropboxSession"))
            {
                var dropboxSession = uploadSession.GetItemOrDefault<string>("DropboxSession");

                Metadata dropboxFile;
                var file = uploadSession.File;
                if (file.ID != null)
                {
                    var dropboxFilePath = MakeDropboxPath(file.ID);
                    dropboxFile = ProviderInfo.Storage.FinishResumableSession(dropboxSession, dropboxFilePath, uploadSession.BytesUploaded);
                }
                else
                {
                    var folderPath = MakeDropboxPath(file.FolderID);
                    var title = GetAvailableTitle(file.Title, folderPath, IsExist);
                    dropboxFile = ProviderInfo.Storage.FinishResumableSession(dropboxSession, folderPath, title, uploadSession.BytesUploaded);
                }

                ProviderInfo.CacheReset(MakeDropboxPath(dropboxFile));
                ProviderInfo.CacheReset(GetParentFolderPath(dropboxFile), false);

                return ToFile(dropboxFile.AsFile);
            }

            using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"),
                                           FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);
            return SaveFile(uploadSession.File, fs);
        }

        public void AbortUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("TempPath"))
            {
                File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
            }
        }

        #endregion
    }
}