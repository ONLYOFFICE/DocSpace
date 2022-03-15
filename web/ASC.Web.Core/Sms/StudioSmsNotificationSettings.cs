namespace ASC.Web.Studio.Core.SMS
{
    [Serializable]
    public class StudioSmsNotificationSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{2802df61-af0d-40d4-abc5-a8506a5352ff}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new StudioSmsNotificationSettings { EnableSetting = false, };
        }

        [JsonPropertyName("Enable")]
        public bool EnableSetting { get; set; }
    }

    [Scope]
    public class StudioSmsNotificationSettingsHelper
    {
        private TenantExtra TenantExtra { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private SetupInfo SetupInfo { get; }
        private SettingsManager SettingsManager { get; }
        private SmsProviderManager SmsProviderManager { get; }

        public StudioSmsNotificationSettingsHelper(
            TenantExtra tenantExtra,
            CoreBaseSettings coreBaseSettings,
            SetupInfo setupInfo,
            SettingsManager settingsManager,
            SmsProviderManager smsProviderManager)
        {
            TenantExtra = tenantExtra;
            CoreBaseSettings = coreBaseSettings;
            SetupInfo = setupInfo;
            SettingsManager = settingsManager;
            SmsProviderManager = smsProviderManager;
        }

        public bool IsVisibleSettings()
        {
            var quota = TenantExtra.GetTenantQuota();
            return CoreBaseSettings.Standalone
                    || ((!quota.Trial || SetupInfo.SmsTrial)
                        && !quota.NonProfit
                        && !quota.Free
                        && !quota.Open);
        }

        public bool Enable
        {
            get { return SettingsManager.Load<StudioSmsNotificationSettings>().EnableSetting && SmsProviderManager.Enabled(); }
            set
            {
                var settings = SettingsManager.Load<StudioSmsNotificationSettings>();
                settings.EnableSetting = value;
                SettingsManager.Save<StudioSmsNotificationSettings>(settings);
            }
        }
    }
}