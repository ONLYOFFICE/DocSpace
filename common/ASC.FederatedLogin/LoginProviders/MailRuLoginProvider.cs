using System.Security.Cryptography;

namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class MailRuLoginProvider : BaseLoginProvider<MailRuLoginProvider>
{
    public override string CodeUrl => "https://connect.mail.ru/oauth/authorize";
    public override string AccessTokenUrl => "https://connect.mail.ru/oauth/token";
    public override string ClientID => this["mailRuClientId"];
    public override string ClientSecret => this["mailRuClientSecret"];
    public override string RedirectUri => this["mailRuRedirectUrl"];

    private const string MailRuApiUrl = "http://www.appsmail.ru/platform/api";

    public MailRuLoginProvider() { }

    public MailRuLoginProvider(
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
            var token = Auth(context, Scopes, out var redirect);

            if (redirect)
            {
                return null;
            }

            if (token == null)
            {
                throw new Exception("Login failed");
            }

            var uid = GetUid(token);

            return RequestProfile(token.AccessToken, uid);
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
        throw new NotImplementedException();
    }

    private LoginProfile RequestProfile(string accessToken, string uid)
    {
        var queryDictionary = new Dictionary<string, string>
                {
                    { "app_id", ClientID },
                    { "method", "users.getInfo" },
                    { "secure", "1" },
                    { "session_key", accessToken },
                    { "uids", uid },
                };

        var sortedKeys = queryDictionary.Keys.ToList();
        sortedKeys.Sort();

        using var md5 = MD5.Create();
        var mailruParams = string.Join("", sortedKeys.Select(key => key + "=" + queryDictionary[key]).ToList());
        var sig = string.Join("", md5.ComputeHash(Encoding.ASCII.GetBytes(mailruParams + ClientSecret)).Select(b => b.ToString("x2")));

        var mailRuProfile = RequestHelper.PerformRequest(
            MailRuApiUrl
            + "?" + string.Join("&", queryDictionary.Select(pair => pair.Key + "=" + HttpUtility.UrlEncode(pair.Value)))
            + "&sig=" + HttpUtility.UrlEncode(sig));
        var loginProfile = ProfileFromMailRu(mailRuProfile);

        return loginProfile;
    }

    private LoginProfile ProfileFromMailRu(string strProfile)
    {
        var jProfile = JArray.Parse(strProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var mailRuProfiles = jProfile.ToObject<List<MailRuProfile>>();
        if (mailRuProfiles.Count == 0)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            EMail = mailRuProfiles[0].Email,
            Id = mailRuProfiles[0].Uid,
            FirstName = mailRuProfiles[0].FirstName,
            LastName = mailRuProfiles[0].LastName,

            Provider = ProviderConstants.MailRu,
        };

        return profile;
    }

    private class MailRuProfile
    {
        public string Uid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    private static string GetUid(OAuth20Token token)
    {
        if (string.IsNullOrEmpty(token.OriginJson))
        {
            return null;
        }

        var parser = JObject.Parse(token.OriginJson);

        return parser?.Value<string>("x_mailru_vid");
    }
}
