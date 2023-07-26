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

namespace ASC.Core.Billing;

[Singletone]
public class TenantExtraConfig
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly LicenseReaderConfig _licenseReaderConfig;

    public TenantExtraConfig(CoreBaseSettings coreBaseSettings, LicenseReaderConfig licenseReaderConfig)
    {
        _coreBaseSettings = coreBaseSettings;
        _licenseReaderConfig = licenseReaderConfig;
    }

    public bool Saas
    {
        get { return !_coreBaseSettings.Standalone; }
    }

    public bool Enterprise
    {
        get { return _coreBaseSettings.Standalone && !string.IsNullOrEmpty(_licenseReaderConfig.LicensePath); }
    }

    public bool Opensource
    {
        get { return _coreBaseSettings.Standalone && string.IsNullOrEmpty(_licenseReaderConfig.LicensePath); }
    }
}

[Singletone]
public class TariffServiceStorage
{
    private static readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _standaloneCacheExpiration = TimeSpan.FromMinutes(15);
    internal readonly ICache Cache;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IServiceProvider _serviceProvider;
    internal readonly ICacheNotify<TariffCacheItem> Notify;
    private TimeSpan _cacheExpiration;

    public TariffServiceStorage(ICacheNotify<TariffCacheItem> notify, ICache cache, CoreBaseSettings coreBaseSettings, IServiceProvider serviceProvider)
    {
        _cacheExpiration = _defaultCacheExpiration;

        Cache = cache;
        _coreBaseSettings = coreBaseSettings;
        _serviceProvider = serviceProvider;
        Notify = notify;
        Notify.Subscribe((i) =>
        {
            Cache.Insert(TariffService.GetTariffNeedToUpdateCacheKey(i.TenantId), "update", _cacheExpiration);

            Cache.Remove(TariffService.GetTariffCacheKey(i.TenantId));
            Cache.Remove(TariffService.GetBillingUrlCacheKey(i.TenantId));
            Cache.Remove(TariffService.GetBillingPaymentCacheKey(i.TenantId)); // clear all payments
        }, CacheNotifyAction.Remove);

        Notify.Subscribe((i) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var tariffService = scope.ServiceProvider.GetService<ITariffService>();
            var tariff = tariffService.GetBillingInfoAsync(i.TenantId, i.TariffId).Result;
            if (tariff != null)
            {
                InsertToCache(i.TenantId, tariff);
            }
        }, CacheNotifyAction.Insert);
    }

    private TimeSpan GetCacheExpiration()
    {
        if (_coreBaseSettings.Standalone && _cacheExpiration < _standaloneCacheExpiration)
        {
            _cacheExpiration = _cacheExpiration.Add(TimeSpan.FromSeconds(30));
        }
        return _cacheExpiration;
    }

    public void InsertToCache(int tenantId, Tariff tariff)
    {
        Cache.Insert(TariffService.GetTariffCacheKey(tenantId), tariff, DateTime.UtcNow.Add(GetCacheExpiration()));
    }

    public void ResetCacheExpiration()
    {
        if (_coreBaseSettings.Standalone)
        {
            _cacheExpiration = _defaultCacheExpiration;
        }
    }
}

public class TariffService : ITariffService
{
    private const int DefaultTrialPeriod = 30;

    private readonly ICache _cache;
    private readonly ICacheNotify<TariffCacheItem> _notify;
    private readonly ILogger<TariffService> _logger;
    private readonly IQuotaService _quotaService;
    private readonly ITenantService _tenantService;
    private readonly int _paymentDelay;
    private readonly bool _trialEnabled;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
    private readonly IDbContextFactory<CoreDbContext> _dbContextFactory;
    private readonly TariffServiceStorage _tariffServiceStorage;
    private readonly BillingClient _billingClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantExtraConfig _tenantExtraConfig;

    //private readonly int _activeUsersMin;
    //private readonly int _activeUsersMax;

    public TariffService()
    {
    }

