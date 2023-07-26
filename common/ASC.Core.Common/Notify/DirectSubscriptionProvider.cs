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

namespace ASC.Core.Notify;

class DirectSubscriptionProvider : ISubscriptionProvider
{
    private readonly IRecipientProvider _recipientProvider;
    private readonly SubscriptionManager _subscriptionManager;
    private readonly string _sourceId;


    public DirectSubscriptionProvider(string sourceID, SubscriptionManager subscriptionManager, IRecipientProvider recipientProvider)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(sourceID);
        _sourceId = sourceID;
        _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        _recipientProvider = recipientProvider ?? throw new ArgumentNullException(nameof(recipientProvider));
    }


    public async Task<object> GetSubscriptionRecordAsync(INotifyAction action, IRecipient recipient, string objectID)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        return await _subscriptionManager.GetSubscriptionRecordAsync(_sourceId, action.ID, recipient.ID, objectID);
    }

    public async Task<string[]> GetSubscriptionsAsync(INotifyAction action, IRecipient recipient, bool checkSubscribe = true)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        return await _subscriptionManager.GetSubscriptionsAsync(_sourceId, action.ID, recipient.ID, checkSubscribe);
    }

    public async Task<IRecipient[]> GetRecipientsAsync(INotifyAction action, string objectID)
    {
        ArgumentNullException.ThrowIfNull(action);

        return await (await _subscriptionManager.GetRecipientsAsync(_sourceId, action.ID, objectID)).ToAsyncEnumerable()
            .SelectAwait(GetRecipientAsync)
            .Where(r => r != null)
            .ToArrayAsync();
    }

    private async ValueTask<IRecipient> GetRecipientAsync(string value)
    {
        return await _recipientProvider.GetRecipientAsync(value);
    }

    public async Task<string[]> GetSubscriptionMethodAsync(INotifyAction action, IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        return await _subscriptionManager.GetSubscriptionMethodAsync(_sourceId, action.ID, recipient.ID);
    }

    public async Task UpdateSubscriptionMethodAsync(INotifyAction action, IRecipient recipient, params string[] senderNames)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        await _subscriptionManager.UpdateSubscriptionMethodAsync(_sourceId, action.ID, recipient.ID, senderNames);
    }

    public async Task<bool> IsUnsubscribeAsync(IDirectRecipient recipient, INotifyAction action, string objectID)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(action);

        return await _subscriptionManager.IsUnsubscribeAsync(_sourceId, recipient.ID, action.ID, objectID);
    }

    public async Task SubscribeAsync(INotifyAction action, string objectID, IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        await _subscriptionManager.SubscribeAsync(_sourceId, action.ID, objectID, recipient.ID);
    }

    public async Task UnSubscribeAsync(INotifyAction action, string objectID, IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        await _subscriptionManager.UnsubscribeAsync(_sourceId, action.ID, objectID, recipient.ID);
    }

    public async Task UnSubscribeAsync(INotifyAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        await _subscriptionManager.UnsubscribeAllAsync(_sourceId, action.ID);
    }

    public async Task UnSubscribeAsync(INotifyAction action, string objectID)
    {
        ArgumentNullException.ThrowIfNull(action);

        await _subscriptionManager.UnsubscribeAllAsync(_sourceId, action.ID, objectID);
    }

    [Obsolete("Use UnSubscribe(INotifyAction, string, IRecipient)", true)]
    public Task UnSubscribeAsync(INotifyAction action, IRecipient recipient)
    {
        throw new NotSupportedException("use UnSubscribe(INotifyAction, string, IRecipient )");
    }
}
