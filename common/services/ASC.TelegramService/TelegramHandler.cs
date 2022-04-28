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

[Singletone(Additional = typeof(TelegramHandlerExtension))]
public class TelegramHandler
{
    private readonly Dictionary<int, TenantTgClient> _clients;
    private readonly CommandModule _command;
    private readonly ILogger _log;
    private readonly IServiceScopeFactory _scopeFactory;

    public TelegramHandler(CommandModule command, ILogger<TelegramHandler> logger, IServiceScopeFactory scopeFactory)
    {
        _command = command;
        _log = logger;
        _scopeFactory = scopeFactory;
        _clients = new Dictionary<int, TenantTgClient>();
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
    }

    public Task SendMessage(NotifyMessage msg)
    {
        if (string.IsNullOrEmpty(msg.Reciever))
        {
            return Task.CompletedTask;
        }

        if (!_clients.ContainsKey(msg.TenantId))
        {
            return Task.CompletedTask;
        }

        return InternalSendMessage(msg);
    }

    private async Task InternalSendMessage(NotifyMessage msg)
    {
        var scope = _scopeFactory.CreateScope();
        var cachedTelegramDao = scope.ServiceProvider.GetService<IOptionsSnapshot<CachedTelegramDao>>().Value;

        var client = _clients[msg.TenantId].Client;

        try
        {
            var tgUser = cachedTelegramDao.GetUser(Guid.Parse(msg.Reciever), msg.TenantId);

            if (tgUser == null)
            {
                _log.LogDebug("Couldn't find telegramId for user '{reciever}'", msg.Reciever);
                return;
            }

            var chat = await client.GetChatAsync(tgUser.TelegramUserId);
            await client.SendTextMessageAsync(chat, msg.Content, Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
        catch (Exception e)
        {
            _log.LogDebug("Couldn't send message for user '{reciever}' got an '{message}'", msg.Reciever, e.Message);
        }
    }

    public void DisableClient(int tenantId)
    {
        if (!_clients.ContainsKey(tenantId))
        {
            return;
        }

        var client = _clients[tenantId];
        client.Client.StopReceiving();

        _clients.Remove(tenantId);
    }

    public void CreateOrUpdateClientForTenant(int tenantId, string token, int tokenLifespan, string proxy, bool startTelegramService, CancellationToken stoppingToken, bool force = false)
    {
        var scope = _scopeFactory.CreateScope();
        var telegramHelper = scope.ServiceProvider.GetService<TelegramHelper>();
        var newClient = telegramHelper.InitClient(token, proxy);

        if (_clients.TryGetValue(tenantId, out var client))
        {
            client.TokenLifeSpan = tokenLifespan;

            if (token != client.Token || proxy != client.Proxy)
            {
                if (startTelegramService)
                {
                    if (!telegramHelper.TestingClient(newClient))
                    {
                        return;
                    }
                }

                client.Client.StopReceiving();

                BindClient(newClient, tenantId, stoppingToken);

                client.Client = newClient;
                client.Token = token;
                client.Proxy = proxy;
            }
        }
        else
        {
            if (!force && startTelegramService)
            {
                if (!telegramHelper.TestingClient(newClient))
                {
                    return;
                }
            }

            BindClient(newClient, tenantId, stoppingToken);

            _clients.Add(tenantId, new TenantTgClient()
            {
                Token = token,
                Client = newClient,
                Proxy = proxy,
                TenantId = tenantId,
                TokenLifeSpan = tokenLifespan
            });
        }
    }

    public void RegisterUser(string userId, int tenantId, string token)
    {
        if (!_clients.ContainsKey(tenantId))
        {
            return;
        }

        var userKey = UserKey(userId, tenantId);
        var dateExpires = DateTimeOffset.Now.AddMinutes(_clients[tenantId].TokenLifeSpan);
        MemoryCache.Default.Set(token, userKey, dateExpires);
    }

    private Task OnMessage(object sender, MessageEventArgs e, TelegramBotClient client, int tenantId)
    {
        if (string.IsNullOrEmpty(e.Message.Text) || e.Message.Text[0] != '/')
        {
            return Task.CompletedTask;
        }

        return InternalOnMessage(sender, e, client, tenantId);
    }

    private async Task InternalOnMessage(object sender, MessageEventArgs e, TelegramBotClient client, int tenantId)
    {
        await _command.HandleCommand(e.Message, client, tenantId);
    }


    private void BindClient(TelegramBotClient client, int tenantId, CancellationToken stoppingToken)
    {
        client.OnMessage += async (sender, e) => { await OnMessage(sender, e, client, tenantId); };
        client.StartReceiving(cancellationToken: stoppingToken);
    }

    private string UserKey(string userId, int tenantId)
    {
        return string.Format("{0}:{1}", userId, tenantId);
    }
}

public static class TelegramHandlerExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<TelegramHelper>();
    }
}
