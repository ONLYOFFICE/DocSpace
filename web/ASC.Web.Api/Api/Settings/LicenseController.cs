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

namespace ASC.Web.Api.Controllers.Settings;

public class LicenseController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly FirstTimeTenantSettings _firstTimeTenantSettings;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly AuthContext _authContext;
    private readonly LicenseReader _licenseReader;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ILogger _log;
    private readonly ITariffService _tariffService;

    public LicenseController(
        ILoggerProvider option,
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
        ITariffService tariffService,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _log = option.CreateLogger("ASC.Api");
        _firstTimeTenantSettings = firstTimeTenantSettings;
        _messageService = messageService;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _authContext = authContext;
        _licenseReader = licenseReader;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
        _tariffService = tariffService;
    }

    [HttpGet("license/refresh")]
    [AllowNotPayment]
    public bool RefreshLicense()
    {
        if (!_coreBaseSettings.Standalone)
        {
            return false;
        }

        _licenseReader.RefreshLicense();
        return true;
    }

    [AllowNotPayment]
    [HttpPost("license/accept")]
    public object AcceptLicense()
    {
        if (!_coreBaseSettings.Standalone)
        {
            return "";
        }

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
    [HttpPost("license/trial")]
    public bool ActivateTrial()
    {
        if (!_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException();
        }

        if (!_userManager.GetUsers(_authContext.CurrentAccount.ID).IsAdmin(_userManager))
        {
            throw new SecurityException();
        }

        var curQuota = _tenantExtra.GetTenantQuota();
        if (curQuota.Tenant != Tenant.DefaultTenant)
        {
            return false;
        }

        if (curQuota.Trial)
        {
            return false;
        }

        var curTariff = _tenantExtra.GetCurrentTariff();
        if (curTariff.DueDate.Date != DateTime.MaxValue.Date)
        {
            return false;
        }

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
            Quotas = new List<Tuple<int, int>> { new Tuple<int, int>(quota.Tenant, 1) },
            DueDate = DateTime.Today.AddDays(DEFAULT_TRIAL_PERIOD)
        };

        _tariffService.SetTariff(-1, tariff);

        _messageService.Send(MessageAction.LicenseKeyUploaded);

        return true;
    }

    [AllowAnonymous]
    [AllowNotPayment]
    [HttpGet("license/required")]
    public bool RequestLicense()
    {
        return _firstTimeTenantSettings.RequestLicense;
    }


    [AllowNotPayment]
    [HttpPost("license")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard, Administrators")]
    public object UploadLicense([FromForm] UploadLicenseRequestsDto inDto)
    {
        try
        {
            ApiContext.AuthByClaim();
            if (!_authContext.IsAuthenticated && _settingsManager.Load<WizardSettings>().Completed)
            {
                throw new SecurityException(Resource.PortalSecurity);
            }

            if (!inDto.Files.Any())
            {
                throw new Exception(Resource.ErrorEmptyUploadFileSelected);
            }

            var licenseFile = inDto.Files.First();
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
            _log.ErrorLicenseUpload(ex);
            throw new Exception(Resource.LicenseErrorExpired);
        }
        catch (LicenseQuotaException ex)
        {
            _log.ErrorLicenseUpload(ex);
            throw new Exception(Resource.LicenseErrorQuota);
        }
        catch (LicensePortalException ex)
        {
            _log.ErrorLicenseUpload(ex);
            throw new Exception(Resource.LicenseErrorPortal);
        }
        catch (Exception ex)
        {
            _log.ErrorLicenseUpload(ex);
            throw new Exception(Resource.LicenseError);
        }
    }
}