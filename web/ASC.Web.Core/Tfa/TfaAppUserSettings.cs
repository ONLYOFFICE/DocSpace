namespace ASC.Web.Studio.Core.TFA
{
    [Serializable]
    public class TfaAppUserSettings : ISettings
    {
        [JsonPropertyName("BackupCodes")]
        public IEnumerable<BackupCode> CodesSetting { get; set; }

        [JsonPropertyName("Salt")]
        public long SaltSetting { get; set; }

        public Guid ID
        {
            get { return new Guid("{EAF10611-BE1E-4634-B7A1-57F913042F78}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TfaAppUserSettings
            {
                CodesSetting = new List<BackupCode>(),
                SaltSetting = 0
            };
        }

        public static long GetSalt(SettingsManager settingsManager, Guid userId)
        {
            var settings = settingsManager.LoadForUser<TfaAppUserSettings>(userId);
            var salt = settings.SaltSetting;
            if (salt == 0)
            {
                var from = new DateTime(2018, 07, 07, 0, 0, 0, DateTimeKind.Utc);
                settings.SaltSetting = salt = (long)(DateTime.UtcNow - from).TotalMilliseconds;

                settingsManager.SaveForUser(settings, userId);
            }
            return salt;
        }

        public static IEnumerable<BackupCode> BackupCodesForUser(SettingsManager settingsManager, Guid userId)
        {
            return settingsManager.LoadForUser<TfaAppUserSettings>(userId).CodesSetting;
        }

        public static void DisableCodeForUser(SettingsManager settingsManager, InstanceCrypto instanceCrypto, Signature signature, Guid userId, string code)
        {
            var settings = settingsManager.LoadForUser<TfaAppUserSettings>(userId);
            var query = settings.CodesSetting.Where(x => x.GetEncryptedCode(instanceCrypto, signature) == code).ToList();

            if (query.Count > 0)
                query.First().IsUsed = true;

            settingsManager.SaveForUser(settings, userId);
        }

        public static bool EnableForUser(SettingsManager settingsManager, Guid guid)
        {
            return settingsManager.LoadForUser<TfaAppUserSettings>(guid).CodesSetting.Any();
        }

        public static void DisableForUser(IServiceProvider serviceProvider, SettingsManager settingsManager, Guid guid)
        {
            if (new TfaAppUserSettings().GetDefault(serviceProvider) is TfaAppUserSettings defaultSettings)
            {
                settingsManager.SaveForUser(defaultSettings, guid);
            }
        }


    }
}