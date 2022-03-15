namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class BoxLoginProvider : Consumer, IOAuthProvider
{
    public string Scopes => string.Empty;
    public string CodeUrl => "https://app.box.com/api/oauth2/authorize";
    public string AccessTokenUrl => "https://app.box.com/api/oauth2/token";
    public string RedirectUri => this["boxRedirectUrl"];
    public string ClientID => this["boxClientId"];
    public string ClientSecret => this["boxClientSecret"];
    public bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }

    public BoxLoginProvider() { }

    public BoxLoginProvider(
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
