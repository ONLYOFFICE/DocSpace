namespace ASC.FederatedLogin.LoginProviders;

public enum LoginProviderEnum
{
    Facebook,
    Google,
    Dropbox,
    Docusign,
    Box,
    OneDrive,
    GosUslugi,
    LinkedIn,
    MailRu,
    VK,
    Wordpress,
    Yahoo,
    Yandex
}

public abstract class BaseLoginProvider<T> : Consumer, ILoginProvider where T : Consumer, ILoginProvider, new()
{
    public T Instance => ConsumerFactory.Get<T>();
    public virtual bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }

    public abstract string CodeUrl { get; }
    public abstract string AccessTokenUrl { get; }
    public abstract string RedirectUri { get; }
    public abstract string ClientID { get; }
    public abstract string ClientSecret { get; }
    public virtual string Scopes => string.Empty;

    internal readonly Signature Signature;
    internal readonly InstanceCrypto InstanceCrypto;

    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    protected BaseLoginProvider() { }

    protected BaseLoginProvider(
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
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
    {
        _oAuth20TokenHelper = oAuth20TokenHelper;
        Signature = signature;
        InstanceCrypto = instanceCrypto;
    }

    public virtual LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        try
        {
            var token = Auth(context, Scopes, out var redirect, @params, additionalStateArgs);

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

    public abstract LoginProfile GetLoginProfile(string accessToken);

    protected virtual OAuth20Token Auth(HttpContext context, string scopes, out bool redirect, IDictionary<string, string> additionalArgs = null, IDictionary<string, string> additionalStateArgs = null)
    {
        var error = context.Request.Query["error"];
        if (!string.IsNullOrEmpty(error))
        {
            if (error == "access_denied")
            {
                error = "Canceled at provider";
            }

            throw new Exception(error);
        }

        var code = context.Request.Query["code"];
        if (string.IsNullOrEmpty(code))
        {
            context.Response.Redirect(_oAuth20TokenHelper.RequestCode<T>(scopes, additionalArgs, additionalStateArgs));
            redirect = true;

            return null;
        }

        redirect = false;

        return OAuth20TokenHelper.GetAccessToken<T>(ConsumerFactory, code);
    }
}

public static class BaseLoginProviderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BoxLoginProvider>();
        services.TryAdd<DropboxLoginProvider>();
        services.TryAdd<OneDriveLoginProvider>();
        services.TryAdd<DocuSignLoginProvider>();
        services.TryAdd<GoogleLoginProvider>();
        services.TryAdd<WordpressLoginProvider>();
    }
}
