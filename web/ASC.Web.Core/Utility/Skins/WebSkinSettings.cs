namespace ASC.Web.Core.Utility.Skins
{
    public class WebSkin
    {
        private static readonly HashSet<string> BaseCultureCss = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public static bool HasCurrentCultureCssFile
        {
            get { return BaseCultureCss.Contains(CultureInfo.CurrentCulture.Name); }
        }

        public WebSkin(IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var dir = CrossPlatform.PathCombine(webHostEnvironment.ContentRootPath, "~/skins/default/");
                if (!Directory.Exists(dir)) return;

                foreach (var f in Directory.GetFiles(dir, "common_style.*.css"))
                {
                    BaseCultureCss.Add(Path.GetFileName(f).Split('.')[1]);
                }
            }
            catch
            {
            }
        }
    }
}