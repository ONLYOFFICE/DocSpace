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

namespace ASC.Web.Core;

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

        if (scaleFactor < 1)
        {
            scaleFactor = 1;
        }

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
                thumbnail.Mutate(x => x.BackgroundColor(Color.White));
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
                thumbnail.Mutate(x => x.BackgroundColor(Color.White));
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
