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

        options._tenantService = _tenantService.Get(name);
        options._quotaService = _quotaService.Get(name);
        options._tariffService = _tariffService.Get(name);
    }

    public void Configure(TenantManager options)
    {
        options._httpContextAccessor = _httpContextAccessor;
        options._coreBaseSettings = _coreBaseSettings;
        options._coreSettings = _coreSettings;

        options._tenantService = _tenantService.Value;
        options._quotaService = _quotaService.Value;
        options._tariffService = _tariffService.Value;
    }
}

[Scope(typeof(ConfigureTenantManager))]
public class TenantManager
{
    private Tenant _currentTenant;

    public const string CurrentTenant = "CURRENT_TENANT";
    internal ITenantService _tenantService;
    internal IQuotaService _quotaService;
    internal ITariffService _tariffService;

    private static readonly List<string> _thisCompAddresses = new List<string>();

    internal IHttpContextAccessor _httpContextAccessor;
    internal CoreBaseSettings _coreBaseSettings;
    internal CoreSettings _coreSettings;

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
        _tenantService = tenantService;
        _quotaService = quotaService;
        _tariffService = tariffService;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
    }

    public TenantManager(
        ITenantService tenantService,
        IQuotaService quotaService,
        ITariffService tariffService,
        IHttpContextAccessor httpContextAccessor,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings) : this(tenantService, quotaService, tariffService, coreBaseSettings, coreSettings)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public List<Tenant> GetTenants(bool active = true)
    {
        return _tenantService.GetTenants(default, active).ToList();
    }
    public List<Tenant> GetTenants(List<int> ids)
    {
        return _tenantService.GetTenants(ids).ToList();
    }

    public Tenant GetTenant(int tenantId)
    {
        return _tenantService.GetTenant(tenantId);
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
            t = _tenantService.GetTenant("localhost");
        }
        var isAlias = false;
        if (t == null)
        {
            var baseUrl = _coreSettings.BaseDomain;
            if (!string.IsNullOrEmpty(baseUrl) && domain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                isAlias = true;
                t = _tenantService.GetTenant(domain.Substring(0, domain.Length - baseUrl.Length - 1));
            }
        }
        if (t == null)
        {
            t = _tenantService.GetTenant(domain);
        }
        if (t == null && _coreBaseSettings.Standalone && !isAlias)
        {
            t = _tenantService.GetTenantForStandaloneWithoutAlias(domain);
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
        var newTenant = _tenantService.SaveTenant(_coreSettings, tenant);
        if (CallContext.GetData(CurrentTenant) is Tenant)
        {
            SetCurrentTenant(newTenant);
        }

        return newTenant;
    }

    public void RemoveTenant(int tenantId, bool auto = false)
    {
        _tenantService.RemoveTenant(tenantId, auto);
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
        return GetCurrentTenant(throwIfNotFound, _httpContextAccessor?.HttpContext);
    }

    public void SetCurrentTenant(Tenant tenant)
    {
        if (tenant != null)
        {
            _currentTenant = tenant;
            if (_httpContextAccessor?.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items[CurrentTenant] = tenant;
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
        _tenantService.ValidateDomain(address);
    }

    public IEnumerable<TenantVersion> GetTenantVersions()
    {
        return _tenantService.GetTenantVersions();
    }


    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        return GetTenantQuotas(false);
    }

    public IEnumerable<TenantQuota> GetTenantQuotas(bool all)
    {
        return _quotaService.GetTenantQuotas().Where(q => q.Tenant < 0 && (all || q.Visible)).OrderByDescending(q => q.Tenant).ToList();
    }

    public TenantQuota GetTenantQuota(int tenant)
    {
        var defaultQuota = _quotaService.GetTenantQuota(tenant) ?? _quotaService.GetTenantQuota(Tenant.DefaultTenant) ?? TenantQuota.Default;
        if (defaultQuota.Tenant != tenant && _tariffService != null)
        {
            var tariff = _tariffService.GetTariff(tenant);
            var currentQuota = _quotaService.GetTenantQuota(tariff.QuotaId);
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

        return _tariffService.GetProductPriceInfo(productIds);
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        quota = _quotaService.SaveTenantQuota(quota);

        return quota;
    }

    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        _quotaService.SetTenantQuotaRow(row, exchange);
    }

    public List<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        return _quotaService.FindTenantQuotaRows(tenantId).ToList();
    }
}
