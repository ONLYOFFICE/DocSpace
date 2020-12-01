using System;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Data.Backup
{
    [Scope]
    public class BackupHelper
    {
        public const long AvailableZipSize = 10 * 1024 * 1024 * 1024L;
        private readonly Guid mailStorageTag = new Guid("666ceac1-4532-4f8c-9cba-8f510eca2fd1");

        private TenantManager TenantManager { get; set; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private TenantStatisticsProvider TenantStatisticsProvider { get; set; }

        public BackupHelper(TenantManager tenantManager, CoreBaseSettings coreBaseSettings, TenantStatisticsProvider tenantStatisticsProvider)
        {
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
            TenantStatisticsProvider = tenantStatisticsProvider;
        }

        public BackupAvailableSize GetAvailableSize(int tenantId)
        {
            if (CoreBaseSettings.Standalone)
                return BackupAvailableSize.Available;

            var size = TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(tenantId))
                        .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty && !new Guid(r.Tag).Equals(mailStorageTag))
                        .Sum(r => r.Counter);
            if (size > AvailableZipSize)
            {
                return BackupAvailableSize.NotAvailable;
            }

            size = TenantStatisticsProvider.GetUsedSize(tenantId);
            if (size > AvailableZipSize)
            {
                return BackupAvailableSize.WithoutMail;
            }

            return BackupAvailableSize.Available;
        }

        public bool ExceedsMaxAvailableSize(int tenantId)
        {
            return GetAvailableSize(tenantId) != BackupAvailableSize.Available;
        }
    }

    public enum BackupAvailableSize
    {
        Available,
        WithoutMail,
        NotAvailable,
    }
}
