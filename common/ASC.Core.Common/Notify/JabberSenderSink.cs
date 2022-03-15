namespace ASC.Core.Notify;

class JabberSenderSink : Sink
{
    private static readonly string _senderName = Configuration.Constants.NotifyMessengerSenderSysName;
    private readonly INotifySender _sender;

    public JabberSenderSink(INotifySender sender, IServiceProvider serviceProvider)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _serviceProvider = serviceProvider;
    }

    private readonly IServiceProvider _serviceProvider;

    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<JabberSenderSinkScope>();
            (var userManager, var tenantManager) = scopeClass;
            var result = SendResult.OK;
            var username = userManager.GetUsers(new Guid(message.Recipient.ID)).UserName;
            if (string.IsNullOrEmpty(username))
            {
                result = SendResult.IncorrectRecipient;
            }
            else
            {
                var m = new NotifyMessage
                {
                    Reciever = username,
                    Subject = message.Subject,
                    ContentType = message.ContentType,
                    Content = message.Body,
                    SenderType = _senderName,
                    CreationDate = DateTime.UtcNow.Ticks,
                };

                var tenant = tenantManager.GetCurrentTenant(false);
                m.TenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

                _sender.Send(m);
            }

            return new SendResponse(message, _senderName, result);
        }
        catch (Exception ex)
        {
            return new SendResponse(message, _senderName, ex);
        }
    }
}

[Scope]
public class JabberSenderSinkScope
{
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;

    public JabberSenderSinkScope(UserManager userManager, TenantManager tenantManager)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
    }

    public void Deconstruct(out UserManager userManager, out TenantManager tenantManager)
    {
        (userManager, tenantManager) = (_userManager, _tenantManager);
    }
}
