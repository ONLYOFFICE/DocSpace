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

public class UploadControllerHelper : FilesHelperBase
{
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly ChunkedUploadSessionHelper _chunkedUploadSessionHelper;
    private readonly TenantManager _tenantManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SecurityContext _securityContext;

    public UploadControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        FilesLinkUtility filesLinkUtility,
        ChunkedUploadSessionHelper chunkedUploadSessionHelper,
        TenantManager tenantManager,
        IHttpClientFactory httpClientFactory,
        SecurityContext securityContext)
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
        _filesLinkUtility = filesLinkUtility;
        _chunkedUploadSessionHelper = chunkedUploadSessionHelper;
        _tenantManager = tenantManager;
        _httpClientFactory = httpClientFactory;
        _securityContext = securityContext;
    }

    public async Task<object> CreateEditSessionAsync<T>(T fileId, long fileSize)
    {
        var file = await _fileUploader.VerifyChunkedUploadForEditing(fileId, fileSize);

        return await CreateUploadSessionAsync(file, false, default(ApiDateTime), true);
    }

    public async Task<object> CreateUploadSessionAsync<T>(T folderId, string fileName, long fileSize, string relativePath, bool encrypted, ApiDateTime createOn, bool keepVersion = false)
    {
        var file = await _fileUploader.VerifyChunkedUploadAsync(folderId, fileName, fileSize, _filesSettingsHelper.UpdateIfExist, relativePath);
        return await CreateUploadSessionAsync(file, encrypted, createOn, keepVersion);
    }

    public async Task<object> CreateUploadSessionAsync<T>(File<T> file, bool encrypted, ApiDateTime createOn, bool keepVersion = false)
    {
        if (_filesLinkUtility.IsLocalFileUploader)
        {
            var session = await _fileUploader.InitiateUploadAsync(file.ParentId, file.Id ?? default, file.Title, file.ContentLength, encrypted, keepVersion, createOn);

            var responseObject = await _chunkedUploadSessionHelper.ToResponseObjectAsync(session, true);

            return new
            {
                success = true,
                data = responseObject
            };
        }

        var createSessionUrl = _filesLinkUtility.GetInitiateUploadSessionUrl(await _tenantManager.GetCurrentTenantIdAsync(), file.ParentId, file.Id, file.Title, file.ContentLength, encrypted, _securityContext);

        var httpClient = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(createSessionUrl),
            Method = HttpMethod.Post
        };

        // hack for uploader.onlyoffice.com in api requests
        //var rewriterHeader = _httpContextAccessor.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
        //if (!string.IsNullOrEmpty(rewriterHeader))
        //{
        //    request.Headers.Add(HttpRequestExtensions.UrlRewriterHeader, rewriterHeader.ToString());
        //}

        using var response = await httpClient.SendAsync(request);
        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(responseStream);

        var responseAsString = await streamReader.ReadToEndAsync();
        var jObject = JObject.Parse(responseAsString); //result is json string

        var result = new
        {
            success = jObject["success"].ToString(),
            data = new
            {
                id = jObject["data"]["id"].ToString(),
                path = jObject["data"]["path"].Values().Select(x => (T)Convert.ChangeType(x, typeof(T))),
                created = jObject["data"]["created"].Value<DateTime>(),
                expired = jObject["data"]["expired"].Value<DateTime>(),
                location = jObject["data"]["location"].ToString(),
                bytes_uploaded = jObject["data"]["bytes_uploaded"].Value<long>(),
                bytes_total = jObject["data"]["bytes_total"].Value<long>()
            }
        };

        return result;
    }

    public async Task<object> UploadFileAsync<T>(T folderId, UploadRequestDto uploadModel)
    {
        if (uploadModel.StoreOriginalFileFlag.HasValue)
        {
            _filesSettingsHelper.StoreOriginalFiles = uploadModel.StoreOriginalFileFlag.Value;
        }

        IEnumerable<IFormFile> files = _httpContextAccessor.HttpContext.Request.Form.Files;
        if (files == null || !files.Any())
        {
            files = uploadModel.Files;
        }

        if (files != null && files.Any())
        {
            if (files.Count() == 1)
            {
                //Only one file. return it
                var postedFile = files.First();

                return await InsertFileAsync(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus);
            }

            //For case with multiple files
            var result = new List<object>();

            foreach (var postedFile in uploadModel.Files)
            {
                result.Add(await InsertFileAsync(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus));
            }

            return result;
        }

        if (uploadModel.File != null)
        {
            var fileName = "file" + MimeMapping.GetExtention(uploadModel.ContentType.MediaType);
            if (uploadModel.ContentDisposition != null)
            {
                fileName = uploadModel.ContentDisposition.FileName;
            }

            return new List<FileDto<T>>
            {
                await InsertFileAsync(folderId, uploadModel.File.OpenReadStream(), fileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)
            };
        }

        throw new InvalidOperationException("No input files");
    }
}
