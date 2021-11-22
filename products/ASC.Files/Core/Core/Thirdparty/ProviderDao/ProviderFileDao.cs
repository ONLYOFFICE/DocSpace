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
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Thirdparty;

namespace ASC.Files.Thirdparty.ProviderDao
{
    [Scope]
    internal class ProviderFileDao : ProviderDaoBase, IFileDao<string>
    {
        public ProviderFileDao(
            IServiceProvider serviceProvider,
            TenantManager tenantManager,
            SecurityDao<string> securityDao,
            TagDao<string> tagDao,
            CrossDao crossDao)
            : base(serviceProvider, tenantManager, securityDao, tagDao, crossDao)
        {

        }

        public void InvalidateCache(string fileId)
        {
            InvalidateCacheAsync(fileId).Wait();
        }

        public async Task InvalidateCacheAsync(string fileId)
        {
            var selector = GetSelector(fileId);
            var fileDao = selector.GetFileDao(fileId);
            await fileDao.InvalidateCacheAsync(selector.ConvertId(fileId));
        }

        public File<string> GetFile(string fileId)
        {
            return GetFileAsync(fileId).Result;
        }

        public async Task<File<string>> GetFileAsync(string fileId)
        {
            var selector = GetSelector(fileId);

            var fileDao = selector.GetFileDao(fileId);
            var result = await fileDao.GetFileAsync(selector.ConvertId(fileId));

            if (result != null)
            {
                SetSharedProperty(new[] { result });
            }

            return result;
        }

        public File<string> GetFile(string fileId, int fileVersion)
        {
            return GetFileAsync(fileId, fileVersion).Result;
        }

        public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
        {
            var selector = GetSelector(fileId);

            var fileDao = selector.GetFileDao(fileId);
            var result = await fileDao.GetFileAsync(selector.ConvertId(fileId), fileVersion);

            if (result != null)
            {
                SetSharedProperty(new[] { result });
            }

            return result;
        }

        public File<string> GetFile(string parentId, string title)
        {
            return GetFileAsync(parentId, title).Result;
        }

        public async Task<File<string>> GetFileAsync(string parentId, string title)
        {
            var selector = GetSelector(parentId);
            var fileDao = selector.GetFileDao(parentId);
            var result = await fileDao.GetFileAsync(selector.ConvertId(parentId), title);

            if (result != null)
            {
                SetSharedProperty(new[] { result });
            }

            return result;
        }

        public File<string> GetFileStable(string fileId, int fileVersion = -1)
        {
            return GetFileStableAsync(fileId, fileVersion).Result;
        }

        public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
        {
            var selector = GetSelector(fileId);

            var fileDao = selector.GetFileDao(fileId);
            var result = await fileDao.GetFileAsync (selector.ConvertId(fileId), fileVersion);

            if (result != null)
            {
                SetSharedProperty(new[] { result });
            }

            return result;
        }

        public List<File<string>> GetFileHistory(string fileId)
        {
            return GetFileHistoryAsync(fileId).Result;
        }

        public async Task<List<File<string>>> GetFileHistoryAsync(string fileId)
        {
            var selector = GetSelector(fileId);
            var fileDao = selector.GetFileDao(fileId);
            return await fileDao.GetFileHistoryAsync(selector.ConvertId(fileId));
        }

        public List<File<string>> GetFiles(IEnumerable<string> fileIds)
        {
            return GetFilesAsync(fileIds).ToListAsync().Result;
        }

        public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
        {
            var result = AsyncEnumerable.Empty<File<string>>();

            foreach (var selector in GetSelectors())
            {
                var selectorLocal = selector;
                var matchedIds = fileIds.Where(selectorLocal.IsMatch);

                if (!matchedIds.Any()) continue;

                result = result.Concat(matchedIds.GroupBy(selectorLocal.GetIdCode)
                                                .ToAsyncEnumerable()
                                                .SelectMany(matchedId =>
                                                 {
                                                     var fileDao = selectorLocal.GetFileDao(matchedId.FirstOrDefault());
                                                     return fileDao.GetFilesAsync(matchedId.Select(selectorLocal.ConvertId).ToList());
                                                 }
                    )
                    .Where(r => r != null));
            }

            return result;
        }

