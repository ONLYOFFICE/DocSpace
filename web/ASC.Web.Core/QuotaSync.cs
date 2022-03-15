namespace ASC.Web.Studio.Core.Quota
{
    public class QuotaSync
    {
        public const string TenantIdKey = "tenantID";
        protected DistributedTask TaskInfo { get; private set; }
        private int TenantId { get; set; }
        private IServiceProvider ServiceProvider { get; }

        public QuotaSync(int tenantId, IServiceProvider serviceProvider)
        {
            TenantId = tenantId;
            TaskInfo = new DistributedTask();
            ServiceProvider = serviceProvider;
        }

        public void RunJob()//DistributedTask distributedTask, CancellationToken cancellationToken)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<QuotaSyncScope>();
            var (tenantManager, storageFactoryConfig, storageFactory) = scopeClass;
            tenantManager.SetCurrentTenant(TenantId);

            var storageModules = storageFactoryConfig.GetModuleList(string.Empty);

            foreach (var module in storageModules)
            {
                var storage = storageFactory.GetStorage(TenantId.ToString(), module);
                storage.ResetQuotaAsync("").Wait();

                var domains = storageFactoryConfig.GetDomainList(string.Empty, module);
                foreach (var domain in domains)
                {
                    storage.ResetQuotaAsync(domain).Wait();
                }

            }
        }

        public virtual DistributedTask GetDistributedTask()
        {
            TaskInfo.SetProperty(TenantIdKey, TenantId);
            return TaskInfo;
        }
    }

    class QuotaSyncScope
    {
        private TenantManager TenantManager { get; }
        private StorageFactoryConfig StorageFactoryConfig { get; }
        private StorageFactory StorageFactory { get; }

        public QuotaSyncScope(TenantManager tenantManager, StorageFactoryConfig storageFactoryConfig, StorageFactory storageFactory)
        {
            TenantManager = tenantManager;
            StorageFactoryConfig = storageFactoryConfig;
            StorageFactory = storageFactory;
        }

        public void Deconstruct(out TenantManager tenantManager, out StorageFactoryConfig storageFactoryConfig, out StorageFactory storageFactory)
        {
            tenantManager = TenantManager;
            storageFactoryConfig = StorageFactoryConfig;
            storageFactory = StorageFactory;
        }
    }
}