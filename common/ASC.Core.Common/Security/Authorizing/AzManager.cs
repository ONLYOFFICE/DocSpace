namespace ASC.Common.Security.Authorizing;

[Scope]
public class AzManager
{
    private readonly IPermissionProvider _permissionProvider;
    private readonly IRoleProvider _roleProvider;

    internal AzManager() { }

    public AzManager(IRoleProvider roleProvider, IPermissionProvider permissionProvider)
        : this()
    {
        _roleProvider = roleProvider ?? throw new ArgumentNullException(nameof(roleProvider));
        _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
    }

    public bool CheckPermission(ISubject subject, IAction action, ISecurityObjectId objectId,
                                ISecurityObjectProvider securityObjProvider, out ISubject denySubject,
                                out IAction denyAction)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(subject);

        var acl = GetAzManagerAcl(subject, action, objectId, securityObjProvider);
        denySubject = acl.DenySubject;
        denyAction = acl.DenyAction;

        return acl.IsAllow;
    }

    internal AzManagerAcl GetAzManagerAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
    {
        if (action.AdministratorAlwaysAllow && (Constants.Admin.ID == subject.ID || _roleProvider.IsSubjectInRole(subject, Constants.Admin)))
        {
            return AzManagerAcl.Allow;
        }

        var acl = AzManagerAcl.Default;
        var exit = false;

        foreach (var s in GetSubjects(subject, objectId, securityObjProvider))
        {
            var aceList = _permissionProvider.GetAcl(s, action, objectId, securityObjProvider);
            foreach (var ace in aceList)
            {
                if (ace.Reaction == AceType.Deny)
                {
                    acl.IsAllow = false;
                    acl.DenySubject = s;
                    acl.DenyAction = action;
                    exit = true;
                }
                if (ace.Reaction == AceType.Allow && !exit)
                {
                    acl.IsAllow = true;
                    if (!action.Conjunction)
                    {
                        // disjunction: first allow and exit
                        exit = true;
                    }
                }
                if (exit)
                {
                    break;
                }
            }
            if (exit)
            {
                break;
            }
        }

        return acl;
    }

    internal IEnumerable<ISubject> GetSubjects(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
    {
        var subjects = new List<ISubject>
            {
                subject
            };
        subjects.AddRange(
            _roleProvider.GetRoles(subject)
                .ConvertAll(r => { return (ISubject)r; })
            );
        if (objectId != null)
        {
            var secObjProviderHelper = new AzObjectSecurityProviderHelper(objectId, securityObjProvider);
            do
            {
                if (!secObjProviderHelper.ObjectRolesSupported)
                {
                    continue;
                }

                foreach (var role in secObjProviderHelper.GetObjectRoles(subject))
                {
                    if (!subjects.Contains(role))
                    {
                        subjects.Add(role);
                    }
                }
            } while (secObjProviderHelper.NextInherit());
        }

        return subjects;
    }

    #region Nested type: AzManagerAcl

    internal class AzManagerAcl
    {
        public IAction DenyAction { get; set; }
        public ISubject DenySubject { get; set; }
        public bool IsAllow { get; set; }
        public static AzManagerAcl Allow => new AzManagerAcl { IsAllow = true };
        public static AzManagerAcl Default => new AzManagerAcl { IsAllow = false };
    }

    #endregion
}
