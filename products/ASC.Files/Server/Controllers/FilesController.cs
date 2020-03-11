/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Users;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Model;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using FileShare = ASC.Files.Core.Security.FileShare;
using MimeMapping = ASC.Common.Web.MimeMapping;
using SortedByType = ASC.Files.Core.SortedByType;

namespace ASC.Api.Documents
{
    /// <summary>
    /// Provides access to documents
    /// </summary>
    [DefaultRoute]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApiContext ApiContext;
        private readonly FileStorageService FileStorageService;

        public GlobalFolderHelper GlobalFolderHelper { get; }
        public FileWrapperHelper FileWrapperHelper { get; }
        public FilesSettingsHelper FilesSettingsHelper { get; }
        public FilesLinkUtility FilesLinkUtility { get; }
        public FileUploader FileUploader { get; }
        public DocumentServiceHelper DocumentServiceHelper { get; }
        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public FolderWrapperHelper FolderWrapperHelper { get; }
        public FileOperationWraperHelper FileOperationWraperHelper { get; }
        public FileShareWrapperHelper FileShareWrapperHelper { get; }
        public FileShareParamsHelper FileShareParamsHelper { get; }
        public EntryManager EntryManager { get; }
        public UserManager UserManager { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public ThirdpartyConfiguration ThirdpartyConfiguration { get; }
        public BoxLoginProvider BoxLoginProvider { get; }
        public DropboxLoginProvider DropboxLoginProvider { get; }
        public GoogleLoginProvider GoogleLoginProvider { get; }
        public OneDriveLoginProvider OneDriveLoginProvider { get; }
        public MessageService MessageService { get; }
        public CommonLinkUtility CommonLinkUtility { get; }
        public DocumentServiceConnector DocumentServiceConnector { get; }
        public FolderContentWrapperHelper FolderContentWrapperHelper { get; }
        public WordpressToken WordpressToken { get; }
        public WordpressHelper WordpressHelper { get; }
        public ConsumerFactory ConsumerFactory { get; }
        public EasyBibHelper EasyBibHelper { get; }
        public ChunkedUploadSessionHelper ChunkedUploadSessionHelper { get; }
        public ProductEntryPoint ProductEntryPoint { get; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileStorageService"></param>
        public FilesController(
            ApiContext context,
            FileStorageService fileStorageService,
            GlobalFolderHelper globalFolderHelper,
            FileWrapperHelper fileWrapperHelper,
            FilesSettingsHelper filesSettingsHelper,
            FilesLinkUtility filesLinkUtility,
            FileUploader fileUploader,
            DocumentServiceHelper documentServiceHelper,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FolderWrapperHelper folderWrapperHelper,
            FileOperationWraperHelper fileOperationWraperHelper,
            FileShareWrapperHelper fileShareWrapperHelper,
            FileShareParamsHelper fileShareParamsHelper,
            EntryManager entryManager,
            UserManager userManager,
            WebItemSecurity webItemSecurity,
            CoreBaseSettings coreBaseSettings,
            ThirdpartyConfiguration thirdpartyConfiguration,
            BoxLoginProvider boxLoginProvider,
            DropboxLoginProvider dropboxLoginProvider,
            GoogleLoginProvider googleLoginProvider,
            OneDriveLoginProvider oneDriveLoginProvider,
            MessageService messageService,
            CommonLinkUtility commonLinkUtility,
            DocumentServiceConnector documentServiceConnector,
            FolderContentWrapperHelper folderContentWrapperHelper,
            WordpressToken wordpressToken,
            WordpressHelper wordpressHelper,
            ConsumerFactory consumerFactory,
            EasyBibHelper easyBibHelper,
            ChunkedUploadSessionHelper chunkedUploadSessionHelper,
            ProductEntryPoint productEntryPoint)
        {
            ApiContext = context;
            FileStorageService = fileStorageService;
            GlobalFolderHelper = globalFolderHelper;
            FileWrapperHelper = fileWrapperHelper;
            FilesSettingsHelper = filesSettingsHelper;
            FilesLinkUtility = filesLinkUtility;
            FileUploader = fileUploader;
            DocumentServiceHelper = documentServiceHelper;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            FolderWrapperHelper = folderWrapperHelper;
            FileOperationWraperHelper = fileOperationWraperHelper;
            FileShareWrapperHelper = fileShareWrapperHelper;
            FileShareParamsHelper = fileShareParamsHelper;
            EntryManager = entryManager;
            UserManager = userManager;
            WebItemSecurity = webItemSecurity;
            CoreBaseSettings = coreBaseSettings;
            ThirdpartyConfiguration = thirdpartyConfiguration;
            BoxLoginProvider = boxLoginProvider;
            DropboxLoginProvider = dropboxLoginProvider;
            GoogleLoginProvider = googleLoginProvider;
            OneDriveLoginProvider = oneDriveLoginProvider;
            MessageService = messageService;
            CommonLinkUtility = commonLinkUtility;
            DocumentServiceConnector = documentServiceConnector;
            FolderContentWrapperHelper = folderContentWrapperHelper;
            WordpressToken = wordpressToken;
            WordpressHelper = wordpressHelper;
            ConsumerFactory = consumerFactory;
            EasyBibHelper = easyBibHelper;
            ChunkedUploadSessionHelper = chunkedUploadSessionHelper;
            ProductEntryPoint = productEntryPoint;
        }

        [Read("info")]
        public Module GetModule()
        {
            ProductEntryPoint.Init();
            return new Module(ProductEntryPoint, true);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the current user 'My Documents' section
        /// </summary>
        /// <short>
        /// My folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>My folder contents</returns>
        [Read("@my")]
        public FolderContentWrapper GetMyFolder(Guid userIdOrGroupId, FilterType filterType)
        {
            return ToFolderContentWrapper(GlobalFolderHelper.FolderMy, userIdOrGroupId, filterType);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the current user 'Projects Documents' section
        /// </summary>
        /// <short>
        /// Projects folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>Projects folder contents</returns>
        [Read("@projects")]
        public FolderContentWrapper GetProjectsFolder(Guid userIdOrGroupId, FilterType filterType)
        {
            return ToFolderContentWrapper(GlobalFolderHelper.FolderProjects, userIdOrGroupId, filterType);
        }


        /// <summary>
        /// Returns the detailed list of files and folders located in the 'Common Documents' section
        /// </summary>
        /// <short>
        /// Common folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>Common folder contents</returns>
        [Read("@common")]
        public FolderContentWrapper GetCommonFolder(Guid userIdOrGroupId, FilterType filterType)
        {
            return ToFolderContentWrapper(GlobalFolderHelper.FolderCommon, userIdOrGroupId, filterType);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the 'Shared with Me' section
        /// </summary>
        /// <short>
        /// Shared folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>Shared folder contents</returns>
        [Read("@share")]
        public FolderContentWrapper GetShareFolder(Guid userIdOrGroupId, FilterType filterType)
        {
            return ToFolderContentWrapper(GlobalFolderHelper.FolderShare, userIdOrGroupId, filterType);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the 'Recycle Bin' section
        /// </summary>
        /// <short>
        /// Trash folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>Trash folder contents</returns>
        [Read("@trash")]
        public FolderContentWrapper GetTrashFolder(Guid userIdOrGroupId, FilterType filterType)
        {
            return ToFolderContentWrapper(GlobalFolderHelper.FolderTrash, userIdOrGroupId, filterType);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the folder with the ID specified in the request
        /// </summary>
        /// <short>
        /// Folder by ID
        /// </short>
        /// <category>Folders</category>
        /// <param name="folderId">Folder ID</param>
        /// <param name="userIdOrGroupId" optional="true">User or group ID</param>
        /// <param name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5) or ImagesOnly (7)">Filter type</param>
        /// <returns>Folder contents</returns>
        [Read("{folderId}", order: int.MaxValue)]
        public FolderContentWrapper GetFolder(string folderId, Guid userIdOrGroupId, FilterType filterType)
        {
            return ToFolderContentWrapper(folderId, userIdOrGroupId, filterType).NotFoundIfNull();

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
        [Create("@my/upload")]
        public object UploadFileToMy(UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return UploadFile(GlobalFolderHelper.FolderMy.ToString(), uploadModel);
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
        [Create("@common/upload")]
        public object UploadFileToCommon(UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return UploadFile(GlobalFolderHelper.FolderCommon.ToString(), uploadModel);
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
        [Create("{folderId}/upload")]
        public object UploadFile(string folderId, UploadModel uploadModel)
        {
            if (uploadModel.StoreOriginalFileFlag.HasValue)
            {
                FilesSettingsHelper.StoreOriginalFiles = uploadModel.StoreOriginalFileFlag.Value;
            }

            if (uploadModel.Files != null && uploadModel.Files.Any())
            {
                if (uploadModel.Files.Count() == 1)
                {
                    //Only one file. return it
                    var postedFile = uploadModel.Files.First();
                    return InsertFile(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus);
                }
                //For case with multiple files
                return uploadModel.Files.Select(postedFile => InsertFile(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)).ToList();
            }
            if (uploadModel.File != null)
            {
                var fileName = "file" + MimeMapping.GetExtention(uploadModel.ContentType.MediaType);
                if (uploadModel.ContentDisposition != null)
                {
                    fileName = uploadModel.ContentDisposition.FileName;
                }

                return InsertFile(folderId, uploadModel.File, fileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus);
            }
            throw new InvalidOperationException("No input files");
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
        [Create("@my/insert")]
        public FileWrapper InsertFileToMy(Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            return InsertFile(GlobalFolderHelper.FolderMy.ToString(), file, title, createNewIfExist, keepConvertStatus);
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
        [Create("@common/insert")]
        public FileWrapper InsertFileToCommon(Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            return InsertFile(GlobalFolderHelper.FolderCommon.ToString(), file, title, createNewIfExist, keepConvertStatus);
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
        [Create("{folderId}/insert")]
        public FileWrapper InsertFile(string folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            try
            {
                var resultFile = FileUploader.Exec(folderId, title, file.Length, file, createNewIfExist ?? !FilesSettingsHelper.UpdateIfExist, !keepConvertStatus);
                return FileWrapperHelper.Get(resultFile);
            }
            catch (FileNotFoundException e)
            {
                throw new ItemNotFoundException("File not found", e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new ItemNotFoundException("Folder not found", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileId"></param>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Update("{fileId}/update")]
        public FileWrapper UpdateFileStream(Stream file, string fileId, bool encrypted = false)
        {
            try
            {
                var resultFile = FileStorageService.UpdateFileStream(fileId, file, encrypted);
                return FileWrapperHelper.Get(resultFile);
            }
            catch (FileNotFoundException e)
            {
                throw new ItemNotFoundException("File not found", e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="fileExtension"></param>
        /// <param name="downloadUri"></param>
        /// <param name="stream"></param>
        /// <param name="doc"></param>
        /// <param name="forcesave"></param>
        /// <category>Files</category>
        /// <returns></returns>
        [Update("file/{fileId}/saveediting")]
        public FileWrapper SaveEditing(string fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
        {
            return FileWrapperHelper.Get(FileStorageService.SaveEditing(fileId, fileExtension, downloadUri, stream, doc, forcesave));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="editingAlone"></param>
        /// <param name="doc"></param>
        /// <category>Files</category>
        /// <returns></returns>
        [Create("file/{fileId}/startedit")]
        public string StartEdit(string fileId, bool editingAlone, string doc)
        {
            return FileStorageService.StartEdit(fileId, editingAlone, doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="tabId"></param>
        /// <param name="docKeyForTrack"></param>
        /// <param name="doc"></param>
        /// <param name="isFinish"></param>
        /// <category>Files</category>
        /// <returns></returns>
        [Read("file/{fileId}/trackeditfile")]
        public KeyValuePair<bool, string> TrackEditFile(string fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FileStorageService.TrackEditFile(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="version"></param>
        /// <param name="doc"></param>
        /// <category>Files</category>
        /// <returns></returns>
        [Read("file/{fileId}/openedit")]
        public Configuration OpenEdit(string fileId, int version, string doc)
        {
            Configuration configuration;
            DocumentServiceHelper.GetParams(fileId, version, doc, true, true, true, out configuration);
            configuration.Type = Configuration.EditorType.External;

            configuration.Token = DocumentServiceHelper.GetSignature(configuration);
            return configuration;
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
        /// Each chunk can have different length but its important what length is multiple of <b>512</b> and greater or equal than <b>5 mb</b>. Last chunk can have any size.
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
        [Create("{folderId}/upload/create_session")]
        public object CreateUploadSession(string folderId, string fileName, long fileSize, string relativePath, bool encrypted)
        {
            var file = FileUploader.VerifyChunkedUpload(folderId, fileName, fileSize, FilesSettingsHelper.UpdateIfExist, relativePath);

            if (FilesLinkUtility.IsLocalFileUploader)
            {
                var session = FileUploader.InitiateUpload(file.FolderID.ToString(), (file.ID ?? "").ToString(), file.Title, file.ContentLength, encrypted);

                var response = ChunkedUploadSessionHelper.ToResponseObject(session, true);
                return new
                {
                    success = true,
                    data = response
                };
            }

            var createSessionUrl = FilesLinkUtility.GetInitiateUploadSessionUrl(TenantManager.GetCurrentTenant().TenantId, file.FolderID, file.ID, file.Title, file.ContentLength, encrypted, SecurityContext);
            var request = (HttpWebRequest)WebRequest.Create(createSessionUrl);
            request.Method = "POST";
            request.ContentLength = 0;

            // hack for uploader.onlyoffice.com in api requests
            var rewriterHeader = ApiContext.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
            if (!string.IsNullOrEmpty(rewriterHeader))
            {
                request.Headers[HttpRequestExtensions.UrlRewriterHeader] = rewriterHeader;
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                return JObject.Parse(new StreamReader(responseStream).ReadToEnd()); //result is json string
            }
        }

        /// <summary>
        /// Creates a text (.txt) file in 'My Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt in 'My'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("@my/text")]
        public FileWrapper CreateTextFileInMy(string title, string content)
        {
            return CreateTextFile(GlobalFolderHelper.FolderMy.ToString(), title, content);
        }

        /// <summary>
        /// Creates a text (.txt) file in 'Common Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt in 'Common'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("@common/text")]
        public FileWrapper CreateTextFileInCommon(string title, string content)
        {
            return CreateTextFile(GlobalFolderHelper.FolderCommon.ToString(), title, content);
        }

        /// <summary>
        /// Creates a text (.txt) file in the selected folder with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt</short>
        /// <category>File Creation</category>
        /// <param name="folderId">Folder ID</param>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("{folderId}/text")]
        public FileWrapper CreateTextFile(string folderId, string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            //Try detect content
            var extension = ".txt";
            if (!string.IsNullOrEmpty(content))
            {
                if (Regex.IsMatch(content, @"<([^\s>]*)(\s[^<]*)>"))
                {
                    extension = ".html";
                }
            }
            return CreateFile(folderId, title, content, extension);
        }

        private FileWrapper CreateFile(string folderId, string title, string content, string extension)
        {
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var file = FileUploader.Exec(folderId,
                                  title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension),
                                  memStream.Length, memStream);
                return FileWrapperHelper.Get(file);
            }
        }

        /// <summary>
        /// Creates an html (.html) file in the selected folder with the title and contents sent in the request
        /// </summary>
        /// <short>Create html</short>
        /// <category>File Creation</category>
        /// <param name="folderId">Folder ID</param>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("{folderId}/html")]
        public FileWrapper CreateHtmlFile(string folderId, string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            return CreateFile(folderId, title, content, ".html");
        }

        /// <summary>
        /// Creates an html (.html) file in 'My Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create html in 'My'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("@my/html")]
        public FileWrapper CreateHtmlFileInMy(string title, string content)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderMy.ToString(), title, content);
        }


        /// <summary>
        /// Creates an html (.html) file in 'Common Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create html in 'Common'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>        
        [Create("@common/html")]
        public FileWrapper CreateHtmlFileInCommon(string title, string content)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderCommon.ToString(), title, content);
        }


        /// <summary>
        /// Creates a new folder with the title sent in the request. The ID of a parent folder can be also specified.
        /// </summary>
        /// <short>
        /// New folder
        /// </short>
        /// <category>Folders</category>
        /// <param name="folderId">Parent folder ID</param>
        /// <param name="title">Title of new folder</param>
        /// <returns>New folder contents</returns>
        [Create("folder/{folderId}")]
        public FolderWrapper CreateFolder(string folderId, FolderModel folderModel)
        {
            var folder = FileStorageService.CreateNewFolder(folderId, folderModel.Title);
            return FolderWrapperHelper.Get(folder);
        }

        /// <summary>
        /// Creates a new file in the 'My Documents' section with the title sent in the request
        /// </summary>
        /// <short>Create file</short>
        /// <category>File Creation</category>
        /// <param name="title" remark="Allowed values: the file must have one of the following extensions: DOCX, XLSX, PPTX">File title</param>
        /// <remarks>In case the extension for the file title differs from DOCX/XLSX/PPTX and belongs to one of the known text, spreadsheet or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not set or is unknown, the DOCX extension will be added to the file title.</remarks>
        /// <returns>New file info</returns>
        [Create("@my/file")]
        public FileWrapper CreateFile([FromBody]FileModelFull model)
        {
            return CreateFile(GlobalFolderHelper.FolderMy.ToString(), model);
        }

        /// <summary>
        /// Creates a new file in the specified folder with the title sent in the request
        /// </summary>
        /// <short>Create file</short>
        /// <category>File Creation</category>
        /// <param name="folderId">Folder ID</param>
        /// <param name="title" remark="Allowed values: the file must have one of the following extensions: DOCX, XLSX, PPTX">File title</param>
        /// <remarks>In case the extension for the file title differs from DOCX/XLSX/PPTX and belongs to one of the known text, spreadsheet or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not set or is unknown, the DOCX extension will be added to the file title.</remarks>
        /// <returns>New file info</returns>
        [Create("{folderId}/file")]
        public FileWrapper CreateFile(string folderId, [FromBody]FileModelFull model)
        {
            var file = FileStorageService.CreateNewFile(new FileModel { ParentId = folderId, Title = model.Title });
            return FileWrapperHelper.Get(file);
        }

        /// <summary>
        /// Renames the selected folder to the new title specified in the request
        /// </summary>
        /// <short>
        /// Rename folder
        /// </short>
        /// <category>Folders</category>
        /// <param name="folderId">Folder ID</param>
        /// <param name="title">New title</param>
        /// <returns>Folder contents</returns>
        [Update("folder/{folderId}")]
        public FolderWrapper RenameFolder(string folderId, FolderModel folderModel)
        {
            var folder = FileStorageService.FolderRename(folderId, folderModel.Title);
            return FolderWrapperHelper.Get(folder);
        }

        /// <summary>
        /// Returns a detailed information about the folder with the ID specified in the request
        /// </summary>
        /// <short>Folder information</short>
        /// <category>Folders</category>
        /// <returns>Folder info</returns>
        [Read("folder/{folderId}")]
        public FolderWrapper GetFolderInfo(string folderId)
        {
            var folder = FileStorageService.GetFolder(folderId).NotFoundIfNull("Folder not found");

            return FolderWrapperHelper.Get(folder);
        }

        /// <summary>
        /// Returns parent folders
        /// </summary>
        /// <param name="folderId"></param>
        /// <category>Folders</category>
        /// <returns>Parent folders</returns>
        [Read("folder/{folderId}/path")]
        public IEnumerable<FolderWrapper> GetFolderPath(string folderId)
        {
            return EntryManager.GetBreadCrumbs(folderId).Select(FolderWrapperHelper.Get);
        }

        /// <summary>
        /// Returns a detailed information about the file with the ID specified in the request
        /// </summary>
        /// <short>File information</short>
        /// <category>Files</category>
        /// <returns>File info</returns>
        [Read("file/{fileId}")]
        public FileWrapper GetFileInfo(string fileId, int version = -1)
        {
            var file = FileStorageService.GetFile(fileId, version).NotFoundIfNull("File not found");
            return FileWrapperHelper.Get(file);
        }

        /// <summary>
        ///     Updates the information of the selected file with the parameters specified in the request
        /// </summary>
        /// <short>Update file info</short>
        /// <category>Files</category>
        /// <param name="fileId">File ID</param>
        /// <param name="title">New title</param>
        /// <param name="lastVersion">File last version number</param>
        /// <returns>File info</returns>
        [Update("file/{fileId}")]
        public FileWrapper UpdateFile(string fileId, [FromBody]FileModelFull model)
        {
            if (!string.IsNullOrEmpty(model.Title))
                FileStorageService.FileRename(fileId.ToString(CultureInfo.InvariantCulture), model.Title);

            if (model.Version > 0)
                FileStorageService.UpdateToVersion(fileId.ToString(CultureInfo.InvariantCulture), model.Version);

            return GetFileInfo(fileId);
        }

        /// <summary>
        /// Deletes the file with the ID specified in the request
        /// </summary>
        /// <short>Delete file</short>
        /// <category>Files</category>
        /// <param name="fileId">File ID</param>
        /// <param name="deleteAfter">Delete after finished</param>
        /// <param name="immediately">Don't move to the Recycle Bin</param>
        /// <returns>Operation result</returns>
        [Delete("file/{fileId}")]
        public IEnumerable<FileOperationWraper> DeleteFile(string fileId, bool deleteAfter, bool immediately)
        {
            var model = new DeleteBatchModel
            {
                FileIds = new List<string> { fileId },
                DeleteAfter = deleteAfter,
                Immediately = immediately
            };

            return DeleteBatchItems(model);
        }

        /// <summary>
        ///  Start conversion
        /// </summary>
        /// <short>Convert</short>
        /// <category>File operations</category>
        /// <param name="fileId"></param>
        /// <returns>Operation result</returns>
        [Update("file/{fileId}/checkconversion")]
        public IEnumerable<ConversationResult> StartConversion(string fileId)
        {
            return CheckConversion(fileId, true);
        }

        /// <summary>
        ///  Check conversion status
        /// </summary>
        /// <short>Convert</short>
        /// <category>File operations</category>
        /// <param name="fileId"></param>
        /// <param name="start"></param>
        /// <returns>Operation result</returns>
        [Read("file/{fileId}/checkconversion")]
        public IEnumerable<ConversationResult> CheckConversion(string fileId, bool start)
        {
            return FileStorageService.CheckConversion(new ItemList<ItemList<string>>
            {
                new ItemList<string> { fileId, "0", start.ToString() }
            })
            .Select(r =>
            {
                var o = new ConversationResult
                {
                    Id = r.Id,
                    Error = r.Error,
                    OperationType = r.OperationType,
                    Processed = r.Processed,
                    Progress = r.Progress,
                    Source = r.Source,
                };
                if (!string.IsNullOrEmpty(r.Result))
                {
                    var jResult = JObject.Parse(r.Result);
                    o.File = GetFileInfo(jResult.Value<string>("id"), jResult.Value<int>("version"));
                }
                return o;
            });
        }

        /// <summary>
        /// Deletes the folder with the ID specified in the request
        /// </summary>
        /// <short>Delete folder</short>
        /// <category>Folders</category>
        /// <param name="folderId">Folder ID</param>
        /// <param name="deleteAfter">Delete after finished</param>
        /// <param name="immediately">Don't move to the Recycle Bin</param>
        /// <returns>Operation result</returns>
        [Delete("folder/{folderId}")]
        public IEnumerable<FileOperationWraper> DeleteFolder(string folderId, bool deleteAfter, bool immediately)
        {
            var model = new DeleteBatchModel
            {
                FolderIds = new List<string> { folderId },
                DeleteAfter = deleteAfter,
                Immediately = immediately
            };

            return DeleteBatchItems(model);
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
        public IEnumerable<FileWrapper> MoveOrCopyBatchCheck(BatchModel batchModel)
        {
            var itemList = new ItemList<string>();

            itemList.AddRange((batchModel.FolderIds ?? new List<string>()).Select(x => "folder_" + x));
            itemList.AddRange((batchModel.FileIds ?? new List<string>()).Select(x => "file_" + x));

            var ids = FileStorageService.MoveOrCopyFilesCheck(itemList, batchModel.DestFolderId).Keys.Select(id => "file_" + id);

            var entries = FileStorageService.GetItems(new ItemList<string>(ids), FilterType.FilesOnly, false, "", "");
            return entries.Select(x => FileWrapperHelper.Get((Files.Core.File)x));
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
        public IEnumerable<FileOperationWraper> MoveBatchItems(BatchModel batchModel)
        {
            var itemList = new ItemList<string>();

            itemList.AddRange((batchModel.FolderIds ?? new List<string>()).Select(x => "folder_" + x));
            itemList.AddRange((batchModel.FileIds ?? new List<string>()).Select(x => "file_" + x));

            return FileStorageService.MoveOrCopyItems(itemList, batchModel.DestFolderId, batchModel.ConflictResolveType, false, batchModel.DeleteAfter).Select(FileOperationWraperHelper.Get);
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
        public IEnumerable<FileOperationWraper> CopyBatchItems(BatchModel batchModel)
        {
            var itemList = new ItemList<string>();

            itemList.AddRange((batchModel.FolderIds ?? new List<string>()).Select(x => "folder_" + x));
            itemList.AddRange((batchModel.FileIds ?? new List<string>()).Select(x => "file_" + x));

            return FileStorageService.MoveOrCopyItems(itemList, batchModel.DestFolderId, batchModel.ConflictResolveType, true, batchModel.DeleteAfter).Select(FileOperationWraperHelper.Get);
        }

        /// <summary>
        ///   Marks all files and folders as read
        /// </summary>
        /// <short>Mark as read</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Update("fileops/markasread")]
        public IEnumerable<FileOperationWraper> MarkAsRead(BaseBatchModel model)
        {
            var itemList = new ItemList<string>();

            itemList.AddRange((model.FolderIds ?? new List<string>()).Select(x => "folder_" + x));
            itemList.AddRange((model.FileIds ?? new List<string>()).Select(x => "file_" + x));

            return FileStorageService.MarkAsRead(itemList).Select(FileOperationWraperHelper.Get);
        }

        /// <summary>
        ///  Finishes all the active file operations
        /// </summary>
        /// <short>Finish all</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Update("fileops/terminate")]
        public IEnumerable<FileOperationWraper> TerminateTasks()
        {
            return FileStorageService.TerminateTasks().Select(FileOperationWraperHelper.Get);
        }


        /// <summary>
        ///  Returns the list of all active file operations
        /// </summary>
        /// <short>Get file operations list</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Read("fileops")]
        public IEnumerable<FileOperationWraper> GetOperationStatuses()
        {
            return FileStorageService.GetTasksStatuses().Select(FileOperationWraperHelper.Get);
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
        public IEnumerable<FileOperationWraper> BulkDownload(DownloadModel model)
        {
            var itemList = new Dictionary<string, string>();

            foreach (var fileId in model.FileConvertIds.Where(fileId => !itemList.ContainsKey(fileId.Key)))
            {
                itemList.Add("file_" + fileId.Key, fileId.Value);
            }

            foreach (var fileId in model.FileIds.Where(fileId => !itemList.ContainsKey(fileId)))
            {
                itemList.Add("file_" + fileId, string.Empty);
            }

            foreach (var folderId in model.FolderIds.Where(folderId => !itemList.ContainsKey(folderId)))
            {
                itemList.Add("folder_" + folderId, string.Empty);
            }

            return FileStorageService.BulkDownload(itemList).Select(FileOperationWraperHelper.Get);
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
        public IEnumerable<FileOperationWraper> DeleteBatchItems(DeleteBatchModel batch)
        {
            var itemList = new ItemList<string>();

            itemList.AddRange((batch.FolderIds ?? new List<string>()).Select(x => "folder_" + x));
            itemList.AddRange((batch.FileIds ?? new List<string>()).Select(x => "file_" + x));

            return FileStorageService.DeleteItems("delete", itemList, false, batch.DeleteAfter, batch.Immediately).Select(FileOperationWraperHelper.Get);
        }

        /// <summary>
        ///   Deletes all files and folders from the recycle bin
        /// </summary>
        /// <short>Clear recycle bin</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Update("fileops/emptytrash")]
        public IEnumerable<FileOperationWraper> EmptyTrash()
        {
            return FileStorageService.EmptyTrash().Select(FileOperationWraperHelper.Get);
        }

        /// <summary>
        /// Returns the detailed information about all the available file versions with the ID specified in the request
        /// </summary>
        /// <short>File versions</short>
        /// <category>Files</category>
        /// <param name="fileId">File ID</param>
        /// <returns>File information</returns>
        [Read("file/{fileId}/history")]
        public IEnumerable<FileWrapper> GetFileVersionInfo(string fileId)
        {
            var files = FileStorageService.GetFileHistory(fileId);
            return files.Select(FileWrapperHelper.Get);
        }

        /// <summary>
        /// Change version history
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="version">Version of history</param>
        /// <param name="continueVersion">Mark as version or revision</param>
        /// <category>Files</category>
        /// <returns></returns>
        [Update("file/{fileId}/history")]
        public IEnumerable<FileWrapper> ChangeHistory(string fileId, int version, bool continueVersion)
        {
            var history = FileStorageService.CompleteVersion(fileId, version, continueVersion).Value;
            return history.Select(FileWrapperHelper.Get);
        }

        /// <summary>
        /// Returns the detailed information about shared file with the ID specified in the request
        /// </summary>
        /// <short>File sharing</short>
        /// <category>Sharing</category>
        /// <param name="fileId">File ID</param>
        /// <returns>Shared file information</returns>
        [Read("file/{fileId}/share")]
        public IEnumerable<FileShareWrapper> GetFileSecurityInfo(string fileId)
        {
            var fileShares = FileStorageService.GetSharedInfo(new ItemList<string> { string.Format("file_{0}", fileId) });
            return fileShares.Select(FileShareWrapperHelper.Get);
        }

        /// <summary>
        /// Returns the detailed information about shared folder with the ID specified in the request
        /// </summary>
        /// <short>Folder sharing</short>
        /// <param name="folderId">Folder ID</param>
        /// <category>Sharing</category>
        /// <returns>Shared folder information</returns>
        [Read("folder/{folderId}/share")]
        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(string folderId)
        {
            var fileShares = FileStorageService.GetSharedInfo(new ItemList<string> { string.Format("folder_{0}", folderId) });
            return fileShares.Select(FileShareWrapperHelper.Get);
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
        public IEnumerable<FileShareWrapper> SetFileSecurityInfo(string fileId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            if (share != null && share.Any())
            {
                var list = new ItemList<AceWrapper>(share.Select(FileShareParamsHelper.ToAceObject));
                var aceCollection = new AceCollection
                {
                    Entries = new ItemList<string> { "file_" + fileId },
                    Aces = list,
                    Message = sharingMessage
                };
                FileStorageService.SetAceObject(aceCollection, notify);
            }
            return GetFileSecurityInfo(fileId);
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
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfo(string folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            if (share != null && share.Any())
            {
                var list = new ItemList<AceWrapper>(share.Select(FileShareParamsHelper.ToAceObject));
                var aceCollection = new AceCollection
                {
                    Entries = new ItemList<string> { "folder_" + folderId },
                    Aces = list,
                    Message = sharingMessage
                };
                FileStorageService.SetAceObject(aceCollection, notify);
            }

            return GetFolderSecurityInfo(folderId);
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
        public bool RemoveSecurityInfo(BaseBatchModel model)
        {
            var itemList = new ItemList<string>();

            itemList.AddRange((model.FolderIds ?? new List<string>()).Select(x => "folder_" + x));
            itemList.AddRange((model.FileIds ?? new List<string>()).Select(x => "file_" + x));

            FileStorageService.RemoveAce(itemList);

            return true;
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
        [Update("{fileId}/sharedlink")]
        public string GenerateSharedLink(string fileId, FileShare share)
        {
            var file = GetFileInfo(fileId);

            var objectId = "file_" + file.Id;
            var sharedInfo = FileStorageService.GetSharedInfo(new ItemList<string> { objectId }).Find(r => r.SubjectId == FileConstant.ShareLinkId);
            if (sharedInfo == null || sharedInfo.Share != share)
            {
                var list = new ItemList<AceWrapper>
                    {
                        new AceWrapper
                            {
                                SubjectId = FileConstant.ShareLinkId,
                                SubjectGroup = true,
                                Share = share
                            }
                    };
                var aceCollection = new AceCollection
                {
                    Entries = new ItemList<string> { objectId },
                    Aces = list
                };
                FileStorageService.SetAceObject(aceCollection, false);
                sharedInfo = FileStorageService.GetSharedInfo(new ItemList<string> { objectId }).Find(r => r.SubjectId == FileConstant.ShareLinkId);
            }

            return sharedInfo.Link;
        }

        /// <summary>
        ///   Get a list of available providers
        /// </summary>
        /// <category>Third-Party Integration</category>
        /// <returns>List of provider key</returns>
        /// <remarks>List of provider key: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive</remarks>
        /// <returns></returns>
        [Read("thirdparty/capabilities")]
        public List<List<string>> Capabilities()
        {
            var result = new List<List<string>>();

            if (UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor(UserManager)
                || (!UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID)
                    && !WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, SecurityContext.CurrentAccount.ID)
                    && !FilesSettingsHelper.EnableThirdParty
                    && !CoreBaseSettings.Personal))
            {
                return result;
            }

            if (ThirdpartyConfiguration.SupportBoxInclusion)
            {
                result.Add(new List<string> { "Box", BoxLoginProvider.Instance.ClientID, BoxLoginProvider.Instance.RedirectUri });
            }
            if (ThirdpartyConfiguration.SupportDropboxInclusion)
            {
                result.Add(new List<string> { "DropboxV2", DropboxLoginProvider.Instance.ClientID, DropboxLoginProvider.Instance.RedirectUri });
            }
            if (ThirdpartyConfiguration.SupportGoogleDriveInclusion)
            {
                result.Add(new List<string> { "GoogleDrive", GoogleLoginProvider.Instance.ClientID, GoogleLoginProvider.Instance.RedirectUri });
            }
            if (ThirdpartyConfiguration.SupportOneDriveInclusion)
            {
                result.Add(new List<string> { "OneDrive", OneDriveLoginProvider.Instance.ClientID, OneDriveLoginProvider.Instance.RedirectUri });
            }
            if (ThirdpartyConfiguration.SupportSharePointInclusion)
            {
                result.Add(new List<string> { "SharePoint" });
            }
            if (ThirdpartyConfiguration.SupportYandexInclusion)
            {
                result.Add(new List<string> { "Yandex" });
            }
            if (ThirdpartyConfiguration.SupportWebDavInclusion)
            {
                result.Add(new List<string> { "WebDav" });
            }

            //Obsolete BoxNet, DropBox, Google, SkyDrive,

            return result;
        }

        /// <summary>
        ///   Saves the third party file storage service account
        /// </summary>
        /// <short>Save third party account</short>
        /// <param name="url">Connection url for SharePoint</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <param name="token">Authentication token</param>
        /// <param name="isCorporate"></param>
        /// <param name="customerTitle">Title</param>
        /// <param name="providerKey">Provider Key</param>
        /// <param name="providerId">Provider ID</param>
        /// <category>Third-Party Integration</category>
        /// <returns>Folder contents</returns>
        /// <remarks>List of provider key: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive</remarks>
        /// <exception cref="ArgumentException"></exception>
        [Create("thirdparty")]
        public FolderWrapper SaveThirdParty(
            string url,
            string login,
            string password,
            string token,
            bool isCorporate,
            string customerTitle,
            string providerKey,
            string providerId)
        {
            var thirdPartyParams = new ThirdPartyParams
            {
                AuthData = new AuthData(url, login, password, token),
                Corporate = isCorporate,
                CustomerTitle = customerTitle,
                ProviderId = providerId,
                ProviderKey = providerKey,
            };

            var folder = FileStorageService.SaveThirdParty(thirdPartyParams);

            return FolderWrapperHelper.Get(folder);
        }

        /// <summary>
        ///    Returns the list of all connected third party services
        /// </summary>
        /// <category>Third-Party Integration</category>
        /// <short>Get third party list</short>
        /// <returns>Connected providers</returns>
        [Read("thirdparty")]
        public IEnumerable<ThirdPartyParams> GetThirdPartyAccounts()
        {
            return FileStorageService.GetThirdParty();
        }

        /// <summary>
        ///    Returns the list of third party services connected in the 'Common Documents' section
        /// </summary>
        /// <category>Third-Party Integration</category>
        /// <short>Get third party folder</short>
        /// <returns>Connected providers folder</returns>
        [Read("thirdparty/common")]
        public IEnumerable<Folder> GetCommonThirdPartyFolders()
        {
            var parent = FileStorageService.GetFolder(GlobalFolderHelper.FolderCommon.ToString());
            return EntryManager.GetThirpartyFolders(parent);
        }

        /// <summary>
        ///   Removes the third party file storage service account with the ID specified in the request
        /// </summary>
        /// <param name="providerId">Provider ID. Provider id is part of folder id.
        /// Example, folder id is "sbox-123", then provider id is "123"
        /// </param>
        /// <short>Remove third party account</short>
        /// <category>Third-Party Integration</category>
        /// <returns>Folder id</returns>
        ///<exception cref="ArgumentException"></exception>
        [Delete("thirdparty/{providerId:int}")]
        public object DeleteThirdParty(int providerId)
        {
            return FileStorageService.DeleteThirdParty(providerId.ToString(CultureInfo.InvariantCulture));

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[Read(@"@search/{query}")]
        //public IEnumerable<FileEntryWrapper> Search(string query)
        //{
        //    var searcher = new SearchHandler();
        //    var files = searcher.SearchFiles(query).Select(r => (FileEntryWrapper)FileWrapperHelper.Get(r));
        //    var folders = searcher.SearchFolders(query).Select(f => (FileEntryWrapper)FolderWrapperHelper.Get(f));

        //    return files.Concat(folders);
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"storeoriginal")]
        public bool StoreOriginal(bool set)
        {
            return FileStorageService.StoreOriginal(set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="save"></param>
        /// <visible>false</visible>
        /// <returns></returns>
        [Update(@"hideconfirmconvert")]
        public bool HideConfirmConvert(bool save)
        {
            return FileStorageService.HideConfirmConvert(save);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"updateifexist")]
        public bool UpdateIfExist(bool set)
        {
            return FileStorageService.UpdateIfExist(set);
        }

        /// <summary>
        ///  Checking document service location
        /// </summary>
        /// <param name="docServiceUrl">Document editing service Domain</param>
        /// <param name="docServiceUrlInternal">Document command service Domain</param>
        /// <param name="docServiceUrlPortal">Community Server Address</param>
        /// <returns></returns>
        [Update("docservice")]
        public IEnumerable<string> CheckDocServiceUrl(string docServiceUrl, string docServiceUrlInternal, string docServiceUrlPortal)
        {
            FilesLinkUtility.DocServiceUrl = docServiceUrl;
            FilesLinkUtility.DocServiceUrlInternal = docServiceUrlInternal;
            FilesLinkUtility.DocServicePortalUrl = docServiceUrlPortal;

            MessageService.Send(MessageAction.DocumentServiceLocationSetting);

            var https = new Regex(@"^https://", RegexOptions.IgnoreCase);
            var http = new Regex(@"^http://", RegexOptions.IgnoreCase);
            if (https.IsMatch(CommonLinkUtility.GetFullAbsolutePath("")) && http.IsMatch(FilesLinkUtility.DocServiceUrl))
            {
                throw new Exception("Mixed Active Content is not allowed. HTTPS address for Document Server is required.");
            }

            DocumentServiceConnector.CheckDocServiceUrl();

            return new[]
                {
                    FilesLinkUtility.DocServiceUrl,
                    FilesLinkUtility.DocServiceUrlInternal,
                    FilesLinkUtility.DocServicePortalUrl
                };
        }

        /// <visible>false</visible>
        [Read("docservice")]
        public object GetDocServiceUrl(bool version)
        {
            var url = CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.DocServiceApiUrl);
            if (!version)
            {
                return url;
            }

            var dsVersion = DocumentServiceConnector.GetVersion();

            return new
            {
                version = dsVersion,
                docServiceUrlApi = url,
            };
        }


        private FolderContentWrapper ToFolderContentWrapper(object folderId, Guid userIdOrGroupId, FilterType filterType)
        {
            SortedByType sortBy;
            if (!Enum.TryParse(ApiContext.SortBy, true, out sortBy))
                sortBy = SortedByType.AZ;
            var startIndex = Convert.ToInt32(ApiContext.StartIndex);
            return FolderContentWrapperHelper.Get(FileStorageService.GetFolderItems(folderId.ToString(),
                                                                               startIndex,
                                                                               Convert.ToInt32(ApiContext.Count) - 1, //NOTE: in ApiContext +1
                                                                               filterType,
                                                                               filterType == FilterType.ByUser,
                                                                               userIdOrGroupId.ToString(),
                                                                               ApiContext.FilterValue,
                                                                               false,
                                                                               false,
                                                                               new OrderBy(sortBy, !ApiContext.SortDescending)),
                                            startIndex);
        }

        #region wordpress

        /// <visible>false</visible>
        [Read("wordpress-info")]
        public object GetWordpressInfo()
        {
            var token = WordpressToken.GetToken();
            if (token != null)
            {
                var meInfo = WordpressHelper.GetWordpressMeInfo(token.AccessToken);
                var blogId = JObject.Parse(meInfo).Value<string>("token_site_id");
                var wordpressUserName = JObject.Parse(meInfo).Value<string>("username");

                var blogInfo = RequestHelper.PerformRequest(WordpressLoginProvider.WordpressSites + blogId, "", "GET", "");
                var jsonBlogInfo = JObject.Parse(blogInfo);
                jsonBlogInfo.Add("username", wordpressUserName);

                blogInfo = jsonBlogInfo.ToString();
                return new
                {
                    success = true,
                    data = blogInfo
                };
            }
            return new
            {
                success = false
            };
        }

        /// <visible>false</visible>
        [Read("wordpress-delete")]
        public object DeleteWordpressInfo()
        {
            var token = WordpressToken.GetToken();
            if (token != null)
            {
                WordpressToken.DeleteToken(token);
                return new
                {
                    success = true
                };
            }
            return new
            {
                success = false
            };
        }

        /// <visible>false</visible>
        [Create("wordpress-save")]
        public object WordpressSave(string code)
        {
            if (code == "")
            {
                return new
                {
                    success = false
                };
            }
            try
            {
                var token = OAuth20TokenHelper.GetAccessToken<WordpressLoginProvider>(ConsumerFactory, code);
                WordpressToken.SaveToken(token);
                var meInfo = WordpressHelper.GetWordpressMeInfo(token.AccessToken);
                var blogId = JObject.Parse(meInfo).Value<string>("token_site_id");

                var wordpressUserName = JObject.Parse(meInfo).Value<string>("username");

                var blogInfo = RequestHelper.PerformRequest(WordpressLoginProvider.WordpressSites + blogId, "", "GET", "");
                var jsonBlogInfo = JObject.Parse(blogInfo);
                jsonBlogInfo.Add("username", wordpressUserName);

                blogInfo = jsonBlogInfo.ToString();
                return new
                {
                    success = true,
                    data = blogInfo
                };
            }
            catch (Exception)
            {
                return new
                {
                    success = false
                };
            }
        }

        /// <visible>false</visible>
        [Create("wordpress")]
        public bool CreateWordpressPost(string code, string title, string content, int status)
        {
            try
            {
                var token = WordpressToken.GetToken();
                var meInfo = WordpressHelper.GetWordpressMeInfo(token.AccessToken);
                var parser = JObject.Parse(meInfo);
                if (parser == null) return false;
                var blogId = parser.Value<string>("token_site_id");

                if (blogId != null)
                {
                    var createPost = WordpressHelper.CreateWordpressPost(title, content, status, blogId, token);
                    return createPost;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region easybib

        /// <visible>false</visible>
        [Read("easybib-citation-list")]
        public object GetEasybibCitationList(int source, string data)
        {
            try
            {
                var citationList = EasyBibHelper.GetEasyBibCitationsList(source, data);
                return new
                {
                    success = true,
                    citations = citationList
                };
            }
            catch (Exception)
            {
                return new
                {
                    success = false
                };
            }

        }

        /// <visible>false</visible>
        [Read("easybib-styles")]
        public object GetEasybibStyles()
        {
            try
            {
                var data = EasyBibHelper.GetEasyBibStyles();
                return new
                {
                    success = true,
                    styles = data
                };
            }
            catch (Exception)
            {
                return new
                {
                    success = false
                };
            }
        }

        /// <visible>false</visible>
        [Create("easybib-citation")]
        public object EasyBibCitationBook(string citationData)
        {
            try
            {
                var citat = EasyBibHelper.GetEasyBibCitation(citationData);
                if (citat != null)
                {
                    return new
                    {
                        success = true,
                        citation = citat
                    };
                }
                else
                {
                    return new
                    {
                        success = false
                    };
                }

            }
            catch (Exception)
            {
                return new
                {
                    success = false
                };
            }
        }

        #endregion

        /// <summary>
        /// Result of file conversation operation.
        /// </summary>
        [DataContract(Name = "operation_result", Namespace = "")]
        public class ConversationResult
        {
            /// <summary>
            /// Operation Id.
            /// </summary>
            [DataMember(Name = "id")]
            public string Id { get; set; }

            /// <summary>
            /// Operation type.
            /// </summary>
            [DataMember(Name = "operation")]
            public FileOperationType OperationType { get; set; }

            /// <summary>
            /// Operation progress.
            /// </summary>
            [DataMember(Name = "progress")]
            public int Progress { get; set; }

            /// <summary>
            /// Source files for operation.
            /// </summary>
            [DataMember(Name = "source")]
            public string Source { get; set; }

            /// <summary>
            /// Result file of operation.
            /// </summary>
            [DataMember(Name = "result")]
            public FileWrapper File { get; set; }

            /// <summary>
            /// Error during conversation.
            /// </summary>
            [DataMember(Name = "error")]
            public string Error { get; set; }

            /// <summary>
            /// Is operation processed.
            /// </summary>
            [DataMember(Name = "processed")]
            public string Processed { get; set; }
        }
    }

    public static class DocumentsControllerExtention
    {
        public static DIHelper AddDocumentsControllerService(this DIHelper services)
        {
            return services
                .AddEasyBibHelperService()
                .AddWordpressTokenService()
                .AddWordpressHelperService()
                .AddFolderContentWrapperHelperService()
                .AddFileUploaderService()
                .AddFileShareParamsService()
                .AddFileShareWrapperService()
                .AddFileOperationWraperHelperService()
                .AddFileWrapperHelperService()
                .AddFolderWrapperHelperService()
                .AddConsumerFactoryService()
                .AddDocumentServiceConnectorService()
                .AddCommonLinkUtilityService()
                .AddMessageServiceService()
                .AddThirdpartyConfigurationService()
                .AddCoreBaseSettingsService()
                .AddWebItemSecurity()
                .AddUserManagerService()
                .AddEntryManagerService()
                .AddTenantManagerService()
                .AddSecurityContextService()
                .AddDocumentServiceHelperService()
                .AddFilesLinkUtilityService()
                .AddApiContextService()
                .AddFileStorageService()
                .AddGlobalFolderHelperService()
                .AddFilesSettingsHelperService()
                .AddBoxLoginProviderService()
                .AddDropboxLoginProviderService()
                .AddOneDriveLoginProviderService()
                .AddGoogleLoginProviderService()
                .AddChunkedUploadSessionHelperService()
                .AddProductEntryPointService()
                ;
        }
    }
}