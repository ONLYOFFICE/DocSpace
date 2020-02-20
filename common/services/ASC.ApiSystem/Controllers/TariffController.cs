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
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Web.Core.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using ASC.Core;
using Microsoft.Extensions.Configuration;
using ASC.Web.Studio.Utility;
using ASC.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using ASC.Common.Utils;
using ASC.Web.Core.Helpers;
using ASC.Core.Users;
using ASC.Core.Common.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace ASC.ApiSystem.Controllers
{
    [ApiController]
    public class TariffController : CommonController
    {
        public TariffController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptionsMonitor<ILog> option, CoreSettings coreSettings, CommonLinkUtility commonLinkUtility, EmailValidationKeyProvider emailValidationKeyProvider, TimeZoneConverter timeZoneConverter, ApiSystemHelper apiSystemHelper, TenantManager tenantManager, UserFormatter userFormatter, TenantDomainValidator tenantDomainValidator, UserManagerWrapper userManagerWrapper, CommonConstants commonConstants, TimeZonesProvider timeZonesProvider, SettingsManager settingsManager, SecurityContext securityContext, IMemoryCache memoryCache, IOptionsSnapshot<HostedSolution> hostedSolutionOptions)
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
                value = "Tariff api works"
            });
        }

        #endregion

        #region API methods

        [HttpPut]
        [ActionName("set")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage SetTariff(TariffModel model)
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

            return GetTariff(tenant);
        }

        [HttpGet]
        [ActionName("get")]
        [AllowCrossSiteJson]
        [AuthSignature]
        public HttpResponseMessage GetTariff([FromQuery] TariffModel model)
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

            return GetTariff(tenant);
        }

        [HttpGet]
        [ActionName("all")]
        [AllowCrossSiteJson]
        public HttpResponseMessage GetTariffs()
        {
            var request = new HttpRequestMessage();

            var tariffs = HostedSolution.GetTenantQuotas()
                .Where(q => !q.Trial && !q.Free && !q.Open)
                .OrderBy(q => q.ActiveUsers)
                .ThenByDescending(q => q.Id)
                .Select(q => ToTariffWrapper(null, q));

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                tariffs
            });
        }

        #endregion

        #region private methods

        private HttpResponseMessage GetTariff(Tenant tenant)
        {
            var request = new HttpRequestMessage();

            var tariff = HostedSolution.GetTariff(tenant.TenantId, false);

            var quota = HostedSolution.GetTenantQuota(tenant.TenantId);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                tenant = ToTenantWrapper(tenant),
                tariff = ToTariffWrapper(tariff, quota),
            });
        }

        private static object ToTariffWrapper(Tariff t, TenantQuota q)
        {
            return new
            {
                activeUsers = q.ActiveUsers,
                dueDate = t == null ? DateTime.MaxValue : t.DueDate,
                features = q.Features,
                maxFileSize = q.MaxFileSize,
                maxTotalSize = q.MaxTotalSize,
                state = t == null ? TariffState.Paid : t.State,
            };
        }

        #endregion
    }

    public static class TariffControllerExtention
    {
        public static IServiceCollection AddTariffController(this IServiceCollection services)
        {
            return services.AddCommonController();
        }
    }
}