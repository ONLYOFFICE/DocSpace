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

namespace ASC.Files.Tests.Infrastructure;
public static class DataTests
{
    public const int MyId = 1;
    public const int ShareId = 2;
    public const int FavoritesId = 3;
    public const int RecentId = 4;
    public const int TrashId = 5;

    public const int EmptyFolderId = 11;
    public const int NotEmptyFolderId = 12;

    public const int SubFolderIdInMy = 11;
    public const string SubFolderNameInMy = "subfolder1";

    public const int SubFolderId = 12;
    public const string SubFolderName = "subfolder2";

    public const int SharedForReadFolderId = 15;
    public const string SharedForReadFolderName = "subfolder4";

    public const int SharedForReadAndWriteFolderId = 16;
    public const string SharedForReadAndWriteFolderName = "subfolder5";

    public const string NewTitle = "NewTitle";

    public const int FileId = 1;
    public const string FileName = "New Document.docx";

    public const int FileIdForDeleted = 2;

    public const int SharedForReadFileId = 3;
    public const string SharedForReadFileName = "New Document.docx";

    public const int SharedForReadAndWriteFileId = 4;
    public const string SharedForReadAndWriteFileName = "New Document.docx";

    public const int FileIdForRecent = 6;
    public const string FileNameForRecent = "New Document.docx";

    public const string MoveBatchItems = " [ { \"folderIds\": [ 1, 2, 3 ] }, { \"fileIds\": [ 1 , 2 ] }, { \"destFolderId\": 4 } ]";
    public const string CopyBatchItems = " [ { \"folderIds\": [ 6 ] }, { \"fileIds\": [ 4 , 5 ] }, { \"destFolderId\": 5 } ]";

    public const bool DeleteAfter = false;
    public const bool Immediately = true;

    public const bool Notify = false;
    public const string Message = "test_message";

    public const string RoomTitle = "Room_Title";

    public const string NewRoomTitle = "New_Room_Title";

    public const int CustomRoomId = 5;
    public const int RoomId = 18;
    public const int RoomIdForDelete = 19;
    public const int RoomIdForArchive = 20;
    public const int RoomIdForUnpin = 21;
    public const int RoomIdForUnarchive = 22;
    public const int RoomIdWithTags = 24;

    public const string TagNames = "name1,name2";
    public const string Email = "test1@gmail.com";
    public const string Image = "appIcon-180.png";
}
