using System.Threading;
using ASC.Api.Documents;
using ASC.Files.Core;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]

    public class MyDocuments : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        private FileWrapper<int> TestFile { get; set; }

        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();

            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");

            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);

        }
        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(1)]
        public void CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);
            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItemsEmpty))]
        [Category("Folder")]
        [Order(2)]
        [Description("Empty Content")]
        public void GetFolderEmptyReturnsFolderContentWrapper(bool withSubFolders,int filesCountExpected,int foldersCountExpected)
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

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItemsNotEmpty))]
        [Category("Folder")]
        [Order(3)]
        [Description("Not Empty Content")]
        public void GetFolderNotEmptyReturnsFolderContentWrapper(bool withSubFolders, int filesCountExpected, int foldersCountExpected)
        {
            FilesControllerHelper.CreateFile(TestFolder.Id, "TestFileToContentInTestFolder", default);
            FilesControllerHelper.CreateFolder(TestFolder.Id, "TestFolderToContentInTestFolder");
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
        [Category("Folder")]
        [Order(4)]
        public void GetFolderInfoReturnsFolderWrapper(string folderTitleExpected)
        {
            var folderWrapper = FilesControllerHelper.GetFolderInfo(TestFolder.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
            Assert.AreEqual(TestFolder.Id, folderWrapper.Id);
            Assert.AreEqual(GlobalFolderHelper.FolderMy, folderWrapper.ParentId);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder")]
        [Order(5)]
        public void RenameFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.RenameFolder(TestFolder.Id, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder")]
        [Order(6)]
        public void DeleteFolderReturnsFolderWrapper(bool deleteAfter, bool immediately)
        {
            FilesControllerHelper.DeleteFolder(TestFolder.Id, deleteAfter, immediately);

            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsTrue(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(1)]
        public void CreateFileReturnsFileWrapper(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, fileTitle, default);
            
            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }
        

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("File")]
        [Order(2)]
        public void GetFileInfoReturnsFilesWrapper(string fileTitleExpected)
        {
            var fileWrapper = FilesControllerHelper.GetFileInfo(TestFile.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File")]
        [Order(3)]
        public void UpdateFileReturnsFileWrapper(string fileTitle, int lastVersion)
        {
            var fileWrapper = FilesControllerHelper.UpdateFile(
                TestFile.Id,
                fileTitle,
                lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File")]
        [Order(4)]
        public void DeleteFileReturnsFileWrapper(bool deleteAfter, bool immediately)
        {
            FilesControllerHelper.DeleteFile(
                TestFile.Id,
                deleteAfter,
                immediately);

            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsTrue(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetMoveBatchItems))]
        [Category("BatchItems")]
        public void MoveBatchItemsReturnsOperationMove(string json)
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
        [Category("BatchItems")]
        public void CopyBatchItemsReturnsOperationCopy(string json)
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

        
    }
}
