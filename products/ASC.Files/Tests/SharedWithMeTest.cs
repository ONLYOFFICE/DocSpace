using System;
using System.Collections.Generic;
using System.Threading;

using ASC.Api.Documents;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class SharedWithMeTest : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        public FileWrapper<int> TestFile { get; private set; }
        public IEnumerable<FileShareParams> TestFolderParam { get; private set; }

        public UserInfo NewUser { get; set; }
        public TenantManager tenantManager { get; private set; }
        public EmployeeWraperFull TestUser { get; private set; }
       
        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);
            NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
            TestFolderParam = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.ID } };

        }
        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        #region Shared Folder and File (Read)
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(1)]
        public void CreateSharedFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderShare, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
        [Category("Folder")]
        [Order(2)]
        public void ShareFolderToAnotherUser (string folderTitle, bool notify, string message)
        {
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);
            var shareFolder = FilesControllerHelper.SetFolderSecurityInfo(folderWrapper.Id, TestFolderParam, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [Test]
        [Category("Folder")]
        [Order(3)]
        public void GetSharedFolderInfoReturnsFolderWrapper()
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.GetFolderInfo(TestFolder.Id));
            Assert.That(folderWrapper.Message == "You don't have enough permission to view the folder content");

        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder")]
        [Order(4)]
        public void RenameSharedFolderReturnsFolderWrapper(string folderTitle)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.RenameFolder(TestFolder.Id, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder")]
        [Order(5)]
        public void DeleteSharedFolderReturnsFolderWrapper( bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);

            FilesControllerHelper.DeleteFolder(
                TestFolder.Id,
                deleteAfter,
                immediately);

            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsFalse(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(1)]
        public void CreateSharedFileReturnsFolderWrapper(string fileTitle)
        {
            /*var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle, default);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);*/
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle, default ));
            Assert.That(fileWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFile))]
        [Category("File")]
        [Order(2)]
        public void ShareFileToAnotherUser(string fileTitle, bool notify, string message)
        {
            var fileWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, fileTitle);
            var shareFolder = FilesControllerHelper.SetFolderSecurityInfo(fileWrapper.Id, TestFolderParam, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [Test]
        [Category("File")]
        [Order(3)]
        public void GetSharedFileInfoReturnsFolderWrapper()
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.GetFolderInfo(TestFile.Id));
            Assert.That(fileWrapper.Message == "You don't have enough permission to view the folder content");

        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File")]
        [Order(4)]
        public void UpdateSharedFileReturnsFolderWrapper(string fileTitle, int lastVersion)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.UpdateFile(TestFile.Id, fileTitle, lastVersion));
            Assert.That(fileWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File")]
        [Order(5)]
        public void DeleteSharedFileReturnsFolderWrapper(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            FilesControllerHelper.DeleteFolder(
                TestFolder.Id,
                deleteAfter,
                immediately);

            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsFalse(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }
        #endregion

    }
}
