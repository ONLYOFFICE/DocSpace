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
    public VirtualRoomsInternalController(
        GlobalFolderHelper globalFolderHelper,
        FileOperationDtoHelper fileOperationDtoHelper,
        CoreBaseSettings coreBaseSettings,
        CustomTagsService<int> customTagsService,
        RoomLogoManager roomLogoManager,
        FileStorageService<int> fileStorageService,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        FileShareDtoHelper fileShareDtoHelper,
        IMapper mapper,
        SocketManager socketManager) : base(
            globalFolderHelper,
            fileOperationDtoHelper,
            coreBaseSettings,
            customTagsService,
            roomLogoManager,
            fileStorageService,
            folderDtoHelper,
            fileDtoHelper,
            fileShareDtoHelper,
            mapper,
            socketManager)
    {
    }

    /// <summary>
    /// Creates a room in the "Virtual rooms" section.
    /// </summary>
    /// <short>Create a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for creating a room: <![CDATA[
    /// <ul>
    ///     <li><b>Title</b> (string) - room name,</li>
    ///     <li><b>RoomType</b> (RoomType) - room type: FillingFormsRoom (1), EditingRoom (2), ReviewRoom (3), ReadOnlyRoom (4), CustomRoom (5),</li>
    ///     <li><b>Private</b> (bool) - private room or not,</li>
    ///     <li><b>Share</b> (IEnumerable&lt;FileShareParams&gt;) - collection of sharing parameters:</li>
    ///     <ul>
    ///         <li><b>ShareTo</b> (Guid) - ID of the user with whom we want to share a room,</li>
    ///         <li><b>Email</b> (string) - user email address,</li>
    ///         <li><b>Access</b> (FileShare) - sharing rights (None, ReadWrite, Read, Restrict, Varies, Review, Comment, FillForms, CustomFilter, RoomAdmin, Editing).</li>
    ///     </ul>
    ///     <li><b>Notify</b> (bool) - notifies users about the shared room or not,</li>
    ///     <li><b>SharingMessage</b> (string) - message to send when notifying about the shared room.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("rooms")]
    public async Task<FolderDto<int>> CreateRoomAsync(CreateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.CreateRoomAsync(inDto.Title, inDto.RoomType, inDto.Private, inDto.Share, inDto.Notify, inDto.SharingMessage);

        return await _folderDtoHelper.GetAsync(room);
    }
}

public class VirtualRoomsThirdPartyController : VirtualRoomsController<string>
{
    public VirtualRoomsThirdPartyController(
        GlobalFolderHelper globalFolderHelper,
        FileOperationDtoHelper fileOperationDtoHelper,
        CoreBaseSettings coreBaseSettings,
        CustomTagsService<string> customTagsService,
        RoomLogoManager roomLogoManager,
        FileStorageService<string> fileStorageService,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        FileShareDtoHelper fileShareDtoHelper,
        IMapper mapper,
        SocketManager socketManager) : base(
            globalFolderHelper,
            fileOperationDtoHelper,
            coreBaseSettings,
            customTagsService,
            roomLogoManager,
            fileStorageService,
            folderDtoHelper,
            fileDtoHelper,
            fileShareDtoHelper,
            mapper,
            socketManager)
    {
    }

