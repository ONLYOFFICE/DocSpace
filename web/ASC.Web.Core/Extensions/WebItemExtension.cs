namespace ASC.Web.Core
{
    public static class WebItemExtension
    {
        public static string GetSysName(this IWebItem webitem)
        {
            if (string.IsNullOrEmpty(webitem.StartURL)) return string.Empty;

            var sysname = string.Empty;
            var parts = webitem.StartURL.ToLower().Split('/', '\\').ToList();

            var index = parts.FindIndex(s => "products".Equals(s));
            if (0 <= index && index < parts.Count - 1)
            {
                sysname = parts[index + 1];
                index = parts.FindIndex(s => "modules".Equals(s));
                if (0 <= index && index < parts.Count - 1)
                {
                    sysname += "-" + parts[index + 1];
                }
                else if (index == parts.Count - 1)
                {
                    sysname = parts[index].Split('.')[0];
                }
                return sysname;
            }

            index = parts.FindIndex(s => "addons".Equals(s));
            if (0 <= index && index < parts.Count - 1)
            {
                sysname = parts[index + 1];
            }

            return sysname;
        }

        public static string GetDisabledIconAbsoluteURL(this IWebItem item, WebImageSupplier webImageSupplier)
        {
            if (item == null || item.Context == null || string.IsNullOrEmpty(item.Context.DisabledIconFileName)) return string.Empty;
            return webImageSupplier.GetAbsoluteWebPath(item.Context.DisabledIconFileName, item.ID);
        }

        public static string GetSmallIconAbsoluteURL(this IWebItem item, WebImageSupplier webImageSupplier)
        {
            if (item == null || item.Context == null || string.IsNullOrEmpty(item.Context.SmallIconFileName)) return string.Empty;
            return webImageSupplier.GetAbsoluteWebPath(item.Context.SmallIconFileName, item.ID);
        }

        public static string GetIconAbsoluteURL(this IWebItem item, WebImageSupplier webImageSupplier)
        {
            if (item == null || item.Context == null || string.IsNullOrEmpty(item.Context.IconFileName)) return string.Empty;
            return webImageSupplier.GetAbsoluteWebPath(item.Context.IconFileName, item.ID);
        }

        public static string GetLargeIconAbsoluteURL(this IWebItem item, WebImageSupplier webImageSupplier)
        {
            if (item == null || item.Context == null || string.IsNullOrEmpty(item.Context.LargeIconFileName)) return string.Empty;
            return webImageSupplier.GetAbsoluteWebPath(item.Context.LargeIconFileName, item.ID);
        }

        public static List<string> GetUserOpportunities(this IWebItem item)
        {
            return item.Context.UserOpportunities != null ? item.Context.UserOpportunities() : new List<string>();
        }

        public static List<string> GetAdminOpportunities(this IWebItem item)
        {
            return item.Context.AdminOpportunities != null ? item.Context.AdminOpportunities() : new List<string>();
        }

        public static bool HasComplexHierarchyOfAccessRights(this IWebItem item)
        {
            return item.Context.HasComplexHierarchyOfAccessRights;
        }

        public static bool CanNotBeDisabled(this IWebItem item)
        {
            return item.Context.CanNotBeDisabled;
        }


        public static bool IsDisabled(this IWebItem item, WebItemSecurity webItemSecurity, AuthContext authContext)
        {
            return item.IsDisabled(authContext.CurrentAccount.ID, webItemSecurity);
        }

        public static bool IsDisabled(this IWebItem item, Guid userID, WebItemSecurity webItemSecurity)
        {
            return item != null && (!webItemSecurity.IsAvailableForUser(item.ID, userID) || !item.Visible);
        }

        public static bool IsSubItem(this IWebItem item)
        {
            return item is IModule && !(item is IProduct);
        }
    }
}
