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
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly FileStorageService<int> _fileStorageService;

    public OperationController(
        FileOperationDtoHelper fileOperationDtoHelper,
        FileStorageService<string> fileStorageServiceString,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        FileStorageService<int> fileStorageService) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _fileStorageServiceString = fileStorageServiceString;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Start downlaod process of files and folders with ID
    /// </summary>
    /// <short>Finish file operations</short>
    /// <param name="fileConvertIds" visible="false">File ID list for download with convert to format</param>
    /// <param name="fileIds">File ID list</param>
    /// <param name="folderIds">Folder ID list</param>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
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

        foreach (var e in _fileStorageServiceString.BulkDownload(folders, files))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///   Copies all the selected files and folders to the folder with the ID specified in the request
    /// </summary>
    /// <short>Copy to folder</short>
    /// <category>File operations</category>
    /// <param name="destFolderId">Destination folder ID</param>
    /// <param name="folderIds">Folder ID list</param>
    /// <param name="fileIds">File ID list</param>
    /// <param name="conflictResolveType">Overwriting behavior: skip(0), overwrite(1) or duplicate(2)</param>
    /// <param name="deleteAfter">Delete after finished</param>
    /// <returns>Operation result</returns>
    [HttpPut("fileops/copy")]
    public async IAsyncEnumerable<FileOperationDto> CopyBatchItems(BatchRequestDto inDto)
    {
        foreach (var e in _fileStorageServiceString.MoveOrCopyItems(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), inDto.DestFolderId, inDto.ConflictResolveType, true, inDto.DeleteAfter))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///   Deletes the files and folders with the IDs specified in the request
    /// </summary>
    /// <param name="folderIds">Folder ID list</param>
    /// <param name="fileIds">File ID list</param>
    /// <param name="deleteAfter">Delete after finished</param>
    /// <param name="immediately">Don't move to the Recycle Bin</param>
    /// <short>Delete files and folders</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [HttpPut("fileops/delete")]
    public async IAsyncEnumerable<FileOperationDto> DeleteBatchItems(DeleteBatchRequestDto inDto)
    {
        var tasks = _fileStorageServiceString.DeleteItems("delete", inDto.FileIds.ToList(), inDto.FolderIds.ToList(), false, inDto.DeleteAfter, inDto.Immediately);

        foreach (var e in tasks)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///   Deletes all files and folders from the recycle bin
    /// </summary>
    /// <short>Clear recycle bin</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
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
    ///  Returns the list of all active file operations
    /// </summary>
    /// <short>Get file operations list</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [HttpGet("fileops")]
    public async IAsyncEnumerable<FileOperationDto> GetOperationStatuses()
    {
        foreach (var e in _fileStorageServiceString.GetTasksStatuses())
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///   Marks all files and folders as read
    /// </summary>
    /// <short>Mark as read</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [HttpPut("fileops/markasread")]
    public async IAsyncEnumerable<FileOperationDto> MarkAsRead(BaseBatchRequestDto inDto)
    {
        foreach (var e in _fileStorageServiceString.MarkAsRead(inDto.FolderIds.ToList(), inDto.FileIds.ToList()))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///   Moves all the selected files and folders to the folder with the ID specified in the request
    /// </summary>
    /// <short>Move to folder</short>
    /// <category>File operations</category>
    /// <param name="destFolderId">Destination folder ID</param>
    /// <param name="folderIds">Folder ID list</param>
    /// <param name="fileIds">File ID list</param>
    /// <param name="conflictResolveType">Overwriting behavior: skip(0), overwrite(1) or duplicate(2)</param>
    /// <param name="deleteAfter">Delete after finished</param>
    /// <returns>Operation result</returns>
    [HttpPut("fileops/move")]
    public async IAsyncEnumerable<FileOperationDto> MoveBatchItems(BatchRequestDto inDto)
    {
        foreach (var e in _fileStorageServiceString.MoveOrCopyItems(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), inDto.DestFolderId, inDto.ConflictResolveType, false, inDto.DeleteAfter))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }

    }

    /// <summary>
    /// Checking for conflicts
    /// </summary>
    /// <category>File operations</category>
    /// <param name="destFolderId">Destination folder ID</param>
    /// <param name="folderIds">Folder ID list</param>
    /// <param name="fileIds">File ID list</param>
    /// <returns>Conflicts file ids</returns>
    [HttpGet("fileops/move")]
    public async IAsyncEnumerable<FileEntryDto> MoveOrCopyBatchCheckAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRequestDto inDto)
    {
        await foreach (var e in MoveOrCopyBatchCheckFullAsync(inDto))
        {
            yield return e.EntryFrom;
        }
    }

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
            ((checkedFiles, conflictFiles), (checkedFolders, conflictFolders)) = await _fileStorageServiceString.MoveOrCopyFilesCheckAsync(inDto.FileIds.ToList(), inDto.FolderIds.ToList(), inDto.DestFolderId.GetInt32());
        }
        else
        {
            ((checkedFiles, conflictFiles), (checkedFolders, conflictFolders)) = await _fileStorageServiceString.MoveOrCopyFilesCheckAsync(inDto.FileIds.ToList(), inDto.FolderIds.ToList(), inDto.DestFolderId.GetString());
        }

        var entries = await _fileStorageServiceString.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");
        entries.AddRange(await _fileStorageServiceString.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));


        var conflictEntries = await _fileStorageServiceString.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");
        conflictEntries.AddRange(await _fileStorageServiceString.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var conflictEntry = conflictEntries[i];
            yield return new MoveDto() { EntryFrom = await GetFileEntryWrapperAsync(entry), EntryTo = await GetFileEntryWrapperAsync(conflictEntry) };
        }
    }

    /// <summary>
    ///  Finishes all the active file operations
    /// </summary>
    /// <short>Finish all</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [HttpPut("fileops/terminate")]
    public async IAsyncEnumerable<FileOperationDto> TerminateTasks()
    {
        var tasks = _fileStorageServiceString.TerminateTasks();

        foreach (var e in tasks)
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }
}