using ASC.Web.Core;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

namespace ASC.Files
{
    public class FilesProduct : Product
    {
        internal const string ProductPath = "/products/files/";

        private ProductContext _context;

        public static Guid ID
        {
            get { return new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}"); }
        }

        public override bool Visible { get { return true; } }

        public override ProductContext Context
        {
            get { return _context; }
        }

        public override string Name
        {
            get { return "Documents"; }
        }

        public override string Description
        {
            get { return "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed."; }
        }

        public override string ExtendedDescription
        {
            get { return ""; }
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
                return "api/2.0/fileStorageService/info.json";
            }
        }

        public override void Init()
        {
            _context = new ProductContext
            {
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "product_logo.png",
                LargeIconFileName = "images/files.svg",
                DefaultSortOrder = 50,
                //AdminOpportunities = () => PeopleResource.ProductAdminOpportunities.Split('|').ToList(),
                //UserOpportunities = () => PeopleResource.ProductUserOpportunities.Split('|').ToList()
            };

            //SearchHandlerManager.Registry(new SearchHandler());
        }
    }
}
