namespace ASC.Core.Notify.Signalr;

[ServiceContract]
public interface ISignalrService
{
    [OperationContract(IsOneWay = true)]
    void SendMessage(string callerUserName, string calleeUserName, string messageText, int tenantId, string domain);

    [OperationContract(IsOneWay = true)]
    void SendInvite(string chatRoomName, string calleeUserName, string domain);

    [OperationContract(IsOneWay = true)]
    void SendState(string from, byte state, int tenantId, string domain);

    [OperationContract(IsOneWay = true)]
    void SendOfflineMessages(string callerUserName, List<string> users, int tenantId);

    [OperationContract(IsOneWay = true)]
    void SendUnreadCounts(Dictionary<string, int> unreadCounts, string domain);

    [OperationContract(IsOneWay = true)]
    void SendUnreadUsers(Dictionary<int, HashSet<Guid>> unreadUsers);

    [OperationContract(IsOneWay = true)]
    void SendUnreadUser(int tenant, string userId);

    [OperationContract(IsOneWay = true)]
    void SendMailNotification(int tenant, string userId, int state);
}
