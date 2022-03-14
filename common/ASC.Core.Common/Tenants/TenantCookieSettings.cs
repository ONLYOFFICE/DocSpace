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
using System.Linq;

using ASC.Common;
using ASC.Core.Common.Settings;

using Microsoft.Extensions.Configuration;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantCookieSettings : ISettings
    {
        public int Index { get; set; }

        public int LifeTime { get; set; }


        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return GetInstance();
        }

        public bool IsDefault()
        {
            var defaultSettings = GetInstance();

            return LifeTime == defaultSettings.LifeTime;
        }

        public static TenantCookieSettings GetInstance()
        {
            return new TenantCookieSettings();
        }

        public Guid ID
        {
            get { return new Guid("{16FB8E67-E96D-4B22-B217-C80F25C5DE1B}"); }
        }
    }

    [Scope]
    public class TenantCookieSettingsHelper
    {
        public bool IsVisibleSettings { get; internal set; }
        private SettingsManager SettingsManager { get; }

        public TenantCookieSettingsHelper(IConfiguration configuration, SettingsManager settingsManager)
        {
            IsVisibleSettings = !(configuration["web:hide-settings"] ?? string.Empty)
            .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Contains("CookieSettings", StringComparer.CurrentCultureIgnoreCase);

            SettingsManager = settingsManager;
        }


        public TenantCookieSettings GetForTenant(int tenantId)
        {
            return IsVisibleSettings
                       ? SettingsManager.LoadForTenant<TenantCookieSettings>(tenantId)
                       : TenantCookieSettings.GetInstance();
        }

        public void SetForTenant(int tenantId, TenantCookieSettings settings = null)
        {
            if (!IsVisibleSettings) return;
            SettingsManager.SaveForTenant(settings ?? TenantCookieSettings.GetInstance(), tenantId);
        }

        public TenantCookieSettings GetForUser(Guid userId)
        {
            return IsVisibleSettings
                       ? SettingsManager.LoadForUser<TenantCookieSettings>(userId)
                       : TenantCookieSettings.GetInstance();
        }

        public TenantCookieSettings GetForUser(int tenantId, Guid userId)
        {
            return IsVisibleSettings
                       ? SettingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, userId)
                       : TenantCookieSettings.GetInstance();
        }

        public void SetForUser(Guid userId, TenantCookieSettings settings = null)
        {
            if (!IsVisibleSettings) return;
            SettingsManager.SaveForUser(settings ?? TenantCookieSettings.GetInstance(), userId);
        }

        public DateTime GetExpiresTime(int tenantId)
        {
            var settingsTenant = GetForTenant(tenantId);
            var expires = settingsTenant.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(settingsTenant.LifeTime);
            return expires;
        }
    }
}