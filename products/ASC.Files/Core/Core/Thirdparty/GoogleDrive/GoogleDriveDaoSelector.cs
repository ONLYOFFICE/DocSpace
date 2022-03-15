namespace ASC.Files.Thirdparty.GoogleDrive;

[Scope(Additional = typeof(GoogleDriveDaoSelectorExtension))]
internal class GoogleDriveDaoSelector : RegexDaoSelectorBase<GoogleDriveProviderInfo>, IDaoSelector
{
    protected internal override string Name => "GoogleDrive";
    protected internal override string Id => "drive";

    public GoogleDriveDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public IFileDao<string> GetFileDao(string id)
    {
        return base.GetFileDao<GoogleDriveFileDao>(id);
    }

    public IFolderDao<string> GetFolderDao(string id)
    {
        return base.GetFolderDao<GoogleDriveFolderDao>(id);
    }

    public ITagDao<string> GetTagDao(string id)
    {
        return base.GetTagDao<GoogleDriveTagDao>(id);
    }

    public ISecurityDao<string> GetSecurityDao(string id)
    {
        return base.GetSecurityDao<GoogleDriveSecurityDao>(id);
    }
}

public static class GoogleDriveDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<GoogleDriveFileDao>();
        services.TryAdd<GoogleDriveFolderDao>();
        services.TryAdd<GoogleDriveTagDao>();
        services.TryAdd<GoogleDriveSecurityDao>();
    }
}
