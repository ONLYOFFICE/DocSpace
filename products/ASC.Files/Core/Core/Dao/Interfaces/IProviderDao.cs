namespace ASC.Files.Core;

[Scope(typeof(ProviderAccountDao), Additional = typeof(ProviderAccountDaoExtension))]
public interface IProviderDao
{
    Task<IProviderInfo> GetProviderInfoAsync(int linkId);
    IAsyncEnumerable<IProviderInfo> GetProvidersInfoAsync();
    IAsyncEnumerable<IProviderInfo> GetProvidersInfoAsync(FolderType folderType, string searchText = null);
    IAsyncEnumerable<IProviderInfo> GetProvidersInfoAsync(Guid userId);
    Task<int> SaveProviderInfoAsync(string providerKey, string customerTitle, AuthData authData, FolderType folderType);
    Task<int> UpdateProviderInfoAsync(int linkId, string customerTitle, AuthData authData, FolderType folderType, Guid? userId = null);
    Task RemoveProviderInfoAsync(int linkId);
}
