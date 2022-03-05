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

namespace ASC.Core;

[Scope]
public class PaymentManager
{
    private readonly ITariffService _tariffService;
    private readonly string _partnerUrl;
    private readonly string _partnerKey;

    private readonly TenantManager _tenantManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;

    public PaymentManager(TenantManager tenantManager, ITariffService tariffService, IConfiguration configuration, IHttpClientFactory clientFactory)
    {
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _configuration = configuration;
        _partnerUrl = (_configuration["core:payment:partners"] ?? "https://partners.onlyoffice.com/api").TrimEnd('/');
        _partnerKey = _configuration["core:machinekey"] ?? "C5C1F4E85A3A43F5B3202C24D97351DF";
        _clientFactory = clientFactory;
    }


    public Tariff GetTariff(int tenantId)
    {
        return _tariffService.GetTariff(tenantId);
    }

    public void SetTariff(int tenantId, Tariff tariff)
    {
        _tariffService.SetTariff(tenantId, tariff);
    }

    public void DeleteDefaultTariff()
    {
        _tariffService.DeleteDefaultBillingInfo();
    }

    public IEnumerable<PaymentInfo> GetTariffPayments(int tenant)
    {
        return _tariffService.GetPayments(tenant);
    }

    public IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(params string[] productIds)
    {
        return _tariffService.GetProductPriceInfo(productIds);
    }

    public Uri GetShoppingUri(int quotaId, bool forCurrentTenant = true, string affiliateId = null, string currency = null, string language = null, string customerId = null, string quantity = null)
    {
        return _tariffService.GetShoppingUri(forCurrentTenant ? _tenantManager.GetCurrentTenant().Id : (int?)null, quotaId, affiliateId, currency, language, customerId, quantity);
    }

    public Uri GetShoppingUri(int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null, string quantity = null)
    {
        return _tariffService.GetShoppingUri(null, quotaId, affiliateId, currency, language, customerId, quantity);
    }

    public void ActivateKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var now = DateTime.UtcNow;
        var actionUrl = "/partnerapi/ActivateKey?code=" + HttpUtility.UrlEncode(key) + "&portal=" + HttpUtility.UrlEncode(_tenantManager.GetCurrentTenant().Alias);

        var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", GetPartnerAuthHeader(actionUrl));
        request.RequestUri = new Uri(_partnerUrl + actionUrl);

        var httpClient = _clientFactory.CreateClient();

        using var response = httpClient.Send(request);

        _tariffService.ClearCache(_tenantManager.GetCurrentTenant().Id);

        var timeout = DateTime.UtcNow - now - TimeSpan.FromSeconds(5);
        if (TimeSpan.Zero < timeout)
        {
            // clear tenant cache
            Thread.Sleep(timeout);
        }

        _tenantManager.GetTenant(_tenantManager.GetCurrentTenant().Id);
    }

    private string GetPartnerAuthHeader(string url)
    {
        using var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(_partnerKey));
        var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var data = string.Join("\n", now, "/api/" + url.TrimStart('/')); //data: UTC DateTime (yyyy:MM:dd HH:mm:ss) + \n + url
        var hash = WebEncoders.Base64UrlEncode(hasher.ComputeHash(Encoding.UTF8.GetBytes(data)));

        return $"ASC :{now}:{hash}";
    }

}
