namespace ASC.Core.Notify.Jabber;

[Scope]
public class JabberServiceClient
{
    private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(2);
    private static DateTime _lastErrorTime = default;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;

    public JabberServiceClient(UserManager userManager, AuthContext authContext, TenantManager tenantManager)
    {
        _userManager = userManager;
        _authContext = authContext;
        _tenantManager = tenantManager;
    }

    private static bool IsServiceProbablyNotAvailable()
    {
        return _lastErrorTime != default && _lastErrorTime + _timeout > DateTime.Now;
    }

    public bool SendMessage(int tenantId, string from, string to, string text, string subject)
    {
        if (IsServiceProbablyNotAvailable()) return false;

        using (var service = GetService())
        {
            try
            {
                service.SendMessage(tenantId, from, to, text, subject);
                return true;
            }
            catch (Exception error)
            {
                ProcessError(error);
            }
        }

        return false;
    }

    public string GetVersion()
    {
        using (var service = GetService())
        {
            try
            {
                return service.GetVersion();
            }
            catch (Exception error)
            {
                ProcessError(error);
            }
        }

        return null;
    }

    public int GetNewMessagesCount()
    {
        const int result = 0;
        if (IsServiceProbablyNotAvailable())
        {
            return result;
        }

        using (var service = GetService())
        {
            try
            {
                return service.GetNewMessagesCount(GetCurrentTenantId(), GetCurrentUserName());
            }
            catch (Exception error)
            {
                ProcessError(error);
            }
        }

        return result;
    }

    public byte AddXmppConnection(string connectionId, byte state)
    {
        byte result = 4;
        if (IsServiceProbablyNotAvailable())
        {
            throw new Exception();
        }

        using var service = GetService();
        try
        {
            result = service.AddXmppConnection(connectionId, GetCurrentUserName(), state, GetCurrentTenantId());
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return result;
    }

    public byte RemoveXmppConnection(string connectionId)
    {
        const byte result = 4;
        if (IsServiceProbablyNotAvailable())
        {
            return result;
        }

        using (var service = GetService())
        {
            try
            {
                return service.RemoveXmppConnection(connectionId, GetCurrentUserName(), GetCurrentTenantId());
            }
            catch (Exception error)
            {
                ProcessError(error);
            }
        }

        return result;
    }

    public byte GetState(string userName)
    {
        const byte defaultState = 0;

        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                return defaultState;
            }

            using var service = GetService();

            return service.GetState(GetCurrentTenantId(), userName);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return defaultState;
    }

    public byte SendState(byte state)
    {
        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                throw new Exception();
            }

            using var service = GetService();

            return service.SendState(GetCurrentTenantId(), GetCurrentUserName(), state);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return 4;
    }

    public Dictionary<string, byte> GetAllStates()
    {
        Dictionary<string, byte> states = null;
        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                throw new Exception();
            }

            using var service = GetService();
            states = service.GetAllStates(GetCurrentTenantId(), GetCurrentUserName());
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return states;
    }

    public MessageClass[] GetRecentMessages(string to, int id)
    {
        MessageClass[] messages = null;
        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                throw new Exception();
            }

            using var service = GetService();
            messages = service.GetRecentMessages(GetCurrentTenantId(), GetCurrentUserName(), to, id);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return messages;
    }

    public void Ping(byte state)
    {
        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                throw new Exception();
            }

            using var service = GetService();
            service.Ping(_authContext.CurrentAccount.ID.ToString(), GetCurrentTenantId(), GetCurrentUserName(), state);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    private int GetCurrentTenantId()
    {
        return _tenantManager.GetCurrentTenant().Id;
    }

    private string GetCurrentUserName()
    {
        return _userManager.GetUsers(_authContext.CurrentAccount.ID).UserName;
    }

    private static void ProcessError(Exception error)
    {
        if (error is FaultException)
        {
            throw error;
        }
        if (error is CommunicationException || error is TimeoutException)
        {
            _lastErrorTime = DateTime.Now;
        }

        throw error;
    }

    private JabberServiceClientWcf GetService()
    {
        return new JabberServiceClientWcf();
    }
}
