using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Core.Common.Notify;

class PushSenderSink : Sink
{
    private readonly ILog _logger;
    private bool _configured = true;

    public PushSenderSink(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
    }

    private readonly IServiceProvider _serviceProvider;

    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

            var notification = new PushNotification
            {
                Module = GetTagValue<PushModule>(message, PushConstants.PushModuleTagName),
                Action = GetTagValue<PushAction>(message, PushConstants.PushActionTagName),
                Item = GetTagValue<PushItem>(message, PushConstants.PushItemTagName),
                ParentItem = GetTagValue<PushItem>(message, PushConstants.PushParentItemTagName),
                Message = message.Body,
                ShortMessage = message.Subject
            };

            if (_configured)
            {
                try
                {
                    using var pushClient = new PushServiceClient();
                    pushClient.EnqueueNotification(
                        tenantManager.GetCurrentTenant().Id,
                        message.Recipient.ID,
                        notification,
                        new List<string>());
                }
                catch (InvalidOperationException)
                {
                    _configured = false;
                    _logger.Debug("push sender endpoint is not configured!");
                }
            }
            else
            {
                _logger.Debug("push sender endpoint is not configured!");
            }

            return new SendResponse(message, Constants.NotifyPushSenderSysName, SendResult.OK);
        }
        catch (Exception error)
        {
            return new SendResponse(message, Constants.NotifyPushSenderSysName, error);
        }
    }

    private T GetTagValue<T>(INoticeMessage message, string tagName)
    {
        var tag = message.Arguments.FirstOrDefault(arg => arg.Tag == tagName);

        return tag != null ? (T)tag.Value : default;
    }
}
