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

namespace ASC.Files.Core.VirtualRooms;

[Scope]
public class CustomTagsService<T>
{
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly AuthContext _authContext;
    private readonly FileSecurityCommon _fileSecurityCommon;

    public CustomTagsService(IDaoFactory daoFactory, FileSecurity fileSecurity, AuthContext authContext, FileSecurityCommon fileSecurityCommon)
    {
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _authContext = authContext;
        _fileSecurityCommon = fileSecurityCommon;
    }

    private ITagDao<T> TagDao => _daoFactory.GetTagDao<T>();
    private IFolderDao<T> FolderDao => _daoFactory.GetFolderDao<T>();

    public async Task<TagInfo> CreateTagAsync(string name)
    {
        if (!_fileSecurityCommon.IsAdministrator(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException("You do not have permission to create tags");
        }

        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(name);

        var tags = await TagDao.GetTagsInfoAsync(name, TagType.Custom, true).ToListAsync();

        if (tags.Any())
        {
            throw new Exception("The tag already exists");
        }

        var tagInfo = new TagInfo
        {
            Name = name,
            Owner = _authContext.CurrentAccount.ID,
            Type = TagType.Custom
        };

        return await TagDao.SaveTagInfoAsync(tagInfo);
    }

    public async Task DeleteTagsAsync(IEnumerable<int> tagIds)
    {
        if (!_fileSecurityCommon.IsAdministrator(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException("You do not have permission to remove tags");
        }

        var tagDao = TagDao;

        await tagDao.RemoveTagsAsync(tagIds);
    }

    public async Task<Folder<T>> AddRoomTagsAsync(T folderId, IEnumerable<int> tagsIds)
    {
        var folder = await FolderDao.GetFolderAsync(folderId);

        if (!await _fileSecurity.CanEditRoomAsync(folder))
        {
            throw new SecurityException("You do not have permission to edit the room");
        }

        var tagDao = TagDao;

        var tagsInfos = await tagDao.GetTagsInfoAsync(tagsIds).ToListAsync();

        var tags = tagsInfos.Select(tagInfo => Tag.Custom(_authContext.CurrentAccount.ID, folder, tagInfo.Name));

        tagDao.SaveTags(tags);

        return folder;
    }

    public async Task<Folder<T>> DeleteRoomTagsAsync(T folderId, IEnumerable<int> tagsIds)
    {
        var folder = await FolderDao.GetFolderAsync(folderId);

        if (!await _fileSecurity.CanEditRoomAsync(folder))
        {
            throw new SecurityException("You do not have permission to edit the room");
        }

        var tagDao = TagDao;

        await tagDao.RemoveTagsAsync(folder, tagsIds);

        var updatedFolder = await FolderDao.GetFolderAsync(folderId);

        return updatedFolder;
    }

    public async IAsyncEnumerable<TagInfo> GetTagsInfoAsync(string searchText, TagType tagType)
    {
        await foreach (var tagInfo in TagDao.GetTagsInfoAsync(searchText, tagType, false))
        {
            yield return tagInfo;
        }
    }
}