    public TariffService(
        IQuotaService quotaService,
        ITenantService tenantService,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        IDbContextFactory<CoreDbContext> coreDbContextManager,
        TariffServiceStorage tariffServiceStorage,
        ILogger<TariffService> logger,
        BillingClient billingClient,
        IServiceProvider serviceProvider,
        TenantExtraConfig tenantExtraConfig)
    {
        _logger = logger;
        _quotaService = quotaService;
        _tenantService = tenantService;
        _coreSettings = coreSettings;
        _tariffServiceStorage = tariffServiceStorage;
        _billingClient = billingClient;
        _serviceProvider = serviceProvider;
        _tenantExtraConfig = tenantExtraConfig;
        _coreBaseSettings = coreBaseSettings;

        var paymentConfiguration = configuration.GetSection("core:payment").Get<PaymentConfiguration>() ?? new PaymentConfiguration();
        _paymentDelay = paymentConfiguration.Delay;
        _trialEnabled = paymentConfiguration.TrialEnabled;

        _cache = _tariffServiceStorage.Cache;
        _notify = _tariffServiceStorage.Notify;
        _dbContextFactory = coreDbContextManager;
    }

    public async Task<Tariff> GetTariffAsync(int tenantId, bool withRequestToPaymentSystem = true, bool refresh = false)
    {
        //single tariff for all portals
        if (_coreBaseSettings.Standalone)
        {
            tenantId = -1;
        }

        var tariff = refresh ? null : _cache.Get<Tariff>(GetTariffCacheKey(tenantId));

        if (tariff == null)
        {
            tariff = await GetBillingInfoAsync(tenantId) ?? await CreateDefaultAsync();
            tariff = await CalculateTariffAsync(tenantId, tariff);
            _tariffServiceStorage.InsertToCache(tenantId, tariff);

            if (_billingClient.Configured && withRequestToPaymentSystem)
            {
                try
                {
                    var currentPayments = _billingClient.GetCurrentPayments(await _coreSettings.GetKeyAsync(tenantId));
                    if (currentPayments.Length == 0)
                    {
                        throw new BillingNotFoundException("Empty PaymentLast");
                    }

                    var asynctariff = await CreateDefaultAsync(true);
                    string email = null;
                    var tenantQuotas = await _quotaService.GetTenantQuotasAsync();

                    foreach (var currentPayment in currentPayments.OrderBy(r => r.EndDate))
                    {
                        var quota = tenantQuotas.SingleOrDefault(q => q.ProductId == currentPayment.ProductId.ToString());
                        if (quota == null)
                        {
                            throw new InvalidOperationException($"Quota with id {currentPayment.ProductId} not found for portal {await _coreSettings.GetKeyAsync(tenantId)}.");
                        }

                        asynctariff.Id = currentPayment.PaymentId;

                        var paymentEndDate = 9999 <= currentPayment.EndDate.Year ? DateTime.MaxValue : currentPayment.EndDate;
                        asynctariff.DueDate = DateTime.Compare(asynctariff.DueDate, paymentEndDate) < 0 ? asynctariff.DueDate : paymentEndDate;

                        asynctariff.Quotas = asynctariff.Quotas.Where(r => r.Id != quota.TenantId).ToList();
                        asynctariff.Quotas.Add(new Quota(quota.TenantId, currentPayment.Quantity));
                        email = currentPayment.PaymentEmail;
                    }

                    TenantQuota updatedQuota = null;

                    foreach (var quota in asynctariff.Quotas)
                    {
                        var tenantQuota = tenantQuotas.SingleOrDefault(q => q.TenantId == quota.Id);

                        tenantQuota *= quota.Quantity;
                        updatedQuota += tenantQuota;
                    }

                    await updatedQuota.CheckAsync(_serviceProvider);

                    if (!string.IsNullOrEmpty(email))
                    {
                        asynctariff.CustomerId = email;
                    }

                    if (await SaveBillingInfoAsync(tenantId, asynctariff))
                    {
                        asynctariff = await CalculateTariffAsync(tenantId, asynctariff);
                        tariff = asynctariff;
                    }

                    UpdateCache(tariff.Id);
                }
                catch (Exception error)
                {
                    if (error is not BillingNotFoundException)
                    {
                        LogError(error, tenantId.ToString());
                    }
                }

                if (tariff.Id == 0)
                {
                    var freeTariff = await tariff.Quotas.ToAsyncEnumerable().FirstOrDefaultAwaitAsync(async tariffRow =>
                    {
                        var q = await _quotaService.GetTenantQuotaAsync(tariffRow.Id);
                        return q == null
                            || (_trialEnabled && q.Trial)
                            || q.Free
                            || q.NonProfit
                            || q.Custom;
                    });

                    var asynctariff = await CreateDefaultAsync();

                    if (freeTariff == null)
                    {
                        asynctariff.DueDate = DateTime.Today.AddDays(-1);
                        asynctariff.State = TariffState.NotPaid;
                    }

                    if (await SaveBillingInfoAsync(tenantId, asynctariff))
                    {
                        asynctariff = await CalculateTariffAsync(tenantId, asynctariff);
                        tariff = asynctariff;
                    }

                    UpdateCache(tariff.Id);
                }
            }
            else if (_tenantExtraConfig.Enterprise && tariff.Id == 0 && tariff.LicenseDate == DateTime.MaxValue)
            {
                var defaultQuota = await _quotaService.GetTenantQuotaAsync(Tenant.DefaultTenant);

                var quota = new TenantQuota(defaultQuota)
                {
                    Name = "start_trial",
                    Trial = true,
                    TenantId = -1000
                };

                await _quotaService.SaveTenantQuotaAsync(quota);

                tariff = new Tariff
                {
                    Quotas = new List<Quota> { new Quota(quota.TenantId, 1) },
                    DueDate = DateTime.UtcNow.AddDays(DefaultTrialPeriod)
                };

                await SetTariffAsync(Tenant.DefaultTenant, tariff, new List<TenantQuota> { quota });
                UpdateCache(tariff.Id);
            }
        }
        else
        {
            tariff = await CalculateTariffAsync(tenantId, tariff);
        }

        return tariff;

        void UpdateCache(int tariffId)
        {
            _notify.Publish(new TariffCacheItem { TenantId = tenantId, TariffId = tariffId }, CacheNotifyAction.Insert);
        }
    }

