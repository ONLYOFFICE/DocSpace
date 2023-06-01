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

namespace ASC.Notify.Engine;

public interface INotifyEngineAction
{
    Task BeforeTransferRequestAsync(NotifyRequest request);
    void AfterTransferRequest(NotifyRequest request);
}

[Singletone]
public class NotifyEngine : INotifyEngine, IDisposable
{
    private readonly ILogger _logger;
    private readonly Context _context;
    private readonly List<SendMethodWrapper> _sendMethods = new List<SendMethodWrapper>();
    private readonly Queue<NotifyRequest> _requests = new Queue<NotifyRequest>(1000);
    private readonly Thread _notifyScheduler;
    private readonly Thread _notifySender;
    private readonly AutoResetEvent _requestsEvent = new AutoResetEvent(false);
    private readonly AutoResetEvent _methodsEvent = new AutoResetEvent(false);
    private readonly Dictionary<string, IPatternStyler> _stylers = new Dictionary<string, IPatternStyler>();
    private readonly IPatternFormatter _sysTagFormatter = new ReplacePatternFormatter(@"_#(?<tagName>[A-Z0-9_\-.]+)#_", true);
    private readonly TimeSpan _defaultSleep = TimeSpan.FromSeconds(10);
    private readonly IServiceScopeFactory _serviceScopeFactory;
    internal readonly ICollection<Type> Actions;


    public NotifyEngine(Context context, ILoggerProvider options, IServiceScopeFactory serviceScopeFactory)
    {
        Actions = new List<Type>();
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = options.CreateLogger("ASC.Notify");
        _serviceScopeFactory = serviceScopeFactory;
        _notifyScheduler = new Thread(NotifySchedulerAsync) { IsBackground = true, Name = "NotifyScheduler" };
        _notifySender = new Thread(NotifySenderAsync) { IsBackground = true, Name = "NotifySender" };
    }

    public void AddAction<T>() where T : INotifyEngineAction
    {
        Actions.Add(typeof(T));
    }

    public void QueueRequest(NotifyRequest request)
    {
        lock (_requests)
        {
            if (!_notifySender.IsAlive)
            {
                _notifySender.Start();
            }

            _requests.Enqueue(request);
        }

        _requestsEvent.Set();
    }

    internal void RegisterSendMethod(Func<DateTime, Task> method, string cron)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(cron);

