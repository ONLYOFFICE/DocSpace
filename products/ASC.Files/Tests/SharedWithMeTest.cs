// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Files.Tests;

[TestFixture]
public partial class BaseFilesTests
{
    private IEnumerable<FileShareParams> _testFolderParamRead;
    private IEnumerable<FileShareParams> _testFolderParamReadAndWrite;


    [OneTimeSetUp]
    public void SetUp()
    {
        var newUser = _userManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
        _testFolderParamRead = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = newUser.Id } };
        _testFolderParamReadAndWrite = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.ReadWrite, ShareTo = newUser.Id } };
    }

    #region Shared Folder and File (Read)

    [TestCase(DataTests.SubFolderId, DataTests.Notify, DataTests.Message)]
    [Category("Folder Read")]
    [Order(2)]
    [Description("put - files/folder/{folderId}/share - share folder to another user for read")]
    public async Task ShareFolderToAnotherUserRead(int folderId, bool notify, string message)
    {
        var share = await PutAsync<IEnumerable<FileShareDto>>($"folder/{folderId}/share", new { Share = _testFolderParamRead, Notify = notify, SharingMessage = message });
        Assert.IsNotNull(share);
    }

    [TestCase(DataTests.SharedForReadFolderId)]
    [Category("Folder Read")]
    [Order(4)]
    [Description("put - files/folder/{folderId} - try to update folder which can only read")]
    public async Task RenameSharedFolderReturnsFolderWrapperReadAsync(int folderId)
    {
        var result = await SendAsync(HttpMethod.Put, "folder/" + folderId, new { Title = "newName" });
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestCase(DataTests.FileId, DataTests.Notify, DataTests.Message)]
    [Category("File Read")]
    [Order(7)]
    [Description("put - file/{fileId}/share - share file to another user for read")]
    public async Task ShareFileToAnotherUserRead(int fileId, bool notify, string message)
    {
        var share = await PutAsync<IEnumerable<FileShareDto>>($"file/{fileId}/share", new { Share = _testFolderParamRead, Notify = notify, SharingMessage = message });
        Assert.IsNotNull(share);
    }

    [TestCase(DataTests.SharedForReadFileId)]
    [Category("File Read")]
    [Order(9)]
    [Description("put - files/file/{fileId} - try to update file which can only read")]
    public async Task UpdateSharedFileReturnsFolderWrapperReadAsync(int fileId)
    {
        var result = await SendAsync(HttpMethod.Put, "file/" + fileId, new { Title = "newName", LastVersion = 0 });
        Assert.That(HttpStatusCode.Forbidden == result.StatusCode);
    }

    #endregion

    #region Shared Folder and File (Read and Write)

    [TestCase(DataTests.SubFolderId, DataTests.Notify, DataTests.Message)]
    [Category("Folder Read and Write")]
    [Order(11)]
    [Description("put - files/folder/{folderId}/share - share folder to another user for read and write")]
    public async Task ShareFolderToAnotherUserReadAndWrite(int folderId, bool notify, string message)
    {
        var share = await PutAsync<IEnumerable<FileShareDto>>($"folder/{folderId}/share", new { Share = _testFolderParamReadAndWrite, Notify = notify, SharingMessage = message });
        Assert.IsNotNull(share);
    }

    [TestCase(DataTests.SharedForReadAndWriteFolderId, DataTests.NewTitle)]
    [Category("Folder Read and Write")]
    [Order(13)]
    [Description("put - files/folder/{folderId} - rename shared for read and write folder")]
    public async Task RenameSharedFolderReturnsFolderWrapperReadAndWrite(int folderId, string newTitle)
    {
        var sharedFolder = await PutAsync<FolderDto<int>>($"folder/{folderId}", new { Title = newTitle });

        Assert.IsNotNull(sharedFolder);
        Assert.AreEqual(newTitle, sharedFolder.Title);
    }

    [TestCase(DataTests.FileId, DataTests.Notify, DataTests.Message)]
    [Category("File Read and Write")]
    [Order(15)]
    [Description("put - files/file/{fileId}/share - share file to another user for read and write")]
    public async Task ShareFileToAnotherUserReadAndWrite(int fileId, bool notify, string message)
    {
        var share = await PutAsync<IEnumerable<FileShareDto>>($"file/{fileId}/share", new { Share = _testFolderParamReadAndWrite, Notify = notify, SharingMessage = message });
        Assert.IsNotNull(share);
    }

    [TestCase(DataTests.SharedForReadAndWriteFileId, DataTests.NewTitle, 0)]
    [Category("File Read and Write")]
    [Order(17)]
    [Description("put - files/file/{fileId} - update shared for read and write file")]
    public async Task UpdateSharedFileReturnsFolderWrapperReadAndWrite(int fileId, string fileTitle, int lastVersion)
    {
        var sharedFile = await PutAsync<FolderDto<int>>($"file/{fileId}", new { Title = fileTitle, LastVersion = lastVersion });

        Assert.IsNotNull(sharedFile);
        Assert.AreEqual(fileTitle + ".docx", sharedFile.Title);
    }
    #endregion

    [TestCase(DataTests.SharedForReadFolderId, DataTests.SharedForReadFolderName, DataTests.ShareId)]
    [TestCase(DataTests.SharedForReadAndWriteFolderId, DataTests.SharedForReadAndWriteFolderName, DataTests.ShareId)]
    [Category("Folder")]
    [Order(3)]
    [Description("get - files/folder/{folderId} - get shared folder")]
    public async Task GetSharedFolderInfoReturnsFolderWrapperRead(int folderId, string folderName, int parentId)
    {
        var sharedFolder = await GetAsync<FolderDto<int>>($"folder/{folderId}");

        Assert.IsNotNull(sharedFolder);
        Assert.AreEqual(folderName, sharedFolder.Title);
        Assert.AreEqual(folderId, sharedFolder.Id);
        Assert.AreEqual(parentId, sharedFolder.ParentId);
    }

    [TestCase(DataTests.SharedForReadFileId, DataTests.SharedForReadFileName)]
    [TestCase(DataTests.SharedForReadAndWriteFileId, DataTests.SharedForReadAndWriteFileName)]
    [Category("File")]
    [Order(8)]
    [Description("get - files/file/{fileId} -  get shared file")]
    public async Task GetSharedFileInfoReturnsFolderWrapperRead(int fileId, string fileName)
    {
        var sharedFile = await GetAsync<FolderDto<int>>($"file/{fileId}");

        Assert.IsNotNull(sharedFile);
        Assert.AreEqual(fileName, sharedFile.Title);
    }

    [TestCase(DataTests.SharedForReadFileId, DataTests.DeleteAfter, DataTests.Immediately)]
    [TestCase(DataTests.SharedForReadAndWriteFileId, DataTests.DeleteAfter, DataTests.Immediately)]
    [Category("File")]
    [Order(10)]
    [Description("delete - files/file/{fileId} - try delete shared file")]
    public async Task DeleteSharedFileReturnsFolderWrapperRead(int fileId, bool deleteAfter, bool immediately)
    {
        var result = (await DeleteAsync<IEnumerable<FileOperationDto>>($"file/{fileId}", new { DeleteAfter = deleteAfter, Immediately = immediately })).FirstOrDefault();

        await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFile);
    }

    [TestCase(DataTests.SharedForReadFolderId, DataTests.DeleteAfter, DataTests.Immediately)]
    [TestCase(DataTests.SharedForReadAndWriteFolderId, DataTests.DeleteAfter, DataTests.Immediately)]
    [Category("Folder")]
    [Order(50)]
    [Description("delete - files/folder/{folderId} - try delete shared folder")]
    public async Task DeleteSharedFolderReturnsFolderWrapperRead(int folderId, bool deleteAfter, bool immediately)
    {
        var result = (await DeleteAsync<IEnumerable<FileOperationDto>>($"folder/{folderId}", new { DeleteAfter = deleteAfter, Immediately = immediately })).FirstOrDefault();

        await WaitLongOperation(result, FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);
    }

    private async Task WaitLongOperation(FileOperationDto result, string assertError)
    {
        if (result != null && result.Finished)
        {
            Assert.That(result.Error == assertError, result.Error);
            return;
        }

        var statuses = await WaitLongOperation();

        var error = string.Join(",", statuses.Select(r => r.Error));
        Assert.That(error == assertError, error);
    }
}
