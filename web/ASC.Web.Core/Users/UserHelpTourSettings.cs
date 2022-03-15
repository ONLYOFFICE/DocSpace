namespace ASC.Web.Core.Users
{
    [Serializable]
    public class UserHelpTourSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{DF4B94B7-42C8-4fce-AAE2-D479F3B39BDD}"); }
        }

        public Dictionary<Guid, int> ModuleHelpTour { get; set; }

        public bool IsNewUser { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new UserHelpTourSettings
            {
                ModuleHelpTour = new Dictionary<Guid, int>(),
                IsNewUser = false
            };
        }
    }

    [Scope]
    public class UserHelpTourHelper
    {
        private SettingsManager SettingsManager { get; }

        public UserHelpTourHelper(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public bool IsNewUser
        {
            get { return SettingsManager.LoadForCurrentUser<UserHelpTourSettings>().IsNewUser; }
            set
            {
                var settings = SettingsManager.LoadForCurrentUser<UserHelpTourSettings>();
                settings.IsNewUser = value;
                SettingsManager.SaveForCurrentUser(settings);
            }
        }
    }
}