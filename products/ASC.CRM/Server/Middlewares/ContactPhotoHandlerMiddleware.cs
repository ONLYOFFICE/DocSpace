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

using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Resources;
using ASC.MessagingSystem;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Utility;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASC.Web.CRM.HttpHandlers
{
    public class ContactPhotoHandlerMiddleware
    {
        public ContactPhotoHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private readonly RequestDelegate _next;

        public System.Threading.Tasks.Task Invoke(HttpContext context,
                                 SetupInfo setupInfo,
                                 CrmSecurity crmSecurity,
                                 FileSizeComment fileSizeComment,
                                 WebItemSecurity webItemSecurity,
                                 MessageTarget messageTarget,
                                 MessageService messageService,
                                 DaoFactory daoFactory,
                                 ContactPhotoManager contactPhotoManager){

            if (!webItemSecurity.IsAvailableForMe(ProductEntryPoint.ID))
                throw crmSecurity.CreateSecurityException();

            context.Request.EnableBuffering();

            var contactId = Convert.ToInt32(context.Request.Form["contactID"]);

            Contact contact = null;

            if (contactId != 0)
            {
                contact = daoFactory.GetContactDao().GetByID(contactId);

                if (!crmSecurity.CanEdit(contact))
                    throw crmSecurity.CreateSecurityException();
            }

            return InternalInvoke(context, setupInfo, fileSizeComment, messageTarget, messageService, contactPhotoManager, contact, contactId);
        }

        private async System.Threading.Tasks.Task InternalInvoke(HttpContext context,
                                 SetupInfo setupInfo,
                                 FileSizeComment fileSizeComment,
                                 MessageTarget messageTarget,
                                 MessageService messageService,
                                 ContactPhotoManager contactPhotoManager,
                                 Contact contact, 
                                 int contactId)
        {
            var fileUploadResult = new FileUploadResult();

            if (context.Request.Form.Files.Count == 0)
            {
                await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
            }

            var fileName = context.Request.Form.Files[0].FileName;
            var contentLength = context.Request.Form.Files[0].Length;

            if (String.IsNullOrEmpty(fileName) || contentLength == 0)
                throw new InvalidOperationException(CRMErrorsResource.InvalidFile);

            if (0 < setupInfo.MaxImageUploadSize && setupInfo.MaxImageUploadSize < contentLength)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = fileSizeComment.GetFileImageSizeNote(CRMCommonResource.ErrorMessage_UploadFileSize, false).HtmlEncode();

                await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
            }

            if (FileUtility.GetFileTypeByFileName(fileName) != FileType.Image)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = CRMJSResource.ErrorMessage_NotImageSupportFormat.HtmlEncode();

                await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
            }

            var uploadOnly = Convert.ToBoolean(context.Request.Form["uploadOnly"]);
            var tmpDirName = context.Request.Form["tmpDirName"];

            try
            {
                ContactPhotoManager.PhotoData photoData;
                if (contactId != 0)
                {
                    photoData = await contactPhotoManager.UploadPhotoAsync(context.Request.Form.Files[0].OpenReadStream(), contactId, uploadOnly);
                }
                else
                {
                    if (String.IsNullOrEmpty(tmpDirName) || tmpDirName == "null")
                    {
                        tmpDirName = Guid.NewGuid().ToString();
                    }
                    photoData = await contactPhotoManager.UploadPhotoToTempAsync(context.Request.Form.Files[0].OpenReadStream(), tmpDirName);
                }

                fileUploadResult.Success = true;
                fileUploadResult.Data = photoData;
            }
            catch (Exception e)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = e.Message.HtmlEncode();

                await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
            }

            if (contact != null)
            {
                var messageAction = contact is Company ? MessageAction.CompanyUpdatedPhoto : MessageAction.PersonUpdatedPhoto;

                messageService.Send(messageAction, messageTarget.Create(contact.ID), contact.GetTitle());

            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(fileUploadResult));
        }
    }

    public static class ContactPhotoHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseContactPhotoHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContactPhotoHandlerMiddleware>();
        }
    }
}