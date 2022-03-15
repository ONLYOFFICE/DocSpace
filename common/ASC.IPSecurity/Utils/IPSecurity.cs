namespace ASC.IPSecurity;

[Scope]
public class IPSecurity
{
    public bool IpSecurityEnabled { get; }

    private readonly ILog _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly IPRestrictionsService _ipRestrictionsService;
    private readonly string _currentIpForTest;

    public IPSecurity(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        AuthContext authContext,
        TenantManager tenantManager,
        IPRestrictionsService iPRestrictionsService,
        IOptionsMonitor<ILog> options)
    {
        _logger = options.Get("ASC.IPSecurity");
        _httpContextAccessor = httpContextAccessor;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _ipRestrictionsService = iPRestrictionsService;
        _currentIpForTest = configuration["ipsecurity:test"];
        var hideSettings = (configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
        IpSecurityEnabled = !hideSettings.Contains("IpSecurity", StringComparer.CurrentCultureIgnoreCase);
    }

    public bool Verify()
    {
        var tenant = _tenantManager.GetCurrentTenant();

        if (!IpSecurityEnabled)
        {
            return true;
        }

        if (_httpContextAccessor?.HttpContext == null)
        {
            return true;
        }

        if (tenant == null || _authContext.CurrentAccount.ID == tenant.OwnerId)
        {
            return true;
        }

        string requestIps = null;
        try
        {
            var restrictions = _ipRestrictionsService.Get(tenant.Id).ToList();

            if (restrictions.Count == 0)
            {
                return true;
            }

            requestIps = _currentIpForTest;

            if (string.IsNullOrWhiteSpace(requestIps))
            {
                var request = _httpContextAccessor.HttpContext.Request;
                requestIps = request.Headers["X-Forwarded-For"].FirstOrDefault() ?? request.GetUserHostAddress();
            }

            var ips = string.IsNullOrWhiteSpace(requestIps)
                          ? Array.Empty<string>()
                          : requestIps.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            if (ips.Any(requestIp => restrictions.Any(restriction => MatchIPs(GetIpWithoutPort(requestIp), restriction.Ip))))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorFormat("Can't verify request with IP-address: {0}. Tenant: {1}. Error: {2} ", requestIps ?? "", tenant, ex);

            return false;
        }

        _logger.InfoFormat("Restricted from IP-address: {0}. Tenant: {1}. Request to: {2}", requestIps ?? "", tenant, _httpContextAccessor.HttpContext.Request.GetDisplayUrl());

        return false;
    }

    private static bool MatchIPs(string requestIp, string restrictionIp)
    {
        var dividerIdx = restrictionIp.IndexOf('-');
        if (dividerIdx > -1)
        {
            var lower = IPAddress.Parse(restrictionIp.Substring(0, dividerIdx).Trim());
            var upper = IPAddress.Parse(restrictionIp.Substring(dividerIdx + 1).Trim());

            var range = new IPAddressRange(lower, upper);

            return range.IsInRange(IPAddress.Parse(requestIp));
        }

        return requestIp == restrictionIp;
    }

    private static string GetIpWithoutPort(string ip)
    {
        var portIdx = ip.IndexOf(':');

        return portIdx > 0 ? ip.Substring(0, portIdx) : ip;
    }
}
