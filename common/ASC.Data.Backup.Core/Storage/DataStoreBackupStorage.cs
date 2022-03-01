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
using ASC.Data.Storage;

namespace ASC.Data.Backup.Storage
{
    [Scope]
    public class DataStoreBackupStorage : IBackupStorage
    {
        private string WebConfigPath { get; set; }
        private int Tenant { get; set; }
        private StorageFactory StorageFactory { get; set; }
        public DataStoreBackupStorage(StorageFactory storageFactory)
        {

            StorageFactory = storageFactory;
        }
        public void Init(int tenant, string webConfigPath)
        {
            WebConfigPath = webConfigPath;
            Tenant = tenant;
        }
        public string Upload(string storageBasePath, string localPath, Guid userId)
        {
            using var stream = File.OpenRead(localPath);
            var storagePath = Path.GetFileName(localPath);
            GetDataStore().SaveAsync("", storagePath, stream).Wait();
            return storagePath;
        }

        public void Download(string storagePath, string targetLocalPath)
        {
            using var source = GetDataStore().GetReadStreamAsync("", storagePath).Result;
            using var destination = File.OpenWrite(targetLocalPath);
            source.CopyTo(destination);
        }

        public void Delete(string storagePath)
        {
            var dataStore = GetDataStore();
            if (dataStore.IsFileAsync("", storagePath).Result)
            {
                dataStore.DeleteAsync("", storagePath).Wait();
            }
        }

        public bool IsExists(string storagePath)
        {
            return GetDataStore().IsFileAsync("", storagePath).Result;
        }

        public string GetPublicLink(string storagePath)
        {
            return GetDataStore().GetPreSignedUriAsync("", storagePath, TimeSpan.FromDays(1), null).Result.ToString();
        }

        protected virtual IDataStore GetDataStore()
        {
            return StorageFactory.GetStorage(WebConfigPath, Tenant.ToString(), "backup", null);
        }
    }
}
