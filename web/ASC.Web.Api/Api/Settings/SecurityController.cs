namespace ASC.Web.Api.Controllers.Settings;

public class SecurityController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly IServiceProvider _serviceProvider;
    private readonly EmployeeWraperHelper _employeeWraperHelper;
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
        IServiceProvider serviceProvider,
        EmployeeWraperHelper employeeWraperHelper,
        MessageTarget messageTarget,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _serviceProvider = serviceProvider;
        _employeeWraperHelper = employeeWraperHelper;
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

    [Read("security")]
    public IEnumerable<SecurityResponseDto> GetWebItemSecurityInfo([FromQuery] IEnumerable<string> ids)
    {
        if (ids == null || !ids.Any())
        {
            ids = _webItemManager.GetItemsAll().Select(i => i.ID.ToString());
        }

        var subItemList = _webItemManager.GetItemsAll().Where(item => item.IsSubItem()).Select(i => i.ID.ToString());

        return ids.Select(r => _webItemSecurity.GetSecurityInfo(r))
                    .Select(i => new SecurityResponseDto
                    {
                        WebItemId = i.WebItemId,
                        Enabled = i.Enabled,
                        Users = i.Users.Select(_employeeWraperHelper.Get),
                        Groups = i.Groups.Select(g => new GroupWrapperSummary(g, _userManager)),
                        IsSubItem = subItemList.Contains(i.WebItemId),
                    }).ToList();
    }

    [Read("security/{id}")]
    public bool GetWebItemSecurityInfo(Guid id)
    {
        var module = _webItemManager[id];

        return module != null && !module.IsDisabled(_webItemSecurity, _authContext);
    }

    [Read("security/modules")]
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

    [Read("security/password", Check = false)]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Everyone")]
    public object GetPasswordSettings()
    {
        var UserPasswordSettings = _settingsManager.Load<PasswordSettings>();

        return UserPasswordSettings;
    }

    [Update("security")]
    public IEnumerable<SecurityResponseDto> SetWebItemSecurityFromBody([FromBody] WebItemSecurityDto model)
    {
        return SetWebItemSecurity(model);
    }

    [Update("security")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<SecurityResponseDto> SetWebItemSecurityFromForm([FromForm] WebItemSecurityDto model)
    {
        return SetWebItemSecurity(model);
    }

    private IEnumerable<SecurityResponseDto> SetWebItemSecurity(WebItemSecurityDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _webItemSecurity.SetSecurity(model.Id, model.Enabled, model.Subjects?.ToArray());
        var securityInfo = GetWebItemSecurityInfo(new List<string> { model.Id });

        if (model.Subjects == null) return securityInfo;

        var productName = GetProductName(new Guid(model.Id));

        if (!model.Subjects.Any())
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

    [Update("security/access")]
    public IEnumerable<SecurityResponseDto> SetAccessToWebItemsFromBody([FromBody] WebItemSecurityDto model)
    {
        return SetAccessToWebItems(model);
    }

    [Update("security/access")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<SecurityResponseDto> SetAccessToWebItemsFromForm([FromForm] WebItemSecurityDto model)
    {
        return SetAccessToWebItems(model);
    }

    private IEnumerable<SecurityResponseDto> SetAccessToWebItems(WebItemSecurityDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var itemList = new ItemDictionary<string, bool>();

        foreach (var item in model.Items)
        {
            if (!itemList.ContainsKey(item.Key))
                itemList.Add(item.Key, item.Value);
        }

        var defaultPageSettings = _settingsManager.Load<StudioDefaultPageSettings>();

        foreach (var item in itemList)
        {
            Guid[] subjects = null;
            var productId = new Guid(item.Key);

            if (item.Value)
            {
                if (_webItemManager[productId] is IProduct webItem || productId == WebItemManager.MailProductID)
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
                _settingsManager.Save((StudioDefaultPageSettings)defaultPageSettings.GetDefault(_serviceProvider));
            }

            _webItemSecurity.SetSecurity(item.Key, item.Value, subjects);
        }

        _messageService.Send(MessageAction.ProductsListUpdated);

        return GetWebItemSecurityInfo(itemList.Keys.ToList());
    }

    [Read("security/administrator/{productid}")]
    public IEnumerable<EmployeeWraper> GetProductAdministrators(Guid productid)
    {
        return _webItemSecurity.GetProductAdministrators(productid)
                                .Select(_employeeWraperHelper.Get)
                                .ToList();
    }

    [Read("security/administrator")]
    public object IsProductAdministrator(Guid productid, Guid userid)
    {
        var result = _webItemSecurity.IsProductAdministrator(productid, userid);
        return new { ProductId = productid, UserId = userid, Administrator = result };
    }

    [Update("security/administrator")]
    public object SetProductAdministratorFromBody([FromBody] SecurityDto model)
    {
        return SetProductAdministrator(model);
    }

    [Update("security/administrator")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SetProductAdministratorFromForm([FromForm] SecurityDto model)
    {
        return SetProductAdministrator(model);
    }

    private object SetProductAdministrator(SecurityDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _webItemSecurity.SetProductAdministrator(model.ProductId, model.UserId, model.Administrator);

        var admin = _userManager.GetUsers(model.UserId);

        if (model.ProductId == Guid.Empty)
        {
            var messageAction = model.Administrator ? MessageAction.AdministratorOpenedFullAccess : MessageAction.AdministratorDeleted;
            _messageService.Send(messageAction, _messageTarget.Create(admin.Id), admin.DisplayUserName(false, _displayUserSettingsHelper));
        }
        else
        {
            var messageAction = model.Administrator ? MessageAction.ProductAddedAdministrator : MessageAction.ProductDeletedAdministrator;
            _messageService.Send(messageAction, _messageTarget.Create(admin.Id), GetProductName(model.ProductId), admin.DisplayUserName(false, _displayUserSettingsHelper));
        }

        return new { model.ProductId, model.UserId, model.Administrator };
    }
}
