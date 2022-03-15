namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class CollaboratorSettings : ISettings
    {
        public bool FirstVisit { get; set; }

        public Guid ID
        {
            get { return new Guid("{73537E08-17F6-4706-BFDA-1414108AA7D2}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new CollaboratorSettings()
            {
                FirstVisit = true
            };
        }
    }
}
