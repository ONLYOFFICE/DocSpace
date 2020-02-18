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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Common.Logging;
using ASC.Web.Core.Users;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
    public class SettingsController : CommonController
    {
        public SettingsController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptionsMonitor<ILog> option, CoreSettings coreSettings, CommonLinkUtility commonLinkUtility, EmailValidationKeyProvider emailValidationKeyProvider, TimeZoneConverter timeZoneConverter, ApiSystemHelper apiSystemHelper, TenantManager tenantManager, UserFormatter userFormatter, TenantDomainValidator tenantDomainValidator, UserManagerWrapper userManagerWrapper, CommonConstants commonConstants, TimeZonesProvider timeZonesProvider, SettingsManager settingsManager, SecurityContext securityContext, IMemoryCache memoryCache)
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
                value = "Settings api works"
            });
        }

        #endregion

        #region API methods

        [HttpGet]
        [ActionName("get")]
        [AuthSignature]
        public HttpResponseMessage GetSettings([FromQuery] SettingsModel model)
        {
            var request = new HttpRequestMessage();

            if (!GetTenant(model, out int tenantId, out object error))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrEmpty(model.Key))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "params",
                    message = "Key is required"
                });
            }

            var settings = CoreSettings.GetSetting(model.Key, tenantId);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                settings
            });
        }

        [HttpPost]
        [ActionName("save")]
        [AuthSignature]
        public HttpResponseMessage SaveSettings([FromBody] SettingsModel model)
        {
            var request = new HttpRequestMessage();

            if (!GetTenant(model, out int tenantId, out object error))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrEmpty(model.Key))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "params",
                    message = "Key is required"
                });
            }

            if (string.IsNullOrEmpty(model.Value))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "params",
                    message = "Value is empty"
                });
            }

            Log.DebugFormat("Set {0} value {1} for {2}", model.Key, model.Value, tenantId);

            CoreSettings.SaveSetting(model.Key, model.Value, tenantId);

            var settings = CoreSettings.GetSetting(model.Key, tenantId);

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                settings
            });
        }

        #endregion

        #region private methods

        private bool GetTenant(SettingsModel model, out int tenantId, out object error)
        {
            tenantId = -1;
            error = null;

            if (model == null)
            {
                error = new {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                };

                Log.Error("Model is null");

                return false;
            }

            if (model.TenantId.HasValue && model.TenantId.Value == -1)
            {
                tenantId = model.TenantId.Value;
                return true;
            }

            if (!GetTenant(model, out Tenant tenant))
            {
                error = new {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                };

                Log.Error("Model without tenant");

                return false;
            }

            if (tenant == null)
            {
                error = new {
                    error = "portalNameNotFound",
                    message = "Portal not found"
                };

                Log.Error("Tenant not found");

                return false;
            }

            tenantId = tenant.TenantId;
            return true;
        }

        #endregion
    }

    public static class SettingsControllerExtention
    {
        public static IServiceCollection AddSettingsController(this IServiceCollection services)
        {
            return services.AddCommonController();
        }
    }
}