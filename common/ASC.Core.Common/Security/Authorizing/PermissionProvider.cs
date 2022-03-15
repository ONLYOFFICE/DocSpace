namespace ASC.Core.Security.Authorizing;

class PermissionProvider : IPermissionProvider
{
    private readonly AuthorizationManager _authorizationManager;

    public PermissionProvider(AuthorizationManager authorizationManager)
    {
        _authorizationManager = authorizationManager;
    }

    public IEnumerable<Ace> GetAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(action);

        return _authorizationManager
            .GetAcesWithInherits(subject.ID, action.ID, objectId, secObjProvider)
            .Select(r => new Ace(r.Action, r.AceType));
    }
}
