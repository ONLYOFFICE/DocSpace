using System;
using System.Collections.Generic;

using ASC.Api.Documents;
using ASC.Core;
using ASC.Web.Api.Models;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class SharedWithMeTest : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        public IEnumerable<FileShareParams> TestFolderParam { get; private set; }
        private FileWrapper<int> aa { get; set; }

        //private const int CURRENT_TENANT = 0;
        //public const string PASSWORD = "111111";
        //public const string DOMAIN = "test_mail.com";

        public TenantManager tenantManager { get; private set; }
        public EmployeeWraperFull TestUser { get; private set; }
       
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");
            TestUser = new EmployeeWraperFull { Id = Guid.NewGuid(), Email = "test@mail.com", FirstName = "Test", LastName = "Test", IsAdmin = true };
            //Guid OwnerId = Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5");
            //TestFolderParam = new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = TestUser.Id };

            var TestFolderShare = (FolderWrapper<int>)FilesControllerHelper.SetFolderSecurityInfo(TestFolder.Id, TestFolderParam, true, "test");
        }
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
            //Assert.AreEqual(folderWrapper.Id, shareFolder.);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
        [Category("section 'Shared Documents'")]
        public void GetSharedFolderInfo(string folderTitleExpected)
        {
            SecurityContext.AuthenticateMe(TestUser.Id);
            var folderWrapper = FilesControllerHelper.GetFolderSecurityInfo(TestFolder.Id);
            Assert.IsNotNull(folderWrapper);
            //Assert.AreEqual(folderTitleExpected, folderWrapper.Title);

        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("section 'Shared Documents'")]
        public void RenameSharedFolderReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            SecurityContext.AuthenticateMe(TestUser.Id);
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.RenameFolder(TestFolder.Id, folderTitle));
            Assert.That(fileWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("section 'Shared Documents'")]
        public void DeleteSharedFolderTest(int folderId, bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(TestUser.Id);
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
            Assert.AreEqual(statusDelete, status.OperationType);
        }

       

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