        public List<File<string>> GetFilesFiltered(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
        {
            return GetFilesFilteredAsync(fileIds, filterType, subjectGroup, subjectID, searchText, searchInContent).ToListAsync().Result;
        }

        public IAsyncEnumerable<File<string>> GetFilesFilteredAsync(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
        {
            var result = AsyncEnumerable.Empty<File<string>>();

            foreach (var selector in GetSelectors())
            {
                var selectorLocal = selector;
                var matchedIds = fileIds.Where(selectorLocal.IsMatch);

                if (!matchedIds.Any()) continue;

                result = result.Concat(matchedIds.GroupBy(selectorLocal.GetIdCode)
                                       .ToAsyncEnumerable()
                                       .SelectMany(matchedId =>
                                       {
                                           var fileDao = selectorLocal.GetFileDao(matchedId.FirstOrDefault());
                                           var result = fileDao.GetFilesFilteredAsync(matchedId.Select(selectorLocal.ConvertId).ToArray(),
                                                   filterType, subjectGroup, subjectID, searchText, searchInContent);
                                           return result;
                                       })
                                       .Where(r => r != null));
            }

            return result;
        }

        public List<string> GetFiles(string parentId)
        {
            return GetFilesAsync(parentId).Result;
        }

        public async Task<List<string>> GetFilesAsync(string parentId)
        {
            var selector = GetSelector(parentId);
            var fileDao = selector.GetFileDao(parentId);
            var files = await fileDao.GetFilesAsync(selector.ConvertId(parentId));
            return files.Where(r => r != null).ToList();
        }

        public List<File<string>> GetFiles(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            return GetFilesAsync(parentId, orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).Result;
        }

        public async Task<List<File<string>>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
        {
            var selector = GetSelector(parentId);

            var fileDao = selector.GetFileDao(parentId);
            var files = await fileDao.GetFilesAsync(selector.ConvertId(parentId), orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders);
            var result = files.Where(r => r != null).ToList();

            if (!result.Any()) return new List<File<string>>();

            SetSharedProperty(result);

            return result;
        }

        public override Stream GetFileStream(File<string> file)
        {
            return GetFileStreamAsync(file).Result;
        }

        public override async Task<Stream> GetFileStreamAsync(File<string> file)
        {
            return await GetFileStreamAsync(file, 0);
        }

        /// <summary>
        /// Get stream of file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="offset"></param>
        /// <returns>Stream</returns>
        public Stream GetFileStream(File<string> file, long offset)
        {
            return GetFileStreamAsync(file, offset).Result;
        }

        public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
        {
            if (file == null) throw new ArgumentNullException("file");
            var fileId = file.ID;
            var selector = GetSelector(fileId);
            file.ID = selector.ConvertId(fileId);

            var fileDao = selector.GetFileDao(fileId);
            var stream = await fileDao.GetFileStreamAsync(file, offset);
            file.ID = fileId; //Restore id
            return stream;
        }

        public bool IsSupportedPreSignedUri(File<string> file)
        {
            return IsSupportedPreSignedUriAsync(file).Result;
        }

        public async Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
        {
            if (file == null) throw new ArgumentNullException("file");
            var fileId = file.ID;
            var selector = GetSelector(fileId);
            file.ID = selector.ConvertId(fileId);

            var fileDao = selector.GetFileDao(fileId);
            var isSupported = await fileDao.IsSupportedPreSignedUriAsync(file);
            file.ID = fileId; //Restore id
            return isSupported;
        }

        public Uri GetPreSignedUri(File<string> file, TimeSpan expires)
        {
            return GetPreSignedUriAsync(file, expires).Result;
        }

        public async Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
        {
            if (file == null) throw new ArgumentNullException("file");
            var fileId = file.ID;
            var selector = GetSelector(fileId);
            file.ID = selector.ConvertId(fileId);

            var fileDao = selector.GetFileDao(fileId);
            var streamUri = await fileDao.GetPreSignedUriAsync(file, expires);
            file.ID = fileId; //Restore id
            return streamUri;
        }

        public File<string> SaveFile(File<string> file, Stream fileStream)
        {
            return SaveFileAsync(file, fileStream).Result;
        }

        public async Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException("file");

            var fileId = file.ID;
            var folderId = file.FolderID;

            IDaoSelector selector;
            File<string> fileSaved = null;
            //Convert
            if (fileId != null)
            {
                selector = GetSelector(fileId);
                file.ID = selector.ConvertId(fileId);
                if (folderId != null)
                    file.FolderID = selector.ConvertId(folderId);
                var fileDao = selector.GetFileDao(fileId);
                fileSaved = await fileDao.SaveFileAsync(file, fileStream);
            }
            else if (folderId != null)
            {
                selector = GetSelector(folderId);
                file.FolderID = selector.ConvertId(folderId);
                var fileDao = selector.GetFileDao(folderId);
                fileSaved = await fileDao.SaveFileAsync(file, fileStream);
            }

            if (fileSaved != null)
            {
                return fileSaved;
            }
            throw new ArgumentException("No file id or folder id toFolderId determine provider");
        }

        public File<string> ReplaceFileVersion(File<string> file, Stream fileStream)
        {
            return ReplaceFileVersionAsync(file, fileStream).Result;
        }

        public async Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (file.ID == null) throw new ArgumentException("No file id or folder id toFolderId determine provider");

            var fileId = file.ID;
            var folderId = file.FolderID;

            //Convert
            var selector = GetSelector(fileId);

            file.ID = selector.ConvertId(fileId);
            if (folderId != null) file.FolderID = selector.ConvertId(folderId);

            var fileDao = selector.GetFileDao(fileId);
            return await fileDao.ReplaceFileVersionAsync(file, fileStream);
        }

