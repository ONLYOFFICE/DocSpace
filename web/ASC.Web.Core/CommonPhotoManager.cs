namespace ASC.Web.Core
{
    public static class CommonPhotoManager
    {

        public static Image DoThumbnail(Image image, Size size, bool crop, bool transparent, bool rectangle)
        {
            var width = size.Width;
            var height = size.Height;
            var realWidth = image.Width;
            var realHeight = image.Height;

            Image thumbnail;

            var maxSide = realWidth > realHeight ? realWidth : realHeight;
            var minSide = realWidth < realHeight ? realWidth : realHeight;
            
            var alignWidth = crop ? (minSide == realWidth) : (maxSide == realWidth);

            var scaleFactor = alignWidth ? (realWidth / (1.0 * width)) : (realHeight / (1.0 * height));

            if (scaleFactor < 1) scaleFactor = 1;

            int locationX, locationY;
            int finalWidth, finalHeigth;

            finalWidth = (int)(realWidth / scaleFactor);
            finalHeigth = (int)(realHeight / scaleFactor);


            if (rectangle)
            {
                thumbnail = new Image<Rgba32>(width, height);
                locationY = (int)((height / 2.0) - (finalHeigth / 2.0));
                locationX = (int)((width / 2.0) - (finalWidth / 2.0));

                if (!transparent)
                {
                    thumbnail.Mutate(x=> x.Clear(Color.White));
                }
                var point = new Point(locationX, locationY);
                image.Mutate(y => y.Resize(finalWidth, finalHeigth));
                thumbnail.Mutate(x => x.DrawImage(image, point, 1));
            }
            else
            {
                thumbnail = new Image<Rgba32>(finalWidth, finalHeigth);

                if (!transparent)
                {
                    thumbnail.Mutate(x => x.Clear(Color.White));
                }
                image.Mutate(y => y.Resize(finalWidth, finalHeigth));
                thumbnail.Mutate(x => x.DrawImage(image, 1));
            }

            return thumbnail;
        }

        public static byte[] SaveToBytes(Image img)
        {
            using var memoryStream = new MemoryStream();
            img.Save(memoryStream, PngFormat.Instance);
            return memoryStream.ToArray();
        }

        public static byte[] SaveToBytes(Image img, IImageFormat imageFormat)
        {
            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                img.Save(memoryStream, imageFormat);
                data = memoryStream.ToArray();
            }
            return data;
        }

        public static string GetImgFormatName(IImageFormat format)
        {
            return format.Name.ToLower();
        }
    }
}
