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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Security.Authorizing;
using ASC.Core.Caching;

namespace ASC.Core
{
    [Scope]
    public class SubscriptionManager
    {
        private readonly ISubscriptionService service;

        private TenantManager TenantManager { get; }

        private ICache Cache { get; set; }
        public static readonly object CacheLocker = new object();
        public static readonly List<Guid> Groups = Groups = new List<Guid>
            {
                Constants.Admin.ID,
                Constants.Everyone.ID,
                Constants.User.ID
            };

        public SubscriptionManager(CachedSubscriptionService service, TenantManager tenantManager, ICache cache)
        {
            this.service = service ?? throw new ArgumentNullException("subscriptionManager");
            TenantManager = tenantManager;
            Cache = cache;
        }


        public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            var s = new SubscriptionRecord
            {
                Tenant = GetTenant(),
                Subscribed = true,
            };

            if (sourceID != null)
            {
                s.SourceId = sourceID;
            }

            if (actionID != null)
            {
                s.ActionId = actionID;
            }

            if (recipientID != null)
            {
                s.RecipientId = recipientID;
            }

            if (objectID != null)
            {
                s.ObjectId = objectID;
            }

            service.SaveSubscription(s);
        }

        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            var s = new SubscriptionRecord
            {
                Tenant = GetTenant(),
                Subscribed = false,
            };

            if (sourceID != null)
            {
                s.SourceId = sourceID;
            }

            if (actionID != null)
            {
                s.ActionId = actionID;
            }

            if (recipientID != null)
            {
                s.RecipientId = recipientID;
            }

            if (objectID != null)
            {
                s.ObjectId = objectID;
            }

            service.SaveSubscription(s);
        }

        public void UnsubscribeAll(string sourceID, string actionID, string objectID)
        {
            service.RemoveSubscriptions(GetTenant(), sourceID, actionID, objectID);
        }

        public void UnsubscribeAll(string sourceID, string actionID)
        {
            service.RemoveSubscriptions(GetTenant(), sourceID, actionID);
        }

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            IEnumerable<SubscriptionMethod> methods;

            if (Groups.Any(r => r.ToString() == recipientID))
            {
                methods = GetDefaultSubscriptionMethodsFromCache(sourceID, actionID, recipientID);
            }
            else
            {
                methods = service.GetSubscriptionMethods(GetTenant(), sourceID, actionID, recipientID);
            }

            var m = methods
                .FirstOrDefault(x => x.ActionId.Equals(actionID, StringComparison.OrdinalIgnoreCase));

            if (m == null)
            {
                m = methods.FirstOrDefault();
            }

            return m != null ? m.Methods : Array.Empty<string>();
        }

        public string[] GetRecipients(string sourceID, string actionID, string objectID)
        {
            return service.GetRecipients(GetTenant(), sourceID, actionID, objectID);
        }

        public object GetSubscriptionRecord(string sourceID, string actionID, string recipientID, string objectID)
        {
            return service.GetSubscription(GetTenant(), sourceID, actionID, recipientID, objectID);
        }

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID, bool checkSubscribe = true)
        {
            return service.GetSubscriptions(GetTenant(), sourceID, actionID, recipientID, checkSubscribe);
        }

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
        {
            return service.IsUnsubscribe(GetTenant(), sourceID, actionID, recipientID, objectID);
        }

        public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            var m = new SubscriptionMethod
            {
                Tenant = GetTenant()
            };

            if (sourceID != null)
            {
                m.SourceId = sourceID;
            }

            if (actionID != null)
            {
                m.ActionId = actionID;
            }

            if (recipientID != null)
            {
                m.RecipientId = recipientID;
            }

            if (senderNames != null)
            {
                m.Methods = senderNames;
            }


            service.SetSubscriptionMethod(m);
        }

        private IEnumerable<SubscriptionMethod> GetDefaultSubscriptionMethodsFromCache(string sourceID, string actionID, string recepient)
        {
            lock (CacheLocker)
            {
                var key = $"subs|-1{sourceID}{actionID}{recepient}";
                var result = Cache.Get<IEnumerable<SubscriptionMethod>>(key);
                if (result == null)
                {
                    result = service.GetSubscriptionMethods(-1, sourceID, actionID, recepient);
                    Cache.Insert(key, result, DateTime.UtcNow.AddDays(1));
                }

                return result;
            }
        }


        private int GetTenant()
        {
            return TenantManager.GetCurrentTenant().TenantId;
        }
    }
}
