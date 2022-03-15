namespace ASC.Core.Notify;

class TelegramSenderSink : Sink
{
    private readonly string _senderName = Configuration.Constants.NotifyTelegramSenderSysName;
    private readonly INotifySender _sender;
    private readonly IServiceProvider _serviceProvider;

    public TelegramSenderSink(INotifySender sender, IServiceProvider serviceProvider)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _serviceProvider = serviceProvider;
    }


    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        try
        {
            const SendResult result = SendResult.OK;
            var m = new NotifyMessage
            {
                Reciever = message.Recipient.ID,
                Subject = message.Subject,
                ContentType = message.ContentType,
                Content = message.Body,
                SenderType = _senderName,
                CreationDate = DateTime.UtcNow.Ticks,
            };

            using var scope = _serviceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

            var tenant = tenantManager.GetCurrentTenant(false);
            m.TenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

            _sender.Send(m);

            return new SendResponse(message, _senderName, result);
        }
        catch (Exception ex)
        {
            return new SendResponse(message, _senderName, ex);
        }
    }
}
