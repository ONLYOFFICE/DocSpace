namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class PersonalSettings : ISettings
    {

        [JsonPropertyName("IsNewUser")]
        public bool IsNewUserSetting { get; set; }

        [JsonPropertyName("IsNotActivated")]
        public bool IsNotActivatedSetting { get; set; }

        public Guid ID
        {
            get { return new Guid("{B3427865-8E32-4E66-B6F3-91C61922239F}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new PersonalSettings
            {
                IsNewUserSetting = false,
                IsNotActivatedSetting = false,
            };
        }
    }

    [Scope]
    public class PersonalSettingsHelper
    {
        public PersonalSettingsHelper(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public bool IsNewUser
        {
            get { return SettingsManager.LoadForCurrentUser<PersonalSettings>().IsNewUserSetting; }
            set
            {
                var settings = SettingsManager.LoadForCurrentUser<PersonalSettings>();
                settings.IsNewUserSetting = value;
                SettingsManager.SaveForCurrentUser(settings);
            }
        }

        public bool IsNotActivated
        {
            get { return SettingsManager.LoadForCurrentUser<PersonalSettings>().IsNotActivatedSetting; }
            set
            {
                var settings = SettingsManager.LoadForCurrentUser<PersonalSettings>();
                settings.IsNotActivatedSetting = value;
                SettingsManager.SaveForCurrentUser(settings);
            }
        }

        private SettingsManager SettingsManager { get; }
    }
}