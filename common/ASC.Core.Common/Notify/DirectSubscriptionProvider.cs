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
using System.Linq;

using ASC.Notify.Model;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    class DirectSubscriptionProvider : ISubscriptionProvider
    {
        private readonly IRecipientProvider recipientProvider;
        private readonly SubscriptionManager subscriptionManager;
        private readonly string sourceID;


        public DirectSubscriptionProvider(string sourceID, SubscriptionManager subscriptionManager, IRecipientProvider recipientProvider)
        {
            if (string.IsNullOrEmpty(sourceID)) throw new ArgumentNullException(nameof(sourceID));
            this.sourceID = sourceID;
            this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            this.recipientProvider = recipientProvider ?? throw new ArgumentNullException(nameof(recipientProvider));
        }


        public object GetSubscriptionRecord(INotifyAction action, IRecipient recipient, string objectID)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));

            return subscriptionManager.GetSubscriptionRecord(sourceID, action.ID, recipient.ID, objectID);
        }

        public string[] GetSubscriptions(INotifyAction action, IRecipient recipient, bool checkSubscribe = true)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));

            return subscriptionManager.GetSubscriptions(sourceID, action.ID, recipient.ID, checkSubscribe);
        }

        public IRecipient[] GetRecipients(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return subscriptionManager.GetRecipients(sourceID, action.ID, objectID)
                .Select(r => recipientProvider.GetRecipient(r))
                .Where(r => r != null)
                .ToArray();
        }

        public string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));

            return subscriptionManager.GetSubscriptionMethod(sourceID, action.ID, recipient.ID);
        }

        public void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            subscriptionManager.UpdateSubscriptionMethod(sourceID, action.ID, recipient.ID, senderNames);
        }

        public bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            if (action == null) throw new ArgumentNullException(nameof(action));

            return subscriptionManager.IsUnsubscribe(sourceID, recipient.ID, action.ID, objectID);
        }

        public void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));

            subscriptionManager.Subscribe(sourceID, action.ID, objectID, recipient.ID);
        }

        public void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));

            subscriptionManager.Unsubscribe(sourceID, action.ID, objectID, recipient.ID);
        }

        public void UnSubscribe(INotifyAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            subscriptionManager.UnsubscribeAll(sourceID, action.ID);
        }

        public void UnSubscribe(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            subscriptionManager.UnsubscribeAll(sourceID, action.ID, objectID);
        }

        [Obsolete("Use UnSubscribe(INotifyAction, string, IRecipient)", true)]
        public void UnSubscribe(INotifyAction action, IRecipient recipient)
        {
            throw new NotSupportedException("use UnSubscribe(INotifyAction, string, IRecipient )");
        }
    }
}