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
        CrossDao crossDao,
        SelectorFactory selectorFactory,
        ISecurityDao<string> securityDao)
        : base(serviceProvider, tenantManager, crossDao, selectorFactory, securityDao)
    {

    }

    public async Task InvalidateCacheAsync(string fileId)
    {
        var selector = _selectorFactory.GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        await fileDao.InvalidateCacheAsync(selector.ConvertId(fileId));
    }

    public async Task<File<string>> GetFileAsync(string fileId)
    {
        var selector = _selectorFactory.GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(fileId));

        return result;
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        var selector = _selectorFactory.GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(fileId), fileVersion);

        return result;
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var selector = _selectorFactory.GetSelector(parentId);
        var fileDao = selector.GetFileDao(parentId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(parentId), title);

        return result;
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        var selector = _selectorFactory.GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var result = await fileDao.GetFileAsync(selector.ConvertId(fileId), fileVersion);

        return result;
    }

    public IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
    {
        var selector = _selectorFactory.GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        return fileDao.GetFileHistoryAsync(selector.ConvertId(fileId));
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
    {
        foreach (var group in _selectorFactory.GetSelectors(fileIds))
        {
            var selectorLocal = group.Key;
            if (selectorLocal == null)
            {
                continue;
            }
            var matchedIds = group.Value;

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
        foreach (var group in _selectorFactory.GetSelectors(fileIds))
        {
            var selectorLocal = group.Key;
            if (selectorLocal == null)
            {
                continue;
            }
            var matchedIds = group.Value;

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
        var selector = _selectorFactory.GetSelector(parentId);
        var fileDao = selector.GetFileDao(parentId);
        var files = fileDao.GetFilesAsync(selector.ConvertId(parentId));

        await foreach (var f in files.Where(r => r != null))
        {
            yield return f;
        }
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, 
        bool searchInContent, bool withSubfolders = false, bool excludeSubject = false, int offset = 0, int count = -1, string roomId = default)
    {
        var selector = _selectorFactory.GetSelector(parentId);

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
    public async Task<Stream> GetFileStreamAsync(File<string> file, long offset)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileId = file.Id;
        var selector = _selectorFactory.GetSelector(fileId);
        file.Id = selector.ConvertId(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var stream = await fileDao.GetFileStreamAsync(file, offset);
        file.Id = fileId; //Restore id

        return stream;
    }

    public async Task<bool> IsSupportedPreSignedUriAsync(File<string> file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileId = file.Id;
        var selector = _selectorFactory.GetSelector(fileId);
        file.Id = selector.ConvertId(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var isSupported = await fileDao.IsSupportedPreSignedUriAsync(file);
        file.Id = fileId; //Restore id

        return isSupported;
    }

    public async Task<Uri> GetPreSignedUriAsync(File<string> file, TimeSpan expires)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileId = file.Id;
        var selector = _selectorFactory.GetSelector(fileId);
        file.Id = selector.ConvertId(fileId);

        var fileDao = selector.GetFileDao(fileId);
        var streamUri = await fileDao.GetPreSignedUriAsync(file, expires);
        file.Id = fileId; //Restore id

        return streamUri;
    }

    public async Task<File<string>> SaveFileAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileId = file.Id;
        var folderId = file.ParentId;

        IDaoSelector selector;
        File<string> fileSaved = null;
        //Convert
        if (fileId != null)
        {
            selector = _selectorFactory.GetSelector(fileId);
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
            selector = _selectorFactory.GetSelector(folderId);
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

    public async Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Id == null)
        {
            throw new ArgumentException("No file id or folder id toFolderId determine provider");
        }

        var fileId = file.Id;
        var folderId = file.ParentId;

        //Convert
        var selector = _selectorFactory.GetSelector(fileId);

        file.Id = selector.ConvertId(fileId);
        if (folderId != null)
        {
            file.ParentId = selector.ConvertId(folderId);
        }

        var fileDao = selector.GetFileDao(fileId);

        return await fileDao.ReplaceFileVersionAsync(file, fileStream);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var selector = _selectorFactory.GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        await fileDao.DeleteFileAsync(selector.ConvertId(fileId));
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var selector = _selectorFactory.GetSelector(folderId.ToString());

        var fileDao = selector.GetFileDao(folderId.ToString());

        return await fileDao.IsExistAsync(title, selector.ConvertId(folderId.ToString()));
    }

    public async Task<TTo> MoveFileAsync<TTo>(string fileId, TTo toFolderId)
    {
        if (toFolderId is int tId)
        {
            return IdConverter.Convert<TTo>(await MoveFileAsync(fileId, tId));
        }

        if (toFolderId is string tsId)
        {
            return IdConverter.Convert<TTo>(await MoveFileAsync(fileId, tsId));
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
        var selector = _selectorFactory.GetSelector(fileId);
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

    public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        return await PerformCrossDaoFileCopyAsync(fileId, toFolderId, false);
    }

    public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        var selector = _selectorFactory.GetSelector(fileId);
        if (IsCrossDao(fileId, toFolderId))
        {
            return await PerformCrossDaoFileCopyAsync(fileId, toFolderId, false);
        }

        var fileDao = selector.GetFileDao(fileId);

        return await fileDao.CopyFileAsync(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
    }

    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var selector = _selectorFactory.GetSelector(file.Id);
        var fileDao = selector.GetFileDao(file.Id);

        return await fileDao.FileRenameAsync(ConvertId(file), newTitle);
    }

    public async Task<string> UpdateCommentAsync(string fileId, int fileVersion, string comment)
    {
        var selector = _selectorFactory.GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);

        return await fileDao.UpdateCommentAsync(selector.ConvertId(fileId), fileVersion, comment);
    }

    public async Task CompleteVersionAsync(string fileId, int fileVersion)
    {
        var selector = _selectorFactory.GetSelector(fileId);

        var fileDao = selector.GetFileDao(fileId);

        await fileDao.CompleteVersionAsync(selector.ConvertId(fileId), fileVersion);
    }

    public async Task ContinueVersionAsync(string fileId, int fileVersion)
    {
        var selector = _selectorFactory.GetSelector(fileId);
        var fileDao = selector.GetFileDao(fileId);

        await fileDao.ContinueVersionAsync(selector.ConvertId(fileId), fileVersion);
    }

    public bool UseTrashForRemove(File<string> file)
    {
        var selector = _selectorFactory.GetSelector(file.Id);
        var fileDao = selector.GetFileDao(file.Id);

        return fileDao.UseTrashForRemove(file);
    }

    #region chunking

    public async Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        var fileDao = GetFileDao(file);

        return await fileDao.CreateUploadSessionAsync(ConvertId(file), contentLength);
    }

    public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
    {
        var fileDao = GetFileDao(uploadSession.File);
        uploadSession.File = ConvertId(uploadSession.File);
        await fileDao.UploadChunkAsync(uploadSession, chunkStream, chunkLength);

        return uploadSession.File;
    }

    public async Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        var fileDao = GetFileDao(uploadSession.File);
        uploadSession.File = ConvertId(uploadSession.File);
        return await fileDao.FinalizeUploadSessionAsync(uploadSession);
    }

    public async Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        var fileDao = GetFileDao(uploadSession.File);
        uploadSession.File = ConvertId(uploadSession.File);

        await fileDao.AbortUploadSessionAsync(uploadSession);
    }

    private IFileDao<string> GetFileDao(File<string> file)
    {
        if (file.Id != null)
        {
            return _selectorFactory.GetSelector(file.Id).GetFileDao(file.Id);
        }

        if (file.ParentId != null)
        {
            return _selectorFactory.GetSelector(file.ParentId).GetFileDao(file.ParentId);
        }

        throw new ArgumentException("Can't create instance of dao for given file.", nameof(file));
    }

    private string ConvertId(string id)
    {
        return id != null ? _selectorFactory.GetSelector(id).ConvertId(id) : null;
    }

    private File<string> ConvertId(File<string> file)
    {
        file.Id = ConvertId(file.Id);
        file.ParentId = ConvertId(file.ParentId);

        return file;
    }

    public override Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        var selector = _selectorFactory.GetSelector(fileId);
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
