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
class Recent : BaseFilesTests
{
    private FolderDto<int> TestFolder { get; set; }
    public FileDto<int> TestFile { get; private set; }
    public IEnumerable<FileShareParams> TestFileShare { get; private set; }
    public UserInfo NewUser { get; set; }

    [OneTimeSetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        TestFolder = await FoldersControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolder");
        TestFile = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFile", default, default);
        NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
        TestFileShare = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.Id } };
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
    [Order(1)]
    public void CreateFolderReturnsFolderWrapper(string folderTitle)
    {
        var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FoldersControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderRecentAsync, folderTitle));
        Assert.That(folderWrapper.Message == "You don't have enough permission to create");
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
    [Category("File")]
    [Order(1)]
    public void CreateFileReturnsFolderWrapper(string folderTitle)
    {
        var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FoldersControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderRecentAsync, folderTitle));
        Assert.That(folderWrapper.Message == "You don't have enough permission to create");
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
    [Category("File")]
    [Order(2)]
    public async Task RecentFileReturnsFolderWrapper(string fileTitleExpected)
    {
        var RecentFolder = await AddToRecentAsync(TestFile.Id);
        Assert.IsNotNull(RecentFolder);
        Assert.AreEqual(fileTitleExpected + ".docx", RecentFolder.Title);
    }
    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
    [Category("File")]
    [Order(4)]
    public async Task DeleteRecentFileReturnsFolderWrapper(string fileTitleExpected)
    {
        var RecentFolder = await AddToRecentAsync(TestFile.Id);
        await FilesControllerHelper.DeleteFileAsync(
            TestFile.Id,
            false,
            true);

        while (true)
        {
            var statuses = FileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
        Assert.IsNotNull(RecentFolder);
        Assert.AreEqual(fileTitleExpected + ".docx", RecentFolder.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToRecentFile))]
    [Category("File")]
    [Order(3)]
    public async Task ShareFileToAnotherUserAddToRecent(string fileTitleExpected, bool notify, string message)
    {
        await SecurityControllerHelper.SetFileSecurityInfoAsync(TestFile.Id, TestFileShare, notify, message);
        SecurityContext.AuthenticateMe(NewUser.Id);
        var RecentFile = await AddToRecentAsync(TestFile.Id);
        Assert.IsNotNull(RecentFile);
        Assert.AreEqual(fileTitleExpected + ".docx", RecentFile.Title);
    }

    private async Task<FileDto<int>> AddToRecentAsync(int fileId)
    {
        var file = await FileStorageService.GetFileAsync(fileId, -1);
        EntryManager.MarkAsRecent(file);

        return await FileDtoHelper.GetAsync(file);
    }
}
