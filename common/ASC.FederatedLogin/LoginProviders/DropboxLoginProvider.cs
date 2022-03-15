namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class DropboxLoginProvider : Consumer, IOAuthProvider
{
    public string Scopes => string.Empty;
    public string CodeUrl => "https://www.dropbox.com/oauth2/authorize";
    public string AccessTokenUrl => "https://api.dropboxapi.com/oauth2/token";
    public string RedirectUri => this["dropboxRedirectUrl"];
    public string ClientID => this["dropboxClientId"];
    public string ClientSecret => this["dropboxClientSecret"];
    public bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }

    public DropboxLoginProvider() { }

    public DropboxLoginProvider(
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
