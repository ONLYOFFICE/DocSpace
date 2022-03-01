/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

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
