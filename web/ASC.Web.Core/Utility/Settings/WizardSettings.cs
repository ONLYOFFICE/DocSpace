namespace ASC.Web.Core.Utility.Settings
{
    [Serializable]
    public class WizardSettings : ISettings
    {
        public bool Completed { get; set; }

        public Guid ID
        {
            get { return new Guid("{9A925891-1F92-4ed7-B277-D6F649739F06}"); }
        }


        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new WizardSettings
            {
                Completed = true
            };
        }
    }
}
