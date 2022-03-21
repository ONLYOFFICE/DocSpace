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

namespace ASC.Files.Helpers;

public class OperationControllerHelper<T> : FilesHelperBase<T>
{
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;

    public OperationControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        FileOperationDtoHelper fileOperationDtoHelper) 
        : base(
            filesSettingsHelper,
            fileUploader,
            socketManager,
            fileDtoHelper,
            apiContext,
            fileStorageService,
            folderContentDtoHelper,
            httpContextAccessor,
            folderDtoHelper)
    {
        _fileOperationDtoHelper = fileOperationDtoHelper;
    }

    public async Task<IEnumerable<FileOperationDto>> BulkDownloadAsync(DownloadRequestDto model)
    {
        var folders = new Dictionary<JsonElement, string>();
        var files = new Dictionary<JsonElement, string>();

        foreach (var fileId in model.FileConvertIds.Where(fileId => !files.ContainsKey(fileId.Key)))
        {
            files.Add(fileId.Key, fileId.Value);
        }

        foreach (var fileId in model.FileIds.Where(fileId => !files.ContainsKey(fileId)))
        {
            files.Add(fileId, string.Empty);
        }

        foreach (var folderId in model.FolderIds.Where(folderId => !folders.ContainsKey(folderId)))
        {
            folders.Add(folderId, string.Empty);
        }

        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.BulkDownload(folders, files))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> CopyBatchItemsAsync(BatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList(), batchRequestDto.DestFolderId, batchRequestDto.ConflictResolveType, true, batchRequestDto.DeleteAfter))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> EmptyTrashAsync()
    {
        var emptyTrash = await _fileStorageService.EmptyTrashAsync();
        var result = new List<FileOperationDto>();

        foreach (var e in emptyTrash)
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> MarkAsReadAsync(BaseBatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MarkAsRead(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList()))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> MoveBatchItemsAsync(BatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList(), batchRequestDto.DestFolderId, batchRequestDto.ConflictResolveType, false, batchRequestDto.DeleteAfter))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async IAsyncEnumerable<FileEntryDto> MoveOrCopyBatchCheckAsync(BatchRequestDto batchRequestDto)
    {
        List<object> checkedFiles;
        List<object> checkedFolders;

        if (batchRequestDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            (checkedFiles, checkedFolders) = await _fileStorageService.MoveOrCopyFilesCheckAsync(batchRequestDto.FileIds.ToList(), batchRequestDto.FolderIds.ToList(), batchRequestDto.DestFolderId.GetInt32());
        }
        else
        {
            (checkedFiles, checkedFolders) = await _fileStorageService.MoveOrCopyFilesCheckAsync(batchRequestDto.FileIds.ToList(), batchRequestDto.FolderIds.ToList(), batchRequestDto.DestFolderId.GetString());
        }

        var entries = await _fileStorageService.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");

        entries.AddRange(await _fileStorageService.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

        foreach (var e in entries)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }
}