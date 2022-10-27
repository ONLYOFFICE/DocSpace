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

namespace ASC.Files.Core.Helpers;

public static class DocSpaceHelper
{
    private static readonly HashSet<FileShare> _fillingFormRoomConstraints
        = new HashSet<FileShare> { FileShare.RoomAdmin, FileShare.FillForms, FileShare.Read };
    private static readonly HashSet<FileShare> _collaborationRoomConstraints
        = new HashSet<FileShare> { FileShare.RoomAdmin, FileShare.Editing, FileShare.Read };
    private static readonly HashSet<FileShare> _reviewRoomConstraints
        = new HashSet<FileShare> { FileShare.RoomAdmin, FileShare.Review, FileShare.Comment, FileShare.Read };
    private static readonly HashSet<FileShare> _viewOnlyRoomConstraints
        = new HashSet<FileShare> { FileShare.RoomAdmin, FileShare.Read };
    private static readonly HashSet<FileShare> _paidShares = new HashSet<FileShare> { FileShare.RoomAdmin };

    public static bool IsRoom(FolderType folderType)
    {
        return folderType == FolderType.CustomRoom || folderType == FolderType.EditingRoom 
            || folderType == FolderType.ReviewRoom || folderType == FolderType.ReadOnlyRoom 
            || folderType == FolderType.FillingFormsRoom;
    }

    public static async Task<bool> LocatedInPrivateRoomAsync<T>(File<T> file, IFolderDao<T> folderDao)
    {
        var parents = await folderDao.GetParentFoldersAsync(file.ParentId).ToListAsync();
        var room = parents.FirstOrDefault(f => IsRoom(f.FolderType));

        return room != null && room.Private;
    }

    public static bool ValidateShare(FolderType folderType, FileShare fileShare, bool isUser)
    {
        if (isUser && _paidShares.Contains(fileShare))
        {
            return false;
        }

        return folderType switch
        {
            FolderType.CustomRoom => true,
            FolderType.FillingFormsRoom => _fillingFormRoomConstraints.Contains(fileShare),
            FolderType.EditingRoom => _collaborationRoomConstraints.Contains(fileShare),
            FolderType.ReviewRoom => _reviewRoomConstraints.Contains(fileShare),
            FolderType.ReadOnlyRoom => _viewOnlyRoomConstraints.Contains(fileShare),
            _ => false
        };
    }
}