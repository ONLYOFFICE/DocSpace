using System.Threading.Tasks;

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
        private FolderWrapper<int> TestFolderNotEmpty { get; set; }
        private FileWrapper<int> TestFile { get; set; }

        [OneTimeSetUp]
        public override async Task SetUp()
        {
            await base.SetUp();

            TestFolder = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFolderNotEmpty = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolderNotEmpty");
            await FilesControllerHelper.CreateFileAsync(TestFolderNotEmpty.Id, "TestFileToContentInTestFolder", default);
            await FilesControllerHelper.CreateFolderAsync(TestFolderNotEmpty.Id, "TestFolderToContentInTestFolder");
            TestFile = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFile", default);
        }

        [OneTimeSetUp]
        public void Authenticate()
        {
            SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await DeleteFolderAsync(TestFolder.Id);
            await DeleteFileAsync(TestFile.Id);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(5)]
        public async Task CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, folderTitle);
            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
            await DeleteFolderAsync(folderWrapper.Id);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItemsEmpty))]
        [Category("Folder")]
        [Order(6)]
        [Description("Empty Content")]
        public async Task GetFolderEmptyReturnsFolderContentWrapper(bool withSubFolders, int filesCountExpected, int foldersCountExpected)
        {
            var folderContentWrapper = await FilesControllerHelper.GetFolderAsync(
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
        [Order(7)]
        [Description("Not Empty Content")]
        public async Task GetFolderNotEmptyReturnsFolderContentWrapper(bool withSubFolders, int filesCountExpected, int foldersCountExpected)
        {
            var folderContentWrapper = await FilesControllerHelper.GetFolderAsync(
                 TestFolderNotEmpty.Id,
                 UserOptions.Id,
                 FilterType.None,
                 withSubFolders);

            var filesCount = folderContentWrapper.Files.Count;
            var foldersCount = folderContentWrapper.Folders.Count;
            Assert.IsNotNull(folderContentWrapper);
            Assert.AreEqual(filesCountExpected, filesCount);
            Assert.AreEqual(foldersCountExpected, foldersCount);
            await DeleteFolderAsync(TestFolderNotEmpty.Id);
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
        [Category("Folder")]
        [Order(8)]
        public async Task GetFolderInfoReturnsFolderWrapper(string folderTitleExpected)
        {
            var folderWrapper = await FilesControllerHelper.GetFolderInfoAsync(TestFolder.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
            Assert.AreEqual(TestFolder.Id, folderWrapper.Id);
            Assert.AreEqual(GlobalFolderHelper.FolderMy, folderWrapper.ParentId);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder")]
        [Order(9)]
        public async Task RenameFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = await FilesControllerHelper.RenameFolderAsync(TestFolder.Id, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder")]
        [Order(10)]
        public async Task DeleteFolderReturnsFolderWrapper(bool deleteAfter, bool immediately)
        {
            await FilesControllerHelper.DeleteFolder(TestFolder.Id, deleteAfter, immediately);
            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                await Task.Delay(100);
            }
            Assert.IsTrue(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(1)]
        public async Task CreateFileReturnsFileWrapper(string fileTitle)
        {
            var fileWrapper = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, fileTitle, default);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
            await DeleteFileAsync(fileWrapper.Id);

        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("File")]
        [Order(2)]
        public async Task GetFileInfoReturnsFilesWrapper(string fileTitleExpected)
        {
            var fileWrapper = await FilesControllerHelper.GetFileInfoAsync(TestFile.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File")]
        [Order(3)]
        public async Task UpdateFileReturnsFileWrapper(string fileTitle, int lastVersion)
        {
            var fileWrapper = await FilesControllerHelper.UpdateFileAsync(
                TestFile.Id,
                fileTitle,
                lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File")]
        [Order(4)]
        public async Task DeleteFileReturnsFileWrapper(bool deleteAfter, bool immediately)
        {
            await FilesControllerHelper.DeleteFileAsync(
                TestFile.Id,
                deleteAfter,
                immediately);

            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                await Task.Delay(100);
            }
            Assert.IsTrue(FileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetMoveBatchItems))]
        [Category("BatchItems")]
        public async Task MoveBatchItemsReturnsOperationMove(string json)
        {
            var batchModel = GetBatchModel(json);

            var statuses = await FilesControllerHelper.MoveBatchItemsAsync(batchModel);

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
        public async Task CopyBatchItemsReturnsOperationCopy(string json)
        {
            var batchModel = GetBatchModel(json);

            var statuses = await FilesControllerHelper.CopyBatchItemsAsync(batchModel);
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
