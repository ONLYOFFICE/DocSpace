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


using System;
using System.Linq;

using ASC.Common.Logging;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;

using Microsoft.Extensions.Options;

namespace ASC.Geolocation
{
    public class GeolocationHelper
    {
        public string Dbid { get; set; }

        public ILog Log { get; }
        public DbContext DbContext { get; }

        public GeolocationHelper(DbContextManager<DbContext> dbContext, IOptionsMonitor<ILog> option)
        {
            Log = option.CurrentValue;
            DbContext = dbContext.Get(Dbid);
        }


        public IPGeolocationInfo GetIPGeolocation(string ip)
        {
            try
            {
                var ipformatted = FormatIP(ip);
                var q = DbContext.DbipLocation
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
                Log.Error(error);
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
}