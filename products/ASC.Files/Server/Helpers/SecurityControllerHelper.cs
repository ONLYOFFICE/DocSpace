using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Files.Helpers;

public class SecurityControllerHelper<T> : FilesHelperBase<T>
{
    private readonly FileShareDtoHelper _fileShareDtoHelper;
    private readonly FileShareParamsHelper _fileShareParamsHelper;

    public SecurityControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        FileShareDtoHelper fileShareDtoHelper,
        FileShareParamsHelper fileShareParamsHelper) 
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
        _fileShareDtoHelper = fileShareDtoHelper;
        _fileShareParamsHelper = fileShareParamsHelper;
    }

    public async Task<string> GenerateSharedLinkAsync(T fileId, FileShare share)
    {
        var file = await GetFileInfoAsync(fileId);

        var tmpInfo = await _fileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
        var sharedInfo = tmpInfo.Find(r => r.SubjectId == FileConstant.ShareLinkId);

        if (sharedInfo == null || sharedInfo.Share != share)
        {
            var list = new List<AceWrapper>
            {
                new AceWrapper
                {
                    SubjectId = FileConstant.ShareLinkId,
                    SubjectGroup = true,
                    Share = share
                }
            };

            var aceCollection = new AceCollection<T>
            {
                Files = new List<T> { fileId },
                Folders = new List<T>(0),
                Aces = list
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, false);

            tmpInfo = await _fileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
            sharedInfo = tmpInfo.Find(r => r.SubjectId == FileConstant.ShareLinkId);
        }

        return sharedInfo.Link;
    }

    public Task<IEnumerable<FileShareDto>> GetFileSecurityInfoAsync(T fileId)
    {
        return GetSecurityInfoAsync(new List<T> { fileId }, new List<T> { });
    }

    public Task<IEnumerable<FileShareDto>> GetFolderSecurityInfoAsync(T folderId)
    {
        return GetSecurityInfoAsync(new List<T> { }, new List<T> { folderId });
    }

    public async Task<IEnumerable<FileShareDto>> GetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
    {
        var fileShares = await _fileStorageService.GetSharedInfoAsync(fileIds, folderIds);

        return fileShares.Select(_fileShareDtoHelper.Get).ToList();
    }

    public async Task<bool> RemoveSecurityInfoAsync(List<T> fileIds, List<T> folderIds)
    {
        await _fileStorageService.RemoveAceAsync(fileIds, folderIds);

        return true;
    }

    public Task<IEnumerable<FileShareDto>> SetFileSecurityInfoAsync(T fileId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        return SetSecurityInfoAsync(new List<T> { fileId }, new List<T>(), share, notify, sharingMessage);
    }

    public Task<IEnumerable<FileShareDto>> SetFolderSecurityInfoAsync(T folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        return SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, share, notify, sharingMessage);
    }

    public async Task<IEnumerable<FileShareDto>> SetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        if (share != null && share.Any())
{
            var list = new List<AceWrapper>(share.Select(_fileShareParamsHelper.ToAceObject));

            var aceCollection = new AceCollection<T>
            {
                Files = fileIds,
                Folders = folderIds,
                Aces = list,
                Message = sharingMessage
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, notify);
        }

        return await GetSecurityInfoAsync(fileIds, folderIds);
    }
}