namespace ASC.Core;

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
    void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType);
    void SetUserPasswordHash(int tenant, Guid id, string passwordHash);
    void SetUserPhoto(int tenant, Guid id, byte[] photo);
}
