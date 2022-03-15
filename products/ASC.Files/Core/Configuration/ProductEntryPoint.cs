namespace ASC.Web.Files.Configuration;

[Scope]
public class ProductEntryPoint : Product
{
    internal const string ProductPath = "/products/files/";

    //public FilesSpaceUsageStatManager FilesSpaceUsageStatManager { get; }
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly NotifyConfiguration _notifyConfiguration;

    //public SubscriptionManager SubscriptionManager { get; }

    public ProductEntryPoint() { }

    public ProductEntryPoint(
        //            FilesSpaceUsageStatManager filesSpaceUsageStatManager,
        CoreBaseSettings coreBaseSettings,
        AuthContext authContext,
        UserManager userManager,
        NotifyConfiguration notifyConfiguration
        //            SubscriptionManager subscriptionManager
        )
    {
        //            FilesSpaceUsageStatManager = filesSpaceUsageStatManager;
        _coreBaseSettings = coreBaseSettings;
        _authContext = authContext;
        _userManager = userManager;
        _notifyConfiguration = notifyConfiguration;
        //SubscriptionManager = subscriptionManager;
    }

    public static readonly Guid ID = WebItemManager.DocumentsProductID;

    private ProductContext _productContext;

    public override bool Visible => true;
    public override bool IsPrimary => true;

    public override void Init()
    {
        List<string> adminOpportunities() => (_coreBaseSettings.CustomMode
                                                           ? CustomModeResource.ProductAdminOpportunitiesCustomMode
                                                           : FilesCommonResource.ProductAdminOpportunities).Split('|').ToList();

        List<string> userOpportunities() => (_coreBaseSettings.CustomMode
                                     ? CustomModeResource.ProductUserOpportunitiesCustomMode
                                     : FilesCommonResource.ProductUserOpportunities).Split('|').ToList();

        _productContext =
            new ProductContext
            {
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "images/files.menu.svg",
                LargeIconFileName = "images/files.svg",
                DefaultSortOrder = 10,
                    //SubscriptionManager = SubscriptionManager,
                    //SpaceUsageStatManager = FilesSpaceUsageStatManager,
                    AdminOpportunities = adminOpportunities,
                UserOpportunities = userOpportunities,
                CanNotBeDisabled = true,
            };

        if (_notifyConfiguration != null)
        {
            _notifyConfiguration.Configure();
        }
        //SearchHandlerManager.Registry(new SearchHandler());
    }

    public string GetModuleResource(string ResourceClassTypeName, string ResourseKey)
    {
        if (string.IsNullOrEmpty(ResourseKey))
        {
            return string.Empty;
        }

        try
        {
            return (string)Type.GetType(ResourceClassTypeName).GetProperty(ResourseKey, BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public override Guid ProductID => ID;
    public override string Name => FilesCommonResource.ProductName;

    public override string Description
    {
        get
        {
            var id = _authContext.CurrentAccount.ID;

            if (_userManager.IsUserInGroup(id, ASC.Core.Users.Constants.GroupVisitor.ID))
            {
                return FilesCommonResource.ProductDescriptionShort;
            }

            if (_userManager.IsUserInGroup(id, ASC.Core.Users.Constants.GroupAdmin.ID) || _userManager.IsUserInGroup(id, ID))
            {
                return FilesCommonResource.ProductDescriptionEx;
            }

            return FilesCommonResource.ProductDescription;
        }
    }

    public override string StartURL => ProductPath;
    public override string HelpURL => PathProvider.StartURL;
    public override string ProductClassName => "files";
    public override ProductContext Context => _productContext;
    public override string ApiURL => string.Empty;
}
