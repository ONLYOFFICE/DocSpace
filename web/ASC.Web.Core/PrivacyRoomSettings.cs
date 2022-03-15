namespace ASC.Web.Studio.Core
{
    public class PrivacyRoomSettings : ISettings
    {
        [JsonPropertyName("enbaled")]
        public bool EnabledSetting { get; set; }

        public Guid ID
        {
            get { return new Guid("{FCF002BC-EC4B-4DAB-A6CE-BDE0ABDA44D3}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new PrivacyRoomSettings
            {
                EnabledSetting = true
            };
        }

        public static bool GetEnabled(SettingsManager settingsManager)
        {
            return settingsManager.Load<PrivacyRoomSettings>().EnabledSetting;
        }

        public static void SetEnabled(TenantManager tenantManager, SettingsManager settingsManager, bool value)
        {
            if (!IsAvailable(tenantManager)) return;

            var settings = settingsManager.Load<PrivacyRoomSettings>();
            settings.EnabledSetting = value;
            settingsManager.Save(settings);
        }

        public static bool IsAvailable(TenantManager tenantManager)
        {
            return SetupInfo.IsVisibleSettings(nameof(ManagementType.PrivacyRoom))
                && tenantManager.GetTenantQuota(tenantManager.GetCurrentTenant().Id).PrivacyRoom;
        }
    }
}