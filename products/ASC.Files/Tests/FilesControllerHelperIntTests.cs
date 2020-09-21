using ASC.Api.Documents;
using ASC.Files.Core;
using ASC.Files.Tests.Infrastructure;
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

        [Test]
        [Category("section 'My Documents'")]
        public void CreateFolderReturnsFolderWrapperTest()
        {
            var folderWrapperOne = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, DocumentsOptions.FolderOptions.CreateItems.TitleOne);
            var folderWrapperTwo = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, DocumentsOptions.FolderOptions.CreateItems.TitleTwo);

            Assert.IsNotNull(folderWrapperOne);
            Assert.IsNotNull(folderWrapperTwo);
            Assert.AreEqual("FolderOne", folderWrapperOne.Title);
            Assert.AreEqual("FolderTwo", folderWrapperTwo.Title);
        }

        [Test]
        [Category("section 'My Documents'")]
        public void GetFolderReturnsFolderContentWrapperTest()
        {
            var folderContentWrapper = FilesControllerHelper.GetFolder(
                DocumentsOptions.FolderOptions.GetItems.Id,
                UserOptions.Id,
                FilterType.None,
                DocumentsOptions.FolderOptions.GetItems.WithSubFolders);

            var filesCount = folderContentWrapper.Files.Count;
            var foldersCount = folderContentWrapper.Folders.Count;
            Assert.IsNotNull(folderContentWrapper);
            Assert.AreEqual(0, filesCount);
            Assert.AreEqual(0, foldersCount);
        }

        [Test]
        [Category("section 'My Documents'")]
        public void GetFolderInfoReturnsFolderWrapperTest()
        {
            var folderWrapper = FilesControllerHelper.GetFolderInfo(DocumentsOptions.FolderOptions.GetInfoItems.id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(DocumentsOptions.FolderOptions.GetInfoItems.TitleExpected, folderWrapper.Title);
        }

        [Test]
        [Category("section 'My Documents'")]
        public void RenameFolderReturnsFolderWrapperTest()
        {
            var folderWrapper = FilesControllerHelper.RenameFolder(
                DocumentsOptions.FolderOptions.RenameItems.Id,
                DocumentsOptions.FolderOptions.RenameItems.Title);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(DocumentsOptions.FolderOptions.RenameItems.Title, folderWrapper.Title);
        }

        [Test]
        [Category("section 'My Documents'")]
        public void DeleteFolderTest()
        {
            var statuses = FilesControllerHelper.DeleteFolder(
                DocumentsOptions.FolderOptions.DeleteItems.Id,
                DocumentsOptions.FolderOptions.DeleteItems.DeleteAfter,
                DocumentsOptions.FolderOptions.DeleteItems.Immediately);

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

        [Test]
        public void CreateFileReturnsFileWrapperTest()
        {
            var fileWrapperOne = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, DocumentsOptions.FileOptions.CreateItems.TitleOne);
            var fileWrapperTwo = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, DocumentsOptions.FileOptions.CreateItems.TitleTwo);

            Assert.IsNotNull(fileWrapperOne);
            Assert.IsNotNull(fileWrapperTwo);
            Assert.AreEqual("FileOne", fileWrapperOne.Title);
            Assert.AreEqual("FileTwo", fileWrapperTwo.Title);
        }

        [Test]
        [Category("section 'My Documents'")]
        public void GetFileInfoReturnsFilesWrapperTest()
        {
            var fileWrapper = FilesControllerHelper.GetFileInfo(DocumentsOptions.FileOptions.GetInfoItems.id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(DocumentsOptions.FileOptions.GetInfoItems.TitleExpected, fileWrapper.Title);
        }

        [Test]
        public void UpdateFileReturnsFileWrapperTest()
        {
            var fileWrapper = FilesControllerHelper.UpdateFile(
                DocumentsOptions.FileOptions.UpdateItems.Id,
                DocumentsOptions.FileOptions.UpdateItems.Title,
                DocumentsOptions.FileOptions.UpdateItems.LastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(DocumentsOptions.FileOptions.UpdateItems.Title, fileWrapper.Title);
        }

        [Test]
        [Category("section 'My Documents'")]
        public void DeleteFileTest()
        {
            var statuses = FilesControllerHelper.DeleteFile(
                DocumentsOptions.FileOptions.DeleteItems.Id,
                DocumentsOptions.FileOptions.DeleteItems.DeleteAfter,
                DocumentsOptions.FileOptions.DeleteItems.Immediately);

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
