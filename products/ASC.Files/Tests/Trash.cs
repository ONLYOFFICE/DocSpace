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
class Trash : BaseFilesTests
{
    private FolderDto<int> TestFolder { get; set; }
    public FileDto<int> TestFile { get; private set; }

    [OneTimeSetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        TestFolder = await _foldersControllerHelper.CreateFolderAsync(_globalFolderHelper.FolderMy, "TestFolder");
        TestFile = await _filesControllerHelper.CreateFileAsync(_globalFolderHelper.FolderMy, "TestFile", default, default);

    }

    [OneTimeSetUp]
    public void Authenticate()
    {
        _securityContext.AuthenticateMe(_currentTenant.OwnerId);
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
        var folderWrapper = Assert.ThrowsAsync<InvalidOperationException>(async () => await _foldersControllerHelper.CreateFolderAsync((int)_globalFolderHelper.FolderTrash, folderTitle));
        Assert.That(folderWrapper.Message == "You don't have enough permission to create");
    }
    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
    [Category("File")]
    [Order(2)]
    public async Task CreateFileReturnsFolderWrapper(string fileTitle)
    {
        var fileWrapper = await _filesControllerHelper.CreateFileAsync((int)_globalFolderHelper.FolderTrash, fileTitle, default, default);
        Assert.AreEqual(fileWrapper.FolderId, _globalFolderHelper.FolderMy);
    }

    [Test]
    [Category("Folder")]
    [Order(2)]
    public async Task DeleteFileFromTrash()
    {
        var Empty = await _operationControllerHelper.EmptyTrashAsync();

        List<FileOperationResult> statuses;

        while (true)
        {
            statuses = _fileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
        Assert.IsTrue(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
    }
}
