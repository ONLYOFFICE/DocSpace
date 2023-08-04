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

namespace ASC.ApiSystem.Controllers;

[Scope]
[ApiController]
[Route("[controller]")]
public class PortalController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SecurityContext _securityContext;
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly ApiSystemHelper _apiSystemHelper;
    private readonly CommonMethods _commonMethods;
    private readonly HostedSolution _hostedSolution;
    private readonly CoreSettings _coreSettings;
    private readonly TenantDomainValidator _tenantDomainValidator;
    private readonly UserFormatter _userFormatter;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly CommonConstants _commonConstants;
    private readonly TimeZonesProvider _timeZonesProvider;
    private readonly TimeZoneConverter _timeZoneConverter;
    private readonly PasswordHasher _passwordHasher;
    private readonly ILogger<PortalController> _log;

    public PortalController(
        IConfiguration configuration,
        SecurityContext securityContext,
        TenantManager tenantManager,
        SettingsManager settingsManager,
        ApiSystemHelper apiSystemHelper,
        CommonMethods commonMethods,
        HostedSolution hostedSolution,
        CoreSettings coreSettings,
        TenantDomainValidator tenantDomainValidator,
        UserFormatter userFormatter,
        UserManagerWrapper userManagerWrapper,
        CommonConstants commonConstants,
        ILogger<PortalController> option,
        TimeZonesProvider timeZonesProvider,
        TimeZoneConverter timeZoneConverter,
        PasswordHasher passwordHasher)
    {
        _configuration = configuration;
        _securityContext = securityContext;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _apiSystemHelper = apiSystemHelper;
        _commonMethods = commonMethods;
        _hostedSolution = hostedSolution;
        _coreSettings = coreSettings;
        _tenantDomainValidator = tenantDomainValidator;
        _userFormatter = userFormatter;
        _userManagerWrapper = userManagerWrapper;
        _commonConstants = commonConstants;
        _timeZonesProvider = timeZonesProvider;
        _timeZoneConverter = timeZoneConverter;
        _passwordHasher = passwordHasher;
        _log = option;
    }

    #region For TEST api

    [HttpGet("test")]
    public IActionResult Check()
    {
        return Ok(new
        {
            value = "Portal api works"
        });
    }

    #endregion

    #region API methods

    [HttpPost("register")]
    [AllowCrossSiteJson]
    [Authorize(AuthenticationSchemes = "auth:allowskip:registerportal")]
    public async ValueTask<IActionResult> RegisterAsync(TenantModel model)
    {
        if (model == null)
        {
            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        if (!ModelState.IsValid)
        {
            var message = new JArray();

            foreach (var k in ModelState.Keys)
            {
                message.Add(ModelState[k].Errors.FirstOrDefault().ErrorMessage);
            }

            return BadRequest(new
            {
                error = "params",
                message
            });
        }

        var sw = Stopwatch.StartNew();

        if (string.IsNullOrEmpty(model.PasswordHash))
        {
            if (!CheckPasswordPolicy(model.Password, out var error1))
            {
                sw.Stop();
                return BadRequest(error1);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                model.PasswordHash = _passwordHasher.GetClientPassword(model.Password);
            }

        }
        model.FirstName = (model.FirstName ?? "").Trim();
        model.LastName = (model.LastName ?? "").Trim();

        if (!CheckValidName(model.FirstName + model.LastName, out var error))
        {
            sw.Stop();

            return BadRequest(error);
        }

        model.PortalName = (model.PortalName ?? "").Trim();
        (var exists, error) = await CheckExistingNamePortalAsync(model.PortalName);

        if (!exists)
        {
            sw.Stop();

            return BadRequest(error);
        }

        _log.LogDebug("PortalName = {0}; Elapsed ms. CheckExistingNamePortal: {1}", model.PortalName, sw.ElapsedMilliseconds);

        var clientIP = _commonMethods.GetClientIp();

        if (_commonMethods.CheckMuchRegistration(model, clientIP, sw))
        {
            return BadRequest(new
            {
                error = "tooMuchAttempts",
                message = "Too much attempts already"
            });
        }

        if (!CheckRecaptcha(model, clientIP, sw, out error))
        {
            return BadRequest(error);
        }

        var language = model.Language ?? string.Empty;

        var tz = _timeZonesProvider.GetCurrentTimeZoneInfo(language);

        _log.LogDebug("PortalName = {0}; Elapsed ms. TimeZonesProvider.GetCurrentTimeZoneInfo: {1}", model.PortalName, sw.ElapsedMilliseconds);

        if (!string.IsNullOrEmpty(model.TimeZoneName))
        {
            tz = _timeZoneConverter.GetTimeZone(model.TimeZoneName.Trim(), false) ?? tz;

            _log.LogDebug("PortalName = {0}; Elapsed ms. TimeZonesProvider.OlsonTimeZoneToTimeZoneInfo: {1}", model.PortalName, sw.ElapsedMilliseconds);
        }

        var lang = _timeZonesProvider.GetCurrentCulture(language);

        _log.LogDebug("PortalName = {0}; model.Language = {1}, resultLang.DisplayName = {2}", model.PortalName, language, lang.DisplayName);

        var info = new TenantRegistrationInfo
        {
            Name = _configuration["web:portal-name"] ?? "",
            Address = model.PortalName,
            Culture = lang,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PasswordHash = string.IsNullOrEmpty(model.PasswordHash) ? null : model.PasswordHash,
            Email = (model.Email ?? "").Trim(),
            TimeZoneInfo = tz,
            MobilePhone = string.IsNullOrEmpty(model.Phone) ? null : model.Phone.Trim(),
            Industry = (TenantIndustry)model.Industry,
            Spam = model.Spam,
            Calls = model.Calls,
        };

        if (!string.IsNullOrEmpty(model.AffiliateId))
        {
            info.AffiliateId = model.AffiliateId;
        }

        if (!string.IsNullOrEmpty(model.Campaign))
        {
            info.Campaign = model.Campaign;
        }

        Tenant t;

        try
        {
            /****REGISTRATION!!!*****/
            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
            {
                await _apiSystemHelper.AddTenantToCacheAsync(info.Address, _securityContext.CurrentAccount.ID);

                _log.LogDebug("PortalName = {0}; Elapsed ms. CacheController.AddTenantToCache: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }

            t = await _hostedSolution.RegisterTenantAsync(info);

            /*********/

            _log.LogDebug("PortalName = {0}; Elapsed ms. HostedSolution.RegisterTenant: {1}", model.PortalName, sw.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            sw.Stop();

            _log.LogError(e, "");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "registerNewTenantError",
                message = e.Message,
                stacktrace = e.StackTrace
            });
        }

        var trialQuota = _configuration["quota:id"];
        if (!string.IsNullOrEmpty(trialQuota))
        {
            if (int.TryParse(trialQuota, out var trialQuotaId))
            {
                var dueDate = DateTime.MaxValue;
                if (int.TryParse(_configuration["quota:due"], out var dueTrial))
                {
                    dueDate = DateTime.UtcNow.AddDays(dueTrial);
                }

                var tariff = new Tariff
                {
                    Quotas = new List<Quota> { new Quota(trialQuotaId, 1) },
                    DueDate = dueDate
                };
                await _hostedSolution.SetTariffAsync(t.Id, tariff);
            }
        }

        var isFirst = true;
        string sendCongratulationsAddress = null;

        if (!string.IsNullOrEmpty(model.PasswordHash))
        {
            isFirst = !_commonMethods.SendCongratulations(Request.Scheme, t, model.SkipWelcome, out sendCongratulationsAddress);
        }
        else if (_configuration["core:base-domain"] == "localhost")
        {
            try
            {
                /* set wizard not completed*/
                _tenantManager.SetCurrentTenant(t);

                var settings = await _settingsManager.LoadAsync<WizardSettings>();

                settings.Completed = false;

                await _settingsManager.SaveAsync(settings);
            }
            catch (Exception e)
            {
                _log.LogError(e, "RegisterAsync");
            }
        }

        var reference = _commonMethods.CreateReference(t.Id, Request.Scheme, t.GetTenantDomain(_coreSettings), info.Email, isFirst);
        _log.LogDebug("PortalName = {0}; Elapsed ms. CreateReferenceByCookie...: {1}", model.PortalName, sw.ElapsedMilliseconds);

        sw.Stop();

        return Ok(new
        {
            reference,
            tenant = _commonMethods.ToTenantWrapper(t),
            referenceWelcome = sendCongratulationsAddress
        });
    }

    [HttpDelete("remove")]
    [AllowCrossSiteJson]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> RemoveAsync([FromQuery] TenantModel model)
    {
        (var succ, var tenant) = await _commonMethods.TryGetTenantAsync(model);
        if (!succ)
        {
            _log.LogError("Model without tenant");

            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        if (tenant == null)
        {
            _log.LogError("Tenant not found");

            return BadRequest(new
            {
                error = "portalNameNotFound",
                message = "Portal not found"
            });
        }

        await _hostedSolution.RemoveTenantAsync(tenant);

        return Ok(new
        {
            tenant = _commonMethods.ToTenantWrapper(tenant)
        });
    }

    [HttpPut("status")]
    [AllowCrossSiteJson]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> ChangeStatusAsync(TenantModel model)
    {
        (var succ, var tenant) = await _commonMethods.TryGetTenantAsync(model);
        if (!succ)
        {
            _log.LogError("Model without tenant");

            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        if (tenant == null)
        {
            _log.LogError("Tenant not found");

            return BadRequest(new
            {
                error = "portalNameNotFound",
                message = "Portal not found"
            });
        }

        var active = model.Status;

        if (active != TenantStatus.Active)
        {
            active = TenantStatus.Suspended;
        }

        tenant.SetStatus(active);

        await _hostedSolution.SaveTenantAsync(tenant);

        return Ok(new
        {
            tenant = _commonMethods.ToTenantWrapper(tenant)
        });
    }

    [HttpPost("validateportalname")]
    [AllowCrossSiteJson]
    public async ValueTask<IActionResult> CheckExistingNamePortalAsync(TenantModel model)
    {
        if (model == null)
        {
            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        var (exists, error) = await CheckExistingNamePortalAsync((model.PortalName ?? "").Trim());

        if (!exists)
        {
            return BadRequest(error);
        }

        return Ok(new
        {
            message = "portalNameReadyToRegister"
        });
    }

    [HttpGet("get")]
    [AllowCrossSiteJson]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> GetPortalsAsync([FromQuery] TenantModel model)
    {
        try
        {
            var tenants = new List<Tenant>();
            var empty = true;

            if (!string.IsNullOrWhiteSpace((model.Email ?? "")))
            {
                empty = false;
                tenants.AddRange(await _hostedSolution.FindTenantsAsync((model.Email ?? "").Trim()));
            }

            if (!string.IsNullOrWhiteSpace((model.PortalName ?? "")))
            {
                empty = false;
                var tenant = (await _hostedSolution.GetTenantAsync((model.PortalName ?? "").Trim()));

                if (tenant != null)
                {
                    tenants.Add(tenant);
                }
            }

            if (model.TenantId.HasValue)
            {
                empty = false;
                var tenant = await _hostedSolution.GetTenantAsync(model.TenantId.Value);

                if (tenant != null)
                {
                    tenants.Add(tenant);
                }
            }

            if (empty)
            {
                tenants.AddRange((await _hostedSolution.GetTenantsAsync(DateTime.MinValue)).OrderBy(t => t.Id).ToList());
            }

            var tenantsWrapper = tenants
                .Distinct()
                .Where(t => t.Status == TenantStatus.Active)
                .OrderBy(t => t.Id)
                .Select(_commonMethods.ToTenantWrapper);

            return Ok(new
            {
                tenants = tenantsWrapper
            });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "error",
                message = ex.Message,
                stacktrace = ex.StackTrace
            });
        }
    }

    #endregion

    #region Validate Method

    private async Task ValidateDomainAsync(string domain)
    {
        // size
        _tenantDomainValidator.ValidateDomainLength(domain);
        // characters
        _tenantDomainValidator.ValidateDomainCharacters(domain);

        var sameAliasTenants = await _apiSystemHelper.FindTenantsInCacheAsync(domain, _securityContext.CurrentAccount.ID);

        if (sameAliasTenants != null)
        {
            throw new TenantAlreadyExistsException("Address busy.", sameAliasTenants);
        }
    }

    private async ValueTask<(bool, object)> CheckExistingNamePortalAsync(string portalName)
    {
        object error = null;
        if (string.IsNullOrEmpty(portalName))
        {
            error = new { error = "portalNameEmpty", message = "PortalName is required" };
            return (false, error);
        }

        try
        {
            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
            {
                await ValidateDomainAsync(portalName.Trim());
            }
            else
            {
                await _hostedSolution.CheckTenantAddressAsync(portalName.Trim());
            }
        }
        catch (TenantAlreadyExistsException ex)
        {
            error = new { error = "portalNameExist", message = "Portal already exists", variants = ex.ExistsTenants.ToArray() };
            return (false, error);
        }
        catch (TenantTooShortException)
        {
            error = new { error = "tooShortError", message = "Portal name is too short" };
            return (false, error);

        }
        catch (TenantIncorrectCharsException)
        {
            error = new { error = "portalNameIncorrect", message = "Unallowable symbols in portalName" };
            return (false, error);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "CheckExistingNamePortal");
            error = new { error = "error", message = ex.Message, stacktrace = ex.StackTrace };
            return (false, error);
        }

        return (true, error);
    }

    private bool CheckValidName(string name, out object error)
    {
        error = null;
        if (string.IsNullOrEmpty(name = (name ?? "").Trim()))
        {
            error = new { error = "error", message = "name is required" };
            return false;
        }

        if (!_userFormatter.IsValidUserName(name, string.Empty))
        {
            error = new { error = "error", message = "name is incorrect" };
            return false;
        }

        return true;
    }

    private bool CheckPasswordPolicy(string pwd, out object error)
    {
        error = null;
        //Validate Password match
        if (string.IsNullOrEmpty(pwd))
        {
            return true;
        }

        var passwordSettings = _settingsManager.GetDefault<PasswordSettings>();

        if (!_userManagerWrapper.CheckPasswordRegex(passwordSettings, pwd))
        {
            error = new { error = "passPolicyError", message = "Password is incorrect" };
            return false;
        }

        return true;
    }


    #region Recaptcha

    private bool CheckRecaptcha(TenantModel model, string clientIP, Stopwatch sw, out object error)
    {
        error = null;
        if (_commonConstants.RecaptchaRequired
            && !_commonMethods.IsTestEmail(model.Email))
        {
            if (!string.IsNullOrEmpty(model.AppKey) && _commonConstants.AppSecretKeys.Contains(model.AppKey))
            {
                _log.LogDebug("PortalName = {0}; Elapsed ms. ValidateRecaptcha via app key: {1}. {2}", model.PortalName, model.AppKey, sw.ElapsedMilliseconds);
                return true;
            }

            var data = $"{model.PortalName} {model.FirstName} {model.LastName} {model.Email} {model.Phone} {model.RecaptchaType}";

            /*** validate recaptcha ***/
            if (!_commonMethods.ValidateRecaptcha(model.RecaptchaResponse, model.RecaptchaType, clientIP))
            {
                _log.LogDebug("PortalName = {0}; Elapsed ms. ValidateRecaptcha error: {1} {2}", model.PortalName, sw.ElapsedMilliseconds, data);
                sw.Stop();

                error = new { error = "recaptchaInvalid", message = "Recaptcha is invalid" };
                return false;

            }

            _log.LogDebug("PortalName = {0}; Elapsed ms. ValidateRecaptcha: {1} {2}", model.PortalName, sw.ElapsedMilliseconds, data);
        }

        return true;
    }

    #endregion

    #endregion
}
