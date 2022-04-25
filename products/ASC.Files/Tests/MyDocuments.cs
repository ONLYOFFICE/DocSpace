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

public class MyDocuments : BaseFilesTests
{
    private FolderDto<int> TestFolder { get; set; }
    private FolderDto<int> TestFolderNotEmpty { get; set; }
    private FileDto<int> TestFile { get; set; }

    [OneTimeSetUp]
    public override async Task SetUp()
    {
        await base.SetUp();

        TestFolder = await FoldersControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolder");
        TestFolderNotEmpty = await FoldersControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolderNotEmpty");
        await FilesControllerHelper.CreateFileAsync(TestFolderNotEmpty.Id, "TestFileToContentInTestFolder", default, default);
        await FoldersControllerHelper.CreateFolderAsync(TestFolderNotEmpty.Id, "TestFolderToContentInTestFolder");
        TestFile = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFile", default, default);
    }

    [OneTimeSetUp]
    public void Authenticate()
    {
        SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await DeleteFolderAsync(TestFolder.Id);
        await DeleteFileAsync(TestFile.Id);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
    [Category("Folder")]
    [Order(5)]
    public async Task CreateFolderReturnsFolderWrapper(string folderTitle)
    {
        var folderWrapper = await FoldersControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, folderTitle);
        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitle, folderWrapper.Title);
        await DeleteFolderAsync(folderWrapper.Id);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItemsEmpty))]
    [Category("Folder")]
    [Order(6)]
    [Description("Empty Content")]
    public async Task GetFolderEmptyReturnsFolderContentWrapper(bool withSubFolders, int filesCountExpected, int foldersCountExpected)
    {
        var folderContentWrapper = await FoldersControllerHelper.GetFolderAsync(
             TestFolder.Id,
             UserOptions.Id,
             FilterType.None,
             false,
             withSubFolders);

        var filesCount = folderContentWrapper.Files.Count;
        var foldersCount = folderContentWrapper.Folders.Count;
        Assert.IsNotNull(folderContentWrapper);
        Assert.AreEqual(filesCountExpected, filesCount);
        Assert.AreEqual(foldersCountExpected, foldersCount);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItemsNotEmpty))]
    [Category("Folder")]
    [Order(7)]
    [Description("Not Empty Content")]
    public async Task GetFolderNotEmptyReturnsFolderContentWrapper(bool withSubFolders, int filesCountExpected, int foldersCountExpected)
    {
        var folderContentWrapper = await FoldersControllerHelper.GetFolderAsync(
             TestFolderNotEmpty.Id,
             UserOptions.Id,
             FilterType.None,
             false,
             withSubFolders);

        var filesCount = folderContentWrapper.Files.Count;
        var foldersCount = folderContentWrapper.Folders.Count;
        Assert.IsNotNull(folderContentWrapper);
        Assert.AreEqual(filesCountExpected, filesCount);
        Assert.AreEqual(foldersCountExpected, foldersCount);
        await DeleteFolderAsync(TestFolderNotEmpty.Id);
    }
    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
    [Category("Folder")]
    [Order(8)]
    public async Task GetFolderInfoReturnsFolderWrapper(string folderTitleExpected)
    {
        var folderWrapper = await FoldersControllerHelper.GetFolderInfoAsync(TestFolder.Id);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
        Assert.AreEqual(TestFolder.Id, folderWrapper.Id);
        Assert.AreEqual(GlobalFolderHelper.FolderMy, folderWrapper.ParentId);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
    [Category("Folder")]
    [Order(9)]
    public async Task RenameFolderReturnsFolderWrapper(string folderTitle)
    {
        var folderWrapper = await FoldersControllerHelper.RenameFolderAsync(TestFolder.Id, folderTitle);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitle, folderWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
    [Category("Folder")]
    [Order(10)]
    public async Task DeleteFolderReturnsFolderWrapper(bool deleteAfter, bool immediately)
    {
        await FoldersControllerHelper.DeleteFolder(TestFolder.Id, deleteAfter, immediately);
        while (true)
        {
            var statuses = FileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
        Assert.IsTrue(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
    [Category("File")]
    [Order(1)]
    public async Task CreateFileReturnsFileWrapper(string fileTitle)
    {
        var fileWrapper = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, fileTitle, default, default);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        await DeleteFileAsync(fileWrapper.Id);

    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
    [Category("File")]
    [Order(2)]
    public async Task GetFileInfoReturnsFilesWrapper(string fileTitleExpected)
    {
        var fileWrapper = await FilesControllerHelper.GetFileInfoAsync(TestFile.Id);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
    [Category("File")]
    [Order(3)]
    public async Task UpdateFileReturnsFileWrapper(string fileTitle, int lastVersion)
    {
        var fileWrapper = await FilesControllerHelper.UpdateFileAsync(
            TestFile.Id,
            fileTitle,
            lastVersion);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
    [Category("File")]
    [Order(4)]
    public async Task DeleteFileReturnsFileWrapper(bool deleteAfter, bool immediately)
    {
        await FilesControllerHelper.DeleteFileAsync(
            TestFile.Id,
            deleteAfter,
            immediately);

        while (true)
        {
            var statuses = FileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
        Assert.IsTrue(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetMoveBatchItems))]
    [Category("BatchItems")]
    public async Task MoveBatchItemsReturnsOperationMove(string json)
    {
        var batchModel = GetBatchModel(json);

        var statuses = await OperationControllerHelper.MoveBatchItemsAsync(batchModel);

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

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCopyBatchItems))]
    [Category("BatchItems")]
    public async Task CopyBatchItemsReturnsOperationCopy(string json)
    {
        var batchModel = GetBatchModel(json);

        var statuses = await OperationControllerHelper.CopyBatchItemsAsync(batchModel);
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
