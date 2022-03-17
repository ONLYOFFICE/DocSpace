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


    public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId)
    {
        var store = GetSubsciptionsStore(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscriptions();
        }
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        var store = GetSubsciptionsStore(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscriptions(recipientId, objectId);
        }
    }

    public string[] GetRecipients(int tenant, string sourceID, string actionID, string objectID)
    {
        return _service.GetRecipients(tenant, sourceID, actionID, objectID);
    }

    public string[] GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, bool checkSubscribe)
    {
        return _service.GetSubscriptions(tenant, sourceId, actionId, recipientId, checkSubscribe);
    }

    public SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        var store = GetSubsciptionsStore(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscription(recipientId, objectId);
        }
    }

    public void SaveSubscription(SubscriptionRecord s)
    {
        _service.SaveSubscription(s);
        _notifyRecord.Publish(s, ASC.Common.Caching.CacheNotifyAction.InsertOrUpdate);
    }

    public void RemoveSubscriptions(int tenant, string sourceId, string actionId)
    {
        _service.RemoveSubscriptions(tenant, sourceId, actionId);
        _notifyRecord.Publish(new SubscriptionRecord { Tenant = tenant, SourceId = sourceId, ActionId = actionId }, ASC.Common.Caching.CacheNotifyAction.Remove);
    }

    public void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId)
    {
        _service.RemoveSubscriptions(tenant, sourceId, actionId, objectId);
        _notifyRecord.Publish(new SubscriptionRecord { Tenant = tenant, SourceId = sourceId, ActionId = actionId, ObjectId = objectId }, ASC.Common.Caching.CacheNotifyAction.Remove);
    }

    public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId)
    {
        var store = GetSubsciptionsStore(tenant, sourceId, actionId);
        lock (store)
        {
            return store.GetSubscriptionMethods(recipientId);
        }
    }

    public void SetSubscriptionMethod(SubscriptionMethod m)
    {
        _service.SetSubscriptionMethod(m);
        _notifyMethod.Publish(m, ASC.Common.Caching.CacheNotifyAction.Any);
    }


    private SubsciptionsStore GetSubsciptionsStore(int tenant, string sourceId, string actionId)
    {
        var key = SubscriptionServiceCache.GetKey(tenant, sourceId, actionId);
        var store = _cache.Get<SubsciptionsStore>(key);
        if (store == null)
        {
            var records = _service.GetSubscriptions(tenant, sourceId, actionId);
            var methods = _service.GetSubscriptionMethods(tenant, sourceId, actionId, null);
            store = new SubsciptionsStore(records, methods);
            _cache.Insert(key, store, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return store;
    }
    public bool IsUnsubscribe(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        return _service.IsUnsubscribe(tenant, sourceId, actionId, recipientId, objectId);
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
