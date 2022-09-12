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

namespace ASC.Core.Caching;

[Singletone]
class QuotaServiceCache
{
    internal const string KeyQuota = "quota";
    internal const string KeyQuotaRows = "quotarows";
    internal const string KeyUserQuotaRows = "userquotarows";
    internal readonly ICache Cache;
    internal readonly ICacheNotify<QuotaCacheItem> CacheNotify;
    internal readonly bool QuotaCacheEnabled;

    public QuotaServiceCache(IConfiguration Configuration, ICacheNotify<QuotaCacheItem> cacheNotify, ICache cache)
    {
        if (Configuration["core:enable-quota-cache"] == null)
        {
            QuotaCacheEnabled = true;
        }
        else
        {
            QuotaCacheEnabled = !bool.TryParse(Configuration["core:enable-quota-cache"], out var enabled) || enabled;
        }

        CacheNotify = cacheNotify;
        Cache = cache;

        cacheNotify.Subscribe((i) =>
        {
            if (i.Key == KeyQuota)
            {
                Cache.Remove(KeyQuota);
            }
            else
            {
                Cache.Remove(i.Key);
            }
        }, CacheNotifyAction.Any);
    }
}

[Scope]
class CachedQuotaService : IQuotaService
{
    internal IQuotaService Service { get; set; }
    internal ICache Cache { get; set; }
    internal ICacheNotify<QuotaCacheItem> CacheNotify { get; set; }
    internal QuotaServiceCache QuotaServiceCache { get; set; }

    private readonly TimeSpan _cacheExpiration;

    public CachedQuotaService()
    {
        _cacheExpiration = TimeSpan.FromMinutes(10);
    }

    public CachedQuotaService(DbQuotaService service, QuotaServiceCache quotaServiceCache) : this()
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        QuotaServiceCache = quotaServiceCache;
        Cache = quotaServiceCache.Cache;
        CacheNotify = quotaServiceCache.CacheNotify;
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        var quotas = Cache.Get<IEnumerable<TenantQuota>>(QuotaServiceCache.KeyQuota);
        if (quotas == null)
        {
            quotas = Service.GetTenantQuotas();
            if (QuotaServiceCache.QuotaCacheEnabled)
            {
                Cache.Insert(QuotaServiceCache.KeyQuota, quotas, DateTime.UtcNow.Add(_cacheExpiration));
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
        var q = Service.SaveTenantQuota(quota);
        CacheNotify.Publish(new QuotaCacheItem { Key = QuotaServiceCache.KeyQuota }, CacheNotifyAction.Any);

        return q;
    }

    public void RemoveTenantQuota(int tenant)
    {
        throw new NotImplementedException();
    }

    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        Service.SetTenantQuotaRow(row, exchange);
        CacheNotify.Publish(new QuotaCacheItem { Key = GetKey(row.Tenant) }, CacheNotifyAction.InsertOrUpdate);
    }

    public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        var key = GetKey(tenantId);
        var result = Cache.Get<IEnumerable<TenantQuotaRow>>(key);

        if (result == null)
        {
            result = Service.FindTenantQuotaRows(tenantId);
            Cache.Insert(key, result, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return result;
    }

    public void SetUserQuotaRow(UserQuotaRow row, bool exchange)
    {
        Service.SetUserQuotaRow(row, exchange);
        CacheNotify.Publish(new QuotaCacheItem { Key = GetKey(row.Tenant, row.UserId) }, CacheNotifyAction.InsertOrUpdate);
    }

    public IEnumerable<UserQuotaRow> FindUserQuotaRows(int tenantId, Guid userId)
    {
        var key = GetKey(tenantId, userId);
        var result = Cache.Get<IEnumerable<UserQuotaRow>>(key);

        if (result == null)
        {
            result = Service.FindUserQuotaRows(tenantId, userId);
            Cache.Insert(key, result, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return result;
    }

    public string GetKey(int tenant)
    {
        return QuotaServiceCache.KeyQuotaRows + tenant;
    }

    public string GetKey(int tenant, Guid userId)
    {
        return QuotaServiceCache.KeyQuotaRows + tenant + userId;
    }
}
