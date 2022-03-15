namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class LinkedInLoginProvider : BaseLoginProvider<LinkedInLoginProvider>
{
    public override string AccessTokenUrl => "https://www.linkedin.com/oauth/v2/accessToken";
    public override string RedirectUri => this["linkedInRedirectUrl"];
    public override string ClientID => this["linkedInKey"];
    public override string ClientSecret => this["linkedInSecret"];
    public override string CodeUrl => "https://www.linkedin.com/oauth/v2/authorization";
    public override string Scopes => "r_liteprofile r_emailaddress";

    private const string LinkedInProfileUrl = "https://api.linkedin.com/v2/me";
    private const string LinkedInEmailUrl = "https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))";

    public LinkedInLoginProvider() { }

    public LinkedInLoginProvider(
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

    internal LoginProfile ProfileFromLinkedIn(string linkedInProfile)
    {
        var jProfile = JObject.Parse(linkedInProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            Id = jProfile.Value<string>("id"),
            FirstName = jProfile.Value<string>("localizedFirstName"),
            LastName = jProfile.Value<string>("localizedLastName"),
            EMail = jProfile.Value<string>("emailAddress"),
            Provider = ProviderConstants.LinkedIn,
        };

        return profile;
    }

    internal static string EmailFromLinkedIn(string linkedInEmail)
    {
        var jEmail = JObject.Parse(linkedInEmail);
        if (jEmail == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        return jEmail.SelectToken("elements[0].handle~.emailAddress").ToString();
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var linkedInProfile = RequestHelper.PerformRequest(LinkedInProfileUrl,
            headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
        var loginProfile = ProfileFromLinkedIn(linkedInProfile);

        var linkedInEmail = RequestHelper.PerformRequest(LinkedInEmailUrl, headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
        loginProfile.EMail = EmailFromLinkedIn(linkedInEmail);

        return loginProfile;
    }
}
