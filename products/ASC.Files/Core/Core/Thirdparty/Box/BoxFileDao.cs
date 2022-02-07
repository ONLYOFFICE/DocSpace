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

using Box.V2.Models;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Box
{
    [Scope]
    internal class BoxFileDao : BoxDaoBase, IFileDao<string>
    {
        private CrossDao CrossDao { get; }
        private BoxDaoSelector BoxDaoSelector { get; }
        private IFileDao<int> FileDao { get; }

        public BoxFileDao(
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
            TempPath tempPath) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            BoxDaoSelector = boxDaoSelector;
            FileDao = fileDao;
        }

        public void InvalidateCache(string fileId)
        {
            var boxFileId = MakeBoxId(fileId);
            ProviderInfo.CacheReset(boxFileId, true);

            var boxFile = GetBoxFile(fileId);
            var parentPath = GetParentFolderId(boxFile);
            if (parentPath != null) ProviderInfo.CacheReset(parentPath);
        }

        public File<string> GetFile(string fileId)
        {
            return GetFile(fileId, 1);
        }

        public File<string> GetFile(string fileId, int fileVersion)
        {
            return ToFile(GetBoxFile(fileId));
        }

        public File<string> GetFile(string parentId, string title)
        {
            return ToFile(GetBoxItems(parentId, false)
                              .FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)) as BoxFile);
        }

        public File<string> GetFileStable(string fileId, int fileVersion = -1)
        {
            return ToFile(GetBoxFile(fileId));
        }

        public List<File<string>> GetFileHistory(string fileId)
        {
            return new List<File<string>> { GetFile(fileId) };
        }

        public List<File<string>> GetFiles(IEnumerable<string> fileIds)
        {
            if (fileIds == null || !fileIds.Any()) return new List<File<string>>();
            return fileIds.Select(GetBoxFile).Select(ToFile).ToList();
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
            return GetBoxItems(parentId, false).Select(entry => MakeId(entry.Id)).ToList();
        }

        public List<File<string>> GetFiles(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<string>>();

            //Get only files
            var files = GetBoxItems(parentId, false).Select(item => ToFile(item as BoxFile));

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
            var boxFileId = MakeBoxId(file.ID);
            ProviderInfo.CacheReset(boxFileId, true);

            var boxFile = GetBoxFile(file.ID);
            if (boxFile == null) throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var fileStream = ProviderInfo.Storage.DownloadStream(boxFile, (int)offset);

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

            BoxFile newBoxFile = null;

            if (file.ID != null)
            {
                var fileId = MakeBoxId(file.ID);
                newBoxFile = ProviderInfo.Storage.SaveStream(fileId, fileStream);

                if (!newBoxFile.Name.Equals(file.Title))
                {
                    var folderId = GetParentFolderId(GetBoxFile(fileId));
                    file.Title = GetAvailableTitle(file.Title, folderId, IsExist);
                    newBoxFile = ProviderInfo.Storage.RenameFile(fileId, file.Title);
                }
            }
            else if (file.FolderID != null)
            {
                var folderId = MakeBoxId(file.FolderID);
                file.Title = GetAvailableTitle(file.Title, folderId, IsExist);
                newBoxFile = ProviderInfo.Storage.CreateFile(fileStream, file.Title, folderId);
            }

            ProviderInfo.CacheReset(newBoxFile);
            var parentId = GetParentFolderId(newBoxFile);
            if (parentId != null) ProviderInfo.CacheReset(parentId);

            return ToFile(newBoxFile);
        }

        public File<string> ReplaceFileVersion(File<string> file, Stream fileStream)
        {
            return SaveFile(file, fileStream);
        }

        public void DeleteFile(string fileId)
        {
            var boxFile = GetBoxFile(fileId);
            if (boxFile == null) return;
            var id = MakeId(boxFile.Id);

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

            if (!(boxFile is ErrorFile))
            {
                ProviderInfo.Storage.DeleteItem(boxFile);
            }

            ProviderInfo.CacheReset(boxFile.Id, true);
            var parentFolderId = GetParentFolderId(boxFile);
            if (parentFolderId != null) ProviderInfo.CacheReset(parentFolderId);
        }

        public bool IsExist(string title, object folderId)
        {
            return GetBoxItems(folderId.ToString(), false)
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

        public int MoveFile(string fileId, int toFolderId)
        {
            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, BoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                true);

            return moved.ID;
        }

        public string MoveFile(string fileId, string toFolderId)
        {
            var boxFile = GetBoxFile(fileId);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toBoxFolder = GetBoxFolder(toFolderId);
            if (toBoxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var fromFolderId = GetParentFolderId(boxFile);

            var newTitle = GetAvailableTitle(boxFile.Name, toBoxFolder.Id, IsExist);
            boxFile = ProviderInfo.Storage.MoveFile(boxFile.Id, newTitle, toBoxFolder.Id);

            ProviderInfo.CacheReset(boxFile.Id, true);
            ProviderInfo.CacheReset(fromFolderId);
            ProviderInfo.CacheReset(toBoxFolder.Id);

            return MakeId(boxFile.Id);
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

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            var boxFile = GetBoxFile(fileId);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toBoxFolder = GetBoxFolder(toFolderId);
            if (toBoxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var newTitle = GetAvailableTitle(boxFile.Name, toBoxFolder.Id, IsExist);
            var newBoxFile = ProviderInfo.Storage.CopyFile(boxFile.Id, newTitle, toBoxFolder.Id);

            ProviderInfo.CacheReset(newBoxFile);
            ProviderInfo.CacheReset(toBoxFolder.Id);

            return ToFile(newBoxFile);
        }

        public File<int> CopyFile(string fileId, int toFolderId)
        {
            var moved = CrossDao.PerformCrossDaoFileCopy(
                fileId, this, BoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false);

            return moved;
        }

        public string FileRename(File<string> file, string newTitle)
        {
            var boxFile = GetBoxFile(file.ID);
            newTitle = GetAvailableTitle(newTitle, GetParentFolderId(boxFile), IsExist);

            boxFile = ProviderInfo.Storage.RenameFile(boxFile.Id, newTitle);

            ProviderInfo.CacheReset(boxFile);
            var parentId = GetParentFolderId(boxFile);
            if (parentId != null) ProviderInfo.CacheReset(parentId);

            return MakeId(boxFile.Id);
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

            uploadSession.Items["TempPath"] = TempPath.GetTempFileName();

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

            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            using (var fs = new FileStream(tempPath, FileMode.Append))
            {
                stream.CopyTo(fs);
            }

            uploadSession.BytesUploaded += chunkLength;

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"),
                                               FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);
                uploadSession.File = SaveFile(uploadSession.File, fs);
            }
            else
            {
                uploadSession.File = RestoreIds(uploadSession.File);
            }

            return uploadSession.File;
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