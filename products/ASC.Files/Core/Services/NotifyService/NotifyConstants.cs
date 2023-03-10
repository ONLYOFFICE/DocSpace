// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Files.Core.Services.NotifyService;

public static class NotifyConstants
{
    #region Events

    public static readonly INotifyAction EventDocuSignComplete = new NotifyAction("DocuSignComplete", "docusign complete");
    public static readonly INotifyAction EventDocuSignStatus = new NotifyAction("DocuSignStatus", "docusign status");
    public static readonly INotifyAction EventMailMergeEnd = new NotifyAction("MailMergeEnd", "mail merge end");
    public static readonly INotifyAction EventShareDocument = new NotifyAction("ShareDocument", "share document");
    public static readonly INotifyAction EventShareEncryptedDocument = new NotifyAction("ShareEncryptedDocument", "share encrypted document");
    public static readonly INotifyAction EventShareFolder = new NotifyAction("ShareFolder", "share folder");
    public static readonly INotifyAction EventEditorMentions = new NotifyAction("EditorMentions", "editor mentions");
    public static readonly INotifyAction EventRoomRemoved = new NotifyAction("RoomRemoved", "room removed");

    #endregion

    #region  Tags

    public static readonly string TagFolderID = "FolderID";
    public static readonly string TagFolderParentId = "FolderParentId";
    public static readonly string TagFolderRootFolderType = "FolderRootFolderType";
    public static readonly string TagDocumentTitle = "DocumentTitle";
    public static readonly string TagDocumentUrl = "DocumentURL";
    public static readonly string TagDocumentExtension = "DocumentExtension";
    public static readonly string TagAccessRights = "AccessRights";
    public static readonly string TagMessage = "Message";
    public static readonly string TagMailsCount = "MailsCount";
    public static readonly string RoomTitle = "RoomTitle";
    public static readonly string RoomUrl = "RoomURL";

    #endregion
}
