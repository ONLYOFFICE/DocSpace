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

namespace ASC.Web.Files.Utils;

[Scope]
public class FileUploader
{
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FileUtility _fileUtility;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly SetupInfo _setupInfo;
    private readonly MaxTotalSizeStatistic _maxTotalSizeStatistic;
    private readonly FileMarker _fileMarker;
    private readonly FileConverter _fileConverter;
    private readonly IDaoFactory _daoFactory;
    private readonly Global _global;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FilesMessageService _filesMessageService;
    private readonly FileSecurity _fileSecurity;
    private readonly EntryManager _entryManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChunkedUploadSessionHolder _chunkedUploadSessionHolder;
    private readonly FileTrackerHelper _fileTracker;
    private readonly SocketManager _socketManager;

    public FileUploader(
        FilesSettingsHelper filesSettingsHelper,
        FileUtility fileUtility,
        UserManager userManager,
        TenantManager tenantManager,
        AuthContext authContext,
        SetupInfo setupInfo,
        MaxTotalSizeStatistic maxTotalSizeStatistic,
        FileMarker fileMarker,
        FileConverter fileConverter,
        IDaoFactory daoFactory,
        Global global,
        FilesLinkUtility filesLinkUtility,
        FilesMessageService filesMessageService,
        FileSecurity fileSecurity,
        EntryManager entryManager,
        IServiceProvider serviceProvider,
        ChunkedUploadSessionHolder chunkedUploadSessionHolder,
        FileTrackerHelper fileTracker,
        SocketManager socketManager)
    {
        _filesSettingsHelper = filesSettingsHelper;
        _fileUtility = fileUtility;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _setupInfo = setupInfo;
        _maxTotalSizeStatistic = maxTotalSizeStatistic;
        _fileMarker = fileMarker;
        _fileConverter = fileConverter;
        _daoFactory = daoFactory;
        _global = global;
        _filesLinkUtility = filesLinkUtility;
        _filesMessageService = filesMessageService;
        _fileSecurity = fileSecurity;
        _entryManager = entryManager;
        _serviceProvider = serviceProvider;
        _chunkedUploadSessionHolder = chunkedUploadSessionHolder;
        _fileTracker = fileTracker;
        _socketManager = socketManager;
    }

    public async Task<File<T>> ExecAsync<T>(T folderId, string title, long contentLength, Stream data)
    {
        return await ExecAsync(folderId, title, contentLength, data, !_filesSettingsHelper.UpdateIfExist);
    }

