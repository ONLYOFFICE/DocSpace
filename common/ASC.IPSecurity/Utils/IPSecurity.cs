/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

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
