namespace ASC.Files.Api;

public class SecutiryController : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;

    public SecutiryController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _fileStorageServiceInt = fileStorageServiceInt;
    }

    [Create("owner")]
    public IAsyncEnumerable<FileEntryDto> ChangeOwnerFromBodyAsync([FromBody] ChangeOwnerRequestDto requestDto)
    {
        return ChangeOwnerAsync(requestDto);
    }

    [Create("owner")]
    [Consumes("application/x-www-form-urlencoded")]
    public IAsyncEnumerable<FileEntryDto> ChangeOwnerFromFormAsync([FromForm] ChangeOwnerRequestDto requestDto)
    {
        return ChangeOwnerAsync(requestDto);
    }

    /// <summary>
    ///   Returns the external link to the shared file with the ID specified in the request
    /// </summary>
    /// <summary>
    ///   File external link
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="share">Access right</param>
    /// <category>Files</category>
    /// <returns>Shared file link</returns>
    [Update("{fileId}/sharedlinkAsync")]
    public async Task<object> GenerateSharedLinkFromBodyAsync(string fileId, [FromBody] GenerateSharedLinkRequestDto requestDto)
    {
        return await _filesControllerHelperString.GenerateSharedLinkAsync(fileId, requestDto.Share);
    }

    [Update("{fileId:int}/sharedlinkAsync")]
    public async Task<object> GenerateSharedLinkFromBodyAsync(int fileId, [FromBody] GenerateSharedLinkRequestDto requestDto)
    {
        return await _filesControllerHelperInt.GenerateSharedLinkAsync(fileId, requestDto.Share);
    }

    [Update("{fileId}/sharedlinkAsync")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> GenerateSharedLinkFromFormAsync(string fileId, [FromForm] GenerateSharedLinkRequestDto requestDto)
    {
        return await _filesControllerHelperString.GenerateSharedLinkAsync(fileId, requestDto.Share);
    }

    [Update("{fileId:int}/sharedlinkAsync")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> GenerateSharedLinkFromFormAsync(int fileId, [FromForm] GenerateSharedLinkRequestDto requestDto)
    {
        return await _filesControllerHelperInt.GenerateSharedLinkAsync(fileId, requestDto.Share);
    }

    /// <summary>
    /// Returns the detailed information about shared file with the ID specified in the request
    /// </summary>
    /// <short>File sharing</short>
    /// <category>Sharing</category>
    /// <param name="fileId">File ID</param>
    /// <returns>Shared file information</returns>
    [Read("file/{fileId}/share")]
    public Task<IEnumerable<FileShareDto>> GetFileSecurityInfoAsync(string fileId)
    {
        return _filesControllerHelperString.GetFileSecurityInfoAsync(fileId);
    }

    [Read("file/{fileId:int}/share")]
    public Task<IEnumerable<FileShareDto>> GetFileSecurityInfoAsync(int fileId)
    {
        return _filesControllerHelperInt.GetFileSecurityInfoAsync(fileId);
    }

    /// <summary>
    /// Returns the detailed information about shared folder with the ID specified in the request
    /// </summary>
    /// <short>Folder sharing</short>
    /// <param name="folderId">Folder ID</param>
    /// <category>Sharing</category>
    /// <returns>Shared folder information</returns>
    [Read("folder/{folderId}/share")]
    public Task<IEnumerable<FileShareDto>> GetFolderSecurityInfoAsync(string folderId)
    {
        return _filesControllerHelperString.GetFolderSecurityInfoAsync(folderId);
    }

    [Read("folder/{folderId:int}/share")]
    public Task<IEnumerable<FileShareDto>> GetFolderSecurityInfoAsync(int folderId)
    {
        return _filesControllerHelperInt.GetFolderSecurityInfoAsync(folderId);
    }

    [Create("share")]
    public async Task<IEnumerable<FileShareDto>> GetSecurityInfoFromBodyAsync([FromBody] BaseBatchRequestDto requestDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(requestDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(requestDto.FileIds);

        var result = new List<FileShareDto>();
        result.AddRange(await _filesControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds));
        result.AddRange(await _filesControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds));

        return result;
    }

    [Create("share")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<FileShareDto>> GetSecurityInfoFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto requestDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(requestDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(requestDto.FileIds);

        var result = new List<FileShareDto>();
        result.AddRange(await _filesControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds));
        result.AddRange(await _filesControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds));

        return result;
    }

    /// <summary>
    ///   Removes sharing rights for the group with the ID specified in the request
    /// </summary>
    /// <param name="folderIds">Folders ID</param>
    /// <param name="fileIds">Files ID</param>
    /// <short>Remove group sharing rights</short>
    /// <category>Sharing</category>
    /// <returns>Shared file information</returns>
    [Delete("share")]
    public async Task<bool> RemoveSecurityInfoAsync(BaseBatchRequestDto requestDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(requestDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(requestDto.FileIds);

        await _filesControllerHelperInt.RemoveSecurityInfoAsync(fileIntIds, folderIntIds);
        await _filesControllerHelperString.RemoveSecurityInfoAsync(fileStringIds, folderStringIds);

        return true;
    }

    [Update("{fileId:int}/setacelink")]
    public Task<bool> SetAceLinkAsync(int fileId, [FromBody] GenerateSharedLinkRequestDto requestDto)
    {
        return _filesControllerHelperInt.SetAceLinkAsync(fileId, requestDto.Share);
    }

    [Update("{fileId}/setacelink")]
    public Task<bool> SetAceLinkAsync(string fileId, [FromBody] GenerateSharedLinkRequestDto requestDto)
    {
        return _filesControllerHelperString.SetAceLinkAsync(fileId, requestDto.Share);
    }

    /// <summary>
    /// Sets sharing settings for the file with the ID specified in the request
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="share">Collection of sharing rights</param>
    /// <param name="notify">Should notify people</param>
    /// <param name="sharingMessage">Sharing message to send when notifying</param>
    /// <short>Share file</short>
    /// <category>Sharing</category>
    /// <remarks>
    /// Each of the FileShareParams must contain two parameters: 'ShareTo' - ID of the user with whom we want to share and 'Access' - access type which we want to grant to the user (Read, ReadWrite, etc) 
    /// </remarks>
    /// <returns>Shared file information</returns>
    [Update("file/{fileId}/share")]
    public Task<IEnumerable<FileShareDto>> SetFileSecurityInfoFromBodyAsync(string fileId, [FromBody] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperString.SetFileSecurityInfoAsync(fileId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("file/{fileId:int}/share")]
    public Task<IEnumerable<FileShareDto>> SetFileSecurityInfoFromBodyAsync(int fileId, [FromBody] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperInt.SetFileSecurityInfoAsync(fileId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("file/{fileId}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareDto>> SetFileSecurityInfoFromFormAsync(string fileId, [FromForm] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperString.SetFileSecurityInfoAsync(fileId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("file/{fileId:int}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareDto>> SetFileSecurityInfoFromFormAsync(int fileId, [FromForm] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperInt.SetFileSecurityInfoAsync(fileId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    /// <summary>
    /// Sets sharing settings for the folder with the ID specified in the request
    /// </summary>
    /// <short>Share folder</short>
    /// <param name="folderId">Folder ID</param>
    /// <param name="share">Collection of sharing rights</param>
    /// <param name="notify">Should notify people</param>
    /// <param name="sharingMessage">Sharing message to send when notifying</param>
    /// <remarks>
    /// Each of the FileShareParams must contain two parameters: 'ShareTo' - ID of the user with whom we want to share and 'Access' - access type which we want to grant to the user (Read, ReadWrite, etc) 
    /// </remarks>
    /// <category>Sharing</category>
    /// <returns>Shared folder information</returns>
    [Update("folder/{folderId}/share")]
    public Task<IEnumerable<FileShareDto>> SetFolderSecurityInfoFromBodyAsync(string folderId, [FromBody] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperString.SetFolderSecurityInfoAsync(folderId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("folder/{folderId:int}/share")]
    public Task<IEnumerable<FileShareDto>> SetFolderSecurityInfoFromBodyAsync(int folderId, [FromBody] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("folder/{folderId}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareDto>> SetFolderSecurityInfoFromFormAsync(string folderId, [FromForm] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperString.SetFolderSecurityInfoAsync(folderId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("folder/{folderId:int}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareDto>> SetFolderSecurityInfoFromFormAsync(int folderId, [FromForm] SecurityInfoRequestDto requestDto)
    {
        return _filesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, requestDto.Share, requestDto.Notify, requestDto.SharingMessage);
    }

    [Update("share")]
    public Task<IEnumerable<FileShareDto>> SetSecurityInfoFromBodyAsync([FromBody] SecurityInfoRequestDto requestDto)
    {
        return SetSecurityInfoAsync(requestDto);
    }

    [Update("share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareDto>> SetSecurityInfoFromFormAsync([FromForm] SecurityInfoRequestDto requestDto)
    {
        return SetSecurityInfoAsync(requestDto);
    }

    private async IAsyncEnumerable<FileEntryDto> ChangeOwnerAsync(ChangeOwnerRequestDto requestDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(requestDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(requestDto.FileIds);

        var result = AsyncEnumerable.Empty<FileEntry>();
        result.Concat(_fileStorageServiceInt.ChangeOwnerAsync(folderIntIds, fileIntIds, requestDto.UserId));
        result.Concat(_fileStorageServiceString.ChangeOwnerAsync(folderStringIds, fileStringIds, requestDto.UserId));

        await foreach (var e in result)
        {
            yield return await _filesControllerHelperInt.GetFileEntryWrapperAsync(e);
        }
    }

    private async Task<IEnumerable<FileShareDto>> SetSecurityInfoAsync(SecurityInfoRequestDto requestDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(requestDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(requestDto.FileIds);

        var result = new List<FileShareDto>();
        result.AddRange(await _filesControllerHelperInt.SetSecurityInfoAsync(fileIntIds, folderIntIds, requestDto.Share, requestDto.Notify, requestDto.SharingMessage));
        result.AddRange(await _filesControllerHelperString.SetSecurityInfoAsync(fileStringIds, folderStringIds, requestDto.Share, requestDto.Notify, requestDto.SharingMessage));

        return result;
    }
}