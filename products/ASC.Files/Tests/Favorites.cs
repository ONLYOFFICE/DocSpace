using System;
using System.Collections.Generic;
using System.Text;

using ASC.Api.Documents;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class Favorites : BaseFilesTests
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
            DeleteFolder(TestFolder.Id, false, true);
            DeleteFile(TestFile.Id, false, true);
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
        public void CreateFileReturnsFolderWrapper(string folderTitle)
        {
            var folderWrapper = Assert.Throws<InvalidOperationException>(() => FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderFavorites, folderTitle, default));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        [Test]
        [Category("Folder")]
        [Order(2)]
        public void GetFavoriteFolderToFolderWrapper()
        {
        }
    }
}
