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

namespace ASC.TelegramService;

[Singletone]
public class TelegramListenerService : BackgroundService
{
    private readonly ICacheNotify<NotifyMessage> _cacheMessage;
    private readonly ICacheNotify<RegisterUserProto> _cacheRegisterUser;
    private readonly ICacheNotify<CreateClientProto> _cacheCreateClient;
    private readonly ICacheNotify<DisableClientProto> _cacheDisableClient;
    private readonly TelegramHandler _telegramHandler;
    private readonly TenantManager _tenantManager;
    private readonly TelegramLoginProvider _telegramLoginProvider;
    private CancellationToken _stoppingToken;

    public TelegramListenerService(ICacheNotify<NotifyMessage> cacheMessage,
        ICacheNotify<RegisterUserProto> cacheRegisterUser,
        ICacheNotify<CreateClientProto> cacheCreateClient,
        TelegramHandler telegramHandler,
        ICacheNotify<DisableClientProto> cacheDisableClient,
        TenantManager tenantManager,
        ConsumerFactory consumerFactory)
    {
        _cacheMessage = cacheMessage;
        _cacheRegisterUser = cacheRegisterUser;
        _cacheCreateClient = cacheCreateClient;
        _cacheDisableClient = cacheDisableClient;
        _telegramLoginProvider = consumerFactory.Get<TelegramLoginProvider>();
        _tenantManager = tenantManager;
        _telegramHandler = telegramHandler;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        CreateClients();
        _cacheMessage.Subscribe(async n => await SendMessage(n), CacheNotifyAction.Insert);
        _cacheRegisterUser.Subscribe(n => RegisterUser(n), CacheNotifyAction.Insert);
        _cacheCreateClient.Subscribe(n => CreateOrUpdateClient(n), CacheNotifyAction.Insert);
        _cacheDisableClient.Subscribe(n => DisableClient(n), CacheNotifyAction.Insert);

        stoppingToken.Register(() =>
        {
            _cacheMessage.Unsubscribe(CacheNotifyAction.Insert);
            _cacheRegisterUser.Unsubscribe(CacheNotifyAction.Insert);
            _cacheCreateClient.Unsubscribe(CacheNotifyAction.Insert);
            _cacheDisableClient.Unsubscribe(CacheNotifyAction.Insert);
        });

        return Task.CompletedTask;
    }

    private void DisableClient(DisableClientProto n)
    {
        _telegramHandler.DisableClient(n.TenantId);
    }

    private async Task SendMessage(NotifyMessage notifyMessage)
    {
        await _telegramHandler.SendMessage(notifyMessage);
    }

    private void RegisterUser(RegisterUserProto registerUserProto)
    {
        _telegramHandler.RegisterUser(registerUserProto.UserId, registerUserProto.TenantId, registerUserProto.Token);
    }

    private void CreateOrUpdateClient(CreateClientProto createClientProto)
    {
        _telegramHandler.CreateOrUpdateClientForTenant(createClientProto.TenantId, createClientProto.Token, createClientProto.TokenLifespan, createClientProto.Proxy, false, _stoppingToken);
    }

    private void CreateClients()
    {
        var tenants = _tenantManager.GetTenants();
        foreach (var tenant in tenants)
        {
            _tenantManager.SetCurrentTenant(tenant);
            if (_telegramLoginProvider.IsEnabled())
            {
                _telegramHandler.CreateOrUpdateClientForTenant(tenant.Id, _telegramLoginProvider.TelegramBotToken, _telegramLoginProvider.TelegramAuthTokenLifespan, _telegramLoginProvider.TelegramProxy, true, _stoppingToken, true);
            }
        }
    }
}
