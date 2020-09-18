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

        [TestCase("folderOne")]
        [TestCase("folderTwo")]
        [Category("section 'My Documents'")]
        public void CreateFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCase(14, true)]
        [Category("section 'My Documents'")]
        public void GetFolderReturnsFolderContentWrapperTest(int folderId, bool withSubFolders)
        {
            var folderContentWrapper = FilesControllerHelper.GetFolder(folderId, UserOptions.Id, FilterType.None, withSubFolders);

            var filesCount = folderContentWrapper.Files.Count;
            var foldersCount = folderContentWrapper.Folders.Count;
            Assert.IsNotNull(folderContentWrapper);
            Assert.AreEqual(0, filesCount);
            Assert.AreEqual(0, foldersCount);
        }

        [TestCase(9, "folder")]
        [Category("section 'My Documents'")]
        public void GetFolderInfoReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.GetFolderInfo(folderId);

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
            var statuses = FilesControllerHelper.DeleteFolder(folderId, deleteAfter, immediately);

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

        [TestCase("fileOne")]
        [TestCase("fileTwo")]
        public void CreateFileReturnsFileWrapperTest(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, fileTitle);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
        }

        [TestCase(1, "fileOne.docx")]
        [Category("section 'My Documents'")]
        public void GetFileInfoReturnsFilesWrapperTest(int fileId, string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.GetFileInfo(fileId);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
        }

        [TestCase(1, "test", 3)]
        public void UpdateFileReturnsFileWrapperTest(int fileId, string fileTitle, int lastVersion)
        {
            var fileWrapper = FilesControllerHelper.UpdateFile(fileId, fileTitle, lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
        }

        [TestCase(2, false, true)]
        [Category("section 'My Documents'")]
        public void DeleteFileTest(int fileId, bool deleteAfter, bool immediately)
        {
            var statuses = FilesControllerHelper.DeleteFile(fileId, deleteAfter, immediately);

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