    public async Task<File<T>> ExecAsync<T>(T folderId, string title, long contentLength, Stream data, bool createNewIfExist, bool deleteConvertStatus = true)
    {
        if (contentLength <= 0)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);
        }

        var file = await VerifyFileUploadAsync(folderId, title, contentLength, !createNewIfExist);

        var dao = _daoFactory.GetFileDao<T>();
        file = await dao.SaveFileAsync(file, data);

        var linkDao = _daoFactory.GetLinkDao();
        await linkDao.DeleteAllLinkAsync(file.Id.ToString());

        await _fileMarker.MarkAsNewAsync(file);

        if (_fileConverter.EnableAsUploaded && _fileConverter.MustConvert(file))
        {
            await _fileConverter.ExecAsynchronouslyAsync(file, deleteConvertStatus);
        }

        return file;
    }

    public async Task<File<T>> VerifyFileUploadAsync<T>(T folderId, string fileName, bool updateIfExists, string relativePath = null)
    {
        fileName = Global.ReplaceInvalidCharsAndTruncate(fileName);

        if (_global.EnableUploadFilter && !_fileUtility.ExtsUploadable.Contains(FileUtility.GetFileExtension(fileName)))
        {
            throw new NotSupportedException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
        }

        folderId = await GetFolderIdAsync(folderId, string.IsNullOrEmpty(relativePath) ? null : relativePath.Split('/').ToList());

        var fileDao = _daoFactory.GetFileDao<T>();
        var file = await fileDao.GetFileAsync(folderId, fileName);

        if (updateIfExists && await CanEditAsync(file))
        {
            file.Title = fileName;
            file.ConvertedType = null;
            file.Comment = FilesCommonResource.CommentUpload;
            file.Version++;
            file.VersionGroup++;
            file.Encrypted = false;
            file.ThumbnailStatus = Thumbnail.Waiting;

            return file;
        }

        var newFile = _serviceProvider.GetService<File<T>>();
        newFile.ParentId = folderId;
        newFile.Title = fileName;

        return newFile;
    }

    public async Task<File<T>> VerifyFileUploadAsync<T>(T folderId, string fileName, long fileSize, bool updateIfExists)
    {
        if (fileSize <= 0)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);
        }

        var maxUploadSize = await GetMaxFileSizeAsync(folderId);

        if (fileSize > maxUploadSize)
        {
            throw FileSizeComment.GetFileSizeException(maxUploadSize);
        }

        var file = await VerifyFileUploadAsync(folderId, fileName, updateIfExists);
        file.ContentLength = fileSize;

        return file;
    }

    private async Task<bool> CanEditAsync<T>(File<T> file)
    {
        return file != null
               && await _fileSecurity.CanEditAsync(file)
               && !await _userManager.IsUserAsync(_authContext.CurrentAccount.ID)
               && !await _entryManager.FileLockedForMeAsync(file.Id)
               && !_fileTracker.IsEditing(file.Id)
               && file.RootFolderType != FolderType.TRASH
               && !file.Encrypted;
    }

    private async Task<T> GetFolderIdAsync<T>(T folderId, IList<string> relativePath)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var folder = await folderDao.GetFolderAsync(folderId);

        if (folder == null)
        {
            throw new DirectoryNotFoundException(FilesCommonResource.ErrorMassage_FolderNotFound);
        }

        if (folder.FolderType == FolderType.VirtualRooms || folder.FolderType == FolderType.Archive || !await _fileSecurity.CanCreateAsync(folder))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
        }

        if (relativePath != null && relativePath.Count > 0)
        {
            var subFolderTitle = Global.ReplaceInvalidCharsAndTruncate(relativePath.FirstOrDefault());

            if (!string.IsNullOrEmpty(subFolderTitle))
            {
                folder = await folderDao.GetFolderAsync(subFolderTitle, folder.Id);

                if (folder == null)
                {
                    var newFolder = _serviceProvider.GetService<Folder<T>>();
                    newFolder.Title = subFolderTitle;
                    newFolder.ParentId = folderId;

                    folderId = await folderDao.SaveFolderAsync(newFolder);
                    folder = await folderDao.GetFolderAsync(folderId);
                    await _socketManager.CreateFolderAsync(folder);
                    _ = _filesMessageService.SendAsync(folder, MessageAction.FolderCreated, folder.Title);
                }

                folderId = folder.Id;

                relativePath.RemoveAt(0);
                folderId = await GetFolderIdAsync(folderId, relativePath);
            }
        }

        return folderId;
    }

    #region chunked upload

    public async Task<File<T>> VerifyChunkedUploadAsync<T>(T folderId, string fileName, long fileSize, bool updateIfExists, string relativePath = null)
    {
        var maxUploadSize = await GetMaxFileSizeAsync(folderId, true);

        if (fileSize > maxUploadSize)
        {
            throw FileSizeComment.GetFileSizeException(maxUploadSize);
        }

        var file = await VerifyFileUploadAsync(folderId, fileName, updateIfExists, relativePath);
        file.ContentLength = fileSize;

        return file;
    }

    public async Task<File<T>> VerifyChunkedUploadForEditing<T>(T fileId, long fileSize)
    {
        var fileDao = _daoFactory.GetFileDao<T>();

        var file = await fileDao.GetFileAsync(fileId);

        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        var maxUploadSize = await GetMaxFileSizeAsync(file.ParentId, true);

        if (fileSize > maxUploadSize)
        {
            throw FileSizeComment.GetFileSizeException(maxUploadSize);
        }

        if (!await CanEditAsync(file))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        }

        file.ConvertedType = null;
        file.Comment = FilesCommonResource.CommentUpload;
        file.Encrypted = false;
        file.ThumbnailStatus = Thumbnail.Waiting;

        file.ContentLength = fileSize;

        return file;
    }

    public async Task<ChunkedUploadSession<T>> InitiateUploadAsync<T>(T folderId, T fileId, string fileName, long contentLength, bool encrypted, bool keepVersion = false, ApiDateTime createOn = null)
    {
        var file = _serviceProvider.GetService<File<T>>();
        file.Id = fileId;
        file.ParentId = folderId;
        file.Title = fileName;
        file.ContentLength = contentLength;
        file.CreateOn = createOn;

        var dao = _daoFactory.GetFileDao<T>();
        var uploadSession = await dao.CreateUploadSessionAsync(file, contentLength);

        uploadSession.Expired = uploadSession.Created + ChunkedUploadSessionHolder.SlidingExpiration;
        uploadSession.Location = _filesLinkUtility.GetUploadChunkLocationUrl(uploadSession.Id);
        uploadSession.TenantId = await _tenantManager.GetCurrentTenantIdAsync();
        uploadSession.UserId = _authContext.CurrentAccount.ID;
        uploadSession.FolderId = folderId;
        uploadSession.CultureName = CultureInfo.CurrentUICulture.Name;
        uploadSession.Encrypted = encrypted;
        uploadSession.KeepVersion = keepVersion;

        await _chunkedUploadSessionHolder.StoreSessionAsync(uploadSession);

        return uploadSession;
    }

    public async Task<ChunkedUploadSession<T>> UploadChunkAsync<T>(string uploadId, Stream stream, long chunkLength)
    {
        var uploadSession = await _chunkedUploadSessionHolder.GetSessionAsync<T>(uploadId);
        uploadSession.Expired = DateTime.UtcNow + ChunkedUploadSessionHolder.SlidingExpiration;

        if (chunkLength <= 0)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);
        }

        if (chunkLength > _setupInfo.ChunkUploadSize)
        {
            throw FileSizeComment.GetFileSizeException(await _setupInfo.MaxUploadSize(_tenantManager, _maxTotalSizeStatistic));
        }

        var maxUploadSize = await GetMaxFileSizeAsync(uploadSession.FolderId, uploadSession.BytesTotal > 0);

        if (uploadSession.BytesUploaded + chunkLength > maxUploadSize)
        {
            await AbortUploadAsync(uploadSession);

            throw FileSizeComment.GetFileSizeException(maxUploadSize);
        }

        var dao = _daoFactory.GetFileDao<T>();
        await dao.UploadChunkAsync(uploadSession, stream, chunkLength);

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal || uploadSession.LastChunk)
        {
            var linkDao = _daoFactory.GetLinkDao();
            await linkDao.DeleteAllLinkAsync(uploadSession.File.Id.ToString());

            await _fileMarker.MarkAsNewAsync(uploadSession.File);
            await _chunkedUploadSessionHolder.RemoveSessionAsync(uploadSession);
        }
        else
        {
            await _chunkedUploadSessionHolder.StoreSessionAsync(uploadSession);
        }

        return uploadSession;
    }

    public async Task AbortUploadAsync<T>(string uploadId)
    {
        await AbortUploadAsync(await _chunkedUploadSessionHolder.GetSessionAsync<T>(uploadId));
    }

    private async Task AbortUploadAsync<T>(ChunkedUploadSession<T> uploadSession)
    {
        await _daoFactory.GetFileDao<T>().AbortUploadSessionAsync(uploadSession);

        await _chunkedUploadSessionHolder.RemoveSessionAsync(uploadSession);
    }

    private async Task<long> GetMaxFileSizeAsync<T>(T folderId, bool chunkedUpload = false)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();

        return await folderDao.GetMaxUploadSizeAsync(folderId, chunkedUpload);
    }

    #endregion
}
