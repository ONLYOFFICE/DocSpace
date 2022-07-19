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

namespace ASC.Files.Tests;

[TestFixture]
public partial class BaseFilesTests
{
    [TestCase(DataTests.MyId, DataTests.NewTitle)]
    [Category("Folder")]
    [Order(5)]
    [Description("post - files/folder/{folderId} - create new folder")]
    public async Task CreateFolderReturnsFolderWrapper(int folderId, string title)
    {
        var folder = await PostAsync<FolderDto<int>>($"folder/{folderId}", new { Title = title });
        Assert.IsNotNull(folder);
        Assert.AreEqual(title, folder.Title);
        Assert.AreEqual(folderId, folder.ParentId);
    }

    [TestCase(DataTests.EmptyFolderId, 0)]
    [TestCase(DataTests.NotEmptyFolderId, 1)]
    [Category("Folder")]
    [Order(6)]
    [Description("get - files/{folderId} - get empty folder / get not empty folder")]
    public async Task GetFolderEmptyReturnsFolderContentWrapper(int folderId, int expectedCount)
    {
        var folder = await GetAsync<FolderContentDto<int>>(folderId.ToString());

        Assert.IsNotNull(folder);
        Assert.AreEqual(expectedCount, folder.Files.Count);
        Assert.AreEqual(expectedCount, folder.Folders.Count);
    }

    [TestCase(DataTests.SubFolderIdInMy, DataTests.SubFolderNameInMy, DataTests.MyId)]
    [Category("Folder")]
    [Order(8)]
    [Description("get - files/folder/{folderId} - get folder info")]
    public async Task GetFolderInfoReturnsFolderWrapper(int folderId, string folderName, int parentId)
    {
        var folder = await GetAsync<FolderDto<int>>($"folder/{folderId}");

        Assert.IsNotNull(folder);
        Assert.AreEqual(folderName, folder.Title);
        Assert.AreEqual(folderId, folder.Id);
        Assert.AreEqual(parentId, folder.ParentId);
    }

    [TestCase(DataTests.SubFolderIdInMy, DataTests.NewTitle)]
    [Category("Folder")]
    [Order(9)]
    [Description("put - files/folder/{folderId} - rename folder")]
    public async Task RenameFolderReturnsFolderWrapper(int folderId, string newTitle)
    {
        var folder = await PutAsync<FolderDto<int>>($"folder/{folderId}", new { Title = newTitle });

        Assert.IsNotNull(folder);
        Assert.AreEqual(folderId, folder.Id);
        Assert.AreEqual(newTitle, folder.Title);
    }

    [TestCase(DataTests.SubFolderIdInMy, DataTests.DeleteAfter, DataTests.Immediately)]
    [Category("Folder")]
    [Order(10)]
    [Description("delete - files/folder/{folderId} - delete folder")]
    public async Task DeleteFolderReturnsFolderWrapper(int folderId, bool deleteAfter, bool immediately)
    {
        await DeleteAsync($"folder/{folderId}", new { DeleteAfter = deleteAfter, Immediately = immediately });
        var statuses = await WaitLongOperation();
        CheckStatuses(statuses);
    }

    [TestCase(DataTests.NewTitle)]
    [Category("File")]
    [Order(1)]
    [Description("post - files/@my/file - create file in myFolder")]
    public async Task CreateFileReturnsFileWrapper(string newTitle)
    {
        var file = await PostAsync<FileDto<int>>("@my/file", new { Title = newTitle });

        Assert.IsNotNull(file);
        Assert.AreEqual($"{newTitle}.docx", file.Title);

    }

    [TestCase(DataTests.FileId, DataTests.FileName)]
    [Category("File")]
    [Order(2)]
    [Description("get - files/file/{fileId} - get file info")]
    public async Task GetFileInfoReturnsFilesWrapper(int fileId, string fileName)
    {
        var file = await GetAsync<FileDto<int>>($"file/{fileId}");

        Assert.IsNotNull(file);
        Assert.AreEqual(fileName, file.Title);
    }

    [TestCase(DataTests.FileId, DataTests.NewTitle, 0)]
    [Category("File")]
    [Order(3)]
    [Description("put - files/file/{fileId} - update file")]
    public async Task UpdateFileReturnsFileWrapper(int fileId, string newTitle, int lastVersion)
    {
        var file = await PutAsync<FileDto<int>>($"file/{fileId}", new { Title = newTitle, LastVersion = lastVersion });

        Assert.IsNotNull(file);
        Assert.AreEqual(newTitle + ".docx", file.Title);
    }

    [TestCase(DataTests.FileIdForDeleted, DataTests.DeleteAfter, DataTests.Immediately)]
    [Category("File")]
    [Order(4)]
    [Description("delete - files/file/{fileId} - delete file")]
    public async Task DeleteFileReturnsFileWrapper(int fileId, bool deleteAfter, bool immediately)
    {
        await DeleteAsync($"file/{fileId}", new { DeleteAfter = deleteAfter, Immediately = immediately });
        var statuses = await WaitLongOperation();
        CheckStatuses(statuses);
    }

    [TestCase(DataTests.MoveBatchItems)]
    [Category("BatchItems")]
    [Description("put - fileops/move - move batch")]
    public async Task MoveBatchItemsReturnsOperationMove(string json)
    {
        var batchModel = GetBatchModel(json);

        var statuses = await PutAsync<IEnumerable<FileOperationDto>>("fileops/move", batchModel);

        FileOperationDto status = null;
        foreach (var item in statuses)
        {
            if (item.OperationType == FileOperationType.Move)
            {
                status = item;
            }
        }

        var statusMove = FileOperationType.Move;
        Assert.IsNotNull(status);
        Assert.AreEqual(statusMove, status.OperationType);
    }

    [TestCase(DataTests.CopyBatchItems)]
    [Category("BatchItems")]
    [Description("put - fileops/move - copy batch")]
    public async Task CopyBatchItemsReturnsOperationCopy(string json)
    {
        var batchModel = GetBatchModel(json);

        var statuses = await PutAsync<IEnumerable<FileOperationDto>>("fileops/copy", batchModel);

        FileOperationDto status = null;
        foreach (var item in statuses)
        {
            if (item.OperationType == FileOperationType.Copy)
            {
                status = item;
            }
        }

        var statusCopy = FileOperationType.Copy;
        Assert.IsNotNull(status);
        Assert.AreEqual(statusCopy, status.OperationType);
    }
}