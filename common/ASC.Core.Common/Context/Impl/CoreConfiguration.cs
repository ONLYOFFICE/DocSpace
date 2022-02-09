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
using System.Text;

using ASC.Common;
using ASC.Core.Caching;
using ASC.Core.Common.Settings;
using ASC.Core.Configuration;
using ASC.Core.Tenants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ASC.Core
{
    [Singletone]
    public class CoreBaseSettings
    {
        private bool? standalone;
        private string basedomain;
        private bool? personal;
        private bool? customMode;

        private IConfiguration Configuration { get; }

        public CoreBaseSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string Basedomain
        {
            get 
            {
                if (basedomain == null)
                {
                    basedomain = Configuration["core:base-domain"] ?? string.Empty;
                }
                return basedomain;
            }
        }

        public bool Standalone
        {
            get { return standalone ?? (bool)(standalone = Configuration["core:base-domain"] == "localhost"); }
        }

        public bool Personal
        {
            get
            {
                //TODO:if (CustomMode && HttpContext.Current != null && HttpContext.Current.Request.SailfishApp()) return true;
                return personal ?? (bool)(personal = string.Equals(Configuration["core:personal"], "true", StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool CustomMode
        {
            get { return customMode ?? (bool)(customMode = string.Equals(Configuration["core:custom-mode"], "true", StringComparison.OrdinalIgnoreCase));}

        }
    }

    class ConfigureCoreSettings : IConfigureNamedOptions<CoreSettings>
    {
        private IOptionsSnapshot<CachedTenantService> TenantService { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private IConfiguration Configuration { get; }

        public ConfigureCoreSettings(
            IOptionsSnapshot<CachedTenantService> tenantService,
            CoreBaseSettings coreBaseSettings,
            IConfiguration configuration
            )
        {
            TenantService = tenantService;
            CoreBaseSettings = coreBaseSettings;
            Configuration = configuration;
        }

        public void Configure(string name, CoreSettings options)
        {
            Configure(options);
            options.TenantService = TenantService.Get(name);
        }

        public void Configure(CoreSettings options)
        {
            options.Configuration = Configuration;
            options.CoreBaseSettings = CoreBaseSettings;
            options.TenantService = TenantService.Value;
        }
    }

    [Scope(typeof(ConfigureCoreSettings))]
    public class CoreSettings
    {
        public string BaseDomain
        {
            get
            {
                string result;
                if (CoreBaseSettings.Standalone || string.IsNullOrEmpty(CoreBaseSettings.Basedomain))
                {
                    result = GetSetting("BaseDomain") ?? CoreBaseSettings.Basedomain;
                }
                else
                {
                    result = CoreBaseSettings.Basedomain;
                }
                return result;
            }
            set
            {
                if (CoreBaseSettings.Standalone || string.IsNullOrEmpty(CoreBaseSettings.Basedomain))
                {
                    SaveSetting("BaseDomain", value);
                }
            }
        }

        internal ITenantService TenantService { get; set; }
        internal CoreBaseSettings CoreBaseSettings { get; set; }
        internal IConfiguration Configuration { get; set; }

        public CoreSettings()
        {

        }

        public CoreSettings(
            ITenantService tenantService,
            CoreBaseSettings coreBaseSettings,
            IConfiguration configuration)
        {
            TenantService = tenantService;
            CoreBaseSettings = coreBaseSettings;
            Configuration = configuration;
        }

        public string GetBaseDomain(string hostedRegion)
        {
            var baseHost = BaseDomain;

            if (string.IsNullOrEmpty(hostedRegion) || string.IsNullOrEmpty(baseHost) || baseHost.IndexOf('.') < 0)
            {
                return baseHost;
            }
            var subdomain = baseHost.Remove(baseHost.IndexOf('.') + 1);
            return hostedRegion.StartsWith(subdomain) ? hostedRegion : (subdomain + hostedRegion.TrimStart('.'));
        }

        public void SaveSetting(string key, string value, int tenant = Tenant.DEFAULT_TENANT)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            byte[] bytes = null;
            if (value != null)
            {
                bytes = Crypto.GetV(Encoding.UTF8.GetBytes(value), 2, true);
            }
            TenantService.SetTenantSettings(tenant, key, bytes);
        }

        public string GetSetting(string key, int tenant = Tenant.DEFAULT_TENANT)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var bytes = TenantService.GetTenantSettings(tenant, key);

            var result = bytes != null ? Encoding.UTF8.GetString(Crypto.GetV(bytes, 2, false)) : null;

            return result;
        }

        public string GetKey(int tenant)
        {
            if (CoreBaseSettings.Standalone)
            {
                var key = GetSetting("PortalId");
                if (string.IsNullOrEmpty(key))
                {
                    lock (TenantService)
                    {
                        // thread safe
                        key = GetSetting("PortalId");
                        if (string.IsNullOrEmpty(key))
                        {
                            key = Guid.NewGuid().ToString();
                            SaveSetting("PortalId", key);
                        }
                    }
                }
                return key;
            }
            else
            {
                var t = TenantService.GetTenant(tenant);
                if (t != null && !string.IsNullOrWhiteSpace(t.PaymentId))
                    return t.PaymentId;

                return Configuration["core:payment:region"] + tenant;
            }
        }

        public string GetAffiliateId(int tenant)
        {
            var t = TenantService.GetTenant(tenant);
            if (t != null && !string.IsNullOrWhiteSpace(t.AffiliateId))
                return t.AffiliateId;

            return null;
        }

        public string GetCampaign(int tenant)
        {
            var t = TenantService.GetTenant(tenant);
            if (t != null && !string.IsNullOrWhiteSpace(t.Campaign))
                return t.Campaign;

            return null;
        }
    }

    [Scope]
    public class CoreConfiguration
    {
        private long? personalMaxSpace;

        public CoreConfiguration(CoreSettings coreSettings, TenantManager tenantManager, IConfiguration configuration)
        {
            CoreSettings = coreSettings;
            TenantManager = tenantManager;
            Configuration = configuration;
        }

        public long PersonalMaxSpace(SettingsManager settingsManager)
        {
            var quotaSettings = settingsManager.LoadForCurrentUser<PersonalQuotaSettings>();

            if (quotaSettings.MaxSpace != long.MaxValue)
                return quotaSettings.MaxSpace;

            if (personalMaxSpace.HasValue)
                return personalMaxSpace.Value;


            if (!long.TryParse(Configuration["core:personal.maxspace"], out var value))
                value = long.MaxValue;

            personalMaxSpace = value;

            return personalMaxSpace.Value;
        }

        public SmtpSettings SmtpSettings
        {
            get
            {
                var isDefaultSettings = false;
                var tenant = TenantManager.GetCurrentTenant(false);

                if (tenant != null)
                {

                    var settingsValue = GetSetting("SmtpSettings", tenant.TenantId);
                    if (string.IsNullOrEmpty(settingsValue))
                    {
                        isDefaultSettings = true;
                        settingsValue = GetSetting("SmtpSettings");
                    }
                    var settings = SmtpSettings.Deserialize(settingsValue);
                    settings.IsDefaultSettings = isDefaultSettings;
                    return settings;
                }
                else
                {
                    var settingsValue = GetSetting("SmtpSettings");

                    var settings = SmtpSettings.Deserialize(settingsValue);
                    settings.IsDefaultSettings = true;
                    return settings;
                }
            }
            set { SaveSetting("SmtpSettings", value?.Serialize(), TenantManager.GetCurrentTenant().TenantId); }
        }

        private CoreSettings CoreSettings { get; }
        private TenantManager TenantManager { get; }
        private IConfiguration Configuration { get; }

        #region Methods Get/Save Setting

        public void SaveSetting(string key, string value, int tenant = Tenant.DEFAULT_TENANT)
        {
            CoreSettings.SaveSetting(key, value, tenant);
        }

        public string GetSetting(string key, int tenant = Tenant.DEFAULT_TENANT)
        {
            return CoreSettings.GetSetting(key, tenant);
        }

        #endregion

        #region Methods Get/Set Section

        public T GetSection<T>() where T : class
        {
            return GetSection<T>(typeof(T).Name);
        }

        public T GetSection<T>(int tenantId) where T : class
        {
            return GetSection<T>(tenantId, typeof(T).Name);
        }

        public T GetSection<T>(string sectionName) where T : class
        {
            return GetSection<T>(TenantManager.GetCurrentTenant().TenantId, sectionName);
        }

        public T GetSection<T>(int tenantId, string sectionName) where T : class
        {
            var serializedSection = GetSetting(sectionName, tenantId);
            if (serializedSection == null && tenantId != Tenant.DEFAULT_TENANT)
            {
                serializedSection = GetSetting(sectionName, Tenant.DEFAULT_TENANT);
            }
            return serializedSection != null ? JsonConvert.DeserializeObject<T>(serializedSection) : null;
        }

        public void SaveSection<T>(string sectionName, T section) where T : class
        {
            SaveSection(TenantManager.GetCurrentTenant().TenantId, sectionName, section);
        }

        public void SaveSection<T>(T section) where T : class
        {
            SaveSection(typeof(T).Name, section);
        }

        public void SaveSection<T>(int tenantId, T section) where T : class
        {
            SaveSection(tenantId, typeof(T).Name, section);
        }

        public void SaveSection<T>(int tenantId, string sectionName, T section) where T : class
        {
            var serializedSection = section != null ? JsonConvert.SerializeObject(section) : null;
            SaveSetting(sectionName, serializedSection, tenantId);
        }

        #endregion
    }
}