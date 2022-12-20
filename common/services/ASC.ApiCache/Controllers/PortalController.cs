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

namespace ASC.ApiCache.Controllers;

[Scope]
[ApiController]
[Route("[controller]")]
public class PortalController : ControllerBase
{
    private readonly PortalControllerHelper _portalControllerHelper;
    private readonly ILogger<PortalController> _log;

    public PortalController(PortalControllerHelper portalControllerHelper,
        ILogger<PortalController> log)
    {
        _portalControllerHelper = portalControllerHelper;
        _log = log;
    }

    [HttpGet("test")]
    public IActionResult Check()
    {
        return Ok(new
        {
            value = "Portal cache api works"
        });
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindPortalNameInCache([FromQuery] CacheDto inDto)
    {
        if (string.IsNullOrEmpty(inDto.PortalName))
        {
            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        _log.LogDebug("FindPortalNameInCache method. portalname = {0}", inDto.PortalName);

        var sameAliasTenants = await _portalControllerHelper.FindTenantsInCacheAsync(inDto.PortalName);
        return Ok(new
        {
            variants = sameAliasTenants
        });
    }

    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> AddPortalNameToCacheAsync(CacheDto inDto)
    {
        if (String.IsNullOrEmpty(inDto.PortalName))
        {
            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        try
        {
            await _portalControllerHelper.AddTenantToCacheAsync(inDto.PortalName);
        }
        catch (Exception e)
        {
            _log.LogError(e, "registerNewTenantError");
            return BadRequest(new { errors = "registerNewTenantError", message = e.Message, stacktrace = e.StackTrace });
        }

        return Ok(new
        {
            portalName = inDto.PortalName
        });
    }

    [HttpDelete("remove")]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> RemoveFromCacheAsync([FromQuery] CacheDto inDto)
    {
        if (String.IsNullOrEmpty(inDto.PortalName))
        {
            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        try
        {
            await _portalControllerHelper.RemoveTenantFromCacheAsync(inDto.PortalName);
        }
        catch (Exception e)
        {
            _log.LogError(e, "removeTenantFromCacheError");
            return BadRequest(new { errors = "removeTenantFromCacheError", message = e.Message, stacktrace = e.StackTrace });
        }

        return Ok(new
        {
            portalName = inDto.PortalName
        });
    }
}
