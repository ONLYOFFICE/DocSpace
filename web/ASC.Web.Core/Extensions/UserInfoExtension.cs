namespace ASC.Core.Users
{
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
            if (userInfoCollection == null) return new List<UserInfo>();

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
}