    /// <summary>
    /// Creates a room in the "Virtual rooms" section stored in a third-party storage.
    /// </summary>
    /// <short>Create a third-party room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.String, System" name="id">ID of the folder in the third-party storage in which the contents of the room will be stored</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for creating a room: <![CDATA[
    /// <ul>
    ///     <li><b>Title</b> (string) - room name,</li>
    ///     <li><b>RoomType</b> (RoomType) - room type: FillingFormsRoom (1), EditingRoom (2), ReviewRoom (3), ReadOnlyRoom (4), CustomRoom (5),</li>
    ///     <li><b>Private</b> (bool) - private room or not,</li>
    ///     <li><b>Share</b> (IEnumerable&lt;FileShareParams&gt;) - collection of sharing parameters:</li>
    ///     <ul>
    ///         <li><b>ShareTo</b> (Guid) - ID of the user with whom we want to share a room,</li>
    ///         <li><b>Email</b> (string) - user email address,</li>
    ///         <li><b>Access</b> (FileShare) - sharing rights (None, ReadWrite, Read, Restrict, Varies, Review, Comment, FillForms, CustomFilter, RoomAdmin, Editing).</li>
    ///     </ul>
    ///     <li><b>Notify</b> (bool) - notifies users about the shared room or not,</li>
    ///     <li><b>SharingMessage</b> (string) - message to send when notifying about the shared room.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/thirdparty/{id}</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("rooms/thirdparty/{id}")]
    public async Task<FolderDto<string>> CreateRoomAsync(string id, CreateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.CreateThirdPartyRoomAsync(inDto.Title, inDto.RoomType, id, inDto.Private, inDto.Share, inDto.Notify, inDto.SharingMessage);

        return await _folderDtoHelper.GetAsync(room);
    }
}

