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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    class DbSubscriptionService : ISubscriptionService
    {
        private Expression<Func<Subscription, SubscriptionRecord>> FromSubscriptionToSubscriptionRecord { get; set; }
        private Expression<Func<DbSubscriptionMethod, SubscriptionMethod>> FromDbSubscriptionMethodToSubscriptionMethod { get; set; }
        private UserDbContext UserDbContext { get; set; }
        public DbSubscriptionService(DbContextManager<UserDbContext> dbContextManager)
        {
            UserDbContext = dbContextManager.Value;

            FromSubscriptionToSubscriptionRecord = r => new SubscriptionRecord
            {
                ActionId = r.Action,
                ObjectId = r.Object,
                RecipientId = r.Recipient.ToString(),
                SourceId = r.Source.ToString(),
                Subscribed = !r.Unsubscribed,
                Tenant = r.Tenant
            };

            FromDbSubscriptionMethodToSubscriptionMethod = r => new SubscriptionMethod
            {
                ActionId = r.Action,
                RecipientId = r.Recipient.ToString(),
                SourceId = r.Source.ToString(),
                Tenant = r.Tenant,
                Methods = r.Sender.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }


        public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

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
            if (recipientId == null) throw new ArgumentNullException("recipientId");

            var q = GetQuery(tenant, sourceId, actionId)
                .Where(r => r.Recipient == recipientId)
                .Where(r => r.Object == (objectId ?? string.Empty));

            return GetSubscriptions(q, tenant).FirstOrDefault();
        }

        public void SaveSubscription(SubscriptionRecord s)
        {
            if (s == null) throw new ArgumentNullException("s");

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
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            using var tr = UserDbContext.Database.BeginTransaction();
            var q = UserDbContext.Subscriptions
                .Where(r => r.Tenant == tenant)
                .Where(r => r.Source == sourceId)
                .Where(r => r.Action == actionId);

            if (objectId != string.Empty)
            {
                q = q.Where(r => r.Object == (objectId ?? string.Empty));
            }

            var sub = q.FirstOrDefault();
            UserDbContext.Subscriptions.Remove(sub);
            tr.Commit();
        }


        public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            var q = UserDbContext.SubscriptionMethods
                .Where(r => r.Tenant == -1 || r.Tenant == tenant)
                .Where(r => r.Source == sourceId);

            if (recipientId != null)
            {
                q = q.Where(r => r.Recipient == recipientId);
            }

            var a = q.OrderBy(r => r.Tenant)
            .GroupBy(r => new { r.Recipient, r.Action }, (x, y) => y.First());


            var methods = a.Select(FromDbSubscriptionMethodToSubscriptionMethod).ToList();

            var result = methods.ToList();

            var common = new Dictionary<string, SubscriptionMethod>();
            foreach (var m in methods)
            {
                var key = m.SourceId + m.ActionId + m.RecipientId;
                if (m.Tenant == Tenant.DEFAULT_TENANT)
                {
                    m.Tenant = tenant;
                    common.Add(key, m);
                }
                else
                {
                    if (common.TryGetValue(key, out var r))
                    {
                        result.Remove(r);
                    }
                }
            }
            return result;
        }

        public void SetSubscriptionMethod(SubscriptionMethod m)
        {
            if (m == null) throw new ArgumentNullException("m");

            using var tr = UserDbContext.Database.BeginTransaction();

            if (m.Methods == null || m.Methods.Length == 0)
            {
                var q = UserDbContext.SubscriptionMethods
                    .Where(r => r.Tenant == m.Tenant)
                    .Where(r => r.Source == m.SourceId)
                    .Where(r => r.Recipient == m.RecipientId)
                    .Where(r => r.Action == m.ActionId);
                UserDbContext.SubscriptionMethods.Remove(q.FirstOrDefault());
            }
            else
            {
                var sm = new DbSubscriptionMethod
                {
                    Action = m.ActionId,
                    Recipient = m.RecipientId,
                    Source = m.SourceId,
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
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            return
                UserDbContext.Subscriptions
                .Where(r => r.Source == sourceId)
                .Where(r => r.Action == actionId)
                .Where(r => r.Tenant == -1 || r.Tenant == tenant)
                .OrderBy(r => r.Tenant)
                ;
        }

        private IEnumerable<SubscriptionRecord> GetSubscriptions(IQueryable<Subscription> q, int tenant)
        {
            var subs = q.Select(FromSubscriptionToSubscriptionRecord).ToList();

            var result = subs.ToList();
            var common = new Dictionary<string, SubscriptionRecord>();
            foreach (var s in subs)
            {
                var key = s.SourceId + s.ActionId + s.RecipientId + s.ObjectId;
                if (s.Tenant == Tenant.DEFAULT_TENANT)
                {
                    s.Tenant = tenant;
                    common.Add(key, s);
                }
                else
                {
                    if (common.TryGetValue(key, out var r))
                    {
                        result.Remove(r);
                    }
                }
            }
            return result;
        }
    }
}
