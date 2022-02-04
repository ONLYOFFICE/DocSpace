namespace ASC.Api.Core
{
    public class Module
    {
        public Guid Id { get; set; }

        public string AppName { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }

        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }

        public string HelpUrl { get; set; }
        public string Description { get; set; }
        public bool IsPrimary { get; set; }

        public Module(Product product)
        {
            Id = product.ProductID;
            AppName = product.ProductClassName;
            Title = product.Name;
            Description = product.Description;
            IconUrl = product.Context.IconFileName;
            ImageUrl = product.Context.LargeIconFileName;
            Link = product.StartURL;
            IsPrimary = product.IsPrimary;
            HelpUrl = product.HelpURL;
        }
    }
}
