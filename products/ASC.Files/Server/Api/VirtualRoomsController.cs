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

namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class VirtualRoomsInternalController : VirtualRoomsController<int>
{
    public VirtualRoomsInternalController(FoldersControllerHelper<int> foldersControllerHelper, FileStorageService<int> fileStorageService, FolderDtoHelper folderDtoHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<int> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomLinksService roomLinksManager, CustomTagsService<int> customTagsService) : base(foldersControllerHelper, fileStorageService, folderDtoHelper, globalFolderHelper, fileOperationDtoHelper, securityControllerHelper, coreBaseSettings, authContext, roomLinksManager, customTagsService)
    {

    }

    [Create("rooms")]
    public async Task<FolderDto<int>> CreateRoomAsync(CreateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.CreateRoomAsync(inDto.Title, inDto.RoomType);

        return await _folderDtoHelper.GetAsync(room);
    }
}

public class VirtualRoomsThirdpartyController : VirtualRoomsController<string>
{
    public VirtualRoomsThirdpartyController(FoldersControllerHelper<string> foldersControllerHelper, FileStorageService<string> fileStorageService, FolderDtoHelper folderDtoHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<string> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomLinksService roomLinksManager, CustomTagsService<string> customTagsService) : base(foldersControllerHelper, fileStorageService, folderDtoHelper, globalFolderHelper, fileOperationDtoHelper, securityControllerHelper, coreBaseSettings, authContext, roomLinksManager, customTagsService)
    {
    }

    [Create("rooms/thirdparty/{id}")]
    public async Task<FolderDto<string>> CreateRoomAsync(string id, CreateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.CreateThirdpartyRoomAsync(inDto.Title, inDto.RoomType, id);

        return await _folderDtoHelper.GetAsync(room);
    }
}

public abstract class VirtualRoomsController<T> : ApiControllerBase
{
    private readonly FoldersControllerHelper<T> _foldersControllerHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly SecurityControllerHelper<T> _securityControllerHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly AuthContext _authContext;
    private readonly RoomLinksService _roomLinksManager;
    private readonly CustomTagsService<T> _customTagsService;
    protected readonly FileStorageService<T> _fileStorageService;
    protected readonly FolderDtoHelper _folderDtoHelper;

    protected VirtualRoomsController(FoldersControllerHelper<T> foldersControllerHelper, FileStorageService<T> fileStorageService, FolderDtoHelper folderDtoHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<T> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomLinksService roomLinksManager, CustomTagsService<T> customTagsService)
    {
        _foldersControllerHelper = foldersControllerHelper;
        _fileStorageService = fileStorageService;
        _folderDtoHelper = folderDtoHelper;
        _globalFolderHelper = globalFolderHelper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _securityControllerHelper = securityControllerHelper;
        _coreBaseSettings = coreBaseSettings;
        _authContext = authContext;
        _roomLinksManager = roomLinksManager;
        _customTagsService = customTagsService;
    }

    [Read("rooms/{id}")]
    public async Task<FolderContentDto<T>> GetRoomAsync(T id, Guid userOrGroupId, FilterType filterType, bool searchInContent, bool withSubFolders)
    {
        ErrorIfNotDocSpace();

        return await _foldersControllerHelper.GetFolderAsync(id, userOrGroupId, filterType, searchInContent, withSubFolders);
    }

