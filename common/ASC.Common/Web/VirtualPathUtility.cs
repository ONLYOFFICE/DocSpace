using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace ASC.Common.Web
{
    public class VirtualPathUtility
    {
        public static string ToAbsolute(string virtualPath)
        {
            var originalUri = HttpContext.Current.Request.GetUrlRewriter();
            return UriHelper.BuildAbsolute(originalUri.Scheme, new HostString(originalUri.Host, originalUri.Port), virtualPath);
        }
    }
}
