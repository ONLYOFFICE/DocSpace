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

namespace ASC.Core.Common
{
    [Scope]
    public class CommonLinkUtilitySettings
    {
        public string ServerUri { get; set; }
    }

    [Scope]
    public class BaseCommonLinkUtility
    {
        public string VirtualRoot => ToAbsolute("~");
        protected IHttpContextAccessor HttpContextAccessor { get; set; }
        public string ServerRootPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_serverRootPath))
                {
                    return _serverRootPath;
                }

                UriBuilder result;
                // first, take from current request
                if (HttpContextAccessor?.HttpContext?.Request != null)
                {
                    var u = HttpContextAccessor?.HttpContext?.Request.GetUrlRewriter();
                    result = new UriBuilder(u.Scheme, u.Host, u.Port);

                    if (CoreBaseSettings.Standalone && !result.Uri.IsLoopback)
                    {
                        _serverRoot.Host = result.Host; // save for stanalone
                    }
                } 
                else result = new UriBuilder(_serverRoot.Uri);

                if (result.Uri.IsLoopback)
                {
                    // take values from db if localhost or no http context thread
                    var tenant = TenantManager.GetCurrentTenant();
                    result.Host = tenant.GetTenantDomain(_coreSettings);

#if DEBUG
                    if (tenant.TenantAlias == Localhost)
                    {
                        result.Host = Localhost; // for Visual Studio debug
                    }
#endif
                    if (!string.IsNullOrEmpty(tenant.MappedDomain))
                    {
                        var mapped = tenant.MappedDomain.ToLowerInvariant();
                        if (!mapped.Contains(Uri.SchemeDelimiter))
                        {
                            mapped = Uri.UriSchemeHttp + Uri.SchemeDelimiter + mapped;
                        }
                        result = new UriBuilder(mapped);
                    }
                }

                return _serverRootPath = result.Uri.ToString().TrimEnd('/');
            }
        }

        private const string Localhost = "localhost";

        protected readonly CoreBaseSettings CoreBaseSettings;
        protected readonly TenantManager TenantManager;

        private readonly CoreSettings _coreSettings;
        private UriBuilder _serverRoot;
        private string _vpath;
        private string _serverRootPath;

        public BaseCommonLinkUtility(
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            TenantManager tenantManager,
            IOptionsMonitor<ILog> options,
            CommonLinkUtilitySettings settings)
            : this(null, coreBaseSettings, coreSettings, tenantManager, options, settings) { }

        public BaseCommonLinkUtility(
            IHttpContextAccessor httpContextAccessor,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            TenantManager tenantManager,
            IOptionsMonitor<ILog> options,
            CommonLinkUtilitySettings settings)
        {
            var serverUri = settings.ServerUri;

            if (!string.IsNullOrEmpty(serverUri))
            {
                var uri = new Uri(serverUri.Replace('*', 'x').Replace('+', 'x'));
                _serverRoot = new UriBuilder(uri.Scheme, uri.Host != "x" ? uri.Host : Localhost, uri.Port);
                _vpath = "/" + uri.AbsolutePath.Trim('/');
            }
            else
            {
                try
                {
                    HttpContextAccessor = httpContextAccessor;
                    var uriBuilder = new UriBuilder(Uri.UriSchemeHttp, Localhost);
                    if (HttpContextAccessor?.HttpContext?.Request != null)
                    {
                        var u = HttpContextAccessor?.HttpContext.Request.GetUrlRewriter();
                        uriBuilder = new UriBuilder(u.Scheme, Localhost, u.Port);
                    }
                    _serverRoot = uriBuilder;
                }
                catch (Exception error)
                {
                    options.Get("ASC.Web").Error(error);
                }
            }

            CoreBaseSettings = coreBaseSettings;
            _coreSettings = coreSettings;
            TenantManager = tenantManager;
        }

        public string GetFullAbsolutePath(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                return ServerRootPath;
            }

            if (virtualPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("javascript:", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                return virtualPath;
            }

            return string.IsNullOrEmpty(virtualPath) || virtualPath.StartsWith("/")
                ? ServerRootPath + virtualPath
                : ServerRootPath + VirtualRoot.TrimEnd('/') + "/" + virtualPath.TrimStart('~', '/');
        }

        public string ToAbsolute(string virtualPath)
        {
            if (_vpath == null)
            {
                return VirtualPathUtility.ToAbsolute(virtualPath);
            }

            return string.IsNullOrEmpty(virtualPath) || virtualPath.StartsWith("/")
                ? virtualPath
                : (_vpath != "/" ? _vpath : string.Empty) + "/" + virtualPath.TrimStart('~', '/');
        }

        public static string GetRegionalUrl(string url, string lang)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            //-replace language
            var regex = new Regex("{.*?}");
            var matches = regex.Matches(url);

            if (string.IsNullOrEmpty(lang))
            {
                url = matches.Cast<Match>().Aggregate(url, (current, match) => current.Replace(match.Value, string.Empty));
            }
            else
            {
                foreach (Match match in matches)
                {
                    var values = match.Value.TrimStart('{').TrimEnd('}').Split('|');
                    url = url.Replace(match.Value, values.Contains(lang) ? lang : string.Empty);
                }
            }
            //-

            //--remove redundant slashes
            var uri = new Uri(url);

            if (uri.Scheme == "mailto")
            {
                return uri.OriginalString;
            }

            var baseUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port).Uri;
            baseUri = uri.Segments.Aggregate(baseUri, (current, segment) => new Uri(current, segment));
            //--
            //todo: lost query string!!!

            return baseUri.ToString().TrimEnd('/');
        }

        public void Initialize(string serverUri, bool localhost = true)
        {
            var uri = new Uri(serverUri.Replace('*', 'x').Replace('+', 'x'));
            _serverRoot = new UriBuilder(uri.Scheme, localhost ? Localhost : uri.Host, uri.Port);
            _vpath = "/" + uri.AbsolutePath.Trim('/');
        }
    }
}
