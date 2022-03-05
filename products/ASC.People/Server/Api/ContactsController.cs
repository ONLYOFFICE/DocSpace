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
    public EmployeeFullDto DeleteMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return DeleteMemberContacts(userid, memberModel);
    }

    [Delete("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto DeleteMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return DeleteMemberContacts(userid, memberModel);
    }

    [Create("{userid}/contacts")]
    public EmployeeFullDto SetMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return SetMemberContacts(userid, memberModel);
    }

    [Create("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto SetMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return SetMemberContacts(userid, memberModel);
    }

    [Update("{userid}/contacts")]
    public EmployeeFullDto UpdateMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return UpdateMemberContacts(userid, memberModel);
    }

    [Update("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return UpdateMemberContacts(userid, memberModel);
    }

    private EmployeeFullDto DeleteMemberContacts(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        DeleteContacts(memberModel.Contacts, user);
        _userManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    private EmployeeFullDto SetMemberContacts(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        user.ContactsList.Clear();
        UpdateContacts(memberModel.Contacts, user);
        _userManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    private EmployeeFullDto UpdateMemberContacts(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        UpdateContacts(memberModel.Contacts, user);
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
