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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.Json;

using ASC.Common;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace ASC.Core.Billing
{
    [Singletone]
    public class BillingClient
    {
        public readonly bool Configured = false;
        private readonly string _billingDomain;
        private readonly string _billingKey;
        private readonly string _billingSecret;
        private readonly bool _test;
        private readonly IHttpClientFactory _httpClientFactory;
        private const int AvangatePaymentSystemId = 1;


        public BillingClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
            : this(false, configuration, httpClientFactory)
        {
        }

        public BillingClient(bool test, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _test = test;
            _httpClientFactory = httpClientFactory;
            var billingDomain = configuration["core:payment-url"];

            _billingDomain = (billingDomain ?? "").Trim().TrimEnd('/');
            if (!string.IsNullOrEmpty(_billingDomain))
            {
                _billingDomain += "/billing/";

                _billingKey = configuration["core:payment-key"];
                _billingSecret = configuration["core:payment-secret"];

                Configured = true;
            }
        }

        public PaymentLast GetLastPayment(string portalId)
        {
            var result = Request("GetActiveResource", portalId);
            var paymentLast = JsonSerializer.Deserialize<PaymentLast>(result);

            if (!_test && paymentLast.PaymentStatus == 4)
            {
                throw new BillingException("Can not accept test payment.", new { PortalId = portalId });
            }

            return paymentLast;
        }

        public IEnumerable<PaymentInfo> GetPayments(string portalId)
        {
            string result = Request("GetPayments", portalId);
            var payments = JsonSerializer.Deserialize<List<PaymentInfo>>(result);

            return payments;
        }

        public IDictionary<string, Tuple<Uri, Uri>> GetPaymentUrls(string portalId, string[] products, string affiliateId = null, string campaign = null, string currency = null, string language = null, string customerId = null, string quantity = null)
        {
            var urls = new Dictionary<string, Tuple<Uri, Uri>>();

            var additionalParameters = new List<Tuple<string, string>>() { Tuple.Create("PaymentSystemId", AvangatePaymentSystemId.ToString()) };
            if (!string.IsNullOrEmpty(affiliateId))
            {
                additionalParameters.Add(Tuple.Create("AffiliateId", affiliateId));
            }
            if (!string.IsNullOrEmpty(campaign))
            {
                additionalParameters.Add(Tuple.Create("campaign", campaign));
            }
            if (!string.IsNullOrEmpty(currency))
            {
                additionalParameters.Add(Tuple.Create("Currency", currency));
            }
            if (!string.IsNullOrEmpty(language))
            {
                additionalParameters.Add(Tuple.Create("Language", language));
            }
            if (!string.IsNullOrEmpty(customerId))
            {
                additionalParameters.Add(Tuple.Create("CustomerID", customerId));
            }
            if (!string.IsNullOrEmpty(quantity))
            {
                additionalParameters.Add(Tuple.Create("Quantity", quantity));
            }

            var parameters = products
                .Distinct()
                .Select(p => Tuple.Create("ProductId", p))
                .Concat(additionalParameters)
                .ToArray();

            //max 100 products
            var result = Request("GetPaymentUrl", portalId, parameters);
            var paymentUrls = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            var upgradeUrls = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(portalId)
                //TODO: remove
                && false)
            {
                try
                {
                    //max 100 products
                    result = Request("GetPaymentUpgradeUrl", portalId, parameters);
                    upgradeUrls = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
                }
                catch (BillingNotFoundException)
                {
                }
            }

            foreach (var p in products)
            {
                string url;
                var paymentUrl = (Uri)null;
                var upgradeUrl = (Uri)null;
                if (paymentUrls.TryGetValue(p, out url))
                {
                    url = ToUrl(url);
                    if (!string.IsNullOrEmpty(url))
                    {
                        paymentUrl = new Uri(url);
                    }
                }
                if (upgradeUrls.TryGetValue(p, out url))
                {
                    url = ToUrl(url);
                    if (!string.IsNullOrEmpty(url))
                    {
                        upgradeUrl = new Uri(url);
                    }
                }
                urls[p] = Tuple.Create(paymentUrl, upgradeUrl);
            }
            return urls;
        }

        public IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(params string[] productIds)
        {
            if (productIds == null)
            {
                throw new ArgumentNullException(nameof(productIds));
            }

            var parameters = productIds.Select(pid => Tuple.Create("ProductId", pid)).ToList();
            parameters.Add(Tuple.Create("PaymentSystemId", AvangatePaymentSystemId.ToString()));

            var result = Request("GetProductsPrices", null, parameters.ToArray());
            var prices = JsonSerializer.Deserialize<Dictionary<int, Dictionary<string, Dictionary<string, decimal>>>>(result);

            if (prices.TryGetValue(AvangatePaymentSystemId, out var pricesPaymentSystem))
            {
                return productIds.Select(productId =>
                {
                    if (pricesPaymentSystem.TryGetValue(productId, out var prices))
                    {
                        return new { ProductId = productId, Prices = prices };
                    }
                    return new { ProductId = productId, Prices = new Dictionary<string, decimal>() };
                })
                    .ToDictionary(e => e.ProductId, e => e.Prices);
            }

            return new Dictionary<string, Dictionary<string, decimal>>();
        }


        private string CreateAuthToken(string pkey, string machinekey)
        {
            using (var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(machinekey)))
            {
                var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var hash = WebEncoders.Base64UrlEncode(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));
                return "ASC " + pkey + ":" + now + ":" + hash;
            }
        }

        private string Request(string method, string portalId, params Tuple<string, string>[] parameters)
        {
            var url = _billingDomain + method;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = HttpMethod.Post;
            if (!string.IsNullOrEmpty(_billingKey))
            {
                request.Headers.Add("Authorization", CreateAuthToken(_billingKey, _billingSecret));
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(60000);

            var data = new Dictionary<string, List<string>>();
            if (!string.IsNullOrEmpty(portalId))
            {
                data.Add("PortalId", new List<string>() { portalId });
            }
            foreach (var parameter in parameters)
            {
                if (!data.ContainsKey(parameter.Item1))
                {
                    data.Add(parameter.Item1, new List<string>() { parameter.Item2 });
                }
                else
                {
                    data[parameter.Item1].Add(parameter.Item2);
                }
            }

            var body = JsonSerializer.Serialize(data);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            string result;
            using (var response = httpClient.Send(request))
            using (var stream = response.Content.ReadAsStream())
            {
                if (stream == null)
                {
                    throw new BillingNotConfiguredException("Billing response is null");
                }
                using (var readStream = new StreamReader(stream))
                {
                    result = readStream.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                throw new BillingNotConfiguredException("Billing response is null");
            }
            if (!result.StartsWith("{\"Message\":\"error"))
            {
                return result;
            }

            var @params = parameters.Select(p => p.Item1 + ": " + p.Item2);
            var info = new { Method = method, PortalId = portalId, Params = string.Join(", ", @params) };
            if (result.Contains("{\"Message\":\"error: cannot find "))
            {
                throw new BillingNotFoundException(result, info);
            }
            throw new BillingException(result, info);
        }

        private string ToUrl(string s)
        {
            s = s.Trim();
            if (s.StartsWith("error", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            if (_test && !s.Contains("&DOTEST = 1"))
            {
                s += "&DOTEST=1";
            }
            return s;
        }
    }


    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        Message Request(Message message);
    }

    [Serializable]
    public class Message
    {
        public string Content { get; set; }

        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        Undefined = 0,
        Data = 1,
        Error = 2,
    }

    [Serializable]
    public class BillingException : Exception
    {
        public BillingException(string message, object debugInfo = null) : base(message + (debugInfo != null ? " Debug info: " + debugInfo : string.Empty))
        {
        }

        public BillingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BillingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class BillingNotFoundException : BillingException
    {
        public BillingNotFoundException(string message, object debugInfo = null) : base(message, debugInfo)
        {
        }

        protected BillingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class BillingNotConfiguredException : BillingException
    {
        public BillingNotConfiguredException(string message, object debugInfo = null) : base(message, debugInfo)
        {
        }

        public BillingNotConfiguredException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BillingNotConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}