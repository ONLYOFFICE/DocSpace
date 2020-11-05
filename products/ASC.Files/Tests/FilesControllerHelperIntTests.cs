using ASC.Api.Documents;
using ASC.Files.Core;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Services.WCFService.FileOperations;
using NUnit.Framework;
using System;

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

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("section 'My Documents'")]
        public void CreateFolderReturnsFolderWrapperTest(string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderMy, folderTitle);
            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("section 'Shared Documents'")]
        public void CreateSharedFolderReturnsFolderWrapperTest(string folderTitle)
        {
            const string exception = "You don't have enough permission to create";
            try
            {
                var folderWrapper = FilesControllerHelper.CreateFolder(GlobalFolderHelper.FolderShare, folderTitle);
                Assert.Fail(exception, folderWrapper.Title);
            }
            catch(Exception ex){ Assert.AreEqual(exception, ex.Message); }
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderItems))]
        [Category("section 'My Documents'")]
        public void GetFolderReturnsFolderContentWrapperTest(int folderId, bool withSubFolders,int filesCountExpected,int foldersCountExpected)
        {
            var folderContentWrapper = FilesControllerHelper.GetFolder(
                 folderId,
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
        public void GetFolderInfoReturnsFolderWrapperTest(int folderId, string folderTitleExpected)
        {
            var folderWrapper = FilesControllerHelper.GetFolderInfo(folderId);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("section 'My Documents'")]
        public void RenameFolderReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            var folderWrapper = FilesControllerHelper.RenameFolder(folderId, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("section 'Shared Documents'")]
        public void RenameSharedFolderReturnsFolderWrapperTest(int folderId, string folderTitle)
        {
            const string exception = "You don't have enough permission to rename the folder";
            try
            {
                var folderWrapper = FilesControllerHelper.RenameFolder(folderId, folderTitle);
                Assert.Fail(exception, folderWrapper.Title);
            }
            catch(Exception ex)
            {
                Assert.AreEqual(exception, ex.Message);
            }
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("section 'My Documents'")]
        public void DeleteFolderTest(int folderId, bool deleteAfter, bool immediately)
        {
            var statuses = FilesControllerHelper.DeleteFolder(
                folderId,
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

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("section 'Shared Documents'")]
        public void DeleteSharedFolderTest(int folderId, bool deleteAfter, bool immediately)
        {
            
                var statuses = FilesControllerHelper.DeleteFolder(
                    folderId,
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
            var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderMy, fileTitle);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("section 'Shared Documents'")]
        public void CreateSharedFileReturnsFileWrapperTest(string fileTitle)
        {
            const string exception = "You don't have enough permission to create";
            try
            {
                var fileWrapper = FilesControllerHelper.CreateFile(GlobalFolderHelper.FolderShare, fileTitle);
                Assert.Fail(exception, fileWrapper.Title);
            }
            catch (Exception ex) { Assert.AreEqual(exception, ex.Message); }

        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
        [Category("section 'My Documents'")]
        public void GetFileInfoReturnsFilesWrapperTest(int fileId, string fileTitleExpected)
        {
            var fileWrapper = FilesControllerHelper.GetFileInfo(fileId);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected, fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("section 'My Documents'")]
        public void UpdateFileReturnsFileWrapperTest(int fileId, string fileTitle, int lastVersion)
        {
            var fileWrapper = FilesControllerHelper.UpdateFile(
                fileId,
                fileTitle,
                lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("section 'My Documents'")]
        public void DeleteFileTest(int fileId, bool deleteAfter, bool immediately)
        {
            var statuses = FilesControllerHelper.DeleteFile(
                fileId,
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
