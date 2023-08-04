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

class NotifyClientImpl : INotifyClient
{
    private readonly InterceptorStorage _interceptors = new InterceptorStorage();
    private readonly ILoggerProvider _loggerFactory;
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly INotifySource _notifySource;

    public NotifyClientImpl(ILoggerProvider loggerFactory, NotifyEngineQueue notifyEngineQueue, INotifySource notifySource)
    {
        _loggerFactory = loggerFactory;
        _notifyEngineQueue = notifyEngineQueue;
        _notifySource = notifySource ?? throw new ArgumentNullException(nameof(notifySource));
    }

    public async Task SendNoticeToAsync(INotifyAction action, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, null, recipients, senderNames, false, args);
    }

    public async Task SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, objectID, recipients, senderNames, false, args);
    }

    public async Task SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, objectID, recipients, null, false, args);
    }

    public async Task SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, objectID, recipients, null, checkSubscription, args);
    }

    public async Task SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, objectID, new[] { recipient }, null, false, args);
    }

    public async Task SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, string sendername, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, objectID, new[] { recipient }, new[] { sendername }, false, args);
    }

    public async Task SendNoticeAsync(int tenantId, INotifyAction action, string objectID, params ITagValue[] args)
    {
        var subscriptionSource = _notifySource.GetSubscriptionProvider();
        var recipients = await subscriptionSource.GetRecipientsAsync(action, objectID);
        await SendNoticeToAsync(action, objectID, recipients, null, false, args);
    }

    public async Task SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args)
    {
        await SendNoticeToAsync(action, objectID, new[] { recipient }, null, checkSubscription, args);
    }

    public void BeginSingleRecipientEvent(string name)
    {
        _interceptors.Add(new SingleRecipientInterceptor(name));
    }

    public void EndSingleRecipientEvent(string name)
    {
        _interceptors.Remove(name);
    }

    public void AddInterceptor(ISendInterceptor interceptor)
    {
        _interceptors.Add(interceptor);
    }

    public void RemoveInterceptor(string name)
    {
        _interceptors.Remove(name);
    }

    public async Task SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, bool checkSubsciption, params ITagValue[] args)
    {
        ArgumentNullException.ThrowIfNull(recipients);

        BeginSingleRecipientEvent("__syspreventduplicateinterceptor");

        foreach (var recipient in recipients)
        {
            var r = CreateRequest(action, objectID, recipient, args, senderNames, checkSubsciption);
            await SendAsync(r);
        }
    }

    private async Task SendAsync(NotifyRequest request)
    {
        request._interceptors = _interceptors.GetAll();
        await _notifyEngineQueue.QueueRequestAsync(request);
    }

    private NotifyRequest CreateRequest(INotifyAction action, string objectID, IRecipient recipient, ITagValue[] args, string[] senders, bool checkSubsciption)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        var request = new NotifyRequest(_loggerFactory, _notifySource, action, objectID, recipient)
        {
            _senderNames = senders,
            _isNeedCheckSubscriptions = checkSubsciption
        };

        if (args != null)
        {
            request.Arguments.AddRange(args);
        }

        return request;
    }
}
