namespace ASC.People.Api;

public abstract class PeopleControllerBase : ApiControllerBase
{
    protected readonly UserManager _userManager;
    protected readonly PermissionContext _permissionContext;
    protected readonly ApiContext _apiContext;
    protected readonly UserPhotoManager _userPhotoManager;
    protected readonly IHttpClientFactory _httpClientFactory;

    public PeopleControllerBase(
        UserManager userManager,
        PermissionContext permissionContext,
        ApiContext apiContext,
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _permissionContext = permissionContext;
        _apiContext = apiContext;
        _userPhotoManager = userPhotoManager;
        _httpClientFactory = httpClientFactory;
    }

    protected UserInfo GetUserInfo(string userNameOrId)
    {
        UserInfo user;
        try
        {
            var userId = new Guid(userNameOrId);
            user = _userManager.GetUsers(userId);
        }
        catch (FormatException)
        {
            user = _userManager.GetUserByUserName(userNameOrId);
        }

        if (user == null || user.Id == Constants.LostUser.Id)
        {
            throw new ItemNotFoundException("user not found");
        }

        return user;
    }

    protected void UpdateContacts(IEnumerable<Contact> contacts, UserInfo user)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

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

        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        if (!files.StartsWith("http://") && !files.StartsWith("https://"))
        {
            files = new Uri(_apiContext.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Authority) + "/" + files.TrimStart('/');
        }

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(files);

        var httpClient = _httpClientFactory.CreateClient();
        using var response = httpClient.Send(request);
        using var inputStream = response.Content.ReadAsStream();
        using var br = new BinaryReader(inputStream);
        var imageByteArray = br.ReadBytes((int)inputStream.Length);

        _userPhotoManager.SaveOrUpdatePhoto(user.Id, imageByteArray);
    }
}