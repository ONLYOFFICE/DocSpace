// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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

    public Uri GetShoppingUri(string currency = null, string language = null, string customerEmail = null, Dictionary<string, int> quantity = null, string backUrl = null)
    {
        return _tariffService.GetShoppingUri(_tenantManager.GetCurrentTenant().Id, currency, language, customerEmail, quantity, backUrl);
    }

    public Uri GetAccountLink(int tenantId, string backUrl)
    {
        return _tariffService.GetAccountLink(tenantId, backUrl);
    }

    public bool ChangePayment(Dictionary<string, int> quantity)
    {
        return _tariffService.PaymentChange(_tenantManager.GetCurrentTenant().Id, quantity);
    }

    public void ActivateKey(string key)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(key);

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
