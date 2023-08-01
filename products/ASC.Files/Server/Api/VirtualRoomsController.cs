﻿// (c) Copyright Ascensio System SIA 2010-2022
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
        CustomTagsService customTagsService,
        RoomLogoManager roomLogoManager,
        FileStorageService fileStorageService,
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
    /// Creates a room in the "Rooms" section.
    /// </summary>
    /// <short>Create a room</short>
    /// <category>Rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateRoomRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
        CustomTagsService customTagsService,
        RoomLogoManager roomLogoManager,
        FileStorageService fileStorageService,
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
    /// Creates a room in the "Rooms" section stored in a third-party storage.
    /// </summary>
    /// <short>Create a third-party room</short>
    /// <category>Rooms</category>
    /// <param type="System.String, System" method="url" name="id">ID of the folder in the third-party storage in which the contents of the room will be stored</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateRoomRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    private readonly CustomTagsService _customTagsService;
    private readonly RoomLogoManager _roomLogoManager;
    protected readonly FileStorageService _fileStorageService;
    private readonly FileShareDtoHelper _fileShareDtoHelper;
    private readonly IMapper _mapper;
    private readonly SocketManager _socketManager;

    protected VirtualRoomsController(
        GlobalFolderHelper globalFolderHelper,
        FileOperationDtoHelper fileOperationDtoHelper,
        CoreBaseSettings coreBaseSettings,
        CustomTagsService customTagsService,
        RoomLogoManager roomLogoManager,
        FileStorageService fileStorageService,
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
    /// Returns the room information.
    /// </summary>
    /// <short>Get room information</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
    /// <path>api/2.0/files/rooms/{id}</path>
    /// <httpMethod>GET</httpMethod>
    [AllowAnonymous]
    [HttpGet("rooms/{id}")]
    public async Task<FolderDto<T>> GetRoomInfoAsync(T id)
    {
        ErrorIfNotDocSpace();

        var folder = await _fileStorageService.GetFolderAsync(id).NotFoundIfNull("Folder not found");

        return await _folderDtoHelper.GetAsync(folder);
    }

    /// <summary>
    /// Renames a room with the ID specified in  the request.
    /// </summary>
    /// <short>Rename a room</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UpdateRoomRequestDto, ASC.Files.Core" name="inDto">Request parameters for updating a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Updated room information</returns>
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
    /// Removes a room with the ID specified in the request.
    /// </summary>
    /// <short>Remove a room</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteRoomRequestDto, ASC.Files.Core" name="inDto">Request parameters for deleting a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">File operation</returns>
    /// <path>api/2.0/files/rooms/{id}</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("rooms/{id}")]
    public async Task<FileOperationDto> DeleteRoomAsync(T id, DeleteRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var operationResult = (await _fileStorageService.DeleteFolderAsync("delete", id, false, inDto.DeleteAfter, true))
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Moves a room with the ID specified in the request to the "Archive" section.
    /// </summary>
    /// <short>Archive a room</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ArchiveRoomRequestDto, ASC.Files.Core" name="inDto">Request parameters for archiving a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">File operation</returns>
    /// <path>api/2.0/files/rooms/{id}/archive</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/archive")]
    public async Task<FileOperationDto> ArchiveRoomAsync(T id, ArchiveRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderArchiveAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = (await _fileStorageService.MoveOrCopyItemsAsync(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter))
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Moves a room with the ID specified in the request from the "Archive" section to the "Rooms" section.
    /// </summary>
    /// <short>Unarchive a room</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ArchiveRoomRequestDto, ASC.Files.Core" name="inDto">Request parameters for unarchiving a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">File operation</returns>
    /// <path>api/2.0/files/rooms/{id}/unarchive</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("rooms/{id}/unarchive")]
    public async Task<FileOperationDto> UnarchiveRoomAsync(T id, ArchiveRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderVirtualRoomsAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = (await _fileStorageService.MoveOrCopyItemsAsync(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter))
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Sets the access rights to a room with the ID specified in the request.
    /// </summary>
    /// <short>Set room access rights</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.RoomInvitationRequestDto, ASC.Files.Core" name="inDto">Request parameters for inviting users to a room</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.RoomSecurityDto, ASC.Files.Core">Room security information</returns>
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
    /// Returns the access rights of a room with the ID specified in the request.
    /// </summary>
    /// <short>Get room access rights</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">Security information of room files</returns>
    /// <path>api/2.0/files/rooms/{id}/share</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
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
    /// Sets an external link to invite the users to a room with the ID specified in the request.
    /// </summary>
    /// <short>Set an external invitation link</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.LinkRequestDto, ASC.Files.Core" name="inDto">Invitation link request parameters</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">Security information of room files</returns>
    /// <path>api/2.0/files/rooms/{id}/links</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("rooms/{id}/links")]
    public async IAsyncEnumerable<FileShareDto> SetLinkAsync(T id, LinkRequestDto inDto)
    {
        var fileShares = inDto.LinkType switch
        {
            LinkType.Invitation => await _fileStorageService.SetInvitationLinkAsync(id, inDto.LinkId, inDto.Title, inDto.Access),
            LinkType.External => await _fileStorageService.SetExternalLinkAsync(id, FileEntryType.Folder, inDto.LinkId, inDto.Title, 
                inDto.Access is not (FileShare.Read or FileShare.None) ? FileShare.Read : inDto.Access , inDto.ExpirationDate ?? default, inDto.Password, inDto.Disabled, inDto.DenyDownload),
            _ => throw new InvalidOperationException()
        };

        foreach (var fileShareDto in fileShares)
        {
            yield return await _fileShareDtoHelper.Get(fileShareDto);
        }
    }

    /// <summary>
    /// Getting room links
    /// </summary>
    /// <short>Set an external invitation link</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.ResponseDto.LinkType, ASC.Files.Core" name="type">Link type</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">Room security info</returns>
    /// <path>api/2.0/files/rooms/{id}/links</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("rooms/{id}/links")]
    public async IAsyncEnumerable<FileShareDto> GetLinksAsync(T id, LinkType? type)
    {
        var subjectTypes = type.HasValue
            ? type.Value switch
            {
                LinkType.Invitation => new[] { SubjectType.InvitationLink },
                LinkType.External => new[] { SubjectType.ExternalLink },
                _ => new[] { SubjectType.InvitationLink, SubjectType.ExternalLink }
            }
            : new[] { SubjectType.InvitationLink, SubjectType.ExternalLink };

        var fileShares = await _fileStorageService.GetSharedInfoAsync(Array.Empty<T>(), new[] { id }, subjectTypes, true);

        foreach (var fileShareDto in fileShares)
        {
            yield return await _fileShareDtoHelper.Get(fileShareDto);
        }
    }

    /// <summary>
    /// Adds the tags to a room with the ID specified in the request.
    /// </summary>
    /// <short>Add room tags</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchTagsRequestDto, ASC.Files.Core" name="inDto">Request parameters for adding tags</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    /// Removes the tags from a room with the ID specified in the request.
    /// </summary>
    /// <short>Remove room tags</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchTagsRequestDto, ASC.Files.Core" name="inDto">Request parameters for removing tags</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    /// Creates a logo for a room with the ID specified in the request.
    /// </summary>
    /// <short>Create a room logo</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.LogoRequestDto, ASC.Files.Core" name="inDto">Logo request parameters</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    /// Removes a logo from a room with the ID specified in the request.
    /// </summary>
    /// <short>Remove a room logo</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    /// Pins a room with the ID specified in the request to the top of the list.
    /// </summary>
    /// <short>Pin a room</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    /// Unpins a room with the ID specified in the request from the top of the list.
    /// </summary>
    /// <short>Unpin a room</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Room information</returns>
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
    /// Resends the email invitations to a room with the ID specified in the request to the selected users.
    /// </summary>
    /// <short>Resend room invitations</short>
    /// <category>Rooms</category>
    /// <param type="System.Int32, System" method="url" name="id">Room ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UserInvintationRequestDto, ASC.Files.Core" name="inDto">User invitation request parameters</param>
    /// <returns></returns>
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
    private readonly FileStorageService _fileStorageService;
    private readonly FolderContentDtoHelper _folderContentDtoHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ApiContext _apiContext;
    private readonly CustomTagsService _customTagsService;
    private readonly RoomLogoManager _roomLogoManager;
    private readonly SetupInfo _setupInfo;
    private readonly FileSizeComment _fileSizeComment;
    private readonly InvitationLinkService _invitationLinkService;
    private readonly AuthContext _authContext;

    public VirtualRoomsCommonController(
        FileStorageService fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        GlobalFolderHelper globalFolderHelper,
        CoreBaseSettings coreBaseSettings,
        ApiContext apiContext,
        CustomTagsService customTagsService,
        RoomLogoManager roomLogoManager,
        SetupInfo setupInfo,
        FileSizeComment fileSizeComment,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        InvitationLinkService invitationLinkService,
        AuthContext authContext) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _folderContentDtoHelper = folderContentDtoHelper;
        _globalFolderHelper = globalFolderHelper;
        _coreBaseSettings = coreBaseSettings;
        _apiContext = apiContext;
        _customTagsService = customTagsService;
        _roomLogoManager = roomLogoManager;
        _setupInfo = setupInfo;
        _fileSizeComment = fileSizeComment;
        _invitationLinkService = invitationLinkService;
        _authContext = authContext;
    }

    /// <summary>
    /// Returns the contents of the "Rooms" section by the parameters specified in the request.
    /// </summary>
    /// <short>Get rooms</short>
    /// <category>Rooms</category>
    /// <param type="System.Nullable{ASC.Files.Core.ApiModels.RequestDto.RoomType}, System" name="type">Filter by room type</param>
    /// <param type="System.String, System" name="subjectId">Filter by user ID</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withSubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{ASC.Files.Core.VirtualRooms.SearchArea}, System" name="searchArea">Room search area (Active, Archive, Any)</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withoutTags">Specifies whether to search by tags or not</param>
    /// <param type="System.String, System" name="tags">Tags in the serialized format</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="excludeSubject">Specifies whether to exclude a subject or not</param>
    /// <param type="System.Nullable{ASC.Files.Core.ProviderFilter}, System" name="provider">Filter by provider name (None, Box, DropBox, GoogleDrive, kDrive, OneDrive, SharePoint, WebDav, Yandex)</param>
    /// <param type="System.Nullable{ASC.Files.Core.Core.SubjectFilter}, System" name="subjectFilter">Filter by subject (Owner - 1, Member - 1)</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">Rooms contents</returns>
    /// <path>api/2.0/files/rooms</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("rooms")]
    public async Task<FolderContentDto<int>> GetRoomsFolderAsync(RoomType? type, string subjectId, bool? searchInContent, bool? withSubfolders, SearchArea? searchArea, bool? withoutTags, string tags, bool? excludeSubject,
        ProviderFilter? provider, SubjectFilter? subjectFilter)
    {
        ErrorIfNotDocSpace();

        var parentId = searchArea != SearchArea.Archive ? await _globalFolderHelper.GetFolderVirtualRooms()
            : await _globalFolderHelper.GetFolderArchive();

        var filter = type switch
        {
            RoomType.FillingFormsRoom => FilterType.FillingFormsRooms,
            RoomType.ReadOnlyRoom => FilterType.ReadOnlyRooms,
            RoomType.EditingRoom => FilterType.EditingRooms,
            RoomType.ReviewRoom => FilterType.ReviewRooms,
            RoomType.CustomRoom => FilterType.CustomRooms,
            RoomType.PublicRoom => FilterType.PublicRooms,
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

        var content = await _fileStorageService.GetFolderItemsAsync(parentId, startIndex, count, filter, false, subjectId, filterValue,
            searchInContent ?? false, withSubfolders ?? false, orderBy, searchArea ?? SearchArea.Active, default, withoutTags ?? false, tagNames, excludeSubject ?? false,
            provider ?? ProviderFilter.None, subjectFilter ?? SubjectFilter.Owner);

        var dto = await _folderContentDtoHelper.GetAsync(content, startIndex);

        return dto.NotFoundIfNull();
    }

    /// <summary>
    /// Creates a custom tag with the parameters specified in the request.
    /// </summary>
    /// <short>Create a tag</short>
    /// <category>Rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTagRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a tag</param>
    /// <returns type="System.Object, System">New tag name</returns>
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
    /// <category>Rooms</category>
    /// <returns type="System.Object, System">List of tag names</returns>
    /// <path>api/2.0/files/tags</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("tags")]
    public async IAsyncEnumerable<object> GetTagsInfoAsync()
    {
        ErrorIfNotDocSpace();

        var from = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);

        await foreach (var tag in _customTagsService.GetTagsInfoAsync<int>(_apiContext.FilterValue, TagType.Custom, from, count))
        {
            yield return tag;
        }
    }

    /// <summary>
    /// Deletes a bunch of custom tags specified in the request.
    /// </summary>
    /// <short>Delete tags</short>
    /// <category>Rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchTagsRequestDto, ASC.Files.Core" name="inDto">Batch tags request parameters</param>
    /// <returns></returns>
    /// <path>api/2.0/files/tags</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("tags")]
    public async Task DeleteTagsAsync(BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        await _customTagsService.DeleteTagsAsync<int>(inDto.Names);
    }

    /// <summary>
    /// Uploads a temporary image to create a room logo.
    /// </summary>
    /// <short>Upload an image for room logo</short>
    /// <category>Rooms</category>
    /// <param type="Microsoft.AspNetCore.Http.IFormCollection, Microsoft.AspNetCore.Http" name="formCollection">Image data</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.UploadResultDto, ASC.Files.Core">Upload result</returns>
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
                await using var inputStream = roomLogo.OpenReadStream();

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
    /// Accepts an invitation to a room via an external link.
    /// </summary>
    /// <short>Accept an invitation</short>
    /// <category>Rooms</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.AcceptInvitationDto, ASC.Files.Core" name="inDto">Request parameters for accepting invitations</param>
    /// <returns></returns>
    /// <path>api/2.0/files/rooms/accept</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("rooms/accept")]
    public async Task SetSecurityByLink(AcceptInvitationDto inDto)
    {
        var linkData = await _invitationLinkService.GetProcessedLinkDataAsync(inDto.Key, null);

        if (!linkData.IsCorrect)
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
        }

        var aces = new List<AceWrapper>
        {
            new()
            {
                Access = linkData.Share,
                Id = _authContext.CurrentAccount.ID
            }
        };

        var settings = new AceAdvancedSettingsWrapper
        {
            InvitationLink = true
        };

        if (int.TryParse(linkData.RoomId, out var id))
        {
            var aceCollection = new AceCollection<int>
            {
                Aces = aces,
                Files = Array.Empty<int>(),
                Folders = new[] { id },
                AdvancedSettings = settings
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, false);
        }
        else
        {
            var aceCollection = new AceCollection<string>
            {
                Aces = aces,
                Files = Array.Empty<string>(),
                Folders = new[] { linkData.RoomId },
                AdvancedSettings = settings
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, false);
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