public abstract class VirtualRoomsController<T> : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CustomTagsService<T> _customTagsService;
    private readonly RoomLogoManager _roomLogoManager;
    protected readonly FileStorageService<T> _fileStorageService;
    private readonly FileShareDtoHelper _fileShareDtoHelper;
    private readonly IMapper _mapper;
    private readonly SocketManager _socketManager;

    protected VirtualRoomsController(
        GlobalFolderHelper globalFolderHelper,
        FileOperationDtoHelper fileOperationDtoHelper,
        CoreBaseSettings coreBaseSettings,
        CustomTagsService<T> customTagsService,
        RoomLogoManager roomLogoManager,
        FileStorageService<T> fileStorageService,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        FileShareDtoHelper fileShareDtoHelper,
        IMapper mapper,
        SocketManager socketManager) : base(folderDtoHelper, fileDtoHelper)
    {
        _globalFolderHelper = globalFolderHelper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _coreBaseSettings = coreBaseSettings;
        _customTagsService = customTagsService;
        _roomLogoManager = roomLogoManager;
        _fileStorageService = fileStorageService;
        _fileShareDtoHelper = fileShareDtoHelper;
        _mapper = mapper;
        _socketManager = socketManager;
    }

    /// <summary>
    /// Returns the virtual room information.
    /// </summary>
    /// <short>Get room information</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("rooms/{id}")]
    public async Task<FolderDto<T>> GetRoomInfoAsync(T id)
    {
        ErrorIfNotDocSpace();

        var folder = await _fileStorageService.GetFolderAsync(id).NotFoundIfNull("Folder not found");

        return await _folderDtoHelper.GetAsync(folder);
    }

    /// <summary>
    /// Renames a virtual room with the ID specified in  the request.
    /// </summary>
    /// <short>Rename a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UpdateRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for updating a virtual room: Title (string) - new room name</param>
    /// <returns>Updated room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}")]
    public async Task<FolderDto<T>> UpdateRoomAsync(T id, UpdateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.FolderRenameAsync(id, inDto.Title);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Removes a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Remove a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for deleting a virtual room: DeleteAfter (bool) - specifies whether to delete a room after the editing session is finished or not</param>
    /// <returns>File operation: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/rooms/{id}</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("rooms/{id}")]
    public async Task<FileOperationDto> DeleteRoomAsync(T id, DeleteRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var operationResult = _fileStorageService.DeleteFolder("delete", id, false, inDto.DeleteAfter, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Moves a virtual room with the ID specified in the request to the "Archive" section.
    /// </summary>
    /// <short>Archive a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ArchiveRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for archiving a virtual room: DeleteAfter (bool) - specifies whether to archive a room after the editing session is finished or not</param>
    /// <returns>File operation: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/rooms/{id}/archive</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/archive")]
    public async Task<FileOperationDto> ArchiveRoomAsync(T id, ArchiveRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderArchiveAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Moves a virtual room with the ID specified in the request from the "Archive" section to the "Virtual room" section.
    /// </summary>
    /// <short>Unarchive a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ArchiveRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for unarchiving a virtual room: DeleteAfter (bool) - specifies whether to unarchive a room after the editing session is finished or not</param>
    /// <returns>File operation: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/rooms/{id}/unarchive</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/unarchive")]
    public async Task<FileOperationDto> UnarchiveRoomAsync(T id, ArchiveRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderVirtualRoomsAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Sets the access rights to a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Set the room access rights</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.RoomInvitationRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for inviting users to a room: <![CDATA[
    /// <ul>
    ///     <li><b>Invitations</b> (IEnumerable&lt;RoomInvitation&gt;) - collection of invitation parameters:</li>
    ///     <ul>
    ///         <li><b>Id</b> (Guid) - ID of the user with whom we want to share a room,</li>
    ///         <li><b>Email</b> (string) - user email address,</li>
    ///         <li><b>Access</b> (FileShare) - sharing rights (None, ReadWrite, Read, Restrict, Varies, Review, Comment, FillForms, CustomFilter, RoomAdmin, Editing).</li>
    ///     </ul>
    ///     <li><b>Notify</b> (bool) - notifies users about the shared room or not,</li>
    ///     <li><b>Message</b> (string) - message to send when notifying about the shared room.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Room security information: room members, warning</returns>
    /// <path>api/2.0/files/rooms/{id}/share</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/share")]
    public async Task<RoomSecurityDto> SetRoomSecurityAsync(T id, RoomInvitationRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var result = new RoomSecurityDto();

        if (inDto.Invitations != null && inDto.Invitations.Any())
        {
            var wrappers = _mapper.Map<IEnumerable<RoomInvitation>, List<AceWrapper>>(inDto.Invitations);

            var aceCollection = new AceCollection<T>
            {
                Files = Array.Empty<T>(),
                Folders = new[] { id },
                Aces = wrappers,
                Message = inDto.Message
            };

            result.Warning = await _fileStorageService.SetAceObjectAsync(aceCollection, inDto.Notify);
        }

        result.Members = await GetRoomSecurityInfoAsync(id).ToListAsync();

        return result;
    }

    /// <summary>
    /// Returns the access rights of a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Get the room access rights</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <returns>Security information of room files: sharing rights, a user who has the access to the specified file, the file is locked by this user or not, this user is an owner of the specified file or not, this user can edit the access to the specified file or not</returns>
    /// <path>api/2.0/files/rooms/{id}/share</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("rooms/{id}/share")]
    public async IAsyncEnumerable<FileShareDto> GetRoomSecurityInfoAsync(T id)
    {
        var fileShares = await _fileStorageService.GetSharedInfoAsync(Array.Empty<T>(), new[] { id });

        foreach (var fileShareDto in fileShares)
        {
            yield return await _fileShareDtoHelper.Get(fileShareDto);
        }
    }

    /// <summary>
    /// Sets an external link to invite the users to a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Set an external invitation link</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.InvintationLinkRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Invitation link request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>LinkId</b> (Guid) - link ID,</li>
    ///     <li><b>Title</b> (string) - external link name,</li>
    ///     <li><b>Access</b> (FileShare) - sharing rights (None, ReadWrite, Read, Restrict, Varies, Review, Comment, FillForms, CustomFilter, RoomAdmin, Editing).</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Security information of room files: sharing rights, a user who has the access to the specified file, the file is locked by this user or not, this user is an owner of the specified file or not, this user can edit the access to the specified file or not</returns>
    /// <path>api/2.0/files/rooms/{id}/links</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/links")]
    public async IAsyncEnumerable<FileShareDto> SetInvintationLinkAsync(T id, InvintationLinkRequestDto inDto)
    {
        var fileShares = await _fileStorageService.SetInvitationLink(id, inDto.LinkId, inDto.Title, inDto.Access);

        foreach (var fileShareDto in fileShares)
        {
            yield return await _fileShareDtoHelper.Get(fileShareDto);
        }
    }

    /// <summary>
    /// Adds the tags to a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Add room tags</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchTagsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for adding tags: Names (IEnumerable&lt;string&gt;) - tag names</param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}/tags</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/tags")]
    public async Task<FolderDto<T>> AddTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.AddRoomTagsAsync(id, inDto.Names);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Removes the tags from a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Remove room tags</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchTagsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for removing tags: Names (IEnumerable&lt;string&gt;) - tag names</param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}/tags</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("rooms/{id}/tags")]
    public async Task<FolderDto<T>> DeleteTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.DeleteRoomTagsAsync(id, inDto.Names);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Creates a logo for a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Create a room logo</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.LogoRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Logo request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>TmpFile</b> (string) - the path to the temporary image file,</li>
    ///     <li><b>X</b> (integer) - the X coordinate of the rectangle starting point,</li>
    ///     <li><b>Y</b> (integer) - the Y coordinate of the rectangle starting point,</li>
    ///     <li><b>Width</b> (integer) - the rectangle width,</li>
    ///     <li><b>Height</b> (integer) - the rectangle height.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}/logo</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("rooms/{id}/logo")]
    public async Task<FolderDto<T>> CreateRoomLogoAsync(T id, LogoRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _roomLogoManager.CreateAsync(id, inDto.TmpFile, inDto.X, inDto.Y, inDto.Width, inDto.Height);

        await _socketManager.UpdateFolderAsync(room);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Removes a logo from a virtual room with the ID specified in the request.
    /// </summary>
    /// <short>Remove a room logo</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}/logo</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("rooms/{id}/logo")]
    public async Task<FolderDto<T>> DeleteRoomLogoAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _roomLogoManager.DeleteAsync(id);

        await _socketManager.UpdateFolderAsync(room);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Pins a virtual room with the ID specified in the request to the top of the list.
    /// </summary>
    /// <short>Pin a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}/pin</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/pin")]
    public async Task<FolderDto<T>> PinRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.SetPinnedStatusAsync(id, true);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Unpins a virtual room with the ID specified in the request from the top of the list.
    /// </summary>
    /// <short>Unpin a room</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <returns>Room information: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/rooms/{id}/unpin</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/unpin")]
    public async Task<FolderDto<T>> UnpinRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.SetPinnedStatusAsync(id, false);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Resends the email invitations to a virtual room with the ID specified in the request to the selected users.
    /// </summary>
    /// <short>Resend room invitations</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Int32, System" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UserInvintationRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">User invitation request parameters: UsersIds (IEnumerable&lt;Guid&gt;) - list of user IDs</param>
    /// <returns>Task awaiter</returns>
    /// <path>api/2.0/files/rooms/{id}/resend</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("rooms/{id}/resend")]
    public async Task ResendEmailInvitationsAsync(T id, UserInvintationRequestDto inDto)
    {
        await _fileStorageService.ResendEmailInvitationsAsync(id, inDto.UsersIds);
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
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly FolderContentDtoHelper _folderContentDtoHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ApiContext _apiContext;
    private readonly CustomTagsService<int> _customTagsService;
    private readonly RoomLogoManager _roomLogoManager;
    private readonly SetupInfo _setupInfo;
    private readonly FileSizeComment _fileSizeComment;
    private readonly RoomLinkService _roomLinkService;
    private readonly AuthContext _authContext;

    public VirtualRoomsCommonController(
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString,
        FolderContentDtoHelper folderContentDtoHelper,
        GlobalFolderHelper globalFolderHelper,
        CoreBaseSettings coreBaseSettings,
        ApiContext apiContext,
        CustomTagsService<int> customTagsService,
        RoomLogoManager roomLogoManager,
        SetupInfo setupInfo,
        FileSizeComment fileSizeComment,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        RoomLinkService roomLinkService,
        AuthContext authContext) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageServiceInt = fileStorageServiceInt;
        _fileStorageServiceString = fileStorageServiceString;
        _folderContentDtoHelper = folderContentDtoHelper;
        _globalFolderHelper = globalFolderHelper;
        _coreBaseSettings = coreBaseSettings;
        _apiContext = apiContext;
        _customTagsService = customTagsService;
        _roomLogoManager = roomLogoManager;
        _setupInfo = setupInfo;
        _fileSizeComment = fileSizeComment;
        _roomLinkService = roomLinkService;
        _authContext = authContext;
    }

    /// <summary>
    /// Returns the contents of the "Virtual rooms" section by the parameters specified in the request.
    /// </summary>
    /// <short>Get rooms</short>
    /// <category>Virtual rooms</category>
    /// <param type="System.Nullable{ASC.Files.Core.ApiModels.RequestDto.RoomFilterType}, System" name="type">Filter by room type (FillingFormsRoomOnly - 1, EditingRoomOnly - 2, ReviewRoomOnly - 3, ReadOnlyRoomOnly - 4, CustomRoomOnly - 5, FoldersOnly - 6)</param>
    /// <param type="System.String, System" name="subjectId">Filter by user ID</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withSubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{ASC.Files.Core.VirtualRooms.SearchArea}, System" name="searchArea">Room search area (Active, Archive, Any)</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withoutTags">Specifies whether to search by tags or not</param>
    /// <param type="System.String, System" name="tags">Tags in the serialized format</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="excludeSubject">Specifies whether to exclude a subject or not</param>
    /// <param type="System.Nullable{ASC.Files.Core.ProviderFilter}, System" name="provider">Filter by provider name (None, Box, DropBox, GoogleDrive, kDrive, OneDrive, SharePoint, WebDav, Yandex)</param>
    /// <param type="System.Nullable{ASC.Files.Core.Core.SubjectFilter}, System" name="subjectFilter">Filter by subject (Owner - 1, Member - 1)</param>
    /// <returns>Virtual rooms contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/rooms</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("rooms")]
    public async Task<FolderContentDto<int>> GetRoomsFolderAsync(RoomFilterType? type, string subjectId, bool? searchInContent, bool? withSubfolders, SearchArea? searchArea, bool? withoutTags, string tags, bool? excludeSubject,
        ProviderFilter? provider, SubjectFilter? subjectFilter)
    {
        ErrorIfNotDocSpace();

        var parentId = searchArea != SearchArea.Archive ? await _globalFolderHelper.GetFolderVirtualRooms<int>()
            : await _globalFolderHelper.GetFolderArchive<int>();

        var filter = type switch
        {
            RoomFilterType.FillingFormsRoomOnly => FilterType.FillingFormsRooms,
            RoomFilterType.ReadOnlyRoomOnly => FilterType.ReadOnlyRooms,
            RoomFilterType.EditingRoomOnly => FilterType.EditingRooms,
            RoomFilterType.ReviewRoomOnly => FilterType.ReviewRooms,
            RoomFilterType.CustomRoomOnly => FilterType.CustomRooms,
            RoomFilterType.FoldersOnly => FilterType.FoldersOnly,
            _ => FilterType.None
        };

        var tagNames = !string.IsNullOrEmpty(tags) ? JsonSerializer.Deserialize<IEnumerable<string>>(tags) : null;

        OrderBy orderBy = null;
        if (SortedByTypeExtensions.TryParse(_apiContext.SortBy, true, out var sortBy))
        {
            orderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
        }

        var startIndex = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);
        var filterValue = _apiContext.FilterValue;

        var content = await _fileStorageServiceInt.GetFolderItemsAsync(parentId, startIndex, count, filter, false, subjectId, filterValue,
            searchInContent ?? false, withSubfolders ?? false, orderBy, searchArea ?? SearchArea.Active, withoutTags ?? false, tagNames, excludeSubject ?? false,
            provider ?? ProviderFilter.None, subjectFilter ?? SubjectFilter.Owner);

        var dto = await _folderContentDtoHelper.GetAsync(content, startIndex);

        return dto.NotFoundIfNull();
    }

    /// <summary>
    /// Creates a custom tag with the parameters specified in the request.
    /// </summary>
    /// <short>Create a tag</short>
    /// <category>Virtual rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTagRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for creating a tag: Name (string) - tag name</param>
    /// <returns>New tag name</returns>
    /// <path>api/2.0/files/tags</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("tags")]
    public async Task<object> CreateTagAsync(CreateTagRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return await _customTagsService.CreateTagAsync(inDto.Name);
    }

    /// <summary>
    /// Returns a list of custom tags.
    /// </summary>
    /// <short>Get tags</short>
    /// <category>Virtual rooms</category>
    /// <returns>List of tag names</returns>
    /// <path>api/2.0/files/tags</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("tags")]
    public async IAsyncEnumerable<object> GetTagsInfoAsync()
    {
        ErrorIfNotDocSpace();

        var from = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);

        await foreach (var tag in _customTagsService.GetTagsInfoAsync(_apiContext.FilterValue, TagType.Custom, from, count))
        {
            yield return tag;
        }
    }

    /// <summary>
    /// Deletes a bunch of custom tags specified in the request.
    /// </summary>
    /// <short>Delete tags</short>
    /// <category>Virtual rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchTagsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Batch tags request parameters: Names (IEnumerable&lt;string&gt;) - list of tag names</param>
    /// <returns>Task awaiter</returns>
    /// <path>api/2.0/files/tags</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("tags")]
    public async Task DeleteTagsAsync(BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        await _customTagsService.DeleteTagsAsync(inDto.Names);
    }

    /// <summary>
    /// Uploads a temporary image to create a virtual room logo.
    /// </summary>
    /// <short>Upload an image for room logo</short>
    /// <category>Virtual rooms</category>
    /// <param type="Microsoft.AspNetCore.Http.IFormCollection, Microsoft.AspNetCore.Http" name="formCollection">Image data</param>
    /// <returns>Upload result: success or not, data, message</returns>
    /// <path>api/2.0/files/logos</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("logos")]
    public async Task<UploadResultDto> UploadRoomLogo(IFormCollection formCollection)
    {
        var result = new UploadResultDto();

        try
        {
            if (formCollection.Files.Count != 0)
            {
                var roomLogo = formCollection.Files[0];

                if (roomLogo.Length > _setupInfo.MaxImageUploadSize)
                {
                    result.Success = false;
                    result.Message = _fileSizeComment.FileImageSizeExceptionString;

                    return result;
                }

                var data = new byte[roomLogo.Length];
                using var inputStream = roomLogo.OpenReadStream();

                var br = new BinaryReader(inputStream);

                br.Read(data, 0, (int)roomLogo.Length);
                br.Close();

                UserPhotoThumbnailManager.CheckImgFormat(data);

                result.Data = await _roomLogoManager.SaveTempAsync(data, _setupInfo.MaxImageUploadSize);
                result.Success = true;
            }
            else
            {
                result.Success = false;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Accepts an invitation to a virtual room via an external link.
    /// </summary>
    /// <short>Accept an invitation</short>
    /// <category>Virtual rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.AcceptInvitationDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for accepting invitations: Key (string) - link key</param>
    /// <returns>Task awaiter</returns>
    /// <path>api/2.0/files/rooms/accept</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("rooms/accept")]
    public async Task SetSecurityByLink(AcceptInvitationDto inDto)
    {
        var options = await _roomLinkService.GetOptionsAsync(inDto.Key, null);

        if (!options.IsCorrect)
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
        }

        var aces = new List<AceWrapper>
        {
            new AceWrapper
            {
                Access = options.Share,
                Id = _authContext.CurrentAccount.ID
            }
        };

        var settings = new AceAdvancedSettingsWrapper
        {
            InvitationLink = true
        };

        if (int.TryParse(options.RoomId, out var id))
        {
            var aceCollection = new AceCollection<int>
            {
                Aces = aces,
                Files = Array.Empty<int>(),
                Folders = new[] { id },
                AdvancedSettings = settings
            };

            await _fileStorageServiceInt.SetAceObjectAsync(aceCollection, false);
        }
        else
        {
            var aceCollection = new AceCollection<string>
            {
                Aces = aces,
                Files = Array.Empty<string>(),
                Folders = new[] { options.RoomId },
                AdvancedSettings = settings
            };

            await _fileStorageServiceString.SetAceObjectAsync(aceCollection, false);
        }
    }

    private void ErrorIfNotDocSpace()
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            throw new NotSupportedException();
        }
    }
}