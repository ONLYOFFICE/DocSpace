using Action = ASC.Common.Security.Authorizing.Action;
using AuthConst = ASC.Common.Security.Authorizing.Constants;

namespace ASC.Core.Users;

[Singletone]
public sealed class Constants
{
    public Constants(IConfiguration configuration)
    {
        _configuration = configuration;
        NamingPoster = new UserInfo
        {
            Id = new Guid("{17097D73-2D1E-4B36-AA07-AEB34AF993CD}"),
            FirstName = configuration["core:system:poster:name"] ?? "ONLYOFFICE Poster",
            LastName = string.Empty,
            ActivationStatus = EmployeeActivationStatus.Activated
        };
    }

    public int MaxEveryoneCount
    {
        get
        {
            if (!int.TryParse(_configuration["core:users"], out var count))
            {
                count = 10000;
            }

            return count;
        }
    }

    public int CoefficientOfVisitors
    {
        get
        {
            int count;
            if (!int.TryParse(_configuration["core:coefficient-of-visitors"], out count))
            {
                count = 2;
            }

            return count;
        }
    }

    private readonly IConfiguration _configuration;


    #region system group and category groups

    public static readonly Guid SysGroupCategoryId = new Guid("{7717039D-FBE9-45ad-81C1-68A1AA10CE1F}");

    public static readonly GroupInfo GroupEveryone = new GroupInfo(SysGroupCategoryId)
    {
        ID = AuthConst.Everyone.ID,
        Name = AuthConst.Everyone.Name,
    };

    public static readonly GroupInfo GroupVisitor = new GroupInfo(SysGroupCategoryId)
    {
        ID = AuthConst.Visitor.ID,
        Name = AuthConst.Visitor.Name,
    };

    public static readonly GroupInfo GroupUser = new GroupInfo(SysGroupCategoryId)
    {
        ID = AuthConst.User.ID,
        Name = AuthConst.User.Name,
    };

    public static readonly GroupInfo GroupAdmin = new GroupInfo(SysGroupCategoryId)
    {
        ID = AuthConst.Admin.ID,
        Name = AuthConst.Admin.Name,
    };

    public static readonly GroupInfo[] BuildinGroups = new[]
    {
            GroupEveryone,
            GroupVisitor,
            GroupUser,
            GroupAdmin,
        };

    public static readonly UserInfo LostUser = new UserInfo
    {
        Id = new Guid("{4A515A15-D4D6-4b8e-828E-E0586F18F3A3}"),
        FirstName = "Unknown",
        LastName = "Unknown",
        ActivationStatus = EmployeeActivationStatus.NotActivated
    };

    public static readonly UserInfo OutsideUser = new UserInfo
    {
        Id = new Guid("{E78F4C20-2F3B-4A9D-AD13-5F298BD5A3BA}"),
        FirstName = "Outside",
        LastName = "Outside",
        ActivationStatus = EmployeeActivationStatus.Activated
    };

    public UserInfo NamingPoster { get; }

    public static readonly GroupInfo LostGroupInfo = new GroupInfo
    {
        ID = new Guid("{74B9CBD1-2412-4e79-9F36-7163583E9D3A}"),
        Name = "Unknown"
    };

    #endregion


    #region authorization rules module to work with users

    public static readonly Action Action_EditUser = new Action(
        new Guid("{EF5E6790-F346-4b6e-B662-722BC28CB0DB}"),
        "Edit user information");

    public static readonly Action Action_AddRemoveUser = new Action(
        new Guid("{D5729C6F-726F-457e-995F-DB0AF58EEE69}"),
        "Add/Remove user");

    public static readonly Action Action_EditGroups = new Action(
        new Guid("{1D4FEEAC-0BF3-4aa9-B096-6D6B104B79B5}"),
        "Edit categories and groups");

    #endregion
}
