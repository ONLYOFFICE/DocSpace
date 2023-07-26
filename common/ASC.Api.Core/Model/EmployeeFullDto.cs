// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Api.Models;

public class EmployeeFullDto : EmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<Contact> Contacts { get; set; }
    public ApiDateTime Birthday { get; set; }
    public string Sex { get; set; }
    public EmployeeStatus Status { get; set; }
    public EmployeeActivationStatus ActivationStatus { get; set; }
    public ApiDateTime Terminated { get; set; }
    public string Department { get; set; }
    public ApiDateTime WorkFrom { get; set; }
    public List<GroupSummaryDto> Groups { get; set; }
    public string Location { get; set; }
    public string Notes { get; set; }
    public string AvatarMax { get; set; }
    public string AvatarMedium { get; set; }
    public string Avatar { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsRoomAdmin { get; set; }
    public bool IsLDAP { get; set; }
    public List<string> ListAdminModules { get; set; }
    public bool IsOwner { get; set; }
    public bool IsVisitor { get; set; }
    public bool IsCollaborator { get; set; }
    public string CultureName { get; set; }
    public string MobilePhone { get; set; }
    public MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }
    public bool IsSSO { get; set; }
    public DarkThemeSettingsEnum? Theme { get; set; }
    public long QuotaLimit { get; set; }
    public double UsedSpace { get; set; }

    public static new EmployeeFullDto GetSample()
    {
        return new EmployeeFullDto
        {
            Avatar = "url to big avatar",
            AvatarSmall = "url to small avatar",
            AvatarMax = "url to max avatar",
            Contacts = new List<Contact> { Contact.GetSample() },
            Email = "my@gmail.com",
            FirstName = "Mike",
            Id = Guid.Empty,
            IsAdmin = false,
            ListAdminModules = new List<string> { "projects", "crm" },
            UserName = "Mike.Zanyatski",
            LastName = "Zanyatski",
            Title = "Manager",
            Groups = new List<GroupSummaryDto> { GroupSummaryDto.GetSample() },
            AvatarMedium = "url to medium avatar",
            Birthday = ApiDateTime.GetSample(),
            Department = "Marketing",
            Location = "Palo Alto",
            Notes = "Notes to worker",
            Sex = "male",
            Status = EmployeeStatus.Active,
            WorkFrom = ApiDateTime.GetSample(),
            Terminated = ApiDateTime.GetSample(),
            CultureName = "en-EN",
            IsLDAP = false,
            IsSSO = false
        };
    }
}

[Scope]
public class EmployeeFullDtoHelper : EmployeeDtoHelper
{
    private readonly ApiContext _context;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly ApiDateTimeHelper _apiDateTimeHelper;
    private readonly WebItemManager _webItemManager;
    private readonly SettingsManager _settingsManager;
    private readonly IQuotaService _quotaService;

    public EmployeeFullDtoHelper(
        ApiContext context,
        UserManager userManager,
        UserPhotoManager userPhotoManager,
        WebItemSecurity webItemSecurity,
        CommonLinkUtility commonLinkUtility,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        ApiDateTimeHelper apiDateTimeHelper,
        WebItemManager webItemManager,
        SettingsManager settingsManager,
        IQuotaService quotaService,
        ILogger<EmployeeDtoHelper> logger)
    : base(context, displayUserSettingsHelper, userPhotoManager, commonLinkUtility, userManager, logger)
    {
        _context = context;
        _webItemSecurity = webItemSecurity;
        _apiDateTimeHelper = apiDateTimeHelper;
        _webItemManager = webItemManager;
        _settingsManager = settingsManager;
        _quotaService = quotaService;
    }

    public static Expression<Func<User, UserInfo>> GetExpression(ApiContext apiContext)
    {
        if (apiContext?.Fields == null)
        {
            return null;
        }

        var newExpr = Expression.New(typeof(UserInfo));

        //i => new UserInfo { ID = i.id } 
        var parameter = Expression.Parameter(typeof(User), "i");
        var bindExprs = new List<MemberAssignment>();

        //foreach (var field in apiContext.Fields)
        //{
        //    var userInfoProp = typeof(UserInfo).GetProperty(field);
        //    var userProp = typeof(User).GetProperty(field);
        //    if (userInfoProp != null && userProp != null)
        //    {
        //        bindExprs.Add(Expression.Bind(userInfoProp, Expression.Property(parameter, userProp)));
        //    }
        //}

        if (apiContext.Check("Id"))
        {
            bindExprs.Add(Expression.Bind(typeof(UserInfo).GetProperty("Id"),
                Expression.Property(parameter, typeof(User).GetProperty("Id"))));
        }

        var body = Expression.MemberInit(newExpr, bindExprs);
        var lambda = Expression.Lambda<Func<User, UserInfo>>(body, parameter);

        return lambda;
    }
    public async Task<EmployeeFullDto> GetSimple(UserInfo userInfo)
    {
        var result = new EmployeeFullDto
        {
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
        };

        await FillGroupsAsync(result, userInfo);

        var photoData = await _userPhotoManager.GetUserPhotoData(userInfo.Id, UserPhotoManager.BigFotoSize);

        if (photoData != null)
        {
            result.Avatar = "data:image/png;base64," + Convert.ToBase64String(photoData);
        }

        result.HasAvatar = await _userPhotoManager.UserHasAvatar(userInfo.Id);

        return result;
    }

