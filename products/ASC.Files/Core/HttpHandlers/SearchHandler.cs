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

namespace ASC.Web.Files.Configuration;

public class SearchHandler
{
    public Guid ProductID => ProductEntryPoint.ID;
    public ImageOptions Logo => new ImageOptions { ImageFileName = "common_search_icon.png" };
    public Guid ModuleID => ProductID;
    public string SearchName => FilesCommonResource.Search;

    private readonly FileSecurity _fileSecurity;
    private readonly IDaoFactory _daoFactory;
    private readonly EntryManager _entryManager;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly PathProvider _pathProvider;
    private readonly ThirdpartyConfiguration _thirdpartyConfiguration;

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
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
        _entryManager = entryManager;
        _globalFolderHelper = globalFolderHelper;
        _filesSettingsHelper = filesSettingsHelper;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _pathProvider = pathProvider;
        _thirdpartyConfiguration = thirdpartyConfiguration;
    }

    public IAsyncEnumerable<File<int>> SearchFilesAsync(string text)
    {
        var security = _fileSecurity;
        var fileDao = _daoFactory.GetFileDao<int>();
        var files = fileDao.SearchAsync(text);

        return files.WhereAwait(async e => await security.CanReadAsync(e));
    }

    public IAsyncEnumerable<Folder<int>> SearchFoldersAsync(string text)
    {
        var security = _fileSecurity;
        var folderDao = _daoFactory.GetFolderDao<int>();
        var folders = folderDao.SearchFoldersAsync(text);
        var result = folders.WhereAwait(async e => await security.CanReadAsync(e));

        if (_thirdpartyConfiguration.SupportInclusion(_daoFactory)
            && _filesSettingsHelper.EnableThirdParty)
        {
            //var id = GlobalFolderHelper.FolderMy;
            //if (!Equals(id, 0))
            //{
            //var folderMy = await folderDao.GetFolderAsync(id);
            //result = result.Concat(EntryManager.GetThirpartyFolders(folderMy, text));
            //}

            //id = await GlobalFolderHelper.FolderCommonAsync;
            //var folderCommon = await folderDao.GetFolderAsync(id);
            //result = result.Concat(EntryManager.GetThirpartyFolders(folderCommon, text));
        }

        return result;
    }

    public async Task<SearchResultItem[]> SearchAsync(string text)
    {
        var folderDao = _daoFactory.GetFolderDao<int>();
        var files = SearchFilesAsync(text);
        var list = new List<SearchResultItem>();
        await foreach (var file in files)
        {
            var searchResultItem = new SearchResultItem
            {
                Name = file.Title ?? string.Empty,
                Description = string.Empty,
                URL = _filesLinkUtility.GetFileWebPreviewUrl(_fileUtility, file.Title, file.Id),
                Date = file.ModifiedOn,
                Additional = new Dictionary<string, object>
                                {
                                    { "Author", file.CreateByString.HtmlEncode() },
                                    { "Path", FolderPathBuilder(await _entryManager.GetBreadCrumbsAsync(file.ParentId, folderDao)) },
                                    { "Size", FileSizeComment.FilesSizeToString(file.ContentLength) }
                                }
            };

            list.Add(searchResultItem);
        }

        var folders = SearchFoldersAsync(text);
        await foreach (var folder in folders)
        {
            var searchResultItem = new SearchResultItem
            {
                Name = folder.Title ?? string.Empty,
                Description = string.Empty,
                URL = await _pathProvider.GetFolderUrlAsync(folder),
                Date = folder.ModifiedOn,
                Additional = new Dictionary<string, object>
                                    {
                                            { "Author", folder.CreateByString.HtmlEncode() },
                                            { "Path", FolderPathBuilder(await _entryManager.GetBreadCrumbsAsync(folder.Id, folderDao)) },
                                            { "IsFolder", true }
                                    }
            };
            list.Add(searchResultItem);
        }

        return list.ToArray();
    }

    private static string FolderPathBuilder(IEnumerable<FileEntry> folders)
    {
        var titles = folders.Select(f => f.Title).ToList();
        const string separator = " \\ ";

        return 4 < titles.Count
                   ? string.Join(separator, new[] { titles.First(), "...", titles.ElementAt(titles.Count - 2), titles.Last() })
                   : string.Join(separator, titles.ToArray());
    }
}
