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

namespace ASC.Web.Core.Users;

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
        if (thumbnailSettings.Size.IsEmpty)
        {
            return null;
        }

        var thumbnailsData = new ThumbnailsData(userId, userPhotoManager);

        var resultBitmaps = new List<ThumbnailItem>();

        using var img = thumbnailsData.MainImgBitmap(out var format);

        if (img == null)
        {
            return null;
        }

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
        var width = x + thumbnailSettings.Size.Width > mainImg.Width ? mainImg.Width : thumbnailSettings.Size.Width;
        var height = y + thumbnailSettings.Size.Height > mainImg.Height ? mainImg.Height : thumbnailSettings.Size.Height;

        var rect = new Rectangle(x,
                                 y,
                                 width,
                                 height);

            var result = mainImg.Clone(x => x.BackgroundColor(Color.White).Crop(rect).Resize(new ResizeOptions
        {
            Size = size
        }));

            return result;
    }

    public static void CheckImgFormat(byte[] data)
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
            throw new UnknownImageFormatException(error);
        }

        if (imgFormat.Name != "PNG" && imgFormat.Name != "JPEG")
        {
            throw new UnknownImageFormatException();
        }
    }

    public static byte[] TryParseImage(byte[] data, long maxFileSize, Size maxsize, out IImageFormat imgFormat, out int width, out int height)
    {
        if (data == null || data.Length <= 0)
        {
            throw new UnknownImageFormatException();
        }

        if (maxFileSize != -1 && data.Length > maxFileSize)
        {
            throw new ImageSizeLimitException();
        }

        //data = ImageHelper.RotateImageByExifOrientationData(data, Log);

        try
        {
            using var img = Image.Load(data, out var format);
            imgFormat = format;
            width = img.Width;
            height = img.Height;
            var maxWidth = maxsize.Width;
            var maxHeight = maxsize.Height;

            if ((maxHeight != -1 && img.Height > maxHeight) || (maxWidth != -1 && img.Width > maxWidth))
            {
                #region calulate height and width

                if (width > maxWidth && height > maxHeight)
                {

                    if (width > height)
                    {
                        height = (int)(height * (double)maxWidth / width + 0.5);
                        width = maxWidth;
                    }
                    else
                    {
                        width = (int)(width * (double)maxHeight / height + 0.5);
                        height = maxHeight;
                    }
                }

                if (width > maxWidth && height <= maxHeight)
                {
                    height = (int)(height * (double)maxWidth / width + 0.5);
                    width = maxWidth;
                }

                if (width <= maxWidth && height > maxHeight)
                {
                    width = (int)(width * (double)maxHeight / height + 0.5);
                    height = maxHeight;
                }

                var tmpW = width;
                var tmpH = height;
                #endregion
                using var destRound = img.Clone(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(tmpW, tmpH),
                    Mode = ResizeMode.Stretch
                }));

                data = CommonPhotoManager.SaveToBytes(destRound);
            }
            return data;
        }
        catch (OutOfMemoryException)
        {
            throw new ImageSizeLimitException();
        }
        catch (ArgumentException error)
        {
            throw new Users.UnknownImageFormatException(error);
        }
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
    private readonly Guid _userId;
    private readonly UserPhotoManager _userPhotoManager;

    public ThumbnailsData(Guid userId, UserPhotoManager userPhotoManager)
    {
        _userId = userId;
        _userPhotoManager = userPhotoManager;
    }

    public Image MainImgBitmap(out IImageFormat format)
    {
        var img = _userPhotoManager.GetPhotoImage(_userId, out var imageFormat);
        format = imageFormat;
        return img;
    }

    public string MainImgUrl()
    {
        return _userPhotoManager.GetPhotoAbsoluteWebPath(_userId);
    }

    public List<ThumbnailItem> ThumbnailList()
    {
        return new List<ThumbnailItem>
                {
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.RetinaFotoSize,
                            ImgUrl = _userPhotoManager.GetRetinaPhotoURL(_userId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.MaxFotoSize,
                            ImgUrl = _userPhotoManager.GetMaxPhotoURL(_userId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.BigFotoSize,
                            ImgUrl = _userPhotoManager.GetBigPhotoURL(_userId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.MediumFotoSize,
                            ImgUrl = _userPhotoManager.GetMediumPhotoURL(_userId)
                        },
                    new ThumbnailItem
                        {
                            Size = UserPhotoManager.SmallFotoSize,
                            ImgUrl = _userPhotoManager.GetSmallPhotoURL(_userId)
                        }
            };
    }

    public void Save(List<ThumbnailItem> bitmaps)
    {
        foreach (var item in bitmaps)
        {
            using var mainImgBitmap = MainImgBitmap(out var format);
            _userPhotoManager.SaveThumbnail(_userId, item.Image, format);
        }
    }
}
