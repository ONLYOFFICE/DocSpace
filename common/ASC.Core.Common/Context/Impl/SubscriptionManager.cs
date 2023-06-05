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
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    public static readonly List<Guid> Groups = Groups = new List<Guid>
    {
        Constants.DocSpaceAdmin.ID,
        Constants.Everyone.ID,
        Constants.RoomAdmin.ID,
        Constants.Collaborator.ID,
    };

    public SubscriptionManager(CachedSubscriptionService service, TenantManager tenantManager, ICache cache)
    {
        _service = service ?? throw new ArgumentNullException("subscriptionManager");
        _tenantManager = tenantManager;
        _cache = cache;
    }

    public async Task SubscribeAsync(string sourceID, string actionID, string objectID, string recipientID)
    {
        var s = new SubscriptionRecord
        {
            Tenant = await GetTenantAsync(),
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

        await _service.SaveSubscriptionAsync(s);
    }

    public async Task UnsubscribeAsync(string sourceID, string actionID, string objectID, string recipientID)
    {
        var s = new SubscriptionRecord
        {
            Tenant = await GetTenantAsync(),
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

        await _service.SaveSubscriptionAsync(s);
    }

    public async Task UnsubscribeAllAsync(string sourceID, string actionID, string objectID)
    {
        await _service.RemoveSubscriptionsAsync(await GetTenantAsync(), sourceID, actionID, objectID);
    }

    public async Task UnsubscribeAllAsync(string sourceID, string actionID)
    {
        await _service.RemoveSubscriptionsAsync(await GetTenantAsync(), sourceID, actionID);
    }

    public async Task<string[]> GetSubscriptionMethodAsync(string sourceID, string actionID, string recipientID)
    {
        IEnumerable<SubscriptionMethod> methods;

        if (Groups.Any(r => r.ToString() == recipientID))
        {
            methods = await GetDefaultSubscriptionMethodsFromCacheAsync(sourceID, actionID, recipientID);
        }
        else
        {
            methods = await _service.GetSubscriptionMethodsAsync(await GetTenantAsync(), sourceID, actionID, recipientID);
        }

        var m = methods
            .FirstOrDefault(x => x.Action.Equals(actionID, StringComparison.OrdinalIgnoreCase));

        if (m == null)
        {
            m = methods.FirstOrDefault();
        }

        return m != null ? m.Methods : Array.Empty<string>();
    }

    public async Task<string[]> GetRecipientsAsync(string sourceID, string actionID, string objectID)
    {
        return await _service.GetRecipientsAsync(await GetTenantAsync(), sourceID, actionID, objectID);
    }

    public async Task<object> GetSubscriptionRecordAsync(string sourceID, string actionID, string recipientID, string objectID)
    {
        return await _service.GetSubscriptionAsync(await GetTenantAsync(), sourceID, actionID, recipientID, objectID);
    }

    public async Task<string[]> GetSubscriptionsAsync(string sourceID, string actionID, string recipientID, bool checkSubscribe = true)
    {
        return await _service.GetSubscriptionsAsync(await GetTenantAsync(), sourceID, actionID, recipientID, checkSubscribe);
    }

    public async Task<bool> IsUnsubscribeAsync(string sourceID, string recipientID, string actionID, string objectID)
    {
        return await _service.IsUnsubscribeAsync(await GetTenantAsync(), sourceID, actionID, recipientID, objectID);
    }

    public async Task UpdateSubscriptionMethodAsync(string sourceID, string actionID, string recipientID, string[] senderNames)
    {
        var m = new SubscriptionMethod
        {
            Tenant = await GetTenantAsync()
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


        await _service.SetSubscriptionMethodAsync(m);
    }

    private async Task<IEnumerable<SubscriptionMethod>> GetDefaultSubscriptionMethodsFromCacheAsync(string sourceID, string actionID, string recepient)
    {
        try
        {
            await _semaphore.WaitAsync();
            var key = $"subs|-1{sourceID}{actionID}{recepient}";
            var result = _cache.Get<IEnumerable<SubscriptionMethod>>(key);
            if (result == null)
            {
                result = await _service.GetSubscriptionMethodsAsync(-1, sourceID, actionID, recepient);
                _cache.Insert(key, result, DateTime.UtcNow.AddDays(1));
            }

            return result;
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<int> GetTenantAsync()
    {
        return await _tenantManager.GetCurrentTenantIdAsync();
    }
}
