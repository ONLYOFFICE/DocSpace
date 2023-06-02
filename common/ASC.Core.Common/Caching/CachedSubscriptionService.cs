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
public class SubscriptionServiceCache
{
    internal readonly ICache Cache;
    internal readonly ICacheNotify<SubscriptionRecord> NotifyRecord;
    internal readonly ICacheNotify<SubscriptionMethodCache> NotifyMethod;

    public SubscriptionServiceCache(ICacheNotify<SubscriptionRecord> notifyRecord, ICacheNotify<SubscriptionMethodCache> notifyMethod, ICache cache)
    {
        Cache = cache;
        NotifyRecord = notifyRecord;
        NotifyMethod = notifyMethod;

        notifyRecord.Subscribe((s) =>
        {
            var store = GetSubsciptionsStore(s.Tenant, s.SourceId, s.ActionId);
            if (store != null)
            {
                lock (store)
                {
                    store.SaveSubscription(s);
                }
            }
        }, CacheNotifyAction.InsertOrUpdate);

        notifyRecord.Subscribe((s) =>
        {
            var store = GetSubsciptionsStore(s.Tenant, s.SourceId, s.ActionId);
            if (store != null)
            {
                lock (store)
                {
                    if (s.ObjectId == null)
                    {
                        store.RemoveSubscriptions();
                    }
                    else
                    {
                        store.RemoveSubscriptions(s.ObjectId);
                    }
                }
            }
        }, CacheNotifyAction.Remove);

        notifyMethod.Subscribe((m) =>
        {
            var store = GetSubsciptionsStore(m.Tenant, m.SourceId, m.ActionId);
            if (store != null)
            {
                lock (store)
                {
                    store.SetSubscriptionMethod(m);
                }
            }
        }, CacheNotifyAction.Any);
    }

    private SubsciptionsStore GetSubsciptionsStore(int tenant, string sourceId, string actionId)
    {
        return Cache.Get<SubsciptionsStore>(GetKey(tenant, sourceId, actionId));
    }

    public static string GetKey(int tenant, string sourceId, string actionId)
    {
        return string.Format("sub/{0}/{1}/{2}", tenant, sourceId, actionId);
    }
}

