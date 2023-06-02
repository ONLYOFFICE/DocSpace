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

namespace ASC.Notify.Model;

public interface ISubscriptionProvider
{
    Task<bool> IsUnsubscribeAsync(IDirectRecipient recipient, INotifyAction action, string objectID);
    Task<IRecipient[]> GetRecipientsAsync(INotifyAction action, string objectID);
    Task<object> GetSubscriptionRecordAsync(INotifyAction action, IRecipient recipient, string objectID);
    Task<string[]> GetSubscriptionMethodAsync(INotifyAction action, IRecipient recipient);
    Task<string[]> GetSubscriptionsAsync(INotifyAction action, IRecipient recipient, bool checkSubscribe = true);
    Task SubscribeAsync(INotifyAction action, string objectID, IRecipient recipient);
    Task UnSubscribeAsync(INotifyAction action);
    Task UnSubscribeAsync(INotifyAction action, IRecipient recipient);
    Task UnSubscribeAsync(INotifyAction action, string objectID);
    Task UnSubscribeAsync(INotifyAction action, string objectID, IRecipient recipient);
    Task UpdateSubscriptionMethodAsync(INotifyAction action, IRecipient recipient, params string[] senderNames);
}

public static class SubscriptionProviderHelper
{
    public static async Task<bool> IsSubscribedAsync(this ISubscriptionProvider provider, ILogger log, INotifyAction action, IRecipient recipient, string objectID)
    {
        var result = false;

        try
        {
            var subscriptionRecord = await provider.GetSubscriptionRecordAsync(action, recipient, objectID);
            if (subscriptionRecord != null)
            {
                var properties = subscriptionRecord.GetType().GetProperties();
                if (properties.Length > 0)
                {
                    var property = properties.Single(p => p.Name == "Subscribed");
                    if (property != null)
                    {
                        result = (bool)property.GetValue(subscriptionRecord, null);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            log.ErrorIsSubscribed(exception);
        }

        return result;
    }
}
