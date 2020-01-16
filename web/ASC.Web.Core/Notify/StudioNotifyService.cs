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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Security.Cryptography;
using ASC.Web.Core.Notify;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.Core.Notify
{
    public class StudioNotifyService
    {
        private readonly StudioNotifyServiceHelper client;

        private static string EMailSenderName { get { return ASC.Core.Configuration.Constants.NotifyEMailSenderSysName; } }

        private UserManager UserManager { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private TenantExtra TenantExtra { get; }
        private AuthManager Authentication { get; }
        private AuthContext AuthContext { get; }
        public IConfiguration Configuration { get; }
        private TenantManager TenantManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private SetupInfo SetupInfo { get; }
        private IServiceProvider ServiceProvider { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public SettingsManager SettingsManager { get; }
        private ILog Log { get; }

        public StudioNotifyService(
            UserManager userManager,
            StudioNotifyHelper studioNotifyHelper,
            StudioNotifyServiceHelper studioNotifyServiceHelper,
            TenantExtra tenantExtra,
            AuthManager authentication,
            AuthContext authContext,
            IConfiguration configuration,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CommonLinkUtility commonLinkUtility,
            SetupInfo setupInfo,
            IServiceProvider serviceProvider,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            SettingsManager settingsManager,
            IOptionsMonitor<ILog> option)
        {
            Log = option.Get("ASC.Notify");
            client = studioNotifyServiceHelper;
            TenantExtra = tenantExtra;
            Authentication = authentication;
            AuthContext = authContext;
            Configuration = configuration;
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
            CommonLinkUtility = commonLinkUtility;
            SetupInfo = setupInfo;
            ServiceProvider = serviceProvider;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            SettingsManager = settingsManager;
            UserManager = userManager;
            StudioNotifyHelper = studioNotifyHelper;
        }

        public void SendMsgToAdminAboutProfileUpdated()
        {
            client.SendNoticeAsync(Actions.SelfProfileUpdated, null);
        }

        public void SendMsgToAdminFromNotAuthUser(string email, string message)
        {
            client.SendNoticeAsync(Actions.UserMessageToAdmin, null, new TagValue(Tags.Body, message), new TagValue(Tags.UserEmail, email));
        }

        public void SendRequestTariff(bool license, string fname, string lname, string title, string email, string phone, string ctitle, string csize, string site, string message)
        {
            fname = (fname ?? "").Trim();
            if (string.IsNullOrEmpty(fname)) throw new ArgumentNullException("fname");
            lname = (lname ?? "").Trim();
            if (string.IsNullOrEmpty(lname)) throw new ArgumentNullException("lname");
            title = (title ?? "").Trim();
            email = (email ?? "").Trim();
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");
            phone = (phone ?? "").Trim();
            if (string.IsNullOrEmpty(phone)) throw new ArgumentNullException("phone");
            ctitle = (ctitle ?? "").Trim();
            if (string.IsNullOrEmpty(ctitle)) throw new ArgumentNullException("ctitle");
            csize = (csize ?? "").Trim();
            if (string.IsNullOrEmpty(csize)) throw new ArgumentNullException("csize");
            site = (site ?? "").Trim();
            if (string.IsNullOrEmpty(site)) throw new ArgumentNullException("site");
            message = (message ?? "").Trim();

            var salesEmail = AdditionalWhiteLabelSettings.Instance(SettingsManager).SalesEmail ?? SetupInfo.SalesEmail;

            var recipient = (IRecipient)(new DirectRecipient(AuthContext.CurrentAccount.ID.ToString(), string.Empty, new[] { salesEmail }, false));

            client.SendNoticeToAsync(license ? Actions.RequestLicense : Actions.RequestTariff,
                                     new[] { recipient },
                                     new[] { "email.sender" },
                                     new TagValue(Tags.UserName, fname),
                                     new TagValue(Tags.UserLastName, lname),
                                     new TagValue(Tags.UserPosition, title),
                                     new TagValue(Tags.UserEmail, email),
                                     new TagValue(Tags.Phone, phone),
                                     new TagValue(Tags.Website, site),
                                     new TagValue(Tags.CompanyTitle, ctitle),
                                     new TagValue(Tags.CompanySize, csize),
                                     new TagValue(Tags.Body, message));
        }

        #region Voip

        public void SendToAdminVoipWarning(double balance)
        {
            client.SendNoticeAsync(Actions.VoipWarning, null, new TagValue(Tags.Body, balance));
        }

        public void SendToAdminVoipBlocked()
        {
            client.SendNoticeAsync(Actions.VoipBlocked, null);
        }

        #endregion

        #region User Password

        public void UserPasswordChange(UserInfo userInfo)
        {
            var hash = Hasher.Base64Hash(Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, userInfo.ID));
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(userInfo.Email, ConfirmType.PasswordChange, hash + userInfo.ID, userInfo.ID);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangePassword;

            var action = CoreBaseSettings.Personal
                             ? (CoreBaseSettings.CustomMode ? Actions.PersonalCustomModePasswordChange : Actions.PersonalPasswordChange)
                             : Actions.PasswordChange;

            client.SendNoticeToAsync(
                        action,
                        StudioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, confirmationUrl));
        }

        #endregion

        #region User Email

        public void SendEmailChangeInstructions(UserInfo user, string email)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmailChange, AuthContext.CurrentAccount.ID);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangeEmail;

            var action = CoreBaseSettings.Personal
                             ? (CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeEmailChange : Actions.PersonalEmailChange)
                             : Actions.EmailChange;

            client.SendNoticeToAsync(
                        action,
                        StudioNotifyHelper.RecipientFromEmail(email, false),
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, confirmationUrl),
                        new TagValue(CommonTags.Culture, user.GetCulture().Name));
        }

        public void SendEmailActivationInstructions(UserInfo user, string email)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmailActivation, null, AuthContext.CurrentAccount.ID);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonActivateEmail;

            client.SendNoticeToAsync(
                        Actions.ActivateEmail,
                        StudioNotifyHelper.RecipientFromEmail(email, false),
                        new[] { EMailSenderName },
                        new TagValue(Tags.InviteLink, confirmationUrl),
                        TagValues.GreenButton(greenButtonText, confirmationUrl),
                        new TagValue(Tags.UserDisplayName, (user.DisplayUserName(DisplayUserSettingsHelper) ?? string.Empty).Trim()));
        }

        #endregion

        #region MailServer

        public void SendMailboxCreated(List<string> toEmails, string username, string address)
        {
            SendMailboxCreated(toEmails, username, address, null, null, -1, -1, null);
        }

        public void SendMailboxCreated(List<string> toEmails, string username, string address, string server,
            string encyption, int portImap, int portSmtp, string login)
        {
            var tags = new List<ITagValue>
            {
                new TagValue(Tags.UserName, username ?? string.Empty),
                new TagValue(Tags.Address, address ?? string.Empty)
            };

            var skipSettings = string.IsNullOrEmpty(server);

            if (!skipSettings)
            {
                var link = string.Format("{0}/addons/mail/#accounts/changepwd={1}",
                    CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/'), address);

                tags.Add(new TagValue(Tags.MyStaffLink, link));
                tags.Add(new TagValue(Tags.Server, server));
                tags.Add(new TagValue(Tags.Encryption, encyption ?? string.Empty));
                tags.Add(new TagValue(Tags.ImapPort, portImap.ToString(CultureInfo.InvariantCulture)));
                tags.Add(new TagValue(Tags.SmtpPort, portSmtp.ToString(CultureInfo.InvariantCulture)));
                tags.Add(new TagValue(Tags.Login, login));
            }

            foreach (var email in toEmails)
            {
                client.SendNoticeToAsync(
                    skipSettings
                        ? Actions.MailboxWithoutSettingsCreated
                        : Actions.MailboxCreated,
                    StudioNotifyHelper.RecipientFromEmail(email, false),
                    new[] { EMailSenderName },
                    tags.ToArray());
            }
        }

        public void SendMailboxPasswordChanged(List<string> toEmails, string username, string address)
        {
            foreach (var email in toEmails)
            {
                client.SendNoticeToAsync(
                    Actions.MailboxPasswordChanged,
                    StudioNotifyHelper.RecipientFromEmail(email, false),
                    new[] { EMailSenderName },
                    new TagValue(Tags.UserName, username ?? string.Empty),
                    new TagValue(Tags.Address, address ?? string.Empty));
            }
        }

        #endregion

        public void SendMsgMobilePhoneChange(UserInfo userInfo)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(userInfo.Email.ToLower(), ConfirmType.PhoneActivation);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangePhone;

            client.SendNoticeToAsync(
                Actions.PhoneChange,
                StudioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl));
        }

        public void SendMsgTfaReset(UserInfo userInfo)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(userInfo.Email.ToLower(), ConfirmType.TfaActivation);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangeTfa;

            client.SendNoticeToAsync(
                Actions.TfaChange,
                StudioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl));
        }


        public void UserHasJoin()
        {
            client.SendNoticeAsync(Actions.UserHasJoin, null);
        }

        public void SendJoinMsg(string email, EmployeeType emplType)
        {
            var inviteUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmpInvite, (int)emplType, AuthContext.CurrentAccount.ID)
                            + string.Format("&emplType={0}", emplType);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonJoin;

            client.SendNoticeToAsync(
                        Actions.JoinUsers,
                        StudioNotifyHelper.RecipientFromEmail(email, true),
                        new[] { EMailSenderName },
                        new TagValue(Tags.InviteLink, inviteUrl),
                        TagValues.GreenButton(greenButtonText, inviteUrl),
                        TagValues.SendFrom(TenantManager, UserManager, AuthContext, DisplayUserSettingsHelper));
        }

        public void UserInfoAddedAfterInvite(UserInfo newUserInfo)
        {
            if (!UserManager.UserExists(newUserInfo)) return;

            INotifyAction notifyAction;
            var footer = "social";
            var analytics = string.Empty;

            if (CoreBaseSettings.Personal)
            {
                if (CoreBaseSettings.CustomMode)
                {
                    notifyAction = Actions.PersonalCustomModeAfterRegistration1;
                    footer = "personalCustomMode";
                }
                else
                {
                    notifyAction = Actions.PersonalAfterRegistration1;
                    footer = "personal";
                }
            }
            else if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding
                                   ? Actions.EnterpriseUserWelcomeV10
                                   : CoreBaseSettings.CustomMode
                                         ? Actions.EnterpriseWhitelabelUserWelcomeCustomMode
                                         : Actions.EnterpriseWhitelabelUserWelcomeV10;
                footer = null;
            }
            else
            {
                notifyAction = Actions.SaasUserWelcomeV10;
                analytics = StudioNotifyHelper.GetNotifyAnalytics(notifyAction, false, false, true, false);
            }

            string greenButtonText() => TenantExtra.Enterprise
                                      ? WebstudioNotifyPatternResource.ButtonAccessYourPortal
                                      : WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
                new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
                TagValues.GreenButton(greenButtonText, CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')),
                new TagValue(CommonTags.Footer, footer),
                new TagValue(CommonTags.MasterTemplate, CoreBaseSettings.Personal ? "HtmlMasterPersonal" : "HtmlMaster"),
                new TagValue(CommonTags.Analytics, analytics));
        }

        public void GuestInfoAddedAfterInvite(UserInfo newUserInfo)
        {
            if (!UserManager.UserExists(newUserInfo)) return;

            INotifyAction notifyAction;
            var analytics = string.Empty;
            var footer = "social";

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseGuestWelcomeV10 : Actions.EnterpriseWhitelabelGuestWelcomeV10;
                footer = null;
            }
            else
            {
                notifyAction = Actions.SaasGuestWelcomeV10;
                var tenant = TenantManager.GetCurrentTenant();
                analytics = StudioNotifyHelper.GetNotifyAnalytics(notifyAction, false, false, false, true);
            }

            string greenButtonText() => TenantExtra.Enterprise
                                      ? WebstudioNotifyPatternResource.ButtonAccessYourPortal
                                      : WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
                new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
                TagValues.GreenButton(greenButtonText, CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')),
                new TagValue(CommonTags.Footer, footer),
                new TagValue(CommonTags.Analytics, analytics));
        }

        public void UserInfoActivation(UserInfo newUserInfo)
        {
            if (newUserInfo.IsActive)
                throw new ArgumentException("User is already activated!");

            INotifyAction notifyAction;
            var analytics = string.Empty;
            var footer = "social";

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseUserActivationV10 : Actions.EnterpriseWhitelabelUserActivationV10;
                footer = null;
            }
            else
            {
                notifyAction = Actions.SaasUserActivationV10;
                analytics = StudioNotifyHelper.GetNotifyAnalytics(notifyAction, false, false, true, false);
            }

            var confirmationUrl = GenerateActivationConfirmUrl(newUserInfo);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonAccept;

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
                new TagValue(CommonTags.Footer, footer),
                TagValues.SendFrom(TenantManager, UserManager, AuthContext, DisplayUserSettingsHelper),
                new TagValue(CommonTags.Analytics, analytics));
        }

        public void GuestInfoActivation(UserInfo newUserInfo)
        {
            if (newUserInfo.IsActive)
                throw new ArgumentException("User is already activated!");

            INotifyAction notifyAction;
            var analytics = string.Empty;
            var footer = "social";

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseGuestActivationV10 : Actions.EnterpriseWhitelabelGuestActivationV10;
                footer = null;
            }
            else
            {
                notifyAction = Actions.SaasGuestActivationV10;
                analytics = StudioNotifyHelper.GetNotifyAnalytics(notifyAction, false, false, false, true);
            }

            var confirmationUrl = GenerateActivationConfirmUrl(newUserInfo);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonAccept;

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
                new TagValue(CommonTags.Footer, footer),
                TagValues.SendFrom(TenantManager, UserManager, AuthContext, DisplayUserSettingsHelper),
                new TagValue(CommonTags.Analytics, analytics));
        }

        public void SendMsgProfileDeletion(UserInfo user)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.ProfileRemove, AuthContext.CurrentAccount.ID, AuthContext.CurrentAccount.ID);

            string greenButtonText() => CoreBaseSettings.Personal ? WebstudioNotifyPatternResource.ButtonConfirmTermination : WebstudioNotifyPatternResource.ButtonRemoveProfile;

            var action = CoreBaseSettings.Personal
                             ? (CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeProfileDelete : Actions.PersonalProfileDelete)
                             : Actions.ProfileDelete;

            client.SendNoticeToAsync(
                action,
                StudioNotifyHelper.RecipientFromEmail(user.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                new TagValue(CommonTags.Culture, user.GetCulture().Name));
        }

        public void SendMsgReassignsCompleted(Guid recipientId, UserInfo fromUser, UserInfo toUser)
        {
            client.SendNoticeToAsync(
                Actions.ReassignsCompleted,
                new[] { StudioNotifyHelper.ToRecipient(recipientId) },
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, DisplayUserSettingsHelper.GetFullUserName(recipientId)),
                new TagValue(Tags.FromUserName, fromUser.DisplayUserName(DisplayUserSettingsHelper)),
                new TagValue(Tags.FromUserLink, GetUserProfileLink(fromUser)),
                new TagValue(Tags.ToUserName, toUser.DisplayUserName(DisplayUserSettingsHelper)),
                new TagValue(Tags.ToUserLink, GetUserProfileLink(toUser)));
        }

        public void SendMsgReassignsFailed(Guid recipientId, UserInfo fromUser, UserInfo toUser, string message)
        {
            client.SendNoticeToAsync(
                Actions.ReassignsFailed,
                new[] { StudioNotifyHelper.ToRecipient(recipientId) },
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, DisplayUserSettingsHelper.GetFullUserName(recipientId)),
                new TagValue(Tags.FromUserName, fromUser.DisplayUserName(DisplayUserSettingsHelper)),
                new TagValue(Tags.FromUserLink, GetUserProfileLink(fromUser)),
                new TagValue(Tags.ToUserName, toUser.DisplayUserName(DisplayUserSettingsHelper)),
                new TagValue(Tags.ToUserLink, GetUserProfileLink(toUser)),
                new TagValue(Tags.Message, message));
        }

        public void SendMsgRemoveUserDataCompleted(Guid recipientId, UserInfo user, string fromUserName, long docsSpace, long crmSpace, long mailSpace, long talkSpace)
        {
            client.SendNoticeToAsync(
                CoreBaseSettings.CustomMode ? Actions.RemoveUserDataCompletedCustomMode : Actions.RemoveUserDataCompleted,
                new[] { StudioNotifyHelper.ToRecipient(recipientId) },
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, DisplayUserSettingsHelper.GetFullUserName(recipientId)),
                new TagValue(Tags.FromUserName, fromUserName.HtmlEncode()),
                new TagValue(Tags.FromUserLink, GetUserProfileLink(user)),
                new TagValue("DocsSpace", FileSizeComment.FilesSizeToString(docsSpace)),
                new TagValue("CrmSpace", FileSizeComment.FilesSizeToString(crmSpace)),
                new TagValue("MailSpace", FileSizeComment.FilesSizeToString(mailSpace)),
                new TagValue("TalkSpace", FileSizeComment.FilesSizeToString(talkSpace)));
        }

        public void SendMsgRemoveUserDataFailed(Guid recipientId, UserInfo user, string fromUserName, string message)
        {
            client.SendNoticeToAsync(
                Actions.RemoveUserDataFailed,
                new[] { StudioNotifyHelper.ToRecipient(recipientId) },
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, DisplayUserSettingsHelper.GetFullUserName(recipientId)),
                new TagValue(Tags.FromUserName, fromUserName.HtmlEncode()),
                new TagValue(Tags.FromUserLink, GetUserProfileLink(user)),
                new TagValue(Tags.Message, message));
        }

        public void SendAdminWelcome(UserInfo newUserInfo)
        {
            if (!UserManager.UserExists(newUserInfo)) return;

            if (!newUserInfo.IsActive)
                throw new ArgumentException("User is not activated yet!");

            INotifyAction notifyAction;
            var tagValues = new List<ITagValue>();

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseAdminWelcomeV10 : Actions.EnterpriseWhitelabelAdminWelcomeV10;

                tagValues.Add(TagValues.GreenButton(() => WebstudioNotifyPatternResource.ButtonAccessControlPanel, CommonLinkUtility.GetFullAbsolutePath("~" + SetupInfo.ControlPanelUrl)));
            }
            else
            {
                notifyAction = Actions.SaasAdminWelcomeV10;

                tagValues.Add(TagValues.GreenButton(() => WebstudioNotifyPatternResource.ButtonConfigureRightNow, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetAdministration(ManagementType.General))));

                var tenant = TenantManager.GetCurrentTenant();
                var analytics = StudioNotifyHelper.GetNotifyAnalytics(notifyAction, false, true, false, false);
                tagValues.Add(new TagValue(CommonTags.Analytics, analytics));

                tagValues.Add(TagValues.TableTop());
                tagValues.Add(TagValues.TableItem(1, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-regional-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_regional, null, null));
                tagValues.Add(TagValues.TableItem(2, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-brand-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_brand, null, null));
                tagValues.Add(TagValues.TableItem(3, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-customize-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_customize, null, null));
                tagValues.Add(TagValues.TableItem(4, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-security-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_security, null, null));
                tagValues.Add(TagValues.TableItem(5, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-usertrack-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_usertrack, null, null));
                tagValues.Add(TagValues.TableItem(6, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-backup-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_backup, null, null));
                tagValues.Add(TagValues.TableItem(7, null, null, "https://static.onlyoffice.com/media/newsletters/images-v10/tips-welcome-telephony-50.png", () => WebstudioNotifyPatternResource.pattern_saas_admin_welcome_v10_item_telephony, null, null));
                tagValues.Add(TagValues.TableBottom());

                tagValues.Add(new TagValue(CommonTags.Footer, "common"));
            }

            tagValues.Add(new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()));
            tagValues.Add(TagValues.SendFrom(TenantManager, UserManager, AuthContext, DisplayUserSettingsHelper));

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                tagValues.ToArray());
        }

        #region Backup & Restore

        public void SendMsgBackupCompleted(Guid userId, string link)
        {
            client.SendNoticeToAsync(
                Actions.BackupCreated,
                new[] { StudioNotifyHelper.ToRecipient(userId) },
                new[] { EMailSenderName },
                new TagValue(Tags.OwnerName, UserManager.GetUsers(userId).DisplayUserName(DisplayUserSettingsHelper)));
        }

        public void SendMsgRestoreStarted(Tenant tenant, bool notifyAllUsers)
        {
            var owner = UserManager.GetUsers(tenant.OwnerId);
            var users =
                notifyAllUsers
                    ? StudioNotifyHelper.RecipientFromEmail(UserManager.GetUsers(EmployeeStatus.Active).Where(r => r.ActivationStatus == EmployeeActivationStatus.Activated).Select(u => u.Email).ToArray(), false)
                    : owner.ActivationStatus == EmployeeActivationStatus.Activated ? StudioNotifyHelper.RecipientFromEmail(owner.Email, false) : new IDirectRecipient[0];

            client.SendNoticeToAsync(
                Actions.RestoreStarted,
                users,
                new[] { EMailSenderName });
        }

        public void SendMsgRestoreCompleted(Tenant tenant, bool notifyAllUsers)
        {
            var owner = UserManager.GetUsers(tenant.OwnerId);

            var users =
                notifyAllUsers
                    ? UserManager.GetUsers(EmployeeStatus.Active).Select(u => StudioNotifyHelper.ToRecipient(u.ID)).ToArray()
                    : new[] { StudioNotifyHelper.ToRecipient(owner.ID) };

            client.SendNoticeToAsync(
                Actions.RestoreCompleted,
                users,
                new[] { EMailSenderName },
                new TagValue(Tags.OwnerName, owner.DisplayUserName(DisplayUserSettingsHelper)));
        }

        #endregion

        #region Portal Deactivation & Deletion

        public void SendMsgPortalDeactivation(Tenant t, string deactivateUrl, string activateUrl)
        {
            var u = UserManager.GetUsers(t.OwnerId);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonDeactivatePortal;

            client.SendNoticeToAsync(
                        Actions.PortalDeactivate,
                        new IRecipient[] { u },
                        new[] { EMailSenderName },
                        new TagValue(Tags.ActivateUrl, activateUrl),
                        TagValues.GreenButton(greenButtonText, deactivateUrl),
                        new TagValue(Tags.OwnerName, u.DisplayUserName(DisplayUserSettingsHelper)));
        }

        public void SendMsgPortalDeletion(Tenant t, string url, bool showAutoRenewText)
        {
            var u = UserManager.GetUsers(t.OwnerId);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonDeletePortal;

            client.SendNoticeToAsync(
                        Actions.PortalDelete,
                        new IRecipient[] { u },
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, url),
                        new TagValue(Tags.AutoRenew, showAutoRenewText.ToString()),
                        new TagValue(Tags.OwnerName, u.DisplayUserName(DisplayUserSettingsHelper)));
        }

        public void SendMsgPortalDeletionSuccess(UserInfo owner, string url)
        {
            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonLeaveFeedback;

            client.SendNoticeToAsync(
                        Actions.PortalDeleteSuccessV10,
                        new IRecipient[] { owner },
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, url),
                        new TagValue(Tags.OwnerName, owner.DisplayUserName(DisplayUserSettingsHelper)));
        }

        #endregion

        public void SendMsgDnsChange(Tenant t, string confirmDnsUpdateUrl, string portalAddress, string portalDns)
        {
            var u = UserManager.GetUsers(t.OwnerId);

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmPortalAddressChange;

            client.SendNoticeToAsync(
                        Actions.DnsChange,
                        new IRecipient[] { u },
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, confirmDnsUpdateUrl),
                        new TagValue("PortalAddress", AddHttpToUrl(portalAddress)),
                        new TagValue("PortalDns", AddHttpToUrl(portalDns ?? string.Empty)),
                        new TagValue(Tags.OwnerName, u.DisplayUserName(DisplayUserSettingsHelper)));
        }

        public void SendMsgConfirmChangeOwner(UserInfo owner, UserInfo newOwner, string confirmOwnerUpdateUrl)
        {
            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmPortalOwnerUpdate;

            client.SendNoticeToAsync(
                Actions.ConfirmOwnerChange,
                null,
                new IRecipient[] { owner },
                new[] { EMailSenderName },
                null,
                TagValues.GreenButton(greenButtonText, confirmOwnerUpdateUrl),
                new TagValue(Tags.UserName, newOwner.DisplayUserName(DisplayUserSettingsHelper)),
                new TagValue(Tags.OwnerName, owner.DisplayUserName(DisplayUserSettingsHelper)));
        }

        public void SendCongratulations(UserInfo u)
        {
            try
            {
                INotifyAction notifyAction;
                var footer = "common";
                var analytics = string.Empty;

                if (TenantExtra.Enterprise)
                {
                    var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                    notifyAction = defaultRebranding ? Actions.EnterpriseAdminActivationV10 : Actions.EnterpriseWhitelabelAdminActivationV10;
                    footer = null;
                }
                else if (TenantExtra.Opensource)
                {
                    notifyAction = Actions.OpensourceAdminActivation;
                    footer = "opensource";
                }
                else
                {
                    notifyAction = Actions.SaasAdminActivationV10;
                    var tenant = TenantManager.GetCurrentTenant();
                    analytics = StudioNotifyHelper.GetNotifyAnalytics(notifyAction, false, true, false, false);
                }

                var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(u.Email, ConfirmType.EmailActivation);
                confirmationUrl += "&first=true";

                static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirm;

                client.SendNoticeToAsync(
                    notifyAction,
                    StudioNotifyHelper.RecipientFromEmail(u.Email, false),
                    new[] { EMailSenderName },
                    new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                    new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
                    TagValues.GreenButton(greenButtonText, confirmationUrl),
                    new TagValue(CommonTags.Footer, footer),
                    new TagValue(CommonTags.Analytics, analytics));
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
        }

        #region Personal

        public void SendInvitePersonal(string email, string additionalMember = "", bool analytics = true)
        {
            var newUserInfo = UserManager.GetUserByEmail(email);
            if (UserManager.UserExists(newUserInfo)) return;

            var lang = CoreBaseSettings.CustomMode
                           ? "ru-RU"
                           : Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            var confirmUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmpInvite, (int)EmployeeType.User)
                             + "&emplType=" + (int)EmployeeType.User
                             + "&analytics=" + analytics
                             + "&lang=" + lang
                             + additionalMember;

            client.SendNoticeToAsync(
                CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeConfirmation : Actions.PersonalConfirmation,
                StudioNotifyHelper.RecipientFromEmail(email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.InviteLink, confirmUrl),
                new TagValue(CommonTags.Footer, CoreBaseSettings.CustomMode ? "personalCustomMode" : "personal"),
                new TagValue(CommonTags.Culture, Thread.CurrentThread.CurrentUICulture.Name));
        }

        public void SendUserWelcomePersonal(UserInfo newUserInfo)
        {
            client.SendNoticeToAsync(
                CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeAfterRegistration1 : Actions.PersonalAfterRegistration1,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, true),
                new[] { EMailSenderName },
                new TagValue(CommonTags.Footer, CoreBaseSettings.CustomMode ? "personalCustomMode" : "personal"),
                new TagValue(CommonTags.MasterTemplate, "HtmlMasterPersonal"),
                TagValues.SendFrom(TenantManager, UserManager, AuthContext, DisplayUserSettingsHelper));
        }

        #endregion

        #region Migration Portal

        public void MigrationPortalStart(Tenant tenant, string region, bool notify)
        {
            MigrationNotify(tenant, Actions.MigrationPortalStart, region, string.Empty, notify);
        }

        public void MigrationPortalSuccess(Tenant tenant, string region, string url, bool notify)
        {
            MigrationNotify(tenant, Actions.MigrationPortalSuccess, region, url, notify);
        }

        public void MigrationPortalError(Tenant tenant, string region, string url, bool notify)
        {
            MigrationNotify(tenant, !string.IsNullOrEmpty(region) ? Actions.MigrationPortalError : Actions.MigrationPortalServerFailure, region, url, notify);
        }

        private void MigrationNotify(Tenant tenant, INotifyAction action, string region, string url, bool notify)
        {
            var users = UserManager.GetUsers()
                .Where(u => notify ? u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) : u.IsOwner(tenant))
                .Select(u => StudioNotifyHelper.ToRecipient(u.ID))
                .ToArray();

            if (users.Any())
            {
                client.SendNoticeToAsync(
                    action,
                    users,
                    new[] { EMailSenderName },
                    new TagValue(Tags.RegionName, TransferResourceHelper.GetRegionDescription(region)),
                    new TagValue(Tags.PortalUrl, url));
            }
        }

        public void PortalRenameNotify(Tenant tenant, string oldVirtualRootPath)
        {
            var users = UserManager.GetUsers()
                        .Where(u => u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated));

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var scope = ServiceProvider.CreateScope();
                    var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                    TenantManager.SetCurrentTenant(tenant);

                    var client = scope.ServiceProvider.GetService<StudioNotifyServiceHelper>();

                    foreach (var u in users)
                    {
                        var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        client.SendNoticeToAsync(
                            Actions.PortalRename,
                            new[] { StudioNotifyHelper.ToRecipient(u.ID) },
                            new[] { EMailSenderName },
                            new TagValue(Tags.PortalUrl, oldVirtualRootPath),
                            new TagValue(Tags.UserDisplayName, u.DisplayUserName(DisplayUserSettingsHelper)));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        #endregion

        #region Helpers

        private string GetMyStaffLink()
        {
            return CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetMyStaff());
        }

        private string GetUserProfileLink(UserInfo userInfo)
        {
            return CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(userInfo));
        }

        private static string AddHttpToUrl(string url)
        {
            var httpPrefix = Uri.UriSchemeHttp + Uri.SchemeDelimiter;
            return !string.IsNullOrEmpty(url) && !url.StartsWith(httpPrefix) ? httpPrefix + url : url;
        }

        private string GenerateActivationConfirmUrl(UserInfo user)
        {
            var confirmUrl = CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.Activation, user.ID, user.ID);

            return confirmUrl + $"&firstname={HttpUtility.UrlEncode(user.FirstName)}&lastname={HttpUtility.UrlEncode(user.LastName)}";
        }

        #endregion
    }

    public static class StudioNotifyServiceExtension
    {
        public static IServiceCollection AddStudioNotifyServiceService(this IServiceCollection services)
        {
            services.TryAddScoped<StudioNotifyService>();

            return services
                .AddDisplayUserSettingsService()
                .AddMailWhiteLabelSettingsService()
                .AddAdditionalWhiteLabelSettingsService()
                .AddStudioNotifyServiceHelper()
                .AddUserManagerService()
                .AddStudioNotifyHelperService()
                .AddTenantExtraService()
                .AddAuthManager()
                .AddAuthContextService()
                .AddTenantManagerService()
                .AddCoreBaseSettingsService()
                .AddCommonLinkUtilityService()
                .AddSetupInfo();
        }
    }
}