    [Update("rooms/{id}")]
    public async Task<FolderDto<T>> UpdateRoomAsync(T id, UpdateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.FolderRenameAsync(id, inDto.Title);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Delete("rooms/{id}")]
    public async Task<FileOperationDto> DeleteRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var operationResult = _fileStorageService.DeleteFolder("delete", id, false, true, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    [Update("rooms/{id}/archive")]
    public async Task<FileOperationDto> ArchiveRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderArchiveAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    [Update("rooms/{id}/unarchive")]
    public async Task<FileOperationDto> UnarchiveRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderVirtualRoomsAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    [Update("rooms/{id}/share")]
    public Task<IEnumerable<FileShareDto>> SetRoomSecurityAsync(T id, SecurityInfoRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return _securityControllerHelper.SetFolderSecurityInfoAsync(id, inDto.Share, inDto.Notify, inDto.SharingMessage);
    }

    [Read("rooms/{id}/invite")]
    public object GetInvitationLink(T id, InviteUserRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return _roomLinksManager.GenerateLink(id, inDto.Email, (int)inDto.Access, inDto.EmployeeType, _authContext.CurrentAccount.ID);
    }

    [Update("rooms/{id}/tags")]
    public async Task<FolderDto<T>> AddTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.AddRoomTagsAsync(id, inDto.TagIds);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Delete("rooms/{id}/tags")]
    public async Task<FolderDto<T>> DeleteTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.DeleteRoomTagsAsync(id, inDto.TagIds);

        return await _folderDtoHelper.GetAsync(room);
    }

    protected void ErrorIfNotDocSpace()
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            throw new NotSupportedException();
        }
    }
}

public class VirtualRoomsCommonController : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageService;
    private readonly FolderContentDtoHelper _folderContentDtoHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ApiContext _apiContext;
    private readonly CustomTagsService<int> _customTagsService;

    public VirtualRoomsCommonController(
        FileStorageService<int> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        GlobalFolderHelper globalFolderHelper,
        CoreBaseSettings coreBaseSettings,
        ApiContext apiContext,
        CustomTagsService<int> customTagsService)
    {
        _fileStorageService = fileStorageService;
        _folderContentDtoHelper = folderContentDtoHelper;
        _globalFolderHelper = globalFolderHelper;
        _coreBaseSettings = coreBaseSettings;
        _apiContext = apiContext;
        _customTagsService = customTagsService;
    }

    [Read("rooms")]
    public async Task<FolderContentDto<int>> GetRoomsFolderAsync(RoomType type, string subjectId, bool searchInContent, bool withSubfolders, SearchArea searchArea, string tags)
    {
        ErrorIfNotDocSpace();

        var parentId = await _globalFolderHelper.GetFolderVirtualRooms<int>();

        var filter = type switch
        {
            RoomType.FillingFormsRoom => FilterType.FillingFormsRoomsOnly,
            RoomType.ReadOnlyRoom => FilterType.ReadOnlyRoomsOnly,
            RoomType.EditingRoom => FilterType.EditingRoomsOnly,
            RoomType.ReviewRoom => FilterType.ReviewRoomsOnly,
            RoomType.CustomRoom => FilterType.CustomRoomsOnly,
            _ => FilterType.None
        };

        var tagIds = !string.IsNullOrEmpty(tags) ? JsonSerializer.Deserialize<IEnumerable<int>>(tags) : null;

        OrderBy orderBy = null;
        if (Enum.TryParse(_apiContext.SortBy, true, out SortedByType sortBy))
        {
            orderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
        }

        var startIndex = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);
        var filterValue = _apiContext.FilterValue;

        var content = await _fileStorageService.GetFolderItemsAsync(parentId, startIndex, count, filter, false, subjectId, filterValue, searchInContent, withSubfolders, orderBy, searchArea, tagIds);

        var dto = await _folderContentDtoHelper.GetAsync(content, startIndex);

        return dto.NotFoundIfNull();
    }

    [Create("tags")]
    public async Task<TagInfo> CreateTagAsync(CreateTagRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return await _customTagsService.CreateTagAsync(inDto.Name);
    }

    [Read("tags")]
    public async IAsyncEnumerable<TagInfo> GetTagsInfoAsync()
    {
        ErrorIfNotDocSpace();

        var from = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);

        await foreach (var tag in _customTagsService.GetTagsInfoAsync(_apiContext.FilterValue, TagType.Custom, from, count))
        {
            yield return tag;
        }
    }

    [Delete("tags")]
    public async Task DeleteTagsAsync(BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        await _customTagsService.DeleteTagsAsync(inDto.TagIds);
    }

    private void ErrorIfNotDocSpace()
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            throw new NotSupportedException();
        }
    }
}