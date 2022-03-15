namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class StudioDefaultPageSettings : ISettings
    {
        public Guid DefaultProductID { get; set; }

        public Guid ID
        {
            get { return new Guid("{F3FF27C5-BDE3-43ae-8DD0-2E8E0D7044F1}"); }
        }

        public Guid FeedModuleID
        {
            get { return new Guid("{48328C27-4C85-4987-BA0E-D6BB17356B10}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new StudioDefaultPageSettings { DefaultProductID = Guid.Empty };
        }
    }
}
