namespace ASC.Web.Api.Controllers.Settings;

public class CustomSchemasController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;

    public CustomSchemasController(
        MessageService messageService,
        ApiContext apiContext,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        CustomNamingPeople customNamingPeople,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _messageService = messageService;
        _customNamingPeople = customNamingPeople;
        _tenantManager = tenantManager;
        _permissionContext = permissionContext;
    }

    [Read("customschemas")]
    public List<SchemaDto> PeopleSchemas()
    {
        return _customNamingPeople
                .GetSchemas()
                .Select(r =>
                {
                    var names = _customNamingPeople.GetPeopleNames(r.Key);

                    return new SchemaDto
                    {
                        Id = names.Id,
                        Name = names.SchemaName,
                        UserCaption = names.UserCaption,
                        UsersCaption = names.UsersCaption,
                        GroupCaption = names.GroupCaption,
                        GroupsCaption = names.GroupsCaption,
                        UserPostCaption = names.UserPostCaption,
                        RegDateCaption = names.RegDateCaption,
                        GroupHeadCaption = names.GroupHeadCaption,
                        GuestCaption = names.GuestCaption,
                        GuestsCaption = names.GuestsCaption,
                    };
                })
                .ToList();
    }

    [Create("customschemas")]
    public SchemaDto SaveNamingSettings(SchemaDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _customNamingPeople.SetPeopleNames(model.Id);

        _tenantManager.SaveTenant(_tenantManager.GetCurrentTenant());

        _messageService.Send(MessageAction.TeamTemplateChanged);

        return PeopleSchema(model.Id);
    }

    [Update("customschemas")]
    public SchemaDto SaveCustomNamingSettings(SchemaDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var usrCaption = (model.UserCaption ?? "").Trim();
        var usrsCaption = (model.UsersCaption ?? "").Trim();
        var grpCaption = (model.GroupCaption ?? "").Trim();
        var grpsCaption = (model.GroupsCaption ?? "").Trim();
        var usrStatusCaption = (model.UserPostCaption ?? "").Trim();
        var regDateCaption = (model.RegDateCaption ?? "").Trim();
        var grpHeadCaption = (model.GroupHeadCaption ?? "").Trim();
        var guestCaption = (model.GuestCaption ?? "").Trim();
        var guestsCaption = (model.GuestsCaption ?? "").Trim();

        if (string.IsNullOrEmpty(usrCaption)
            || string.IsNullOrEmpty(usrsCaption)
            || string.IsNullOrEmpty(grpCaption)
            || string.IsNullOrEmpty(grpsCaption)
            || string.IsNullOrEmpty(usrStatusCaption)
            || string.IsNullOrEmpty(regDateCaption)
            || string.IsNullOrEmpty(grpHeadCaption)
            || string.IsNullOrEmpty(guestCaption)
            || string.IsNullOrEmpty(guestsCaption))
        {
            throw new Exception(Resource.ErrorEmptyFields);
        }

        var names = new PeopleNamesItem
        {
            Id = PeopleNamesItem.CustomID,
            UserCaption = usrCaption.Substring(0, Math.Min(30, usrCaption.Length)),
            UsersCaption = usrsCaption.Substring(0, Math.Min(30, usrsCaption.Length)),
            GroupCaption = grpCaption.Substring(0, Math.Min(30, grpCaption.Length)),
            GroupsCaption = grpsCaption.Substring(0, Math.Min(30, grpsCaption.Length)),
            UserPostCaption = usrStatusCaption.Substring(0, Math.Min(30, usrStatusCaption.Length)),
            RegDateCaption = regDateCaption.Substring(0, Math.Min(30, regDateCaption.Length)),
            GroupHeadCaption = grpHeadCaption.Substring(0, Math.Min(30, grpHeadCaption.Length)),
            GuestCaption = guestCaption.Substring(0, Math.Min(30, guestCaption.Length)),
            GuestsCaption = guestsCaption.Substring(0, Math.Min(30, guestsCaption.Length)),
        };

        _customNamingPeople.SetPeopleNames(names);

        _tenantManager.SaveTenant(_tenantManager.GetCurrentTenant());

        _messageService.Send(MessageAction.TeamTemplateChanged);

        return PeopleSchema(PeopleNamesItem.CustomID);
    }

    [Read("customschemas/{id}")]
    public SchemaDto PeopleSchema(string id)
    {
        var names = _customNamingPeople.GetPeopleNames(id);
        var schemaItem = new SchemaDto
        {
            Id = names.Id,
            Name = names.SchemaName,
            UserCaption = names.UserCaption,
            UsersCaption = names.UsersCaption,
            GroupCaption = names.GroupCaption,
            GroupsCaption = names.GroupsCaption,
            UserPostCaption = names.UserPostCaption,
            RegDateCaption = names.RegDateCaption,
            GroupHeadCaption = names.GroupHeadCaption,
            GuestCaption = names.GuestCaption,
            GuestsCaption = names.GuestsCaption,
        };
        return schemaItem;
    }
}