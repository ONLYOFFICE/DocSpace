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

namespace ASC.Web.Api.Controllers.Settings;

public class TfaappController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly SmsProviderManager _smsProviderManager;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly CookiesManager _cookiesManager;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly TfaManager _tfaManager;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;
    private readonly StudioSmsNotificationSettingsHelper _studioSmsNotificationSettingsHelper;
    private readonly TfaAppAuthSettingsHelper _tfaAppAuthSettingsHelper;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly Signature _signature;
    private readonly SecurityContext _securityContext;

    public TfaappController(
        MessageService messageService,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        UserManager userManager,
        AuthContext authContext,
        CookiesManager cookiesManager,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        TfaManager tfaManager,
        WebItemManager webItemManager,
        CommonLinkUtility commonLinkUtility,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        MessageTarget messageTarget,
        StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
        TfaAppAuthSettingsHelper tfaAppAuthSettingsHelper,
        SmsProviderManager smsProviderManager,
        IMemoryCache memoryCache,
        InstanceCrypto instanceCrypto,
        Signature signature,
        SecurityContext securityContext,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _smsProviderManager = smsProviderManager;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _authContext = authContext;
        _cookiesManager = cookiesManager;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tfaManager = tfaManager;
        _commonLinkUtility = commonLinkUtility;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
        _studioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
        _tfaAppAuthSettingsHelper = tfaAppAuthSettingsHelper;
        _instanceCrypto = instanceCrypto;
        _signature = signature;
        _securityContext = securityContext;
    }

    /// <summary>
    /// Returns the current two-factor authentication settings.
    /// </summary>
    /// <short>Get the TFA settings</short>
    /// <category>TFA settings</category>
    /// <returns>TFA settings: ID, title, enabled or not, available or not, list of trusted IP addresses, list of users who must use the TFA verification, list of groups who must use the TFA verification</returns>
    ///<path>api/2.0/settings/tfaapp</path>
    ///<httpMethod>GET</httpMethod>
    [HttpGet("tfaapp")]
    public IEnumerable<TfaSettingsDto> GetTfaSettings()
    {
        var result = new List<TfaSettingsDto>();

        var SmsVisible = _studioSmsNotificationSettingsHelper.IsVisibleSettings;
        var SmsEnable = SmsVisible && _smsProviderManager.Enabled();
        var TfaVisible = _tfaAppAuthSettingsHelper.IsVisibleSettings;

        var tfaAppSettings = _settingsManager.Load<TfaAppAuthSettings>();
        var tfaSmsSettings = _settingsManager.Load<StudioSmsNotificationSettings>();

        if (SmsVisible)
        {
            result.Add(new TfaSettingsDto
            {
                Enabled = tfaSmsSettings.EnableSetting && _smsProviderManager.Enabled(),
                Id = "sms",
                Title = Resource.ButtonSmsEnable,
                Avaliable = SmsEnable,
                MandatoryUsers = tfaSmsSettings.MandatoryUsers,
                MandatoryGroups = tfaSmsSettings.MandatoryGroups,
                TrustedIps = tfaSmsSettings.TrustedIps
            });
        }

        if (TfaVisible)
        {
            result.Add(new TfaSettingsDto
            {
                Enabled = tfaAppSettings.EnableSetting,
                Id = "app",
                Title = Resource.ButtonTfaAppEnable,
                Avaliable = true,
                MandatoryUsers = tfaAppSettings.MandatoryUsers,
                MandatoryGroups = tfaAppSettings.MandatoryGroups,
                TrustedIps = tfaAppSettings.TrustedIps
            });
        }

        return result;
    }

    /// <summary>
    /// Validates the two-factor authentication code specified in the request.
    /// </summary>
    /// <short>Validate the TFA code</short>
    /// <category>TFA settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.TfaValidateRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">TFA validation request parameters: Code (string) - TFA code</param>
    /// <returns>True if the code is valid</returns>
    ///<path>api/2.0/settings/tfaapp/validate</path>
    ///<httpMethod>POST</httpMethod>
    [HttpPost("tfaapp/validate")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation,TfaAuth,Everyone")]
    public bool TfaValidateAuthCode(TfaValidateRequestsDto inDto)
    {
        ApiContext.AuthByClaim();
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        _securityContext.Logout();
        return _tfaManager.ValidateAuthCode(user, inDto.Code);
    }

    /// <summary>
    /// Returns the confirmation email URL for authorization via SMS or TFA application.
    /// </summary>
    /// <short>Get confirmation email</short>
    /// <category>TFA settings</category>
    /// <returns>Confirmation email URL</returns>
    ///<path>api/2.0/settings/tfaapp/confirm</path>
    ///<httpMethod>GET</httpMethod>
    [HttpGet("tfaapp/confirm")]
    public object TfaConfirmUrl()
    {
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (_studioSmsNotificationSettingsHelper.IsVisibleSettings && _studioSmsNotificationSettingsHelper.TfaEnabledForUser(user.Id))// && smsConfirm.ToLower() != "true")
        {
            var confirmType = string.IsNullOrEmpty(user.MobilePhone) ||
                            user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated
                                ? ConfirmType.PhoneActivation
                                : ConfirmType.PhoneAuth;

            return _commonLinkUtility.GetConfirmationEmailUrl(user.Email, confirmType);
        }

        if (_tfaAppAuthSettingsHelper.IsVisibleSettings && _tfaAppAuthSettingsHelper.TfaEnabledForUser(user.Id))
        {
            var confirmType = TfaAppUserSettings.EnableForUser(_settingsManager, _authContext.CurrentAccount.ID)
                ? ConfirmType.TfaAuth
                : ConfirmType.TfaActivation;

            return _commonLinkUtility.GetConfirmationEmailUrl(user.Email, confirmType);
        }

        return string.Empty;
    }

    /// <summary>
    /// Updates the two-factor authentication settings with the parameters specified in the request.
    /// </summary>
    /// <short>Update the TFA settings</short>
    /// <category>TFA settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.TfaRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">TFA settings request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>Type</b> (string) - TFA type (None, Sms, or App),</li>
    ///     <li><b>TrustedIps</b> (List&lt;string&gt;) - list of trusted IP addresses,</li>
    ///     <li><b>MandatoryUsers</b> (List&lt;Guid&gt;) - list of users who must use the TFA verification,</li>
    ///     <li><b>MandatoryGroups</b> (List&lt;Guid&gt;) - list of groups who must use the TFA verification.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>True if the operation is successful</returns>
    ///<path>api/2.0/settings/tfaapp</path>
    ///<httpMethod>PUT</httpMethod>
    [HttpPut("tfaapp")]
    public async Task<bool> TfaSettings(TfaRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var result = false;

        MessageAction action;

        switch (inDto.Type)
        {
            case "sms":
                if (!_studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettings())
                {
                    throw new Exception(Resource.SmsNotAvailable);
                }

                if (!_smsProviderManager.Enabled())
                {
                    throw new MethodAccessException();
                }

                var smsSettings = _settingsManager.Load<StudioSmsNotificationSettings>();
                SetSettingsProperty(smsSettings);
                _settingsManager.Save(smsSettings);

                action = MessageAction.TwoFactorAuthenticationEnabledBySms;

                if (_tfaAppAuthSettingsHelper.Enable)
                {
                    _tfaAppAuthSettingsHelper.Enable = false;
                }

                result = true;

                break;

            case "app":
                if (!_tfaAppAuthSettingsHelper.IsVisibleSettings)
                {
                    throw new Exception(Resource.TfaAppNotAvailable);
                }

                var appSettings = _settingsManager.Load<TfaAppAuthSettings>();
                SetSettingsProperty(appSettings);
                _settingsManager.Save(appSettings);


                action = MessageAction.TwoFactorAuthenticationEnabledByTfaApp;

                if (_studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettings() && _studioSmsNotificationSettingsHelper.Enable)
                {
                    _studioSmsNotificationSettingsHelper.Enable = false;
                }

                result = true;

                break;

            default:
                if (_tfaAppAuthSettingsHelper.Enable)
                {
                    _tfaAppAuthSettingsHelper.Enable = false;
                }

                if (_studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettings() && _studioSmsNotificationSettingsHelper.Enable)
                {
                    _studioSmsNotificationSettingsHelper.Enable = false;
                }

                action = MessageAction.TwoFactorAuthenticationDisabled;

                break;
        }

        if (result)
        {
            await _cookiesManager.ResetTenantCookie();
        }

        _messageService.Send(action);
        return result;

        void SetSettingsProperty<T>(TfaSettingsBase<T> settings) where T : class, ISettings<T>
        {
            settings.EnableSetting = true;
            settings.TrustedIps = inDto.TrustedIps ?? new List<string>();
            settings.MandatoryUsers = inDto.MandatoryUsers ?? new List<Guid>();
            settings.MandatoryGroups = inDto.MandatoryGroups ?? new List<Guid>();
        }
    }

    /// <summary>
    /// Returns the confirmation email URL for updating TFA settings.
    /// </summary>
    /// <short>Get confirmation email for updating TFA settings</short>
    /// <category>TFA settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.TfaRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">TFA settings request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>Type</b> (string) - TFA type (None, Sms, or App),</li>
    ///     <li><b>TrustedIps</b> (List&lt;string&gt;) - list of trusted IP addresses,</li>
    ///     <li><b>MandatoryUsers</b> (List&lt;Guid&gt;) - list of users who must use the TFA verification,</li>
    ///     <li><b>MandatoryGroups</b> (List&lt;Guid&gt;) - list of groups who must use the TFA verification.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Confirmation email URL</returns>
    /// <path>api/2.0/settings/tfaappwithlink</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("tfaappwithlink")]
    public async Task<object> TfaSettingsLink(TfaRequestsDto inDto)
    {
        if (await TfaSettings(inDto))
        {
            return TfaConfirmUrl();
        }

        return string.Empty;
    }

    /// <summary>
    /// Generates the setup TFA code for the current user.
    /// </summary>
    /// <short>Generate setup code</short>
    /// <category>TFA settings</category>
    /// <returns>Setup code</returns>
    /// <path>api/2.0/settings/tfaapp/setup</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("tfaapp/setup")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation")]
    public SetupCode TfaAppGenerateSetupCode()
    {
        ApiContext.AuthByClaim();
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings ||
            !_settingsManager.Load<TfaAppAuthSettings>().EnableSetting ||
            TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsOutsider(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        return _tfaManager.GenerateSetupCode(currentUser);
    }

    /// <summary>
    /// Returns the two-factor authentication application codes.
    /// </summary>
    /// <short>Get the TFA codes</short>
    /// <category>TFA settings</category>
    /// <returns>List of TFA application codes</returns>
    /// <path>api/2.0/settings/tfaappcodes</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("tfaappcodes")]
    public IEnumerable<object> TfaAppGetCodes()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsUser(currentUser) || _userManager.IsOutsider(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        return _settingsManager.LoadForCurrentUser<TfaAppUserSettings>().CodesSetting.Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
    }

    /// <summary>
    /// Requests the new backup codes for the two-factor authentication application.
    /// </summary>
    /// <short>Update the TFA codes</short>
    /// <category>TFA settings</category>
    /// <returns>New backup codes</returns>
    /// <path>api/2.0/settings/tfaappnewcodes</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("tfaappnewcodes")]
    public IEnumerable<object> TfaAppRequestNewCodes()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsUser(currentUser) || _userManager.IsOutsider(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        var codes = _tfaManager.GenerateBackupCodes().Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
        _messageService.Send(MessageAction.UserConnectedTfaApp, _messageTarget.Create(currentUser.Id), currentUser.DisplayUserName(false, _displayUserSettingsHelper));
        return codes;
    }

    /// <summary>
    /// Unlinks the current two-factor authentication application from the user account specified in the request.
    /// </summary>
    /// <short>Unlink the TFA application</short>
    /// <category>TFA settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.TfaRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">TFA settings request parameters: Id (Guid?) - user ID</param>
    /// <returns>Login URL</returns>
    /// <path>api/2.0/settings/tfaappnewapp</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("tfaappnewapp")]
    public async Task<object> TfaAppNewApp(TfaRequestsDto inDto)
    {
        var id = inDto?.Id ?? Guid.Empty;
        var isMe = id.Equals(Guid.Empty) || id.Equals(_authContext.CurrentAccount.ID);

        var user = _userManager.GetUsers(id);

        if (!isMe && !_permissionContext.CheckPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser))
        {
            throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);
        }

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, user.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsUser(user) || _userManager.IsOutsider(user))
        {
            throw new NotSupportedException("Not available.");
        }

        TfaAppUserSettings.DisableForUser(_settingsManager, user.Id);
        _messageService.Send(MessageAction.UserDisconnectedTfaApp, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        if (isMe)
        {
            await _cookiesManager.ResetTenantCookie();
            return _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.TfaActivation);
        }

        _studioNotifyService.SendMsgTfaReset(user);
        return string.Empty;
    }
}
