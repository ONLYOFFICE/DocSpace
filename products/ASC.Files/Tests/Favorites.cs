using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ASC.Api.Documents;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class Favorites : BaseFilesTests
    {
        private FolderWrapper<int> TestFolder { get; set; }
        public FileWrapper<int> TestFile { get; private set; }

        public IEnumerable<int> folderIds;
        public IEnumerable<int> fileIds;

        [OneTimeSetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            TestFolder = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolder").ConfigureAwait(false);
            TestFile = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFile", default).ConfigureAwait(false);
            folderIds = new List<int> { TestFolder.Id };
            fileIds = new List<int> { TestFile.Id };
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
        [Order(1)]
        public void CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderFavoritesAsync, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(1)]
        public async Task CreateFileReturnsFolderWrapper(string fileTitle)
        {
            var fileWrapper = await FilesControllerHelper.CreateFileAsync(await GlobalFolderHelper.FolderShareAsync, fileTitle, default);
            Assert.AreEqual(fileWrapper.FolderId, GlobalFolderHelper.FolderMy);
        }

        [Test]
        [Category("Favorite")]
        [Order(2)]
        public async Task GetFavoriteFolderToFolderWrapper()
        {
            var favorite = await FileStorageService.AddToFavoritesAsync(folderIds, fileIds);

            Assert.IsNotNull(favorite);
        }
        [Test]
        [Category("Favorite")]
        [Order(3)]
        public async Task DeleteFavoriteFolderToFolderWrapper()
        {
            var favorite = await FileStorageService.DeleteFavoritesAsync(folderIds, fileIds);

            Assert.IsNotNull(favorite);

        }


    }
}
