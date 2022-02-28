/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
using System.Text.Json;
using System.Threading.Tasks;

using ASC.CRM.Core.Dao;
using ASC.CRM.Resources;
using ASC.Files.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.CRM.HttpHandlers
{
    public class FileUploaderHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public FileUploaderHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context,
                                   SetupInfo setupInfo,
                                   DaoFactory daoFactory,
                                   FileSizeComment fileSizeComment,
                                   IServiceProvider serviceProvider,
                                   TenantExtra tenantExtra,
                                   TenantStatisticsProvider tenantStatisticsProvider)
        {
            context.Request.EnableBuffering();

            var fileUploadResult = new FileUploadResult();

            if (context.Request.Form.Files.Count == 0)
            {
                await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
            }

            var fileName = context.Request.Form.Files[0].FileName;
            var contentLength = context.Request.Form.Files[0].Length;

            if (String.IsNullOrEmpty(fileName) || contentLength == 0)
                throw new InvalidOperationException(CRMErrorsResource.InvalidFile);

            if (0 < setupInfo.MaxUploadSize(tenantExtra, tenantStatisticsProvider) && setupInfo.MaxUploadSize(tenantExtra, tenantStatisticsProvider) < contentLength)
                throw fileSizeComment.FileSizeException;

            fileName = fileName.LastIndexOf('\\') != -1
               ? fileName.Substring(fileName.LastIndexOf('\\') + 1)
               : fileName;

            var document = serviceProvider.GetService<File<int>>();

            document.Title = fileName;
            document.FolderID = await daoFactory.GetFileDao().GetRootAsync();
            document.ContentLength = contentLength;

            document = await daoFactory.GetFileDao().SaveFileAsync(document, context.Request.Form.Files[0].OpenReadStream());

            fileUploadResult.Data = document.ID;
            fileUploadResult.FileName = document.Title;
            fileUploadResult.FileURL = document.DownloadUrl;
            fileUploadResult.Success = true;

            await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
        }
    }

    public static class FileUploaderHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseFileUploaderHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FileUploaderHandlerMiddleware>();
        }
    }
}