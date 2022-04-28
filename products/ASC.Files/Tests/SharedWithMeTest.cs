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
class SharedWithMeTest : BaseFilesTests
{
    private FolderDto<int> TestFolderRead { get; set; }
    public FileDto<int> TestFileRead { get; private set; }
    private FolderDto<int> TestFolderReadAndWrite { get; set; }
    public FileDto<int> TestFileReadAndWrite { get; private set; }
    public IEnumerable<FileShareParams> TestFolderParamRead { get; private set; }
    public IEnumerable<FileShareParams> TestFolderParamReadAndWrite { get; private set; }
    public UserInfo NewUser { get; set; }
    public TenantManager tenantManager { get; private set; }
    public EmployeeFullDto TestUser { get; private set; }

    [OneTimeSetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        TestFolderRead = await FoldersControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolderRead");
        TestFileRead = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFileRead", default, default);
        TestFolderReadAndWrite = await FoldersControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolderReadAndWrite");
        TestFileReadAndWrite = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFileReadAndWrite", default, default);
        NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
        TestFolderParamRead = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.Id } };
        TestFolderParamReadAndWrite = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.ReadWrite, ShareTo = NewUser.Id } };
    }

    [OneTimeSetUp]
    public void Authenticate()
    {
        SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await DeleteFolderAsync(TestFolderRead.Id);
        await DeleteFolderAsync(TestFolderReadAndWrite.Id);
        await DeleteFileAsync(TestFileRead.Id);
        await DeleteFileAsync(TestFileReadAndWrite.Id);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
    [Category("Folder Read")]
    [Order(1)]
    public void CreateSharedFolderReturnsFolderWrapperRead(string folderTitle)
    {
        var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FoldersControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderShareAsync, folderTitle));
        Assert.That(folderWrapper.Message == "You don't have enough permission to create");
    }

    #region Shared Folder and File (Read)

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
    [Category("Folder Read")]
    [Order(2)]
    public async Task ShareFolderToAnotherUserRead(bool notify, string message)
    {
        var shareFolder = await SecurityControllerHelper.SetFolderSecurityInfoAsync(TestFolderRead.Id, TestFolderParamRead, notify, message);
        Assert.IsNotNull(shareFolder);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedFolderInfoRead))]
    [Category("Folder Read")]
    [Order(3)]
    public async Task GetSharedFolderInfoReturnsFolderWrapperRead(string folderTitleExpected)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var folderWrapper = await FoldersControllerHelper.GetFolderInfoAsync(TestFolderRead.Id);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
        Assert.AreEqual(TestFolderRead.Id, folderWrapper.Id);
        Assert.AreEqual(await GlobalFolderHelper.FolderShareAsync, folderWrapper.ParentId);
    }
    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
    [Category("Folder Read")]
    [Order(4)]
    public void RenameSharedFolderReturnsFolderWrapperRead(string folderTitle)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FoldersControllerHelper.RenameFolderAsync(TestFolderRead.Id, folderTitle));
        Assert.That(folderWrapper.Message == "You don't have enough permission to rename the folder");
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
    [Category("Folder Read")]
    [Order(5)]
    public async Task DeleteSharedFolderReturnsFolderWrapperRead(bool deleteAfter, bool immediately)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);

        var result = (await FoldersControllerHelper.DeleteFolder(
            TestFolderRead.Id,
            deleteAfter,
            immediately))
            .FirstOrDefault();

        await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
    [Category("File Read")]
    [Order(6)]
    public async Task CreateSharedFileReturnsFolderWrapperRead(string fileTitle)
    {
        var fileWrapper = await FilesControllerHelper.CreateFileAsync(await GlobalFolderHelper.FolderShareAsync, fileTitle, default, default);
        Assert.AreEqual(fileWrapper.FolderId, GlobalFolderHelper.FolderMy);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFileRead))]
    [Category("File Read")]
    [Order(7)]
    public async Task ShareFileToAnotherUserRead(bool notify, string message)
    {
        var shareFolder = await SecurityControllerHelper.SetFileSecurityInfoAsync(TestFileRead.Id, TestFolderParamRead, notify, message);
        Assert.IsNotNull(shareFolder);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfo))]
    [Category("File Read")]
    [Order(8)]
    public async Task GetSharedFileInfoReturnsFolderWrapperRead(string fileTitleExpected)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var fileWrapper = await FilesControllerHelper.GetFileInfoAsync(TestFileRead.Id);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);

    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
    [Category("File Read")]
    [Order(9)]
    public void UpdateSharedFileReturnsFolderWrapperRead(string fileTitle, int lastVersion)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var fileWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.UpdateFileAsync(TestFileRead.Id, fileTitle, lastVersion));
        Assert.That(fileWrapper.Message == "You don't have enough permission to rename the file");
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
    [Category("File Read")]
    [Order(10)]
    public async Task DeleteSharedFileReturnsFolderWrapperRead(bool deleteAfter, bool immediately)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var result = (await FilesControllerHelper.DeleteFileAsync(
            TestFileRead.Id,
            deleteAfter,
            immediately))
            .FirstOrDefault();

        await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFile);
    }
    #endregion

    #region Shared Folder and File (Read and Write)

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
    [Category("Folder Read and Write")]
    [Order(11)]
    public async Task ShareFolderToAnotherUserReadAndWrite(bool notify, string message)
    {
        var shareFolder = await SecurityControllerHelper.SetFolderSecurityInfoAsync(TestFolderReadAndWrite.Id, TestFolderParamReadAndWrite, notify, message);
        Assert.IsNotNull(shareFolder);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedFolderInfoReadAndWrite))]
    [Category("Folder Read and Write")]
    [Order(12)]
    public async Task GetSharedFolderInfoReturnsFolderWrapperReadAndWrite(string folderTitleExpected)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var folderWrapper = await FoldersControllerHelper.GetFolderInfoAsync(TestFolderReadAndWrite.Id);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
        Assert.AreEqual(TestFolderReadAndWrite.Id, folderWrapper.Id);
        Assert.AreEqual(await GlobalFolderHelper.FolderShareAsync, folderWrapper.ParentId);
    }


    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
    [Category("Folder Read and Write")]
    [Order(13)]
    public async Task RenameSharedFolderReturnsFolderWrapperReadAndWrite(string folderTitle)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var folderWrapper = await FoldersControllerHelper.RenameFolderAsync(TestFolderReadAndWrite.Id, folderTitle);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitle, folderWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
    [Category("Folder Read and Write")]
    [Order(14)]
    public async Task DeleteSharedFolderReturnsFolderWrapperReadAndWrite(bool deleteAfter, bool immediately)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);

        var result = (await FoldersControllerHelper.DeleteFolder(
            TestFolderReadAndWrite.Id,
            deleteAfter,
            immediately))
            .FirstOrDefault();

        await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFile))]
    [Category("File Read and Write")]
    [Order(15)]
    public async Task ShareFileToAnotherUserReadAndWrite(bool notify, string message)
    {
        var shareFolder = await SecurityControllerHelper.SetFileSecurityInfoAsync(TestFileReadAndWrite.Id, TestFolderParamReadAndWrite, notify, message);
        Assert.IsNotNull(shareFolder);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfoReadAndWrite))]
    [Category("File Read and Write")]
    [Order(16)]
    public async Task GetSharedFileInfoReturnsFolderWrapperReadAndWrite(string fileTitleExpected)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var fileWrapper = await FilesControllerHelper.GetFileInfoAsync(TestFileReadAndWrite.Id);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
    [Category("File Read and Write")]
    [Order(17)]
    public async Task UpdateSharedFileReturnsFolderWrapperReadAndWrite(string fileTitle, int lastVersion)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var fileWrapper = await FilesControllerHelper.UpdateFileAsync(TestFileReadAndWrite.Id, fileTitle, lastVersion);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
    [Category("File Read and Write")]
    [Order(18)]
    public async Task DeleteSharedFileReturnsFolderWrapperReadAndWrite(bool deleteAfter, bool immediately)
    {
        SecurityContext.AuthenticateMe(NewUser.Id);
        var result = (await FilesControllerHelper.DeleteFileAsync(
            TestFileReadAndWrite.Id,
            deleteAfter,
            immediately))
            .FirstOrDefault();

        await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFile);
    }
    #endregion

    private async Task WaitLongOperation(FileOperationDto result, string assertError)
    {
        if (result != null && result.Finished)
        {
            Assert.That(result.Error == assertError, result.Error);
            return;
        }

        List<FileOperationResult> statuses;
        while (true)
        {
            statuses = FileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
            {
                break;
            }

            await Task.Delay(100);
        }

        var error = string.Join(",", statuses.Select(r => r.Error));
        Assert.That(error == assertError, error);
    }
}
