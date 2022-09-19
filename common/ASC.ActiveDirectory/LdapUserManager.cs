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
using Mapping = ASC.ActiveDirectory.Base.Settings.LdapSettings.MappingFields;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.ActiveDirectory;
[Scope]
public class LdapUserManager
{
    private readonly ILogger<LdapUserManager> _logger;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly TenantUtil _tenantUtil;
    private readonly SecurityContext _securityContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly SettingsManager _settingsManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly UserFormatter _userFormatter;
    private readonly IServiceProvider _serviceProvider;
    private readonly NovellLdapUserImporter _novellLdapUserImporter;
    private readonly CountManagerChecker _countManagerChecker;
    private LdapLocalization _resource;

    public LdapUserManager(
        ILogger<LdapUserManager> logger,
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SecurityContext securityContext,
        CommonLinkUtility commonLinkUtility,
        SettingsManager settingsManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        UserFormatter userFormatter,
        NovellLdapUserImporter novellLdapUserImporter,
        CountManagerChecker countManagerChecker)
    {
        _logger = logger;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantUtil = tenantUtil;
        _securityContext = securityContext;
        _commonLinkUtility = commonLinkUtility;
        _settingsManager = settingsManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _userFormatter = userFormatter;
        _serviceProvider = serviceProvider;
        _novellLdapUserImporter = novellLdapUserImporter;
        _countManagerChecker = countManagerChecker;
    }

    public void Init(LdapLocalization resource = null)
    {
        _resource = resource ?? new LdapLocalization();
    }

    private bool TestUniqueUserName(string uniqueName)
    {
        return !string.IsNullOrEmpty(uniqueName) && Equals(_userManager.GetUserByUserName(uniqueName), Constants.LostUser);
    }

