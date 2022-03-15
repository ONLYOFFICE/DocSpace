namespace ASC.Web.Core.WhiteLabel
{
    [Scope]
    public class TenantLogoManager
    {
        private string CacheKey
        {
            get { return "letterlogodata" + TenantManager.GetCurrentTenant().Id; }
        }

        public bool WhiteLabelEnabled
        {
            get;
            private set;
        }

        private ICache Cache { get; }
        private ICacheNotify<TenantLogoCacheItem> CacheNotify { get; }

        public TenantLogoManager(
            TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
            SettingsManager settingsManager,
            TenantInfoSettingsHelper tenantInfoSettingsHelper,
            TenantManager tenantManager,
            AuthContext authContext,
            IConfiguration configuration,
            ICacheNotify<TenantLogoCacheItem> cacheNotify,
            ICache cache)
        {
            TenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
            SettingsManager = settingsManager;
            TenantInfoSettingsHelper = tenantInfoSettingsHelper;
            TenantManager = tenantManager;
            AuthContext = authContext;
            Configuration = configuration;
            var hideSettings = (Configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
            WhiteLabelEnabled = !hideSettings.Contains("WhiteLabel", StringComparer.CurrentCultureIgnoreCase);
            Cache = cache;
            CacheNotify = cacheNotify;
        }

        public string GetFavicon(bool general, bool timeParam)
        {
            string faviconPath;
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
            if (WhiteLabelEnabled)
            {
                faviconPath = TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, general);
                if (timeParam)
                {
                    var now = DateTime.Now;
                    faviconPath = string.Format("{0}?t={1}", faviconPath, now.Ticks);
                }
            }
            else
            {
                faviconPath = TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Favicon, general);
            }

            return faviconPath;
        }

        public string GetTopLogo(bool general)//LogoLightSmall
        {
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

            if (WhiteLabelEnabled)
            {
                return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, general);
            }
            return TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LightSmall, general);
        }

        public string GetLogoDark(bool general)
        {
            if (WhiteLabelEnabled)
            {
                var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
                return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, general);
            }

            /*** simple scheme ***/
            return TenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(SettingsManager.Load<TenantInfoSettings>());
            /***/
        }

        public string GetLogoDocsEditor(bool general)
        {
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

            if (WhiteLabelEnabled)
            {
                return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, general);
            }
            return TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, general);
        }

        public string GetLogoDocsEditorEmbed(bool general)
        {
            var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

            if (WhiteLabelEnabled)
            {
                return TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditorEmbed, general);
            }
            return TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditorEmbed, general);
        }


        public string GetLogoText()
        {
            if (WhiteLabelEnabled)
            {
                var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

                return tenantWhiteLabelSettings.GetLogoText(SettingsManager) ?? TenantWhiteLabelSettings.DefaultLogoText;
            }
            return TenantWhiteLabelSettings.DefaultLogoText;
        }

        public bool IsRetina(HttpRequest request)
        {
            if (request != null)
            {
                var cookie = request.Cookies["is_retina"];
                if (cookie != null && !string.IsNullOrEmpty(cookie))
                {
                    if (bool.TryParse(cookie, out var result))
                    {
                        return result;
                    }
                }
            }
            return !AuthContext.IsAuthenticated;
        }

        public bool WhiteLabelPaid
        {
            get
            {
                return TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().Id).WhiteLabel;
            }
        }

        private TenantWhiteLabelSettingsHelper TenantWhiteLabelSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }
        private TenantInfoSettingsHelper TenantInfoSettingsHelper { get; }
        private TenantManager TenantManager { get; }
        private AuthContext AuthContext { get; }
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Get logo stream or null in case of default logo
        /// </summary>
        public Stream GetWhitelabelMailLogo()
        {
            if (WhiteLabelEnabled)
            {
                var tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();
                return TenantWhiteLabelSettingsHelper.GetWhitelabelLogoData(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, true);
            }

            /*** simple scheme ***/
            return TenantInfoSettingsHelper.GetStorageLogoData(SettingsManager.Load<TenantInfoSettings>());
            /***/
        }


        public byte[] GetMailLogoDataFromCache()
        {
            return Cache.Get<byte[]>(CacheKey);
        }

        public void InsertMailLogoDataToCache(byte[] data)
        {
            Cache.Insert(CacheKey, data, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));
        }

        public void RemoveMailLogoDataFromCache()
        {
            CacheNotify.Publish(new TenantLogoCacheItem() { Key = CacheKey }, Common.Caching.CacheNotifyAction.Remove);
        }
    }
}