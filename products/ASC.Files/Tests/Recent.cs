using System;
using System.Collections.Generic;

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
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);
            NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
            TestFileShare = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.ID } };
        }
        [OneTimeTearDown]
        public override void TearDown()
        {
            base.TearDown();
            DeleteFolder(TestFolder.Id, false, true);
            DeleteFile(TestFile.Id, false, true);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(1)]
        public void CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderRecent, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
        [Category("File")]
        [Order(1)]
        public void CreateFileReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderRecent, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("File")]
        [Order(2)]
        public void RecentFileReturnsFolderWrapper(string fileTitleExpected)
        {
            var RecentFolder = FilesControllerHelper.AddToRecent(TestFile.Id);
            Assert.IsNotNull(RecentFolder);
            Assert.AreEqual(fileTitleExpected + ".docx", RecentFolder.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToRecentFile))]
        [Category("File")]
        [Order(3)]
        public void ShareFileToAnotherUserAddToRecent(string fileTitleExpected, bool notify, string message)
        {
            FilesControllerHelper.SetFileSecurityInfo(TestFile.Id, TestFileShare, notify, message);
            SecurityContext.AuthenticateMe(NewUser.ID);
            var RecentFile = FilesControllerHelper.AddToRecent(TestFile.Id);
            Assert.IsNotNull(RecentFile);
            Assert.AreEqual(fileTitleExpected + ".docx", RecentFile.Title);
        }
    }
}
