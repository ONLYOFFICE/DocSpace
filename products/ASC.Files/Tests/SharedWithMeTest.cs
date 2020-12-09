using System;
using System.Collections.Generic;
using System.Threading;

using ASC.Api.Documents;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class SharedWithMeTest : BaseFilesTests
    {
        private FolderWrapper<int> TestFolderRead { get; set; }
        public FileWrapper<int> TestFileRead { get; private set; }
        private FolderWrapper<int> TestFolderReadAndWrite { get; set; }
        public FileWrapper<int> TestFileReadAndWrite { get; private set; }
        public IEnumerable<FileShareParams> TestFolderParamRead { get; private set; }
        public IEnumerable<FileShareParams> TestFolderParamReadAndWrite { get; private set; }
        public UserInfo NewUser { get; set; }
        public TenantManager tenantManager { get; private set; }
        public EmployeeWraperFull TestUser { get; private set; }
       
        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestFolderRead = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolderRead");
            TestFileRead = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFileRead", default);
            TestFolderReadAndWrite = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolderReadAndWrite");
            TestFileReadAndWrite = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFileReadAndWrite", default);
            NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
            TestFolderParamRead = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.ID } };
            TestFolderParamReadAndWrite = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.ReadWrite, ShareTo = NewUser.ID } };
        }
        [OneTimeTearDown]
        public override void TearDown()
        {
            base.TearDown();
            DeleteFolder(TestFolderRead.Id, false, true);
            DeleteFolder(TestFolderReadAndWrite.Id, false, true);
            DeleteFile(TestFileRead.Id, false, true);
            DeleteFile(TestFileReadAndWrite.Id, false, true);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder Read")]
        [Order(1)]
        public void CreateSharedFolderReturnsFolderWrapperRead(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderShare, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        #region Shared Folder and File (Read)

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
        [Category("Folder Read")]
        [Order(2)]
        public void ShareFolderToAnotherUserRead (bool notify, string message)
        {
            var shareFolder = FilesControllerHelper.SetFolderSecurityInfo(TestFolderRead.Id, TestFolderParamRead, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedFolderInfoRead))]
        [Category("Folder Read")]
        [Order(3)]
        public void GetSharedFolderInfoReturnsFolderWrapperRead(string folderTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = FilesControllerHelper.GetFolderInfo(TestFolderRead.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
            Assert.AreEqual(TestFolderRead.Id, folderWrapper.Id);
            Assert.AreEqual(GlobalFolderHelper.FolderShare, folderWrapper.ParentId);
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder Read")]
        [Order(4)]
        public void RenameSharedFolderReturnsFolderWrapperRead(string folderTitle)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.RenameFolder(TestFolderRead.Id, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder Read")]
        [Order(5)]
        public void DeleteSharedFolderReturnsFolderWrapperRead( bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);

            FilesControllerHelper.DeleteFolder(
                TestFolderRead.Id,
                deleteAfter,
                immediately);

            ItemList<FileOperationResult> statuses;

            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsFalse(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File Read")]
        [Order(1)]
        public void CreateSharedFileReturnsFolderWrapperRead(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle, default);
            Assert.AreEqual(fileWrapper.FolderId, GlobalFolderHelper.FolderMy);
            DeleteFile(fileWrapper.Id, false, true);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFileRead))]
        [Category("File Read")]
        [Order(2)]
        public void ShareFileToAnotherUserRead( bool notify, string message)
        {
            var shareFolder = FilesControllerHelper.SetFileSecurityInfo(TestFileRead.Id, TestFolderParamRead, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfo))]
        [Category("File Read")]
        [Order(3)]
        public void GetSharedFileInfoReturnsFolderWrapperRead(string fileTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = FilesControllerHelper.GetFileInfo(TestFileRead.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);

        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File Read")]
        [Order(4)]
        public void UpdateSharedFileReturnsFolderWrapperRead(string fileTitle, int lastVersion)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.UpdateFile(TestFileRead.Id, fileTitle, lastVersion));
            Assert.That(fileWrapper.Message == "You don't have enough permission to rename the file");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File Read")]
        [Order(5)]
        public void DeleteSharedFileReturnsFolderWrapperRead(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            FilesControllerHelper.DeleteFile(
                TestFileRead.Id,
                deleteAfter,
                immediately);

            ItemList<FileOperationResult> statuses;

            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsFalse(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }
        #endregion

        #region Shared Folder and File (Read and Write)

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
        [Category("Folder Read and Write")]
        [Order(2)]
        public void ShareFolderToAnotherUserReadAndWrite(bool notify, string message)
        {
            var shareFolder = FilesControllerHelper.SetFolderSecurityInfo(TestFolderReadAndWrite.Id, TestFolderParamReadAndWrite, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedFolderInfoReadAndWrite))]
        [Category("Folder Read and Write")]
        [Order(3)]
        public void GetSharedFolderInfoReturnsFolderWrapperReadAndWrite(string folderTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = FilesControllerHelper.GetFolderInfo(TestFolderReadAndWrite.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
            Assert.AreEqual(TestFolderReadAndWrite.Id, folderWrapper.Id);
            Assert.AreEqual(GlobalFolderHelper.FolderShare, folderWrapper.ParentId);
        }


        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder Read and Write")]
        [Order(4)]
        public void RenameSharedFolderReturnsFolderWrapperReadAndWrite(string folderTitle)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = FilesControllerHelper.RenameFolder(TestFolderReadAndWrite.Id, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder Read and Write")]
        [Order(5)]
        public void DeleteSharedFolderReturnsFolderWrapperReadAndWrite(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);

            FilesControllerHelper.DeleteFolder(
                TestFolderReadAndWrite.Id,
                deleteAfter,
                immediately);

            ItemList<FileOperationResult> statuses;

            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsFalse(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFile))]
        [Category("File Read and Write")]
        [Order(2)]
        public void ShareFileToAnotherUserReadAndWrite( bool notify, string message)
        {
            var shareFolder = FilesControllerHelper.SetFileSecurityInfo(TestFileReadAndWrite.Id, TestFolderParamReadAndWrite, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfoReadAndWrite))]
        [Category("File Read and Write")]
        [Order(3)]
        public void GetSharedFileInfoReturnsFolderWrapperReadAndWrite(string fileTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = FilesControllerHelper.GetFileInfo(TestFileReadAndWrite.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File Read and Write")]
        [Order(4)]
        public void UpdateSharedFileReturnsFolderWrapperReadAndWrite(string fileTitle, int lastVersion)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = FilesControllerHelper.UpdateFile(TestFileReadAndWrite.Id, fileTitle, lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File Read and Write")]
        [Order(5)]
        public void DeleteSharedFileReturnsFolderWrapperReadAndWrite(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            FilesControllerHelper.DeleteFile(
                TestFileReadAndWrite.Id,
                deleteAfter,
                immediately);

            ItemList<FileOperationResult> statuses;

            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsFalse(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }
        #endregion
    }
}
