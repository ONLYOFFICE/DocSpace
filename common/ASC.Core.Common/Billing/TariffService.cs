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
            var tariff = tariffService.GetBillingInfo(i.TenantId, i.TariffId);
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

    public Tariff GetTariff(int tenantId, bool withRequestToPaymentSystem = true, bool refresh = false)
    {
        //single tariff for all portals
        if (_coreBaseSettings.Standalone)
        {
            tenantId = -1;
        }

        var tariff = refresh ? null : _cache.Get<Tariff>(GetTariffCacheKey(tenantId));

        if (tariff == null)
        {
            tariff = GetBillingInfo(tenantId) ?? CreateDefault();
            tariff = CalculateTariff(tenantId, tariff);
            _tariffServiceStorage.InsertToCache(tenantId, tariff);

            if (_billingClient.Configured && withRequestToPaymentSystem)
            {
                try
                {
                    var currentPayments = _billingClient.GetCurrentPayments(GetPortalId(tenantId));
                    if (currentPayments.Length == 0)
                    {
                        throw new BillingNotFoundException("Empty PaymentLast");
                    }

                    var asynctariff = CreateDefault(true);
                    string email = null;
                    var tenantQuotas = _quotaService.GetTenantQuotas();

                    foreach (var currentPayment in currentPayments.OrderBy(r => r.EndDate))
                    {
                        var quota = tenantQuotas.SingleOrDefault(q => q.ProductId == currentPayment.ProductId.ToString());
                        if (quota == null)
                        {
                            throw new InvalidOperationException($"Quota with id {currentPayment.ProductId} not found for portal {GetPortalId(tenantId)}.");
                        }

                        asynctariff.Id = currentPayment.PaymentId;

                        var paymentEndDate = 9999 <= currentPayment.EndDate.Year ? DateTime.MaxValue : currentPayment.EndDate;
                        asynctariff.DueDate = DateTime.Compare(asynctariff.DueDate, paymentEndDate) < 0 ? asynctariff.DueDate : paymentEndDate;

                        asynctariff.Quotas = asynctariff.Quotas.Where(r => r.Id != quota.Tenant).ToList();
                        asynctariff.Quotas.Add(new Quota(quota.Tenant, currentPayment.Quantity));
                        email = currentPayment.PaymentEmail;
                    }

                    TenantQuota updatedQuota = null;

                    foreach (var quota in asynctariff.Quotas)
                    {
                        var tenantQuota = tenantQuotas.SingleOrDefault(q => q.Tenant == quota.Id);

                        tenantQuota *= quota.Quantity;
                        updatedQuota += tenantQuota;
                    }

                    updatedQuota.Check(_serviceProvider).Wait();

                    if (!string.IsNullOrEmpty(email))
                    {
                        asynctariff.CustomerId = email;
                    }

                    if (SaveBillingInfo(tenantId, asynctariff))
                    {
                        asynctariff = CalculateTariff(tenantId, asynctariff);
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
                    var freeTariff = tariff.Quotas.FirstOrDefault(tariffRow =>
                    {
                        var q = _quotaService.GetTenantQuota(tariffRow.Id);
                        return q == null
                            || (_trialEnabled && q.Trial)
                            || q.Free
                            || q.NonProfit
                            || q.Custom;
                    });

                    var asynctariff = CreateDefault();

                    if (freeTariff == null)
                    {
                        asynctariff.DueDate = DateTime.Today.AddDays(-1);
                        asynctariff.State = TariffState.NotPaid;
                    }

                    if (SaveBillingInfo(tenantId, asynctariff))
                    {
                        asynctariff = CalculateTariff(tenantId, asynctariff);
                        tariff = asynctariff;
                    }

                    UpdateCache(tariff.Id);
                }
            }
            else if (_tenantExtraConfig.Enterprise && tariff.Id == 0 && tariff.LicenseDate == DateTime.MaxValue)
            {
                var defaultQuota = _quotaService.GetTenantQuota(Tenant.DefaultTenant);

                var quota = new TenantQuota(defaultQuota)
                {
                    Name = "start_trial",
                    Trial = true,
                    Tenant = -1000
                };

                _quotaService.SaveTenantQuota(quota);

                tariff = new Tariff
                {
                    Quotas = new List<Quota> { new Quota(quota.Tenant, 1) },
                    DueDate = DateTime.UtcNow.AddDays(DefaultTrialPeriod)
                };

                SetTariff(-1, tariff, new List<TenantQuota> { quota });
                UpdateCache(tariff.Id);
            }
        }
        else
        {
            tariff = CalculateTariff(tenantId, tariff);
        }

        return tariff;

        void UpdateCache(int tariffId)
        {
            _notify.Publish(new TariffCacheItem { TenantId = tenantId, TariffId = tariffId }, CacheNotifyAction.Insert);
        }
    }

    public async Task<bool> PaymentChange(int tenantId, Dictionary<string, int> quantity)
    {
        if (quantity == null || !quantity.Any()
            || !_billingClient.Configured)
        {
            return false;
        }

        var allQuotas = _quotaService.GetTenantQuotas().Where(q => !string.IsNullOrEmpty(q.ProductId));
        var newQuotas = quantity.Keys.Select(name => allQuotas.FirstOrDefault(q => q.Name == name));

        var tariff = GetTariff(tenantId);

        // update the quantity of present quotas
        TenantQuota updatedQuota = null;
        foreach (var tariffRow in tariff.Quotas)
        {
            var quotaId = tariffRow.Id;
            var qty = tariffRow.Quantity;

            var quota = _quotaService.GetTenantQuota(quotaId);

            var mustUpdateQuota = newQuotas.FirstOrDefault(q => q.Tenant == quota.Tenant);
            if (mustUpdateQuota != null)
            {
                qty = quantity[mustUpdateQuota.Name];
            }

            quota *= qty;
            updatedQuota += quota;
        }

        // add new quotas
        var addedQuotas = newQuotas.Where(q => !tariff.Quotas.Any(t => t.Id == q.Tenant));
        foreach (var addedQuota in addedQuotas)
        {
            var qty = quantity[addedQuota.Name];

            var quota = addedQuota;

            quota *= qty;
            updatedQuota += quota;
        }

        await updatedQuota.Check(_serviceProvider);

        var productIds = newQuotas.Select(q => q.ProductId);

        try
        {
            var changed = _billingClient.ChangePayment(GetPortalId(tenantId), productIds.ToArray(), quantity.Values.ToArray());

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


    public void SetTariff(int tenantId, Tariff tariff, List<TenantQuota> quotas = null)
    {
        ArgumentNullException.ThrowIfNull(tariff);

        if (tariff.Quotas == null ||
            (quotas ??= tariff.Quotas.Select(q => _quotaService.GetTenantQuota(q.Id)).ToList()).Any(q => q == null))
        {
            return;
        }

        SaveBillingInfo(tenantId, tariff);

        if (quotas.Any(q => q.Trial))
        {
            // reset trial date
            var tenant = _tenantService.GetTenant(tenantId);
            if (tenant != null)
            {
                tenant.VersionChanged = DateTime.UtcNow;
                _tenantService.SaveTenant(_coreSettings, tenant);
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

    public IEnumerable<PaymentInfo> GetPayments(int tenantId)
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
                    var quotas = _quotaService.GetTenantQuotas();
                    foreach (var pi in _billingClient.GetPayments(GetPortalId(tenantId)))
                    {
                        var quota = quotas.SingleOrDefault(q => q.ProductId == pi.ProductRef.ToString());
                        if (quota != null)
                        {
                            pi.QuotaId = quota.Tenant;
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

    public async Task<Uri> GetShoppingUri(int tenant, string currency = null, string language = null, string customerEmail = null, Dictionary<string, int> quantity = null, string backUrl = null)
    {
        List<TenantQuota> newQuotas = new();

        if (_billingClient.Configured)
        {
            var allQuotas = _quotaService.GetTenantQuotas().Where(q => !string.IsNullOrEmpty(q.ProductId) && q.Visible).ToList();
            newQuotas = quantity.Select(item => allQuotas.FirstOrDefault(q => q.Name == item.Key)).ToList();

            TenantQuota updatedQuota = null;
            foreach (var addedQuota in newQuotas)
            {
                var qty = quantity[addedQuota.Name];

                var quota = addedQuota;

                quota *= qty;
                updatedQuota += quota;
            }

            await updatedQuota.Check(_serviceProvider);
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
                               .Replace("__Tenant__", HttpUtility.UrlEncode(GetPortalId(tenant)))
                               .Replace("__Currency__", HttpUtility.UrlEncode(currency ?? ""))
                               .Replace("__Language__", HttpUtility.UrlEncode((language ?? "").ToLower()))
                               .Replace("__CustomerEmail__", HttpUtility.UrlEncode(customerEmail ?? ""))
                               .Replace("__Quantity__", hasQuantity ? string.Join(',', quantity.Values) : "")
                               .Replace("__BackUrl__", HttpUtility.UrlEncode(backUrl ?? "")));
        return result;
    }

    public Uri GetShoppingUri(int? tenant, int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null, string quantity = null)
    {
        var quota = _quotaService.GetTenantQuota(quotaId);
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
                    var products = _quotaService.GetTenantQuotas()
                                               .Where(q => !string.IsNullOrEmpty(q.ProductId) && q.Visible == quota.Visible)
                                               .Select(q => q.ProductId)
                                               .ToArray();

                    urls =
                        _billingClient.GetPaymentUrls(
                            tenant.HasValue ? GetPortalId(tenant.Value) : null,
                            products,
                            tenant.HasValue ? GetAffiliateId(tenant.Value) : affiliateId,
                            tenant.HasValue ? GetCampaign(tenant.Value) : null,
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

    public Uri GetAccountLink(int tenant, string backUrl)
    {
        var key = "accountlink_" + tenant;
        var url = _cache.Get<string>(key);
        if (url == null)
        {
            if (_billingClient.Configured)
            {
                try
                {
                    url = _billingClient.GetAccountLink(GetPortalId(tenant), backUrl);
                }
                catch (Exception error)
                {
                    LogError(error);
                }
            }
            _cache.Insert(key, url, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }
        if (!string.IsNullOrEmpty(url))
        {
            return new Uri(url);
        }

        return null;
    }


    public Tariff GetBillingInfo(int? tenant = null, int? id = null)
    {
        using var coreDbContext = _dbContextFactory.CreateDbContext();
        var q = coreDbContext.Tariffs.AsQueryable();

        if (tenant.HasValue)
        {
            q = q.Where(r => r.Tenant == tenant.Value);
        }

        if (id.HasValue)
        {
            q = q.Where(r => r.Id == id.Value);
        }

        var r = q.OrderByDescending(r => r.Id).FirstOrDefault();

        if (r == null)
        {
            return null;
        }

        var tariff = CreateDefault(true);
        tariff.Id = r.Id;
        tariff.DueDate = r.Stamp.Year < 9999 ? r.Stamp : DateTime.MaxValue;
        tariff.CustomerId = r.CustomerId;

        var tariffRows = coreDbContext.TariffRows
            .Where(row => row.TariffId == r.Id && row.Tenant == tenant);

        tariff.Quotas = tariffRows.Select(r => new Quota(r.Quota, r.Quantity)).ToList();

        return tariff;
    }

    private bool SaveBillingInfo(int tenant, Tariff tariffInfo)
    {
        var inserted = false;
        var currentTariff = GetBillingInfo(tenant);
        if (!tariffInfo.EqualsByParams(currentTariff))
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var strategy = dbContext.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using var dbContext = _dbContextFactory.CreateDbContext();
                    using var tx = dbContext.Database.BeginTransaction();

                    var stamp = tariffInfo.DueDate;
                    if (stamp.Equals(DateTime.MaxValue))
                    {
                        stamp = stamp.Date.Add(new TimeSpan(tariffInfo.DueDate.Hour, tariffInfo.DueDate.Minute, tariffInfo.DueDate.Second));
                    }

                    var efTariff = new DbTariff
                    {
                        Id = tariffInfo.Id,
                        Tenant = tenant,
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

                    efTariff = dbContext.AddOrUpdate(dbContext.Tariffs, efTariff);
                    dbContext.SaveChanges();

                    foreach (var q in tariffInfo.Quotas)
                    {
                        dbContext.AddOrUpdate(dbContext.TariffRows, new DbTariffRow
                        {
                            TariffId = efTariff.Id,
                            Quota = q.Id,
                            Quantity = q.Quantity,
                            Tenant = tenant
                        });
                    }

                    dbContext.SaveChanges();

                    inserted = true;

                    tx.Commit();
                });
            }
            catch (DbUpdateException)
            {

            }
        }

        if (inserted)
        {
            var t = _tenantService.GetTenant(tenant);
            if (t != null)
            {
                // update tenant.LastModified to flush cache in documents
                _tenantService.SaveTenant(_coreSettings, t);
            }
            ClearCache(tenant);

            NotifyWebSocket(currentTariff, tariffInfo);
        }

        return inserted;
    }

    public void DeleteDefaultBillingInfo()
    {
        const int tenant = Tenant.DefaultTenant;

        using var coreDbContext = _dbContextFactory.CreateDbContext();
        var tariffs = coreDbContext.Tariffs.Where(r => r.Tenant == tenant).ToList();

        foreach (var t in tariffs)
        {
            t.Tenant = -2;
            t.CreateOn = DateTime.UtcNow;
        }

        coreDbContext.SaveChanges();

        ClearCache(tenant);
    }


    private Tariff CalculateTariff(int tenantId, Tariff tariff)
    {
        tariff.State = TariffState.Paid;

        if (tariff.Quotas.Count == 0)
        {
            AddDefaultQuota(tariff);
        }

        var delay = 0;
        var setDelay = true;

        if (_trialEnabled)
        {
            var trial = tariff.Quotas.Exists(q => _quotaService.GetTenantQuota(q.Id).Trial);
            if (trial)
            {
                setDelay = false;
                tariff.State = TariffState.Trial;
                if (tariff.DueDate == DateTime.MinValue || tariff.DueDate == DateTime.MaxValue)
                {
                    var tenant = _tenantService.GetTenant(tenantId);
                    if (tenant != null)
                    {
                        var fromDate = tenant.CreationDateTime < tenant.VersionChanged ? tenant.VersionChanged : tenant.CreationDateTime;
                        var trialPeriod = GetPeriod("TrialPeriod", DefaultTrialPeriod);
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

                var tenantQuotas = _quotaService.GetTenantQuotas();

                foreach (var quota in tariff.Quotas)
                {
                    var tenantQuota = tenantQuotas.SingleOrDefault(q => q.Tenant == quota.Id);

                    if (tenantQuota != null)
                    {
                        tenantQuota *= quota.Quantity;
                        updatedQuota += tenantQuota;
                    }
                }

                var defaultQuota = _quotaService.GetTenantQuota(Tenant.DefaultTenant);
                defaultQuota.Name = "overdue";
                defaultQuota.Features = updatedQuota.Features;

                _quotaService.SaveTenantQuota(defaultQuota);

                var unlimTariff = CreateDefault();
                unlimTariff.LicenseDate = tariff.DueDate;
                unlimTariff.DueDate = tariff.DueDate;
                unlimTariff.Quotas = new List<Quota>()
                {
                    new Quota(defaultQuota.Tenant, 1)
                };

                tariff = unlimTariff;
            }
        }

        return tariff;
    }

    private int GetPeriod(string key, int defaultValue)
    {
        var settings = _tenantService.GetTenantSettings(Tenant.DefaultTenant, key);

        return settings != null ? Convert.ToInt32(Encoding.UTF8.GetString(settings)) : defaultValue;
    }

    private string GetPortalId(int tenant)
    {
        return _coreSettings.GetKey(tenant);
    }

    private string GetAffiliateId(int tenant)
    {
        return _coreSettings.GetAffiliateId(tenant);
    }

    private string GetCampaign(int tenant)
    {
        return _coreSettings.GetCampaign(tenant);
    }

    private Tariff CreateDefault(bool empty = false)
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
            AddDefaultQuota(result);
        }

        return result;
    }

    private void AddDefaultQuota(Tariff tariff)
    {
        var allQuotas = _quotaService.GetTenantQuotas();
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
            tariff.Quotas.Add(new Quota(toAdd.Tenant, 1));
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

    private void NotifyWebSocket(Tariff currenTariff, Tariff newTariff)
    {
        var quotaSocketManager = _serviceProvider.GetRequiredService<QuotaSocketManager>();

        var updatedQuota = GetTenantQuotaFromTariff(newTariff);

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
            var currentQuota = GetTenantQuotaFromTariff(currenTariff);

            var free = updatedQuota.Free;
            if (currentQuota.Free != free)
            {
                var freeFeatureName = updatedQuota.GetFeature<FreeFeature>().Name;

                _ = quotaSocketManager.ChangeQuotaFeatureValue(freeFeatureName, free);
            }
        }
    }

    private TenantQuota GetTenantQuotaFromTariff(Tariff tariff)
    {
        TenantQuota result = null;
        foreach (var tariffRow in tariff.Quotas)
        {
            var qty = tariffRow.Quantity;

            var quota = _quotaService.GetTenantQuota(tariffRow.Id);

            quota *= qty;
            result += quota;
        }

        return result;
    }

    public int GetPaymentDelay()
    {
        return _paymentDelay;
    }
}
