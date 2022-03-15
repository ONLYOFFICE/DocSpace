namespace ASC.Web.Studio.Utility
{
    [Scope]
    public class TenantLogoHelper
    {
        private TenantLogoManager TenantLogoManager { get; }
        private SettingsManager SettingsManager { get; }
        private TenantWhiteLabelSettingsHelper TenantWhiteLabelSettingsHelper { get; }
        private TenantInfoSettingsHelper TenantInfoSettingsHelper { get; }

        public TenantLogoHelper(
            TenantLogoManager tenantLogoManager,
            SettingsManager settingsManager,
            TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
            TenantInfoSettingsHelper tenantInfoSettingsHelper)
        {
            TenantLogoManager = tenantLogoManager;
            SettingsManager = settingsManager;
            TenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
            TenantInfoSettingsHelper = tenantInfoSettingsHelper;
        }

        public string GetLogo(WhiteLabelLogoTypeEnum type, bool general = true, bool isDefIfNoWhiteLabel = false)
        {
            string imgUrl;
            if (TenantLogoManager.WhiteLabelEnabled)
            {
                var _tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
                return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, type, general);
            }
            else
            {
                if (isDefIfNoWhiteLabel)
                {
                    imgUrl = TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(type, general);
                }
                else
                {
                    if (type == WhiteLabelLogoTypeEnum.Dark)
                    {
                        /*** simple scheme ***/
                        var _tenantInfoSettings = SettingsManager.Load<TenantInfoSettings>();
                        imgUrl = TenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(_tenantInfoSettings);
                        /***/
                    }
                    else
                    {
                        imgUrl = TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(type, general);
                    }
                }
            }

            return imgUrl;

        }
    }
}
