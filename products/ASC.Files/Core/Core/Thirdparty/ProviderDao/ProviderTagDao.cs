// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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
        return GetSelector(parentFolder.Id)
            .GetTagDao(parentFolder.Id)
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
