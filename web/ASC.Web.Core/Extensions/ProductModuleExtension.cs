namespace ASC.Web.Core
{
    public static class ProductModuleExtension
    {


        public static string GetSmallIconAbsoluteURL(this IModule module, WebImageSupplier webImageSupplier)
        {
            if (module == null || module.Context == null || string.IsNullOrEmpty(module.Context.SmallIconFileName))
                return "";

            return webImageSupplier.GetAbsoluteWebPath(module.Context.SmallIconFileName, module.ID);
        }

        public static string GetSmallIconAbsoluteURL(this IProduct product, WebImageSupplier webImageSupplier)
        {
            if (product == null || product.Context == null || string.IsNullOrEmpty(product.Context.SmallIconFileName))
                return "";

            return webImageSupplier.GetAbsoluteWebPath(product.Context.SmallIconFileName, product.ID);
        }

        public static string GetIconAbsoluteURL(this IModule module, WebImageSupplier webImageSupplier)
        {
            if (module == null || module.Context == null || string.IsNullOrEmpty(module.Context.IconFileName))
                return "";

            return webImageSupplier.GetAbsoluteWebPath(module.Context.IconFileName, module.ID);
        }

        public static string GetIconAbsoluteURL(this IProduct product, WebImageSupplier webImageSupplier)
        {
            if (product == null || product.Context == null || string.IsNullOrEmpty(product.Context.IconFileName))
                return "";

            return webImageSupplier.GetAbsoluteWebPath(product.Context.IconFileName, product.ID);
        }
    }
}
