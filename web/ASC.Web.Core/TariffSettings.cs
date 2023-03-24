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

    public static async Task<bool> GetHideNotifyAsync(SettingsManager settingsManager)
    {
        return (await settingsManager.LoadForCurrentUserAsync<TariffSettings>()).HideNotifySetting;
    }

    public static async Task SetHideNotifyAsync(SettingsManager settingsManager, bool newVal)
    {
        var tariffSettings = await settingsManager.LoadForCurrentUserAsync<TariffSettings>();
        tariffSettings.HideNotifySetting = newVal;
        await settingsManager.SaveForCurrentUserAsync(tariffSettings);
    }

    public static async Task<bool> GetHidePricingPageAsync(SettingsManager settingsManager)
    {
        return (await settingsManager.LoadAsync<TariffSettings>()).HidePricingPageForUsers;
    }

    public static async Task SetHidePricingPageAsync(SettingsManager settingsManager, bool newVal)
    {
        var tariffSettings = await settingsManager.LoadAsync<TariffSettings>();
        tariffSettings.HidePricingPageForUsers = newVal;
        await settingsManager.SaveAsync(tariffSettings);
    }

    public static async Task<bool> GetLicenseAcceptAsync(SettingsManager settingsManager)
    {
        return !DateTime.MinValue.ToString(_cultureInfo).Equals((await settingsManager.LoadForDefaultTenantAsync<TariffSettings>()).LicenseAcceptSetting);
    }

    public static async Task SetLicenseAcceptAsync(SettingsManager settingsManager)
    {
        var tariffSettings = await settingsManager.LoadForDefaultTenantAsync<TariffSettings>();
        if (DateTime.MinValue.ToString(_cultureInfo).Equals(tariffSettings.LicenseAcceptSetting))
        {
            tariffSettings.LicenseAcceptSetting = DateTime.UtcNow.ToString(_cultureInfo);
            await settingsManager.SaveForDefaultTenantAsync(tariffSettings);
        }
    }
}
