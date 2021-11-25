/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;

using ASC.Core.Common.Settings;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ASC.Web.Core.Users
{
    public class UserPhotoThumbnailManager
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
            var rect = new Rectangle(x,
                                     y,
                                     thumbnailSettings.Size.Width,
                                     thumbnailSettings.Size.Height);

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