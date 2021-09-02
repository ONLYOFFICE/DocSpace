/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.IO;

using ASC.Common;
using ASC.Core;

using Microsoft.AspNetCore.Http;

namespace ASC.Web.Studio.Core.Backup
{
    [Scope]
    public class BackupFileUploadHandler
    {
        private const long MaxBackupFileSize = 1024L * 1024L * 1024L;
        private const string BackupTempFolder = "backup";
        private const string BackupFileName = "backup.tmp";

        private PermissionContext PermissionContext { get; }
        private TempPath TempPath { get; }
        private TenantManager TenantManager { get; }

        public BackupFileUploadHandler(
            PermissionContext permissionContext,
            TempPath tempPath,
            TenantManager tenantManager)
        {
            PermissionContext = permissionContext;
            TempPath = tempPath;
            TenantManager = tenantManager;
        }

        public string ProcessUpload(IFormFile file)
        {
            if (file == null)
            {
                return "No files.";
            }

            if (!PermissionContext.CheckPermissions(SecutiryConstants.EditPortalSettings))
            {
                return "Access denied.";
            }

            if (file.Length <= 0 || file.Length > MaxBackupFileSize)
            {
                return $"File size must be greater than 0 and less than {MaxBackupFileSize} bytes";
            }

            try
            {
                var filePath = GetFilePath();

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var fileStream = File.Create(filePath))
                {
                    file.CopyTo(fileStream);
                }

                return string.Empty;
            }
            catch (Exception error)
            {
                return error.Message;
            }
        }

        internal string GetFilePath()
        {
            var folder = Path.Combine(TempPath.GetTempPath(), BackupTempFolder, TenantManager.GetCurrentTenant().TenantId.ToString());

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return Path.Combine(folder, BackupFileName);
        }
    }
}