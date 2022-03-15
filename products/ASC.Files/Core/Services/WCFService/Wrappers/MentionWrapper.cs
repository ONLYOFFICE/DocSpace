namespace ASC.Web.Files.Services.WCFService;

public class MentionWrapper
{
    public UserInfo User { get; set; }
    public string Email => User.Email;
    public string Id => User.Id.ToString();
    public bool HasAccess { get; set; }
    public string Name => User.DisplayUserName(false, _displayUserSettingsHelper);

    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;

    public MentionWrapper(UserInfo user, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        User = user;
        _displayUserSettingsHelper = displayUserSettingsHelper;
    }
}

public class MentionMessageWrapper
{
    public ActionLinkConfig ActionLink { get; set; }
    public List<string> Emails { get; set; }
    public string Message { get; set; }
}
