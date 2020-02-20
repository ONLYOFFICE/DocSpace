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
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using ASC.Web.Studio.Utility;
using ASC.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using ASC.Core.Common.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace ASC.ApiSystem.Controllers
{
    [Obsolete("Registration is deprecated, please use PortalController or TariffController instead.")]
    [ApiController]
    public class RegistrationController : CommonController
    {
        public RegistrationController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptionsMonitor<ILog> option, CoreSettings coreSettings, CommonLinkUtility commonLinkUtility, EmailValidationKeyProvider emailValidationKeyProvider, TimeZoneConverter timeZoneConverter, ApiSystemHelper apiSystemHelper, TenantManager tenantManager, UserFormatter userFormatter, TenantDomainValidator tenantDomainValidator, UserManagerWrapper userManagerWrapper, CommonConstants commonConstants, TimeZonesProvider timeZonesProvider, SettingsManager settingsManager, SecurityContext securityContext, IMemoryCache memoryCache, IOptionsSnapshot<HostedSolution> hostedSolutionOptions)
            : base(httpContextAccessor, configuration, option, coreSettings, commonLinkUtility, emailValidationKeyProvider, timeZoneConverter, apiSystemHelper, tenantManager, userFormatter, tenantDomainValidator, userManagerWrapper, commonConstants, timeZonesProvider, settingsManager, securityContext, memoryCache, hostedSolutionOptions)
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
                value = "Registration"
            });
        }

        #endregion

        #region API methods

        [HttpPost]
        [ActionName("registerportal")]
        [AllowCrossSiteJson]
        [AuthSignature("auth.allowskip.registerportal")]
        public HttpResponseMessage Register(TenantModel model)
        {
            var request = new HttpRequestMessage();

            if (model == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "Tenant data is required."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = new JArray();

                foreach (var k in ModelState.Keys)
                {
                    errors.Add(ModelState[k].Errors.FirstOrDefault().ErrorMessage);
                }

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors
                });
            }

            var sw = Stopwatch.StartNew();

            object error;

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (!CheckPasswordPolicy(model.Password, out error))
                {
                    sw.Stop();

                    return request.CreateResponse(HttpStatusCode.OK, error);
                }
            }

            if (!CheckValidName(model.FirstName.Trim() + model.LastName.Trim(), out error))
            {
                sw.Stop();

                return request.CreateResponse(HttpStatusCode.OK, error);
            }

            var checkTenantBusyPesp = CheckExistingNamePortal(model.PortalName.Trim());

            if (checkTenantBusyPesp != null)
            {
                sw.Stop();

                return checkTenantBusyPesp;
            }

            Log.DebugFormat("PortalName = {0}; Elapsed ms. CheckExistingNamePortal: {1}", model.PortalName, sw.ElapsedMilliseconds);

            var clientIP = GetClientIp();

            Log.DebugFormat("clientIP = {0}", clientIP);

            if (CheckMuchRegistration(model, clientIP, sw))
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = new[] { "tooMuchAttempts" }
                });
            }

            if (CommonConstants.RecaptchaRequired && !IsTestEmail(model.Email))
            {
                /*** validate recaptcha ***/
                if (!ValidateRecaptcha(model.RecaptchaResponse, clientIP))
                {
                    Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha: {1}", model.PortalName, sw.ElapsedMilliseconds);

                    sw.Stop();

                    return request.CreateResponse(HttpStatusCode.OK, new
                    {
                        errors = new[] { "recaptchaInvalid" },
                        message = "Recaptcha is invalid"
                    });

                }

                Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }

            //check payment portal count
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
                        return request.CreateResponse(HttpStatusCode.OK, new
                        {
                            errors = new[] { "portalsCountTooMuch" },
                            message = "Too much portals registered already",
                        });
                    }
                }
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
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Password = string.IsNullOrEmpty(model.Password) ? null : model.Password,
                Email = model.Email.Trim(),
                TimeZoneInfo = tz,
                MobilePhone = string.IsNullOrEmpty(model.Phone) ? null : model.Phone.Trim(),
                Industry = (TenantIndustry)model.Industry,
                Spam = model.Spam,
                Calls = model.Calls,
                Analytics = model.Analytics
            };

            if (!string.IsNullOrEmpty(model.PartnerId))
            {
                if (Guid.TryParse(model.PartnerId, out Guid guid))
                {
                    // valid guid
                    info.PartnerId = model.PartnerId;
                }
            }

            if (!string.IsNullOrEmpty(model.AffiliateId))
            {
                info.AffiliateId = model.AffiliateId;
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

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = new[] {"registerNewTenantError"},
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

            var reference = CreateReference(Request.Scheme, t.GetTenantDomain(CoreSettings), info.Email, isFirst, model.Module);

            Log.DebugFormat("PortalName = {0}; Elapsed ms. CreateReferenceByCookie...: {1}", model.PortalName, sw.ElapsedMilliseconds);

            sw.Stop();

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                errors = "",
                reference,
                tenant = ToTenantWrapper(t),
                referenceWelcome = sendCongratulationsAddress,
            });
        }

        [HttpDelete]
        [ActionName("removeportal")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage Remove(string portalName)
        {
            var request = new HttpRequestMessage();

            if (string.IsNullOrEmpty(portalName))
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "portalName is required."
                });
            }

            var tenant = HostedSolution.GetTenant(portalName.Trim());

            if (tenant == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "Tenant not found."
                });
            }

            HostedSolution.RemoveTenant(tenant);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant)
            });
        }

        [HttpPut]
        [ActionName("tariff")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage SetTariff(TariffModel model)
        {
            var request = new HttpRequestMessage();

            if (model == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "PortalName is required."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = new JArray();

                foreach (var k in ModelState.Keys)
                {
                    errors.Add(ModelState[k].Errors.FirstOrDefault().ErrorMessage);
                }

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors
                });
            }

            var tenant = HostedSolution.GetTenant((model.PortalName ?? "").Trim());

            if (tenant == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "Tenant not found."
                });
            }

            var quota = new TenantQuota(tenant.TenantId)
            {
                ActiveUsers = 10000,
                Features = model.Features ?? "",
                MaxFileSize = 1024 * 1024 * 1024,
                MaxTotalSize = 1024L * 1024 * 1024 * 1024 - 1,
                Name = "api",
            };

            if (model.ActiveUsers != default)
            {
                quota.ActiveUsers = model.ActiveUsers;
            }

            if (model.MaxTotalSize != default)
            {
                quota.MaxTotalSize = model.MaxTotalSize;
            }

            if (model.MaxFileSize != default)
            {
                quota.MaxFileSize = model.MaxFileSize;
            }

            HostedSolution.SaveTenantQuota(quota);

            var tariff = new Tariff
            {
                QuotaId = quota.Id,
                DueDate = model.DueDate != default ? model.DueDate : DateTime.MaxValue,
            };

            HostedSolution.SetTariff(tenant.TenantId, tariff);

            tariff = HostedSolution.GetTariff(tenant.TenantId, false);

            quota = HostedSolution.GetTenantQuota(tenant.TenantId);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant),
                tariff = ToTariffWrapper(tariff, quota)
            });
        }

        [HttpGet]
        [ActionName("tariff")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage GetTariff(string portalName)
        {
            var request = new HttpRequestMessage();

            if (string.IsNullOrEmpty(portalName))
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "portalName is required."
                });
            }

            var tenant = HostedSolution.GetTenant(portalName.Trim());

            if (tenant == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "Tenant not found."
                });
            }

            var tariff = HostedSolution.GetTariff(tenant.TenantId, false);

            var quota = HostedSolution.GetTenantQuota(tenant.TenantId);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant),
                tariff = ToTariffWrapper(tariff, quota)
            });
        }

        [HttpPut]
        [ActionName("statusportal")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage ChangeStatus(TenantModel model, bool active)
        {
            var request = new HttpRequestMessage();

            if (model == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "PortalName is required."
                });
            }

            var tenant = HostedSolution.GetTenant((model.PortalName ?? "").Trim());

            if (tenant == null)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "Tenant not found."
                });
            }

            tenant.SetStatus(active ? TenantStatus.Active : TenantStatus.Suspended);

            HostedSolution.SaveTenant(tenant);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                errors = "",
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
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "PortalName is required."
                });
            }

            var response = CheckExistingNamePortal((model.PortalName ?? "").Trim());

            return response ?? request.CreateResponse(HttpStatusCode.OK, new
            {
                errors = "",
                message = "portalNameReadyToRegister"
            });
        }


        [HttpGet]
        [ActionName("getportals")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage GetPortalsByEmail(string email)
        {
            var request = new HttpRequestMessage();

            try
            {
                var tenants = string.IsNullOrEmpty(email)
                                  ? HostedSolution.GetTenants(DateTime.MinValue).OrderBy(t => t.TenantId).ToList()
                                  : HostedSolution.FindTenants(email.Trim()).OrderBy(t => t.TenantId).ToList();


                var refers = tenants.Where(t => t.Status == TenantStatus.Active).ToList()
                                    .ConvertAll(t => string.Format("{0}{1}{2}",
                                                                   Request.Scheme,
                                                                   Uri.SchemeDelimiter,
                                                                   t.GetTenantDomain(CoreSettings)));

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = "",
                    message = "",
                    portals = refers
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    errors = new[] {"error"},
                    message = ex.Message,
                    stacktrace = ex.StackTrace
                });
            }
        }

        #endregion

        #region private methods

        private object ToTenantWrapper(Tenant t)
        {
            return new
            {
                tenantId = t.TenantId,
                tenantAlias = t.TenantAlias,
                tenantDomain = t.GetTenantDomain(CoreSettings),
                hostedRegion = t.HostedRegion,
                name = t.Name,
                ownerId = t.OwnerId,
                status = t.Status,
                partnerId = t.PartnerId,
                paymentId = t.PaymentId,
                language = t.Language,
                industry = t.Industry,
                timeZoneName = TimeZoneConverter.GetTimeZone(t.TimeZone).DisplayName,
                createdDateTime = t.CreatedDateTime
            };
        }

        private object ToTariffWrapper(Tariff t, TenantQuota q)
        {
            return new
            {
                t.DueDate,
                t.State,
                q.MaxTotalSize,
                q.MaxFileSize,
                q.ActiveUsers,
                q.Features
            };
        }

        private HttpResponseMessage CheckExistingNamePortal(string portalName)
        {
            var request = new HttpRequestMessage();

            if (string.IsNullOrEmpty(portalName))
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    error = "portalNameEmpty"
                });
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
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = new[] { "portalNameExist" },
                    variants = ex.ExistsTenants.ToArray()
                });
            }
            catch (TenantTooShortException)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = new[] { "tooShortError" }
                });

            }
            catch (TenantIncorrectCharsException)
            {
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    errors = new[] { "portalNameIncorrect" }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    errors = new[] { "error" },
                    message = ex.Message,
                    stacktrace = ex.StackTrace
                });
            }

            return null;
        }

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

        private bool CheckValidName(string name, out object error)
        {
            error = null;

            if (string.IsNullOrEmpty(name = (name ?? "").Trim()))
            {
                error = new
                {
                    error = new[] { "error" },
                    message = "name is required"
                };

                return false;
            }

            if (!UserFormatter.IsValidUserName(name, string.Empty))
            {
                error = new
                {
                    error = new[] { "error" },
                    message = "name is incorrect"
                };

                return false;
            }

            return true;
        }

        public bool CheckPasswordPolicy(string pwd, out object error)
        {
            error = null;
            //Validate Password match
            if (string.IsNullOrEmpty(pwd))
            {
                error = new
                {
                    errors = new[] { "passEmpty" },
                    message = "password is empty"
                };

                return false;
            }

            if (!UserManagerWrapper.CheckPasswordRegex((PasswordSettings)new PasswordSettings().GetDefault(Configuration), pwd))
            {
                error = new
                {
                    errors = new[] { "passPolicyError" },
                    message = "password is incorrect"
                };

                return false;
            }
            return true;
        }

        #endregion
    }

    public static class RegistrationControllerExtention
    {
        public static IServiceCollection AddRegistrationController(this IServiceCollection services)
        {
            return services.AddCommonController();
        }
    }
}