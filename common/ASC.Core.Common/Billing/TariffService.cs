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

namespace ASC.Core.Billing;

[Singletone]
public class TariffServiceStorage
{
    public ICache Cache { get; }
    internal ICacheNotify<TariffCacheItem> EventBusTariff { get; }

    public TariffServiceStorage(ICacheNotify<TariffCacheItem> eventBus, ICache cache)
    {
        Cache = cache;
        EventBusTariff = eventBus;
        EventBusTariff.Subscribe((i) =>
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
    private readonly IOptionsMonitor<ILog> _logger;

    public ConfigureTariffService(
        IOptionsSnapshot<CachedQuotaService> quotaService,
        IOptionsSnapshot<CachedTenantService> tenantService,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        DbContextManager<CoreDbContext> coreDbContextManager,
        TariffServiceStorage tariffServiceStorage,
        IOptionsMonitor<ILog> logger)
    {
        _quotaService = quotaService;
        _tenantService = tenantService;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
        _configuration = configuration;
        _coreDbContextManager = coreDbContextManager;
        _tariffServiceStorage = tariffServiceStorage;
        _logger = logger;
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
        options.Logger = _logger.CurrentValue;
        options.CoreSettings = _coreSettings;
        options.Configuration = _configuration;
        options.TariffServiceStorage = _tariffServiceStorage;
        options.Options = _logger;
        options.CoreBaseSettings = _coreBaseSettings;
        options.Test = _configuration["core:payment:test"] == "true";
        int.TryParse(_configuration["core:payment:delay"], out var paymentDelay);
        options.PaymentDelay = paymentDelay;
        options.Cache = _tariffServiceStorage.Cache;
        options.EventBus = _tariffServiceStorage.EventBusTariff;

        options.QuotaService = _quotaService.Value;
        options.TenantService = _tenantService.Value;
        options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => _coreDbContextManager.Value);
    }
}

public class TariffService : ITariffService
{
    public TimeSpan CacheExpiration { get; set; }
    public BillingClient BillingClient { get; }
    internal ICache Cache { get; set; }
    internal ICacheNotify<TariffCacheItem> EventBus { get; set; }
    internal ILog Logger { get; set; }
    internal IQuotaService QuotaService { get; set; }
    internal ITenantService TenantService { get; set; }
    internal bool Test { get; set; }
    internal int PaymentDelay { get; set; }
    internal CoreBaseSettings CoreBaseSettings { get; set; }
    internal CoreSettings CoreSettings { get; set; }
    internal IConfiguration Configuration { get; set; }
    internal CoreDbContext CoreDbContext => LazyCoreDbContext.Value;
    internal Lazy<CoreDbContext> LazyCoreDbContext { get; set; }
    internal TariffServiceStorage TariffServiceStorage { get; set; }
    internal IOptionsMonitor<ILog> Options { get; set; }

    public readonly int ActiveUsersMin;
    public readonly int ActiveUsersMax;

    private const int DefaultTrialPeriod = 30;

    private static readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _standaloneCacheExpiration = TimeSpan.FromMinutes(15);

    public TariffService() => CacheExpiration = _defaultCacheExpiration;

    public TariffService(
        IQuotaService quotaService,
        ITenantService tenantService,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        DbContextManager<CoreDbContext> coreDbContextManager,
        TariffServiceStorage tariffServiceStorage,
        IOptionsMonitor<ILog> options,
        Users.Constants constants,
        BillingClient billingClient)
        : this()

    {
        Logger = options.CurrentValue;
        QuotaService = quotaService;
        TenantService = tenantService;
        CoreSettings = coreSettings;
        Configuration = configuration;
        TariffServiceStorage = tariffServiceStorage;
        Options = options;
        BillingClient = billingClient;
        CoreBaseSettings = coreBaseSettings;
        Test = configuration["core:payment:test"] == "true";
        int.TryParse(configuration["core:payment:delay"], out var paymentDelay);

        PaymentDelay = paymentDelay;

        Cache = TariffServiceStorage.Cache;
        EventBus = TariffServiceStorage.EventBusTariff;
        LazyCoreDbContext = new Lazy<CoreDbContext>(() => coreDbContextManager.Value);
        var range = (Configuration["core.payment-user-range"] ?? "").Split('-');

        if (!int.TryParse(range[0], out ActiveUsersMin))
        {
            ActiveUsersMin = 0;
        }

        if (range.Length < 2 || !int.TryParse(range[1], out ActiveUsersMax))
        {
            ActiveUsersMax = constants.MaxEveryoneCount;
        }
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

            if (BillingClient.Configured && withRequestToPaymentSystem)
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
                              throw new InvalidOperationException(string.Format("Quota with id {0} not found for portal {1}.", lastPayment.ProductId, GetPortalId(tenantId)));
                          }

