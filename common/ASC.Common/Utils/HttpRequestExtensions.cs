using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace System.Web;

public static class HttpRequestExtensions
{
    public static readonly string UrlRewriterHeader = "X-REWRITER-URL";

    public static Uri GetUrlRewriter(this HttpRequest request)
    {
        return request != null ? GetUrlRewriter(request.Headers, request) : null;
    }

    public static Uri Url(this HttpRequest request)
    {
        return request != null ? new Uri(request.GetDisplayUrl()) : null;
    }

    /*public static Uri GetUrlRewriter(this HttpRequestBase request)
    {
        return request != null ? GetUrlRewriter(request.Headers, request.Url) : null;
    }*/

    public static Uri GetUrlRewriter(IHeaderDictionary headers, HttpRequest request)
    {
        if (headers != null)
        {
            var h = headers[UrlRewriterHeader];
            var rewriterUri = !string.IsNullOrEmpty(h) ? ParseRewriterUrl(h) : null;

            if (request != null && rewriterUri != null)
            {
                var result = new UriBuilder()
                {
                    Scheme = rewriterUri.Scheme,
                    Host = rewriterUri.Host,
                    Port = rewriterUri.Port
                };

                result.Query = request.QueryString.Value;
                result.Path = request.Path.Value;

                return result.Uri;
            }
        }

        if (request != null && request.Query != null)
        {
            var h = request.Query[UrlRewriterHeader];
            var rewriterUri = !string.IsNullOrEmpty(h) ? ParseRewriterUrl(h) : null;
            if (rewriterUri != null)
            {
                var result = new UriBuilder()
                {
                    Scheme = rewriterUri.Scheme,
                    Host = rewriterUri.Host,
                    Port = rewriterUri.Port
                };
                result.Query = request.QueryString.Value;
                result.Path = request.Path.Value;

                return result.Uri;
            }
        }

        return request.Url();
    }

    public static Uri PushRewritenUri(this HttpContext context)
    {
        return context != null ? PushRewritenUri(context, GetUrlRewriter(context.Request)) : null;
    }

    public static Uri PushRewritenUri(this HttpContext context, Uri rewrittenUri)
    {
        Uri oldUri = null;

        if (context != null)
        {
            var request = context.Request;

            var url = new Uri(request.GetDisplayUrl());

            if (url != rewrittenUri)
            {
                var requestUri = url;
                try
                {
                    //Push it
                    request.Headers.SetCommaSeparatedValues("HTTPS", rewrittenUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? "on" : "off");
                    request.Headers.SetCommaSeparatedValues("SERVER_NAME", rewrittenUri.Host);
                    request.Headers.SetCommaSeparatedValues("SERVER_PORT",
                                                rewrittenUri.Port.ToString(CultureInfo.InvariantCulture));

                    if (rewrittenUri.IsDefaultPort)
                    {
                        request.Headers.SetCommaSeparatedValues("HTTP_HOST",
                                                    rewrittenUri.Host);
                    }
                    else
                    {
                        request.Headers.SetCommaSeparatedValues("HTTP_HOST",
                                                    rewrittenUri.Host + ":" + requestUri.Port);
                    }
                    //Hack:
                    typeof(HttpRequest).InvokeMember("_url",
                                                      BindingFlags.NonPublic | BindingFlags.SetField |
                                                      BindingFlags.Instance,
                                                      null, request,
                                                      new object[] { null });
                    oldUri = requestUri;
                    context.Items["oldUri"] = oldUri;

                }
                catch (Exception) { }
            }
        }

        return oldUri;
    }

    public static Uri PopRewritenUri(this HttpContext context)
    {
        if (context != null && context.Items["oldUri"] != null)
        {
            var rewriteTo = context.Items["oldUri"] as Uri;

            if (rewriteTo != null)
            {
                return PushRewritenUri(context, rewriteTo);
            }
        }

        return null;
    }

    public static bool DesktopApp(this HttpRequest request)
    {
        return request != null
            && (!string.IsNullOrEmpty(request.Query["desktop"])
                || !string.IsNullOrEmpty(request.Headers[HeaderNames.UserAgent]) && request.Headers[HeaderNames.UserAgent].ToString().Contains("AscDesktopEditor"));
    }

    public static bool SailfishApp(this HttpRequest request)
    {
        return request != null
               && (!string.IsNullOrEmpty(request.Headers["sailfish"])
                   || !string.IsNullOrEmpty(request.Headers[HeaderNames.UserAgent]) && request.Headers[HeaderNames.UserAgent].ToString().Contains("SailfishOS"));
    }

    public static string GetUserHostAddress(this HttpRequest request)
    {
        return request.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
    }

    private static Uri ParseRewriterUrl(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
            if (0 < s.Length && s.StartsWith('0'))
        {
            s = Uri.UriSchemeHttp + s.Substring(1);
        }
        else if (3 < s.Length && s.StartsWith("OFF", cmp))
        {
            s = Uri.UriSchemeHttp + s.Substring(3);
        }
            else if (0 < s.Length && s.StartsWith('1'))
        {
            s = Uri.UriSchemeHttps + s.Substring(1);
        }
        else if (2 < s.Length && s.StartsWith("ON", cmp))
        {
            s = Uri.UriSchemeHttps + s.Substring(2);
        }
        else if (s.StartsWith(Uri.UriSchemeHttp + "%3A%2F%2F", cmp) || s.StartsWith(Uri.UriSchemeHttps + "%3A%2F%2F", cmp))
        {
            s = HttpUtility.UrlDecode(s);
        }

        Uri.TryCreate(s, UriKind.Absolute, out var result);

        return result;
    }
}
