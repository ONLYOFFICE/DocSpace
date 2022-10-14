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
public class CalDavController : ControllerBase
{
    private CommonMethods CommonMethods { get; }
    private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
    private CoreSettings CoreSettings { get; }
    private CommonConstants CommonConstants { get; }
    public InstanceCrypto InstanceCrypto { get; }
    private ILogger<CalDavController> Log { get; }
    private IHttpClientFactory ClientFactory { get; }

    public CalDavController(
        CommonMethods commonMethods,
        EmailValidationKeyProvider emailValidationKeyProvider,
        CoreSettings coreSettings,
        CommonConstants commonConstants,
        InstanceCrypto instanceCrypto,
        ILogger<CalDavController> logger)
    {
        CommonMethods = commonMethods;
        EmailValidationKeyProvider = emailValidationKeyProvider;
        CoreSettings = coreSettings;
        CommonConstants = commonConstants;
        InstanceCrypto = instanceCrypto;
        Log = logger;
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
            var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.Id, change + ConfirmType.Auth);

            SendToApi(Request.Scheme, tenant, "calendar/change_to_storage", new Dictionary<string, string> { { "change", change }, { "key", validationKey } });
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "Error change_to_storage");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "apiError",
                message = ex.Message
            });
        }

        return Ok();
    }

    [HttpGet("caldav_delete_event")]
    [Authorize(AuthenticationSchemes = "auth:allowskip")]
    public IActionResult CaldavDeleteEvent(string eventInfo)
    {
        if (!GetTenant(eventInfo, out var tenant, out var error))
        {
            return BadRequest(error);
        }

        try
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.Id, eventInfo + ConfirmType.Auth);

            SendToApi(Request.Scheme, tenant, "calendar/caldav_delete_event", new Dictionary<string, string> { { "eventInfo", eventInfo }, { "key", validationKey } });
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "Error caldav_delete_event");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "apiError",
                message = ex.Message
            });
        }

        return Ok();
    }

    [HttpPost("is_caldav_authenticated")]
    [Authorize(AuthenticationSchemes = "auth:allowskip")]
    public IActionResult IsCaldavAuthenticated(UserPassword userPassword)
    {
        if (userPassword == null || string.IsNullOrEmpty(userPassword.User) || string.IsNullOrEmpty(userPassword.Password))
        {
            Log.LogError("CalDav authenticated data is null");

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
            Log.LogInformation(string.Format("Caldav auth user: {0}, tenant: {1}", email, tenant.Id));

            if (InstanceCrypto.Encrypt(email) == userPassword.Password)
            {
                return Ok(new
                {
                    value = "true"
                });
            }

            var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.Id, email + userPassword.Password + ConfirmType.Auth);

            var authData = $"userName={HttpUtility.UrlEncode(email)}&password={HttpUtility.UrlEncode(userPassword.Password)}&key={HttpUtility.UrlEncode(validationKey)}";

            SendToApi(Request.Scheme, tenant, "authentication/login", null, WebRequestMethods.Http.Post, authData);

            return Ok(new
            {
                value = "true"
            });
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "Caldav authenticated");

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
            Log.LogError("calendarParam is empty");

            error = new
            {
                value = "false",
                error = "portalNameEmpty",
                message = "Argument is required"
            };

            return false;
        }

        Log.LogInformation($"CalDav calendarParam: {calendarParam}");

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
            Log.LogError("userParam is empty");

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
            Log.LogError($"Error Caldav username: {userParam}");

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

        Log.LogInformation($"CalDav: user:{userParam} tenantName:{tenantName}");

        var tenantModel = new TenantModel { PortalName = tenantName };

        if (!CommonMethods.GetTenant(tenantModel, out tenant))
        {
            Log.LogError("Model without tenant");

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
            Log.LogError("Tenant not found " + tenantName);

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

        var url = $"{requestUriScheme}{Uri.SchemeDelimiter}{tenant.GetTenantDomain(CoreSettings)}{CommonConstants.WebApiBaseUrl}{path}{(string.IsNullOrEmpty(query) ? "" : "?" + query)}";

        Log.LogInformation($"CalDav: SendToApi: {url}");

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);
        request.Method = new HttpMethod(httpMethod);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        var httpClient = ClientFactory.CreateClient();

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
