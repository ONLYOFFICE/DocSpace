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

namespace ASC.Web.Studio.Core.SMS;

[Serializable]
public class StudioSmsNotificationSettings : ISettings<StudioSmsNotificationSettings>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{2802df61-af0d-40d4-abc5-a8506a5352ff}"); }
    }

    public StudioSmsNotificationSettings GetDefault()
    {
        return new StudioSmsNotificationSettings { EnableSetting = false, };
    }

    [JsonPropertyName("Enable")]
    public bool EnableSetting { get; set; }
}

[Scope]
public class StudioSmsNotificationSettingsHelper
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly SetupInfo _setupInfo;
    private readonly SettingsManager _settingsManager;
    private readonly SmsProviderManager _smsProviderManager;
    private readonly TenantManager _tenantManager;

    public StudioSmsNotificationSettingsHelper(
        CoreBaseSettings coreBaseSettings,
        SetupInfo setupInfo,
        SettingsManager settingsManager,
        SmsProviderManager smsProviderManager,
        TenantManager tenantManager)
    {
        _coreBaseSettings = coreBaseSettings;
        _setupInfo = setupInfo;
        _settingsManager = settingsManager;
        _smsProviderManager = smsProviderManager;
        _tenantManager = tenantManager;
    }

    public bool IsVisibleSettings()
    {
        return SetupInfo.IsVisibleSettings<StudioSmsNotificationSettings>();
    }

    public bool IsVisibleAndAvailableSettings()
    {
        return IsVisibleSettings() && IsAvailableSettings();
    }

    public bool IsAvailableSettings()
    {
        var quota = _tenantManager.GetCurrentTenantQuota();
        return _coreBaseSettings.Standalone
                || ((!quota.Trial || _setupInfo.SmsTrial)
                    && !quota.NonProfit
                    && !quota.Free);
    }

    public bool Enable
    {
        get { return _settingsManager.Load<StudioSmsNotificationSettings>().EnableSetting && _smsProviderManager.Enabled(); }
        set
        {
            var settings = _settingsManager.Load<StudioSmsNotificationSettings>();
            settings.EnableSetting = value;
            _settingsManager.Save(settings);
        }
    }
}
