namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class OneDriveLoginProvider : Consumer, IOAuthProvider
{
    private const string OneDriveOauthUrl = "https://login.live.com/";
    public const string OneDriveApiUrl = "https://api.onedrive.com";

    public static string OneDriveLoginProviderScopes => "wl.signin wl.skydrive_update wl.offline_access";
    public string Scopes => OneDriveLoginProviderScopes;
    public string CodeUrl => OneDriveOauthUrl + "oauth20_authorize.srf";
    public string AccessTokenUrl => OneDriveOauthUrl + "oauth20_token.srf";
    public string RedirectUri => this["skydriveRedirectUrl"];
    public string ClientID => this["skydriveappkey"];
    public string ClientSecret => this["skydriveappsecret"];

    public bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }

    public OneDriveLoginProvider() { }

    public OneDriveLoginProvider(
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
