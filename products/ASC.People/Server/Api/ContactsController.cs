namespace ASC.People.Api;

public class ContactsController : ApiControllerBase
{
    private readonly ContactsControllerEngine _contactsControllerEngine;

    public ContactsController(ContactsControllerEngine contactsControllerEngine)
    {
        _contactsControllerEngine = contactsControllerEngine;
    }

    [Delete("{userid}/contacts")]
    public EmployeeFullDto DeleteMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return _contactsControllerEngine.DeleteMemberContacts(userid, memberModel);
    }

    [Delete("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto DeleteMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return _contactsControllerEngine.DeleteMemberContacts(userid, memberModel);
    }

    [Create("{userid}/contacts")]
    public EmployeeFullDto SetMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return _contactsControllerEngine.SetMemberContacts(userid, memberModel);
    }

    [Create("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto SetMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return _contactsControllerEngine.SetMemberContacts(userid, memberModel);
    }

    [Update("{userid}/contacts")]
    public EmployeeFullDto UpdateMemberContactsFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return _contactsControllerEngine.UpdateMemberContacts(userid, memberModel);
    }

    [Update("{userid}/contacts")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberContactsFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return _contactsControllerEngine.UpdateMemberContacts(userid, memberModel);
    }
}
