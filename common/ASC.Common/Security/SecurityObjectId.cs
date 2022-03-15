namespace ASC.Common.Security;

[DebuggerDisplay("ObjectType: {ObjectType.Name}, SecurityId: {SecurityId}")]
public class SecurityObjectId : ISecurityObjectId
{
    public object SecurityId { get; private set; }
    public Type ObjectType { get; private set; }
    public string FullId => AzObjectIdHelper.GetFullObjectId(this);

    public SecurityObjectId(object id, Type objType)
    {
        ArgumentNullException.ThrowIfNull(objType);

        SecurityId = id;
        ObjectType = objType;
    }

    public override int GetHashCode()
    {
        return AzObjectIdHelper.GetFullObjectId(this).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is SecurityObjectId other &&
            Equals(AzObjectIdHelper.GetFullObjectId(other), AzObjectIdHelper.GetFullObjectId(this));
    }
}