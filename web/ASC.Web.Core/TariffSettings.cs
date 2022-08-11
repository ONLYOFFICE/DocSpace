// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Studio.UserControls.Management;

[Serializable]
public class TariffSettings : ISettings<TariffSettings>
{
    private static readonly CultureInfo _cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

    [JsonPropertyName("HideNotify")]
    public bool HideNotifySetting { get; set; }

    [JsonPropertyName("HidePricingPage")]
    public bool HidePricingPageForUsers { get; set; }

    [JsonPropertyName("LicenseAccept")]
    public string LicenseAcceptSetting { get; set; }

    public TariffSettings GetDefault()
    {
        return new TariffSettings
        {
            HideNotifySetting = false,
            HidePricingPageForUsers = false,
            LicenseAcceptSetting = DateTime.MinValue.ToString(_cultureInfo),
        };
    }

    [JsonIgnore]
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
        settingsManager.Save(tariffSettings);
    }

    public static bool GetLicenseAccept(SettingsManager settingsManager)
    {
        return !DateTime.MinValue.ToString(_cultureInfo).Equals(settingsManager.LoadForDefaultTenant<TariffSettings>().LicenseAcceptSetting);
    }

    public static void SetLicenseAccept(SettingsManager settingsManager)
    {
        var tariffSettings = settingsManager.LoadForDefaultTenant<TariffSettings>();
        if (DateTime.MinValue.ToString(_cultureInfo).Equals(tariffSettings.LicenseAcceptSetting))
        {
            tariffSettings.LicenseAcceptSetting = DateTime.UtcNow.ToString(_cultureInfo);
            settingsManager.SaveForDefaultTenant(tariffSettings);
        }
    }
}
