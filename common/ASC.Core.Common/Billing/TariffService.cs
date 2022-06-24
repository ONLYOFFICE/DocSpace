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
public class TariffServiceStorage
{
    public ICache Cache { get; }
    internal readonly ICacheNotify<TariffCacheItem> Notify;

    public TariffServiceStorage(ICacheNotify<TariffCacheItem> notify, ICache cache)
    {
        Cache = cache;
        Notify = notify;
        Notify.Subscribe((i) =>
        {
            Cache.Remove(TariffService.GetTariffCacheKey(i.TenantId));
            Cache.Remove(TariffService.GetBillingUrlCacheKey(i.TenantId));
            Cache.Remove(TariffService.GetBillingPaymentCacheKey(i.TenantId)); // clear all payments
        }, CacheNotifyAction.Remove);

        //TODO: Change code of WCF -> not supported in .NET standard/.Net Core
        /*try
        {
            var section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            if (section != null)
            {
                billingConfigured = section.Endpoints.Cast<ChannelEndpointElement>()
                    .Any(e => e.Contract == typeof(IService).FullName);
            }
        }
        catch (Exception err)
        {
            log.Error(err);
        }*/
    }
}

[Scope]
class ConfigureTariffService : IConfigureNamedOptions<TariffService>
{
    private readonly IOptionsSnapshot<CachedQuotaService> _quotaService;
    private readonly IOptionsSnapshot<CachedTenantService> _tenantService;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
    private readonly IConfiguration _configuration;
    private readonly DbContextManager<CoreDbContext> _coreDbContextManager;
    private readonly TariffServiceStorage _tariffServiceStorage;
    private readonly ILogger<TariffService> _logger;

    public ConfigureTariffService(
        IOptionsSnapshot<CachedQuotaService> quotaService,
        IOptionsSnapshot<CachedTenantService> tenantService,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        DbContextManager<CoreDbContext> coreDbContextManager,
        TariffServiceStorage tariffServiceStorage,
        ILogger<TariffService> options)
    {
        _quotaService = quotaService;
        _tenantService = tenantService;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
        _configuration = configuration;
        _coreDbContextManager = coreDbContextManager;
        _tariffServiceStorage = tariffServiceStorage;
        _logger = options;
    }


    public void Configure(string name, TariffService options)
    {
        Configure(options);
        options.QuotaService = _quotaService.Get(name);
        options.TenantService = _tenantService.Get(name);
        options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => _coreDbContextManager.Get(name));
    }

    public void Configure(TariffService options)
    {
        options.Logger = _logger;
        options.CoreSettings = _coreSettings;
        options.Configuration = _configuration;
        options.TariffServiceStorage = _tariffServiceStorage;
        options.CoreBaseSettings = _coreBaseSettings;
        options.Test = _configuration["core:payment:test"] == "true";
        int.TryParse(_configuration["core:payment:delay"], out var paymentDelay);
        options.PaymentDelay = paymentDelay;
        options.Cache = _tariffServiceStorage.Cache;
        options.Notify = _tariffServiceStorage.Notify;

        options.QuotaService = _quotaService.Value;
        options.TenantService = _tenantService.Value;
        options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => _coreDbContextManager.Value);
    }
}

public class TariffService : ITariffService
{
    private const int DefaultTrialPeriod = 30;
    private static readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _standaloneCacheExpiration = TimeSpan.FromMinutes(15);

    internal ICache Cache { get; set; }
    internal ICacheNotify<TariffCacheItem> Notify { get; set; }
    internal ILogger<TariffService> Logger { get; set; }
    internal IQuotaService QuotaService { get; set; }
    internal ITenantService TenantService { get; set; }
    internal bool Test { get; set; }
    internal int PaymentDelay { get; set; }

    public TimeSpan CacheExpiration { get; set; }
    internal CoreBaseSettings CoreBaseSettings { get; set; }
    internal CoreSettings CoreSettings { get; set; }
    internal IConfiguration Configuration { get; set; }
    internal CoreDbContext CoreDbContext => LazyCoreDbContext.Value;
    internal Lazy<CoreDbContext> LazyCoreDbContext;
    internal TariffServiceStorage TariffServiceStorage { get; set; }

