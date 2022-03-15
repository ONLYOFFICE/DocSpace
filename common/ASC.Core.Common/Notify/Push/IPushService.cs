namespace ASC.Core.Common.Notify.Push;

[ServiceContract]
public interface IPushService
{
    [OperationContract]
    string RegisterDevice(int tenantID, string userID, string token, MobileAppType type);

    [OperationContract]
    void DeregisterDevice(int tenantID, string userID, string token);

    [OperationContract]
    void EnqueueNotification(int tenantID, string userID, PushNotification notification, List<string> targetDevices);

    [OperationContract]
    List<PushNotification> GetFeed(int tenantID, string userID, string deviceToken, DateTime from, DateTime to);
}
