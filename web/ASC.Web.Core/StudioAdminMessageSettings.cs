namespace ASC.Web.Studio.Core
{
    public class StudioAdminMessageSettings : ISettings
    {
        public bool Enable { get; set; }

        public Guid ID
        {
            get { return new Guid("{28902650-58A9-11E1-B6A9-0F194924019B}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new StudioAdminMessageSettings { Enable = false };
        }
    }
}
