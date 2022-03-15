namespace ASC.People;

public class PeopleProduct : Product
{
    internal const string ProductPath = "/products/people/";

    private ProductContext _context;
    public static Guid ID => new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}");
    public override bool Visible => true;
    public override ProductContext Context => _context;
    public override string Name => PeopleResource.ProductName;
    public override string Description => PeopleResource.ProductDescription;
    public override string ExtendedDescription => PeopleResource.ProductDescription;
    public override Guid ProductID => ID;
    public override string StartURL => ProductPath;
    public override string HelpURL => string.Concat(ProductPath, "help.aspx");
    public override string ProductClassName => "people";
    public override string ApiURL => "api/2.0/people/info.json";
    public override bool IsPrimary { get => false; }

    public override void Init()
    {
        _context = new ProductContext
        {
            DisabledIconFileName = "product_disabled_logo.png",
            IconFileName = "images/people.menu.svg",
            LargeIconFileName = "images/people.svg",
            DefaultSortOrder = 50,
            AdminOpportunities = () => PeopleResource.ProductAdminOpportunities.Split('|').ToList(),
            UserOpportunities = () => PeopleResource.ProductUserOpportunities.Split('|').ToList()
        };

        //SearchHandlerManager.Registry(new SearchHandler());
    }
}
