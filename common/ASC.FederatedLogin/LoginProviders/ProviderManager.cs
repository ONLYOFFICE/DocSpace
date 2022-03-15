namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class ProviderManager
{
    public bool IsNotEmpty
    {
        get
        {
            return AuthProviders
                .Select(GetLoginProvider)
                .Any(loginProvider => loginProvider != null && loginProvider.IsEnabled);
        }
    }

    public static readonly List<string> AuthProviders = new List<string>
        {
            ProviderConstants.Google,
            ProviderConstants.Facebook,
            ProviderConstants.Twitter,
            ProviderConstants.LinkedIn,
            ProviderConstants.MailRu,
            ProviderConstants.VK,
            ProviderConstants.Yandex,
            ProviderConstants.GosUslugi
        };

    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly ConsumerFactory _consumerFactory;

    public ProviderManager(Signature signature, InstanceCrypto instanceCrypto, ConsumerFactory consumerFactory)
    {
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _consumerFactory = consumerFactory;
    }

    public ILoginProvider GetLoginProvider(string providerType)
    {
        return providerType == ProviderConstants.OpenId
            ? new OpenIdLoginProvider(_signature, _instanceCrypto, _consumerFactory)
            : _consumerFactory.GetByKey(providerType) as ILoginProvider;
    }

    public LoginProfile Process(string providerType, HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs = null)
    {
        return GetLoginProvider(providerType).ProcessAuthoriztion(context, @params, additionalStateArgs);
    }

    public LoginProfile GetLoginProfile(string providerType, string accessToken)
    {
        var consumer = GetLoginProvider(providerType);
        if (consumer == null)
        {
            throw new ArgumentException("Unknown provider type", nameof(providerType));
        }

        try
        {
            return consumer.GetLoginProfile(accessToken);
        }
        catch (Exception ex)
        {
            return LoginProfile.FromError(_signature, _instanceCrypto, ex);
        }
    }
}
