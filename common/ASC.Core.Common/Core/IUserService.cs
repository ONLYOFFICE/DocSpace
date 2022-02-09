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

namespace ASC.Core
{
    [Scope(typeof(ConfigureEFUserService), typeof(ConfigureCachedUserService))]
    public interface IUserService
    {
        IEnumerable<UserInfo> GetUsers(int tenant);

        IQueryable<UserInfo> GetUsers(int tenant, bool isAdmin,
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

        UserInfo GetUser(int tenant, Guid id);

        UserInfo GetUser(int tenant, string email);

        UserInfo GetUserByUserName(int tenant, string userName);

        UserInfo GetUser(int tenant, Guid id, Expression<Func<User, UserInfo>> exp);

        UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash);

        UserInfo SaveUser(int tenant, UserInfo user);

        void RemoveUser(int tenant, Guid id);

        byte[] GetUserPhoto(int tenant, Guid id);

        void SetUserPhoto(int tenant, Guid id, byte[] photo);

        DateTime GetUserPasswordStamp(int tenant, Guid id);

        void SetUserPasswordHash(int tenant, Guid id, string passwordHash);


        IEnumerable<Group> GetGroups(int tenant);

        Group GetGroup(int tenant, Guid id);

        Group SaveGroup(int tenant, Group group);

        void RemoveGroup(int tenant, Guid id);

        UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType);
        IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant);

        UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r);

        void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType);

        IEnumerable<UserInfo> GetUsersAllTenants(IEnumerable<Guid> userIds);
    }
}
