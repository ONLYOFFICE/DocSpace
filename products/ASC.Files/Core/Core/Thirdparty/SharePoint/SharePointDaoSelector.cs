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

using File = Microsoft.SharePoint.Client.File;
using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint;

[Scope(Additional = typeof(SharePointDaoSelectorExtension))]
internal class SharePointDaoSelector : RegexDaoSelectorBase<File, Folder, ClientObject>
{

    public SharePointDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
        : base(serviceProvider, daoFactory)
    {
    }

    public override IFileDao<string> GetFileDao(string id)
    {
        var fileDao = _serviceProvider.GetService<SharePointFileDao>();
        var info = GetInfo(id);
        fileDao.Init(info.PathPrefix, info.ProviderInfo);
        return fileDao;
    }

    public override IFolderDao<string> GetFolderDao(string id)
    {
        var folderDao = _serviceProvider.GetService<SharePointFolderDao>();
        var info = GetInfo(id);
        folderDao.Init(info.PathPrefix, info.ProviderInfo);
        return folderDao;
    }

    public override IThirdPartyTagDao GetTagDao(string id)
    {
        var tagDao = _serviceProvider.GetService<SharePointTagDao>();
        var info = GetInfo(id);
        tagDao.Init(info.PathPrefix, info.ProviderInfo);
        return tagDao;
    }

    public override string ConvertId(string id)
    {
        if (id != null)
        {
            var match = Selector.Match(id);
            if (match.Success)
            {
                //return GetInfo(id).ProviderInfo.SpRootFolderId + match.Groups["path"].Value.Replace('|', '/');
                return "" + match.Groups["path"].Value.Replace('|', '/');
            }

            throw new ArgumentException("Id is not a sharepoint id");
        }

        return base.ConvertId(null);
    }
}

public static class SharePointDaoSelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<SharePointFileDao>();
        services.TryAdd<SharePointFolderDao>();
        services.TryAdd<IProviderInfo<File, Folder, ClientObject>, SharePointProviderInfo>();
        services.TryAdd<SharePointTagDao>();
        services.TryAdd<IDaoSelector<File, Folder, ClientObject>, SharePointDaoSelector>();
    }
}
