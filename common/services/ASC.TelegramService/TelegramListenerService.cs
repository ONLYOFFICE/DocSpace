// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.TelegramService;

[Singletone]
public class TelegramListenerService : BackgroundService
{
    private readonly ILogger<TelegramHandler> _logger;
    private readonly ICacheNotify<RegisterUserProto> _cacheRegisterUser;
    private readonly ICacheNotify<CreateClientProto> _cacheCreateClient;
    private readonly ICacheNotify<DisableClientProto> _cacheDisableClient;
    private readonly TelegramHandler _telegramHandler;
    private readonly TenantManager _tenantManager;
    private readonly TelegramLoginProvider _telegramLoginProvider;
    private CancellationToken _stoppingToken;

    public TelegramListenerService(
        ICacheNotify<RegisterUserProto> cacheRegisterUser,
        ICacheNotify<CreateClientProto> cacheCreateClient,
        ILogger<TelegramHandler> logger,
        TelegramHandler telegramHandler,
        ICacheNotify<DisableClientProto> cacheDisableClient,
        TenantManager tenantManager,
        ConsumerFactory consumerFactory)
    {
        _cacheRegisterUser = cacheRegisterUser;
        _cacheCreateClient = cacheCreateClient;
        _cacheDisableClient = cacheDisableClient;
        _telegramLoginProvider = consumerFactory.Get<TelegramLoginProvider>();
        _tenantManager = tenantManager;
        _telegramHandler = telegramHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        await CreateClientsAsync();

        _cacheRegisterUser.Subscribe(n => RegisterUser(n), CacheNotifyAction.Insert);
        _cacheCreateClient.Subscribe(n => CreateOrUpdateClient(n), CacheNotifyAction.Insert);
        _cacheDisableClient.Subscribe(n => DisableClient(n), CacheNotifyAction.Insert);

        stoppingToken.Register(() =>
        {
            _logger.DebugTelegramStopping();

            _cacheRegisterUser.Unsubscribe(CacheNotifyAction.Insert);
            _cacheCreateClient.Unsubscribe(CacheNotifyAction.Insert);
            _cacheDisableClient.Unsubscribe(CacheNotifyAction.Insert);
        });
    }

    private void DisableClient(DisableClientProto n)
    {
        _telegramHandler.DisableClient(n.TenantId);
    }

    private void RegisterUser(RegisterUserProto registerUserProto)
    {
        _telegramHandler.RegisterUser(registerUserProto.UserId, registerUserProto.TenantId, registerUserProto.Token);
    }

    private void CreateOrUpdateClient(CreateClientProto createClientProto)
    {
        _telegramHandler.CreateOrUpdateClientForTenant(createClientProto.TenantId, createClientProto.Token, createClientProto.TokenLifespan, createClientProto.Proxy, false, _stoppingToken);
    }

    private async Task CreateClientsAsync()
    {
        var tenants = await _tenantManager.GetTenantsAsync();

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
