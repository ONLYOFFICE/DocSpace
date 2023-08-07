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

public class GreetingSettingsController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;

    public GreetingSettingsController(
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        MessageService messageService,
        ApiContext apiContext,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _messageService = messageService;
        _tenantManager = tenantManager;
        _permissionContext = permissionContext;
    }

    /// <summary>
    /// Returns the greeting settings for the current portal.
    /// </summary>
    /// <short>Get greeting settings</short>
    /// <category>Greeting settings</category>
    /// <returns type="System.Object, System">Greeting settings: tenant name</returns>
    /// <path>api/2.0/settings/greetingsettings</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("greetingsettings")]
    public ContentResult GetGreetingSettings()
    {
        return new ContentResult { Content = Tenant.Name == "" ? Resource.PortalName : Tenant.Name };
    }

    /// <summary>
    /// Checks if the greeting settings of the current portal are set to default or not.
    /// </summary>
    /// <short>Check the default greeting settings</short>
    /// <category>Greeting settings</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the greeting settings of the current portal are set to default</returns>
    /// <path>api/2.0/settings/greetingsettings/isdefault</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("greetingsettings/isdefault")]
    public bool IsDefault()
    {
        return Tenant.Name == "";
    }

    /// <summary>
    /// Saves the greeting settings specified in the request to the current portal.
    /// </summary>
    /// <short>Save the greeting settings</short>
    /// <category>Greeting settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.GreetingSettingsRequestsDto, ASC.Web.Api" name="inDto">Greeting settings</param>
    /// <returns type="System.Object, System">Message about saving greeting settings successfully</returns>
    /// <path>api/2.0/settings/greetingsettings</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("greetingsettings")]
    public async Task<ContentResult> SaveGreetingSettingsAsync(GreetingSettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        Tenant.Name = inDto.Title;
        await _tenantManager.SaveTenantAsync(Tenant);

        await _messageService.SendAsync(MessageAction.GreetingSettingsUpdated);

        return new ContentResult { Content = Resource.SuccessfullySaveGreetingSettingsMessage };
    }

    /// <summary>
    /// Restores the current portal greeting settings.
    /// </summary>
    /// <short>Restore the greeting settings</short>
    /// <category>Greeting settings</category>
    /// <returns type="System.Object, System">Greeting settings: tenant name</returns>
    /// <path>api/2.0/settings/greetingsettings/restore</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("greetingsettings/restore")]
    public async Task<ContentResult> RestoreGreetingSettingsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _tenantInfoSettingsHelper.RestoreDefaultTenantNameAsync();

        return new ContentResult
        {
            Content = Tenant.Name == "" ? Resource.PortalName : Tenant.Name
        };
    }
}
