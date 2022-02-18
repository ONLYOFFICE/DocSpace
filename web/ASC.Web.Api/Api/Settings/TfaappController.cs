using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class TfaappController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly IServiceProvider _serviceProvider;
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
        IServiceProvider serviceProvider,
        SmsProviderManager smsProviderManager,
        IMemoryCache memoryCache,
        InstanceCrypto instanceCrypto,
        Signature signature) : base(apiContext, memoryCache, webItemManager)
    {
        _serviceProvider = serviceProvider;
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
    }

    [Read("tfaapp")]
    public IEnumerable<TfaSettingsDto> GetTfaSettings()
    {
        var result = new List<TfaSettingsDto>();

        var SmsVisible = _studioSmsNotificationSettingsHelper.IsVisibleSettings();
        var SmsEnable = SmsVisible && _smsProviderManager.Enabled();
        var TfaVisible = TfaAppAuthSettings.IsVisibleSettings;

        if (SmsVisible)
        {
            result.Add(new TfaSettingsDto
            {
                Enabled = _studioSmsNotificationSettingsHelper.Enable,
                Id = "sms",
                Title = Resource.ButtonSmsEnable,
                Avaliable = SmsEnable
            });
        }

        if (TfaVisible)
        {
            result.Add(new TfaSettingsDto
            {
                Enabled = _settingsManager.Load<TfaAppAuthSettings>().EnableSetting,
                Id = "app",
                Title = Resource.ButtonTfaAppEnable,
                Avaliable = true
            });
        }

        return result;
    }

    [Create("tfaapp/validate")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation,Everyone")]
    public bool TfaValidateAuthCode(TfaValidateDto model)
    {
        _apiContext.AuthByClaim();
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        return _tfaManager.ValidateAuthCode(user, model.Code);
    }

    [Read("tfaapp/confirm")]
    public object TfaConfirmUrl()
    {
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)// && smsConfirm.ToLower() != "true")
        {
            var confirmType = string.IsNullOrEmpty(user.MobilePhone) ||
                            user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated
                                ? ConfirmType.PhoneActivation
                                : ConfirmType.PhoneAuth;

            return _commonLinkUtility.GetConfirmationUrl(user.Email, confirmType);
        }

        if (TfaAppAuthSettings.IsVisibleSettings && _settingsManager.Load<TfaAppAuthSettings>().EnableSetting)
        {
            var confirmType = TfaAppUserSettings.EnableForUser(_settingsManager, _authContext.CurrentAccount.ID)
                ? ConfirmType.TfaAuth
                : ConfirmType.TfaActivation;

            return _commonLinkUtility.GetConfirmationUrl(user.Email, confirmType);
        }

        return string.Empty;
    }

    [Update("tfaapp")]
    public bool TfaSettingsFromBody([FromBody] TfaDto model)
    {
        return TfaSettingsUpdate(model);
    }

    [Update("tfaapp")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool TfaSettingsFromForm([FromForm] TfaDto model)
    {
        return TfaSettingsUpdate(model);
    }

    private bool TfaSettingsUpdate(TfaDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var result = false;

        MessageAction action;
        var settings = _settingsManager.Load<TfaAppAuthSettings>();

        switch (model.Type)
        {
            case "sms":
                if (!_studioSmsNotificationSettingsHelper.IsVisibleSettings())
                    throw new Exception(Resource.SmsNotAvailable);

                if (!_smsProviderManager.Enabled())
                    throw new MethodAccessException();

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

                if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)
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

                if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)
                {
                    _studioSmsNotificationSettingsHelper.Enable = false;
                }

                action = MessageAction.TwoFactorAuthenticationDisabled;

                break;
        }

        if (result)
        {
            _cookiesManager.ResetTenantCookie();
        }

        _messageService.Send(action);
        return result;
    }

    [Read("tfaapp/setup")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation")]
    public SetupCode TfaAppGenerateSetupCode()
    {
        _apiContext.AuthByClaim();
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!TfaAppAuthSettings.IsVisibleSettings ||
            !_settingsManager.Load<TfaAppAuthSettings>().EnableSetting ||
            TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.ID))
            throw new Exception(Resource.TfaAppNotAvailable);

        if (currentUser.IsVisitor(_userManager) || currentUser.IsOutsider(_userManager))
            throw new NotSupportedException("Not available.");

        return _tfaManager.GenerateSetupCode(currentUser);
    }

    [Read("tfaappcodes")]
    public IEnumerable<object> TfaAppGetCodes()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.ID))
            throw new Exception(Resource.TfaAppNotAvailable);

        if (currentUser.IsVisitor(_userManager) || currentUser.IsOutsider(_userManager))
            throw new NotSupportedException("Not available.");

        return _settingsManager.LoadForCurrentUser<TfaAppUserSettings>().CodesSetting.Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
    }

    [Update("tfaappnewcodes")]
    public IEnumerable<object> TfaAppRequestNewCodes()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, currentUser.ID))
            throw new Exception(Resource.TfaAppNotAvailable);

        if (currentUser.IsVisitor(_userManager) || currentUser.IsOutsider(_userManager))
            throw new NotSupportedException("Not available.");

        var codes = _tfaManager.GenerateBackupCodes().Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(_instanceCrypto, _signature) }).ToList();
        _messageService.Send(MessageAction.UserConnectedTfaApp, _messageTarget.Create(currentUser.ID), currentUser.DisplayUserName(false, _displayUserSettingsHelper));
        return codes;
    }

    [Update("tfaappnewapp")]
    public object TfaAppNewAppFromBody([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] TfaDto model)
    {
        return TfaAppNewApp(model);
    }

    [Update("tfaappnewapp")]
    [Consumes("application/x-www-form-urlencoded")]
    public object TfaAppNewAppFromForm([FromForm] TfaDto model)
    {
        return TfaAppNewApp(model);
    }

    private object TfaAppNewApp(TfaDto model)
    {
        var id = model?.Id ?? Guid.Empty;
        var isMe = id.Equals(Guid.Empty);
        var user = _userManager.GetUsers(isMe ? _authContext.CurrentAccount.ID : id);

        if (!isMe && !_permissionContext.CheckPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser))
            throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);

        if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(_settingsManager, user.ID))
            throw new Exception(Resource.TfaAppNotAvailable);

        if (user.IsVisitor(_userManager) || user.IsOutsider(_userManager))
            throw new NotSupportedException("Not available.");

        TfaAppUserSettings.DisableForUser(_serviceProvider, _settingsManager, user.ID);
        _messageService.Send(MessageAction.UserDisconnectedTfaApp, _messageTarget.Create(user.ID), user.DisplayUserName(false, _displayUserSettingsHelper));

        if (isMe)
        {
            return _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaActivation);
        }

        _studioNotifyService.SendMsgTfaReset(user);
        return string.Empty;
    }
}
