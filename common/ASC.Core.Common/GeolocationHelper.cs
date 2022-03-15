using DbContext = ASC.Core.Common.EF.Context.DbContext;

namespace ASC.Geolocation;

public class GeolocationHelper
{
    public string Dbid { get; set; }
    public ILog Logger { get; }
    private readonly DbContext _dbContext;

    public GeolocationHelper(DbContextManager<DbContext> dbContext, IOptionsMonitor<ILog> option)
    {
        Logger = option.CurrentValue;
        _dbContext = dbContext.Get(Dbid);
    }

    public IPGeolocationInfo GetIPGeolocation(string ip)
    {
        try
        {
            var ipformatted = FormatIP(ip);
            var q = _dbContext.DbipLocation
                .Where(r => r.IPStart.CompareTo(ipformatted) <= 0)
                .Where(r => ipformatted.CompareTo(r.IPEnd) <= 0)
                .OrderByDescending(r => r.IPStart)
                .Select(r => new IPGeolocationInfo
                {
                    City = r.City,
                    IPEnd = r.IPEnd,
                    IPStart = r.IPStart,
                    Key = r.Country,
                    TimezoneOffset = r.TimezoneOffset,
                    TimezoneName = r.TimezoneName
                })
                .FirstOrDefault();

            return q ?? IPGeolocationInfo.Default;
        }
        catch (Exception error)
        {
            Logger.Error(error);
        }

        return IPGeolocationInfo.Default;
    }

    public IPGeolocationInfo GetIPGeolocationFromHttpContext(Microsoft.AspNetCore.Http.HttpContext context)
    {
        if (context != null && context.Request != null)
        {
            var ip = (string)(context.Request.HttpContext.Items["X-Forwarded-For"] ?? context.Request.HttpContext.Items["REMOTE_ADDR"]);
            if (!string.IsNullOrWhiteSpace(ip))
            {
                return GetIPGeolocation(ip);
            }
        }

        return IPGeolocationInfo.Default;
    }

    private static string FormatIP(string ip)
    {
        ip = (ip ?? "").Trim();
        if (ip.Contains('.'))
        {
            //ip v4
            if (ip.Length == 15)
            {
                return ip;
            }

            return string.Join(".", ip.Split(':')[0].Split('.').Select(s => ("00" + s).Substring(s.Length - 1)).ToArray());
        }
        else if (ip.Contains(':'))
        {
            //ip v6
            if (ip.Length == 39)
            {
                return ip;
            }
            var index = ip.IndexOf("::");
            if (0 <= index)
            {
                ip = ip.Insert(index + 2, new string(':', 8 - ip.Split(':').Length));
            }

            return string.Join(":", ip.Split(':').Select(s => ("0000" + s).Substring(s.Length)).ToArray());
        }
        else
        {
            throw new ArgumentException("Unknown ip " + ip);
        }
    }
}
