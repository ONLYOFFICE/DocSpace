namespace ASC.Files.Thirdparty.ProviderDao;

[Scope]
internal class ProviderTagDao : ProviderDaoBase, ITagDao<string>
{
    public ProviderTagDao(
        IServiceProvider serviceProvider,
        TenantManager tenantManager,
        SecurityDao<string> securityDao,
        TagDao<string> tagDao,
        CrossDao crossDao)
        : base(serviceProvider, tenantManager, securityDao, tagDao, crossDao)
    {
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
    {
        return TagDao.GetTagsAsync(subject, tagType, fileEntries);
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
    {
        return TagDao.GetTagsAsync(tagType, fileEntries);
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
    {
        return GetSelector(parentFolder.ID)
            .GetTagDao(parentFolder.ID)
            .GetNewTagsAsync(subject, parentFolder, deepSearch);
    }

    #region Only for Teamlab Documents

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
    {
        return TagDao.GetNewTagsAsync(subject, fileEntries);
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, FileEntry<string> fileEntry)
    {
        return TagDao.GetNewTagsAsync(subject, fileEntry);
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
    {
        return TagDao.GetTagsAsync(owner, tagType);
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
    {
        return TagDao.GetTagsAsync(name, tagType);
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
    {
        return TagDao.GetTagsAsync(names, tagType);
    }

    public IEnumerable<Tag> SaveTags(IEnumerable<Tag> tag)
    {
        return TagDao.SaveTags(tag);
    }

    public IEnumerable<Tag> SaveTags(Tag tag)
    {
        return TagDao.SaveTags(tag);
    }

    public void UpdateNewTags(IEnumerable<Tag> tag)
    {
        TagDao.UpdateNewTags(tag);
    }

    public void UpdateNewTags(Tag tag)
    {
        TagDao.UpdateNewTags(tag);
    }

    public void RemoveTags(IEnumerable<Tag> tag)
    {
        TagDao.RemoveTags(tag);
    }

    public void RemoveTags(Tag tag)
    {
        TagDao.RemoveTags(tag);
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string entryID, FileEntryType entryType, TagType tagType)
    {
        return TagDao.GetTagsAsync(entryID, entryType, tagType);
    }

    public Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<string>> fileEntries)
    {
        return TagDao.GetTagsAsync(subject, tagType, fileEntries);
    }

    #endregion
}
