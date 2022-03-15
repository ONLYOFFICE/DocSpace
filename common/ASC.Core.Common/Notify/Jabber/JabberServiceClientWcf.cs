namespace ASC.Core.Notify.Jabber;

public class JabberServiceClientWcf : BaseWcfClient<IJabberService>, IJabberService
{
    public JabberServiceClientWcf() { }

    public string GetVersion()
    {
        return Channel.GetVersion();
    }

    public byte AddXmppConnection(string connectionId, string userName, byte state, int tenantId)
    {
        return Channel.AddXmppConnection(connectionId, userName, state, tenantId);
    }

    public byte RemoveXmppConnection(string connectionId, string userName, int tenantId)
    {
        return Channel.RemoveXmppConnection(connectionId, userName, tenantId);
    }

    public int GetNewMessagesCount(int tenantId, string userName)
    {
        return Channel.GetNewMessagesCount(tenantId, userName);
    }

    public string GetUserToken(int tenantId, string userName)
    {
        return Channel.GetUserToken(tenantId, userName);
    }

    public void SendCommand(int tenantId, string from, string to, string command, bool fromTenant)
    {
        Channel.SendCommand(tenantId, from, to, command, fromTenant);
    }

    public void SendMessage(int tenantId, string from, string to, string text, string subject)
    {
        Channel.SendMessage(tenantId, from, to, text, subject);
    }

    public byte SendState(int tenantId, string userName, byte state)
    {
        return Channel.SendState(tenantId, userName, state);
    }

    public MessageClass[] GetRecentMessages(int tenantId, string from, string to, int id)
    {
        return Channel.GetRecentMessages(tenantId, from, to, id);
    }

    public Dictionary<string, byte> GetAllStates(int tenantId, string userName)
    {
        return Channel.GetAllStates(tenantId, userName);
    }

    public byte GetState(int tenantId, string userName)
    {
        return Channel.GetState(tenantId, userName);
    }

    public void Ping(string userId, int tenantId, string userName, byte state)
    {
        Channel.Ping(userId, tenantId, userName, state);
    }

    public string HealthCheck(string userName, int tenantId)
    {
        return Channel.HealthCheck(userName, tenantId);
    }
}
