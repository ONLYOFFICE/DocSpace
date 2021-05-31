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
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Web.Studio.Utility
{
    [Scope]
    public class TenantExtra
    {
        private UserManager UserManager { get; }
        private TenantStatisticsProvider TenantStatisticsProvider { get; }
        private AuthContext AuthContext { get; }
        private TenantManager TenantManager { get; }
        private PaymentManager PaymentManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private LicenseReader LicenseReader { get; }
        private SetupInfo SetupInfo { get; }
        private SettingsManager SettingsManager { get; }

        public TenantExtra(
            UserManager userManager,
            TenantStatisticsProvider tenantStatisticsProvider,
            AuthContext authContext,
            TenantManager tenantManager,
            PaymentManager paymentManager,
            CoreBaseSettings coreBaseSettings,
            LicenseReader licenseReader,
            SetupInfo setupInfo,
            SettingsManager settingsManager)
        {
            UserManager = userManager;
            TenantStatisticsProvider = tenantStatisticsProvider;
            AuthContext = authContext;
            TenantManager = tenantManager;
            PaymentManager = paymentManager;
            CoreBaseSettings = coreBaseSettings;
            LicenseReader = licenseReader;
            SetupInfo = setupInfo;
            SettingsManager = settingsManager;
        }

        public bool EnableTariffSettings
        {
            get
            {
                return
                    SetupInfo.IsVisibleSettings<TariffSettings>()
                    && !SettingsManager.Load<TenantAccessSettings>().Anyone
                    && (!CoreBaseSettings.Standalone || !string.IsNullOrEmpty(LicenseReader.LicensePath))
                    && string.IsNullOrEmpty(SetupInfo.AmiMetaUrl);
            }
        }

        public bool Saas
        {
            get { return !CoreBaseSettings.Standalone; }
        }

        public bool Enterprise
        {
            get { return CoreBaseSettings.Standalone && !string.IsNullOrEmpty(LicenseReader.LicensePath); }
        }

        public bool Opensource
        {
            get { return CoreBaseSettings.Standalone && string.IsNullOrEmpty(LicenseReader.LicensePath); }
        }

        public bool EnterprisePaid
        {
            get { return Enterprise && GetCurrentTariff().State < TariffState.NotPaid; }
        }

        public bool EnableControlPanel
        {
            get
            {
                return CoreBaseSettings.Standalone &&
                    !string.IsNullOrEmpty(SetupInfo.ControlPanelUrl) &&
                    GetTenantQuota().ControlPanel &&
                    GetCurrentTariff().State < TariffState.NotPaid &&
                    UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsAdmin(UserManager);
            }
        }

        public bool EnableDocbuilder
        {
            get { return !Opensource; }
        }
        public string GetAppsPageLink()
        {
            return VirtualPathUtility.ToAbsolute("~/AppInstall.aspx");
        }

        public string GetTariffPageLink()
        {
            return VirtualPathUtility.ToAbsolute("~/Tariffs.aspx");
        }

        public Tariff GetCurrentTariff()
        {
            return PaymentManager.GetTariff(TenantManager.GetCurrentTenant().TenantId);
        }

        public TenantQuota GetTenantQuota()
        {
            return GetTenantQuota(TenantManager.GetCurrentTenant().TenantId);
        }

        public TenantQuota GetTenantQuota(int tenant)
        {
            return TenantManager.GetTenantQuota(tenant);
        }

        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return TenantManager.GetTenantQuotas();
        }

        private TenantQuota GetPrevQuota(TenantQuota curQuota)
        {
            TenantQuota prev = null;
            foreach (var quota in GetTenantQuotas().OrderBy(r => r.ActiveUsers).Where(r => r.Year == curQuota.Year && r.Year3 == curQuota.Year3))
            {
                if (quota.Id == curQuota.Id)
                    return prev;

                prev = quota;
            }
            return null;
        }

        public int GetPrevUsersCount(TenantQuota quota)
        {
            var prevQuota = GetPrevQuota(quota);
            if (prevQuota == null || prevQuota.Trial)
                return 1;
            return prevQuota.ActiveUsers + 1;
        }

        public int GetRightQuotaId()
        {
            var q = GetRightQuota();
            return q != null ? q.Id : 0;
        }

        public TenantQuota GetRightQuota()
        {
            var usedSpace = TenantStatisticsProvider.GetUsedSize();
            var needUsersCount = TenantStatisticsProvider.GetUsersCount();
            var quotas = GetTenantQuotas();

            return quotas.OrderBy(q => q.ActiveUsers)
                         .ThenBy(q => q.Year)
                         .FirstOrDefault(q =>
                                         q.ActiveUsers > needUsersCount
                                         && q.MaxTotalSize > usedSpace
                                         && !q.Free
                                         && !q.Trial);
        }

        public int GetRemainingCountUsers()
        {
            return GetTenantQuota().ActiveUsers - TenantStatisticsProvider.GetUsersCount();
        }

        public bool UpdatedWithoutLicense
        {
            get
            {
                DateTime licenseDay;
                return CoreBaseSettings.Standalone
                       && (licenseDay = GetCurrentTariff().LicenseDate.Date) < DateTime.Today
                       && licenseDay < LicenseReader.VersionReleaseDate;
            }
        }

        public void DemandControlPanelPermission()
        {
            if (!CoreBaseSettings.Standalone || SettingsManager.Load<TenantControlPanelSettings>().LimitedAccess)
            {
                throw new System.Security.SecurityException(Resource.ErrorAccessDenied);
            }
        }

        public bool IsNotPaid()
        {
            Tariff tariff;
            return EnableTariffSettings
                   && ((tariff = GetCurrentTariff()).State >= TariffState.NotPaid
                       || Enterprise && !EnterprisePaid && tariff.LicenseDate == DateTime.MaxValue);
        }

        /// <summary>
        /// Max possible file size for not chunked upload. Less or equal than 100 mb.
        /// </summary>
        public long MaxUploadSize
        {
            get { return Math.Min(SetupInfo.AvailableFileSize, MaxChunkedUploadSize); }
        }

        /// <summary>
        /// Max possible file size for chunked upload.
        /// </summary>
        public long MaxChunkedUploadSize
        {
            get
            {
                var diskQuota = GetTenantQuota();
                if (diskQuota != null)
                {
                    var usedSize = TenantStatisticsProvider.GetUsedSize();
                    var freeSize = Math.Max(diskQuota.MaxTotalSize - usedSize, 0);
                    return Math.Min(freeSize, diskQuota.MaxFileSize);
                }
                return SetupInfo.ChunkUploadSize;
            }
        }
    }
}