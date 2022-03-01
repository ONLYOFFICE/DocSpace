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

class NotifyClientImpl : INotifyClient
{
    private readonly Context _context;
    private readonly InterceptorStorage _interceptors = new InterceptorStorage();
    private readonly INotifySource _notifySource;
    public readonly IServiceScope _serviceScope;

    public NotifyClientImpl(Context context, INotifySource notifySource, IServiceScope serviceScope)
    {
        this._notifySource = notifySource ?? throw new ArgumentNullException(nameof(notifySource));
        _serviceScope = serviceScope;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void SendNoticeToAsync(INotifyAction action, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
    {
        SendNoticeToAsync(action, null, recipients, senderNames, false, args);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, recipients, senderNames, false, args);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, recipients, null, false, args);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, recipients, null, checkSubscription, args);
    }

    public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, new[] { recipient }, null, false, args);
    }

    public void SendNoticeAsync(int tenantId, INotifyAction action, string objectID, params ITagValue[] args)
    {
        var subscriptionSource = _notifySource.GetSubscriptionProvider();
        var recipients = subscriptionSource.GetRecipients(action, objectID);
        SendNoticeToAsync(action, objectID, recipients, null, false, args);
    }

    public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, new[] { recipient }, null, checkSubscription, args);
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

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, bool checkSubsciption, params ITagValue[] args)
    {
        if (recipients == null)
        {
            throw new ArgumentNullException(nameof(recipients));
        }

        BeginSingleRecipientEvent("__syspreventduplicateinterceptor");

        foreach (var recipient in recipients)
        {
            var r = CreateRequest(action, objectID, recipient, args, senderNames, checkSubsciption);
            SendAsync(r);
        }
    }

    private void SendAsync(NotifyRequest request)
    {
        request.Interceptors = _interceptors.GetAll();
        _context.NotifyEngine.QueueRequest(request, _serviceScope);
    }

    private NotifyRequest CreateRequest(INotifyAction action, string objectID, IRecipient recipient, ITagValue[] args, string[] senders, bool checkSubsciption)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        var request = new NotifyRequest(_notifySource, action, objectID, recipient)
        {
            SenderNames = senders,
            IsNeedCheckSubscriptions = checkSubsciption
        };

        if (args != null)
        {
            request.Arguments.AddRange(args);
        }

        return request;
    }
}