[Scope]
public class CachedSubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionService _service;
    private readonly ICache _cache;
    private readonly ICacheNotify<SubscriptionRecord> _notifyRecord;
    private readonly ICacheNotify<SubscriptionMethodCache> _notifyMethod;
    private readonly TimeSpan _cacheExpiration;

    public CachedSubscriptionService(DbSubscriptionService service, SubscriptionServiceCache subscriptionServiceCache)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _cache = subscriptionServiceCache.Cache;
        _notifyRecord = subscriptionServiceCache.NotifyRecord;
        _notifyMethod = subscriptionServiceCache.NotifyMethod;
        _cacheExpiration = TimeSpan.FromMinutes(5);
    }


    public async Task<IEnumerable<SubscriptionRecord>> GetSubscriptionsAsync(int tenant, string sourceId, string actionId)
    {
        var store = await GetSubsciptionsStoreAsync(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscriptions();
        }
    }

    public async Task<IEnumerable<SubscriptionRecord>> GetSubscriptionsAsync(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        var store = await GetSubsciptionsStoreAsync(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscriptions(recipientId, objectId);
        }
    }

    public async Task<string[]> GetRecipientsAsync(int tenant, string sourceID, string actionID, string objectID)
    {
        return await _service.GetRecipientsAsync(tenant, sourceID, actionID, objectID);
    }

    public async Task<string[]> GetSubscriptionsAsync(int tenant, string sourceId, string actionId, string recipientId, bool checkSubscribe)
    {
        return await _service.GetSubscriptionsAsync(tenant, sourceId, actionId, recipientId, checkSubscribe);
    }

    public async Task<SubscriptionRecord> GetSubscriptionAsync(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        var store = await GetSubsciptionsStoreAsync(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscription(recipientId, objectId);
        }
    }

    public async Task SaveSubscriptionAsync(SubscriptionRecord s)
    {
        await _service.SaveSubscriptionAsync(s);
        _notifyRecord.Publish(s, CacheNotifyAction.InsertOrUpdate);
    }

    public async Task RemoveSubscriptionsAsync(int tenant, string sourceId, string actionId)
    {
        await _service.RemoveSubscriptionsAsync(tenant, sourceId, actionId);
        _notifyRecord.Publish(new SubscriptionRecord { Tenant = tenant, SourceId = sourceId, ActionId = actionId }, CacheNotifyAction.Remove);
    }

    public async Task RemoveSubscriptionsAsync(int tenant, string sourceId, string actionId, string objectId)
    {
        await _service.RemoveSubscriptionsAsync(tenant, sourceId, actionId, objectId);
        _notifyRecord.Publish(new SubscriptionRecord { Tenant = tenant, SourceId = sourceId, ActionId = actionId, ObjectId = objectId }, CacheNotifyAction.Remove);
    }

    public async Task<IEnumerable<SubscriptionMethod>> GetSubscriptionMethodsAsync(int tenant, string sourceId, string actionId, string recipientId)
    {
        var store = await GetSubsciptionsStoreAsync(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscriptionMethods(recipientId);
        }
    }

    public async Task SetSubscriptionMethodAsync(SubscriptionMethod m)
    {
        await _service.SetSubscriptionMethodAsync(m);
        _notifyMethod.Publish(m, CacheNotifyAction.Any);
    }


    private async Task<SubsciptionsStore> GetSubsciptionsStoreAsync(int tenant, string sourceId, string actionId)
    {
        var key = SubscriptionServiceCache.GetKey(tenant, sourceId, actionId);
        var store = _cache.Get<SubsciptionsStore>(key);
        if (store == null)
        {
            var records = await _service.GetSubscriptionsAsync(tenant, sourceId, actionId);
            var methods = await _service.GetSubscriptionMethodsAsync(tenant, sourceId, actionId, null);
            store = new SubsciptionsStore(records, methods);
            _cache.Insert(key, store, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return store;
    }
    public async Task<bool> IsUnsubscribeAsync(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        return await _service.IsUnsubscribeAsync(tenant, sourceId, actionId, recipientId, objectId);
    }
}

internal class SubsciptionsStore
{
    private readonly List<SubscriptionRecord> _records;
    private IDictionary<string, List<SubscriptionRecord>> _recordsByRec;
    private IDictionary<string, List<SubscriptionRecord>> _recordsByObj;

    private readonly List<SubscriptionMethod> _methods;
    private IDictionary<string, List<SubscriptionMethod>> _methodsByRec;

    public SubsciptionsStore(IEnumerable<SubscriptionRecord> records, IEnumerable<SubscriptionMethod> methods)
    {
        _records = records.ToList();
        _methods = methods.ToList();
        BuildSubscriptionsIndex(records);
        BuildMethodsIndex(methods);
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions()
    {
        return _records.ToList();
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions(string recipientId, string objectId)
    {
        return recipientId != null ?
            _recordsByRec.ContainsKey(recipientId) ? _recordsByRec[recipientId].ToList() : new List<SubscriptionRecord>() :
            _recordsByObj.ContainsKey(objectId ?? string.Empty) ? _recordsByObj[objectId ?? string.Empty].ToList() : new List<SubscriptionRecord>();
    }

    public SubscriptionRecord GetSubscription(string recipientId, string objectId)
    {
        return _recordsByRec.ContainsKey(recipientId) ?
            _recordsByRec[recipientId].Where(s => s.ObjectId == (objectId ?? "")).FirstOrDefault() :
            null;
    }

    public void SaveSubscription(SubscriptionRecord s)
    {
        var old = GetSubscription(s.RecipientId, s.ObjectId);
        if (old != null)
        {
            old.Subscribed = s.Subscribed;
        }
        else
        {
            _records.Add(s);
            BuildSubscriptionsIndex(_records);
        }
    }

    public void RemoveSubscriptions()
    {
        _records.Clear();
        BuildSubscriptionsIndex(_records);
    }

    public void RemoveSubscriptions(string objectId)
    {
        _records.RemoveAll(s => s.ObjectId == (objectId ?? ""));
        BuildSubscriptionsIndex(_records);
    }

    public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(string recipientId)
    {
        return string.IsNullOrEmpty(recipientId) ?
            _methods.ToList() :
            _methodsByRec.ContainsKey(recipientId) ? _methodsByRec[recipientId].ToList() : new List<SubscriptionMethod>();
    }

    public void SetSubscriptionMethod(SubscriptionMethod m)
    {
        _methods.RemoveAll(r => r.Tenant == m.Tenant && r.Source == m.Source && r.Action == m.Action && r.Recipient == m.Recipient);
        if (m.Methods != null && 0 < m.Methods.Length)
        {
            _methods.Add(m);
        }

        BuildMethodsIndex(_methods);
    }

    private void BuildSubscriptionsIndex(IEnumerable<SubscriptionRecord> records)
    {
        _recordsByRec = records.GroupBy(r => r.RecipientId).ToDictionary(g => g.Key, g => g.ToList());
        _recordsByObj = records.GroupBy(r => r.ObjectId ?? string.Empty).ToDictionary(g => g.Key, g => g.ToList());
    }

    private void BuildMethodsIndex(IEnumerable<SubscriptionMethod> methods)
    {
        _methodsByRec = methods.GroupBy(r => r.Recipient).ToDictionary(g => g.Key, g => g.ToList());
    }
}
