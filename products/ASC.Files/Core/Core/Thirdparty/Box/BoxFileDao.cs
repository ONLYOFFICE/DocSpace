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

        public async Task InvalidateCacheAsync(string fileId)
        {
            var boxFileId = MakeBoxId(fileId);
            await ProviderInfo.CacheResetAsync(boxFileId, true).ConfigureAwait(false);

            var boxFile = await GetBoxFileAsync(fileId).ConfigureAwait(false);
            var parentPath = GetParentFolderId(boxFile);
            if (parentPath != null) await ProviderInfo.CacheResetAsync(parentPath).ConfigureAwait(false);
        }

        public Task<File<string>> GetFileAsync(string fileId)
        {
            return GetFileAsync(fileId, 1);
        }

        public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
        {
            return ToFile(await GetBoxFileAsync(fileId).ConfigureAwait(false));
        }

        public async Task<File<string>> GetFileAsync(string parentId, string title)
        {
            var items = await GetBoxItemsAsync(parentId, false).ConfigureAwait(false);
            return ToFile(items.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)) as BoxFile);
        }

        public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
        {
            return ToFile(await GetBoxFileAsync(fileId).ConfigureAwait(false));
        }

        public IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
        {
            return GetFileAsync(fileId).ToAsyncEnumerable();
        }

        public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
        {
            if (fileIds == null || !fileIds.Any()) return AsyncEnumerable.Empty<File<string>>();

            var result = fileIds.ToAsyncEnumerable().SelectAwait(async e => ToFile(await GetBoxFileAsync(e).ConfigureAwait(false)));

            return result;
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

            return files;
        }

        public async Task<List<string>> GetFilesAsync(string parentId)
        {
            var items = await GetBoxItemsAsync(parentId, false).ConfigureAwait(false);
            return items.Select(entry => MakeId(entry.Id)).ToList();
        }

        public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            if (filterType == FilterType.FoldersOnly) yield break;

            //Get only files
            var filesWait = await GetBoxItemsAsync(parentId, false).ConfigureAwait(false);
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
                    yield break;
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

            foreach (var f in files)
            {
                yield return f;
            }
        }

        public override Task<Stream> GetFileStreamAsync(File<string> file)
        {
            return GetFileStreamAsync(file, 0);
        }

        public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
        {
            var boxFileId = MakeBoxId(file.ID);
            await ProviderInfo.CacheResetAsync(boxFileId, true).ConfigureAwait(false);

            var boxFile = await GetBoxFileAsync(file.ID).ConfigureAwait(false);
            if (boxFile == null) throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var storage = await ProviderInfo.StorageAsync;
            var fileStream = await storage.DownloadStreamAsync(boxFile, (int)offset).ConfigureAwait(false);

            return fileStream;
        }

        public Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
        {
            throw new NotSupportedException();
        }

        public Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
        {
            return Task.FromResult(false);
        }

        public Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

            return InternalSaveFileAsync(file, fileStream);
        }

        private async Task<File<string>> InternalSaveFileAsync(File<string> file, Stream fileStream)
        {
            BoxFile newBoxFile = null;
            var storage = await ProviderInfo.StorageAsync;

            if (file.ID != null)
            {
                var fileId = MakeBoxId(file.ID);
                newBoxFile = await storage.SaveStreamAsync(fileId, fileStream).ConfigureAwait(false);

                if (!newBoxFile.Name.Equals(file.Title))
                {
                    var folderId = GetParentFolderId(await GetBoxFileAsync(fileId).ConfigureAwait(false));
                    file.Title = await GetAvailableTitleAsync(file.Title, folderId, IsExistAsync).ConfigureAwait(false);
                    newBoxFile = await storage.RenameFileAsync(fileId, file.Title).ConfigureAwait(false);
                }
            }
            else if (file.FolderID != null)
            {
                var folderId = MakeBoxId(file.FolderID);
                file.Title = await GetAvailableTitleAsync(file.Title, folderId, IsExistAsync).ConfigureAwait(false);
                newBoxFile = await storage.CreateFileAsync(fileStream, file.Title, folderId).ConfigureAwait(false);
            }

            await ProviderInfo.CacheResetAsync(newBoxFile).ConfigureAwait(false);
            var parentId = GetParentFolderId(newBoxFile);
            if (parentId != null) await ProviderInfo.CacheResetAsync(parentId).ConfigureAwait(false);

            return ToFile(newBoxFile);
        }

        public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
        {
            return SaveFileAsync(file, fileStream);
        }

        public async Task DeleteFileAsync(string fileId)
        {
            var boxFile = await GetBoxFileAsync(fileId).ConfigureAwait(false);
            if (boxFile == null) return;
            var id = MakeId(boxFile.Id);

            using (var tx = await FilesDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var hashIDs = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.Id.StartsWith(id))
                    .Select(r => r.HashId);

                var link = await Query(FilesDbContext.TagLink)
                    .Where(r => hashIDs.Any(h => h == r.EntryId))
                    .ToListAsync();

                FilesDbContext.TagLink.RemoveRange(link);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var tagsToRemove = from ft in FilesDbContext.Tag
                                   join ftl in FilesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                                   where ftl == null
                                   select ft;

                FilesDbContext.Tag.RemoveRange(await tagsToRemove.ToListAsync());

                var securityToDelete = Query(FilesDbContext.Security)
                    .Where(r => hashIDs.Any(h => h == r.EntryId));

                FilesDbContext.Security.RemoveRange(await securityToDelete.ToListAsync());
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                var mappingToDelete = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => hashIDs.Any(h => h == r.HashId));

                FilesDbContext.ThirdpartyIdMapping.RemoveRange(await mappingToDelete.ToListAsync());
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false);
            }

            if (!(boxFile is ErrorFile))
            {
                var storage = await ProviderInfo.StorageAsync;
                await storage.DeleteItemAsync(boxFile);
            }

            await ProviderInfo.CacheResetAsync(boxFile.Id, true).ConfigureAwait(false);
            var parentFolderId = GetParentFolderId(boxFile);
            if (parentFolderId != null) await ProviderInfo.CacheResetAsync(parentFolderId).ConfigureAwait(false);
        }

        public async Task<bool> IsExistAsync(string title, object folderId)
        {
            var item = await GetBoxItemsAsync(folderId.ToString(), false).ConfigureAwait(false);
            return item.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
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

        public async Task<int> MoveFileAsync(string fileId, int toFolderId)
        {
            var moved = await CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, BoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                true)
                .ConfigureAwait(false);

            return moved.ID;
        }

        public async Task<string> MoveFileAsync(string fileId, string toFolderId)
        {
            var boxFile = await GetBoxFileAsync(fileId).ConfigureAwait(false);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toBoxFolder = await GetBoxFolderAsync(toFolderId).ConfigureAwait(false);
            if (toBoxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var fromFolderId = GetParentFolderId(boxFile);

            var newTitle = await GetAvailableTitleAsync(boxFile.Name, toBoxFolder.Id, IsExistAsync).ConfigureAwait(false);
            var storage = await ProviderInfo.StorageAsync;
            boxFile = await storage.MoveFileAsync(boxFile.Id, newTitle, toBoxFolder.Id).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(boxFile.Id, true).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(fromFolderId).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toBoxFolder.Id).ConfigureAwait(false);

            return MakeId(boxFile.Id);
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

        public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
        {
            var boxFile = await GetBoxFileAsync(fileId).ConfigureAwait(false);
            if (boxFile is ErrorFile errorFile) throw new Exception(errorFile.Error);

            var toBoxFolder = await GetBoxFolderAsync(toFolderId).ConfigureAwait(false);
            if (toBoxFolder is ErrorFolder errorFolder) throw new Exception(errorFolder.Error);

            var newTitle = await GetAvailableTitleAsync(boxFile.Name, toBoxFolder.Id, IsExistAsync).ConfigureAwait(false);
            var storage = await ProviderInfo.StorageAsync;
            var newBoxFile = await storage.CopyFileAsync(boxFile.Id, newTitle, toBoxFolder.Id);

            await ProviderInfo.CacheResetAsync(newBoxFile).ConfigureAwait(false);
            await ProviderInfo.CacheResetAsync(toBoxFolder.Id).ConfigureAwait(false);

            return ToFile(newBoxFile);
        }

        public Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
        {
            var moved = CrossDao.PerformCrossDaoFileCopyAsync(
                fileId, this, BoxDaoSelector.ConvertId,
                toFolderId, FileDao, r => r,
                false);

            return moved;
        }

        public async Task<string> FileRenameAsync(File<string> file, string newTitle)
        {
            var boxFile = await GetBoxFileAsync(file.ID).ConfigureAwait(false);
            newTitle = await GetAvailableTitleAsync(newTitle, GetParentFolderId(boxFile), IsExistAsync).ConfigureAwait(false);

            var storage = await ProviderInfo.StorageAsync;
            boxFile = await storage.RenameFileAsync(boxFile.Id, newTitle).ConfigureAwait(false);

            await ProviderInfo.CacheResetAsync(boxFile).ConfigureAwait(false);
            var parentId = GetParentFolderId(boxFile);
            if (parentId != null) await ProviderInfo.CacheResetAsync(parentId).ConfigureAwait(false);

            return MakeId(boxFile.Id);
        }

        public Task<string> UpdateCommentAsync(string fileId, int fileVersion, string comment)
        {
            return Task.FromResult(string.Empty);
        }

        public Task CompleteVersionAsync(string fileId, int fileVersion)
        {
            return Task.CompletedTask;
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

        public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
        {
            if (SetupInfo.ChunkUploadSize > contentLength)
                return Task.FromResult(new ChunkedUploadSession<string>(RestoreIds(file), contentLength) { UseChunks = false });

            var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

            uploadSession.Items["TempPath"] = TempPath.GetTempFileName();

            uploadSession.File = RestoreIds(uploadSession.File);
            return Task.FromResult(uploadSession);
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

            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            using (var fs = new FileStream(tempPath, FileMode.Append))
            {
                await stream.CopyToAsync(fs).ConfigureAwait(false);
            }

            uploadSession.BytesUploaded += chunkLength;

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"),
                                               FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);
                uploadSession.File = await SaveFileAsync(uploadSession.File, fs).ConfigureAwait(false);
            }
            else
            {
                uploadSession.File = RestoreIds(uploadSession.File);
            }

            return uploadSession.File;
        }

        public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
        {
            if (uploadSession.Items.ContainsKey("TempPath"))
            {
                File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}