using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Studio.UserControls.Statistics;
using System;
using System.Linq;

namespace ASC.Data.Backup
{

    public class BackupHelper
    {
        public const long AvailableZipSize = 10 * 1024 * 1024 * 1024L;
        private readonly Guid mailStorageTag = new Guid("666ceac1-4532-4f8c-9cba-8f510eca2fd1");
        private TenantManager tenantManager;
        private CoreBaseSettings coreBaseSettings;
        private TenantStatisticsProvider tenantStatisticsProvider;
        public BackupHelper(TenantManager tenantManager, CoreBaseSettings coreBaseSettings, TenantStatisticsProvider tenantStatisticsProvider)
        {
            this.tenantManager = tenantManager;
            this.coreBaseSettings = coreBaseSettings;
            this.tenantStatisticsProvider = tenantStatisticsProvider;
        }
        public BackupAvailableSize GetAvailableSize(int tenantId)
        {
            if (coreBaseSettings.Standalone)
                return BackupAvailableSize.Available;

            var size = tenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(tenantId))
                        .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty && !new Guid(r.Tag).Equals(mailStorageTag))
                        .Sum(r => r.Counter);
            if (size > AvailableZipSize)
            {
                return BackupAvailableSize.NotAvailable;
            }

            size = tenantStatisticsProvider.GetUsedSize(tenantId);
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
