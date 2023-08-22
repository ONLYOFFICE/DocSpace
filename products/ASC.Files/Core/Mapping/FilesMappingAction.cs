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

namespace ASC.Files.Core.Mapping;

[Scope]
public class FilesMappingAction : IMappingAction<DbFolderQuery, Folder<int>>, IMappingAction<FileShareRecord, DbFilesSecurity>
{
    private readonly TenantUtil _tenantUtil;

    public FilesMappingAction(TenantUtil tenantUtil)
    {
        _tenantUtil = tenantUtil;
    }

    public void Process(DbFolderQuery source, Folder<int> destination, ResolutionContext context)
    {
        switch (destination.FolderType)
        {
            case FolderType.COMMON:
                destination.Title = FilesUCResource.CorporateFiles;
                break;
            case FolderType.USER:
                destination.Title = FilesUCResource.MyFiles;
                break;
            case FolderType.SHARE:
                destination.Title = FilesUCResource.SharedForMe;
                break;
            case FolderType.Recent:
                destination.Title = FilesUCResource.Recent;
                break;
            case FolderType.Favorites:
                destination.Title = FilesUCResource.Favorites;
                break;
            case FolderType.TRASH:
                destination.Title = FilesUCResource.Trash;
                break;
            case FolderType.Privacy:
                destination.Title = FilesUCResource.PrivacyRoom;
                break;
            case FolderType.Projects:
                destination.Title = FilesUCResource.ProjectFiles;
                break;
            case FolderType.VirtualRooms:
                destination.Title = FilesUCResource.VirtualRooms;
                break;
            case FolderType.Archive:
                destination.Title = FilesUCResource.Archive;
                break;
            case FolderType.BUNCH:
                try
                {
                    destination.Title = string.Empty;
                }
                catch (Exception)
                {
                    //Global.Logger.Error(e);
                }
                break;
        }

        if (destination.FolderType != FolderType.DEFAULT)
        {
            if (0.Equals(destination.ParentId))
            {
                destination.RootFolderType = destination.FolderType;
            }

            if (destination.RootCreateBy == default)
            {
                destination.RootCreateBy = destination.CreateBy;
            }

            if (0.Equals(destination.RootId))
            {
                destination.RootId = destination.Id;
            }
        }
    }

    public void Process(FileShareRecord source, DbFilesSecurity destination, ResolutionContext context)
    {
        if (source.Options == null)
        {
            return;
        }
        
        source.Options.ExpirationDate = _tenantUtil.DateTimeToUtc(source.Options.ExpirationDate);
    }
}
