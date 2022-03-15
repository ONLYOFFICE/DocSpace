namespace ASC.Core.Security.Authorizing;

[Scope]
class RoleProvider : IRoleProvider
{
    //circ dep
    private readonly IServiceProvider _serviceProvider;
    public RoleProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public List<IRole> GetRoles(ISubject account)
    {
        var roles = new List<IRole>();
        if (!(account is ISystemAccount))
        {
            if (account is IRole)
            {
                roles = GetParentRoles(account.ID).ToList();
            }
            else if (account is IUserAccount)
            {
                roles = _serviceProvider.GetService<UserManager>()
                                   .GetUserGroups(account.ID, IncludeType.Distinct | IncludeType.InParent)
                                   .Select(g => (IRole)g)
                                   .ToList();
            }
        }

        return roles;
    }

    public bool IsSubjectInRole(ISubject account, IRole role)
    {
        return _serviceProvider.GetService<UserManager>().IsUserInGroup(account.ID, role.ID);
    }

    private List<IRole> GetParentRoles(Guid roleID)
    {
        var roles = new List<IRole>();
        var gi = _serviceProvider.GetService<UserManager>().GetGroupInfo(roleID);
        if (gi != null)
        {
            var parent = gi.Parent;
            while (parent != null)
            {
                roles.Add(parent);
                parent = parent.Parent;
            }
        }

        return roles;
    }
}
