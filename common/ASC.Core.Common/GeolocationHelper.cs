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

// hack for EF Core
public static class EntityFrameworkHelper
{
    public static int Compare(this byte[] b1, byte[] b2)
    {
        throw new Exception("This method can only be used in EF LINQ Context");
    }
}

[Scope]
public class GeolocationHelper
{
    private readonly IDbContextFactory<CustomDbContext> _dbContextFactory;
    private readonly ILogger<GeolocationHelper> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICache _cache;

    public GeolocationHelper(
        IDbContextFactory<CustomDbContext> dbContextFactory,
        ILogger<GeolocationHelper> logger,
        IHttpContextAccessor httpContextAccessor,
        ICache cache)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    public async Task<BaseEvent> AddGeolocationAsync(BaseEvent baseEvent)
    {
        var location = await GetGeolocationAsync(baseEvent.IP);
        baseEvent.Country = location[0];
        baseEvent.City = location[1];
        return baseEvent;
    }

    public async Task<string[]> GetGeolocationAsync(string ip)
    {
        try
        {
            var location = await GetIPGeolocationAsync(IPAddress.Parse(ip));
            if (string.IsNullOrEmpty(location.Key) || (location.Key == "ZZ"))
            {
                return new string[] { string.Empty, string.Empty };
            }
            var regionInfo = new RegionInfo(location.Key).EnglishName;
            return new string[] { regionInfo, location.City };
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
            return new string[] { string.Empty, string.Empty };
        }
    }

    public async Task<IPGeolocationInfo> GetIPGeolocationAsync(IPAddress address)
    {
        try
        {
            var cacheKey = $"ip_geolocation_info_${address}";
            var fromCache = _cache.Get<IPGeolocationInfo>(cacheKey);

            if (fromCache != null) return fromCache;

            await using var dbContext = _dbContextFactory.CreateDbContext();

            var addrType = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? "ipv4" : "ipv6";

            var result = await Queries.IpGeolocationInfoAsync(dbContext, addrType, address.GetAddressBytes());

            if (result != null)
            {
                _cache.Insert(cacheKey, result, TimeSpan.FromSeconds(15));
            }

            return result ?? IPGeolocationInfo.Default;
        }
        catch (Exception error)
        {
            _logger.ErrorGetIPGeolocation(error);
        }

        return IPGeolocationInfo.Default;
    }

    public async Task<IPGeolocationInfo> GetIPGeolocationFromHttpContextAsync()
    {
        if (_httpContextAccessor.HttpContext?.Request != null)
        {
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

            if (!ip.Equals(IPAddress.Loopback))
            {
                _logger.TraceRemoteIpAddress(ip.ToString());

                return await GetIPGeolocationAsync(ip);
            }
        }

        return IPGeolocationInfo.Default;
    }
}

static file class Queries
{
    public static readonly Func<CustomDbContext, string, byte[], Task<IPGeolocationInfo>> IpGeolocationInfoAsync =
        EF.CompileAsyncQuery(
            (CustomDbContext ctx, string addrType, byte[] address) =>
                ctx.DbIPLookup
                    .Where(r => r.AddrType == addrType && r.IPStart.Compare(address) <= 0)
                    .OrderByDescending(r => r.IPStart)
                    .Select(r => new IPGeolocationInfo
                    {
                        City = r.City,
                        IPEnd = new IPAddress(r.IPEnd),
                        IPStart = new IPAddress(r.IPStart),
                        Key = r.Country,
                        TimezoneOffset = r.TimezoneOffset,
                        TimezoneName = r.TimezoneName,
                        Continent = r.Continent
                    })
                    .FirstOrDefault());
}