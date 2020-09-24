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
using System.Threading;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Notify.Messages;

namespace ASC.Core.Common.Notify
{
    public class TelegramServiceClient : ITelegramService
    {
        private ICacheNotify<NotifyMessage> CacheMessage { get; }
        private ICacheNotify<RegisterUserProto> CacheRegisterUser { get; }
        private ICacheNotify<CheckConnectionProto> CacheCheckConnection { get; }
        private ICacheNotify<RegistrationTokenProto> CacheRegistrationToken { get; }

        private ICacheNotify<GetConnectionProto> CacheGetConnection { get; }
        private ICacheNotify<SuccessfulRegTokenProto> CacheSuccessfulRegistrationToken { get; }

        private ICache Cache { get; }

        public TelegramServiceClient(ICacheNotify<NotifyMessage> cacheMessage,
            ICacheNotify<RegisterUserProto> cacheRegisterUser,
            ICacheNotify<CheckConnectionProto> cacheCheckConnection,
            ICacheNotify<RegistrationTokenProto> cacheRegistrationToken,
            ICacheNotify<GetConnectionProto> cacheGetConnection,
            ICacheNotify<SuccessfulRegTokenProto> cacheSuccessfulRegistrationToken)
        {
            CacheMessage = cacheMessage;
            CacheRegisterUser = cacheRegisterUser;
            CacheCheckConnection = cacheCheckConnection;
            CacheRegistrationToken = cacheRegistrationToken;
            CacheGetConnection = cacheGetConnection;
            CacheSuccessfulRegistrationToken = cacheSuccessfulRegistrationToken;
            CacheGetConnection.Subscribe(n => SaveAnswerConnect(n), CacheNotifyAction.Insert);
            CacheSuccessfulRegistrationToken.Subscribe(n => SaveAnswerRegistration(n), CacheNotifyAction.Insert);
            Cache = AscCache.Memory;
        }

        public void SendMessage(NotifyMessage m)
        {
            CacheMessage.Publish(m, CacheNotifyAction.Insert);
        }

        public void RegisterUser(string userId, int tenantId, string token)
        {
            CacheRegisterUser.Publish(new RegisterUserProto() { 
                UserId = userId,
                TenantId = tenantId,
                Token = token } , CacheNotifyAction.Insert);
        }

        public bool CheckConnection(int tenantId, string token, int tokenLifespan, string proxy)
        {
            var time = DateTime.Now.ToString("o");
            CacheCheckConnection.Publish(new CheckConnectionProto()
            {
                TenantId = tenantId,
                Token = token,
                TokenLifespan = tokenLifespan,
                Proxy = proxy,
                Time = time
            }, CacheNotifyAction.Insert);

            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    var cache = Cache.Get<GetConnectionProto>(GetCacheConnectKey(tenantId, time)).Connect;
                    Cache.Remove(GetCacheConnectKey(tenantId, time));
                    return cache;
                }
                catch (Exception e)
                {

                }
            }
        }

        public string RegistrationToken(string userId, int tenantId)
        {
            var time = DateTime.Now.ToString("o");
            CacheRegistrationToken.Publish(new RegistrationTokenProto()
            {
                UserId = userId,
                TenantId = tenantId,
                Time = time
            }, CacheNotifyAction.Insert);
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    var cache = Cache.Get<SuccessfulRegTokenProto>(GetCacheRegKey(tenantId, time)).Token;
                    Cache.Remove(GetCacheRegKey(tenantId, time));
                    return cache;
                }
                catch (Exception e)
                {

                }
            }
        }

        private string GetCacheConnectKey(int tenantId, string time)
        {
            return typeof(GetConnectionProto).FullName + tenantId + time;
        }

        private string GetCacheRegKey(int tenantId, string time)
        {
            return typeof(SuccessfulRegTokenProto).FullName + tenantId + time;
        }

        private void SaveAnswerConnect(GetConnectionProto getConnectionProto)
        {
            Cache.Insert(GetCacheConnectKey(getConnectionProto.TenantId, getConnectionProto.Time), getConnectionProto, DateTime.MaxValue);
        }

        private void SaveAnswerRegistration(SuccessfulRegTokenProto successfulRegTokenProto)
        {
            Cache.Insert(GetCacheRegKey(successfulRegTokenProto.TenantId, successfulRegTokenProto.Time), successfulRegTokenProto, DateTime.MaxValue);
        }
    }

    public static class TelegramServiceClientExtension
    {
        public static DIHelper AddTelegramServiceClient(this DIHelper services)
        {
            services.TryAddSingleton<TelegramServiceClient>();
            return services;
        }
    }
}
