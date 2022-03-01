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

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Core;
using ASC.Web.Core.Notify;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.Core.Notify
{
    [Scope(Additional = typeof(StudioNotifyServiceExtension))]
    public class StudioNotifyService
    {
        private readonly StudioNotifyServiceHelper client;

        public static string EMailSenderName { get { return ASC.Core.Configuration.Constants.NotifyEMailSenderSysName; } }

        private UserManager UserManager { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private TenantExtra TenantExtra { get; }
        private AuthManager Authentication { get; }
        private AuthContext AuthContext { get; }
        private IConfiguration Configuration { get; }
        private TenantManager TenantManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private SetupInfo SetupInfo { get; }
        private IServiceProvider ServiceProvider { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }
        private WebItemSecurity WebItemSecurity { get; }
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
            WebItemSecurity webItemSecurity,
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
            WebItemSecurity = webItemSecurity;
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
            if (string.IsNullOrEmpty(fname)) throw new ArgumentNullException(nameof(fname));
            lname = (lname ?? "").Trim();
            if (string.IsNullOrEmpty(lname)) throw new ArgumentNullException(nameof(lname));
            title = (title ?? "").Trim();
            email = (email ?? "").Trim();
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            phone = (phone ?? "").Trim();
            if (string.IsNullOrEmpty(phone)) throw new ArgumentNullException(nameof(phone));
            ctitle = (ctitle ?? "").Trim();
            if (string.IsNullOrEmpty(ctitle)) throw new ArgumentNullException(nameof(ctitle));
            csize = (csize ?? "").Trim();
            if (string.IsNullOrEmpty(csize)) throw new ArgumentNullException(nameof(csize));
            site = (site ?? "").Trim();
            if (string.IsNullOrEmpty(site) && !CoreBaseSettings.CustomMode) throw new ArgumentNullException(nameof(site));
            message = (message ?? "").Trim();
            if (string.IsNullOrEmpty(message) && !CoreBaseSettings.CustomMode) throw new ArgumentNullException(nameof(message));

            var salesEmail = SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().SalesEmail ?? SetupInfo.SalesEmail;

            var recipient = (IRecipient)new DirectRecipient(AuthContext.CurrentAccount.ID.ToString(), string.Empty, new[] { salesEmail }, false);

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
            var hash = Authentication.GetUserPasswordStamp(userInfo.ID).ToString("s");
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(userInfo.Email, ConfirmType.PasswordChange, hash, userInfo.ID);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangePassword;

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

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangeEmail;

            var action = CoreBaseSettings.Personal
                             ? (CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeEmailChangeV115 : Actions.PersonalEmailChangeV115)
                             : Actions.EmailChangeV115;

            client.SendNoticeToAsync(
                        action,
                        StudioNotifyHelper.RecipientFromEmail(email, false),
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, confirmationUrl),
                        new TagValue(CommonTags.Culture, user.GetCulture().Name));
        }

        public void SendEmailActivationInstructions(UserInfo user, string email)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmailActivation, null, user.ID);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonActivateEmail;

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
            string encyption, int portImap, int portSmtp, string login, bool skipSettings = false)
        {
            var tags = new List<ITagValue>
            {
                new TagValue(Tags.UserName, username ?? string.Empty),
                new TagValue(Tags.Address, address ?? string.Empty)
            };

            if (!skipSettings)
            {
                var link = $"{CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/addons/mail/#accounts/changepwd={address}";

                tags.Add(new TagValue(Tags.MyStaffLink, link));
                tags.Add(new TagValue(Tags.Server, server));
                tags.Add(new TagValue(Tags.Encryption, encyption ?? string.Empty));
                tags.Add(new TagValue(Tags.ImapPort, portImap.ToString(CultureInfo.InvariantCulture)));
                tags.Add(new TagValue(Tags.SmtpPort, portSmtp.ToString(CultureInfo.InvariantCulture)));
                tags.Add(new TagValue(Tags.Login, login));
            }

            client.SendNoticeToAsync(
                skipSettings
                    ? Actions.MailboxWithoutSettingsCreated
                    : Actions.MailboxCreated,
                null,
                StudioNotifyHelper.RecipientFromEmail(toEmails, false),
                new[] { EMailSenderName });
        }

        public void SendMailboxPasswordChanged(List<string> toEmails, string username, string address)
        {
            client.SendNoticeToAsync(
                Actions.MailboxPasswordChanged,
                null,
                StudioNotifyHelper.RecipientFromEmail(toEmails, false),
                new[] { EMailSenderName },
                new TagValue(Tags.UserName, username ?? string.Empty),
                new TagValue(Tags.Address, address ?? string.Empty));
        }

        #endregion

        public void SendMsgMobilePhoneChange(UserInfo userInfo)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(userInfo.Email.ToLower(), ConfirmType.PhoneActivation);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangePhone;

            client.SendNoticeToAsync(
                Actions.PhoneChange,
                StudioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl));
        }

        public void SendMsgTfaReset(UserInfo userInfo)
        {
            var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(userInfo.Email.ToLower(), ConfirmType.TfaActivation);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangeTfa;

            client.SendNoticeToAsync(
                Actions.TfaChange,
                StudioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl));
        }


        public void UserHasJoin()
        {
            if (!CoreBaseSettings.Personal)
            {
                client.SendNoticeAsync(Actions.UserHasJoin, null);
            }
        }

        public void SendJoinMsg(string email, EmployeeType emplType)
        {
            var inviteUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmpInvite, (int)emplType)
                            + string.Format("&emplType={0}", (int)emplType);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonJoin;

            client.SendNoticeToAsync(
                        Actions.JoinUsers,
                        StudioNotifyHelper.RecipientFromEmail(email, true),
                        new[] { EMailSenderName },
                        new TagValue(Tags.InviteLink, inviteUrl),
                        TagValues.GreenButton(greenButtonText, inviteUrl));
        }

        public void UserInfoAddedAfterInvite(UserInfo newUserInfo)
        {
            if (!UserManager.UserExists(newUserInfo)) return;

            INotifyAction notifyAction;
            var footer = "social";

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
            else if (TenantExtra.Opensource)
            {
                notifyAction = Actions.OpensourceUserWelcomeV11;
                footer = "opensource";
            }
            else
            {
                notifyAction = Actions.SaasUserWelcomeV115;
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
                new TagValue(CommonTags.MasterTemplate, CoreBaseSettings.Personal ? "HtmlMasterPersonal" : "HtmlMaster"));
        }

        public void GuestInfoAddedAfterInvite(UserInfo newUserInfo)
        {
            if (!UserManager.UserExists(newUserInfo)) return;

            INotifyAction notifyAction;
            var footer = "social";

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseGuestWelcomeV10 : Actions.EnterpriseWhitelabelGuestWelcomeV10;
                footer = null;
            }
            else if (TenantExtra.Opensource)
            {
                notifyAction = Actions.OpensourceGuestWelcomeV11;
                footer = "opensource";
            }
            else
            {
                notifyAction = Actions.SaasGuestWelcomeV115;
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
                new TagValue(CommonTags.Footer, footer));
        }

        public void UserInfoActivation(UserInfo newUserInfo)
        {
            if (newUserInfo.IsActive)
                throw new ArgumentException("User is already activated!");

            INotifyAction notifyAction;
            var footer = "social";

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseUserActivationV10 : Actions.EnterpriseWhitelabelUserActivationV10;
                footer = null;
            }
            else if (TenantExtra.Opensource)
            {
                notifyAction = Actions.OpensourceUserActivationV11;
                footer = "opensource";
            }
            else
            {
                notifyAction = Actions.SaasUserActivationV115;
            }

            var confirmationUrl = GenerateActivationConfirmUrl(newUserInfo);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonAccept;

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.ActivateUrl, confirmationUrl),
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
                new TagValue(CommonTags.Footer, footer));
        }

        public void GuestInfoActivation(UserInfo newUserInfo)
        {
            if (newUserInfo.IsActive)
                throw new ArgumentException("User is already activated!");

            INotifyAction notifyAction;
            var footer = "social";

            if (TenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                notifyAction = defaultRebranding ? Actions.EnterpriseGuestActivationV10 : Actions.EnterpriseWhitelabelGuestActivationV10;
                footer = null;
            }
            else if (TenantExtra.Opensource)
            {
                notifyAction = Actions.OpensourceGuestActivationV11;
                footer = "opensource";
            }
            else
            {
                notifyAction = Actions.SaasGuestActivationV115;
            }

            var confirmationUrl = GenerateActivationConfirmUrl(newUserInfo);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonAccept;

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.ActivateUrl, confirmationUrl),
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
                new TagValue(CommonTags.Footer, footer));
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

        public void SendMsgProfileHasDeletedItself(UserInfo user)
        {
            var tenant = TenantManager.GetCurrentTenant();
            var admins = UserManager.GetUsers()
                        .Where(u => WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, u.ID));

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    TenantManager.SetCurrentTenant(tenant);

                    foreach (var admin in admins)
                    {
                        var culture = string.IsNullOrEmpty(admin.CultureName) ? tenant.GetCulture() : admin.GetCulture();
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        client.SendNoticeToAsync(
                            Actions.ProfileHasDeletedItself,
                            null,
                            new IRecipient[] { admin },
                            new[] { EMailSenderName },
                            new TagValue(Tags.FromUserName, user.DisplayUserName(DisplayUserSettingsHelper)),
                            new TagValue(Tags.FromUserLink, GetUserProfileLink(user)));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });

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

                tagValues.Add(TagValues.GreenButton(() => WebstudioNotifyPatternResource.ButtonAccessControlPanel, CommonLinkUtility.GetFullAbsolutePath(SetupInfo.ControlPanelUrl)));
            }
            else if (TenantExtra.Opensource)
            {
                notifyAction = Actions.OpensourceAdminWelcomeV11;
                tagValues.Add(new TagValue(CommonTags.Footer, "opensource"));
                tagValues.Add(new TagValue(Tags.ControlPanelUrl, CommonLinkUtility.GetFullAbsolutePath(SetupInfo.ControlPanelUrl).TrimEnd('/')));
            }
            else
            {
                notifyAction = Actions.SaasAdminWelcomeV115;
                //tagValues.Add(TagValues.GreenButton(() => WebstudioNotifyPatternResource.ButtonConfigureRightNow, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetAdministration(ManagementType.General))));

                tagValues.Add(new TagValue(CommonTags.Footer, "common"));
            }

            tagValues.Add(new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()));

            client.SendNoticeToAsync(
                notifyAction,
                StudioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
                new[] { EMailSenderName },
                tagValues.ToArray());
        }

        #region Portal Deactivation & Deletion

        public void SendMsgPortalDeactivation(Tenant t, string deactivateUrl, string activateUrl)
        {
            var u = UserManager.GetUsers(t.OwnerId);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonDeactivatePortal;

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

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonDeletePortal;

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
            string greenButtonText() => WebstudioNotifyPatternResource.ButtonLeaveFeedback;

            client.SendNoticeToAsync(
                        Actions.PortalDeleteSuccessV115,
                        new IRecipient[] { owner },
                        new[] { EMailSenderName },
                        TagValues.GreenButton(greenButtonText, url),
                        new TagValue(Tags.OwnerName, owner.DisplayUserName(DisplayUserSettingsHelper)));
        }

        #endregion

        public void SendMsgDnsChange(Tenant t, string confirmDnsUpdateUrl, string portalAddress, string portalDns)
        {
            var u = UserManager.GetUsers(t.OwnerId);

            string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmPortalAddressChange;

            client.SendNoticeToAsync(
                        Actions.DnsChange,
                        new IRecipient[] { u },
                        new[] { EMailSenderName },
                        new TagValue("ConfirmDnsUpdate", confirmDnsUpdateUrl),//TODO: Tag is deprecated and replaced by TagGreenButton
                        TagValues.GreenButton(greenButtonText, confirmDnsUpdateUrl),
                        new TagValue("PortalAddress", AddHttpToUrl(portalAddress)),
                        new TagValue("PortalDns", AddHttpToUrl(portalDns ?? string.Empty)),
                        new TagValue(Tags.OwnerName, u.DisplayUserName(DisplayUserSettingsHelper)));
        }

        public void SendMsgConfirmChangeOwner(UserInfo owner, UserInfo newOwner, string confirmOwnerUpdateUrl)
        {
            string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmPortalOwnerUpdate;

            client.SendNoticeToAsync(
                Actions.ConfirmOwnerChange,
                null,
                new IRecipient[] { owner },
                new[] { EMailSenderName },
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

                if (TenantExtra.Enterprise)
                {
                    var defaultRebranding = MailWhiteLabelSettings.IsDefault(SettingsManager, Configuration);
                    notifyAction = defaultRebranding ? Actions.EnterpriseAdminActivationV10 : Actions.EnterpriseWhitelabelAdminActivationV10;
                    footer = null;
                }
                else if (TenantExtra.Opensource)
                {
                    notifyAction = Actions.OpensourceAdminActivationV11;
                    footer = "opensource";
                }
                else
                {
                    notifyAction = Actions.SaasAdminActivationV115;
                }

                var confirmationUrl = CommonLinkUtility.GetConfirmationUrl(u.Email, ConfirmType.EmailActivation);
                confirmationUrl += "&first=true";

                string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirm;

                client.SendNoticeToAsync(
                    notifyAction,
                    StudioNotifyHelper.RecipientFromEmail(u.Email, false),
                    new[] { EMailSenderName },
                    new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                    new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
                    new TagValue(Tags.ActivateUrl, confirmationUrl),
                    TagValues.GreenButton(greenButtonText, confirmationUrl),
                    new TagValue(CommonTags.Footer, footer));
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
        }

        #region Personal

        public void SendInvitePersonal(string email, string additionalMember = "")
        {
            var newUserInfo = UserManager.GetUserByEmail(email);
            if (UserManager.UserExists(newUserInfo)) return;

            var lang = CoreBaseSettings.CustomMode
                           ? "ru-RU"
                           : Thread.CurrentThread.CurrentUICulture.Name;

            var culture = SetupInfo.GetPersonalCulture(lang);

            var confirmUrl = CommonLinkUtility.GetConfirmationUrl(email, ConfirmType.EmpInvite, (int)EmployeeType.User)
                             + "&emplType=" + (int)EmployeeType.User
                             + "&lang=" + culture.Key
                             + additionalMember;

            client.SendNoticeToAsync(
                CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeConfirmation : Actions.PersonalConfirmation,
                StudioNotifyHelper.RecipientFromEmail(email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.InviteLink, confirmUrl),
                new TagValue(CommonTags.Footer, CoreBaseSettings.CustomMode ? "personalCustomMode" : "personal"),
                new TagValue(CommonTags.Culture, Thread.CurrentThread.CurrentUICulture.Name));
        }

        public void SendAlreadyExist(string email)
        {
            var userInfo = UserManager.GetUserByEmail(email);
            if (!UserManager.UserExists(userInfo)) return;

            var portalUrl = CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/');

            var hash = Authentication.GetUserPasswordStamp(userInfo.ID).ToString("s");

            var linkToRecovery = CommonLinkUtility.GetConfirmationUrl(userInfo.Email, ConfirmType.PasswordChange, hash, userInfo.ID);

            client.SendNoticeToAsync(
                CoreBaseSettings.CustomMode ? Actions.PersonalCustomModeAlreadyExist : Actions.PersonalAlreadyExist,
                StudioNotifyHelper.RecipientFromEmail(email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.PortalUrl, portalUrl),
                new TagValue(Tags.LinkToRecovery, linkToRecovery),
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
                new TagValue(CommonTags.MasterTemplate, "HtmlMasterPersonal"));
        }

        #endregion

        #region Migration Portal

        public void PortalRenameNotify(Tenant tenant, string oldVirtualRootPath)
        {
            var users = UserManager.GetUsers()
                        .Where(u => u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated));

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<StudioNotifyServiceScope>();
                    var (tenantManager, studioNotifyServiceHelper) = scopeClass;
                    tenantManager.SetCurrentTenant(tenant);

                    foreach (var u in users)
                    {
                        var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        studioNotifyServiceHelper.SendNoticeToAsync(
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


        public void SendRegData(UserInfo u)
        {
            try
            {
                if (!TenantExtra.Saas || !CoreBaseSettings.CustomMode) return;

                var settings = SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
                var salesEmail = settings.SalesEmail ?? SetupInfo.SalesEmail;

                if (string.IsNullOrEmpty(salesEmail)) return;

                var recipient = new DirectRecipient(salesEmail, null, new[] { salesEmail }, false);

                client.SendNoticeToAsync(
                    Actions.SaasCustomModeRegData,
                    null,
                    new IRecipient[] { recipient },
                    new[] { EMailSenderName },
                    new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                    new TagValue(Tags.UserLastName, u.LastName.HtmlEncode()),
                    new TagValue(Tags.UserEmail, u.Email.HtmlEncode()),
                    new TagValue(Tags.Phone, u.MobilePhone != null ? u.MobilePhone.HtmlEncode() : "-"),
                    new TagValue(Tags.Date, u.CreateDate.ToShortDateString() + " " + u.CreateDate.ToShortTimeString()),
                    new TagValue(CommonTags.Footer, null),
                    TagValues.WithoutUnsubscribe());
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
        }

        #endregion

        #region Storage encryption

        public void SendStorageEncryptionStart(string serverRootPath)
        {
            SendStorageEncryptionNotify(Actions.StorageEncryptionStart, false, serverRootPath);
        }

        public void SendStorageEncryptionSuccess(string serverRootPath)
        {
            SendStorageEncryptionNotify(Actions.StorageEncryptionSuccess, false, serverRootPath);
        }

        public void SendStorageEncryptionError(string serverRootPath)
        {
            SendStorageEncryptionNotify(Actions.StorageEncryptionError, true, serverRootPath);
        }

        public void SendStorageDecryptionStart(string serverRootPath)
        {
            SendStorageEncryptionNotify(Actions.StorageDecryptionStart, false, serverRootPath);
        }

        public void SendStorageDecryptionSuccess(string serverRootPath)
        {
            SendStorageEncryptionNotify(Actions.StorageDecryptionSuccess, false, serverRootPath);
        }

        public void SendStorageDecryptionError(string serverRootPath)
        {
            SendStorageEncryptionNotify(Actions.StorageDecryptionError, true, serverRootPath);
        }

        private void SendStorageEncryptionNotify(INotifyAction action, bool notifyAdminsOnly, string serverRootPath)
        {
            var users = notifyAdminsOnly
                    ? UserManager.GetUsersByGroup(Constants.GroupAdmin.ID)
                    : UserManager.GetUsers().Where(u => u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated));

            foreach (var u in users)
            {
                client.SendNoticeToAsync(
                    action,
                    null,
                    new[] { StudioNotifyHelper.ToRecipient(u.ID) },
                    new[] { EMailSenderName },
                    new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                    new TagValue(Tags.PortalUrl, serverRootPath),
                    new TagValue(Tags.ControlPanelUrl, GetControlPanelUrl(serverRootPath)));
            }
        }

        private string GetControlPanelUrl(string serverRootPath)
        {
            var controlPanelUrl = SetupInfo.ControlPanelUrl;

            if (string.IsNullOrEmpty(controlPanelUrl))
                return string.Empty;

            if (controlPanelUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                controlPanelUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                return controlPanelUrl;

            return serverRootPath + "/" + controlPanelUrl.TrimStart('~', '/').TrimEnd('/');
        }

        #endregion
    }

    [Scope]
    public class StudioNotifyServiceScope
    {
        private TenantManager TenantManager { get; }
        private StudioNotifyServiceHelper StudioNotifyServiceHelper { get; }

        public StudioNotifyServiceScope(TenantManager tenantManager, StudioNotifyServiceHelper studioNotifyServiceHelper)
        {
            TenantManager = tenantManager;
            StudioNotifyServiceHelper = studioNotifyServiceHelper;
        }

        public void Deconstruct(out TenantManager tenantManager, out StudioNotifyServiceHelper studioNotifyServiceHelper)
        {
            tenantManager = TenantManager;
            studioNotifyServiceHelper = StudioNotifyServiceHelper;
        }
    }

    public static class StudioNotifyServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<StudioNotifyServiceScope>();
        }
    }
}