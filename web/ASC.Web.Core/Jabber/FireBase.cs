namespace ASC.Web.Core.Jabber
{
    public class FireBase : Consumer
    {
        public string Authorization
        {
            get { return this["firebase_authorization"]; }
        }

        public string ProjectId
        {
            get { return this["firebase_projectId"]; }
        }

        public string ApiKey
        {
            get { return this["firebase_apiKey"]; }
        }

        public string MessagingSenderId
        {
            get { return this["firebase_messagingSenderId"]; }
        }

        public FireBase()
        {
        }

        public FireBase(
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
        {
        }
    }
}