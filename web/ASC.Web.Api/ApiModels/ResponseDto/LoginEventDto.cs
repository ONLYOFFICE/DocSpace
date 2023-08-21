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

namespace ASC.Web.Api.ApiModel.ResponseDto;

/// <summary>
/// </summary>
public class LoginEventDto
{
    /// <summary>ID</summary>
    /// <type>System.Int32, System</type>
    public int Id { get; set; }

    /// <summary>Date</summary>
    /// <type>ASC.Api.Core.ApiDateTime, ASC.Api.Core</type>
    public ApiDateTime Date { get; set; }

    /// <summary>User</summary>
    /// <type>System.String, System</type>
    public string User { get; set; }

    /// <summary>User ID</summary>
    /// <type>System.Guid, System</type>
    public Guid UserId { get; set; }

    /// <summary>Login</summary>
    /// <type>System.String, System</type>
    public string Login { get; set; }

    /// <summary>Action</summary>
    /// <type>System.String, System</type>
    public string Action { get; set; }

    /// <summary>Action ID</summary>
    /// <type>ASC.MessagingSystem.Core.MessageAction, ASC.Core.Common</type>
    public MessageAction ActionId { get; set; }

    /// <summary>IP</summary>
    /// <type>System.String, System</type>
    public string IP { get; set; }

    /// <summary>Browser</summary>
    /// <type>System.String, System</type>
    public string Browser { get; set; }

    /// <summary>Platform</summary>
    /// <type>System.String, System</type>
    public string Platform { get; set; }

    /// <summary>Page</summary>
    /// <type>System.String, System</type>
    public string Page { get; set; }

    public LoginEventDto(AuditTrail.Models.LoginEventDto loginEvent)
    {
        Id = loginEvent.Id;
        Date = new ApiDateTime(loginEvent.Date, TimeSpan.Zero);
        User = loginEvent.UserName;
        UserId = loginEvent.UserId;
        Login = loginEvent.Login;
        Action = loginEvent.ActionText;
        ActionId = (MessageAction)loginEvent.Action;
        IP = loginEvent.IP;
        Browser = loginEvent.Browser;
        Platform = loginEvent.Platform;
        Page = loginEvent.Page;
    }
}