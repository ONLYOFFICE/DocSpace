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
using System.Collections.Generic;
using System.Linq;

using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Web.Core.Files;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files.Configuration
{
    public class SearchHandler
    {
        public Guid ProductID
        {
            get { return ProductEntryPoint.ID; }
        }

        public ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "common_search_icon.png" }; }
        }

        public Guid ModuleID
        {
            get { return ProductID; }
        }

        public string SearchName
        {
            get { return FilesCommonResource.Search; }
        }

        private FileSecurity FileSecurity { get; }
        private IDaoFactory DaoFactory { get; }
        private EntryManager EntryManager { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUtility FileUtility { get; }
        private PathProvider PathProvider { get; }
        private ThirdpartyConfiguration ThirdpartyConfiguration { get; }

        public SearchHandler(
            FileSecurity fileSecurity,
            IDaoFactory daoFactory,
            EntryManager entryManager,
            GlobalFolderHelper globalFolderHelper,
            FilesSettingsHelper filesSettingsHelper,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            PathProvider pathProvider,
            ThirdpartyConfiguration thirdpartyConfiguration)
        {
            FileSecurity = fileSecurity;
            DaoFactory = daoFactory;
            EntryManager = entryManager;
            GlobalFolderHelper = globalFolderHelper;
            FilesSettingsHelper = filesSettingsHelper;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            PathProvider = pathProvider;
            ThirdpartyConfiguration = thirdpartyConfiguration;
        }

        public IEnumerable<File<int>> SearchFiles(string text)
        {
            var security = FileSecurity;
            var fileDao = DaoFactory.GetFileDao<int>();
            return fileDao.Search(text).Where(security.CanRead);
        }

        public IEnumerable<Folder<int>> SearchFolders(string text)
        {
            var security = FileSecurity;
            IEnumerable<Folder<int>> result;
            var folderDao = DaoFactory.GetFolderDao<int>();
            result = folderDao.SearchFolders(text).Where(security.CanRead);

            if (ThirdpartyConfiguration.SupportInclusion(DaoFactory)
                && FilesSettingsHelper.EnableThirdParty)
            {
                var id = GlobalFolderHelper.FolderMy;
                if (!Equals(id, 0))
                {
                    var folderMy = folderDao.GetFolder(id);
                    //result = result.Concat(EntryManager.GetThirpartyFolders(folderMy, text));
                }

                id = GlobalFolderHelper.FolderCommon;
                var folderCommon = folderDao.GetFolder(id);
                //result = result.Concat(EntryManager.GetThirpartyFolders(folderCommon, text));
            }

            return result;
        }

        public SearchResultItem[] Search(string text)
        {
            var folderDao = DaoFactory.GetFolderDao<int>();
            var files = SearchFiles(text);
            List<SearchResultItem> list = new List<SearchResultItem>();
            foreach (var file in files)
            {
                var searchResultItem = new SearchResultItem
                {
                    Name = file.Title ?? string.Empty,
                    Description = string.Empty,
                    URL = FilesLinkUtility.GetFileWebPreviewUrl(FileUtility, file.Title, file.ID),
                    Date = file.ModifiedOn,
                    Additional = new Dictionary<string, object>
                                {
                                    { "Author", file.CreateByString.HtmlEncode() },
                                    { "Path", FolderPathBuilder<int>(EntryManager.GetBreadCrumbs(file.FolderID, folderDao)) },
                                    { "Size", FileSizeComment.FilesSizeToString(file.ContentLength) }
                                }
                };
                list.Add(searchResultItem);
            }

            var folders = SearchFolders(text);
            foreach (var folder in folders)
            {
                var searchResultItem = new SearchResultItem
                {
                    Name = folder.Title ?? string.Empty,
                    Description = string.Empty,
                    URL = PathProvider.GetFolderUrl(folder),
                    Date = folder.ModifiedOn,
                    Additional = new Dictionary<string, object>
                                    {
                                            { "Author", folder.CreateByString.HtmlEncode() },
                                            { "Path", FolderPathBuilder<int>(EntryManager.GetBreadCrumbs(folder.ID, folderDao)) },
                                            { "IsFolder", true }
                                    }
                };
                list.Add(searchResultItem);
            }

            return list.ToArray();
        }

        private static string FolderPathBuilder<T>(IEnumerable<FileEntry> folders)
        {
            var titles = folders.Select(f => f.Title).ToList();
            const string separator = " \\ ";
            return 4 < titles.Count
                       ? string.Join(separator, new[] { titles.First(), "...", titles.ElementAt(titles.Count - 2), titles.Last() })
                       : string.Join(separator, titles.ToArray());
        }
    }
}