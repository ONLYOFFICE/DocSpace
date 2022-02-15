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

namespace ASC.Notify.Engine;

public class NotifyRequest
{
    private INotifySource NotifySource { get; set; }
    public INotifyAction NotifyAction { get; internal set; }
    public string ObjectID { get; internal set; }
    public IRecipient Recipient { get; internal set; }
    public List<ITagValue> Arguments { get; internal set; }
    public string CurrentSender { get; internal set; }
    public INoticeMessage CurrentMessage { get; internal set; }
    public Hashtable Properties { get; private set; }
    internal string[] SenderNames;
    internal IPattern[] Patterns;
    internal List<string> RequaredTags;
    internal List<ISendInterceptor> Interceptors;
    internal bool IsNeedCheckSubscriptions;

    public NotifyRequest(INotifySource notifySource, INotifyAction action, string objectID, IRecipient recipient)
    {
        Properties = new Hashtable();
        Arguments = new List<ITagValue>();
        RequaredTags = new List<string>();
        Interceptors = new List<ISendInterceptor>();

        NotifySource = notifySource ?? throw new ArgumentNullException(nameof(notifySource));
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        NotifyAction = action ?? throw new ArgumentNullException(nameof(action));
        ObjectID = objectID;

        IsNeedCheckSubscriptions = true;
    }

    internal bool Intercept(InterceptorPlace place, IServiceScope serviceScope)
    {
        var result = false;
        foreach (var interceptor in Interceptors)
        {
            if ((interceptor.PreventPlace & place) == place)
            {
                try
                {
                    if (interceptor.PreventSend(this, place, serviceScope))
                    {
                        result = true;
                    }
                }
                catch (Exception err)
                {
                    serviceScope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().Get("ASC.Notify").ErrorFormat("{0} {1} {2}: {3}", interceptor.Name, NotifyAction, Recipient, err);
                }
            }
        }

        return result;
    }

    internal IPattern GetSenderPattern(string senderName)
    {
        if (SenderNames == null || Patterns == null ||
            SenderNames.Length == 0 || Patterns.Length == 0 ||
            SenderNames.Length != Patterns.Length)
        {
            return null;
        }

        var index = Array.IndexOf(SenderNames, senderName);
        if (index < 0)
        {
            throw new ApplicationException($"Sender with tag {senderName} dnot found");
        }

        return Patterns[index];
    }

    internal NotifyRequest Split(IRecipient recipient)
    {
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        var newRequest = new NotifyRequest(NotifySource, NotifyAction, ObjectID, recipient)
        {
            SenderNames = SenderNames,
            Patterns = Patterns,
            Arguments = new List<ITagValue>(Arguments),
            RequaredTags = RequaredTags,
            CurrentSender = CurrentSender,
            CurrentMessage = CurrentMessage
        };
        newRequest.Interceptors.AddRange(Interceptors);

        return newRequest;
    }

    internal NoticeMessage CreateMessage(IDirectRecipient recipient)
    {
        return new NoticeMessage(recipient, NotifyAction, ObjectID);
    }

    public IActionProvider GetActionProvider(IServiceScope scope)
    {
        return ((INotifySource)scope.ServiceProvider.GetService(NotifySource.GetType())).GetActionProvider();
    }

    public IPatternProvider GetPatternProvider(IServiceScope scope)
    {
        return ((INotifySource)scope.ServiceProvider.GetService(NotifySource.GetType())).GetPatternProvider();
    }

    public IRecipientProvider GetRecipientsProvider(IServiceScope scope)
    {
        return ((INotifySource)scope.ServiceProvider.GetService(NotifySource.GetType())).GetRecipientsProvider();
    }

    public ISubscriptionProvider GetSubscriptionProvider(IServiceScope scope)
    {
        return ((INotifySource)scope.ServiceProvider.GetService(NotifySource.GetType())).GetSubscriptionProvider();
    }
}
