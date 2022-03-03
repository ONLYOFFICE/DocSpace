namespace ASC.Files.Api;

public class OperationController : ApiControllerBase
{
    private readonly FileOperationWraperHelper _fileOperationWraperHelper;
    private readonly FileStorageService<string> _fileStorageServiceString;

    public OperationController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        FileOperationWraperHelper fileOperationWraperHelper,
        FileStorageService<string> fileStorageServiceString) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _fileOperationWraperHelper = fileOperationWraperHelper;
        _fileStorageServiceString = fileStorageServiceString;
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
    [Update("fileops/bulkdownload")]
    public Task<IEnumerable<FileOperationWraper>> BulkDownload([FromBody] DownloadModel model)
    {
        return _filesControllerHelperString.BulkDownloadAsync(model);
    }

    [Update("fileops/bulkdownload")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileOperationWraper>> BulkDownloadFromForm([FromForm] DownloadModel model)
    {
        return _filesControllerHelperString.BulkDownloadAsync(model);
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
    [Update("fileops/copy")]
    public Task<IEnumerable<FileOperationWraper>> CopyBatchItemsFromBody([FromBody] BatchModel batchModel)
    {
        return _filesControllerHelperString.CopyBatchItemsAsync(batchModel);
    }

    [Update("fileops/copy")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileOperationWraper>> CopyBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
    {
        return _filesControllerHelperString.CopyBatchItemsAsync(batchModel);
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
    [Update("fileops/delete")]
    public async IAsyncEnumerable<FileOperationWraper> DeleteBatchItemsFromBody([FromBody] DeleteBatchModel batch)
    {
        var tasks = _fileStorageServiceString.DeleteItems("delete", batch.FileIds.ToList(), batch.FolderIds.ToList(), false, batch.DeleteAfter, batch.Immediately);

        foreach (var e in tasks)
        {
            yield return await _fileOperationWraperHelper.GetAsync(e);
        }
    }

    [Update("fileops/delete")]
    [Consumes("application/x-www-form-urlencoded")]
    public async IAsyncEnumerable<FileOperationWraper> DeleteBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(DeleteBatchModelBinder))] DeleteBatchModel batch)
    {
        var tasks = _fileStorageServiceString.DeleteItems("delete", batch.FileIds.ToList(), batch.FolderIds.ToList(), false, batch.DeleteAfter, batch.Immediately);

        foreach (var e in tasks)
        {
            yield return await _fileOperationWraperHelper.GetAsync(e);
        }
    }

    /// <summary>
    ///   Deletes all files and folders from the recycle bin
    /// </summary>
    /// <short>Clear recycle bin</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [Update("fileops/emptytrash")]
    public Task<IEnumerable<FileOperationWraper>> EmptyTrashAsync()
    {
        return _filesControllerHelperInt.EmptyTrashAsync();
    }

    /// <summary>
    ///  Returns the list of all active file operations
    /// </summary>
    /// <short>Get file operations list</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [Read("fileops")]
    public async Task<IEnumerable<FileOperationWraper>> GetOperationStatuses()
    {
        var result = new List<FileOperationWraper>();

        foreach (var e in _fileStorageServiceString.GetTasksStatuses())
        {
            result.Add(await _fileOperationWraperHelper.GetAsync(e));
        }

        return result;
    }

    /// <summary>
    ///   Marks all files and folders as read
    /// </summary>
    /// <short>Mark as read</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [Update("fileops/markasread")]
    public Task<IEnumerable<FileOperationWraper>> MarkAsReadFromBody([FromBody] BaseBatchModel model)
    {
        return _filesControllerHelperString.MarkAsReadAsync(model);
    }

    [Update("fileops/markasread")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileOperationWraper>> MarkAsReadFromForm([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
    {
        return _filesControllerHelperString.MarkAsReadAsync(model);
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
    [Update("fileops/move")]
    public Task<IEnumerable<FileOperationWraper>> MoveBatchItemsFromBody([FromBody] BatchModel batchModel)
    {
        return _filesControllerHelperString.MoveBatchItemsAsync(batchModel);
    }

    [Update("fileops/move")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileOperationWraper>> MoveBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
    {
        return _filesControllerHelperString.MoveBatchItemsAsync(batchModel);
    }

    /// <summary>
    /// Checking for conflicts
    /// </summary>
    /// <category>File operations</category>
    /// <param name="destFolderId">Destination folder ID</param>
    /// <param name="folderIds">Folder ID list</param>
    /// <param name="fileIds">File ID list</param>
    /// <returns>Conflicts file ids</returns>
    [Read("fileops/move")]
    public IAsyncEnumerable<FileEntryWrapper> MoveOrCopyBatchCheckAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
    {
        return _filesControllerHelperString.MoveOrCopyBatchCheckAsync(batchModel);
    }
    /// <summary>
    ///  Finishes all the active file operations
    /// </summary>
    /// <short>Finish all</short>
    /// <category>File operations</category>
    /// <returns>Operation result</returns>
    [Update("fileops/terminate")]
    public async IAsyncEnumerable<FileOperationWraper> TerminateTasks()
    {
        var tasks = _fileStorageServiceString.TerminateTasks();

        foreach (var e in tasks)
        {
            yield return await _fileOperationWraperHelper.GetAsync(e);
        }
    }
}