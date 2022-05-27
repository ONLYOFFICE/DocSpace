/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

using ASC.ApiSystem.Classes;
using ASC.ApiSystem.Models;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;


namespace ASC.ApiSystem.Controllers
{
    [Scope]
    [ApiController]
    [Route("[controller]")]
    public class PortalController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        private Core.SecurityContext SecurityContext { get; }
        private TenantManager TenantManager { get; }
        private SettingsManager SettingsManager { get; }
        private ApiSystemHelper ApiSystemHelper { get; }
        private CommonMethods CommonMethods { get; }
        private HostedSolution HostedSolution { get; }
        private CoreSettings CoreSettings { get; }
        private TenantDomainValidator TenantDomainValidator { get; }
        private UserFormatter UserFormatter { get; }
        private UserManagerWrapper UserManagerWrapper { get; }
        private CommonConstants CommonConstants { get; }
        private TimeZonesProvider TimeZonesProvider { get; }
        private TimeZoneConverter TimeZoneConverter { get; }
        public PasswordHasher PasswordHasher { get; }
        private ILog Log { get; }

        public PortalController(
            IConfiguration configuration,
            Core.SecurityContext securityContext,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            ApiSystemHelper apiSystemHelper,
            CommonMethods commonMethods,
            IOptionsSnapshot<HostedSolution> hostedSolutionOptions,
            CoreSettings coreSettings,
            TenantDomainValidator tenantDomainValidator,
            UserFormatter userFormatter,
            UserManagerWrapper userManagerWrapper,
            CommonConstants commonConstants,
            IOptionsMonitor<ILog> option,
            TimeZonesProvider timeZonesProvider,
            TimeZoneConverter timeZoneConverter,
            PasswordHasher passwordHasher)
        {
            Configuration = configuration;
            SecurityContext = securityContext;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            ApiSystemHelper = apiSystemHelper;
            CommonMethods = commonMethods;
            HostedSolution = hostedSolutionOptions.Value;
            CoreSettings = coreSettings;
            TenantDomainValidator = tenantDomainValidator;
            UserFormatter = userFormatter;
            UserManagerWrapper = userManagerWrapper;
            CommonConstants = commonConstants;
            TimeZonesProvider = timeZonesProvider;
            TimeZoneConverter = timeZoneConverter;
            PasswordHasher = passwordHasher;
            Log = option.Get("ASC.ApiSystem");
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
        [Authorize(AuthenticationSchemes = "auth.allowskip.registerportal")]
        public Task<IActionResult> RegisterAsync(TenantModel model)
        {
            if (model == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                }));
            }

            if (!ModelState.IsValid)
            {
                var message = new JArray();

                foreach (var k in ModelState.Keys)
                {
                    message.Add(ModelState[k].Errors.FirstOrDefault().ErrorMessage);
                }

                return Task.FromResult<IActionResult>(BadRequest(new
                {
                    error = "params",
                    message
                }));
            }

            var sw = Stopwatch.StartNew();

            if (string.IsNullOrEmpty(model.PasswordHash))
            {
                if (!CheckPasswordPolicy(model.Password, out var error1))
                {
                    sw.Stop();
                    return Task.FromResult<IActionResult>(BadRequest(error1));
                }

                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.PasswordHash = PasswordHasher.GetClientPassword(model.Password);
                }

            }
            model.FirstName = (model.FirstName ?? "").Trim();
            model.LastName = (model.LastName ?? "").Trim();

            if (!CheckValidName(model.FirstName + model.LastName, out var error))
            {
                sw.Stop();

                return Task.FromResult<IActionResult>(BadRequest(error));
            }

            return InternalRegisterAsync(model, error, sw);
        }

        private async Task<IActionResult> InternalRegisterAsync(TenantModel model, object error, Stopwatch sw)
        {
            model.PortalName = (model.PortalName ?? "").Trim();
            var (exists, _) = await CheckExistingNamePortalAsync(model.PortalName);

            if (!exists)
            {
                sw.Stop();

                return BadRequest(error);
            }

            Log.DebugFormat("PortalName = {0}; Elapsed ms. CheckExistingNamePortal: {1}", model.PortalName, sw.ElapsedMilliseconds);

            var clientIP = CommonMethods.GetClientIp();

            if (CommonMethods.CheckMuchRegistration(model, clientIP, sw))
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

            if (!CheckRegistrationPayment(out error))
            {
                return BadRequest(error);
            }

            var language = model.Language ?? string.Empty;

            var tz = TimeZonesProvider.GetCurrentTimeZoneInfo(language);

            Log.DebugFormat("PortalName = {0}; Elapsed ms. TimeZonesProvider.GetCurrentTimeZoneInfo: {1}", model.PortalName, sw.ElapsedMilliseconds);

            if (!string.IsNullOrEmpty(model.TimeZoneName))
            {
                tz = TimeZoneConverter.GetTimeZone(model.TimeZoneName.Trim(), false) ?? tz;

                Log.DebugFormat("PortalName = {0}; Elapsed ms. TimeZonesProvider.OlsonTimeZoneToTimeZoneInfo: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }

            var lang = TimeZonesProvider.GetCurrentCulture(language);

            Log.DebugFormat("PortalName = {0}; model.Language = {1}, resultLang.DisplayName = {2}", model.PortalName, language, lang.DisplayName);

            var info = new TenantRegistrationInfo
            {
                Name = Configuration["web:portal-name"] ?? "Cloud Office Applications",
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
                LimitedControlPanel = model.LimitedControlPanel
            };

            if (!string.IsNullOrEmpty(model.PartnerId))
            {
                if (Guid.TryParse(model.PartnerId, out _))
                {
                    // valid guid
                    info.PartnerId = model.PartnerId;
                }
            }

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
                if (!string.IsNullOrEmpty(ApiSystemHelper.ApiCacheUrl))
                {
                    await ApiSystemHelper.AddTenantToCacheAsync(info.Address, SecurityContext.CurrentAccount.ID);

                    Log.DebugFormat("PortalName = {0}; Elapsed ms. CacheController.AddTenantToCache: {1}", model.PortalName, sw.ElapsedMilliseconds);
                }

                HostedSolution.RegisterTenant(info, out t);

                /*********/

                Log.DebugFormat("PortalName = {0}; Elapsed ms. HostedSolution.RegisterTenant: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                sw.Stop();

                Log.Error(e);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "registerNewTenantError",
                    message = e.Message,
                    stacktrace = e.StackTrace
                });
            }

            var trialQuota = Configuration["trial-quota"];
            if (!string.IsNullOrEmpty(trialQuota))
            {
                if (int.TryParse(trialQuota, out var trialQuotaId))
                {
                    var dueDate = DateTime.MaxValue;
                    if (int.TryParse(Configuration["trial-due"], out var dueTrial))
                    {
                        dueDate = DateTime.UtcNow.AddDays(dueTrial);
                    }

                    var tariff = new Tariff
                    {
                        QuotaId = trialQuotaId,
                        DueDate = dueDate
                    };
                    HostedSolution.SetTariff(t.TenantId, tariff);
                }
            }


            var isFirst = true;
            string sendCongratulationsAddress = null;

            if (!string.IsNullOrEmpty(model.PasswordHash))
            {
                isFirst = !CommonMethods.SendCongratulations(Request.Scheme, t, model.SkipWelcome, out sendCongratulationsAddress);
            }
            else if (Configuration["core:base-domain"] == "localhost")
            {
                try
                {
                    /* set wizard not completed*/
                    TenantManager.SetCurrentTenant(t);

                    var settings = SettingsManager.Load<WizardSettings>();

                    settings.Completed = false;

                    SettingsManager.Save(settings);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            var reference = CommonMethods.CreateReference(Request.Scheme, t.GetTenantDomain(CoreSettings), info.Email, isFirst);

            Log.DebugFormat("PortalName = {0}; Elapsed ms. CreateReferenceByCookie...: {1}", model.PortalName, sw.ElapsedMilliseconds);

            sw.Stop();

            return Ok(new
            {
                reference,
                tenant = CommonMethods.ToTenantWrapper(t),
                referenceWelcome = sendCongratulationsAddress
            });
        }

        [HttpDelete("remove")]
        [AllowCrossSiteJson]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult Remove([FromQuery] TenantModel model)
        {
            if (!CommonMethods.GetTenant(model, out var tenant))
            {
                Log.Error("Model without tenant");

                return BadRequest(new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                });
            }

            if (tenant == null)
            {
                Log.Error("Tenant not found");

                return BadRequest(new
                {
                    error = "portalNameNotFound",
                    message = "Portal not found"
                });
            }

            HostedSolution.RemoveTenant(tenant);

            return Ok(new
            {
                tenant = CommonMethods.ToTenantWrapper(tenant)
            });
        }

        [HttpPut("status")]
        [AllowCrossSiteJson]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult ChangeStatus(TenantModel model)
        {
            if (!CommonMethods.GetTenant(model, out var tenant))
            {
                Log.Error("Model without tenant");

                return BadRequest(new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                });
            }

            if (tenant == null)
            {
                Log.Error("Tenant not found");

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

            HostedSolution.SaveTenant(tenant);

            return Ok(new
            {
                tenant = CommonMethods.ToTenantWrapper(tenant)
            });
        }

        [HttpPost("validateportalname")]
        [AllowCrossSiteJson]
        public Task<IActionResult> CheckExistingNamePortalAsync(TenantModel model)
        {
            if (model == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                }));
            }

            return InternalCheckExistingNamePortalAsync(model);
        }

        private async Task<IActionResult> InternalCheckExistingNamePortalAsync(TenantModel model)
        {
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
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult GetPortals([FromQuery] TenantModel model)
        {
            try
            {
                var tenants = new List<Tenant>();
                var empty = true;

                if (!string.IsNullOrWhiteSpace((model.Email ?? "")))
                {
                    empty = false;
                    tenants.AddRange(HostedSolution.FindTenants((model.Email ?? "").Trim()));
                }

                if (!string.IsNullOrWhiteSpace((model.PortalName ?? "")))
                {
                    empty = false;
                    var tenant = HostedSolution.GetTenant((model.PortalName ?? "").Trim());

                    if (tenant != null)
                    {
                        tenants.Add(tenant);
                    }
                }

                if (model.TenantId.HasValue)
                {
                    empty = false;
                    var tenant = HostedSolution.GetTenant(model.TenantId.Value);

                    if (tenant != null)
                    {
                        tenants.Add(tenant);
                    }
                }

                if (empty)
                {
                    tenants.AddRange(HostedSolution.GetTenants(DateTime.MinValue).OrderBy(t => t.TenantId).ToList());
                }

                var tenantsWrapper = tenants
                    .Distinct()
                    .Where(t => t.Status == TenantStatus.Active)
                    .OrderBy(t => t.TenantId)
                    .Select(CommonMethods.ToTenantWrapper);

                return Ok(new
                {
                    tenants = tenantsWrapper
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);

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
            TenantDomainValidator.ValidateDomainLength(domain);
            // characters
            TenantDomainValidator.ValidateDomainCharacters(domain);

            var sameAliasTenants = await ApiSystemHelper.FindTenantsInCacheAsync(domain, SecurityContext.CurrentAccount.ID);

            if (sameAliasTenants != null)
            {
                throw new TenantAlreadyExistsException("Address busy.", sameAliasTenants);
            }
        }

        private Task<(bool exists, object error)> CheckExistingNamePortalAsync(string portalName)
        {
            if (string.IsNullOrEmpty(portalName))
            {
                object error = new { error = "portalNameEmpty", message = "PortalName is required" };
                return Task.FromResult((false, error));
            }

            return internalCheckExistingNamePortalAsync(portalName);
        }

        private async Task<(bool exists, object error)> internalCheckExistingNamePortalAsync(string portalName)
        {
            object error = null;
            try
            {
                if (!string.IsNullOrEmpty(ApiSystemHelper.ApiCacheUrl))
                {
                    await ValidateDomainAsync(portalName.Trim());
                }
                else
                {
                    HostedSolution.CheckTenantAddress(portalName.Trim());
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
                Log.Error(ex);
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

            if (!UserFormatter.IsValidUserName(name, string.Empty))
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

            var passwordSettings = (PasswordSettings)new PasswordSettings().GetDefault(Configuration);

            if (!UserManagerWrapper.CheckPasswordRegex(passwordSettings, pwd))
            {
                error = new { error = "passPolicyError", message = "Password is incorrect" };
                return false;
            }

            return true;
        }

        private bool CheckRegistrationPayment(out object error)
        {
            error = null;
            if (Configuration["core:base-domain"] == "localhost")
            {
                var tenants = HostedSolution.GetTenants(DateTime.MinValue);
                var firstTenant = tenants.FirstOrDefault();
                if (firstTenant != null)
                {
                    var activePortals = tenants.Count(r => r.Status != TenantStatus.Suspended && r.Status != TenantStatus.RemovePending);

                    var quota = HostedSolution.GetTenantQuota(firstTenant.TenantId);
                    if (quota.CountPortals > 0 && quota.CountPortals <= activePortals)
                    {
                        error = new { error = "portalsCountTooMuch", message = "Too much portals registered already", };
                        return false;
                    }
                }
            }
            return true;
        }


        #region Recaptcha

        private bool CheckRecaptcha(TenantModel model, string clientIP, Stopwatch sw, out object error)
        {
            error = null;
            if (CommonConstants.RecaptchaRequired
                && !CommonMethods.IsTestEmail(model.Email))
            {
                if (!string.IsNullOrEmpty(model.AppKey) && CommonConstants.AppSecretKeys.Contains(model.AppKey))
                {
                    Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha via app key: {1}. {2}", model.PortalName, model.AppKey, sw.ElapsedMilliseconds);
                    return true;
                }

                var data = $"{model.PortalName} {model.FirstName} {model.LastName} {model.Email} {model.Phone} {model.RecaptchaType}";

                /*** validate recaptcha ***/
                if (!CommonMethods.ValidateRecaptcha(model.RecaptchaResponse, model.RecaptchaType, clientIP))
                {
                    Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha error: {1} {2}", model.PortalName, sw.ElapsedMilliseconds, data);
                    sw.Stop();

                    error = new { error = "recaptchaInvalid", message = "Recaptcha is invalid" };
                    return false;

                }

                Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha: {1} {2}", model.PortalName, sw.ElapsedMilliseconds, data);
            }

            return true;
        }

        #endregion

        #endregion
    }
}