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


using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Notify;
using ASC.Core.Common.Notify.Telegram;
using ASC.Notify.Messages;
using ASC.TelegramService.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Args;

namespace ASC.TelegramService
{
    [Singletone(Additional = typeof(TelegramHandlerExtension))]
    public class TelegramHandler
    {
        private Dictionary<int, TenantTgClient> Clients { get; set; }
        private CommandModule Command { get; set; }
        private ILog Log { get; set; }
        private IServiceProvider ServiceProvider { get; set; }

        public TelegramHandler(CommandModule command, IOptionsMonitor<ILog> option, IServiceProvider serviceProvider)
        {
            Command = command;
            Log = option.CurrentValue;
            ServiceProvider = serviceProvider;
            Clients = new Dictionary<int, TenantTgClient>();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        }

        public Task SendMessage(NotifyMessage msg)
        {
            if (string.IsNullOrEmpty(msg.To)) return Task.CompletedTask;
            if (!Clients.ContainsKey(msg.Tenant)) return Task.CompletedTask;

            return InternalSendMessage(msg);
        }

        private async Task InternalSendMessage(NotifyMessage msg)
        {
            var scope = ServiceProvider.CreateScope();
            var cachedTelegramDao = scope.ServiceProvider.GetService<IOptionsSnapshot<CachedTelegramDao>>().Value;

            var client = Clients[msg.Tenant].Client;

            try
            {
                var tgUser = cachedTelegramDao.GetUser(Guid.Parse(msg.To), msg.Tenant);

                if (tgUser == null)
                {
                    Log.DebugFormat("Couldn't find telegramId for user '{0}'", msg.To);
                    return;
                }

                var chat = await client.GetChatAsync(tgUser.TelegramUserId);
                await client.SendTextMessageAsync(chat, msg.Content, Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception e)
            {
                Log.DebugFormat("Couldn't send message for user '{0}' got an '{1}'", msg.To, e.Message);
            }
        }

        public void DisableClient(int tenantId)
        {
            if (!Clients.ContainsKey(tenantId)) return;

            var client = Clients[tenantId];
            client.Client.StopReceiving();

            Clients.Remove(tenantId);
        }

        public void CreateOrUpdateClientForTenant(int tenantId, string token, int tokenLifespan, string proxy, bool startTelegramService, bool force = false)
        {
            var scope = ServiceProvider.CreateScope();
            var telegramHelper = scope.ServiceProvider.GetService<TelegramHelper>();
            var newClient = telegramHelper.InitClient(token, proxy);

            if (Clients.TryGetValue(tenantId, out var client))
            {
                client.TokenLifeSpan = tokenLifespan;

                if (token != client.Token || proxy != client.Proxy)
                {
                    if (startTelegramService)
                    {
                        if (!telegramHelper.TestingClient(newClient)) return;
                    }

                    client.Client.StopReceiving();

                    BindClient(newClient, tenantId);

                    client.Client = newClient;
                    client.Token = token;
                    client.Proxy = proxy;
                }
            }
            else
            {
                if (!force && startTelegramService)
                {
                    if (!telegramHelper.TestingClient(newClient)) return;
                }

                BindClient(newClient, tenantId);

                Clients.Add(tenantId, new TenantTgClient()
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
            if (!Clients.ContainsKey(tenantId)) return;

            var userKey = UserKey(userId, tenantId);
            var dateExpires = DateTimeOffset.Now.AddMinutes(Clients[tenantId].TokenLifeSpan);
            MemoryCache.Default.Set(token, userKey, dateExpires);
        }

        private Task OnMessage(object sender, MessageEventArgs e, TelegramBotClient client, int tenantId)
        {
            if (string.IsNullOrEmpty(e.Message.Text) || e.Message.Text[0] != '/') return Task.CompletedTask;
            return InternalOnMessage(sender, e, client, tenantId);
        }

        private async Task InternalOnMessage(object sender, MessageEventArgs e, TelegramBotClient client, int tenantId)
        {
            await Command.HandleCommand(e.Message, client, tenantId);
        }


        private void BindClient(TelegramBotClient client, int tenantId)
        {
            client.OnMessage += async (sender, e) => { await OnMessage(sender, e, client, tenantId); };
            client.StartReceiving();
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
}
