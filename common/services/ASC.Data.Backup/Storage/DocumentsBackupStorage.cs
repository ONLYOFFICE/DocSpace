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


using ASC.Core;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using System;
using System.IO;
using System.Net;
//using File = ASC.Files.Core.File;//тут
using IoFile = System.IO.File;

namespace ASC.Data.Backup.Storage
{
    public class DocumentsBackupStorage : IBackupStorage
    {
        private readonly int tenantId;
        private readonly string webConfigPath;
        private readonly TenantManager tenantManager;
        private readonly SecurityContext securityContext;
        private readonly IDaoFactory daoFactory;
        private readonly StorageFactory storageFactory;
        public DocumentsBackupStorage(int tenantId, string webConfigPath, TenantManager tenantManager, SecurityContext securityContext, IDaoFactory daoFactory, StorageFactory storageFactory)
        {
            this.tenantId = tenantId;
            this.webConfigPath = webConfigPath;
            this.tenantManager = tenantManager;
            this.securityContext = securityContext;
            this.daoFactory = daoFactory;
            this.storageFactory = storageFactory;
        }

        public string Upload(string folderId, string localPath, Guid userId)
        {
            /*  tenantManager.SetCurrentTenant(tenantId);
              if (!userId.Equals(Guid.Empty))
              {
                  securityContext.AuthenticateMe(userId);
              }
              else
              {
                  var tenant = tenantManager.GetTenant(tenantId);
                  securityContext.AuthenticateMe(tenant.OwnerId);
              }

              using (var folderDao = GetFolderDao())
              using (var fileDao = GetFileDao())
              {
                  var folder = folderDao.GetFolder(folderId);
                  if (folder == null)
                  {
                      throw new FileNotFoundException("Folder not found.");
                  }
                  using (var source = IoFile.OpenRead(localPath))
                  {
                      var file = fileDao.SaveFile(
                          new File
                              {
                                  Title = Path.GetFileName(localPath),
                                  FolderID = folder.ID,
                                  ContentLength = source.Length
                              },
                          source);

                      return Convert.ToString(file.ID);
                  }
              }*/
            return null;
        }

        public void Download(string fileId, string targetLocalPath)
        {
           /* tenantManager.SetCurrentTenant(tenantId);
            using (var fileDao = GetFileDao())
            {
                var file = fileDao.GetFile(fileId);
                if (file == null)
                {
                    throw new FileNotFoundException("File not found.");
                }
                using (var source = fileDao.GetFileStream(file))
                using (var destination = IoFile.OpenWrite(targetLocalPath))
                {
                    source.StreamCopyTo(destination);
                }
            }*/
        }

        public void Delete(string fileId)
        {
          /*  tenantManager.SetCurrentTenant(tenantId);
            using (var fileDao = GetFileDao())
            {
                fileDao.DeleteFile(fileId);
            }*/
        }

        public bool IsExists(string fileId)
        {
            /*  tenantManager.SetCurrentTenant(tenantId);
              using (var fileDao = GetFileDao())
              {
                  try
                  {

                      var file = fileDao.GetFile(fileId);
                      return file != null && file.RootFolderType != FolderType.TRASH;
                  }
                  catch (Exception)
                  {
                      return false;
                  }
              }*/
            return false;
        }

        public string GetPublicLink(string fileId)
        {
            return string.Empty;
        }

     /*   private IFolderDao GetFolderDao()//тут
+        {
            return daoFactory.GetFolderDao;
        }

        private IFileDao GetFileDao()//тут
        {
            // hack: create storage using webConfigPath and put it into DataStoreCache
            // FileDao will use this storage and will not try to create the new one from service config
            storageFactory.GetStorage(webConfigPath, tenantId.ToString(), "files");
            return daoFactory.GetFileDao;
        }*/
    }
}
