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
        get { return "letterlogodata" + TenantManager.GetCurrentTenant().Id; }
    }

    public bool WhiteLabelEnabled
    {
        get;
        private set;
    }

    private ICache Cache { get; }
    private ICacheNotify<TenantLogoCacheItem> CacheNotify { get; }

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
        TenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        SettingsManager = settingsManager;
        TenantInfoSettingsHelper = tenantInfoSettingsHelper;
        TenantManager = tenantManager;
        AuthContext = authContext;
        Configuration = configuration;
        var hideSettings = (Configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
        WhiteLabelEnabled = !hideSettings.Contains("WhiteLabel", StringComparer.CurrentCultureIgnoreCase);
        Cache = cache;
        CacheNotify = cacheNotify;
    }

    public string GetFavicon(bool general, bool timeParam)
    {
        string faviconPath;
        var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
        if (WhiteLabelEnabled)
        {
            faviconPath = TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, general);
            if (timeParam)
            {
                var now = DateTime.Now;
                faviconPath = string.Format("{0}?t={1}", faviconPath, now.Ticks);
            }
        }
        else
        {
            faviconPath = TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Favicon, general);
        }

        return faviconPath;
    }

    public string GetTopLogo(bool general)//LogoLightSmall
    {
        var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, general);
        }
        return TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LightSmall, general);
    }

    public string GetLogoDark(bool general)
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
            return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, general);
        }

        /*** simple scheme ***/
        return TenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(SettingsManager.Load<TenantInfoSettings>());
        /***/
    }

    public string GetLogoDocsEditor(bool general)
    {
        var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, general);
        }
        return TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, general);
    }

    public string GetLogoDocsEditorEmbed(bool general)
    {
        var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditorEmbed, general);
        }
        return TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditorEmbed, general);
    }


    public string GetLogoText()
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

            return tenantWhiteLabelSettings.GetLogoText(SettingsManager) ?? TenantWhiteLabelSettings.DefaultLogoText;
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
        return !AuthContext.IsAuthenticated;
    }

    public bool WhiteLabelPaid
    {
        get
        {
            return TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().Id).WhiteLabel;
        }
    }

    private TenantWhiteLabelSettingsHelper TenantWhiteLabelSettingsHelper { get; }
    private SettingsManager SettingsManager { get; }
    private TenantInfoSettingsHelper TenantInfoSettingsHelper { get; }
    private TenantManager TenantManager { get; }
    private AuthContext AuthContext { get; }
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Get logo stream or null in case of default logo
    /// </summary>
    public Stream GetWhitelabelMailLogo()
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
            return TenantWhiteLabelSettingsHelper.GetWhitelabelLogoData(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, true);
        }

        /*** simple scheme ***/
        return TenantInfoSettingsHelper.GetStorageLogoData(SettingsManager.Load<TenantInfoSettings>());
        /***/
    }


    public byte[] GetMailLogoDataFromCache()
    {
        return Cache.Get<byte[]>(CacheKey);
    }

    public void InsertMailLogoDataToCache(byte[] data)
    {
        Cache.Insert(CacheKey, data, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));
    }

    public void RemoveMailLogoDataFromCache()
    {
        CacheNotify.Publish(new TenantLogoCacheItem() { Key = CacheKey }, Common.Caching.CacheNotifyAction.Remove);
    }
}
