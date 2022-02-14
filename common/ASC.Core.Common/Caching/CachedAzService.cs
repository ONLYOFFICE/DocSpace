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
class AzServiceCache
{
    internal ICache Cache { get; }
    internal ICacheNotify<AzRecordCache> AzRecordCacheNotify { get; }

    public AzServiceCache(ICacheNotify<AzRecordCache> azRecordCacheNotify, ICache cache)
    {
        AzRecordCacheNotify = azRecordCacheNotify;
        Cache = cache;

        azRecordCacheNotify.Subscribe((r) => UpdateCache(r, true), CacheNotifyAction.Remove);
        azRecordCacheNotify.Subscribe((r) => UpdateCache(r, false), CacheNotifyAction.InsertOrUpdate);
    }

    public static string GetKey(int tenant)
    {
        return $"acl{tenant}";
    }

    private void UpdateCache(AzRecord r, bool remove)
    {
        var aces = Cache.Get<AzRecordStore>(GetKey(r.Tenant));
        if (aces != null)
        {
            lock (aces)
            {
                if (remove)
                {
                    aces.Remove(r);
                }
                else
                {
                    aces.Add(r);
                }
            }
        }
    }
}

[Scope]
class CachedAzService : IAzService
{
    private readonly IAzService _service;
    private readonly ICacheNotify<AzRecordCache> _azRecordCacheNotify;
    private readonly ICache _cache;
    private TimeSpan _cacheExpiration;

    public CachedAzService(DbAzService service, AzServiceCache azServiceCache)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _cache = azServiceCache.Cache;
        _azRecordCacheNotify = azServiceCache.AzRecordCacheNotify;
        _cacheExpiration = TimeSpan.FromMinutes(10);
    }

    public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
    {
        var key = AzServiceCache.GetKey(tenant);
        var aces = _cache.Get<AzRecordStore>(key);
        if (aces == null)
        {
            var records = _service.GetAces(tenant, default);
            _cache.Insert(key, aces = new AzRecordStore(records), DateTime.UtcNow.Add(_cacheExpiration));
        }

        return aces;
    }

    public AzRecord SaveAce(int tenant, AzRecord r)
    {
        r = _service.SaveAce(tenant, r);
        _azRecordCacheNotify.Publish(r, CacheNotifyAction.InsertOrUpdate);

        return r;
    }

    public void RemoveAce(int tenant, AzRecord r)
    {
        _service.RemoveAce(tenant, r);
        _azRecordCacheNotify.Publish(r, CacheNotifyAction.Remove);
    }
}
