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

using ASC.ApiSystem.Models;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.ApiSystem.Controllers
{
    [Scope]
    [Obsolete("CoreSettingsController is deprecated, please use SettingsController instead.")]
    [ApiController]
    [Route("[controller]")]
    public class CoreSettingsController : ControllerBase
    {
        private CoreSettings CoreSettings { get; }
        private ILog Log { get; }

        public CoreSettingsController(
            CoreSettings coreSettings,
            IOptionsMonitor<ILog> option
            )
        {
            CoreSettings = coreSettings;
            Log = option.Get("ASC.ApiSystem");
        }

        #region For TEST api

        [HttpGet("test")]
        public IActionResult Check()
        {
            return Ok(new
            {
                value = "CoreSettings api works"
            });
        }

        #endregion

        #region API methods

        [HttpGet("get")]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult GetSettings(int tenant, string key)
        {
            try
            {
                if (tenant < -1)
                {
                    return BadRequest(new
                    {
                        error = "portalNameIncorrect",
                        message = "Tenant is incorrect"
                    });
                }

                if (string.IsNullOrEmpty(key))
                {
                    return BadRequest(new
                    {
                        error = "params",
                        message = "Key is empty"
                    });
                }

                var settings = CoreSettings.GetSetting(key, tenant);

                return Ok(new
                {
                    settings
                });
            }
            catch (ArgumentException ae)
            {
                Log.Error(ae);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "error",
                    message = ae.Message
                });
            }
            catch (Exception e)
            {
                Log.Error(e);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "error",
                    message = e.Message
                });
            }
        }

        [HttpPost("save")]
        [Authorize(AuthenticationSchemes = "auth.allowskip")]
        public IActionResult SaveSettings([FromBody] CoreSettingsModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Key))
                {
                    return BadRequest(new
                    {
                        error = "params",
                        message = "Key is empty"
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

                var tenant = model.Tenant;

                if (tenant < -1)
                {
                    tenant = -1;
                }

                CoreSettings.SaveSetting(model.Key, model.Value, tenant);

                var settings = CoreSettings.GetSetting(model.Key, tenant);

                return Ok(new
                {
                    settings
                });
            }
            catch (ArgumentException ae)
            {
                Log.Error(ae);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "params",
                    message = ae.Message
                });
            }
            catch (Exception e)
            {
                Log.Error(e);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "error",
                    message = e.Message
                });
            }
        }

        #endregion
    }
}