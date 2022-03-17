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

namespace ASC.Core.Caching;

[Singletone]
class QuotaServiceCache
{
    internal const string _keyQuota = "quota";
    internal const string _keyQuotaRows = "quotarows";

    internal TrustInterval _interval;
    internal readonly ICache _cache;
    internal readonly ICacheNotify<QuotaCacheItem> _cacheNotify;
    internal readonly bool _quotaCacheEnabled;

    public QuotaServiceCache(IConfiguration Configuration, ICacheNotify<QuotaCacheItem> cacheNotify, ICache cache)
    {
        if (Configuration["core:enable-quota-cache"] == null)
        {
            _quotaCacheEnabled = true;
        }
        else
        {
            _quotaCacheEnabled = !bool.TryParse(Configuration["core:enable-quota-cache"], out var enabled) || enabled;
        }

        _cacheNotify = cacheNotify;
        _cache = cache;
        _interval = new TrustInterval();

        cacheNotify.Subscribe((i) =>
        {
            if (i.Key == _keyQuota)
            {
                _cache.Remove(_keyQuota);
            }
            else
            {
                _cache.Remove(i.Key);
            }
        }, CacheNotifyAction.Any);
    }
}

[Scope]
class ConfigureCachedQuotaService : IConfigureNamedOptions<CachedQuotaService>
{
    private readonly IOptionsSnapshot<DbQuotaService> _service;
    private readonly QuotaServiceCache _quotaServiceCache;

    public ConfigureCachedQuotaService(
        IOptionsSnapshot<DbQuotaService> service,
        QuotaServiceCache quotaServiceCache)
    {
        _service = service;
        _quotaServiceCache = quotaServiceCache;
    }

    public void Configure(string name, CachedQuotaService options)
    {
        Configure(options);
        options._service = _service.Get(name);
    }

    public void Configure(CachedQuotaService options)
    {
        options._service = _service.Value;
        options._quotaServiceCache = _quotaServiceCache;
        options._cache = _quotaServiceCache._cache;
        options._cacheNotify = _quotaServiceCache._cacheNotify;
    }
}

[Scope]
class CachedQuotaService : IQuotaService
{
    internal IQuotaService _service;
    internal ICache _cache;
    internal ICacheNotify<QuotaCacheItem> _cacheNotify;
    internal TrustInterval _interval;

    internal TimeSpan _cacheExpiration;
    internal QuotaServiceCache _quotaServiceCache;

    public CachedQuotaService()
    {
        _interval = new TrustInterval();
        _cacheExpiration = TimeSpan.FromMinutes(10);
    }

    public CachedQuotaService(DbQuotaService service, QuotaServiceCache quotaServiceCache) : this()
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _quotaServiceCache = quotaServiceCache;
        _cache = quotaServiceCache._cache;
        _cacheNotify = quotaServiceCache._cacheNotify;
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        var quotas = _cache.Get<IEnumerable<TenantQuota>>(QuotaServiceCache._keyQuota);
        if (quotas == null)
        {
            quotas = _service.GetTenantQuotas();
            if (_quotaServiceCache._quotaCacheEnabled)
            {
                _cache.Insert(QuotaServiceCache._keyQuota, quotas, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return quotas;
    }

    public TenantQuota GetTenantQuota(int tenant)
    {
        return GetTenantQuotas().SingleOrDefault(q => q.Tenant == tenant);
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        var q = _service.SaveTenantQuota(quota);
        _cacheNotify.Publish(new QuotaCacheItem { Key = QuotaServiceCache._keyQuota }, CacheNotifyAction.Any);

        return q;
    }

    public void RemoveTenantQuota(int tenant)
    {
        throw new NotImplementedException();
    }

    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        _service.SetTenantQuotaRow(row, exchange);
        _cacheNotify.Publish(new QuotaCacheItem { Key = GetKey(row.Tenant) }, CacheNotifyAction.InsertOrUpdate);
    }

    public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        var key = GetKey(tenantId);
        var result = _cache.Get<IEnumerable<TenantQuotaRow>>(key);

        if (result == null)
        {
            result = _service.FindTenantQuotaRows(tenantId);
            _cache.Insert(key, result, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return result;
    }

    public string GetKey(int tenant)
    {
        return QuotaServiceCache._keyQuotaRows + tenant;
    }
}
