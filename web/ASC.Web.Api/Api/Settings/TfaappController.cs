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

    [HttpGet("tfaapp")]
    public async Task<IEnumerable<TfaSettingsDto>> GetTfaSettingsAsync()
    {
        var result = new List<TfaSettingsDto>();

        var SmsVisible = _studioSmsNotificationSettingsHelper.IsVisibleSettings;
        var SmsEnable = SmsVisible && _smsProviderManager.Enabled();
        var TfaVisible = _tfaAppAuthSettingsHelper.IsVisibleSettings;

        var tfaAppSettings = await _settingsManager.LoadAsync<TfaAppAuthSettings>();
        var tfaSmsSettings = await _settingsManager.LoadAsync<StudioSmsNotificationSettings>();

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

    [HttpPost("tfaapp/validate")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation,TfaAuth,Everyone")]
    public async Task<bool> TfaValidateAuthCodeAsync(TfaValidateRequestsDto inDto)
    {
        await ApiContext.AuthByClaimAsync();
        var user = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);
        _securityContext.Logout();
        return await _tfaManager.ValidateAuthCodeAsync(user, inDto.Code);
    }

    [HttpGet("tfaapp/confirm")]
    public async Task<object> TfaConfirmUrlAsync()
    {
        var user = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        if (_studioSmsNotificationSettingsHelper.IsVisibleSettings && await _studioSmsNotificationSettingsHelper.TfaEnabledForUserAsync(user.Id))// && smsConfirm.ToLower() != "true")
        {
            var confirmType = string.IsNullOrEmpty(user.MobilePhone) ||
                            user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated
                                ? ConfirmType.PhoneActivation
                                : ConfirmType.PhoneAuth;

            return await _commonLinkUtility.GetConfirmationEmailUrlAsync(user.Email, confirmType);
        }

        if (_tfaAppAuthSettingsHelper.IsVisibleSettings && await _tfaAppAuthSettingsHelper.TfaEnabledForUserAsync(user.Id))
        {
            var confirmType = await TfaAppUserSettings.EnableForUserAsync(_settingsManager, _authContext.CurrentAccount.ID)
                ? ConfirmType.TfaAuth
                : ConfirmType.TfaActivation;

            return await _commonLinkUtility.GetConfirmationEmailUrlAsync(user.Email, confirmType);
        }

        return string.Empty;
    }

    [HttpPut("tfaapp")]
    public async Task<bool> TfaSettingsAsync(TfaRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var result = false;

        MessageAction action;

        switch (inDto.Type)
        {
            case "sms":
                if (! await _studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettingsAsync())
                {
                    throw new Exception(Resource.SmsNotAvailable);
                }

                if (!_smsProviderManager.Enabled())
                {
                    throw new MethodAccessException();
                }

                var smsSettings = await _settingsManager.LoadAsync<StudioSmsNotificationSettings>();
                SetSettingsProperty(smsSettings);
                await _settingsManager.SaveAsync(smsSettings);

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

                var appSettings = await _settingsManager.LoadAsync<TfaAppAuthSettings>();
                SetSettingsProperty(appSettings);
                await _settingsManager.SaveAsync(appSettings);


                action = MessageAction.TwoFactorAuthenticationEnabledByTfaApp;

                if (await _studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettingsAsync() && _studioSmsNotificationSettingsHelper.Enable)
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

                if (await _studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettingsAsync() && _studioSmsNotificationSettingsHelper.Enable)
                {
                    _studioSmsNotificationSettingsHelper.Enable = false;
                }

                action = MessageAction.TwoFactorAuthenticationDisabled;

                break;
        }

        if (result)
        {
            await _cookiesManager.ResetTenantCookieAsync();
        }

        await _messageService.SendAsync(action);
        return result;

        void SetSettingsProperty<T>(TfaSettingsBase<T> settings) where T : class, ISettings<T>
        {
            settings.EnableSetting = true;
            settings.TrustedIps = inDto.TrustedIps ?? new List<string>();
            settings.MandatoryUsers = inDto.MandatoryUsers ?? new List<Guid>();
            settings.MandatoryGroups = inDto.MandatoryGroups ?? new List<Guid>();
        }
    }

    [HttpPut("tfaappwithlink")]
    public async Task<object> TfaSettingsLink(TfaRequestsDto inDto)
    {
        if (await TfaSettingsAsync(inDto))
        {
            return await TfaConfirmUrlAsync();
        }

        return string.Empty;
    }

    [HttpGet("tfaapp/setup")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation")]
    public async Task<SetupCode> TfaAppGenerateSetupCodeAsync()
    {
        await ApiContext.AuthByClaimAsync();
        var currentUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings ||
            !(await _settingsManager.LoadAsync<TfaAppAuthSettings>()).EnableSetting ||
            await TfaAppUserSettings.EnableForUserAsync(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (await _userManager.IsOutsiderAsync(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        return await _tfaManager.GenerateSetupCodeAsync(currentUser);
    }

    [HttpGet("tfaappcodes")]
    public async Task<IEnumerable<object>> TfaAppGetCodesAsync()
    {
        var currentUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings || !await TfaAppUserSettings.EnableForUserAsync(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (await _userManager.IsOutsiderAsync(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        return (await _settingsManager.LoadForCurrentUserAsync<TfaAppUserSettings>()).CodesSetting.Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
    }

    [HttpPut("tfaappnewcodes")]
    public async Task<IEnumerable<object>> TfaAppRequestNewCodesAsync()
    {
        var currentUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings || !await TfaAppUserSettings.EnableForUserAsync(_settingsManager, currentUser.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (await _userManager.IsOutsiderAsync(currentUser))
        {
            throw new NotSupportedException("Not available.");
        }

        var codes = (await _tfaManager.GenerateBackupCodesAsync()).Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
        await _messageService.SendAsync(MessageAction.UserConnectedTfaApp, _messageTarget.Create(currentUser.Id), currentUser.DisplayUserName(false, _displayUserSettingsHelper));
        return codes;
    }

    [HttpPut("tfaappnewapp")]
    public async Task<object> TfaAppNewAppAsync(TfaRequestsDto inDto)
    {
        var id = inDto?.Id ?? Guid.Empty;
        var isMe = id.Equals(Guid.Empty) || id.Equals(_authContext.CurrentAccount.ID);

        var user = await _userManager.GetUsersAsync(id);

        if (!isMe && !await _permissionContext.CheckPermissionsAsync(new UserSecurityProvider(user.Id), Constants.Action_EditUser))
        {
            throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);
        }

        if (!_tfaAppAuthSettingsHelper.IsVisibleSettings || !await TfaAppUserSettings.EnableForUserAsync(_settingsManager, user.Id))
        {
            throw new Exception(Resource.TfaAppNotAvailable);
        }

        if (await _userManager.IsOutsiderAsync(user))
        {
            throw new NotSupportedException("Not available.");
        }

        await TfaAppUserSettings.DisableForUserAsync(_settingsManager, user.Id);
        await _messageService.SendAsync(MessageAction.UserDisconnectedTfaApp, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        if (isMe)
        {
            await _cookiesManager.ResetTenantCookieAsync();
            return await _commonLinkUtility.GetConfirmationEmailUrlAsync(user.Email, ConfirmType.TfaActivation);
        }

        await _studioNotifyService.SendMsgTfaResetAsync(user);
        return string.Empty;
    }
}
