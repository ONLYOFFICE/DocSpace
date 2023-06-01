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
    private Tenant Tenant => _tenantManager.GetCurrentTenant();
    
    private readonly StudioNotifyService _studioNotifyService;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly TenantUtil _tenantUtil;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IPSecurity.IPSecurity _iPSecurity;
    private readonly SettingsManager _settingsManager;
    private readonly UserFormatter _userFormatter;
    private readonly CountPaidUserChecker _countPaidUserChecker;
    private readonly TenantManager _tenantManager;
    private readonly WebItemSecurityCache _webItemSecurityCache;
    private readonly QuotaSocketManager _quotaSocketManager;
    private readonly TenantQuotaFeatureStatHelper _tenantQuotaFeatureStatHelper;

    public UserManagerWrapper(
        StudioNotifyService studioNotifyService,
        UserManager userManager,
        SecurityContext securityContext,
        CustomNamingPeople customNamingPeople,
        TenantUtil tenantUtil,
        CoreBaseSettings coreBaseSettings,
        IPSecurity.IPSecurity iPSecurity,
        SettingsManager settingsManager,
        UserFormatter userFormatter,
        CountPaidUserChecker countPaidUserChecker,
        TenantManager tenantManager,
        WebItemSecurityCache webItemSecurityCache,
        QuotaSocketManager quotaSocketManager,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper)
    {
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _securityContext = securityContext;
        _customNamingPeople = customNamingPeople;
        _tenantUtil = tenantUtil;
        _coreBaseSettings = coreBaseSettings;
        _iPSecurity = iPSecurity;
        _settingsManager = settingsManager;
        _userFormatter = userFormatter;
        _countPaidUserChecker = countPaidUserChecker;
        _tenantManager = tenantManager;
        _webItemSecurityCache = webItemSecurityCache;
        _quotaSocketManager = quotaSocketManager;
        _tenantQuotaFeatureStatHelper = tenantQuotaFeatureStatHelper;
    }

    private async Task<bool> TestUniqueUserNameAsync(string uniqueName)
    {
        if (string.IsNullOrEmpty(uniqueName))
        {
            return false;
        }

        return Equals(await _userManager.GetUserByUserNameAsync(uniqueName), Constants.LostUser);
    }

    private async Task<string> MakeUniqueNameAsync(UserInfo userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.Email))
        {
            throw new ArgumentException(Resource.ErrorEmailEmpty, nameof(userInfo));
        }

        var uniqueName = new MailAddress(userInfo.Email).User;
        var startUniqueName = uniqueName;
        var i = 0;
        while (!await TestUniqueUserNameAsync(uniqueName))
        {
            uniqueName = $"{startUniqueName}{(++i).ToString(CultureInfo.InvariantCulture)}";
        }
        return uniqueName;
    }

    public async Task<bool> CheckUniqueEmailAsync(Guid userId, string email)
    {
        var foundUser = await _userManager.GetUserByEmailAsync(email);
        return Equals(foundUser, Constants.LostUser) || foundUser.Id == userId;
    }

    public async Task<UserInfo> AddInvitedUserAsync(string email, EmployeeType type)
    {
        var mail = new MailAddress(email);

        if ((await _userManager.GetUserByEmailAsync(mail.Address)).Id != Constants.LostUser.Id)
        {
            throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
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

        user.UserName = await MakeUniqueNameAsync(user);

        var newUser = await _userManager.SaveUserInfo(user, type);

        var groupId = type switch
        {
            EmployeeType.User => Constants.GroupUser.ID,
            EmployeeType.DocSpaceAdmin => Constants.GroupAdmin.ID,
            EmployeeType.Collaborator => Constants.GroupCollaborator.ID,
            _ => Guid.Empty,
        };

        if (groupId != Guid.Empty)
        {
            await _userManager.AddUserIntoGroupAsync(newUser.Id, groupId, true);
        }
        else if(type == EmployeeType.RoomAdmin)
        {
            var (name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<CountPaidUserFeature, int>();
            _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
        }

        return newUser;
    }

    public async Task<UserInfo> AddUserAsync(UserInfo userInfo, string passwordHash, bool afterInvite = false, bool notify = true, EmployeeType type = EmployeeType.RoomAdmin, bool fromInviteLink = false, bool makeUniqueName = true, bool isCardDav = false,
        bool updateExising = false)
    {
        ArgumentNullException.ThrowIfNull(userInfo);

        if (!_userFormatter.IsValidUserName(userInfo.FirstName, userInfo.LastName))
        {
            throw new Exception(Resource.ErrorIncorrectUserName);
        }

        if (!updateExising && !await CheckUniqueEmailAsync(userInfo.Id, userInfo.Email))
        {
            throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
        }

        if (makeUniqueName)
        {
            userInfo.UserName = await MakeUniqueNameAsync(userInfo);
        }
        if (!userInfo.WorkFromDate.HasValue)
        {
            userInfo.WorkFromDate = _tenantUtil.DateTimeNow();
        }

        if (!_coreBaseSettings.Personal && (!fromInviteLink || updateExising))
        {
            userInfo.ActivationStatus = !afterInvite ? EmployeeActivationStatus.Pending : EmployeeActivationStatus.Activated;
        }

        var newUserInfo = await _userManager.SaveUserInfo(userInfo, type, isCardDav, !updateExising);
        await _securityContext.SetUserPasswordHashAsync(newUserInfo.Id, passwordHash);

        if (_coreBaseSettings.Personal)
        {
            await _studioNotifyService.SendUserWelcomePersonalAsync(newUserInfo);
            return newUserInfo;
        }

        if ((newUserInfo.Status & EmployeeStatus.Active) == EmployeeStatus.Active && notify)
        {
            //NOTE: Notify user only if it's active
            if (afterInvite)
            {
                if (type is EmployeeType.User)
                {
                    await _studioNotifyService.GuestInfoAddedAfterInviteAsync(newUserInfo);
                }
                else
                {
                    await _studioNotifyService.UserInfoAddedAfterInviteAsync(newUserInfo);
                }

                if (fromInviteLink && newUserInfo.ActivationStatus != EmployeeActivationStatus.Activated)
                {
                    await _studioNotifyService.SendEmailActivationInstructionsAsync(newUserInfo, newUserInfo.Email);
                }
            }
            else
            {
                //Send user invite
                if (type is EmployeeType.User)
                {
                    await _studioNotifyService.GuestInfoActivationAsync(newUserInfo);
                }
                else
                {
                    await _studioNotifyService.UserInfoActivationAsync(newUserInfo);
                }

            }
        }

        if (updateExising)
        {
            return newUserInfo;
        }

        switch (type)
        {
            case EmployeeType.User:
                await _userManager.AddUserIntoGroupAsync(newUserInfo.Id, Constants.GroupUser.ID, true);
                break;
            case EmployeeType.DocSpaceAdmin:
                await _userManager.AddUserIntoGroupAsync(newUserInfo.Id, Constants.GroupAdmin.ID, true);
                break;
            case EmployeeType.Collaborator:
                await _userManager.AddUserIntoGroupAsync(newUserInfo.Id, Constants.GroupCollaborator.ID, true);
                break;
        }

        return newUserInfo;
    }

    #region Password

    public async Task<bool> UpdateUserTypeAsync(UserInfo user, EmployeeType type)
    {
        var currentUser = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        var changed = false;

        if (user.IsOwner(Tenant) || user.IsMe(currentUser.Id))
        {
            return await Task.FromResult(false);
        }
        
        var currentType = await _userManager.GetUserTypeAsync(user.Id);
        
        if (type is EmployeeType.DocSpaceAdmin && currentUser.IsOwner(Tenant))
        {
            if (currentType is EmployeeType.RoomAdmin)
            {
                await _userManager.AddUserIntoGroupAsync(user.Id, Constants.GroupAdmin.ID, notifyWebSocket: false);
                _webItemSecurityCache.ClearCache(Tenant.Id);
                changed = true;
            }
            else if (currentType is EmployeeType.Collaborator)
            {
                await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupCollaborator.ID);
                await _userManager.AddUserIntoGroupAsync(user.Id, Constants.GroupAdmin.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
                changed = true;
            }
            else if (currentType is EmployeeType.User)
            {
                await _countPaidUserChecker.CheckAppend();
                await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupUser.ID);
                await _userManager.AddUserIntoGroupAsync(user.Id, Constants.GroupAdmin.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
                changed = true;
            }
        }
        else if (type is EmployeeType.RoomAdmin)
        {
            if (currentType is EmployeeType.DocSpaceAdmin && currentUser.IsOwner(Tenant))
            {
                await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupAdmin.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
                changed = true;
            }
            else if (currentType is EmployeeType.Collaborator)
            {
                await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupCollaborator.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
                changed = true;
            }
            else if (currentType is EmployeeType.User)
            {
                await _countPaidUserChecker.CheckAppend();
                await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupUser.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
                changed = true;
            }
        }
        else if (type is EmployeeType.Collaborator && currentType is EmployeeType.User)
        {
            await _countPaidUserChecker.CheckAppend();
            await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupUser.ID);
            await _userManager.AddUserIntoGroupAsync(user.Id, Constants.GroupCollaborator.ID);
            _webItemSecurityCache.ClearCache(Tenant.Id);
            changed = true;
        }

        return changed;
    }

    public async Task CheckPasswordPolicyAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception(Resource.ErrorPasswordEmpty);
        }

        var passwordSettingsObj = await _settingsManager.LoadAsync<PasswordSettings>();

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

    public async Task<string> SendUserPasswordAsync(string email)
    {
        email = (email ?? "").Trim();
        if (!email.TestEmailRegex())
        {
            throw new ArgumentNullException(nameof(email), Resource.ErrorNotCorrectEmail);
        }

        var settings = await _settingsManager.LoadAsync<IPRestrictionsSettings>();

        if (settings.Enable && !await _iPSecurity.VerifyAsync())
        {
            throw new Exception(Resource.ErrorAccessRestricted);
        }

        var userInfo = await _userManager.GetUserByEmailAsync(email);
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

        await _studioNotifyService.UserPasswordChangeAsync(userInfo);

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

    public async Task<string> GetPasswordHelpMessageAsync()
    {
        return GetPasswordHelpMessage(await _settingsManager.LoadAsync<PasswordSettings>());
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