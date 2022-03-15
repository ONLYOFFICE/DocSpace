namespace ASC.Core.Common.Notify.Push;

public class PushServiceClient : BaseWcfClient<IPushService>, IPushService
{
    public string RegisterDevice(int tenantID, string userID, string token, MobileAppType type)
    {
        return Channel.RegisterDevice(tenantID, userID, token, type);
    }

    public void DeregisterDevice(int tenantID, string userID, string token)
    {
        Channel.DeregisterDevice(tenantID, userID, token);
    }

    public void EnqueueNotification(int tenantID, string userID, PushNotification notification, List<string> targetDevices)
    {
        Channel.EnqueueNotification(tenantID, userID, notification, targetDevices);
    }

    public List<PushNotification> GetFeed(int tenantID, string userID, string deviceToken, DateTime from, DateTime to)
    {
        return Channel.GetFeed(tenantID, userID, deviceToken, from, to);
    }
}
