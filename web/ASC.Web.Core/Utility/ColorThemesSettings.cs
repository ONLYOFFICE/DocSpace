/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.IO;

using ASC.Common;
using ASC.Core.Common.Settings;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

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
                var filePath = Path.Combine(HostEnvironment.ContentRootPath, resolvedPath);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("", path);
            }
            catch (Exception)
            {
                resolvedPath = path.ToLower().Replace(ColorThemesSettings.ThemeFolderTemplate, "default");

                if (!urlHelper.IsLocalUrl(resolvedPath))
                    resolvedPath = urlHelper.Action(resolvedPath);

                var filePath = Path.Combine(HostEnvironment.ContentRootPath, resolvedPath);

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
                var filePath = Path.Combine(HostEnvironment.ContentRootPath, resolvedPath);
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