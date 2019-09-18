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
using ASC.Common.Utils;
using ASC.Core.Configuration;
using ASC.Core.Tenants;
using Newtonsoft.Json;

namespace ASC.Core
{
    public class CoreBaseSettings
    {
        private bool? standalone;
        private bool? personal;
        private bool? customMode;

        public bool Standalone
        {
            get { return standalone ?? (bool)(standalone = ConfigurationManager.AppSettings["core:base-domain"] == "localhost"); }
        }

        public bool Personal
        {
            get { return personal ?? (bool)(personal = ConfigurationManager.AppSettings["core.personal"] == "true"); }
        }

        public bool CustomMode
        {
            get { return customMode ?? (bool)(customMode = ConfigurationManager.AppSettings["core.custom-mode"] == "true"); }
        }
    }

    public class CoreSettings
    {
        private string basedomain;

        public string BaseDomain
        {
            get
            {
                if (basedomain == null)
                {
                    basedomain = ConfigurationManager.AppSettings["core:base-domain"] ?? string.Empty;
                }

                string result;
                if (CoreBaseSettings.Standalone || string.IsNullOrEmpty(basedomain))
                {
                    result = GetSetting("BaseDomain") ?? basedomain;
                }
                else
                {
                    result = basedomain;
                }
                return result;
            }
            set
            {
                if (CoreBaseSettings.Standalone || string.IsNullOrEmpty(basedomain))
                {
                    SaveSetting("BaseDomain", value);
                }
            }
        }

        public ITenantService TenantService { get; }
        public CoreBaseSettings CoreBaseSettings { get; }

        public CoreSettings(ITenantService tenantService, CoreBaseSettings coreBaseSettings)
        {
            TenantService = tenantService;
            CoreBaseSettings = coreBaseSettings;
        }

        public void SaveSetting(string key, string value, int tenant = Tenant.DEFAULT_TENANT)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
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
                throw new ArgumentNullException("key");
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
                            SaveSetting("PortalId", key = Guid.NewGuid().ToString());
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

                return ConfigurationManager.AppSettings["core:payment:region"] + tenant;
            }
        }

        public string GetAffiliateId(int tenant)
        {
            var t = TenantService.GetTenant(tenant);
            if (t != null && !string.IsNullOrWhiteSpace(t.AffiliateId))
                return t.AffiliateId;

            return null;
        }
    }

    public class CoreConfiguration
    {
        private long? personalMaxSpace;

        public CoreConfiguration(CoreBaseSettings coreBaseSettings, CoreSettings coreSettings, TenantManager tenantManager)
        {
            CoreBaseSettings = coreBaseSettings;
            CoreSettings = coreSettings;
            TenantManager = tenantManager;
        }

        public bool Standalone
        {
            get  => CoreBaseSettings.Standalone;
        }

        public bool Personal
        {
            get => CoreBaseSettings.Personal;
        }

        public bool CustomMode
        {
            get => CoreBaseSettings.CustomMode;
        }

        public long PersonalMaxSpace(PersonalQuotaSettings personalQuotaSettings)
        {
            var quotaSettings = personalQuotaSettings.LoadForCurrentUser();

            if (quotaSettings.MaxSpace != long.MaxValue)
                return quotaSettings.MaxSpace;

            if (personalMaxSpace.HasValue)
                return personalMaxSpace.Value;


            if (!long.TryParse(ConfigurationManager.AppSettings["core.personal.maxspace"], out var value))
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

        public string BaseDomain
        {
            get => CoreSettings.BaseDomain;
            set => CoreSettings.BaseDomain = value;
        }
        public CoreBaseSettings CoreBaseSettings { get; }
        public CoreSettings CoreSettings { get; }
        public TenantManager TenantManager { get; }

        #region Methods Get/Save Setting

        public void SaveSetting(string key, string value, int tenant = Tenant.DEFAULT_TENANT) => CoreSettings.SaveSetting(key, value, tenant);

        public string GetSetting(string key, int tenant = Tenant.DEFAULT_TENANT) => CoreSettings.GetSetting(key, tenant);

        #endregion

        public string GetKey(int tenant) => CoreSettings.GetKey(tenant);

        public string GetAffiliateId(int tenant) => CoreSettings.GetAffiliateId(tenant);

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