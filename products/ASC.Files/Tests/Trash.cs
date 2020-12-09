using System;
using System.Threading;

using ASC.Api.Documents;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class Trash :BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        public FileWrapper<int> TestFile { get; private set; }

        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);

        }
        [OneTimeTearDown]
        public override void TearDown()
        {
            base.TearDown();
            DeleteFile(TestFile.Id, false, true);
            DeleteFolder(TestFolder.Id, false, true);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(1)]
        public void CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder((int)GlobalFolderHelper.FolderTrash, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(2)]
        public void CreateFileReturnsFolderWrapper(string fileTitle)
        {
            var fileWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFile((int)GlobalFolderHelper.FolderTrash, fileTitle, default));
            Assert.That(fileWrapper.Message == "You don't have enough permission to create");
        }

        [Test]
        [Category("Folder")]
        [Order(2)]
        public void DeleteFileFromTrash()
        {
            var Empty = FilesControllerHelper.EmptyTrash();
            
            ItemList<FileOperationResult> statuses;

            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
            Assert.IsTrue(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }
    }
}
