namespace ASC.Data.Storage;

[Singletone]
public class PathUtils
{
    public IHostEnvironment HostEnvironment { get; }

    private readonly string _storageRoot;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PathUtils(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        HostEnvironment = hostEnvironment;
        _storageRoot = _configuration[Constants.StorageRootParam];
    }

    public PathUtils(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IWebHostEnvironment webHostEnvironment) : this(configuration, hostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public static string Normalize(string path, bool addTailingSeparator = false)
    {
        path = path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace("\\\\", Path.DirectorySeparatorChar.ToString())
            .Replace("//", Path.DirectorySeparatorChar.ToString())
            .TrimEnd(Path.DirectorySeparatorChar);

        return addTailingSeparator && 0 < path.Length ? path + Path.DirectorySeparatorChar : path;
    }

    public string ResolveVirtualPath(string module, string domain)
    {
            var url = $"~/storage/{module}/{(string.IsNullOrEmpty(domain) ? "root" : domain)}/";

        return ResolveVirtualPath(url);
    }

    public string ResolveVirtualPath(string virtPath, bool addTrailingSlash = true)
    {
        if (virtPath == null)
        {
            virtPath = "";
        }

        if (virtPath.StartsWith('~') && !Uri.IsWellFormedUriString(virtPath, UriKind.Absolute))
        {
            var rootPath = "/";
            if (!string.IsNullOrEmpty(_webHostEnvironment?.WebRootPath) && _webHostEnvironment?.WebRootPath.Length > 1)
            {
                rootPath = _webHostEnvironment?.WebRootPath.Trim('/');
            }

            virtPath = virtPath.Replace("~", rootPath);
        }
        if (addTrailingSlash)
        {
            virtPath += "/";
        }
        else
        {
            virtPath = virtPath.TrimEnd('/');
        }

        return virtPath.Replace("//", "/");
    }

    public string ResolvePhysicalPath(string physPath, IDictionary<string, string> storageConfig)
    {
        physPath = Normalize(physPath, false).TrimStart('~');

        if (physPath.Contains(Constants.StorageRootParam))
        {
            physPath = physPath.Replace(Constants.StorageRootParam, _storageRoot ?? storageConfig[Constants.StorageRootParam]);
        }

        if (!Path.IsPathRooted(physPath))
        {
            physPath = Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, physPath.Trim(Path.DirectorySeparatorChar)));
        }

        return physPath;
    }
}
