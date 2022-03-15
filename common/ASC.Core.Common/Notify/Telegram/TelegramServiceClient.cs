namespace ASC.Core.Common.Notify;

[Singletone]
public class TelegramServiceClient : ITelegramService
{
    private readonly ICacheNotify<NotifyMessage> _cacheMessage;
    private readonly ICacheNotify<RegisterUserProto> _cacheRegisterUser;
    private readonly ICacheNotify<CreateClientProto> _cacheCreateClient;
    private readonly ICacheNotify<DisableClientProto> _cacheDisableClient;
    private readonly ICache _cache;

    public TelegramServiceClient(ICacheNotify<NotifyMessage> cacheMessage,
        ICacheNotify<RegisterUserProto> cacheRegisterUser,
        ICacheNotify<CreateClientProto> cacheCreateClient,
        ICacheNotify<DisableClientProto> cacheDisableClient,
        ICache cache)
    {
        _cacheMessage = cacheMessage;
        _cacheRegisterUser = cacheRegisterUser;
        _cacheCreateClient = cacheCreateClient;
        _cacheDisableClient = cacheDisableClient;
        _cache = cache;
    }

    public void SendMessage(NotifyMessage m)
    {
        _cacheMessage.Publish(m, ASC.Common.Caching.CacheNotifyAction.Insert);
    }

    public void RegisterUser(string userId, int tenantId, string token)
    {
        _cache.Insert(GetCacheTokenKey(tenantId, userId), token, DateTime.MaxValue);
        _cacheRegisterUser.Publish(new RegisterUserProto()
        {
            UserId = userId,
            TenantId = tenantId,
            Token = token
        }, ASC.Common.Caching.CacheNotifyAction.Insert);
    }

    public void CreateOrUpdateClient(int tenantId, string token, int tokenLifespan, string proxy)
    {
        _cacheCreateClient.Publish(new CreateClientProto()
        {
            TenantId = tenantId,
            Token = token,
            TokenLifespan = tokenLifespan,
            Proxy = proxy
        }, ASC.Common.Caching.CacheNotifyAction.Insert);
    }

    public void DisableClient(int tenantId)
    {
        _cacheDisableClient.Publish(new DisableClientProto() { TenantId = tenantId }, ASC.Common.Caching.CacheNotifyAction.Insert);
    }

    public string RegistrationToken(string userId, int tenantId)
    {
        return _cache.Get<string>(GetCacheTokenKey(tenantId, userId));
    }

    private string GetCacheTokenKey(int tenantId, string userId)
    {
        return "Token" + userId + tenantId;
    }
}
