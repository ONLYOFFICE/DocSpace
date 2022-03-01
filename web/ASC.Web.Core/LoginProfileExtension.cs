namespace ASC.Web.Core
{
    public static class LoginProfileExtension
    {
        public static UserInfo ProfileToUserInfo(this LoginProfile loginProfile, CoreBaseSettings coreBaseSettings)
        {
            if (string.IsNullOrEmpty(loginProfile.EMail)) throw new Exception(Resource.ErrorNotCorrectEmail);

            var firstName = loginProfile.FirstName;
            if (string.IsNullOrEmpty(firstName)) firstName = loginProfile.DisplayName;

            var userInfo = new UserInfo
            {
                FirstName = string.IsNullOrEmpty(firstName) ? UserControlsCommonResource.UnknownFirstName : firstName,
                LastName = string.IsNullOrEmpty(loginProfile.LastName) ? UserControlsCommonResource.UnknownLastName : loginProfile.LastName,
                Email = loginProfile.EMail,
                Title = string.Empty,
                Location = string.Empty,
                CultureName = coreBaseSettings.CustomMode ? "ru-RU" : Thread.CurrentThread.CurrentUICulture.Name,
                ActivationStatus = EmployeeActivationStatus.Activated,
            };

            var gender = loginProfile.Gender;
            if (!string.IsNullOrEmpty(gender))
            {
                userInfo.Sex = gender == "male";
            }

            return userInfo;
        }
    }
}
