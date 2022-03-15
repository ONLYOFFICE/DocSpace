namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class EmailActivationSettings : ISettings
    {
        public bool Show { get; set; }

        public Guid ID
        {
            get { return new Guid("{85987929-1339-48EB-B06D-B9D097BDACF6}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new EmailActivationSettings { Show = true };
        }
    }
}
