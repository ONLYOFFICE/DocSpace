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
public class VirtualRoomsControllerInternal : VirtualRoomsController<int>
{
    public VirtualRoomsControllerInternal(FoldersControllerHelper<int> foldersControllerHelper, GlobalFolderHelper globalFolderHelper, FileStorageService<int> fileStorageService, FolderDtoHelper folderDtoHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<int> securityControllerHelper, CoreBaseSettings coreBaseSettings, FolderContentDtoHelper folderContentDtoHelper, ApiContext apiContext) : base(foldersControllerHelper, globalFolderHelper, fileStorageService, folderDtoHelper, fileOperationDtoHelper, securityControllerHelper, coreBaseSettings, folderContentDtoHelper, apiContext)
    {
    }
}

public abstract class VirtualRoomsController<T> : ApiControllerBase
{
    private readonly FoldersControllerHelper<T> _foldersControllerHelper;
    private readonly FileStorageService<T> _fileStorageService;
    private readonly FolderDtoHelper _folderDtoHelper;
    private readonly FolderContentDtoHelper _folderContentDtoHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly SecurityControllerHelper<T> _securityControllerHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ApiContext _apiContext;

    public VirtualRoomsController(FoldersControllerHelper<T> foldersControllerHelper, GlobalFolderHelper globalFolderHelper,
        FileStorageService<T> fileStorageService, FolderDtoHelper folderDtoHelper, FileOperationDtoHelper fileOperationDtoHelper,
        SecurityControllerHelper<T> securityControllerHelper, CoreBaseSettings coreBaseSettings,
        FolderContentDtoHelper folderContentDtoHelper, ApiContext apiContext)
    {
        _foldersControllerHelper = foldersControllerHelper;
        _globalFolderHelper = globalFolderHelper;
        _fileStorageService = fileStorageService;
        _folderDtoHelper = folderDtoHelper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _securityControllerHelper = securityControllerHelper;
        _coreBaseSettings = coreBaseSettings;
        _folderContentDtoHelper = folderContentDtoHelper;
        _apiContext = apiContext;
    }

    [Read("rooms")]
    public async Task<FolderContentDto<T>> GetRoomsFolderAsync(RoomType type, string subjectId, bool searchInContent, bool withSubfolders, SearchArea searchArea)
    {
        ErrorIfNotDocSpace();

        var parentId = await _globalFolderHelper.GetFolderVirtualRooms<T>();

        var filter = type switch
        {
            RoomType.FillingFormsRoom => FilterType.FillingFormsRoomsOnly,
            RoomType.ReadOnlyRoom => FilterType.ReadOnlyRoomsOnly,
            RoomType.EditingRoom => FilterType.EditingRoomsOnly,
            RoomType.ReviewRoom => FilterType.ReviewRoomsOnly,
            RoomType.CustomRoom => FilterType.CustomRoomsOnly,
            _ => FilterType.None
        };

        OrderBy orderBy = null;
        if (Enum.TryParse(_apiContext.SortBy, true, out SortedByType sortBy))
        {
            orderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
        }

        var startIndex = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);
        var filterValue = _apiContext.FilterValue;

        var content = await _fileStorageService.GetFolderItemsAsync(parentId, startIndex, count, filter, false, subjectId, filterValue, searchInContent, withSubfolders, orderBy, searchArea);

        var dto = await _folderContentDtoHelper.GetAsync(content, startIndex);

        return dto.NotFoundIfNull();
    }

    [Create("rooms")]
    public async Task<FolderDto<T>> CreateRoomAsync([FromBody] CreateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.CreateRoom(inDto.Title, inDto.RoomType);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Read("rooms/{id}")]
    public async Task<FolderContentDto<T>> GetRoomAsync(T id, Guid userOrGroupId, FilterType filterType, bool withSubFolders)
    {
        ErrorIfNotDocSpace();

        return await _foldersControllerHelper.GetFolderAsync(id, userOrGroupId, filterType, withSubFolders);
    }

    [Update("rooms/{id}")]
    public async Task<FolderDto<T>> UpdateRoomAsync(T id, [FromBody] UpdateRoomRequestDto inDto)
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
    public Task<IEnumerable<FileShareDto>> SetRoomSecurityAsync(T id, [FromBody] SecurityInfoRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return _securityControllerHelper.SetFolderSecurityInfoAsync(id, inDto.Share, inDto.Notify, inDto.SharingMessage);
    }

    private void ErrorIfNotDocSpace()
    {
        if (!_coreBaseSettings.DocSpace)
        {
            throw new NotSupportedException();
        }
    }
}