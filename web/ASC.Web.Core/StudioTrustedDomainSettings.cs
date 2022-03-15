namespace ASC.Web.Studio.Core
{
    public class StudioTrustedDomainSettings : ISettings
    {
        public bool InviteUsersAsVisitors { get; set; }

        public Guid ID
        {
            get { return new Guid("{00A2DB01-BAE3-48aa-BE32-CE768D7C874E}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new StudioTrustedDomainSettings { InviteUsersAsVisitors = false };
        }
    }
}
