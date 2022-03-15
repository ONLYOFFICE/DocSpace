namespace ASC.Web.Core.WhiteLabel
{
    public class CompanyWhiteLabelSettingsWrapper
    {
        public CompanyWhiteLabelSettings Settings { get; set; }
    }

    [Serializable]
    public class CompanyWhiteLabelSettings : ISettings
    {
        public string CompanyName { get; set; }

        public string Site { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        [JsonPropertyName("IsLicensor")]
        public bool IsLicensor { get; set; }


        public bool IsDefault(CoreSettings coreSettings)
        {
            if (!(GetDefault(coreSettings) is CompanyWhiteLabelSettings defaultSettings)) return false;

            return CompanyName == defaultSettings.CompanyName &&
                    Site == defaultSettings.Site &&
                    Email == defaultSettings.Email &&
                    Address == defaultSettings.Address &&
                    Phone == defaultSettings.Phone &&
                    IsLicensor == defaultSettings.IsLicensor;
        }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{C3C5A846-01A3-476D-A962-1CFD78C04ADB}"); }
        }

        private static CompanyWhiteLabelSettings _default;

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            if (_default != null) return _default;

            return GetDefault(serviceProvider.GetService<CoreSettings>());
        }

        public ISettings GetDefault(CoreSettings coreSettings)
        {
            if (_default != null) return _default;

            var settings = coreSettings.GetSetting("CompanyWhiteLabelSettings");

            _default = string.IsNullOrEmpty(settings) ? new CompanyWhiteLabelSettings() : Newtonsoft.Json.JsonConvert.DeserializeObject<CompanyWhiteLabelSettings>(settings);

            return _default;
        }

        #endregion

        public static CompanyWhiteLabelSettings Instance(SettingsManager settingsManager)
        {
            return settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>();
        }
    }
}
