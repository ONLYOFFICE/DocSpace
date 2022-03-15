namespace ASC.Web.Core.Users
{
    public static class UserPhotoThumbnailManager
    {
        public static List<ThumbnailItem> SaveThumbnails(UserPhotoManager userPhotoManager, SettingsManager settingsManager, int x, int y, int width, int height, Guid userId)
        {
            return SaveThumbnails(userPhotoManager, settingsManager, new UserPhotoThumbnailSettings(x, y, width, height), userId);
        }

        public static List<ThumbnailItem> SaveThumbnails(UserPhotoManager userPhotoManager, SettingsManager settingsManager, Point point, Size size, Guid userId)
        {
            return SaveThumbnails(userPhotoManager, settingsManager, new UserPhotoThumbnailSettings(point, size), userId);
        }

        public static List<ThumbnailItem> SaveThumbnails(UserPhotoManager userPhotoManager, SettingsManager settingsManager, UserPhotoThumbnailSettings thumbnailSettings, Guid userId)
        {
            if (thumbnailSettings.Size.IsEmpty) return null;

            var thumbnailsData = new ThumbnailsData(userId, userPhotoManager);

            var resultBitmaps = new List<ThumbnailItem>();

            using var img = thumbnailsData.MainImgBitmap(out var format);

            if (img == null) return null;

            foreach (var thumbnail in thumbnailsData.ThumbnailList())
            {
                thumbnail.Image = GetImage(img, thumbnail.Size, thumbnailSettings);

                resultBitmaps.Add(thumbnail);
            }

            thumbnailsData.Save(resultBitmaps);

            settingsManager.SaveForUser(thumbnailSettings, userId);

            return thumbnailsData.ThumbnailList();
        }

        public static Image GetImage(Image mainImg, Size size, UserPhotoThumbnailSettings thumbnailSettings)
        {

            var x = thumbnailSettings.Point.X > 0 ? thumbnailSettings.Point.X : 0;
            var y = thumbnailSettings.Point.Y > 0 ? thumbnailSettings.Point.Y : 0;
            var width =  x + thumbnailSettings.Size.Width > mainImg.Width ? mainImg.Width : thumbnailSettings.Size.Width;
            var height = y + thumbnailSettings.Size.Height > mainImg.Height ? mainImg.Height : thumbnailSettings.Size.Height;

            var rect = new Rectangle(x,
                                     y,
                                     width,
                                     height);

            Image destRound = mainImg.Clone(x => x.Crop(rect).Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Stretch
            }));

            return destRound;
        }
    }

    public class ThumbnailItem
    {
        public Size Size { get; set; }
        public string ImgUrl { get; set; }
        public Image Image { get; set; }
    }

    public class ThumbnailsData
    {
        private Guid UserId { get; set; }
        private UserPhotoManager UserPhotoManager { get; }

        public ThumbnailsData(Guid userId, UserPhotoManager userPhotoManager)
        {
            UserId = userId;
            UserPhotoManager = userPhotoManager;
        }

        public Image MainImgBitmap(out IImageFormat format)
        {
            var img = UserPhotoManager.GetPhotoImage(UserId, out var imageFormat);
            format = imageFormat;
            return img;
        }

        public string MainImgUrl()
        {
            return UserPhotoManager.GetPhotoAbsoluteWebPath(UserId);
        }

        public List<ThumbnailItem> ThumbnailList()
        {
            return new List<ThumbnailItem>
                {
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.RetinaFotoSize,
                            ImgUrl = UserPhotoManager.GetRetinaPhotoURL(UserId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.MaxFotoSize,
                            ImgUrl = UserPhotoManager.GetMaxPhotoURL(UserId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.BigFotoSize,
                            ImgUrl = UserPhotoManager.GetBigPhotoURL(UserId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.MediumFotoSize,
                            ImgUrl = UserPhotoManager.GetMediumPhotoURL(UserId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.SmallFotoSize,
                            ImgUrl = UserPhotoManager.GetSmallPhotoURL(UserId)
                        }
            };
        }

        public void Save(List<ThumbnailItem> bitmaps)
        {
            foreach (var item in bitmaps)
            {
                using var mainImgBitmap = MainImgBitmap(out var format);
                UserPhotoManager.SaveThumbnail(UserId, item.Image, format);
            }
        }
    }
}