namespace ASC.Files.Core;

public interface IProviderInfo : IDisposable
{
    int ID { get; set; }
    string ProviderKey { get; }
    Guid Owner { get; }
    FolderType RootFolderType { get; }
    DateTime CreateOn { get; }
    string CustomerTitle { get; }
    string RootFolderId { get; }

    Task<bool> CheckAccessAsync();
    Task InvalidateStorageAsync();
    void UpdateTitle(string newtitle);
}

public class ProviderInfoArgumentException : ArgumentException
{
    public ProviderInfoArgumentException(string message) : base(message) { }
}
