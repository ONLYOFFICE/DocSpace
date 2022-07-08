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
public class DocumentsBackupStorage : IBackupStorage
{
    private int _tenantId;
    private string _webConfigPath;
    private readonly SetupInfo _setupInfo;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly IDaoFactory _daoFactory;
    private readonly StorageFactory _storageFactory;
    private readonly IServiceProvider _serviceProvider;

    public DocumentsBackupStorage(
        SetupInfo setupInfo,
        TenantManager tenantManager,
        SecurityContext securityContext,
        IDaoFactory daoFactory,
        StorageFactory storageFactory,
        IServiceProvider serviceProvider)
    {
        _setupInfo = setupInfo;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _daoFactory = daoFactory;
        _storageFactory = storageFactory;
        _serviceProvider = serviceProvider;
    }

    public void Init(int tenantId, string webConfigPath)
    {
        _tenantId = tenantId;
        _webConfigPath = webConfigPath;
    }

    public string Upload(string folderId, string localPath, Guid userId)
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
            return Upload(fId, localPath).ToString();
        }

        return Upload(folderId, localPath);
    }

    public void Download(string fileId, string targetLocalPath)
    {
        _tenantManager.SetCurrentTenant(_tenantId);

        if (int.TryParse(fileId, out var fId))
        {
            DownloadDao(fId, targetLocalPath);

            return;
        }

        DownloadDao(fileId, targetLocalPath);
    }

    public void Delete(string fileId)
    {
        _tenantManager.SetCurrentTenant(_tenantId);

        if (int.TryParse(fileId, out var fId))
        {
            DeleteDao(fId);

            return;
        }

        DeleteDao(fileId);
    }

    public bool IsExists(string fileId)
    {
        _tenantManager.SetCurrentTenant(_tenantId);
        if (int.TryParse(fileId, out var fId))
        {
            return IsExistsDao(fId);
        }

        return IsExistsDao(fileId);
    }

    public string GetPublicLink(string fileId)
    {
        return string.Empty;
    }

    private T Upload<T>(T folderId, string localPath)
    {
        var folderDao = GetFolderDao<T>();
        var fileDao = GetFileDao<T>();

        var folder = folderDao.GetFolderAsync(folderId).Result;
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
        var chunkedUploadSession = fileDao.CreateUploadSessionAsync(newFile, source.Length).Result;
        chunkedUploadSession.CheckQuota = false;

        int bytesRead;

        while ((bytesRead = source.Read(buffer, 0, (int)_setupInfo.ChunkUploadSize)) > 0)
        {
            using (var theMemStream = new MemoryStream())
            {
                theMemStream.Write(buffer, 0, bytesRead);
                theMemStream.Position = 0;
                file = fileDao.UploadChunkAsync(chunkedUploadSession, theMemStream, bytesRead).Result;
            }
        }

        return file.Id;
    }

    private void DownloadDao<T>(T fileId, string targetLocalPath)
    {
        _tenantManager.SetCurrentTenant(_tenantId);
        var fileDao = GetFileDao<T>();
        var file = fileDao.GetFileAsync(fileId).Result;
        if (file == null)
        {
            throw new FileNotFoundException("File not found.");
        }

        using var source = fileDao.GetFileStreamAsync(file).Result;
        using var destination = File.OpenWrite(targetLocalPath);
        source.CopyTo(destination);
    }

    private void DeleteDao<T>(T fileId)
    {
        var fileDao = GetFileDao<T>();
        fileDao.DeleteFileAsync(fileId).Wait();
    }

    private bool IsExistsDao<T>(T fileId)
    {
        var fileDao = GetFileDao<T>();
        try
        {

            var file = fileDao.GetFileAsync(fileId).Result;

            return file != null && file.RootFolderType != FolderType.TRASH;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private IFolderDao<T> GetFolderDao<T>()
    {
        return _daoFactory.GetFolderDao<T>();
    }

    private IFileDao<T> GetFileDao<T>()
    {
        // hack: create storage using webConfigPath and put it into DataStoreCache
        // FileDao will use this storage and will not try to create the new one from service config
        _storageFactory.GetStorage(_webConfigPath, _tenantId.ToString(), "files");
        return _daoFactory.GetFileDao<T>();
    }
}
