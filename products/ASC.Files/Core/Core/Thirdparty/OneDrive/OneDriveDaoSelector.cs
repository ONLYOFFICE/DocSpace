namespace ASC.Files.Thirdparty.OneDrive;

[Scope(Additional = typeof(OneDriveDaoSelectorExtension))]
internal class OneDriveDaoSelector : RegexDaoSelectorBase<OneDriveProviderInfo>, IDaoSelector
{
    protected internal override string Name => "OneDrive";
    protected internal override string Id => "onedrive";

    public OneDriveDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public IFileDao<string> GetFileDao(string id)
    {
        return base.GetFileDao<OneDriveFileDao>(id);
    }

    public IFolderDao<string> GetFolderDao(string id)
    {
        return base.GetFolderDao<OneDriveFolderDao>(id);
    }

    public ITagDao<string> GetTagDao(string id)
    {
        return base.GetTagDao<OneDriveTagDao>(id);
    }

    public ISecurityDao<string> GetSecurityDao(string id)
    {
        return base.GetSecurityDao<OneDriveSecurityDao>(id);
    }
}

public static class OneDriveDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<OneDriveFileDao>();
        services.TryAdd<OneDriveFolderDao>();
        services.TryAdd<OneDriveTagDao>();
        services.TryAdd<OneDriveSecurityDao>();
    }
}
