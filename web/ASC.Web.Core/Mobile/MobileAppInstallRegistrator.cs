using DbContext = ASC.Core.Common.EF.Context.DbContext;

namespace ASC.Web.Core.Mobile
{
    [Scope]
    public class MobileAppInstallRegistrator : IMobileAppInstallRegistrator
    {
        private Lazy<DbContext> LazyDbContext { get; }
        private DbContext DbContext { get => LazyDbContext.Value; }

        public MobileAppInstallRegistrator(DbContextManager<DbContext> dbContext)
        {
            LazyDbContext = new Lazy<DbContext>(() => dbContext.Value);
        }

        public void RegisterInstall(string userEmail, MobileAppType appType)
        {
            var mai = new MobileAppInstall
            {
                AppType = (int)appType,
                UserEmail = userEmail,
                RegisteredOn = DateTime.UtcNow,
                LastSign = DateTime.UtcNow
            };

            DbContext.MobileAppInstall.Add(mai);
            DbContext.SaveChanges();
        }

        public bool IsInstallRegistered(string userEmail, MobileAppType? appType)
        {
            var q = DbContext.MobileAppInstall.Where(r => r.UserEmail == userEmail);

            if (appType.HasValue)
            {
                q = q.Where(r => r.AppType == (int)appType.Value);
            }

            return q.Any();
        }
    }
}