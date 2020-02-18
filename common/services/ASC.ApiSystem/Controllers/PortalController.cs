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


using ASC.ApiSystem.Classes;
using ASC.ApiSystem.Models;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace ASC.ApiSystem.Controllers
{
    [ApiController]
    public class PortalController : CommonController
    {
        public PortalController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptionsMonitor<ILog> option, CoreSettings coreSettings, CommonLinkUtility commonLinkUtility, EmailValidationKeyProvider emailValidationKeyProvider, TimeZoneConverter timeZoneConverter, ApiSystemHelper apiSystemHelper, TenantManager tenantManager, UserFormatter userFormatter, TenantDomainValidator tenantDomainValidator, UserManagerWrapper userManagerWrapper, CommonConstants commonConstants, TimeZonesProvider timeZonesProvider, SettingsManager settingsManager, SecurityContext securityContext, IMemoryCache memoryCache)
            : base(httpContextAccessor, configuration, option, coreSettings, commonLinkUtility, emailValidationKeyProvider, timeZoneConverter, apiSystemHelper, tenantManager, userFormatter, tenantDomainValidator, userManagerWrapper, commonConstants, timeZonesProvider, settingsManager, securityContext, memoryCache)
        {
            
        }

        #region For TEST api

        [HttpGet]
        [ActionName("test")]
        public HttpResponseMessage Check()
        {
            var request = new HttpRequestMessage();

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                value = "Portal api works"
            });
        }

        #endregion

        #region API methods

        [HttpPost]
        [ActionName("register")]
        [AllowCrossSiteJson]
        [AuthSignature("auth.allowskip.registerportal")]
        public HttpResponseMessage Register(TenantModel model)
        {
            var request = new HttpRequestMessage();

            if (model == null)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
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

                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "params",
                    message
                });
            }

            var sw = Stopwatch.StartNew();

            if (!CheckPasswordPolicy(model.Password, out object error))
            {
                sw.Stop();

                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            model.FirstName = (model.FirstName ?? "").Trim();
            model.LastName = (model.LastName ?? "").Trim();

            if (!CheckValidName(model.FirstName + model.LastName, out error))
            {
                sw.Stop();

                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            model.PortalName = (model.PortalName ?? "").Trim();

            if (!CheckExistingNamePortal(model.PortalName, out error))
            {
                sw.Stop();

                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            Log.DebugFormat("PortalName = {0}; Elapsed ms. CheckExistingNamePortal: {1}", model.PortalName, sw.ElapsedMilliseconds);

            var clientIP = GetClientIp();

            if (CheckMuchRegistration(model, clientIP, sw))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "tooMuchAttempts",
                    message = "Too much attempts already"
                });
            }

            if (!CheckRecaptcha(model, clientIP, sw, out error))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            if (!CheckRegistrationPayment(out error))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, error);
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
                Name = Configuration["web.portal-name"] ?? "Cloud Office Applications",
                Address = model.PortalName,
                Culture = lang,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = string.IsNullOrEmpty(model.Password) ? null : model.Password,
                Email = (model.Email ?? "").Trim(),
                TimeZoneInfo = tz,
                MobilePhone = string.IsNullOrEmpty(model.Phone) ? null : model.Phone.Trim(),
                Industry = (TenantIndustry)model.Industry,
                Spam = model.Spam,
                Calls = model.Calls,
                Analytics = model.Analytics
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
                    ApiSystemHelper.AddTenantToCache(info.Address, SecurityContext.CurrentAccount.ID);

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

                return request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    error = "registerNewTenantError",
                    message = e.Message,
                    stacktrace = e.StackTrace
                });
            }

            var isFirst = true;
            string sendCongratulationsAddress = null;

            if (!string.IsNullOrEmpty(model.Password))
            {
                isFirst = !SendCongratulations(Request.Scheme, t, model.SkipWelcome, out sendCongratulationsAddress);
            }
            else if (Configuration["core.base-domain"] == "localhost")
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

            var reference = CreateReference(Request.Scheme, t.GetTenantDomain(CoreSettings), info.Email, isFirst);

            Log.DebugFormat("PortalName = {0}; Elapsed ms. CreateReferenceByCookie...: {1}", model.PortalName, sw.ElapsedMilliseconds);
            sw.Stop();

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                reference,
                tenant = ToTenantWrapper(t),
                referenceWelcome = sendCongratulationsAddress
            });
        }

        [HttpDelete]
        [ActionName("remove")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage Remove([FromQuery] TenantModel model)
        {
            var request = new HttpRequestMessage();

            if (!GetTenant(model, out Tenant tenant))
            {
                Log.Error("Model without tenant");

                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                });
            }

            if (tenant == null)
            {
                Log.Error("Tenant not found");

                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "portalNameNotFound",
                    message = "Portal not found"
                });
            }

            HostedSolution.RemoveTenant(tenant);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                tenant = ToTenantWrapper(tenant)
            });
        }

        [HttpPut]
        [ActionName("status")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage ChangeStatus(TenantModel model)
        {
            var request = new HttpRequestMessage();

            if (!GetTenant(model, out Tenant tenant))
            {
                Log.Error("Model without tenant");

                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                });
            }

            if (tenant == null)
            {
                Log.Error("Tenant not found");

                return request.CreateResponse(HttpStatusCode.BadRequest, new
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

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                tenant = ToTenantWrapper(tenant)
            });
        }

        [HttpPost]
        [ActionName("validateportalname")]
        [AllowCrossSiteJson]
        public HttpResponseMessage CheckExistingNamePortal(TenantModel model)
        {
            var request = new HttpRequestMessage();

            if (model == null)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                });
            }

            if (!CheckExistingNamePortal((model.PortalName ?? "").Trim(), out object error))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                message = "portalNameReadyToRegister"
            });
        }

        [HttpGet]
        [ActionName("get")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage GetPortals([FromQuery] TenantModel model)
        {
            var request = new HttpRequestMessage();

            try
            {
                var tenants = new List<Tenant>();
                var empty = true;

                if (!string.IsNullOrEmpty((model.Email ?? "").Trim()))
                {
                    empty = false;
                    tenants.AddRange(HostedSolution.FindTenants((model.Email ?? "").Trim()));
                }

                if (!string.IsNullOrEmpty((model.PortalName ?? "").Trim()))
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
                    .Select(ToTenantWrapper);

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    tenants = tenantsWrapper
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    error = "error",
                    message = ex.Message,
                    stacktrace = ex.StackTrace
                });
            }
        }

        #endregion

        #region Validate Method

        private void ValidateDomain(string domain)
        {
            // size
            TenantDomainValidator.ValidateDomainLength(domain);
            // characters
            TenantDomainValidator.ValidateDomainCharacters(domain);

            var sameAliasTenants = ApiSystemHelper.FindTenantsInCache(domain, SecurityContext.CurrentAccount.ID);

            if (sameAliasTenants != null)
            {
                throw new TenantAlreadyExistsException("Address busy.", sameAliasTenants);
            }
        }

        private bool CheckExistingNamePortal(string portalName, out object error)
        {
            error = null;
            if (string.IsNullOrEmpty(portalName))
            {
                error = new {error = "portalNameEmpty", message = "PortalName is required"};
                return false;
            }
            try
            {
                if (!string.IsNullOrEmpty(ApiSystemHelper.ApiCacheUrl))
                {
                    ValidateDomain(portalName.Trim());
                }
                else
                {
                    HostedSolution.CheckTenantAddress(portalName.Trim());
                }
            }
            catch (TenantAlreadyExistsException ex)
            {
                error = new {error = "portalNameExist", message = "Portal already exists", variants = ex.ExistsTenants.ToArray()};
                return false;
            }
            catch (TenantTooShortException)
            {
                error = new {error = "tooShortError", message = "Portal name is too short"};
                return false;

            }
            catch (TenantIncorrectCharsException)
            {
                error = new {error = "portalNameIncorrect", message = "Unallowable symbols in portalName"};
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                error = new {error = "error", message = ex.Message, stacktrace = ex.StackTrace};
                return false;
            }

            return true;
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

            if (!UserManagerWrapper.CheckPasswordRegex((PasswordSettings)new PasswordSettings().GetDefault(Configuration), pwd))
            {
                error = new {error = "passPolicyError", message = "Password is incorrect"};
                return false;
            }
            return true;
        }

        private bool CheckRegistrationPayment(out object error)
        {
            error = null;
            if (Configuration["core.base-domain"] == "localhost")
            {
                var tenants = HostedSolution.GetTenants(DateTime.MinValue);
                var firstTenant = tenants.FirstOrDefault();
                if (firstTenant != null)
                {
                    var activePortals = tenants.Count(r => r.Status != TenantStatus.Suspended && r.Status != TenantStatus.RemovePending);

                    var quota = HostedSolution.GetTenantQuota(firstTenant.TenantId);
                    if (quota.CountPortals > 0 && quota.CountPortals <= activePortals)
                    {
                        error = new {error = "portalsCountTooMuch", message = "Too much portals registered already",};
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
                && !IsTestEmail(model.Email))
            {
                if (!string.IsNullOrEmpty(model.AppKey) && CommonConstants.AppSecretKeys.Contains(model.AppKey))
                {
                    Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha via app key: {1}. {2}", model.PortalName, model.AppKey, sw.ElapsedMilliseconds);
                    return true;
                }

                var data = string.Format("{0} {1} {2} {3} {4}", model.PortalName, model.FirstName, model.LastName, model.Email, model.Phone);

                /*** validate recaptcha ***/
                if (!ValidateRecaptcha(model.RecaptchaResponse, clientIP))
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

    public static class PortalControllerExtention
    {
        public static IServiceCollection AddPortalController(this IServiceCollection services)
        {
            return services.AddCommonController();
        }
    }
}