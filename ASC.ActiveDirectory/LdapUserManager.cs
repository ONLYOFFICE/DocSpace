/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System.Globalization;
using System.Net.Mail;

using ASC.ActiveDirectory.Base;
using ASC.ActiveDirectory.Base.Data;
using ASC.ActiveDirectory.Base.Settings;
using ASC.ActiveDirectory.ComplexOperations;
using ASC.ActiveDirectory.ComplexOperations.Data;
using ASC.ActiveDirectory.Novell;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Mapping = ASC.ActiveDirectory.Base.Settings.LdapSettings.MappingFields;

namespace ASC.ActiveDirectory
{
    [Scope]
    public class LdapUserManager
    {
        private readonly ILog _log;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly TenantUtil _tenantUtil;
        private readonly SecurityContext _securityContext;
        private readonly CookiesManager _cookiesManager;
        private readonly CommonLinkUtility _commonLinkUtility;
        private readonly LdapNotifyHelper _ldapNotifyHelper;
        private readonly SettingsManager _settingsManager;
        private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
        private readonly UserFormatter _userFormatter;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly IOptionsMonitor<ILog> _options;
        private readonly InstanceCrypto _instanceCrypto;
        private readonly IConfiguration _configuration;
        private readonly NovellLdapUserImporter _novellLdapUserImporter;
        private readonly LdapNotifySource _ldapNotifySource;

        public LdapLocalization Resource { get; private set; }

        public LdapUserManager(
            IOptionsMonitor<ILog> option,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IServiceScope serviceScope,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            CookiesManager cookiesManager,
            CommonLinkUtility commonLinkUtility,
            LdapNotifyHelper ldapNotifyHelper,
            SettingsManager settingsManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            UserFormatter userFormatter,
            InstanceCrypto instanceCrypto,
            LdapNotifySource ldapNotifySource,
            NovellLdapUserImporter novellLdapUserImporter)
        {
            _log = option.Get("ASC");
            _options = option;
            _userManager = userManager;
            _configuration = configuration;
            _ldapNotifySource = ldapNotifySource;
            _tenantManager = tenantManager;
            _serviceScope = serviceScope;
            _tenantUtil = tenantUtil;
            _securityContext = securityContext;
            _cookiesManager = cookiesManager;
            _commonLinkUtility = commonLinkUtility;
            _ldapNotifyHelper = ldapNotifyHelper;
            _settingsManager = settingsManager;
            _displayUserSettingsHelper = displayUserSettingsHelper;
            _userFormatter = userFormatter;
            _serviceProvider = serviceProvider;
            _instanceCrypto = instanceCrypto;
            _novellLdapUserImporter = novellLdapUserImporter;
        }

        public void Init(LdapLocalization resource = null)
        {
            Resource = resource ?? new LdapLocalization();
        }

        private bool TestUniqueUserName(string uniqueName)
        {
            return !string.IsNullOrEmpty(uniqueName) && Equals(_userManager.GetUserByUserName(uniqueName), Constants.LostUser);
        }

