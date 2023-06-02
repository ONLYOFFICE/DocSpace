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

public class SecurityController : BaseSettingsController
{
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly CoreBaseSettings _coreBaseSettings;
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
    private readonly IMapper _mapper;

    public SecurityController(
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        CoreBaseSettings coreBaseSettings,
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
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _employeeHelperDto = employeeWraperHelper;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _coreBaseSettings = coreBaseSettings;
        _messageService = messageService;
        _userManager = userManager;
        _authContext = authContext;
        _webItemSecurity = webItemSecurity;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _webItemManagerSecurity = webItemManagerSecurity;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
        _mapper = mapper;
    }

    [HttpGet("security")]
    public async IAsyncEnumerable<SecurityDto> GetWebItemSecurityInfo([FromQuery] IEnumerable<string> ids)
    {
        if (ids == null || !ids.Any())
        {
            ids = WebItemManager.GetItemsAll().Select(i => i.ID.ToString());
        }

        var subItemList = WebItemManager.GetItemsAll().Where(item => item.IsSubItem()).Select(i => i.ID.ToString());

        foreach (var r in ids)
        {
            var i = await _webItemSecurity.GetSecurityInfoAsync(r);

            var s = new SecurityDto
            {
                WebItemId = i.WebItemId,
                Enabled = i.Enabled,
                Groups = i.Groups.Select(g => new GroupSummaryDto(g, _userManager)),
                IsSubItem = subItemList.Contains(i.WebItemId),
            };

            s.Users = new List<EmployeeDto>();

            foreach (var e in i.Users)
            {
                s.Users.Add(await _employeeHelperDto.GetAsync(e));
            }

            yield return s;

        }
    }

    [HttpGet("security/{id}")]
    public async Task<bool> GetWebItemSecurityInfoAsync(Guid id)
    {
        var module = WebItemManager[id];

        return module != null && !await module.IsDisabledAsync(_webItemSecurity, _authContext);
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
    public async Task<PasswordSettings> GetPasswordSettingsAsync()
    {
        return await _settingsManager.LoadAsync<PasswordSettings>();
    }

    [HttpPut("security/password")]
    public async Task<PasswordSettings> UpdatePasswordSettingsAsync(PasswordSettingsRequestsDto model)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var userPasswordSettings = await _settingsManager.LoadAsync<PasswordSettings>();

        userPasswordSettings.MinLength = model.MinLength;
        userPasswordSettings.UpperCase = model.UpperCase;
        userPasswordSettings.Digits = model.Digits;
        userPasswordSettings.SpecSymbols = model.SpecSymbols;

        await _settingsManager.SaveAsync(userPasswordSettings);

        await _messageService.SendAsync(MessageAction.PasswordStrengthSettingsUpdated);

        return userPasswordSettings;

    }

    [HttpPut("security")]
    public async Task<IEnumerable<SecurityDto>> SetWebItemSecurity(WebItemSecurityRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _webItemSecurity.SetSecurityAsync(inDto.Id, inDto.Enabled, inDto.Subjects?.ToArray());
        var securityInfo = await GetWebItemSecurityInfo(new List<string> { inDto.Id }).ToListAsync();

        if (inDto.Subjects == null)
        {
            return securityInfo;
        }

        var productName = GetProductName(new Guid(inDto.Id));

        if (!inDto.Subjects.Any())
        {
            await _messageService.SendAsync(MessageAction.ProductAccessOpened, productName);
        }
        else
        {
            foreach (var info in securityInfo)
            {
                if (info.Groups.Any())
                {
                    await _messageService.SendAsync(MessageAction.GroupsOpenedProductAccess, productName, info.Groups.Select(x => x.Name));
                }
                if (info.Users.Any())
                {
                    await _messageService.SendAsync(MessageAction.UsersOpenedProductAccess, productName, info.Users.Select(x => HttpUtility.HtmlDecode(x.DisplayName)));
                }
            }
        }

        return securityInfo;
    }

