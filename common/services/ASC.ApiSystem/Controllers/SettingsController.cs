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
public class SettingsController : ControllerBase
{
    private CommonMethods CommonMethods { get; }
    private CoreSettings CoreSettings { get; }
    private ILogger<SettingsController> Log { get; }

    public SettingsController(
        CommonMethods commonMethods,
        CoreSettings coreSettings,
        ILogger<SettingsController> option)
    {
        CommonMethods = commonMethods;
        CoreSettings = coreSettings;
        Log = option;
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
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> GetSettingsAsync([FromQuery] SettingsModel model)
    {
        (var succ, var tenantId, var error) = await GetTenantAsync(model);
        if (!succ)
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

        var settings = await CoreSettings.GetSettingAsync(model.Key, tenantId);

        return Ok(new
        {
            settings
        });
    }

    [HttpPost("save")]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> SaveSettingsAsync([FromBody] SettingsModel model)
    {
        (var succ, var tenantId, var error) = await GetTenantAsync(model);
        if (!succ)
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

        Log.LogDebug("Set {0} value {1} for {2}", model.Key, model.Value, tenantId);

        await CoreSettings.SaveSettingAsync(model.Key, model.Value, tenantId);

        var settings = await CoreSettings.GetSettingAsync(model.Key, tenantId);

        return Ok(new
        {
            settings
        });
    }

    #endregion

    #region private methods

    private async Task<(bool, int, object)> GetTenantAsync(SettingsModel model)
    {
        object error = null;
        var tenantId = -1;

        if (model == null)
        {
            error = new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            };

            Log.LogError("Model is null");

            return (false, tenantId, error);
        }

        if (model.TenantId.HasValue && model.TenantId.Value == -1)
        {
            tenantId = model.TenantId.Value;
            return (true, tenantId, error);
        }

        (var succ, var tenant) = await CommonMethods.TryGetTenantAsync(model);
        if (!succ)
        {
            error = new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            };

            Log.LogError("Model without tenant");

            return (false, tenantId, error);
        }

        if (tenant == null)
        {
            error = new
            {
                error = "portalNameNotFound",
                message = "Portal not found"
            };

            Log.LogError("Tenant not found");

            return (false, tenantId, error);
        }

        tenantId = tenant.Id;
        return (true, tenantId, error);
    }

    #endregion
}
