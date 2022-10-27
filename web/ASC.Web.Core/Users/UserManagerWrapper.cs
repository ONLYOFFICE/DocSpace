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

namespace ASC.Web.Core.Users;

/// <summary>
/// Web studio user manager helper
/// </summary>
/// 
[Scope]
public sealed class UserManagerWrapper
{
    private readonly StudioNotifyService _studioNotifyService;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly MessageService _messageService;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly TenantUtil _tenantUtil;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IPSecurity.IPSecurity _iPSecurity;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly SettingsManager _settingsManager;
    private readonly UserFormatter _userFormatter;
    private readonly CountRoomAdminChecker _countManagerChecker;

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
        UserFormatter userFormatter,
        CountRoomAdminChecker countManagerChecker)
    {
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _securityContext = securityContext;
        _messageService = messageService;
        _customNamingPeople = customNamingPeople;
        _tenantUtil = tenantUtil;
        _coreBaseSettings = coreBaseSettings;
        _iPSecurity = iPSecurity;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _settingsManager = settingsManager;
        _userFormatter = userFormatter;
        _countManagerChecker = countManagerChecker;
    }

    private bool TestUniqueUserName(string uniqueName)
    {
        if (string.IsNullOrEmpty(uniqueName))
        {
            return false;
        }

        return Equals(_userManager.GetUserByUserName(uniqueName), Constants.LostUser);
    }

    private string MakeUniqueName(UserInfo userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.Email))
        {
            throw new ArgumentException(Resource.ErrorEmailEmpty, nameof(userInfo));
        }

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
        var foundUser = _userManager.GetUserByEmail(email);
        return Equals(foundUser, Constants.LostUser) || foundUser.Id == userId;
    }

    public async Task<UserInfo> AddInvitedUserAsync(string email, EmployeeType type)
    {
        var mail = new MailAddress(email);

        if (_userManager.GetUserByEmail(mail.Address).Id != Constants.LostUser.Id)
        {
            throw new InvalidOperationException($"User with email {mail.Address} already exists or is invited");
        }

        if (type is EmployeeType.User or EmployeeType.DocSpaceAdmin)
        {
            await _countManagerChecker.CheckAppend();
        }

        var user = new UserInfo
        {
            Email = mail.Address,
            UserName = mail.User,
            LastName = string.Empty,
            FirstName = string.Empty,
            ActivationStatus = EmployeeActivationStatus.Pending,
            Status = EmployeeStatus.Active,
        };

        var newUser = _userManager.SaveUserInfo(user);

        var groupId = type switch
        {
            EmployeeType.User => Constants.GroupUser.ID,
            EmployeeType.DocSpaceAdmin => Constants.GroupAdmin.ID,
            _ => Guid.Empty,
        };

        if (groupId != Guid.Empty)
        {
            _userManager.AddUserIntoGroup(newUser.Id, groupId);
        }

        return newUser;
    }

    public UserInfo AddUser(UserInfo userInfo, string passwordHash, bool afterInvite = false, bool notify = true, bool isUser = false, bool fromInviteLink = false, bool makeUniqueName = true, bool isCardDav = false, 
        bool updateExising = false, bool isAdmin = false)
    {
        ArgumentNullException.ThrowIfNull(userInfo);

        if (!_userFormatter.IsValidUserName(userInfo.FirstName, userInfo.LastName))
        {
            throw new Exception(Resource.ErrorIncorrectUserName);
        }

        if (!updateExising && !CheckUniqueEmail(userInfo.Id, userInfo.Email))
        {
            throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
        }

        if (makeUniqueName)
        {
            userInfo.UserName = MakeUniqueName(userInfo);
        }
        if (!userInfo.WorkFromDate.HasValue)
        {
            userInfo.WorkFromDate = _tenantUtil.DateTimeNow();
        }

        if (!_coreBaseSettings.Personal && (!fromInviteLink || updateExising))
        {
            userInfo.ActivationStatus = !afterInvite ? EmployeeActivationStatus.Pending : EmployeeActivationStatus.Activated;
        }

        var newUserInfo = _userManager.SaveUserInfo(userInfo, isUser, isCardDav);
        _securityContext.SetUserPasswordHash(newUserInfo.Id, passwordHash);

        if (_coreBaseSettings.Personal)
        {
            _studioNotifyService.SendUserWelcomePersonal(newUserInfo);
            return newUserInfo;
        }

        if ((newUserInfo.Status & EmployeeStatus.Active) == EmployeeStatus.Active && notify)
        {
            //NOTE: Notify user only if it's active
            if (afterInvite)
            {
                if (isUser)
                {
                    _studioNotifyService.GuestInfoAddedAfterInvite(newUserInfo);
                }
                else
                {
                    _studioNotifyService.UserInfoAddedAfterInvite(newUserInfo);
                }

                if (fromInviteLink)
                {
                    _studioNotifyService.SendEmailActivationInstructions(newUserInfo, newUserInfo.Email);
                }
            }
            else
            {
                //Send user invite
                if (isUser)
                {
                    _studioNotifyService.GuestInfoActivation(newUserInfo);
                }
                else
                {
                    _studioNotifyService.UserInfoActivation(newUserInfo);
                }

            }
        }

        if (isUser)
        {
            _userManager.AddUserIntoGroup(newUserInfo.Id, Constants.GroupUser.ID);
        }
        else if (isAdmin)
        {
            _userManager.AddUserIntoGroup(newUserInfo.Id, Constants.GroupAdmin.ID);
        }

        return newUserInfo;
    }

    #region Password

    public void CheckPasswordPolicy(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception(Resource.ErrorPasswordEmpty);
        }

        var passwordSettingsObj = _settingsManager.Load<PasswordSettings>();

        if (!CheckPasswordRegex(passwordSettingsObj, password))
        {
            throw new Exception(GetPasswordHelpMessage(passwordSettingsObj));
        }
    }

    public string GetPasswordRegex(PasswordSettings passwordSettings)
    {
        var pwdBuilder = new StringBuilder("^");

        if (passwordSettings.Digits)
        {
            pwdBuilder.Append(passwordSettings.DigitsRegexStr);
        }

        if (passwordSettings.UpperCase)
        {
            pwdBuilder.Append(passwordSettings.UpperCaseRegexStr);
        }

        if (passwordSettings.SpecSymbols)
        {
            pwdBuilder.Append(passwordSettings.SpecSymbolsRegexStr);
        }

        pwdBuilder.Append($"{passwordSettings.AllowedCharactersRegexStr}{{{passwordSettings.MinLength},{PasswordSettings.MaxLength}}}$");

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
        if (!email.TestEmailRegex())
        {
            throw new ArgumentNullException(nameof(email), Resource.ErrorNotCorrectEmail);
        }

        var settings = _settingsManager.Load<IPRestrictionsSettings>();

        if (settings.Enable && !_iPSecurity.Verify())
        {
            throw new Exception(Resource.ErrorAccessRestricted);
        }

        var userInfo = _userManager.GetUserByEmail(email);
        if (!_userManager.UserExists(userInfo) || string.IsNullOrEmpty(userInfo.Email))
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

        _studioNotifyService.UserPasswordChange(userInfo);

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

    public static string GetPasswordHelpMessage(PasswordSettings passwordSettings)
    {
        var text = new StringBuilder();

        text.AppendFormat("{0} ", Resource.ErrorPasswordMessage);
        text.AppendFormat(Resource.ErrorPasswordLength, passwordSettings.MinLength, PasswordSettings.MaxLength);
        text.AppendFormat(", {0}", Resource.ErrorPasswordOnlyLatinLetters);
        text.AppendFormat(", {0}", Resource.ErrorPasswordNoSpaces);

        if (passwordSettings.UpperCase)
        {
            text.AppendFormat(", {0}", Resource.ErrorPasswordNoUpperCase);
        }

        if (passwordSettings.Digits)
        {
            text.AppendFormat(", {0}", Resource.ErrorPasswordNoDigits);
        }

        if (passwordSettings.SpecSymbols)
        {
            text.AppendFormat(", {0}", Resource.ErrorPasswordNoSpecialSymbols);
        }

        return text.ToString();
    }

    public string GetPasswordHelpMessage()
    {
        return GetPasswordHelpMessage(_settingsManager.Load<PasswordSettings>());
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
