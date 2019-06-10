using System;
using System.Collections.Generic;
using ASC.Web.Core;

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
            get { return "People"; }//PeopleResource.ProductName; }
        }

        public override string Description
        {
            get { return "People Description"; }//PeopleResource.ProductDescription; }
        }

        public override string ExtendedDescription
        {
            get { return "People ExtendedDescription"; }//PeopleResource.ProductDescription; }
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

        public override void Init()
        {
            _context = new ProductContext
            {
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "product_logo.png",
                LargeIconFileName = "images/people_logolarge.png",
                DefaultSortOrder = 50,
                AdminOpportunities = () => new List<string>(),//PeopleResource.ProductAdminOpportunities.Split('|').ToList(),
                UserOpportunities = () => new List<string>() //PeopleResource.ProductUserOpportunities.Split('|').ToList()
            };

            //SearchHandlerManager.Registry(new SearchHandler());
        }
    }
}
