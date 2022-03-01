using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class LicenseController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly FirstTimeTenantSettings _firstTimeTenantSettings;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly AuthContext _authContext;
    private readonly LicenseReader _licenseReader;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ILog _log;
    private readonly PaymentManager _paymentManager;

    public LicenseController(
        IOptionsMonitor<ILog> option,
        MessageService messageService,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        AuthContext authContext,
        LicenseReader licenseReader,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        CoreBaseSettings coreBaseSettings,
        IMemoryCache memoryCache,
        FirstTimeTenantSettings firstTimeTenantSettings,
        PaymentManager paymentManager) : base(apiContext, memoryCache, webItemManager)
    {
        _log = option.Get("ASC.Api");
        _firstTimeTenantSettings = firstTimeTenantSettings;
        _messageService = messageService;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _authContext = authContext;
        _licenseReader = licenseReader;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
        _paymentManager = paymentManager;
    }

    [Read("license/refresh", Check = false)]
    public bool RefreshLicense()
    {
        if (!_coreBaseSettings.Standalone) return false;
        _licenseReader.RefreshLicense();
        return true;
    }

    [Create("license/accept", Check = false)]
    public object AcceptLicense()
    {
        if (!_coreBaseSettings.Standalone) return "";

        TariffSettings.SetLicenseAccept(_settingsManager);
        _messageService.Send(MessageAction.LicenseKeyUploaded);

        try
        {
            _licenseReader.RefreshLicense();
        }
        catch (BillingNotFoundException)
        {
            return UserControlsCommonResource.LicenseKeyNotFound;
        }
        catch (BillingNotConfiguredException)
        {
            return UserControlsCommonResource.LicenseKeyNotCorrect;
        }
        catch (BillingException)
        {
            return UserControlsCommonResource.LicenseException;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "";
    }

    ///<visible>false</visible>
    [Create("license/trial")]
    public bool ActivateTrial()
    {
        if (!_coreBaseSettings.Standalone) throw new NotSupportedException();
        if (!_userManager.GetUsers(_authContext.CurrentAccount.ID).IsAdmin(_userManager)) throw new SecurityException();

        var curQuota = _tenantExtra.GetTenantQuota();
        if (curQuota.Tenant != Tenant.DefaultTenant) return false;
        if (curQuota.Trial) return false;

        var curTariff = _tenantExtra.GetCurrentTariff();
        if (curTariff.DueDate.Date != DateTime.MaxValue.Date) return false;

        var quota = new TenantQuota(-1000)
        {
            Name = "apirequest",
            ActiveUsers = curQuota.ActiveUsers,
            MaxFileSize = curQuota.MaxFileSize,
            MaxTotalSize = curQuota.MaxTotalSize,
            Features = curQuota.Features
        };
        quota.Trial = true;

        _tenantManager.SaveTenantQuota(quota);

        const int DEFAULT_TRIAL_PERIOD = 30;

        var tariff = new Tariff
        {
            QuotaId = quota.Tenant,
            DueDate = DateTime.Today.AddDays(DEFAULT_TRIAL_PERIOD)
        };

        _paymentManager.SetTariff(-1, tariff);

        _messageService.Send(MessageAction.LicenseKeyUploaded);

        return true;
    }

    [AllowAnonymous]
    [Read("license/required", Check = false)]
    public bool RequestLicense()
    {
        return _firstTimeTenantSettings.RequestLicense;
    }


    [Create("license", Check = false)]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard, Administrators")]
    public object UploadLicense([FromForm] UploadLicenseDto model)
    {
        try
        {
            _apiContext.AuthByClaim();
            if (!_authContext.IsAuthenticated && _settingsManager.Load<WizardSettings>().Completed) throw new SecurityException(Resource.PortalSecurity);
            if (!model.Files.Any()) throw new Exception(Resource.ErrorEmptyUploadFileSelected);



            var licenseFile = model.Files.First();
            var dueDate = _licenseReader.SaveLicenseTemp(licenseFile.OpenReadStream());

            return dueDate >= DateTime.UtcNow.Date
                                    ? Resource.LicenseUploaded
                                    : string.Format(
                                        _tenantExtra.GetTenantQuota().Update
                                            ? Resource.LicenseUploadedOverdueSupport
                                            : Resource.LicenseUploadedOverdue,
                                                    "",
                                                    "",
                                                    dueDate.Date.ToLongDateString());
        }
        catch (LicenseExpiredException ex)
        {
            _log.Error("License upload", ex);
            throw new Exception(Resource.LicenseErrorExpired);
        }
        catch (LicenseQuotaException ex)
        {
            _log.Error("License upload", ex);
            throw new Exception(Resource.LicenseErrorQuota);
        }
        catch (LicensePortalException ex)
        {
            _log.Error("License upload", ex);
            throw new Exception(Resource.LicenseErrorPortal);
        }
        catch (Exception ex)
        {
            _log.Error("License upload", ex);
            throw new Exception(Resource.LicenseError);
        }
    }
}