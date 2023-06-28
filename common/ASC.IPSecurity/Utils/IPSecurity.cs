// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.IPSecurity;

[Scope]
public class IPSecurity
{
    public bool IpSecurityEnabled { get; }

    private readonly ILogger<IPSecurity> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly IPRestrictionsService _ipRestrictionsService;
    private readonly string _currentIpForTest;
    private readonly string _myNetworks;
    private readonly UserManager _userManager;

    public IPSecurity(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        AuthContext authContext,
        TenantManager tenantManager,
        IPRestrictionsService iPRestrictionsService,
        UserManager userManager,
        ILogger<IPSecurity> logger)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _ipRestrictionsService = iPRestrictionsService;
        _userManager = userManager;
        _currentIpForTest = configuration["ipsecurity:test"];
        _myNetworks = configuration["ipsecurity:mynetworks"];
        var hideSettings = (configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
        IpSecurityEnabled = !hideSettings.Contains("IpSecurity", StringComparer.CurrentCultureIgnoreCase);
    }

    public async Task<bool> VerifyAsync()
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();

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
            var restrictions = (await _ipRestrictionsService.GetAsync(tenant.Id)).ToList();

            if (restrictions.Count == 0)
            {
                return true;
            }

            requestIps = _currentIpForTest;

            if (string.IsNullOrWhiteSpace(requestIps))
            {
                requestIps = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            var ips = string.IsNullOrWhiteSpace(requestIps)
                          ? Array.Empty<string>()
                          : requestIps.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            var isDocSpaceAdmin = await _userManager.IsUserInGroupAsync(_authContext.CurrentAccount.ID, Core.Users.Constants.GroupAdmin.ID);

            if (ips.Any(requestIp => restrictions.Any(restriction => (restriction.ForAdmin ? isDocSpaceAdmin : true) && MatchIPs(GetIpWithoutPort(requestIp), restriction.Ip))))
            {
                return true;
            }
            if (IsMyNetwork(ips))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorCantVerifyRequest(requestIps ?? "", tenant, ex);

            return false;
        }

        _logger.InformationRestricted(requestIps ?? "", tenant, _httpContextAccessor.HttpContext.Request.GetDisplayUrl());

        return false;
    }

    public static bool MatchIPs(string requestIp, string restrictionIp)
    {
        var dividerIdx = restrictionIp.IndexOf('-');
        if (dividerIdx > 0)
        {
            var lower = IPAddress.Parse(restrictionIp.Substring(0, dividerIdx).Trim());
            var upper = IPAddress.Parse(restrictionIp.Substring(dividerIdx + 1).Trim());

            var range = new IPAddressRange(lower, upper);

            return range.IsInRange(IPAddress.Parse(requestIp));
        }

        if (restrictionIp.IndexOf('/') > 0)
        {
            return IPAddressRange.IsInRange(requestIp, restrictionIp);
        }

        return requestIp == restrictionIp;
    }

    private static string GetIpWithoutPort(string ip)
    {
        var portIdx = ip.IndexOf(':');

        return portIdx > 0 ? ip.Substring(0, portIdx) : ip;
    }

    private bool IsMyNetwork(string[] ips)
    {
        try
        {
            if (!string.IsNullOrEmpty(_myNetworks))
            {
                var myNetworkIps = _myNetworks.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

                if (ips.Any(requestIp => myNetworkIps.Any(ipAddress => MatchIPs(GetIpWithoutPort(requestIp), ipAddress))))
                {
                    return true;
                }
            }

            var hostName = Dns.GetHostName();
            var hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());

            var localIPs = new List<IPAddress> { IPAddress.IPv6Loopback, IPAddress.Loopback };

            localIPs.AddRange(hostAddresses.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6));

            foreach (var ipAddress in localIPs)
            {
                if (ips.Contains(ipAddress.ToString()))
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorCantVerifyLocalNetWork(string.Join(",", ips), ex);
        }

        return false;
    }
}