    public async Task<EmployeeFullDto> GetFullAsync(UserInfo userInfo)
    {
        var currentType = await _userManager.GetUserTypeAsync(userInfo.Id);

        var result = new EmployeeFullDto
        {
            UserName = userInfo.UserName,
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            Birthday = _apiDateTimeHelper.Get(userInfo.BirthDate),
            Status = userInfo.Status,
            ActivationStatus = userInfo.ActivationStatus & ~EmployeeActivationStatus.AutoGenerated,
            Terminated = _apiDateTimeHelper.Get(userInfo.TerminatedDate),
            WorkFrom = _apiDateTimeHelper.Get(userInfo.WorkFromDate),
            Email = userInfo.Email,
            IsVisitor = await _userManager.IsUserAsync(userInfo),
            IsAdmin = currentType is EmployeeType.DocSpaceAdmin,
            IsRoomAdmin = currentType is EmployeeType.RoomAdmin,
            IsOwner = userInfo.IsOwner(_context.Tenant),
            IsCollaborator = currentType is EmployeeType.Collaborator,
            IsLDAP = userInfo.IsLDAP(),
            IsSSO = userInfo.IsSSO()
        };

        await InitAsync(result, userInfo);

        var quotaSettings = await _settingsManager.LoadAsync<TenantUserQuotaSettings>();

        if (quotaSettings.EnableUserQuota)
        {
            result.UsedSpace = Math.Max(0, (await _quotaService.FindUserQuotaRowsAsync(_context.Tenant.Id, userInfo.Id)).Where(r => !string.IsNullOrEmpty(r.Tag)).Sum(r => r.Counter));
            var userQuotaSettings = await _settingsManager.LoadAsync<UserQuotaSettings>(userInfo);
            result.QuotaLimit = userQuotaSettings != null ? userQuotaSettings.UserQuota : quotaSettings.DefaultUserQuota;
        }

        if (userInfo.Sex.HasValue)
        {
            result.Sex = userInfo.Sex.Value ? "male" : "female";
        }

        if (!string.IsNullOrEmpty(userInfo.Location))
        {
            result.Location = userInfo.Location;
        }

        if (!string.IsNullOrEmpty(userInfo.Notes))
        {
            result.Notes = userInfo.Notes;
        }

        if (!string.IsNullOrEmpty(userInfo.MobilePhone))
        {
            result.MobilePhone = userInfo.MobilePhone;
        }

        result.MobilePhoneActivationStatus = userInfo.MobilePhoneActivationStatus;

        if (!string.IsNullOrEmpty(userInfo.CultureName))
        {
            result.CultureName = userInfo.CultureName;
        }

        FillConacts(result, userInfo);
        await FillGroupsAsync(result, userInfo);

        var cacheKey = Math.Abs(userInfo.LastModified.GetHashCode());


        if (_context.Check("avatarMax"))
        {
            result.AvatarMax = await _userPhotoManager.GetMaxPhotoURL(userInfo.Id) + $"?hash={cacheKey}";
        }

        if (_context.Check("avatarMedium"))
        {
            result.AvatarMedium = await _userPhotoManager.GetMediumPhotoURL(userInfo.Id) + $"?hash={cacheKey}";
        }

        if (_context.Check("avatar"))
        {
            result.Avatar = await _userPhotoManager.GetBigPhotoURL(userInfo.Id) + $"?hash={cacheKey}";
        }

        if (_context.Check("listAdminModules"))
        {
            var listAdminModules = await userInfo.GetListAdminModulesAsync(_webItemSecurity, _webItemManager);
            if (listAdminModules.Count > 0)
            {
                result.ListAdminModules = listAdminModules;
            }
        }

        return result;
    }
    private async Task FillGroupsAsync(EmployeeFullDto result, UserInfo userInfo)
    {
        if (!_context.Check("groups") && !_context.Check("department"))
        {
            return;
        }

        var groups = (await _userManager.GetUserGroupsAsync(userInfo.Id))
            .Select(x => new GroupSummaryDto(x, _userManager))
            .ToList();

        if (groups.Count > 0)
        {
            result.Groups = groups;
            result.Department = string.Join(", ", result.Groups.Select(d => d.Name.HtmlEncode()));
        }
        else
        {
            result.Department = "";
        }
    }

    private void FillConacts(EmployeeFullDto employeeWraperFull, UserInfo userInfo)
    {
        if (userInfo.ContactsList == null)
        {
            return;
        }

        var contacts = new List<Contact>();

        for (var i = 0; i < userInfo.ContactsList.Count; i += 2)
        {
            if (i + 1 < userInfo.ContactsList.Count)
            {
                contacts.Add(new Contact(userInfo.ContactsList[i], userInfo.ContactsList[i + 1]));
            }
        }

        if (contacts.Count > 0)
        {
            employeeWraperFull.Contacts = contacts;
        }
    }
}