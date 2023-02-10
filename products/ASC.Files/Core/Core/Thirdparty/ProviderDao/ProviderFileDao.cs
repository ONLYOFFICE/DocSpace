// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Files.Thirdparty.ProviderDao;

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

    public Task InvalidateCacheAsync(string fileId)
    {
        var selector = GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        return fileDao.InvalidateCacheAsync(selector.ConvertId(fileId));
    }

    public async Task<File<string>> GetFileAsync(string fileId)
    {
        var selector = GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(fileId));

        return result;
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        var selector = GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(fileId), fileVersion);

        return result;
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var selector = GetSelector(parentId);
        var fileDao = selector.GetFileDao(parentId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(parentId), title);

        return result;
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        var selector = GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(fileId), fileVersion);

        return result;
    }

    public IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
    {
        var selector = GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        return fileDao.GetFileHistoryAsync(selector.ConvertId(fileId));
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
    {

        foreach (var selector in GetSelectors())
        {
            var selectorLocal = selector;
            var matchedIds = fileIds.Where(selectorLocal.IsMatch);

            if (!matchedIds.Any())
            {
                continue;
            }

            foreach (var matchedId in matchedIds.GroupBy(selectorLocal.GetIdCode))
            {
                var fileDao = selectorLocal.GetFileDao(matchedId.FirstOrDefault());

                await foreach (var file in fileDao.GetFilesAsync(matchedId.Select(selectorLocal.ConvertId).ToList()))
                {
                    if (file != null)
                    {
                        yield return file;
                    }
                }
            }
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesFilteredAsync(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
    {
        foreach (var selector in GetSelectors())
        {
            var selectorLocal = selector;
            var matchedIds = fileIds.Where(selectorLocal.IsMatch);

            if (!matchedIds.Any())
            {
                continue;
            }

            foreach (var matchedId in matchedIds.GroupBy(selectorLocal.GetIdCode))
            {
                var fileDao = selectorLocal.GetFileDao(matchedId.FirstOrDefault());

                await foreach (var file in fileDao.GetFilesFilteredAsync(matchedId.Select(selectorLocal.ConvertId).ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent))
                {
                    if (file != null)
                    {
                        yield return file;
                    }
                }
            }
        }
    }

    public async IAsyncEnumerable<string> GetFilesAsync(string parentId)
    {
        var selector = GetSelector(parentId);
        var fileDao = selector.GetFileDao(parentId);
        var files = fileDao.GetFilesAsync(selector.ConvertId(parentId));

        await foreach (var f in files.Where(r => r != null))
        {
            yield return f;
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false, bool excludeSubject = false)
    {
        var selector = GetSelector(parentId);

        var fileDao = selector.GetFileDao(parentId);
        var files = fileDao.GetFilesAsync(selector.ConvertId(parentId), orderBy, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders, excludeSubject);
        var result = await files.Where(r => r != null).ToListAsync();

        foreach (var r in result)
        {
            yield return r;
        }
    }

    public override Task<Stream> GetFileStreamAsync(File<string> file)
    {
        return GetFileStreamAsync(file, 0);
    }

    /// <summary>
    /// Get stream of file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="offset"></param>
    /// <returns>Stream</returns>
    public Task<Stream> GetFileStreamAsync(File<string> file, long offset)
    {
        ArgumentNullException.ThrowIfNull(file);

        return InternalGetFileStreamAsync(file, offset);
    }

    private async Task<Stream> InternalGetFileStreamAsync(File<string> file, long offset)
    {
        var fileId = file.Id;
        var selector = GetSelector(fileId);
        file.Id = selector.ConvertId(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var stream = await fileDao.GetFileStreamAsync(file, offset);
        file.Id = fileId; //Restore id

        return stream;
    }

    public Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return InternalIsSupportedPreSignedUriAsync(file);
    }

    private async Task<bool> InternalIsSupportedPreSignedUriAsync(File<string> file)
    {
        var fileId = file.Id;
        var selector = GetSelector(fileId);
        file.Id = selector.ConvertId(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var isSupported = await fileDao.IsSupportedPreSignedUriAsync(file);
        file.Id = fileId; //Restore id

        return isSupported;
    }

    public Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
    {
        ArgumentNullException.ThrowIfNull(file);

        return InternalGetPreSignedUriAsync(file, expires);
    }

    private async Task<Uri> InternalGetPreSignedUriAsync(File<string> file, TimeSpan expires)
    {
        var fileId = file.Id;
        var selector = GetSelector(fileId);
        file.Id = selector.ConvertId(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var streamUri = await fileDao.GetPreSignedUriAsync(file, expires);
        file.Id = fileId; //Restore id

        return streamUri;
    }

    public Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);

        return InternalSaveFileAsync(file, fileStream);
    }

    private async Task<File<string>> InternalSaveFileAsync(File<string> file, Stream fileStream)
    {
        var fileId = file.Id;
        var folderId = file.ParentId;

        IDaoSelector selector;
        File<string> fileSaved = null;
        //Convert
        if (fileId != null)
        {
            selector = GetSelector(fileId);
            file.Id = selector.ConvertId(fileId);
            if (folderId != null)
            {
                file.ParentId = selector.ConvertId(folderId);
            }

            var fileDao = selector.GetFileDao(fileId);
            fileSaved = await fileDao.SaveFileAsync(file, fileStream);
        }
        else if (folderId != null)
        {
            selector = GetSelector(folderId);
            file.ParentId = selector.ConvertId(folderId);
            var fileDao = selector.GetFileDao(folderId);
            fileSaved = await fileDao.SaveFileAsync(file, fileStream);
        }

        if (fileSaved != null)
        {
            return fileSaved;
        }

        throw new ArgumentException("No file id or folder id toFolderId determine provider");
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Id == null)
        {
            throw new ArgumentException("No file id or folder id toFolderId determine provider");
        }

        var fileId = file.Id;
        var folderId = file.ParentId;

        //Convert
        var selector = GetSelector(fileId);

        file.Id = selector.ConvertId(fileId);
        if (folderId != null)
        {
            file.ParentId = selector.ConvertId(folderId);
        }

        var fileDao = selector.GetFileDao(fileId);

        return fileDao.ReplaceFileVersionAsync(file, fileStream);
    }

    public Task DeleteFileAsync(string fileId)
    {
        var selector = GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        return fileDao.DeleteFileAsync(selector.ConvertId(fileId));
    }

    public Task<bool> IsExistAsync(string title, object folderId)
    {
        var selector = GetSelector(folderId.ToString());

        var fileDao = selector.GetFileDao(folderId.ToString());

        return fileDao.IsExistAsync(title, selector.ConvertId(folderId.ToString()));
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

    public async Task<int> MoveFileAsync(string fileId, int toFolderId)
    {
        var movedFile = await PerformCrossDaoFileCopyAsync(fileId, toFolderId, true);

        return movedFile.Id;
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var selector = GetSelector(fileId);
        if (IsCrossDao(fileId, toFolderId))
        {
            var movedFile = await PerformCrossDaoFileCopyAsync(fileId, toFolderId, true);

            return movedFile.Id;
        }

        var fileDao = selector.GetFileDao(fileId);

        return await fileDao.MoveFileAsync(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
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

    public Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        return PerformCrossDaoFileCopyAsync(fileId, toFolderId, false);
    }

    public Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var selector = GetSelector(fileId);
        if (IsCrossDao(fileId, toFolderId))
        {
            return PerformCrossDaoFileCopyAsync(fileId, toFolderId, false);
        }

        var fileDao = selector.GetFileDao(fileId);

        return fileDao.CopyFileAsync(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
    }

    public Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var selector = GetSelector(file.Id);
        var fileDao = selector.GetFileDao(file.Id);

        return fileDao.FileRenameAsync(ConvertId(file), newTitle);
    }

    public Task<string> UpdateCommentAsync(string fileId, int fileVersion, string comment)
    {
        var selector = GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);

        return fileDao.UpdateCommentAsync(selector.ConvertId(fileId), fileVersion, comment);
    }

    public Task CompleteVersionAsync(string fileId, int fileVersion)
    {
        var selector = GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);

        return fileDao.CompleteVersionAsync(selector.ConvertId(fileId), fileVersion);
    }

    public Task ContinueVersionAsync(string fileId, int fileVersion)
    {
        var selector = GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        return fileDao.ContinueVersionAsync(selector.ConvertId(fileId), fileVersion);
    }

    public bool UseTrashForRemove(File<string> file)
    {
        var selector = GetSelector(file.Id);
        var fileDao = selector.GetFileDao(file.Id);

        return fileDao.UseTrashForRemove(file);
    }

    #region chunking

    public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        var fileDao = GetFileDao(file);

        return fileDao.CreateUploadSessionAsync(ConvertId(file), contentLength);
    }

    public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
    {
        var fileDao = GetFileDao(uploadSession.File);
        uploadSession.File = ConvertId(uploadSession.File);
        await fileDao.UploadChunkAsync(uploadSession, chunkStream, chunkLength);

        return uploadSession.File;
    }

    public Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        var fileDao = GetFileDao(uploadSession.File);
        uploadSession.File = ConvertId(uploadSession.File);
        return fileDao.FinalizeUploadSessionAsync(uploadSession);
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        var fileDao = GetFileDao(uploadSession.File);
        uploadSession.File = ConvertId(uploadSession.File);

        return fileDao.AbortUploadSessionAsync(uploadSession);
    }

    private IFileDao<string> GetFileDao(File<string> file)
    {
        if (file.Id != null)
        {
            return GetSelector(file.Id).GetFileDao(file.Id);
        }

        if (file.ParentId != null)
        {
            return GetSelector(file.ParentId).GetFileDao(file.ParentId);
        }

        throw new ArgumentException("Can't create instance of dao for given file.", nameof(file));
    }

    private string ConvertId(string id)
    {
        return id != null ? GetSelector(id).ConvertId(id) : null;
    }

    private File<string> ConvertId(File<string> file)
    {
        file.Id = ConvertId(file.Id);
        file.ParentId = ConvertId(file.ParentId);

        return file;
    }

    public override Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        var selector = GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);
        return fileDao.GetThumbnailAsync(fileId, width, height);
    }

    public override Task<Stream> GetThumbnailAsync(File<string> file, int width, int height)
    {
        var fileDao = GetFileDao(file);
        return fileDao.GetThumbnailAsync(file, width, height);
    }

    #endregion
}
