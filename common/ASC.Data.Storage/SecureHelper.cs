namespace ASC.Data.Storage;

public static class SecureHelper
{
    public static bool IsSecure(HttpContext httpContext, IOptionsMonitor<ILog> options)
    {
        try
        {
            return httpContext != null && Uri.UriSchemeHttps.Equals(httpContext.Request.GetUrlRewriter().Scheme, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception err)
        {
            options.Get("ASC.Data.Storage.SecureHelper").Error(err);

            return false;
        }
    }
}