        var w = new SendMethodWrapper(method, cron, _logger);
        lock (_sendMethods)
        {
            if (!_notifyScheduler.IsAlive)
            {
                _notifyScheduler.Start();
            }

            _sendMethods.Remove(w);
            _sendMethods.Add(w);
        }
        _methodsEvent.Set();
    }

    internal void UnregisterSendMethod(Func<DateTime, Task> method)
    {
        ArgumentNullException.ThrowIfNull(method);

        lock (_sendMethods)
        {
            _sendMethods.Remove(new SendMethodWrapper(method, null, _logger));
        }
    }

    private async void NotifySchedulerAsync()
    {
        try
        {
            while (true)
            {
                var min = DateTime.MaxValue;
                var now = DateTime.UtcNow;
                List<SendMethodWrapper> copy;
                lock (_sendMethods)
                {
                    copy = _sendMethods.ToList();
                }

                foreach (var w in copy)
                {
                    if (!w.ScheduleDate.HasValue)
                    {
                        lock (_sendMethods)
                        {
                            _sendMethods.Remove(w);
                        }
                    }

                    if (w.ScheduleDate.Value <= now)
                    {
                        try
                        {
                            await w.InvokeSendMethod(now);
                        }
                        catch (Exception error)
                        {
                            _logger.ErrorInvokeSendMethod(error);
                        }
                        w.UpdateScheduleDate(now);
                    }

                    if (w.ScheduleDate.Value > now && w.ScheduleDate.Value < min)
                    {
                        min = w.ScheduleDate.Value;
                    }
                }

                var wait = min != DateTime.MaxValue ? min - DateTime.UtcNow : _defaultSleep;

                if (wait < _defaultSleep)
                {
                    wait = _defaultSleep;
                }
                else if (wait.Ticks > int.MaxValue)
                {
                    wait = TimeSpan.FromTicks(int.MaxValue);
                }
                _methodsEvent.WaitOne(wait, false);
            }
        }
        catch (ThreadAbortException)
        {
            return;
        }
        catch (Exception e)
        {
            _logger.ErrorNotifyScheduler(e);
        }
    }


    private async void NotifySenderAsync()
    {
        try
        {
            while (true)
            {
                NotifyRequest request = null;
                lock (_requests)
                {
                    if (_requests.Count > 0)
                    {
                        request = _requests.Dequeue();
                    }
                }
                if (request != null)
                {
                    await using var scope = _serviceScopeFactory.CreateAsyncScope();
                    foreach (var action in Actions)
                    {
                        ((INotifyEngineAction)scope.ServiceProvider.GetRequiredService(action)).AfterTransferRequest(request);
                    }

                    try
                    {
                        await SendNotify(request, scope);
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorSendNotify(e);
                    }
                }
                else
                {
                    _requestsEvent.WaitOne();
                }
            }
        }
        catch (ThreadAbortException)
        {
            return;
        }
        catch (Exception e)
        {
            _logger.ErrorNotifySender(e);
        }
    }


    private async Task<NotifyResult> SendNotify(NotifyRequest request, IServiceScope serviceScope)
    {
        var sendResponces = new List<SendResponse>();

        var response = await CheckPreventInterceptors(request, InterceptorPlace.Prepare, serviceScope, null);
        if (response != null)
        {
            sendResponces.Add(response);
        }
        else
        {
            sendResponces.AddRange(await SendGroupNotify(request, serviceScope));
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
        _logger.Debug(result.ToString());

        return result;
    }

    private async Task<SendResponse> CheckPreventInterceptors(NotifyRequest request, InterceptorPlace place, IServiceScope serviceScope, string sender)
    {
        return await request.Intercept(place, serviceScope) ? new SendResponse(request.NotifyAction, sender, request.Recipient, SendResult.Prevented) : null;
    }

    private async Task<List<SendResponse>> SendGroupNotify(NotifyRequest request, IServiceScope serviceScope)
    {
        var responces = new List<SendResponse>();
        await SendGroupNotify(request, responces, serviceScope);

        return responces;
    }

    private async Task SendGroupNotify(NotifyRequest request, List<SendResponse> responces, IServiceScope serviceScope)
    {
        if (request.Recipient is IDirectRecipient)
        {
            var subscriptionSource = request.GetSubscriptionProvider(serviceScope);
            if (!request._isNeedCheckSubscriptions || !await subscriptionSource.IsUnsubscribeAsync(request.Recipient as IDirectRecipient, request.NotifyAction, request.ObjectID))
            {
                var directresponses = new List<SendResponse>(1);
                try
                {
                    directresponses = await SendDirectNotifyAsync(request, serviceScope);
                }
                catch (Exception exc)
                {
                    directresponses.Add(new SendResponse(request.NotifyAction, request.Recipient, exc));
                    _logger.ErrorWithException(exc);
                }

                responces.AddRange(directresponses);
            }
        }
        else
        {
            if (request.Recipient is IRecipientsGroup)
            {
                var checkresp = await CheckPreventInterceptors(request, InterceptorPlace.GroupSend, serviceScope, null);
                if (checkresp != null)
                {
                    responces.Add(checkresp);
                }
                else
                {
                    var recipientProvider = request.GetRecipientsProvider(serviceScope);

                    try
                    {
                        var recipients = await recipientProvider.GetGroupEntriesAsync(request.Recipient as IRecipientsGroup) ?? new IRecipient[0];
                        foreach (var recipient in recipients)
                        {
                            try
                            {
                                var newRequest = request.Split(recipient);
                                await SendGroupNotify(newRequest, responces, serviceScope);
                            }
                            catch (Exception exc)
                            {
                                responces.Add(new SendResponse(request.NotifyAction, request.Recipient, exc));
                                _logger.ErrorWithException(exc);
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

    private async Task<List<SendResponse>> SendDirectNotifyAsync(NotifyRequest request, IServiceScope serviceScope)
    {
        if (request.Recipient is not IDirectRecipient)
        {
            throw new ArgumentException("request.Recipient not IDirectRecipient", nameof(request));
        }

        var responses = new List<SendResponse>();
        var response = await CheckPreventInterceptors(request, InterceptorPlace.DirectSend, serviceScope, null);
        if (response != null)
        {
            responses.Add(response);

            return responses;
        }

        try
        {
            await PrepareRequestFillSendersAsync(request, serviceScope);
            PrepareRequestFillPatterns(request, serviceScope);
            PrepareRequestFillTags(request, serviceScope);
        }
        catch (Exception ex)
        {
            responses.Add(new SendResponse(request.NotifyAction, null, request.Recipient, SendResult.Impossible));
            _logger.ErrorPrepare(ex);
        }

        if (request._senderNames != null && request._senderNames.Length > 0)
        {
            foreach (var sendertag in request._senderNames)
            {
                var channel = _context.GetSender(sendertag);
                if (channel != null)
                {
                    try
                    {
                        response = await SendDirectNotify(request, channel, serviceScope);
                    }
                    catch (Exception exc)
                    {
                        response = new SendResponse(request.NotifyAction, channel.SenderName, request.Recipient, exc);
                        _logger.ErrorWithException(exc);
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

    private async Task<SendResponse> SendDirectNotify(NotifyRequest request, ISenderChannel channel, IServiceScope serviceScope)
    {
        if (request.Recipient is not IDirectRecipient)
        {
            throw new ArgumentException("request.Recipient not IDirectRecipient", nameof(request));
        }

        request.CurrentSender = channel.SenderName;

        (var oops, var noticeMessage) = await CreateNoticeMessageFromNotifyRequestAsync(request, channel.SenderName, serviceScope);
        if (oops != null)
        {
            return oops;
        }

        request.CurrentMessage = noticeMessage;
        var preventresponse = await CheckPreventInterceptors(request, InterceptorPlace.MessageSend, serviceScope, channel.SenderName);
        if (preventresponse != null)
        {
            return preventresponse;
        }

        await channel.SendAsync(noticeMessage);

        return new SendResponse(noticeMessage, channel.SenderName, SendResult.Inprogress);
    }

    private async Task<(SendResponse, NoticeMessage)> CreateNoticeMessageFromNotifyRequestAsync(NotifyRequest request, string sender, IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(request);
        NoticeMessage noticeMessage = null;
        var recipientProvider = request.GetRecipientsProvider(serviceScope);
        var recipient = request.Recipient as IDirectRecipient;

        var addresses = recipient.Addresses;
        if (addresses == null || addresses.Length == 0)
        {
            addresses = await recipientProvider.GetRecipientAddressesAsync(request.Recipient as IDirectRecipient, sender);
            recipient = new DirectRecipient(request.Recipient.ID, request.Recipient.Name, addresses);
        }

        recipient = await recipientProvider.FilterRecipientAddressesAsync(recipient);
        noticeMessage = request.CreateMessage(recipient);

        addresses = recipient.Addresses;
        if (addresses == null || !addresses.Any(a => !string.IsNullOrEmpty(a)))
        {
            //checking addresses
            return (new SendResponse(request.NotifyAction, sender, recipient, new NotifyException(string.Format("For recipient {0} by sender {1} no one addresses getted.", recipient, sender))), noticeMessage);
        }

        var pattern = request.GetSenderPattern(sender);
        if (pattern == null)
        {
            return (new SendResponse(request.NotifyAction, sender, recipient, new NotifyException(string.Format("For action \"{0}\" by sender \"{1}\" no one patterns getted.", request.NotifyAction, sender))), noticeMessage);
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
            _sysTagFormatter.FormatMessage(
                noticeMessage, new[]
                                       {
                                               new TagValue(Context.SysRecipientId, request.Recipient.ID),
                                               new TagValue(Context.SysRecipientName, request.Recipient.Name),
                                               new TagValue(Context.SysRecipientAddress, addresses != null && addresses.Length > 0 ? addresses[0] : null)
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
            return (new SendResponse(request.NotifyAction, sender, recipient, exc), noticeMessage);
        }

        return (null, noticeMessage);
    }

    private void StyleMessage(IServiceScope scope, NoticeMessage message)
    {
        try
        {
            if (!_stylers.ContainsKey(message.Pattern.Styler))
            {
                if (scope.ServiceProvider.GetService(Type.GetType(message.Pattern.Styler, true)) is IPatternStyler styler)
                {
                    _stylers.Add(message.Pattern.Styler, styler);
                }
            }
            _stylers[message.Pattern.Styler].ApplyFormating(message);
        }
        catch (Exception exc)
        {
            _logger.WarningErrorStyling(exc);
        }
    }

    private async Task PrepareRequestFillSendersAsync(NotifyRequest request, IServiceScope serviceScope)
    {
        if (request._senderNames == null)
        {
            var subscriptionProvider = request.GetSubscriptionProvider(serviceScope);

            var senderNames = new List<string>();
            senderNames.AddRange(await subscriptionProvider.GetSubscriptionMethodAsync(request.NotifyAction, request.Recipient) ?? Array.Empty<string>());
            senderNames.AddRange(request.Arguments.OfType<AdditionalSenderTag>().Select(tag => (string)tag.Value));

            request._senderNames = senderNames.ToArray();
        }
    }

    private void PrepareRequestFillPatterns(NotifyRequest request, IServiceScope serviceScope)
    {
        if (request._patterns == null)
        {
            request._patterns = new IPattern[request._senderNames.Length];
            if (request._patterns.Length == 0)
            {
                return;
            }

            var apProvider = request.GetPatternProvider(serviceScope);
            for (var i = 0; i < request._senderNames.Length; i++)
            {
                var senderName = request._senderNames[i];
                IPattern pattern = null;
                if (apProvider.GetPatternMethod != null)
                {
                    pattern = apProvider.GetPatternMethod(request.NotifyAction, senderName, request);
                }
                if (pattern == null)
                {
                    pattern = apProvider.GetPattern(request.NotifyAction, senderName);
                }

                request._patterns[i] = pattern ?? throw new NotifyException($"For action \"{request.NotifyAction.ID}\" by sender \"{senderName}\" no one patterns getted.");
            }
        }
    }

    private void PrepareRequestFillTags(NotifyRequest request, IServiceScope serviceScope)
    {
        var patternProvider = request.GetPatternProvider(serviceScope);
        foreach (var pattern in request._patterns)
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

            foreach (var tag in tags.Where(tag => !request.Arguments.Exists(tagValue => Equals(tagValue.Tag, tag)) && !request._requaredTags.Exists(rtag => Equals(rtag, tag))))
            {
                request._requaredTags.Add(tag);
            }
        }
    }


    private sealed class SendMethodWrapper : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly CronExpression _cronExpression;
        private readonly Action<DateTime> _method;
        private readonly Func<DateTime, Task> _asyncMethod;
        private readonly ILogger _logger;

        public DateTime? ScheduleDate { get; private set; }

        public SendMethodWrapper(Action<DateTime> method, string cron, ILogger log)
        {
            _semaphore = new SemaphoreSlim(1);
            _method = method;
            _logger = log;

            if (!string.IsNullOrEmpty(cron))
            {
                _cronExpression = new CronExpression(cron);
            }

            UpdateScheduleDate(DateTime.UtcNow);
        }

        public SendMethodWrapper(Func<DateTime, Task> method, string cron, ILogger log) : this((Action<DateTime>)null, cron, log)
        {
            _asyncMethod = method;
        }

        public void UpdateScheduleDate(DateTime d)
        {
            try
            {
                if (_cronExpression != null)
                {
                    ScheduleDate = _cronExpression.GetTimeAfter(d);
                }
            }
            catch (Exception e)
            {
                _logger.ErrorUpdateScheduleDate(e);
            }
        }

        public async Task InvokeSendMethod(DateTime d)
        {
            await _semaphore.WaitAsync();
            await Task.Run(async () =>
            {
                try
                {
                    if (_method != null)
                    {
                        _method(d);
                    }
                    else if (_asyncMethod != null)
                    {
                        await _asyncMethod(d);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorInvokeSendMethod(e);
                }
            });
            _semaphore.Release();
        }

        public override bool Equals(object obj)
        {
            return obj is SendMethodWrapper w && ((_method != null && _method.Equals(w._method)) || (_asyncMethod != null && _asyncMethod.Equals(w._asyncMethod)));
        }

        public override int GetHashCode()
        {
            return _method.GetHashCode();
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }

    public void Dispose()
    {
        if (_methodsEvent != null)
        {
            _methodsEvent.Dispose();
        }

        if (_requestsEvent != null)
        {
            _requestsEvent.Dispose();
        }
    }
}

[Scope]
public class NotifyEngineQueue
{
    private readonly NotifyEngine _notifyEngine;
    private readonly IServiceProvider _serviceProvider;

    public NotifyEngineQueue(NotifyEngine notifyEngine, IServiceProvider serviceProvider)
    {
        _notifyEngine = notifyEngine;
        _serviceProvider = serviceProvider;
    }

    public async Task QueueRequestAsync(NotifyRequest request)
    {
        foreach (var action in _notifyEngine.Actions)
        {
            await ((INotifyEngineAction)_serviceProvider.GetRequiredService(action)).BeforeTransferRequestAsync(request);
        }

        _notifyEngine.QueueRequest(request);
    }
}
