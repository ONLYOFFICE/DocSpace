namespace ASC.Common.Security;

[Scope(typeof(PermissionResolver))]
public interface IPermissionResolver
{
    bool Check(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions);
    bool Check(ISubject subject, params IAction[] actions);
    void Demand(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions);
    void Demand(ISubject subject, params IAction[] actions);
}
