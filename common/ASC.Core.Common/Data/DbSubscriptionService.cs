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

namespace ASC.Core.Data;

[Scope]
public class DbSubscriptionService : ISubscriptionService
{
    private readonly IDbContextFactory<UserDbContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public DbSubscriptionService(IDbContextFactory<UserDbContext> dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public string[] GetRecipients(int tenant, string sourceId, string actionId, string objectId)
    {
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        var q = GetQuery(userDbContext, tenant, sourceId, actionId)
            .Where(r => r.Object == (objectId ?? string.Empty))
            .Where(r => !r.Unsubscribed)
            .Select(r => r.Recipient)
            .Distinct();

        return q.ToArray();
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId)
    {
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        using var userDbContext = _dbContextFactory.CreateDbContext();

        var q = GetQuery(userDbContext, tenant, sourceId, actionId);

        return GetSubscriptions(q, tenant);
    }

    public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();

        var q = GetQuery(userDbContext, tenant, sourceId, actionId);

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
        ArgumentNullException.ThrowIfNull(recipientId);

        using var userDbContext = _dbContextFactory.CreateDbContext();

        var q = GetQuery(userDbContext, tenant, sourceId, actionId)
            .Where(r => r.Recipient == recipientId)
            .Where(r => r.Object == (objectId ?? string.Empty));

        return GetSubscriptions(q, tenant).Take(1).FirstOrDefault();
    }

    public bool IsUnsubscribe(int tenant, string sourceId, string actionId, string recipientId, string objectId)
    {
        ArgumentNullException.ThrowIfNull(recipientId);
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        var q = userDbContext.Subscriptions
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
        ArgumentNullException.ThrowIfNull(recipientId);
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        using var userDbContext = _dbContextFactory.CreateDbContext();

        var q = GetQuery(userDbContext, tenant, sourceId, actionId)
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
        ArgumentNullException.ThrowIfNull(s);

        var subs = new Subscription
        {
            Action = s.ActionId,
            Object = s.ObjectId ?? string.Empty,
            Recipient = s.RecipientId,
            Source = s.SourceId,
            Unsubscribed = !s.Subscribed,
            Tenant = s.Tenant
        };

        using var userDbContext = _dbContextFactory.CreateDbContext();
        userDbContext.AddOrUpdate(userDbContext.Subscriptions, subs);
        userDbContext.SaveChanges();
    }

    public void RemoveSubscriptions(int tenant, string sourceId, string actionId)
    {
        RemoveSubscriptions(tenant, sourceId, actionId, string.Empty);
    }

    public void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId)
    {
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        var strategy = userDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var userDbContext = _dbContextFactory.CreateDbContext();
            using var tr = userDbContext.Database.BeginTransaction();
            var q = userDbContext.Subscriptions
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
                userDbContext.Subscriptions.Remove(sub);
            }

            tr.Commit();
        });
    }


    public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId)
    {
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        var q = userDbContext.SubscriptionMethods
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
        ArgumentNullException.ThrowIfNull(m);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        var strategy = userDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var userDbContext = _dbContextFactory.CreateDbContext();
            using var tr = userDbContext.Database.BeginTransaction();

            if (m.Methods == null || m.Methods.Length == 0)
            {
                var q = userDbContext.SubscriptionMethods
                    .Where(r => r.Tenant == m.Tenant)
                    .Where(r => r.Source == m.Source)
                    .Where(r => r.Recipient == m.Recipient)
                    .Where(r => r.Action == m.Action);

                var sm = q.FirstOrDefault();

                if (sm != null)
                {
                    userDbContext.SubscriptionMethods.Remove(sm);
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
                userDbContext.AddOrUpdate(userDbContext.SubscriptionMethods, sm);
            }

            userDbContext.SaveChanges();
            tr.Commit();
        });
    }


    private IQueryable<Subscription> GetQuery(UserDbContext userDbContext, int tenant, string sourceId, string actionId)
    {
        ArgumentNullException.ThrowIfNull(sourceId);
        ArgumentNullException.ThrowIfNull(actionId);

        return
            userDbContext.Subscriptions
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
