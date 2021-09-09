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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ASC.Core;
using ASC.Mail.Models;

namespace ASC.Mail.Storage
{
    public static class MailStoragePathCombiner
    {
        public const string BODY_FILE_NAME = "body.html";
        public const string EML_FILE_NAME = "message.eml";

        private static readonly Dictionary<char, string> Replacements = new Dictionary<char, string>
                {
                    {'+', "%2b"}, {'#', "%23"}, {'|', "_"}, {'<', "_"}, {'>', "_"}, {'"', "_"}, {':', "_"}, {'~', "_"}, {'?', "_"}
                };

        private const string BAD_CHARS_IN_PATH = "|<>:\"~?";

        private static string ComplexReplace(string str, string replacement)
        {
            return replacement.Aggregate(str, (current, badChar) => current.Replace(badChar.ToString(CultureInfo.InvariantCulture), Replacements[badChar]));
        }

        public static string PrepareAttachmentName(string name)
        {
            return ComplexReplace(name, BAD_CHARS_IN_PATH);
        }

        //public static string GetPreSignedUri(int fileId, int tenant, string user, string stream, int fileNumber, string fileName)
        //{
        //    //TODO: Move url to config;
        //    const string attachmentPath = "/addons/mail/httphandlers/download.ashx";

        //    var uriBuilder = new UriBuilder(CommonLinkUtility.GetFullAbsolutePath(attachmentPath));
        //    if (uriBuilder.Uri.IsLoopback)
        //    {
        //        uriBuilder.Host = Dns.GetHostName();
        //    }

        //    var query = uriBuilder.Query;

        //    query += "attachid=" + fileId + "&";
        //    query += "stream=" + stream + "&";
        //    query += FilesLinkUtility.AuthKey + "=" + EmailValidationKeyProvider.GetEmailKey(fileId + stream);

        //    var url = uriBuilder.Uri + "?" + query;

        //    return url;
        //}

        public static string GetStoredUrl(Uri uri)
        {
            var url = uri.ToString();

            if (WorkContext.IsMono && uri.Scheme == Uri.UriSchemeFile)
            {
                if (url.StartsWith("file://"))
                    url = url.Substring(7);
            }

            return GetStoredUrl(url);
        }

        private static string GetStoredUrl(string fullUrl)
        {
            return ComplexReplace(fullUrl, "#");
        }

        public static string GetFileKey(string user, string stream, int fileNumber, string fileName)
        {
            return string.Format("{0}/{1}/attachments/{2}/{3}", user, stream, fileNumber, ComplexReplace(fileName, BAD_CHARS_IN_PATH));
        }

        public static string GetTempFileKey(string user, string stream, int fileNumber, string fileName)
        {
            return string.Format("temp/{0}/{1}/attachments/{2}/{3}", user, stream, fileNumber, ComplexReplace(fileName, BAD_CHARS_IN_PATH));
        }

        public static string GetBodyKey(string stream)
        {
            return string.Format("{0}/{1}", stream, BODY_FILE_NAME);
        }

        public static string GetBodyKey(string user, string stream)
        {
            return string.Format("{0}/{1}/{2}", user, stream, BODY_FILE_NAME);
        }

        public static string GetEmlKey(string user, string stream)
        {
            return string.Format("{0}/{1}/{2}", user, stream, EML_FILE_NAME);
        }

        public static string GerStoredFilePath(MailAttachmentData mailAttachmentData)
        {
            return GetFileKey(mailAttachmentData.user, mailAttachmentData.streamId, mailAttachmentData.fileNumber, mailAttachmentData.storedName);
        }

        public static string GetTempStoredFilePath(MailAttachmentData mailAttachmentData)
        {
            return GetTempFileKey(mailAttachmentData.user, mailAttachmentData.streamId, mailAttachmentData.fileNumber, mailAttachmentData.storedName);
        }

        //public static string GetPreSignedUrl(MailAttachmentData mailAttachmentData)
        //{
        //    return GetPreSignedUri(mailAttachmentData.fileId, mailAttachmentData.tenant, mailAttachmentData.user, mailAttachmentData.streamId, mailAttachmentData.fileNumber, mailAttachmentData.storedName);
        //}

        public static string GerStoredSignatureImagePath(string user, int mailboxId, string storedName)
        {
            return String.Format("{0}/signatures/{1}/{2}", user, mailboxId, ComplexReplace(storedName, BAD_CHARS_IN_PATH));
        }

        public static string GetMessageDirectory(string user, string stream)
        {
            return String.Format("{0}/{1}", user, stream);
        }

        public static string GetUserMailsDirectory(string user)
        {
            return String.Format("{0}", user);
        }

        public static string GetEditorSmileBaseUrl()
        {
            return "/usercontrols/common/ckeditor/plugins/smiley/teamlab_images";
        }

        public static string GetEditorImagesBaseUrl()
        {
            return "/usercontrols/common/ckeditor/plugins/filetype/images";
        }

        public static string GetProxyHttpHandlerUrl()
        {
            return "/httphandlers/urlProxy.ashx";
        }
    }
}
