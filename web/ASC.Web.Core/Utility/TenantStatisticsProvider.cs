using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.UserControls.Statistics
{
    [Scope]
    public class TenantStatisticsProvider
    {
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }

        public TenantStatisticsProvider(UserManager userManager, TenantManager tenantManager)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
        }

        public int GetUsersCount()
        {
            return UserManager.GetUsersByGroup(Constants.GroupUser.ID).Length;
        }

        public int GetVisitorsCount()
        {
            return UserManager.GetUsersByGroup(Constants.GroupVisitor.ID).Length;
        }

        public int GetAdminsCount()
        {
            return UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).Length;
        }


        public long GetUsedSize()
        {
            return GetUsedSize(TenantManager.GetCurrentTenant().Id);
        }

        public long GetUsedSize(int tenant)
        {
            return GetQuotaRows(tenant).Sum(r => r.Counter);
        }

        public long GetUsedSize(Guid moduleId)
        {
            return GetQuotaRows(TenantManager.GetCurrentTenant().Id).Where(r => new Guid(r.Tag).Equals(moduleId)).Sum(r => r.Counter);
        }

        public IEnumerable<TenantQuotaRow> GetQuotaRows(int tenant)
        {
            return TenantManager.FindTenantQuotaRows(tenant)
                .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty);
        }
    }
}