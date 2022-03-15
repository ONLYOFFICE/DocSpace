namespace ASC.Common.Security;

public class SecurityCallContext
{
    public object UserData { get; set; }
    public List<ISecurityObjectId> ObjectsStack { get; private set; }
    public List<IRole> RolesList { get; private set; }

    public SecurityCallContext()
    {
        ObjectsStack = new List<ISecurityObjectId>();
        RolesList = new List<IRole>();
    }
}
