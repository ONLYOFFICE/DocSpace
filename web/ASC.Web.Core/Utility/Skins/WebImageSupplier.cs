namespace ASC.Web.Core.Utility.Skins
{
    [Scope]
    public class WebImageSupplier
    {
        private string FolderName { get; }
        private WebItemManager WebItemManager { get; }
        private WebPath WebPath { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public WebImageSupplier(WebItemManager webItemManager, WebPath webPath, IConfiguration configuration)
        {
            WebItemManager = webItemManager;
            WebPath = webPath;
            FolderName = configuration["web:images"];
        }
        public WebImageSupplier(WebItemManager webItemManager, WebPath webPath, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : this(webItemManager, webPath, configuration)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public string GetAbsoluteWebPath(string imgFileName)
        {
            return GetAbsoluteWebPath(imgFileName, Guid.Empty);
        }

        public string GetAbsoluteWebPath(string imgFileName, Guid moduleID)
        {
            return GetImageAbsoluteWebPath(imgFileName, moduleID);
        }

        public string GetImageFolderAbsoluteWebPath()
        {
            return GetImageFolderAbsoluteWebPath(Guid.Empty);
        }

        public string GetImageFolderAbsoluteWebPath(Guid moduleID)
        {
            if (HttpContextAccessor?.HttpContext == null) return string.Empty;

            var currentThemePath = GetPartImageFolderRel(moduleID);
            return WebPath.GetPathAsync(currentThemePath).Result;
        }

        private string GetImageAbsoluteWebPath(string fileName, Guid partID)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            var filepath = GetPartImageFolderRel(partID) + "/" + fileName;
            return WebPath.GetPathAsync(filepath).Result;
        }

        private string GetPartImageFolderRel(Guid partID)
        {
            var folderName = FolderName;
            string itemFolder = null;
            if (!Guid.Empty.Equals(partID))
            {
                var product = WebItemManager[partID];
                if (product != null && product.Context != null)
                {
                    itemFolder = GetAppThemeVirtualPath(product) + "/default/images";
                }

                folderName = itemFolder ?? folderName;
            }
            return folderName.TrimStart('~');
        }

        private static string GetAppThemeVirtualPath(IWebItem webitem)
        {
            if (webitem == null || string.IsNullOrEmpty(webitem.StartURL))
            {
                return string.Empty;
            }

            var dir = webitem.StartURL.Contains('.') ?
                          webitem.StartURL.Substring(0, webitem.StartURL.LastIndexOf('/')) :
                          webitem.StartURL.TrimEnd('/');
            return dir + "/App_Themes";
        }
    }
}