    private string MakeUniqueName(UserInfo userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.Email))
        {
            throw new ArgumentException(_resource.ErrorEmailEmpty, "userInfo");
        }

        var uniqueName = new MailAddress(userInfo.Email).User;
        var startUniqueName = uniqueName;
        var i = 0;
        while (!TestUniqueUserName(uniqueName))
        {
            uniqueName = string.Format("{0}{1}", startUniqueName, (++i).ToString(CultureInfo.InvariantCulture));
        }
        return uniqueName;
    }

    private bool CheckUniqueEmail(Guid userId, string email)
    {
        var foundUser = _userManager.GetUserByEmail(email);
        return Equals(foundUser, Constants.LostUser) || foundUser.Id == userId;
    }

    public bool TryAddLDAPUser(UserInfo ldapUserInfo, bool onlyGetChanges, out UserInfo portalUserInfo)
    {
        portalUserInfo = Constants.LostUser;

        try
        {
            if (ldapUserInfo == null)
            {
                throw new ArgumentNullException("ldapUserInfo");
            }

            _logger.DebugTryAddLdapUser(ldapUserInfo.Sid, ldapUserInfo.Email, ldapUserInfo.UserName);

            if (!CheckUniqueEmail(ldapUserInfo.Id, ldapUserInfo.Email))
            {
                _logger.DebugUserAlredyExistsForEmail(ldapUserInfo.Sid, ldapUserInfo.Email);

                return false;
            }

            if (!TryChangeExistingUserName(ldapUserInfo.UserName, onlyGetChanges))
            {
                _logger.DebugUserAlredyExistsForUserName(ldapUserInfo.Sid, ldapUserInfo.UserName);

                return false;
            }

            try
            {
                _countManagerChecker.CheckUsed().Wait();
            }
            catch (Exception)
            {
                _logger.DebugExceedQuota(ldapUserInfo.Sid, ldapUserInfo.UserName);
                throw;
            }

            if (!ldapUserInfo.WorkFromDate.HasValue)
            {
                ldapUserInfo.WorkFromDate = _tenantUtil.DateTimeNow();
            }

            if (onlyGetChanges)
            {
                portalUserInfo = ldapUserInfo;
                return true;
            }

            _logger.DebugSaveUserInfo(ldapUserInfo.GetUserInfoString());

            portalUserInfo = _userManager.SaveUserInfo(ldapUserInfo);

            var passwordHash = LdapUtils.GeneratePassword();

            _logger.DebugSetUserPassword(portalUserInfo.Id);

            _securityContext.SetUserPasswordHash(portalUserInfo.Id, passwordHash);

            return true;
        }
        catch (TenantQuotaException ex)
        {
            // rethrow if quota
            throw ex;
        }
        catch (Exception ex)
        {
            if (ldapUserInfo != null)
            {
                _logger.ErrorTryAddLdapUser(ldapUserInfo.UserName, ldapUserInfo.Sid, ex);
            }
        }

        return false;
    }

    private bool TryChangeExistingUserName(string ldapUserName, bool onlyGetChanges)
    {
        try
        {
            if (string.IsNullOrEmpty(ldapUserName))
            {
                return false;
            }

            var otherUser = _userManager.GetUserByUserName(ldapUserName);

            if (Equals(otherUser, Constants.LostUser))
            {
                return true;
            }

            if (otherUser.IsLDAP())
            {
                return false;
            }

            otherUser.UserName = MakeUniqueName(otherUser);

            if (onlyGetChanges)
            {
                return true;
            }

            _logger.DebugTryChangeExistingUserName();

            _logger.DebugSaveUserInfo(otherUser.GetUserInfoString());

            _userManager.SaveUserInfo(otherUser);

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorTryChangeOtherUserName(ldapUserName, ex);
        }

        return false;
    }

    public UserInfo GetLDAPSyncUserChange(UserInfo ldapUserInfo, List<UserInfo> ldapUsers, out LdapChangeCollection changes)
    {
        return SyncLDAPUser(ldapUserInfo, ldapUsers, out changes, true);
    }

    public UserInfo SyncLDAPUser(UserInfo ldapUserInfo, List<UserInfo> ldapUsers = null)
    {
        LdapChangeCollection changes;
        return SyncLDAPUser(ldapUserInfo, ldapUsers, out changes);
    }

    private UserInfo SyncLDAPUser(UserInfo ldapUserInfo, List<UserInfo> ldapUsers, out LdapChangeCollection changes, bool onlyGetChanges = false)
    {
        UserInfo result;

        changes = new LdapChangeCollection(_userFormatter);

        UserInfo userToUpdate;

        var userBySid = _userManager.GetUserBySid(ldapUserInfo.Sid);

        if (Equals(userBySid, Constants.LostUser))
        {
            var userByEmail = _userManager.GetUserByEmail(ldapUserInfo.Email);

            if (Equals(userByEmail, Constants.LostUser))
            {
                if (ldapUserInfo.Status != EmployeeStatus.Active)
                {
                    if (onlyGetChanges)
                    {
                        changes.SetSkipUserChange(ldapUserInfo);
                    }

                    _logger.DebugSyncUserLdapFailedWithStatus(ldapUserInfo.Sid, ldapUserInfo.UserName,
                        Enum.GetName(typeof(EmployeeStatus), ldapUserInfo.Status));

                    return Constants.LostUser;
                }

                if (!TryAddLDAPUser(ldapUserInfo, onlyGetChanges, out result))
                {
                    if (onlyGetChanges)
                    {
                        changes.SetSkipUserChange(ldapUserInfo);
                    }

                    return Constants.LostUser;
                }

                if (onlyGetChanges)
                {
                    changes.SetAddUserChange(result, _logger);
                }

                if (!onlyGetChanges && _settingsManager.Load<LdapSettings>().SendWelcomeEmail &&
                    (ldapUserInfo.ActivationStatus != EmployeeActivationStatus.AutoGenerated))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
                    var ldapNotifyHelper = scope.ServiceProvider.GetRequiredService<LdapNotifyService>();
                    var source = scope.ServiceProvider.GetRequiredService<LdapNotifySource>();
                    source.Init(tenantManager.GetCurrentTenant());
                    var workContext = scope.ServiceProvider.GetRequiredService<WorkContext>();
                    var notifuEngineQueue = scope.ServiceProvider.GetRequiredService<NotifyEngineQueue>();
                    var client = workContext.NotifyContext.RegisterClient(notifuEngineQueue, source);

                    var confirmLink = _commonLinkUtility.GetConfirmationUrl(ldapUserInfo.Email, ConfirmType.EmailActivation);

                    client.SendNoticeToAsync(
                        NotifyConstants.ActionLdapActivation,
                        null,
                        new[] { new DirectRecipient(ldapUserInfo.Email, null, new[] { ldapUserInfo.Email }, false) },
                        new[] { ASC.Core.Configuration.Constants.NotifyEMailSenderSysName },
                        null,
                        new TagValue(NotifyConstants.TagUserName, ldapUserInfo.DisplayUserName(_displayUserSettingsHelper)),
                        new TagValue(NotifyConstants.TagUserEmail, ldapUserInfo.Email),
                        new TagValue(NotifyConstants.TagMyStaffLink, _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetMyStaff())),
                        NotifyConstants.TagGreenButton(_resource.NotifyButtonJoin, confirmLink),
                        new TagValue(NotifyCommonTags.WithoutUnsubscribe, true));
                }

                return result;
            }

            if (userByEmail.IsLDAP())
            {
                if (ldapUsers == null || ldapUsers.Any(u => u.Sid.Equals(userByEmail.Sid)))
                {
                    if (onlyGetChanges)
                    {
                        changes.SetSkipUserChange(ldapUserInfo);
                    }

                    _logger.DebugSyncUserLdapFailedWithEmail(
                        ldapUserInfo.Sid, ldapUserInfo.UserName, ldapUserInfo.Email);

                    return Constants.LostUser;
                }
            }

            userToUpdate = userByEmail;
        }
        else
        {
            userToUpdate = userBySid;
        }

        UpdateLdapUserContacts(ldapUserInfo, userToUpdate.ContactsList);

        if (!NeedUpdateUser(userToUpdate, ldapUserInfo))
        {
            _logger.DebugSyncUserLdapSkipping(ldapUserInfo.Sid, ldapUserInfo.UserName);
            if (onlyGetChanges)
            {
                changes.SetNoneUserChange(ldapUserInfo);
            }

            return userBySid;
        }

        _logger.DebugSyncUserLdapUpdaiting(ldapUserInfo.Sid, ldapUserInfo.UserName);
        if (!TryUpdateUserWithLDAPInfo(userToUpdate, ldapUserInfo, onlyGetChanges, out result))
        {
            if (onlyGetChanges)
            {
                changes.SetSkipUserChange(ldapUserInfo);
            }

            return Constants.LostUser;
        }

        if (onlyGetChanges)
        {
            changes.SetUpdateUserChange(ldapUserInfo, result, _logger);
        }

        return result;
    }

    private const string EXT_MOB_PHONE = "extmobphone";
    private const string EXT_MAIL = "extmail";
    private const string EXT_PHONE = "extphone";
    private const string EXT_SKYPE = "extskype";

    private static void UpdateLdapUserContacts(UserInfo ldapUser, List<string> portalUserContacts)
    {
        if (portalUserContacts == null || !portalUserContacts.Any())
        {
            return;
        }

        var ldapUserContacts = ldapUser.Contacts;

        var newContacts = new List<string>(ldapUser.ContactsList);

        for (int i = 0; i < portalUserContacts.Count; i += 2)
        {
            if (portalUserContacts[i] == EXT_MOB_PHONE || portalUserContacts[i] == EXT_MAIL
                || portalUserContacts[i] == EXT_PHONE || portalUserContacts[i] == EXT_SKYPE)
            {
                continue;
            }

            if (i + 1 >= portalUserContacts.Count)
            {
                continue;
            }

            newContacts.Add(portalUserContacts[i]);
            newContacts.Add(portalUserContacts[i + 1]);
        }

        ldapUser.ContactsList = newContacts;
    }

    private bool NeedUpdateUser(UserInfo portalUser, UserInfo ldapUser)
    {
        var needUpdate = false;

        try
        {
            var settings = _settingsManager.Load<LdapSettings>();

            Func<string, string, bool> notEqual =
                (f1, f2) =>
                    f1 == null && f2 != null ||
                    f1 != null && !f1.Equals(f2, StringComparison.InvariantCultureIgnoreCase);

            if (notEqual(portalUser.FirstName, ldapUser.FirstName))
            {
                _logger.DebugNeedUpdateUserByFirstName(portalUser.FirstName ?? "NULL",
                    ldapUser.FirstName ?? "NULL");
                needUpdate = true;
            }

            if (notEqual(portalUser.LastName, ldapUser.LastName))
            {
                _logger.DebugNeedUpdateUserByLastName(portalUser.LastName ?? "NULL",
                    ldapUser.LastName ?? "NULL");
                needUpdate = true;
            }

            if (notEqual(portalUser.UserName, ldapUser.UserName))
            {
                _logger.DebugNeedUpdateUserByUserName(portalUser.UserName ?? "NULL",
                    ldapUser.UserName ?? "NULL");
                needUpdate = true;
            }

            if (notEqual(portalUser.Email, ldapUser.Email) &&
                (ldapUser.ActivationStatus != EmployeeActivationStatus.AutoGenerated
                    || ldapUser.ActivationStatus == EmployeeActivationStatus.AutoGenerated && portalUser.ActivationStatus == EmployeeActivationStatus.AutoGenerated))
            {
                _logger.DebugNeedUpdateUserByEmail(portalUser.Email ?? "NULL",
                    ldapUser.Email ?? "NULL");
                needUpdate = true;
            }

            if (notEqual(portalUser.Sid, ldapUser.Sid))
            {
                _logger.DebugNeedUpdateUserBySid(portalUser.Sid ?? "NULL",
                    ldapUser.Sid ?? "NULL");
                needUpdate = true;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.TitleAttribute) && notEqual(portalUser.Title, ldapUser.Title))
            {
                _logger.DebugNeedUpdateUserByTitle(portalUser.Title ?? "NULL",
                    ldapUser.Title ?? "NULL");
                needUpdate = true;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.LocationAttribute) && notEqual(portalUser.Location, ldapUser.Location))
            {
                _logger.DebugNeedUpdateUserByLocation(portalUser.Location ?? "NULL",
                    ldapUser.Location ?? "NULL");
                needUpdate = true;
            }

            if (portalUser.ActivationStatus != ldapUser.ActivationStatus &&
                (!portalUser.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) || portalUser.Email != ldapUser.Email) &&
                ldapUser.ActivationStatus != EmployeeActivationStatus.AutoGenerated)
            {
                _logger.DebugNeedUpdateUserByActivationStatus(portalUser.ActivationStatus,
                    ldapUser.ActivationStatus);
                needUpdate = true;
            }

            if (portalUser.Status != ldapUser.Status)
            {
                _logger.DebugNeedUpdateUserByStatus(portalUser.Status,
                    ldapUser.Status);
                needUpdate = true;
            }

            if (portalUser.ContactsList == null && ldapUser.ContactsList.Count != 0 || portalUser.ContactsList != null && (ldapUser.ContactsList.Count != portalUser.ContactsList.Count ||
                !ldapUser.Contacts.All(portalUser.Contacts.Contains)))
            {
                _logger.DebugNeedUpdateUserByContacts(string.Join("|", portalUser.Contacts),
                    string.Join("|", ldapUser.Contacts));
                needUpdate = true;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.MobilePhoneAttribute) && notEqual(portalUser.MobilePhone, ldapUser.MobilePhone))
            {
                _logger.DebugNeedUpdateUserByPrimaryPhone(portalUser.MobilePhone ?? "NULL",
                    ldapUser.MobilePhone ?? "NULL");
                needUpdate = true;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.BirthDayAttribute) && portalUser.BirthDate == null && ldapUser.BirthDate != null || portalUser.BirthDate != null && !portalUser.BirthDate.Equals(ldapUser.BirthDate))
            {
                _logger.DebugNeedUpdateUserByBirthDate(portalUser.BirthDate.ToString() ?? "NULL",
                    ldapUser.BirthDate.ToString() ?? "NULL");
                needUpdate = true;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.GenderAttribute) && portalUser.Sex == null && ldapUser.Sex != null || portalUser.Sex != null && !portalUser.Sex.Equals(ldapUser.Sex))
            {
                _logger.DebugNeedUpdateUserBySex(portalUser.Sex.ToString() ?? "NULL",
                    ldapUser.Sex.ToString() ?? "NULL");
                needUpdate = true;
            }
        }
        catch (Exception ex)
        {
            _logger.DebugNeedUpdateUser(ex);
        }

        return needUpdate;
    }

    private bool TryUpdateUserWithLDAPInfo(UserInfo userToUpdate, UserInfo updateInfo, bool onlyGetChanges, out UserInfo portlaUserInfo)
    {
        portlaUserInfo = Constants.LostUser;

        try
        {
            _logger.DebugTryUpdateUserWithLdapInfo();

            var settings = _settingsManager.Load<LdapSettings>();

            if (!userToUpdate.UserName.Equals(updateInfo.UserName, StringComparison.InvariantCultureIgnoreCase)
                && !TryChangeExistingUserName(updateInfo.UserName, onlyGetChanges))
            {
                _logger.DebugUpdateUserUserNameAlredyExists(userToUpdate.Id, userToUpdate.UserName, updateInfo.UserName);

                return false;
            }

            if (!userToUpdate.Email.Equals(updateInfo.Email, StringComparison.InvariantCultureIgnoreCase)
                && !CheckUniqueEmail(userToUpdate.Id, updateInfo.Email))
            {
                _logger.DebugUpdateUserEmailAlreadyExists(userToUpdate.Id, userToUpdate.Email, updateInfo.Email);

                return false;
            }

            if (userToUpdate.Email != updateInfo.Email && !(updateInfo.ActivationStatus == EmployeeActivationStatus.AutoGenerated &&
                userToUpdate.ActivationStatus == (EmployeeActivationStatus.AutoGenerated | EmployeeActivationStatus.Activated)))
            {
                userToUpdate.ActivationStatus = updateInfo.ActivationStatus;
                userToUpdate.Email = updateInfo.Email;
            }

            userToUpdate.UserName = updateInfo.UserName;
            userToUpdate.FirstName = updateInfo.FirstName;
            userToUpdate.LastName = updateInfo.LastName;
            userToUpdate.Sid = updateInfo.Sid;
            userToUpdate.Contacts = updateInfo.Contacts;

            if (settings.LdapMapping.ContainsKey(Mapping.TitleAttribute))
            {
                userToUpdate.Title = updateInfo.Title;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.LocationAttribute))
            {
                userToUpdate.Location = updateInfo.Location;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.GenderAttribute))
            {
                userToUpdate.Sex = updateInfo.Sex;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.BirthDayAttribute))
            {
                userToUpdate.BirthDate = updateInfo.BirthDate;
            }

            if (settings.LdapMapping.ContainsKey(Mapping.MobilePhoneAttribute))
            {
                userToUpdate.MobilePhone = updateInfo.MobilePhone;
            }

            if (!userToUpdate.IsOwner(_tenantManager.GetCurrentTenant())) // Owner must never be terminated by LDAP!
            {
                userToUpdate.Status = updateInfo.Status;
            }

            if (!onlyGetChanges)
            {
                _logger.DebugSaveUserInfo(userToUpdate.GetUserInfoString());

                portlaUserInfo = _userManager.SaveUserInfo(userToUpdate);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorUpdateUserWithLDAPInfo(userToUpdate.Id, userToUpdate.UserName,
                userToUpdate.Sid, ex);
        }

        return false;
    }

    public bool TryGetAndSyncLdapUserInfo(string login, string password, out UserInfo userInfo)
    {
        userInfo = Constants.LostUser;


        try
        {
            var settings = _settingsManager.Load<LdapSettings>();

            if (!settings.EnableLdapAuthentication)
            {
                return false;
            }

            _logger.DebugTryGetAndSyncLdapUserInfo(login);

            _novellLdapUserImporter.Init(settings, _resource);

            var ldapUserInfo = _novellLdapUserImporter.Login(login, password);

            if (ldapUserInfo == null || ldapUserInfo.Item1.Equals(Constants.LostUser))
            {
                _logger.DebugNovellLdapUserImporterLoginFailed(login);
                return false;
            }

            var portalUser = _userManager.GetUserBySid(ldapUserInfo.Item1.Sid);

            if (portalUser.Status == EmployeeStatus.Terminated || portalUser.Equals(Constants.LostUser))
            {
                if (!ldapUserInfo.Item2.IsDisabled)
                {
                    _logger.DebugTryCheckAndSyncToLdapUser(ldapUserInfo.Item1.UserName, ldapUserInfo.Item1.Email, ldapUserInfo.Item2.DistinguishedName);

                    if (!TryCheckAndSyncToLdapUser(ldapUserInfo, _novellLdapUserImporter, out userInfo))
                    {
                        _logger.DebugTryCheckAndSyncToLdapUserFailed();
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                _logger.DebugTryCheckAndSyncToLdapUser(ldapUserInfo.Item1.UserName, ldapUserInfo.Item1.Email, ldapUserInfo.Item2.DistinguishedName);

                var tenant = _tenantManager.GetCurrentTenant();

                new Task(async () =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
                    var securityContext = scope.ServiceProvider.GetRequiredService<SecurityContext>();
                    var novellLdapUserImporter = scope.ServiceProvider.GetRequiredService<NovellLdapUserImporter>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
                    var cookiesManager = scope.ServiceProvider.GetRequiredService<CookiesManager>();
                    var log = scope.ServiceProvider.GetRequiredService<ILogger<LdapUserManager>>();

                    tenantManager.SetCurrentTenant(tenant);
                    securityContext.AuthenticateMe(Core.Configuration.Constants.CoreSystem);

                    var uInfo = SyncLDAPUser(ldapUserInfo.Item1);

                    var newLdapUserInfo = new Tuple<UserInfo, LdapObject>(uInfo, ldapUserInfo.Item2);

                    if (novellLdapUserImporter.Settings.GroupMembership)
                    {
                        if (!novellLdapUserImporter.TrySyncUserGroupMembership(newLdapUserInfo))
                        {
                            log.DebugTryGetAndSyncLdapUserInfoDisablingUser(login, uInfo);
                            uInfo.Status = EmployeeStatus.Terminated;
                            uInfo.Sid = null;
                            userManager.SaveUserInfo(uInfo);
                            await cookiesManager.ResetUserCookie(uInfo.Id);
                        }
                    }
                }).Start();

                if (ldapUserInfo.Item2.IsDisabled)
                {
                    _logger.DebugTryGetAndSyncLdapUserInfo(login);
                    return false;
                }
                else
                {
                    userInfo = portalUser;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorTryGetLdapUserInfoFailed(login, ex);
            userInfo = Constants.LostUser;
            return false;
        }
    }

    private bool TryCheckAndSyncToLdapUser(Tuple<UserInfo, LdapObject> ldapUserInfo, LdapUserImporter importer,
        out UserInfo userInfo)
    {
        try
        {
            _securityContext.AuthenticateMe(Core.Configuration.Constants.CoreSystem);

            userInfo = SyncLDAPUser(ldapUserInfo.Item1);

            if (userInfo == null || userInfo.Equals(Constants.LostUser))
            {
                throw new Exception("The user did not pass the configuration check by ldap user settings");
            }

            var newLdapUserInfo = new Tuple<UserInfo, LdapObject>(userInfo, ldapUserInfo.Item2);

            if (!importer.Settings.GroupMembership)
            {
                return true;
            }

            if (!importer.TrySyncUserGroupMembership(newLdapUserInfo))
            {
                userInfo.Sid = null;
                userInfo.Status = EmployeeStatus.Terminated;
                _userManager.SaveUserInfo(userInfo);
                throw new Exception("The user did not pass the configuration check by ldap group settings");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorTrySyncLdapUser(ldapUserInfo.Item1.Sid,
                ldapUserInfo.Item1.Email, ex);
        }
        finally
        {
            _securityContext.Logout();
        }

        userInfo = Constants.LostUser;
        return false;
    }
}
