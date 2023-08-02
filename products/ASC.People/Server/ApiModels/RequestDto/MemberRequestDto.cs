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

namespace ASC.People.ApiModels.RequestDto;

/// <summary>
/// </summary>
public class MemberRequestDto
{
    /// <summary>Employee type (All, RoomAdmin, User, DocSpaceAdmin, Collaborator)</summary>
    /// <type>ASC.Core.Users.EmployeeType, ASC.Core.Common</type>
    public EmployeeType Type { get; set; }

    /// <summary>Specifies if this is a guest or a user</summary>
    /// <type>System.Boolean, System</type>
    public bool IsUser { get; set; }

    /// <summary>Email</summary>
    /// <type>System.String, System</type>
    public string Email { get; set; }

    /// <summary>First name</summary>
    /// <type>System.String, System</type>
    public string Firstname { get; set; }

    /// <summary>Last name</summary>
    /// <type>System.String, System</type>
    public string Lastname { get; set; }

    /// <summary>List of user departments</summary>
    /// <type>System.Guid[], System</type>
    public Guid[] Department { get; set; }

    /// <summary>Title</summary>
    /// <type>System.String, System</type>
    public string Title { get; set; }

    /// <summary>Location</summary>
    /// <type>System.String, System</type>
    public string Location { get; set; }

    /// <summary>Sex (male or female)</summary>
    /// <type>System.String, System</type>
    public string Sex { get; set; }

    /// <summary>Birthday</summary>
    /// <type>ASC.Api.Core.ApiDateTime, ASC.Api.Core</type>
    public ApiDateTime Birthday { get; set; }

    /// <summary>Registration date (if it is not specified, then the current date will be set)</summary>
    /// <type>ASC.Api.Core.ApiDateTime, ASC.Api.Core</type>
    public ApiDateTime Worksfrom { get; set; }

    /// <summary>Comment</summary>
    /// <type>System.String, System</type>
    public string Comment { get; set; }

    /// <summary>List of user contacts</summary>
    /// <type>System.Collections.Generic.IEnumerable{ASC.Web.Api.Models.Contact}, System.Collections.Generic</type>
    public IEnumerable<Contact> Contacts { get; set; }

    /// <summary>Avatar photo URL</summary>
    /// <type>System.String, System</type>
    public string Files { get; set; }

    /// <summary>Password</summary>
    /// <type>System.String, System</type>
    public string Password { get; set; }

    /// <summary>Password hash</summary>
    /// <type>System.String, System</type>
    public string PasswordHash { get; set; }

    /// <summary>Specifies if the user is added via the invitation link or not</summary>
    /// <type>System.Boolean, System</type>
    public bool FromInviteLink { get; set; }

    /// <summary>Key</summary>
    /// <type>System.String, System</type>
    public string Key { get; set; }

    /// <summary>Language</summary>
    /// <type>System.String, System</type>
    public string CultureName { get; set; }

    /// <summary>Target</summary>
    /// <type>System.Guid, System</type>
    public Guid Target { get; set; }
}

/// <summary>
/// </summary>
public class UpdateMemberRequestDto : MemberRequestDto
{
    /// <summary>User ID</summary>
    /// <type>System.String, System</type>
    public string UserId { get; set; }

    /// <summary>Specifies whether to disable a user or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? Disable { get; set; }
}
