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
class ConfigureTenantManager : IConfigureNamedOptions<TenantManager>
{
    private readonly IOptionsSnapshot<CachedTenantService> _tenantService;
    private readonly IOptionsSnapshot<CachedQuotaService> _quotaService;
    private readonly IOptionsSnapshot<TariffService> _tariffService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;

    public ConfigureTenantManager(
        IOptionsSnapshot<CachedTenantService> tenantService,
        IOptionsSnapshot<CachedQuotaService> quotaService,
        IOptionsSnapshot<TariffService> tariffService,
        IHttpContextAccessor httpContextAccessor,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings
        )
    {
        _tenantService = tenantService;
        _quotaService = quotaService;
        _tariffService = tariffService;
        _httpContextAccessor = httpContextAccessor;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
    }

    public void Configure(string name, TenantManager options)
    {
        Configure(options);

        options.TenantService = _tenantService.Get(name);
        options.QuotaService = _quotaService.Get(name);
        options.TariffService = _tariffService.Get(name);
    }

    public void Configure(TenantManager options)
    {
        options.HttpContextAccessor = _httpContextAccessor;
        options.CoreBaseSettings = _coreBaseSettings;
        options.CoreSettings = _coreSettings;

        options.TenantService = _tenantService.Value;
        options.QuotaService = _quotaService.Value;
        options.TariffService = _tariffService.Value;
    }
}

[Scope(typeof(ConfigureTenantManager))]
public class TenantManager
{
    private Tenant _currentTenant;

    public const string CurrentTenant = "CURRENT_TENANT";
    internal ITenantService TenantService;
    internal IQuotaService QuotaService;
    internal ITariffService TariffService;

    private static readonly List<string> _thisCompAddresses = new List<string>();

    internal IHttpContextAccessor HttpContextAccessor;
    internal CoreBaseSettings CoreBaseSettings;
    internal CoreSettings CoreSettings;

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
        if (tenant == null)
        {
            throw new ArgumentNullException(nameof(tenant));
        }

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


    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        return GetTenantQuotas(false);
    }

    public IEnumerable<TenantQuota> GetTenantQuotas(bool all)
    {
        return QuotaService.GetTenantQuotas().Where(q => q.Tenant < 0 && (all || q.Visible)).OrderByDescending(q => q.Tenant).ToList();
    }

    public TenantQuota GetTenantQuota(int tenant)
    {
        var defaultQuota = QuotaService.GetTenantQuota(tenant) ?? QuotaService.GetTenantQuota(Tenant.DefaultTenant) ?? TenantQuota.Default;
        if (defaultQuota.Tenant != tenant && TariffService != null)
        {
            var tariff = TariffService.GetTariff(tenant);
            var currentQuota = QuotaService.GetTenantQuota(tariff.QuotaId);
            if (currentQuota != null)
            {
                currentQuota = (TenantQuota)currentQuota.Clone();

                if (currentQuota.ActiveUsers == -1)
                {
                    currentQuota.ActiveUsers = tariff.Quantity;
                    currentQuota.MaxTotalSize *= currentQuota.ActiveUsers;
                }

                return currentQuota;
            }
        }

        return defaultQuota;
    }

    public IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(bool all = true)
    {
        var productIds = GetTenantQuotas(all)
            .Select(p => p.AvangateId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToArray();

        return TariffService.GetProductPriceInfo(productIds);
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        quota = QuotaService.SaveTenantQuota(quota);

        return quota;
    }

    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        QuotaService.SetTenantQuotaRow(row, exchange);
    }

    public List<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        return QuotaService.FindTenantQuotaRows(tenantId).ToList();
    }
}
