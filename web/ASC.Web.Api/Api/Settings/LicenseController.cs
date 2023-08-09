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

    /// <summary>
    /// Refreshes the license.
    /// </summary>
    /// <short>Refresh the license</short>
    /// <category>License</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/license/refresh</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("license/refresh")]
    [AllowNotPayment]
    public async Task<bool> RefreshLicenseAsync()
    {
        if (!_coreBaseSettings.Standalone)
        {
            return false;
        }

        await _licenseReader.RefreshLicenseAsync();
        return true;
    }

    /// <summary>
    /// Activates a license for the portal.
    /// </summary>
    /// <short>
    /// Activate a license
    /// </short>
    /// <category>License</category>
    /// <returns type="System.Object, System">Message about the result of activating license</returns>
    /// <path>api/2.0/settings/license/accept</path>
    /// <httpMethod>POST</httpMethod>
    [AllowNotPayment]
    [HttpPost("license/accept")]
    public async Task<object> AcceptLicenseAsync()
    {
        if (!_coreBaseSettings.Standalone)
        {
            return "";
        }

        await TariffSettings.SetLicenseAcceptAsync(_settingsManager);
        await _messageService.SendAsync(MessageAction.LicenseKeyUploaded);

        try
        {
            await _licenseReader.RefreshLicenseAsync();
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

    /// <summary>
    /// Activates a trial license for the portal.
    /// </summary>
    /// <short>
    /// Activate a trial license
    /// </short>
    /// <category>License</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/license/trial</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("license/trial")]
    public async Task<bool> ActivateTrialAsync()
    {
        if (!_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException();
        }

        if (!await _userManager.IsDocSpaceAdminAsync(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException();
        }

        var curQuota = await _tenantManager.GetCurrentTenantQuotaAsync();
        if (curQuota.TenantId != Tenant.DefaultTenant)
        {
            return false;
        }

        if (curQuota.Trial)
        {
            return false;
        }

        var curTariff = await _tenantExtra.GetCurrentTariffAsync();
        if (curTariff.DueDate.Date != DateTime.MaxValue.Date)
        {
            return false;
        }

        var quota = new TenantQuota(-1000)
        {
            Name = "apirequest",
            CountUser = curQuota.CountUser,
            MaxFileSize = curQuota.MaxFileSize,
            MaxTotalSize = curQuota.MaxTotalSize,
            Features = curQuota.Features
        };
        quota.Trial = true;

        await _tenantManager.SaveTenantQuotaAsync(quota);

        const int DEFAULT_TRIAL_PERIOD = 30;

        var tariff = new Tariff
        {
            Quotas = new List<Quota> { new Quota(quota.TenantId, 1) },
            DueDate = DateTime.Today.AddDays(DEFAULT_TRIAL_PERIOD)
        };

        await _tariffService.SetTariffAsync(Tenant.DefaultTenant, tariff, new List<TenantQuota>() { quota });

        await _messageService.SendAsync(MessageAction.LicenseKeyUploaded);

        return true;
    }

    /// <summary>
    /// Requests a portal license if necessary.
    /// </summary>
    /// <short>
    /// Request a license
    /// </short>
    /// <category>License</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the license is required</returns>
    /// <path>api/2.0/settings/license/required</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [AllowNotPayment]
    [HttpGet("license/required")]
    public bool RequestLicense()
    {
        return _firstTimeTenantSettings.RequestLicense;
    }


    /// <summary>
    /// Uploads a portal license specified in the request.
    /// </summary>
    /// <short>
    /// Upload a license
    /// </short>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.UploadLicenseRequestsDto, ASC.Web.Api" name="inDto">Request parameters to upload a license</param>
    /// <category>License</category>
    /// <returns type="System.Object, System">License</returns>
    /// <path>api/2.0/settings/license</path>
    /// <httpMethod>POST</httpMethod>
    [AllowNotPayment]
    [HttpPost("license")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard, Administrators")]
    public async Task<object> UploadLicenseAsync([FromForm] UploadLicenseRequestsDto inDto)
    {
        try
        {
            await ApiContext.AuthByClaimAsync();
            if (!_authContext.IsAuthenticated && (await _settingsManager.LoadAsync<WizardSettings>()).Completed)
            {
                throw new SecurityException(Resource.PortalSecurity);
            }

            if (!_coreBaseSettings.Standalone)
            {
                throw new NotSupportedException();
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
                                        (await _tenantManager.GetCurrentTenantQuotaAsync()).Update
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