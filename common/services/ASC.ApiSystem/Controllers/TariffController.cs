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
public class TariffController : ControllerBase
{
    private CommonMethods CommonMethods { get; }
    private HostedSolution HostedSolution { get; }
    private ILogger<TariffController> Log { get; }
    public TariffController(
        CommonMethods commonMethods,
        HostedSolution hostedSolution,
        ILogger<TariffController> option
        )
    {
        CommonMethods = commonMethods;
        HostedSolution = hostedSolution;
        Log = option;
    }

    #region For TEST api

    [HttpGet("test")]
    public IActionResult Check()
    {
        return Ok(new
        {
            value = "Tariff api works"
        });
    }

    #endregion

    #region API methods

    [HttpPut("set")]
    [AllowCrossSiteJson]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> SetTariffAsync(TariffModel model)
    {
        (var succ, var tenant) = await CommonMethods.TryGetTenantAsync(model);
        if (!succ)
        {
            Log.LogError("Model without tenant");

            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        if (tenant == null)
        {
            Log.LogError("Tenant not found");

            return BadRequest(new
            {
                error = "portalNameNotFound",
                message = "Portal not found"
            });
        }

        var quota = new TenantQuota(tenant.Id)
        {
            CountRoomAdmin = 10000,
            Features = model.Features ?? "",
            MaxFileSize = 1024 * 1024 * 1024,
            MaxTotalSize = 1024L * 1024 * 1024 * 1024 - 1,
            Name = "api",
        };

        if (model.ActiveUsers != default)
        {
            quota.CountRoomAdmin = model.ActiveUsers;
        }

        if (model.MaxTotalSize != default)
        {
            quota.MaxTotalSize = model.MaxTotalSize;
        }

        if (model.MaxFileSize != default)
        {
            quota.MaxFileSize = model.MaxFileSize;
        }

        await HostedSolution.SaveTenantQuotaAsync(quota);

        var tariff = new Tariff
        {
            Quotas = new List<Quota> { new Quota(quota.Tenant, 1) },
            DueDate = model.DueDate != default ? model.DueDate : DateTime.MaxValue.AddSeconds(-1),
        };

        await HostedSolution.SetTariffAsync(tenant.Id, tariff);

        return await GetTariffAsync(tenant);
    }

    [HttpGet("get")]
    [AllowCrossSiteJson]
    [Authorize(AuthenticationSchemes = "auth:allowskip:default")]
    public async Task<IActionResult> GetTariffAsync([FromQuery] TariffModel model)
    {
        (var succ, var tenant) = await CommonMethods.TryGetTenantAsync(model);
        if (!succ)
        {
            Log.LogError("Model without tenant");

            return BadRequest(new
            {
                error = "portalNameEmpty",
                message = "PortalName is required"
            });
        }

        if (tenant == null)
        {
            Log.LogError("Tenant not found");

            return BadRequest(new
            {
                error = "portalNameNotFound",
                message = "Portal not found"
            });
        }

        return await GetTariffAsync(tenant);
    }

    [HttpGet("all")]
    [AllowCrossSiteJson]
    public async Task<IActionResult> GetTariffsAsync()
    {
        var tariffs = (await HostedSolution.GetTenantQuotasAsync())
            .Where(q => !q.Trial && !q.Free)
            .OrderBy(q => q.CountRoomAdmin)
            .ThenByDescending(q => q.Tenant)
            .Select(q => ToTariffWrapper(null, q));

        return Ok(new
        {
            tariffs
        });
    }

    #endregion

    #region private methods

    private async Task<IActionResult> GetTariffAsync(Tenant tenant)
    {
        var tariff = await HostedSolution.GetTariffAsync(tenant.Id, false);

        var quota = await HostedSolution.GetTenantQuotaAsync(tenant.Id);

        return Ok(new
        {
            tenant = CommonMethods.ToTenantWrapper(tenant),
            tariff = ToTariffWrapper(tariff, quota),
        });
    }

    private static object ToTariffWrapper(Tariff t, TenantQuota q)
    {
        return new
        {
            countManager = q.CountRoomAdmin,
            dueDate = t == null ? DateTime.MaxValue : t.DueDate,
            features = q.Features,
            maxFileSize = q.MaxFileSize,
            maxTotalSize = q.MaxTotalSize,
            state = t == null ? TariffState.Paid : t.State,
        };
    }

    #endregion
}
