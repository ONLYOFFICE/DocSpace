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
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.CRM.Core;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASC.Web.CRM.HttpHandlers
{
    public class ImportFileHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ImportFileHandlerMiddleware(
                                RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context,
                                WebItemSecurity webItemSecurity,
                                CrmSecurity crmSecurity,
                                Global global,
                                ImportFromCSV importFromCSV)
        {
            if (!webItemSecurity.IsAvailableForMe(ProductEntryPoint.ID))
                throw crmSecurity.CreateSecurityException();

            return InternalInvoke(context, webItemSecurity, crmSecurity, global, importFromCSV);
        }

        private async Task InternalInvoke(HttpContext context,
                                WebItemSecurity webItemSecurity,
                                CrmSecurity crmSecurity,
                                Global global,
                                ImportFromCSV importFromCSV)
        {
            var fileUploadResult = new FileUploadResult();

            if (context.Request.Form.Files.Count == 0)
            {
                await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
            }

            var fileName = context.Request.Form.Files[0].FileName;
            var contentLength = context.Request.Form.Files[0].Length;

            String assignedPath;

            await global.GetStore().SaveTempAsync("temp", out assignedPath, context.Request.Form.Files[0].OpenReadStream());

            var jObject = importFromCSV.GetInfo(context.Request.Form.Files[0].OpenReadStream(), context.Request.Form["importSettings"]);

            var jsonDocumentAsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jObject.ToString());

            jsonDocumentAsDictionary.Add("assignedPath", assignedPath);

            fileUploadResult.Success = true;
            fileUploadResult.Data = Global.EncodeTo64(JsonSerializer.Serialize(jsonDocumentAsDictionary));

            await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
        }
    }

    public static class ImportFileHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseImportFileHandlerHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImportFileHandlerMiddleware>();
        }
    }
}