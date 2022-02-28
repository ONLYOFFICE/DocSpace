using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class BasePeopleController : BaseApiController
{
    protected readonly UserPhotoManager UserPhotoManager;
    protected readonly DisplayUserSettingsHelper DisplayUserSettingsHelper;
    protected readonly SetupInfo SetupInfo;

    private readonly IHttpClientFactory _httpClientFactory;

    protected BasePeopleController(
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
        SetupInfo setupInfo)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService)
    {
        UserPhotoManager = userPhotoManager;
        _httpClientFactory = httpClientFactory;
        DisplayUserSettingsHelper = displayUserSettingsHelper;
        SetupInfo = setupInfo;
    }

    protected UserInfo GetUserInfo(string userNameOrId)
    {
        UserInfo user;
        try
        {
            var userId = new Guid(userNameOrId);
            user = UserManager.GetUsers(userId);
        }
        catch (FormatException)
        {
            user = UserManager.GetUserByUserName(userNameOrId);
        }

        if (user == null || user.ID == Constants.LostUser.ID)
        {
            throw new ItemNotFoundException("user not found");
        }

        return user;
    }

    protected void UpdateContacts(IEnumerable<Contact> contacts, UserInfo user)
    {
        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

        if (contacts == null)
        {
            return;
        }

        var values = contacts.Where(r => !string.IsNullOrEmpty(r.Value)).Select(r => $"{r.Type}|{r.Value}");
        user.Contacts = string.Join('|', values);
    }

    protected void UpdatePhotoUrl(string files, UserInfo user)
    {
        if (string.IsNullOrEmpty(files))
        {
            return;
        }

        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

        if (!files.StartsWith("http://") && !files.StartsWith("https://"))
        {
            files = new Uri(ApiContext.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Authority) + "/" + files.TrimStart('/');
        }
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(files);

        var httpClient = _httpClientFactory.CreateClient();
        using var response = httpClient.Send(request);
        using var inputStream = response.Content.ReadAsStream();
        using var br = new BinaryReader(inputStream);
        var imageByteArray = br.ReadBytes((int)inputStream.Length);
        UserPhotoManager.SaveOrUpdatePhoto(user.ID, imageByteArray);
    }
}
