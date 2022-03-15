namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class FacebookLoginProvider : BaseLoginProvider<FacebookLoginProvider>
{
    public override string AccessTokenUrl => "https://graph.facebook.com/v2.7/oauth/access_token";
    public override string RedirectUri => this["facebookRedirectUrl"];
    public override string ClientID => this["facebookClientId"];
    public override string ClientSecret => this["facebookClientSecret"];
    public override string CodeUrl => "https://www.facebook.com/v2.7/dialog/oauth/";
    public override string Scopes => "email,public_profile";

    private const string FacebookProfileUrl = "https://graph.facebook.com/v2.7/me?fields=email,id,birthday,link,first_name,last_name,gender,timezone,locale";

    public FacebookLoginProvider() { }

    public FacebookLoginProvider(
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

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Login failed");
        }

        return RequestProfile(accessToken);
    }

    internal LoginProfile ProfileFromFacebook(string facebookProfile)
    {
        var jProfile = JObject.Parse(facebookProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            BirthDay = jProfile.Value<string>("birthday"),
            Link = jProfile.Value<string>("link"),
            FirstName = jProfile.Value<string>("first_name"),
            LastName = jProfile.Value<string>("last_name"),
            Gender = jProfile.Value<string>("gender"),
            EMail = jProfile.Value<string>("email"),
            Id = jProfile.Value<string>("id"),
            TimeZone = jProfile.Value<string>("timezone"),
            Locale = jProfile.Value<string>("locale"),
            Provider = ProviderConstants.Facebook,
            Avatar = "http://graph.facebook.com/" + jProfile.Value<string>("id") + "/picture?type=large"
        };

        return profile;
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var facebookProfile = RequestHelper.PerformRequest(FacebookProfileUrl + "&access_token=" + accessToken);
        var loginProfile = ProfileFromFacebook(facebookProfile);

        return loginProfile;
    }
}
