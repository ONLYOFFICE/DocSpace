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

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class VirtualRoomsInternalController : VirtualRoomsController<int>
{
    public VirtualRoomsInternalController(FoldersControllerHelper<int> foldersControllerHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<int> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomInvitationLinksService roomLinksService, CustomTagsService<int> customTagsService, RoomLogoManager roomLogoManager, StudioNotifyService studioNotifyService, FileStorageService<int> fileStorageService, FolderDtoHelper folderDtoHelper, FileSecurity fileSecurity, FileSecurityCommon fileSecurityCommon, EmailValidationKeyProvider emailValidationKeyProvider) : base(foldersControllerHelper, globalFolderHelper, fileOperationDtoHelper, securityControllerHelper, coreBaseSettings, authContext, roomLinksService, customTagsService, roomLogoManager, studioNotifyService, fileStorageService, folderDtoHelper, fileSecurity, fileSecurityCommon, emailValidationKeyProvider)
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

        var room = await _fileStorageService.CreateRoomAsync(inDto.Title, inDto.RoomType);

        return await _folderDtoHelper.GetAsync(room);
    }
}

public class VirtualRoomsThirdpartyController : VirtualRoomsController<string>
{
    public VirtualRoomsThirdpartyController(FoldersControllerHelper<string> foldersControllerHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<string> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomInvitationLinksService roomLinksService, CustomTagsService<string> customTagsService, RoomLogoManager roomLogoManager, StudioNotifyService studioNotifyService, FileStorageService<string> fileStorageService, FolderDtoHelper folderDtoHelper, FileSecurity fileSecurity, FileSecurityCommon fileSecurityCommon, EmailValidationKeyProvider emailValidationKeyProvider) : base(foldersControllerHelper, globalFolderHelper, fileOperationDtoHelper, securityControllerHelper, coreBaseSettings, authContext, roomLinksService, customTagsService, roomLogoManager, studioNotifyService, fileStorageService, folderDtoHelper, fileSecurity, fileSecurityCommon, emailValidationKeyProvider)
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
    private readonly RoomInvitationLinksService _roomLinksService;
    private readonly CustomTagsService<T> _customTagsService;
    private readonly RoomLogoManager _roomLogoManager;
    private readonly StudioNotifyService _studioNotifyService;
    protected readonly FileStorageService<T> _fileStorageService;
    protected readonly FolderDtoHelper _folderDtoHelper;
    private readonly FileSecurity _fileSecurity;
    private readonly FileSecurityCommon _fileSecurityCommon;
    protected readonly EmailValidationKeyProvider _emailValidationKeyProvider;

    protected VirtualRoomsController(FoldersControllerHelper<T> foldersControllerHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<T> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomInvitationLinksService roomLinksService, CustomTagsService<T> customTagsService, RoomLogoManager roomLogoManager, StudioNotifyService studioNotifyService, FileStorageService<T> fileStorageService, FolderDtoHelper folderDtoHelper, FileSecurity fileSecurity, FileSecurityCommon fileSecurityCommon, EmailValidationKeyProvider emailValidationKeyProvider)
    {
        _foldersControllerHelper = foldersControllerHelper;
        _globalFolderHelper = globalFolderHelper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _securityControllerHelper = securityControllerHelper;
        _coreBaseSettings = coreBaseSettings;
        _authContext = authContext;
        _roomLinksService = roomLinksService;
        _customTagsService = customTagsService;
        _roomLogoManager = roomLogoManager;
        _studioNotifyService = studioNotifyService;
        _fileStorageService = fileStorageService;
        _folderDtoHelper = folderDtoHelper;
        _fileSecurity = fileSecurity;
        _fileSecurityCommon = fileSecurityCommon;
        _emailValidationKeyProvider = emailValidationKeyProvider;
    }

