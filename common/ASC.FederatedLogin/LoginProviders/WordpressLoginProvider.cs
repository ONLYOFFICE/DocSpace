namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class WordpressLoginProvider : BaseLoginProvider<WordpressLoginProvider>
{
    public const string WordpressMeInfoUrl = "https://public-api.wordpress.com/rest/v1/me";
    public const string WordpressSites = "https://public-api.wordpress.com/rest/v1.2/sites/";

    public override string CodeUrl => "https://public-api.wordpress.com/oauth2/authorize";
    public override string AccessTokenUrl => "https://public-api.wordpress.com/oauth2/token";
    public override string RedirectUri => this["wpRedirectUrl"];
    public override string ClientID => this["wpClientId"];
    public override string ClientSecret => this["wpClientSecret"];

    public WordpressLoginProvider() { }

    public WordpressLoginProvider(
        OAuth20TokenHelper oAuth20TokenHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        Signature signature,
        InstanceCrypto instanceCrypto,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional)
    {
    }

    public static string GetWordpressMeInfo(string token)
    {
        var headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer " + token }
                };

        return RequestHelper.PerformRequest(WordpressMeInfoUrl, "", "GET", "", headers);
    }

    public static bool CreateWordpressPost(string title, string content, string status, string blogId, OAuth20Token token)
    {
        try
        {
            var uri = WordpressSites + blogId + "/posts/new";
            const string contentType = "application/x-www-form-urlencoded";
            const string method = "POST";
            var body = "title=" + HttpUtility.UrlEncode(title) + "&content=" + HttpUtility.UrlEncode(content) + "&status=" + status + "&format=standard";
            var headers = new Dictionary<string, string>
                    {
                        { "Authorization", "bearer " + token.AccessToken }
                    };

            RequestHelper.PerformRequest(uri, contentType, method, body, headers);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        throw new NotImplementedException();
    }
}
