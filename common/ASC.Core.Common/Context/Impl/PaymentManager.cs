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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Core.Billing;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;


namespace ASC.Core
{
    [Scope]
    public class PaymentManager
    {
        private readonly ITariffService tariffService;
        private readonly string partnerUrl;
        private readonly string partnerKey;

        private TenantManager TenantManager { get; }
        private IConfiguration Configuration { get; }

        public PaymentManager(TenantManager tenantManager, ITariffService tariffService, IConfiguration configuration)
        {
            TenantManager = tenantManager;
            this.tariffService = tariffService;
            Configuration = configuration;
            partnerUrl = (Configuration["core:payment:partners"] ?? "https://partners.onlyoffice.com/api").TrimEnd('/');
            partnerKey = (Configuration["core:machinekey"] ?? "C5C1F4E85A3A43F5B3202C24D97351DF");
        }


        public Tariff GetTariff(int tenantId)
        {
            return tariffService.GetTariff(tenantId);
        }

        public void SetTariff(int tenantId, Tariff tariff)
        {
            tariffService.SetTariff(tenantId, tariff);
        }

        public void DeleteDefaultTariff()
        {
            tariffService.DeleteDefaultBillingInfo();
        }

        public IEnumerable<PaymentInfo> GetTariffPayments(int tenant)
        {
            return tariffService.GetPayments(tenant);
        }

        public IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(params string[] productIds)
        {
            return tariffService.GetProductPriceInfo(productIds);
        }


        public Uri GetShoppingUri(int quotaId, bool forCurrentTenant = true, string affiliateId = null, string currency = null, string language = null, string customerId = null, string quantity = null)
        {
            return tariffService.GetShoppingUri(forCurrentTenant ? TenantManager.GetCurrentTenant().TenantId : (int?)null, quotaId, affiliateId, currency, language, customerId, quantity);
        }

        public Uri GetShoppingUri(int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null, string quantity = null)
        {
            return tariffService.GetShoppingUri(null, quotaId, affiliateId, currency, language, customerId, quantity);
        }

        public void ActivateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            var now = DateTime.UtcNow;
            var actionUrl = "/partnerapi/ActivateKey?code=" + HttpUtility.UrlEncode(key) + "&portal=" + HttpUtility.UrlEncode(TenantManager.GetCurrentTenant().TenantAlias);
            using var webClient = new WebClient();
            webClient.Headers.Add("Authorization", GetPartnerAuthHeader(actionUrl));
            try
            {
                webClient.DownloadData(partnerUrl + actionUrl);
            }
            catch (WebException we)
            {
                var error = GetException(we);
                if (error != null)
                {
                    throw error;
                }
                throw;
            }
            tariffService.ClearCache(TenantManager.GetCurrentTenant().TenantId);

            var timeout = DateTime.UtcNow - now - TimeSpan.FromSeconds(5);
            if (TimeSpan.Zero < timeout)
            {
                // clear tenant cache
                Thread.Sleep(timeout);
            }
            TenantManager.GetTenant(TenantManager.GetCurrentTenant().TenantId);
        }

        private string GetPartnerAuthHeader(string url)
        {
            using var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(partnerKey));
            var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var data = string.Join("\n", now, "/api/" + url.TrimStart('/')); //data: UTC DateTime (yyyy:MM:dd HH:mm:ss) + \n + url
            var hash = WebEncoders.Base64UrlEncode(hasher.ComputeHash(Encoding.UTF8.GetBytes(data)));
            return string.Format("ASC :{0}:{1}", now, hash);
        }

        private static Exception GetException(WebException we)
        {
            var response = (HttpWebResponse)we.Response;
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var result = reader.ReadToEnd();
                var excInfo = JsonConvert.DeserializeObject<ExceptionJson>(result);
                return (Exception)Activator.CreateInstance(Type.GetType(excInfo.exceptionType, true), excInfo.exceptionMessage);
            }
            return null;
        }


        private class ExceptionJson
        {
            public string message = null;
            public string exceptionMessage = null;
            public string exceptionType = null;
            public string stackTrace = null;
        }
    }
}
