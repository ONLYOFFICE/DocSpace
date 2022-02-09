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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Common.Notify.Patterns;
using ASC.Notify.Channels;
using ASC.Notify.Cron;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Notify.Engine
{
    public class NotifyEngine : INotifyEngine
    {
        private readonly ILog log;

        private readonly Context context;

        private readonly List<SendMethodWrapper> sendMethods = new List<SendMethodWrapper>();

        private readonly Queue<NotifyRequest> requests = new Queue<NotifyRequest>(1000);

        private readonly Thread notifyScheduler;

        private readonly Thread notifySender;

        private readonly AutoResetEvent requestsEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent methodsEvent = new AutoResetEvent(false);

        private readonly Dictionary<string, IPatternStyler> stylers = new Dictionary<string, IPatternStyler>();

        private readonly IPatternFormatter sysTagFormatter = new ReplacePatternFormatter(@"_#(?<tagName>[A-Z0-9_\-.]+)#_", true);

        private readonly TimeSpan defaultSleep = TimeSpan.FromSeconds(10);

        private IServiceProvider ServiceProvider { get; }

        public event Action<NotifyEngine, NotifyRequest, IServiceScope> BeforeTransferRequest;

        public event Action<NotifyEngine, NotifyRequest, IServiceScope> AfterTransferRequest;


        public NotifyEngine(Context context, IServiceProvider serviceProvider)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            log = serviceProvider.GetService<IOptionsMonitor<ILog>>().Get("ASC.Notify");
            ServiceProvider = serviceProvider;
            notifyScheduler = new Thread(NotifyScheduler) { IsBackground = true, Name = "NotifyScheduler" };
            notifySender = new Thread(NotifySender) { IsBackground = true, Name = "NotifySender" };
        }


        public virtual void QueueRequest(NotifyRequest request, IServiceScope serviceScope)
        {
            BeforeTransferRequest?.Invoke(this, request, serviceScope);
            lock (requests)
            {
                if (!notifySender.IsAlive)
                {
                    notifySender.Start();
                }

                requests.Enqueue(request);
            }
            requestsEvent.Set();
        }


        internal void RegisterSendMethod(Action<DateTime> method, string cron)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (string.IsNullOrEmpty(cron)) throw new ArgumentNullException(nameof(cron));

            var w = new SendMethodWrapper(method, cron, log);
            lock (sendMethods)
            {
                if (!notifyScheduler.IsAlive)
                {
                    notifyScheduler.Start();
                }

                sendMethods.Remove(w);
                sendMethods.Add(w);
            }
            methodsEvent.Set();
        }

        internal void UnregisterSendMethod(Action<DateTime> method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            lock (sendMethods)
            {
                sendMethods.Remove(new SendMethodWrapper(method, null, log));
            }
        }

        private void NotifyScheduler(object state)
        {
            try
            {
                while (true)
                {
                    var min = DateTime.MaxValue;
                    var now = DateTime.UtcNow;
                    List<SendMethodWrapper> copy;
                    lock (sendMethods)
                    {
                        copy = sendMethods.ToList();
                    }

                    foreach (var w in copy)
                    {
                        if (!w.ScheduleDate.HasValue)
                        {
                            lock (sendMethods)
                            {
                                sendMethods.Remove(w);
                            }
                        }

                        if (w.ScheduleDate.Value <= now)
                        {
                            try
                            {
                                w.InvokeSendMethod(now);
                            }
                            catch (Exception error)
                            {
                                log.Error(error);
                            }
                            w.UpdateScheduleDate(now);
                        }

                        if (w.ScheduleDate.Value > now && w.ScheduleDate.Value < min)
                        {
                            min = w.ScheduleDate.Value;
                        }
                    }

                    var wait = min != DateTime.MaxValue ? min - DateTime.UtcNow : defaultSleep;

                    if (wait < defaultSleep)
                    {
                        wait = defaultSleep;
                    }
                    else if (wait.Ticks > int.MaxValue)
                    {
                        wait = TimeSpan.FromTicks(int.MaxValue);
                    }
                    methodsEvent.WaitOne(wait, false);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }


        private void NotifySender(object state)
        {
            try
            {
                while (true)
                {
                    NotifyRequest request = null;
                    lock (requests)
                    {
                        if (requests.Count > 0)
                        {
                            request = requests.Dequeue();
                        }
                    }
                    if (request != null)
                    {
                        using var scope = ServiceProvider.CreateScope();
                        AfterTransferRequest?.Invoke(this, request, scope);
                        try
                        {
                            SendNotify(request, scope);
                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                    }
                    else
                    {
                        requestsEvent.WaitOne();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }


        private NotifyResult SendNotify(NotifyRequest request, IServiceScope serviceScope)
        {
            var sendResponces = new List<SendResponse>();

            var response = CheckPreventInterceptors(request, InterceptorPlace.Prepare, serviceScope, null);
            if (response != null)
            {
                sendResponces.Add(response);
            }
            else
            {
                sendResponces.AddRange(SendGroupNotify(request, serviceScope));
            }

            NotifyResult result;
            if (sendResponces.Count == 0)
            {
                result = new NotifyResult(SendResult.OK, sendResponces);
            }
            else
            {
                result = new NotifyResult(sendResponces.Aggregate((SendResult)0, (s, r) => r.Result), sendResponces);
            }
            log.Debug(result);
            return result;
        }

        private SendResponse CheckPreventInterceptors(NotifyRequest request, InterceptorPlace place, IServiceScope serviceScope, string sender)
        {
            return request.Intercept(place, serviceScope) ? new SendResponse(request.NotifyAction, sender, request.Recipient, SendResult.Prevented) : null;
        }

        private List<SendResponse> SendGroupNotify(NotifyRequest request, IServiceScope serviceScope)
        {
            var responces = new List<SendResponse>();
            SendGroupNotify(request, responces, serviceScope);
            return responces;
        }

        private void SendGroupNotify(NotifyRequest request, List<SendResponse> responces, IServiceScope serviceScope)
        {
            if (request.Recipient is IDirectRecipient)
            {
                var subscriptionSource = request.GetSubscriptionProvider(serviceScope);
                if (!request.IsNeedCheckSubscriptions || !subscriptionSource.IsUnsubscribe(request.Recipient as IDirectRecipient, request.NotifyAction, request.ObjectID))
                {
                    var directresponses = new List<SendResponse>(1);
                    try
                    {
                        directresponses = SendDirectNotify(request, serviceScope);
                    }
                    catch (Exception exc)
                    {
                        directresponses.Add(new SendResponse(request.NotifyAction, request.Recipient, exc));
                    }
                    responces.AddRange(directresponses);
                }
            }
            else
            {
                if (request.Recipient is IRecipientsGroup)
                {
                    var checkresp = CheckPreventInterceptors(request, InterceptorPlace.GroupSend, serviceScope, null);
                    if (checkresp != null)
                    {
                        responces.Add(checkresp);
                    }
                    else
                    {
                        var recipientProvider = request.GetRecipientsProvider(serviceScope);

                        try
                        {
                            var recipients = recipientProvider.GetGroupEntries(request.Recipient as IRecipientsGroup) ?? new IRecipient[0];
                            foreach (var recipient in recipients)
                            {
                                try
                                {
                                    var newRequest = request.Split(recipient);
                                    SendGroupNotify(newRequest, responces, serviceScope);
                                }
                                catch (Exception exc)
                                {
                                    responces.Add(new SendResponse(request.NotifyAction, request.Recipient, exc));
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            responces.Add(new SendResponse(request.NotifyAction, request.Recipient, exc) { Result = SendResult.IncorrectRecipient });
                        }
                    }
                }
                else
                {
                    responces.Add(new SendResponse(request.NotifyAction, request.Recipient, null)
                    {
                        Result = SendResult.IncorrectRecipient,
                        Exception = new NotifyException("recipient may be IRecipientsGroup or IDirectRecipient")
                    });
                }
            }
        }

        private List<SendResponse> SendDirectNotify(NotifyRequest request, IServiceScope serviceScope)
        {
            if (!(request.Recipient is IDirectRecipient)) throw new ArgumentException("request.Recipient not IDirectRecipient", nameof(request));

            var responses = new List<SendResponse>();
            var response = CheckPreventInterceptors(request, InterceptorPlace.DirectSend, serviceScope, null);
            if (response != null)
            {
                responses.Add(response);
                return responses;
            }

            try
            {
                PrepareRequestFillSenders(request, serviceScope);
                PrepareRequestFillPatterns(request, serviceScope);
                PrepareRequestFillTags(request, serviceScope);
            }
            catch (Exception ex)
            {
                responses.Add(new SendResponse(request.NotifyAction, null, request.Recipient, SendResult.Impossible));
                log.Error("Prepare", ex);
            }

            if (request.SenderNames != null && request.SenderNames.Length > 0)
            {
                foreach (var sendertag in request.SenderNames)
                {
                    var channel = context.NotifyService.GetSender(sendertag);
                    if (channel != null)
                    {
                        try
                        {
                            response = SendDirectNotify(request, channel, serviceScope);
                        }
                        catch (Exception exc)
                        {
                            response = new SendResponse(request.NotifyAction, channel.SenderName, request.Recipient, exc);
                        }
                    }
                    else
                    {
                        response = new SendResponse(request.NotifyAction, sendertag, request.Recipient, new NotifyException($"Not registered sender \"{sendertag}\"."));
                    }
                    responses.Add(response);
                }
            }
            else
            {
                response = new SendResponse(request.NotifyAction, request.Recipient, new NotifyException("Notice hasn't any senders."));
                responses.Add(response);
            }
            return responses;
        }

        private SendResponse SendDirectNotify(NotifyRequest request, ISenderChannel channel, IServiceScope serviceScope)
        {
            if (!(request.Recipient is IDirectRecipient)) throw new ArgumentException("request.Recipient not IDirectRecipient", nameof(request));

            request.CurrentSender = channel.SenderName;

            var oops = CreateNoticeMessageFromNotifyRequest(request, channel.SenderName, serviceScope, out var noticeMessage);
            if (oops != null) return oops;

            request.CurrentMessage = noticeMessage;
            var preventresponse = CheckPreventInterceptors(request, InterceptorPlace.MessageSend, serviceScope, channel.SenderName);
            if (preventresponse != null) return preventresponse;

            channel.SendAsync(noticeMessage);

            return new SendResponse(noticeMessage, channel.SenderName, SendResult.Inprogress);
        }

        private SendResponse CreateNoticeMessageFromNotifyRequest(NotifyRequest request, string sender, IServiceScope serviceScope, out NoticeMessage noticeMessage)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var recipientProvider = request.GetRecipientsProvider(serviceScope);
            var recipient = request.Recipient as IDirectRecipient;

            var addresses = recipient.Addresses;
            if (addresses == null || addresses.Length == 0)
            {
                addresses = recipientProvider.GetRecipientAddresses(request.Recipient as IDirectRecipient, sender);
                recipient = new DirectRecipient(request.Recipient.ID, request.Recipient.Name, addresses);
            }

            recipient = recipientProvider.FilterRecipientAddresses(recipient);
            noticeMessage = request.CreateMessage(recipient);

            addresses = recipient.Addresses;
            if (addresses == null || !addresses.Any(a => !string.IsNullOrEmpty(a)))
            {
                //checking addresses
                return new SendResponse(request.NotifyAction, sender, recipient, new NotifyException(string.Format("For recipient {0} by sender {1} no one addresses getted.", recipient, sender)));
            }

            var pattern = request.GetSenderPattern(sender);
            if (pattern == null)
            {
                return new SendResponse(request.NotifyAction, sender, recipient, new NotifyException(string.Format("For action \"{0}\" by sender \"{1}\" no one patterns getted.", request.NotifyAction, sender)));
            }

            noticeMessage.Pattern = pattern;
            noticeMessage.ContentType = pattern.ContentType;
            noticeMessage.AddArgument(request.Arguments.ToArray());
            var patternProvider = request.GetPatternProvider(serviceScope);

            var formatter = patternProvider.GetFormatter(pattern);
            try
            {
                if (formatter != null)
                {
                    formatter.FormatMessage(noticeMessage, noticeMessage.Arguments);
                }
                sysTagFormatter.FormatMessage(
                    noticeMessage, new[]
                                           {
                                               new TagValue(Context._SYS_RECIPIENT_ID, request.Recipient.ID),
                                               new TagValue(Context._SYS_RECIPIENT_NAME, request.Recipient.Name),
                                               new TagValue(Context._SYS_RECIPIENT_ADDRESS, addresses != null && addresses.Length > 0 ? addresses[0] : null)
                                           }
                    );
                //Do styling here
                if (!string.IsNullOrEmpty(pattern.Styler))
                {
                    //We need to run through styler before templating
                    StyleMessage(serviceScope, noticeMessage);
                }
            }
            catch (Exception exc)
            {
                return new SendResponse(request.NotifyAction, sender, recipient, exc);
            }
            return null;
        }

        private void StyleMessage(IServiceScope scope, NoticeMessage message)
        {
            try
            {
                if (!stylers.ContainsKey(message.Pattern.Styler))
                {
                    if (scope.ServiceProvider.GetService(Type.GetType(message.Pattern.Styler, true)) is IPatternStyler styler)
                    {
                        stylers.Add(message.Pattern.Styler, styler);
                    }
                }
                stylers[message.Pattern.Styler].ApplyFormating(message);
            }
            catch (Exception exc)
            {
                log.Warn("error styling message", exc);
            }
        }

        private void PrepareRequestFillSenders(NotifyRequest request, IServiceScope serviceScope)
        {
            if (request.SenderNames == null)
            {
                var subscriptionProvider = request.GetSubscriptionProvider(serviceScope);

                var senderNames = new List<string>();
                senderNames.AddRange(subscriptionProvider.GetSubscriptionMethod(request.NotifyAction, request.Recipient) ?? Array.Empty<string>());
                senderNames.AddRange(request.Arguments.OfType<AdditionalSenderTag>().Select(tag => (string)tag.Value));

                request.SenderNames = senderNames.ToArray();
            }
        }

        private void PrepareRequestFillPatterns(NotifyRequest request, IServiceScope serviceScope)
        {
            if (request.Patterns == null)
            {
                request.Patterns = new IPattern[request.SenderNames.Length];
                if (request.Patterns.Length == 0) return;

                var apProvider = request.GetPatternProvider(serviceScope);
                for (var i = 0; i < request.SenderNames.Length; i++)
                {
                    var senderName = request.SenderNames[i];
                    IPattern pattern = null;
                    if (apProvider.GetPatternMethod != null)
                    {
                        pattern = apProvider.GetPatternMethod(request.NotifyAction, senderName, request);
                    }
                    if (pattern == null)
                    {
                        pattern = apProvider.GetPattern(request.NotifyAction, senderName);
                    }

                    request.Patterns[i] = pattern ?? throw new NotifyException($"For action \"{request.NotifyAction.ID}\" by sender \"{senderName}\" no one patterns getted.");
                }
            }
        }

        private void PrepareRequestFillTags(NotifyRequest request, IServiceScope serviceScope)
        {
            var patternProvider = request.GetPatternProvider(serviceScope);
            foreach (var pattern in request.Patterns)
            {
                IPatternFormatter formatter;
                try
                {
                    formatter = patternProvider.GetFormatter(pattern);
                }
                catch (Exception exc)
                {
                    throw new NotifyException(string.Format("For pattern \"{0}\" formatter not instanced.", pattern), exc);
                }
                var tags = Array.Empty<string>();
                try
                {
                    if (formatter != null)
                    {
                        tags = formatter.GetTags(pattern) ?? Array.Empty<string>();
                    }
                }
                catch (Exception exc)
                {
                    throw new NotifyException(string.Format("Get tags from formatter of pattern \"{0}\" failed.", pattern), exc);
                }

                foreach (var tag in tags.Where(tag => !request.Arguments.Exists(tagValue => Equals(tagValue.Tag, tag)) && !request.RequaredTags.Exists(rtag => Equals(rtag, tag))))
                {
                    request.RequaredTags.Add(tag);
                }
            }
        }


        private sealed class SendMethodWrapper
        {
            private readonly object locker = new object();
            private readonly CronExpression cronExpression;
            private readonly Action<DateTime> method;

            public DateTime? ScheduleDate { get; private set; }
            public ILog Log { get; }

            public SendMethodWrapper(Action<DateTime> method, string cron, ILog log)
            {
                this.method = method;
                Log = log;
                if (!string.IsNullOrEmpty(cron))
                {
                    this.cronExpression = new CronExpression(cron);
                }
                UpdateScheduleDate(DateTime.UtcNow);
            }

            public void UpdateScheduleDate(DateTime d)
            {
                try
                {
                    if (cronExpression != null)
                    {
                        ScheduleDate = cronExpression.GetTimeAfter(d);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            public void InvokeSendMethod(DateTime d)
            {
                lock (locker)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            method(d);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }).Wait();
                }
            }

            public override bool Equals(object obj)
            {
                return obj is SendMethodWrapper w && method.Equals(w.method);
            }

            public override int GetHashCode()
            {
                return method.GetHashCode();
            }
        }
    }
}
