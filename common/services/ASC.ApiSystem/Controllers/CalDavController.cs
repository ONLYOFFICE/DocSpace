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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

using ASC.ApiSystem.Classes;
using ASC.ApiSystem.Models;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.ApiSystem.Controllers
{
    [Scope]
    [ApiController]
    [Route("[controller]")]
    public class CalDavController : ControllerBase
    {
        private CommonMethods CommonMethods { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        private CoreSettings CoreSettings { get; }
        private CommonConstants CommonConstants { get; }
        public InstanceCrypto InstanceCrypto { get; }
        private ILog Log { get; }

        public CalDavController(
            CommonMethods commonMethods,
            EmailValidationKeyProvider emailValidationKeyProvider,
            CoreSettings coreSettings,
            CommonConstants commonConstants,
            InstanceCrypto instanceCrypto,
            IOptionsMonitor<ILog> option)
        {
            CommonMethods = commonMethods;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            CoreSettings = coreSettings;
            CommonConstants = commonConstants;
            InstanceCrypto = instanceCrypto;
            Log = option.Get("ASC.ApiSystem");
        }

        #region For TEST api

        [HttpGet("test")]
        public IActionResult Check()
        {
            return Ok(new
            {
                value = "CalDav api works"
            });
        }

        #endregion

        #region API methods

        [HttpGet("change_to_storage")]
        public IActionResult СhangeOfCalendarStorage(string change)
        {
            if (!GetTenant(change, out var tenant, out var error))
            {
                return BadRequest(error);
            }

            try
            {
                var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.TenantId, change + ConfirmType.Auth);

                SendToApi(Request.Scheme, tenant, "calendar/change_to_storage", new Dictionary<string, string> { { "change", change }, { "key", validationKey } });
            }
            catch (Exception ex)
            {
                Log.Error("Error change_to_storage", ex);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "apiError",
                    message = ex.Message
                });
            }

            return Ok();
        }

        [HttpGet("caldav_delete_event")]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult CaldavDeleteEvent(string eventInfo)
        {
            if (!GetTenant(eventInfo, out var tenant, out var error))
            {
                return BadRequest(error);
            }

            try
            {
                var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.TenantId, eventInfo + ConfirmType.Auth);

                SendToApi(Request.Scheme, tenant, "calendar/caldav_delete_event", new Dictionary<string, string> { { "eventInfo", eventInfo }, { "key", validationKey } });
            }
            catch (Exception ex)
            {
                Log.Error("Error caldav_delete_event", ex);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "apiError",
                    message = ex.Message
                });
            }

            return Ok();
        }

        [HttpPost("is_caldav_authenticated")]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult IsCaldavAuthenticated(UserPassword userPassword)
        {
            if (userPassword == null || string.IsNullOrEmpty(userPassword.User) || string.IsNullOrEmpty(userPassword.Password))
            {
                Log.Error("CalDav authenticated data is null");

                return BadRequest(new
                {
                    value = "false",
                    error = "portalNameEmpty",
                    message = "Argument is required"
                });
            }

            if (!GetUserData(userPassword.User, out var email, out var tenant, out var error))
            {
                return BadRequest(error);
            }

            try
            {
                Log.Info(string.Format("Caldav auth user: {0}, tenant: {1}", email, tenant.TenantId));

                if (InstanceCrypto.Encrypt(email) == userPassword.Password)
                {
                    return Ok(new
                    {
                        value = "true"
                    });
                }

                var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.TenantId, email + userPassword.Password + ConfirmType.Auth);

                var authData = string.Format("userName={0}&password={1}&key={2}",
                                             HttpUtility.UrlEncode(email),
                                             HttpUtility.UrlEncode(userPassword.Password),
                                             HttpUtility.UrlEncode(validationKey));

                SendToApi(Request.Scheme, tenant, "authentication/login", null, WebRequestMethods.Http.Post, authData);

                return Ok(new
                {
                    value = "true"
                });
            }
            catch (Exception ex)
            {
                Log.Error("Caldav authenticated", ex);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    value = "false",
                    message = ex.Message
                });
            }
        }

        #endregion

        #region private methods

        private bool GetTenant(string calendarParam, out Tenant tenant, out object error)
        {
            tenant = null;

            if (string.IsNullOrEmpty(calendarParam))
            {
                Log.Error("calendarParam is empty");

                error = new
                {
                    value = "false",
                    error = "portalNameEmpty",
                    message = "Argument is required"
                };

                return false;
            }

            Log.Info(string.Format("CalDav calendarParam: {0}", calendarParam));

            var userParam = calendarParam.Split('/')[0];
            return GetUserData(userParam, out _, out tenant, out error);
        }

        private bool GetUserData(string userParam, out string email, out Tenant tenant, out object error)
        {
            email = null;
            tenant = null;
            error = null;

            if (string.IsNullOrEmpty(userParam))
            {
                Log.Error("userParam is empty");

                error = new
                {
                    value = "false",
                    error = "portalNameEmpty",
                    message = "Argument is required"
                };

                return false;
            }

            var userData = userParam.Split('@');

            if (userData.Length < 3)
            {
                Log.Error(string.Format("Error Caldav username: {0}", userParam));

                error = new
                {
                    value = "false",
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                };

                return false;
            }

            email = string.Join("@", userData[0], userData[1]);

            var tenantName = userData[2];

            var baseUrl = CoreSettings.BaseDomain;

            if (!string.IsNullOrEmpty(baseUrl) && tenantName.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                tenantName = tenantName.Replace("." + baseUrl, "");
            }

            Log.Info(string.Format("CalDav: user:{0} tenantName:{1}", userParam, tenantName));

            var tenantModel = new TenantModel { PortalName = tenantName };

            if (!CommonMethods.GetTenant(tenantModel, out tenant))
            {
                Log.Error("Model without tenant");

                error = new
                {
                    value = "false",
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                };

                return false;
            }

            if (tenant == null)
            {
                Log.Error("Tenant not found " + tenantName);

                error = new
                {
                    value = "false",
                    error = "portalNameNotFound",
                    message = "Portal not found"
                };

                return false;
            }

            return true;
        }

        private void SendToApi(string requestUriScheme,
                                Tenant tenant,
                                string path,
                                IEnumerable<KeyValuePair<string, string>> args = null,
                                string httpMethod = WebRequestMethods.Http.Get,
                                string data = null)
        {
            var query = args == null
                            ? null
                            : string.Join("&", args.Select(arg => HttpUtility.UrlEncode(arg.Key) + "=" + HttpUtility.UrlEncode(arg.Value)).ToArray());

            var url = string.Format("{0}{1}{2}{3}{4}{5}",
                                    requestUriScheme,
                                    Uri.SchemeDelimiter,
                                    tenant.GetTenantDomain(CoreSettings),
                                    CommonConstants.WebApiBaseUrl,
                                    path,
                                    string.IsNullOrEmpty(query) ? "" : "?" + query);

            Log.Info(string.Format("CalDav: SendToApi: {0}", url));

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = new HttpMethod(httpMethod);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            using var httpClient = new HttpClient();

            if (data != null)
            {
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            httpClient.Send(request);
        }

        #endregion

        public class UserPassword
        {
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}