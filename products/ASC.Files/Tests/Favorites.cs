using System;
using System.Collections.Generic;
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
        public override void SetUp()
        {
            base.SetUp();
            TestFolder = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, "TestFolder");
            TestFile = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, "TestFile", default);
            folderIds = new List<int> { TestFolder.Id };
            fileIds = new List<int> { TestFile.Id };
        }
        [OneTimeTearDown]
        public override void TearDown()
        {
            DeleteFolder(TestFolder.Id, false, true);
            DeleteFile(TestFile.Id, false, true);
            base.TearDown();
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder")]
        [Order(1)]
        public void CreateFolderReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderFavorites, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File")]
        [Order(1)]
        public void CreateFileReturnsFolderWrapper(string fileTitle)
        {
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle, default);
            Assert.AreEqual(fileWrapper.FolderId, GlobalFolderHelper.FolderMy);
        }

        [Test]
        [Category("Favorite")]
        [Order(2)]
        public void GetFavoriteFolderToFolderWrapper()
        {
            var favorite = FileStorageService.AddToFavorites(folderIds, fileIds);

            Assert.IsNotNull(favorite);
        }
        [Test]
        [Category("Favorite")]
        [Order(3)]
        public void DeleteFavoriteFolderToFolderWrapper()
        {
            var favorite = FileStorageService.DeleteFavorites(folderIds, fileIds);

            Assert.IsNotNull(favorite);

        }
        

}
}
