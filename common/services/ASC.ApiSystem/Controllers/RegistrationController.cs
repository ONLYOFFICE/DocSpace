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
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ASC.ApiSystem.Controllers
{
    [Obsolete("Registration is deprecated, please use PortalController or TariffController instead.")]
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        public CommonMethods CommonMethods { get; }

        public RegistrationController(CommonMethods commonMethods)
        {
            CommonMethods = commonMethods;
        }

        #region For TEST api

        [HttpGet("test")]
        public IActionResult Check()
        {
            return Ok(new
            {
                value = "Registration api works"
            });
        }

        #endregion

        #region API methods

        [HttpPost("registerportal")]
        [AllowCrossSiteJson]
        [AuthSignature("auth.allowskip.registerportal")]
        public IActionResult Register(TenantModel model)
        {
            if (model == null)
            {
                return BadRequest(new
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

                return Ok(new
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

                    return BadRequest(error);
                }
            }

            if (!CheckValidName(model.FirstName.Trim() + model.LastName.Trim(), out error))
            {
                sw.Stop();

                return BadRequest(error);
            }

            var checkTenantBusyPesp = CheckExistingNamePortal(model.PortalName.Trim());

            if (checkTenantBusyPesp != null)
            {
                sw.Stop();

                return checkTenantBusyPesp;
            }

            CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. CheckExistingNamePortal: {1}", model.PortalName, sw.ElapsedMilliseconds);

            var clientIP = CommonMethods.GetClientIp();

            CommonMethods.Log.DebugFormat("clientIP = {0}", clientIP);

            if (CommonMethods.CheckMuchRegistration(model, clientIP, sw))
            {
                return BadRequest(new
                {
                    errors = new[] { "tooMuchAttempts" }
                });
            }

            if (CommonMethods.CommonConstants.RecaptchaRequired && !CommonMethods.IsTestEmail(model.Email))
            {
                /*** validate recaptcha ***/
                if (!CommonMethods.ValidateRecaptcha(model.RecaptchaResponse, clientIP))
                {
                    CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha: {1}", model.PortalName, sw.ElapsedMilliseconds);

                    sw.Stop();

                    return BadRequest(new
                    {
                        errors = new[] { "recaptchaInvalid" },
                        message = "Recaptcha is invalid"
                    });

                }

                CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. ValidateRecaptcha: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }

            //check payment portal count
            if (CommonMethods.Configuration["core.base-domain"] == "localhost")
            {
                var tenants = CommonMethods.HostedSolution.GetTenants(DateTime.MinValue);
                var firstTenant = tenants.FirstOrDefault();

                if (firstTenant != null)
                {
                    var activePortals = tenants.Count(r => r.Status != TenantStatus.Suspended && r.Status != TenantStatus.RemovePending);

                    var quota = CommonMethods.HostedSolution.GetTenantQuota(firstTenant.TenantId);

                    if (quota.CountPortals > 0 && quota.CountPortals <= activePortals)
                    {
                        return BadRequest(new
                        {
                            errors = new[] { "portalsCountTooMuch" },
                            message = "Too much portals registered already",
                        });
                    }
                }
            }

            var language = model.Language ?? string.Empty;

            var tz = CommonMethods.TimeZonesProvider.GetCurrentTimeZoneInfo(language);

            CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. TimeZonesProvider.GetCurrentTimeZoneInfo: {1}", model.PortalName, sw.ElapsedMilliseconds);

            if (!string.IsNullOrEmpty(model.TimeZoneName))
            {
                tz = CommonMethods.TimeZoneConverter.GetTimeZone(model.TimeZoneName.Trim(), false) ?? tz;

                CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. TimeZonesProvider.OlsonTimeZoneToTimeZoneInfo: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }

            var lang = CommonMethods.TimeZonesProvider.GetCurrentCulture(language);

            CommonMethods.Log.DebugFormat("PortalName = {0}; model.Language = {1}, resultLang.DisplayName = {2}", model.PortalName, language, lang.DisplayName);

            var info = new TenantRegistrationInfo
            {
                Name = CommonMethods.Configuration["web.portal-name"] ?? "Cloud Office Applications",
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
                if (!string.IsNullOrEmpty(CommonMethods.ApiSystemHelper.ApiCacheUrl))
                {
                    CommonMethods.ApiSystemHelper.AddTenantToCache(info.Address, CommonMethods.SecurityContext.CurrentAccount.ID);

                    CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. CacheController.AddTenantToCache: {1}", model.PortalName, sw.ElapsedMilliseconds);
                }

                CommonMethods.HostedSolution.RegisterTenant(info, out t);

                /*********/

                CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. HostedSolution.RegisterTenant: {1}", model.PortalName, sw.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                sw.Stop();

                CommonMethods.Log.Error(e);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    errors = new[] { "registerNewTenantError" },
                    message = e.Message,
                    stacktrace = e.StackTrace
                });
            }

            var isFirst = true;

            string sendCongratulationsAddress = null;

            if (!string.IsNullOrEmpty(model.Password))
            {
                isFirst = !CommonMethods.SendCongratulations(Request.Scheme, t, model.SkipWelcome, out sendCongratulationsAddress);
            }
            else if (CommonMethods.Configuration["core.base-domain"] == "localhost")
            {
                try
                {
                    /* set wizard not completed*/
                    CommonMethods.TenantManager.SetCurrentTenant(t);

                    var settings = CommonMethods.SettingsManager.Load<WizardSettings>();

                    settings.Completed = false;

                    CommonMethods.SettingsManager.Save(settings);
                }
                catch (Exception e)
                {
                    CommonMethods.Log.Error(e);
                }
            }

            var reference = CommonMethods.CreateReference(Request.Scheme, t.GetTenantDomain(CommonMethods.CoreSettings), info.Email, isFirst, model.Module);

            CommonMethods.Log.DebugFormat("PortalName = {0}; Elapsed ms. CreateReferenceByCookie...: {1}", model.PortalName, sw.ElapsedMilliseconds);

            sw.Stop();

            return Ok(new
            {
                errors = "",
                reference,
                tenant = ToTenantWrapper(t),
                referenceWelcome = sendCongratulationsAddress,
            });
        }

        [HttpDelete("removeportal")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public IActionResult Remove(string portalName)
        {
            if (string.IsNullOrEmpty(portalName))
            {
                return BadRequest(new
                {
                    errors = "portalName is required."
                });
            }

            var tenant = CommonMethods.HostedSolution.GetTenant(portalName.Trim());

            if (tenant == null)
            {
                return BadRequest(new
                {
                    errors = "Tenant not found."
                });
            }

            CommonMethods.HostedSolution.RemoveTenant(tenant);

            return Ok(new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant)
            });
        }

        [HttpPut("tariff")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public IActionResult SetTariff(TariffModel model)
        {
            if (model == null)
            {
                return BadRequest(new
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

                return BadRequest(new
                {
                    errors
                });
            }

            var tenant = CommonMethods.HostedSolution.GetTenant((model.PortalName ?? "").Trim());

            if (tenant == null)
            {
                return BadRequest(new
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

            CommonMethods.HostedSolution.SaveTenantQuota(quota);

            var tariff = new Tariff
            {
                QuotaId = quota.Id,
                DueDate = model.DueDate != default ? model.DueDate : DateTime.MaxValue,
            };

            CommonMethods.HostedSolution.SetTariff(tenant.TenantId, tariff);

            tariff = CommonMethods.HostedSolution.GetTariff(tenant.TenantId, false);

            quota = CommonMethods.HostedSolution.GetTenantQuota(tenant.TenantId);

            return Ok(new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant),
                tariff = ToTariffWrapper(tariff, quota)
            });
        }

        [HttpGet("tariff")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public IActionResult GetTariff(string portalName)
        {
            if (string.IsNullOrEmpty(portalName))
            {
                return BadRequest(new
                {
                    errors = "portalName is required."
                });
            }

            var tenant = CommonMethods.HostedSolution.GetTenant(portalName.Trim());

            if (tenant == null)
            {
                return BadRequest(new
                {
                    errors = "Tenant not found."
                });
            }

            var tariff = CommonMethods.HostedSolution.GetTariff(tenant.TenantId, false);

            var quota = CommonMethods.HostedSolution.GetTenantQuota(tenant.TenantId);

            return Ok(new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant),
                tariff = ToTariffWrapper(tariff, quota)
            });
        }

        [HttpPut("statusportal")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public IActionResult ChangeStatus(TenantModel model, bool active)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    errors = "PortalName is required."
                });
            }

            var tenant = CommonMethods.HostedSolution.GetTenant((model.PortalName ?? "").Trim());

            if (tenant == null)
            {
                return BadRequest(new
                {
                    errors = "Tenant not found."
                });
            }

            tenant.SetStatus(active ? TenantStatus.Active : TenantStatus.Suspended);

            CommonMethods.HostedSolution.SaveTenant(tenant);

            return Ok(new
            {
                errors = "",
                tenant = ToTenantWrapper(tenant)
            });
        }


        [HttpPost("validateportalname")]
        [AllowCrossSiteJson]
        public IActionResult CheckExistingNamePortal(TenantModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    errors = "PortalName is required."
                });
            }

            var response = CheckExistingNamePortal((model.PortalName ?? "").Trim());

            return response ?? Ok(new
            {
                errors = "",
                message = "portalNameReadyToRegister"
            });
        }


        [HttpGet("getportals")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public IActionResult GetPortalsByEmail(string email)
        {
            try
            {
                var tenants = string.IsNullOrEmpty(email)
                                  ? CommonMethods.HostedSolution.GetTenants(DateTime.MinValue).OrderBy(t => t.TenantId).ToList()
                                  : CommonMethods.HostedSolution.FindTenants(email.Trim()).OrderBy(t => t.TenantId).ToList();


                var refers = tenants.Where(t => t.Status == TenantStatus.Active).ToList()
                                    .ConvertAll(t => string.Format("{0}{1}{2}",
                                                                   Request.Scheme,
                                                                   Uri.SchemeDelimiter,
                                                                   t.GetTenantDomain(CommonMethods.CoreSettings)));

                return Ok(new
                {
                    errors = "",
                    message = "",
                    portals = refers
                });
            }
            catch (Exception ex)
            {
                CommonMethods.Log.Error(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    errors = new[] { "error" },
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
                tenantDomain = t.GetTenantDomain(CommonMethods.CoreSettings),
                hostedRegion = t.HostedRegion,
                name = t.Name,
                ownerId = t.OwnerId,
                status = t.Status,
                partnerId = t.PartnerId,
                paymentId = t.PaymentId,
                language = t.Language,
                industry = t.Industry,
                timeZoneName = CommonMethods.TimeZoneConverter.GetTimeZone(t.TimeZone).DisplayName,
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

        private IActionResult CheckExistingNamePortal(string portalName)
        {
            if (string.IsNullOrEmpty(portalName))
            {
                return BadRequest(new
                {
                    error = "portalNameEmpty"
                });
            }

            try
            {
                if (!string.IsNullOrEmpty(CommonMethods.ApiSystemHelper.ApiCacheUrl))
                {
                    ValidateDomain(portalName.Trim());
                }
                else
                {
                    CommonMethods.HostedSolution.CheckTenantAddress(portalName.Trim());
                }
            }
            catch (TenantAlreadyExistsException ex)
            {
                return BadRequest(new
                {
                    errors = new[] { "portalNameExist" },
                    variants = ex.ExistsTenants.ToArray()
                });
            }
            catch (TenantTooShortException)
            {
                return BadRequest(new
                {
                    errors = new[] { "tooShortError" }
                });

            }
            catch (TenantIncorrectCharsException)
            {
                return BadRequest(new
                {
                    errors = new[] { "portalNameIncorrect" }
                });
            }
            catch (Exception ex)
            {
                CommonMethods.Log.Error(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, new
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
            CommonMethods.TenantDomainValidator.ValidateDomainLength(domain);
            // characters
            TenantDomainValidator.ValidateDomainCharacters(domain);

            var sameAliasTenants = CommonMethods.ApiSystemHelper.FindTenantsInCache(domain, CommonMethods.SecurityContext.CurrentAccount.ID);

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

            if (!CommonMethods.UserFormatter.IsValidUserName(name, string.Empty))
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

            var passwordSettings = (PasswordSettings)new PasswordSettings().GetDefault(CommonMethods.Configuration);

            if (!CommonMethods.UserManagerWrapper.CheckPasswordRegex(passwordSettings, pwd))
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
}