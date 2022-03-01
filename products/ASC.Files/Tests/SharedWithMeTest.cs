using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Api.Documents;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core.Resources;
using ASC.Web.Api.Models;
using ASC.Web.Files.Services.WCFService.FileOperations;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class SharedWithMeTest : BaseFilesTests
    {
        private FolderWrapper<int> TestFolderRead { get; set; }
        public FileWrapper<int> TestFileRead { get; private set; }
        private FolderWrapper<int> TestFolderReadAndWrite { get; set; }
        public FileWrapper<int> TestFileReadAndWrite { get; private set; }
        public IEnumerable<FileShareParams> TestFolderParamRead { get; private set; }
        public IEnumerable<FileShareParams> TestFolderParamReadAndWrite { get; private set; }
        public UserInfo NewUser { get; set; }
        public TenantManager tenantManager { get; private set; }
        public EmployeeWraperFull TestUser { get; private set; }

        [OneTimeSetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            TestFolderRead = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolderRead");
            TestFileRead = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFileRead", default);
            TestFolderReadAndWrite = await FilesControllerHelper.CreateFolderAsync(GlobalFolderHelper.FolderMy, "TestFolderReadAndWrite");
            TestFileReadAndWrite = await FilesControllerHelper.CreateFileAsync(GlobalFolderHelper.FolderMy, "TestFileReadAndWrite", default);
            NewUser = UserManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
            TestFolderParamRead = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = NewUser.ID } };
            TestFolderParamReadAndWrite = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.ReadWrite, ShareTo = NewUser.ID } };
        }

        [OneTimeSetUp]
        public void Authenticate()
        {
            SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await DeleteFolderAsync(TestFolderRead.Id);
            await DeleteFolderAsync(TestFolderReadAndWrite.Id);
            await DeleteFileAsync(TestFileRead.Id);
            await DeleteFileAsync(TestFileReadAndWrite.Id);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
        [Category("Folder Read")]
        [Order(1)]
        public void CreateSharedFolderReturnsFolderWrapperRead(string folderTitle)
        {
            var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.CreateFolderAsync(await GlobalFolderHelper.FolderShareAsync, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to create");
        }

        #region Shared Folder and File (Read)

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
        [Category("Folder Read")]
        [Order(2)]
        public async Task ShareFolderToAnotherUserRead(bool notify, string message)
        {
            var shareFolder = await FilesControllerHelper.SetFolderSecurityInfoAsync(TestFolderRead.Id, TestFolderParamRead, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedFolderInfoRead))]
        [Category("Folder Read")]
        [Order(3)]
        public async Task GetSharedFolderInfoReturnsFolderWrapperRead(string folderTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = await FilesControllerHelper.GetFolderInfoAsync(TestFolderRead.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
            Assert.AreEqual(TestFolderRead.Id, folderWrapper.Id);
            Assert.AreEqual(await GlobalFolderHelper.FolderShareAsync, folderWrapper.ParentId);
        }
        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder Read")]
        [Order(4)]
        public void RenameSharedFolderReturnsFolderWrapperRead(string folderTitle)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.RenameFolderAsync(TestFolderRead.Id, folderTitle));
            Assert.That(folderWrapper.Message == "You don't have enough permission to rename the folder");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder Read")]
        [Order(5)]
        public async Task DeleteSharedFolderReturnsFolderWrapperRead(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);

            var result = (await FilesControllerHelper.DeleteFolder(
                TestFolderRead.Id,
                deleteAfter,
                immediately))
                .FirstOrDefault();

            await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
        [Category("File Read")]
        [Order(6)]
        public async Task CreateSharedFileReturnsFolderWrapperRead(string fileTitle)
        {
            var fileWrapper = await FilesControllerHelper.CreateFileAsync(await GlobalFolderHelper.FolderShareAsync, fileTitle, default);
            Assert.AreEqual(fileWrapper.FolderId, GlobalFolderHelper.FolderMy);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFileRead))]
        [Category("File Read")]
        [Order(7)]
        public async Task ShareFileToAnotherUserRead(bool notify, string message)
        {
            var shareFolder = await FilesControllerHelper.SetFileSecurityInfoAsync(TestFileRead.Id, TestFolderParamRead, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfo))]
        [Category("File Read")]
        [Order(8)]
        public async Task GetSharedFileInfoReturnsFolderWrapperRead(string fileTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = await FilesControllerHelper.GetFileInfoAsync(TestFileRead.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);

        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File Read")]
        [Order(9)]
        public void UpdateSharedFileReturnsFolderWrapperRead(string fileTitle, int lastVersion)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await FilesControllerHelper.UpdateFileAsync(TestFileRead.Id, fileTitle, lastVersion));
            Assert.That(fileWrapper.Message == "You don't have enough permission to rename the file");
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File Read")]
        [Order(10)]
        public async Task DeleteSharedFileReturnsFolderWrapperRead(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var result = (await FilesControllerHelper.DeleteFileAsync(
                TestFileRead.Id,
                deleteAfter,
                immediately))
                .FirstOrDefault();

            await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFile);
        }
        #endregion

        #region Shared Folder and File (Read and Write)

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFolder))]
        [Category("Folder Read and Write")]
        [Order(11)]
        public async Task ShareFolderToAnotherUserReadAndWrite(bool notify, string message)
        {
            var shareFolder = await FilesControllerHelper.SetFolderSecurityInfoAsync(TestFolderReadAndWrite.Id, TestFolderParamReadAndWrite, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedFolderInfoReadAndWrite))]
        [Category("Folder Read and Write")]
        [Order(12)]
        public async Task GetSharedFolderInfoReturnsFolderWrapperReadAndWrite(string folderTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = await FilesControllerHelper.GetFolderInfoAsync(TestFolderReadAndWrite.Id);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
            Assert.AreEqual(TestFolderReadAndWrite.Id, folderWrapper.Id);
            Assert.AreEqual(await GlobalFolderHelper.FolderShareAsync, folderWrapper.ParentId);
        }


        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
        [Category("Folder Read and Write")]
        [Order(13)]
        public async Task RenameSharedFolderReturnsFolderWrapperReadAndWrite(string folderTitle)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var folderWrapper = await FilesControllerHelper.RenameFolderAsync(TestFolderReadAndWrite.Id, folderTitle);

            Assert.IsNotNull(folderWrapper);
            Assert.AreEqual(folderTitle, folderWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
        [Category("Folder Read and Write")]
        [Order(14)]
        public async Task DeleteSharedFolderReturnsFolderWrapperReadAndWrite(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);

            var result = (await FilesControllerHelper.DeleteFolder(
                TestFolderReadAndWrite.Id,
                deleteAfter,
                immediately))
                .FirstOrDefault();

            await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.ShareParamToFile))]
        [Category("File Read and Write")]
        [Order(15)]
        public async Task ShareFileToAnotherUserReadAndWrite(bool notify, string message)
        {
            var shareFolder = await FilesControllerHelper.SetFileSecurityInfoAsync(TestFileReadAndWrite.Id, TestFolderParamReadAndWrite, notify, message);
            Assert.IsNotNull(shareFolder);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetSharedInfoReadAndWrite))]
        [Category("File Read and Write")]
        [Order(16)]
        public async Task GetSharedFileInfoReturnsFolderWrapperReadAndWrite(string fileTitleExpected)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = await FilesControllerHelper.GetFileInfoAsync(TestFileReadAndWrite.Id);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
        [Category("File Read and Write")]
        [Order(17)]
        public async Task UpdateSharedFileReturnsFolderWrapperReadAndWrite(string fileTitle, int lastVersion)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var fileWrapper = await FilesControllerHelper.UpdateFileAsync(TestFileReadAndWrite.Id, fileTitle, lastVersion);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFileItems))]
        [Category("File Read and Write")]
        [Order(18)]
        public async Task DeleteSharedFileReturnsFolderWrapperReadAndWrite(bool deleteAfter, bool immediately)
        {
            SecurityContext.AuthenticateMe(NewUser.ID);
            var result = (await FilesControllerHelper.DeleteFileAsync(
                TestFileReadAndWrite.Id,
                deleteAfter,
                immediately))
                .FirstOrDefault();

            await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFile);
        }
        #endregion

        private async Task WaitLongOperation(FileOperationWraper result, string assertError)
        {
            if (result != null && result.Finished)
            {
                Assert.That(result.Error == assertError, result.Error);
                return;
            }

            List<FileOperationResult> statuses;
            while (true)
            {
                statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                {
                    break;
                }

                await Task.Delay(100);
            }

            var error = string.Join(",", statuses.Select(r => r.Error));
            Assert.That(error == assertError, error);
        }
    }
}
