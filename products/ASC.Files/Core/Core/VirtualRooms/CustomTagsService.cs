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
public class CustomTagsService
{
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly AuthContext _authContext;
    private readonly FilesMessageService _filesMessageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager _userManager;

    public CustomTagsService(
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        AuthContext authContext,
        FilesMessageService filesMessageService,
        IHttpContextAccessor httpContextAccessor,
        UserManager userManager)
    {
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _authContext = authContext;
        _filesMessageService = filesMessageService;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    private IDictionary<string, StringValues> Headers => _httpContextAccessor?.HttpContext?.Request?.Headers;

    public async Task<string> CreateTagAsync(string name)
    {
        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(name);

        var tagDao = _daoFactory.GetTagDao<int>();
        var tags = await tagDao.GetTagsInfoAsync(name, TagType.Custom, true).ToListAsync();

        if (tags.Any())
        {
            throw new InvalidOperationException();
        }

        var tagInfo = new TagInfo
        {
            Name = name,
            Owner = Guid.Empty,
            Type = TagType.Custom
        };

        var savedTag = await tagDao.SaveTagInfoAsync(tagInfo);

        await _filesMessageService.SendAsync(Headers, MessageAction.TagCreated, savedTag.Name);

        return savedTag.Name;
    }

    public async Task DeleteTagsAsync<T>(IEnumerable<string> names)
    {
        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        if (!names.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tags = await tagDao.GetTagsInfoAsync(names).ToListAsync();

        await tagDao.RemoveTagsAsync(tags.Select(t => t.Id));

        await _filesMessageService.SendAsync(Headers, MessageAction.TagsDeleted, string.Join(',', tags.Select(t => t.Name).ToArray()));
    }

    public async Task<Folder<T>> AddRoomTagsAsync<T>(T folderId, IEnumerable<string> names)
    {
        var folder = await _daoFactory.GetFolderDao<T>().GetFolderAsync(folderId);

        if (folder.RootFolderType == FolderType.Archive || !await _fileSecurity.CanEditRoomAsync(folder))
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_SecurityException_EditRoom);
        }

        if (!names.Any())
        {
            return folder;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tagsInfos = await tagDao.GetTagsInfoAsync(names).ToListAsync();

        var tags = tagsInfos.Select(tagInfo => Tag.Custom(Guid.Empty, folder, tagInfo.Name));

        await tagDao.SaveTags(tags);

        _ = _filesMessageService.SendAsync(folder, Headers, MessageAction.AddedRoomTags, folder.Title, string.Join(',', tagsInfos.Select(t => t.Name)));

        return folder;
    }

    public async Task<Folder<T>> DeleteRoomTagsAsync<T>(T folderId, IEnumerable<string> names)
    {
        var folder = await _daoFactory.GetFolderDao<T>().GetFolderAsync(folderId);

        if (folder.RootFolderType == FolderType.Archive || !await _fileSecurity.CanEditRoomAsync(folder))
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_SecurityException_EditRoom);
        }

        if (!names.Any())
        {
            return folder;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tagsInfos = await tagDao.GetTagsInfoAsync(names).ToListAsync();

        await tagDao.RemoveTagsAsync(folder, tagsInfos.Select(t => t.Id));

        _ = _filesMessageService.SendAsync(folder, Headers, MessageAction.DeletedRoomTags, folder.Title, string.Join(',', tagsInfos.Select(t => t.Name)));

        return folder;
    }

    public async IAsyncEnumerable<object> GetTagsInfoAsync<T>(string searchText, TagType tagType, int from, int count)
    {
        await foreach (var tagInfo in _daoFactory.GetTagDao<T>().GetTagsInfoAsync(searchText, tagType, false, from, count))
        {
            yield return tagInfo.Name;
        }
    }
}