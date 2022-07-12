// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Core.Utility;

[Serializable]
public class ColorThemesSettings : ISettings<ColorThemesSettings>
{
    public const string ThemeFolderTemplate = "<theme_folder>";
    private const string DefaultName = "pure-orange";


    public string ColorThemeName { get; set; }

    public bool FirstRequest { get; set; }

    public ColorThemesSettings GetDefault()
    {
        return new ColorThemesSettings
        {
            ColorThemeName = DefaultName,
            FirstRequest = true
        };
    }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{AB5B3C97-A972-475C-BB13-71936186C4E6}"); }
    }
}

[Scope]
public class ColorThemesSettingsHelper
{
    private readonly SettingsManager _settingsManager;
    private readonly IHostEnvironment _hostEnvironment;

    public ColorThemesSettingsHelper(
        SettingsManager settingsManager,
        IHostEnvironment hostEnvironment)
    {
        _settingsManager = settingsManager;
        _hostEnvironment = hostEnvironment;
    }

    public string GetThemeFolderName(IUrlHelper urlHelper, string path)
    {
        var folderName = GetColorThemesSettings();
        var resolvedPath = path.ToLower().Replace(ColorThemesSettings.ThemeFolderTemplate, folderName);

        //TODO check
        if (!urlHelper.IsLocalUrl(resolvedPath))
        {
            resolvedPath = urlHelper.Action(resolvedPath);
        }

        try
        {
            var filePath = CrossPlatform.PathCombine(_hostEnvironment.ContentRootPath, resolvedPath);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("", path);
            }
        }
        catch (Exception)
        {
            resolvedPath = path.ToLower().Replace(ColorThemesSettings.ThemeFolderTemplate, "default");

            if (!urlHelper.IsLocalUrl(resolvedPath))
            {
                resolvedPath = urlHelper.Action(resolvedPath);
            }

            var filePath = CrossPlatform.PathCombine(_hostEnvironment.ContentRootPath, resolvedPath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("", path);
            }
        }

        return resolvedPath;
    }

    public string GetColorThemesSettings()
    {
        var colorTheme = _settingsManager.Load<ColorThemesSettings>();
        var colorThemeName = colorTheme.ColorThemeName;

        if (colorTheme.FirstRequest)
        {
            colorTheme.FirstRequest = false;
            _settingsManager.Save(colorTheme);
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
            var filePath = CrossPlatform.PathCombine(_hostEnvironment.ContentRootPath, resolvedPath);
            if (Directory.Exists(filePath))
            {
                _settingsManager.Save(settings);
            }
        }
        catch (Exception)
        {

        }
    }
}
