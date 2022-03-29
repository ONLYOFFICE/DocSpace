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
using System.Text.Json.Serialization;

using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core.Quota
{
    public class QuotaWrapper
    {
        public ulong StorageSize { get; set; }

        public ulong MaxFileSize { get; set; }

        public ulong UsedSize { get; set; }

        public int MaxUsersCount { get; set; }

        public int UsersCount { get; set; }

        public ulong AvailableSize
        {
            get { return Math.Max(0, StorageSize > UsedSize ? StorageSize - UsedSize : 0); }
            set { throw new NotImplementedException(); }
        }

        public int AvailableUsersCount
        {
            get { return Math.Max(0, MaxUsersCount - UsersCount); }
            set { throw new NotImplementedException(); }
        }

        public IList<QuotaUsage> StorageUsage { get; set; }

        public long UserStorageSize { get; set; }

        public long UserUsedSize { get; set; }

        public long UserAvailableSize
        {
            get { return Math.Max(0, UserStorageSize - UserUsedSize); }
            set { throw new NotImplementedException(); }
        }

        public long MaxVisitors { get; set; }

        public long VisitorsCount { get; set; }

        [JsonIgnore]
        private TenantExtra TenantExtra { get; }

        [JsonIgnore]
        private TenantStatisticsProvider TenantStatisticsProvider { get; }

        [JsonIgnore]
        private WebItemManager WebItemManager { get; }

        public QuotaWrapper()
        {

        }

        public QuotaWrapper(
            Tenant tenant,
            CoreBaseSettings coreBaseSettings,
            CoreConfiguration configuration,
            TenantExtra tenantExtra,
            TenantStatisticsProvider tenantStatisticsProvider,
            AuthContext authContext,
            SettingsManager settingsManager,
            WebItemManager webItemManager,
            Constants constants)
        {
            TenantExtra = tenantExtra;
            TenantStatisticsProvider = tenantStatisticsProvider;
            WebItemManager = webItemManager;
            var quota = TenantExtra.GetTenantQuota();
            var quotaRows = TenantStatisticsProvider.GetQuotaRows(tenant.TenantId).ToList();

            StorageSize = (ulong)Math.Max(0, quota.MaxTotalSize);
            UsedSize = (ulong)Math.Max(0, quotaRows.Sum(r => r.Counter));
            MaxUsersCount = quota.ActiveUsers;
            UsersCount = coreBaseSettings.Personal ? 1 : TenantStatisticsProvider.GetUsersCount();
            MaxVisitors = coreBaseSettings.Standalone ? -1 : constants.CoefficientOfVisitors * quota.ActiveUsers;
            VisitorsCount = coreBaseSettings.Personal ? 0 : TenantStatisticsProvider.GetVisitorsCount();

            StorageUsage = quotaRows
                    .Select(x => new QuotaUsage { Path = x.Path.TrimStart('/').TrimEnd('/'), Size = x.Counter, })
                    .ToList();

            if (coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
            {
                UserStorageSize = configuration.PersonalMaxSpace(settingsManager);

                var webItem = WebItemManager[WebItemManager.DocumentsProductID];
                if (webItem.Context.SpaceUsageStatManager is IUserSpaceUsage spaceUsageManager)
                {
                    UserUsedSize = spaceUsageManager.GetUserSpaceUsageAsync(authContext.CurrentAccount.ID).Result;
                }
            }

            MaxFileSize = Math.Min(AvailableSize, (ulong)quota.MaxFileSize);
        }

        public static QuotaWrapper GetSample()
        {
            return new QuotaWrapper
            {
                MaxFileSize = 25 * 1024 * 1024,
                StorageSize = 1024 * 1024 * 1024,
                UsedSize = 250 * 1024 * 1024,
                StorageUsage = new List<QuotaUsage>
                        {
                            new QuotaUsage { Size = 100*1024*1024, Path = "crm" },
                            new QuotaUsage { Size = 150*1024*1024, Path = "files" }
                        }
            };
        }

        public class QuotaUsage
        {
            public string Path { get; set; }

            public long Size { get; set; }
        }
    }
}