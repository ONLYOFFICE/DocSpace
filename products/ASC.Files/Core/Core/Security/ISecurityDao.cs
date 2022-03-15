namespace ASC.Files.Core.Security;

[Scope]
public interface ISecurityDao<T>
{
    Task SetShareAsync(FileShareRecord r);
    ValueTask<List<FileShareRecord>> GetSharesAsync(IEnumerable<Guid> subjects);
    Task<IEnumerable<FileShareRecord>> GetSharesAsync(IEnumerable<FileEntry<T>> entry);
    Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<T> entry);
    Task RemoveSubjectAsync(Guid subject);
    Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries);
    Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(FileEntry<T> entry);
    Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records);
    ValueTask<bool> IsSharedAsync(object entryId, FileEntryType type);
}
