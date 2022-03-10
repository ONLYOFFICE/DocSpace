namespace ASC.People.Api;

public class ContactsController : PeopleControllerBase
{
    private readonly EmployeeFullDtoHelper _employeeFullDtoHelper;

    public ContactsController(
        UserManager userManager,
        PermissionContext permissionContext,
        ApiContext apiContext,
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory,
        EmployeeFullDtoHelper employeeFullDtoHelper) 
        : base(userManager, permissionContext, apiContext, userPhotoManager, httpClientFactory)
    {
        _employeeFullDtoHelper = employeeFullDtoHelper;
    }

    [Delete("{userid}/contacts")]
    public EmployeeFullDto DeleteMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto inDto)
    {
        return DeleteMemberContacts(userid, inDto);
    }

    [Delete("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto DeleteMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto inDto)
    {
        return DeleteMemberContacts(userid, inDto);
    }

    [Create("{userid}/contacts")]
    public EmployeeFullDto SetMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto inDto)
    {
        return SetMemberContacts(userid, inDto);
    }

    [Create("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto SetMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto inDto)
    {
        return SetMemberContacts(userid, inDto);
    }

    [Update("{userid}/contacts")]
    public EmployeeFullDto UpdateMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto inDto)
    {
        return UpdateMemberContacts(userid, inDto);
    }

    [Update("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto inDto)
    {
        return UpdateMemberContacts(userid, inDto);
    }

    private EmployeeFullDto DeleteMemberContacts(string userid, UpdateMemberRequestDto inDto)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        DeleteContacts(inDto.Contacts, user);
        _userManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    private EmployeeFullDto SetMemberContacts(string userid, UpdateMemberRequestDto inDto)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        user.ContactsList.Clear();
        UpdateContacts(inDto.Contacts, user);
        _userManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    private EmployeeFullDto UpdateMemberContacts(string userid, UpdateMemberRequestDto inDto)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        UpdateContacts(inDto.Contacts, user);
        _userManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    private void DeleteContacts(IEnumerable<Contact> contacts, UserInfo user)
    {   
        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        if (contacts == null)
        {
            return;
        }

        if (user.ContactsList == null)
        {
            user.ContactsList = new List<string>();
        }

        foreach (var contact in contacts)
        {
            var index = user.ContactsList.IndexOf(contact.Type);
            if (index != -1)
            {
                //Remove existing
                user.ContactsList.RemoveRange(index, 2);
            }
        }
    }
}
