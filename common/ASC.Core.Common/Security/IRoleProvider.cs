namespace ASC.Common.Security;

[Scope(typeof(RoleProvider))]
public interface IRoleProvider
{
    List<IRole> GetRoles(ISubject account);
    bool IsSubjectInRole(ISubject account, IRole role);
}
