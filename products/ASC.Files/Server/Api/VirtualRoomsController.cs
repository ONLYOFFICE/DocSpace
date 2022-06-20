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

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class VirtualRoomsInternalController : VirtualRoomsController<int>
{
    public VirtualRoomsInternalController(FoldersControllerHelper<int> foldersControllerHelper, GlobalFolderHelper globalFolderHelper, FileOperationDtoHelper fileOperationDtoHelper, SecurityControllerHelper<int> securityControllerHelper, CoreBaseSettings coreBaseSettings, AuthContext authContext, RoomInvitationLinksService roomLinksService, CustomTagsService<int> customTagsService, RoomLogoManager roomLogoManager, StudioNotifyService studioNotifyService, FileStorageService<int> fileStorageService, FolderDtoHelper folderDtoHelper, FileSecurity fileSecurity, FileSecurityCommon fileSecurityCommon, EmailValidationKeyProvider emailValidationKeyProvider) : base(foldersControllerHelper, globalFolderHelper, fileOperationDtoHelper, securityControllerHelper, coreBaseSettings, authContext, roomLinksService, customTagsService, roomLogoManager, studioNotifyService, fileStorageService, folderDtoHelper, fileSecurity, fileSecurityCommon, emailValidationKeyProvider)
    {
    }

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

    [HttpGet("rooms/{id}")]
    public async Task<FolderContentDto<T>> GetRoomAsync(T id, Guid userOrGroupId, FilterType filterType, bool searchInContent, bool withSubFolders)
    {
        ErrorIfNotDocSpace();

        return await _foldersControllerHelper.GetFolderAsync(id, userOrGroupId, filterType, searchInContent, withSubFolders);
    }

    [HttpPut("rooms/{id}")]
    public async Task<FolderDto<T>> UpdateRoomAsync(T id, UpdateRoomRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.FolderRenameAsync(id, inDto.Title);

        return await _folderDtoHelper.GetAsync(room);
    }

    [HttpDelete("rooms/{id}")]
    public async Task<FileOperationDto> DeleteRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var operationResult = _fileStorageService.DeleteFolder("delete", id, false, true, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    [HttpPut("rooms/{id}/archive")]
    public async Task<FileOperationDto> ArchiveRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderArchiveAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

    [HttpPut("rooms/{id}/unarchive")]
    public async Task<FileOperationDto> UnarchiveRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var destFolder = JsonSerializer.SerializeToElement(await _globalFolderHelper.FolderVirtualRoomsAsync);
        var movableRoom = JsonSerializer.SerializeToElement(id);

        var operationResult = _fileStorageService.MoveOrCopyItems(new List<JsonElement> { movableRoom }, new List<JsonElement>(), destFolder, FileConflictResolveType.Skip, false, true)
            .FirstOrDefault();

        return await _fileOperationDtoHelper.GetAsync(operationResult);
    }

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

    [HttpGet("rooms/{id}/links")]
    public async Task<object> GetInvitationLinkAsync(T id, InviteLinkDto inDto)
    {
        ErrorIfNotDocSpace();

        await ErrorIfNotRights(id, inDto.Access);

        return _roomLinksService.GenerateLink(id, (int)inDto.Access, EmployeeType.User, _authContext.CurrentAccount.ID);
    }

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

    [HttpPut("rooms/{id}/tags")]
    public async Task<FolderDto<T>> AddTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.AddRoomTagsAsync(id, inDto.Names);

        return await _folderDtoHelper.GetAsync(room);
    }

    [HttpDelete("rooms/{id}/tags")]
    public async Task<FolderDto<T>> DeleteTagsAsync(T id, BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _customTagsService.DeleteRoomTagsAsync(id, inDto.Names);

        return await _folderDtoHelper.GetAsync(room);
    }

    [HttpPost("rooms/{id}/logo")]
    public async Task<FolderDto<T>> CreateRoomLogoAsync(T id, LogoRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        var room = await _roomLogoManager.CreateAsync(id, inDto.TmpFile, inDto.X, inDto.Y, inDto.Width, inDto.Height);

        return await _folderDtoHelper.GetAsync(room);
    }

    [HttpDelete("rooms/{id}/logo")]
    public async Task<FolderDto<T>> DeleteRoomLogoAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _roomLogoManager.DeleteAsync(id);

        return await _folderDtoHelper.GetAsync(room);
    }

    [HttpPut("rooms/{id}/pin")]
    public async Task<FolderDto<T>> PinRoomAsync(T id)
    {
        ErrorIfNotDocSpace();

        var room = await _fileStorageService.SetPinnedStatusAsync(id, true);

        return await _folderDtoHelper.GetAsync(room);
    }

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

    private async Task<IEnumerable<FileShareDto>> SetRoomSecurityByLinkAsync(T id, Guid userId, Core.Security.FileShare access, string key)
    {
        var result = _emailValidationKeyProvider.ValidateEmailKey(string.Empty + ConfirmType.RoomInvite + ((int)EmployeeType.User + (int)access + id.ToString()), key,
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
        if (Enum.TryParse(_apiContext.SortBy, true, out SortedByType sortBy))
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

    [HttpPost("tags")]
    public async Task<object> CreateTagAsync(CreateTagRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        return await _customTagsService.CreateTagAsync(inDto.Name);
    }

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

    [HttpDelete("tags")]
    public async Task DeleteTagsAsync(BatchTagsRequestDto inDto)
    {
        ErrorIfNotDocSpace();

        await _customTagsService.DeleteTagsAsync(inDto.Names);
    }

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