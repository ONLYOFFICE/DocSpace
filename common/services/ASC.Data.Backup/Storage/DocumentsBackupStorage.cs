/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.IO;

using ASC.Common;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Data;
//using File = ASC.Files.Core.File;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Backup.Storage
{
    public class DocumentsBackupStorage : IBackupStorage
    {
        private int TenantId { get; set; }
        private string WebConfigPath { get; set; }
        private TenantManager TenantManager { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private IDaoFactory DaoFactory { get; set; }
        private StorageFactory StorageFactory { get; set; }
        public IServiceProvider ServiceProvider { get; }

        public DocumentsBackupStorage(
            TenantManager tenantManager,
            SecurityContext securityContext,
            IDaoFactory daoFactory,
            StorageFactory storageFactory,
            IServiceProvider serviceProvider)
        {
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            DaoFactory = daoFactory;
            StorageFactory = storageFactory;
            ServiceProvider = serviceProvider;
        }
        public void Init(int tenantId, string webConfigPath)
        {
            TenantId = tenantId;
            WebConfigPath = webConfigPath;
        }
        public string Upload(string folderId, string localPath, Guid userId)
        {
            TenantManager.SetCurrentTenant(TenantId);
            if (!userId.Equals(Guid.Empty))
            {
                SecurityContext.AuthenticateMe(userId);
            }
            else
            {
                var tenant = TenantManager.GetTenant(TenantId);
                SecurityContext.AuthenticateMe(tenant.OwnerId);
            }

            if (int.TryParse(folderId, out var fId))
            {
                return Upload(fId, localPath).ToString();
            }

            return Upload(folderId, localPath);
        }

        public void Download(string fileId, string targetLocalPath)
        {
            TenantManager.SetCurrentTenant(TenantId);

            if (int.TryParse(fileId, out var fId))
            {
                DownloadDao(fId, targetLocalPath);
                return;
            }

            DownloadDao(fileId, targetLocalPath);
        }

        public void Delete(string fileId)
        {
            TenantManager.SetCurrentTenant(TenantId);

            if (int.TryParse(fileId, out var fId))
            {
                DeleteDao(fId);
                return;
            }

            DeleteDao(fileId);
        }

        public bool IsExists(string fileId)
        {
            TenantManager.SetCurrentTenant(TenantId);
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

            var folder = folderDao.GetFolder(folderId);
            if (folder == null)
            {
                throw new FileNotFoundException("Folder not found.");
            }

            using var source = File.OpenRead(localPath);
            var newFile = ServiceProvider.GetService<File<T>>();
            newFile.Title = Path.GetFileName(localPath);
            newFile.FolderID = folder.ID;
            newFile.ContentLength = source.Length;

            var file = fileDao.SaveFile(newFile, source);

            return file.ID;
        }

        private void DownloadDao<T>(T fileId, string targetLocalPath)
        {
            TenantManager.SetCurrentTenant(TenantId);
            var fileDao = GetFileDao<T>();
            var file = fileDao.GetFile(fileId);
            if (file == null)
            {
                throw new FileNotFoundException("File not found.");
            }

            using var source = fileDao.GetFileStream(file);
            using var destination = File.OpenWrite(targetLocalPath);
            source.CopyTo(destination);
        }

        private void DeleteDao<T>(T fileId)
        {
            var fileDao = GetFileDao<T>();
            fileDao.DeleteFile(fileId);
        }

        private bool IsExistsDao<T>(T fileId)
        {
            var fileDao = GetFileDao<T>();
            try
            {

                var file = fileDao.GetFile(fileId);
                return file != null && file.RootFolderType != FolderType.TRASH;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private IFolderDao<T> GetFolderDao<T>()
        {
            return DaoFactory.GetFolderDao<T>();
        }

        private IFileDao<T> GetFileDao<T>()
        {
            // hack: create storage using webConfigPath and put it into DataStoreCache
            // FileDao will use this storage and will not try to create the new one from service config
            StorageFactory.GetStorage(WebConfigPath, TenantId.ToString(), "files");
            return DaoFactory.GetFileDao<T>();
        }
    }
    public static class DocumentsBackupStorageExtension
    {
        public static DIHelper AddDocumentsBackupStorage(this DIHelper services)
        {
            services.TryAddScoped<DocumentsBackupStorage>();
            return services
                .AddTenantManagerService()
                .AddSecurityContextService()
                .AddStorageFactoryService()
                .AddDaoFactoryService();
        }
    }
}
