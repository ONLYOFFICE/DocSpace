using System;

using ASC.Web.Core;

namespace ASC.Api.Core
{
    public class Module
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsPrimary { get; set; }

        public Module(Product product, bool isPrimary = false)
        {
            Id = product.ProductID;
            Title = product.Name;
            Description = product.Description;
            ImageUrl = product.Context.LargeIconFileName;
            Link = product.StartURL;
            IsPrimary = isPrimary;
        }
    }
}
