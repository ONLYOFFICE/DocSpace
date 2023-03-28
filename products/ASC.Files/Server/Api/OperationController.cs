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
    /// Starts the download process of files and folders with the IDs specified in the request.
    /// </summary>
    /// <short>Bulk download</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DownloadRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for downloading files: <![CDATA[
    /// <ul>
    ///     <li><b>FileConvertIds</b> (IEnumerable&lt;ItemKeyValuePair&lt;JsonElement, string&gt;&gt;) - list of file IDs which will be converted,</li>
    ///     <li><b>FileIds</b> (IEnumerable&lt;JsonElement&gt;) - list of file IDs,</li>
    ///     <li><b>FolderIds</b> (IEnumerable&lt;JsonElement&gt;) - list of folder IDs.</li>
    /// </ul>
    /// ]]></param>
    /// <category>Operations</category>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/bulkdownload</path>
    /// <httpMethod>PUT</httpMethod>
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
    /// Copies all the selected files and folders to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Copy to a folder</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for copying files: <![CDATA[
    /// <ul>
    ///     <li><b>DestFolderId</b> (JsonElement) - destination folder ID,</li>
    ///     <li><b>ConflictResolveType</b> (FileConflictResolveType) - overwriting behavior: Skip (0), Overwrite (1), or Duplicate (2),</li>
    ///     <li><b>DeleteAfter</b> (bool) - specifies whether to delete a folder after the editing session is finished or not,</li>
    ///     <li><b>FileIds</b> (IEnumerable&lt;JsonElement&gt;) - list of file IDs,</li>
    ///     <li><b>FolderIds</b> (IEnumerable&lt;JsonElement&gt;) - list of folder IDs.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/copy</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("fileops/copy")]
    public async IAsyncEnumerable<FileOperationDto> CopyBatchItems(BatchRequestDto inDto)
    {
        foreach (var e in _fileStorageServiceString.MoveOrCopyItems(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), inDto.DestFolderId, inDto.ConflictResolveType, true, inDto.DeleteAfter))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Deletes the files and folders with the IDs specified in the request.
    /// </summary>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteBatchRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for deleting files: <![CDATA[
    /// <ul>
    ///     <li><b>DeleteAfter</b> (bool) - specifies whether to delete a file after the editing session is finished or not,</li>
    ///     <li><b>Immediately</b> (bool) - specifies whether to move a file to the "Trash" folder or delete it immediately,</li>
    ///     <li><b>FileIds</b> (IEnumerable&lt;JsonElement&gt;) - list of file IDs,</li>
    ///     <li><b>FolderIds</b> (IEnumerable&lt;JsonElement&gt;) - list of folder IDs.</li>
    /// </ul>
    /// ]]></param>
    /// <short>Delete files and folders</short>
    /// <category>Operations</category>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/delete</path>
    /// <httpMethod>PUT</httpMethod>
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
    /// Deletes all the files and folders from the "Trash" folder.
    /// </summary>
    /// <short>Empty the "Trash" folder</short>
    /// <category>Operations</category>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/emptytrash</path>
    /// <httpMethod>PUT</httpMethod>
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
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("fileops")]
    public async IAsyncEnumerable<FileOperationDto> GetOperationStatuses()
    {
        foreach (var e in _fileStorageServiceString.GetTasksStatuses())
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Marks the files and folders with the IDs specified in the request as read.
    /// </summary>
    /// <short>Mark as read</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Base batch request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>FileIds</b> (IEnumerable&lt;JsonElement&gt;) - list of file IDs,</li>
    ///     <li><b>FolderIds</b> (IEnumerable&lt;JsonElement&gt;) - list of folder IDs.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/markasread</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("fileops/markasread")]
    public async IAsyncEnumerable<FileOperationDto> MarkAsRead(BaseBatchRequestDto inDto)
    {
        foreach (var e in _fileStorageServiceString.MarkAsRead(inDto.FolderIds.ToList(), inDto.FileIds.ToList()))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Moves all the selected files and folders to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Move to a folder</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for moving files and folders: <![CDATA[
    /// <ul>
    ///     <li><b>DestFolderId</b> (JsonElement) - destination folder ID,</li>
    ///     <li><b>ConflictResolveType</b> (FileConflictResolveType) - overwriting behavior: Skip (0), Overwrite (1), or Duplicate (2),</li>
    ///     <li><b>DeleteAfter</b> (bool) - specifies whether to delete a folder after the editing session is finished or not,</li>
    ///     <li><b>FileIds</b> (IEnumerable&lt;JsonElement&gt;) - list of file IDs,</li>
    ///     <li><b>FolderIds</b> (IEnumerable&lt;JsonElement&gt;) - list of folder IDs.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/move</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("fileops/move")]
    public async IAsyncEnumerable<FileOperationDto> MoveBatchItems(BatchRequestDto inDto)
    {
        foreach (var e in _fileStorageServiceString.MoveOrCopyItems(inDto.FolderIds.ToList(), inDto.FileIds.ToList(), inDto.DestFolderId, inDto.ConflictResolveType, false, inDto.DeleteAfter))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }

    }

    /// <summary>
    /// Checks a batch of files and folders for conflicts when moving or copying them to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Check files and folders for conflicts</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for checking files and folders for conflicts: <![CDATA[
    /// <ul>
    ///     <li><b>DestFolderId</b> (JsonElement) - destination folder ID,</li>
    ///     <li><b>FileIds</b> (IEnumerable&lt;JsonElement&gt;) - list of file IDs,</li>
    ///     <li><b>FolderIds</b> (IEnumerable&lt;JsonElement&gt;) - list of folder IDs.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of file entry information: title, access rights, shared or not, creation time, author, time of the last file update, root folder type, a user who updated a file, provider is specified or not, provider key, provider ID</returns>
    /// <path>api/2.0/files/fileops/move</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("fileops/move")]
    public async IAsyncEnumerable<FileEntryDto> MoveOrCopyBatchCheckAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchRequestDto inDto)
    {
        List<object> checkedFiles;
        List<object> checkedFolders;

        if (inDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            (checkedFiles, checkedFolders) = await _fileStorageServiceString.MoveOrCopyFilesCheckAsync(inDto.FileIds.ToList(), inDto.FolderIds.ToList(), inDto.DestFolderId.GetInt32());
        }
        else
        {
            (checkedFiles, checkedFolders) = await _fileStorageServiceString.MoveOrCopyFilesCheckAsync(inDto.FileIds.ToList(), inDto.FolderIds.ToList(), inDto.DestFolderId.GetString());
        }

        var entries = await _fileStorageServiceString.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");

        entries.AddRange(await _fileStorageServiceString.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

        foreach (var e in entries)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }
    /// <summary>
    /// Finishes all the active operations.
    /// </summary>
    /// <short>Finish active operations</short>
    /// <category>Operations</category>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/fileops/terminate</path>
    /// <httpMethod>PUT</httpMethod>
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