    /// <summary>
    /// Getting the contents of a virtual room
    /// </summary>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="startIndex">
    /// The value of the beginning of the enumeration
    /// </param>
    /// <param name="count">
    /// Quantity
    /// </param>
    /// <param name="filterValue">
    /// Filter by name
    /// </param>
    /// <param name="userOrGroupId">
    /// User or Group ID
    /// </param>
    /// <param name="filterType">
    /// Content filtering type
    /// </param>
    /// <param name="searchInContent">
    /// Full-text content search
    /// </param>
    /// <param name="withSubFolders">
    /// Search by subfolders
    /// </param>
    /// <returns>
    /// Room content
    /// </returns>
    [HttpGet("rooms/{id}")]
    public async Task<FolderContentDto<T>> GetRoomAsync(T id, Guid userOrGroupId, FilterType filterType, bool searchInContent, bool withSubFolders)
    {
        ErrorIfNotDocSpace();

        return await _foldersControllerHelper.GetFolderAsync(id, userOrGroupId, filterType, searchInContent, withSubFolders);
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

        var operationResult = _fileStorageService.DeleteFolder("delete", id, false, inDto.DeleteAfter, true)
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

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter)
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

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, inDto.DeleteAfter)
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
    public Task<IEnumerable<FileShareDto>> SetRoomSecurityAsync(T id, SecurityInfoRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        if (!string.IsNullOrEmpty(inDto.Key))
        {
            return SetRoomSecurityByLinkAsync(id, _authContext.CurrentAccount.ID, inDto.Access, inDto.Key);
        }

        return _securityControllerHelper.SetFolderSecurityInfoAsync(id, inDto.Share, inDto.Notify, inDto.SharingMessage);
    }

    /// <summary>
    /// Getting an invitation link to a virtual room
    /// </summary>
    /// <short>
    /// Get invitation link
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="access">
    /// Access level
    /// </param>
    /// <returns>
    /// Invitation link
    /// </returns>
    [HttpGet("rooms/{id}/links")]
    public async Task<object> GetInvitationLinkAsync(T id, FileShare access)
    {
        ErrorIfNotDocSpace();

        await ErrorIfNotRights(id, access);

        return _roomLinksService.GenerateLink(id, (int)access, EmployeeType.User, _authContext.CurrentAccount.ID);
    }

    /// <summary>
    /// Inviting users to the virtual room by email
    /// </summary>
    /// <short>
    /// Send invitation link
    /// </short>
    /// <param name="id">
    /// Room ID
    /// </param>
    /// <param name="emails">
    /// Mailbox addresses
    /// </param>
    /// <returns>
    /// Invitations result
    /// </returns>
    [HttpPut("rooms/{id}/links/send")]
    public async Task<IEnumerable<InviteResultDto>> SendInvitesToRoomByEmail(T id, InviteUsersByEmailRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        await ErrorIfNotRights(id, inDto.Access);

        var results = new List<InviteResultDto>();

        foreach (var email in inDto.Emails)
        {
            var result = new InviteResultDto
            {
                Email = email
            };

            try
            {
                var link = _roomLinksService.GenerateLink(id, email, (int)inDto.Access, inDto.EmployeeType, _authContext.CurrentAccount.ID);
                _studioNotifyService.SendEmailRoomInvite(email, link);

                result.Success = true;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }

            results.Add(result);
        }

        return results;
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

    protected void ErrorIfNotDocSpace()
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            throw new NotSupportedException();
        }
    }

    private async Task<IEnumerable<FileShareDto>> SetRoomSecurityByLinkAsync(T id, Guid userId, FileShare access, string key)
    {
        var result = _emailValidationKeyProvider.ValidateEmailKey(string.Empty + ConfirmType.LinkInvite + ((int)EmployeeType.User + (int)access + id.ToString()), key,
                _emailValidationKeyProvider.ValidEmailKeyInterval);

        if (result != EmailValidationKeyProvider.ValidationResult.Ok)
        {
            throw new InvalidDataException();
        }

        var share = new FileShareParams
        {
            ShareTo = userId,
            Access = access
        };

        return await _securityControllerHelper.SetFolderSecurityInfoAsync(id, new[] { share }, false, null, true);
    }

    private async Task ErrorIfNotRights(T id, FileShare share)
    {
        var room = await _fileStorageService.GetFolderAsync(id);

        if ((share == FileShare.RoomManager && !_fileSecurityCommon.IsAdministrator(_authContext.CurrentAccount.ID)) 
            || !await _fileSecurity.CanEditRoomAsync(room))
        {
            throw new InvalidOperationException("You don't have the rights to invite users to the room");
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
    private readonly RoomLogoManager _roomLogoManager;
    private readonly SetupInfo _setupInfo;
    private readonly FileSizeComment _fileSizeComment;

    public VirtualRoomsCommonController(FileStorageService<int> fileStorageService, FolderContentDtoHelper folderContentDtoHelper, GlobalFolderHelper globalFolderHelper, CoreBaseSettings coreBaseSettings, ApiContext apiContext, CustomTagsService<int> customTagsService, RoomLogoManager roomLogoManager, SetupInfo setupInfo, FileSizeComment fileSizeComment)
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
    /// <param name="tags">
    /// Filter by tags
    /// </param>
    /// <returns>
    /// Virtual Rooms content
    /// </returns>
    [HttpGet("rooms")]
    public async Task<FolderContentDto<int>> GetRoomsFolderAsync(string types, string subjectId, bool searchInContent, bool withSubfolders, SearchArea searchArea, string tags)
    {
        ErrorIfNotDocSpace();

        var parentId = await _globalFolderHelper.GetFolderVirtualRooms<int>();

        var roomTypes = !string.IsNullOrEmpty(types) ? JsonSerializer.Deserialize<IEnumerable<RoomType>>(types) : null;

        var filterTypes = roomTypes != null ? roomTypes.Select(t => t switch
        {
            RoomType.FillingFormsRoom => FilterType.FillingFormsRooms,
            RoomType.ReadOnlyRoom => FilterType.ReadOnlyRooms,
            RoomType.EditingRoom => FilterType.EditingRooms,
            RoomType.ReviewRoom => FilterType.ReviewRooms,
            RoomType.CustomRoom => FilterType.CustomRooms,
            _ => FilterType.None
        }) : new[] { FilterType.None };

        var tagNames = !string.IsNullOrEmpty(tags) ? JsonSerializer.Deserialize<IEnumerable<string>>(tags) : null;

        OrderBy orderBy = null;
        if (SortedByTypeExtensions.TryParse(_apiContext.SortBy, true, out var sortBy))
        {
            orderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
        }

        var startIndex = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);
        var filterValue = _apiContext.FilterValue;

        var content = await _fileStorageService.GetFolderItemsAsync(parentId, startIndex, count, filterTypes, false, subjectId, filterValue, searchInContent, withSubfolders, orderBy, searchArea, tagNames);

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

        await foreach (var tag in _customTagsService.GetTagsInfoAsync(_apiContext.FilterValue, TagType.Custom, from, count))
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

        await _customTagsService.DeleteTagsAsync(inDto.Names);
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
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return result;
    }

    private void ErrorIfNotDocSpace()
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            throw new NotSupportedException();
        }
    }
}