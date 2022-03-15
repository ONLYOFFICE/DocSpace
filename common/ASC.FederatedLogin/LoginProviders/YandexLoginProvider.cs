namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class YandexLoginProvider : BaseLoginProvider<YandexLoginProvider>
{
    public override string CodeUrl => "https://oauth.yandex.ru/authorize";
    public override string AccessTokenUrl => "https://oauth.yandex.ru/token";
    public override string ClientID => this["yandexClientId"];
    public override string ClientSecret => this["yandexClientSecret"];
    public override string RedirectUri => this["yandexRedirectUrl"];

    private const string YandexProfileUrl = "https://login.yandex.ru/info";


    public YandexLoginProvider() { }

    public YandexLoginProvider(
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

    public override LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        try
        {
            var token = Auth(context, Scopes, out var redirect, context.Request.Query["access_type"] == "offline"
                ? new Dictionary<string, string>
                {
                        { "force_confirm", "true" }
                }
                : null, additionalStateArgs);

            if (redirect)
            {
                return null;
            }

            return GetLoginProfile(token?.AccessToken);
        }
        catch (ThreadAbortException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return LoginProfile.FromError(Signature, InstanceCrypto, ex);
        }
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Login failed");
        }

        return RequestProfile(accessToken);
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var yandexProfile = RequestHelper.PerformRequest(YandexProfileUrl + "?format=json&oauth_token=" + accessToken);
        var loginProfile = ProfileFromYandex(yandexProfile);

        return loginProfile;
    }

    private LoginProfile ProfileFromYandex(string strProfile)
    {
        var jProfile = JObject.Parse(strProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            EMail = jProfile.Value<string>("default_email"),
            Id = jProfile.Value<string>("id"),
            FirstName = jProfile.Value<string>("first_name"),
            LastName = jProfile.Value<string>("last_name"),
            DisplayName = jProfile.Value<string>("display_name"),
            Gender = jProfile.Value<string>("sex"),

            Provider = ProviderConstants.Yandex,
        };

        return profile;
    }
}
