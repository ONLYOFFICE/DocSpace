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
    public List<SchemaRequestsDto> PeopleSchemas()
    {
        return _customNamingPeople
                .GetSchemas()
                .Select(r =>
                {
                    var names = _customNamingPeople.GetPeopleNames(r.Key);

                    return new SchemaRequestsDto
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
    public SchemaRequestsDto SaveNamingSettings(SchemaRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _customNamingPeople.SetPeopleNames(inDto.Id);

        _tenantManager.SaveTenant(_tenantManager.GetCurrentTenant());

        _messageService.Send(MessageAction.TeamTemplateChanged);

        return PeopleSchema(inDto.Id);
    }

    [Update("customschemas")]
    public SchemaRequestsDto SaveCustomNamingSettings(SchemaRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var usrCaption = (inDto.UserCaption ?? "").Trim();
        var usrsCaption = (inDto.UsersCaption ?? "").Trim();
        var grpCaption = (inDto.GroupCaption ?? "").Trim();
        var grpsCaption = (inDto.GroupsCaption ?? "").Trim();
        var usrStatusCaption = (inDto.UserPostCaption ?? "").Trim();
        var regDateCaption = (inDto.RegDateCaption ?? "").Trim();
        var grpHeadCaption = (inDto.GroupHeadCaption ?? "").Trim();
        var guestCaption = (inDto.GuestCaption ?? "").Trim();
        var guestsCaption = (inDto.GuestsCaption ?? "").Trim();

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
    public SchemaRequestsDto PeopleSchema(string id)
    {
        var names = _customNamingPeople.GetPeopleNames(id);
        var schemaItem = new SchemaRequestsDto
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