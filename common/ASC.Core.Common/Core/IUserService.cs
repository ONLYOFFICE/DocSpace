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
    Task<IEnumerable<UserInfo>> GetUsersAsync(int tenant);
    IQueryable<UserInfo> GetUsers(int tenant, bool isDocSpaceAdmin,
        EmployeeStatus? employeeStatus,
        List<List<Guid>> includeGroups,
        List<Guid> excludeGroups,
        List<Tuple<List<List<Guid>>, List<Guid>>> combinedGroups,
        EmployeeActivationStatus? activationStatus,
        AccountLoginType? accountLoginType,
        string text,
        string sortBy,
        bool sortOrderAsc,
        long limit,
        long offset,
        out int total,
        out int count);
    Task<byte[]> GetUserPhotoAsync(int tenant, Guid id);
    Task<DateTime> GetUserPasswordStampAsync(int tenant, Guid id);
    Task<Group> GetGroupAsync(int tenant, Guid id);
    Task<Group> SaveGroupAsync(int tenant, Group group);
    Task<IDictionary<string, UserGroupRef>> GetUserGroupRefsAsync(int tenant);
    IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant);
    Task<IEnumerable<Group>> GetGroupsAsync(int tenant);
    Task<IEnumerable<UserInfo>> GetUsersAllTenantsAsync(IEnumerable<Guid> userIds);
    Task<UserGroupRef> GetUserGroupRefAsync(int tenant, Guid groupId, UserGroupRefType refType);
    UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType);
    Task<UserGroupRef> SaveUserGroupRefAsync(int tenant, UserGroupRef r);
    Task<UserInfo> GetUserAsync(int tenant, Guid id);
    UserInfo GetUser(int tenant, Guid id);
    Task<UserInfo> GetUserAsync(int tenant, Guid id, Expression<Func<User, UserInfo>> exp);
    Task<UserInfo> GetUserAsync(int tenant, string email);
    Task<UserInfo> GetUserByPasswordHashAsync(int tenant, string login, string passwordHash);
    Task<UserInfo> GetUserByUserName(int tenant, string userName);
    Task<UserInfo> SaveUserAsync(int tenant, UserInfo user);
    Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime from);
    Task RemoveGroupAsync(int tenant, Guid id);
    Task RemoveUserAsync(int tenant, Guid id);
    Task<IEnumerable<string>> GetDavUserEmailsAsync(int tenant);
    Task RemoveUserGroupRefAsync(int tenant, Guid userId, Guid groupId, UserGroupRefType refType);
    Task SetUserPasswordHashAsync(int tenant, Guid id, string passwordHash);
    Task SetUserPhotoAsync(int tenant, Guid id, byte[] photo);
}
