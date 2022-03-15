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

namespace ASC.Data.Storage.DiscStorage;

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

    private static string CombinePath(string physPath, HttpContext requestContext)
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
