namespace ASC.Data.Storage.Encryption;

[Scope]
public class NotifyHelper
{
    private const string NotifyService = "ASC.Web.Studio.Core.Notify.StudioNotifyService, ASC.Web.Core";

    private string _serverRootPath;
    private readonly NotifyServiceClient _notifyServiceClient;
    private readonly ILog _logger;

    public NotifyHelper(IOptionsMonitor<ILog> option, NotifyServiceClient notifyServiceClient)
    {
        _notifyServiceClient = notifyServiceClient;
        _logger = option.CurrentValue;
    }

    public void Init(string serverRootPath)
    {
        _serverRootPath = serverRootPath;
    }

    public void SendStorageEncryptionStart(int tenantId)
    {
        SendStorageEncryptionNotification("SendStorageEncryptionStart", tenantId);
    }

    public void SendStorageEncryptionSuccess(int tenantId)
    {
        SendStorageEncryptionNotification("SendStorageEncryptionSuccess", tenantId);
    }

    public void SendStorageEncryptionError(int tenantId)
    {
        SendStorageEncryptionNotification("SendStorageEncryptionError", tenantId);
    }

    public void SendStorageDecryptionStart(int tenantId)
    {
        SendStorageEncryptionNotification("SendStorageDecryptionStart", tenantId);
    }

    public void SendStorageDecryptionSuccess(int tenantId)
    {
        SendStorageEncryptionNotification("SendStorageDecryptionSuccess", tenantId);
    }

    public void SendStorageDecryptionError(int tenantId)
    {
        SendStorageEncryptionNotification("SendStorageDecryptionError", tenantId);
    }

    private void SendStorageEncryptionNotification(string method, int tenantId)
    {
        var notifyInvoke = new NotifyInvoke()
        {
            Service = NotifyService,
            Method = method,
            Tenant = tenantId
        };

        notifyInvoke.Parameters.Add(_serverRootPath);

        try
        {
            _notifyServiceClient.InvokeSendMethod(notifyInvoke);
        }
        catch (Exception error)
        {
            _logger.Warn("Error while sending notification", error);
        }
    }
}
