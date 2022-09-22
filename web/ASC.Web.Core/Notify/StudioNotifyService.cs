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

namespace ASC.Web.Studio.Core.Notify;

[Scope]
public class StudioNotifyService
{
    private readonly StudioNotifyServiceHelper _client;

    public static string EMailSenderName { get { return ASC.Core.Configuration.Constants.NotifyEMailSenderSysName; } }

    private readonly UserManager _userManager;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly TenantExtra _tenantExtra;
    private readonly AuthManager _authentication;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly SetupInfo _setupInfo;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly SettingsManager _settingsManager;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly ILogger _log;

    public StudioNotifyService(
        UserManager userManager,
        StudioNotifyHelper studioNotifyHelper,
        StudioNotifyServiceHelper studioNotifyServiceHelper,
        TenantExtra tenantExtra,
        AuthManager authentication,
        AuthContext authContext,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        SetupInfo setupInfo,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        SettingsManager settingsManager,
        WebItemSecurity webItemSecurity,
        MessageService messageService,
        MessageTarget messageTarget,
        ILoggerProvider option)
    {
        _log = option.CreateLogger("ASC.Notify");
        _client = studioNotifyServiceHelper;
        _tenantExtra = tenantExtra;
        _authentication = authentication;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _setupInfo = setupInfo;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _settingsManager = settingsManager;
        _webItemSecurity = webItemSecurity;
        _messageService = messageService;
        _messageTarget = messageTarget;
        _userManager = userManager;
        _studioNotifyHelper = studioNotifyHelper;
    }

    public void SendMsgToAdminAboutProfileUpdated()
    {
        _client.SendNoticeAsync(Actions.SelfProfileUpdated, null);
    }

    public void SendMsgToAdminFromNotAuthUser(string email, string message)
    {
        _client.SendNoticeAsync(Actions.UserMessageToAdmin, null, new TagValue(Tags.Body, message), new TagValue(Tags.UserEmail, email));
    }

    public void SendRequestTariff(bool license, string fname, string lname, string title, string email, string phone, string ctitle, string csize, string site, string message)
    {
        fname = (fname ?? "").Trim();
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(fname);

        lname = (lname ?? "").Trim();
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(lname);

        title = (title ?? "").Trim();
        email = (email ?? "").Trim();
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(email);

        phone = (phone ?? "").Trim();
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(phone);

        ctitle = (ctitle ?? "").Trim();
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(ctitle);

        csize = (csize ?? "").Trim();
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(csize);
        site = (site ?? "").Trim();
        if (string.IsNullOrEmpty(site) && !_coreBaseSettings.CustomMode)
        {
            throw new ArgumentNullException(nameof(site));
        }

        message = (message ?? "").Trim();
        if (string.IsNullOrEmpty(message) && !_coreBaseSettings.CustomMode)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var salesEmail = _settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().SalesEmail ?? _setupInfo.SalesEmail;

        var recipient = (IRecipient)new DirectRecipient(_authContext.CurrentAccount.ID.ToString(), string.Empty, new[] { salesEmail }, false);

        _client.SendNoticeToAsync(license ? Actions.RequestLicense : Actions.RequestTariff,
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

    #region User Password

    public void UserPasswordChange(UserInfo userInfo)
    {
        var auditEventDate = DateTime.UtcNow;

        var hash = auditEventDate.ToString("s");

        var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(userInfo.Email, ConfirmType.PasswordChange, hash, userInfo.Id);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangePassword;

        var action = _coreBaseSettings.Personal
                         ? (_coreBaseSettings.CustomMode ? Actions.PersonalCustomModeEmailChangeV115 : Actions.PersonalPasswordChangeV115)
                     : Actions.PasswordChangeV115;

        _client.SendNoticeToAsync(
                action,
                    _studioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl));

        var displayUserName = userInfo.DisplayUserName(false, _displayUserSettingsHelper);

        _messageService.Send(auditEventDate, MessageAction.UserSentPasswordChangeInstructions, _messageTarget.Create(userInfo.Id), displayUserName);
    }

    #endregion

    #region User Email

    public void SendEmailChangeInstructions(UserInfo user, string email)
    {
        var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.EmailChange, _authContext.CurrentAccount.ID);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangeEmail;

        var action = _coreBaseSettings.Personal
                         ? (_coreBaseSettings.CustomMode ? Actions.PersonalCustomModeEmailChangeV115 : Actions.PersonalEmailChangeV115)
                     : Actions.EmailChangeV115;

        _client.SendNoticeToAsync(
                action,
                    _studioNotifyHelper.RecipientFromEmail(email, false),
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                new TagValue(CommonTags.Culture, user.GetCulture().Name));
    }

