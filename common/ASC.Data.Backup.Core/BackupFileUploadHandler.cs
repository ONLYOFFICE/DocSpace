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
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASC.Data.Backup
{
    public class BackupFileUploadHandler
    {
        private const long MaxBackupFileSize = 1024L * 1024L * 1024L;

        public BackupFileUploadHandler(RequestDelegate next)
        {

        }
        public async Task Invoke(HttpContext context,
            PermissionContext permissionContext,
            BackupAjaxHandler backupAjaxHandler)
        {
            FileUploadResult result = null;
            try
            {
            if (context.Request.Form.Files.Count == 0)
            {
                result = Error("No files.");
            }

            if (!permissionContext.CheckPermissions(SecutiryConstants.EditPortalSettings))
            {
                result = Error("Access denied.");
            }

            var file = context.Request.Form.Files[0];

            if (file.Length <= 0 || file.Length > MaxBackupFileSize)
            {
                result = Error($"File size must be greater than 0 and less than {MaxBackupFileSize} bytes");
            }

            
                var filePath = backupAjaxHandler.GetTmpFilePath();

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var fileStream = File.Create(filePath))
                {
                    await file.CopyToAsync(fileStream);
                }

                result = Success();
            }
            catch (Exception error)
            {
                result = Error(error.Message);
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

        private FileUploadResult Success()
        {
            return new FileUploadResult
            {
                Success = true
            };
        }

        private FileUploadResult Error(string messageFormat, params object[] args)
        {
            return new FileUploadResult
            {
                Success = false,
                Message = string.Format(messageFormat, args)
            };
        }
    }

    public static class BackupFileUploadHandlerExtensions
    {
        public static IApplicationBuilder UseBackupFileUploadHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BackupFileUploadHandler>();
        }
    }
}