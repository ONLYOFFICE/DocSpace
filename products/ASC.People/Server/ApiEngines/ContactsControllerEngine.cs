using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.ApiHelpers;

public class ContactsControllerEngine : PeopleControllerEngine
{
    private readonly EmployeeFullDtoHelper _employeeFullDtoHelper;

    public ContactsControllerEngine(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        SetupInfo setupInfo,
        EmployeeFullDtoHelper employeeFullDtoHelper)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService,
            userPhotoManager,
            httpClientFactory,
            displayUserSettingsHelper,
            setupInfo)
    {
        _employeeFullDtoHelper = employeeFullDtoHelper;
    }

    public void DeleteContacts(IEnumerable<Contact> contacts, UserInfo user)
    {
        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);
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

    public EmployeeFullDto DeleteMemberContacts(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        DeleteContacts(memberModel.Contacts, user);
        UserManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    public EmployeeFullDto SetMemberContacts(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        user.ContactsList.Clear();
        UpdateContacts(memberModel.Contacts, user);
        UserManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }

    public EmployeeFullDto UpdateMemberContacts(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        UpdateContacts(memberModel.Contacts, user);
        UserManager.SaveUserInfo(user);

        return _employeeFullDtoHelper.GetFull(user);
    }
}