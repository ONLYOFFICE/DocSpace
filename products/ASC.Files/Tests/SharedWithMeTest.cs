using ASC.Api.Documents;
using ASC.Web.Files.Services.WCFService.FileOperations;
using NUnit.Framework;
using System;

namespace ASC.Files.Tests
{
    [TestFixture]
    class SharedWithMeTest : BaseFilesTests<int>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("section 'Shared Documents'")]
        public void CreateSharedFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderShare, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("section 'Shared Documents'")]
        public void RenameSharedFolderReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.RenameFolder(folderId, folderTitle));
            Assert.That(fileWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("section 'Shared Documents'")]
        public void DeleteSharedFolderTest(int folderId, bool deleteAfter, bool immediately)
        {

            var statuses = FilesControllerHelper.DeleteFolder(
                GlobalFolderHelper.FolderShare,
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

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("section 'Shared Documents'")]
        public void CreateSharedFileReturnsFileWrapperTest(string fileTitle)
        {
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle));
            Assert.That(fileWrapper.Message == "You don't have enough permission to create"); 
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
