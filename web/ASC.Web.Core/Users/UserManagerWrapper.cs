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

using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Core.Users
{
    /// <summary>
    /// Web studio user manager helper
    /// </summary>
    /// 
    [Scope]
    public sealed class UserManagerWrapper
    {
        private StudioNotifyService StudioNotifyService { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private MessageService MessageService { get; }
        private CustomNamingPeople CustomNamingPeople { get; }
        private TenantUtil TenantUtil { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private IPSecurity.IPSecurity IPSecurity { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }
        private UserFormatter UserFormatter { get; }

        public UserManagerWrapper(
            StudioNotifyService studioNotifyService,
            UserManager userManager,
            SecurityContext securityContext,
            MessageService messageService,
            CustomNamingPeople customNamingPeople,
            TenantUtil tenantUtil,
            CoreBaseSettings coreBaseSettings,
            IPSecurity.IPSecurity iPSecurity,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            SettingsManager settingsManager,
            UserFormatter userFormatter)
        {
            StudioNotifyService = studioNotifyService;
            UserManager = userManager;
            SecurityContext = securityContext;
            MessageService = messageService;
            CustomNamingPeople = customNamingPeople;
            TenantUtil = tenantUtil;
            CoreBaseSettings = coreBaseSettings;
            IPSecurity = iPSecurity;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            SettingsManager = settingsManager;
            UserFormatter = userFormatter;
        }

        private bool TestUniqueUserName(string uniqueName)
        {
            if (string.IsNullOrEmpty(uniqueName))
                return false;
            return Equals(UserManager.GetUserByUserName(uniqueName), Constants.LostUser);
        }

        private string MakeUniqueName(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.Email))
                throw new ArgumentException(Resource.ErrorEmailEmpty, nameof(userInfo));

            var uniqueName = new MailAddress(userInfo.Email).User;
            var startUniqueName = uniqueName;
            var i = 0;
            while (!TestUniqueUserName(uniqueName))
            {
                uniqueName = $"{startUniqueName}{(++i).ToString(CultureInfo.InvariantCulture)}";
            }
            return uniqueName;
        }

        public bool CheckUniqueEmail(Guid userId, string email)
        {
            var foundUser = UserManager.GetUserByEmail(email);
            return Equals(foundUser, Constants.LostUser) || foundUser.Id == userId;
        }

        public UserInfo AddUser(UserInfo userInfo, string passwordHash, bool afterInvite = false, bool notify = true, bool isVisitor = false, bool fromInviteLink = false, bool makeUniqueName = true)
        {
            ArgumentNullException.ThrowIfNull(userInfo);

            if (!UserFormatter.IsValidUserName(userInfo.FirstName, userInfo.LastName))
                throw new Exception(Resource.ErrorIncorrectUserName);

            if (!CheckUniqueEmail(userInfo.Id, userInfo.Email))
                throw new Exception(CustomNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
            if (makeUniqueName)
            {
                userInfo.UserName = MakeUniqueName(userInfo);
            }
            if (!userInfo.WorkFromDate.HasValue)
            {
                userInfo.WorkFromDate = TenantUtil.DateTimeNow();
            }

            if (!CoreBaseSettings.Personal && !fromInviteLink)
            {
                userInfo.ActivationStatus = !afterInvite ? EmployeeActivationStatus.Pending : EmployeeActivationStatus.Activated;
            }

            var newUserInfo = UserManager.SaveUserInfo(userInfo);
            SecurityContext.SetUserPasswordHash(newUserInfo.Id, passwordHash);

            if (CoreBaseSettings.Personal)
            {
                StudioNotifyService.SendUserWelcomePersonal(newUserInfo);
                return newUserInfo;
            }

            if ((newUserInfo.Status & EmployeeStatus.Active) == EmployeeStatus.Active && notify)
            {
                //NOTE: Notify user only if it's active
                if (afterInvite)
                {
                    if (isVisitor)
                    {
                        StudioNotifyService.GuestInfoAddedAfterInvite(newUserInfo);
                    }
                    else
                    {
                        StudioNotifyService.UserInfoAddedAfterInvite(newUserInfo);
                    }

                    if (fromInviteLink)
                    {
                        StudioNotifyService.SendEmailActivationInstructions(newUserInfo, newUserInfo.Email);
                    }
                }
                else
                {
                    //Send user invite
                    if (isVisitor)
                    {
                        StudioNotifyService.GuestInfoActivation(newUserInfo);
                    }
                    else
                    {
                        StudioNotifyService.UserInfoActivation(newUserInfo);
                    }

                }
            }

            if (isVisitor)
            {
                UserManager.AddUserIntoGroup(newUserInfo.Id, Constants.GroupVisitor.ID);
            }

            return newUserInfo;
        }

        #region Password

        public void CheckPasswordPolicy(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception(Resource.ErrorPasswordEmpty);

            var passwordSettingsObj = SettingsManager.Load<PasswordSettings>();

            if (!CheckPasswordRegex(passwordSettingsObj, password))
                throw new Exception(GenerateErrorMessage(passwordSettingsObj));
        }

        public string GetPasswordRegex(PasswordSettings passwordSettings)
        {
            var pwdBuilder = new StringBuilder();

            if (CoreBaseSettings.CustomMode)
            {
                pwdBuilder.Append(@"^(?=.*[a-z]{0,})");

                if (passwordSettings.Digits)
                    pwdBuilder.Append(@"(?=.*\d)");

                if (passwordSettings.UpperCase)
                    pwdBuilder.Append(@"(?=.*[A-Z])");

                if (passwordSettings.SpecSymbols)
                    pwdBuilder.Append(@"(?=.*[_\-.~!$^*()=|])");

                pwdBuilder.Append(@"[0-9a-zA-Z_\-.~!$^*()=|]");
            }
            else
            {
                pwdBuilder.Append(@"^(?=.*\p{Ll}{0,})");

                if (passwordSettings.Digits)
                    pwdBuilder.Append(@"(?=.*\d)");

                if (passwordSettings.UpperCase)
                    pwdBuilder.Append(@"(?=.*\p{Lu})");

                if (passwordSettings.SpecSymbols)
                    pwdBuilder.Append(@"(?=.*[\W])");

                pwdBuilder.Append('.');
            }

            pwdBuilder.Append('{');
            pwdBuilder.Append(passwordSettings.MinLength);
            pwdBuilder.Append(',');
            pwdBuilder.Append(PasswordSettings.MaxLength);
            pwdBuilder.Append(@"}$");

            return pwdBuilder.ToString();
        }

        public bool CheckPasswordRegex(PasswordSettings passwordSettings, string password)
        {
            var passwordRegex = GetPasswordRegex(passwordSettings);

            return new Regex(passwordRegex).IsMatch(password);
        }

        public string SendUserPassword(string email)
        {
            email = (email ?? "").Trim();
            if (!email.TestEmailRegex()) throw new ArgumentNullException(nameof(email), Resource.ErrorNotCorrectEmail);

            if (!IPSecurity.Verify())
            {
                throw new Exception(Resource.ErrorAccessRestricted);
            }

            var userInfo = UserManager.GetUserByEmail(email);
            if (!UserManager.UserExists(userInfo) || string.IsNullOrEmpty(userInfo.Email))
            {
                return string.Format(Resource.ErrorUserNotFoundByEmail, email);
            }
            if (userInfo.Status == EmployeeStatus.Terminated)
            {
                return Resource.ErrorDisabledProfile;
            }
            if (userInfo.IsLDAP())
            {
                return Resource.CouldNotRecoverPasswordForLdapUser;
            }
            if (userInfo.IsSSO())
            {
                return Resource.CouldNotRecoverPasswordForSsoUser;
            }

            StudioNotifyService.UserPasswordChange(userInfo);

            var displayUserName = userInfo.DisplayUserName(false, DisplayUserSettingsHelper);
            MessageService.Send(MessageAction.UserSentPasswordChangeInstructions, displayUserName);

            return null;
        }

        public static string GeneratePassword()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GeneratePassword(int minLength, int maxLength, string noise)
        {
            var length = RandomNumberGenerator.GetInt32(minLength, maxLength + 1);

            var sb = new StringBuilder();
            while (length-- > 0)
            {
                sb.Append(noise[RandomNumberGenerator.GetInt32(noise.Length - 1)]);
            }
            return sb.ToString();
        }

        internal static string GenerateErrorMessage(PasswordSettings passwordSettings)
        {
            var error = new StringBuilder();

            error.Append($"{Resource.ErrorPasswordMessage} ");
            error.AppendFormat(Resource.ErrorPasswordLength, passwordSettings.MinLength, PasswordSettings.MaxLength);
            if (passwordSettings.UpperCase)
                error.AppendFormat($", {Resource.ErrorPasswordNoUpperCase}");
            if (passwordSettings.Digits)
                error.Append($", {Resource.ErrorPasswordNoDigits}");
            if (passwordSettings.SpecSymbols)
                error.Append($", {Resource.ErrorPasswordNoSpecialSymbols}");

            return error.ToString();
        }

        public string GetPasswordHelpMessage()
        {
            var info = new StringBuilder();
            var passwordSettings = SettingsManager.Load<PasswordSettings>();
            info.Append($"{Resource.ErrorPasswordMessageStart} ");
            info.AppendFormat(Resource.ErrorPasswordLength, passwordSettings.MinLength, PasswordSettings.MaxLength);
            if (passwordSettings.UpperCase)
                info.Append($", {Resource.ErrorPasswordNoUpperCase}");
            if (passwordSettings.Digits)
                info.Append($", {Resource.ErrorPasswordNoDigits}");
            if (passwordSettings.SpecSymbols)
                info.Append($", {Resource.ErrorPasswordNoSpecialSymbols}");

            return info.ToString();
        }

        #endregion

        public static bool ValidateEmail(string email)
        {
            const string pattern = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                   + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                   + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
            return new Regex(pattern, options).IsMatch(email);
        }
    }
}