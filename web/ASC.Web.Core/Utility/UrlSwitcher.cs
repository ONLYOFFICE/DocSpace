namespace ASC.Web.Core.Utility
{
    public static class UrlSwitcher
    {
        public static string SelectCurrentUriScheme(this HttpContext httpContext, string uri)
        {
            return httpContext != null ? SelectUriScheme(uri, httpContext.Request.GetUrlRewriter().Scheme) : uri;
        }

        public static string SelectUriScheme(string uri, string scheme)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute) ? SelectUriScheme(new Uri(uri, UriKind.Absolute), scheme).ToString() : uri;
        }

        public static Uri SelectCurrentUriScheme(this HttpContext httpContext, Uri uri)
        {
            if (httpContext != null)
            {
                return SelectUriScheme(uri, httpContext.Request.GetUrlRewriter().Scheme);
            }
            return uri;
        }

        public static Uri SelectUriScheme(Uri uri, string scheme)
        {
            if (!string.IsNullOrEmpty(scheme) && !scheme.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Switch
                var builder = new UriBuilder(uri) { Scheme = scheme.ToLowerInvariant(), Port = scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? 443 : 80 };//Set proper port!
                return builder.Uri;
            }
            return uri;
        }

        public static Uri ToCurrentScheme(this Uri uri, HttpContext httpContext)
        {
            return SelectCurrentUriScheme(httpContext, uri);
        }

        public static Uri ToScheme(this Uri uri, string scheme)
        {
            return SelectUriScheme(uri, scheme);
        }
    }
}