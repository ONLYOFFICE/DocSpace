using ASC.Api.Documents;
using ASC.Files.Tests;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Tests.ASC.Files.Tests
{
    [TestFixture]
    public class FilesControllerTests : BaseTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TestCase("folderOne")]
        [TestCase("folderTwo")]
        [Category("section 'My Documents'")]
        public void CreateFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = FilesController.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCase(14, "fold")]
        [Category("section 'My Documents'")]
        public void RenameFolderReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            var folderWrapper = FilesController.RenameFolder(folderId, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCase(14, false, true)]
        [Category("section 'My Documents'")]
        public void DeleteFolderTest(int folderId, bool deleteAfter, bool immediately)
        {
            FilesController.DeleteFolder(folderId, deleteAfter, immediately);

            var statuses = FilesController.GetOperationStatuses();

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

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
