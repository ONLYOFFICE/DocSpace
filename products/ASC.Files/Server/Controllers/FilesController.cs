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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

using Newtonsoft.Json.Linq;

namespace ASC.Api.Documents
{
    /// <summary>
    /// Provides access to documents
    /// </summary>
    [Scope]
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
        private EasyBibHelper EasyBibHelper { get; }
        private ProductEntryPoint ProductEntryPoint { get; }
        private TenantManager TenantManager { get; }
        private FileUtility FileUtility { get; }

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
            ProductEntryPoint productEntryPoint,
            TenantManager tenantManager,
            FileUtility fileUtility,
            ConsumerFactory consumerFactory)
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
            MessageService = messageService;
            CommonLinkUtility = commonLinkUtility;
            DocumentServiceConnector = documentServiceConnector;
            WordpressToken = wordpressToken;
            WordpressHelper = wordpressHelper;
            EasyBibHelper = consumerFactory.Get<EasyBibHelper>();
            ProductEntryPoint = productEntryPoint;
            TenantManager = tenantManager;
            FileUtility = fileUtility;
        }

        [Read("info")]
        public Module GetModule()
        {
            ProductEntryPoint.Init();
            return new Module(ProductEntryPoint);
        }

        [Read("@root")]
        public IEnumerable<FolderContentWrapper<int>> GetRootFolders(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders, bool withoutTrash, bool withoutAdditionalFolder)
        {
            var IsVisitor = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor(UserManager);
            var IsOutsider = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOutsider(UserManager);
            var result = new SortedSet<int>();

            if (IsOutsider)
            {
                withoutTrash = true;
                withoutAdditionalFolder = true;
            }

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

                if (!CoreBaseSettings.Personal && PrivacyRoomSettings.IsAvailable(TenantManager))
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

            return result.Select(r => FilesControllerHelperInt.GetFolder(r, userIdOrGroupId, filterType, withsubfolders)).ToList();
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

        [Read("{folderId}/subfolders")]
        public IEnumerable<FileEntryWrapper> GetFolders(string folderId)
        {
            return FilesControllerHelperString.GetFolders(folderId);
        }

        [Read("{folderId:int}/subfolders")]
        public IEnumerable<FileEntryWrapper> GetFolders(int folderId)
        {
            return FilesControllerHelperInt.GetFolders(folderId);
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
        public List<FileWrapper<int>> UploadFileToMyFromBody([FromBody] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return FilesControllerHelperInt.UploadFile(GlobalFolderHelper.FolderMy, uploadModel);
        }

        [Create("@my/upload")]
        [Consumes("application/x-www-form-urlencoded")]
        public List<FileWrapper<int>> UploadFileToMyFromForm([FromForm] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return FilesControllerHelperInt.UploadFile(GlobalFolderHelper.FolderMy, uploadModel);
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
        public List<FileWrapper<int>> UploadFileToCommonFromBody([FromBody] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return FilesControllerHelperInt.UploadFile(GlobalFolderHelper.FolderCommon, uploadModel);
        }

        [Create("@common/upload")]
        [Consumes("application/x-www-form-urlencoded")]
        public List<FileWrapper<int>> UploadFileToCommonFromForm([FromForm] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return FilesControllerHelperInt.UploadFile(GlobalFolderHelper.FolderCommon, uploadModel);
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
        public List<FileWrapper<string>> UploadFileFromBody(string folderId, [FromBody] UploadModel uploadModel)
        {
            return FilesControllerHelperString.UploadFile(folderId, uploadModel);
        }

        [Create("{folderId}/upload")]
        [Consumes("application/x-www-form-urlencoded")]
        public List<FileWrapper<string>> UploadFileFromForm(string folderId, [FromForm] UploadModel uploadModel)
        {
            return FilesControllerHelperString.UploadFile(folderId, uploadModel);
        }


        [Create("{folderId:int}/upload")]
        public List<FileWrapper<int>> UploadFileFromBody(int folderId, [FromBody] UploadModel uploadModel)
        {
            return FilesControllerHelperInt.UploadFile(folderId, uploadModel);
        }

        [Create("{folderId:int}/upload")]
        [Consumes("application/x-www-form-urlencoded")]
        public List<FileWrapper<int>> UploadFileFromForm(int folderId, [FromForm] UploadModel uploadModel)
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
        public FileWrapper<int> InsertFileToMyFromBody([FromForm] InsertFileModel model)
        {
            return InsertFile(GlobalFolderHelper.FolderMy, model);
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
        public FileWrapper<int> InsertFileToCommonFromBody([FromForm] InsertFileModel model)
        {
            return InsertFile(GlobalFolderHelper.FolderCommon, model);
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
        [Create("{folderId}/insert", order: int.MaxValue)]
        public FileWrapper<string> InsertFile(string folderId, [FromForm] InsertFileModel model)
        {
            return FilesControllerHelperString.InsertFile(folderId, model.File.OpenReadStream(), model.Title, model.CreateNewIfExist, model.KeepConvertStatus);
        }

        [Create("{folderId:int}/insert", order: int.MaxValue - 1)]
        public FileWrapper<int> InsertFileFromForm(int folderId, [FromForm] InsertFileModel model)
        {
            return InsertFile(folderId, model);
        }

        private FileWrapper<int> InsertFile(int folderId, InsertFileModel model)
        {
            return FilesControllerHelperInt.InsertFile(folderId, model.File.OpenReadStream(), model.Title, model.CreateNewIfExist, model.KeepConvertStatus);
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
        public FileWrapper<string> UpdateFileStreamFromForm(string fileId, [FromForm] FileStreamModel model)
        {
            return FilesControllerHelperString.UpdateFileStream(model.File.OpenReadStream(), fileId, model.Encrypted, model.Forcesave);
        }

        [Update("{fileId:int}/update")]
        public FileWrapper<int> UpdateFileStreamFromForm(int fileId, [FromForm] FileStreamModel model)
        {
            return FilesControllerHelperInt.UpdateFileStream(model.File.OpenReadStream(), fileId, model.Encrypted, model.Forcesave);
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
        public FileWrapper<string> SaveEditingFromForm(string fileId, [FromForm] SaveEditingModel model)
        {
            using var stream = model.Stream.OpenReadStream();
            return FilesControllerHelperString.SaveEditing(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
        }

        [Update("file/{fileId:int}/saveediting")]
        public FileWrapper<int> SaveEditingFromForm(int fileId, [FromForm] SaveEditingModel model)
        {
            using var stream = model.Stream.OpenReadStream();
            return FilesControllerHelperInt.SaveEditing(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
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
        public object StartEditFromBody(string fileId, [FromBody] StartEditModel model)
        {
            return FilesControllerHelperString.StartEdit(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId}/startedit")]
        [Consumes("application/x-www-form-urlencoded")]
        public object StartEditFromForm(string fileId, [FromForm] StartEditModel model)
        {
            return FilesControllerHelperString.StartEdit(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId:int}/startedit")]
        public object StartEditFromBody(int fileId, [FromBody] StartEditModel model)
        {
            return FilesControllerHelperInt.StartEdit(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId:int}/startedit")]
        [Consumes("application/x-www-form-urlencoded")]
        public object StartEditFromForm(int fileId, [FromForm] StartEditModel model)
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
        [Read("file/{fileId}/trackeditfile")]
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
        [AllowAnonymous]
        [Read("file/{fileId}/openedit", Check = false)]
        public Configuration<string> OpenEdit(string fileId, int version, string doc, bool view)
        {
            return FilesControllerHelperString.OpenEdit(fileId, version, doc, view);
        }

        [AllowAnonymous]
        [Read("file/{fileId:int}/openedit", Check = false)]
        public Configuration<int> OpenEdit(int fileId, int version, string doc, bool view)
        {
            return FilesControllerHelperInt.OpenEdit(fileId, version, doc, view);
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
        [Create("{folderId}/upload/create_session")]
        public object CreateUploadSessionFromBody(string folderId, [FromBody] SessionModel sessionModel)
        {
            return FilesControllerHelperString.CreateUploadSession(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId}/upload/create_session")]
        [Consumes("application/x-www-form-urlencoded")]
        public object CreateUploadSessionFromForm(string folderId, [FromForm] SessionModel sessionModel)
        {
            return FilesControllerHelperString.CreateUploadSession(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        public object CreateUploadSessionFromBody(int folderId, [FromBody] SessionModel sessionModel)
        {
            return FilesControllerHelperInt.CreateUploadSession(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        [Consumes("application/x-www-form-urlencoded")]
        public object CreateUploadSessionFromForm(int folderId, [FromForm] SessionModel sessionModel)
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
        public FileWrapper<int> CreateTextFileInMyFromBody([FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(GlobalFolderHelper.FolderMy, model);
        }

        [Create("@my/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateTextFileInMyFromForm([FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(GlobalFolderHelper.FolderMy, model);
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
        public FileWrapper<int> CreateTextFileInCommonFromBody([FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(GlobalFolderHelper.FolderCommon, model);
        }

        [Create("@common/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateTextFileInCommonFromForm([FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(GlobalFolderHelper.FolderCommon, model);
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
        public FileWrapper<string> CreateTextFileFromBody(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(folderId, model);
        }

        [Create("{folderId}/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<string> CreateTextFileFromForm(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(folderId, model);
        }

        private FileWrapper<string> CreateTextFile(string folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperString.CreateTextFile(folderId, model.Title, model.Content);
        }

        [Create("{folderId:int}/text")]
        public FileWrapper<int> CreateTextFileFromBody(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(folderId, model);
        }

        [Create("{folderId:int}/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateTextFileFromForm(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFile(folderId, model);
        }

        private FileWrapper<int> CreateTextFile(int folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperInt.CreateTextFile(folderId, model.Title, model.Content);
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
        public FileWrapper<string> CreateHtmlFileFromBody(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(folderId, model);
        }

        [Create("{folderId}/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<string> CreateHtmlFileFromForm(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(folderId, model);
        }

        private FileWrapper<string> CreateHtmlFile(string folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperString.CreateHtmlFile(folderId, model.Title, model.Content);
        }

        [Create("{folderId:int}/html")]
        public FileWrapper<int> CreateHtmlFileFromBody(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(folderId, model);
        }

        [Create("{folderId:int}/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateHtmlFileFromForm(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(folderId, model);
        }

        private FileWrapper<int> CreateHtmlFile(int folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperInt.CreateHtmlFile(folderId, model.Title, model.Content);
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
        public FileWrapper<int> CreateHtmlFileInMyFromBody([FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderMy, model);
        }

        [Create("@my/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateHtmlFileInMyFromForm([FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderMy, model);
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
        public FileWrapper<int> CreateHtmlFileInCommonFromBody([FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderCommon, model);
        }

        [Create("@common/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateHtmlFileInCommonFromForm([FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFile(GlobalFolderHelper.FolderCommon, model);
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
        public FolderWrapper<string> CreateFolderFromBody(string folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.CreateFolder(folderId, folderModel.Title);
        }

        [Create("folder/{folderId}", DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public FolderWrapper<string> CreateFolderFromForm(string folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.CreateFolder(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}")]
        public FolderWrapper<int> CreateFolderFromBody(int folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.CreateFolder(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public FolderWrapper<int> CreateFolderFromForm(int folderId, [FromForm] CreateFolderModel folderModel)
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
        public FileWrapper<int> CreateFileFromBody([FromBody] CreateFileModel<int> model)
        {
            return FilesControllerHelperInt.CreateFile(GlobalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("@my/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateFileFromForm([FromForm] CreateFileModel<int> model)
        {
            return FilesControllerHelperInt.CreateFile(GlobalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
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
        public FileWrapper<string> CreateFileFromBody(string folderId, [FromBody] CreateFileModel<string> model)
        {
            return FilesControllerHelperString.CreateFile(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId}/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<string> CreateFileFromForm(string folderId, [FromForm] CreateFileModel<string> model)
        {
            return FilesControllerHelperString.CreateFile(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId:int}/file")]
        public FileWrapper<int> CreateFileFromBody(int folderId, [FromBody] CreateFileModel<int> model)
        {
            return FilesControllerHelperInt.CreateFile(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId:int}/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> CreateFileFromForm(int folderId, [FromForm] CreateFileModel<int> model)
        {
            return FilesControllerHelperInt.CreateFile(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
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
        public FolderWrapper<string> RenameFolderFromBody(string folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.RenameFolder(folderId, folderModel.Title);
        }

        [Update("folder/{folderId}", DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public FolderWrapper<string> RenameFolderFromForm(string folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.RenameFolder(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}")]
        public FolderWrapper<int> RenameFolderFromBody(int folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.RenameFolder(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public FolderWrapper<int> RenameFolderFromForm(int folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.RenameFolder(folderId, folderModel.Title);
        }

        [Create("owner")]
        public IEnumerable<FileEntryWrapper> ChangeOwnerFromBody([FromBody] ChangeOwnerModel model)
        {
            return ChangeOwner(model);
        }

        [Create("owner")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileEntryWrapper> ChangeOwnerFromForm([FromForm] ChangeOwnerModel model)
        {
            return ChangeOwner(model);
        }

        public IEnumerable<FileEntryWrapper> ChangeOwner(ChangeOwnerModel model)
        {
            var result = new List<FileEntry>();
            result.AddRange(FileStorageServiceInt.ChangeOwner(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()).ToList(), model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()).ToList(), model.UserId));
            result.AddRange(FileStorageService.ChangeOwner(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()).ToList(), model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()).ToList(), model.UserId));
            return result.Select(FilesControllerHelperInt.GetFileEntryWrapper).ToList();
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
        [Read("folder/{folderId}/path")]
        public IEnumerable<FileEntryWrapper> GetFolderPath(string folderId)
        {
            return FilesControllerHelperString.GetFolderPath(folderId);
        }

        [Read("folder/{folderId:int}/path")]
        public IEnumerable<FileEntryWrapper> GetFolderPath(int folderId)
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

        [Read("file/{fileId:int}")]
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
        public FileWrapper<string> UpdateFileFromBody(string fileId, [FromBody] UpdateFileModel model)
        {
            return FilesControllerHelperString.UpdateFile(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId}", DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<string> UpdateFileFromForm(string fileId, [FromForm] UpdateFileModel model)
        {
            return FilesControllerHelperString.UpdateFile(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId:int}")]
        public FileWrapper<int> UpdateFileFromBody(int fileId, [FromBody] UpdateFileModel model)
        {
            return FilesControllerHelperInt.UpdateFile(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> UpdateFileFromForm(int fileId, [FromForm] UpdateFileModel model)
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
        public IEnumerable<FileOperationWraper> DeleteFile(string fileId, [FromBody] DeleteModel model)
        {
            return FilesControllerHelperString.DeleteFile(fileId, model.DeleteAfter, model.Immediately);
        }

        [Delete("file/{fileId:int}")]
        public IEnumerable<FileOperationWraper> DeleteFile(int fileId, [FromBody] DeleteModel model)
        {
            return FilesControllerHelperInt.DeleteFile(fileId, model.DeleteAfter, model.Immediately);
        }

        /// <summary>
        ///  Start conversion
        /// </summary>
        /// <short>Convert</short>
        /// <category>File operations</category>
        /// <param name="fileId"></param>
        /// <returns>Operation result</returns>
        [Update("file/{fileId}/checkconversion")]
        public IEnumerable<ConversationResult<string>> StartConversion(string fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel model)
        {
            return FilesControllerHelperString.StartConversion(fileId, model?.Sync ?? false);
        }

        [Update("file/{fileId:int}/checkconversion")]
        public IEnumerable<ConversationResult<int>> StartConversion(int fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel model)
        {
            return FilesControllerHelperInt.StartConversion(fileId, model?.Sync ?? false);
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
        [Create("fileops/move")]
        public IEnumerable<FileEntryWrapper> MoveOrCopyBatchCheckFromBody([FromBody] BatchModel batchModel)
        {
            return FilesControllerHelperString.MoveOrCopyBatchCheck(batchModel);
        }

        [Create("fileops/move")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileEntryWrapper> MoveOrCopyBatchCheckFromForm([FromForm] BatchModel batchModel)
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
        public IEnumerable<FileOperationWraper> MoveBatchItemsFromBody([FromBody] BatchModel batchModel)
        {
            return FilesControllerHelperString.MoveBatchItems(batchModel);
        }

        [Update("fileops/move")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileOperationWraper> MoveBatchItemsFromForm([FromForm] BatchModel batchModel)
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
        public IEnumerable<FileOperationWraper> CopyBatchItemsFromBody([FromBody] BatchModel batchModel)
        {
            return FilesControllerHelperString.CopyBatchItems(batchModel);
        }

        [Update("fileops/copy")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileOperationWraper> CopyBatchItemsFromForm([FromForm] BatchModel batchModel)
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
        public IEnumerable<FileOperationWraper> MarkAsReadFromBody([FromBody] BaseBatchModel<JsonElement> model)
        {
            return FilesControllerHelperString.MarkAsRead(model);
        }

        [Update("fileops/markasread")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileOperationWraper> MarkAsReadFromForm([FromForm] BaseBatchModel<JsonElement> model)
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
        public IEnumerable<FileOperationWraper> BulkDownload([FromBody] DownloadModel model)
        {
            return FilesControllerHelperString.BulkDownload(model);
        }

        [Update("fileops/bulkdownload")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileOperationWraper> BulkDownloadFromForm([FromForm] DownloadModel model)
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
        public IEnumerable<FileOperationWraper> DeleteBatchItemsFromBody([FromBody] DeleteBatchModel batch)
        {
            return FileStorageService.DeleteItems("delete", batch.FileIds.ToList(), batch.FolderIds.ToList(), false, batch.DeleteAfter, batch.Immediately)
                .Select(FileOperationWraperHelper.Get);
        }

        [Update("fileops/delete")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileOperationWraper> DeleteBatchItemsFromForm([FromForm] DeleteBatchModel batch)
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
        [Read("file/{fileId}/history")]
        public IEnumerable<FileWrapper<string>> GetFileVersionInfo(string fileId)
        {
            return FilesControllerHelperString.GetFileVersionInfo(fileId);
        }

        [Read("file/{fileId:int}/history")]
        public IEnumerable<FileWrapper<int>> GetFileVersionInfo(int fileId)
        {
            return FilesControllerHelperInt.GetFileVersionInfo(fileId);
        }

        [Read("file/{fileId}/presigned")]
        public DocumentService.FileLink GetPresignedUri(string fileId)
        {
            return FilesControllerHelperString.GetPresignedUri(fileId);
        }

        [Read("file/{fileId:int}/presigned")]
        public DocumentService.FileLink GetPresignedUri(int fileId)
        {
            return FilesControllerHelperInt.GetPresignedUri(fileId);
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
        public IEnumerable<FileWrapper<string>> ChangeHistoryFromBody(string fileId, [FromBody] ChangeHistoryModel model)
        {
            return FilesControllerHelperString.ChangeHistory(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/history")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileWrapper<string>> ChangeHistoryFromForm(string fileId, [FromForm] ChangeHistoryModel model)
        {
            return FilesControllerHelperString.ChangeHistory(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        public IEnumerable<FileWrapper<int>> ChangeHistoryFromBody(int fileId, [FromBody] ChangeHistoryModel model)
        {
            return FilesControllerHelperInt.ChangeHistory(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileWrapper<int>> ChangeHistoryFromForm(int fileId, [FromForm] ChangeHistoryModel model)
        {
            return FilesControllerHelperInt.ChangeHistory(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/lock")]
        public FileWrapper<string> LockFileFromBody(string fileId, [FromBody] LockFileModel model)
        {
            return FilesControllerHelperString.LockFile(fileId, model.LockFile);
        }

        [Update("file/{fileId}/lock")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<string> LockFileFromForm(string fileId, [FromForm] LockFileModel model)
        {
            return FilesControllerHelperString.LockFile(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        public FileWrapper<int> LockFileFromBody(int fileId, [FromBody] LockFileModel model)
        {
            return FilesControllerHelperInt.LockFile(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        [Consumes("application/x-www-form-urlencoded")]
        public FileWrapper<int> LockFileFromForm(int fileId, [FromForm] LockFileModel model)
        {
            return FilesControllerHelperInt.LockFile(fileId, model.LockFile);
        }

        [Update("file/{fileId}/comment")]
        public object UpdateCommentFromBody(string fileId, [FromBody] UpdateCommentModel model)
        {
            return FilesControllerHelperString.UpdateComment(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId}/comment")]
        [Consumes("application/x-www-form-urlencoded")]
        public object UpdateCommentFromForm(string fileId, [FromForm] UpdateCommentModel model)
        {
            return FilesControllerHelperString.UpdateComment(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId:int}/comment")]
        public object UpdateCommentFromBody(int fileId, [FromBody] UpdateCommentModel model)
        {
            return FilesControllerHelperInt.UpdateComment(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId:int}/comment")]
        [Consumes("application/x-www-form-urlencoded")]
        public object UpdateCommentFromForm(int fileId, [FromForm] UpdateCommentModel model)
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
        [Read("file/{fileId}/share")]
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
        [Read("folder/{folderId}/share")]
        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(string folderId)
        {
            return FilesControllerHelperString.GetFolderSecurityInfo(folderId);
        }

        [Read("folder/{folderId:int}/share")]
        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(int folderId)
        {
            return FilesControllerHelperInt.GetFolderSecurityInfo(folderId);
        }

        [Create("share")]
        public IEnumerable<FileShareWrapper> GetSecurityInfoFromBody([FromBody] BaseBatchModel<JsonElement> model)
        {
            var result = new List<FileShareWrapper>();
            result.AddRange(FilesControllerHelperInt.GetSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32())));
            result.AddRange(FilesControllerHelperString.GetSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString())));
            return result;
        }

        [Create("share")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileShareWrapper> GetSecurityInfoFromForm([FromForm] BaseBatchModel<JsonElement> model)
        {
            var result = new List<FileShareWrapper>();
            result.AddRange(FilesControllerHelperInt.GetSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32())));
            result.AddRange(FilesControllerHelperString.GetSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString())));
            return result;
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
        public IEnumerable<FileShareWrapper> SetFileSecurityInfoFromBody(string fileId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFileSecurityInfo(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileShareWrapper> SetFileSecurityInfoFromForm(string fileId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFileSecurityInfo(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        public IEnumerable<FileShareWrapper> SetFileSecurityInfoFromBody(int fileId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFileSecurityInfo(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileShareWrapper> SetFileSecurityInfoFromForm(int fileId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFileSecurityInfo(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("share")]
        public IEnumerable<FileShareWrapper> SetSecurityInfoFromBody([FromBody] SecurityInfoModel model)
        {
            return SetSecurityInfo(model);
        }

        [Update("share")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileShareWrapper> SetSecurityInfoFromForm([FromForm] SecurityInfoModel model)
        {
            return SetSecurityInfo(model);
        }

        public IEnumerable<FileShareWrapper> SetSecurityInfo(SecurityInfoModel model)
        {
            var result = new List<FileShareWrapper>();
            result.AddRange(FilesControllerHelperInt.SetSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()).ToList(), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()).ToList(), model.Share, model.Notify, model.SharingMessage));
            result.AddRange(FilesControllerHelperString.SetSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()).ToList(), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()).ToList(), model.Share, model.Notify, model.SharingMessage));
            return result;
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
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfoFromBody(string folderId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFolderSecurityInfo(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfoFromForm(string folderId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFolderSecurityInfo(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId:int}/share")]
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfoFromBody(int folderId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFolderSecurityInfo(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId:int}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileShareWrapper> SetFolderSecurityInfoFromForm(int folderId, [FromForm] SecurityInfoModel model)
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
        public bool RemoveSecurityInfo(BaseBatchModel<JsonElement> model)
        {
            FilesControllerHelperInt.RemoveSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()).ToList(), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()).ToList());
            FilesControllerHelperString.RemoveSecurityInfo(model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()).ToList(), model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()).ToList());
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
        public object GenerateSharedLinkFromBody(string fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperString.GenerateSharedLink(fileId, model.Share);
        }

        [Update("{fileId}/sharedlink")]
        [Consumes("application/x-www-form-urlencoded")]
        public object GenerateSharedLinkFromForm(string fileId, [FromForm] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperString.GenerateSharedLink(fileId, model.Share);
        }

        [Update("{fileId:int}/sharedlink")]
        public object GenerateSharedLinkFromBody(int fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperInt.GenerateSharedLink(fileId, model.Share);
        }

        [Update("{fileId:int}/sharedlink")]
        [Consumes("application/x-www-form-urlencoded")]
        public object GenerateSharedLinkFromForm(int fileId, [FromForm] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperInt.GenerateSharedLink(fileId, model.Share);
        }

        [Update("{fileId:int}/setacelink")]
        public bool SetAceLink(int fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperInt.SetAceLink(fileId, model.Share);
        }

        [Update("{fileId}/setacelink")]
        public bool SetAceLink(string fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperString.SetAceLink(fileId, model.Share);
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

            return ThirdpartyConfiguration.GetProviders();
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
        public FolderWrapper<string> SaveThirdPartyFromBody([FromBody] ThirdPartyModel model)
        {
            return SaveThirdParty(model);
        }

        [Create("thirdparty")]
        [Consumes("application/x-www-form-urlencoded")]
        public FolderWrapper<string> SaveThirdPartyFromForm([FromForm] ThirdPartyModel model)
        {
            return SaveThirdParty(model);
        }

        private FolderWrapper<string> SaveThirdParty(ThirdPartyModel model)
        {
            var thirdPartyParams = new ThirdPartyParams
            {
                AuthData = new AuthData(model.Url, model.Login, model.Password, model.Token),
                Corporate = model.IsCorporate,
                CustomerTitle = model.CustomerTitle,
                ProviderId = model.ProviderId,
                ProviderKey = model.ProviderKey,
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
            return EntryManager.GetThirpartyFolders(parent).Select(r => FolderWrapperHelper.Get(r)).ToList();
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
        public bool AddFavoritesFromBody([FromBody] BaseBatchModel<JsonElement> model)
        {
            return AddFavorites(model);
        }

        [Create("favorites")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool AddFavoritesFromForm([FromForm] BaseBatchModel<JsonElement> model)
        {
            return AddFavorites(model);
        }

        private bool AddFavorites(BaseBatchModel<JsonElement> model)
        {
            FileStorageServiceInt.AddToFavorites(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()), model.FileIds.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()));
            FileStorageService.AddToFavorites(model.FolderIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()), model.FileIds.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()));
            return true;
        }

        [Read("favorites/{fileId}")]
        public bool ToggleFileFavorite(string fileId, bool favorite)
        {
            return FileStorageService.ToggleFileFavorite(fileId, favorite);
        }

        [Read("favorites/{fileId:int}")]
        public bool ToggleFavoriteFromForm(int fileId, bool favorite)
        {
            return FileStorageServiceInt.ToggleFileFavorite(fileId, favorite);
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
        public bool AddTemplatesFromBody([FromBody] TemplatesModel model)
        {
            FileStorageServiceInt.AddToTemplates(model.FileIds);
            return true;
        }

        [Create("templates")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool AddTemplatesFromForm([FromForm] TemplatesModel model)
        {
            FileStorageServiceInt.AddToTemplates(model.FileIds);
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
        public bool StoreOriginalFromBody([FromBody] SettingsModel model)
        {
            return FileStorageService.StoreOriginal(model.Set);
        }

        [Update(@"storeoriginal")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool StoreOriginalFromForm([FromForm] SettingsModel model)
        {
            return FileStorageService.StoreOriginal(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Read(@"settings")]
        public FilesSettingsHelper GetFilesSettings()
        {
            return FilesSettingsHelper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="save"></param>
        /// <visible>false</visible>
        /// <returns></returns>
        [Update(@"hideconfirmconvert")]
        public bool HideConfirmConvertFromBody([FromBody] HideConfirmConvertModel model)
        {
            return FileStorageService.HideConfirmConvert(model.Save);
        }

        [Update(@"hideconfirmconvert")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool HideConfirmConvertFromForm([FromForm] HideConfirmConvertModel model)
        {
            return FileStorageService.HideConfirmConvert(model.Save);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"updateifexist")]
        public bool UpdateIfExistFromBody([FromBody] SettingsModel model)
        {
            return FileStorageService.UpdateIfExist(model.Set);
        }

        [Update(@"updateifexist")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool UpdateIfExistFromForm([FromForm] SettingsModel model)
        {
            return FileStorageService.UpdateIfExist(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"changedeleteconfrim")]
        public bool ChangeDeleteConfrimFromBody([FromBody] SettingsModel model)
        {
            return FileStorageService.ChangeDeleteConfrim(model.Set);
        }

        [Update(@"changedeleteconfrim")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool ChangeDeleteConfrimFromForm([FromForm] SettingsModel model)
        {
            return FileStorageService.ChangeDeleteConfrim(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"storeforcesave")]
        public bool StoreForcesaveFromBody([FromBody] SettingsModel model)
        {
            return FileStorageService.StoreForcesave(model.Set);
        }

        [Update(@"storeforcesave")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool StoreForcesaveFromForm([FromForm] SettingsModel model)
        {
            return FileStorageService.StoreForcesave(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"forcesave")]
        public bool ForcesaveFromBody([FromBody] SettingsModel model)
        {
            return FileStorageService.Forcesave(model.Set);
        }

        [Update(@"forcesave")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool ForcesaveFromForm([FromForm] SettingsModel model)
        {
            return FileStorageService.Forcesave(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [Update(@"thirdparty")]
        public bool ChangeAccessToThirdpartyFromBody([FromBody] SettingsModel model)
        {
            return FileStorageService.ChangeAccessToThirdparty(model.Set);
        }

        [Update(@"thirdparty")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool ChangeAccessToThirdpartyFromForm([FromForm] SettingsModel model)
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
        public bool DisplayRecentFromBody([FromBody] DisplayModel model)
        {
            return FileStorageService.DisplayRecent(model.Set);
        }

        [Update(@"displayRecent")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool DisplayRecentFromForm([FromForm] DisplayModel model)
        {
            return FileStorageService.DisplayRecent(model.Set);
        }

        /// <summary>
        /// Display favorite folder
        /// </summary>
        /// <param name="set"></param>
        /// <category>Settings</category>
        /// <returns></returns>
        [Update(@"settings/favorites")]
        public bool DisplayFavoriteFromBody([FromBody] DisplayModel model)
        {
            return FileStorageService.DisplayFavorite(model.Set);
        }

        [Update(@"settings/favorites")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool DisplayFavoriteFromForm([FromForm] DisplayModel model)
        {
            return FileStorageService.DisplayFavorite(model.Set);
        }

        /// <summary>
        /// Display template folder
        /// </summary>
        /// <param name="set"></param>
        /// <category>Settings</category>
        /// <returns></returns>
        [Update(@"settings/templates")]
        public bool DisplayTemplatesFromBody([FromBody] DisplayModel model)
        {
            return FileStorageService.DisplayTemplates(model.Set);
        }

        [Update(@"settings/templates")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool DisplayTemplatesFromForm([FromForm] DisplayModel model)
        {
            return FileStorageService.DisplayTemplates(model.Set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <category>Settings</category>
        /// <returns></returns>
        [Update(@"settings/downloadtargz")]
        public bool ChangeDownloadZipFromBody([FromBody] DisplayModel model)
        {
            return FileStorageService.ChangeDownloadTarGz(model.Set);
        }

        [Update(@"settings/downloadtargz")]
        public bool ChangeDownloadZipFromForm([FromForm] DisplayModel model)
        {
            return FileStorageService.ChangeDownloadTarGz(model.Set);
        }

        /// <summary>
        ///  Checking document service location
        /// </summary>
        /// <param name="docServiceUrl">Document editing service Domain</param>
        /// <param name="docServiceUrlInternal">Document command service Domain</param>
        /// <param name="docServiceUrlPortal">Community Server Address</param>
        /// <returns></returns>
        [Update("docservice")]
        public IEnumerable<string> CheckDocServiceUrlFromBody([FromBody] CheckDocServiceUrlModel model)
        {
            return CheckDocServiceUrl(model);
        }

        [Update("docservice")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<string> CheckDocServiceUrlFromForm([FromForm] CheckDocServiceUrlModel model)
        {
            return CheckDocServiceUrl(model);
        }

        /// <summary>
        /// Create thumbnails for files with the IDs specified in the request
        /// </summary>
        /// <short>Create thumbnails</short>
        /// <category>Files</category>
        /// <param name="fileIds">File IDs</param>
        /// <visible>false</visible>
        /// <returns></returns>
        [Create("thumbnails")]
        public IEnumerable<JsonElement> CreateThumbnailsFromBody([FromBody] BaseBatchModel<JsonElement> model)
        {
            return FileStorageService.CreateThumbnails(model.FileIds);
        }

        [Create("thumbnails")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<JsonElement> CreateThumbnailsFromForm([FromForm] BaseBatchModel<JsonElement> model)
        {
            return FileStorageService.CreateThumbnails(model.FileIds);
        }

        public IEnumerable<string> CheckDocServiceUrl(CheckDocServiceUrlModel model)
        {
            FilesLinkUtility.DocServiceUrl = model.DocServiceUrl;
            FilesLinkUtility.DocServiceUrlInternal = model.DocServiceUrlInternal;
            FilesLinkUtility.DocServicePortalUrl = model.DocServiceUrlPortal;

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
        [AllowAnonymous]
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
        public object WordpressSaveFromBody([FromBody] WordpressSaveModel model)
        {
            return WordpressSave(model);
        }

        [Create("wordpress-save")]
        [Consumes("application/x-www-form-urlencoded")]
        public object WordpressSaveFromForm([FromForm] WordpressSaveModel model)
        {
            return WordpressSave(model);
        }

        private object WordpressSave(WordpressSaveModel model)
        {
            if (model.Code == "")
            {
                return new
                {
                    success = false
                };
            }
            try
            {
                var token = WordpressToken.SaveTokenFromCode(model.Code);
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
        public bool CreateWordpressPostFromBody([FromBody] CreateWordpressPostModel model)
        {
            return CreateWordpressPost(model);
        }

        [Create("wordpress")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool CreateWordpressPostFromForm([FromForm] CreateWordpressPostModel model)
        {
            return CreateWordpressPost(model);
        }

        private bool CreateWordpressPost(CreateWordpressPostModel model)
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
                    var createPost = WordpressHelper.CreateWordpressPost(model.Title, model.Content, model.Status, blogId, token);
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
        public object EasyBibCitationBookFromBody([FromBody] EasyBibCitationBookModel model)
        {
            return EasyBibCitationBook(model);
        }

        [Create("easybib-citation")]
        [Consumes("application/x-www-form-urlencoded")]
        public object EasyBibCitationBookFromForm([FromForm] EasyBibCitationBookModel model)
        {
            return EasyBibCitationBook(model);
        }

        private object EasyBibCitationBook(EasyBibCitationBookModel model)
        {
            try
            {
                var citat = EasyBibHelper.GetEasyBibCitation(model.CitationData);
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