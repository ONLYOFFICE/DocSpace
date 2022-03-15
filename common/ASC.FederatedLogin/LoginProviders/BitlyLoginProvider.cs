namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class BitlyLoginProvider : Consumer, IValidateKeysProvider
{
    private string BitlyClientId => this["bitlyClientId"];
    private string BitlyClientSecret => this["bitlyClientSecret"];
    private string BitlyUrl => this["bitlyUrl"];

    public BitlyLoginProvider() { }

    public BitlyLoginProvider(
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

    public bool ValidateKeys()
    {
        try
        {
            return !string.IsNullOrEmpty(GetShortenLink("https://www.onlyoffice.com"));
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Enabled
    {
        get
        {
            return !string.IsNullOrEmpty(BitlyClientId) &&
                   !string.IsNullOrEmpty(BitlyClientSecret) &&
                   !string.IsNullOrEmpty(BitlyUrl);
        }
    }

    public string GetShortenLink(string shareLink)
    {
        var uri = new Uri(shareLink);

        var bitly = string.Format(BitlyUrl, BitlyClientId, BitlyClientSecret, Uri.EscapeDataString(uri.ToString()));
        XDocument response;
        try
        {
            response = XDocument.Load(bitly);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message, e);
        }

        var status = response.XPathSelectElement("/response/status_code").Value;
        if (status != ((int)HttpStatusCode.OK).ToString(CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException(status);
        }

        var data = response.XPathSelectElement("/response/data/url");

        return data.Value;
    }
}
