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
    /// Create a room in the virtual rooms section
    /// </summary>
    /// <short>
    /// Create room
    /// </short>
    /// <param name="title">
    /// Room name
    /// </param>
    /// <param name="roomType">
    /// Room preset type
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
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
    /// Creates a room in the virtual rooms section stored in a third-party storage
    /// </summary>
    /// <short>
    /// Create third-party room
    /// </short>
    /// <param name="id">
    /// ID of the folder in the third-party storage in which the contents of the room will be stored
    /// </param>
    /// <param name="title">
    /// Room name
    /// </param>
    /// <param name="roomType">
    /// Room preset type
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
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
    /// Getting virtual room information
    /// </summary>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpGet("rooms/{id}")]
    public async Task<FolderDto<T>> GetRoomInfoAsync(T id)
    {
        ErrorIfNotDocSpace();

        var folder = await _fileStorageService.GetFolderAsync(id).NotFoundIfNull("Folder not found");

        return await _folderDtoHelper.GetAsync(folder);
    }

    /// <summary>
    /// Renaming a virtual room
    /// </summary>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="title">
    /// New room name
    /// </param>
    /// <returns>
    /// Updated room info
    /// </returns>
    [HttpPut("rooms/{id}")]
    public async Task<FolderDto<T>> UpdateRoomAsync(T id, UpdateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.FolderRenameAsync(id, inDto.Title);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Permanent deletion of a virtual room
    /// </summary>
    /// <short>
    /// Deleting room
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="deleteAfter">
    /// Delete after finished
    /// </param>
    /// <returns>
    /// Result of the operation
    /// </returns>
    [HttpDelete("rooms/{id}")]
    public async Task<FileOperationDto> DeleteRoomAsync(T id, DeleteRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var operationResult = (await _fileStorageService.DeleteFolderAsync("delete", id, false, inDto.DeleteAfter, true))
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    /// <summary>
    /// Moving a room to the archive section
    /// </summary>
    /// <short>
    /// Archiving room
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="deleteAfter">
    /// Archive after finished
    /// </param>
    /// <returns>
    /// Result of the operation
    /// </returns>
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
    /// Moving a room to the virtual room section
    /// </summary>
    /// <short>
    /// Unarchiving room
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="deleteAfter">
    /// Unarchive after finished
    /// </param>
    /// <returns>
    /// Result of the operation
    /// </returns>
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
    /// Setting access rights for a virtual room
    /// </summary>
    /// <short>
    /// Set access
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="shareTo">
    /// ID of the user to whom access will be assigned
    /// </param>
    /// <param name="access">
    /// Access level
    /// </param>
    /// <param name="notify">
    /// Notifying users about access
    /// </param>
    /// <param name="sharingMessage">
    /// Notification message
    /// </param>
    /// <returns>
    /// Room security info
    /// </returns>
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
    /// Setting access rights for a virtual room
    /// </summary>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <returns>Room security info</returns>
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
    /// Setting an external invite link
    /// </summary>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="linkId">
    /// Link ID
    /// </param>
    /// <param name="title">
    /// External link name
    /// </param>
    /// /// <param name="access">
    /// Access level
    /// </param>
    /// <returns>Room security info</returns>
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
    /// Add tags for a virtual room
    /// </summary>
    /// <short>
    /// Add tags
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="names">
    /// Tag names
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpPut("rooms/{id}/tags")]
    public async Task<FolderDto<T>> AddTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.AddRoomTagsAsync(id, inDto.Names);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Attaching a tag to a virtual room
    /// </summary>
    /// <short>
    /// Add tags
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="names">
    /// Tag names
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpDelete("rooms/{id}/tags")]
    public async Task<FolderDto<T>> DeleteTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.DeleteRoomTagsAsync(id, inDto.Names);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Creating a logo for a virtual room
    /// </summary>
    /// <short>
    /// Create room logo
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="tmpFile">
    /// The path to the temporary image file
    /// </param>
    /// <param name="x">
    /// The coordinate X of the rectangle's starting point
    /// </param>
    /// <param name="y">
    /// The coordinate Y of the rectangle's starting point
    /// </param>
    /// <param name="width">
    /// Rectangle width
    /// </param>
    /// <param name="height">
    /// Rectangle height
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpPost("rooms/{id}/logo")]
    public async Task<FolderDto<T>> CreateRoomLogoAsync(T id, LogoRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _roomLogoManager.CreateAsync(id, inDto.TmpFile, inDto.X, inDto.Y, inDto.Width, inDto.Height);

        await _socketManager.UpdateFolderAsync(room);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Removing the virtual room logo
    /// </summary>
    /// <short>
    /// Remove room logo
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpDelete("rooms/{id}/logo")]
    public async Task<FolderDto<T>> DeleteRoomLogoAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _roomLogoManager.DeleteAsync(id);

        await _socketManager.UpdateFolderAsync(room);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Pins a virtual room to the list
    /// </summary>
    /// <short>
    /// Pin room
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpPut("rooms/{id}/pin")]
    public async Task<FolderDto<T>> PinRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.SetPinnedStatusAsync(id, true);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Unpins a virtual room to the list
    /// </summary>
    /// <short>
    /// Unpin room
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <returns>
    /// Room info
    /// </returns>
    [HttpPut("rooms/{id}/unpin")]
    public async Task<FolderDto<T>> UnpinRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.SetPinnedStatusAsync(id, false);

        return await _folderDtoHelper.GetAsync(room);
    }

    /// <summary>
    /// Resend room invitations
    /// </summary>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="usersIds">
    /// User IDs
    /// </param>
    /// <returns>
    /// Void
    /// </returns>
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
    /// Getting the contents of the virtual rooms section
    /// </summary>
    /// <short>
    /// Get rooms
    /// </short>
    /// <param name="startIndex">
    /// The value of the beginning of the enumeration
    /// </param>
    /// <param name="count">
    /// Quantity
    /// </param>
    /// <param name="filterValue">
    /// Filter by name
    /// </param>
    /// <param name="types">
    /// Filter by room type
    /// </param>
    /// <param name="subjectId">
    /// Filter by user ID
    /// </param>
    /// <param name="searchInContent">
    /// Full-text content search
    /// </param>
    /// <param name="withSubfolders">
    /// Search by subfolders
    /// </param>
    /// <param name="searchArea">
    /// Room search area
    /// </param>
    /// <param name="withoutTags">
    /// Search by rooms without tags
    /// </param>
    /// <param name="tags">
    /// Filter by tags
    /// </param>
    /// <param name="excludeSubject">
    /// Exclude subject from search
    /// </param>
    /// <returns>
    /// Virtual Rooms content
    /// </returns>
    [HttpGet("rooms")]
    public async Task<FolderContentDto<int>> GetRoomsFolderAsync(RoomFilterType? type, string subjectId, bool? searchInContent, bool? withSubfolders, SearchArea? searchArea, bool? withoutTags, string tags, bool? excludeSubject,
        ProviderFilter? provider, SubjectFilter? subjectFilter)
    {
        ErrorIfNotDocSpace();

        var parentId = searchArea != SearchArea.Archive ? await _globalFolderHelper.GetFolderVirtualRooms()
            : await _globalFolderHelper.GetFolderArchive();

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

        var content = await _fileStorageService.GetFolderItemsAsync(parentId, startIndex, count, filter, false, subjectId, filterValue,
            searchInContent ?? false, withSubfolders ?? false, orderBy, searchArea ?? SearchArea.Active, default, withoutTags ?? false, tagNames, excludeSubject ?? false,
            provider ?? ProviderFilter.None, subjectFilter ?? SubjectFilter.Owner);

        var dto = await _folderContentDtoHelper.GetAsync(content, startIndex);

        return dto.NotFoundIfNull();
    }

    /// <summary>
    /// Create a custom tag
    /// </summary>
    /// <short>
    /// Create tag
    /// </short>
    /// <param name="name">
    /// Tag name
    /// </param>
    /// <returns>
    /// Tag name
    /// </returns>
    [HttpPost("tags")]
    public async Task<object> CreateTagAsync(CreateTagRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return await _customTagsService.CreateTagAsync(inDto.Name);
    }

    /// <summary>
    /// Getting a list of custom tags
    /// </summary>
    /// <short>
    /// Get tags
    /// </short>
    /// <param name="startIndex">
    /// The value of the beginning of the enumeration
    /// </param>
    /// <param name="count">
    /// Quantity
    /// </param>
    /// <param name="filterValue">
    /// Filter by name
    /// </param>
    /// <returns>
    /// Tag names
    /// </returns>
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
    /// Delete a bunch of custom tags
    /// </summary>
    /// <short>
    /// Delete tags
    /// </short>
    /// <returns>
    /// Void
    /// </returns>
    [HttpDelete("tags")]
    public async Task DeleteTagsAsync(BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        await _customTagsService.DeleteTagsAsync<int>(inDto.Names);
    }

    /// <summary>
    /// Upload a temporary image to create a virtual room logo
    /// </summary>
    /// <short>
    /// Upload image for room logo
    /// </short>
    /// <param name="formCollection">
    /// Image data
    /// </param>
    /// <returns>
    /// Upload result
    /// </returns>
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
    /// Accept an invitation in a room via an external link
    /// </summary>
    /// <param name="key">
    /// Link key
    /// </param>
    /// <returns>
    /// Void
    /// </returns>
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