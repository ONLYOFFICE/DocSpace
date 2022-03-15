namespace ASC.Web.Studio.UserControls.Management
{
    [Serializable]
    public class TariffSettings : ISettings
    {
        private static readonly CultureInfo CultureInfo = CultureInfo.CreateSpecificCulture("en-US");

        [JsonPropertyName("HideNotify")]
        public bool HideNotifySetting { get; set; }

        [JsonPropertyName("HidePricingPage")]
        public bool HidePricingPageForUsers { get; set; }

        [JsonPropertyName("LicenseAccept")]
        public string LicenseAcceptSetting { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TariffSettings
            {
                HideNotifySetting = false,
                HidePricingPageForUsers = false,
                LicenseAcceptSetting = DateTime.MinValue.ToString(CultureInfo),
            };
        }

        public Guid ID
        {
            get { return new Guid("{07956D46-86F7-433b-A657-226768EF9B0D}"); }
        }

        public static bool GetHideNotify(SettingsManager settingsManager)
        {
            return settingsManager.LoadForCurrentUser<TariffSettings>().HideNotifySetting;
        }

        public static void SetHideNotify(SettingsManager settingsManager, bool newVal)
        {
            var tariffSettings = settingsManager.LoadForCurrentUser<TariffSettings>();
            tariffSettings.HideNotifySetting = newVal;
            settingsManager.SaveForCurrentUser(tariffSettings);
        }

        public static bool GetHidePricingPage(SettingsManager settingsManager)
        {
            return settingsManager.Load<TariffSettings>().HidePricingPageForUsers;
        }

        public static void SetHidePricingPage(SettingsManager settingsManager, bool newVal)
        {
            var tariffSettings = settingsManager.Load<TariffSettings>();
            tariffSettings.HidePricingPageForUsers = newVal;
            settingsManager.Save<TariffSettings>(tariffSettings);
        }

        public static bool GetLicenseAccept(SettingsManager settingsManager)
        {
            return !DateTime.MinValue.ToString(CultureInfo).Equals(settingsManager.LoadForDefaultTenant<TariffSettings>().LicenseAcceptSetting);
        }

        public static void SetLicenseAccept(SettingsManager settingsManager)
        {
            var tariffSettings = settingsManager.LoadForDefaultTenant<TariffSettings>();
            if (DateTime.MinValue.ToString(CultureInfo).Equals(tariffSettings.LicenseAcceptSetting))
            {
                tariffSettings.LicenseAcceptSetting = DateTime.UtcNow.ToString(CultureInfo);
                settingsManager.SaveForDefaultTenant(tariffSettings);
            }
        }
    }
}