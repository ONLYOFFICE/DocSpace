namespace ASC.Web.Core.Mobile
{
    public class CachedMobileAppInstallRegistrator : IMobileAppInstallRegistrator
    {
        private ICache Cache { get; set; }
        private readonly TimeSpan cacheExpiration;
        private readonly IMobileAppInstallRegistrator registrator;

        private TenantManager TenantManager { get; }

        public CachedMobileAppInstallRegistrator(MobileAppInstallRegistrator registrator, TenantManager tenantManager, ICache cache)
            : this(registrator, TimeSpan.FromMinutes(30), tenantManager, cache)
        {
        }

        public CachedMobileAppInstallRegistrator(MobileAppInstallRegistrator registrator, TimeSpan cacheExpiration, TenantManager tenantManager, ICache cache)
        {
            Cache = cache;
            TenantManager = tenantManager;
            this.registrator = registrator ?? throw new ArgumentNullException(nameof(registrator));
            this.cacheExpiration = cacheExpiration;
        }

        public void RegisterInstall(string userEmail, MobileAppType appType)
        {
            if (string.IsNullOrEmpty(userEmail)) return;
            registrator.RegisterInstall(userEmail, appType);
            Cache.Insert(GetCacheKey(userEmail, null), true, cacheExpiration);
            Cache.Insert(GetCacheKey(userEmail, appType), true, cacheExpiration);
        }

        public bool IsInstallRegistered(string userEmail, MobileAppType? appType)
        {
            if (string.IsNullOrEmpty(userEmail)) return false;

            var fromCache = Cache.Get<string>(GetCacheKey(userEmail, appType));


            if (bool.TryParse(fromCache, out var cachedValue))
            {
                return cachedValue;
            }

            var isRegistered = registrator.IsInstallRegistered(userEmail, appType);
            Cache.Insert(GetCacheKey(userEmail, appType), isRegistered.ToString(), cacheExpiration);
            return isRegistered;
        }

        private string GetCacheKey(string userEmail, MobileAppType? appType)
        {
            var cacheKey = appType.HasValue ? userEmail + "/" + appType.ToString() : userEmail;

            return string.Format("{0}:mobile:{1}", TenantManager.GetCurrentTenant().Id, cacheKey);
        }
    }
}
