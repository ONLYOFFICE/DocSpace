namespace ASC.People
{
    public class PeopleProduct : Product
    {
        internal const string ProductPath = "/products/people/";

        private ProductContext _context;

        public static Guid ID
        {
            get { return new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}"); }
        }

        public override bool Visible { get { return true; } }

        public override ProductContext Context
        {
            get { return _context; }
        }

        public override string Name
        {
            get { return PeopleResource.ProductName; }
        }

        public override string Description
        {
            get { return PeopleResource.ProductDescription; }
        }

        public override string ExtendedDescription
        {
            get { return PeopleResource.ProductDescription; }
        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string StartURL
        {
            get { return ProductPath; }
        }

        public override string HelpURL
        {
            get { return string.Concat(ProductPath, "help.aspx"); }
        }

        public override string ProductClassName
        {
            get { return "people"; }
        }

        public override string ApiURL
        {
            get
            {
                return "api/2.0/people/info.json";
            }
        }

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
}