    public async Task<bool> PaymentChangeAsync(int tenantId, Dictionary<string, int> quantity)
    {
        if (quantity == null || !quantity.Any()
            || !_billingClient.Configured)
        {
            return false;
        }

        var allQuotas = (await _quotaService.GetTenantQuotasAsync()).Where(q => !string.IsNullOrEmpty(q.ProductId));
        var newQuotas = quantity.Keys.Select(name => allQuotas.FirstOrDefault(q => q.Name == name));

        var tariff = await GetTariffAsync(tenantId);

        // update the quantity of present quotas
        TenantQuota updatedQuota = null;
        foreach (var tariffRow in tariff.Quotas)
        {
            var quotaId = tariffRow.Id;
            var qty = tariffRow.Quantity;

            var quota = await _quotaService.GetTenantQuotaAsync(quotaId);

            var mustUpdateQuota = newQuotas.FirstOrDefault(q => q.TenantId == quota.TenantId);
            if (mustUpdateQuota != null)
            {
                qty = quantity[mustUpdateQuota.Name];
            }

            quota *= qty;
            updatedQuota += quota;
        }

        // add new quotas
        var addedQuotas = newQuotas.Where(q => !tariff.Quotas.Any(t => t.Id == q.TenantId));
        foreach (var addedQuota in addedQuotas)
        {
            var qty = quantity[addedQuota.Name];

            var quota = addedQuota;

            quota *= qty;
            updatedQuota += quota;
        }

        await updatedQuota.CheckAsync(_serviceProvider);

        var productIds = newQuotas.Select(q => q.ProductId);

        try
        {
            var changed = _billingClient.ChangePayment(await _coreSettings.GetKeyAsync(tenantId), productIds.ToArray(), quantity.Values.ToArray());

            if (!changed)
            {
                return false;
            }

            ClearCache(tenantId);
        }
        catch (Exception error)
        {
            _logger.ErrorWithException(error);
        }

        return true;
    }


    public async Task SetTariffAsync(int tenantId, Tariff tariff, List<TenantQuota> quotas = null)
    {
        ArgumentNullException.ThrowIfNull(tariff);

        if (tariff.Quotas == null ||
            (quotas ??= await tariff.Quotas.ToAsyncEnumerable().SelectAwait(async q => await _quotaService.GetTenantQuotaAsync(q.Id)).ToListAsync()).Any(q => q == null))
        {
            return;
        }

        await SaveBillingInfoAsync(tenantId, tariff);

        if (quotas.Any(q => q.Trial))
        {
            // reset trial date
            if (tenantId != Tenant.DefaultTenant)
            {
                var tenant = await _tenantService.GetTenantAsync(tenantId);
                if (tenant != null)
                {
                    tenant.VersionChanged = DateTime.UtcNow;
                    await _tenantService.SaveTenantAsync(_coreSettings, tenant);
                }
            }
        }
    }

