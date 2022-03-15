namespace ASC.Web.Files.Api;

[Scope]
public class FilesIntegration
{
    private static readonly IDictionary<string, IFileSecurityProvider> _providers = new Dictionary<string, IFileSecurityProvider>();

    public IDaoFactory DaoFactory { get; }

    public FilesIntegration(IDaoFactory daoFactory)
    {
        DaoFactory = daoFactory;
    }

    public Task<T> RegisterBunchAsync<T>(string module, string bunch, string data)
    {
        var folderDao = DaoFactory.GetFolderDao<T>();

        return folderDao.GetFolderIDAsync(module, bunch, data, true);
    }

    public Task<IEnumerable<T>> RegisterBunchFoldersAsync<T>(string module, string bunch, IEnumerable<string> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        data = data.ToList();
        if (!data.Any())
        {
            return Task.FromResult<IEnumerable<T>>(new List<T>());
        }

        var folderDao = DaoFactory.GetFolderDao<T>();
        return folderDao.GetFolderIDsAsync(module, bunch, data, true);
    }

    public bool IsRegisteredFileSecurityProvider(string module, string bunch)
    {
        lock (_providers)
        {
            return _providers.ContainsKey(module + bunch);
        }

    }

    public void RegisterFileSecurityProvider(string module, string bunch, IFileSecurityProvider securityProvider)
    {
        lock (_providers)
        {
            _providers[module + bunch] = securityProvider;
        }
    }

    internal static IFileSecurity GetFileSecurity(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var parts = path.Split('/');
        if (parts.Length < 3)
        {
            return null;
        }

        IFileSecurityProvider provider;
        lock (_providers)
        {
            _providers.TryGetValue(parts[0] + parts[1], out provider);
        }

        return provider?.GetFileSecurity(parts[2]);
    }

    internal static Dictionary<object, IFileSecurity> GetFileSecurity(Dictionary<string, string> paths)
    {
        var result = new Dictionary<object, IFileSecurity>();
        var gropped = paths.GroupBy(r =>
        {
            var parts = r.Value.Split('/');
            if (parts.Length < 3)
            {
                return string.Empty;
            }

            return parts[0] + parts[1];
        }, v =>
        {
            var parts = v.Value.Split('/');
            if (parts.Length < 3)
            {
                return new KeyValuePair<string, string>(v.Key, "");
            }

            return new KeyValuePair<string, string>(v.Key, parts[2]);
        });

        foreach (var grouping in gropped)
        {
            IFileSecurityProvider provider;
            lock (_providers)
            {
                _providers.TryGetValue(grouping.Key, out provider);
            }

            if (provider == null)
            {
                continue;
            }

            var data = provider.GetFileSecurity(grouping.ToDictionary(r => r.Key, r => r.Value));

            foreach (var d in data)
            {
                result.Add(d.Key, d.Value);
            }
        }

        return result;
    }
}
