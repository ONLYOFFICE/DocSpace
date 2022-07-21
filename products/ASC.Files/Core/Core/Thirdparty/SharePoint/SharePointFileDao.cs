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

namespace ASC.Files.Thirdparty.SharePoint;

[Scope]
internal class SharePointFileDao : SharePointDaoBase, IFileDao<string>
{
    private readonly CrossDao _crossDao;
    private readonly SharePointDaoSelector _sharePointDaoSelector;
    private readonly IFileDao<int> _fileDao;

    public SharePointFileDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharePointFileDao> monitor,
        FileUtility fileUtility,
        CrossDao crossDao,
        SharePointDaoSelector sharePointDaoSelector,
        IFileDao<int> fileDao,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
        _crossDao = crossDao;
        _sharePointDaoSelector = sharePointDaoSelector;
        _fileDao = fileDao;
    }

    public Task InvalidateCacheAsync(string fileId)
    {
        return ProviderInfo.InvalidateStorageAsync();
    }

    public Task<File<string>> GetFileAsync(string fileId)
    {
        return GetFileAsync(fileId, 1);
    }

    public async Task<File<string>> GetFileAsync(string fileId, int fileVersion)
    {
        return ProviderInfo.ToFile(await ProviderInfo.GetFileByIdAsync(fileId).ConfigureAwait(false));
    }

    public async Task<File<string>> GetFileAsync(string parentId, string title)
    {
        var files = await ProviderInfo.GetFolderFilesAsync(parentId).ConfigureAwait(false);

        return ProviderInfo.ToFile(files.FirstOrDefault(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)));
    }

    public async Task<File<string>> GetFileStableAsync(string fileId, int fileVersion = -1)
    {
        return ProviderInfo.ToFile(await ProviderInfo.GetFileByIdAsync(fileId).ConfigureAwait(false));
    }

    public IAsyncEnumerable<File<string>> GetFileHistoryAsync(string fileId)
    {
        return GetFileAsync(fileId).ToAsyncEnumerable();
    }

    public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> fileIds)
    {
        var list = new List<File<string>>();

        if (fileIds == null || !fileIds.Any())
        {
            return AsyncEnumerable.Empty<File<string>>();
        }

        var result = fileIds.ToAsyncEnumerable().SelectAwait(async e => ProviderInfo.ToFile(await ProviderInfo.GetFileByIdAsync(fileIds).ConfigureAwait(false)));

        return result;
    }

    public IAsyncEnumerable<File<string>> GetFilesFilteredAsync(IEnumerable<string> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false)
    {
        if (fileIds == null || !fileIds.Any() || filterType == FilterType.FoldersOnly)
        {
            return AsyncEnumerable.Empty<File<string>>();
        }

        var files = GetFilesAsync(fileIds);

        //Filter
        if (subjectID != Guid.Empty)
        {
            files = files.Where(x => subjectGroup
                                         ? _userManager.IsUserInGroup(x.CreateBy, subjectID)
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
                    var fileType = FileUtility.GetFileTypeByFileName(x.Title);

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
        {
            files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
        }

        return files;
    }

    public async Task<List<string>> GetFilesAsync(string parentId)
    {
        var files = await ProviderInfo.GetFolderFilesAsync(parentId).ConfigureAwait(false);

        return files.Select(r => ProviderInfo.ToFile(r).Id).ToList();
    }

    public async IAsyncEnumerable<File<string>> GetFilesAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false)
    {
        if (filterType == FilterType.FoldersOnly)
        {
            yield break;
        }

        //Get only files
        var folderFiles = await ProviderInfo.GetFolderFilesAsync(parentId).ConfigureAwait(false);
        var files = folderFiles.Select(r => ProviderInfo.ToFile(r));

        //Filter
        if (subjectID != Guid.Empty)
        {
            files = files.Where(x => subjectGroup
                                         ? _userManager.IsUserInGroup(x.CreateBy, subjectID)
                                         : x.CreateBy == subjectID);
        }

        switch (filterType)
        {
            case FilterType.FoldersOnly:
                yield break;
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
                    var fileType = FileUtility.GetFileTypeByFileName(x.Title);

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
        {
            files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1).ToList();
        }

        if (orderBy == null)
        {
            orderBy = new OrderBy(SortedByType.DateAndTime, false);
        }

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
        var fileToDownload = await ProviderInfo.GetFileByIdAsync(file.Id).ConfigureAwait(false);
        if (fileToDownload == null)
        {
            throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
        }

        var fileStream = await ProviderInfo.GetFileStreamAsync(fileToDownload.ServerRelativeUrl, (int)offset).ConfigureAwait(false);

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
        ArgumentNullException.ThrowIfNull(fileStream);

        return internalSaveFileAsync(file, fileStream);
    }

    private async Task<File<string>> internalSaveFileAsync(File<string> file, Stream fileStream)
    {
        if (file.Id != null)
        {
            var sharePointFile = await ProviderInfo.CreateFileAsync(file.Id, fileStream).ConfigureAwait(false);

            var resultFile = ProviderInfo.ToFile(sharePointFile);
            if (!sharePointFile.Name.Equals(file.Title))
            {
                var folder = await ProviderInfo.GetFolderByIdAsync(file.ParentId).ConfigureAwait(false);
                file.Title = await GetAvailableTitleAsync(file.Title, folder, IsExistAsync).ConfigureAwait(false);

                var id = await ProviderInfo.RenameFileAsync(DaoSelector.ConvertId(resultFile.Id), file.Title).ConfigureAwait(false);

                return await GetFileAsync(DaoSelector.ConvertId(id)).ConfigureAwait(false);
            }

            return resultFile;
        }

        if (file.ParentId != null)
        {
            var folder = await ProviderInfo.GetFolderByIdAsync(file.ParentId).ConfigureAwait(false);
            file.Title = await GetAvailableTitleAsync(file.Title, folder, IsExistAsync).ConfigureAwait(false);

            return ProviderInfo.ToFile(await ProviderInfo.CreateFileAsync(folder.ServerRelativeUrl + "/" + file.Title, fileStream).ConfigureAwait(false));

        }

        return null;
    }

    public Task<File<string>> ReplaceFileVersionAsync(File<string> file, Stream fileStream)
    {
        return SaveFileAsync(file, fileStream);
    }

    public Task DeleteFileAsync(string fileId)
    {
        return ProviderInfo.DeleteFileAsync(fileId);
    }

    public async Task<bool> IsExistAsync(string title, object folderId)
    {
        var files = await ProviderInfo.GetFolderFilesAsync(folderId).ConfigureAwait(false);

        return files.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<bool> IsExistAsync(string title, Microsoft.SharePoint.Client.Folder folder)
    {
        var files = await ProviderInfo.GetFolderFilesAsync(folder.ServerRelativeUrl).ConfigureAwait(false);

        return files.Any(item => item.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase));
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
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _sharePointDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            true)
            .ConfigureAwait(false);

        return moved.Id;
    }

    public async Task<string> MoveFileAsync(string fileId, string toFolderId)
    {
        var newFileId = await ProviderInfo.MoveFileAsync(fileId, toFolderId).ConfigureAwait(false);
        await UpdatePathInDBAsync(ProviderInfo.MakeId(fileId), newFileId).ConfigureAwait(false);

        return newFileId;
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

    public async Task<File<int>> CopyFileAsync(string fileId, int toFolderId)
    {
        var moved = await _crossDao.PerformCrossDaoFileCopyAsync(
            fileId, this, _sharePointDaoSelector.ConvertId,
            toFolderId, _fileDao, r => r,
            false)
            .ConfigureAwait(false);

        return moved;
    }

    public async Task<File<string>> CopyFileAsync(string fileId, string toFolderId)
    {
        return ProviderInfo.ToFile(await ProviderInfo.CopyFileAsync(fileId, toFolderId).ConfigureAwait(false));
    }


    public async Task<string> FileRenameAsync(File<string> file, string newTitle)
    {
        var newFileId = await ProviderInfo.RenameFileAsync(file.Id, newTitle).ConfigureAwait(false);
        await UpdatePathInDBAsync(ProviderInfo.MakeId(file.Id), newFileId).ConfigureAwait(false);

        return newFileId;
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

    public Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        return Task.FromResult(new ChunkedUploadSession<string>(FixId(file), contentLength) { UseChunks = false });
    }

    public async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream chunkStream, long chunkLength)
    {
        if (!uploadSession.UseChunks)
        {
            if (uploadSession.BytesTotal == 0)
            {
                uploadSession.BytesTotal = chunkLength;
            }

            uploadSession.File = await SaveFileAsync(uploadSession.File, chunkStream).ConfigureAwait(false);
            uploadSession.BytesUploaded = chunkLength;

            return uploadSession.File;
        }

        throw new NotImplementedException();
    }

    public Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        return Task.FromResult(0);
        //throw new NotImplementedException();
    }

    private File<string> FixId(File<string> file)
    {
        if (file.Id != null)
        {
            file.Id = ProviderInfo.MakeId(file.Id);
        }

        if (file.ParentId != null)
        {
            file.ParentId = ProviderInfo.MakeId(file.ParentId);
        }

        return file;
    }
}
