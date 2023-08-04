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

namespace ASC.People.Api;

public class PhotoController : PeopleControllerBase
{
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly SecurityContext _securityContext;
    private readonly SettingsManager _settingsManager;
    private readonly FileSizeComment _fileSizeComment;
    private readonly SetupInfo _setupInfo;

    public PhotoController(
        UserManager userManager,
        PermissionContext permissionContext,
        ApiContext apiContext,
        UserPhotoManager userPhotoManager,
        MessageService messageService,
        MessageTarget messageTarget,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        SecurityContext securityContext,
        SettingsManager settingsManager,
        FileSizeComment fileSizeComment,
        SetupInfo setupInfo,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
        : base(userManager, permissionContext, apiContext, userPhotoManager, httpClientFactory, httpContextAccessor)
    {
        _messageService = messageService;
        _messageTarget = messageTarget;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _securityContext = securityContext;
        _settingsManager = settingsManager;
        _fileSizeComment = fileSizeComment;
        _setupInfo = setupInfo;
    }

    /// <summary>
    /// Creates photo thumbnails by coordinates of the original image specified in the request.
    /// </summary>
    /// <short>
    /// Create photo thumbnails
    /// </short>
    /// <category>Photos</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.ThumbnailsRequestDto, ASC.People" name="inDto">Thumbnail request parameters</param>
    /// <path>api/2.0/people/{userid}/photo/thumbnails</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns type="ASC.People.ApiModels.ResponseDto.ThumbnailsDataDto, ASC.People">Thumbnail parameters</returns>
    [HttpPost("{userid}/photo/thumbnails")]
    public async Task<ThumbnailsDataDto> CreateMemberPhotoThumbnails(string userid, ThumbnailsRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        if (!string.IsNullOrEmpty(inDto.TmpFile))
        {
            var fileName = Path.GetFileName(inDto.TmpFile);
            var data = await _userPhotoManager.GetTempPhotoData(fileName);

            UserPhotoThumbnailSettings settings = null;

            if (inDto.Width == 0 && inDto.Height == 0)
            {
                using var img = Image.Load(data);
                settings = new UserPhotoThumbnailSettings(inDto.X, inDto.Y, img.Width, img.Height);
            }
            else
            {
                settings = new UserPhotoThumbnailSettings(inDto.X, inDto.Y, inDto.Width, inDto.Height);
            }

            await _settingsManager.SaveAsync(settings, user.Id);

            await _userPhotoManager.RemovePhotoAsync(user.Id);
            await _userPhotoManager.SaveOrUpdatePhoto(user.Id, data);
            await _userPhotoManager.RemoveTempPhotoAsync(fileName);
        }
        else
        {
            await UserPhotoThumbnailManager.SaveThumbnails(_userPhotoManager, _settingsManager, inDto.X, inDto.Y, inDto.Width, inDto.Height, user.Id);
        }

        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
        await _messageService.SendAsync(MessageAction.UserUpdatedAvatarThumbnails, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));
        return await ThumbnailsDataDto.Create(user, _userPhotoManager);
    }

    /// <summary>
    /// Deletes a photo of the user with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Delete a user photo
    /// </short>
    /// <category>Photos</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.ThumbnailsDataDto, ASC.People">Thumbnail parameters: original photo, retina, maximum size photo, big, medium, small</returns>
    /// <path>api/2.0/people/{userid}/photo</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("{userid}/photo")]
    public async Task<ThumbnailsDataDto> DeleteMemberPhotoAsync(string userid)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        await _userPhotoManager.RemovePhotoAsync(user.Id);
        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
        await _messageService.SendAsync(MessageAction.UserDeletedAvatar, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        return await ThumbnailsDataDto.Create(user, _userPhotoManager);
    }

