using System;
using System.Collections.Generic;

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
       
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);
            NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
            TestFolderParam = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.ID } };

        }

        #region Shared Folder and File (Read)
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("section 'Shared Documents'")]
        public void CreateSharedFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderShare, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
        [Category("section 'Shared Documents'")]
        public void ShareFolderToAnotherUser (string folderTitle, bool notify, string message)
        {
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);
            var shareFolder = FilesControllerHelper.SetFolderSecurityInfo(folderWrapper.Id, TestFolderParam, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfo))]
        [Category("section 'Shared Documents'")]
        public void GetSharedFolderInfo(string folderTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.GetFolderInfo(TestFolder.Id));
            Assert.That(folderWrapper.Message == "You don't have enough permission to view the folder content");

        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("section 'Shared Documents'")]
        public void RenameSharedFolderReturnsFolderWrapperTest(string folderTitle)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.RenameFolder(TestFolder.Id, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("section 'Shared Documents'")]
        public void DeleteSharedFolderTest( bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var statuses = FilesControllerHelper.DeleteFolder(
                TestFolder.Id,
                deleteAfter,
                immediately);

            FileOperationWraper status = null;
            foreach (var item in statuses)
            {
                if (item.OperationType == FileOperationType.Delete)
                {
                    status = item;
                }
            }

            var statusDelete = FileOperationType.Delete;
            Assert.IsNotNull(status);
            Assert.AreNotEqual(statusDelete, status.OperationType);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("section 'Shared Documents'")]
        public void CreateSharedFileReturnsFolderWrapperTest(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle, default);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
            /// var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle, default ));
            //Assert.That(fileWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFile))]
        [Category("section 'Shared Documents'")]
        public void ShareFileToAnotherUser(string fileTitle, bool notify, string message)
        {
            var fileWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, fileTitle);
            var shareFolder = FilesControllerHelper.SetFolderSecurityInfo(fileWrapper.Id, TestFolderParam, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [Test]
        [Category("section 'Shared Documents'")]
        public void GetSharedFileInfo()
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.GetFolderInfo(TestFile.Id));
            Assert.That(fileWrapper.Message == "You don't have enough permission to view the folder content");

        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("section 'Shared Documents'")]
        public void UpdateSharedFileReturnsFolderWrapperTest(string fileTitle, int lastVersion)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.UpdateFile(TestFile.Id, fileTitle, lastVersion));
            Assert.That(fileWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("section 'Shared Documents'")]
        public void DeleteSharedFileTest(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var statuses = FilesControllerHelper.DeleteFolder(
                TestFolder.Id,
                deleteAfter,
                immediately);

            FileOperationWraper status = null;
            foreach (var item in statuses)
            {
                if (item.OperationType == FileOperationType.Delete)
                {
                    status = item;
                }
            }

            var statusDelete = FileOperationType.Delete;
            Assert.IsNotNull(status);
            Assert.AreNotEqual(statusDelete, status.OperationType);
           /* var statuses = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.DeleteFolder(
                TestFile.Id,
                deleteAfter,
                immediately));
            Assert.That(statuses.Message == "You don't have enough permission to delete the folder");*/
        }
        #endregion


        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
