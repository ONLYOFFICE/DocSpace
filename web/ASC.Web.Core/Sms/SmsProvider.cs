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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Tenants;
using ASC.FederatedLogin.LoginProviders;
using ASC.VoipService.Dao;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ASC.Web.Core.Sms
{
    [Scope(Additional = typeof(TwilioProviderExtention))]
    public class SmsProviderManager
    {
        public SmscProvider SmscProvider { get => ConsumerFactory.Get<SmscProvider>(); }
        private ConsumerFactory ConsumerFactory { get; }

        public ClickatellProvider ClickatellProvider { get => ConsumerFactory.Get<ClickatellProvider>(); }
        public TwilioProvider TwilioProvider { get => ConsumerFactory.Get<TwilioProvider>(); }

        public ClickatellProvider ClickatellUSAProvider { get => ConsumerFactory.Get<ClickatellUSAProvider>(); }
        public TwilioProvider TwilioSaaSProvider { get => ConsumerFactory.Get<TwilioSaaSProvider>(); }

        public SmsProviderManager(ConsumerFactory consumerFactory)
        {
            ConsumerFactory = consumerFactory;
        }

        public bool Enabled()
        {
            return SmscProvider.Enable() || ClickatellProvider.Enable() || ClickatellUSAProvider.Enable() || TwilioProvider.Enable() || TwilioSaaSProvider.Enable();
        }

        public Task<bool> SendMessageAsync(string number, string message)
        {
            if (!Enabled()) return Task.FromResult(false);

            SmsProvider provider = null;
            if (ClickatellProvider.Enable())
            {
                provider = ClickatellProvider;
            }

            string smsUsa;
            if (ClickatellUSAProvider.Enable()
                && !string.IsNullOrEmpty(smsUsa = ClickatellProvider["clickatellUSA"]) && Regex.IsMatch(number, smsUsa))
            {
                provider = ClickatellUSAProvider;
            }

            if (provider == null && TwilioProvider.Enable())
            {
                provider = TwilioProvider;
            }

            if (provider == null && TwilioSaaSProvider.Enable())
            {
                provider = TwilioSaaSProvider;
            }

            if (SmscProvider.Enable()
                && (provider == null
                    || SmscProvider.SuitableNumber(number)))
            {
                provider = SmscProvider;
            }

            if (provider == null)
            {
                return Task.FromResult(false);
            }

            return provider.SendMessageAsync(number, message);
        }
    }

    public abstract class SmsProvider : Consumer
    {
        protected ILog Log { get; }
        protected IHttpClientFactory ClientFactory { get; }
        protected ICache MemoryCache { get; set; }

        protected virtual string SendMessageUrlFormat { get; set; }
        protected virtual string GetBalanceUrlFormat { get; set; }
        protected virtual string Key { get; set; }
        protected virtual string Secret { get; set; }
        protected virtual string Sender { get; set; }

        protected SmsProvider()
        {
        }

        protected SmsProvider(
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory,
            ICache memCache,
            string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
        {
            MemoryCache = memCache;
            Log = options.CurrentValue;
            ClientFactory = clientFactory;
        }

        public virtual bool Enable()
        {
            return true;
        }

        private string SendMessageUrl()
        {
            return SendMessageUrlFormat
                .Replace("{key}", Key)
                .Replace("{secret}", Secret)
                .Replace("{sender}", Sender);
        }

        public virtual async Task<bool> SendMessageAsync(string number, string message)
        {
            try
            {
                var url = SendMessageUrl();
                url = url.Replace("{phone}", number).Replace("{text}", HttpUtility.UrlEncode(message));

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var httpClient = ClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromMilliseconds(15000);

                using var response = await httpClient.SendAsync(request);
                using var stream = await response.Content.ReadAsStreamAsync();
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var result = await reader.ReadToEndAsync();
                    Log.InfoFormat("SMS was sent to {0}, service returned: {1}", number, result);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to send sms message", ex);
            }
            return false;
        }
    }

    public class SmscProvider : SmsProvider, IValidateKeysProvider
    {
        public SmscProvider()
        {
        }

        public SmscProvider(
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory,
            ICache memCache,
            string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, options, clientFactory, memCache, name, order, props, additional)
        {
        }

        protected override string SendMessageUrlFormat
        {
            get { return "https://smsc.ru/sys/send.php?login={key}&psw={secret}&phones={phone}&mes={text}&fmt=3&sender={sender}&charset=utf-8"; }
        }

        protected override string GetBalanceUrlFormat
        {
            get { return "https://smsc.ru/sys/balance.php?login={key}&psw={secret}"; }
        }

        protected override string Key
        {
            get { return this["smsclogin"]; }
        }

        protected override string Secret
        {
            get { return this["smscpsw"]; }
        }

        protected override string Sender
        {
            get { return this["smscsender"]; }
        }

        public override bool Enable()
        {
            return
                !string.IsNullOrEmpty(Key)
                && !string.IsNullOrEmpty(Secret);
        }

        public async Task<string> GetBalanceAsync(Tenant tenant, bool eraseCache = false)
        {
            var tenantCache = tenant == null ? Tenant.DEFAULT_TENANT : tenant.TenantId;

            var key = "sms/smsc/" + tenantCache;
            if (eraseCache) MemoryCache.Remove(key);

            var balance = MemoryCache.Get<string>(key);

            if (string.IsNullOrEmpty(balance))
            {
                try
                {
                    var url = GetBalanceUrl();

                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var httpClient = ClientFactory.CreateClient();
                    httpClient.Timeout = TimeSpan.FromMilliseconds(1000);

                    using var response = await httpClient.SendAsync(request);
                    using var stream = await response.Content.ReadAsStreamAsync();
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream);
                        var result = await reader.ReadToEndAsync();
                        Log.InfoFormat("SMS balance service returned: {0}", result);

                        balance = result;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Failed request sms balance", ex);
                    balance = string.Empty;
                }

                MemoryCache.Insert(key, balance, TimeSpan.FromMinutes(1));
            }

            return balance;
        }

        private string GetBalanceUrl()
        {
            return GetBalanceUrlFormat
                .Replace("{key}", Key)
                .Replace("{secret}", Secret);
        }

        public bool SuitableNumber(string number)
        {
            var smsCis = this["smsccis"];
            return !string.IsNullOrEmpty(smsCis) && Regex.IsMatch(number, smsCis);
        }

        public bool ValidateKeys()
        {
            return double.TryParse(GetBalanceAsync(TenantManager.GetCurrentTenant(false), true).Result, NumberStyles.Number, CultureInfo.InvariantCulture, out var balance) && balance > 0;
        }
    }

    public class ClickatellProvider : SmsProvider
    {
        protected override string SendMessageUrlFormat
        {
            get { return "https://platform.clickatell.com/messages/http/send?apiKey={secret}&to={phone}&content={text}&from={sender}"; }
        }

        protected override string Secret
        {
            get { return this["clickatellapiKey"]; }
        }

        protected override string Sender
        {
            get { return this["clickatellSender"]; }
        }

        public override bool Enable()
        {
            return !string.IsNullOrEmpty(Secret);
        }

        public ClickatellProvider()
        {
        }

        public ClickatellProvider(
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory,
            ICache memCache,
            string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, options, clientFactory, memCache, name, order, props, additional)
        {
        }
    }

    public class ClickatellUSAProvider : ClickatellProvider
    {
        public ClickatellUSAProvider()
        {
        }

        public ClickatellUSAProvider(
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory,
            ICache memCache,
            string name, int order, Dictionary<string, string> additional = null)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, options, clientFactory, memCache, name, order, null, additional)
        {
        }
    }

    [Scope]
    public class TwilioProvider : SmsProvider, IValidateKeysProvider
    {
        protected override string Key
        {
            get { return this["twilioAccountSid"]; }
        }

        protected override string Secret
        {
            get { return this["twilioAuthToken"]; }
        }

        protected override string Sender
        {
            get { return this["twiliosender"]; }
        }

        public AuthContext AuthContext { get; }
        public TenantUtil TenantUtil { get; }
        public SecurityContext SecurityContext { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        public TwilioProviderCleaner TwilioProviderCleaner { get; }

        public override bool Enable()
        {
            return
                !string.IsNullOrEmpty(Key)
                && !string.IsNullOrEmpty(Secret)
                && !string.IsNullOrEmpty(Sender);
        }

        public override Task<bool> SendMessageAsync(string number, string message)
        {
            if (!number.StartsWith('+')) number = "+" + number;
            var twilioRestClient = new TwilioRestClient(Key, Secret);

            try
            {
                var smsMessage = MessageResource.Create(new PhoneNumber(number), body: message, @from: new PhoneNumber(Sender), client: twilioRestClient);
                Log.InfoFormat("SMS was sent to {0}, status: {1}", number, smsMessage.Status);
                if (!smsMessage.ErrorCode.HasValue)
                {
                    return Task.FromResult(true);
                }
                Log.Error("Failed to send sms. code: " + smsMessage.ErrorCode.Value + " message: " + smsMessage.ErrorMessage);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to send sms message via tiwilio", ex);
            }

            return Task.FromResult(false);
        }

        public TwilioProvider()
        {
        }

        public TwilioProvider(
            AuthContext authContext,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            BaseCommonLinkUtility baseCommonLinkUtility,
            TwilioProviderCleaner twilioProviderCleaner,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory,
            ICache memCache,
            string name, int order, Dictionary<string, string> props)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, options, clientFactory, memCache, name, order, props)
        {
            AuthContext = authContext;
            TenantUtil = tenantUtil;
            SecurityContext = securityContext;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            TwilioProviderCleaner = twilioProviderCleaner;
        }


        public bool ValidateKeys()
        {
            try
            {
                new VoipService.Twilio.TwilioProvider(Key, Secret, AuthContext, TenantUtil, SecurityContext, BaseCommonLinkUtility).GetExistingPhoneNumbers();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ClearOldNumbers()
        {
            TwilioProviderCleaner.ClearOldNumbers(Key, Secret);
        }
    }

    [Scope]
    public class TwilioSaaSProvider : TwilioProvider
    {
        public TwilioSaaSProvider()
        {
        }

        public TwilioSaaSProvider(
            AuthContext authContext,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            BaseCommonLinkUtility baseCommonLinkUtility,
            TwilioProviderCleaner twilioProviderCleaner,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory,
            ICache memCache,
            string name, int order)
            : base(authContext, tenantUtil, securityContext, baseCommonLinkUtility, twilioProviderCleaner, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, options, clientFactory, memCache, name, order, null)
        {
        }
    }

    [Scope]
    public class TwilioProviderCleaner
    {
        private VoipDao VoipDao { get; }
        private AuthContext AuthContext { get; }
        private TenantUtil TenantUtil { get; }
        private SecurityContext SecurityContext { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }

        public TwilioProviderCleaner(VoipDao voipDao, AuthContext authContext, TenantUtil tenantUtil, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
        {
            VoipDao = voipDao;
            AuthContext = authContext;
            TenantUtil = tenantUtil;
            SecurityContext = securityContext;
            BaseCommonLinkUtility = baseCommonLinkUtility;
        }

        public void ClearOldNumbers(string key, string secret)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret)) return;

            var provider = new VoipService.Twilio.TwilioProvider(key, secret, AuthContext, TenantUtil, SecurityContext, BaseCommonLinkUtility);

            var numbers = VoipDao.GetNumbers();
            foreach (var number in numbers)
            {
                provider.DisablePhone(number);
                VoipDao.DeleteNumber(number.Id);
            }
        }
    }

    public static class TwilioProviderExtention
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<TwilioSaaSProvider>();
        }
    }
}