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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<FolderContentWrapper<int>>> GetRootFoldersAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders, bool withoutTrash, bool withoutAdditionalFolder)
        {
            var IsVisitor = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor(UserManager);
            var IsOutsider = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOutsider(UserManager);
            var folders = new SortedSet<int>();

            if (IsOutsider)
            {
                withoutTrash = true;
                withoutAdditionalFolder = true;
            }

            if (!IsVisitor)
            {
                folders.Add(GlobalFolderHelper.FolderMy);
            }

            if (!CoreBaseSettings.Personal && !UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOutsider(UserManager))
            {
                folders.Add(GlobalFolderHelper.FolderShare);
            }

            if (!IsVisitor && !withoutAdditionalFolder)
            {
                if (FilesSettingsHelper.FavoritesSection)
                {
                    folders.Add(GlobalFolderHelper.FolderFavorites);
                }

                if (FilesSettingsHelper.RecentSection)
                {
                    folders.Add(GlobalFolderHelper.FolderRecent);
                }

                if (!CoreBaseSettings.Personal && PrivacyRoomSettings.IsAvailable(TenantManager))
                {
                    folders.Add(GlobalFolderHelper.FolderPrivacy);
                }
            }

            if (!CoreBaseSettings.Personal)
            {
                folders.Add(GlobalFolderHelper.FolderCommon);
            }

            if (!IsVisitor
               && !withoutAdditionalFolder
               && FileUtility.ExtsWebTemplate.Any()
               && FilesSettingsHelper.TemplatesSection)
            {
                folders.Add(GlobalFolderHelper.FolderTemplates);
            }

            if (!withoutTrash)
            {
                folders.Add((int)GlobalFolderHelper.FolderTrash);
            }

            var result = new List<FolderContentWrapper<int>>();
            foreach (var folder in folders)
            {
                result.Add(await FilesControllerHelperInt.GetFolderAsync(folder, userIdOrGroupId, filterType, withsubfolders));
            }

            return result;
        }

        [Read("@privacy")]
        public async Task<FolderContentWrapper<int>> GetPrivacyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            if (!IsAvailablePrivacyRoomSettings()) throw new System.Security.SecurityException();
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderPrivacy, userIdOrGroupId, filterType, withsubfolders);
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
        public async Task<FolderContentWrapper<int>> GetMyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderMy, userIdOrGroupId, filterType, withsubfolders);
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
        public async Task<FolderContentWrapper<string>> GetProjectsFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperString.GetFolderAsync(GlobalFolderHelper.GetFolderProjects<string>(), userIdOrGroupId, filterType, withsubfolders);
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
        public async Task<FolderContentWrapper<int>> GetCommonFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderCommon, userIdOrGroupId, filterType, withsubfolders);
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
        public async Task<FolderContentWrapper<int>> GetShareFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderShare, userIdOrGroupId, filterType, withsubfolders);
        }

        /// <summary>
        /// Returns the detailed list of recent files
        /// </summary>
        /// <short>Section Recent</short>
        /// <category>Folders</category>
        /// <returns>Recent contents</returns>
        [Read("@recent")]
        public async Task<FolderContentWrapper<int>> GetRecentFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderRecent, userIdOrGroupId, filterType, withsubfolders);
        }

        [Create("file/{fileId}/recent", order: int.MaxValue)]
        public async Task<FileWrapper<string>> AddToRecentAsync(string fileId)
        {
            return await FilesControllerHelperString.AddToRecentAsync(fileId);
        }

        [Create("file/{fileId:int}/recent", order: int.MaxValue - 1)]
        public async Task<FileWrapper<int>> AddToRecentAsync(int fileId)
        {
            return await FilesControllerHelperInt.AddToRecentAsync(fileId);
        }

        /// <summary>
        /// Returns the detailed list of favorites files
        /// </summary>
        /// <short>Section Favorite</short>
        /// <category>Folders</category>
        /// <returns>Favorites contents</returns>
        [Read("@favorites")]
        public async Task<FolderContentWrapper<int>> GetFavoritesFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderFavorites, userIdOrGroupId, filterType, withsubfolders);
        }

        /// <summary>
        /// Returns the detailed list of templates files
        /// </summary>
        /// <short>Section Template</short>
        /// <category>Folders</category>
        /// <returns>Templates contents</returns>
        [Read("@templates")]
        public async Task<FolderContentWrapper<int>> GetTemplatesFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderTemplates, userIdOrGroupId, filterType, withsubfolders);
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
        public async Task<FolderContentWrapper<int>> GetTrashFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(Convert.ToInt32(GlobalFolderHelper.FolderTrash), userIdOrGroupId, filterType, withsubfolders);
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
        public async Task<FolderContentWrapper<string>> GetFolderAsync(string folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            var folder = await FilesControllerHelperString.GetFolderAsync(folderId, userIdOrGroupId, filterType, withsubfolders);
            return folder.NotFoundIfNull();
        }

        [Read("{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public async Task<FolderContentWrapper<int>> GetFolderAsync(int folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(folderId, userIdOrGroupId, filterType, withsubfolders);
        }

        [Read("{folderId}/subfolders")]
        public async Task<IEnumerable<FileEntryWrapper>> GetFoldersAsync(string folderId)
        {
            return await FilesControllerHelperString.GetFoldersAsync(folderId).ToListAsync();
        }

        [Read("{folderId:int}/subfolders")]
        public async Task<IEnumerable<FileEntryWrapper>> GetFoldersAsync(int folderId)
        {
            return await FilesControllerHelperInt.GetFoldersAsync(folderId).ToListAsync();
        }

        [Read("{folderId}/news")]
        public async Task<List<FileEntryWrapper>> GetNewItemsAsync(string folderId)
        {
            return await FilesControllerHelperString.GetNewItemsAsync(folderId);
        }

        [Read("{folderId:int}/news")]
        public async Task<List<FileEntryWrapper>> GetNewItemsAsync(int folderId)
        {
            return await FilesControllerHelperInt.GetNewItemsAsync(folderId);
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
        public async Task<object> UploadFileToMyAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return await FilesControllerHelperInt.UploadFileAsync(GlobalFolderHelper.FolderMy, uploadModel);
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
        public async Task<object> UploadFileToCommonAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return await FilesControllerHelperInt.UploadFileAsync(GlobalFolderHelper.FolderCommon, uploadModel);
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
        [Create("{folderId}/upload", order: int.MaxValue)]
        public async Task<object> UploadFileAsync(string folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            return await FilesControllerHelperString.UploadFileAsync(folderId, uploadModel);
        }

        [Create("{folderId:int}/upload", order: int.MaxValue - 1)]
        public async Task<object> UploadFileAsync(int folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            return await FilesControllerHelperInt.UploadFileAsync(folderId, uploadModel);
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
        public async Task<FileWrapper<int>> InsertFileToMyFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return await InsertFileAsync(GlobalFolderHelper.FolderMy, model);
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
        public async Task<FileWrapper<int>> InsertFileToCommonFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return await InsertFileAsync(GlobalFolderHelper.FolderCommon, model);
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
        public async Task<FileWrapper<string>> InsertFileAsync(string folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return await FilesControllerHelperString.InsertFileAsync(folderId, model.Stream, model.Title, model.CreateNewIfExist, model.KeepConvertStatus);
        }

        [Create("{folderId:int}/insert", order: int.MaxValue - 1)]
        public async Task<FileWrapper<int>> InsertFileFromFormAsync(int folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return await InsertFileAsync(folderId, model);
        }

        private async Task<FileWrapper<int>> InsertFileAsync(int folderId, InsertFileModel model)
        {
            return await FilesControllerHelperInt.InsertFileAsync(folderId, model.Stream, model.Title, model.CreateNewIfExist, model.KeepConvertStatus);
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
            return FilesControllerHelperString.UpdateFileStream(FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream(), fileId, model.Encrypted, model.Forcesave);
        }

        [Update("{fileId:int}/update")]
        public FileWrapper<int> UpdateFileStreamFromForm(int fileId, [FromForm] FileStreamModel model)
        {
            return FilesControllerHelperInt.UpdateFileStream(FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream(), fileId, model.Encrypted, model.Forcesave);
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
        public async Task<FileWrapper<string>> SaveEditingFromFormAsync(string fileId, [FromForm] SaveEditingModel model)
        {
            using var stream = FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream();
            return await FilesControllerHelperString.SaveEditingAsync(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
        }

        [Update("file/{fileId:int}/saveediting")]
        public async Task<FileWrapper<int>> SaveEditingFromFormAsync(int fileId, [FromForm] SaveEditingModel model)
        {
            using var stream = FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream();
            return await FilesControllerHelperInt.SaveEditingAsync(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
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
        [Consumes("application/json")]
        public async Task<object> StartEditFromBodyAsync(string fileId, [FromBody] StartEditModel model)
        {
            return await FilesControllerHelperString.StartEditAsync(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId}/startedit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> StartEditFromFormAsync(string fileId, [FromForm] StartEditModel model)
        {
            return await FilesControllerHelperString.StartEditAsync(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId:int}/startedit")]
        [Consumes("application/json")]
        public async Task<object> StartEditFromBodyAsync(int fileId, [FromBody] StartEditModel model)
        {
            return await FilesControllerHelperInt.StartEditAsync(fileId, model.EditingAlone, model.Doc);
        }

        [Create("file/{fileId:int}/startedit")]
        public async Task<object> StartEditAsync(int fileId)
        {
            return await FilesControllerHelperInt.StartEditAsync(fileId, false, null);
        }

        [Create("file/{fileId:int}/startedit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> StartEditFromFormAsync(int fileId, [FromForm] StartEditModel model)
        {
            return await FilesControllerHelperInt.StartEditAsync(fileId, model.EditingAlone, model.Doc);
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
        public async Task<KeyValuePair<bool, string>> TrackEditFileAsync(string fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return await FilesControllerHelperString.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        [Read("file/{fileId:int}/trackeditfile")]
        public async Task<KeyValuePair<bool, string>> TrackEditFileAsync(int fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return await FilesControllerHelperInt.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
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
        public async Task<Configuration<string>> OpenEditAsync(string fileId, int version, string doc, bool view)
        {
            return await FilesControllerHelperString.OpenEditAsync(fileId, version, doc, view);
        }

        [AllowAnonymous]
        [Read("file/{fileId:int}/openedit", Check = false)]
        public async Task<Configuration<int>> OpenEditAsync(int fileId, int version, string doc, bool view)
        {
            return await FilesControllerHelperInt.OpenEditAsync(fileId, version, doc, view);
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
        public async Task<object> CreateUploadSessionFromBodyAsync(string folderId, [FromBody] SessionModel sessionModel)
        {
            return await FilesControllerHelperString.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId}/upload/create_session")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> CreateUploadSessionFromFormAsync(string folderId, [FromForm] SessionModel sessionModel)
        {
            return await FilesControllerHelperString.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        public async Task<object> CreateUploadSessionFromBodyAsync(int folderId, [FromBody] SessionModel sessionModel)
        {
            return await FilesControllerHelperInt.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> CreateUploadSessionFromFormAsync(int folderId, [FromForm] SessionModel sessionModel)
        {
            return await FilesControllerHelperInt.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.Encrypted);
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
        public async Task<FileWrapper<int>> CreateTextFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(GlobalFolderHelper.FolderMy, model);
        }

        [Create("@my/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateTextFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(GlobalFolderHelper.FolderMy, model);
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
        public async Task<FileWrapper<int>> CreateTextFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(GlobalFolderHelper.FolderCommon, model);
        }

        [Create("@common/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateTextFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(GlobalFolderHelper.FolderCommon, model);
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
        public async Task<FileWrapper<string>> CreateTextFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(folderId, model);
        }

        [Create("{folderId}/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<string>> CreateTextFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(folderId, model);
        }

        private async Task<FileWrapper<string>> CreateTextFileAsync(string folderId, CreateTextOrHtmlFileModel model)
        {
            return await FilesControllerHelperString.CreateTextFileAsync(folderId, model.Title, model.Content);
        }

        [Create("{folderId:int}/text")]
        public async Task<FileWrapper<int>> CreateTextFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(folderId, model);
        }

        [Create("{folderId:int}/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateTextFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(folderId, model);
        }

        private async Task<FileWrapper<int>> CreateTextFileAsync(int folderId, CreateTextOrHtmlFileModel model)
        {
            return await FilesControllerHelperInt.CreateTextFileAsync(folderId, model.Title, model.Content);
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
        public async Task<FileWrapper<string>> CreateHtmlFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(folderId, model);
        }

        [Create("{folderId}/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<string>> CreateHtmlFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(folderId, model);
        }

        private async Task<FileWrapper<string>> CreateHtmlFileAsync(string folderId, CreateTextOrHtmlFileModel model)
        {
            return await FilesControllerHelperString.CreateHtmlFileAsync(folderId, model.Title, model.Content);
        }

        [Create("{folderId:int}/html")]
        public async Task<FileWrapper<int>> CreateHtmlFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(folderId, model);
        }

        [Create("{folderId:int}/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateHtmlFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(folderId, model);
        }

        private async Task<FileWrapper<int>> CreateHtmlFileAsync(int folderId, CreateTextOrHtmlFileModel model)
        {
            return await FilesControllerHelperInt.CreateHtmlFileAsync(folderId, model.Title, model.Content);
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
        public async Task<FileWrapper<int>> CreateHtmlFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(GlobalFolderHelper.FolderMy, model);
        }

        [Create("@my/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateHtmlFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(GlobalFolderHelper.FolderMy, model);
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
        public async Task<FileWrapper<int>> CreateHtmlFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(GlobalFolderHelper.FolderCommon, model);
        }

        [Create("@common/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateHtmlFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(GlobalFolderHelper.FolderCommon, model);
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
        [Create("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        public async Task<FolderWrapper<string>> CreateFolderFromBodyAsync(string folderId, [FromBody] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperString.CreateFolderAsync(folderId, folderModel.Title);
        }

        [Create("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FolderWrapper<string>> CreateFolderFromFormAsync(string folderId, [FromForm] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperString.CreateFolderAsync(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public async Task<FolderWrapper<int>> CreateFolderFromBodyAsync(int folderId, [FromBody] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperInt.CreateFolderAsync(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FolderWrapper<int>> CreateFolderFromFormAsync(int folderId, [FromForm] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperInt.CreateFolderAsync(folderId, folderModel.Title);
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
        public async Task<FileWrapper<int>> CreateFileFromBodyAsync([FromBody] CreateFileModel<int> model)
        {
            return await FilesControllerHelperInt.CreateFileAsync(GlobalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("@my/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateFileFromFormAsync([FromForm] CreateFileModel<int> model)
        {
            return await FilesControllerHelperInt.CreateFileAsync(GlobalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
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
        public async Task<FileWrapper<string>> CreateFileFromBodyAsync(string folderId, [FromBody] CreateFileModel<string> model)
        {
            return await FilesControllerHelperString.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId}/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<string>> CreateFileFromFormAsync(string folderId, [FromForm] CreateFileModel<string> model)
        {
            return await FilesControllerHelperString.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId:int}/file")]
        public async Task<FileWrapper<int>> CreateFileFromBodyAsync(int folderId, [FromBody] CreateFileModel<int> model)
        {
            return await FilesControllerHelperInt.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId:int}/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateFileFromFormAsync(int folderId, [FromForm] CreateFileModel<int> model)
        {
            return await FilesControllerHelperInt.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
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

        [Update("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        public async Task<FolderWrapper<string>> RenameFolderFromBodyAsync(string folderId, [FromBody] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperString.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Update("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FolderWrapper<string>> RenameFolderFromFormAsync(string folderId, [FromForm] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperString.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public async Task<FolderWrapper<int>> RenameFolderFromBodyAsync(int folderId, [FromBody] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperInt.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FolderWrapper<int>> RenameFolderFromFormAsync(int folderId, [FromForm] CreateFolderModel folderModel)
        {
            return await FilesControllerHelperInt.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Create("owner")]
        public async Task<IEnumerable<FileEntryWrapper>> ChangeOwnerFromBodyAsync([FromBody] ChangeOwnerModel model)
        {
            return await ChangeOwnerAsync(model).ToListAsync();
        }

        [Create("owner")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileEntryWrapper>> ChangeOwnerFromFormAsync([FromForm] ChangeOwnerModel model)
        {
            return await ChangeOwnerAsync(model).ToListAsync();
        }

        public async IAsyncEnumerable<FileEntryWrapper> ChangeOwnerAsync(ChangeOwnerModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            var result = new List<FileEntry>();
            result.AddRange(await FileStorageServiceInt.ChangeOwnerAsync(folderIntIds, fileIntIds, model.UserId));
            result.AddRange(await FileStorageService.ChangeOwnerAsync(folderStringIds, fileStringIds, model.UserId));

            foreach (var e in result)
            {
                yield return await FilesControllerHelperInt.GetFileEntryWrapperAsync(e);
            }
        }

        /// <summary>
        /// Returns a detailed information about the folder with the ID specified in the request
        /// </summary>
        /// <short>Folder information</short>
        /// <category>Folders</category>
        /// <returns>Folder info</returns>

        [Read("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        public async Task<FolderWrapper<string>> GetFolderInfoAsync(string folderId)
        {
            return await FilesControllerHelperString.GetFolderInfoAsync(folderId);
        }

        [Read("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public async Task<FolderWrapper<int>> GetFolderInfoAsync(int folderId)
        {
            return await FilesControllerHelperInt.GetFolderInfoAsync(folderId);
        }

        /// <summary>
        /// Returns parent folders
        /// </summary>
        /// <param name="folderId"></param>
        /// <category>Folders</category>
        /// <returns>Parent folders</returns>

        [Read("folder/{folderId}/path")]
        public async Task<IEnumerable<FileEntryWrapper>> GetFolderPathAsync(string folderId)
        {
            return await FilesControllerHelperString.GetFolderPathAsync(folderId).ToListAsync();
        }


        [Read("folder/{folderId:int}/path")]
        public async Task<IEnumerable<FileEntryWrapper>> GetFolderPathAsync(int folderId)
        {
            return await FilesControllerHelperInt.GetFolderPathAsync(folderId).ToListAsync();
        }

        /// <summary>
        /// Returns a detailed information about the file with the ID specified in the request
        /// </summary>
        /// <short>File information</short>
        /// <category>Files</category>
        /// <returns>File info</returns>

        [Read("fileAsync/{fileId}", order: int.MaxValue, DisableFormat = true)]
        public async Task<FileWrapper<string>> GetFileInfoAsync(string fileId, int version = -1)
        {
            return await FilesControllerHelperString.GetFileInfoAsync(fileId, version);
        }

        [Read("fileAsync/{fileId:int}")]
        public async Task<FileWrapper<int>> GetFileInfoAsync(int fileId, int version = -1)
        {
            return await FilesControllerHelperInt.GetFileInfoAsync(fileId, version);
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
        [Update("fileAsync/{fileId}", order: int.MaxValue, DisableFormat = true)]
        public async Task<FileWrapper<string>> UpdateFileFromBodyAsync(string fileId, [FromBody] UpdateFileModel model)
        {
            return await FilesControllerHelperString.UpdateFileAsync(fileId, model.Title, model.LastVersion);
        }

        [Update("fileAsync/{fileId}", order: int.MaxValue, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<string>> UpdateFileFromFormAsync(string fileId, [FromForm] UpdateFileModel model)
        {
            return await FilesControllerHelperString.UpdateFileAsync(fileId, model.Title, model.LastVersion);
        }

        [Update("fileAsync/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public async Task<FileWrapper<int>> UpdateFileFromBodyAsync(int fileId, [FromBody] UpdateFileModel model)
        {
            return await FilesControllerHelperInt.UpdateFileAsync(fileId, model.Title, model.LastVersion);
        }

        [Update("fileAsync/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> UpdateFileFromFormAsync(int fileId, [FromForm] UpdateFileModel model)
        {
            return await FilesControllerHelperInt.UpdateFileAsync(fileId, model.Title, model.LastVersion);
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
        [Delete("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
        public IEnumerable<FileOperationWraper> DeleteFile(string fileId, [FromBody] DeleteModel model)
        {
            return FilesControllerHelperString.DeleteFile(fileId, model.DeleteAfter, model.Immediately);
        }

        [Delete("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
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
        public async Task<IEnumerable<ConversationResult<string>>> StartConversionAsync(string fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel model)
        {
            return await FilesControllerHelperString.StartConversionAsync(fileId, model?.Sync ?? false).ToListAsync();
        }

        [Update("file/{fileId:int}/checkconversion")]
        public async Task<IEnumerable<ConversationResult<int>>> StartConversionAsync(int fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel model)
        {
            return await FilesControllerHelperInt.StartConversionAsync(fileId, model?.Sync ?? false).ToListAsync();
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
        public async Task<IEnumerable<ConversationResult<string>>> CheckConversionAsync(string fileId, bool start)
        {
            return await FilesControllerHelperString.CheckConversionAsync(fileId, start).ToListAsync();
        }


        [Read("file/{fileId:int}/checkconversion")]
        public async Task<IEnumerable<ConversationResult<int>>> CheckConversionAsync(int fileId, bool start)
        {
            return await FilesControllerHelperInt.CheckConversionAsync(fileId, start).ToListAsync();
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
        [Delete("folder/{folderId}", order: int.MaxValue - 1, DisableFormat = true)]
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
        public async Task<IEnumerable<FileEntryWrapper>> MoveOrCopyBatchCheckAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
        {
            return await FilesControllerHelperString.MoveOrCopyBatchCheckAsync(batchModel).ToListAsync();
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
        public IEnumerable<FileOperationWraper> MoveBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
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
        public IEnumerable<FileOperationWraper> CopyBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
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
        public IEnumerable<FileOperationWraper> MarkAsReadFromBody([FromBody] BaseBatchModel model)
        {
            return FilesControllerHelperString.MarkAsRead(model);
        }

        [Update("fileops/markasread")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<FileOperationWraper> MarkAsReadFromForm([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
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
        public IEnumerable<FileOperationWraper> DeleteBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(DeleteBatchModelBinder))] DeleteBatchModel batch)
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
        public async Task<IEnumerable<FileWrapper<string>>> GetFileVersionInfoAsync(string fileId)
        {
            return await FilesControllerHelperString.GetFileVersionInfoAsync(fileId);
        }

        [Read("file/{fileId:int}/history")]
        public async Task<IEnumerable<FileWrapper<int>>> GetFileVersionInfoAsync(int fileId)
        {
            return await FilesControllerHelperInt.GetFileVersionInfoAsync(fileId);
        }

        [Read("file/{fileId}/presigned")]
        public async Task<DocumentService.FileLink> GetPresignedUriAsync(string fileId)
        {
            return await FilesControllerHelperString.GetPresignedUriAsync(fileId);
        }

        [Read("file/{fileId:int}/presigned")]
        public async Task<DocumentService.FileLink> GetPresignedUriAsync(int fileId)
        {
            return await FilesControllerHelperInt.GetPresignedUriAsync(fileId);
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
        public async Task<IEnumerable<FileWrapper<string>>> ChangeHistoryFromBodyAsync(string fileId, [FromBody] ChangeHistoryModel model)
        {
            return await FilesControllerHelperString.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/history")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileWrapper<string>>> ChangeHistoryFromFormAsync(string fileId, [FromForm] ChangeHistoryModel model)
        {
            return await FilesControllerHelperString.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        public async Task<IEnumerable<FileWrapper<int>>> ChangeHistoryFromBodyAsync(int fileId, [FromBody] ChangeHistoryModel model)
        {
            return await FilesControllerHelperInt.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileWrapper<int>>> ChangeHistoryFromFormAsync(int fileId, [FromForm] ChangeHistoryModel model)
        {
            return await FilesControllerHelperInt.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/lock")]
        public async Task<FileWrapper<string>> LockFileFromBodyAsync(string fileId, [FromBody] LockFileModel model)
        {
            return await FilesControllerHelperString.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId}/lock")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<string>> LockFileFromFormAsync(string fileId, [FromForm] LockFileModel model)
        {
            return await FilesControllerHelperString.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        public async Task<FileWrapper<int>> LockFileFromBodyAsync(int fileId, [FromBody] LockFileModel model)
        {
            return await FilesControllerHelperInt.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> LockFileFromFormAsync(int fileId, [FromForm] LockFileModel model)
        {
            return await FilesControllerHelperInt.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId}/comment")]
        public async Task<object> UpdateCommentFromBodyAsync(string fileId, [FromBody] UpdateCommentModel model)
        {
            return await FilesControllerHelperString.UpdateCommentAsync(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId}/comment")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> UpdateCommentFromFormAsync(string fileId, [FromForm] UpdateCommentModel model)
        {
            return await FilesControllerHelperString.UpdateCommentAsync(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId:int}/comment")]
        public async Task<object> UpdateCommentFromBodyAsync(int fileId, [FromBody] UpdateCommentModel model)
        {
            return await FilesControllerHelperInt.UpdateCommentAsync(fileId, model.Version, model.Comment);
        }

        [Update("file/{fileId:int}/comment")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> UpdateCommentFromFormAsync(int fileId, [FromForm] UpdateCommentModel model)
        {
            return await FilesControllerHelperInt.UpdateCommentAsync(fileId, model.Version, model.Comment);
        }

        /// <summary>
        /// Returns the detailed information about shared file with the ID specified in the request
        /// </summary>
        /// <short>File sharing</short>
        /// <category>Sharing</category>
        /// <param name="fileId">File ID</param>
        /// <returns>Shared file information</returns>

        [Read("file/{fileId}/share")]
        public async Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(string fileId)
        {
            return await FilesControllerHelperString.GetFileSecurityInfoAsync(fileId);
        }

        [Read("file/{fileId:int}/share")]
        public async Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(int fileId)
        {
            return await FilesControllerHelperInt.GetFileSecurityInfoAsync(fileId);
        }

        /// <summary>
        /// Returns the detailed information about shared folder with the ID specified in the request
        /// </summary>
        /// <short>Folder sharing</short>
        /// <param name="folderId">Folder ID</param>
        /// <category>Sharing</category>
        /// <returns>Shared folder information</returns>

        [Read("folder/{folderId}/share")]
        public async Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(string folderId)
        {
            return await FilesControllerHelperString.GetFolderSecurityInfoAsync(folderId);
        }

        [Read("folder/{folderId:int}/share")]
        public async Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(int folderId)
        {
            return await FilesControllerHelperInt.GetFolderSecurityInfoAsync(folderId);
        }

        [Create("share")]
        public async Task<IEnumerable<FileShareWrapper>> GetSecurityInfoFromBodyAsync([FromBody] BaseBatchModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            var result = new List<FileShareWrapper>();
            result.AddRange(await FilesControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds));
            result.AddRange(await FilesControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds));
            return result;
        }

        [Create("share")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileShareWrapper>> GetSecurityInfoFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            var result = new List<FileShareWrapper>();
            result.AddRange(await FilesControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds));
            result.AddRange(await FilesControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds));
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
        public async Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromBodyAsync(string fileId, [FromBody] SecurityInfoModel model)
        {
            return await FilesControllerHelperString.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromFormAsync(string fileId, [FromForm] SecurityInfoModel model)
        {
            return await FilesControllerHelperString.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        public async Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromBodyAsync(int fileId, [FromBody] SecurityInfoModel model)
        {
            return await FilesControllerHelperInt.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromFormAsync(int fileId, [FromForm] SecurityInfoModel model)
        {
            return await FilesControllerHelperInt.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("share")]
        public async Task<IEnumerable<FileShareWrapper>> SetSecurityInfoFromBodyAsync([FromBody] SecurityInfoModel model)
        {
            return await SetSecurityInfoAsync(model);
        }

        [Update("share")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileShareWrapper>> SetSecurityInfoFromFormAsync([FromForm] SecurityInfoModel model)
        {
            return await SetSecurityInfoAsync(model);
        }

        public async Task<IEnumerable<FileShareWrapper>> SetSecurityInfoAsync(SecurityInfoModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            var result = new List<FileShareWrapper>();
            result.AddRange(await FilesControllerHelperInt.SetSecurityInfoAsync(fileIntIds, folderIntIds, model.Share, model.Notify, model.SharingMessage));
            result.AddRange(await FilesControllerHelperString.SetSecurityInfoAsync(fileStringIds, folderStringIds, model.Share, model.Notify, model.SharingMessage));
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
        public async Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromBodyAsync(string folderId, [FromBody] SecurityInfoModel model)
        {
            return await FilesControllerHelperString.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromFormAsync(string folderId, [FromForm] SecurityInfoModel model)
        {
            return await FilesControllerHelperString.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId:int}/share")]
        public async Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromBodyAsync(int folderId, [FromBody] SecurityInfoModel model)
        {
            return await FilesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId:int}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromFormAsync(int folderId, [FromForm] SecurityInfoModel model)
        {
            return await FilesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
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
        public async Task<bool> RemoveSecurityInfoAsync(BaseBatchModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            await FilesControllerHelperInt.RemoveSecurityInfoAsync(fileIntIds, folderIntIds);
            await FilesControllerHelperString.RemoveSecurityInfoAsync(fileStringIds, folderStringIds);
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

        [Update("{fileId}/sharedlinkAsync")]
        public async Task<object> GenerateSharedLinkFromBodyAsync(string fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return await FilesControllerHelperString.GenerateSharedLinkAsync(fileId, model.Share);
        }

        [Update("{fileId}/sharedlinkAsync")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> GenerateSharedLinkFromFormAsync(string fileId, [FromForm] GenerateSharedLinkModel model)
        {
            return await FilesControllerHelperString.GenerateSharedLinkAsync(fileId, model.Share);
        }

        [Update("{fileId:int}/sharedlinkAsync")]
        public async Task<object> GenerateSharedLinkFromBodyAsync(int fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return await FilesControllerHelperInt.GenerateSharedLinkAsync(fileId, model.Share);
        }

        [Update("{fileId:int}/sharedlinkAsync")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> GenerateSharedLinkFromFormAsync(int fileId, [FromForm] GenerateSharedLinkModel model)
        {
            return await FilesControllerHelperInt.GenerateSharedLinkAsync(fileId, model.Share);
        }

        [Update("{fileId:int}/setacelink")]
        public async Task<bool> SetAceLinkAsync(int fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return await FilesControllerHelperInt.SetAceLinkAsync(fileId, model.Share);
        }

        [Update("{fileId}/setacelink")]
        public async Task<bool> SetAceLinkAsync(string fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return await FilesControllerHelperString.SetAceLinkAsync(fileId, model.Share);
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
        public async Task<FolderWrapper<string>> SaveThirdPartyFromBodyAsync([FromBody] ThirdPartyModel model)
        {
            return await SaveThirdPartyAsync(model);
        }

        [Create("thirdparty")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FolderWrapper<string>> SaveThirdPartyFromFormAsync([FromForm] ThirdPartyModel model)
        {
            return await SaveThirdPartyAsync(model);
        }

        private async Task<FolderWrapper<string>> SaveThirdPartyAsync(ThirdPartyModel model)
        {
            var thirdPartyParams = new ThirdPartyParams
            {
                AuthData = new AuthData(model.Url, model.Login, model.Password, model.Token),
                Corporate = model.IsCorporate,
                CustomerTitle = model.CustomerTitle,
                ProviderId = model.ProviderId,
                ProviderKey = model.ProviderKey,
            };

            var folder = await FileStorageService.SaveThirdPartyAsync(thirdPartyParams);

            return await FolderWrapperHelper.GetAsync(folder);
        }

        /// <summary>
        ///    Returns the list of all connected third party services
        /// </summary>
        /// <category>Third-Party Integration</category>
        /// <short>Get third party list</short>
        /// <returns>Connected providers</returns>

        [Read("thirdparty")]
        public async Task<IEnumerable<ThirdPartyParams>> GetThirdPartyAccountsAsync()
        {
            return await FileStorageService.GetThirdPartyAsync();
        }

        /// <summary>
        ///    Returns the list of third party services connected in the 'Common Documents' section
        /// </summary>
        /// <category>Third-Party Integration</category>
        /// <short>Get third party folder</short>
        /// <returns>Connected providers folder</returns>

        [Read("thirdparty/common")]
        public async Task<IEnumerable<FolderWrapper<string>>> GetCommonThirdPartyFoldersAsync()
        {
            var parent = FileStorageServiceInt.GetFolder(GlobalFolderHelper.FolderCommon);
            var thirdpartyFolders = await EntryManager.GetThirpartyFoldersAsync(parent);
            return thirdpartyFolders.Select(r => FolderWrapperHelper.Get(r)).ToList();
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
        public async Task<object> DeleteThirdPartyAsync(int providerId)
        {
            return await FileStorageService.DeleteThirdPartyAsync(providerId.ToString(CultureInfo.InvariantCulture));

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
        public async Task<bool> AddFavoritesFromBodyAsync([FromBody] BaseBatchModel model)
        {
            return await AddFavoritesAsync(model);
        }

        [Create("favorites")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<bool> AddFavoritesFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
        {
            return await AddFavoritesAsync(model);
        }

        private async Task<bool> AddFavoritesAsync(BaseBatchModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            await FileStorageServiceInt.AddToFavoritesAsync(folderIntIds, fileIntIds);
            await FileStorageService.AddToFavoritesAsync(folderStringIds, fileStringIds);
            return true;
        }

        [Read("favorites/{fileId}")]
        public async Task<bool> ToggleFileFavoriteAsync(string fileId, bool favorite)
        {
            return await FileStorageService.ToggleFileFavoriteAsync(fileId, favorite);
        }

        [Read("favorites/{fileId:int}")]
        public async Task<bool> ToggleFavoriteFromFormAsync(int fileId, bool favorite)
        {
            return await FileStorageServiceInt.ToggleFileFavoriteAsync(fileId, favorite);
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
        [Consumes("application/json")]
        public async Task<bool> DeleteFavoritesFromBodyAsync([FromBody] BaseBatchModel model)
        {
            return await DeleteFavoritesAsync(model);
        }

        [Delete("favorites")]
        public async Task<bool> DeleteFavoritesFromQueryAsync([FromQuery][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
        {
            return await DeleteFavoritesAsync(model);
        }

        private async Task<bool> DeleteFavoritesAsync(BaseBatchModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            await FileStorageServiceInt.DeleteFavoritesAsync(folderIntIds, fileIntIds);
            await FileStorageService.DeleteFavoritesAsync(folderStringIds, fileStringIds);
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
        public async Task<bool> AddTemplatesFromBodyAsync([FromBody] TemplatesModel model)
        {
            await FileStorageServiceInt.AddToTemplatesAsync(model.FileIds);
            return true;
        }

        [Create("templates")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<bool> AddTemplatesFromFormAsync([FromForm] TemplatesModel model)
        {
            await FileStorageServiceInt.AddToTemplatesAsync(model.FileIds);
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
        public async Task<bool> DeleteTemplatesAsync(IEnumerable<int> fileIds)
        {
            await FileStorageServiceInt.DeleteTemplatesAsync(fileIds);
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
        public async Task<IEnumerable<JsonElement>> CreateThumbnailsFromBodyAsync([FromBody] BaseBatchModel model)
        {
            return await FileStorageService.CreateThumbnailsAsync(model.FileIds.ToList());
        }

        [Create("thumbnails")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<JsonElement>> CreateThumbnailsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
        {
            return await FileStorageService.CreateThumbnailsAsync(model.FileIds.ToList());
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
}