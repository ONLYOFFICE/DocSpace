namespace ASC.Common.Security;

public interface ISecurityObjectProvider
{
    bool InheritSupported { get; }
    bool ObjectRolesSupported { get; }

    ISecurityObjectId InheritFrom(ISecurityObjectId objectId);
    IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext);
}
