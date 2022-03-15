namespace ASC.Core.Users
{
    public static class StudioUserInfoExtension
    {
        public static string GetUserProfilePageURL(this UserInfo userInfo, CommonLinkUtility commonLinkUtility)
        {
            return userInfo == null ? "" : commonLinkUtility.GetUserProfile(userInfo);
        }

        public static List<string> GetListAdminModules(this UserInfo ui, WebItemSecurity webItemSecurity)
        {
            var products = webItemSecurity.WebItemManager.GetItemsAll().Where(i => i is IProduct || i.ID == WebItemManager.MailProductID);

            return (from product in products where webItemSecurity.IsProductAdministrator(product.ID, ui.Id) select product.ProductClassName).ToList();
        }
    }
}