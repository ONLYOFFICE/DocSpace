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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Users;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Model;
using ASC.Files.Helpers;
using ASC.Files.Model;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

using Newtonsoft.Json.Linq;

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Api.Documents
{
    /// <summary>
    /// Provides access to documents
    /// </summary>
    [DefaultRoute]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileStorageService<string> FileStorageService;

        private FilesControllerHelper<string> FilesControllerHelperString { get; }
        private FilesControllerHelper<int> FilesControllerHelperInt { get; }
        private FileStorageService<int> FileStorageServiceInt { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private SecurityContext SecurityContext { get; }
        private FolderWrapperHelper FolderWrapperHelper { get; }
        private FileOperationWraperHelper FileOperationWraperHelper { get; }
        private EntryManager EntryManager { get; }
        private UserManager UserManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private ThirdpartyConfiguration ThirdpartyConfiguration { get; }
        private MessageService MessageService { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private WordpressToken WordpressToken { get; }
        private WordpressHelper WordpressHelper { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private EasyBibHelper EasyBibHelper { get; }
        private ProductEntryPoint ProductEntryPoint { get; }
        public TenantManager TenantManager { get; }
        public FileUtility FileUtility { get; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileStorageService"></param>
        public FilesController(
            FilesControllerHelper<string> filesControllerHelperString,
            FilesControllerHelper<int> filesControllerHelperInt,
            FileStorageService<string> fileStorageService,
            FileStorageService<int> fileStorageServiceInt,
            GlobalFolderHelper globalFolderHelper,
            FilesSettingsHelper filesSettingsHelper,
            FilesLinkUtility filesLinkUtility,
            SecurityContext securityContext,
            FolderWrapperHelper folderWrapperHelper,
            FileOperationWraperHelper fileOperationWraperHelper,
            EntryManager entryManager,
            UserManager userManager,
            CoreBaseSettings coreBaseSettings,
            ThirdpartyConfiguration thirdpartyConfiguration,
            MessageService messageService,
            CommonLinkUtility commonLinkUtility,
            DocumentServiceConnector documentServiceConnector,
            WordpressToken wordpressToken,
            WordpressHelper wordpressHelper,
            ConsumerFactory consumerFactory,
            EasyBibHelper easyBibHelper,
            ProductEntryPoint productEntryPoint,
            TenantManager tenantManager,
            FileUtility fileUtility)
        {
            FilesControllerHelperString = filesControllerHelperString;
            FilesControllerHelperInt = filesControllerHelperInt;
            FileStorageService = fileStorageService;
            FileStorageServiceInt = fileStorageServiceInt;
            GlobalFolderHelper = globalFolderHelper;
            FilesSettingsHelper = filesSettingsHelper;
            FilesLinkUtility = filesLinkUtility;
            SecurityContext = securityContext;
            FolderWrapperHelper = folderWrapperHelper;
            FileOperationWraperHelper = fileOperationWraperHelper;
            EntryManager = entryManager;
            UserManager = userManager;
            CoreBaseSettings = coreBaseSettings;
            ThirdpartyConfiguration = thirdpartyConfiguration;
            ConsumerFactory = consumerFactory;
            MessageService = messageService;
            CommonLinkUtility = commonLinkUtility;
            DocumentServiceConnector = documentServiceConnector;
            WordpressToken = wordpressToken;
            WordpressHelper = wordpressHelper;
            EasyBibHelper = easyBibHelper;
            ProductEntryPoint = productEntryPoint;
            TenantManager = tenantManager;
            FileUtility = fileUtility;
        }

        [Read("info")]
        public Module GetModule()
        {
            ProductEntryPoint.Init();
            return new Module(ProductEntryPoint, true);
        }

        [Read("@root")]
        public IEnumerable<FolderContentWrapper<int>> GetRootFolders(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders, bool withoutTrash, bool withoutAdditionalFolder)
        {
            var IsVisitor = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor(UserManager);
            var result = new SortedSet<int>();

            if (!IsVisitor)
            {
                result.Add(GlobalFolderHelper.FolderMy);
            }

            if (!CoreBaseSettings.Personal && !UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOutsider(UserManager))
            {
                result.Add(GlobalFolderHelper.FolderShare);
            }

            if (!IsVisitor && !withoutAdditionalFolder)
            {
                if (FilesSettingsHelper.FavoritesSection)
                {
                    result.Add(GlobalFolderHelper.FolderFavorites);
                }

                if (FilesSettingsHelper.RecentSection)
                {
                    result.Add(GlobalFolderHelper.FolderRecent);
                }

                if (PrivacyRoomSettings.IsAvailable(TenantManager))
                {
                    result.Add(GlobalFolderHelper.FolderPrivacy);
                }
            }

            if (!CoreBaseSettings.Personal)
            {
                result.Add(GlobalFolderHelper.FolderCommon);
            }

            if (!IsVisitor
               && !withoutAdditionalFolder
               && FileUtility.ExtsWebTemplate.Any()
               && FilesSettingsHelper.TemplatesSection)
            {
                result.Add(GlobalFolderHelper.FolderTemplates);
            }

            if (!withoutTrash)
            {
                result.Add((int)GlobalFolderHelper.FolderTrash);
            }

            return result.Select(r => FilesControllerHelperInt.GetFolder(r, userIdOrGroupId, filterType, withsubfolders));
        }


        [Read("@privacy")]
        public FolderContentWrapper<int> GetPrivacyFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            if (!IsAvailablePrivacyRoomSettings()) throw new System.Security.SecurityException();
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderPrivacy, userIdOrGroupId, filterType, withsubfolders);
        }

        [Read("@privacy/available")]
        public bool IsAvailablePrivacyRoomSettings()
        {
            return PrivacyRoomSettings.IsAvailable(TenantManager);
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
        public FolderContentWrapper<int> GetMyFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderMy, userIdOrGroupId, filterType, withsubfolders);
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
        public FolderContentWrapper<string> GetProjectsFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperString.GetFolder(GlobalFolderHelper.GetFolderProjects<string>(), userIdOrGroupId, filterType, withsubfolders);
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
        public FolderContentWrapper<int> GetCommonFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderCommon, userIdOrGroupId, filterType, withsubfolders);
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
        public FolderContentWrapper<int> GetShareFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderShare, userIdOrGroupId, filterType, withsubfolders);
        }

        /// <summary>
        /// Returns the detailed list of recent files
        /// </summary>
        /// <short>Section Recent</short>
        /// <category>Folders</category>
        /// <returns>Recent contents</returns>
        [Read("@recent")]
        public FolderContentWrapper<int> GetRecentFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderRecent, userIdOrGroupId, filterType, withsubfolders);
        }

        [Create("file/{fileId}/recent", order: int.MaxValue)]
        public FileWrapper<string> AddToRecent(string fileId)
        {
            return FilesControllerHelperString.AddToRecent(fileId);
        }

        [Create("file/{fileId:int}/recent", order: int.MaxValue - 1)]
        public FileWrapper<int> AddToRecent(int fileId)
        {
            return FilesControllerHelperInt.AddToRecent(fileId);
        }

        /// <summary>
        /// Returns the detailed list of favorites files
        /// </summary>
        /// <short>Section Favorite</short>
        /// <category>Folders</category>
        /// <returns>Favorites contents</returns>
        [Read("@favorites")]
        public FolderContentWrapper<int> GetFavoritesFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderFavorites, userIdOrGroupId, filterType, withsubfolders);
        }

        /// <summary>
        /// Returns the detailed list of templates files
        /// </summary>
        /// <short>Section Template</short>
        /// <category>Folders</category>
        /// <returns>Templates contents</returns>
        [Read("@templates")]
        public FolderContentWrapper<int> GetTemplatesFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(GlobalFolderHelper.FolderTemplates, userIdOrGroupId, filterType, withsubfolders);
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
        public FolderContentWrapper<int> GetTrashFolder(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(Convert.ToInt32(GlobalFolderHelper.FolderTrash), userIdOrGroupId, filterType, withsubfolders);
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
        [Read("{folderId}", order: int.MaxValue, DisableFormat = true)]
        public FolderContentWrapper<string> GetFolder(string folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperString.GetFolder(folderId, userIdOrGroupId, filterType, withsubfolders).NotFoundIfNull();
        }

        [Read("{folderId:int}", order: int.MaxValue - 1)]
        public FolderContentWrapper<int> GetFolder(int folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolder(folderId, userIdOrGroupId, filterType, withsubfolders);
        }

        [Read("{folderId}/news")]
        public List<FileEntryWrapper> GetNewItems(string folderId)
        {
            return FilesControllerHelperString.GetNewItems(folderId);
        }

        [Read("{folderId:int}/news")]
        public List<FileEntryWrapper> GetNewItems(int folderId)
        {
            return FilesControllerHelperInt.GetNewItems(folderId);
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
        public List<FileWrapper<int>> UploadFileToMy(UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return UploadFile(GlobalFolderHelper.FolderMy, uploadModel);
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
        public List<FileWrapper<int>> UploadFileToCommon(UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return UploadFile(GlobalFolderHelper.FolderCommon, uploadModel);
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
        [Create("{folderId}/upload", DisableFormat = true)]
        public List<FileWrapper<string>> UploadFile(string folderId, UploadModel uploadModel)
        {
            return FilesControllerHelperString.UploadFile(folderId, uploadModel);
        }

        [Create("{folderId:int}/upload")]
        public List<FileWrapper<int>> UploadFile(int folderId, UploadModel uploadModel)
        {
            return FilesControllerHelperInt.UploadFile(folderId, uploadModel);
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
        public FileWrapper<int> InsertFileToMy(Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            return InsertFile(GlobalFolderHelper.FolderMy, file, title, createNewIfExist, keepConvertStatus);
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
        public FileWrapper<int> InsertFileToCommon(Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            return InsertFile(GlobalFolderHelper.FolderCommon, file, title, createNewIfExist, keepConvertStatus);
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
        [Create("{folderId}/insert", DisableFormat = true)]
        public FileWrapper<string> InsertFile(string folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            return FilesControllerHelperString.InsertFile(folderId, file, title, createNewIfExist, keepConvertStatus);
        }

        [Create("{folderId:int}/insert")]
        public FileWrapper<int> InsertFile(int folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            return FilesControllerHelperInt.InsertFile(folderId, file, title, createNewIfExist, keepConvertStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileId"></param>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Update("{fileId}/update", DisableFormat = true)]
        public FileWrapper<string> UpdateFileStream(Stream file, string fileId, bool encrypted = false, bool forcesave = false)
        {
            return FilesControllerHelperString.UpdateFileStream(file, fileId, encrypted, forcesave);
        }

        [Update("{fileId:int}/update")]
        public FileWrapper<int> UpdateFileStream(Stream file, int fileId, bool encrypted = false, bool forcesave = false)
        {
            return FilesControllerHelperInt.UpdateFileStream(file, fileId, encrypted, forcesave);
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
        [Update("file/{fileId}/saveediting", DisableFormat = true)]
        public FileWrapper<string> SaveEditing(string fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
        {
            return FilesControllerHelperString.SaveEditing(fileId, fileExtension, downloadUri, stream, doc, forcesave);
        }

        [Update("file/{fileId:int}/saveediting")]
        public FileWrapper<int> SaveEditing(int fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
        {
            return FilesControllerHelperInt.SaveEditing(fileId, fileExtension, downloadUri, stream, doc, forcesave);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="editingAlone"></param>
        /// <param name="doc"></param>
        /// <category>Files</category>
        /// <returns></returns>
        [Create("file/{fileId}/startedit", DisableFormat = true)]
        public object StartEdit(string fileId, StartEditModel model)
        {
            return FilesControllerHelperString.StartEdit(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId:int}/startedit")]
        public object StartEdit(int fileId, StartEditModel model)
        {
            return FilesControllerHelperInt.StartEdit(fileId, model.EditingAlone, model.Doc);
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
        [Read("file/{fileId}/trackeditfile", DisableFormat = true)]
        public KeyValuePair<bool, string> TrackEditFile(string fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FilesControllerHelperString.TrackEditFile(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        [Read("file/{fileId:int}/trackeditfile")]
        public KeyValuePair<bool, string> TrackEditFile(int fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FilesControllerHelperInt.TrackEditFile(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="version"></param>
        /// <param name="doc"></param>
        /// <category>Files</category>
        /// <returns></returns>
        [Read("file/{fileId}/openedit", DisableFormat = true)]
        public Configuration<string> OpenEdit(string fileId, int version, string doc)
        {
            return FilesControllerHelperString.OpenEdit(fileId, version, doc);
        }

        [Read("file/{fileId:int}/openedit")]
        public Configuration<int> OpenEdit(int fileId, int version, string doc)
        {
            return FilesControllerHelperInt.OpenEdit(fileId, version, doc);
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
        [Create("{folderId}/upload/create_session", DisableFormat = true)]
        public object CreateUploadSession(string folderId, SessionModel sessionModel)
        {
            return FilesControllerHelperString.CreateUploadSession(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        public object CreateUploadSession(int folderId, SessionModel sessionModel)
        {
            return FilesControllerHelperInt.CreateUploadSession(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
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
        public FileWrapper<int> CreateTextFileInMy(string title, string content)
        {
            return CreateTextFile(GlobalFolderHelper.FolderMy, title, content);
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
        public FileWrapper<int> CreateTextFileInCommon(string title, string content)
        {
            return CreateTextFile(GlobalFolderHelper.FolderCommon, title, content);
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
        [Create("{folderId}/text", DisableFormat = true)]
        public FileWrapper<string> CreateTextFile(string folderId, string title, string content)
        {
            return FilesControllerHelperString.CreateTextFile(folderId, title, content);
        }

        [Create("{folderId:int}/text")]
        public FileWrapper<int> CreateTextFile(int folderId, string title, string content)
        {
            return FilesControllerHelperInt.CreateTextFile(folderId, title, content);
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
        [Create("{folderId}/html", DisableFormat = true)]
        public FileWrapper<string> CreateHtmlFile(string folderId, string title, string content)
        {
            return FilesControllerHelperString.CreateHtmlFile(folderId, title, content);
        }

        [Create("{folderId:int}/html")]
        public FileWrapper<int> CreateHtmlFile(int folderId, string title, string content)
        {
            return FilesControllerHelperInt.CreateHtmlFile(folderId, title, content);
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
        public FileWrapper<int> CreateHtmlFileInMy(string title, string content)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderMy, title, content);
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
        public FileWrapper<int> CreateHtmlFileInCommon(string title, string content)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderCommon, title, content);
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
        [Create("folder/{folderId}", DisableFormat = true)]
        public FolderWrapper<string> CreateFolder(string folderId, CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.CreateFolder(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}")]
        public FolderWrapper<int> CreateFolder(int folderId, CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.CreateFolder(folderId, folderModel.Title);
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
        public FileWrapper<int> CreateFile(CreateFileModel<int> model)
        {
            return CreateFile(GlobalFolderHelper.FolderMy, model);
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
        [Create("{folderId}/file", DisableFormat = true)]
        public FileWrapper<string> CreateFile(string folderId, CreateFileModel<string> model)
        {
            return FilesControllerHelperString.CreateFile(folderId, model.Title, model.TemplateId);
        }

        [Create("{folderId:int}/file")]
        public FileWrapper<int> CreateFile(int folderId, CreateFileModel<int> model)
        {
            return FilesControllerHelperInt.CreateFile(folderId, model.Title, model.TemplateId);
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
        [Update("folder/{folderId}", DisableFormat = true)]
        public FolderWrapper<string> RenameFolder(string folderId, CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.RenameFolder(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}")]
        public FolderWrapper<int> RenameFolder(int folderId, CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.RenameFolder(folderId, folderModel.Title);
        }

        /// <summary>
        /// Returns a detailed information about the folder with the ID specified in the request
        /// </summary>
        /// <short>Folder information</short>
        /// <category>Folders</category>
        /// <returns>Folder info</returns>
        [Read("folder/{folderId}", DisableFormat = true)]
        public FolderWrapper<string> GetFolderInfo(string folderId)
        {
            return FilesControllerHelperString.GetFolderInfo(folderId);
        }

        [Read("folder/{folderId:int}")]
        public FolderWrapper<int> GetFolderInfo(int folderId)
        {
            return FilesControllerHelperInt.GetFolderInfo(folderId);
        }

        /// <summary>
        /// Returns parent folders
        /// </summary>
        /// <param name="folderId"></param>
        /// <category>Folders</category>
        /// <returns>Parent folders</returns>
        [Read("folder/{folderId}/path", DisableFormat = true)]
        public IEnumerable<FolderWrapper<string>> GetFolderPath(string folderId)
        {
            return FilesControllerHelperString.GetFolderPath(folderId);
        }

        [Read("folder/{folderId:int}/path")]
        public IEnumerable<FolderWrapper<int>> GetFolderPath(int folderId)
        {
            return FilesControllerHelperInt.GetFolderPath(folderId);
        }

        /// <summary>
        /// Returns a detailed information about the file with the ID specified in the request
        /// </summary>
        /// <short>File information</short>
        /// <category>Files</category>
        /// <returns>File info</returns>
        [Read("file/{fileId}", DisableFormat = true)]
        public FileWrapper<string> GetFileInfo(string fileId, int version = -1)
        {
            return FilesControllerHelperString.GetFileInfo(fileId, version);
        }

        [Read("file/{fileId:int}", DisableFormat = true)]
        public FileWrapper<int> GetFileInfo(int fileId, int version = -1)
        {
            return FilesControllerHelperInt.GetFileInfo(fileId, version);
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
        [Update("file/{fileId}", DisableFormat = true)]
        public FileWrapper<string> UpdateFile(string fileId, UpdateFileModel model)
        {
            return FilesControllerHelperString.UpdateFile(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId:int}")]
        public FileWrapper<int> UpdateFile(int fileId, UpdateFileModel model)
        {
            return FilesControllerHelperInt.UpdateFile(fileId, model.Title, model.LastVersion);
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
        [Delete("file/{fileId}", DisableFormat = true)]
        public IEnumerable<FileOperationWraper> DeleteFile(string fileId, bool deleteAfter, bool immediately)
        {
            return FilesControllerHelperString.DeleteFile(fileId, deleteAfter, immediately);
        }

        [Delete("file/{fileId:int}")]
        public IEnumerable<FileOperationWraper> DeleteFile(int fileId, bool deleteAfter, bool immediately)
        {
            return FilesControllerHelperInt.DeleteFile(fileId, deleteAfter, immediately);
        }

        /// <summary>
        ///  Start conversion
        /// </summary>
        /// <short>Convert</short>
        /// <category>File operations</category>
        /// <param name="fileId"></param>
        /// <returns>Operation result</returns>
        [Update("file/{fileId}/checkconversion", DisableFormat = true)]
        public IEnumerable<ConversationResult<string>> StartConversion(string fileId)
        {
            return FilesControllerHelperString.StartConversion(fileId);
        }

        [Update("file/{fileId:int}/checkconversion")]
        public IEnumerable<ConversationResult<int>> StartConversion(int fileId)
        {
            return FilesControllerHelperInt.StartConversion(fileId);
        }

        /// <summary>
        ///  Check conversion status
        /// </summary>
        /// <short>Convert</short>
        /// <category>File operations</category>
        /// <param name="fileId"></param>
        /// <param name="start"></param>
        /// <returns>Operation result</returns>
        [Read("file/{fileId}/checkconversion", DisableFormat = true)]
        public IEnumerable<ConversationResult<string>> CheckConversion(string fileId, bool start)
        {
            return FilesControllerHelperString.CheckConversion(fileId, start);
        }

        [Read("file/{fileId:int}/checkconversion")]
        public IEnumerable<ConversationResult<int>> CheckConversion(int fileId, bool start)
        {
            return FilesControllerHelperInt.CheckConversion(fileId, start);
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
        [Delete("folder/{folderId}", DisableFormat = true)]
        public IEnumerable<FileOperationWraper> DeleteFolder(string folderId, bool deleteAfter, bool immediately)
        {
            return FilesControllerHelperString.DeleteFolder(folderId, deleteAfter, immediately);
        }

        [Delete("folder/{folderId:int}")]
        public IEnumerable<FileOperationWraper> DeleteFolder(int folderId, bool deleteAfter, bool immediately)
        {
            return FilesControllerHelperInt.DeleteFolder(folderId, deleteAfter, immediately);
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
        public IEnumerable<FileEntryWrapper> MoveOrCopyBatchCheck(BatchModel batchModel)
        {
            return FilesControllerHelperString.MoveOrCopyBatchCheck(batchModel);
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
            return FilesControllerHelperString.MoveBatchItems(batchModel);
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
            return FilesControllerHelperString.CopyBatchItems(batchModel);
        }

        /// <summary>
        ///   Marks all files and folders as read
        /// </summary>
        /// <short>Mark as read</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Update("fileops/markasread")]
        public IEnumerable<FileOperationWraper> MarkAsRead(BaseBatchModel<JsonElement> model)
        {
            return FilesControllerHelperString.MarkAsRead(model);
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
            return FilesControllerHelperString.BulkDownload(model);
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
            return FileStorageService.DeleteItems("delete", batch.FileIds.ToList(), batch.FolderIds.ToList(), false, batch.DeleteAfter, batch.Immediately)
                .Select(FileOperationWraperHelper.Get);
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
            return FilesControllerHelperInt.EmptyTrash();
        }

        /// <summary>
        /// Returns the detailed information about all the available file versions with the ID specified in the request
        /// </summary>
        /// <short>File versions</short>
        /// <category>Files</category>
        /// <param name="fileId">File ID</param>
        /// <returns>File information</returns>
        [Read("file/{fileId}/history", DisableFormat = true)]
        public IEnumerable<FileWrapper<string>> GetFileVersionInfo(string fileId)
        {
            return FilesControllerHelperString.GetFileVersionInfo(fileId);
        }

        [Read("file/{fileId:int}/history")]
        public IEnumerable<FileWrapper<int>> GetFileVersionInfo(int fileId)
        {
            return FilesControllerHelperInt.GetFileVersionInfo(fileId);
        }

        /// <summary>
        /// Change version history
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <param name="version">Version of history</param>
        /// <param name="continueVersion">Mark as version or revision</param>
        /// <category>Files</category>
        /// <returns></returns>
        [Update("file/{fileId}/history", DisableFormat = true)]
        public IEnumerable<FileWrapper<string>> ChangeHistory(string fileId, ChangeHistoryModel model)
        {
            return FilesControllerHelperString.ChangeHistory(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        public IEnumerable<FileWrapper<int>> ChangeHistory(int fileId, ChangeHistoryModel model)
        {
            return FilesControllerHelperInt.ChangeHistory(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/lock", DisableFormat = true)]
        public FileWrapper<string> LockFile(string fileId, LockFileModel model)
        {
            return FilesControllerHelperString.LockFile(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        public FileWrapper<int> LockFile(int fileId, LockFileModel model)
        {
            return FilesControllerHelperInt.LockFile(fileId, model.LockFile);
        }

        [Update("file/{fileId}/comment", DisableFormat = true)]
        public object UpdateComment(string fileId, UpdateCommentModel model)
        {
            return FilesControllerHelperString.UpdateComment(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId:int}/comment")]
        public object UpdateComment(int fileId, UpdateCommentModel model)
        {
            return FilesControllerHelperInt.UpdateComment(fileId, model.Version, model.Comment);
        }

        /// <summary>
        /// Returns the detailed information about shared file with the ID specified in the request
        /// </summary>
        /// <short>File sharing</short>
        /// <category>Sharing</category>
        /// <param name="fileId">File ID</param>
        /// <returns>Shared file information</returns>
        [Read("file/{fileId}/share", DisableFormat = true)]
        public IEnumerable<FileShareWrapper> GetFileSecurityInfo(string fileId)
        {
            return FilesControllerHelperString.GetFileSecurityInfo(fileId);
        }

        [Read("file/{fileId:int}/share")]
        public IEnumerable<FileShareWrapper> GetFileSecurityInfo(int fileId)
        {
            return FilesControllerHelperInt.GetFileSecurityInfo(fileId);
        }

        /// <summary>
        /// Returns the detailed information about shared folder with the ID specified in the request
        /// </summary>
        /// <short>Folder sharing</short>
        /// <param name="folderId">Folder ID</param>
        /// <category>Sharing</category>
        /// <returns>Shared folder information</returns>
        [Read("folder/{folderId}/share", DisableFormat = true)]
        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(string folderId)
        {
            return FilesControllerHelperString.GetFolderSecurityInfo(folderId);
        }

        [Read("folder/{folderId:int}/share")]
        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(int folderId)
        {
            return FilesControllerHelperInt.GetFolderSecurityInfo(folderId);
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
        [Update("file/{fileId}/share", DisableFormat = true)]
        public IEnumerable<FileShareWrapper> SetFileSecurityInfo(string fileId, SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFileSecurityInfo(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        public IEnumerable<FileShareWrapper> SetFileSecurityInfo(int fileId, SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFileSecurityInfo(fileId, model.Share, model.Notify, model.SharingMessage);
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
        [Update("folder/{folderId}/share", DisableFormat = true)]
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfo(string folderId, SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFolderSecurityInfo(folderId, model.Share, model.Notify, model.SharingMessage);
        }
        [Update("folder/{folderId:int}/share")]
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfo(int folderId, SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFolderSecurityInfo(folderId, model.Share, model.Notify, model.SharingMessage);
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
        public bool RemoveSecurityInfo(BaseBatchModel<object> model)
        {
            FileStorageService.RemoveAce(model.FileIds.OfType<string>().ToList(), model.FolderIds.OfType<string>().ToList());
            FileStorageServiceInt.RemoveAce(model.FileIds.OfType<long>().Select(r => Convert.ToInt32(r)).ToList(), model.FolderIds.OfType<long>().Select(r => Convert.ToInt32(r)).ToList());
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
        [Update("{fileId}/sharedlink", DisableFormat = true)]
        public object GenerateSharedLink(string fileId, FileShare share)
        {
            return FilesControllerHelperString.GenerateSharedLink(fileId, share);
        }

        [Update("{fileId:int}/sharedlink")]
        public object GenerateSharedLink(int fileId, FileShare share)
        {
            return FilesControllerHelperInt.GenerateSharedLink(fileId, share);
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
                    || (!FilesSettingsHelper.EnableThirdParty
                    && !CoreBaseSettings.Personal))
            {
                return result;
            }

            if (ThirdpartyConfiguration.SupportBoxInclusion)
            {
                var boxLoginProvider = ConsumerFactory.Get<BoxLoginProvider>();
                result.Add(new List<string> { "Box", boxLoginProvider.ClientID, boxLoginProvider.RedirectUri });
            }
            if (ThirdpartyConfiguration.SupportDropboxInclusion)
            {
                var dropboxLoginProvider = ConsumerFactory.Get<DropboxLoginProvider>();
                result.Add(new List<string> { "DropboxV2", dropboxLoginProvider.ClientID, dropboxLoginProvider.RedirectUri });
            }

            if (ThirdpartyConfiguration.SupportGoogleDriveInclusion)
            {
                var googleLoginProvider = ConsumerFactory.Get<GoogleLoginProvider>();
                result.Add(new List<string> { "GoogleDrive", googleLoginProvider.ClientID, googleLoginProvider.RedirectUri });
            }

            if (ThirdpartyConfiguration.SupportOneDriveInclusion)
            {
                var oneDriveLoginProvider = ConsumerFactory.Get<OneDriveLoginProvider>();
                result.Add(new List<string> { "OneDrive", oneDriveLoginProvider.ClientID, oneDriveLoginProvider.RedirectUri });
            }
            if (ThirdpartyConfiguration.SupportSharePointInclusion)
            {
                result.Add(new List<string> { "SharePoint" });
            }
            if (ThirdpartyConfiguration.SupportkDriveInclusion)
            {
                result.Add(new List<string> { "kDrive" });
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
        public FolderWrapper<string> SaveThirdParty(
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
        public IEnumerable<FolderWrapper<string>> GetCommonThirdPartyFolders()
        {
            var parent = FileStorageServiceInt.GetFolder(GlobalFolderHelper.FolderCommon);
            return EntryManager.GetThirpartyFolders(parent).Select(FolderWrapperHelper.Get).ToList();
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
        /// Adding files to favorite list
        /// </summary>
        /// <short>Favorite add</short>
        /// <category>Files</category>
        /// <param name="folderIds" visible="false"></param>
        /// <param name="fileIds">File IDs</param>
        /// <returns></returns>
        [Create("favorites")]
        public bool AddFavorites(BaseBatchModel<JsonElement> model)
        {
            FileStorageServiceInt.AddToFavorites(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()), model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()));
            FileStorageService.AddToFavorites(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()), model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()));
            return true;
        }

        /// <summary>
        /// Removing files from favorite list
        /// </summary>
        /// <short>Favorite delete</short>
        /// <category>Files</category>
        /// <param name="folderIds" visible="false"></param>
        /// <param name="fileIds">File IDs</param>
        /// <returns></returns>
        [Delete("favorites")]
        public bool DeleteFavorites(BaseBatchModel<JsonElement> model)
        {
            FileStorageServiceInt.DeleteFavorites(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()), model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()));
            FileStorageService.DeleteFavorites(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()), model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()));
            return true;
        }

        /// <summary>
        /// Adding files to template list
        /// </summary>
        /// <short>Template add</short>
        /// <category>Files</category>
        /// <param name="fileIds">File IDs</param>
        /// <returns></returns>
        [Create("templates")]
        public bool AddTemplates(IEnumerable<int> fileIds)
        {
            FileStorageServiceInt.AddToTemplates(fileIds);
            return true;
        }

        /// <summary>
        /// Removing files from template list
        /// </summary>
        /// <short>Template delete</short>
        /// <category>Files</category>
        /// <param name="fileIds">File IDs</param>
        /// <returns></returns>
        [Delete("templates")]
        public bool DeleteTemplates(IEnumerable<int> fileIds)
        {
            FileStorageServiceInt.DeleteTemplates(fileIds);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"storeoriginal")]
        public bool StoreOriginal(SettingsModel model)
        {
            return FileStorageService.StoreOriginal(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Read(@"settings")]
        public object GetFilesSettings()
        {
            return new
            {
                FilesSettingsHelper.StoreOriginalFiles,
                FilesSettingsHelper.ConfirmDelete,
                FilesSettingsHelper.UpdateIfExist,
                FilesSettingsHelper.Forcesave,
                FilesSettingsHelper.StoreForcesave,
                FilesSettingsHelper.EnableThirdParty
            };
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
        public bool UpdateIfExist(SettingsModel model)
        {
            return FileStorageService.UpdateIfExist(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"changedeleteconfrim")]
        public bool ChangeDeleteConfrim(SettingsModel model)
        {
            return FileStorageService.ChangeDeleteConfrim(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"storeforcesave")]
        public bool StoreForcesave(SettingsModel model)
        {
            return FileStorageService.StoreForcesave(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"forcesave")]
        public bool Forcesave(SettingsModel model)
        {
            return FileStorageService.Forcesave(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"thirdparty")]
        public bool ChangeAccessToThirdparty(SettingsModel model)
        {
            return FileStorageService.ChangeAccessToThirdparty(model.Set);
        }

        /// <summary>
        /// Display recent folder
        /// </summary>
        /// <param name="set"></param>
        /// <category>Settings</category>
        /// <returns></returns>
        [Update(@"displayRecent")]
        public bool DisplayRecent(bool set)
        {
            return FileStorageService.DisplayRecent(set);
        }

        /// <summary>
        /// Display favorite folder
        /// </summary>
        /// <param name="set"></param>
        /// <category>Settings</category>
        /// <returns></returns>
        [Update(@"settings/favorites")]
        public bool DisplayFavorite(bool set)
        {
            return FileStorageService.DisplayFavorite(set);
        }

        /// <summary>
        /// Display template folder
        /// </summary>
        /// <param name="set"></param>
        /// <category>Settings</category>
        /// <returns></returns>
        [Update(@"settings/templates")]
        public bool DisplayTemplates(bool set)
        {
            return FileStorageService.DisplayTemplates(set);
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
        public class ConversationResult<T>
        {
            /// <summary>
            /// Operation Id.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Operation type.
            /// </summary>
            [JsonPropertyName("Operation")]
            public FileOperationType OperationType { get; set; }

            /// <summary>
            /// Operation progress.
            /// </summary>
            public int Progress { get; set; }

            /// <summary>
            /// Source files for operation.
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// Result file of operation.
            /// </summary>
            [JsonPropertyName("result")]
            public FileWrapper<T> File { get; set; }

            /// <summary>
            /// Error during conversation.
            /// </summary>
            public string Error { get; set; }

            /// <summary>
            /// Is operation processed.
            /// </summary>
            public string Processed { get; set; }
        }
    }

    public static class DocumentsControllerExtention
    {
        public static DIHelper AddDocumentsControllerService(this DIHelper services)
        {
            return services.AddFilesControllerHelperService();
        }
    }

    public class BodySpecificAttribute : Attribute, IActionConstraint
    {
        public BodySpecificAttribute()
        {
        }

        public int Order
        {
            get
            {
                return 0;
            }
        }

        public bool Accept(ActionConstraintContext context)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                context.RouteContext.HttpContext.Request.EnableBuffering();
                _ = JsonSerializer.DeserializeAsync<BaseBatchModel<int>>(context.RouteContext.HttpContext.Request.Body, options).Result;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                context.RouteContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }

        }
    }
}