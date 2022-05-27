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
    bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID);
    IRecipient[] GetRecipients(INotifyAction action, string objectID);
    object GetSubscriptionRecord(INotifyAction action, IRecipient recipient, string objectID);
    string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient);
    string[] GetSubscriptions(INotifyAction action, IRecipient recipient, bool checkSubscribe = true);
    void Subscribe(INotifyAction action, string objectID, IRecipient recipient);
    void UnSubscribe(INotifyAction action);
    void UnSubscribe(INotifyAction action, IRecipient recipient);
    void UnSubscribe(INotifyAction action, string objectID);
    void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient);
    void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames);
}

public static class SubscriptionProviderHelper
{
    public static bool IsSubscribed(this ISubscriptionProvider provider, ILogger log, INotifyAction action, IRecipient recipient, string objectID)
    {
        var result = false;

        try
        {
            var subscriptionRecord = provider.GetSubscriptionRecord(action, recipient, objectID);
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