        private string MakeUniqueName(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.Email))
                throw new ArgumentException(Resource.ErrorEmailEmpty, "userInfo");

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
            return Equals(foundUser, Constants.LostUser) || foundUser.ID == userId;
        }

        public bool TryAddLDAPUser(UserInfo ldapUserInfo, bool onlyGetChanges, out UserInfo portalUserInfo)
        {
            portalUserInfo = Constants.LostUser;

            try
            {
                if (ldapUserInfo == null)
                    throw new ArgumentNullException("ldapUserInfo");

                _log.DebugFormat("TryAddLDAPUser(SID: {0}): Email '{1}' UserName: {2}", ldapUserInfo.Sid,
                    ldapUserInfo.Email, ldapUserInfo.UserName);

                if (!CheckUniqueEmail(ldapUserInfo.ID, ldapUserInfo.Email))
                {
                    _log.DebugFormat("TryAddLDAPUser(SID: {0}): Email '{1}' already exists.",
                        ldapUserInfo.Sid, ldapUserInfo.Email);

                    return false;
                }

                if (!TryChangeExistingUserName(ldapUserInfo.UserName, onlyGetChanges))
                {
                    _log.DebugFormat("TryAddLDAPUser(SID: {0}): Username '{1}' already exists.",
                        ldapUserInfo.Sid, ldapUserInfo.UserName);

                    return false;
                }

                var q = _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId);
                if (q.ActiveUsers <= _userManager.GetUsersByGroup(Constants.GroupUser.ID).Length)
                {
                    _log.DebugFormat("TryAddLDAPUser(SID: {0}): Username '{1}' adding this user would exceed quota.",
                        ldapUserInfo.Sid, ldapUserInfo.UserName);
                    throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", q.ActiveUsers));
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

                _log.DebugFormat("CoreContext.UserManager.SaveUserInfo({0})", ldapUserInfo.GetUserInfoString());

                portalUserInfo = _userManager.SaveUserInfo(ldapUserInfo);

                var passwordHash = LdapUtils.GeneratePassword();

                _log.DebugFormat("SecurityContext.SetUserPassword(ID:{0})", portalUserInfo.ID);

                _securityContext.SetUserPasswordHash(portalUserInfo.ID, passwordHash);

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
                    _log.ErrorFormat("TryAddLDAPUser(UserName='{0}' Sid='{1}') failed: Error: {2}", ldapUserInfo.UserName,
                        ldapUserInfo.Sid, ex);
            }

            return false;
        }

        private bool TryChangeExistingUserName(string ldapUserName, bool onlyGetChanges)
        {
            try
            {
                if (string.IsNullOrEmpty(ldapUserName))
                    return false;

                var otherUser = _userManager.GetUserByUserName(ldapUserName);

                if (Equals(otherUser, Constants.LostUser))
                    return true;

                if (otherUser.IsLDAP())
                    return false;

                otherUser.UserName = MakeUniqueName(otherUser);

                if (onlyGetChanges)
                    return true;

                _log.Debug("TryChangeExistingUserName()");

                _log.DebugFormat("CoreContext.UserManager.SaveUserInfo({0})", otherUser.GetUserInfoString());

                _userManager.SaveUserInfo(otherUser);

                return true;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("TryChangeOtherUserName({0}) failed. Error: {1}", ldapUserName, ex);
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
                            changes.SetSkipUserChange(ldapUserInfo);

                        _log.DebugFormat("SyncUserLDAP(SID: {0}, Username: '{1}') ADD failed: Status is {2}",
                            ldapUserInfo.Sid, ldapUserInfo.UserName,
                            Enum.GetName(typeof(EmployeeStatus), ldapUserInfo.Status));

                        return Constants.LostUser;
                    }

                    if (!TryAddLDAPUser(ldapUserInfo, onlyGetChanges, out result))
                    {
                        if (onlyGetChanges)
                            changes.SetSkipUserChange(ldapUserInfo);

                        return Constants.LostUser;
                    }

                    if (onlyGetChanges)
                        changes.SetAddUserChange(result, _log);

                    if (!onlyGetChanges && _settingsManager.Load<LdapSettings>().SendWelcomeEmail &&
                        (ldapUserInfo.ActivationStatus != EmployeeActivationStatus.AutoGenerated))
                    {
                        var client = WorkContext.NotifyContext.NotifyService.RegisterClient(_ldapNotifySource, _serviceScope);

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
                            NotifyConstants.TagGreenButton(Resource.NotifyButtonJoin, confirmLink),
                            new TagValue(NotifyCommonTags.WithoutUnsubscribe, true));
                    }

                    return result;
                }

                if (userByEmail.IsLDAP())
                {
                    if (ldapUsers == null || ldapUsers.Any(u => u.Sid.Equals(userByEmail.Sid)))
                    {
                        if (onlyGetChanges)
                            changes.SetSkipUserChange(ldapUserInfo);

                        _log.DebugFormat(
                            "SyncUserLDAP(SID: {0}, Username: '{1}') ADD failed: Another ldap user with email '{2}' already exists",
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
                _log.DebugFormat("SyncUserLDAP(SID: {0}, Username: '{1}') No need to update, skipping", ldapUserInfo.Sid, ldapUserInfo.UserName);
                if (onlyGetChanges)
                    changes.SetNoneUserChange(ldapUserInfo);

                return userBySid;
            }

            _log.DebugFormat("SyncUserLDAP(SID: {0}, Username: '{1}') Userinfo is outdated, updating", ldapUserInfo.Sid, ldapUserInfo.UserName);
            if (!TryUpdateUserWithLDAPInfo(userToUpdate, ldapUserInfo, onlyGetChanges, out result))
            {
                if (onlyGetChanges)
                    changes.SetSkipUserChange(ldapUserInfo);

                return Constants.LostUser;
            }

            if (onlyGetChanges)
                changes.SetUpdateUserChange(ldapUserInfo, result, _log);

            return result;
        }

        private const string EXT_MOB_PHONE = "extmobphone";
        private const string EXT_MAIL = "extmail";
        private const string EXT_PHONE = "extphone";
        private const string EXT_SKYPE = "extskype";

        private static void UpdateLdapUserContacts(UserInfo ldapUser, List<string> portalUserContacts)
        {
            if (!portalUserContacts.Any())
                return;

            var ldapUserContacts = ldapUser.Contacts;

            var newContacts = new List<string>(ldapUser.ContactsList);

            for (int i = 0; i < portalUserContacts.Count; i += 2)
            {
                if (portalUserContacts[i] == EXT_MOB_PHONE || portalUserContacts[i] == EXT_MAIL
                    || portalUserContacts[i] == EXT_PHONE || portalUserContacts[i] == EXT_SKYPE)
                    continue;
                if (i + 1 >= portalUserContacts.Count)
                    continue;

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
                    _log.DebugFormat("NeedUpdateUser by FirstName -> portal: '{0}', ldap: '{1}'",
                        portalUser.FirstName ?? "NULL",
                        ldapUser.FirstName ?? "NULL");
                    needUpdate = true;
                }

                if (notEqual(portalUser.LastName, ldapUser.LastName))
                {
                    _log.DebugFormat("NeedUpdateUser by LastName -> portal: '{0}', ldap: '{1}'",
                        portalUser.LastName ?? "NULL",
                        ldapUser.LastName ?? "NULL");
                    needUpdate = true;
                }

                if (notEqual(portalUser.UserName, ldapUser.UserName))
                {
                    _log.DebugFormat("NeedUpdateUser by UserName -> portal: '{0}', ldap: '{1}'",
                        portalUser.UserName ?? "NULL",
                        ldapUser.UserName ?? "NULL");
                    needUpdate = true;
                }

                if (notEqual(portalUser.Email, ldapUser.Email) &&
                    (ldapUser.ActivationStatus != EmployeeActivationStatus.AutoGenerated
                        || ldapUser.ActivationStatus == EmployeeActivationStatus.AutoGenerated && portalUser.ActivationStatus == EmployeeActivationStatus.AutoGenerated))
                {
                    _log.DebugFormat("NeedUpdateUser by Email -> portal: '{0}', ldap: '{1}'",
                        portalUser.Email ?? "NULL",
                        ldapUser.Email ?? "NULL");
                    needUpdate = true;
                }

                if (notEqual(portalUser.Sid, ldapUser.Sid))
                {
                    _log.DebugFormat("NeedUpdateUser by Sid -> portal: '{0}', ldap: '{1}'",
                        portalUser.Sid ?? "NULL",
                        ldapUser.Sid ?? "NULL");
                    needUpdate = true;
                }

                if (settings.LdapMapping.ContainsKey(Mapping.TitleAttribute) && notEqual(portalUser.Title, ldapUser.Title))
                {
                    _log.DebugFormat("NeedUpdateUser by Title -> portal: '{0}', ldap: '{1}'",
                        portalUser.Title ?? "NULL",
                        ldapUser.Title ?? "NULL");
                    needUpdate = true;
                }

                if (settings.LdapMapping.ContainsKey(Mapping.LocationAttribute) && notEqual(portalUser.Location, ldapUser.Location))
                {
                    _log.DebugFormat("NeedUpdateUser by Location -> portal: '{0}', ldap: '{1}'",
                        portalUser.Location ?? "NULL",
                        ldapUser.Location ?? "NULL");
                    needUpdate = true;
                }

                if (portalUser.ActivationStatus != ldapUser.ActivationStatus &&
                    (!portalUser.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) || portalUser.Email != ldapUser.Email) &&
                    ldapUser.ActivationStatus != EmployeeActivationStatus.AutoGenerated)
                {
                    _log.DebugFormat("NeedUpdateUser by ActivationStatus -> portal: '{0}', ldap: '{1}'",
                        portalUser.ActivationStatus,
                        ldapUser.ActivationStatus);
                    needUpdate = true;
                }

                if (portalUser.Status != ldapUser.Status)
                {
                    _log.DebugFormat("NeedUpdateUser by Status -> portal: '{0}', ldap: '{1}'",
                        portalUser.Status,
                        ldapUser.Status);
                    needUpdate = true;
                }

                if (ldapUser.ContactsList.Count != portalUser.ContactsList.Count ||
                    !ldapUser.Contacts.All(portalUser.Contacts.Contains))
                {
                    _log.DebugFormat("NeedUpdateUser by Contacts -> portal: '{0}', ldap: '{1}'",
                        string.Join("|", portalUser.Contacts),
                        string.Join("|", ldapUser.Contacts));
                    needUpdate = true;
                }

                if (settings.LdapMapping.ContainsKey(Mapping.MobilePhoneAttribute) && notEqual(portalUser.MobilePhone, ldapUser.MobilePhone))
                {
                    _log.DebugFormat("NeedUpdateUser by PrimaryPhone -> portal: '{0}', ldap: '{1}'",
                        portalUser.MobilePhone ?? "NULL",
                        ldapUser.MobilePhone ?? "NULL");
                    needUpdate = true;
                }

                if (settings.LdapMapping.ContainsKey(Mapping.BirthDayAttribute) && portalUser.BirthDate == null && ldapUser.BirthDate != null || portalUser.BirthDate != null && !portalUser.BirthDate.Equals(ldapUser.BirthDate))
                {
                    _log.DebugFormat("NeedUpdateUser by BirthDate -> portal: '{0}', ldap: '{1}'",
                        portalUser.BirthDate.ToString() ?? "NULL",
                        ldapUser.BirthDate.ToString() ?? "NULL");
                    needUpdate = true;
                }

                if (settings.LdapMapping.ContainsKey(Mapping.GenderAttribute) && portalUser.Sex == null && ldapUser.Sex != null || portalUser.Sex != null && !portalUser.Sex.Equals(ldapUser.Sex))
                {
                    _log.DebugFormat("NeedUpdateUser by Sex -> portal: '{0}', ldap: '{1}'",
                        portalUser.Sex.ToString() ?? "NULL",
                        ldapUser.Sex.ToString() ?? "NULL");
                    needUpdate = true;
                }
            }
            catch (Exception ex)
            {
                _log.DebugFormat("NeedUpdateUser failed: error: {0}", ex);
            }

            return needUpdate;
        }

        private bool TryUpdateUserWithLDAPInfo(UserInfo userToUpdate, UserInfo updateInfo, bool onlyGetChanges, out UserInfo portlaUserInfo)
        {
            portlaUserInfo = Constants.LostUser;

            try
            {
                _log.Debug("TryUpdateUserWithLDAPInfo()");

                var settings = _settingsManager.Load<LdapSettings>();

                if (!userToUpdate.UserName.Equals(updateInfo.UserName, StringComparison.InvariantCultureIgnoreCase)
                    && !TryChangeExistingUserName(updateInfo.UserName, onlyGetChanges))
                {
                    _log.DebugFormat(
                        "UpdateUserWithLDAPInfo(ID: {0}): New username already exists. (Old: '{1})' New: '{2}'",
                        userToUpdate.ID, userToUpdate.UserName, updateInfo.UserName);

                    return false;
                }

                if (!userToUpdate.Email.Equals(updateInfo.Email, StringComparison.InvariantCultureIgnoreCase)
                    && !CheckUniqueEmail(userToUpdate.ID, updateInfo.Email))
                {
                    _log.DebugFormat(
                        "UpdateUserWithLDAPInfo(ID: {0}): New email already exists. (Old: '{1})' New: '{2}'",
                        userToUpdate.ID, userToUpdate.Email, updateInfo.Email);

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

                if (settings.LdapMapping.ContainsKey(Mapping.TitleAttribute)) userToUpdate.Title = updateInfo.Title;
                if (settings.LdapMapping.ContainsKey(Mapping.LocationAttribute)) userToUpdate.Location = updateInfo.Location;
                if (settings.LdapMapping.ContainsKey(Mapping.GenderAttribute)) userToUpdate.Sex = updateInfo.Sex;
                if (settings.LdapMapping.ContainsKey(Mapping.BirthDayAttribute)) userToUpdate.BirthDate = updateInfo.BirthDate;
                if (settings.LdapMapping.ContainsKey(Mapping.MobilePhoneAttribute)) userToUpdate.MobilePhone = updateInfo.MobilePhone;

                if (!userToUpdate.IsOwner(_tenantManager.GetCurrentTenant())) // Owner must never be terminated by LDAP!
                {
                    userToUpdate.Status = updateInfo.Status;
                }

                if (!onlyGetChanges)
                {
                    _log.DebugFormat("CoreContext.UserManager.SaveUserInfo({0})", userToUpdate.GetUserInfoString());

                    portlaUserInfo = _userManager.SaveUserInfo(userToUpdate);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("UpdateUserWithLDAPInfo(Id='{0}' UserName='{1}' Sid='{2}') failed: Error: {3}",
                    userToUpdate.ID, userToUpdate.UserName,
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
                    return false;

                _log.DebugFormat("TryGetAndSyncLdapUserInfo(login: \"{0}\")", login);

                _novellLdapUserImporter.Init(settings, Resource);

                var ldapUserInfo = _novellLdapUserImporter.Login(login, password);

                if (ldapUserInfo == null || ldapUserInfo.Item1.Equals(Constants.LostUser))
                {
                    _log.DebugFormat("NovellLdapUserImporter.Login('{0}') failed.", login);
                    return false;
                }

                var portalUser = _userManager.GetUserBySid(ldapUserInfo.Item1.Sid);

                if (portalUser.Status == EmployeeStatus.Terminated || portalUser.Equals(Constants.LostUser))
                {
                    if (!ldapUserInfo.Item2.IsDisabled)
                    {
                        _log.DebugFormat("TryCheckAndSyncToLdapUser(Username: '{0}', Email: {1}, DN: {2})",
                            ldapUserInfo.Item1.UserName, ldapUserInfo.Item1.Email, ldapUserInfo.Item2.DistinguishedName);

                        if (!TryCheckAndSyncToLdapUser(ldapUserInfo, _novellLdapUserImporter, out userInfo))
                        {
                            _novellLdapUserImporter.Dispose();
                            _log.Debug("TryCheckAndSyncToLdapUser() failed");
                            return false;
                        }
                        _novellLdapUserImporter.Dispose();
                    }
                    else
                    {
                        _novellLdapUserImporter.Dispose();
                        return false;
                    }
                }
                else
                {
                    _log.DebugFormat("TryCheckAndSyncToLdapUser(Username: '{0}', Email: {1}, DN: {2})",
                        ldapUserInfo.Item1.UserName, ldapUserInfo.Item1.Email, ldapUserInfo.Item2.DistinguishedName);

                    var tenant = _tenantManager.GetCurrentTenant();

                    new Task(() =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                        var securityContext = scope.ServiceProvider.GetService<SecurityContext>();
                        var novellLdapUserImporter = scope.ServiceProvider.GetService<NovellLdapUserImporter>();
                        var userManager = scope.ServiceProvider.GetService<UserManager>();
                        var cookiesManager = scope.ServiceProvider.GetService<CookiesManager>();
                        var log = scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().Get("ASC");

                        tenantManager.SetCurrentTenant(tenant);
                        securityContext.AuthenticateMe(Core.Configuration.Constants.CoreSystem);

                        var uInfo = SyncLDAPUser(ldapUserInfo.Item1);

                        var newLdapUserInfo = new Tuple<UserInfo, LdapObject>(uInfo, ldapUserInfo.Item2);

                        if (novellLdapUserImporter.Settings.GroupMembership)
                        {
                            if (!novellLdapUserImporter.TrySyncUserGroupMembership(newLdapUserInfo))
                            {
                                log.DebugFormat("TryGetAndSyncLdapUserInfo(login: \"{0}\") disabling user {1} due to not being included in any ldap group", login, uInfo);
                                uInfo.Status = EmployeeStatus.Terminated;
                                uInfo.Sid = null;
                                userManager.SaveUserInfo(uInfo);
                                cookiesManager.ResetUserCookie(uInfo.ID);
                            }
                        }
                        novellLdapUserImporter.Dispose();

                    }).Start();

                    if (ldapUserInfo.Item2.IsDisabled)
                    {
                        _log.DebugFormat("TryGetAndSyncLdapUserInfo(login: \"{0}\") failed, user is disabled in ldap", login);
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
                if (_novellLdapUserImporter != null)
                {
                    _novellLdapUserImporter.Dispose();
                }

                _log.ErrorFormat("TryGetLdapUserInfo(login: '{0}') failed. Error: {1}", login, ex);
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
                _log.ErrorFormat("TrySyncLdapUser(SID: '{0}', Email: {1}) failed. Error: {2}", ldapUserInfo.Item1.Sid,
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
}
