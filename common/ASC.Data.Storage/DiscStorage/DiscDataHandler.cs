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


using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using ASC.Common.Utils;
using ASC.Common.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ASC.Data.Storage.DiscStorage
{
    public class DiscDataHandler
    {
        private readonly string _physPath;
        private readonly bool _checkAuth;

        public DiscDataHandler(string physPath, bool checkAuth = true)
        {
            _physPath = physPath;
            _checkAuth = checkAuth;
        }

        public async Task Invoke(HttpContext context)
        {
            //TODO
            //if (_checkAuth && !Core.SecurityContext.IsAuthenticated)
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //    return;
            //}

            var path = CombinePath(_physPath, context);
            if (File.Exists(path))
            {
                var lastwrite = File.GetLastWriteTime(path);
                var etag = '"' + lastwrite.Ticks.ToString("X8", CultureInfo.InvariantCulture) + '"';

                var notmodified = context.Request.Headers["If-None-Match"] == etag ||
                                  context.Request.Headers["If-Modified-Since"] == lastwrite.ToString("R");

                if (notmodified)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                }
                else
                {
                    if (File.Exists(path + ".gz"))
                    {
                        await context.Response.SendFileAsync(path + ".gz");
                        context.Response.Headers["Content-Encoding"] = "gzip";
                    }
                    else
                    {
                        await context.Response.SendFileAsync(path);
                    }
                    context.Response.ContentType = MimeMapping.GetMimeMapping(path);
                    //TODO
                    //context.Response.Cache.SetVaryByCustom("*");
                    //context.Response.Cache.SetAllowResponseInBrowserHistory(true);
                    //context.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
                    //context.Response.Cache.SetLastModified(lastwrite);
                    //context.Response.Cache.SetETag(etag);
                    //context.Response.Cache.SetCacheability(HttpCacheability.Public);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        static string CombinePath(string physPath, HttpContext requestContext)
        {
            var pathInfo = GetRouteValue("pathInfo").Replace('/', Path.DirectorySeparatorChar);

            var path = CrossPlatform.PathCombine(physPath, pathInfo);

            var tenant = GetRouteValue("0");
            if (string.IsNullOrEmpty(tenant))
            {
                tenant = CrossPlatform.PathCombine(GetRouteValue("t1"), GetRouteValue("t2"), GetRouteValue("t3"));
            }

            path = path.Replace("{0}", tenant);
            return path;

            string GetRouteValue(string name)
            {
                return (requestContext.GetRouteValue(name) ?? "").ToString();
            }
        }
    }

    public static class DiscDataHandlerExtensions
    {
        public static IEndpointRouteBuilder RegisterDiscDataHandler(this IEndpointRouteBuilder builder, string virtPath, string physPath, bool publicRoute = false)
        {
            if (virtPath != "/")
            {
                var handler = new DiscDataHandler(physPath, !publicRoute);
                var url = virtPath + "{*pathInfo}";

                if (!builder.DataSources.Any(r => r.Endpoints.Any(e => e.DisplayName == url)))
                {
                    builder.Map(url, handler.Invoke);

                    var newUrl = url.Replace("{0}", "{t1}/{t2}/{t3}");

                    if (newUrl != url)
                    {
                        builder.Map(url, handler.Invoke);
                    }
                }
            }

            return builder;
        }
    }
}