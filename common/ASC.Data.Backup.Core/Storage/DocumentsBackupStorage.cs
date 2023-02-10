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

using ASC.Web.Files.Utils;

namespace ASC.Data.Backup.Storage;

[Scope]
public class DocumentsBackupStorage : IBackupStorage, IGetterWriteOperator
{
    private int _tenantId;
    private readonly SetupInfo _setupInfo;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly IDaoFactory _daoFactory;
    private readonly StorageFactory _storageFactory;
    private readonly IServiceProvider _serviceProvider;
    private FilesChunkedUploadSessionHolder _sessionHolder;
    private readonly TempPath _tempPath;
    private readonly ILogger<DocumentsBackupStorage> _logger;

    public DocumentsBackupStorage(
        SetupInfo setupInfo,
        TenantManager tenantManager,
        SecurityContext securityContext,
        IDaoFactory daoFactory,
        StorageFactory storageFactory,
        IServiceProvider serviceProvider,
        TempPath tempPath,
        ILogger<DocumentsBackupStorage> logger)
    {
        _setupInfo = setupInfo;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _daoFactory = daoFactory;
        _storageFactory = storageFactory;
        _serviceProvider = serviceProvider;
        _tempPath = tempPath;
        _logger = logger;
    }

    public void Init(int tenantId)
    {
        _tenantId = tenantId;
        var store = _storageFactory.GetStorage(_tenantId, "files");
        _sessionHolder = new FilesChunkedUploadSessionHolder(_daoFactory, _tempPath, _logger, store, "", _setupInfo.ChunkUploadSize);
    }

    public async Task<string> Upload(string folderId, string localPath, Guid userId)
    {
        _tenantManager.SetCurrentTenant(_tenantId);
        if (!userId.Equals(Guid.Empty))
        {
            _securityContext.AuthenticateMeWithoutCookie(userId);
        }
        else
        {
            var tenant = _tenantManager.GetTenant(_tenantId);
            _securityContext.AuthenticateMeWithoutCookie(tenant.OwnerId);
        }

        if (int.TryParse(folderId, out var fId))
        {
            return (await Upload(fId, localPath)).ToString();
        }

        return await Upload(folderId, localPath);
    }

    public async Task Download(string fileId, string targetLocalPath)
    {
        _tenantManager.SetCurrentTenant(_tenantId);

        if (int.TryParse(fileId, out var fId))
        {
            await DownloadDao(fId, targetLocalPath);

            return;
        }

        await DownloadDao(fileId, targetLocalPath);
    }

    public async Task Delete(string fileId)
    {
        _tenantManager.SetCurrentTenant(_tenantId);

        if (int.TryParse(fileId, out var fId))
        {
            await DeleteDao(fId);

            return;
        }

        await DeleteDao(fileId);
    }

    public async Task<bool> IsExists(string fileId)
    {
        _tenantManager.SetCurrentTenant(_tenantId);
        if (int.TryParse(fileId, out var fId))
        {
            return await IsExistsDao(fId);
        }

        return await IsExistsDao(fileId);
    }

    public Task<string> GetPublicLink(string fileId)
    {
        return Task.FromResult(String.Empty);
    }

    private async Task<T> Upload<T>(T folderId, string localPath)
    {
        var folderDao = GetFolderDao<T>();
        var fileDao = GetFileDao<T>();

        var folder = await folderDao.GetFolderAsync(folderId);
        if (folder == null)
        {
            throw new FileNotFoundException("Folder not found.");
        }

        using var source = File.OpenRead(localPath);
        var newFile = _serviceProvider.GetService<File<T>>();
        newFile.Title = Path.GetFileName(localPath);
        newFile.ParentId = folder.Id;
        newFile.ContentLength = source.Length;

        File<T> file = null;
        var buffer = new byte[_setupInfo.ChunkUploadSize];
        var chunkedUploadSession = await fileDao.CreateUploadSessionAsync(newFile, source.Length);
        chunkedUploadSession.CheckQuota = false;

        int bytesRead;

        while ((bytesRead = await source.ReadAsync(buffer, 0, (int)_setupInfo.ChunkUploadSize)) > 0)
        {
            using (var theMemStream = new MemoryStream())
            {
                await theMemStream.WriteAsync(buffer, 0, bytesRead);
                theMemStream.Position = 0;
                file = await fileDao.UploadChunkAsync(chunkedUploadSession, theMemStream, bytesRead);
            }
        }

        return file.Id;
    }

    private async Task DownloadDao<T>(T fileId, string targetLocalPath)
    {
        _tenantManager.SetCurrentTenant(_tenantId);
        var fileDao = GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            throw new FileNotFoundException("File not found.");
        }

        using var source = await fileDao.GetFileStreamAsync(file);
        using var destination = File.OpenWrite(targetLocalPath);
        await source.CopyToAsync(destination);
    }

    private async Task DeleteDao<T>(T fileId)
    {
        var fileDao = GetFileDao<T>();
        await fileDao.DeleteFileAsync(fileId);
    }

    private async Task<bool> IsExistsDao<T>(T fileId)
    {
        var fileDao = GetFileDao<T>();
        try
        {

            var file = await fileDao.GetFileAsync(fileId);

            return file != null && file.RootFolderType != FolderType.TRASH;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IDataWriteOperator> GetWriteOperatorAsync(string storageBasePath, string title, Guid userId)
    {
        _tenantManager.SetCurrentTenant(_tenantId);
        if (!userId.Equals(Guid.Empty))
        {
            _securityContext.AuthenticateMeWithoutCookie(userId);
        }
        else
        {
            var tenant = _tenantManager.GetTenant(_tenantId);
            _securityContext.AuthenticateMeWithoutCookie(tenant.OwnerId);
        }
        if (int.TryParse(storageBasePath, out var fId))
        {
            var uploadSession = await InitUploadChunkAsync(fId, title);
            var folderDao = GetFolderDao<int>();
            return folderDao.CreateDataWriteOperator(fId, uploadSession, _sessionHolder);
        }
        else
        {
            var uploadSession = await InitUploadChunkAsync(storageBasePath, title);
            var folderDao = GetFolderDao<string>();
            return folderDao.CreateDataWriteOperator(storageBasePath, uploadSession, _sessionHolder);
        }
    }

    private async Task<CommonChunkedUploadSession> InitUploadChunkAsync<T>(T folderId, string title)
    {
        var folderDao = GetFolderDao<T>();
        var fileDao = GetFileDao<T>();

        var folder = await folderDao.GetFolderAsync(folderId);
        var newFile = _serviceProvider.GetService<File<T>>();

        newFile.Title = title;
        newFile.ParentId = folder.Id;

        var chunkedUploadSession = await fileDao.CreateUploadSessionAsync(newFile, -1);
        chunkedUploadSession.CheckQuota = false;
        return chunkedUploadSession;
    }

    private IFolderDao<T> GetFolderDao<T>()
    {
        return _daoFactory.GetFolderDao<T>();
    }

    private IFileDao<T> GetFileDao<T>()
    {
        // hack: create storage using webConfigPath and put it into DataStoreCache
        // FileDao will use this storage and will not try to create the new one from service config
        _storageFactory.GetStorage(_tenantId, "files");
        return _daoFactory.GetFileDao<T>();
    }
}
