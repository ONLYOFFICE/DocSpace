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


using System.Web;

using ASC.Common;
using ASC.Common.Utils;
using ASC.Core.Common;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Web.Files.Utils
{
    [Scope]
    public class FileShareLink
    {
        private FileUtility FileUtility { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private Global Global { get; }
        private FileSecurity FileSecurity { get; }

        public FileShareLink(
            FileUtility fileUtility,
            FilesLinkUtility filesLinkUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            Global global,
            FileSecurity fileSecurity)
        {
            FileUtility = fileUtility;
            FilesLinkUtility = filesLinkUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            Global = global;
            FileSecurity = fileSecurity;
        }

        public string GetLink<T>(File<T> file, bool withHash = true)
        {
            var url = file.DownloadUrl;

            if (FileUtility.CanWebView(file.Title))
                url = FilesLinkUtility.GetFileWebPreviewUrl(FileUtility, file.Title, file.ID);

            if (withHash)
            {
                var linkParams = CreateKey(file.ID);
                url += "&" + FilesLinkUtility.DocShareKey + "=" + HttpUtility.UrlEncode(linkParams);
            }

            return BaseCommonLinkUtility.GetFullAbsolutePath(url);
        }

        public string CreateKey<T>(T fileId)
        {
            return Signature.Create(fileId, Global.GetDocDbKey());
        }

        public string Parse(string doc)
        {
            return Signature.Read<string>(doc ?? string.Empty, Global.GetDocDbKey());
        }
        public T Parse<T>(string doc)
        {
            return Signature.Read<T>(doc ?? string.Empty, Global.GetDocDbKey());
        }

        public bool Check<T>(string doc, bool checkRead, IFileDao<T> fileDao, out File<T> file)
        {
            var fileShare = Check(doc, fileDao, out file);
            return (!checkRead
                    && (fileShare == FileShare.ReadWrite
                        || fileShare == FileShare.CustomFilter
                        || fileShare == FileShare.Review
                        || fileShare == FileShare.FillForms
                        || fileShare == FileShare.Comment))
                || (checkRead && fileShare != FileShare.Restrict);
        }

        public FileShare Check<T>(string doc, IFileDao<T> fileDao, out File<T> file)
        {
            file = null;
            if (string.IsNullOrEmpty(doc)) return FileShare.Restrict;
            var fileId = Parse<T>(doc);
            file = fileDao.GetFile(fileId);
            if (file == null) return FileShare.Restrict;

            var filesSecurity = FileSecurity;
            if (filesSecurity.CanEdit(file, FileConstant.ShareLinkId)) return FileShare.ReadWrite;
            if (filesSecurity.CanCustomFilterEdit(file, FileConstant.ShareLinkId)) return FileShare.CustomFilter;
            if (filesSecurity.CanReview(file, FileConstant.ShareLinkId)) return FileShare.Review;
            if (filesSecurity.CanFillForms(file, FileConstant.ShareLinkId)) return FileShare.FillForms;
            if (filesSecurity.CanComment(file, FileConstant.ShareLinkId)) return FileShare.Comment;
            if (filesSecurity.CanRead(file, FileConstant.ShareLinkId)) return FileShare.Read;
            return FileShare.Restrict;
        }
    }
}