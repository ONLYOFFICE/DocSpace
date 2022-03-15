namespace ASC.Files.Core;

[Scope(typeof(DaoFactory), Additional = typeof(DaoFactoryExtension))]
public interface IDaoFactory
{
    IProviderDao ProviderDao { get; }

    IFolderDao<T> GetFolderDao<T>();
    IFileDao<T> GetFileDao<T>();
    ITagDao<T> GetTagDao<T>();
    ISecurityDao<T> GetSecurityDao<T>();
    ILinkDao GetLinkDao();
}
