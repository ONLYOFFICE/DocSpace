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

using Constants = ASC.Common.Security.Authorizing.Constants;

namespace ASC.Core;

[Scope]
public class SubscriptionManager
{
    private readonly ISubscriptionService _service;
    private readonly TenantManager _tenantManager;
    private readonly ICache _cache;
    public static readonly object CacheLocker = new object();
    public static readonly List<Guid> Groups = Groups = new List<Guid>
    {
        Constants.DocSpaceAdmin.ID,
        Constants.Everyone.ID,
        Constants.RoomAdmin.ID
    };

    public SubscriptionManager(CachedSubscriptionService service, TenantManager tenantManager, ICache cache)
    {
        _service = service ?? throw new ArgumentNullException("subscriptionManager");
        _tenantManager = tenantManager;
        _cache = cache;
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

        _service.SaveSubscription(s);
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

        _service.SaveSubscription(s);
    }

    public void UnsubscribeAll(string sourceID, string actionID, string objectID)
    {
        _service.RemoveSubscriptions(GetTenant(), sourceID, actionID, objectID);
    }

    public void UnsubscribeAll(string sourceID, string actionID)
    {
        _service.RemoveSubscriptions(GetTenant(), sourceID, actionID);
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
            methods = _service.GetSubscriptionMethods(GetTenant(), sourceID, actionID, recipientID);
        }

        var m = methods
            .FirstOrDefault(x => x.Action.Equals(actionID, StringComparison.OrdinalIgnoreCase));

        if (m == null)
        {
            m = methods.FirstOrDefault();
        }

        return m != null ? m.Methods : Array.Empty<string>();
    }

    public string[] GetRecipients(string sourceID, string actionID, string objectID)
    {
        return _service.GetRecipients(GetTenant(), sourceID, actionID, objectID);
    }

    public object GetSubscriptionRecord(string sourceID, string actionID, string recipientID, string objectID)
    {
        return _service.GetSubscription(GetTenant(), sourceID, actionID, recipientID, objectID);
    }

    public string[] GetSubscriptions(string sourceID, string actionID, string recipientID, bool checkSubscribe = true)
    {
        return _service.GetSubscriptions(GetTenant(), sourceID, actionID, recipientID, checkSubscribe);
    }

    public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
    {
        return _service.IsUnsubscribe(GetTenant(), sourceID, actionID, recipientID, objectID);
    }

    public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
    {
        var m = new SubscriptionMethod
        {
            Tenant = GetTenant()
        };

        if (sourceID != null)
        {
            m.Source = sourceID;
        }

        if (actionID != null)
        {
            m.Action = actionID;
        }

        if (recipientID != null)
        {
            m.Recipient = recipientID;
        }

        if (senderNames != null)
        {
            m.Methods = senderNames;
        }


        _service.SetSubscriptionMethod(m);
    }

    private IEnumerable<SubscriptionMethod> GetDefaultSubscriptionMethodsFromCache(string sourceID, string actionID, string recepient)
    {
        lock (CacheLocker)
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

    private int GetTenant()
    {
        return _tenantManager.GetCurrentTenant().Id;
    }
}
