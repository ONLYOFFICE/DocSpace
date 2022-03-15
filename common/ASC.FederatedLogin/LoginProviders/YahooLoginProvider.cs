namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class YahooLoginProvider : BaseLoginProvider<YahooLoginProvider>
{
    public const string YahooUrlUserGuid = "https://social.yahooapis.com/v1/me/guid";
    public const string YahooUrlContactsFormat = "https://social.yahooapis.com/v1/user/{0}/contacts";

    public override string CodeUrl => "https://api.login.yahoo.com/oauth2/request_auth";
    public override string AccessTokenUrl => "https://api.login.yahoo.com/oauth2/get_token";
    public override string RedirectUri => this["yahooRedirectUrl"];
    public override string ClientID => this["yahooClientId"];
    public override string ClientSecret => this["yahooClientSecret"];
    public override string Scopes => "sdct-r";

    public YahooLoginProvider() { }

    public YahooLoginProvider(
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
        : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional) { }

    public OAuth20Token Auth(HttpContext context)
    {
        return Auth(context, Scopes, out var _);
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        throw new NotImplementedException();
    }
}
