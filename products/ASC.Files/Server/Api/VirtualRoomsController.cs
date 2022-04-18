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
    public VirtualRoomsControllerInternal(FoldersControllerHelper<int> foldersControllerHelper, GlobalFolderHelper globalFolderHelper, FileStorageService<int> fileStorageService, FolderDtoHelper folderDtoHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<int> securityControllerHelper) : base(foldersControllerHelper, globalFolderHelper, fileStorageService, folderDtoHelper, fileOperationDtoHelper, securityControllerHelper)
    {
    }
}

public abstract class VirtualRoomsController<T> : ApiControllerBase
{
    private readonly FoldersControllerHelper<T> _foldersControllerHelper;
    private readonly FileStorageService<T> _fileStorageService;
    private readonly FolderDtoHelper _folderDtoHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly SecurityControllerHelper<T> _securityControllerHelper;

    public VirtualRoomsController(FoldersControllerHelper<T> foldersControllerHelper, GlobalFolderHelper globalFolderHelper,
        FileStorageService<T> fileStorageService, FolderDtoHelper folderDtoHelper, FileOperationDtoHelper fileOperationDtoHelper,
        SecurityControllerHelper<T> securityControllerHelper)
    {
        _foldersControllerHelper = foldersControllerHelper;
        _globalFolderHelper = globalFolderHelper;
        _fileStorageService = fileStorageService;
        _folderDtoHelper = folderDtoHelper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _securityControllerHelper = securityControllerHelper;
    }

    [Read("@rooms")]
    public async Task<FolderContentDto<T>> GetRoomsFolderAsync()
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.GetFolderVirtualRooms<T>(), Guid.Empty, FilterType.None, true);
    }

    [Read("@archive")]
    public async Task<FolderContentDto<T>> GetArchiveFolderAsync()
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.GetFolderArchive<T>(), Guid.Empty, FilterType.None, true);
    }

    [Create("room")]
    public async Task<FolderDto<T>> CreateRoomFromBodyAsync([FromBody] CreateRoomRequestDto inDto)
    {
        var room = await _fileStorageService.CreateRoom(inDto.Title, inDto.RoomType);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Create("room")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FolderDto<T>> CreateRoomFromFormAsync([FromForm] CreateRoomRequestDto inDto)
    {
        var room = await _fileStorageService.CreateRoom(inDto.Title, inDto.RoomType);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Update("room/{roomId}")]
    public async Task<FolderDto<T>> RenameRoomFromBodyAsync(T roomId, [FromBody] RenameRoomRequestDto inDto)
    {
        var room = await _fileStorageService.FolderRenameAsync(roomId, inDto.Title);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Update("room/{roomId}")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FolderDto<T>> RenameRoomFromFormAsync(T roomId, [FromForm] RenameRoomRequestDto inDto)
    {
        var room = await _fileStorageService.FolderRenameAsync(roomId, inDto.Title);

        return await _folderDtoHelper.GetAsync(room);
    }

    [Delete("room")]
    public async IAsyncEnumerable<FileOperationDto> DeleteRoomsFromBodyAsync([FromBody] BatchRoomsRequestDto inDto)
    {
        var tasks = _fileStorageService.DeleteItems("delete", inDto.FileIds.ToList(), inDto.FolderIds.ToList(), false, inDto.DeleteAfter, true);

        foreach (var e in tasks)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    [Delete("room")]
    [Consumes("application/x-www-form-urlencoded")]
    public async IAsyncEnumerable<FileOperationDto> DeleteRoomsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRoomsRequestDto inDto)
    {
        var tasks = _fileStorageService.DeleteItems("delete", inDto.FileIds.ToList(), inDto.FolderIds.ToList(), false, inDto.DeleteAfter, true);

        foreach (var e in tasks)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    [Update("room/archive")]
    public async Task<IEnumerable<FileOperationDto>> ArchiveRoomsFromBodyAsync([FromBody] BatchRoomsRequestDto inDto)
    {
        return await ArchiveRoomsAsync(inDto);
    }

    [Update("room/archive")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<FileOperationDto>> ArchiveRoomsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRoomsRequestDto inDto)
    {
        return await ArchiveRoomsAsync(inDto);
    }

    [Update("room/unarchive")]
    public async Task<IEnumerable<FileOperationDto>> UnarchiveRoomsFromBodyAsync([FromBody] BatchRoomsRequestDto inDto)
    {
        return await UnarchiveRoomsAsync(inDto);
    }

    [Update("room/unarchive")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<FileOperationDto>> UnarchiveRoomsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRoomsRequestDto inDto)
    {
        return await UnarchiveRoomsAsync(inDto);
    }

    [Update("room/{roomId}/share")]
    public Task<IEnumerable<FileShareDto>> SetRoomSecurityFromBodyAsync(T roomId, [FromBody] SecurityInfoRequestDto inDto)
    {
        return _securityControllerHelper.SetFolderSecurityInfoAsync(roomId, inDto.Share, inDto.Notify, inDto.SharingMessage);
    }

    [Update("room/{roomId}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareDto>> SetRoomSecurityFromFormAsync(T roomId, [FromForm] SecurityInfoRequestDto inDto)
    {
        return _securityControllerHelper.SetFolderSecurityInfoAsync(roomId, inDto.Share, inDto.Notify, inDto.SharingMessage);
    }

    private async Task<IEnumerable<FileOperationDto>> ArchiveRoomsAsync(BatchRoomsRequestDto inDto)
    {
        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderArchiveAsync);

        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    private async Task<IEnumerable<FileOperationDto>> UnarchiveRoomsAsync(BatchRoomsRequestDto inDto)
    {
        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderVirtualRoomsAsync);

        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }
}