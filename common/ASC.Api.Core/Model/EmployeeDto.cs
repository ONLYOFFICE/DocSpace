namespace ASC.Web.Api.Models;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Title { get; set; }
    public string AvatarSmall { get; set; }
    public string ProfileUrl { get; set; }

    public static EmployeeDto GetSample()
    {
        return new EmployeeDto
        {
            Id = Guid.Empty,
            DisplayName = "Mike Zanyatski",
            Title = "Manager",
            AvatarSmall = "url to small avatar",
        };
    }
}

[Scope]
public class EmployeeDtoHelper
{
    protected readonly UserPhotoManager UserPhotoManager;
    protected readonly UserManager UserManager;

    private readonly ApiContext _httpContext;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly CommonLinkUtility _commonLinkUtility;

    public EmployeeDtoHelper(
        ApiContext httpContext,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        UserPhotoManager userPhotoManager,
        CommonLinkUtility commonLinkUtility,
        UserManager userManager)
    {
        UserPhotoManager = userPhotoManager;
        UserManager = userManager;
        _httpContext = httpContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _commonLinkUtility = commonLinkUtility;
    }

    public EmployeeDto Get(UserInfo userInfo)
    {
        return Init(new EmployeeDto(), userInfo);
    }

    public EmployeeDto Get(Guid userId)
    {
        try
        {
            return Get(UserManager.GetUsers(userId));
        }
        catch (Exception)
        {
            return Get(Constants.LostUser);
        }
    }

    protected EmployeeDto Init(EmployeeDto result, UserInfo userInfo)
    {
        result.Id = userInfo.Id;
        result.DisplayName = _displayUserSettingsHelper.GetFullUserName(userInfo);

        if (!string.IsNullOrEmpty(userInfo.Title))
        {
            result.Title = userInfo.Title;
        }

        var userInfoLM = userInfo.LastModified.GetHashCode();

        if (_httpContext.Check("avatarSmall"))
        {
            result.AvatarSmall = UserPhotoManager.GetSmallPhotoURL(userInfo.Id, out var isdef) 
                + (isdef ? "" : $"?_={userInfoLM}");
        }     

        if (result.Id != Guid.Empty)
        {
            var profileUrl = _commonLinkUtility.GetUserProfile(userInfo, false);
            result.ProfileUrl = _commonLinkUtility.GetFullAbsolutePath(profileUrl);
        }

        return result;
    }
}