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


using System;
using System.IO;
using System.Linq;
using ASC.Common.Caching;
using ASC.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.Web.Core.WhiteLabel
{
    public class TenantLogoManager
    {
        private string CacheKey
        {
            get { return "letterlogodata" + TenantManager.GetCurrentTenant().TenantId; }
        }

        public bool WhiteLabelEnabled
        {
            get;
            private set;
        }

        public ICache Cache { get; }
        public ICacheNotify<TenantLogoCacheItem> CacheNotify { get; }

        public TenantLogoManager(
            TenantWhiteLabelSettings tenantWhiteLabelSettings,
            TenantInfoSettings tenantInfoSettings,
            TenantManager tenantManager,
            IConfiguration configuration,
            ICacheNotify<TenantLogoCacheItem> cacheNotify)
        {
            TenantWhiteLabelSettings = tenantWhiteLabelSettings;
            TenantInfoSettings = tenantInfoSettings;
            TenantManager = tenantManager;
            Configuration = configuration;
            var hideSettings = (Configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
            WhiteLabelEnabled = !hideSettings.Contains("WhiteLabel", StringComparer.CurrentCultureIgnoreCase);
            Cache = AscCache.Memory;
            CacheNotify = cacheNotify;
        }

        public string GetFavicon(bool general, bool timeParam)
        {
            string faviconPath;
            var tenantWhiteLabelSettings = TenantWhiteLabelSettings.Load();
            if (WhiteLabelEnabled)
            {
                faviconPath = tenantWhiteLabelSettings.GetAbsoluteLogoPath(WhiteLabelLogoTypeEnum.Favicon, general);
                if (timeParam)
                {
                    var now = DateTime.Now;
                    faviconPath = string.Format("{0}?t={1}", faviconPath, now.Ticks);
                }
            }
            else
            {
                faviconPath = tenantWhiteLabelSettings.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Favicon, general);
            }

            return faviconPath;
        }

        public string GetTopLogo(bool general)//LogoLightSmall
        {
            var tenantWhiteLabelSettings = TenantWhiteLabelSettings.Load();

            if (WhiteLabelEnabled)
            {
                return tenantWhiteLabelSettings.GetAbsoluteLogoPath(WhiteLabelLogoTypeEnum.LightSmall, general);
            }
            return tenantWhiteLabelSettings.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LightSmall, general);
        }

        public string GetLogoDark(bool general)
        {
            if (WhiteLabelEnabled)
            {
                var tenantWhiteLabelSettings = TenantWhiteLabelSettings.Load();
                return tenantWhiteLabelSettings.GetAbsoluteLogoPath(WhiteLabelLogoTypeEnum.Dark, general);
            }

            /*** simple scheme ***/
            var tenantInfoSettings = TenantInfoSettings.Load();
            return tenantInfoSettings.GetAbsoluteCompanyLogoPath();
            /***/
        }

        public string GetLogoDocsEditor(bool general)
        {
            var tenantWhiteLabelSettings = TenantWhiteLabelSettings.Load();

            if (WhiteLabelEnabled)
            {
                return tenantWhiteLabelSettings.GetAbsoluteLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, general);
            }
            return tenantWhiteLabelSettings.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, general);
        }

        public string GetLogoText()
        {
            if (WhiteLabelEnabled)
            {
                var tenantWhiteLabelSettings = TenantWhiteLabelSettings.LoadForDefaultTenant();

                return tenantWhiteLabelSettings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
            }
            return TenantWhiteLabelSettings.DefaultLogoText;
        }

        public bool IsRetina(HttpRequest request)
        {
            var isRetina = false;
            if (request != null)
            {
                var cookie = request.Cookies["is_retina"];
                if (cookie != null && !string.IsNullOrEmpty(cookie))
                {
                    if (bool.TryParse(cookie, out var result))
                    {
                        isRetina = result;
                    }
                }
            }
            return isRetina;
        }

        public bool WhiteLabelPaid
        {
            get
            {
                return TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).WhiteLabel;
            }
        }

        public TenantWhiteLabelSettings TenantWhiteLabelSettings { get; }
        public TenantInfoSettings TenantInfoSettings { get; }
        public TenantManager TenantManager { get; }
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Get logo stream or null in case of default logo
        /// </summary>
        public Stream GetWhitelabelMailLogo()
        {
            if (WhiteLabelEnabled)
            {
                var tenantWhiteLabelSettings = TenantWhiteLabelSettings.Load();
                return tenantWhiteLabelSettings.GetWhitelabelLogoData(WhiteLabelLogoTypeEnum.Dark, true);
            }

            /*** simple scheme ***/
            var tenantInfoSettings = TenantInfoSettings.Load();
            return tenantInfoSettings.GetStorageLogoData();
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
            CacheNotify.Publish(new TenantLogoCacheItem() { Key = CacheKey }, CacheNotifyAction.Remove);
        }
    }

    public static class TenantLogoManagerFactory
    {
        public static IServiceCollection AddTenantLogoManagerService(this IServiceCollection services)
        {
            services.TryAddScoped<TenantLogoManager>();

            return services
                .AddTenantWhiteLabelSettingsService()
                .AddTenantInfoSettingsService()
                .AddTenantManagerService();
        }
    }
}