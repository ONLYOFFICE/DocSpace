namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class DocuSignLoginProvider : Consumer, IOAuthProvider
{
    public static string DocuSignLoginProviderScopes => "signature";
    public string Scopes => DocuSignLoginProviderScopes;
    public string CodeUrl => DocuSignHost + "/oauth/auth";
    public string AccessTokenUrl => DocuSignHost + "/oauth/token";
    public string RedirectUri => this["docuSignRedirectUrl"];
    public string ClientID => this["docuSignClientId"];
    public string ClientSecret => this["docuSignClientSecret"];
    public string DocuSignHost => "https://" + this["docuSignHost"];
    public bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }
    private string AuthHeader
    {
        get
        {
            var codeAuth = $"{ClientID}:{ClientSecret}";
            var codeAuthBytes = Encoding.UTF8.GetBytes(codeAuth);
            var codeAuthBase64 = Convert.ToBase64String(codeAuthBytes);

            return "Basic " + codeAuthBase64;
        }
    }

    public DocuSignLoginProvider() { }

    public DocuSignLoginProvider(
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

    public OAuth20Token GetAccessToken(string authCode)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(authCode);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(ClientID);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(ClientSecret);

        var data = $"grant_type=authorization_code&code={authCode}";
        var headers = new Dictionary<string, string> { { "Authorization", AuthHeader } };

        var json = RequestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", data, headers);
        if (json == null)
        {
            throw new Exception("Can not get token");
        }

        if (!json.StartsWith('{'))
        {
            json = "{\"" + json.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
        }

        var token = OAuth20Token.FromJson(json);
        if (token == null)
        {
            return null;
        }

        token.ClientID = ClientID;
        token.ClientSecret = ClientSecret;
        token.RedirectUri = RedirectUri;

        return token;
    }

    public OAuth20Token RefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(ClientID) || string.IsNullOrEmpty(ClientSecret))
        {
            throw new ArgumentException("Can not refresh given token");
        }

        var data = $"grant_type=refresh_token&refresh_token={refreshToken}";
        var headers = new Dictionary<string, string> { { "Authorization", AuthHeader } };

        var json = RequestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", data, headers);
        if (json == null)
        {
            throw new Exception("Can not get token");
        }

        var refreshed = OAuth20Token.FromJson(json);
        refreshed.ClientID = ClientID;
        refreshed.ClientSecret = ClientSecret;
        refreshed.RedirectUri = RedirectUri;
        refreshed.RefreshToken ??= refreshToken;

        return refreshed;
    }
}
