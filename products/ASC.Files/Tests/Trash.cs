using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ASC.Api.Documents;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class Trash : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        public FileWrapper<int> TestFile { get; private set; }

        [OneTimeSetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            TestFolder = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolder");
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
            await DeleteFileAsync(TestFile.Id);
            await DeleteFolderAsync(TestFolder.Id);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(1)]
        public void CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.CreateFolderAsync((int)GlobalFolderHelper.FolderTrash, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(2)]
        public async Task CreateFileReturnsFolderWrapper(string fileTitle)
        {
            var fileWrapper = await FilesControllerHelper.CreateFileAsync((int)GlobalFolderHelper.FolderTrash, fileTitle, default);
            Assert.AreEqual(fileWrapper.FolderId, GlobalFolderHelper.FolderMy);
        }

        [Test]
        [Category("Folder")]
        [Order(2)]
        public async Task DeleteFileFromTrash()
        {
            var Empty = await FilesControllerHelper.EmptyTrashAsync();

            List<FileOperationResult> statuses;

            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                await Task.Delay(100);
            }
            Assert.IsTrue(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
        }
    }
}
