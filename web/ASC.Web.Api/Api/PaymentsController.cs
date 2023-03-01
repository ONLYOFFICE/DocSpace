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

namespace ASC.Web.Api.Controllers;

///<summary>
/// Portal information access.
///</summary>
///<name>portal</name>
[Scope]
[DefaultRoute]
[ApiController]
[AllowNotPayment]
[ControllerName("portal")]
public class PaymentController : ControllerBase
{
    private readonly ApiContext _apiContext;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly ITariffService _tariffService;
    private readonly SecurityContext _securityContext;
    private readonly RegionHelper _regionHelper;
    private readonly QuotaHelper _quotaHelper;
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly int _maxCount = 10;
    private readonly int _expirationMinutes = 2;
    protected Tenant Tenant { get { return _apiContext.Tenant; } }

    public PaymentController(
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        ITariffService tariffService,
        SecurityContext securityContext,
        RegionHelper regionHelper,
        QuotaHelper tariffHelper,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        MessageService messageService,
        StudioNotifyService studioNotifyService)
    {
        _apiContext = apiContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _securityContext = securityContext;
        _regionHelper = regionHelper;
        _quotaHelper = tariffHelper;
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
    }

    /// <summary>
    /// Returns the URL to the payment page.
    /// </summary>
    /// <short>
    /// Get the payment page URL
    /// </short>
    /// <param name="inDto">Payment URL request parameters: the quantity of payment, back URL</param>
    /// <returns>The URL to the payment page</returns>
    /// <path>api/2.0/portal/payment/url</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("payment/url")]
    public async Task<Uri> GetPaymentUrl(PaymentUrlRequestsDto inDto)
    {
        if (_tariffService.GetPayments(Tenant.Id).Any() ||
            !_userManager.IsDocSpaceAdmin(_securityContext.CurrentAccount.ID))
        {
            return null;
        }

        var currency = _regionHelper.GetCurrencyFromRequest();

        return await _tariffService.GetShoppingUri(Tenant.Id, currency,
            Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName,
            _userManager.GetUsers(_securityContext.CurrentAccount.ID).Email,
            inDto.Quantity,
            inDto.BackUrl);
    }

    /// <summary>
    /// Updates the quantity of payment.
    /// </summary>
    /// <short>
    /// Update the payment quantity
    /// </short>
    /// <param name="inDto">Payment URL request parameters: the quantity of payment, back URL</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/portal/payment/update</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("payment/update")]
    public async Task<bool> PaymentUpdate(PaymentUrlRequestsDto inDto)
    {
        var payerId = _tariffService.GetTariff(Tenant.Id).CustomerId;
        var payer = _userManager.GetUserByEmail(payerId);

        if (!_tariffService.GetPayments(Tenant.Id).Any() ||
            _securityContext.CurrentAccount.ID != payer.Id)
        {
            return false;
        }

        return await _tariffService.PaymentChange(Tenant.Id, inDto.Quantity);
    }

    /// <summary>
    /// Returns the URL to the payment account.
    /// </summary>
    /// <short>
    /// Get the payment account
    /// </short>
    /// <param name="backUrl">Back URL</param>
    /// <returns>The URL to the payment account</returns>
    /// <path>api/2.0/portal/payment/account</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("payment/account")]
    public Uri GetPaymentAccount(string backUrl)
    {
        var payerId = _tariffService.GetTariff(Tenant.Id).CustomerId;
        var payer = _userManager.GetUserByEmail(payerId);

        if (_securityContext.CurrentAccount.ID != payer.Id &&
            _securityContext.CurrentAccount.ID != Tenant.OwnerId)
        {
            return null;
        }

        return _tariffService.GetAccountLink(Tenant.Id, backUrl);
    }

    /// <summary>
    /// Returns the available portal prices.
    /// </summary>
    /// <short>
    /// Get prices
    /// </short>
    /// <returns>List of available portal prices</returns>
    /// <path>api/2.0/portal/payment/prices</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("payment/prices")]
    public object GetPrices()
    {
        var currency = _regionHelper.GetCurrencyFromRequest();
        var result = _tenantManager.GetProductPriceInfo()
            .ToDictionary(pr => pr.Key, pr => pr.Value.ContainsKey(currency) ? pr.Value[currency] : 0);
        return result;
    }


    /// <summary>
    /// Returns the available portal currencies.
    /// </summary>
    /// <short>
    /// Get currencies
    /// </short>
    /// <returns>List of available portal currencies</returns>
    /// <path>api/2.0/portal/payment/currencies</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("payment/currencies")]
    public IEnumerable<CurrenciesDto> GetCurrencies()
    {
        var defaultRegion = _regionHelper.GetDefaultRegionInfo();
        var currentRegion = _regionHelper.GetCurrentRegionInfo();

        yield return new CurrenciesDto(defaultRegion);

        if (!currentRegion.Name.Equals(defaultRegion.Name))
        {
            yield return new CurrenciesDto(currentRegion);
        }
    }

    /// <summary>
    /// Returns the available portal quotas.
    /// </summary>
    /// <short>
    /// Get quotas
    /// </short>
    /// <returns>List of available portal quotas</returns>
    /// <path>api/2.0/portal/payment/quotas</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("payment/quotas")]
    public IEnumerable<QuotaDto> GetQuotas()
    {
        return _quotaHelper.GetQuotas();
    }

    /// <summary>
    /// Returns the current portal quota.
    /// </summary>
    /// <short>
    /// Get portal quota
    /// </short>
    /// <returns>Current portal quota</returns>
    /// <path>api/2.0/portal/payment/quota</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("payment/quota")]
    public QuotaDto GetQuota()
    {
        return _quotaHelper.GetCurrentQuota();
    }

    /// <summary>
    /// Sends a request for portal payment.
    /// </summary>
    /// <short>
    /// Send a payment request
    /// </short>
    /// <param name="inDto">Portal payment request parameters: username, email, message</param>
    /// <returns></returns>
    /// <path>api/2.0/portal/payment/request</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("payment/request")]
    public void SendSalesRequest(SalesRequestsDto inDto)
    {
        if (!inDto.Email.TestEmailRegex())
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        if (string.IsNullOrEmpty(inDto.Message))
        {
            throw new Exception(Resource.ErrorEmptyMessage);
        }

        CheckCache("salesrequest");

        _studioNotifyService.SendMsgToSales(inDto.Email, inDto.UserName, inDto.Message);
        _messageService.Send(MessageAction.ContactSalesMailSent);
    }

    internal void CheckCache(string basekey)
    {
        var key = _httpContextAccessor.HttpContext.Request.GetUserHostAddress() + basekey;
        if (_memoryCache.TryGetValue<int>(key, out var count))
        {
            if (count > _maxCount)
            {
                throw new Exception(Resource.ErrorRequestLimitExceeded);
            }
        }

        _memoryCache.Set(key, count + 1, TimeSpan.FromMinutes(_expirationMinutes));
    }
}
