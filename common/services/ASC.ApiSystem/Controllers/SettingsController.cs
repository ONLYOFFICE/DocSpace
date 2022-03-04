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

namespace ASC.ApiSystem.Controllers
{
    [Scope]
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private CommonMethods CommonMethods { get; }
        private CoreSettings CoreSettings { get; }
        private ILog Log { get; }

        public SettingsController(
            CommonMethods commonMethods,
            CoreSettings coreSettings,
            IOptionsMonitor<ILog> option)
        {
            CommonMethods = commonMethods;
            CoreSettings = coreSettings;
            Log = option.Get("ASC.ApiSystem");
        }

        #region For TEST api

        [HttpGet("test")]
        public IActionResult Check()
        {
            return Ok(new
            {
                value = "Settings api works"
            });
        }

        #endregion

        #region API methods

        [HttpGet("get")]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult GetSettings([FromQuery] SettingsModel model)
        {
            if (!GetTenant(model, out var tenantId, out var error))
            {
                return BadRequest(error);
            }

            if (string.IsNullOrEmpty(model.Key))
            {
                return BadRequest(new
                {
                    error = "params",
                    message = "Key is required"
                });
            }

            var settings = CoreSettings.GetSetting(model.Key, tenantId);

            return Ok(new
            {
                settings
            });
        }

        [HttpPost("save")]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult SaveSettings([FromBody] SettingsModel model)
        {
            if (!GetTenant(model, out var tenantId, out var error))
            {
                return BadRequest(error);
            }

            if (string.IsNullOrEmpty(model.Key))
            {
                return BadRequest(new
                {
                    error = "params",
                    message = "Key is required"
                });
            }

            if (string.IsNullOrEmpty(model.Value))
            {
                return BadRequest(new
                {
                    error = "params",
                    message = "Value is empty"
                });
            }

            Log.DebugFormat("Set {0} value {1} for {2}", model.Key, model.Value, tenantId);

            CoreSettings.SaveSetting(model.Key, model.Value, tenantId);

            var settings = CoreSettings.GetSetting(model.Key, tenantId);

            return Ok(new
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
                error = new
                {
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

            if (!CommonMethods.GetTenant(model, out var tenant))
            {
                error = new
                {
                    error = "portalNameEmpty",
                    message = "PortalName is required"
                };

                Log.Error("Model without tenant");

                return false;
            }

            if (tenant == null)
            {
                error = new
                {
                    error = "portalNameNotFound",
                    message = "Portal not found"
                };

                Log.Error("Tenant not found");

                return false;
            }

            tenantId = tenant.Id;
            return true;
        }

        #endregion
    }
}