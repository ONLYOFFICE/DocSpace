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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Core.Billing
{
    public class TariffServiceStorage
    {
        public ICache Cache { get; }
        public ICacheNotify<TariffCacheItem> Notify { get; }

        public TariffServiceStorage(ICacheNotify<TariffCacheItem> notify)
        {
            Cache = AscCache.Memory;
            Notify = notify;
            Notify.Subscribe((i) =>
            {
                Cache.Remove(TariffService.GetTariffCacheKey(i.TenantId));
                Cache.Remove(TariffService.GetBillingUrlCacheKey(i.TenantId));
                Cache.Remove(TariffService.GetBillingPaymentCacheKey(i.TenantId, DateTime.MinValue, DateTime.MaxValue)); // clear all payments
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

    public class TariffService : ITariffService
    {
        private readonly ICache cache;
        private readonly ICacheNotify<TariffCacheItem> notify;

        private const int DEFAULT_TRIAL_PERIOD = 30;
        private static readonly TimeSpan DEFAULT_CACHE_EXPIRATION = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan STANDALONE_CACHE_EXPIRATION = TimeSpan.FromMinutes(15);

        private readonly static bool billingConfigured = false;

        private readonly ILog log;
        private readonly IQuotaService quotaService;
        private readonly ITenantService tenantService;
        private readonly bool test;
        private readonly int paymentDelay;


        public TimeSpan CacheExpiration { get; set; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public CoreSettings CoreSettings { get; }
        public IConfiguration Configuration { get; }
        public CoreDbContext CoreDbContext { get; set; }
        public TariffServiceStorage TariffServiceStorage { get; }
        public IOptionsMonitor<ILog> Options { get; }

        private TariffService(
            IQuotaService quotaService,
            ITenantService tenantService,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            TariffServiceStorage tariffServiceStorage,
            IOptionsMonitor<ILog> options)

        {
            log = options.CurrentValue;
            this.quotaService = quotaService;
            this.tenantService = tenantService;
            CoreSettings = coreSettings;
            Configuration = configuration;
            TariffServiceStorage = tariffServiceStorage;
            Options = options;
            CoreBaseSettings = coreBaseSettings;
            CacheExpiration = DEFAULT_CACHE_EXPIRATION;
            test = configuration["core:payment:test"] == "true";
            int.TryParse(configuration["core:payment:delay"], out paymentDelay);

            cache = TariffServiceStorage.Cache;
            notify = TariffServiceStorage.Notify;
        }
        public TariffService(
            IQuotaService quotaService,
            ITenantService tenantService,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            DbContextManager<CoreDbContext> coreDbContextManager,
            TariffServiceStorage tariffServiceStorage,
            IOptionsMonitor<ILog> options)
            : this(quotaService, tenantService, coreBaseSettings, coreSettings, configuration, tariffServiceStorage, options)

        {
            CoreDbContext = coreDbContextManager.Value;
        }
        public TariffService(
            IQuotaService quotaService,
            ITenantService tenantService,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            CoreDbContext coreDbContext,
            TariffServiceStorage tariffServiceStorage,
            IOptionsMonitor<ILog> options)
            : this(quotaService, tenantService, coreBaseSettings, coreSettings, configuration, tariffServiceStorage, options)

        {
            CoreDbContext = coreDbContext;
        }


        public Tariff GetTariff(int tenantId, bool withRequestToPaymentSystem = true)
        {
            //single tariff for all portals
            if (CoreBaseSettings.Standalone)
                tenantId = -1;

            var key = GetTariffCacheKey(tenantId);
            var tariff = cache.Get<Tariff>(key);
            if (tariff == null)
            {
                tariff = Tariff.CreateDefault();

                var cached = GetBillingInfo(tenantId);
                if (cached != null)
                {
                    tariff.QuotaId = cached.Item1;
                    tariff.DueDate = cached.Item2;
                }

                tariff = CalculateTariff(tenantId, tariff);
                cache.Insert(key, tariff, DateTime.UtcNow.Add(GetCacheExpiration()));

                if (billingConfigured && withRequestToPaymentSystem)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            using var client = GetBillingClient();
                            var p = client.GetLastPayment(GetPortalId(tenantId));
                            var quota = quotaService.GetTenantQuotas().SingleOrDefault(q => q.AvangateId == p.ProductId);
                            if (quota == null)
                            {
                                throw new InvalidOperationException(string.Format("Quota with id {0} not found for portal {1}.", p.ProductId, GetPortalId(tenantId)));
                            }
                            var asynctariff = Tariff.CreateDefault();
                            asynctariff.QuotaId = quota.Id;
                            asynctariff.Autorenewal = p.Autorenewal;
                            asynctariff.DueDate = 9999 <= p.EndDate.Year ? DateTime.MaxValue : p.EndDate;

                            if (SaveBillingInfo(tenantId, Tuple.Create(asynctariff.QuotaId, asynctariff.DueDate), false))
                            {
                                asynctariff = CalculateTariff(tenantId, asynctariff);
                                ClearCache(tenantId);
                                cache.Insert(key, asynctariff, DateTime.UtcNow.Add(GetCacheExpiration()));
                            }
                        }
                        catch (Exception error)
                        {
                            LogError(error);
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
                throw new ArgumentNullException("tariff");
            }

            var q = quotaService.GetTenantQuota(tariff.QuotaId);
            if (q == null) return;
            SaveBillingInfo(tenantId, Tuple.Create(tariff.QuotaId, tariff.DueDate));
            if (q.Trial)
            {
                // reset trial date
                var tenant = tenantService.GetTenant(tenantId);
                if (tenant != null)
                {
                    tenant.VersionChanged = DateTime.UtcNow;
                    tenantService.SaveTenant(CoreSettings, tenant);
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

        internal static string GetBillingPaymentCacheKey(int tenantId, DateTime from, DateTime to)
        {
            return string.Format("{0}:{1}:{2}-{3}", tenantId, "billing:payments", from.ToString("yyyyMMddHHmmss"), to.ToString("yyyyMMddHHmmss"));
        }


        public void ClearCache(int tenantId)
        {
            notify.Publish(new TariffCacheItem { TenantId = tenantId }, CacheNotifyAction.Remove);
        }

        public IEnumerable<PaymentInfo> GetPayments(int tenantId, DateTime from, DateTime to)
        {
            from = from.Date;
            to = to.Date.AddTicks(TimeSpan.TicksPerDay - 1);
            var key = GetBillingPaymentCacheKey(tenantId, from, to);
            var payments = cache.Get<List<PaymentInfo>>(key);
            if (payments == null)
            {
                payments = new List<PaymentInfo>();
                if (billingConfigured)
                {
                    try
                    {
                        var quotas = quotaService.GetTenantQuotas();
                        using var client = GetBillingClient();
                        foreach (var pi in client.GetPayments(GetPortalId(tenantId), from, to))
                        {
                            var quota = quotas.SingleOrDefault(q => q.AvangateId == pi.ProductId);
                            if (quota != null)
                            {
                                pi.QuotaId = quota.Id;
                            }
                            payments.Add(pi);
                        }
                    }
                    catch (Exception error)
                    {
                        LogError(error);
                    }
                }

                cache.Insert(key, payments, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
            }

            return payments;
        }

        public Uri GetShoppingUri(int? tenant, int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null)
        {
            var quota = quotaService.GetTenantQuota(quotaId);
            if (quota == null) return null;

            var key = tenant.HasValue
                          ? GetBillingUrlCacheKey(tenant.Value)
                          : string.Format("notenant{0}", !string.IsNullOrEmpty(affiliateId) ? "_" + affiliateId : "");
            key += quota.Visible ? "" : "0";
            if (!(cache.Get<Dictionary<string, Tuple<Uri, Uri>>>(key) is IDictionary<string, Tuple<Uri, Uri>> urls))
            {
                urls = new Dictionary<string, Tuple<Uri, Uri>>();
                if (billingConfigured)
                {
                    try
                    {
                        var products = quotaService.GetTenantQuotas()
                                                   .Where(q => !string.IsNullOrEmpty(q.AvangateId) && q.Visible == quota.Visible)
                                                   .Select(q => q.AvangateId)
                                                   .ToArray();

                        using var client = GetBillingClient();
                        urls = tenant.HasValue ?
client.GetPaymentUrls(GetPortalId(tenant.Value), products, GetAffiliateId(tenant.Value), "__Currency__", "__Language__", "__CustomerID__") :
client.GetPaymentUrls(null, products, !string.IsNullOrEmpty(affiliateId) ? affiliateId : null, "__Currency__", "__Language__", "__CustomerID__");
                    }
                    catch (Exception error)
                    {
                        log.Error(error);
                    }
                }
                cache.Insert(key, urls, DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)));
            }

            ResetCacheExpiration();

            if (!string.IsNullOrEmpty(quota.AvangateId) && urls.TryGetValue(quota.AvangateId, out var tuple))
            {
                var result = tuple.Item2;

                var tariff = tenant.HasValue ? GetTariff(tenant.Value) : null;
                if (result == null || tariff == null || tariff.QuotaId == quotaId || tariff.State >= TariffState.Delay)
                {
                    result = tuple.Item1;
                }

                result = new Uri(result.ToString()
                                       .Replace("__Currency__", currency ?? "")
                                       .Replace("__Language__", (language ?? "").ToLower())
                                       .Replace("__CustomerID__", customerId ?? ""));
                return result;
            }
            return null;
        }

        public IDictionary<string, IEnumerable<Tuple<string, decimal>>> GetProductPriceInfo(params string[] productIds)
        {
            if (productIds == null)
            {
                throw new ArgumentNullException("productIds");
            }
            try
            {
                var key = "biling-prices" + string.Join(",", productIds);
                var result = cache.Get<IDictionary<string, IEnumerable<Tuple<string, decimal>>>>(key);
                if (result == null)
                {
                    using (var client = GetBillingClient())
                    {
                        result = client.GetProductPriceInfo(productIds);
                    }
                    cache.Insert(key, result, DateTime.Now.AddHours(1));
                }
                return result;
            }
            catch (Exception error)
            {
                LogError(error);
                return productIds
                    .Select(p => new { ProductId = p, Prices = Enumerable.Empty<Tuple<string, decimal>>() })
                    .ToDictionary(e => e.ProductId, e => e.Prices);
            }
        }

        public Invoice GetInvoice(string paymentId)
        {
            var result = new Invoice();

            if (billingConfigured)
            {
                try
                {
                    using var client = GetBillingClient();
                    result = client.GetInvoice(paymentId);
                }
                catch (Exception error)
                {
                    LogError(error);
                }
            }
            return result;
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


        private Tuple<int, DateTime> GetBillingInfo(int tenant)
        {
            var r = CoreDbContext.Tariffs
                .Where(r => r.Tenant == tenant)
                .OrderByDescending(r => r.Id)
                .SingleOrDefault();

            return r != null ? Tuple.Create(r.Tariff, r.Stamp.Year < 9999 ? r.Stamp : DateTime.MaxValue) : null;
        }

        private bool SaveBillingInfo(int tenant, Tuple<int, DateTime> bi, bool renewal = true)
        {
            var inserted = false;
            if (!Equals(bi, GetBillingInfo(tenant)))
            {
                using var tx = CoreDbContext.Database.BeginTransaction();

                // last record is not the same
                var count = CoreDbContext.Tariffs
                    .Count(r => r.Tenant == tenant && r.Tariff == bi.Item1 && r.Stamp == bi.Item2);

                if (bi.Item2 == DateTime.MaxValue || renewal || count == 0)
                {
                    var efTariff = new DbTariff
                    {
                        Tenant = tenant,
                        Tariff = bi.Item1,
                        Stamp = bi.Item2
                    };

                    CoreDbContext.Tariffs.Add(efTariff);
                    CoreDbContext.SaveChanges();

                    cache.Remove(GetTariffCacheKey(tenant));
                    inserted = true;
                }
                tx.Commit();
            }

            if (inserted)
            {
                var t = tenantService.GetTenant(tenant);
                if (t != null)
                {
                    // update tenant.LastModified to flush cache in documents
                    tenantService.SaveTenant(CoreSettings, t);
                }
            }
            return inserted;
        }

        public void DeleteDefaultBillingInfo()
        {
            const int tenant = Tenant.DEFAULT_TENANT;

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
            var q = quotaService.GetTenantQuota(tariff.QuotaId);

            if (q == null || q.GetFeature("old"))
            {
                tariff.QuotaId = Tenant.DEFAULT_TENANT;
                q = quotaService.GetTenantQuota(tariff.QuotaId);
            }

            var delay = 0;
            if (q != null && q.Trial)
            {
                tariff.State = TariffState.Trial;
                if (tariff.DueDate == DateTime.MinValue || tariff.DueDate == DateTime.MaxValue)
                {
                    var tenant = tenantService.GetTenant(tenantId);
                    if (tenant != null)
                    {
                        var fromDate = tenant.CreatedDateTime < tenant.VersionChanged ? tenant.VersionChanged : tenant.CreatedDateTime;
                        var trialPeriod = GetPeriod("TrialPeriod", DEFAULT_TRIAL_PERIOD);
                        if (fromDate == DateTime.MinValue) fromDate = DateTime.UtcNow.Date;
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
                delay = paymentDelay;
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

                if ((q == null || !q.Trial) && CoreBaseSettings.Standalone)
                {
                    if (q != null)
                    {
                        var defaultQuota = quotaService.GetTenantQuota(Tenant.DEFAULT_TENANT);
                        if (defaultQuota.CountPortals != q.CountPortals)
                        {
                            defaultQuota.CountPortals = q.CountPortals;
                            quotaService.SaveTenantQuota(defaultQuota);
                        }
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
            var settings = tenantService.GetTenantSettings(Tenant.DEFAULT_TENANT, key);
            return settings != null ? Convert.ToInt32(Encoding.UTF8.GetString(settings)) : defaultValue;
        }

        private BillingClient GetBillingClient()
        {
            try
            {
                return new BillingClient(test, Configuration, Options);
            }
            catch (InvalidOperationException ioe)
            {
                throw new BillingNotConfiguredException(ioe.Message, ioe);
            }
            catch (ReflectionTypeLoadException rtle)
            {
                log.ErrorFormat("{0}{1}LoaderExceptions: {2}",
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

        private TimeSpan GetCacheExpiration()
        {
            if (CoreBaseSettings.Standalone && CacheExpiration < STANDALONE_CACHE_EXPIRATION)
            {
                CacheExpiration = CacheExpiration.Add(TimeSpan.FromSeconds(30));
            }
            return CacheExpiration;
        }

        private void ResetCacheExpiration()
        {
            if (CoreBaseSettings.Standalone)
            {
                CacheExpiration = DEFAULT_CACHE_EXPIRATION;
            }
        }

        private void LogError(Exception error)
        {
            if (error is BillingNotFoundException)
            {
                log.DebugFormat("Payment not found: {0}", error.Message);
            }
            else if (error is BillingNotConfiguredException)
            {
                log.DebugFormat("Billing not configured: {0}", error.Message);
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Error(error);
                }
                else
                {
                    log.Error(error.Message);
                }
            }
        }
    }

    public static class TariffConfigExtension
    {
        public static IServiceCollection AddTariffService(this IServiceCollection services)
        {
            services.AddCoreDbContextService();

            services.TryAddSingleton<TariffServiceStorage>();
            services.TryAddScoped<ITariffService, TariffService>();
            return services;
        }
    }
}