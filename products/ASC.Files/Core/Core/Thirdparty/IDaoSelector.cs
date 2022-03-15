namespace ASC.Files.Thirdparty;

internal interface IDaoSelector : IDisposable
{
    bool IsMatch(string id);
    IFileDao<string> GetFileDao(string id);
    IFolderDao<string> GetFolderDao(string id);
    ISecurityDao<string> GetSecurityDao(string id);
    ITagDao<string> GetTagDao(string id);
    string ConvertId(string id);
    string GetIdCode(string id);
}

internal interface IDaoSelector<T> where T : class, IProviderInfo
{
    bool IsMatch(string id);
    IFileDao<string> GetFileDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFileDao<string>;
    IFolderDao<string> GetFolderDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFolderDao<string>;
    ISecurityDao<string> GetSecurityDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ISecurityDao<string>;
    ITagDao<string> GetTagDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ITagDao<string>;
    string ConvertId(string id);
    string GetIdCode(string id);
}
