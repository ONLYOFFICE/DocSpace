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

public class CustomNavigationController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly StorageHelper _storageHelper;

    public CustomNavigationController(
        MessageService messageService,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        StorageHelper storageHelper,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _messageService = messageService;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _storageHelper = storageHelper;
    }

    /// <summary>
    /// Returns a list of the custom navigation items.
    /// </summary>
    /// <short>Get the custom navigation items</short>
    /// <category>Custom navigation</category>
    /// <returns type="ASC.Web.Studio.Core.CustomNavigationItem, ASC.Web.Core">List of the custom navigation items</returns>
    /// <path>api/2.0/settings/customnavigation/getall</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("customnavigation/getall")]
    public async Task<List<CustomNavigationItem>> GetCustomNavigationItemsAsync()
    {
        return (await _settingsManager.LoadAsync<CustomNavigationSettings>()).Items;
    }

    /// <summary>
    /// Returns a custom navigation item sample.
    /// </summary>
    /// <short>Get a custom navigation item sample</short>
    /// <category>Custom navigation</category>
    /// <returns type="ASC.Web.Studio.Core.CustomNavigationItem, ASC.Web.Core">Custom navigation item</returns>
    /// <path>api/2.0/settings/customnavigation/getsample</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("customnavigation/getsample")]
    public CustomNavigationItem GetCustomNavigationItemSample()
    {
        return CustomNavigationItem.GetSample();
    }

    /// <summary>
    /// Returns a custom navigation item by the ID specified in the request.
    /// </summary>
    /// <short>Get a custom navigation item by ID</short>
    /// <category>Custom navigation</category>
    /// <param type="System.Guid, System" method="url" name="id">Custom navigation item ID</param>
    /// <returns type="ASC.Web.Studio.Core.CustomNavigationItem, ASC.Web.Core">Custom navigation item</returns>
    /// <path>api/2.0/settings/customnavigation/get/{id}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("customnavigation/get/{id}")]
    public async Task<CustomNavigationItem> GetCustomNavigationItemAsync(Guid id)
    {
        return (await _settingsManager.LoadAsync<CustomNavigationSettings>()).Items.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Adds a custom navigation item with the parameters specified in the request.
    /// </summary>
    /// <short>Add a custom navigation item</short>
    /// <category>Custom navigation</category>
    /// <param type="ASC.Web.Studio.Core.CustomNavigationItem, ASC.Web.Core" name="inDto">Custom navigation parameters</param>
    /// <returns type="ASC.Web.Studio.Core.CustomNavigationItem, ASC.Web.Core">Custom navigation item</returns>
    /// <path>api/2.0/settings/customnavigation/create</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("customnavigation/create")]
    public async Task<CustomNavigationItem> CreateCustomNavigationItem(CustomNavigationItem inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settings = await _settingsManager.LoadAsync<CustomNavigationSettings>();

        var exist = false;

        foreach (var existItem in settings.Items)
        {
            if (existItem.Id != inDto.Id)
            {
                continue;
            }

            existItem.Label = inDto.Label;
            existItem.Url = inDto.Url;
            existItem.ShowInMenu = inDto.ShowInMenu;
            existItem.ShowOnHomePage = inDto.ShowOnHomePage;

            if (existItem.SmallImg != inDto.SmallImg)
            {
                await _storageHelper.DeleteLogoAsync(existItem.SmallImg);
                existItem.SmallImg = await _storageHelper.SaveTmpLogo(inDto.SmallImg);
            }

            if (existItem.BigImg != inDto.BigImg)
            {
                await _storageHelper.DeleteLogoAsync(existItem.BigImg);
                existItem.BigImg = await _storageHelper.SaveTmpLogo(inDto.BigImg);
            }

            exist = true;
            break;
        }

        if (!exist)
        {
            inDto.Id = Guid.NewGuid();
            inDto.SmallImg = await _storageHelper.SaveTmpLogo(inDto.SmallImg);
            inDto.BigImg = await _storageHelper.SaveTmpLogo(inDto.BigImg);

            settings.Items.Add(inDto);
        }

        await _settingsManager.SaveAsync(settings);

        await _messageService.SendAsync(MessageAction.CustomNavigationSettingsUpdated);

        return inDto;
    }

    /// <summary>
    /// Deletes a custom navigation item with the ID specified in the request.
    /// </summary>
    /// <short>Delete a custom navigation item</short>
    /// <category>Custom navigation</category>
    /// <param type="System.Guid, System" method="url" name="id">Custom navigation item ID</param>
    /// <path>api/2.0/settings/customnavigation/delete/{id}</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <returns></returns>
    [HttpDelete("customnavigation/delete/{id}")]
    public async Task DeleteCustomNavigationItem(Guid id)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settings = await _settingsManager.LoadAsync<CustomNavigationSettings>();

        var terget = settings.Items.FirstOrDefault(item => item.Id == id);

        if (terget == null)
        {
            return;
        }

        await _storageHelper.DeleteLogoAsync(terget.SmallImg);
        await _storageHelper.DeleteLogoAsync(terget.BigImg);

        settings.Items.Remove(terget);
        await _settingsManager.SaveAsync(settings);

        await _messageService.SendAsync(MessageAction.CustomNavigationSettingsUpdated);
    }
}
