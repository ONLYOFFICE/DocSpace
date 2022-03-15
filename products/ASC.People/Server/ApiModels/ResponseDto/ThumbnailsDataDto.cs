namespace ASC.People.ApiModels.ResponseDto;

public class ThumbnailsDataDto
{
    public ThumbnailsDataDto(Guid userId, UserPhotoManager userPhotoManager)
    {
        Original = userPhotoManager.GetPhotoAbsoluteWebPath(userId);
        Retina = userPhotoManager.GetRetinaPhotoURL(userId);
        Max = userPhotoManager.GetMaxPhotoURL(userId);
        Big = userPhotoManager.GetBigPhotoURL(userId);
        Medium = userPhotoManager.GetMediumPhotoURL(userId);
        Small = userPhotoManager.GetSmallPhotoURL(userId);
    }

    private ThumbnailsDataDto() { }

    public string Original { get; set; }
    public string Retina { get; set; }
    public string Max { get; set; }
    public string Big { get; set; }
    public string Medium { get; set; }
    public string Small { get; set; }

    public static ThumbnailsDataDto GetSample()
    {
        return new ThumbnailsDataDto
        {
            Original = "default_user_photo_size_1280-1280.png",
            Retina = "default_user_photo_size_360-360.png",
            Max = "default_user_photo_size_200-200.png",
            Big = "default_user_photo_size_82-82.png",
            Medium = "default_user_photo_size_48-48.png",
            Small = "default_user_photo_size_32-32.png",
        };
    }
}