    /// <summary>
    /// Returns a photo of the user with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Get a user photo
    /// </short>
    /// <category>Photos</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.ThumbnailsDataDto, ASC.People">Thumbnail parameters: original photo, retina, maximum size photo, big, medium, small</returns>
    /// <path>api/2.0/people/{userid}/photo</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("{userid}/photo")]
    public async Task<ThumbnailsDataDto> GetMemberPhoto(string userid)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        return await ThumbnailsDataDto.Create(user, _userPhotoManager);
    }

    /// <summary>
    /// Updates a photo of the user with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Update a user photo
    /// </short>
    /// <category>Photos</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Request parameters for updating user photo</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.ThumbnailsDataDto, ASC.People">Updated thumbnail parameters: original photo, retina, maximum size photo, big, medium, small</returns>
    /// <path>api/2.0/people/{userid}/photo</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}/photo")]
    public async Task<ThumbnailsDataDto> UpdateMemberPhoto(string userid, UpdateMemberRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        if (inDto.Files != await _userPhotoManager.GetPhotoAbsoluteWebPath(user.Id))
        {
            await UpdatePhotoUrlAsync(inDto.Files, user);
        }

        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
        await _messageService.SendAsync(MessageAction.UserAddedAvatar, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        return await ThumbnailsDataDto.Create(user, _userPhotoManager);
    }

    /// <summary>
    /// Uploads a photo of the user with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Upload a user photo
    /// </short>
    /// <category>Photos</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="Microsoft.AspNetCore.Http.IFormCollection, Microsoft.AspNetCore.Http" name="formCollection">Image data</param>
    /// <path>api/2.0/people/{userid}/photo</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns type="ASC.People.ApiModels.ResponseDto.FileUploadResultDto, ASC.People">Result of file uploading</returns>
    [HttpPost("{userid}/photo")]
    public async Task<FileUploadResultDto> UploadMemberPhoto(string userid, IFormCollection formCollection)
    {
        var result = new FileUploadResultDto();
        var autosave = bool.Parse(formCollection["Autosave"]);

        try
        {
            if (formCollection.Files.Count != 0)
            {
                Guid userId;
                try
                {
                    userId = new Guid(userid);
                }
                catch
                {
                    userId = _securityContext.CurrentAccount.ID;
                }

                await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(userId), Constants.Action_EditUser);

                var userPhoto = formCollection.Files[0];

                if (userPhoto.Length > _setupInfo.MaxImageUploadSize)
                {
                    result.Success = false;
                    result.Message = _fileSizeComment.FileImageSizeExceptionString;

                    return result;
                }

                var data = new byte[userPhoto.Length];
                await using var inputStream = userPhoto.OpenReadStream();

                var br = new BinaryReader(inputStream);
                br.Read(data, 0, (int)userPhoto.Length);
                br.Close();

                CheckImgFormat(data);

                if (autosave)
                {
                    if (data.Length > _setupInfo.MaxImageUploadSize)
                    {
                        throw new ImageSizeLimitException();
                    }

                    var mainPhoto = await _userPhotoManager.SaveOrUpdatePhoto(userId, data);
                    var userInfo = _userManager.GetUsers(userId);
                    var cacheKey = Math.Abs(userInfo.LastModified.GetHashCode());

                    result.Data =
                        new
                        {
                            main = mainPhoto.Item1 + $"?hash={cacheKey}",
                            retina = await _userPhotoManager.GetRetinaPhotoURL(userId) + $"?hash={cacheKey}",
                            max = await _userPhotoManager.GetMaxPhotoURL(userId) + $"?hash={cacheKey}",
                            big = await _userPhotoManager.GetBigPhotoURL(userId) + $"?hash={cacheKey}",
                            medium = await _userPhotoManager.GetMediumPhotoURL(userId) + $"?hash={cacheKey}",
                            small = await _userPhotoManager.GetSmallPhotoURL(userId) + $"?hash={cacheKey}"
                        };
                }
                else
                {
                    result.Data = await _userPhotoManager.SaveTempPhoto(data, _setupInfo.MaxImageUploadSize, UserPhotoManager.OriginalFotoSize.Width, UserPhotoManager.OriginalFotoSize.Height);
                }

                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorEmptyUploadFileSelected;
            }

        }
        catch (Web.Core.Users.UnknownImageFormatException)
        {
            result.Success = false;
            result.Message = PeopleResource.ErrorUnknownFileImageType;
        }
        catch (ImageWeightLimitException)
        {
            result.Success = false;
            result.Message = PeopleResource.ErrorImageWeightLimit;
        }
        catch (ImageSizeLimitException)
        {
            result.Success = false;
            result.Message = PeopleResource.ErrorImageSizetLimit;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message.HtmlEncode();
        }

        return result;
    }

    private static void CheckImgFormat(byte[] data)
    {
        IImageFormat imgFormat;
        try
        {
            using var img = Image.Load(data);
            imgFormat = img.Metadata.DecodedImageFormat;
        }
        catch (OutOfMemoryException)
        {
            throw new ImageSizeLimitException();
        }
        catch (ArgumentException error)
        {
            throw new Web.Core.Users.UnknownImageFormatException(error);
        }

        if (imgFormat.Name != "PNG" && imgFormat.Name != "JPEG")
        {
            throw new Web.Core.Users.UnknownImageFormatException();
        }
    }
}
