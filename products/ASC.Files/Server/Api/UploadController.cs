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
public class UploadControllerInternal : UploadController<int>
{
    public UploadControllerInternal(
        UploadControllerHelper filesControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(filesControllerHelper,
        folderDtoHelper,
        fileDtoHelper)
    {
    }
}

public class UploadControllerThirdparty : UploadController<string>
{
    public UploadControllerThirdparty(
        UploadControllerHelper filesControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(filesControllerHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public abstract class UploadController<T> : ApiControllerBase
{
    private readonly UploadControllerHelper _filesControllerHelper;

    public UploadController(UploadControllerHelper filesControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _filesControllerHelper = filesControllerHelper;
    }

    /// <summary>
    /// Creates a session to upload large files in multiple chunks to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Chunked upload</short>
    /// <category>Operations</category>
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SessionRequestDto, ASC.Files.Core" name="inDto">Session request parameters</param>
    /// <remarks>
    /// <![CDATA[
    /// Each chunk can have different length but the length should be multiple of <b>512</b> and greater or equal to <b>10 mb</b>. Last chunk can have any size.
    /// After the initial response to the request with the <b>200 OK</b> status, you must get the <em>location</em> field value from the response. Send all your chunks to this location.
    /// Each chunk must be sent in the exact order the chunks appear in the file.
    /// After receiving each chunk, the server will respond with the current information about the upload session if no errors occurred.
    /// When the number of bytes uploaded is equal to the number of bytes you sent in the initial request, the server responds with the <b>201 Created</b> status and sends you information about the uploaded file.
    /// ]]>
    /// </remarks>
    /// <returns type="System.Object, System">
    /// <![CDATA[
    /// Information about created session which includes:
    /// <ul>
    /// <li><b>id:</b> unique ID of this upload session,</li>
    /// <li><b>created:</b> UTC time when the session was created,</li>
    /// <li><b>expired:</b> UTC time when the session will expire if no chunks are sent before that time,</li>
    /// <li><b>location:</b> URL where you should send your next chunk,</li>
    /// <li><b>bytes_uploaded:</b> number of bytes uploaded for the specific upload ID,</li>
    /// <li><b>bytes_total:</b> total number of bytes which will be uploaded.</li>
    /// </ul>
    /// ]]>
    /// </returns>
    /// <path>api/2.0/files/{folderId}/upload/create_session</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{folderId}/upload/create_session")]
    public async Task<object> CreateUploadSessionAsync(T folderId, SessionRequestDto inDto)
    {
        return await _filesControllerHelper.CreateUploadSessionAsync(folderId, inDto.FileName, inDto.FileSize, inDto.RelativePath, inDto.Encrypted, inDto.CreateOn);
    }

    /// <summary>
    /// Creates a session to edit the existing file with multiple chunks (needed for WebDAV).
    /// </summary>
    /// <short>Create the editing session</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" name="fileId">File ID</param>
    /// <param type="System.Int64, System" name="fileSize">File size in bytes</param>
    /// <returns type="System.Object, System">
    /// <![CDATA[
    /// Information about created session which includes:
    /// <ul>
    /// <li><b>id:</b> unique ID of this upload session,</li>
    /// <li><b>created:</b> UTC time when the session was created,</li>
    /// <li><b>expired:</b> UTC time when the session will expire if no chunks are sent before that time,</li>
    /// <li><b>location:</b> URL where you should send your next chunk,</li>
    /// <li><b>bytes_uploaded:</b> number of bytes uploaded for the specific upload ID,</li>
    /// <li><b>bytes_total:</b> total number of bytes which will be uploaded.</li>
    /// </ul>
    /// ]]>
    /// </returns>
    /// <path>api/2.0/files/file/{fileId}/edit_session</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("file/{fileId}/edit_session")]
    public async Task<object> CreateEditSession(T fileId, long fileSize)
    {
        return await _filesControllerHelper.CreateEditSessionAsync(fileId, fileSize);
    }

    /// <summary>
    /// Inserts a file specified in the request to the selected folder by single file uploading.
    /// </summary>
    /// <short>Insert a file</short>
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.InsertFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for inserting a file</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Inserted file informationy</returns>
    /// <path>api/2.0/files/{folderId}/insert</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{folderId}/insert", Order = 1)]
    public async Task<FileDto<T>> InsertFileAsync(T folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelper.InsertFileAsync(folderId, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }


    /// <summary>
    /// Uploads a file specified in the request to the selected folder by single file uploading or standart multipart/form-data method.
    /// </summary>
    /// <short>Upload a file</short>
    /// <category>Folders</category>
    /// <remarks>
    /// <![CDATA[
    ///  You can upload files in two different ways:
    ///  <ol>
    /// <li>Using single file upload. You should set the Content-Type and Content-Disposition headers to specify a file name and content type, and send the file to the request body.</li>
    /// <li>Using standart multipart/form-data method.</li>
    /// </ol>]]>
    /// </remarks>
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UploadRequestDto, ASC.Files.Core" name="inDto">Request parameters for uploading a file</param>
    /// <returns type="System.Object, System">Uploaded file(s)</returns>
    /// <path>api/2.0/files/{folderId}/upload</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{folderId}/upload", Order = 1)]
    public async Task<object> UploadFileAsync(T folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        return await _filesControllerHelper.UploadFileAsync(folderId, inDto);
    }
}

public class UploadControllerCommon : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly UploadControllerHelper _filesControllerHelper;

    public UploadControllerCommon(
        GlobalFolderHelper globalFolderHelper,
        UploadControllerHelper filesControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _globalFolderHelper = globalFolderHelper;
        _filesControllerHelper = filesControllerHelper;
    }


    /// <summary>
    /// Inserts a file specified in the request to the "Common" section by single file uploading.
    /// </summary>
    /// <short>Insert a file to the "Common" section</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.InsertFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for inserting a file</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Inserted file</returns>
    /// <path>api/2.0/files/@common/insert</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@common/insert")]
    public async Task<FileDto<int>> InsertFileToCommonFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelper.InsertFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Inserts a file specified in the request to the "My documents" section by single file uploading.
    /// </summary>
    /// <short>Insert a file to the "My documents" section</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.InsertFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for inserting a file</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Inserted file</returns>
    /// <path>api/2.0/files/@my/insert</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@my/insert")]
    public async Task<FileDto<int>> InsertFileToMyFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelper.InsertFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Uploads a file specified in the request to the "Common" section by single file uploading or standart multipart/form-data method.
    /// </summary>
    /// <short>Upload a file to the "Common" section</short>
    /// <category>Folders</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UploadRequestDto, ASC.Files.Core" name="inDto">Request parameters for uploading a file</param>
    /// <remarks>
    /// <![CDATA[
    ///  You can upload files in two different ways:
    ///  <ol>
    /// <li>Using single file upload. You should set the Content-Type and Content-Disposition headers to specify a file name and content type, and send the file to the request body.</li>
    /// <li>Using standart multipart/form-data method.</li>
    /// </ol>]]>
    /// </remarks>
    /// <returns type="System.Object, System">Uploaded file(s)</returns>
    /// <path>api/2.0/files/@common/upload</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@common/upload")]
    public async Task<object> UploadFileToCommonAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        inDto.CreateNewIfExist = false;

        return await _filesControllerHelper.UploadFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto);
    }

    /// <summary>
    /// Uploads a file specified in the request to the "My documents" section by single file uploading or standart multipart/form-data method.
    /// </summary>
    /// <short>Upload a file to the "My documents" section</short>
    /// <category>Folders</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UploadRequestDto, ASC.Files.Core" name="inDto">Request parameters for uploading a file</param>
    /// <remarks>
    /// <![CDATA[
    ///  You can upload files in two different ways:
    ///  <ol>
    /// <li>Using single file upload. You should set the Content-Type and Content-Disposition headers to specify a file name and content type, and send the file to the request body.</li>
    /// <li>Using standart multipart/form-data method.</li>
    /// </ol>]]>
    /// </remarks>
    /// <returns type="System.Object, System">Uploaded file(s)</returns>
    /// <path>api/2.0/files/@my/upload</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@my/upload")]
    public async Task<object> UploadFileToMyAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        inDto.CreateNewIfExist = false;

        return await _filesControllerHelper.UploadFileAsync(await _globalFolderHelper.FolderMyAsync, inDto);
    }
}
