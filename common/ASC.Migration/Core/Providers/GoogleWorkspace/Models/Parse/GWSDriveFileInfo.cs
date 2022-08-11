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


namespace ASC.Migration.GoogleWorkspace.Models.Parse;

public class GwsDriveFileInfo
{
    public bool Starred { get; set; }

    [JsonPropertyName("viewers_can_download")]
    public bool ViewersCanDownload { get; set; }

    [JsonPropertyName("editors_can_edit_access")]
    public bool EditorsCanEditAccess { get; set; }

    [JsonPropertyName("last_modified_by_any_user")]
    public DateTimeOffset LastModifiedByAnyUser { get; set; }

    [JsonPropertyName("last_modified_by_me")]
    public DateTimeOffset LastModifiedByMe { get; set; }

    [JsonPropertyName("content_last_modified")]
    public DateTimeOffset ContentLastModified { get; set; }
    public DateTimeOffset Created { get; set; }
    public List<GwsDriveFilePermission> Permissions { get; set; }
}

public class GwsDriveFilePermission
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Role { get; set; }

    [JsonPropertyName("additional_roles")]
    public List<string> AdditionalRoles { get; set; }
    public string Kind { get; set; }

    [JsonPropertyName("self_link")]
    public Uri SelfLink { get; set; }

    [JsonPropertyName("email_address")]
    public string EmailAddress { get; set; }
    public string Domain { get; set; }
    public string Etag { get; set; }
    public bool Deleted { get; set; }
}
