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

namespace ASC.People.ApiModels.ResponseDto;

/// <summary>
/// </summary>
public class ThumbnailsDataDto
{
    private ThumbnailsDataDto() { }

    public static async Task<ThumbnailsDataDto> Create(UserInfo userInfo, UserPhotoManager userPhotoManager)
    {
        var cacheKey = Math.Abs(userInfo.LastModified.GetHashCode());

        return new ThumbnailsDataDto
        {
            Original = await userPhotoManager.GetPhotoAbsoluteWebPath(userInfo.Id) + $"?hash={cacheKey}",
            Retina = await userPhotoManager.GetRetinaPhotoURL(userInfo.Id) + $"?hash={cacheKey}",
            Max = await userPhotoManager.GetMaxPhotoURL(userInfo.Id) + $"?hash={cacheKey}",
            Big = await userPhotoManager.GetBigPhotoURL(userInfo.Id) + $"?hash={cacheKey}",
            Medium = await userPhotoManager.GetMediumPhotoURL(userInfo.Id) + $"?hash={cacheKey}",
            Small = await userPhotoManager.GetSmallPhotoURL(userInfo.Id) + $"?hash={cacheKey}"
        };
    }

    /// <summary>Original photo</summary>
    /// <type>System.String, System</type>
    public string Original { get; set; }

    /// <summary>Retina</summary>
    /// <type>System.String, System</type>
    public string Retina { get; set; }

    /// <summary>Maximum size</summary>
    /// <type>System.String, System</type>
    public string Max { get; set; }

    /// <summary>Big</summary>
    /// <type>System.String, System</type>
    public string Big { get; set; }

    /// <summary>Medium</summary>
    /// <type>System.String, System</type>
    public string Medium { get; set; }

    /// <summary>Small</summary>
    /// <type>System.String, System</type>
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
