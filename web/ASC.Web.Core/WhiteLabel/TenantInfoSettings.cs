namespace ASC.Web.Core.WhiteLabel
{
    [Serializable]
    public class TenantInfoSettings : ISettings
    {
        [JsonPropertyName("LogoSize")]
        public Size CompanyLogoSize { get; internal set; }

        [JsonPropertyName("LogoFileName")]
        public string CompanyLogoFileName { get; set; }

        [JsonPropertyName("Default")]
        internal bool IsDefault { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TenantInfoSettings()
            {
                IsDefault = true
            };
        }

        public Guid ID
        {
            get { return new Guid("{5116B892-CCDD-4406-98CD-4F18297C0C0A}"); }
        }
    }

    [Scope]
    public class TenantInfoSettingsHelper
    {
        private WebImageSupplier WebImageSupplier { get; }
        private StorageFactory StorageFactory { get; }
        private TenantManager TenantManager { get; }
        private IConfiguration Configuration { get; }

        public TenantInfoSettingsHelper(
            WebImageSupplier webImageSupplier,
            StorageFactory storageFactory,
            TenantManager tenantManager,
            IConfiguration configuration)
        {
            WebImageSupplier = webImageSupplier;
            StorageFactory = storageFactory;
            TenantManager = tenantManager;
            Configuration = configuration;
        }
        public void RestoreDefault(TenantInfoSettings tenantInfoSettings, TenantLogoManager tenantLogoManager)
        {
            RestoreDefaultTenantName();
            RestoreDefaultLogo(tenantInfoSettings, tenantLogoManager);
        }

        public void RestoreDefaultTenantName()
        {
            var currentTenant = TenantManager.GetCurrentTenant();
            currentTenant.Name = Configuration["web:portal-name"] ?? "Cloud Office Applications";
            TenantManager.SaveTenant(currentTenant);
        }

        public void RestoreDefaultLogo(TenantInfoSettings tenantInfoSettings, TenantLogoManager tenantLogoManager)
        {
            tenantInfoSettings.IsDefault = true;

            var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().Id.ToString(), "logo");
            try
            {
                store.DeleteFilesAsync("", "*", false).Wait();
            }
            catch
            {
            }
            tenantInfoSettings.CompanyLogoSize = default;

            tenantLogoManager.RemoveMailLogoDataFromCache();
        }

        public void SetCompanyLogo(string companyLogoFileName, byte[] data, TenantInfoSettings tenantInfoSettings, TenantLogoManager tenantLogoManager)
        {
            var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().Id.ToString(), "logo");

            if (!tenantInfoSettings.IsDefault)
            {
                try
                {
                    store.DeleteFilesAsync("", "*", false).Wait();
                }
                catch
                {
                }
            }
            using (var memory = new MemoryStream(data))
            using (var image = Image.Load(memory))
            {
                tenantInfoSettings.CompanyLogoSize = image.Size();
                memory.Seek(0, SeekOrigin.Begin);
                store.SaveAsync(companyLogoFileName, memory).Wait();
                tenantInfoSettings.CompanyLogoFileName = companyLogoFileName;
            }
            tenantInfoSettings.IsDefault = false;

            tenantLogoManager.RemoveMailLogoDataFromCache();
        }

        public string GetAbsoluteCompanyLogoPath(TenantInfoSettings tenantInfoSettings)
        {
            if (tenantInfoSettings.IsDefault)
            {
                return WebImageSupplier.GetAbsoluteWebPath("logo/dark_general.png");
            }

            var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().Id.ToString(), "logo");
            return store.GetUriAsync(tenantInfoSettings.CompanyLogoFileName ?? "").Result.ToString();
        }

        /// <summary>
        /// Get logo stream or null in case of default logo
        /// </summary>
        public Stream GetStorageLogoData(TenantInfoSettings tenantInfoSettings)
        {
            if (tenantInfoSettings.IsDefault) return null;

            var storage = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().Id.ToString(CultureInfo.InvariantCulture), "logo");

            if (storage == null) return null;

            var fileName = tenantInfoSettings.CompanyLogoFileName ?? "";

            return storage.IsFileAsync(fileName).Result ? storage.GetReadStreamAsync(fileName).Result : null;
        }
    }
}