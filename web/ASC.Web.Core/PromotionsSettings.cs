namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class PromotionsSettings : ISettings
    {
        public bool Show { get; set; }

        public Guid ID
        {
            get { return new Guid("{D291A4C1-179D-4ced-895A-E094E809C859}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new PromotionsSettings { Show = true };
        }
    }
}