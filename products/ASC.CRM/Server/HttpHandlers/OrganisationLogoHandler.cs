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


using ASC.Common.Web;
using ASC.CRM.Core;
using ASC.CRM.Resources;
using ASC.Web.Core.Files;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace ASC.Web.CRM.Classes
{
    public class OrganisationLogoHandler : IFileUploadHandler
    {
        public OrganisationLogoHandler(CRMSecurity cRMSecurity,
            SetupInfo setupInfo,
            FileSizeComment fileSizeComment)
        {
            CRMSecurity = cRMSecurity;
            SetupInfo = setupInfo;
            FileSizeComment = fileSizeComment;
        }

        public OrganisationLogoManager OrganisationLogoManager { get; }
        public ContactPhotoManager ContactPhotoManager { get; }
        public FileSizeComment FileSizeComment { get; }
        public SetupInfo SetupInfo { get; }
        public CRMSecurity CRMSecurity { get; }

        public FileUploadResult ProcessUpload(HttpContext context)
        {
            if (!CRMSecurity.IsAdmin)
                throw CRMSecurity.CreateSecurityException();

            var fileUploadResult = new FileUploadResult();

            if (!FileToUpload.HasFilesToUpload(context)) return fileUploadResult;

            var file = new FileToUpload(context);

            if (String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                throw new InvalidOperationException(CRMErrorsResource.InvalidFile);

            if (0 < SetupInfo.MaxImageUploadSize && SetupInfo.MaxImageUploadSize < file.ContentLength)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = FileSizeComment.GetFileImageSizeNote(CRMCommonResource.ErrorMessage_UploadFileSize, false).HtmlEncode();
                return fileUploadResult;
            }

            if (FileUtility.GetFileTypeByFileName(file.FileName) != FileType.Image)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = CRMJSResource.ErrorMessage_NotImageSupportFormat.HtmlEncode();
                return fileUploadResult;
            }

            try
            {
                var imageData = Global.ToByteArray(file.InputStream);
                var imageFormat = ContactPhotoManager.CheckImgFormat(imageData);
                var photoUri = OrganisationLogoManager.UploadLogo(imageData, imageFormat);

                fileUploadResult.Success = true;
                fileUploadResult.Data = photoUri;
                return fileUploadResult;
            }
            catch (Exception exception)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = exception.Message.HtmlEncode();
                return fileUploadResult;
            }
        }
    }
}



namespace ASC.Web.Studio.Controls.FileUploader
{
    public class FileToUpload
    {
        public string FileName { get; private set; }
        public Stream InputStream { get; private set; }
        public string FileContentType { get; private set; }
        public long ContentLength { get; private set; }
        public bool NeedSaveToTemp { get; private set; }

        public FileToUpload(HttpContext context)
        {
            if (IsHtml5Upload(context))
            {
                FileName = GetFileName(context);
                InputStream = context.Request.InputStream;
                FileContentType = GetFileContentType(context);
                ContentLength = (int)context.Request.InputStream.Length;
            }
            else
            {
                var file = context.Request.Files[0];
                FileName = file.FileName;
                InputStream = file.InputStream;
                FileContentType = file.ContentType;
                ContentLength = file.ContentLength;
            }

            NeedSaveToTemp = Convert.ToBoolean(GetNeedSaveToTemp(context));

            if (string.IsNullOrEmpty(FileContentType))
            {
                FileContentType = MimeMapping.GetMimeMapping(FileName) ?? string.Empty;
            }
            FileName = FileName.Replace("'", "_").Replace("\"", "_");
        }

        public static bool HasFilesToUpload(HttpContext context)
        {
            return 0 < context.Request.Form.Files.Count || (IsHtml5Upload(context) && context.Request.Form..InputStream != null);
        }

        private static string GetFileName(HttpContext context)
        {
            return context.Request["fileName"];
        }

        private static string GetNeedSaveToTemp(HttpContext context)
        {
            return context.Request["needSaveToTemp"];
        }

        private static string GetFileContentType(HttpContext context)
        {
            return context.Request["fileContentType"];
        }

        private static bool IsHtml5Upload(HttpContext context)
        {
            return "html5".Equals(context.Request["type"]);
        }
    }
}