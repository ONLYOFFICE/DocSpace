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

namespace ASC.Files.Api;

public class OperationController : ApiControllerBase
{
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly FileStorageService _fileStorageService;

    public OperationController(
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        FileStorageService fileStorageService) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Starts the download process of files and folders with the IDs specified in the request.
    /// </summary>
    /// <short>Bulk download</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DownloadRequestDto, ASC.Files.Core" name="inDto">Request parameters for downloading files</param>
    /// <category>Operations</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/bulkdownload</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [AllowAnonymous]
    [HttpPut("fileops/bulkdownload")]
    public async IAsyncEnumerable<FileOperationDto> BulkDownload(DownloadRequestDto inDto)
    {
        var folders = new Dictionary<JsonElement, string>();
        var files = new Dictionary<JsonElement, string>();

        foreach (var fileId in inDto.FileConvertIds.Where(fileId => !files.ContainsKey(fileId.Key)))
        {
            files.Add(fileId.Key, fileId.Value);
        }

        foreach (var fileId in inDto.FileIds.Where(fileId => !files.ContainsKey(fileId)))
        {
            files.Add(fileId, string.Empty);
        }

        foreach (var folderId in inDto.FolderIds.Where(folderId => !folders.ContainsKey(folderId)))
        {
            folders.Add(folderId, string.Empty);
        }

        foreach (var e in await _fileStorageService.BulkDownloadAsync(folders, files))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Copies all the selected files and folders to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Copy to a folder</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core" name="inDto">Request parameters for copying files</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/copy</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("fileops/copy")]
    public async IAsyncEnumerable<FileOperationDto> CopyBatchItems(BatchRequestDto inDto)
    {
        foreach (var e in await _fileStorageService.MoveOrCopyItemsAsync(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), inDto.DestFolderId, inDto.ConflictResolveType, true, inDto.DeleteAfter))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Deletes the files and folders with the IDs specified in the request.
    /// </summary>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteBatchRequestDto, ASC.Files.Core" name="inDto">Request parameters for deleting files</param>
    /// <short>Delete files and folders</short>
    /// <category>Operations</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto}, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/delete</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("fileops/delete")]
    public async IAsyncEnumerable<FileOperationDto> DeleteBatchItems(DeleteBatchRequestDto inDto)
    {
        var tasks = await _fileStorageService.DeleteItemsAsync("delete", inDto.FileIds.ToList(), inDto.FolderIds.ToList(), false, inDto.DeleteAfter, inDto.Immediately);

        foreach (var e in tasks)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Deletes all the files and folders from the "Trash" folder.
    /// </summary>
    /// <short>Empty the "Trash" folder</short>
    /// <category>Operations</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/emptytrash</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("fileops/emptytrash")]
    public async IAsyncEnumerable<FileOperationDto> EmptyTrashAsync()
    {
        var emptyTrash = await _fileStorageService.EmptyTrashAsync();

        foreach (var e in emptyTrash)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///  Returns a list of all the active operations.
    /// </summary>
    /// <short>Get active operations</short>
    /// <category>Operations</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [AllowAnonymous]
    [HttpGet("fileops")]
    public async IAsyncEnumerable<FileOperationDto> GetOperationStatuses()
    {
        foreach (var e in _fileStorageService.GetTasksStatuses())
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Marks the files and folders with the IDs specified in the request as read.
    /// </summary>
    /// <short>Mark as read</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/markasread</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("fileops/markasread")]
    public async IAsyncEnumerable<FileOperationDto> MarkAsRead(BaseBatchRequestDto inDto)
    {
        foreach (var e in await _fileStorageService.MarkAsReadAsync(inDto.FolderIds.ToList(), inDto.FileIds.ToList()))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Moves all the selected files and folders to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Move to a folder</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core" name="inDto">Request parameters for moving files and folders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/move</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("fileops/move")]
    public async IAsyncEnumerable<FileOperationDto> MoveBatchItems(BatchRequestDto inDto)
    {
        foreach (var e in await _fileStorageService.MoveOrCopyItemsAsync(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), inDto.DestFolderId, inDto.ConflictResolveType, false, inDto.DeleteAfter))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }

    }

    /// <summary>
    /// Checks a batch of files and folders for conflicts when moving or copying them to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Check files and folders for conflicts</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core" name="inDto">Request parameters for checking files and folders for conflicts</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">List of file entry information</returns>
    /// <path>api/2.0/files/fileops/move</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("fileops/move")]
    public async IAsyncEnumerable<FileEntryDto> MoveOrCopyBatchCheckAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRequestDto inDto)
    {
        await foreach (var e in MoveOrCopyBatchCheckFullAsync(inDto))
        {
            yield return e.EntryFrom;
        }
    }

    /// <summary>
    /// Checks a batch of files and folders for conflicts when moving or copying them to the folder with the ID specified in the request.
    /// This method returns the entry information about both the initial and destination files.
    /// </summary>
    /// <short>Check files and folders for conflicts (full information)</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core" name="inDto">Request parameters for checking files and folders for conflicts</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.MoveDto, ASC.Files.Core">Entry information about initial and destination files</returns>
    /// <path>api/2.0/files/fileops/move/full</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpGet("fileops/move/full")]
    public async IAsyncEnumerable<MoveDto> MoveOrCopyBatchCheckFullAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRequestDto inDto)
    {
        var checkedFiles = new List<object>();
        var conflictFiles = new List<object>();
        var checkedFolders = new List<object>();
        var conflictFolders = new List<object>();

        if (inDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            ((checkedFiles, conflictFiles), (checkedFolders, conflictFolders)) = await _fileStorageService.MoveOrCopyFilesCheckAsync(inDto.FileIds.ToList(), inDto.FolderIds.ToList(), inDto.DestFolderId.GetInt32());
        }
        else
        {
            ((checkedFiles, conflictFiles), (checkedFolders, conflictFolders)) = await _fileStorageService.MoveOrCopyFilesCheckAsync(inDto.FileIds.ToList(), inDto.FolderIds.ToList(), inDto.DestFolderId.GetString());
        }

        var entries = await _fileStorageService.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");
        entries.AddRange(await _fileStorageService.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));


        var conflictEntries = await _fileStorageService.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");
        conflictEntries.AddRange(await _fileStorageService.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var conflictEntry = conflictEntries[i];
            yield return new MoveDto() { EntryFrom = await GetFileEntryWrapperAsync(entry), EntryTo = await GetFileEntryWrapperAsync(conflictEntry) };
        }
    }

    /// <summary>
    /// Finishes all the active operations.
    /// </summary>
    /// <short>Finish active operations</short>
    /// <category>Operations</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/fileops/terminate</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [AllowAnonymous]
    [HttpPut("fileops/terminate")]
    public async IAsyncEnumerable<FileOperationDto> TerminateTasks()
    {
        var tasks = _fileStorageService.TerminateTasks();

        foreach (var e in tasks)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }
}