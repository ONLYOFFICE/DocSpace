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

namespace ASC.Notify.Model;

public class TopSubscriptionProvider : ISubscriptionProvider
{
    private readonly string[] _defaultSenderMethods = Array.Empty<string>();
    private readonly ISubscriptionProvider _subscriptionProvider;
    private readonly IRecipientProvider _recipientProvider;


    public TopSubscriptionProvider(IRecipientProvider recipientProvider, ISubscriptionProvider directSubscriptionProvider)
    {
        _recipientProvider = recipientProvider ?? throw new ArgumentNullException(nameof(recipientProvider));
        _subscriptionProvider = directSubscriptionProvider ?? throw new ArgumentNullException(nameof(directSubscriptionProvider));
    }

    public TopSubscriptionProvider(IRecipientProvider recipientProvider, ISubscriptionProvider directSubscriptionProvider, string[] defaultSenderMethods)
        : this(recipientProvider, directSubscriptionProvider)
    {
        _defaultSenderMethods = defaultSenderMethods;
    }


    public virtual string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        var senders = _subscriptionProvider.GetSubscriptionMethod(action, recipient);
        if (senders == null || senders.Length == 0)
        {
            var parents = WalkUp(recipient);
            foreach (var parent in parents)
            {
                senders = _subscriptionProvider.GetSubscriptionMethod(action, parent);
                if (senders != null && senders.Length != 0)
                {
                    break;
                }
            }
        }

        return senders != null && 0 < senders.Length ? senders : _defaultSenderMethods;
    }

    public virtual IRecipient[] GetRecipients(INotifyAction action, string objectID)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var recipents = new List<IRecipient>(5);
        var directRecipients = _subscriptionProvider.GetRecipients(action, objectID) ?? new IRecipient[0];
        recipents.AddRange(directRecipients);

        return recipents.ToArray();
    }

    public virtual bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        return _subscriptionProvider.IsUnsubscribe(recipient, action, objectID);
    }


    public virtual void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        _subscriptionProvider.Subscribe(action, objectID, recipient);
    }

    public virtual void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        _subscriptionProvider.UnSubscribe(action, objectID, recipient);
    }

    public void UnSubscribe(INotifyAction action, string objectID)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _subscriptionProvider.UnSubscribe(action, objectID);
    }

    public void UnSubscribe(INotifyAction action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _subscriptionProvider.UnSubscribe(action);
    }

    public virtual void UnSubscribe(INotifyAction action, IRecipient recipient)
    {
        var objects = GetSubscriptions(action, recipient);
        foreach (var objectID in objects)
        {
            _subscriptionProvider.UnSubscribe(action, objectID, recipient);
        }
    }

    public virtual void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }
        if (senderNames == null)
        {
            throw new ArgumentNullException(nameof(senderNames));
        }

        _subscriptionProvider.UpdateSubscriptionMethod(action, recipient, senderNames);
    }

    public virtual object GetSubscriptionRecord(INotifyAction action, IRecipient recipient, string objectID)
    {
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var subscriptionRecord = _subscriptionProvider.GetSubscriptionRecord(action, recipient, objectID);

        if (subscriptionRecord != null)
        {
            return subscriptionRecord;
        }

        var parents = WalkUp(recipient);

        foreach (var parent in parents)
        {
            subscriptionRecord = _subscriptionProvider.GetSubscriptionRecord(action, parent, objectID);

            if (subscriptionRecord != null)
            {
                break;
            }
        }

        return subscriptionRecord;
    }

    public virtual string[] GetSubscriptions(INotifyAction action, IRecipient recipient, bool checkSubscription = true)
    {
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var objects = new List<string>();
        var direct = _subscriptionProvider.GetSubscriptions(action, recipient, checkSubscription) ?? Array.Empty<string>();
        MergeObjects(objects, direct);
        var parents = WalkUp(recipient);
        foreach (var parent in parents)
        {
            direct = _subscriptionProvider.GetSubscriptions(action, parent, checkSubscription) ?? Array.Empty<string>();
            if (recipient is IDirectRecipient)
            {
                foreach (var groupsubscr in direct)
                {
                    if (!objects.Contains(groupsubscr) && !_subscriptionProvider.IsUnsubscribe(recipient as IDirectRecipient, action, groupsubscr))
                    {
                        objects.Add(groupsubscr);
                    }
                }
            }
            else
            {
                MergeObjects(objects, direct);
            }
        }

        return objects.ToArray();
    }


    private List<IRecipient> WalkUp(IRecipient recipient)
    {
        var parents = new List<IRecipient>();
        var groups = _recipientProvider.GetGroups(recipient) ?? new IRecipientsGroup[0];
        foreach (var group in groups)
        {
            parents.Add(group);
            parents.AddRange(WalkUp(group));
        }

        return parents;
    }

    private void MergeActions(List<INotifyAction> result, IEnumerable<INotifyAction> additions)
    {
        foreach (var addition in additions)
        {
            if (!result.Contains(addition))
            {
                result.Add(addition);
            }
        }
    }

    private void MergeObjects(List<string> result, IEnumerable<string> additions)
    {
        foreach (var addition in additions)
        {
            if (!result.Contains(addition))
            {
                result.Add(addition);
            }
        }
    }
}
