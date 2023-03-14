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

using Microsoft.Net.Http.Headers;

namespace ASC.Core;


[Scope]
public class TenantManager
{
    private Tenant _currentTenant;

    public const string CurrentTenant = "CURRENT_TENANT";
    internal ITenantService TenantService { get; set; }
    internal IQuotaService QuotaService { get; set; }
    internal ITariffService TariffService { get; set; }

    private static readonly List<string> _thisCompAddresses = new List<string>();

    internal IHttpContextAccessor HttpContextAccessor { get; set; }
    internal CoreBaseSettings CoreBaseSettings { get; set; }
    internal CoreSettings CoreSettings { get; set; }

    private readonly static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    static TenantManager()
    {
        _thisCompAddresses.Add("localhost");
        _thisCompAddresses.Add(Dns.GetHostName().ToLowerInvariant());
        _thisCompAddresses.AddRange(Dns.GetHostAddresses("localhost").Select(a => a.ToString()));
        try
        {
            _thisCompAddresses.AddRange(Dns.GetHostAddresses(Dns.GetHostName()).Select(a => a.ToString()));
        }
        catch
        {
            // ignore
        }
    }

    public TenantManager()
    {

    }

    public TenantManager(
        ITenantService tenantService,
        IQuotaService quotaService,
        ITariffService tariffService,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings)
    {
        TenantService = tenantService;
        QuotaService = quotaService;
        TariffService = tariffService;
        CoreBaseSettings = coreBaseSettings;
        CoreSettings = coreSettings;
    }

    public TenantManager(
        ITenantService tenantService,
        IQuotaService quotaService,
        ITariffService tariffService,
        IHttpContextAccessor httpContextAccessor,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings) : this(tenantService, quotaService, tariffService, coreBaseSettings, coreSettings)
    {
        HttpContextAccessor = httpContextAccessor;
    }


    public List<Tenant> GetTenants(bool active = true)
    {
        return TenantService.GetTenants(default, active).ToList();
    }
    public List<Tenant> GetTenants(List<int> ids)
    {
        return TenantService.GetTenants(ids).ToList();
    }

    public Tenant GetTenant(int tenantId)
    {
        return TenantService.GetTenant(tenantId);
    }

    public Tenant GetTenant(string domain)
    {
        if (string.IsNullOrEmpty(domain))
        {
            return null;
        }

        Tenant t = null;
        if (_thisCompAddresses.Contains(domain, StringComparer.InvariantCultureIgnoreCase))
        {
            t = TenantService.GetTenant("localhost");
        }
        var isAlias = false;
        if (t == null)
        {
            var baseUrl = CoreSettings.BaseDomain;
            if (!string.IsNullOrEmpty(baseUrl) && domain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                isAlias = true;
                t = TenantService.GetTenant(domain.Substring(0, domain.Length - baseUrl.Length - 1));
            }
        }
        if (t == null)
        {
            t = TenantService.GetTenant(domain);
        }
        if (t == null && CoreBaseSettings.Standalone && !isAlias)
        {
            t = TenantService.GetTenantForStandaloneWithoutAlias(domain);
        }

        return t;
    }

    public Tenant SetTenantVersion(Tenant tenant, int version)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        if (tenant.Version != version)
        {
            tenant.Version = version;
            SaveTenant(tenant);
        }
        else
        {
            throw new ArgumentException("This is current version already");
        }

