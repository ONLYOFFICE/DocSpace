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
