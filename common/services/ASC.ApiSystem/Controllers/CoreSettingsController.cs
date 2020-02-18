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
using ASC.Common.Logging;
using ASC.Web.Core.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using ASC.Web.Studio.Utility;
using ASC.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using ASC.Common.Utils;
using ASC.Web.Core.Helpers;
using ASC.Core.Users;
using ASC.Core.Tenants;
using ASC.Core.Common.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace ASC.ApiSystem.Controllers
{
    [Obsolete("CoreSettingsController is deprecated, please use SettingsController instead.")]
    [ApiController]
    public class CoreSettingsController : CommonController
    {
        public CoreSettingsController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptionsMonitor<ILog> option, CoreSettings coreSettings, CommonLinkUtility commonLinkUtility, EmailValidationKeyProvider emailValidationKeyProvider, TimeZoneConverter timeZoneConverter, ApiSystemHelper apiSystemHelper, TenantManager tenantManager, UserFormatter userFormatter, TenantDomainValidator tenantDomainValidator, UserManagerWrapper userManagerWrapper, CommonConstants commonConstants, TimeZonesProvider timeZonesProvider, SettingsManager settingsManager, SecurityContext securityContext, IMemoryCache memoryCache)
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
                value = "CoreSettings api works"
            });
        }

        #endregion

        #region API methods

        [HttpGet]
        [ActionName("get")]
        [AuthSignature]
        public HttpResponseMessage GetSettings(int tenant, string key)
        {
            var request = new HttpRequestMessage();

            try
            {
                if (tenant < -1)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = "portalNameIncorrect",
                        message = "Tenant is incorrect"
                    });
                }

                if (string.IsNullOrEmpty(key))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = "params",
                        message = "Key is empty"
                    });
                }

                var settings = CoreSettings.GetSetting(key, tenant);

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    settings
                });
            }
            catch (ArgumentException ae)
            {
                Log.Error(ae);

                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "error",
                    message = ae.Message
                });
            }
            catch (Exception e)
            {
                Log.Error(e);

                return request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    error = "error",
                    message = e.Message
                });
            }
        }

        [HttpPost]
        [ActionName("save")]
        [AuthSignature]
        public HttpResponseMessage SaveSettings([FromBody] CoreSettingsModel model)
        {
            var request = new HttpRequestMessage();

            try
            {
                if (model == null || string.IsNullOrEmpty(model.Key))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = "params",
                        message = "Key is empty"
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

                var tenant = model.Tenant;

                if (tenant < -1)
                {
                    tenant = -1;
                }

                CoreSettings.SaveSetting(model.Key, model.Value, tenant);

                var settings = CoreSettings.GetSetting(model.Key, tenant);

                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    settings
                });
            }
            catch (ArgumentException ae)
            {
                Log.Error(ae);

                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "params",
                    message = ae.Message
                });
            }
            catch (Exception e)
            {
                Log.Error(e);

                return request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    error = "error",
                    message = e.Message
                });
            }
        }

        #endregion
    }

    public static class CoreSettingsControllerExtention
    {
        public static IServiceCollection AddCoreSettingsController(this IServiceCollection services)
        {
            return services.AddCommonController();
        }
    }
}