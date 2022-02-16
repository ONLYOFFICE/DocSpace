using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class LicenseController: BaseSettingsController
{
    public LicenseController(IOptionsMonitor<ILog> option,
    MessageService messageService,
    StudioNotifyService studioNotifyService,
    ApiContext apiContext,
    UserManager userManager,
    TenantManager tenantManager,
    TenantExtra tenantExtra,
    TenantStatisticsProvider tenantStatisticsProvider,
    AuthContext authContext,
    CookiesManager cookiesManager,
    WebItemSecurity webItemSecurity,
    StudioNotifyHelper studioNotifyHelper,
    LicenseReader licenseReader,
    PermissionContext permissionContext,
    SettingsManager settingsManager,
    TfaManager tfaManager,
    WebItemManager webItemManager,
    WebItemManagerSecurity webItemManagerSecurity,
    TenantInfoSettingsHelper tenantInfoSettingsHelper,
    TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
    StorageHelper storageHelper,
    TenantLogoManager tenantLogoManager,
    TenantUtil tenantUtil,
    CoreBaseSettings coreBaseSettings,
    CommonLinkUtility commonLinkUtility,
    ColorThemesSettingsHelper colorThemesSettingsHelper,
    IConfiguration configuration,
    SetupInfo setupInfo,
    BuildVersion buildVersion,
    DisplayUserSettingsHelper displayUserSettingsHelper,
    StatisticManager statisticManager,
    IPRestrictionsService iPRestrictionsService,
    CoreConfiguration coreConfiguration,
    MessageTarget messageTarget,
    StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
    CoreSettings coreSettings,
    StorageSettingsHelper storageSettingsHelper,
    IWebHostEnvironment webHostEnvironment,
    IServiceProvider serviceProvider,
    EmployeeWraperHelper employeeWraperHelper,
    ConsumerFactory consumerFactory,
    SmsProviderManager smsProviderManager,
    TimeZoneConverter timeZoneConverter,
    CustomNamingPeople customNamingPeople,
    IPSecurity.IPSecurity ipSecurity,
    IMemoryCache memoryCache,
    ProviderManager providerManager,
    FirstTimeTenantSettings firstTimeTenantSettings,
    ServiceClient serviceClient,
    TelegramHelper telegramHelper,
    StorageFactory storageFactory,
    UrlShortener urlShortener,
    EncryptionServiceClient encryptionServiceClient,
    EncryptionSettingsHelper encryptionSettingsHelper,
    BackupAjaxHandler backupAjaxHandler,
    ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
    EncryptionWorker encryptionWorker,
    PasswordHasher passwordHasher,
    PaymentManager paymentManager,
    Constants constants,
    InstanceCrypto instanceCrypto,
    Signature signature,
    DbWorker dbWorker,
    IHttpClientFactory clientFactory) : base(option, messageService, studioNotifyService, apiContext, userManager, tenantManager, tenantExtra, tenantStatisticsProvider, authContext, cookiesManager, webItemSecurity, studioNotifyHelper, licenseReader, permissionContext, settingsManager, tfaManager, webItemManager, webItemManagerSecurity, tenantInfoSettingsHelper, tenantWhiteLabelSettingsHelper, storageHelper, tenantLogoManager, tenantUtil, coreBaseSettings, commonLinkUtility, colorThemesSettingsHelper, configuration, setupInfo, buildVersion, displayUserSettingsHelper, statisticManager, iPRestrictionsService, coreConfiguration, messageTarget, studioSmsNotificationSettingsHelper, coreSettings, storageSettingsHelper, webHostEnvironment, serviceProvider, employeeWraperHelper, consumerFactory, smsProviderManager, timeZoneConverter, customNamingPeople, ipSecurity, memoryCache, providerManager, firstTimeTenantSettings, serviceClient, telegramHelper, storageFactory, urlShortener, encryptionServiceClient, encryptionSettingsHelper, backupAjaxHandler, cacheDeleteSchedule, encryptionWorker, passwordHasher, paymentManager, constants, instanceCrypto, signature, dbWorker, clientFactory)
    {
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
        if (curQuota.Id != Tenant.DEFAULT_TENANT) return false;
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
            QuotaId = quota.Id,
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