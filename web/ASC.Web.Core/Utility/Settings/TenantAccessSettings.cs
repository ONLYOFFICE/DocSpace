namespace ASC.Web.Core.Utility.Settings
{
    [Serializable]
    public class TenantAccessSettings : ISettings
    {
        public bool Anyone { get; set; }

        public bool RegisterUsersImmediately { get; set; }

        public Guid ID
        {
            get { return new Guid("{0CB4C871-0040-45AB-AE79-4CC292B91EF1}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TenantAccessSettings { Anyone = false, RegisterUsersImmediately = false };
        }
    }
}
