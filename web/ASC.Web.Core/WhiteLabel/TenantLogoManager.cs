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

namespace ASC.Web.Core.WhiteLabel;

[Scope]
public class TenantLogoManager
{
    private string CacheKey
    {
        get { return "letterlogodata" + _tenantManager.GetCurrentTenant().Id; }
    }

    public bool WhiteLabelEnabled { get; private set; }

    private readonly ICache _cache;
    private readonly ICacheNotify<TenantLogoCacheItem> _cacheNotify;

    public TenantLogoManager(
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        SettingsManager settingsManager,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantManager tenantManager,
        AuthContext authContext,
        IConfiguration configuration,
        ICacheNotify<TenantLogoCacheItem> cacheNotify,
        ICache cache)
    {
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _settingsManager = settingsManager;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _configuration = configuration;
        var hideSettings = (_configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
        WhiteLabelEnabled = !hideSettings.Contains("WhiteLabel", StringComparer.CurrentCultureIgnoreCase);
        _cache = cache;
        _cacheNotify = cacheNotify;
    }

    public string GetFavicon(bool general, bool timeParam)
    {
        string faviconPath;
        var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();
        if (WhiteLabelEnabled)
        {
            faviconPath = _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, general);
            if (timeParam)
            {
                var now = DateTime.Now;
                faviconPath = string.Format("{0}?t={1}", faviconPath, now.Ticks);
            }
        }
        else
        {
            faviconPath = _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Favicon, general);
        }

        return faviconPath;
    }

    public string GetTopLogo(bool general)//LogoLightSmall
    {
        var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, general);
        }
        return _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LightSmall, general);
    }

    public string GetLogoDark(bool general)
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();
            return _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, general);
        }

        /*** simple scheme ***/
        return _tenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(_settingsManager.Load<TenantInfoSettings>());
        /***/
    }

    public string GetLogoDocsEditor(bool general)
    {
        var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, general);
        }
        return _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, general);
    }

    public string GetLogoDocsEditorEmbed(bool general)
    {
        var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditorEmbed, general);
        }
        return _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditorEmbed, general);
    }


    public string GetLogoText()
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

            return tenantWhiteLabelSettings.GetLogoText(_settingsManager) ?? TenantWhiteLabelSettings.DefaultLogoText;
        }
        return TenantWhiteLabelSettings.DefaultLogoText;
    }

    public bool IsRetina(HttpRequest request)
    {
        if (request != null)
        {
            var cookie = request.Cookies["is_retina"];
            if (cookie != null && !string.IsNullOrEmpty(cookie))
            {
                if (bool.TryParse(cookie, out var result))
                {
                    return result;
                }
            }
        }
        return !_authContext.IsAuthenticated;
    }

    public bool WhiteLabelPaid
    {
        get
        {
            return _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).WhiteLabel;
        }
    }

    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private readonly SettingsManager _settingsManager;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Get logo stream or null in case of default logo
    /// </summary>
    public Stream GetWhitelabelMailLogo()
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();
            return _tenantWhiteLabelSettingsHelper.GetWhitelabelLogoData(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, true);
        }

        /*** simple scheme ***/
        return _tenantInfoSettingsHelper.GetStorageLogoData(_settingsManager.Load<TenantInfoSettings>());
        /***/
    }


    public byte[] GetMailLogoDataFromCache()
    {
        return _cache.Get<byte[]>(CacheKey);
    }

    public void InsertMailLogoDataToCache(byte[] data)
    {
        _cache.Insert(CacheKey, data, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));
    }

    public void RemoveMailLogoDataFromCache()
    {
        _cacheNotify.Publish(new TenantLogoCacheItem() { Key = CacheKey }, CacheNotifyAction.Remove);
    }
}
