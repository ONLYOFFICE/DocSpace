namespace ASC.FederatedLogin.LoginProviders;

public class TelegramLoginProvider : Consumer, IValidateKeysProvider, ITelegramLoginProvider
{
    public string TelegramBotToken => this["telegramBotToken"];
    public string TelegramBotName => this["telegramBotName"];
    public int TelegramAuthTokenLifespan => int.Parse(this["telegramAuthTokenLifespan"]);
    public string TelegramProxy => this["telegramProxy"];

    private readonly TelegramHelper _telegramHelper;

    public TelegramLoginProvider() { }

    public TelegramLoginProvider(
        TelegramHelper telegramHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
    {
        _telegramHelper = telegramHelper;
    }

    public bool IsEnabled()
    {
        return !string.IsNullOrEmpty(TelegramBotToken) && !string.IsNullOrEmpty(TelegramBotName);
    }

    public bool ValidateKeys()
    {
        if (TelegramBotToken.Length == 0)
        {
            _telegramHelper.DisableClient(TenantManager.GetCurrentTenant().Id);

            return true;
        }
        else
        {
            return _telegramHelper.CreateClient(TenantManager.GetCurrentTenant().Id, TelegramBotToken, TelegramAuthTokenLifespan, TelegramProxy);
        }
    }
}
