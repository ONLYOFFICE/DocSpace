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

/*
using System.IO;

using ASC.Data.Storage;

namespace ASC.Web.Studio.Helpers
{
    public class ImageInfo
    {
        #region Members

        private string name;

        private int originalWidth;
        private int originalHeight;
        private int previewWidth;
        private int previewHeight;
        private int thumbnailWidth;
        private int thumbnailHeight;

        private string originalPath;
        private string previewPath;
        private string thumbnailPath;

        private string actionDate;

        #endregion

        #region Properties

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual int OriginalWidth
        {
            get { return originalWidth; }
            set { originalWidth = value; }
        }
        public virtual int OriginalHeight
        {
            get { return originalHeight; }
            set { originalHeight = value; }
        }
        public virtual int PreviewWidth
        {
            get { return previewWidth; }
            set { previewWidth = value; }
        }
        public virtual int PreviewHeight
        {
            get { return previewHeight; }
            set { previewHeight = value; }
        }
        public virtual int ThumbnailWidth
        {
            get { return thumbnailWidth; }
            set { thumbnailWidth = value; }
        }
        public virtual int ThumbnailHeight
        {
            get { return thumbnailHeight; }
            set { thumbnailHeight = value; }
        }

        public virtual string OriginalPath
        {
            get { return originalPath; }
            set { originalPath = value; }
        }
        public virtual string PreviewPath
        {
            get { return previewPath; }
            set { previewPath = value; }
        }
        public virtual string ThumbnailPath
        {
            get { return thumbnailPath; }
            set { thumbnailPath = value; }
        }

        public virtual string ActionDate
        {
            get { return actionDate; }
            set { actionDate = value; }
        }

        #endregion
    }

    public class ThumbnailGenerator
    {
        readonly bool _crop = false;
        readonly int _width;
        readonly int _heigth;
        readonly int _widthPreview;
        readonly int _heightPreview;

        public IDataStore Store
        {
            get;
            set;
        }

        public ThumbnailGenerator(bool crop, int width, int heigth, int widthPreview, int heightPreview)
        {
            _crop = crop;
            _width = width;
            _heigth = heigth;
            _widthPreview = widthPreview;
            _heightPreview = heightPreview;
        }

        public void DoThumbnail(string path, string outputPath, ref ImageInfo imageInfo)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            DoThumbnail(fs, outputPath, ref imageInfo);
        }
        public void DoThumbnail(Stream image, string outputPath, ref ImageInfo imageInfo)
        {
            using var img = Image.FromStream(image);
            DoThumbnail(img, outputPath, ref imageInfo);
        }
        public void DoThumbnail(Image image, string outputPath, ref ImageInfo imageInfo)
        {
            var realWidth = image.Width;
            var realHeight = image.Height;

            imageInfo.OriginalWidth = realWidth;
            imageInfo.OriginalHeight = realHeight;

            var ep = new EncoderParameters(1);

            ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)90);

            var icJPG = GetCodecInfo("image/jpeg");

            if (!string.IsNullOrEmpty(imageInfo.Name) && imageInfo.Name.Contains("."))
            {
                var indexDot = imageInfo.Name.ToLower().LastIndexOf(".");

                if (imageInfo.Name.ToLower().IndexOf("png", indexDot) > indexDot)
                    icJPG = GetCodecInfo("image/png");
                else if (imageInfo.Name.ToLower().IndexOf("gif", indexDot) > indexDot)
                    icJPG = GetCodecInfo("image/png");
            }
            Bitmap thumbnail;

            if (realWidth < _width && realHeight < _heigth)
            {
                imageInfo.ThumbnailWidth = realWidth;
                imageInfo.ThumbnailHeight = realHeight;

                if (Store == null)
                    image.Save(outputPath);
                else
                {

                    var ms = new MemoryStream();
                    image.Save(ms, icJPG, ep);
                    ms.Seek(0, SeekOrigin.Begin);
                    Store.Save(outputPath, ms);
                    ms.Dispose();
                }
                return;
            }
            else
            {
                thumbnail = new Bitmap(_width < realWidth ? _width : realWidth, _heigth < realHeight ? _heigth : realHeight);

                var maxSide = realWidth > realHeight ? realWidth : realHeight;
                var minSide = realWidth < realHeight ? realWidth : realHeight;

                var alignWidth = true;
                if (_crop)
                    alignWidth = (minSide == realWidth);
                else
                    alignWidth = (maxSide == realWidth);

                var scaleFactor = (alignWidth) ? (realWidth / (1.0 * _width)) : (realHeight / (1.0 * _heigth));

                if (scaleFactor < 1) scaleFactor = 1;

                int locationX, locationY;
                int finalWidth, finalHeigth;

                finalWidth = (int)(realWidth / scaleFactor);
                finalHeigth = (int)(realHeight / scaleFactor);


                locationY = (int)(((_heigth < realHeight ? _heigth : realHeight) / 2.0) - (finalHeigth / 2.0));
                locationX = (int)(((_width < realWidth ? _width : realWidth) / 2.0) - (finalWidth / 2.0));

                var rect = new Rectangle(locationX, locationY, finalWidth, finalHeigth);

                imageInfo.ThumbnailWidth = thumbnail.Width;
                imageInfo.ThumbnailHeight = thumbnail.Height;


                using var graphic = Graphics.FromImage(thumbnail);
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.DrawImage(image, rect);
            }

            if (Store == null)
                thumbnail.Save(outputPath, icJPG, ep);
            else
            {

                var ms = new MemoryStream();
                thumbnail.Save(ms, icJPG, ep);
                ms.Seek(0, SeekOrigin.Begin);
                Store.Save(outputPath, ms);
                ms.Dispose();
            }

            thumbnail.Dispose();
        }



        public void DoPreviewImage(string path, string outputPath, ref ImageInfo imageInfo)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            DoPreviewImage(fs, outputPath, ref imageInfo);
        }
        public void DoPreviewImage(Stream image, string outputPath, ref ImageInfo imageInfo)
        {
            using var img = Image.FromStream(image);
            DoPreviewImage(img, outputPath, ref imageInfo);
        }
        public void DoPreviewImage(Image image, string outputPath, ref ImageInfo imageInfo)
        {
            var realWidth = image.Width;
            var realHeight = image.Height;

            var heightPreview = realHeight;
            var widthPreview = realWidth;

            var ep = new EncoderParameters(1);
            var icJPG = GetCodecInfo("image/jpeg");
            ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)90);

            if (realWidth <= _widthPreview && realHeight <= _heightPreview)
            {
                imageInfo.PreviewWidth = widthPreview;
                imageInfo.PreviewHeight = heightPreview;

                if (Store == null)
                    image.Save(outputPath);
                else
                {

                    var ms = new MemoryStream();
                    image.Save(ms, icJPG, ep);
                    ms.Seek(0, SeekOrigin.Begin);
                    Store.Save(outputPath, ms);
                    ms.Dispose();
                }

                return;
            }
            else if (realHeight / (double)_heightPreview > realWidth / (double)_widthPreview)
            {
                if (heightPreview > _heightPreview)
                {
                    widthPreview = (int)(realWidth * _heightPreview * 1.0 / realHeight + 0.5);
                    heightPreview = _heightPreview;
                }
            }
            else
            {
                if (widthPreview > _widthPreview)
                {
                    heightPreview = (int)(realHeight * _widthPreview * 1.0 / realWidth + 0.5);
                    widthPreview = _widthPreview;
                }
            }

            imageInfo.PreviewWidth = widthPreview;
            imageInfo.PreviewHeight = heightPreview;

            var preview = new Bitmap(widthPreview, heightPreview);

            using (var graphic = Graphics.FromImage(preview))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.DrawImage(image, 0, 0, widthPreview, heightPreview);
            }

            if (Store == null)
                preview.Save(outputPath, icJPG, ep);
            else
            {

                var ms = new MemoryStream();
                preview.Save(ms, icJPG, ep);
                ms.Seek(0, SeekOrigin.Begin);
                Store.Save(outputPath, ms);
                ms.Dispose();
            }

            preview.Dispose();
        }

        public void RotateImage(string path, string outputPath, bool back)
        {
            try
            {
                using var stream = Store.GetReadStream(path);
                using var image = Image.FromStream(stream);
                if (back)
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                else
                {
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }

                var ep = new EncoderParameters(1);
                var icJPG = GetCodecInfo("image/jpeg");
                ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);

                if (Store == null)
                {
                    image.Save(outputPath, icJPG, ep);
                }
                else
                {
                    using var ms = new MemoryStream();
                    image.Save(ms, icJPG, ep);
                    ms.Seek(0, SeekOrigin.Begin);
                    Store.Save(outputPath, ms);
                }

                Store.Delete(path);
            }
            catch { }
        }

        private static ImageCodecInfo GetCodecInfo(string mt)
        {
            var ici = ImageCodecInfo.GetImageEncoders();
            var idx = 0;
            for (var ii = 0; ii < ici.Length; ii++)
            {
                if (ici[ii].MimeType == mt)
                {
                    idx = ii;
                    break;
                }
            }
            return ici[idx];
        }
    }

    public class ImageHelper
    {
        public const int maxSize = 200;
        public const int maxWidthPreview = 933;
        public const int maxHeightPreview = 700;

        public static void GenerateThumbnail(string path, string outputPath, ref ImageInfo imageInfo)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview);

            _generator.DoThumbnail(path, outputPath, ref imageInfo);
        }

        public static void GenerateThumbnail(string path, string outputPath, ref ImageInfo imageInfo, int maxWidth, int maxHeight)
        {
            var _generator = new ThumbnailGenerator(true,
                maxWidth,
                maxHeight,
                maxWidthPreview,
                maxHeightPreview);

            _generator.DoThumbnail(path, outputPath, ref imageInfo);
        }
        public static void GenerateThumbnail(Stream stream, string outputPath, ref ImageInfo imageInfo, int maxWidth, int maxHeight)
        {
            var _generator = new ThumbnailGenerator(true,
                maxWidth,
                maxHeight,
                maxWidthPreview,
                maxHeightPreview);

            _generator.DoThumbnail(stream, outputPath, ref imageInfo);
        }

        public static void GenerateThumbnail(string path, string outputPath, ref ImageInfo imageInfo, int maxWidth, int maxHeight, IDataStore store)
        {
            var _generator = new ThumbnailGenerator(true,
                maxWidth,
                maxHeight,
                maxWidthPreview,
                maxHeightPreview)
            {
                Store = store
            };

            _generator.DoThumbnail(path, outputPath, ref imageInfo);
        }
        public static void GenerateThumbnail(Stream stream, string outputPath, ref ImageInfo imageInfo, int maxWidth, int maxHeight, IDataStore store)
        {
            var _generator = new ThumbnailGenerator(true,
                maxWidth,
                maxHeight,
                maxWidthPreview,
                maxHeightPreview)
            {
                Store = store
            };

            _generator.DoThumbnail(stream, outputPath, ref imageInfo);
        }


        public static void GenerateThumbnail(Stream stream, string outputPath, ref ImageInfo imageInfo)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview);

            _generator.DoThumbnail(stream, outputPath, ref imageInfo);
        }

        public static void GenerateThumbnail(Stream stream, string outputPath, ref ImageInfo imageInfo, IDataStore store)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview)
            {
                Store = store
            };
            _generator.DoThumbnail(stream, outputPath, ref imageInfo);
        }


        public static void GeneratePreview(string path, string outputPath, ref ImageInfo imageInfo)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview);

            _generator.DoPreviewImage(path, outputPath, ref imageInfo);

        }
        public static void GeneratePreview(Stream stream, string outputPath, ref ImageInfo imageInfo)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview);

            _generator.DoPreviewImage(stream, outputPath, ref imageInfo);

        }
        public static void GeneratePreview(Stream stream, string outputPath, ref ImageInfo imageInfo, IDataStore store)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview)
            {
                Store = store
            };
            _generator.DoPreviewImage(stream, outputPath, ref imageInfo);

        }


        public static void RotateImage(string path, string outputPath, bool back, IDataStore store)
        {
            var _generator = new ThumbnailGenerator(true,
                maxSize,
                maxSize,
                maxWidthPreview,
                maxHeightPreview)
            {
                Store = store
            };

            _generator.RotateImage(path, outputPath, back);
        }
    }
}
*/