using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.ApiModel.ResponseDto;
public class QuotaDto
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

    public QuotaDto()
    {

    }

    public QuotaDto(
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
        var quota = tenantExtra.GetTenantQuota();
        var quotaRows = tenantStatisticsProvider.GetQuotaRows(tenant.Id).ToList();

        StorageSize = (ulong)Math.Max(0, quota.MaxTotalSize);
        UsedSize = (ulong)Math.Max(0, quotaRows.Sum(r => r.Counter));
        MaxUsersCount = quota.ActiveUsers;
        UsersCount = coreBaseSettings.Personal ? 1 : tenantStatisticsProvider.GetUsersCount();
        MaxVisitors = coreBaseSettings.Standalone ? -1 : constants.CoefficientOfVisitors * quota.ActiveUsers;
        VisitorsCount = coreBaseSettings.Personal ? 0 : tenantStatisticsProvider.GetVisitorsCount();

        StorageUsage = quotaRows
                .Select(x => new QuotaUsage { Path = x.Path.TrimStart('/').TrimEnd('/'), Size = x.Counter, })
                .ToList();

        if (coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            UserStorageSize = configuration.PersonalMaxSpace(settingsManager);

            var webItem = webItemManager[WebItemManager.DocumentsProductID];
            if (webItem.Context.SpaceUsageStatManager is IUserSpaceUsage spaceUsageManager)
            {
                UserUsedSize = spaceUsageManager.GetUserSpaceUsageAsync(authContext.CurrentAccount.ID).Result;
            }
        }

        MaxFileSize = Math.Min(AvailableSize, (ulong)quota.MaxFileSize);
    }

    public static QuotaDto GetSample()
    {
        return new QuotaDto
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