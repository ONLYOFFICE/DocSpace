using System;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Api.Models
{
    public class EmployeeWraper
    {
        protected EmployeeWraper()
        {
        }

        public EmployeeWraper(UserInfo userInfo)
        {
            Id = userInfo.ID;
            DisplayName = "";
            if (!string.IsNullOrEmpty(userInfo.Title))
            {
                Title = userInfo.Title;
            }

            //if (EmployeeWraperFull.CheckContext(context, "avatarSmall"))
            //{
            //    AvatarSmall = UserPhotoManager.GetSmallPhotoURL(userInfo.ID) + "?_=" + userInfo.LastModified.GetHashCode();
            //}
        }

        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Title { get; set; }

        public string AvatarSmall { get; set; }

        public string ProfileUrl
        {
            get
            {
                //if (Id == Guid.Empty) return string.Empty;
                //var profileUrl = CommonLinkUtility.GetUserProfile(Id.ToString(), false);
                return string.Empty;
            }
        }

        public static EmployeeWraper Get(Guid userId)
        {
            try
            {
                return Get(CoreContext.UserManager.GetUsers(userId));
            }
            catch (Exception)
            {
                return Get(Constants.LostUser);
            }
        }

        public static EmployeeWraper Get(UserInfo userInfo)
        {
            return new EmployeeWraper(userInfo);
        }

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
}
