namespace ASC.Web.Core.Users
{
    [Serializable]
    public class UserPhotoThumbnailSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{CC3AA821-43CA-421B-BDCD-81FB6D3361CF}"); }
        }
        public UserPhotoThumbnailSettings()
        {

        }

        public UserPhotoThumbnailSettings(Point point, Size size)
        {
            Point = point;
            Size = size;
        }

        public UserPhotoThumbnailSettings(int x, int y, int width, int height)
        {
            Point = new Point(x, y);
            Size = new Size(width, height);
        }

        public Point Point { get; set; }

        public Size Size { get; set; }

        public bool IsDefault { get; private set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new UserPhotoThumbnailSettings
            {
                Point = new Point(0, 0),
                Size = new Size(UserPhotoManager.MaxFotoSize.Width, UserPhotoManager.MaxFotoSize.Height),
                IsDefault = true
            };
        }
    }
}