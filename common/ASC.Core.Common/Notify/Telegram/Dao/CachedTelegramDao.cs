namespace ASC.Core.Common.Notify.Telegram;

class ConfigureCachedTelegramDao : IConfigureNamedOptions<CachedTelegramDao>
{
    private readonly IOptionsSnapshot<TelegramDao> _service;
    private readonly ICache _cache;

    public ConfigureCachedTelegramDao(IOptionsSnapshot<TelegramDao> service, ICache cache)
    {
        _service = service;
        _cache = cache;
    }

    public void Configure(string name, CachedTelegramDao options)
    {
        Configure(options);
        options.TgDao = _service.Get(name);
    }

    public void Configure(CachedTelegramDao options)
    {
        options.TgDao = _service.Value;
        options.Cache = _cache;
        options.Expiration = TimeSpan.FromMinutes(20);

        options.PairKeyFormat = "tgUser:{0}:{1}";
        options.SingleKeyFormat = "tgUser:{0}";
    }
}

[Scope(typeof(ConfigureCachedTelegramDao))]
public class CachedTelegramDao
{
    public TelegramDao TgDao { get; set; }
    public ICache Cache { get; set; }
    public TimeSpan Expiration { get; set; }
    public string PairKeyFormat { get; set; }
    public string SingleKeyFormat { get; set; }


    public void Delete(Guid userId, int tenantId)
    {
        Cache.Remove(string.Format(PairKeyFormat, userId, tenantId));
        TgDao.Delete(userId, tenantId);
    }

    public void Delete(int telegramId)
    {
        Cache.Remove(string.Format(SingleKeyFormat, telegramId));
        TgDao.Delete(telegramId);
    }

    public TelegramUser GetUser(Guid userId, int tenantId)
    {
        var key = string.Format(PairKeyFormat, userId, tenantId);

        var user = Cache.Get<TelegramUser>(key);
        if (user != null)
        {
            return user;
        }

        user = TgDao.GetUser(userId, tenantId);
        if (user != null)
        {
            Cache.Insert(key, user, Expiration);
        }

        return user;
    }

    public List<TelegramUser> GetUser(int telegramId)
    {
        var key = string.Format(SingleKeyFormat, telegramId);

        var users = Cache.Get<List<TelegramUser>>(key);
        if (users != null)
        {
            return users;
        }

        users = TgDao.GetUser(telegramId);
        if (users.Count > 0)
        {
            Cache.Insert(key, users, Expiration);
        }

        return users;
    }

    public void RegisterUser(Guid userId, int tenantId, int telegramId)
    {
        TgDao.RegisterUser(userId, tenantId, telegramId);

        var key = string.Format(PairKeyFormat, userId, tenantId);
        Cache.Insert(key, new TelegramUser { PortalUserId = userId, TenantId = tenantId, TelegramUserId = telegramId }, Expiration);
    }
}
