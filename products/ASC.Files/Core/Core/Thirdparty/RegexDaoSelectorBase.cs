namespace ASC.Files.Thirdparty;

internal abstract class RegexDaoSelectorBase<T> : IDaoSelector<T> where T : class, IProviderInfo
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDaoFactory _daoFactory;
    protected internal abstract string Name { get; }
    protected internal abstract string Id { get; }
    public Regex Selector => _selector ??= new Regex(@"^" + Id + @"-(?'id'\d+)(-(?'path'.*)){0,1}$", RegexOptions.Singleline | RegexOptions.Compiled);
    private Regex _selector;

    private Dictionary<string, ThirdPartyProviderDao<T>> Providers { get; set; }

    protected RegexDaoSelectorBase(
        IServiceProvider serviceProvider,
        IDaoFactory daoFactory)
    {
        _serviceProvider = serviceProvider;
        _daoFactory = daoFactory;
        Providers = new Dictionary<string, ThirdPartyProviderDao<T>>();
    }

    public virtual string ConvertId(string id)
    {
        try
        {
            if (id == null)
            {
                return null;
            }

            var match = Selector.Match(id);
            if (match.Success)
            {
                return match.Groups["path"].Value.Replace('|', '/');
            }

            throw new ArgumentException($"Id is not a {Name} id");
        }
        catch (Exception fe)
        {
            throw new FormatException("Can not convert id: " + id, fe);
        }
    }

    public string GetIdCode(string id)
    {
        if (id != null)
        {
            var match = Selector.Match(id);
            if (match.Success)
            {
                return match.Groups["id"].Value;
            }
        }

        throw new ArgumentException($"Id is not a {Name} id");
    }

    public virtual bool IsMatch(string id)
    {
        return id != null && Selector.IsMatch(id);
    }

    public virtual ISecurityDao<string> GetSecurityDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ISecurityDao<string>
    {
        return GetDao<T1>(id);
    }

    public virtual IFileDao<string> GetFileDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFileDao<string>
    {
        return GetDao<T1>(id);
    }

    public virtual ITagDao<string> GetTagDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ITagDao<string>
    {
        return GetDao<T1>(id);
    }

    public virtual IFolderDao<string> GetFolderDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFolderDao<string>
    {
        return GetDao<T1>(id);
    }

    private T1 GetDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>
    {
        var providerKey = $"{id}{typeof(T1)}";
        if (Providers.TryGetValue(providerKey, out var provider))
        {
            return (T1)provider;
        }

        var res = _serviceProvider.GetService<T1>();

        res.Init(GetInfo(id), this);

        Providers.Add(providerKey, res);

        return res;
    }

    internal BaseProviderInfo<T> GetInfo(string objectId)
    {
        ArgumentNullException.ThrowIfNull(objectId);

        var id = objectId;
        var match = Selector.Match(id);
        if (match.Success)
        {
            var providerInfo = GetProviderInfo(Convert.ToInt32(match.Groups["id"].Value));

            return new BaseProviderInfo<T>
            {
                Path = match.Groups["path"].Value,
                ProviderInfo = providerInfo,
                PathPrefix = Id + "-" + match.Groups["id"].Value
            };
        }

        throw new ArgumentException($"Id is not {Name} id");
    }

    public async Task RenameProviderAsync(T provider, string newTitle)
    {
        var dbDao = _serviceProvider.GetService<ProviderAccountDao>();
        await dbDao.UpdateProviderInfoAsync(provider.ID, newTitle, null, provider.RootFolderType);
        provider.UpdateTitle(newTitle); //This will update cached version too
    }

    protected virtual T GetProviderInfo(int linkId)
    {
        var dbDao = _daoFactory.ProviderDao;
        try
        {
            return dbDao.GetProviderInfoAsync(linkId).Result as T;
        }
        catch (InvalidOperationException)
        {
            throw new ProviderInfoArgumentException("Provider id not found or you have no access");
        }
    }

    public void Dispose()
    {
        foreach (var p in Providers)
        {
            p.Value.Dispose();
        }
    }
}
