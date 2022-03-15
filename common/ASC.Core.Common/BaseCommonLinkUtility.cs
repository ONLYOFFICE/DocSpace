namespace ASC.Core.Common;

[Scope]
public class CommonLinkUtilitySettings
{
    public string ServerUri { get; set; }
}


[Scope]
public class BaseCommonLinkUtility
{
    private const string LocalHost = "localhost";

    private UriBuilder _serverRoot;
    private string _vpath;

    protected IHttpContextAccessor HttpContextAccessor;

    public BaseCommonLinkUtility(
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        TenantManager tenantManager,
        IOptionsMonitor<ILog> options,
        CommonLinkUtilitySettings settings)
        : this(null, coreBaseSettings, coreSettings, tenantManager, options, settings)
    {
    }

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
            _serverRoot = new UriBuilder(uri.Scheme, uri.Host != "x" ? uri.Host : LocalHost, uri.Port);
            _vpath = "/" + uri.AbsolutePath.Trim('/');
        }
        else
        {
            try
            {
                HttpContextAccessor = httpContextAccessor;
                var uriBuilder = new UriBuilder(Uri.UriSchemeHttp, LocalHost);
                if (HttpContextAccessor?.HttpContext?.Request != null)
                {
                    var u = HttpContextAccessor?.HttpContext.Request.GetUrlRewriter();

                    ArgumentNullException.ThrowIfNull(u);

                    uriBuilder = new UriBuilder(u.Scheme, LocalHost, u.Port);
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

    public string VirtualRoot => ToAbsolute("~");

    protected CoreBaseSettings CoreBaseSettings;
    private readonly CoreSettings _coreSettings;
    protected TenantManager TenantManager;

    private string _serverRootPath;
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

                ArgumentNullException.ThrowIfNull(u);

                result = new UriBuilder(u.Scheme, u.Host, u.Port);

                if (CoreBaseSettings.Standalone && !result.Uri.IsLoopback)
                {
                    // save for stanalone
                    _serverRoot.Host = result.Host;
                }
            }
            else
            {
                result = new UriBuilder(_serverRoot.Uri);
            }

            if (result.Uri.IsLoopback)
            {
                // take values from db if localhost or no http context thread
                var tenant = TenantManager.GetCurrentTenant();
                result.Host = tenant.GetTenantDomain(_coreSettings);

#if DEBUG
                // for Visual Studio debug
                if (tenant.Alias == LocalHost)
                {
                    result.Host = LocalHost;
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

        if (virtualPath.StartsWith('/'))
        {
            return ServerRootPath + virtualPath;
        }

        return ServerRootPath + VirtualRoot.TrimEnd('/') + "/" + virtualPath.TrimStart('~', '/');
    }

    public string ToAbsolute(string virtualPath)
    {
        if (_vpath == null)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        if (string.IsNullOrEmpty(virtualPath) || virtualPath.StartsWith('/'))
        {
            return virtualPath;
        }

        return (_vpath != "/" ? _vpath : string.Empty) + "/" + virtualPath.TrimStart('~', '/');
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
            url = matches.Aggregate(url, (current, match) => current.Replace(match.Value, string.Empty));
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
        _serverRoot = new UriBuilder(uri.Scheme, localhost ? LocalHost : uri.Host, uri.Port);
        _vpath = "/" + uri.AbsolutePath.Trim('/');
    }
}
