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
    internal class SharpBoxFileDao : SharpBoxDaoBase, IFileDao<string>
    {
        private TempStream TempStream { get; }
        private CrossDao CrossDao { get; }
        private SharpBoxDaoSelector SharpBoxDaoSelector { get; }
        private IFileDao<int> FileDao { get; }

        public SharpBoxFileDao(
            IServiceProvider serviceProvider,
            TempStream tempStream,
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
            TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            TempStream = tempStream;
            CrossDao = crossDao;
            SharpBoxDaoSelector = sharpBoxDaoSelector;
            FileDao = fileDao;
        }

        public void InvalidateCache(string fileId)
        {
            InvalidateCacheAsync(fileId).Wait();
        }

        public Task InvalidateCacheAsync(string fileId)
        {
            ProviderInfo.InvalidateStorage();
            return Task.FromResult(0);
        }

        public File<string> GetFile(string fileId)
        {
            return GetFileAsync(fileId).Result;
        }

        public async Task<File<string>> GetFileAsync(string fileId)
        {
            return await GetFileAsync(fileId, 1).ConfigureAwait(false);
        }

        public File<string> GetFile(string fileId, int fileVersion)
        {
            return GetFileAsync(fileId, fileVersion).Result;
        }

        public Task<File<string>> GetFileAsync(string fileId, int fileVersion)
        {
            return Task.FromResult(ToFile(GetFileById(fileId)));
        }


        public File<string> GetFile(string parentId, string title)
        {
            return GetFileAsync(parentId, title).Result;
        }

        public Task<File<string>> GetFileAsync(string parentId, string title)
        {
            return Task.FromResult(ToFile(GetFolderFiles(parentId).FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase))));
        }

        public File<string> GetFileStable(string fileId, int fileVersion)
        {
            return GetFileStableAsync(fileId, fileVersion).Result;
        }

        public Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
        {
            return Task.FromResult(ToFile(GetFileById(fileId)));
        }

        public List<File<string>> GetFileHistory(string fileId)
        {
            return GetFileHistoryAsync(fileId).Result;
        }

        public async Task<List<File<string>>> GetFileHistoryAsync(string fileId)
        {
            return new List<File<string>> { await GetFileAsync(fileId).ConfigureAwait(false) };
        }

        public List<File<string>> GetFiles(IEnumerable<string> fileIds)
        {
            return GetFilesAsync(fileIds).ToListAsync().Result;
        }

        public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
        {
            return fileIds.Select(fileId => ToFile(GetFileById(fileId))).ToAsyncEnumerable();
        }

        public List<File<string>> GetFilesFiltered(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
        {
            return GetFilesFilteredAsync(fileIds, filterType, subjectGroup, subjectID, searchText, searchInContent).ToListAsync().Result;
        }

        public IAsyncEnumerable<File<string>> GetFilesFilteredAsync(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
        {
            if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly) return AsyncEnumerable.Empty<File<string>>();

            var files = GetFilesAsync(fileIds);

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
                    return AsyncEnumerable.Empty<File<string>>();
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

            return files;
        }

        public List<string> GetFiles(string parentId)
        {
            return GetFilesAsync(parentId).Result;
        }

        public Task<List<string>> GetFilesAsync(string parentId)
        {
            var folder = GetFolderById(parentId).AsEnumerable();

            return Task.FromResult(folder
                    .Where(x => !(x is ICloudDirectoryEntry))
                    .Select(x => MakeId(x)).ToList());        
        }

        public List<File<string>> GetFiles(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            return GetFilesAsync(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).Result;
        }

        public Task<List<File<string>>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return Task.FromResult(new List<File<string>>());

            //Get only files
            var files = GetFolderById(parentId).Where(x => !(x is ICloudDirectoryEntry)).Select(ToFile);

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
                    return Task.FromResult(new List<File<string>>());
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
            return Task.FromResult(files.ToList());
        }

        public Stream GetFileStream(File<string> file, long offset)
        {
            return GetFileStreamAsync(file, offset).Result;
        }

        public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
        {
            var fileToDownload = GetFileById(file.ID);

            if (fileToDownload == null)
                throw new ArgumentNullException("file", FilesCommonResource.ErrorMassage_FileNotFound);
            if (fileToDownload is ErrorEntry errorEntry)
                throw new Exception(errorEntry.Error);

            var fileStream = fileToDownload.GetDataTransferAccessor().GetDownloadStream();

            if (fileStream != null && offset > 0)
            {
                if (!fileStream.CanSeek)
                {
                    var tempBuffer = TempStream.Create();

                    await fileStream.CopyToAsync(tempBuffer).ConfigureAwait(false);
                    await tempBuffer.FlushAsync().ConfigureAwait(false);
                    tempBuffer.Seek(offset, SeekOrigin.Begin);

                    await fileStream.DisposeAsync().ConfigureAwait(false);
                    return tempBuffer;
                }

                fileStream.Seek(offset, SeekOrigin.Begin);
            }

            return fileStream;
        }

        public Uri GetPreSignedUri(File<string> file, TimeSpan expires)
        {
            return GetPreSignedUriAsync(file, expires).Result;
        }

        public Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
        {
            throw new NotSupportedException();
        }

        public bool IsSupportedPreSignedUri(File<string> file)
        {
            return IsSupportedPreSignedUriAsync(file).Result;
        }

        public Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
        {
            return Task.FromResult(false);
        }

        public override Stream GetFileStream(File<string> file)
        {
            return GetFileStreamAsync(file, 0).Result;
        }

        public override async Task<Stream> GetFileStreamAsync(File<string> file)
        {
            return await GetFileStreamAsync(file, 0).ConfigureAwait(false);
        }

        public File<string> SaveFile(File<string> file, Stream fileStream)
        {
            return SaveFileAsync(file, fileStream).Result;
        }

        public Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
        {
            if (fileStream == null) throw new ArgumentNullException("fileStream");
            ICloudFileSystemEntry entry = null;
            if (file.ID != null)
            {
                entry = ProviderInfo.Storage.GetFile(MakePath(file.ID), null);
            }
            else if (file.FolderID != null)
            {
                var folder = GetFolderById(file.FolderID);
                file.Title = GetAvailableTitle(file.Title, folder, IsExist);
                entry = ProviderInfo.Storage.CreateFile(folder, file.Title);
            }

            if (entry == null)
            {
                return null;
            }

            try
            {
                entry.GetDataTransferAccessor().Transfer(TempStream.GetBuffered(fileStream), nTransferDirection.nUpload);
            }
            catch (SharpBoxException e)
            {
                var webException = (WebException)e.InnerException;
                if (webException != null)
                {
                    var response = ((HttpWebResponse)webException.Response);
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

            if (file.ID != null && !entry.Name.Equals(file.Title))
            {
                file.Title = GetAvailableTitle(file.Title, entry.Parent, IsExist);
                ProviderInfo.Storage.RenameFileSystemEntry(entry, file.Title);
            }

            return Task.FromResult(ToFile(entry));
        }

        public File<string> ReplaceFileVersion(File<string> file, Stream fileStream)
        {
            return ReplaceFileVersionAsync(file, fileStream).Result;
        }

        public async Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
        {
            return await SaveFileAsync(file, fileStream).ConfigureAwait(false);
        }

        public void DeleteFile(string fileId)
        {
           DeleteFileAsync(fileId).Wait();
        }

        public async Task DeleteFileAsync(string fileId)
        {
            var file = GetFileById(fileId);
            if (file == null) return;
            var id = MakeId(file);

            using (var tx = FilesDbContext.Database.BeginTransaction())
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

            if (!(file is ErrorEntry))
                ProviderInfo.Storage.DeleteFileSystemEntry(file);
        }

        public bool IsExist(string title, object folderId)
        {
            return IsExistAsync(title, folderId).Result;
        }

        public Task<bool> IsExistAsync(string title, object folderId)
        {
            var folder = ProviderInfo.Storage.GetFolder(MakePath(folderId));
            return Task.FromResult(IsExist(title, folder));
        }

        public bool IsExist(string title, ICloudDirectoryEntry folder)
        {
            return IsExistAsync(title, folder).Result;
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

        public TTo MoveFile<TTo>(string fileId, TTo toFolderId)
        {
            return MoveFileAsync(fileId, toFolderId).Result;
        }

        public async Task<TTo> MoveFileAsync<TTo>(string fileId, TTo toFolderId)
        {
            if (toFolderId is int tId)
            {
                return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tId).ConfigureAwait(false), typeof(TTo));
            }

            if (toFolderId is string tsId)
            {
                return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tsId).ConfigureAwait(false), typeof(TTo));
            }

            throw new NotImplementedException();
        }

        public int MoveFile(string fileId, int toFolderId)
        {
            return MoveFileAsync(fileId, toFolderId).Result;
        }

        public async Task<int> MoveFileAsync(string fileId, int toFolderId)
        {
            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, SharpBoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                true)
                .ConfigureAwait(false);

            return moved.ID;
        }

        public string MoveFile(string fileId, string toFolderId)
        {
            return MoveFileAsync(fileId, toFolderId).Result;
        }

        public async Task<string> MoveFileAsync(string fileId, string toFolderId)
        {
            var entry = GetFileById(fileId);
            var folder = GetFolderById(toFolderId);

            var oldFileId = MakeId(entry);

            if (!ProviderInfo.Storage.MoveFileSystemEntry(entry, folder))
                throw new Exception("Error while moving");

            var newFileId = MakeId(entry);

            await UpdatePathInDBAsync(oldFileId, newFileId).ConfigureAwait(false);

            return newFileId;
        }

        public File<TTo> CopyFile<TTo>(string fileId, TTo toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<TTo>> CopyFileAsync<TTo>(string fileId, TTo toFolderId)
        {
            if (toFolderId is int tId)
            {
                return await CopyFileAsync(fileId, tId).ConfigureAwait(false) as File<TTo>;
            }

            if (toFolderId is string tsId)
            {
                return await CopyFileAsync(fileId, tsId).ConfigureAwait(false) as File<TTo>;
            }

            throw new NotImplementedException();
        }

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
        {
            var file = GetFileById(fileId);
            if (!ProviderInfo.Storage.CopyFileSystemEntry(MakePath(fileId), MakePath(toFolderId)))
                throw new Exception("Error while copying");
            return Task.FromResult(ToFile(GetFolderById(toFolderId).FirstOrDefault(x => x.Name == file.Name)));
        }

        public File<int> CopyFile(string fileId, int toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
        {
            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, SharpBoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false)
                .ConfigureAwait(false);

            return moved;
        }

        public string FileRename(File<string> file, string newTitle)
        {
            return FileRenameAsync(file, newTitle).Result;
        }

        public async Task<string> FileRenameAsync(File<string> file, string newTitle)
        {
            var entry = GetFileById(file.ID);

            if (entry == null)
                throw new ArgumentNullException("file", FilesCommonResource.ErrorMassage_FileNotFound);

            var oldFileId = MakeId(entry);
            var newFileId = oldFileId;

            var folder = GetFolderById(file.FolderID);
            newTitle = await GetAvailableTitleAsync(newTitle, folder, IsExistAsync).ConfigureAwait(false);

            if (ProviderInfo.Storage.RenameFileSystemEntry(entry, newTitle))
            {
                //File data must be already updated by provider
                //We can't search google files by title because root can have multiple folders with the same name
                //var newFile = SharpBoxProviderInfo.Storage.GetFileSystemObject(newTitle, file.Parent);
                newFileId = MakeId(entry);
            }

            await UpdatePathInDBAsync(oldFileId, newFileId).ConfigureAwait(false);

            return newFileId;
        }

        public string UpdateComment(string fileId, int fileVersion, string comment)
        {
            return UpdateCommentAsync(fileId, fileVersion, comment).Result;
        }

        public Task<string> UpdateCommentAsync(string fileId, int fileVersion, string comment)
        {
            return Task.FromResult(string.Empty);
        }

        public void CompleteVersion(string fileId, int fileVersion)
        {
        }

        public Task CompleteVersionAsync(string fileId, int fileVersion)
        {
            return Task.CompletedTask;
        }

        public void ContinueVersion(string fileId, int fileVersion)
        {
            ContinueVersionAsync(fileId, fileVersion).Wait();
        }

        public Task ContinueVersionAsync(string fileId, int fileVersion)
        {
            return Task.FromResult(0);
        }

        public bool UseTrashForRemove(File<string> file)
        {
            return UseTrashForRemoveAsync(file).Result;
        }

        public Task<bool> UseTrashForRemoveAsync(File<string> file)
        {
            return Task.FromResult(false);
        }

        #region chunking

        public ChunkedUploadSession<string> CreateUploadSession(File<string> file, long contentLength)
        {
            return CreateUploadSessionAsync(file, contentLength).Result;
        }

        public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
        {
            if (SetupInfo.ChunkUploadSize > contentLength)
                return Task.FromResult(new ChunkedUploadSession<string>(MakeId(file), contentLength) { UseChunks = false });

            var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

            var isNewFile = false;

            ICloudFileSystemEntry sharpboxFile;
            if (file.ID != null)
            {
                sharpboxFile = GetFileById(file.ID);
            }
            else
            {
                var folder = GetFolderById(file.FolderID);
                sharpboxFile = ProviderInfo.Storage.CreateFile(folder, GetAvailableTitle(file.Title, folder, IsExist));
                isNewFile = true;
            }

            var sharpboxSession = sharpboxFile.GetDataTransferAccessor().CreateResumableSession(contentLength);
            if (sharpboxSession != null)
            {
                uploadSession.Items["SharpboxSession"] = sharpboxSession;
                uploadSession.Items["IsNewFile"] = isNewFile;
            }
            else
            {
                uploadSession.Items["TempPath"] = TempPath.GetTempFileName();
            }

            uploadSession.File = MakeId(uploadSession.File);
            return Task.FromResult(uploadSession);
        }

        public File<string> UploadChunk(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength)
        {
            return UploadChunkAsync(uploadSession, stream, chunkLength).Result;
        }

        public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength)
        {
            if (!uploadSession.UseChunks)
            {
                if (uploadSession.BytesTotal == 0)
                    uploadSession.BytesTotal = chunkLength;

                uploadSession.File = await SaveFileAsync(uploadSession.File, stream).ConfigureAwait(false);
                uploadSession.BytesUploaded = chunkLength;
                return uploadSession.File;
            }

            if (uploadSession.Items.ContainsKey("SharpboxSession"))
            {
                var sharpboxSession =
                    uploadSession.GetItemOrDefault<AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects.ResumableUploadSession>("SharpboxSession");

                var isNewFile = uploadSession.Items.ContainsKey("IsNewFile") && uploadSession.GetItemOrDefault<bool>("IsNewFile");
                var sharpboxFile =
                    isNewFile
                        ? ProviderInfo.Storage.CreateFile(GetFolderById(sharpboxSession.ParentId), sharpboxSession.FileName)
                        : GetFileById(sharpboxSession.FileId);

                sharpboxFile.GetDataTransferAccessor().Transfer(sharpboxSession, stream, chunkLength);
            }
            else
            {
                var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
                using var fs = new FileStream(tempPath, FileMode.Append);
                await stream.CopyToAsync(fs);
            }

            uploadSession.BytesUploaded += chunkLength;

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                uploadSession.File = await FinalizeUploadSessionAsync(uploadSession).ConfigureAwait(false);
            }
            else
            {
                uploadSession.File = MakeId(uploadSession.File);
            }

            return uploadSession.File;
        }

        public File<string> FinalizeUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            return FinalizeUploadSessionAsync(uploadSession).Result;
        }

        public async Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("SharpboxSession"))
            {
                var sharpboxSession =
                    uploadSession.GetItemOrDefault<AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects.ResumableUploadSession>("SharpboxSession");
                return ToFile(GetFileById(sharpboxSession.FileId));
            }

            using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"), FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);
            return await SaveFileAsync(uploadSession.File, fs).ConfigureAwait(false);
        }

        public void AbortUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            AbortUploadSessionAsync(uploadSession).Wait();
        }

        public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("SharpboxSession"))
            {
                var sharpboxSession =
                    uploadSession.GetItemOrDefault<AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects.ResumableUploadSession>("SharpboxSession");

                var isNewFile = uploadSession.Items.ContainsKey("IsNewFile") && uploadSession.GetItemOrDefault<bool>("IsNewFile");
                var sharpboxFile =
                    isNewFile
                        ? ProviderInfo.Storage.CreateFile(GetFolderById(sharpboxSession.ParentId), sharpboxSession.FileName)
                        : GetFileById(sharpboxSession.FileId);
                sharpboxFile.GetDataTransferAccessor().AbortResumableSession(sharpboxSession);
                return Task.FromResult(0);
            }
            else if (uploadSession.Items.ContainsKey("TempPath"))
            {
                File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
                return Task.FromResult(0);
            }
            return Task.FromResult(0);
        }

        private File<string> MakeId(File<string> file)
        {
            if (file.ID != null)
                file.ID = PathPrefix + "-" + file.ID;

            if (file.FolderID != null)
                file.FolderID = PathPrefix + "-" + file.FolderID;

            return file;
        }
        #endregion
    }
}