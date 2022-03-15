namespace ASC.Files.Thirdparty.Box;

[Scope(Additional = typeof(BoxDaoSelectorExtension))]
internal class BoxDaoSelector : RegexDaoSelectorBase<BoxProviderInfo>, IDaoSelector
{
    protected internal override string Name => "Box";
    protected internal override string Id => "box";

    public BoxDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public IFileDao<string> GetFileDao(string id)
    {
        return base.GetFileDao<BoxFileDao>(id);
    }

    public IFolderDao<string> GetFolderDao(string id)
    {
        return base.GetFolderDao<BoxFolderDao>(id);
    }

    public ITagDao<string> GetTagDao(string id)
    {
        return base.GetTagDao<BoxTagDao>(id);
    }

    public ISecurityDao<string> GetSecurityDao(string id)
    {
        return base.GetSecurityDao<BoxSecurityDao>(id);
    }
}

public static class BoxDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BoxFileDao>();
        services.TryAdd<BoxFolderDao>();
        services.TryAdd<BoxTagDao>();
        services.TryAdd<BoxSecurityDao>();
    }
}
