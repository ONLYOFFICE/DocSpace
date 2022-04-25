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

public class MyDocuments : BaseFilesTests
{
    private FolderDto<int> TestFolder { get; set; }
    private FolderDto<int> TestFolderNotEmpty { get; set; }
    private FileDto<int> TestFile { get; set; }

    readonly JsonSerializerOptions _options;

    public MyDocuments()
    {
        _options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        _options.Converters.Add(new ApiDateTimeConverter());
        _options.Converters.Add(new FileEntryWrapperConverter());
        _options.Converters.Add(new FileShareConverter());
    }

    [OneTimeSetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFolderItems))]
    [Category("Folder")]
    [Order(5)]
    public async Task CreateFolderReturnsFolderWrapper(string title)
    {
        FileDto<int> file = null;
        var request = await _client.PostAsync("@my/file", JsonContent.Create(new { Title = title }));
        var result = await request.Content.ReadFromJsonAsync<SuccessApiResponse>();

        if (result.Response is JsonElement jsonElement)
        {
            file = jsonElement.Deserialize<FileDto<int>>(_options);
        }

        Assert.IsNotNull(file);
        Assert.AreEqual($"{title}.docx", file.Title);
    }

    [Test]
    [Category("Folder")]
    [Order(6)]
    [Description("Empty Content")]
    public async Task GetFolderEmptyReturnsFolderContentWrapper()
    {
        FolderDto<int> folder = null;
        var request = await _client.GetAsync("11");
        var result = await request.Content.ReadFromJsonAsync<SuccessApiResponse>();

        if (result.Response is JsonElement jsonElement)
        {
            folder = jsonElement.Deserialize<FolderDto<int>>(_options);
        }

        Assert.IsNotNull(folder);
        Assert.AreEqual(0, folder.FilesCount);
        Assert.AreEqual(0, folder.FoldersCount);
    }

    [Test]
    [Category("Folder")]
    [Order(7)]
    [Description("Not Empty Content")]
    public async Task GetFolderNotEmptyReturnsFolderContentWrapper()
    {
        FolderContentDto<int> folder = null;
        var request = await _client.GetAsync("12");
        var result = await request.Content.ReadFromJsonAsync<SuccessApiResponse>();

        if (result.Response is JsonElement jsonElement)
        {
            folder = jsonElement.Deserialize<FolderContentDto<int>>(_options);
        }

        Assert.IsNotNull(folder.Current);
        Assert.AreEqual(1, folder.Files.Count);
        Assert.AreEqual(1, folder.Folders.Count);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFolderInfoItems))]
    [Category("Folder")]
    [Order(8)]
    public async Task GetFolderInfoReturnsFolderWrapper(string folderTitleExpected)
    {
        var folderWrapper = await _foldersControllerHelper.GetFolderInfoAsync(TestFolder.Id);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitleExpected, folderWrapper.Title);
        Assert.AreEqual(TestFolder.Id, folderWrapper.Id);
        Assert.AreEqual(_globalFolderHelper.FolderMy, folderWrapper.ParentId);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetRenameFolderItems))]
    [Category("Folder")]
    [Order(9)]
    public async Task RenameFolderReturnsFolderWrapper(string folderTitle)
    {
        var folderWrapper = await _foldersControllerHelper.RenameFolderAsync(TestFolder.Id, folderTitle);

        Assert.IsNotNull(folderWrapper);
        Assert.AreEqual(folderTitle, folderWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetDeleteFolderItems))]
    [Category("Folder")]
    [Order(10)]
    public async Task DeleteFolderReturnsFolderWrapper(bool deleteAfter, bool immediately)
    {
        await _foldersControllerHelper.DeleteFolder(TestFolder.Id, deleteAfter, immediately);
        while (true)
        {
            var statuses = _fileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
        Assert.IsTrue(_fileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetCreateFileItems))]
    [Category("File")]
    [Order(1)]
    public async Task CreateFileReturnsFileWrapper(string fileTitle)
    {
        var fileWrapper = await _filesControllerHelper.CreateFileAsync(_globalFolderHelper.FolderMy, fileTitle, default, default);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitle + ".docx", fileWrapper.Title);
        await DeleteFileAsync(fileWrapper.Id);

    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetFileInfoItems))]
    [Category("File")]
    [Order(2)]
    public async Task GetFileInfoReturnsFilesWrapper(string fileTitleExpected)
    {
        var fileWrapper = await _filesControllerHelper.GetFileInfoAsync(TestFile.Id);

        Assert.IsNotNull(fileWrapper);
        Assert.AreEqual(fileTitleExpected + ".docx", fileWrapper.Title);
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetUpdateFileItems))]
    [Category("File")]
    [Order(3)]
    public async Task UpdateFileReturnsFileWrapper(string fileTitle, int lastVersion)
    {
        var fileWrapper = await _filesControllerHelper.UpdateFileAsync(
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
        await _filesControllerHelper.DeleteFileAsync(
            TestFile.Id,
            deleteAfter,
            immediately);

        while (true)
        {
            var statuses = _fileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
        Assert.IsTrue(_fileStorageService.GetTasksStatuses().TrueForAll(r => string.IsNullOrEmpty(r.Error)));
    }

    [TestCaseSource(typeof(DocumentData), nameof(DocumentData.GetMoveBatchItems))]
    [Category("BatchItems")]
    public async Task MoveBatchItemsReturnsOperationMove(string json)
    {
        var batchModel = GetBatchModel(json);

        var statuses = await _operationControllerHelper.MoveBatchItemsAsync(batchModel);

        FileOperationDto status = null;
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

        var statuses = await _operationControllerHelper.CopyBatchItemsAsync(batchModel);
        FileOperationDto status = null;
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
