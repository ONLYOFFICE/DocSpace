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
    IAsyncEnumerable<TagInfo> GetTagsInfoAsync(string searchText, TagType tagType, bool byName, int from = 0, int count = 0);
    IAsyncEnumerable<TagInfo> GetTagsInfoAsync(IEnumerable<string> names);
    Task<IEnumerable<Tag>> SaveTags(IEnumerable<Tag> tag, Guid createdBy = default);
    Task<IEnumerable<Tag>> SaveTagsAsync(Tag tag);
    Task<TagInfo> SaveTagInfoAsync(TagInfo tagInfo);
    Task UpdateNewTags(IEnumerable<Tag> tag, Guid createdBy = default);
    Task UpdateNewTags(Tag tag);
    Task RemoveTagsAsync(IEnumerable<int> tagsIds);
    Task RemoveTagsAsync(FileEntry<T> entry, IEnumerable<int> tagsIds);
    Task RemoveTags(IEnumerable<Tag> tag);
    Task RemoveTags(Tag tag);
    Task<int> RemoveTagLinksAsync(T entryId, FileEntryType entryType, TagType tagType);
}