                          var asynctariff = Tariff.CreateDefault();
                          asynctariff.QuotaId = quota.Tenant;
                          asynctariff.Autorenewal = lastPayment.Autorenewal;
                          asynctariff.DueDate = 9999 <= lastPayment.EndDate.Year ? DateTime.MaxValue : lastPayment.EndDate;

                          if (quota.ActiveUsers == -1
                              && lastPayment.Quantity < ActiveUsersMin)
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
                      catch (BillingNotFoundException) { }
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
        if (tariff == null)
        {
            throw new ArgumentNullException(nameof(tariff));
        }

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

    public void ClearCache(int tenantId) =>
        EventBus.Publish(new TariffCacheItem { TenantId = tenantId }, CacheNotifyAction.Remove);

    public IEnumerable<PaymentInfo> GetPayments(int tenantId)
    {
        var key = GetBillingPaymentCacheKey(tenantId);
        var payments = Cache.Get<List<PaymentInfo>>(key);
        if (payments == null)
        {
            payments = new List<PaymentInfo>();
            if (BillingClient.Configured)
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
                      : string.Format("notenant{0}", !string.IsNullOrEmpty(affiliateId) ? "_" + affiliateId : "");
        key += quota.Visible ? "" : "0";

        if (!(Cache.Get<Dictionary<string, Tuple<Uri, Uri>>>(key) is IDictionary<string, Tuple<Uri, Uri>> urls))
        {
            urls = new Dictionary<string, Tuple<Uri, Uri>>();

            if (BillingClient.Configured)
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
                    Logger.Error(error);
                }
            }

            Cache.Insert(key, urls, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
        }

        ResetCacheExpiration();

        if (!string.IsNullOrEmpty(quota.AvangateId) && urls.TryGetValue(quota.AvangateId, out var tuple))
        {
            var result = tuple.Item2;

            if (result == null)
            {
                result = tuple.Item1;
            }
            else
            {
                var tariff = tenant.HasValue ? GetTariff(tenant.Value) : null;
                if (tariff == null || tariff.QuotaId == quotaId || tariff.State >= TariffState.Delay)
                {
                    result = tuple.Item1;
                }
            }

            if (result == null)
            {
                return null;
            }

            result = new Uri(result.ToString()
                                   .Replace("__Currency__", HttpUtility.UrlEncode(currency ?? ""))
                                   .Replace("__Language__", HttpUtility.UrlEncode((language ?? "").ToLower()))
                                   .Replace("__CustomerID__", HttpUtility.UrlEncode(customerId ?? ""))
                                   .Replace("__Quantity__", HttpUtility.UrlEncode(quantity ?? "")));

            return result;
        }

        return null;
    }

    public IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(params string[] productIds)
    {
        if (productIds == null)
        {
            throw new ArgumentNullException(nameof(productIds));
        }

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

    internal static string GetTariffCacheKey(int tenantId)
    {
        return $"{tenantId}:tariff";
    }

    internal static string GetBillingUrlCacheKey(int tenantId)
    {
        return $"{tenantId}:billing:urls";
    }

    internal static string GetBillingPaymentCacheKey(int tenantId)
    {
        return $"{tenantId}:billing:payments";
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
            using var tx = CoreDbContext.Database.BeginTransaction();

            // last record is not the same
            var count = CoreDbContext.Tariffs
                .Count(r => r.Tenant == tenant && r.Tariff == tariffInfo.QuotaId && r.Stamp == tariffInfo.DueDate && r.Quantity == tariffInfo.Quantity);

            if (tariffInfo.DueDate == DateTime.MaxValue || renewal || count == 0)
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
        }

        if (inserted)
        {
            var t = TenantService.GetTenant(tenant);
            if (t != null)
            {
                TenantService.SaveTenant(CoreSettings, t); // update tenant.LastModified to flush cache in documents
            }
        }

        return inserted;
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
                    var fromDate = tenant.CreatedDateTime < tenant.VersionChanged ? tenant.VersionChanged : tenant.CreatedDateTime;
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
                    defaultQuota.Support = false;

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
            return new BillingClient(Test, Configuration);
        }
        catch (InvalidOperationException ioe)
        {
            throw new BillingNotConfiguredException(ioe.Message, ioe);
        }
        catch (ReflectionTypeLoadException rtle)
        {
            Logger.ErrorFormat("{0}{1}LoaderExceptions: {2}",
                rtle,
                Environment.NewLine,
                string.Join(Environment.NewLine, rtle.LoaderExceptions.Select(e => e.ToString())));
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
            Logger.DebugFormat("Payment tenant {0} not found: {1}", tenantId, error.Message);
        }
        else if (error is BillingNotConfiguredException)
        {
            Logger.DebugFormat("Billing tenant {0} not configured: {1}", tenantId, error.Message);
        }
        else
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Error("Billing tenant " + tenantId);
                Logger.Error(error);
            }
            else
            {
                Logger.ErrorFormat("Billing tenant {0}: {1}", tenantId, error.Message);
            }
        }
    }
}
