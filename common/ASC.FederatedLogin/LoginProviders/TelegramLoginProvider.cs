/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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


using System.Collections.Generic;

using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Notify;
using ASC.Core.Common.Notify.Telegram;

using Microsoft.Extensions.Configuration;

namespace ASC.FederatedLogin.LoginProviders
{
    public class TelegramLoginProvider : Consumer, IValidateKeysProvider, ITelegramLoginProvider
    {
        public string TelegramBotToken
        {
            get { return this["telegramBotToken"]; }
        }

        public string TelegramBotName
        {
            get { return this["telegramBotName"]; }
        }

        public int TelegramAuthTokenLifespan
        {
            get { return int.Parse(this["telegramAuthTokenLifespan"]); }
        }

        public string TelegramProxy
        {
            get { return this["telegramProxy"]; }
        }

        public bool IsEnabled()
        {
            return !string.IsNullOrEmpty(TelegramBotToken) && !string.IsNullOrEmpty(TelegramBotName);
        }

        private TelegramHelper TelegramHelper { get; }

        public TelegramLoginProvider() { }

        public TelegramLoginProvider(
            TelegramHelper telegramHelper,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
        {
            TelegramHelper = telegramHelper;
        }

        public bool ValidateKeys()
        {
            if (TelegramBotToken.Length == 0)
            {
                TelegramHelper.DisableClient(TenantManager.GetCurrentTenant().TenantId);
                return true;
            }
            else
            {
                return TelegramHelper.CreateClient(TenantManager.GetCurrentTenant().TenantId, TelegramBotToken, TelegramAuthTokenLifespan, TelegramProxy);
            }
        }
    }
}
