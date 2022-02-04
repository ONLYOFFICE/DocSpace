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

namespace ASC.Core
{
    [Scope]
    public class SubscriptionManager
    {
        public static readonly object cacheLocker;
        public static readonly List<Guid> groups;

        private readonly ISubscriptionService _service;
        private readonly TenantManager _tenantManager;
        private ICache _cache;

        static SubscriptionManager()
        {
            cacheLocker = new object();
            groups = new List<Guid>
            {
                Constants.Admin.ID,
                Constants.Everyone.ID,
                Constants.User.ID
            };
        }

        public SubscriptionManager(ISubscriptionService service, TenantManager tenantManager, ICache cache)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tenantManager = tenantManager;
            _cache = cache;
        }

        public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            var s = new SubscriptionRecord
            {
                Tenant = GetTenant(),
                SourceId = sourceID,
                ActionId = actionID,
                RecipientId = recipientID,
                ObjectId = objectID,
                Subscribed = true,
            };

            _service.SaveSubscription(s);
        }

        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            var s = new SubscriptionRecord
            {
                Tenant = GetTenant(),
                SourceId = sourceID,
                ActionId = actionID,
                RecipientId = recipientID,
                ObjectId = objectID,
                Subscribed = false,
            };

            _service.SaveSubscription(s);
        }

        public void UnsubscribeAll(string sourceID, string actionID, string objectID) =>
            _service.RemoveSubscriptions(GetTenant(), sourceID, actionID, objectID);

        public void UnsubscribeAll(string sourceID, string actionID) =>
            _service.RemoveSubscriptions(GetTenant(), sourceID, actionID);

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            IEnumerable<SubscriptionMethod> methods;

            if (groups.Any(r => r.ToString() == recipientID))
                methods = GetDefaultSubscriptionMethodsFromCache(sourceID, actionID, recipientID);
            else
                methods = _service.GetSubscriptionMethods(GetTenant(), sourceID, actionID, recipientID);

            var m = methods
                .FirstOrDefault(x => x.Action.Equals(actionID, StringComparison.OrdinalIgnoreCase));

            if (m == null) m = methods.FirstOrDefault();

            return m != null ? m.Methods : Array.Empty<string>();
        }

        public string[] GetRecipients(string sourceID, string actionID, string objectID) =>
            _service.GetRecipients(GetTenant(), sourceID, actionID, objectID);

        public object GetSubscriptionRecord(string sourceID, string actionID, string recipientID, string objectID) =>
            _service.GetSubscription(GetTenant(), sourceID, actionID, recipientID, objectID);

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID, bool checkSubscribe = true) =>
            _service.GetSubscriptions(GetTenant(), sourceID, actionID, recipientID, checkSubscribe);

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID) =>
            _service.IsUnsubscribe(GetTenant(), sourceID, actionID, recipientID, objectID);

        public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            var m = new SubscriptionMethod
            {
                Tenant = GetTenant(),
                SourceId = sourceID,
                Action = actionID,
                Recipient = recipientID,
                Methods = senderNames,
            };

            _service.SetSubscriptionMethod(m);
        }

        private IEnumerable<SubscriptionMethod> GetDefaultSubscriptionMethodsFromCache(string sourceID, string actionID, string recepient)
        {
            lock (cacheLocker)
            {
                var key = $"subs|-1{sourceID}{actionID}{recepient}";
                var result = _cache.Get<IEnumerable<SubscriptionMethod>>(key);
                if (result == null)
                {
                    result = _service.GetSubscriptionMethods(-1, sourceID, actionID, recepient);
                    _cache.Insert(key, result, DateTime.UtcNow.AddDays(1));
                }

                return result;
            }
        }

        private int GetTenant() => _tenantManager.GetCurrentTenant().Id;
    }
}