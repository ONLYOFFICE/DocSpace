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


using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace ASC.Web.Core
{
    public class CommonPhotoManager
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
            
            var alignWidth = true;
            if (crop) alignWidth = (minSide == realWidth);
            else alignWidth = (maxSide == realWidth);

            var scaleFactor = (alignWidth) ? (realWidth / (1.0 * width)) : (realHeight / (1.0 * height));

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
