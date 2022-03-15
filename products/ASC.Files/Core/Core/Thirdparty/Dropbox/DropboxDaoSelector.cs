namespace ASC.Files.Thirdparty.Dropbox;

[Scope(Additional = typeof(DropboxDaoSelectorExtension))]
internal class DropboxDaoSelector : RegexDaoSelectorBase<DropboxProviderInfo>, IDaoSelector
{
    protected internal override string Name => "Dropbox";
    protected internal override string Id => "dropbox";

    public DropboxDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public IFileDao<string> GetFileDao(string id)
    {
        return base.GetFileDao<DropboxFileDao>(id);
    }

    public IFolderDao<string> GetFolderDao(string id)
    {
        return base.GetFolderDao<DropboxFolderDao>(id);
    }

    public ITagDao<string> GetTagDao(string id)
    {
        return base.GetTagDao<DropboxTagDao>(id);
    }

    public ISecurityDao<string> GetSecurityDao(string id)
    {
        return base.GetSecurityDao<DropboxSecurityDao>(id);
    }
}

public static class DropboxDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DropboxFileDao>();
        services.TryAdd<DropboxFolderDao>();
        services.TryAdd<DropboxTagDao>();
        services.TryAdd<DropboxSecurityDao>();
    }
}
