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
static internal partial class LdapUserImporterLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "LdapUserImporter.GetGroupUsers(Group name: {groupName})")]
    public static partial void DebugGetGroupUsers(this ILogger<LdapUserImporter> logger, string groupName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Found nested LDAP Group: {groupName}")]
    public static partial void DebugFoundNestedLdapGroup(this ILogger<LdapUserImporter> logger, string groupName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Skip already watched nested LDAP Group: {groupName}")]
    public static partial void DebugSkipAlreadyWatched(this ILogger<LdapUserImporter> logger, string groupName);

    [LoggerMessage(Level = LogLevel.Error, Message = "IsUserExistInGroups(login: '{login}' sid: '{sid}')")]
    public static partial void ErrorIsUserExistInGroups(this ILogger<LdapUserImporter> logger, string login, string sid, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GetAndCheckCurrentGroups(login: '{login}' sid: '{sid}')")]
    public static partial void ErrorGetAndCheckCurrentGroups(this ILogger<LdapUserImporter> logger, string login, string sid, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "TrySyncUserGroupMembership(groupname: '{groupName}' sid: '{sid}') no portal group found, creating")]
    public static partial void DebugTrySyncUserGroupMembershipCreatingPortalGroup(this ILogger<LdapUserImporter> logger, string groupName, string sid);

    [LoggerMessage(Level = LogLevel.Debug, Message = "TrySyncUserGroupMembership(username: '{userName}' sid: '{userSid}') adding user to group (groupname: '{groupName}' sid: '{groupSid}')")]
    public static partial void DebugTrySyncUserGroupMembershipAddingUserToGroup(this ILogger<LdapUserImporter> logger, string userName, string userSid, string groupName, string groupSid);

    [LoggerMessage(Level = LogLevel.Debug, Message = "TrySyncUserGroupMembership(username: '{userName}' sid: '{userSid}') removing user from group (groupname: '{groupName}' sid: '{groupSid}')")]
    public static partial void DebugTrySyncUserGroupMembershipRemovingUserFromGroup(this ILogger<LdapUserImporter> logger, string userName, string userSid, string groupName, string groupSid);

    [LoggerMessage(Level = LogLevel.Error, Message = "TryLoadLDAPUsers(): Incorrect filter. userFilter = {userFilter}")]
    public static partial void ErrorTryLoadLDAPUsersIncorrectUserFilter(this ILogger<LdapUserImporter> logger, string userFilter);

    [LoggerMessage(Level = LogLevel.Error, Message = "TryLoadLDAPGroups(): Incorrect group filter. groupFilter = {groupFilter}")]
    public static partial void ErrorTryLoadLDAPUsersIncorrectGroupFilter(this ILogger<LdapUserImporter> logger, string groupFilter);

    [LoggerMessage(Level = LogLevel.Error, Message = "LoadLDAPDomain(): Error")]
    public static partial void ErrorLoadLDAPDomain(this ILogger<LdapUserImporter> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Login Attribute parameter ({loginAttributeParametr}) not found: DN = {distinguishedName}")]
    public static partial void DebugLoginAttributeParameterNotFound(this ILogger<LdapUserImporter> logger, string loginAttributeParametr, string distinguishedName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Login Attribute parameter ({loginAttributeParametr}) not found: loginAttribute = {loginAttribute}")]
    public static partial void ErrorLoginAttributeParameterNotFound(this ILogger<LdapUserImporter> logger, string loginAttributeParametr, string loginAttribute, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "User Attribute parameter ({userAttributeParametr}) not found: DN = {distinguishedName}")]
    public static partial void DebugUserAttributeParameterNotFound(this ILogger<LdapUserImporter> logger, string userAttributeParametr, string distinguishedName);

    [LoggerMessage(Level = LogLevel.Error, Message = "User Attribute parameter ({userAttributeParametr}) not found: userAttr = {userAttribute}")]
    public static partial void ErrorUserAttributeParameterNotFound(this ILogger<LdapUserImporter> logger, string userAttributeParametr, string userAttribute, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Group Attribute parameter ({groupAttributeParametr}) not found: {groupAttribute}")]
    public static partial void ErrorGroupAttributeParameterNotFound(this ILogger<LdapUserImporter> logger, string groupAttributeParametr, string groupAttribute, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Group Name Attribute parameter ({groupNameAttributeParametr}) not found: {groupAttribute}")]
    public static partial void DebugGroupNameAttributeParameterNotFound(this ILogger<LdapUserImporter> logger, string groupNameAttributeParametr, string groupAttribute);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LdapUserImporter.FindUsersByPrimaryGroup()")]
    public static partial void DebugFindUsersByPrimaryGroup(this ILogger<LdapUserImporter> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LdapUserImporter.FindUserByMember(user attr: {userAttribute})")]
    public static partial void DebugFindUserByMember(this ILogger<LdapUserImporter> logger, string userAttribute);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LdapUserImporter.FindGroupByMember(member: {member})")]
    public static partial void DebugFindGroupByMember(this ILogger<LdapUserImporter> logger, string member);

    [LoggerMessage(Level = LogLevel.Error, Message = "FindLdapUser->ToUserInfo() failed")]
    public static partial void ErrorToUserInfo(this ILogger<LdapUserImporter> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "LDAP: DN: '{distinguishedName}' Login Attribute '{loginAttribute}' is empty")]
    public static partial void WarnLoginAttributeIsEmpty(this ILogger<LdapUserImporter> logger, string distinguishedName, string loginAttribute);

    [LoggerMessage(Level = LogLevel.Debug, Message = "FindLdapUsers(login '{login}') found: {usersCount} users")]
    public static partial void DebugFindLdapUsers(this ILogger<LdapUserImporter> logger, string login, int usersCount);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LdapUserImporter->Login(login: '{login}', dn: '{sid}') failed. Error: missing DN or SID")]
    public static partial void DebugLdapUserImporterFailed(this ILogger<LdapUserImporter> logger, string login, string sid);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LdapUserImporter.Login('{login}')")]
    public static partial void DebugLdapUserImporterLogin(this ILogger<LdapUserImporter> logger, string login);

    [LoggerMessage(Level = LogLevel.Error, Message = "LdapUserImporter->Login(login: '{login}') failed")]
    public static partial void ErrorLdapUserImporterLoginFailed(this ILogger<LdapUserImporter> logger, string login, Exception exception);
}
