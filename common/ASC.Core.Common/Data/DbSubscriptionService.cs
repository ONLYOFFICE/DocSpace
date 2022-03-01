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

namespace ASC.Core.Data;

[Scope]
public class DbSubscriptionService : ISubscriptionService
{
    private readonly Lazy<UserDbContext> _lazyUserDbContext;
    private UserDbContext UserDbContext => _lazyUserDbContext.Value;
    private readonly IMapper _mapper;

    public DbSubscriptionService(DbContextManager<UserDbContext> dbContextManager, IMapper mapper)
    {
        _lazyUserDbContext = new Lazy<UserDbContext>(() => dbContextManager.Value);
        _mapper = mapper;
    }

    public string[] GetRecipients(int tenant, string sourceId, string actionId, string objectId)
    {
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }
        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        var q = GetQuery(tenant, sourceId, actionId)
            .Where(r => r.Object == (objectId ?? string.Empty))
            .Where(r => !r.Unsubscribed)
            .Select(r => r.Recipient)
            .Distinct();

        return q.ToArray();
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId)
    {
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }
        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        var q = GetQuery(tenant, sourceId, actionId);

        return GetSubscriptions(q, tenant);
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        var q = GetQuery(tenant, sourceId, actionId);

        if (recipientId != null)
        {
            q = q.Where(r => r.Recipient == recipientId);
        }
        else
        {
            q = q.Where(r => r.Object == (objectId ?? string.Empty));
        }

        return GetSubscriptions(q, tenant);
    }

    public SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        if (recipientId == null)
        {
            throw new ArgumentNullException(nameof(recipientId));
        }

        var q = GetQuery(tenant, sourceId, actionId)
            .Where(r => r.Recipient == recipientId)
            .Where(r => r.Object == (objectId ?? string.Empty));

        return GetSubscriptions(q, tenant).Take(1).FirstOrDefault();
    }

    public bool IsUnsubscribe(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        if (recipientId == null)
        {
            throw new ArgumentNullException(nameof(recipientId));
        }
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }
        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        var q = UserDbContext.Subscriptions
            .Where(r => r.Source == sourceId &&
                        r.Action == actionId &&
                        r.Tenant == tenant &&
                        r.Recipient == recipientId &&
                        r.Unsubscribed);

        if (!string.IsNullOrEmpty(objectId))
        {
            q = q.Where(r => r.Object == objectId || r.Object == string.Empty);
        }
        else
        {
            q = q.Where(r => r.Object == string.Empty);
        }

        return q.Any();
    }

    public string[] GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, bool checkSubscribe)
    {
        if (recipientId == null)
        {
            throw new ArgumentNullException(nameof(recipientId));
        }
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }
        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        var q = GetQuery(tenant, sourceId, actionId)
            .Where(r => r.Recipient == recipientId)
            .Distinct();

        if (checkSubscribe)
        {
            q = q.Where(r => !r.Unsubscribed);
        }

        return q.Select(r => r.Object).ToArray();
    }


    public void SaveSubscription(SubscriptionRecord s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        var subs = new Subscription
        {
            Action = s.ActionId,
            Object = s.ObjectId ?? string.Empty,
            Recipient = s.RecipientId,
            Source = s.SourceId,
            Unsubscribed = !s.Subscribed,
            Tenant = s.Tenant
        };

        UserDbContext.AddOrUpdate(r => r.Subscriptions, subs);
        UserDbContext.SaveChanges();
    }

    public void RemoveSubscriptions(int tenant, string sourceId, string actionId)
    {
        RemoveSubscriptions(tenant, sourceId, actionId, string.Empty);
    }

    public void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId)
    {
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }

        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        using var tr = UserDbContext.Database.BeginTransaction();
        var q = UserDbContext.Subscriptions
            .Where(r => r.Tenant == tenant)
            .Where(r => r.Source == sourceId)
            .Where(r => r.Action == actionId);

        if (objectId.Length != 0)
        {
            q = q.Where(r => r.Object == (objectId ?? string.Empty));
        }

        var sub = q.FirstOrDefault();

        if (sub != null)
        {
            UserDbContext.Subscriptions.Remove(sub);
        }

        tr.Commit();
    }


    public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId)
    {
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }
        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        var q = UserDbContext.SubscriptionMethods
            .Where(r => r.Tenant == -1 || r.Tenant == tenant)
            .Where(r => r.Source == sourceId);

        if (recipientId != null)
        {
            q = q.Where(r => r.Recipient == recipientId);
        }

        var a = q
            .OrderBy(r => r.Tenant)
            .Distinct();


        var methods = a.ToList();
        var result = new List<SubscriptionMethod>();
        var common = new Dictionary<string, SubscriptionMethod>();

        foreach (var r in methods)
        {
            var m = _mapper.Map<DbSubscriptionMethod, SubscriptionMethod>(r);
            var key = m.Source + m.Action + m.Recipient;
            if (m.Tenant == Tenant.DefaultTenant)
            {
                m.Tenant = tenant;
                common.Add(key, m);
                result.Add(m);
            }
            else
            {
                if (!common.TryGetValue(key, out var rec))
                {
                    result.Add(rec);
                }
            }
        }
        return result;
    }

    public void SetSubscriptionMethod(SubscriptionMethod m)
    {
        if (m == null)
        {
            throw new ArgumentNullException(nameof(m));
        }

        using var tr = UserDbContext.Database.BeginTransaction();

        if (m.Methods == null || m.Methods.Length == 0)
        {
            var q = UserDbContext.SubscriptionMethods
                .Where(r => r.Tenant == m.Tenant)
                .Where(r => r.Source == m.Source)
                .Where(r => r.Recipient == m.Recipient)
                .Where(r => r.Action == m.Action);

            var sm = q.FirstOrDefault();

            if (sm != null)
            {
                UserDbContext.SubscriptionMethods.Remove(sm);
            }
        }
        else
        {
            var sm = new DbSubscriptionMethod
            {
                Action = m.Action,
                Recipient = m.Recipient,
                Source = m.Source,
                Tenant = m.Tenant,
                Sender = string.Join("|", m.Methods)
            };
            UserDbContext.AddOrUpdate(r => r.SubscriptionMethods, sm);
        }

        UserDbContext.SaveChanges();
        tr.Commit();
    }


    private IQueryable<Subscription> GetQuery(int tenant, string sourceId, string actionId)
    {
        if (sourceId == null)
        {
            throw new ArgumentNullException(nameof(sourceId));
        }
        if (actionId == null)
        {
            throw new ArgumentNullException(nameof(actionId));
        }

        return
            UserDbContext.Subscriptions
            .Where(r => r.Source == sourceId)
            .Where(r => r.Action == actionId)
            .Where(r => r.Tenant == -1 || r.Tenant == tenant)
            .OrderBy(r => r.Tenant);
    }

    private IEnumerable<SubscriptionRecord> GetSubscriptions(IQueryable<Subscription> q, int tenant)
    {
        var subs = q.ToList();
        var result = new List<SubscriptionRecord>();
        var common = new Dictionary<string, SubscriptionRecord>();

        foreach (var r in subs)
        {
            var s = _mapper.Map<Subscription, SubscriptionRecord>(r);
            var key = s.SourceId + s.ActionId + s.RecipientId + s.ObjectId;
            if (s.Tenant == Tenant.DefaultTenant)
            {
                s.Tenant = tenant;
                common.Add(key, s);
                result.Add(s);
            }
            else
            {
                if (!common.TryGetValue(key, out _))
                {
                    result.Add(s);
                }
            }
        }

        return result;
    }
}
