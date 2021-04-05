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


using ASC.Api.Core;
using ASC.Common;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Api.Models
{
    public class EmployeeWraper
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Title { get; set; }

        public string AvatarSmall { get; set; }

        public string ProfileUrl { get; set; }

        public static EmployeeWraper GetSample()
        {
            return new EmployeeWraper
            {
                Id = Guid.Empty,
                DisplayName = "Mike Zanyatski",
                Title = "Manager",
                AvatarSmall = "url to small avatar",
            };
        }
    }

    [Scope]
    public class EmployeeWraperHelper
    {
        private ApiContext HttpContext { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        protected UserPhotoManager UserPhotoManager { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        protected UserManager UserManager { get; }

        public EmployeeWraperHelper(
            ApiContext httpContext,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            UserPhotoManager userPhotoManager,
            CommonLinkUtility commonLinkUtility,
            UserManager userManager)
        {
            HttpContext = httpContext;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            UserPhotoManager = userPhotoManager;
            CommonLinkUtility = commonLinkUtility;
            UserManager = userManager;
        }

        public EmployeeWraper Get(UserInfo userInfo)
        {
            return Init(new EmployeeWraper(), userInfo);
        }

        public EmployeeWraper Get(Guid userId)
        {
            try
            {
                return Get(UserManager.GetUsers(userId));
            }
            catch (Exception)
            {
                return Get(Constants.LostUser);
            }
        }

        protected EmployeeWraper Init(EmployeeWraper result, UserInfo userInfo)
        {
            result.Id = userInfo.ID;
            result.DisplayName = DisplayUserSettingsHelper.GetFullUserName(userInfo);
            if (!string.IsNullOrEmpty(userInfo.Title))
            {
                result.Title = userInfo.Title;
            }

            var userInfoLM = userInfo.LastModified.GetHashCode();

            if (HttpContext.Check("avatarSmall"))
            {
                result.AvatarSmall = UserPhotoManager.GetSmallPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (result.Id != Guid.Empty)
            {
                var profileUrl = CommonLinkUtility.GetUserProfile(userInfo, false);
                result.ProfileUrl = CommonLinkUtility.GetFullAbsolutePath(profileUrl);
            }

            return result;
        }
    }
}