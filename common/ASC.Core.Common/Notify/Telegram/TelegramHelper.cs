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
using System.Linq;
using System.Net;
using System.Security.Cryptography;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Notify.Telegram;
using ASC.Notify.Messages;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace ASC.Core.Common.Notify
{
    [Scope]
    public class TelegramHelper
    {
        public enum RegStatus
        {
            NotRegistered,
            Registered,
            AwaitingConfirmation
        }

        private ConsumerFactory ConsumerFactory { get; }
        private CachedTelegramDao CachedTelegramDao { get; }
        private TelegramServiceClient TelegramServiceClient { get; }
        private ILog Log { get; }

        public TelegramHelper(
            ConsumerFactory consumerFactory,
            IOptionsSnapshot<CachedTelegramDao> cachedTelegramDao,
            TelegramServiceClient telegramServiceClient,
            IOptionsMonitor<ILog> options)
        {
            ConsumerFactory = consumerFactory;
            CachedTelegramDao = cachedTelegramDao.Value;
            TelegramServiceClient = telegramServiceClient;
            Log = options.CurrentValue;
        }

        public string RegisterUser(Guid userId, int tenantId)
        {
            var token = GenerateToken(userId);

            TelegramServiceClient.RegisterUser(userId.ToString(), tenantId, token);

            return GetLink(token);
        }

        public void SendMessage(NotifyMessage msg)
        {
            TelegramServiceClient.SendMessage(msg);
        }

        public bool CreateClient(int tenantId, string token, int tokenLifespan, string proxy)
        {
            var client = InitClient(token, proxy);
            if (TestingClient(client))
            {
                TelegramServiceClient.CreateOrUpdateClient(tenantId, token, tokenLifespan, proxy);
                return true;
            }
            else
            {
                return false;
            }
        }

        public RegStatus UserIsConnected(Guid userId, int tenantId)
        {
            if (CachedTelegramDao.GetUser(userId, tenantId) != null) return RegStatus.Registered;

            return IsAwaitingRegistration(userId, tenantId) ? RegStatus.AwaitingConfirmation : RegStatus.NotRegistered;
        }

        public string CurrentRegistrationLink(Guid userId, int tenantId)
        {
            var token = GetCurrentToken(userId, tenantId);
            if (token == null || token == "") return "";

            return GetLink(token);
        }

        public void DisableClient(int tenantId)
        {
            TelegramServiceClient.DisableClient(tenantId);
        }

        public void Disconnect(Guid userId, int tenantId)
        {
            CachedTelegramDao.Delete(userId, tenantId);
        }

        private bool IsAwaitingRegistration(Guid userId, int tenantId)
        {
            return GetCurrentToken(userId, tenantId) != null;
        }

        private string GetCurrentToken(Guid userId, int tenantId)
        {
            return TelegramServiceClient.RegistrationToken(userId.ToString(), tenantId);
        }

        private string GenerateToken(Guid userId)
        {
            var id = userId.ToByteArray();
            var d = BitConverter.GetBytes(DateTime.Now.Ticks);

            var buf = id.Concat(d).ToArray();

            using var sha = new SHA256CryptoServiceProvider();
            return Convert.ToBase64String(sha.ComputeHash(buf))
                .Replace('+', '-').Replace('/', '_').Replace("=", ""); // make base64 url safe
        }

        private string GetLink(string token)
        {
            var tgProvider = (ITelegramLoginProvider)ConsumerFactory.GetByKey("telegram");
            var botname = tgProvider == null ? default : tgProvider.TelegramBotName;
            if (string.IsNullOrEmpty(botname)) return null;

            return string.Format("t.me/{0}?start={1}", botname, token);
        }

        public bool TestingClient(TelegramBotClient telegramBotClient)
        {
            try
            {
                if (!telegramBotClient.TestApiAsync().GetAwaiter().GetResult()) return false;
            }
            catch (Exception e)
            {
                Log.DebugFormat("Couldn't test api connection: {0}", e);
                return false;
            }
            return true;
        }

        public TelegramBotClient InitClient(string token, string proxy)
        {
            return string.IsNullOrEmpty(proxy) ? new TelegramBotClient(token) : new TelegramBotClient(token, new WebProxy(proxy));
        }
    }
}
