namespace ASC.Web.Core.Users;

[Serializable]
public class DisplayUserSettings : ISettings
{
    public Guid ID => new Guid("2EF59652-E1A7-4814-BF71-FEB990149428");

    public bool IsDisableGettingStarted { get; set; }

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new DisplayUserSettings
        {
            IsDisableGettingStarted = false,
        };
    }
}

[Scope]
public class DisplayUserSettingsHelper
{
    private readonly string _removedProfileName;
    public DisplayUserSettingsHelper(UserManager userManager, UserFormatter userFormatter, IConfiguration configuration)
    {
        _userManager = userManager;
        _userFormatter = userFormatter;
        _removedProfileName = configuration["web:removed-profile-name"] ?? "profile removed";
    }

    private readonly UserManager _userManager;
    private readonly UserFormatter _userFormatter;

    public string GetFullUserName(Guid userID, bool withHtmlEncode = true)
    {
        return GetFullUserName(_userManager.GetUsers(userID), withHtmlEncode);
    }

    public string GetFullUserName(UserInfo userInfo, bool withHtmlEncode = true)
    {
        return GetFullUserName(userInfo, DisplayUserNameFormat.Default, withHtmlEncode);
    }

    public string GetFullUserName(UserInfo userInfo, DisplayUserNameFormat format, bool withHtmlEncode)
    {
        if (userInfo == null)
        {
            return string.Empty;
        }
        if (!userInfo.Id.Equals(Guid.Empty) && !_userManager.UserExists(userInfo))
        {
            try
            {
                var resourceType = Type.GetType("ASC.Web.Core.PublicResources.Resource, ASC.Web.Core");
                var resourceProperty = resourceType.GetProperty("ProfileRemoved", BindingFlags.Static | BindingFlags.Public);
                var resourceValue = (string)resourceProperty.GetValue(null);

                return string.IsNullOrEmpty(resourceValue) ? _removedProfileName : resourceValue;
            }
            catch (Exception)
            {
                return _removedProfileName;
            }
        }
        var result = _userFormatter.GetUserName(userInfo, format);

        return withHtmlEncode ? HtmlEncode(result) : result;
    }
    public string HtmlEncode(string str)
    {
        return !string.IsNullOrEmpty(str) ? HttpUtility.HtmlEncode(str) : str;
    }
}
