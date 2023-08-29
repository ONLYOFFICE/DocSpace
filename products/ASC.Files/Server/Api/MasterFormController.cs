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

[ConstraintRoute("int")]
public class MasterFormControllerInternal : MasterFormController<int>
{
    public MasterFormControllerInternal(
        FileStorageService fileStorageServiceString,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageServiceString, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class MasterFormControllerThirdparty : MasterFormController<string>
{
    public MasterFormControllerThirdparty(
        FileStorageService fileStorageService,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(fileStorageService, folderDtoHelper, fileDtoHelper)
    {
    }
}

public abstract class MasterFormController<T> : ApiControllerBase
{
    private readonly FileStorageService _fileStorageService;

    public MasterFormController(FileStorageService fileStorageService,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Checks if the current file is a form draft which can be filled out.
    /// </summary>
    /// <short>Check the form draft</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CheckFillFormDraftRequestDto, ASC.Files.Core" name="inDto">Request parameters for checking a form draft</param>
    /// <returns type="System.Object, System">Link to the form</returns>
    /// <path>api/2.0/files/masterform/{fileId}/checkfillformdraft</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("masterform/{fileId}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftAsync(T fileId, CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageService.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }
}