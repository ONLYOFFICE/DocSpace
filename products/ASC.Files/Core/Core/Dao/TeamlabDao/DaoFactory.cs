namespace ASC.Files.Core.Data;

[Scope]
public class DaoFactory : IDaoFactory
{
    private readonly IServiceProvider _serviceProvider;
    public IProviderDao ProviderDao { get; }

    public DaoFactory(IServiceProvider serviceProvider, IProviderDao providerDao)
    {
        _serviceProvider = serviceProvider;
        ProviderDao = providerDao;
    }

    public IFileDao<T> GetFileDao<T>()
    {
        return _serviceProvider.GetService<IFileDao<T>>();
    }

    public IFolderDao<T> GetFolderDao<T>()
    {
        return _serviceProvider.GetService<IFolderDao<T>>();
    }

    public ITagDao<T> GetTagDao<T>()
    {
        return _serviceProvider.GetService<ITagDao<T>>();
    }

    public ISecurityDao<T> GetSecurityDao<T>()
    {
        return _serviceProvider.GetService<ISecurityDao<T>>();
    }

    public ILinkDao GetLinkDao()
    {
        return _serviceProvider.GetService<ILinkDao>();
    }
}

public static class DaoFactoryExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<File<int>>();
        services.TryAdd<IFileDao<int>, FileDao>();

        services.TryAdd<File<string>>();
        services.TryAdd<IFileDao<string>, ProviderFileDao>();

        services.TryAdd<Folder<int>>();
        services.TryAdd<IFolderDao<int>, FolderDao>();

        services.TryAdd<Folder<string>>();
        services.TryAdd<IFolderDao<string>, ProviderFolderDao>();

        services.TryAdd<SecurityDao<int>>();
        services.TryAdd<ISecurityDao<int>, SecurityDao<int>>();
        services.TryAdd<ISecurityDao<string>, ProviderSecurityDao>();

        services.TryAdd<ITagDao<int>, TagDao<int>>();
        services.TryAdd<ITagDao<string>, ProviderTagDao>();

        services.TryAdd<ILinkDao, LinkDao>();

        services.TryAdd<EditHistory>();
    }
}
