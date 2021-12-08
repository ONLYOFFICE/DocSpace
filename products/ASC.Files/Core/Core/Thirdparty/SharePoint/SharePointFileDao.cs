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
using System.Threading.Tasks;

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

namespace ASC.Files.Thirdparty.SharePoint
{
    [Scope]
    internal class SharePointFileDao : SharePointDaoBase, IFileDao<string>
    {
        private CrossDao CrossDao { get; }  
        private SharePointDaoSelector SharePointDaoSelector { get; }
        private IFileDao<int> FileDao { get; }

        public SharePointFileDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            CrossDao crossDao,
            SharePointDaoSelector sharePointDaoSelector,
            IFileDao<int> fileDao,
            TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
            CrossDao = crossDao;
            SharePointDaoSelector = sharePointDaoSelector;
            FileDao = fileDao;
        }

        public void InvalidateCache(string fileId)
        {
            InvalidateCacheAsync(fileId).Wait();
        }

        public async Task InvalidateCacheAsync(string fileId)
        {
            await ProviderInfo.InvalidateStorageAsync().ConfigureAwait(false);
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

        public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
        {
            return ProviderInfo.ToFile(await ProviderInfo.GetFileByIdAsync(fileId).ConfigureAwait(false));
        }

        public File<string> GetFile(string parentId, string title)
        {
            return GetFileAsync(parentId, title).Result;
        }

        public async Task<File<string>> GetFileAsync(string parentId, string title)
        {
            var files = await ProviderInfo.GetFolderFilesAsync(parentId).ConfigureAwait(false);
            return ProviderInfo.ToFile(files.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
        }

        public File<string> GetFileStable(string fileId, int fileVersion)
        {
            return GetFileStableAsync(fileId, fileVersion).Result;
        }

        public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
        {
            return ProviderInfo.ToFile(await ProviderInfo.GetFileByIdAsync(fileId).ConfigureAwait(false));
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
            var list = new List<File<string>>();

            if (fileIds == null || !fileIds.Any()) return AsyncEnumerable.Empty<File<string>>();

            var result = fileIds.ToAsyncEnumerable().SelectAwait(async e => ProviderInfo.ToFile(await ProviderInfo.GetFileByIdAsync(fileIds).ConfigureAwait(false)));

            return result;
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

        public async Task<List<string>> GetFilesAsync(string parentId)
        {
            var files = await ProviderInfo.GetFolderFilesAsync(parentId).ConfigureAwait(false);
            return files.Select(r => ProviderInfo.ToFile(r).ID).ToList();
        }

        public List<File<string>> GetFiles(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            return GetFilesAsync(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).Result;
        }

        public async Task<List<File<string>>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<string>>();

            //Get only files
            var folderFiles = await ProviderInfo.GetFolderFilesAsync(parentId).ConfigureAwait(false);
            var files = folderFiles.Select(r => ProviderInfo.ToFile(r));

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
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Document).ToList();
                    break;
                case FilterType.PresentationsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Presentation).ToList();
                    break;
                case FilterType.SpreadsheetsOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Spreadsheet).ToList();
                    break;
                case FilterType.ImagesOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Image).ToList();
                    break;
                case FilterType.ArchiveOnly:
                    files = files.Where(x => FileUtility.GetFileTypeByFileName(x.Title) == FileType.Archive).ToList();
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
                files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1).ToList();

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
            return GetFileStreamAsync(file).Result;
        }

        public override async Task<Stream> GetFileStreamAsync(File<string> file)
        {
            return await  GetFileStreamAsync(file, 0).ConfigureAwait(false);
        }

        public Stream GetFileStream(File<string> file, long offset)
        {
            return GetFileStreamAsync(file, offset).Result;
        }

        public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
        {
            var fileToDownload = await ProviderInfo.GetFileByIdAsync(file.ID).ConfigureAwait(false);
            if (fileToDownload == null)
                throw new ArgumentNullException("file", FilesCommonResource.ErrorMassage_FileNotFound);

            var fileStream = await ProviderInfo.GetFileStreamAsync(fileToDownload.ServerRelativeUrl, (int)offset).ConfigureAwait(false);

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

        public File<string> SaveFile(File<string> file, Stream fileStream)
        {
            return SaveFileAsync(file, fileStream).Result;
        }

        public async Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
        {
            if (fileStream == null) throw new ArgumentNullException("fileStream");

            if (file.ID != null)
            {
                var sharePointFile = await ProviderInfo.CreateFileAsync(file.ID, fileStream).ConfigureAwait(false);

                var resultFile = ProviderInfo.ToFile(sharePointFile);
                if (!sharePointFile.Name.Equals(file.Title))
                {
                    var folder = await ProviderInfo.GetFolderByIdAsync(file.FolderID).ConfigureAwait(false);
                    file.Title = GetAvailableTitle(file.Title, folder, IsExist);

                    var id = await ProviderInfo.RenameFileAsync(DaoSelector.ConvertId(resultFile.ID).ToString(), file.Title).ConfigureAwait(false);
                    return await GetFileAsync(DaoSelector.ConvertId(id)).ConfigureAwait(false);
                }
                return resultFile;
            }

            if (file.FolderID != null)
            {
                var folder = await ProviderInfo.GetFolderByIdAsync(file.FolderID).ConfigureAwait(false);
                file.Title = GetAvailableTitle(file.Title, folder, IsExist);
                return ProviderInfo.ToFile(await ProviderInfo.CreateFileAsync(folder.ServerRelativeUrl + "/" + file.Title, fileStream).ConfigureAwait(false));
            }

            return null;
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
            ProviderInfo.DeleteFileAsync(fileId).Wait();
        }

        public async Task DeleteFileAsync(string fileId)
        {
            await ProviderInfo.DeleteFileAsync(fileId).ConfigureAwait(false);
        }

        public bool IsExist(string title, object folderId)
        {
            return IsExistAsync(title, folderId).Result;
        }

        public async Task<bool> IsExistAsync(string title, object folderId)
        {
            var files = await ProviderInfo.GetFolderFilesAsync(folderId).ConfigureAwait(false);
            return files.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsExist(string title, Microsoft.SharePoint.Client.Folder folder)
        {
            return IsExistAsync(title, folder).Result;
        }

        public async Task<bool> IsExistAsync(string title, Microsoft.SharePoint.Client.Folder folder)
        {
            var files = await ProviderInfo.GetFolderFilesAsync(folder.ServerRelativeUrl).ConfigureAwait(false);
            return files.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
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
                fileId, this, SharePointDaoSelector.ConvertId,
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
            var newFileId = await ProviderInfo.MoveFileAsync(fileId, toFolderId).ConfigureAwait(false);
            await UpdatePathInDBAsync(ProviderInfo.MakeId(fileId), newFileId).ConfigureAwait(false);
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

        public File<int> CopyFile(string fileId, int toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
        {
            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, SharePointDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false)
                .ConfigureAwait(false);

            return moved;
        }

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
        {
            return ProviderInfo.ToFile(await ProviderInfo.CopyFileAsync(fileId, toFolderId).ConfigureAwait(false));
        }

        public string FileRename(File<string> file, string newTitle)
        {
            return FileRenameAsync(file, newTitle).Result;
        }

        public async Task<string> FileRenameAsync(File<string> file, string newTitle)
        {
            var newFileId = await ProviderInfo.RenameFileAsync(file.ID, newTitle).ConfigureAwait(false);
            await UpdatePathInDBAsync(ProviderInfo.MakeId(file.ID), newFileId).ConfigureAwait(false);
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
            return false;
        }

        public ChunkedUploadSession<string> CreateUploadSession(File<string> file, long contentLength)
        {
            return CreateUploadSessionAsync(file, contentLength).Result;
        }

        public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
        {
            return Task.FromResult(new ChunkedUploadSession<string>(FixId(file), contentLength) { UseChunks = false });
        }

        public File<string> UploadChunk(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
        {
            return UploadChunkAsync(uploadSession, chunkStream, chunkLength).Result;
        }

        public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
        {
            if (!uploadSession.UseChunks)
            {
                if (uploadSession.BytesTotal == 0)
                    uploadSession.BytesTotal = chunkLength;

                uploadSession.File = await SaveFileAsync(uploadSession.File, chunkStream).ConfigureAwait(false);
                uploadSession.BytesUploaded = chunkLength;
                return uploadSession.File;
            }

            throw new NotImplementedException();
        }

        public void AbortUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            AbortUploadSessionAsync(uploadSession).Wait();
            //throw new NotImplementedException();
        }

        public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
        {
            return Task.FromResult(0);
            //throw new NotImplementedException();
        }

        private File<string> FixId(File<string> file)
        {
            if (file.ID != null)
                file.ID = ProviderInfo.MakeId(file.ID);

            if (file.FolderID != null)
                file.FolderID = ProviderInfo.MakeId(file.FolderID);

            return file;
        }
    }
}
