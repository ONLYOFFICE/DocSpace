using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Core.Security.Authorizing;

[Scope]
class PermissionResolver : IPermissionResolver
{
    private readonly AzManager _azManager;

    public PermissionResolver(AzManager azManager)
    {
        _azManager = azManager ?? throw new ArgumentNullException(nameof(azManager));
    }

    public bool Check(ISubject subject, params IAction[] actions)
    {
        return Check(subject, null, null, actions);
    }

    public bool Check(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        var denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
        return denyActions.Length == 0;
    }

    public void Demand(ISubject subject, params IAction[] actions)
    {
        Demand(subject, null, null, actions);
    }

    public void Demand(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        var denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
        if (0 < denyActions.Length)
        {
            throw new AuthorizingException(
                subject,
                Array.ConvertAll(denyActions, r => r._targetAction),
                Array.ConvertAll(denyActions, r => r._denySubject),
                Array.ConvertAll(denyActions, r => r._denyAction));
        }
    }


    private DenyResult[] GetDenyActions(ISubject subject, IAction[] actions, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
    {
        var denyActions = new List<DenyResult>();
        if (actions == null)
        {
            actions = Array.Empty<IAction>();
        }

        if (subject == null)
        {
            denyActions = actions.Select(a => new DenyResult(a, null, null)).ToList();
        }
        else if (subject is ISystemAccount && subject.ID == Constants.CoreSystem.ID)
        {
            // allow all
        }
        else
        {
            ISubject denySubject = null;
            IAction denyAction = null;
            foreach (var action in actions)
            {
                var allow = _azManager.CheckPermission(subject, action, objectId, securityObjProvider, out denySubject, out denyAction);
                if (!allow)
                {
                    denyActions.Add(new DenyResult(action, denySubject, denyAction));
                    break;
                }
            }
        }

        return denyActions.ToArray();
    }

    private class DenyResult
    {
        public readonly IAction _targetAction;
        public readonly ISubject _denySubject;
        public readonly IAction _denyAction;

        public DenyResult(IAction targetAction, ISubject denySubject, IAction denyAction)
        {
            _targetAction = targetAction;
            _denySubject = denySubject;
            _denyAction = denyAction;
        }
    }
}