    private readonly BillingClient _billingClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly int _activeUsersMin;
    //private readonly int _activeUsersMax;

    public TariffService()
    {
        CacheExpiration = _defaultCacheExpiration;
    }

    public TariffService(
        IQuotaService quotaService,
        ITenantService tenantService,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        DbContextManager<CoreDbContext> coreDbContextManager,
        TariffServiceStorage tariffServiceStorage,
        ILogger<TariffService> logger,
        BillingClient billingClient,
        IHttpClientFactory httpClientFactory)
        : this()

    {
        Logger = logger;
        QuotaService = quotaService;
        TenantService = tenantService;
        CoreSettings = coreSettings;
        Configuration = configuration;
        TariffServiceStorage = tariffServiceStorage;
        _billingClient = billingClient;
        _httpClientFactory = httpClientFactory;
        CoreBaseSettings = coreBaseSettings;
        Test = configuration["core:payment:test"] == "true";
        int.TryParse(configuration["core:payment:delay"], out var paymentDelay);

        PaymentDelay = paymentDelay;

        Cache = TariffServiceStorage.Cache;
        Notify = TariffServiceStorage.Notify;
        LazyCoreDbContext = new Lazy<CoreDbContext>(() => coreDbContextManager.Value);
        var range = (Configuration["core.payment-user-range"] ?? "").Split('-');
        if (!int.TryParse(range[0], out _activeUsersMin))
        {
            _activeUsersMin = 0;
        }
        //if (range.Length < 2 || !int.TryParse(range[1], out _activeUsersMax))
        //{
        //    _activeUsersMax = constants.MaxEveryoneCount;
        //}
    }

    public Tariff GetTariff(int tenantId, bool withRequestToPaymentSystem = true)
    {
        //single tariff for all portals
        if (CoreBaseSettings.Standalone)
        {
            tenantId = -1;
        }

        var key = GetTariffCacheKey(tenantId);
        var tariff = Cache.Get<Tariff>(key);
        if (tariff == null)
        {
            tariff = GetBillingInfo(tenantId);
            tariff = CalculateTariff(tenantId, tariff);
            Cache.Insert(key, tariff, DateTime.UtcNow.Add(GetCacheExpiration()));

            if (_billingClient.Configured && withRequestToPaymentSystem)
            {
                Task.Run(() =>
                  {
                      try
                      {
                          var client = GetBillingClient();
                          var lastPayment = client.GetLastPayment(GetPortalId(tenantId));

                          var quota = QuotaService.GetTenantQuotas().SingleOrDefault(q => q.AvangateId == lastPayment.ProductId);
                          if (quota == null)
                          {
                              throw new InvalidOperationException($"Quota with id {lastPayment.ProductId} not found for portal {GetPortalId(tenantId)}.");
                          }

                          var asynctariff = Tariff.CreateDefault();
                          asynctariff.QuotaId = quota.Tenant;
                          asynctariff.Autorenewal = lastPayment.Autorenewal;
                          asynctariff.DueDate = 9999 <= lastPayment.EndDate.Year ? DateTime.MaxValue : lastPayment.EndDate;

                          if (quota.ActiveUsers == -1
                              && lastPayment.Quantity < _activeUsersMin)
                          {
                              throw new BillingException(string.Format("The portal {0} is paid for {1} users", tenantId, lastPayment.Quantity));
                          }
                          asynctariff.Quantity = lastPayment.Quantity;

                          if (SaveBillingInfo(tenantId, asynctariff, false))
                          {
                              asynctariff = CalculateTariff(tenantId, asynctariff);
                              ClearCache(tenantId);
                              Cache.Insert(key, asynctariff, DateTime.UtcNow.Add(GetCacheExpiration()));
                          }
                      }
                      catch (BillingNotFoundException)
                      {
                          var q = QuotaService.GetTenantQuota(tariff.QuotaId);

                          if (q != null
                              && !q.Trial
                              && !q.Free
                              && !q.NonProfit
                              && !q.Open
                              && !q.Custom)
                          {
                              var asynctariff = Tariff.CreateDefault();
                              asynctariff.DueDate = DateTime.Today.AddDays(-1);
                              asynctariff.Prolongable = false;
                              asynctariff.Autorenewal = false;
                              asynctariff.State = TariffState.NotPaid;

                              if (SaveBillingInfo(tenantId, asynctariff))
                              {
                                  asynctariff = CalculateTariff(tenantId, asynctariff);
                                  ClearCache(tenantId);
                                  Cache.Insert(key, asynctariff, DateTime.UtcNow.Add(GetCacheExpiration()));
                              }
                          }
                      }
                      catch (Exception error)
                      {
                          LogError(error, tenantId.ToString());
                      }
                  });
            }
        }

        return tariff;
    }

