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

namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class SecutiryControllerInternal : SecutiryController<int>
{
    public SecutiryControllerInternal(
        FileStorageService<int> fileStorageService,
        SecurityControllerHelper<int> securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, securityControllerHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class SecutiryControllerThirdparty : SecutiryController<string>
{
    public SecutiryControllerThirdparty(
        FileStorageService<string> fileStorageService,
        SecurityControllerHelper<string> securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, securityControllerHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public abstract class SecutiryController<T> : ApiControllerBase
{
    private readonly FileStorageService<T> _fileStorageService;
    private readonly SecurityControllerHelper<T> _securityControllerHelper;

    public SecutiryController(FileStorageService<T> fileStorageService, SecurityControllerHelper<T> securityControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _securityControllerHelper = securityControllerHelper;
    }

    /// <summary>
    /// Returns an external link to the shared file with the ID specified in the request.
    /// </summary>
    /// <short>Get the shared link</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.GenerateSharedLinkRequestDto, ASC.Files.Core" name="inDto">Request parameters for generating the shared link</param>
    /// <category>Sharing</category>
    /// <returns type="System.Object, System">Shared file link</returns>
    /// <path>api/2.0/files/{fileId}/sharedlinkAsync</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{fileId}/sharedlinkAsync")]
    public async Task<object> GenerateSharedLinkAsync(T fileId, GenerateSharedLinkRequestDto inDto)
    {
        return await _securityControllerHelper.GenerateSharedLinkAsync(fileId, inDto.Share);
    }

    /// <summary>
    /// Returns the detailed information about the shared file with the ID specified in the request.
    /// </summary>
    /// <short>Get the shared file information</short>
    /// <category>Sharing</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">List of shared file information</returns>
    /// <path>api/2.0/files/file/{fileId}/share</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpGet("file/{fileId}/share")]
    public async IAsyncEnumerable<FileShareDto> GetFileSecurityInfoAsync(T fileId)
    {
        await foreach (var s in _securityControllerHelper.GetFileSecurityInfoAsync(fileId))
        {
            yield return s;
        }
    }

    /// <summary>
    /// Returns the detailed information about the shared folder with the ID specified in the request.
    /// </summary>
    /// <short>Get the shared folder information</short>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <category>Sharing</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">List of shared folder information</returns>
    /// <path>api/2.0/files/folder/{folderId}/share</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpGet("folder/{folderId}/share")]
    public async IAsyncEnumerable<FileShareDto> GetFolderSecurityInfoAsync(T folderId)
    {
        await foreach (var s in _securityControllerHelper.GetFolderSecurityInfoAsync(folderId))
        {
            yield return s;
        }
    }

    /// <summary>
    /// Sets the access status for the external link to the file with the ID specified in the request.
    /// </summary>
    /// <short>Set the link access status</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.GenerateSharedLinkRequestDto, ASC.Files.Core" name="inDto">Request parameters for generating the sharing link</param>
    /// <category>Sharing</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the file is successfully shared</returns>
    /// <path>api/2.0/files/{fileId}/setacelink</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{fileId}/setacelink")]
    public Task<bool> SetAceLinkAsync(T fileId, [FromBody] GenerateSharedLinkRequestDto inDto)
    {
        return _fileStorageService.SetAceLinkAsync(fileId, inDto.Share);
    }

    /// <summary>
    /// Sets the sharing settings to a file with the ID specified in the request.
    /// </summary>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SecurityInfoRequestDto, ASC.Files.Core" name="inDto">Security information request parameters</param>
    /// <short>Share a file</short>
    /// <category>Sharing</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">List of shared file information: sharing rights, a user who has the access to the specified file, the file is locked by this user or not, this user is an owner of the specified file or not, this user can edit the access to the specified file or not</returns>
    /// <path>api/2.0/files/file/{fileId}/share</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpPut("file/{fileId}/share")]
    public async IAsyncEnumerable<FileShareDto> SetFileSecurityInfoAsync(T fileId, SecurityInfoRequestDto inDto)
    {
        await foreach (var s in _securityControllerHelper.SetSecurityInfoAsync(new List<T> { fileId }, new List<T>(), inDto.Share, inDto.Notify, inDto.SharingMessage))
        {
            yield return s;
        }
    }

    /// <summary>
    /// Sets the sharing settings to a folder with the ID specified in the request.
    /// </summary>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SecurityInfoRequestDto, ASC.Files.Core" name="inDto">Security information request parameters</param>
    /// <short>Share a folder</short>
    /// <category>Sharing</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto}, ASC.Files.Core">List of shared folder information: sharing rights, a user who has the access to the specified folder, the folder is locked by this user or not, this user is an owner of the specified folder or not, this user can edit the access to the specified folder or not</returns>
    /// <path>api/2.0/files/folder/{folderId}/share</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpPut("folder/{folderId}/share")]
    public async IAsyncEnumerable<FileShareDto> SetFolderSecurityInfoAsync(T folderId, SecurityInfoRequestDto inDto)
    {
        await foreach (var s in _securityControllerHelper.SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, inDto.Share, inDto.Notify, inDto.SharingMessage))
        {
            yield return s;
        }
    }

    /// <summary>
    /// Returns the encryption keys to access a file with the ID specified in the request.
    /// </summary>
    /// <short>Get file encryption keys</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <category>Sharing</category>
    /// <returns type="ASC.Web.Files.Core.Entries.EncryptionKeyPairDto, ASC.Files.Core">List of encryption key pairs: encrypted private key, public key, user ID</returns>
    /// <path>api/2.0/files/file/{fileId}/publickeys</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("file/{fileId}/publickeys")]
    public Task<List<EncryptionKeyPairDto>> GetEncryptionAccess(T fileId)
    {
        return _fileStorageService.GetEncryptionAccessAsync(fileId);
    }

    /// <summary>
    /// Sends a message to the users who are mentioned in the file with the ID specified in the request.
    /// </summary>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Web.Files.Services.WCFService.MentionMessageWrapper, ASC.Files.Core" name="mentionMessage">Mention message request parameters</param>
    /// <short>Send the mention message</short>
    /// <category>Sharing</category>
    /// <returns type="ASC.Web.Files.Services.WCFService.AceShortWrapper, ASC.Files.Core">List of access rights information</returns>
    /// <path>api/2.0/files/file/{fileId}/sendeditornotify</path>
    /// <httpMethod>POST</httpMethod>
    /// <collection>list</collection>
    [HttpPost("file/{fileId}/sendeditornotify")]
    public Task<List<AceShortWrapper>> SendEditorNotify(T fileId, MentionMessageWrapper mentionMessage)
    {
        return _fileStorageService.SendEditorNotifyAsync(fileId, mentionMessage);
    }
}

public class SecutiryControllerCommon : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly SecurityControllerHelper<int> _securityControllerHelperInt;
    private readonly SecurityControllerHelper<string> _securityControllerHelperString;

    public SecutiryControllerCommon(
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString,
        SecurityControllerHelper<int> securityControllerHelperInt,
        SecurityControllerHelper<string> securityControllerHelperString,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageServiceInt = fileStorageServiceInt;
        _fileStorageServiceString = fileStorageServiceString;
        _securityControllerHelperInt = securityControllerHelperInt;
        _securityControllerHelperString = securityControllerHelperString;
    }

    /// <summary>
    /// Changes the owner of the file with the ID specified in the request.
    /// </summary>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ChangeOwnerRequestDto, ASC.Files.Core" name="inDto">Request parameters for changing the file owner</param>
    /// <short>Change the file owner</short>
    /// <category>Sharing</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">File entry information</returns>
    /// <path>api/2.0/files/owner</path>
    /// <httpMethod>POST</httpMethod>
    /// <collection>list</collection>
    [HttpPost("owner")]
    public async IAsyncEnumerable<FileEntryDto> ChangeOwnerAsync(ChangeOwnerRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        var data = AsyncEnumerable.Empty<FileEntry>();
        data = data.Concat(_fileStorageServiceInt.ChangeOwnerAsync(folderIntIds, fileIntIds, inDto.UserId));
        data = data.Concat(_fileStorageServiceString.ChangeOwnerAsync(folderStringIds, fileStringIds, inDto.UserId));

        await foreach (var e in data)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    /// <summary>
    /// Returns the sharing rights for all the files and folders specified in the request.
    /// </summary>
    /// <short>Get the sharing rights</short>
    /// <category>Sharing</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">List of shared files and folders information</returns>
    /// <path>api/2.0/files/share</path>
    /// <httpMethod>POST</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpPost("share")]
    public async IAsyncEnumerable<FileShareDto> GetSecurityInfoAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        var internalIds = _securityControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds);
        var thirdpartyIds = _securityControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds);

        await foreach (var r in internalIds.Concat(thirdpartyIds))
        {
            yield return r;
        }
    }

    /// <summary>
    /// Removes the sharing rights from all the files and folders specified in the request.
    /// </summary>
    /// <short>Remove the sharing rights</short>
    /// <category>Sharing</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/share</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <visible>false</visible>
    [HttpDelete("share")]
    public async Task<bool> RemoveSecurityInfoAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _securityControllerHelperInt.RemoveSecurityInfoAsync(fileIntIds, folderIntIds);
        await _securityControllerHelperString.RemoveSecurityInfoAsync(fileStringIds, folderStringIds);

        return true;
    }


    /// <summary>
    /// Sets the sharing rights to all the files and folders specified in the request.
    /// </summary>
    /// <short>Set the sharing rights</short>
    /// <category>Sharing</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SecurityInfoRequestDto, ASC.Files.Core" name="inDto">Security information request parameters</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileShareDto, ASC.Files.Core">List of shared files and folders information: sharing rights, a user who has the access to the specified folder, the folder is locked by this user or not, this user is an owner of the specified folder or not, this user can edit the access to the specified folder or not</returns>
    /// <path>api/2.0/files/share</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpPut("share")]
    public async IAsyncEnumerable<FileShareDto> SetSecurityInfoAsync(SecurityInfoRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        var internalIds = _securityControllerHelperInt.SetSecurityInfoAsync(fileIntIds, folderIntIds, inDto.Share, inDto.Notify, inDto.SharingMessage);
        var thirdpartyIds = _securityControllerHelperString.SetSecurityInfoAsync(fileStringIds, folderStringIds, inDto.Share, inDto.Notify, inDto.SharingMessage);

        await foreach (var s in internalIds.Concat(thirdpartyIds))
        {
            yield return s;
        }
    }
}