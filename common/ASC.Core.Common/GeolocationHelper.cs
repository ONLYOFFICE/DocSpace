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

namespace ASC.Geolocation;

[Scope]
public class GeolocationHelper
{
    private readonly IDbContextFactory<CustomDbContext> _dbContextFactory;
    private readonly ILogger<GeolocationHelper> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GeolocationHelper(
        IDbContextFactory<CustomDbContext> dbContextFactory,
        ILogger<GeolocationHelper> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IPGeolocationInfo GetIPGeolocation(string ip)
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var ipformatted = FormatIP(ip);
            var q = dbContext.DbipLocation
                .Where(r => r.IPStart.CompareTo(ipformatted) <= 0)
                .Where(r => ipformatted.CompareTo(r.IPEnd) <= 0)
                .OrderByDescending(r => r.IPStart)
                .Select(r => new IPGeolocationInfo
                {
                    City = r.City,
                    IPEnd = r.IPEnd,
                    IPStart = r.IPStart,
                    Key = r.Country,
                    TimezoneOffset = r.TimezoneOffset ?? 0,
                    TimezoneName = r.TimezoneName
                })
                .FirstOrDefault();

            return q ?? IPGeolocationInfo.Default;
        }
        catch (Exception error)
        {
            _logger.ErrorGetIPGeolocation(error);
        }

        return IPGeolocationInfo.Default;
    }

    public IPGeolocationInfo GetIPGeolocationFromHttpContext()
    {
        if (_httpContextAccessor.HttpContext?.Request != null)
        {
            var ip = (string)(_httpContextAccessor.HttpContext.Items["X-Forwarded-For"] ?? _httpContextAccessor.HttpContext.Items["REMOTE_ADDR"]);
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
