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

using DriveFile = Google.Apis.Drive.v3.Data.File;
using File = Microsoft.SharePoint.Client.File;
using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Core.Core.Thirdparty.ProviderDao;

[Scope(Additional = typeof(SelectorFactoryExtension))]
internal class SelectorFactory
{
    private readonly IServiceProvider _serviceProvider;

    private Regex Selector => new Regex(@"^(?'selector'.*)-(?'id'\d+)(-(?'path'.*)){0,1}$", RegexOptions.Singleline | RegexOptions.Compiled);

    public SelectorFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDaoSelector GetSelector(string id)
    {
        var selector = Match(id);
        return GetSelectorInternal(selector);
    }

    private IDaoSelector GetSelectorInternal(string selector)
    {
        if (selector == Selectors.SharpBox.Id)
            return _serviceProvider.GetService<IDaoSelector<ICloudFileSystemEntry, ICloudDirectoryEntry, ICloudFileSystemEntry>>();
        else if (selector == Selectors.SharePoint.Id)
            return _serviceProvider.GetService<IDaoSelector<File, Folder, ClientObject>>();
        else if (selector == Selectors.GoogleDrive.Id)
            return _serviceProvider.GetService<IDaoSelector<DriveFile, DriveFile, DriveFile>>();
        else if (selector == Selectors.Box.Id)
            return _serviceProvider.GetService<IDaoSelector<BoxFile, BoxFolder, BoxItem>>();
        else if (selector == Selectors.Dropbox.Id)
            return _serviceProvider.GetService<IDaoSelector<FileMetadata, FolderMetadata, Metadata>>();
        else if (selector == Selectors.OneDrive.Id)
            return _serviceProvider.GetService<IDaoSelector<Item, Item, Item>>();
        else
            return null;
    }

    public Dictionary<IDaoSelector, List<string>> GetSelectors(IEnumerable<string> ids)
    {
        var groups = ids.GroupBy(Match);
        groups = groups.Where(g => g.Key != "");
        var dict = new Dictionary<IDaoSelector, List<string>>();
        foreach (var group in groups)
        {
            dict.Add(GetSelectorInternal(group.Key), group.ToList());
        }
        return dict;
    }

    private string Match(string id)
    {
        var match = Selector.Match(id);

        return match.Success ? match.Groups["selector"].Value : "";
    }
}

public static class SelectorFactoryExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<SharpBoxDaoSelector>();
        services.TryAdd<SharePointDaoSelector>();
        OneDriveDaoSelectorExtension.Register(services);
        GoogleDriveDaoSelectorExtension.Register(services);
        DropboxDaoSelectorExtension.Register(services);
        BoxDaoSelectorExtension.Register(services);
    }
}