    public void SetTariff(int tenantId, Tariff tariff)
    {
        ArgumentNullException.ThrowIfNull(tariff);

        var q = QuotaService.GetTenantQuota(tariff.QuotaId);
        if (q == null)
        {
            return;
        }

        SaveBillingInfo(tenantId, tariff);
        if (q.Trial)
        {
            // reset trial date
            var tenant = TenantService.GetTenant(tenantId);
            if (tenant != null)
            {
                tenant.VersionChanged = DateTime.UtcNow;
                TenantService.SaveTenant(CoreSettings, tenant);
            }
        }

        ClearCache(tenantId);
    }

    internal static string GetTariffCacheKey(int tenantId)
    {
        return string.Format("{0}:{1}", tenantId, "tariff");
    }

    internal static string GetBillingUrlCacheKey(int tenantId)
    {
        return string.Format("{0}:{1}", tenantId, "billing:urls");
    }

    internal static string GetBillingPaymentCacheKey(int tenantId)
    {
        return string.Format("{0}:{1}", tenantId, "billing:payments");
    }


    public void ClearCache(int tenantId)
    {
        Notify.Publish(new TariffCacheItem { TenantId = tenantId }, CacheNotifyAction.Remove);
    }

    public IEnumerable<PaymentInfo> GetPayments(int tenantId)
    {
        var key = GetBillingPaymentCacheKey(tenantId);
        var payments = Cache.Get<List<PaymentInfo>>(key);
        if (payments == null)
        {
            payments = new List<PaymentInfo>();
            if (_billingClient.Configured)
            {
                try
                {
                    var quotas = QuotaService.GetTenantQuotas();
                    var client = GetBillingClient();
                    foreach (var pi in client.GetPayments(GetPortalId(tenantId)))
                    {
                        var quota = quotas.SingleOrDefault(q => q.AvangateId == pi.ProductRef);
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

            Cache.Insert(key, payments, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        return payments;
    }

    public Uri GetShoppingUri(int? tenant, int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null, string quantity = null)
    {
        var quota = QuotaService.GetTenantQuota(quotaId);
        if (quota == null)
        {
            return null;
        }

        var key = tenant.HasValue
                      ? GetBillingUrlCacheKey(tenant.Value)
                      : string.Format($"notenant{(!string.IsNullOrEmpty(affiliateId) ? "_" + affiliateId : "")}");
        key += quota.Visible ? "" : "0";
        if (Cache.Get<Dictionary<string, Uri>>(key) is not IDictionary<string, Uri> urls)
        {
            urls = new Dictionary<string, Uri>();
            if (_billingClient.Configured)
            {
                try
                {
                    var products = QuotaService.GetTenantQuotas()
                                               .Where(q => !string.IsNullOrEmpty(q.AvangateId) && q.Visible == quota.Visible)
                                               .Select(q => q.AvangateId)
                                               .ToArray();

                    var client = GetBillingClient();
                    urls =
                        client.GetPaymentUrls(
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
                    Logger.ErrorGetShoppingUri(error);
                }
            }
            Cache.Insert(key, urls, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        ResetCacheExpiration();

        if (!string.IsNullOrEmpty(quota.AvangateId) && urls.TryGetValue(quota.AvangateId, out var url))
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
        var url = Cache.Get<string>(key);
        if (url == null)
        {
            url = string.Empty;
            if (_billingClient.Configured)
            {
                try
                {
                    var client = GetBillingClient();
                    url =
                        client.GetPaymentUrl(
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
                    Logger.ErrorWithException(error);
                }
            }
            Cache.Insert(key, url, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        ResetCacheExpiration();


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
            var result = Cache.Get<IDictionary<string, Dictionary<string, decimal>>>(key);
            if (result == null)
            {
                var client = GetBillingClient();
                result = client.GetProductPriceInfo(productIds);
                Cache.Insert(key, result, DateTime.Now.AddHours(1));
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


    public string GetButton(int tariffId, string partnerId)
    {
        return CoreDbContext.Buttons
            .Where(r => r.TariffId == tariffId && r.PartnerId == partnerId)
            .Select(r => r.ButtonUrl)
            .SingleOrDefault();
    }

    public void SaveButton(int tariffId, string partnerId, string buttonUrl)
    {
        var efButton = new DbButton()
        {
            TariffId = tariffId,
            PartnerId = partnerId,
            ButtonUrl = buttonUrl
        };

        CoreDbContext.AddOrUpdate(r => r.Buttons, efButton);
        CoreDbContext.SaveChanges();
    }


    private Tariff GetBillingInfo(int tenant)
    {
        var r = CoreDbContext.Tariffs
            .Where(r => r.Tenant == tenant)
            .OrderByDescending(r => r.Id)
            .FirstOrDefault();

        if (r == null)
        {
            return Tariff.CreateDefault();
        }

        var tariff = Tariff.CreateDefault();
        tariff.QuotaId = r.Tariff;
        tariff.DueDate = r.Stamp.Year < 9999 ? r.Stamp : DateTime.MaxValue;
        tariff.Quantity = r.Quantity;

        return tariff;
    }

    private bool SaveBillingInfo(int tenant, Tariff tariffInfo, bool renewal = true)
    {
        var inserted = false;
        var currentTariff = GetBillingInfo(tenant);
        if (!tariffInfo.EqualsByParams(currentTariff))
        {
            var strategy = CoreDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
                {
                    using var tx = CoreDbContext.Database.BeginTransaction();

                    // last record is not the same
                    var any = CoreDbContext.Tariffs
                        .Any(r => r.Tenant == tenant && r.Tariff == tariffInfo.QuotaId && r.Stamp == tariffInfo.DueDate && r.Quantity == tariffInfo.Quantity);

                    if (tariffInfo.DueDate == DateTime.MaxValue || renewal || any)
                    {
                        var efTariff = new DbTariff
                        {
                            Tenant = tenant,
                            Tariff = tariffInfo.QuotaId,
                            Stamp = tariffInfo.DueDate,
                            Quantity = tariffInfo.Quantity,
                            CreateOn = DateTime.UtcNow
                        };

                        CoreDbContext.Tariffs.Add(efTariff);
                        CoreDbContext.SaveChanges();

                        Cache.Remove(GetTariffCacheKey(tenant));
                        inserted = true;
                    }

                    tx.Commit();
                });
        }

        if (inserted)
        {
            var t = TenantService.GetTenant(tenant);
            if (t != null)
            {
                // update tenant.LastModified to flush cache in documents
                TenantService.SaveTenant(CoreSettings, t);
            }
        }

        return inserted;
    }

    public void DeleteDefaultBillingInfo()
    {
        const int tenant = Tenant.DefaultTenant;

        var tariffs = CoreDbContext.Tariffs.Where(r => r.Tenant == tenant).ToList();

        foreach (var t in tariffs)
        {
            t.Tenant = -2;
        }

        CoreDbContext.SaveChanges();

        ClearCache(tenant);
    }


    private Tariff CalculateTariff(int tenantId, Tariff tariff)
    {
        tariff.State = TariffState.Paid;
        var q = QuotaService.GetTenantQuota(tariff.QuotaId);

        if (q == null || q.GetFeature("old"))
        {
            tariff.QuotaId = Tenant.DefaultTenant;
            q = QuotaService.GetTenantQuota(tariff.QuotaId);
        }

        var delay = 0;
        if (q != null && q.Trial)
        {
            tariff.State = TariffState.Trial;
            if (tariff.DueDate == DateTime.MinValue || tariff.DueDate == DateTime.MaxValue)
            {
                var tenant = TenantService.GetTenant(tenantId);
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
        else
        {
            delay = PaymentDelay;
        }

        if (tariff.DueDate != DateTime.MinValue && tariff.DueDate.Date < DateTime.Today && delay > 0)
        {
            tariff.State = TariffState.Delay;

            tariff.DelayDueDate = tariff.DueDate.Date.AddDays(delay);
        }

        if (tariff.DueDate == DateTime.MinValue ||
            tariff.DueDate != DateTime.MaxValue && tariff.DueDate.Date.AddDays(delay) < DateTime.Today)
        {
            tariff.State = TariffState.NotPaid;

            if (CoreBaseSettings.Standalone)
            {
                if (q != null)
                {
                    var defaultQuota = QuotaService.GetTenantQuota(Tenant.DefaultTenant);
                    defaultQuota.Name = "overdue";

                    defaultQuota.Features = q.Features;

                    QuotaService.SaveTenantQuota(defaultQuota);
                }

                var unlimTariff = Tariff.CreateDefault();
                unlimTariff.LicenseDate = tariff.DueDate;

                tariff = unlimTariff;
            }
        }

        tariff.Prolongable = tariff.DueDate == DateTime.MinValue || tariff.DueDate == DateTime.MaxValue ||
                             tariff.State == TariffState.Trial ||
                             new DateTime(tariff.DueDate.Year, tariff.DueDate.Month, 1) <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);

        return tariff;
    }

    private int GetPeriod(string key, int defaultValue)
    {
        var settings = TenantService.GetTenantSettings(Tenant.DefaultTenant, key);

        return settings != null ? Convert.ToInt32(Encoding.UTF8.GetString(settings)) : defaultValue;
    }

    private BillingClient GetBillingClient()
    {
        try
        {
            return new BillingClient(Test, Configuration, _httpClientFactory);
        }
        catch (InvalidOperationException ioe)
        {
            throw new BillingNotConfiguredException(ioe.Message, ioe);
        }
        catch (ReflectionTypeLoadException rtle)
        {
            Logger.ErrorLoaderExceptions(string.Join(Environment.NewLine, rtle.LoaderExceptions.Select(e => e.ToString())), rtle);
            throw;
        }
    }

    private string GetPortalId(int tenant)
    {
        return CoreSettings.GetKey(tenant);
    }

    private string GetAffiliateId(int tenant)
    {
        return CoreSettings.GetAffiliateId(tenant);
    }

    private string GetCampaign(int tenant)
    {
        return CoreSettings.GetCampaign(tenant);
    }

    private TimeSpan GetCacheExpiration()
    {
        if (CoreBaseSettings.Standalone && CacheExpiration < _standaloneCacheExpiration)
        {
            CacheExpiration = CacheExpiration.Add(TimeSpan.FromSeconds(30));
        }
        return CacheExpiration;
    }

    private void ResetCacheExpiration()
    {
        if (CoreBaseSettings.Standalone)
        {
            CacheExpiration = _defaultCacheExpiration;
        }
    }

    private void LogError(Exception error, string tenantId = null)
    {
        if (error is BillingNotFoundException)
        {
            Logger.DebugPaymentTenant(tenantId, error.Message);
        }
        else if (error is BillingNotConfiguredException)
        {
            Logger.DebugBillingTenant(tenantId, error.Message);
        }
        else
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.ErrorBillingWithException(tenantId, error);
            }
            else
            {
                Logger.ErrorBilling(tenantId, error.Message);
            }
        }
    }
}
