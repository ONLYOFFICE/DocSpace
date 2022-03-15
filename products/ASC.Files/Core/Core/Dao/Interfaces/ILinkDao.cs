namespace ASC.Files.Core;

[Scope]
public interface ILinkDao
{
    Task AddLinkAsync(string sourceId, string linkedId);
    Task<string> GetSourceAsync(string linkedId);
    Task<string> GetLinkedAsync(string sourceId);
    Task DeleteLinkAsync(string sourceId);
    Task DeleteAllLinkAsync(string sourceId);
}
