using System;

namespace ASC.Common.Web
{
    public static class VirtualPathUtility
    {
        public static string ToAbsolute(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
                return virtualPath;

            if (Uri.IsWellFormedUriString(virtualPath, UriKind.Absolute))
                return virtualPath;

            return "/" + virtualPath.TrimStart('~', '/');
        }
    }
}
