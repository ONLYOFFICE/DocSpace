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

    [HttpGet("customnavigation/getall")]
    public List<CustomNavigationItem> GetCustomNavigationItems()
    {
        return _settingsManager.Load<CustomNavigationSettings>().Items;
    }

    [HttpGet("customnavigation/getsample")]
    public CustomNavigationItem GetCustomNavigationItemSample()
    {
        return CustomNavigationItem.GetSample();
    }

    [HttpGet("customnavigation/get/{id}")]
    public CustomNavigationItem GetCustomNavigationItem(Guid id)
    {
        return _settingsManager.Load<CustomNavigationSettings>().Items.FirstOrDefault(item => item.Id == id);
    }

    [HttpPost("customnavigation/create")]
    public async Task<CustomNavigationItem> CreateCustomNavigationItem(CustomNavigationItem item)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomNavigationSettings>();

        var exist = false;

        foreach (var existItem in settings.Items)
        {
            if (existItem.Id != item.Id)
            {
                continue;
            }

            existItem.Label = item.Label;
            existItem.Url = item.Url;
            existItem.ShowInMenu = item.ShowInMenu;
            existItem.ShowOnHomePage = item.ShowOnHomePage;

            if (existItem.SmallImg != item.SmallImg)
            {
                await _storageHelper.DeleteLogo(existItem.SmallImg);
                existItem.SmallImg = await _storageHelper.SaveTmpLogo(item.SmallImg);
            }

            if (existItem.BigImg != item.BigImg)
            {
                await _storageHelper.DeleteLogo(existItem.BigImg);
                existItem.BigImg = await _storageHelper.SaveTmpLogo(item.BigImg);
            }

            exist = true;
            break;
        }

        if (!exist)
        {
            item.Id = Guid.NewGuid();
            item.SmallImg = await _storageHelper.SaveTmpLogo(item.SmallImg);
            item.BigImg = await _storageHelper.SaveTmpLogo(item.BigImg);

            settings.Items.Add(item);
        }

        _settingsManager.Save(settings);

        _messageService.Send(MessageAction.CustomNavigationSettingsUpdated);

        return item;
    }

    [HttpDelete("customnavigation/delete/{id}")]
    public async Task DeleteCustomNavigationItem(Guid id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomNavigationSettings>();

        var terget = settings.Items.FirstOrDefault(item => item.Id == id);

        if (terget == null)
        {
            return;
        }

        await _storageHelper.DeleteLogo(terget.SmallImg);
        await _storageHelper.DeleteLogo(terget.BigImg);

        settings.Items.Remove(terget);
        _settingsManager.Save(settings);

        _messageService.Send(MessageAction.CustomNavigationSettingsUpdated);
    }
}
