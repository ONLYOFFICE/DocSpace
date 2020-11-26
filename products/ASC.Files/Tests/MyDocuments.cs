using ASC.Api.Documents;
using ASC.Files.Core;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]

    //private 
    public class MyDocuments : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        private FileWrapper<int> TestFile { get; set; }


        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");

            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);

        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("section 'My Documents'")]
        public void CreateFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);
            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItems))]
        [Category("section 'My Documents'")]
        public void GetFolderReturnsFolderContentWrapperTest(bool withSubFolders,int filesCountExpected,int foldersCountExpected)
        {
            var folderContentWrapper = FilesControllerHelper.GetFolder(
                 TestFolder.Id,
                 UserOptions.Id,
                 FilterType.None,
                 withSubFolders);

            var filesCount = folderContentWrapper.Files.Count;
            var foldersCount = folderContentWrapper.Folders.Count;
            Assert.IsNotNull(folderContentWrapper);
            Assert.AreEqual(filesCountExpected, filesCount);
            Assert.AreEqual(foldersCountExpected, foldersCount);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
        [Category("section 'My Documents'")]
        public void GetFolderInfoReturnsFolderWrapperTest(string folderTitleExpected)
        {
            var folderWrapper = FilesControllerHelper.GetFolderInfo(TestFolder.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("section 'My Documents'")]
        public void RenameFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.RenameFolder(TestFolder.Id, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("section 'My Documents'")]
        public void DeleteFolderTest(bool deleteAfter, bool immediately)
        {
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

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("section 'My Documents'")]
        public void CreateFileReturnsFileWrapperTest(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, fileTitle, default);
            
            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }
        

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("section 'My Documents'")]
        public void GetFileInfoReturnsFilesWrapperTest(string fileTitleExpected)
        {
            var fileWrapper = FilesControllerHelper.GetFileInfo(TestFile.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("section 'My Documents'")]
        public void UpdateFileReturnsFileWrapperTest(string fileTitle, int lastVersion)
        {
            var fileWrapper = FilesControllerHelper.UpdateFile(
                TestFile.Id,
                fileTitle,
                lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("section 'My Documents'")]
        public void DeleteFileTest(bool deleteAfter, bool immediately)
        {
            var statuses = FilesControllerHelper.DeleteFile(
                TestFile.Id,
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

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetMoveBatchItems))]
        [Category("section 'My Documents'")]
        public void MoveBatchItemsReturnsOperationMoveTest(string json)
        {
            var batchModel = GetBatchModel(json);

            var statuses = FilesControllerHelper.MoveBatchItems(batchModel);

            FileOperationWraper status = null;
            foreach (var item in statuses)
            {
                if (item.OperationType == FileOperationType.Move)
                {
                    status = item;
                }
            }

            var statusMove = FileOperationType.Move;
            Assert.IsNotNull(status);
            Assert.AreEqual(statusMove, status.OperationType);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCopyBatchItems))]
        [Category("section 'My Documents'")]
        public void CopyBatchItemsReturnsOperationCopyTest(string json)
        {
            var batchModel = GetBatchModel(json);

            var statuses = FilesControllerHelper.CopyBatchItems(batchModel);

            FileOperationWraper status = null;
            foreach (var item in statuses)
            {
                if (item.OperationType == FileOperationType.Copy)
                {
                    status = item;
                }
            }

            var statusCopy = FileOperationType.Copy;
            Assert.IsNotNull(status);
            Assert.AreEqual(statusCopy, status.OperationType);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
