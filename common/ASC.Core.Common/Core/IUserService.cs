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

namespace ASC.Core;

[Scope(typeof(CachedUserService))]
public interface IUserService
{
    IEnumerable<UserInfo> GetUsers(int tenant);
    IQueryable<UserInfo> GetUsers(int tenant, bool isDocSpaceAdmin,
        EmployeeStatus? employeeStatus,
        List<List<Guid>> includeGroups,
        List<Guid> excludeGroups,
        EmployeeActivationStatus? activationStatus,
        string text,
        string sortBy,
        bool sortOrderAsc,
        long limit,
        long offset,
        out int total,
        out int count);
    byte[] GetUserPhoto(int tenant, Guid id);
    DateTime GetUserPasswordStamp(int tenant, Guid id);
    Group GetGroup(int tenant, Guid id);
    Group SaveGroup(int tenant, Group group);
    IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant);
    IEnumerable<Group> GetGroups(int tenant);
    IEnumerable<UserInfo> GetUsersAllTenants(IEnumerable<Guid> userIds);
    UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType);
    UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r);
    UserInfo GetUser(int tenant, Guid id);
    UserInfo GetUser(int tenant, Guid id, Expression<Func<User, UserInfo>> exp);
    UserInfo GetUser(int tenant, string email);
    UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash);
    UserInfo GetUserByUserName(int tenant, string userName);
    UserInfo SaveUser(int tenant, UserInfo user);
    void RemoveGroup(int tenant, Guid id);
    void RemoveUser(int tenant, Guid id);
    IEnumerable<string> GetDavUserEmails(int tenant);
    void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType);
    void SetUserPasswordHash(int tenant, Guid id, string passwordHash);
    void SetUserPhoto(int tenant, Guid id, byte[] photo);
}
