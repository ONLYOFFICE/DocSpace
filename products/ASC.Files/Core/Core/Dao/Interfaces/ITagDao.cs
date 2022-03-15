namespace ASC.Files.Core;

[Scope]
public interface ITagDao<T>
{
    IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<T>> fileEntries);
    IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<T> parentFolder, bool deepSearch);
    IAsyncEnumerable<Tag> GetTagsAsync(T entryID, FileEntryType entryType, TagType tagType);
    IAsyncEnumerable<Tag> GetTagsAsync(TagType tagType, IEnumerable<FileEntry<T>> fileEntries);
    Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<T>> fileEntries);
    IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IEnumerable<FileEntry<T>> fileEntries);
    IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, FileEntry<T> fileEntry);
    IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType);
    IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType);
    IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType);
    IEnumerable<Tag> SaveTags(IEnumerable<Tag> tag);
    IEnumerable<Tag> SaveTags(Tag tag);
    void UpdateNewTags(IEnumerable<Tag> tag);
    void UpdateNewTags(Tag tag);
    void RemoveTags(IEnumerable<Tag> tag);
    void RemoveTags(Tag tag);
}
