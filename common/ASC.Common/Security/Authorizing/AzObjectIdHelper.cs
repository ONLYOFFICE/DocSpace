namespace ASC.Common.Security.Authorizing;

public static class AzObjectIdHelper
{
    private static readonly string _separator = "|";

    public static string GetFullObjectId(ISecurityObjectId objectId)
    {
        if (objectId == null)
        {
            return null;
        }

        return $"{objectId.ObjectType.FullName}{_separator}{objectId.SecurityId}";
    }
}
