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

    public DocumentsBackupStorage(
        SetupInfo setupInfo,
        TenantManager tenantManager,
        SecurityContext securityContext,
        IDaoFactory daoFactory,
        StorageFactory storageFactory,
        IServiceProvider serviceProvider,
        TempPath tempPath)
    {
        _setupInfo = setupInfo;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _daoFactory = daoFactory;
        _storageFactory = storageFactory;
        _serviceProvider = serviceProvider;
        _tempPath = tempPath;
    }

    public async Task InitAsync(int tenantId)
    {
        _tenantId = tenantId;
        var store = await _storageFactory.GetStorageAsync(_tenantId, "files");
        _sessionHolder = new FilesChunkedUploadSessionHolder(_daoFactory, _tempPath, store, "", _setupInfo.ChunkUploadSize);
    }

    public async Task<string> UploadAsync(string folderId, string localPath, Guid userId)
    {
        await _tenantManager.SetCurrentTenantAsync(_tenantId);
        if (!userId.Equals(Guid.Empty))
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(userId);
        }
        else
        {
            var tenant = await _tenantManager.GetTenantAsync(_tenantId);
            await _securityContext.AuthenticateMeWithoutCookieAsync(tenant.OwnerId);
        }

        if (int.TryParse(folderId, out var fId))
        {
            return (await Upload(fId, localPath)).ToString();
        }

        return await Upload(folderId, localPath);
    }

    public async Task DownloadAsync(string fileId, string targetLocalPath)
    {
        await _tenantManager.SetCurrentTenantAsync(_tenantId);

        if (int.TryParse(fileId, out var fId))
        {
            await DownloadDaoAsync(fId, targetLocalPath);

            return;
        }

        await DownloadDaoAsync(fileId, targetLocalPath);
    }

    public async Task DeleteAsync(string fileId)
    {
        await _tenantManager.SetCurrentTenantAsync(_tenantId);

        if (int.TryParse(fileId, out var fId))
        {
            await DeleteDaoAsync(fId);

            return;
        }

        await DeleteDaoAsync(fileId);
    }

    public async Task<bool> IsExistsAsync(string fileId)
    {
        await _tenantManager.SetCurrentTenantAsync(_tenantId);
        if (int.TryParse(fileId, out var fId))
        {
            return await IsExistsDaoAsync(fId);
        }

        return await IsExistsDaoAsync(fileId);
    }

    public Task<string> GetPublicLinkAsync(string fileId)
    {
        return Task.FromResult(String.Empty);
    }

    private async Task<T> Upload<T>(T folderId, string localPath)
    {
        var folderDao = GetFolderDao<T>();
        var fileDao = await GetFileDaoAsync<T>();

        var folder = await folderDao.GetFolderAsync(folderId);
        if (folder == null)
        {
            throw new FileNotFoundException("Folder not found.");
        }

        await using var source = File.OpenRead(localPath);
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

    private async Task DownloadDaoAsync<T>(T fileId, string targetLocalPath)
    {
        await _tenantManager.SetCurrentTenantAsync(_tenantId);
        var fileDao = await GetFileDaoAsync<T>();
        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            throw new FileNotFoundException("File not found.");
        }

        await using var source = await fileDao.GetFileStreamAsync(file);
        await using var destination = File.OpenWrite(targetLocalPath);
        await source.CopyToAsync(destination);
    }

    private async Task DeleteDaoAsync<T>(T fileId)
    {
        var fileDao = await GetFileDaoAsync<T>();
        await fileDao.DeleteFileAsync(fileId);
    }

    private async Task<bool> IsExistsDaoAsync<T>(T fileId)
    {
        var fileDao = await GetFileDaoAsync<T>();
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
        await _tenantManager.SetCurrentTenantAsync(_tenantId);
        if (!userId.Equals(Guid.Empty))
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(userId);
        }
        else
        {
            var tenant = await _tenantManager.GetTenantAsync(_tenantId);
            await _securityContext.AuthenticateMeWithoutCookieAsync(tenant.OwnerId);
        }
        if (int.TryParse(storageBasePath, out var fId))
        {
            var uploadSession = await InitUploadChunkAsync(fId, title);
            var folderDao = GetFolderDao<int>();
            return await folderDao.CreateDataWriteOperatorAsync(fId, uploadSession, _sessionHolder);
        }
        else
        {
            var uploadSession = await InitUploadChunkAsync(storageBasePath, title);
            var folderDao = GetFolderDao<string>();
            return await folderDao.CreateDataWriteOperatorAsync(storageBasePath, uploadSession, _sessionHolder);
        }
    }

    private async Task<CommonChunkedUploadSession> InitUploadChunkAsync<T>(T folderId, string title)
    {
        var folderDao = GetFolderDao<T>();
        var fileDao = await GetFileDaoAsync<T>();

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

    private async Task<IFileDao<T>> GetFileDaoAsync<T>()
    {
        // hack: create storage using webConfigPath and put it into DataStoreCache
        // FileDao will use this storage and will not try to create the new one from service config
        await _storageFactory.GetStorageAsync(_tenantId, "files");
        return _daoFactory.GetFileDao<T>();
    }
}
