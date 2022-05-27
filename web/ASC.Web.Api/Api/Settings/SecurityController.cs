﻿// (c) Copyright Ascensio System SIA 2010-2022
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

public class SecurityController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly EmployeeDtoHelper _employeeHelperDto;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly WebItemManagerSecurity _webItemManagerSecurity;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;

    public SecurityController(
        MessageService messageService,
        ApiContext apiContext,
        UserManager userManager,
        AuthContext authContext,
        WebItemSecurity webItemSecurity,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        WebItemManagerSecurity webItemManagerSecurity,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        EmployeeDtoHelper employeeWraperHelper,
        MessageTarget messageTarget,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _employeeHelperDto = employeeWraperHelper;
        _messageService = messageService;
        _userManager = userManager;
        _authContext = authContext;
        _webItemSecurity = webItemSecurity;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _webItemManagerSecurity = webItemManagerSecurity;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
    }

    [HttpGet("security")]
    public IEnumerable<SecurityDto> GetWebItemSecurityInfo([FromQuery] IEnumerable<string> ids)
    {
        if (ids == null || !ids.Any())
        {
            ids = WebItemManager.GetItemsAll().Select(i => i.ID.ToString());
        }

        var subItemList = WebItemManager.GetItemsAll().Where(item => item.IsSubItem()).Select(i => i.ID.ToString());

        return ids.Select(r => _webItemSecurity.GetSecurityInfo(r))
                    .Select(i => new SecurityDto
                    {
                        WebItemId = i.WebItemId,
                        Enabled = i.Enabled,
                        Users = i.Users.Select(_employeeHelperDto.Get),
                        Groups = i.Groups.Select(g => new GroupSummaryDto(g, _userManager)),
                        IsSubItem = subItemList.Contains(i.WebItemId),
                    }).ToList();
    }

    [HttpGet("security/{id}")]
    public bool GetWebItemSecurityInfo(Guid id)
    {
        var module = WebItemManager[id];

        return module != null && !module.IsDisabled(_webItemSecurity, _authContext);
    }

    [HttpGet("security/modules")]
    public object GetEnabledModules()
    {
        var EnabledModules = _webItemManagerSecurity.GetItems(WebZoneType.All, ItemAvailableState.Normal)
                                    .Where(item => !item.IsSubItem() && item.Visible)
                                    .Select(item => new
                                    {
                                        id = item.ProductClassName.HtmlEncode(),
                                        title = item.Name.HtmlEncode()
                                    });

        return EnabledModules;
    }

    [HttpGet("security/password")]
    [AllowNotPayment]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Everyone")]
    public PasswordSettings GetPasswordSettings()
    {
        return _settingsManager.Load<PasswordSettings>();
    }

    [HttpPut("security/password")]
    public PasswordSettings UpdatePasswordSettings(PasswordSettingsRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var userPasswordSettings = _settingsManager.Load<PasswordSettings>();

        userPasswordSettings.MinLength = model.MinLength;
        userPasswordSettings.UpperCase = model.UpperCase;
        userPasswordSettings.Digits = model.Digits;
        userPasswordSettings.SpecSymbols = model.SpecSymbols;

        _settingsManager.Save(userPasswordSettings);

        _messageService.Send(MessageAction.PasswordStrengthSettingsUpdated);

        return userPasswordSettings;

    }

    [HttpPut("security")]
    public IEnumerable<SecurityDto> SetWebItemSecurity(WebItemSecurityRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _webItemSecurity.SetSecurity(inDto.Id, inDto.Enabled, inDto.Subjects?.ToArray());
        var securityInfo = GetWebItemSecurityInfo(new List<string> { inDto.Id });

        if (inDto.Subjects == null)
        {
            return securityInfo;
        }

        var productName = GetProductName(new Guid(inDto.Id));

        if (!inDto.Subjects.Any())
        {
            _messageService.Send(MessageAction.ProductAccessOpened, productName);
        }
        else
        {
            foreach (var info in securityInfo)
            {
                if (info.Groups.Any())
                {
                    _messageService.Send(MessageAction.GroupsOpenedProductAccess, productName, info.Groups.Select(x => x.Name));
                }
                if (info.Users.Any())
                {
                    _messageService.Send(MessageAction.UsersOpenedProductAccess, productName, info.Users.Select(x => HttpUtility.HtmlDecode(x.DisplayName)));
                }
            }
        }

        return securityInfo;
    }

    [HttpPut("security/access")]
    public IEnumerable<SecurityDto> SetAccessToWebItems(WebItemSecurityRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var itemList = new ItemDictionary<string, bool>();

        foreach (var item in inDto.Items)
        {
            if (!itemList.ContainsKey(item.Key))
            {
                itemList.Add(item.Key, item.Value);
            }
        }

        var defaultPageSettings = _settingsManager.Load<StudioDefaultPageSettings>();

        foreach (var item in itemList)
        {
            Guid[] subjects = null;
            var productId = new Guid(item.Key);

            if (item.Value)
            {
                if (WebItemManager[productId] is IProduct webItem || productId == WebItemManager.MailProductID)
                {
                    var productInfo = _webItemSecurity.GetSecurityInfo(item.Key);
                    var selectedGroups = productInfo.Groups.Select(group => group.ID).ToList();
                    var selectedUsers = productInfo.Users.Select(user => user.Id).ToList();
                    selectedUsers.AddRange(selectedGroups);
                    if (selectedUsers.Count > 0)
                    {
                        subjects = selectedUsers.ToArray();
                    }
                }
            }
            else if (productId == defaultPageSettings.DefaultProductID)
            {
                _settingsManager.Save(_settingsManager.GetDefault<StudioDefaultPageSettings>());
            }

            _webItemSecurity.SetSecurity(item.Key, item.Value, subjects);
        }

        _messageService.Send(MessageAction.ProductsListUpdated);

        return GetWebItemSecurityInfo(itemList.Keys.ToList());
    }

    [HttpGet("security/administrator/{productid}")]
    public IEnumerable<EmployeeDto> GetProductAdministrators(Guid productid)
    {
        return _webItemSecurity.GetProductAdministrators(productid)
                                .Select(_employeeHelperDto.Get)
                                .ToList();
    }

    [HttpGet("security/administrator")]
    public object IsProductAdministrator(Guid productid, Guid userid)
    {
        var result = _webItemSecurity.IsProductAdministrator(productid, userid);
        return new { ProductId = productid, UserId = userid, Administrator = result };
    }

    [HttpPut("security/administrator")]
    public object SetProductAdministrator(SecurityRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _webItemSecurity.SetProductAdministrator(inDto.ProductId, inDto.UserId, inDto.Administrator);

        var admin = _userManager.GetUsers(inDto.UserId);

        if (inDto.ProductId == Guid.Empty)
        {
            var messageAction = inDto.Administrator ? MessageAction.AdministratorOpenedFullAccess : MessageAction.AdministratorDeleted;
            _messageService.Send(messageAction, _messageTarget.Create(admin.Id), admin.DisplayUserName(false, _displayUserSettingsHelper));
        }
        else
        {
            var messageAction = inDto.Administrator ? MessageAction.ProductAddedAdministrator : MessageAction.ProductDeletedAdministrator;
            _messageService.Send(messageAction, _messageTarget.Create(admin.Id), GetProductName(inDto.ProductId), admin.DisplayUserName(false, _displayUserSettingsHelper));
        }

        return new { inDto.ProductId, inDto.UserId, inDto.Administrator };
    }
}
