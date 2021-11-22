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

using Box.V2.Models;

using Microsoft.EntityFrameworkCore;
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
            InvalidateCacheAsync(fileId).Wait();
        }

        public async Task InvalidateCacheAsync(string fileId)
        {
            var boxFileId = MakeBoxId(fileId);
            await ProviderInfo.CacheResetAsync(boxFileId, true);

            var boxFile = await GetBoxFileAsync(fileId);
            var parentPath = GetParentFolderId(boxFile);
            if (parentPath != null) await ProviderInfo.CacheResetAsync(parentPath);
        }

        public File<string> GetFile(string fileId)
        {
            return GetFileAsync(fileId).Result;
        }

        public async Task<File<string>> GetFileAsync(string fileId)
        {
            return await GetFileAsync(fileId, 1);
        }

        public File<string> GetFile(string fileId, int fileVersion)
        {
            return GetFileAsync(fileId, fileVersion).Result;
        }

        public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
        {
            return ToFile(await GetBoxFileAsync(fileId));
        }

        public File<string> GetFile(string parentId, string title)
        {
            return GetFileAsync(parentId, title).Result;
        }

        public async Task<File<string>> GetFileAsync(string parentId, string title)
        {
            var items = await GetBoxItemsAsync(parentId, false);
            return ToFile(items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)) as BoxFile);
        }

        public File<string> GetFileStable(string fileId, int fileVersion)
        {
            return GetFileStableAsync(fileId, fileVersion).Result;
        }

        public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion)
        {
            return ToFile(await GetBoxFileAsync(fileId));
        }

        public List<File<string>> GetFileHistory(string fileId)
        {
            return GetFileHistoryAsync(fileId).Result;
        }

        public async Task<List<File<string>>> GetFileHistoryAsync(string fileId)
        {
            return new List<File<string>> { await GetFileAsync(fileId) };
        }


        public List<File<string>> GetFiles(IEnumerable<string> fileIds)
        {
            //if (fileIds == null || !fileIds.Any()) return new List<File<string>>();
            //return fileIds.Select(GetBoxFile).Select(ToFile).ToList();

            return GetFilesAsync(fileIds).ToListAsync().Result;
        }

        public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
        {
            var list = new List<File<string>>();

            if (fileIds == null || !fileIds.Any()) return AsyncEnumerable.Empty<File<string>>();

            var result = fileIds.ToAsyncEnumerable().SelectAwait(async e => ToFile(await GetBoxFileAsync(e)));

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
            return GetBoxItems(parentId, false).Select(entry => MakeId(entry.Id)).ToList();
        }

        public async Task<List<string>> GetFilesAsync(string parentId)
        {
            var items = await GetBoxItemsAsync(parentId, false);
            return items.Select(entry => MakeId(entry.Id)).ToList();
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

        public async Task<List<File<string>>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) return new List<File<string>>();

            //Get only files
            var filesWait = await GetBoxItemsAsync(parentId, false);
            var files = filesWait.Select(item => ToFile(item as BoxFile));

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
            return GetFileStreamAsync(file).Result;
        }

        public override async Task<Stream> GetFileStreamAsync(File<string> file)
        {
            return await GetFileStreamAsync(file, 0);
        }

        public Stream GetFileStream(File<string> file, long offset)
        {
            return GetFileStreamAsync(file, offset).Result;
        }

        public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
        {
            var boxFileId = MakeBoxId(file.ID);
            await ProviderInfo.CacheResetAsync(boxFileId, true);

            var boxFile = await GetBoxFileAsync(file.ID);
            if (boxFile == null) throw new ArgumentNullException("file", FilesCommonResource.ErrorMassage_FileNotFound);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var fileStream = await ProviderInfo.Storage.DownloadStreamAsync(boxFile, (int)offset);

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
            if (file == null) throw new ArgumentNullException("file");
            if (fileStream == null) throw new ArgumentNullException("fileStream");

            BoxFile newBoxFile = null;

            if (file.ID != null)
            {
                var fileId = MakeBoxId(file.ID);
                newBoxFile = await ProviderInfo.Storage.SaveStreamAsync(fileId, fileStream);

                if (!newBoxFile.Name.Equals(file.Title))
                {
                    var folderId = GetParentFolderId(await GetBoxFileAsync(fileId));
                    file.Title = await GetAvailableTitleAsync(file.Title, folderId, IsExistAsync);
                    newBoxFile = await ProviderInfo.Storage.RenameFileAsync(fileId, file.Title);
                }
            }
            else if (file.FolderID != null)
            {
                var folderId = MakeBoxId(file.FolderID);
                file.Title = await GetAvailableTitleAsync(file.Title, folderId, IsExistAsync);
                newBoxFile = await  ProviderInfo.Storage.CreateFileAsync(fileStream, file.Title, folderId);
            }

            await ProviderInfo.CacheResetAsync(newBoxFile);
            var parentId = GetParentFolderId(newBoxFile);
            if (parentId != null) await ProviderInfo.CacheResetAsync(parentId);

            return ToFile(newBoxFile);
        }

        public File<string> ReplaceFileVersion(File<string> file, Stream fileStream)
        {
            return ReplaceFileVersionAsync(file, fileStream).Result;
        }

        public async Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
        {
            return await SaveFileAsync(file, fileStream);
        }

        public void DeleteFile(string fileId)
        {
            DeleteFileAsync(fileId).Wait();
        }

        public async Task DeleteFileAsync(string fileId)
        {
            var boxFile = await GetBoxFileAsync(fileId);
            if (boxFile == null) return;
            var id = MakeId(boxFile.Id);

            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var hashIDs = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.Id.StartsWith(id))
                    .Select(r => r.HashId);

                var link = Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.TagLink.RemoveRange(link);
                await FilesDbContext.SaveChangesAsync();

                var tagsToRemove = Query(FilesDbContext.Tag)
                    .Where(r => Query(FilesDbContext.TagLink).Where(a => a.TagId == r.Id).Any());

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

            if (!(boxFile is ErrorFile))
            {
                ProviderInfo.Storage.DeleteItem(boxFile);
            }

            await ProviderInfo.CacheResetAsync(boxFile.Id, true);
            var parentFolderId = GetParentFolderId(boxFile);
            if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId);
        }

        public bool IsExist(string title, object folderId)
        {
            return IsExistAsync(title, folderId).Result;
        }

        public async Task<bool> IsExistAsync(string title, object folderId)
        {
            var item = await GetBoxItemsAsync(folderId.ToString(), false);
            return item.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        }

        public TTo MoveFile<TTo>(string fileId, TTo toFolderId)
        {
            return MoveFileAsync(fileId, toFolderId).Result;
        }

        public async Task<TTo> MoveFileAsync<TTo>(string fileId, TTo toFolderId)
        {
            if (toFolderId is int tId)
            {
                return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tId), typeof(TTo));
            }

            if (toFolderId is string tsId)
            {
                return (TTo)Convert.ChangeType(await MoveFileAsync(fileId, tsId), typeof(TTo));
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
                fileId, this, BoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                true);

            return moved.ID;
        }

        public string MoveFile(string fileId, string toFolderId)
        {
            return MoveFileAsync(fileId, toFolderId).Result;
        }

        public async Task<string> MoveFileAsync(string fileId, string toFolderId)
        {
            var boxFile = await GetBoxFileAsync(fileId);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toBoxFolder = await GetBoxFolderAsync(toFolderId);
            if (toBoxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var fromFolderId = GetParentFolderId(boxFile);

            var newTitle = await GetAvailableTitleAsync(boxFile.Name, toBoxFolder.Id, IsExistAsync);
            boxFile = await ProviderInfo.Storage.MoveFileAsync(boxFile.Id, newTitle, toBoxFolder.Id);

            await ProviderInfo.CacheResetAsync(boxFile.Id, true);
            await ProviderInfo.CacheResetAsync(fromFolderId);
            await ProviderInfo.CacheResetAsync(toBoxFolder.Id);

            return MakeId(boxFile.Id);
        }

        public File<TTo> CopyFile<TTo>(string fileId, TTo toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<TTo>> CopyFileAsync<TTo>(string fileId, TTo toFolderId)
        {
            if (toFolderId is int tId)
            {
                return await CopyFileAsync(fileId, tId) as File<TTo>;
            }

            if (toFolderId is string tsId)
            {
                return await CopyFileAsync(fileId, tsId) as File<TTo>;
            }

            throw new NotImplementedException();
        }

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
        {
            var boxFile = await GetBoxFileAsync(fileId);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toBoxFolder = await GetBoxFolderAsync(toFolderId);
            if (toBoxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var newTitle = await GetAvailableTitleAsync(boxFile.Name, toBoxFolder.Id, IsExistAsync);
            var newBoxFile = ProviderInfo.Storage.CopyFile(boxFile.Id, newTitle, toBoxFolder.Id);

            await ProviderInfo.CacheResetAsync(newBoxFile);
            await ProviderInfo.CacheResetAsync (toBoxFolder.Id);

            return ToFile(newBoxFile);
        }

        public File<int> CopyFile(string fileId, int toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
        {
            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, BoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false);

            return moved;
        }

        public string FileRename(File<string> file, string newTitle)
        {
            return FileRenameAsync(file, newTitle).Result;
        }

        public async Task<string> FileRenameAsync(File<string> file, string newTitle)
        {
            var boxFile = await GetBoxFileAsync(file.ID);
            newTitle = await GetAvailableTitleAsync(newTitle, GetParentFolderId(boxFile), IsExistAsync);

            boxFile = await ProviderInfo.Storage.RenameFileAsync(boxFile.Id, newTitle);

            await ProviderInfo.CacheResetAsync(boxFile);
            var parentId = GetParentFolderId(boxFile);
            if (parentId != null) await ProviderInfo.CacheResetAsync(parentId);

            return MakeId(boxFile.Id);
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
            return CreateUploadSessionAsync(file, contentLength).Result;
        }

        public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
        {
            if (SetupInfo.ChunkUploadSize > contentLength)
                return Task.FromResult(new ChunkedUploadSession<string>(RestoreIds(file), contentLength) { UseChunks = false });

            var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

            uploadSession.Items["TempPath"] = TempPath.GetTempFileName();

            uploadSession.File = RestoreIds(uploadSession.File);
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

                uploadSession.File = await SaveFileAsync(uploadSession.File, stream);
                uploadSession.BytesUploaded = chunkLength;
                return uploadSession.File;
            }

            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            using (var fs = new FileStream(tempPath, FileMode.Append))
            {
                await stream.CopyToAsync(fs);
            }

            uploadSession.BytesUploaded += chunkLength;

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"),
                                               FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);
                uploadSession.File = await SaveFileAsync(uploadSession.File, fs);
            }
            else
            {
                uploadSession.File = RestoreIds(uploadSession.File);
            }

            return uploadSession.File;
        }

        public void AbortUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            AbortUploadSessionAsync(uploadSession).Wait();
        }

        public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("TempPath"))
            {
                File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
            }

            return Task.FromResult(0);
        }
        #endregion
    }
}