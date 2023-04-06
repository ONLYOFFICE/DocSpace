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

public class SecurityControllerHelper : FilesHelperBase
{
    private readonly FileShareDtoHelper _fileShareDtoHelper;
    private readonly FileShareParamsHelper _fileShareParamsHelper;

    public SecurityControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService fileStorageService,
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

    public async Task<string> GenerateSharedLinkAsync<T>(T fileId, FileShare share)
    {
        var file = await GetFileInfoAsync(fileId);

        var tmpInfo = await _fileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
        var sharedInfo = tmpInfo.Find(r => r.Id == FileConstant.ShareLinkId);

        if (sharedInfo == null || sharedInfo.Access != share)
        {
            var list = new List<AceWrapper>
            {
                new AceWrapper
                {
                    Id = FileConstant.ShareLinkId,
                    SubjectGroup = true,
                    Access = share
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
            sharedInfo = tmpInfo.Find(r => r.Id == FileConstant.ShareLinkId);
        }

        return sharedInfo.Link;
    }

    public IAsyncEnumerable<FileShareDto> GetFileSecurityInfoAsync<T>(T fileId)
    {
        return GetSecurityInfoAsync(new List<T> { fileId }, new List<T> { });
    }

    public IAsyncEnumerable<FileShareDto> GetFolderSecurityInfoAsync<T>(T folderId)
    {
        return GetSecurityInfoAsync(new List<T> { }, new List<T> { folderId });
    }

    public async IAsyncEnumerable<FileShareDto> GetSecurityInfoAsync<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
    {
        var fileShares = await _fileStorageService.GetSharedInfoAsync(fileIds, folderIds);

        foreach (var fileShareDto in fileShares)
        {
            yield return await _fileShareDtoHelper.Get(fileShareDto);
        }
    }

    public async Task<bool> RemoveSecurityInfoAsync<T>(List<T> fileIds, List<T> folderIds)
    {
        await _fileStorageService.RemoveAceAsync(fileIds, folderIds);

        return true;
    }

    public IAsyncEnumerable<FileShareDto> SetFolderSecurityInfoAsync<T>(T folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        return SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, share, notify, sharingMessage);
    }

    public async IAsyncEnumerable<FileShareDto> SetSecurityInfoAsync<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        if (share != null && share.Any())
        {
            var list = await share.ToAsyncEnumerable().SelectAwait(async s => await _fileShareParamsHelper.ToAceObjectAsync(s)).ToListAsync();

            var aceCollection = new AceCollection<T>
            {
                Files = fileIds,
                Folders = folderIds,
                Aces = list,
                Message = sharingMessage
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, notify);
        }

        await foreach (var s in GetSecurityInfoAsync(fileIds, folderIds))
        {
            yield return s;
        }
    }
}