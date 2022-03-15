namespace ASC.Files.Thirdparty.Sharpbox;

[Scope(Additional = typeof(SharpBoxDaoSelectorExtension))]
internal class SharpBoxDaoSelector : RegexDaoSelectorBase<SharpBoxProviderInfo>, IDaoSelector
{
    protected internal override string Name => "SharpBox";
    protected internal override string Id => "sbox";

    public SharpBoxDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public IFileDao<string> GetFileDao(string id)
    {
        return base.GetFileDao<SharpBoxFileDao>(id);
    }

    public IFolderDao<string> GetFolderDao(string id)
    {
        return base.GetFolderDao<SharpBoxFolderDao>(id);
    }

    public ITagDao<string> GetTagDao(string id)
    {
        return base.GetTagDao<SharpBoxTagDao>(id);
    }

    public ISecurityDao<string> GetSecurityDao(string id)
    {
        return base.GetSecurityDao<SharpBoxSecurityDao>(id);
    }
}

public static class SharpBoxDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<SharpBoxFileDao>();
        services.TryAdd<SharpBoxFolderDao>();
        services.TryAdd<SharpBoxTagDao>();
        services.TryAdd<SharpBoxSecurityDao>();
    }
}
