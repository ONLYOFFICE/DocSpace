using ASC.Api.Documents;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    public class FilesControllerHelperIntTests : BaseFilesTests<int>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TestCase("folderTwo")]
        [TestCase("folderTwo")]
        [Category("section 'My Documents'")]
        public void CreateFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var qwe = Configuration[""];
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCase(14, "fold")]
        [Category("section 'My Documents'")]
        public void RenameFolderReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.RenameFolder(folderId, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCase(14, false, true)]
        [Category("section 'My Documents'")]
        public void DeleteFolderTest(int folderId, bool deleteAfter, bool immediately)
        {
            FilesControllerHelper.DeleteFolder(folderId, deleteAfter, immediately);

            var statuses = FilesControllerHelper.GetOperationStatuses();

            FileOperationWraper status = null;

            foreach (var item in statuses)
            {
                if (item.OperationType == FileOperationType.Delete)
                {
                    status = item;
                }
            }

            Assert.IsNotNull(status);
        }

        [TestCase("fileOne")]
        [TestCase("fileTwo")]
        public void CreateFileReturnsFilesWrapperTest(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, fileTitle);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
