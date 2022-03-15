namespace ASC.Data.Storage.DiscStorage;

internal class MappedPath
{
    public string PhysicalPath { get; set; }

    private readonly PathUtils _pathUtils;

    public MappedPath(PathUtils pathUtils, string tenant, bool appendTenant, string ppath, IDictionary<string, string> storageConfig) : this(pathUtils)
    {
        tenant = tenant.Trim('/');

        ppath = _pathUtils.ResolvePhysicalPath(ppath, storageConfig);
        PhysicalPath = ppath.IndexOf('{') == -1 && appendTenant ? CrossPlatform.PathCombine(ppath, tenant) : string.Format(ppath, tenant);
    }

    private MappedPath(PathUtils pathUtils)
    {
        _pathUtils = pathUtils;
    }

    public MappedPath AppendDomain(string domain)
    {
        domain = domain.Replace('.', '_'); //Domain prep. Remove dots

        return new MappedPath(_pathUtils)
        {
            PhysicalPath = CrossPlatform.PathCombine(PhysicalPath, PathUtils.Normalize(domain, true)),
        };
    }
}