        public void DeleteFile(string fileId)
        {
            DeleteFileAsync(fileId).Wait();
        }

        public async Task DeleteFileAsync(string fileId)
        {
            var selector = GetSelector(fileId);
            var fileDao = selector.GetFileDao(fileId);
            await fileDao.DeleteFileAsync(selector.ConvertId(fileId));
        }

        public bool IsExist(string title, object folderId)
        {
            return IsExistAsync(title, folderId).Result;
        }

        public async Task<bool> IsExistAsync(string title, object folderId)
        {
            var selector = GetSelector(folderId.ToString());

            var fileDao = selector.GetFileDao(folderId.ToString());
            return await fileDao.IsExistAsync(title, selector.ConvertId(folderId.ToString()));
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
            var movedFile = await PerformCrossDaoFileCopyAsync(fileId, toFolderId, true);
            return movedFile.ID;
        }

        public string MoveFile(string fileId, string toFolderId)
        {
            return MoveFileAsync(fileId, toFolderId).Result;
        }

        public async Task<string> MoveFileAsync(string fileId, string toFolderId)
        {
            var selector = GetSelector(fileId);
            if (IsCrossDao(fileId, toFolderId))
            {
                var movedFile = await PerformCrossDaoFileCopyAsync(fileId, toFolderId, true);
                return movedFile.ID;
            }

            var fileDao = selector.GetFileDao(fileId);
            return await fileDao.MoveFileAsync(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
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

        public File<int> CopyFile(string fileId, int toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
        {
            return await PerformCrossDaoFileCopyAsync(fileId, toFolderId, false);
        }

        public File<string> CopyFile(string fileId, string toFolderId)
        {
            return CopyFileAsync(fileId, toFolderId).Result;
        }

        public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
        {
            var selector = GetSelector(fileId);
            if (IsCrossDao(fileId, toFolderId))
            {
                return await PerformCrossDaoFileCopyAsync(fileId, toFolderId, false);
            }

            var fileDao = selector.GetFileDao(fileId);
            return await fileDao.CopyFileAsync(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
        }

        public string FileRename(File<string> file, string newTitle)
        {
            return FileRenameAsync(file, newTitle).Result;
        }

        public async Task<string> FileRenameAsync(File<string> file, string newTitle)
        {
            var selector = GetSelector(file.ID);
            var fileDao = selector.GetFileDao(file.ID);
            return await fileDao.FileRenameAsync(ConvertId(file), newTitle);
        }

        public string UpdateComment(string fileId, int fileVersion, string comment)
        {
            return UpdateCommentAsync(fileId, fileVersion, comment).Result;
        }

        public async Task<string> UpdateCommentAsync(string fileId, int fileVersion, string comment)
        {
            var selector = GetSelector(fileId);

            var fileDao = selector.GetFileDao(fileId);
            return await fileDao.UpdateCommentAsync(selector.ConvertId(fileId), fileVersion, comment);
        }

        public void CompleteVersion(string fileId, int fileVersion)
        {
            var selector = GetSelector(fileId);

            var fileDao = selector.GetFileDao(fileId);
            fileDao.CompleteVersion(selector.ConvertId(fileId), fileVersion);
        }

        public void ContinueVersion(string fileId, int fileVersion)
        {
            ContinueVersionAsync(fileId, fileVersion).Wait();
        }

        public async Task ContinueVersionAsync(string fileId, int fileVersion)
        {
            var selector = GetSelector(fileId);
            var fileDao = selector.GetFileDao(fileId);
            await fileDao.ContinueVersionAsync(selector.ConvertId(fileId), fileVersion);
        }

        public bool UseTrashForRemove(File<string> file)
        {
            var selector = GetSelector(file.ID);
            var fileDao = selector.GetFileDao(file.ID);
            return UseTrashForRemove(file);
        }

        #region chunking

        public ChunkedUploadSession<string> CreateUploadSession(File<string> file, long contentLength)
        {
            return CreateUploadSessionAsync(file, contentLength).Result;
        }

        public async Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
        {
            var fileDao = GetFileDao(file);
            return await fileDao.CreateUploadSessionAsync(ConvertId(file), contentLength);
        }

        public File<string> UploadChunk(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
        {
            return UploadChunkAsync(uploadSession, chunkStream, chunkLength).Result;
        }

        public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
        {
            var fileDao = GetFileDao(uploadSession.File);
            uploadSession.File = ConvertId(uploadSession.File);
            await fileDao.UploadChunkAsync(uploadSession, chunkStream, chunkLength);
            return uploadSession.File;
        }

        public void AbortUploadSession(ChunkedUploadSession<string> uploadSession)
        {
            AbortUploadSessionAsync(uploadSession).Wait();
        }

        public async Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
        {
            var fileDao = GetFileDao(uploadSession.File);
            uploadSession.File = ConvertId(uploadSession.File);
            await fileDao.AbortUploadSessionAsync(uploadSession);
        }

        private IFileDao<string> GetFileDao(File<string> file)
        {
            if (file.ID != null)
                return GetSelector(file.ID).GetFileDao(file.ID);

            if (file.FolderID != null)
                return GetSelector(file.FolderID).GetFileDao(file.FolderID);

            throw new ArgumentException("Can't create instance of dao for given file.", "file");
        }

        private string ConvertId(string id)
        {
            return id != null ? GetSelector(id).ConvertId(id) : null;
        }

        private File<string> ConvertId(File<string> file)
        {
            file.ID = ConvertId(file.ID);
            file.FolderID = ConvertId(file.FolderID);
            return file;
        }
        #endregion
    }
}