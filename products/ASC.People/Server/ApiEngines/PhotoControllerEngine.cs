using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.ApiHelpers;

public class PhotoControllerEngine : PeopleControllerEngine
{
    private readonly FileSizeComment _fileSizeComment;
    private readonly SettingsManager _settingsManager;

    public PhotoControllerEngine(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        FileSizeComment fileSizeComment,
        SettingsManager settingsManager,
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        SetupInfo setupInfo)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService,
            userPhotoManager,
            httpClientFactory,
            displayUserSettingsHelper,
            setupInfo)
    {
        _fileSizeComment = fileSizeComment;
        _settingsManager = settingsManager;
    }

    public ThumbnailsDataDto CreateMemberPhotoThumbnails(string userid, ThumbnailsRequestDto thumbnailsModel)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        if (!string.IsNullOrEmpty(thumbnailsModel.TmpFile))
        {
            var fileName = Path.GetFileName(thumbnailsModel.TmpFile);
            var data = _userPhotoManager.GetTempPhotoData(fileName);

            var settings = new UserPhotoThumbnailSettings(thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height);
            _settingsManager.SaveForUser(settings, user.Id);
            _userPhotoManager.RemovePhoto(user.Id);
            _userPhotoManager.SaveOrUpdatePhoto(user.Id, data);
            _userPhotoManager.RemoveTempPhoto(fileName);
        }
        else
        {
            UserPhotoThumbnailManager.SaveThumbnails(_userPhotoManager, _settingsManager, thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height, user.Id);
        }

        _userManager.SaveUserInfo(user);
        _messageService.Send(MessageAction.UserUpdatedAvatarThumbnails, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        return new ThumbnailsDataDto(user.Id, _userPhotoManager);
    }

    public ThumbnailsDataDto DeleteMemberPhoto(string userid)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        _userPhotoManager.RemovePhoto(user.Id);
        _userManager.SaveUserInfo(user);
        _messageService.Send(MessageAction.UserDeletedAvatar, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        return new ThumbnailsDataDto(user.Id, _userPhotoManager);
    }

    public ThumbnailsDataDto GetMemberPhoto(string userid)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        return new ThumbnailsDataDto(user.Id, _userPhotoManager);
    }

    public ThumbnailsDataDto UpdateMemberPhoto(string userid, UpdateMemberRequestDto model)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        if (model.Files != _userPhotoManager.GetPhotoAbsoluteWebPath(user.Id))
        {
            UpdatePhotoUrl(model.Files, user);
        }

        _userManager.SaveUserInfo(user);
        _messageService.Send(MessageAction.UserAddedAvatar, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        return new ThumbnailsDataDto(user.Id, _userPhotoManager);
    }

    public FileUploadResultDto UploadMemberPhoto(string userid, IFormCollection model)
    {
        var result = new FileUploadResultDto();
        var autosave = bool.Parse(model["Autosave"]);

        try
        {
            if (model.Files.Count != 0)
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

                _permissionContext.DemandPermissions(new UserSecurityProvider(userId), Constants.Action_EditUser);

                var userPhoto = model.Files[0];

                if (userPhoto.Length > _setupInfo.MaxImageUploadSize)
                {
                    result.Success = false;
                    result.Message = _fileSizeComment.FileImageSizeExceptionString;

                    return result;
                }

                var data = new byte[userPhoto.Length];
                using var inputStream = userPhoto.OpenReadStream();

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

                    var mainPhoto = _userPhotoManager.SaveOrUpdatePhoto(userId, data);

                    result.Data =
                        new
                        {
                            main = mainPhoto,
                            retina = _userPhotoManager.GetRetinaPhotoURL(userId),
                            max = _userPhotoManager.GetMaxPhotoURL(userId),
                            big = _userPhotoManager.GetBigPhotoURL(userId),
                            medium = _userPhotoManager.GetMediumPhotoURL(userId),
                            small = _userPhotoManager.GetSmallPhotoURL(userId),
                        };
                }
                else
                {
                    result.Data = _userPhotoManager.SaveTempPhoto(data, _setupInfo.MaxImageUploadSize, UserPhotoManager.OriginalFotoSize.Width, UserPhotoManager.OriginalFotoSize.Height);
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
            using var img = Image.Load(data, out var format);
            imgFormat = format;
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
