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

using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core.Common.Notify;
using ASC.Notify.Messages;

namespace ASC.TelegramService
{
    [Singletone]
    public class TelegramListener
    {
        private IEventBus<NotifyMessage> CacheMessage { get; }
        private IEventBus<RegisterUserProto> CacheRegisterUser { get; }
        private IEventBus<CreateClientProto> CacheCreateClient { get; }
        private IEventBus<DisableClientProto> CacheDisableClient { get; }

        private TelegramHandler TelegramHandler { get; set; }

        public TelegramListener(IEventBus<NotifyMessage> cacheMessage,
            IEventBus<RegisterUserProto> cacheRegisterUser,
            IEventBus<CreateClientProto> cacheCreateClient,
            TelegramHandler telegramHandler,
            IEventBus<DisableClientProto> cacheDisableClient)
        {
            CacheMessage = cacheMessage;
            CacheRegisterUser = cacheRegisterUser;
            CacheCreateClient = cacheCreateClient;
            CacheDisableClient = cacheDisableClient;

            TelegramHandler = telegramHandler;
        }

        public void Start()
        {
            CacheMessage.Subscribe(async n => await SendMessage(n), EventType.Insert);
            CacheRegisterUser.Subscribe(n => RegisterUser(n), EventType.Insert);
            CacheCreateClient.Subscribe(n => CreateOrUpdateClient(n), EventType.Insert);
            CacheDisableClient.Subscribe(n => DisableClient(n), EventType.Insert);

        }

        public void Stop()
        {
            CacheMessage.Unsubscribe(EventType.Insert);
            CacheRegisterUser.Unsubscribe(EventType.Insert);
            CacheCreateClient.Unsubscribe(EventType.Insert);
            CacheDisableClient.Unsubscribe(EventType.Insert);
        }

        private void DisableClient(DisableClientProto n)
        {
            TelegramHandler.DisableClient(n.TenantId);
        }

        private async Task SendMessage(NotifyMessage notifyMessage)
        {
            await TelegramHandler.SendMessage(notifyMessage);
        }

        private void RegisterUser(RegisterUserProto registerUserProto)
        {
            TelegramHandler.RegisterUser(registerUserProto.UserId, registerUserProto.TenantId, registerUserProto.Token);
        }

        private void CreateOrUpdateClient(CreateClientProto createClientProto)
        {
            TelegramHandler.CreateOrUpdateClientForTenant(createClientProto.TenantId, createClientProto.Token, createClientProto.TokenLifespan, createClientProto.Proxy, false);
        }
    }
}