    internal static string GetTariffCacheKey(int tenantId)
    {
        return $"{tenantId}:tariff";
    }

    internal static string GetTariffNeedToUpdateCacheKey(int tenantId)
    {
        return $"{tenantId}:update";
    }

    internal static string GetBillingUrlCacheKey(int tenantId)
    {
        return $"{tenantId}:billing:urls";
    }

    internal static string GetBillingPaymentCacheKey(int tenantId)
    {
        return $"{tenantId}:billing:payments";
    }


    public void ClearCache(int tenantId)
    {
        _notify.Publish(new TariffCacheItem { TenantId = tenantId, TariffId = -1 }, CacheNotifyAction.Remove);
    }

    public async Task<IEnumerable<PaymentInfo>> GetPaymentsAsync(int tenantId)
    {
        var key = GetBillingPaymentCacheKey(tenantId);
        var payments = _cache.Get<List<PaymentInfo>>(key);
        if (payments == null)
        {
            payments = new List<PaymentInfo>();
            if (_billingClient.Configured)
            {
                try
                {
                    var quotas = await _quotaService.GetTenantQuotasAsync();
                    foreach (var pi in _billingClient.GetPayments(await _coreSettings.GetKeyAsync(tenantId)))
                    {
                        var quota = quotas.SingleOrDefault(q => q.ProductId == pi.ProductRef.ToString());
                        if (quota != null)
                        {
                            pi.QuotaId = quota.TenantId;
                        }
                        payments.Add(pi);
                    }
                }
                catch (Exception error)
                {
                    LogError(error, tenantId.ToString());
                }
            }

            _cache.Insert(key, payments, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        return payments;
    }

    public async Task<Uri> GetShoppingUriAsync(int tenant, string currency = null, string language = null, string customerEmail = null, Dictionary<string, int> quantity = null, string backUrl = null)
    {
        List<TenantQuota> newQuotas = new();

        if (_billingClient.Configured)
        {
            var allQuotas = (await _quotaService.GetTenantQuotasAsync()).Where(q => !string.IsNullOrEmpty(q.ProductId) && q.Visible).ToList();
            newQuotas = quantity.Select(item => allQuotas.FirstOrDefault(q => q.Name == item.Key)).ToList();

            TenantQuota updatedQuota = null;
            foreach (var addedQuota in newQuotas)
            {
                var qty = quantity[addedQuota.Name];

                var quota = addedQuota;

                quota *= qty;
                updatedQuota += quota;
            }

            await updatedQuota.CheckAsync(_serviceProvider);
        }

        var hasQuantity = quantity != null && quantity.Any();
        var key = "shopingurl_" + (hasQuantity ? string.Join('_', quantity.Keys.ToArray()) : "all");
        var url = _cache.Get<string>(key);
        if (url == null)
        {
            url = string.Empty;
            if (_billingClient.Configured)
            {
                var productIds = newQuotas.Select(q => q.ProductId);

                try
                {
                    url =
                        _billingClient.GetPaymentUrl(
                            "__Tenant__",
                            productIds.ToArray(),
                            null,
                            null,
                            !string.IsNullOrEmpty(currency) ? "__Currency__" : null,
                            !string.IsNullOrEmpty(language) ? "__Language__" : null,
                            !string.IsNullOrEmpty(customerEmail) ? "__CustomerEmail__" : null,
                            hasQuantity ? "__Quantity__" : null,
                            !string.IsNullOrEmpty(backUrl) ? "__BackUrl__" : null
                            );
                }
                catch (Exception error)
                {
                    _logger.ErrorWithException(error);
                }
            }
            _cache.Insert(key, url, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        _tariffServiceStorage.ResetCacheExpiration();

        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        var result = new Uri(url.ToString()
                               .Replace("__Tenant__", HttpUtility.UrlEncode(await _coreSettings.GetKeyAsync(tenant)))
                               .Replace("__Currency__", HttpUtility.UrlEncode(currency ?? ""))
                               .Replace("__Language__", HttpUtility.UrlEncode((language ?? "").ToLower()))
                               .Replace("__CustomerEmail__", HttpUtility.UrlEncode(customerEmail ?? ""))
                               .Replace("__Quantity__", hasQuantity ? string.Join(',', quantity.Values) : "")
                               .Replace("__BackUrl__", HttpUtility.UrlEncode(backUrl ?? "")));
        return result;
    }

    public async Task<Uri> GetShoppingUriAsync(int? tenant, int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null, string quantity = null)
    {
        var quota = await _quotaService.GetTenantQuotaAsync(quotaId);
        if (quota == null)
        {
            return null;
        }

        var key = tenant.HasValue
                      ? GetBillingUrlCacheKey(tenant.Value)
                      : string.Format($"notenant{(!string.IsNullOrEmpty(affiliateId) ? "_" + affiliateId : "")}");
        key += quota.Visible ? "" : "0";
        if (_cache.Get<Dictionary<string, Uri>>(key) is not IDictionary<string, Uri> urls)
        {
            urls = new Dictionary<string, Uri>();
            if (_billingClient.Configured)
            {
                try
                {
                    var products = (await _quotaService.GetTenantQuotasAsync())
                                               .Where(q => !string.IsNullOrEmpty(q.ProductId) && q.Visible == quota.Visible)
                                               .Select(q => q.ProductId)
                                               .ToArray();

                    urls =
                        _billingClient.GetPaymentUrls(
                            tenant.HasValue ? await _coreSettings.GetKeyAsync(tenant.Value) : null,
                            products,
                            tenant.HasValue ? await _coreSettings.GetAffiliateIdAsync(tenant.Value) : affiliateId,
                            tenant.HasValue ? await _coreSettings.GetCampaignAsync(tenant.Value) : null,
                            !string.IsNullOrEmpty(currency) ? "__Currency__" : null,
                            !string.IsNullOrEmpty(language) ? "__Language__" : null,
                            !string.IsNullOrEmpty(customerId) ? "__CustomerID__" : null,
                            !string.IsNullOrEmpty(quantity) ? "__Quantity__" : null
                            );
                }
                catch (Exception error)
                {
                    _logger.ErrorGetShoppingUri(error);
                }
            }
            _cache.Insert(key, urls, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        _tariffServiceStorage.ResetCacheExpiration();

        if (!string.IsNullOrEmpty(quota.ProductId) && urls.TryGetValue(quota.ProductId, out var url))
        {
            if (url == null)
            {
                return null;
            }

            url = new Uri(url.ToString()
                                   .Replace("__Currency__", HttpUtility.UrlEncode(currency ?? ""))
                                   .Replace("__Language__", HttpUtility.UrlEncode((language ?? "").ToLower()))
                                   .Replace("__CustomerID__", HttpUtility.UrlEncode(customerId ?? ""))
                                   .Replace("__Quantity__", HttpUtility.UrlEncode(quantity ?? "")));
            return url;
        }
        return null;
    }

    public Uri GetShoppingUri(string[] productIds, string affiliateId = null, string currency = null, string language = null, string customerId = null, string quantity = null)
    {
        var key = "shopingurl" + string.Join("_", productIds) + (!string.IsNullOrEmpty(affiliateId) ? "_" + affiliateId : "");
        var url = _cache.Get<string>(key);
        if (url == null)
        {
            url = string.Empty;
            if (_billingClient.Configured)
            {
                try
                {
                    url =
                        _billingClient.GetPaymentUrl(
                            null,
                            productIds,
                            affiliateId,
                            null,
                            !string.IsNullOrEmpty(currency) ? "__Currency__" : null,
                            !string.IsNullOrEmpty(language) ? "__Language__" : null,
                            !string.IsNullOrEmpty(customerId) ? "__CustomerID__" : null,
                            !string.IsNullOrEmpty(quantity) ? "__Quantity__" : null
                            );
                }
                catch (Exception error)
                {
                    _logger.ErrorWithException(error);
                }
            }
            _cache.Insert(key, url, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        _tariffServiceStorage.ResetCacheExpiration();

        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        var result = new Uri(url.ToString()
                               .Replace("__Currency__", HttpUtility.UrlEncode(currency ?? ""))
                               .Replace("__Language__", HttpUtility.UrlEncode((language ?? "").ToLower()))
                               .Replace("__CustomerID__", HttpUtility.UrlEncode(customerId ?? ""))
                               .Replace("__Quantity__", HttpUtility.UrlEncode(quantity ?? "")));
        return result;
    }

    public IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(params string[] productIds)
    {
        ArgumentNullException.ThrowIfNull(productIds);

        try
        {
            var key = "biling-prices" + string.Join(",", productIds);
            var result = _cache.Get<IDictionary<string, Dictionary<string, decimal>>>(key);
            if (result == null)
            {
                result = _billingClient.GetProductPriceInfo(productIds);
                _cache.Insert(key, result, DateTime.Now.AddHours(1));
            }

            return result;
        }
        catch (Exception error)
        {
            LogError(error);

            return productIds
                .Select(p => new { ProductId = p, Prices = new Dictionary<string, decimal>() })
                .ToDictionary(e => e.ProductId, e => e.Prices);
        }
    }

    public async Task<Uri> GetAccountLinkAsync(int tenant, string backUrl)
    {
        var key = "accountlink_" + tenant;
        var url = _cache.Get<string>(key);
        if (url == null)
        {
            if (_billingClient.Configured)
            {
                try
                {
                    url = _billingClient.GetAccountLink(await _coreSettings.GetKeyAsync(tenant), backUrl);
                    _cache.Insert(key, url, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
                }
                catch (Exception error)
                {
                    LogError(error);
                }
            }
        }
        if (!string.IsNullOrEmpty(url))
        {
            return new Uri(url);
        }

        return null;
    }


    public async Task<Tariff> GetBillingInfoAsync(int? tenant = null, int? id = null)
    {
        await using var coreDbContext = _dbContextFactory.CreateDbContext();

        var r = await Queries.TariffAsync(coreDbContext, tenant, id);

        if (r == null)
        {
            return null;
        }

        var tariff = await CreateDefaultAsync(true);
        tariff.Id = r.Id;
        tariff.DueDate = r.Stamp.Year < 9999 ? r.Stamp : DateTime.MaxValue;
        tariff.CustomerId = r.CustomerId;
        tariff.Quotas = await Queries.QuotasAsync(coreDbContext, r.TenantId, r.Id).ToListAsync();

        return tariff;
    }

    private async Task<bool> SaveBillingInfoAsync(int tenant, Tariff tariffInfo)
    {
        var inserted = false;
        var currentTariff = await GetBillingInfoAsync(tenant);
        if (!tariffInfo.EqualsByParams(currentTariff))
        {
            try
            {
                await using var dbContext = _dbContextFactory.CreateDbContext();

                var stamp = tariffInfo.DueDate;
                if (stamp.Equals(DateTime.MaxValue))
                {
                    stamp = stamp.Date.Add(new TimeSpan(tariffInfo.DueDate.Hour, tariffInfo.DueDate.Minute, tariffInfo.DueDate.Second));
                }

                var efTariff = new DbTariff
                {
                    Id = tariffInfo.Id,
                    TenantId = tenant,
                    Stamp = stamp,
                    CustomerId = tariffInfo.CustomerId,
                    CreateOn = DateTime.UtcNow
                };

                if (efTariff.Id == default)
                {
                    efTariff.Id = (-tenant);
                    tariffInfo.Id = efTariff.Id;
                }

                if (efTariff.CustomerId == default)
                {
                    efTariff.CustomerId = "";
                }

                efTariff = await dbContext.AddOrUpdateAsync(q => q.Tariffs, efTariff);

                foreach (var q in tariffInfo.Quotas)
                {
                    await dbContext.AddOrUpdateAsync(q => q.TariffRows, new DbTariffRow
                    {
                        TariffId = efTariff.Id,
                        Quota = q.Id,
                        Quantity = q.Quantity,
                        TenantId = tenant
                    });
                }

                await dbContext.SaveChangesAsync();

                inserted = true;
            }
            catch (DbUpdateException)
            {

            }
        }

        if (inserted)
        {
            if (tenant != Tenant.DefaultTenant)
            {
                var t = await _tenantService.GetTenantAsync(tenant);
                if (t != null)
                {
                    // update tenant.LastModified to flush cache in documents
                    await _tenantService.SaveTenantAsync(_coreSettings, t);
                }
            }

            ClearCache(tenant);

            await NotifyWebSocketAsync(currentTariff, tariffInfo);
        }

        return inserted;
    }

    public async Task DeleteDefaultBillingInfoAsync()
    {
        const int tenant = Tenant.DefaultTenant;

        await using var coreDbContext = _dbContextFactory.CreateDbContext();
        await Queries.UpdateTariffs(coreDbContext, tenant);

        ClearCache(tenant);
    }


    private async Task<Tariff> CalculateTariffAsync(int tenantId, Tariff tariff)
    {
        tariff.State = TariffState.Paid;

        if (tariff.Quotas.Count == 0)
        {
            await AddDefaultQuotaAsync(tariff);
        }

        var delay = 0;
        var setDelay = true;

        if (_trialEnabled)
        {
            var trial = await tariff.Quotas.ToAsyncEnumerable().AnyAwaitAsync(async q => (await _quotaService.GetTenantQuotaAsync(q.Id)).Trial);
            if (trial)
            {
                setDelay = false;
                tariff.State = TariffState.Trial;
                if (tariff.DueDate == DateTime.MinValue || tariff.DueDate == DateTime.MaxValue)
                {
                    var tenant = await _tenantService.GetTenantAsync(tenantId);
                    if (tenant != null)
                    {
                        var fromDate = tenant.CreationDateTime < tenant.VersionChanged ? tenant.VersionChanged : tenant.CreationDateTime;
                        var trialPeriod = await GetPeriodAsync("TrialPeriod", DefaultTrialPeriod);
                        if (fromDate == DateTime.MinValue)
                        {
                            fromDate = DateTime.UtcNow.Date;
                        }

                        tariff.DueDate = trialPeriod != default ? fromDate.Date.AddDays(trialPeriod) : DateTime.MaxValue;
                    }
                    else
                    {
                        tariff.DueDate = DateTime.MaxValue;
                    }
                }
            }
        }

        if (setDelay)
        {
            delay = _paymentDelay;
        }

        if (tariff.DueDate != DateTime.MinValue && tariff.DueDate.Date < DateTime.UtcNow.Date && delay > 0)
        {
            tariff.State = TariffState.Delay;
            tariff.DelayDueDate = tariff.DueDate.Date.AddDays(delay);
        }

        if (tariff.DueDate == DateTime.MinValue ||
            tariff.DueDate != DateTime.MaxValue && tariff.DueDate.Date < DateTime.UtcNow.Date.AddDays(-delay))
        {
            tariff.State = TariffState.NotPaid;

            if (_coreBaseSettings.Standalone)
            {
                TenantQuota updatedQuota = null;

                var tenantQuotas = await _quotaService.GetTenantQuotasAsync();

                foreach (var quota in tariff.Quotas)
                {
                    var tenantQuota = tenantQuotas.SingleOrDefault(q => q.TenantId == quota.Id);

                    if (tenantQuota != null)
                    {
                        tenantQuota *= quota.Quantity;
                        updatedQuota += tenantQuota;
                    }
                }

                var defaultQuota = await _quotaService.GetTenantQuotaAsync(Tenant.DefaultTenant);
                defaultQuota.Name = "overdue";
                defaultQuota.Features = updatedQuota.Features;

                await _quotaService.SaveTenantQuotaAsync(defaultQuota);

                var unlimTariff = await CreateDefaultAsync();
                unlimTariff.LicenseDate = tariff.DueDate;
                unlimTariff.DueDate = tariff.DueDate;
                unlimTariff.Quotas = new List<Quota>()
                {
                    new Quota(defaultQuota.TenantId, 1)
                };

                tariff = unlimTariff;
            }
        }

        return tariff;
    }

    private async Task<int> GetPeriodAsync(string key, int defaultValue)
    {
        var settings = await _tenantService.GetTenantSettingsAsync(Tenant.DefaultTenant, key);

        return settings != null ? Convert.ToInt32(Encoding.UTF8.GetString(settings)) : defaultValue;
    }

    private async Task<Tariff> CreateDefaultAsync(bool empty = false)
    {
        var result = new Tariff
        {
            State = TariffState.Paid,
            DueDate = DateTime.MaxValue,
            DelayDueDate = DateTime.MaxValue,
            LicenseDate = DateTime.MaxValue,
            CustomerId = "",
            Quotas = new List<Quota>()
        };

        if (!empty)
        {
            await AddDefaultQuotaAsync(result);
        }

        return result;
    }

    private async Task AddDefaultQuotaAsync(Tariff tariff)
    {
        var allQuotas = await _quotaService.GetTenantQuotasAsync();
        TenantQuota toAdd = null;
        if (_trialEnabled)
        {
            toAdd = allQuotas.FirstOrDefault(r => r.Trial && !r.Custom);
        }
        else
        {
            toAdd = allQuotas.FirstOrDefault(r => _coreBaseSettings.Standalone || r.Free && !r.Custom);
        }

        if (toAdd != null)
        {
            tariff.Quotas.Add(new Quota(toAdd.TenantId, 1));
        }
    }

    private void LogError(Exception error, string tenantId = null)
    {
        if (error is BillingNotFoundException)
        {
            _logger.DebugPaymentTenant(tenantId, error.Message);
        }
        else if (error is BillingNotConfiguredException)
        {
            _logger.DebugBillingTenant(tenantId, error.Message);
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.ErrorBillingWithException(tenantId, error);
            }
            else
            {
                _logger.ErrorBilling(tenantId, error.Message);
            }
        }
    }

    private async Task NotifyWebSocketAsync(Tariff currenTariff, Tariff newTariff)
    {
        var quotaSocketManager = _serviceProvider.GetRequiredService<QuotaSocketManager>();

        var updatedQuota = await GetTenantQuotaFromTariffAsync(newTariff);

        var maxTotalSize = updatedQuota.MaxTotalSize;
        var maxTotalSizeFeatureName = updatedQuota.GetFeature<MaxTotalSizeFeature>().Name;

        _ = quotaSocketManager.ChangeQuotaFeatureValue(maxTotalSizeFeatureName, maxTotalSize);

        var maxPaidUsers = updatedQuota.CountRoomAdmin;
        var maxPaidUsersFeatureName = updatedQuota.GetFeature<CountPaidUserFeature>().Name;

        _ = quotaSocketManager.ChangeQuotaFeatureValue(maxPaidUsersFeatureName, maxPaidUsers);

        var maxRoomCount = updatedQuota.CountRoom == int.MaxValue ? -1 : updatedQuota.CountRoom;
        var maxRoomCountFeatureName = updatedQuota.GetFeature<CountRoomFeature>().Name;

        _ = quotaSocketManager.ChangeQuotaFeatureValue(maxRoomCountFeatureName, maxRoomCount);

        if (currenTariff != null)
        {
            var currentQuota = await GetTenantQuotaFromTariffAsync(currenTariff);

            var free = updatedQuota.Free;
            if (currentQuota.Free != free)
            {
                var freeFeatureName = updatedQuota.GetFeature<FreeFeature>().Name;

                _ = quotaSocketManager.ChangeQuotaFeatureValue(freeFeatureName, free);
            }
        }
    }

    private async Task<TenantQuota> GetTenantQuotaFromTariffAsync(Tariff tariff)
    {
        TenantQuota result = null;
        foreach (var tariffRow in tariff.Quotas)
        {
            var qty = tariffRow.Quantity;

            var quota = await _quotaService.GetTenantQuotaAsync(tariffRow.Id);

            quota *= qty;
            result += quota;
        }

        return result;
    }

    public int GetPaymentDelay()
    {
        return _paymentDelay;
    }

    public bool IsConfigured()
    {
        return _billingClient.Configured;
    }
}

static file class Queries
{
    public static readonly Func<CoreDbContext, int?, int?, Task<DbTariff>> TariffAsync =
        EF.CompileAsyncQuery(
            (CoreDbContext ctx, int? tenantId, int? id) =>
                ctx.Tariffs
                    .Where(r => !tenantId.HasValue || r.TenantId == tenantId)
                    .Where(r => !id.HasValue || r.Id == id.Value)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault());

    public static readonly Func<CoreDbContext, int, int, IAsyncEnumerable<Quota>> QuotasAsync =
        EF.CompileAsyncQuery(
            (CoreDbContext ctx, int tenantId, int id) =>
                ctx.TariffRows
                    .Where(r => r.TariffId == id && r.TenantId == tenantId)
                    .Select(r => new Quota(r.Quota, r.Quantity)));

    public static readonly Func<CoreDbContext, int, Task<int>> UpdateTariffs =
        EF.CompileAsyncQuery(
            (CoreDbContext ctx, int tenantId) =>
                ctx.Tariffs.Where(r => r.TenantId == tenantId).ExecuteUpdate(t =>
                    t.SetProperty(p => p.TenantId, -2)
                        .SetProperty(p => p.CreateOn, DateTime.UtcNow)));
}
