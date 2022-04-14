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

        var destRound = mainImg.Clone(x => x.Crop(rect).Resize(new ResizeOptions
        {
            Size = size
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
