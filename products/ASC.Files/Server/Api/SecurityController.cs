﻿// (c) Copyright Ascensio System SIA 2010-2022
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

using ASC.MessagingSystem;
using ASC.Web.Core;

namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class SecurityControllerInternal : SecurityController<int>
{
    public SecurityControllerInternal(
        FileStorageService fileStorageService,
        SecurityControllerHelper securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, securityControllerHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class SecurityControllerThirdparty : SecurityController<string>
{
    public SecurityControllerThirdparty(
        FileStorageService fileStorageService,
        SecurityControllerHelper securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, securityControllerHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public abstract class SecurityController<T> : ApiControllerBase
{
    private readonly FileStorageService _fileStorageService;
    private readonly SecurityControllerHelper _securityControllerHelper;

    public SecurityController(
        FileStorageService fileStorageService, 
        SecurityControllerHelper securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _securityControllerHelper = securityControllerHelper;
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
    [HttpPut("{fileId}/sharedlinkAsync")]
    public async Task<object> GenerateSharedLinkAsync(T fileId, GenerateSharedLinkRequestDto inDto)
    {
        return await _securityControllerHelper.GenerateSharedLinkAsync(fileId, inDto.Share);
    }

    /// <summary>
    /// Returns the detailed information about shared file with the ID specified in the request
    /// </summary>
    /// <short>File sharing</short>
    /// <category>Sharing</category>
    /// <param name="fileId">File ID</param>
    /// <returns>Shared file information</returns>
    [HttpGet("file/{fileId}/share")]
    public async IAsyncEnumerable<FileShareDto> GetFileSecurityInfoAsync(T fileId)
    {
        await foreach (var s in _securityControllerHelper.GetFileSecurityInfoAsync(fileId))
        {
            yield return s;
        }
    }

    /// <summary>
    /// Returns the detailed information about shared folder with the ID specified in the request
    /// </summary>
    /// <short>Folder sharing</short>
    /// <param name="folderId">Folder ID</param>
    /// <category>Sharing</category>
    /// <returns>Shared folder information</returns>
    [HttpGet("folder/{folderId}/share")]
    public async IAsyncEnumerable<FileShareDto> GetFolderSecurityInfoAsync(T folderId)
    {
        await foreach (var s in _securityControllerHelper.GetFolderSecurityInfoAsync(folderId))
        {
            yield return s;
        }
    }

    [HttpPut("{fileId}/setacelink")]
    public async Task<bool> SetAceLinkAsync(T fileId, [FromBody] GenerateSharedLinkRequestDto inDto)
    {
        return await _fileStorageService.SetAceLinkAsync(fileId, inDto.Share);
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
    [HttpPut("file/{fileId}/share")]
    public async IAsyncEnumerable<FileShareDto> SetFileSecurityInfoAsync(T fileId, SecurityInfoRequestDto inDto)
    {
        await foreach (var s in _securityControllerHelper.SetSecurityInfoAsync(new List<T> { fileId }, new List<T>(), inDto.Share, inDto.Notify, inDto.SharingMessage))
        {
            yield return s;
        }
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
    [HttpPut("folder/{folderId}/share")]
    public async IAsyncEnumerable<FileShareDto> SetFolderSecurityInfoAsync(T folderId, SecurityInfoRequestDto inDto)
    {
        await foreach (var s in _securityControllerHelper.SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, inDto.Share, inDto.Notify, inDto.SharingMessage))
        {
            yield return s;
        }
    }

    [HttpGet("file/{fileId}/publickeys")]
    public async Task<List<EncryptionKeyPairDto>> GetEncryptionAccess(T fileId)
    {
        return await _fileStorageService.GetEncryptionAccessAsync(fileId);
    }

    [HttpPost("file/{fileId}/sendeditornotify")]
    public async Task<List<AceShortWrapper>> SendEditorNotify(T fileId, MentionMessageWrapper mentionMessage)
    {
        return await _fileStorageService.SendEditorNotifyAsync(fileId, mentionMessage);
    }
}

public class SecurityControllerCommon : ApiControllerBase
{
    private readonly FileStorageService _fileStorageService;
    private readonly SecurityControllerHelper _securityControllerHelper;
    private readonly BruteForceLoginManager _bruteForceLoginManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ExternalLinkHelper _externalLinkHelper;
    private readonly IMapper _mapper;

    public SecurityControllerCommon(
        FileStorageService fileStorageService,
        SecurityControllerHelper securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        BruteForceLoginManager bruteForceLoginManager, 
        IHttpContextAccessor httpContextAccessor,
        ExternalLinkHelper externalLinkHelper,
        IMapper mapper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _securityControllerHelper = securityControllerHelper;
        _bruteForceLoginManager = bruteForceLoginManager;
        _httpContextAccessor = httpContextAccessor;
        _externalLinkHelper = externalLinkHelper;
        _mapper = mapper;
    }

    [HttpPost("owner")]
    public async IAsyncEnumerable<FileEntryDto> ChangeOwnerAsync(ChangeOwnerRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        var data = AsyncEnumerable.Empty<FileEntry>();
        data = data.Concat(_fileStorageService.ChangeOwnerAsync(folderIntIds, fileIntIds, inDto.UserId));
        data = data.Concat(_fileStorageService.ChangeOwnerAsync(folderStringIds, fileStringIds, inDto.UserId));

        await foreach (var e in data)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    [HttpPost("share")]
    public async IAsyncEnumerable<FileShareDto> GetSecurityInfoAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        var internalIds = _securityControllerHelper.GetSecurityInfoAsync(fileIntIds, folderIntIds);
        var thirdpartyIds = _securityControllerHelper.GetSecurityInfoAsync(fileStringIds, folderStringIds);

        await foreach (var r in internalIds.Concat(thirdpartyIds))
        {
            yield return r;
        }
    }

    /// <summary>
    ///   Removes sharing rights for the group with the ID specified in the request
    /// </summary>
    /// <param name="folderIds">Folders ID</param>
    /// <param name="fileIds">Files ID</param>
    /// <short>Remove group sharing rights</short>
    /// <category>Sharing</category>
    /// <returns>Shared file information</returns>
    [HttpDelete("share")]
    public async Task<bool> RemoveSecurityInfoAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _securityControllerHelper.RemoveSecurityInfoAsync(fileIntIds, folderIntIds);
        await _securityControllerHelper.RemoveSecurityInfoAsync(fileStringIds, folderStringIds);

        return true;
    }


    [HttpPut("share")]
    public async IAsyncEnumerable<FileShareDto> SetSecurityInfoAsync(SecurityInfoRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        var internalIds = _securityControllerHelper.SetSecurityInfoAsync(fileIntIds, folderIntIds, inDto.Share, inDto.Notify, inDto.SharingMessage);
        var thirdpartyIds = _securityControllerHelper.SetSecurityInfoAsync(fileStringIds, folderStringIds, inDto.Share, inDto.Notify, inDto.SharingMessage);

        await foreach (var s in internalIds.Concat(thirdpartyIds))
        {
            yield return s;
        }
    }
    
    [AllowAnonymous]
    [HttpGet("share/{key}")]
    public async Task<ExternalShareDto> GetExternalShareDataAsync(string key)
    {
        var validationInfo = await _externalLinkHelper.ValidateAsync(key);

        return _mapper.Map<ValidationInfo, ExternalShareDto>(validationInfo);
    }

    [AllowAnonymous]
    [HttpPost("share/{key}/password")]
    public async Task<ExternalShareDto> ApplyExternalSharePasswordAsync(string key, ExternalShareRequestDto inDto)
    {
        var ip = MessageSettings.GetIP(_httpContextAccessor.HttpContext?.Request);
        
        await _bruteForceLoginManager.IncrementAsync(key, ip, true, FilesCommonResource.ErrorMessage_SharePasswordManyAttempts);
        
        var validationInfo =  await _externalLinkHelper.ValidateAsync(key, inDto.Password);

        if (validationInfo.Status != Status.InvalidPassword)
        {
            await _bruteForceLoginManager.DecrementAsync(key, ip);
        }

        return _mapper.Map<ValidationInfo, ExternalShareDto>(validationInfo);
    }
}