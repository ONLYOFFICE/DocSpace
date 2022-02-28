using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class PhotoController : BasePeopleController
{
    private readonly FileSizeComment _fileSizeComment;
    private readonly SettingsManager _settingsManager;

    public PhotoController(
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
        IMapper mapper)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService,
            mapper)
    {
        _fileSizeComment = fileSizeComment;
        _settingsManager = settingsManager;
    }

    [Create("{userid}/photo/thumbnails")]
    public ThumbnailsDataDto CreateMemberPhotoThumbnailsFromBody(string userid, [FromBody] ThumbnailsRequestDto thumbnailsModel)
    {
        return CreateMemberPhotoThumbnails(userid, thumbnailsModel);
    }

    [Create("{userid}/photo/thumbnails")]
    [Consumes("application/x-www-form-urlencoded")]
    public ThumbnailsDataDto CreateMemberPhotoThumbnailsFromForm(string userid, [FromForm] ThumbnailsRequestDto thumbnailsModel)
    {
        return CreateMemberPhotoThumbnails(userid, thumbnailsModel);
    }

    [Delete("{userid}/photo")]
    public ThumbnailsDataDto DeleteMemberPhoto(string userid)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

        UserPhotoManager.RemovePhoto(user.ID);
        UserManager.SaveUserInfo(user);
        MessageService.Send(MessageAction.UserDeletedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

        return new ThumbnailsDataDto(user.ID, UserPhotoManager);
    }

    [Read("{userid}/photo")]
    public ThumbnailsDataDto GetMemberPhoto(string userid)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        return new ThumbnailsDataDto(user.ID, UserPhotoManager);
    }


    [Update("{userid}/photo")]
    public ThumbnailsDataDto UpdateMemberPhotoFromBody(string userid, [FromBody] UpdateMemberRequestDto model)
    {
        return UpdateMemberPhoto(userid, model);
    }

    [Update("{userid}/photo")]
    [Consumes("application/x-www-form-urlencoded")]
    public ThumbnailsDataDto UpdateMemberPhotoFromForm(string userid, [FromForm] UpdateMemberRequestDto model)
    {
        return UpdateMemberPhoto(userid, model);
    }

    [Create("{userid}/photo")]
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
                    userId = SecurityContext.CurrentAccount.ID;
                }

                PermissionContext.DemandPermissions(new UserSecurityProvider(userId), Constants.Action_EditUser);

                var userPhoto = model.Files[0];

                if (userPhoto.Length > SetupInfo.MaxImageUploadSize)
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
                    if (data.Length > SetupInfo.MaxImageUploadSize)
                    {
                        throw new ImageSizeLimitException();
                    }

                    var mainPhoto = UserPhotoManager.SaveOrUpdatePhoto(userId, data);

                    result.Data =
                        new
                        {
                            main = mainPhoto,
                            retina = UserPhotoManager.GetRetinaPhotoURL(userId),
                            max = UserPhotoManager.GetMaxPhotoURL(userId),
                            big = UserPhotoManager.GetBigPhotoURL(userId),
                            medium = UserPhotoManager.GetMediumPhotoURL(userId),
                            small = UserPhotoManager.GetSmallPhotoURL(userId),
                        };
                }
                else
                {
                    result.Data = UserPhotoManager.SaveTempPhoto(data, SetupInfo.MaxImageUploadSize, UserPhotoManager.OriginalFotoSize.Width, UserPhotoManager.OriginalFotoSize.Height);
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

    private ThumbnailsDataDto CreateMemberPhotoThumbnails(string userid, ThumbnailsRequestDto thumbnailsModel)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

        if (!string.IsNullOrEmpty(thumbnailsModel.TmpFile))
        {
            var fileName = Path.GetFileName(thumbnailsModel.TmpFile);
            var data = UserPhotoManager.GetTempPhotoData(fileName);

            var settings = new UserPhotoThumbnailSettings(thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height);
            _settingsManager.SaveForUser(settings, user.ID);
            UserPhotoManager.RemovePhoto(user.ID);
            UserPhotoManager.SaveOrUpdatePhoto(user.ID, data);
            UserPhotoManager.RemoveTempPhoto(fileName);
        }
        else
        {
            UserPhotoThumbnailManager.SaveThumbnails(UserPhotoManager, _settingsManager, thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height, user.ID);
        }

        UserManager.SaveUserInfo(user);
        MessageService.Send(MessageAction.UserUpdatedAvatarThumbnails, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

        return new ThumbnailsDataDto(user.ID, UserPhotoManager);
    }

    private ThumbnailsDataDto UpdateMemberPhoto(string userid, UpdateMemberRequestDto model)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        if (model.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(user.ID))
        {
            UpdatePhotoUrl(model.Files, user);
        }

        UserManager.SaveUserInfo(user);
        MessageService.Send(MessageAction.UserAddedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

        return new ThumbnailsDataDto(user.ID, UserPhotoManager);
    }
}
