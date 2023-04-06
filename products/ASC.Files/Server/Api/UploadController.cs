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
    /// Creates session to upload large files in multiple chunks.
    /// </summary>
    /// <short>Chunked upload</short>
    /// <category>Uploads</category>
    /// <param name="folderId">Id of the folder in which file will be uploaded</param>
    /// <param name="fileName">Name of file which has to be uploaded</param>
    /// <param name="fileSize">Length in bytes of file which has to be uploaded</param>
    /// <param name="relativePath">Relative folder from folderId</param>
    /// <param name="encrypted" visible="false"></param>
    /// <remarks>
    /// <![CDATA[
    /// Each chunk can have different length but its important what length is multiple of <b>512</b> and greater or equal than <b>10 mb</b>. Last chunk can have any size.
    /// After initial request respond with status 200 OK you must obtain value of 'location' field from the response. Send all your chunks to that location.
    /// Each chunk must be sent in strict order in which chunks appears in file.
    /// After receiving each chunk if no errors occured server will respond with current information about upload session.
    /// When number of uploaded bytes equal to the number of bytes you send in initial request server will respond with 201 Created and will send you info about uploaded file.
    /// ]]>
    /// </remarks>
    /// <returns>
    /// <![CDATA[
    /// Information about created session. Which includes:
    /// <ul>
    /// <li><b>id:</b> unique id of this upload session</li>
    /// <li><b>created:</b> UTC time when session was created</li>
    /// <li><b>expired:</b> UTC time when session will be expired if no chunks will be sent until that time</li>
    /// <li><b>location:</b> URL to which you must send your next chunk</li>
    /// <li><b>bytes_uploaded:</b> If exists contains number of bytes uploaded for specific upload id</li>
    /// <li><b>bytes_total:</b> Number of bytes which has to be uploaded</li>
    /// </ul>
    /// ]]>
    /// </returns>
    [HttpPost("{folderId}/upload/create_session")]
    public async Task<object> CreateUploadSessionAsync(T folderId, SessionRequestDto inDto)
    {
        return await _filesControllerHelper.CreateUploadSessionAsync(folderId, inDto.FileName, inDto.FileSize, inDto.RelativePath, inDto.Encrypted, inDto.CreateOn);
    }

    [HttpPost("file/{fileId}/edit_session")]
    public async Task<object> CreateEditSession(T fileId, long fileSize)
    {
        return await _filesControllerHelper.CreateEditSessionAsync(fileId, fileSize);
    }

    /// <summary>
    /// Uploads the file specified with single file upload
    /// </summary>
    /// <param name="folderId">Folder ID to upload to</param>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="title">Name of file which has to be uploaded</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <category>Uploads</category>
    /// <returns></returns>
    [HttpPost("{folderId}/insert", Order = 1)]
    public async Task<FileDto<T>> InsertFileAsync(T folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelper.InsertFileAsync(folderId, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }


    /// <summary>
    /// Uploads the file specified with single file upload or standart multipart/form-data method to the selected folder
    /// </summary>
    /// <short>Upload to folder</short>
    /// <category>Uploads</category>
    /// <remarks>
    /// <![CDATA[
    ///  Upload can be done in 2 different ways:
    ///  <ol>
    /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
    /// <li>Using standart multipart/form-data method</li>
    /// </ol>]]>
    /// </remarks>
    /// <param name="folderId">Folder ID to upload to</param>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="contentType" visible="false">Content-Type Header</param>
    /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
    /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="storeOriginalFileFlag" visible="false">If True, upload documents in original formats as well</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <returns>Uploaded file</returns>
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
    /// Uploads the file specified with single file upload to 'Common Documents' section
    /// </summary>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="title">Name of file which has to be uploaded</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <category>Uploads</category>
    /// <returns></returns>
    [HttpPost("@common/insert")]
    public async Task<FileDto<int>> InsertFileToCommonFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelper.InsertFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Uploads the file specified with single file upload to 'Common Documents' section
    /// </summary>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="title">Name of file which has to be uploaded</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <category>Uploads</category>
    /// <returns></returns>
    [HttpPost("@my/insert")]
    public async Task<FileDto<int>> InsertFileToMyFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelper.InsertFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Uploads the file specified with single file upload or standart multipart/form-data method to 'Common Documents' section
    /// </summary>
    /// <short>Upload to Common</short>
    /// <category>Uploads</category>
    /// <remarks>
    /// <![CDATA[
    ///  Upload can be done in 2 different ways:
    ///  <ol>
    /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
    /// <li>Using standart multipart/form-data method</li>
    /// </ol>]]>
    /// </remarks>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="contentType" visible="false">Content-Type Header</param>
    /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
    /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
    /// <returns>Uploaded file</returns>
    [HttpPost("@common/upload")]
    public async Task<object> UploadFileToCommonAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        inDto.CreateNewIfExist = false;

        return await _filesControllerHelper.UploadFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto);
    }

    /// <summary>
    /// Uploads the file specified with single file upload or standart multipart/form-data method to 'My Documents' section
    /// </summary>
    /// <short>Upload to My</short>
    /// <category>Uploads</category>
    /// <remarks>
    /// <![CDATA[
    ///  Upload can be done in 2 different ways:
    ///  <ol>
    /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
    /// <li>Using standart multipart/form-data method</li>
    /// </ol>]]>
    /// </remarks>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="contentType" visible="false">Content-Type Header</param>
    /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
    /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
    /// <returns>Uploaded file</returns>
    [HttpPost("@my/upload")]
    public async Task<object> UploadFileToMyAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        inDto.CreateNewIfExist = false;

        return await _filesControllerHelper.UploadFileAsync(await _globalFolderHelper.FolderMyAsync, inDto);
    }
}
