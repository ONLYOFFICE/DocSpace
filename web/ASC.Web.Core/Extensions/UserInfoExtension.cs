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

namespace ASC.Core.Users;

public static class UserInfoExtension
{
    public static string DisplayUserName(this UserInfo userInfo, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        return DisplayUserName(userInfo, true, displayUserSettingsHelper);
    }

    public static string DisplayUserName(this UserInfo userInfo, bool withHtmlEncode, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        return displayUserSettingsHelper.GetFullUserName(userInfo, withHtmlEncode);
    }

    public static List<UserInfo> SortByUserName(this IEnumerable<UserInfo> userInfoCollection)
    {
        if (userInfoCollection == null)
        {
            return new List<UserInfo>();
        }

        var users = new List<UserInfo>(userInfoCollection);
        users.Sort(UserInfoComparer.Default);
        return users;
    }

    public static bool HasAvatar(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.UserHasAvatar(userInfo.Id);
    }

    public static Size GetPhotoSize(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetPhotoSize(userInfo.Id);
    }

    public static string GetPhotoURL(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetPhotoAbsoluteWebPath(userInfo.Id);
    }

    public static string GetRetinaPhotoURL(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetRetinaPhotoURL(userInfo.Id);
    }

    public static string GetMaxPhotoURL(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetMaxPhotoURL(userInfo.Id);
    }

    public static string GetBigPhotoURL(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetBigPhotoURL(userInfo.Id);
    }

    public static string GetMediumPhotoURL(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetMediumPhotoURL(userInfo.Id);
    }

    public static string GetSmallPhotoURL(this UserInfo userInfo, UserPhotoManager UserPhotoManager)
    {
        return UserPhotoManager.GetSmallPhotoURL(userInfo.Id);
    }

    public static string RenderProfileLinkBase(this UserInfo userInfo, CommonLinkUtility commonLinkUtility, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        var sb = new StringBuilder();

        //check for removed users
        if (userInfo.Id == Constants.LostUser.Id)
        {
            sb.Append($"<span class='userLink text-medium-describe' style='white-space:nowrap;'>{userInfo.DisplayUserName(displayUserSettingsHelper)}</span>");
        }
        else
        {
            var popupID = Guid.NewGuid();
            sb.Append($"<span class=\"userLink\" style='white-space:nowrap;' id='{popupID}' data-uid='{userInfo.Id}'>");
            sb.Append($"<a class='linkDescribe' href=\"{userInfo.GetUserProfilePageURLGeneral(commonLinkUtility)}\">{userInfo.DisplayUserName(displayUserSettingsHelper)}</a>");
            sb.Append("</span>");

            sb.AppendFormat("<script language='javascript'> StudioUserProfileInfo.RegistryElement('{0}','\"{1}\"); </script>", popupID, userInfo.Id);
        }
        return sb.ToString();
    }

    /// <summary>
    /// return absolute profile link
    /// </summary>
    /// <param name="userInfo"></param>        
    /// <returns></returns>
    private static string GetUserProfilePageURLGeneral(this UserInfo userInfo, CommonLinkUtility commonLinkUtility)
    {
        return commonLinkUtility.GetUserProfile(userInfo);
    }
}
