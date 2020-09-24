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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core.Common.Notify;
using ASC.Notify.Messages;

namespace ASC.TelegramService
{
    public class TelegramListener
    {
        private ICacheNotify<NotifyMessage> CacheMessage { get; }
        private ICacheNotify<RegisterUserProto> CacheRegisterUser { get; }
        private ICacheNotify<CheckConnectionProto> CacheCheckConnection { get; }
        private ICacheNotify<RegistrationTokenProto> CacheRegistrationToken { get; }

        private ICacheNotify<GetConnectionProto> CacheGetConnection { get; }
        private ICacheNotify<SuccessfulRegTokenProto> CacheSuccessfulRegToken { get; }
        
        private TelegramService TelegramService { get; set; }

        public TelegramListener(ICacheNotify<NotifyMessage> cacheMessage, ICacheNotify<RegisterUserProto> cacheRegisterUser, ICacheNotify<CheckConnectionProto> cacheCheckConnection, ICacheNotify<RegistrationTokenProto> cacheRegistrationToken, ICacheNotify<GetConnectionProto> cacheGetConnection, ICacheNotify<SuccessfulRegTokenProto> cacheSuccessfulRegToken, TelegramService telegramService)
        {
            CacheMessage = cacheMessage;
            CacheRegisterUser = cacheRegisterUser;
            CacheCheckConnection = cacheCheckConnection;
            CacheRegistrationToken = cacheRegistrationToken;

            CacheGetConnection = cacheGetConnection;
            CacheSuccessfulRegToken = cacheSuccessfulRegToken;
            TelegramService = telegramService;
        }

        public void Start()
        {
            CacheMessage.Subscribe(n=> SendMessage(n), CacheNotifyAction.Insert);
            CacheRegisterUser.Subscribe(n=> RegisterUser(n), CacheNotifyAction.Insert);
            CacheCheckConnection.Subscribe(n=> CheckConnection(n), CacheNotifyAction.Insert);
            CacheRegistrationToken.Subscribe(n=> RegistrationToken(n), CacheNotifyAction.Insert);

        }

        public void Stop()
        {
            CacheMessage.Unsubscribe(CacheNotifyAction.Insert);
            CacheRegisterUser.Unsubscribe(CacheNotifyAction.Insert);
            CacheCheckConnection.Unsubscribe(CacheNotifyAction.Insert);
            CacheRegistrationToken.Unsubscribe(CacheNotifyAction.Insert);
        }
        
        public void SendMessage(NotifyMessage notifyMessage)
        {
            TelegramService.SendMessage(notifyMessage);
        }

        private void RegisterUser(RegisterUserProto registerUserProto)
        {
            TelegramService.RegisterUser(registerUserProto.UserId, registerUserProto.TenantId, registerUserProto.Token);
        }

        private void CheckConnection(CheckConnectionProto checkConnectionProto)
        {
            GetConnectionProto getConnectionProto = new GetConnectionProto();
            getConnectionProto.Connect = TelegramService.CheckConnection(checkConnectionProto.TenantId, checkConnectionProto.Token, checkConnectionProto.TokenLifespan, checkConnectionProto.Proxy);
            getConnectionProto.TenantId = checkConnectionProto.TenantId;
            getConnectionProto.Time = checkConnectionProto.Time;
            CacheGetConnection.Publish(getConnectionProto, CacheNotifyAction.Insert);
        }

        private void RegistrationToken(RegistrationTokenProto registrationTokenProto)
        {
            SuccessfulRegTokenProto successfulRegTokenProto = new SuccessfulRegTokenProto();
            var tmp = TelegramService.RegistrationToken(registrationTokenProto.UserId, registrationTokenProto.TenantId);
            successfulRegTokenProto.Token = tmp == null ? "" : tmp;
            successfulRegTokenProto.TenantId = registrationTokenProto.TenantId;
            successfulRegTokenProto.Time = registrationTokenProto.Time;
            CacheSuccessfulRegToken.Publish(successfulRegTokenProto, CacheNotifyAction.Insert);
        }
    }

    public static class TelegramListenerExtension
    {
        public static DIHelper AddTelegramListenerService(this DIHelper services)
        {
            services.TryAddSingleton<TelegramListener>();
            return services.AddTelegramService();
        }
    }
}
