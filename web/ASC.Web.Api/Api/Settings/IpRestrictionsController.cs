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

namespace ASC.Web.Api.Controllers.Settings;

public class IpRestrictionsController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly IPRestrictionsService _iPRestrictionsService;

    public IpRestrictionsController(
        ApiContext apiContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        IPRestrictionsService iPRestrictionsService,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _iPRestrictionsService = iPRestrictionsService;
    }

    /// <summary>
    /// Returns the IP portal restrictions.
    /// </summary>
    /// <short>Get the IP portal restrictions</short>
    /// <category>IP restrictions</category>
    /// <returns type="ASC.IPSecurity.IPRestriction, ASC.IPSecurity">List of IP restrictions parameters</returns>
    /// <path>api/2.0/settings/iprestrictions</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("iprestrictions")]
    public async Task<IEnumerable<IPRestriction>> GetIpRestrictionsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        return await _iPRestrictionsService.GetAsync(Tenant.Id);
    }

    /// <summary>
    /// Saves the new portal IP restrictions specified in the request.
    /// </summary>
    /// <short>Save the IP restrictions</short>
    /// <category>IP restrictions</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.IpRestrictionsRequestsDto, ASC.Web.Api" name="inDto">IP restrictions request parameters</param>
    /// <returns type="ASC.IPSecurity.IpRestrictionBase, ASC.IPSecurity">List of updated IP restrictions</returns>
    /// <path>api/2.0/settings/iprestrictions</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("iprestrictions")]
    public async Task<IEnumerable<IpRestrictionBase>> SaveIpRestrictionsAsync(IpRestrictionsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        return await _iPRestrictionsService.SaveAsync(inDto.IpRestrictions, Tenant.Id);
    }

    /// <summary>
    /// Returns the IP restriction settings.
    /// </summary>
    /// <short>Get the IP restriction settings</short>
    /// <category>IP restrictions</category>
    /// <returns type="ASC.IPSecurity.IPRestrictionsSettings, ASC.IPSecurity">IP restriction settings</returns>
    /// <path>api/2.0/settings/iprestrictions/settings</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("iprestrictions/settings")]
    public async Task<IPRestrictionsSettings> ReadIpRestrictionsSettingsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        return await _settingsManager.LoadAsync<IPRestrictionsSettings>();
    }

    /// <summary>
    /// Updates the IP restriction settings with a parameter specified in the request.
    /// </summary>
    /// <short>Update the IP restriction settings</short>
    /// <category>IP restrictions</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.IpRestrictionsRequestsDto, ASC.Web.Api" name="inDto">New IP restriction settings</param>
    /// <returns type="ASC.IPSecurity.IPRestrictionsSettings, ASC.IPSecurity">Updated IP restriction settings</returns>
    /// <path>api/2.0/settings/iprestrictions/settings</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("iprestrictions/settings")]
    public async Task<IPRestrictionsSettings> UpdateIpRestrictionsSettingsAsync(IpRestrictionsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settings = new IPRestrictionsSettings { Enable = inDto.Enable };
        await _settingsManager.SaveAsync(settings);

        return settings;
    }
}