        return tenant;
    }

    public Tenant SaveTenant(Tenant tenant)
    {
        var newTenant = TenantService.SaveTenant(CoreSettings, tenant);
        if (CallContext.GetData(CurrentTenant) is Tenant)
        {
            SetCurrentTenant(newTenant);
        }

        return newTenant;
    }

    public void RemoveTenant(int tenantId, bool auto = false)
    {
        TenantService.RemoveTenant(tenantId, auto);
    }

    public Tenant GetCurrentTenant(HttpContext context)
    {
        return GetCurrentTenant(true, context);
    }

    public Tenant GetCurrentTenant(bool throwIfNotFound, HttpContext context)
    {
        if (_currentTenant != null)
        {
            return _currentTenant;
        }

        Tenant tenant = null;

        if (context != null)
        {
            tenant = context.Items[CurrentTenant] as Tenant;
            if (tenant == null && context.Request != null)
            {
                tenant = GetTenant(context.Request.GetUrlRewriter().Host);
                context.Items[CurrentTenant] = tenant;
            }

            if (tenant == null && context.Request != null)
            {
                var origin = context.Request.Headers[HeaderNames.Origin].FirstOrDefault();

                if (!string.IsNullOrEmpty(origin))
                {
                    var originUri = new Uri(origin);

                    tenant = GetTenant(originUri.Host);
                    context.Items[CurrentTenant] = tenant;
                }
            }
        }

        if (tenant == null && throwIfNotFound)
        {
            throw new Exception("Could not resolve current tenant :-(.");
        }

        _currentTenant = tenant;

        return tenant;
    }

    public Tenant GetCurrentTenant()
    {
        return GetCurrentTenant(true);
    }

    public Tenant GetCurrentTenant(bool throwIfNotFound)
    {
        return GetCurrentTenant(throwIfNotFound, HttpContextAccessor?.HttpContext);
    }

    public void SetCurrentTenant(Tenant tenant)
    {
        if (tenant != null)
        {
            _currentTenant = tenant;
            if (HttpContextAccessor?.HttpContext != null)
            {
                HttpContextAccessor.HttpContext.Items[CurrentTenant] = tenant;
            }

            Thread.CurrentThread.CurrentCulture = tenant.GetCulture();
            Thread.CurrentThread.CurrentUICulture = tenant.GetCulture();
        }
    }

    public Tenant SetCurrentTenant(int tenantId)
    {
        var result = GetTenant(tenantId);
        SetCurrentTenant(result);

        return result;
    }

    public Tenant SetCurrentTenant(string domain)
    {
        var result = GetTenant(domain);
        SetCurrentTenant(result);

        return result;
    }

    public void CheckTenantAddress(string address)
    {
        TenantService.ValidateDomain(address);
    }

    public IEnumerable<TenantVersion> GetTenantVersions()
    {
        return TenantService.GetTenantVersions();
    }


    public Task<IEnumerable<TenantQuota>> GetTenantQuotasAsync()
    {
        return GetTenantQuotasAsync(false);
    }

    public async Task<IEnumerable<TenantQuota>> GetTenantQuotasAsync(bool all)
    {
        return (await QuotaService.GetTenantQuotasAsync()).Where(q => q.Tenant < 0 && (all || q.Visible)).OrderByDescending(q => q.Tenant).ToList();
    }

    public Task<TenantQuota> GetCurrentTenantQuotaAsync()
    {
        return GetTenantQuotaAsync(GetCurrentTenant().Id);
    }

    public async Task<TenantQuota> GetTenantQuotaAsync(int tenant)
    {
        var defaultQuota = await QuotaService.GetTenantQuotaAsync(tenant) ?? await QuotaService.GetTenantQuotaAsync(Tenant.DefaultTenant) ?? TenantQuota.Default;
        if (defaultQuota.Tenant != tenant && TariffService != null)
        {
            var tariff = await TariffService.GetTariffAsync(tenant);

            TenantQuota currentQuota = null;
            foreach (var tariffRow in tariff.Quotas)
            {
                var qty = tariffRow.Quantity;

                var quota = await QuotaService.GetTenantQuotaAsync(tariffRow.Id);

                quota *= qty;
                currentQuota += quota;
            }

            return currentQuota;
        }

        return defaultQuota;
    }

    public async Task<IDictionary<string, Dictionary<string, decimal>>> GetProductPriceInfoAsync()
    {
        var quotas = await GetTenantQuotasAsync(false);
        var productIds = quotas
            .Select(p => p.ProductId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToArray();

        var prices = TariffService.GetProductPriceInfo(productIds);
        var result = prices.ToDictionary(price => quotas.First(quota => quota.ProductId == price.Key).Name, price => price.Value);
        return result;
    }

    public async Task<TenantQuota> SaveTenantQuotaAsync(TenantQuota quota)
    {
        quota = await QuotaService.SaveTenantQuotaAsync(quota);

        return quota;
    }

    public async Task SetTenantQuotaRowAsync(TenantQuotaRow row, bool exchange)
    {
        await _semaphore.WaitAsync();

        await QuotaService.SetTenantQuotaRowAsync(row, exchange);

        _semaphore.Release();
    }

    public async Task<List<TenantQuotaRow>> FindTenantQuotaRowsAsync(int tenantId)
    {
        return (await QuotaService.FindTenantQuotaRowsAsync(tenantId)).ToList();
    }
}
