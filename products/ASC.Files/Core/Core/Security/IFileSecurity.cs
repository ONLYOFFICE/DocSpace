namespace ASC.Files.Core.Security;

[Scope(typeof(FileSecurity))]
public interface IFileSecurity
{
    Task<bool> CanReadAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanCommentAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanReviewAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanCreateAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanEditAsync<T>(FileEntry<T> entry, Guid userId);
    Task<bool> CanDeleteAsync<T>(FileEntry<T> entry, Guid userId);
    Task<IEnumerable<Guid>> WhoCanReadAsync<T>(FileEntry<T> entry);
}
