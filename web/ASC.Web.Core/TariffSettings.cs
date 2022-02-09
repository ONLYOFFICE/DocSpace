/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

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