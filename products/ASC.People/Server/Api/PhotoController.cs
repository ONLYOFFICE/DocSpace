namespace ASC.People.Api;

public class PhotoController : ApiControllerBase
{
    private readonly PhotoControllerEngine _photoControllerEgine;

    public PhotoController(PhotoControllerEngine photoControllerEgine)
    {
        _photoControllerEgine = photoControllerEgine;
    }

    [Create("{userid}/photo/thumbnails")]
    public ThumbnailsDataDto CreateMemberPhotoThumbnailsFromBody(string userid, [FromBody] ThumbnailsRequestDto thumbnailsModel)
    {
        return _photoControllerEgine.CreateMemberPhotoThumbnails(userid, thumbnailsModel);
    }

    [Create("{userid}/photo/thumbnails")]
    [Consumes("application/x-www-form-urlencoded")]
    public ThumbnailsDataDto CreateMemberPhotoThumbnailsFromForm(string userid, [FromForm] ThumbnailsRequestDto thumbnailsModel)
    {
        return _photoControllerEgine.CreateMemberPhotoThumbnails(userid, thumbnailsModel);
    }

    [Delete("{userid}/photo")]
    public ThumbnailsDataDto DeleteMemberPhoto(string userid)
    {
        return _photoControllerEgine.DeleteMemberPhoto(userid);
    }

    [Read("{userid}/photo")]
    public ThumbnailsDataDto GetMemberPhoto(string userid)
    {
        return _photoControllerEgine.GetMemberPhoto(userid);
    }

    [Update("{userid}/photo")]
    public ThumbnailsDataDto UpdateMemberPhotoFromBody(string userid, [FromBody] UpdateMemberRequestDto model)
    {
        return _photoControllerEgine.UpdateMemberPhoto(userid, model);
    }

    [Update("{userid}/photo")]
    [Consumes("application/x-www-form-urlencoded")]
    public ThumbnailsDataDto UpdateMemberPhotoFromForm(string userid, [FromForm] UpdateMemberRequestDto model)
    {
        return _photoControllerEgine.UpdateMemberPhoto(userid, model);
    }

    [Create("{userid}/photo")]
    public FileUploadResultDto UploadMemberPhoto(string userid, IFormCollection model)
    {
        return _photoControllerEgine.UploadMemberPhoto(userid, model);
    }
}