    public void SendEmailActivationInstructions(UserInfo user, string email)
    {
        var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.EmailActivation, null, user.Id);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonActivateEmail;

        _client.SendNoticeToAsync(
                Actions.ActivateEmail,
                    _studioNotifyHelper.RecipientFromEmail(email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.InviteLink, confirmationUrl),
                TagValues.GreenButton(greenButtonText, confirmationUrl),
                    new TagValue(Tags.UserDisplayName, (user.DisplayUserName(_displayUserSettingsHelper) ?? string.Empty).Trim()));
    }

    public void SendEmailRoomInvite(string email, string confirmationUrl)
    {
        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmRoomInvite;

        _client.SendNoticeToAsync(
            Actions.RoomInvite,
                _studioNotifyHelper.RecipientFromEmail(email, false),
                new[] { EMailSenderName },
                new TagValue(Tags.InviteLink, confirmationUrl),
                TagValues.GreenButton(greenButtonText, confirmationUrl));
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
            var link = $"{_commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/addons/mail/#accounts/changepwd={address}";

            tags.Add(new TagValue(Tags.MyStaffLink, link));
            tags.Add(new TagValue(Tags.Server, server));
            tags.Add(new TagValue(Tags.Encryption, encyption ?? string.Empty));
            tags.Add(new TagValue(Tags.ImapPort, portImap.ToString(CultureInfo.InvariantCulture)));
            tags.Add(new TagValue(Tags.SmtpPort, portSmtp.ToString(CultureInfo.InvariantCulture)));
            tags.Add(new TagValue(Tags.Login, login));
        }

        _client.SendNoticeToAsync(
        skipSettings
            ? Actions.MailboxWithoutSettingsCreated
            : Actions.MailboxCreated,
        null,
            _studioNotifyHelper.RecipientFromEmail(toEmails, false),
        new[] { EMailSenderName });
    }

    public void SendMailboxPasswordChanged(List<string> toEmails, string username, string address)
    {
        _client.SendNoticeToAsync(
        Actions.MailboxPasswordChanged,
        null,
            _studioNotifyHelper.RecipientFromEmail(toEmails, false),
        new[] { EMailSenderName },
        new TagValue(Tags.UserName, username ?? string.Empty),
        new TagValue(Tags.Address, address ?? string.Empty));
    }

    #endregion

    public void SendMsgMobilePhoneChange(UserInfo userInfo)
    {
        var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(userInfo.Email.ToLower(), ConfirmType.PhoneActivation);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangePhone;

        _client.SendNoticeToAsync(
        Actions.PhoneChange,
            _studioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
        new[] { EMailSenderName },
        TagValues.GreenButton(greenButtonText, confirmationUrl));
    }

    public void SendMsgTfaReset(UserInfo userInfo)
    {
        var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(userInfo.Email.ToLower(), ConfirmType.TfaActivation);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonChangeTfa;

        _client.SendNoticeToAsync(
        Actions.TfaChange,
            _studioNotifyHelper.RecipientFromEmail(userInfo.Email, false),
        new[] { EMailSenderName },
        TagValues.GreenButton(greenButtonText, confirmationUrl));
    }


    public void UserHasJoin()
    {
        if (!_coreBaseSettings.Personal)
        {
            _client.SendNoticeAsync(Actions.UserHasJoin, null);
        }
    }

    public void SendJoinMsg(string email, EmployeeType emplType)
    {
        var inviteUrl = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.EmpInvite, (int)emplType)
                    + string.Format("&emplType={0}", (int)emplType);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonJoin;

        _client.SendNoticeToAsync(
                Actions.JoinUsers,
                    _studioNotifyHelper.RecipientFromEmail(email, true),
                new[] { EMailSenderName },
                new TagValue(Tags.InviteLink, inviteUrl),
                TagValues.GreenButton(greenButtonText, inviteUrl));
    }

    public void UserInfoAddedAfterInvite(UserInfo newUserInfo)
    {
        if (!_userManager.UserExists(newUserInfo))
        {
            return;
        }

        INotifyAction notifyAction;
        var footer = "social";

        if (_coreBaseSettings.Personal)
        {
            if (_coreBaseSettings.CustomMode)
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
        else if (_tenantExtra.Enterprise)
        {
            var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
            notifyAction = defaultRebranding
                               ? Actions.EnterpriseUserWelcomeV10
                                   : _coreBaseSettings.CustomMode
                                     ? Actions.EnterpriseWhitelabelUserWelcomeCustomMode
                                     : Actions.EnterpriseWhitelabelUserWelcomeV10;
            footer = null;
        }
        else if (_tenantExtra.Opensource)
        {
            notifyAction = Actions.OpensourceUserWelcomeV11;
            footer = "opensource";
        }
        else
        {
            notifyAction = Actions.SaasUserWelcomeV115;
        }

        string greenButtonText() => _tenantExtra.Enterprise
                              ? WebstudioNotifyPatternResource.ButtonAccessYourPortal
                              : WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;

        _client.SendNoticeToAsync(
        notifyAction,
            _studioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
        new[] { EMailSenderName },
        new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
        new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
            TagValues.GreenButton(greenButtonText, _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')),
        new TagValue(CommonTags.Footer, footer),
            new TagValue(CommonTags.MasterTemplate, _coreBaseSettings.Personal ? "HtmlMasterPersonal" : "HtmlMaster"));
    }

    public void GuestInfoAddedAfterInvite(UserInfo newUserInfo)
    {
        if (!_userManager.UserExists(newUserInfo))
        {
            return;
        }

        INotifyAction notifyAction;
        var footer = "social";

        if (_tenantExtra.Enterprise)
        {
            var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
            notifyAction = defaultRebranding ? Actions.EnterpriseGuestWelcomeV10 : Actions.EnterpriseWhitelabelGuestWelcomeV10;
            footer = null;
        }
        else if (_tenantExtra.Opensource)
        {
            notifyAction = Actions.OpensourceGuestWelcomeV11;
            footer = "opensource";
        }
        else
        {
            notifyAction = Actions.SaasGuestWelcomeV115;
        }

        string greenButtonText() => _tenantExtra.Enterprise
                              ? WebstudioNotifyPatternResource.ButtonAccessYourPortal
                              : WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;

        _client.SendNoticeToAsync(
        notifyAction,
            _studioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
        new[] { EMailSenderName },
        new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
        new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
            TagValues.GreenButton(greenButtonText, _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')),
        new TagValue(CommonTags.Footer, footer));
    }

    public void UserInfoActivation(UserInfo newUserInfo)
    {
        if (newUserInfo.IsActive)
        {
            throw new ArgumentException("User is already activated!");
        }

        INotifyAction notifyAction;
        var footer = "social";

        if (_tenantExtra.Enterprise)
        {
            var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
            notifyAction = defaultRebranding ? Actions.EnterpriseUserActivationV10 : Actions.EnterpriseWhitelabelUserActivationV10;
            footer = null;
        }
        else if (_tenantExtra.Opensource)
        {
            notifyAction = Actions.OpensourceUserActivationV11;
            footer = "opensource";
        }
        else
        {
            notifyAction = Actions.SaasUserActivationV115;
        }

        var confirmationUrl = GenerateActivationConfirmUrl(newUserInfo);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonAccept;

        _client.SendNoticeToAsync(
        notifyAction,
            _studioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
        new[] { EMailSenderName },
        new TagValue(Tags.ActivateUrl, confirmationUrl),
        TagValues.GreenButton(greenButtonText, confirmationUrl),
        new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
        new TagValue(CommonTags.Footer, footer));
    }

    public void GuestInfoActivation(UserInfo newUserInfo)
    {
        if (newUserInfo.IsActive)
        {
            throw new ArgumentException("User is already activated!");
        }

        INotifyAction notifyAction;
        var footer = "social";

        if (_tenantExtra.Enterprise)
        {
            var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
            notifyAction = defaultRebranding ? Actions.EnterpriseGuestActivationV10 : Actions.EnterpriseWhitelabelGuestActivationV10;
            footer = null;
        }
        else if (_tenantExtra.Opensource)
        {
            notifyAction = Actions.OpensourceGuestActivationV11;
            footer = "opensource";
        }
        else
        {
            notifyAction = Actions.SaasGuestActivationV115;
        }

        var confirmationUrl = GenerateActivationConfirmUrl(newUserInfo);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonAccept;

        _client.SendNoticeToAsync(
        notifyAction,
            _studioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
        new[] { EMailSenderName },
        new TagValue(Tags.ActivateUrl, confirmationUrl),
        TagValues.GreenButton(greenButtonText, confirmationUrl),
        new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()),
        new TagValue(CommonTags.Footer, footer));
    }

    public void SendMsgProfileDeletion(UserInfo user)
    {
        var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.ProfileRemove, _authContext.CurrentAccount.ID, _authContext.CurrentAccount.ID);

        string greenButtonText() => _coreBaseSettings.Personal ? WebstudioNotifyPatternResource.ButtonConfirmTermination : WebstudioNotifyPatternResource.ButtonRemoveProfile;

        var action = _coreBaseSettings.Personal
                         ? (_coreBaseSettings.CustomMode ? Actions.PersonalCustomModeProfileDelete : Actions.PersonalProfileDelete)
                     : Actions.ProfileDelete;

        _client.SendNoticeToAsync(
        action,
            _studioNotifyHelper.RecipientFromEmail(user.Email, false),
        new[] { EMailSenderName },
        TagValues.GreenButton(greenButtonText, confirmationUrl),
        new TagValue(CommonTags.Culture, user.GetCulture().Name));
    }

    public void SendMsgProfileHasDeletedItself(UserInfo user)
    {
        var tenant = _tenantManager.GetCurrentTenant();
        var admins = _userManager.GetUsers()
                    .Where(u => _webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, u.Id));

        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                _tenantManager.SetCurrentTenant(tenant);

                foreach (var admin in admins)
                {
                    var culture = string.IsNullOrEmpty(admin.CultureName) ? tenant.GetCulture() : admin.GetCulture();
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;

                    _client.SendNoticeToAsync(
                    Actions.ProfileHasDeletedItself,
                    null,
                    new IRecipient[] { admin },
                    new[] { EMailSenderName },
                        new TagValue(Tags.FromUserName, user.DisplayUserName(_displayUserSettingsHelper)),
                    new TagValue(Tags.FromUserLink, GetUserProfileLink(user)));
                }
            }
            catch (Exception ex)
            {
                _log.ErrorSendMsgProfileHasDeletedItself(ex);
            }
        });

    }

    public void SendMsgReassignsCompleted(Guid recipientId, UserInfo fromUser, UserInfo toUser)
    {
        _client.SendNoticeToAsync(
        Actions.ReassignsCompleted,
            new[] { _studioNotifyHelper.ToRecipient(recipientId) },
        new[] { EMailSenderName },
            new TagValue(Tags.UserName, _displayUserSettingsHelper.GetFullUserName(recipientId)),
            new TagValue(Tags.FromUserName, fromUser.DisplayUserName(_displayUserSettingsHelper)),
        new TagValue(Tags.FromUserLink, GetUserProfileLink(fromUser)),
            new TagValue(Tags.ToUserName, toUser.DisplayUserName(_displayUserSettingsHelper)),
        new TagValue(Tags.ToUserLink, GetUserProfileLink(toUser)));
    }

    public void SendMsgReassignsFailed(Guid recipientId, UserInfo fromUser, UserInfo toUser, string message)
    {
        _client.SendNoticeToAsync(
        Actions.ReassignsFailed,
            new[] { _studioNotifyHelper.ToRecipient(recipientId) },
        new[] { EMailSenderName },
            new TagValue(Tags.UserName, _displayUserSettingsHelper.GetFullUserName(recipientId)),
            new TagValue(Tags.FromUserName, fromUser.DisplayUserName(_displayUserSettingsHelper)),
        new TagValue(Tags.FromUserLink, GetUserProfileLink(fromUser)),
            new TagValue(Tags.ToUserName, toUser.DisplayUserName(_displayUserSettingsHelper)),
        new TagValue(Tags.ToUserLink, GetUserProfileLink(toUser)),
        new TagValue(Tags.Message, message));
    }

    public void SendMsgRemoveUserDataCompleted(Guid recipientId, UserInfo user, string fromUserName, long docsSpace, long crmSpace, long mailSpace, long talkSpace)
    {
        _client.SendNoticeToAsync(
            _coreBaseSettings.CustomMode ? Actions.RemoveUserDataCompletedCustomMode : Actions.RemoveUserDataCompleted,
            new[] { _studioNotifyHelper.ToRecipient(recipientId) },
        new[] { EMailSenderName },
            new TagValue(Tags.UserName, _displayUserSettingsHelper.GetFullUserName(recipientId)),
        new TagValue(Tags.FromUserName, fromUserName.HtmlEncode()),
        new TagValue(Tags.FromUserLink, GetUserProfileLink(user)),
        new TagValue("DocsSpace", FileSizeComment.FilesSizeToString(docsSpace)),
        new TagValue("CrmSpace", FileSizeComment.FilesSizeToString(crmSpace)),
        new TagValue("MailSpace", FileSizeComment.FilesSizeToString(mailSpace)),
        new TagValue("TalkSpace", FileSizeComment.FilesSizeToString(talkSpace)));
    }

    public void SendMsgRemoveUserDataFailed(Guid recipientId, UserInfo user, string fromUserName, string message)
    {
        _client.SendNoticeToAsync(
        Actions.RemoveUserDataFailed,
            new[] { _studioNotifyHelper.ToRecipient(recipientId) },
        new[] { EMailSenderName },
            new TagValue(Tags.UserName, _displayUserSettingsHelper.GetFullUserName(recipientId)),
        new TagValue(Tags.FromUserName, fromUserName.HtmlEncode()),
        new TagValue(Tags.FromUserLink, GetUserProfileLink(user)),
        new TagValue(Tags.Message, message));
    }

    public void SendAdminWelcome(UserInfo newUserInfo)
    {
        if (!_userManager.UserExists(newUserInfo))
        {
            return;
        }

        if (!newUserInfo.IsActive)
        {
            throw new ArgumentException("User is not activated yet!");
        }

        INotifyAction notifyAction;
        var tagValues = new List<ITagValue>();

        if (_tenantExtra.Enterprise)
        {
            var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
            notifyAction = defaultRebranding ? Actions.EnterpriseAdminWelcomeV10 : Actions.EnterpriseWhitelabelAdminWelcomeV10;

            tagValues.Add(TagValues.GreenButton(() => WebstudioNotifyPatternResource.ButtonAccessControlPanel, _commonLinkUtility.GetFullAbsolutePath(_setupInfo.ControlPanelUrl)));
            tagValues.Add(new TagValue(Tags.ControlPanelUrl, _commonLinkUtility.GetFullAbsolutePath(_setupInfo.ControlPanelUrl).TrimEnd('/')));
        }
        else if (_tenantExtra.Opensource)
        {
            notifyAction = Actions.OpensourceAdminWelcomeV11;
            tagValues.Add(new TagValue(CommonTags.Footer, "opensource"));
            tagValues.Add(new TagValue(Tags.ControlPanelUrl, _commonLinkUtility.GetFullAbsolutePath(_setupInfo.ControlPanelUrl).TrimEnd('/')));
        }
        else
        {
            notifyAction = Actions.SaasAdminWelcomeV115;
            //tagValues.Add(TagValues.GreenButton(() => WebstudioNotifyPatternResource.ButtonConfigureRightNow, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetAdministration(ManagementType.General))));

            tagValues.Add(new TagValue(CommonTags.Footer, "common"));
            tagValues.Add(new TagValue(Tags.PricingPage, _commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx")));
        }

        tagValues.Add(new TagValue(Tags.UserName, newUserInfo.FirstName.HtmlEncode()));

        _client.SendNoticeToAsync(
        notifyAction,
            _studioNotifyHelper.RecipientFromEmail(newUserInfo.Email, false),
        new[] { EMailSenderName },
        tagValues.ToArray());
    }

    #region Portal Deactivation & Deletion

    public void SendMsgPortalDeactivation(Tenant t, string deactivateUrl, string activateUrl)
    {
        var u = _userManager.GetUsers(t.OwnerId);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonDeactivatePortal;

        _client.SendNoticeToAsync(
                Actions.PortalDeactivate,
                new IRecipient[] { u },
                new[] { EMailSenderName },
                new TagValue(Tags.ActivateUrl, activateUrl),
                TagValues.GreenButton(greenButtonText, deactivateUrl),
                    new TagValue(Tags.OwnerName, u.DisplayUserName(_displayUserSettingsHelper)));
    }

    public void SendMsgPortalDeletion(Tenant t, string url, bool showAutoRenewText)
    {
        var u = _userManager.GetUsers(t.OwnerId);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonDeletePortal;

        _client.SendNoticeToAsync(
                Actions.PortalDelete,
                new IRecipient[] { u },
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, url),
                new TagValue(Tags.AutoRenew, showAutoRenewText.ToString()),
                    new TagValue(Tags.OwnerName, u.DisplayUserName(_displayUserSettingsHelper)));
    }

    public void SendMsgPortalDeletionSuccess(UserInfo owner, string url)
    {
        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonLeaveFeedback;

        _client.SendNoticeToAsync(
                Actions.PortalDeleteSuccessV115,
                new IRecipient[] { owner },
                new[] { EMailSenderName },
                TagValues.GreenButton(greenButtonText, url),
                    new TagValue(Tags.OwnerName, owner.DisplayUserName(_displayUserSettingsHelper)));
    }

    #endregion

    public void SendMsgDnsChange(Tenant t, string confirmDnsUpdateUrl, string portalAddress, string portalDns)
    {
        var u = _userManager.GetUsers(t.OwnerId);

        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmPortalAddressChange;

        _client.SendNoticeToAsync(
                Actions.DnsChange,
                new IRecipient[] { u },
                new[] { EMailSenderName },
                new TagValue("ConfirmDnsUpdate", confirmDnsUpdateUrl),//TODO: Tag is deprecated and replaced by TagGreenButton
                TagValues.GreenButton(greenButtonText, confirmDnsUpdateUrl),
                new TagValue("PortalAddress", AddHttpToUrl(portalAddress)),
                new TagValue("PortalDns", AddHttpToUrl(portalDns ?? string.Empty)),
                    new TagValue(Tags.OwnerName, u.DisplayUserName(_displayUserSettingsHelper)));
    }

    public void SendMsgConfirmChangeOwner(UserInfo owner, UserInfo newOwner, string confirmOwnerUpdateUrl)
    {
        static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirmPortalOwnerUpdate;

        _client.SendNoticeToAsync(
        Actions.ConfirmOwnerChange,
        null,
        new IRecipient[] { owner },
        new[] { EMailSenderName },
        TagValues.GreenButton(greenButtonText, confirmOwnerUpdateUrl),
            new TagValue(Tags.UserName, newOwner.DisplayUserName(_displayUserSettingsHelper)),
            new TagValue(Tags.OwnerName, owner.DisplayUserName(_displayUserSettingsHelper)));
    }

    public void SendCongratulations(UserInfo u)
    {
        try
        {
            INotifyAction notifyAction;
            var footer = "common";

            if (_tenantExtra.Enterprise)
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
                notifyAction = defaultRebranding ? Actions.EnterpriseAdminActivationV10 : Actions.EnterpriseWhitelabelAdminActivationV10;
                footer = null;
            }
            else if (_tenantExtra.Opensource)
            {
                notifyAction = Actions.OpensourceAdminActivationV11;
                footer = "opensource";
            }
            else
            {
                notifyAction = Actions.SaasAdminActivationV115;
            }

            var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(u.Email, ConfirmType.EmailActivation);
            confirmationUrl += "&first=true";

            static string greenButtonText() => WebstudioNotifyPatternResource.ButtonConfirm;

            _client.SendNoticeToAsync(
            notifyAction,
                _studioNotifyHelper.RecipientFromEmail(u.Email, false),
            new[] { EMailSenderName },
            new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
            new TagValue(Tags.MyStaffLink, GetMyStaffLink()),
            new TagValue(Tags.ActivateUrl, confirmationUrl),
            TagValues.GreenButton(greenButtonText, confirmationUrl),
            new TagValue(CommonTags.Footer, footer));
        }
        catch (Exception error)
        {
            _log.ErrorSendCongratulations(error);
        }
    }

    #region Personal

    public void SendInvitePersonal(string email, string additionalMember = "")
    {
        var newUserInfo = _userManager.GetUserByEmail(email);
        if (_userManager.UserExists(newUserInfo))
        {
            return;
        }

        var lang = _coreBaseSettings.CustomMode
                       ? "ru-RU"
                       : Thread.CurrentThread.CurrentUICulture.Name;

        var culture = _setupInfo.GetPersonalCulture(lang);

        var confirmUrl = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.EmpInvite, (int)EmployeeType.User)
                     + "&emplType=" + (int)EmployeeType.User
                     + "&lang=" + culture.Key
                     + additionalMember;

        _client.SendNoticeToAsync(
            _coreBaseSettings.CustomMode ? Actions.PersonalCustomModeConfirmation : Actions.PersonalConfirmation,
            _studioNotifyHelper.RecipientFromEmail(email, false),
        new[] { EMailSenderName },
        new TagValue(Tags.InviteLink, confirmUrl),
            new TagValue(CommonTags.Footer, _coreBaseSettings.CustomMode ? "personalCustomMode" : "personal"),
        new TagValue(CommonTags.Culture, Thread.CurrentThread.CurrentUICulture.Name));
    }

    public void SendAlreadyExist(string email)
    {
        var userInfo = _userManager.GetUserByEmail(email);
        if (!_userManager.UserExists(userInfo))
        {
            return;
        }

        var portalUrl = _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/');

        var hash = _authentication.GetUserPasswordStamp(userInfo.Id).ToString("s");

        var linkToRecovery = _commonLinkUtility.GetConfirmationEmailUrl(userInfo.Email, ConfirmType.PasswordChange, hash, userInfo.Id);

        _client.SendNoticeToAsync(
            _coreBaseSettings.CustomMode ? Actions.PersonalCustomModeAlreadyExist : Actions.PersonalAlreadyExist,
            _studioNotifyHelper.RecipientFromEmail(email, false),
        new[] { EMailSenderName },
        new TagValue(Tags.PortalUrl, portalUrl),
        new TagValue(Tags.LinkToRecovery, linkToRecovery),
            new TagValue(CommonTags.Footer, _coreBaseSettings.CustomMode ? "personalCustomMode" : "personal"),
        new TagValue(CommonTags.Culture, Thread.CurrentThread.CurrentUICulture.Name));
    }

    public void SendUserWelcomePersonal(UserInfo newUserInfo)
    {
        _client.SendNoticeToAsync(
            _coreBaseSettings.CustomMode ? Actions.PersonalCustomModeAfterRegistration1 : Actions.PersonalAfterRegistration1,
            _studioNotifyHelper.RecipientFromEmail(newUserInfo.Email, true),
        new[] { EMailSenderName },
            new TagValue(CommonTags.Footer, _coreBaseSettings.CustomMode ? "personalCustomMode" : "personal"),
        new TagValue(CommonTags.MasterTemplate, "HtmlMasterPersonal"));
    }

    #endregion

    #region Migration Portal

    public void PortalRenameNotify(Tenant tenant, string oldVirtualRootPath)
    {
        var users = _userManager.GetUsers()
                .Where(u => u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated));

        try
        {
            _tenantManager.SetCurrentTenant(tenant);

            foreach (var u in users)
            {
                var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                _client.SendNoticeToAsync(
                    Actions.PortalRename,
                    new[] { _studioNotifyHelper.ToRecipient(u.Id) },
                    new[] { EMailSenderName },
                    new TagValue(Tags.PortalUrl, oldVirtualRootPath),
                    new TagValue(Tags.UserDisplayName, u.DisplayUserName(_displayUserSettingsHelper)));
            }
        }
        catch (Exception ex)
        {
            _log.ErrorPortalRenameNotify(ex);
        }
    }

    #endregion

    #region Helpers

    private string GetMyStaffLink()
    {
        return _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetMyStaff());
    }

    private string GetUserProfileLink(UserInfo userInfo)
    {
        return _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetUserProfile(userInfo));
    }

    private static string AddHttpToUrl(string url)
    {
        var httpPrefix = Uri.UriSchemeHttp + Uri.SchemeDelimiter;
        return !string.IsNullOrEmpty(url) && !url.StartsWith(httpPrefix) ? httpPrefix + url : url;
    }

    private string GenerateActivationConfirmUrl(UserInfo user)
    {
        var confirmUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.Activation, user.Id, user.Id);

        return confirmUrl + $"&firstname={HttpUtility.UrlEncode(user.FirstName)}&lastname={HttpUtility.UrlEncode(user.LastName)}";
    }


    public void SendRegData(UserInfo u)
    {
        try
        {
            if (!_tenantExtra.Saas || !_coreBaseSettings.CustomMode)
            {
                return;
            }

            var settings = _settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
            var salesEmail = settings.SalesEmail ?? _setupInfo.SalesEmail;

            if (string.IsNullOrEmpty(salesEmail))
            {
                return;
            }

            var recipient = new DirectRecipient(salesEmail, null, new[] { salesEmail }, false);

            _client.SendNoticeToAsync(
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
            _log.ErrorSendRegData(error);
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
                    ? _userManager.GetUsersByGroup(Constants.GroupAdmin.ID)
                    : _userManager.GetUsers().Where(u => u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated));

        foreach (var u in users)
        {
            _client.SendNoticeToAsync(
            action,
            null,
                new[] { _studioNotifyHelper.ToRecipient(u.Id) },
            new[] { EMailSenderName },
            new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
            new TagValue(Tags.PortalUrl, serverRootPath),
            new TagValue(Tags.ControlPanelUrl, GetControlPanelUrl(serverRootPath)));
        }
    }

    private string GetControlPanelUrl(string serverRootPath)
    {
        var controlPanelUrl = _setupInfo.ControlPanelUrl;

        if (string.IsNullOrEmpty(controlPanelUrl))
        {
            return string.Empty;
        }

        if (controlPanelUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
            controlPanelUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
        {
            return controlPanelUrl;
        }

        return serverRootPath + "/" + controlPanelUrl.TrimStart('~', '/').TrimEnd('/');
    }

    #endregion
}
