namespace ASC.Web.Core.Users.Import
{
    public interface IUserImporter
    {
        IEnumerable<UserInfo> GetDiscoveredUsers();
    }
}