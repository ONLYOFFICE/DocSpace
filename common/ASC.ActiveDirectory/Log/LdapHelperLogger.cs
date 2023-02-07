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


namespace ASC.ActiveDirectory.Log;
static internal partial class LdapHelperLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "UserExistsInGroup() failed")]
    public static partial void ErrorUserExistsInGroupFailed(this ILogger<LdapHelper> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "NovellLdapHelper->SearchDomain() failed")]
    public static partial void WarnSearchDomainFailed(this ILogger<LdapHelper> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "NovellLdapHelper->CheckUserDn(userDn: {userDn}) Wrong User DN parameter")]
    public static partial void ErrorWrongUserDnParameter(this ILogger<LdapHelper> logger, string userDn);

    [LoggerMessage(Level = LogLevel.Error, Message = "NovellLdapHelper->CheckGroupDn(groupDn: {groupDn}): Wrong Group DN parameter")]
    public static partial void ErrorWrongGroupDnParameter(this ILogger<LdapHelper> logger, string groupDn);

    [LoggerMessage(Level = LogLevel.Error, Message = "NovellLdapHelper->GetUsers(filter: '{filter}' limit: {limit}) failed")]
    public static partial void ErrorGetUsersFailed(this ILogger<LdapHelper> logger, string filter, int limit, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "NovellLdapHelper->GetUserBySid(sid: '{sid}') failed")]
    public static partial void ErrorGetUserBySidFailed(this ILogger<LdapHelper> logger, string sid, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "NovellLdapHelper->GetGroups(criteria: '{criteria}') failed")]
    public static partial void ErrorGetGroupsFailed(this ILogger<LdapHelper> logger, Criteria criteria, Exception exception);
}
