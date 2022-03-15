namespace ASC.MessagingSystem.Models;

[Singletone]
public class MessagePolicy
{
    private readonly IEnumerable<string> _secretIps;

    public MessagePolicy(IConfiguration configuration)
    {
        _secretIps =
            configuration["messaging.secret-ips"] == null
            ? Array.Empty<string>()
            : configuration["messaging.secret-ips"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public bool Check(EventMessage message)
    {
        if (message == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(message.Ip))
        {
            return true;
        }

        var ip = GetIpWithoutPort(message.Ip);

        return _secretIps.All(x => x != ip);
    }

    private static string GetIpWithoutPort(string ip)
    {
        if (ip == null)
        {
            return null;
        }

        var portIdx = ip.IndexOf(':');

        return portIdx > -1 ? ip.Substring(0, portIdx) : ip;
    }
}
