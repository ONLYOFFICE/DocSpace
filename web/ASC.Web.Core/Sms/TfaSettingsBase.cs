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

namespace ASC.Web.Core.Sms;

public abstract class TfaSettingsBase<T> : ISettings<T> where T : ISettings<T>
{
    [JsonPropertyName("Enable")]
    public bool EnableSetting { get; set; }
    public abstract Guid ID { get; }

    [JsonPropertyName("TrustedIps")]
    public List<string> TrustedIps { get; set; }

    [JsonPropertyName("MandatoryUsers")]
    public List<Guid> MandatoryUsers { get; set; }

    [JsonPropertyName("MandatoryGroups")]
    public List<Guid> MandatoryGroups { get; set; }

    public abstract T GetDefault();
}


public abstract class TfaSettingsHelperBase<T> where T : TfaSettingsBase<T>, new()
{
    private readonly UserManager _userManager;
    private readonly SettingsManager _settingsManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TfaSettingsHelperBase(
        SettingsManager settingsManager,
        IHttpContextAccessor httpContextAccessor,
        UserManager userManager)
    {
        _settingsManager = settingsManager;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<bool> TfaEnabledForUserAsync(Guid userGuid)
    {
        var settings = await _settingsManager.LoadAsync<T>();

        if (!settings.EnableSetting)
        {
            return false;
        }

        if (settings.MandatoryGroups != null)
        {
            foreach (var mandatory in settings.MandatoryGroups)
            {
                if (await _userManager.IsUserInGroupAsync(userGuid, mandatory))
                {
                    return true;
                }
            }
        }

        if (settings.MandatoryUsers != null)
        {
            foreach (var mandatory in settings.MandatoryUsers)
            {
                if (mandatory == userGuid)
                {
                    return true;
                }
            }
        }

        if (settings.TrustedIps != null && settings.TrustedIps.Any())
        {
            var requestIP = MessageSettings.GetIP(_httpContextAccessor.HttpContext.Request);
            if (!string.IsNullOrWhiteSpace(requestIP) && settings.TrustedIps.Any(trustedIp => IPSecurity.IPSecurity.MatchIPs(requestIP, trustedIp)))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsVisibleSettings
    {
        get { return SetupInfo.IsVisibleSettings<T>(); }
    }

    public virtual bool Enable
    {
        get { return _settingsManager.Load<T>().EnableSetting; }
        set
        {
            T settings;
            if (value)
            {
                settings = _settingsManager.Load<T>();
                settings.EnableSetting = value;
            }
            else
            {
                settings = new T();
            }
            _settingsManager.Save(settings);
        }
    }
}
