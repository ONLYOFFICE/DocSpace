namespace ASC.Core.Notify.Jabber;

[ServiceContract]
public interface IJabberService
{
    [OperationContract]
    string GetVersion();

    [OperationContract]
    byte AddXmppConnection(string connectionId, string userName, byte state, int tenantId);

    [OperationContract]
    byte RemoveXmppConnection(string connectionId, string userName, int tenantId);

    [OperationContract]
    int GetNewMessagesCount(int tenantId, string userName);

    [OperationContract]
    string GetUserToken(int tenantId, string userName);

    [OperationContract(IsOneWay = true)]
    void SendCommand(int tenantId, string from, string to, string command, bool fromTenant);

    [OperationContract(IsOneWay = true)]
    void SendMessage(int tenantId, string from, string to, string text, string subject);

    [OperationContract]
    byte SendState(int tenantId, string userName, byte state);

    [OperationContract]
    MessageClass[] GetRecentMessages(int tenantId, string from, string to, int id);

    [OperationContract(IsOneWay = true)]
    void Ping(string userId, int tenantId, string userName, byte state);

    [OperationContract]
    Dictionary<string, byte> GetAllStates(int tenantId, string userName);

    [OperationContract]
    byte GetState(int tenantId, string userName);

    [OperationContract]
    string HealthCheck(string userName, int tenantId);
}
