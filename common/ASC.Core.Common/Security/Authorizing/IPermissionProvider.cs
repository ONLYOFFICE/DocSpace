namespace ASC.Common.Security;

[Scope(typeof(PermissionProvider))]
public interface IPermissionProvider
{
    IEnumerable<Ace> GetAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider);
}
