namespace ASC.Web.Api.Controllers.Settings;

public class CustomNavigationController: BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly StorageHelper _storageHelper;

    public CustomNavigationController(
        ApiContext apiContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        StorageHelper storageHelper,
        IMemoryCache memoryCache) : base (apiContext, memoryCache, webItemManager)
    {
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _storageHelper = storageHelper;
    }

    [Read("customnavigation/getall")]
    public List<CustomNavigationItem> GetCustomNavigationItems()
    {
        return _settingsManager.Load<CustomNavigationSettings>().Items;
    }

    [Read("customnavigation/getsample")]
    public CustomNavigationItem GetCustomNavigationItemSample()
    {
        return CustomNavigationItem.GetSample();
    }

    [Read("customnavigation/get/{id}")]
    public CustomNavigationItem GetCustomNavigationItem(Guid id)
    {
        return _settingsManager.Load<CustomNavigationSettings>().Items.FirstOrDefault(item => item.Id == id);
    }

    [Create("customnavigation/create")]
    public CustomNavigationItem CreateCustomNavigationItemFromBody([FromBody] CustomNavigationItem item)
    {
        return CreateCustomNavigationItem(item);
    }

    [Create("customnavigation/create")]
    [Consumes("application/x-www-form-urlencoded")]
    public CustomNavigationItem CreateCustomNavigationItemFromForm([FromForm] CustomNavigationItem item)
    {
        return CreateCustomNavigationItem(item);
    }

    private CustomNavigationItem CreateCustomNavigationItem(CustomNavigationItem item)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomNavigationSettings>();

        var exist = false;

        foreach (var existItem in settings.Items)
        {
            if (existItem.Id != item.Id) continue;

            existItem.Label = item.Label;
            existItem.Url = item.Url;
            existItem.ShowInMenu = item.ShowInMenu;
            existItem.ShowOnHomePage = item.ShowOnHomePage;

            if (existItem.SmallImg != item.SmallImg)
            {
                _storageHelper.DeleteLogo(existItem.SmallImg);
                existItem.SmallImg = _storageHelper.SaveTmpLogo(item.SmallImg);
            }

            if (existItem.BigImg != item.BigImg)
            {
                _storageHelper.DeleteLogo(existItem.BigImg);
                existItem.BigImg = _storageHelper.SaveTmpLogo(item.BigImg);
            }

            exist = true;
            break;
        }

        if (!exist)
        {
            item.Id = Guid.NewGuid();
            item.SmallImg = _storageHelper.SaveTmpLogo(item.SmallImg);
            item.BigImg = _storageHelper.SaveTmpLogo(item.BigImg);

            settings.Items.Add(item);
        }

        _settingsManager.Save(settings);

        _messageService.Send(MessageAction.CustomNavigationSettingsUpdated);

        return item;
    }

    [Delete("customnavigation/delete/{id}")]
    public void DeleteCustomNavigationItem(Guid id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomNavigationSettings>();

        var terget = settings.Items.FirstOrDefault(item => item.Id == id);

        if (terget == null) return;

        _storageHelper.DeleteLogo(terget.SmallImg);
        _storageHelper.DeleteLogo(terget.BigImg);

        settings.Items.Remove(terget);
        _settingsManager.Save(settings);

        _messageService.Send(MessageAction.CustomNavigationSettingsUpdated);
    }
}
