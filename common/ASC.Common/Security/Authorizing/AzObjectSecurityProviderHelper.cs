namespace ASC.Common.Security.Authorizing;

public class AzObjectSecurityProviderHelper
{
    public ISecurityObjectId CurrentObjectId { get; private set; }
    public bool ObjectRolesSupported => _currSecObjProvider != null && _currSecObjProvider.ObjectRolesSupported;

    private readonly SecurityCallContext _callContext;
    private readonly bool _currObjIdAsProvider;
    private ISecurityObjectProvider _currSecObjProvider;

    public AzObjectSecurityProviderHelper(ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
    {
        ArgumentNullException.ThrowIfNull(objectId);
        _currObjIdAsProvider = false;
        CurrentObjectId = objectId;
        _currSecObjProvider = secObjProvider;

        if (_currSecObjProvider == null && CurrentObjectId is ISecurityObjectProvider securityObjectProvider)
        {
            _currObjIdAsProvider = true;
            _currSecObjProvider = securityObjectProvider;
        }

        _callContext = new SecurityCallContext();
    }

    public IEnumerable<IRole> GetObjectRoles(ISubject account)
    {
        var roles = _currSecObjProvider.GetObjectRoles(account, CurrentObjectId, _callContext);

        foreach (var role in roles)
        {
            if (!_callContext.RolesList.Contains(role))
            {
                _callContext.RolesList.Add(role);
            }
        }

        return roles;
    }

    public bool NextInherit()
    {
        if (_currSecObjProvider == null || !_currSecObjProvider.InheritSupported)
        {
            return false;
        }

        CurrentObjectId = _currSecObjProvider.InheritFrom(CurrentObjectId);
        if (CurrentObjectId == null)
        {
            return false;
        }

        if (_currObjIdAsProvider)
        {
            _currSecObjProvider = CurrentObjectId as ISecurityObjectProvider;
        }

        _callContext.ObjectsStack.Insert(0, CurrentObjectId);

        return _currSecObjProvider != null;
    }
}