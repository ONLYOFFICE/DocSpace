namespace ASC.Web.Core.Utility
{
    [Serializable]
    public class ColorThemesSettings : ISettings
    {
        public const string ThemeFolderTemplate = "<theme_folder>";
        private const string DefaultName = "pure-orange";


        public string ColorThemeName { get; set; }

        public bool FirstRequest { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new ColorThemesSettings
            {
                ColorThemeName = DefaultName,
                FirstRequest = true
            };
        }

        public Guid ID
        {
            get { return new Guid("{AB5B3C97-A972-475C-BB13-71936186C4E6}"); }
        }
    }

    [Scope]
    public class ColorThemesSettingsHelper
    {
        private SettingsManager SettingsManager { get; }
        public IHostEnvironment HostEnvironment { get; }

        public ColorThemesSettingsHelper(
            SettingsManager settingsManager,
            IHostEnvironment hostEnvironment)
        {
            SettingsManager = settingsManager;
            HostEnvironment = hostEnvironment;
        }

        public string GetThemeFolderName(IUrlHelper urlHelper, string path)
        {
            var folderName = GetColorThemesSettings();
            var resolvedPath = path.ToLower().Replace(ColorThemesSettings.ThemeFolderTemplate, folderName);

            //TODO check
            if (!urlHelper.IsLocalUrl(resolvedPath))
                resolvedPath = urlHelper.Action(resolvedPath);

            try
            {
                var filePath = CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, resolvedPath);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("", path);
            }
            catch (Exception)
            {
                resolvedPath = path.ToLower().Replace(ColorThemesSettings.ThemeFolderTemplate, "default");

                if (!urlHelper.IsLocalUrl(resolvedPath))
                    resolvedPath = urlHelper.Action(resolvedPath);

                var filePath = CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, resolvedPath);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("", path);
            }

            return resolvedPath;
        }

        public string GetColorThemesSettings()
        {
            var colorTheme = SettingsManager.Load<ColorThemesSettings>();
            var colorThemeName = colorTheme.ColorThemeName;

            if (colorTheme.FirstRequest)
            {
                colorTheme.FirstRequest = false;
                SettingsManager.Save(colorTheme);
            }

            return colorThemeName;
        }

        public void SaveColorTheme(string theme)
        {
            var settings = new ColorThemesSettings { ColorThemeName = theme, FirstRequest = false };
            var path = "/skins/" + ColorThemesSettings.ThemeFolderTemplate;
            var resolvedPath = path.ToLower().Replace(ColorThemesSettings.ThemeFolderTemplate, theme);

            try
            {
                var filePath = CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, resolvedPath);
                if (Directory.Exists(filePath))
                {
                    SettingsManager.Save(settings);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}