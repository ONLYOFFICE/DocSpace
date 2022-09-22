﻿// (c) Copyright Ascensio System SIA 2010-2022
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
        _instanceCrypto = instanceCrypto;
        _signature = signature;
        _securityContext = securityContext;
    }

    [HttpGet("tfaapp")]
    public IEnumerable<TfaSettingsRequestsDto> GetTfaSettings()
    {
        var result = new List<TfaSettingsRequestsDto>();

        var SmsVisible = _studioSmsNotificationSettingsHelper.IsVisibleSettings();
        var SmsEnable = SmsVisible && _smsProviderManager.Enabled();
        var TfaVisible = TfaAppAuthSettings.IsVisibleSettings;

        if (SmsVisible)
        {
            result.Add(new TfaSettingsRequestsDto
            {
                Enabled = _studioSmsNotificationSettingsHelper.Enable,
                Id = "sms",
                Title = Resource.ButtonSmsEnable,
                Avaliable = SmsEnable
            });
        }

        if (TfaVisible)
        {
            result.Add(new TfaSettingsRequestsDto
            {
                Enabled = _settingsManager.Load<TfaAppAuthSettings>().EnableSetting,
                Id = "app",
                Title = Resource.ButtonTfaAppEnable,
                Avaliable = true
            });
        }

        return result;
    }

    [HttpPost("tfaapp/validate")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation,TfaAuth,Everyone")]
    public bool TfaValidateAuthCode(TfaValidateRequestsDto inDto)
    {
        ApiContext.AuthByClaim();
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        _securityContext.Logout();
        return _tfaManager.ValidateAuthCode(user, inDto.Code);
    }

    [HttpGet("tfaapp/confirm")]
    public object TfaConfirmUrl()
    {
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)// && smsConfirm.ToLower() != "true")
        {
            var confirmType = string.IsNullOrEmpty(user.MobilePhone) ||
                            user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated
                                ? ConfirmType.PhoneActivation
                                : ConfirmType.PhoneAuth;

            return _commonLinkUtility.GetConfirmationEmailUrl(user.Email, confirmType);
        }

        if (TfaAppAuthSettings.IsVisibleSettings && _settingsManager.Load<TfaAppAuthSettings>().EnableSetting)
        {
            var confirmType = TfaAppUserSettings.EnableForUser(_settingsManager, _authContext.CurrentAccount.ID)
                ? ConfirmType.TfaAuth
                : ConfirmType.TfaActivation;

            return _commonLinkUtility.GetConfirmationEmailUrl(user.Email, confirmType);
        }

        return string.Empty;
    }

    [HttpPut("tfaapp")]
    public async Task<bool> TfaSettings(TfaRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var result = false;

        MessageAction action;
        var settings = _settingsManager.Load<TfaAppAuthSettings>();

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

                _studioSmsNotificationSettingsHelper.Enable = true;
                action = MessageAction.TwoFactorAuthenticationEnabledBySms;

                if (settings.EnableSetting)
                {
                    settings.EnableSetting = false;
                    _settingsManager.Save(settings);
                }

                result = true;

                break;

            case "app":
                if (!TfaAppAuthSettings.IsVisibleSettings)
                {
                    throw new Exception(Resource.TfaAppNotAvailable);
                }

                settings.EnableSetting = true;
                _settingsManager.Save(settings);

                action = MessageAction.TwoFactorAuthenticationEnabledByTfaApp;

                if (_studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettings() && _studioSmsNotificationSettingsHelper.Enable)
                {
                    _studioSmsNotificationSettingsHelper.Enable = false;
                }

                result = true;

                break;

            default:
                if (settings.EnableSetting)
                {
                    settings.EnableSetting = false;
                    _settingsManager.Save(settings);
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
    }

    [HttpPut("tfaappwithlink")]
    public async Task<object> TfaSettingsLink(TfaRequestsDto inDto)
    {
        if (await TfaSettings(inDto))
        {
            return TfaConfirmUrl();
        }

        return string.Empty;
    }

    [HttpGet("tfaapp/setup")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation")]
    public SetupCode TfaAppGenerateSetupCode()
    {
        ApiContext.AuthByClaim();
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!TfaAppAuthSettings.IsVisibleSettings ||
            !_settingsManager.Load<TfaAppAuthSettings>().EnableSetting ||
            TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsVisitor(currentUser) || _userManager.IsOutsider(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        return _tfaManager.GenerateSetupCode(currentUser);
    }

    [HttpGet("tfaappcodes")]
    public IEnumerable<object> TfaAppGetCodes()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsVisitor(currentUser) || _userManager.IsOutsider(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        return _settingsManager.LoadForCurrentUser<TfaAppUserSettings>().CodesSetting.Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
    }

    [HttpPut("tfaappnewcodes")]
    public IEnumerable<object> TfaAppRequestNewCodes()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsVisitor(currentUser) || _userManager.IsOutsider(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        var codes = _tfaManager.GenerateBackupCodes().Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
        _messageService.Send(MessageAction.UserConnectedTfaApp, _messageTarget.Create(currentUser.Id), currentUser.DisplayUserName(false, _displayUserSettingsHelper));
        return codes;
    }

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

        if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, user.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (_userManager.IsVisitor(user) || _userManager.IsOutsider(user))
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
