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
using System.Text.Json.Serialization;

using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;

namespace ASC.Web.Files.Services.WCFService
{
    public class AceCollection<T>
    {
        public IEnumerable<T> Files { get; set; }
        public IEnumerable<T> Folders { get; set; }

        public List<AceWrapper> Aces { get; set; }

        public string Message { get; set; }
    }

    public class AceWrapper
    {
        public Guid SubjectId { get; set; }

        [JsonPropertyName("title")]
        public string SubjectName { get; set; }

        public string Link { get; set; }

        [JsonPropertyName("is_group")]
        public bool SubjectGroup { get; set; }

        public bool Owner { get; set; }

        [JsonPropertyName("ace_status")]
        public FileShare Share { get; set; }

        [JsonPropertyName("locked")]
        public bool LockedRights { get; set; }

        [JsonPropertyName("disable_remove")]
        public bool DisableRemove { get; set; }
    }

    public class AceShortWrapper
    {
        public string User { get; set; }

        public string Permissions { get; set; }

        public bool? IsLink { get; set; }

        public AceShortWrapper(AceWrapper aceWrapper)
        {
            var permission = string.Empty;

            switch (aceWrapper.Share)
            {
                case FileShare.Read:
                    permission = FilesCommonResource.AceStatusEnum_Read;
                    break;
                case FileShare.ReadWrite:
                    permission = FilesCommonResource.AceStatusEnum_ReadWrite;
                    break;
                case FileShare.CustomFilter:
                    permission = FilesCommonResource.AceStatusEnum_CustomFilter;
                    break;
                case FileShare.Review:
                    permission = FilesCommonResource.AceStatusEnum_Review;
                    break;
                case FileShare.FillForms:
                    permission = FilesCommonResource.AceStatusEnum_FillForms;
                    break;
                case FileShare.Comment:
                    permission = FilesCommonResource.AceStatusEnum_Comment;
                    break;
                case FileShare.Restrict:
                    permission = FilesCommonResource.AceStatusEnum_Restrict;
                    break;
            }

            User = aceWrapper.SubjectName;
            if (aceWrapper.SubjectId.Equals(FileConstant.ShareLinkId))
            {
                IsLink = true;
                User = FilesCommonResource.AceShareLink;
            }
            Permissions = permission;
        }
    }
}