using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ASC.Api.Documents;
using ASC.Core.Users;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class Recent : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        public FileWrapper<int> TestFile { get; private set; }
        public IEnumerable<FileShareParams> TestFileShare { get; private set; }
        public UserInfo NewUser { get; set; }

        [OneTimeSetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            TestFolder = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFile = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFile", default);
            NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
            TestFileShare = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.ID } };
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
            var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderRecentAsync, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
        [Category("File")]
        [Order(1)]
        public void CreateFileReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderRecentAsync, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("File")]
        [Order(2)]
        public async Task RecentFileReturnsFolderWrapper(string fileTitleExpected)
        {
            var RecentFolder = await FilesControllerHelper.AddToRecentAsync(TestFile.Id);
            Assert.IsNotNull(RecentFolder);
            Assert.AreEqual(fileTitleExpected + ".docx", RecentFolder.Title);
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("File")]
        [Order(4)]
        public async Task DeleteRecentFileReturnsFolderWrapper(string fileTitleExpected)
        {
            var RecentFolder = await FilesControllerHelper.AddToRecentAsync(TestFile.Id);
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
            await FilesControllerHelper.SetFileSecurityInfoAsync(TestFile.Id, TestFileShare, notify, message);
            SecurityContext.AuthenticateMe(NewUser.ID);
            var RecentFile = await FilesControllerHelper.AddToRecentAsync(TestFile.Id);
            Assert.IsNotNull(RecentFile);
            Assert.AreEqual(fileTitleExpected + ".docx", RecentFile.Title);
        }
    }
}