    [HttpPut("security/access")]
    public async Task<IEnumerable<SecurityDto>> SetAccessToWebItems(WebItemSecurityRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var itemList = new ItemDictionary<string, bool>();

        foreach (var item in inDto.Items)
        {
            if (!itemList.ContainsKey(item.Key))
            {
                itemList.Add(item.Key, item.Value);
            }
        }

        var defaultPageSettings = await _settingsManager.LoadAsync<StudioDefaultPageSettings>();

        foreach (var item in itemList)
        {
            Guid[] subjects = null;
            var productId = new Guid(item.Key);

            if (item.Value)
            {
                if (WebItemManager[productId] is IProduct webItem || productId == WebItemManager.MailProductID)
                {
                    var productInfo = await _webItemSecurity.GetSecurityInfoAsync(item.Key);
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
                await _settingsManager.SaveAsync(_settingsManager.GetDefault<StudioDefaultPageSettings>());
            }

            await _webItemSecurity.SetSecurityAsync(item.Key, item.Value, subjects);
        }

        await _messageService.SendAsync(MessageAction.ProductsListUpdated);

        return await GetWebItemSecurityInfo(itemList.Keys.ToList()).ToListAsync();
    }

    [HttpGet("security/administrator/{productid}")]
    public async IAsyncEnumerable<EmployeeDto> GetProductAdministrators(Guid productid)
    {
        var admins = await _webItemSecurity.GetProductAdministratorsAsync(productid);

        foreach (var a in admins)
        {
            yield return await _employeeHelperDto.GetAsync(a);
        }
    }

    [HttpGet("security/administrator")]
    public async Task<object> IsProductAdministratorAsync(Guid productid, Guid userid)
    {
        var result = await _webItemSecurity.IsProductAdministratorAsync(productid, userid);
        return new { ProductId = productid, UserId = userid, Administrator = result };
    }

    [HttpPut("security/administrator")]
    public async Task<object> SetProductAdministrator(SecurityRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var isStartup = !_coreBaseSettings.CustomMode && _tenantExtra.Saas && (await _tenantManager.GetCurrentTenantQuotaAsync()).Free;
        if (isStartup)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Administrator");
        }

        await _webItemSecurity.SetProductAdministrator(inDto.ProductId, inDto.UserId, inDto.Administrator);

        var admin = await _userManager.GetUsersAsync(inDto.UserId);

        if (inDto.ProductId == Guid.Empty)
        {
            var messageAction = inDto.Administrator ? MessageAction.AdministratorOpenedFullAccess : MessageAction.AdministratorDeleted;
            await _messageService.SendAsync(messageAction, _messageTarget.Create(admin.Id), admin.DisplayUserName(false, _displayUserSettingsHelper));
        }
        else
        {
            var messageAction = inDto.Administrator ? MessageAction.ProductAddedAdministrator : MessageAction.ProductDeletedAdministrator;
            await _messageService.SendAsync(messageAction, _messageTarget.Create(admin.Id), GetProductName(inDto.ProductId), admin.DisplayUserName(false, _displayUserSettingsHelper));
        }

        return new { inDto.ProductId, inDto.UserId, inDto.Administrator };
    }

    [HttpPut("security/loginSettings")]
    public async Task<LoginSettingsDto> UpdateLoginSettingsAsync(LoginSettingsRequestDto loginSettingsRequestDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var attemptsCount = loginSettingsRequestDto.AttemptCount;
        var checkPeriod = loginSettingsRequestDto.CheckPeriod;
        var blockTime = loginSettingsRequestDto.BlockTime;

        if (attemptsCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptsCount));
        }
        if (checkPeriod < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(checkPeriod));
        }
        if (blockTime < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockTime));
        }

        var settings = new LoginSettings
        {
            AttemptCount = attemptsCount,
            CheckPeriod = checkPeriod,
            BlockTime = blockTime
        };

        await _settingsManager.SaveAsync(settings);

        return _mapper.Map<LoginSettings, LoginSettingsDto>(settings);
    }

    [HttpGet("security/loginSettings")]
    public async Task<LoginSettingsDto> GetLoginSettingsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settings = await _settingsManager.LoadAsync<LoginSettings>();

        return _mapper.Map<LoginSettings, LoginSettingsDto>(settings);
    }
}
