namespace ASC.Files.Thirdparty.SharePoint;

[Scope(Additional = typeof(SharePointDaoSelectorExtension))]
internal class SharePointDaoSelector : RegexDaoSelectorBase<SharePointProviderInfo>, IDaoSelector
{
    protected internal override string Name => "sharepoint";
    protected internal override string Id => "spoint";

    public SharePointDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public IFileDao<string> GetFileDao(string id)
    {
        return base.GetFileDao<SharePointFileDao>(id);
    }

    public IFolderDao<string> GetFolderDao(string id)
    {
        return base.GetFolderDao<SharePointFolderDao>(id);
    }

    public ITagDao<string> GetTagDao(string id)
    {
        return base.GetTagDao<SharePointTagDao>(id);
    }

    public ISecurityDao<string> GetSecurityDao(string id)
    {
        return base.GetSecurityDao<SharePointSecurityDao>(id);
    }

    public override string ConvertId(string id)
    {
        if (id != null)
        {
            var match = Selector.Match(id);
            if (match.Success)
            {
                return GetInfo(id).ProviderInfo.SpRootFolderId + match.Groups["path"].Value.Replace('|', '/');
            }

            throw new ArgumentException("Id is not a sharepoint id");
        }

        return base.ConvertId(null);
    }
}

public static class SharePointDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<SharePointFileDao>();
        services.TryAdd<SharePointFolderDao>();
        services.TryAdd<SharePointTagDao>();
        services.TryAdd<SharePointSecurityDao>();
    }
}
