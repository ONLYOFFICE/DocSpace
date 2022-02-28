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
using ASC.Web.Files.Core.Compress;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
        private IServiceProvider ServiceProvider { get; }

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
            ConsumerFactory consumerFactory,
            IServiceProvider serviceProvider)
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
            ServiceProvider = serviceProvider;
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
                folders.Add(await GlobalFolderHelper.FolderShareAsync);
            }

            if (!IsVisitor && !withoutAdditionalFolder)
            {
                if (FilesSettingsHelper.FavoritesSection)
                {
                    folders.Add(await GlobalFolderHelper.FolderFavoritesAsync);
                }

                if (FilesSettingsHelper.RecentSection)
                {
                    folders.Add(await GlobalFolderHelper.FolderRecentAsync);
                }

                if (!CoreBaseSettings.Personal && PrivacyRoomSettings.IsAvailable(TenantManager))
                {
                    folders.Add(await GlobalFolderHelper.FolderPrivacyAsync);
                }
            }

            if (!CoreBaseSettings.Personal)
            {
                folders.Add(await GlobalFolderHelper.FolderCommonAsync);
            }

            if (!IsVisitor
               && !withoutAdditionalFolder
               && FileUtility.ExtsWebTemplate.Count > 0
               && FilesSettingsHelper.TemplatesSection)
            {
                folders.Add(await GlobalFolderHelper.FolderTemplatesAsync);
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
        public Task<FolderContentWrapper<int>> GetPrivacyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            if (!IsAvailablePrivacyRoomSettings()) throw new System.Security.SecurityException();
            return InternalGetPrivacyFolderAsync(userIdOrGroupId, filterType, withsubfolders);
        }

        private async Task<FolderContentWrapper<int>> InternalGetPrivacyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return await FilesControllerHelperInt.GetFolderAsync(await GlobalFolderHelper.FolderPrivacyAsync, userIdOrGroupId, filterType, withsubfolders);
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
        public Task<FolderContentWrapper<int>> GetMyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolderAsync(GlobalFolderHelper.FolderMy, userIdOrGroupId, filterType, withsubfolders);
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
            return await FilesControllerHelperString.GetFolderAsync(await GlobalFolderHelper.GetFolderProjectsAsync<string>(), userIdOrGroupId, filterType, withsubfolders);
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
            return await FilesControllerHelperInt.GetFolderAsync(await GlobalFolderHelper.FolderCommonAsync, userIdOrGroupId, filterType, withsubfolders);
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
            return await FilesControllerHelperInt.GetFolderAsync(await GlobalFolderHelper.FolderShareAsync, userIdOrGroupId, filterType, withsubfolders);
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
            return await FilesControllerHelperInt.GetFolderAsync(await GlobalFolderHelper.FolderRecentAsync, userIdOrGroupId, filterType, withsubfolders);
        }

        [Create("file/{fileId}/recent", order: int.MaxValue)]
        public Task<FileWrapper<string>> AddToRecentAsync(string fileId)
        {
            return FilesControllerHelperString.AddToRecentAsync(fileId);
        }

        [Create("file/{fileId:int}/recent", order: int.MaxValue - 1)]
        public Task<FileWrapper<int>> AddToRecentAsync(int fileId)
        {
            return FilesControllerHelperInt.AddToRecentAsync(fileId);
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
            return await FilesControllerHelperInt.GetFolderAsync(await GlobalFolderHelper.FolderFavoritesAsync, userIdOrGroupId, filterType, withsubfolders);
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
            return await FilesControllerHelperInt.GetFolderAsync(await GlobalFolderHelper.FolderTemplatesAsync, userIdOrGroupId, filterType, withsubfolders);
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
        public Task<FolderContentWrapper<int>> GetTrashFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolderAsync(Convert.ToInt32(GlobalFolderHelper.FolderTrash), userIdOrGroupId, filterType, withsubfolders);
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
        public Task<FolderContentWrapper<int>> GetFolderAsync(int folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
        {
            return FilesControllerHelperInt.GetFolderAsync(folderId, userIdOrGroupId, filterType, withsubfolders);
        }

        [Read("{folderId}/subfolders")]
        public IAsyncEnumerable<FileEntryWrapper> GetFoldersAsync(string folderId)
        {
            return FilesControllerHelperString.GetFoldersAsync(folderId);
        }

        [Read("{folderId:int}/subfolders")]
        public IAsyncEnumerable<FileEntryWrapper> GetFoldersAsync(int folderId)
        {
            return FilesControllerHelperInt.GetFoldersAsync(folderId);
        }

        [Read("{folderId}/news")]
        public Task<List<FileEntryWrapper>> GetNewItemsAsync(string folderId)
        {
            return FilesControllerHelperString.GetNewItemsAsync(folderId);
        }

        [Read("{folderId:int}/news")]
        public Task<List<FileEntryWrapper>> GetNewItemsAsync(int folderId)
        {
            return FilesControllerHelperInt.GetNewItemsAsync(folderId);
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
        public Task<object> UploadFileToMyAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            uploadModel.CreateNewIfExist = false;
            return FilesControllerHelperInt.UploadFileAsync(GlobalFolderHelper.FolderMy, uploadModel);
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
            return await FilesControllerHelperInt.UploadFileAsync(await GlobalFolderHelper.FolderCommonAsync, uploadModel);
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
        public Task<object> UploadFileAsync(string folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            return FilesControllerHelperString.UploadFileAsync(folderId, uploadModel);
        }

        [Create("{folderId:int}/upload", order: int.MaxValue - 1)]
        public Task<object> UploadFileAsync(int folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadModel uploadModel)
        {
            return FilesControllerHelperInt.UploadFileAsync(folderId, uploadModel);
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
        public Task<FileWrapper<int>> InsertFileToMyFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return InsertFileAsync(GlobalFolderHelper.FolderMy, model);
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
            return await InsertFileAsync(await GlobalFolderHelper.FolderCommonAsync, model);
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
        public Task<FileWrapper<string>> InsertFileAsync(string folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return FilesControllerHelperString.InsertFileAsync(folderId, model.Stream, model.Title, model.CreateNewIfExist, model.KeepConvertStatus);
        }

        [Create("{folderId:int}/insert", order: int.MaxValue - 1)]
        public Task<FileWrapper<int>> InsertFileFromFormAsync(int folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileModel model)
        {
            return InsertFileAsync(folderId, model);
        }

        private Task<FileWrapper<int>> InsertFileAsync(int folderId, InsertFileModel model)
        {
            return FilesControllerHelperInt.InsertFileAsync(folderId, model.Stream, model.Title, model.CreateNewIfExist, model.KeepConvertStatus);
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
        public Task<FileWrapper<string>> UpdateFileStreamFromFormAsync(string fileId, [FromForm] FileStreamModel model)
        {
            return FilesControllerHelperString.UpdateFileStreamAsync(FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream(), fileId, model.FileExtension, model.Encrypted, model.Forcesave);
        }

        [Update("{fileId:int}/update")]
        public Task<FileWrapper<int>> UpdateFileStreamFromFormAsync(int fileId, [FromForm] FileStreamModel model)
        {
            return FilesControllerHelperInt.UpdateFileStreamAsync(FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream(), fileId, model.FileExtension, model.Encrypted, model.Forcesave);
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
        public Task<FileWrapper<string>> SaveEditingFromFormAsync(string fileId, [FromForm] SaveEditingModel model)
        {
            using var stream = FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream();
            return FilesControllerHelperString.SaveEditingAsync(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
        }

        [Update("file/{fileId:int}/saveediting")]
        public Task<FileWrapper<int>> SaveEditingFromFormAsync(int fileId, [FromForm] SaveEditingModel model)
        {
            using var stream = FilesControllerHelperInt.GetFileFromRequest(model).OpenReadStream();
            return FilesControllerHelperInt.SaveEditingAsync(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
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
        public Task<KeyValuePair<bool, string>> TrackEditFileAsync(string fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FilesControllerHelperString.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        [Read("file/{fileId:int}/trackeditfile")]
        public Task<KeyValuePair<bool, string>> TrackEditFileAsync(int fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FilesControllerHelperInt.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
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
        public Task<Configuration<string>> OpenEditAsync(string fileId, int version, string doc, bool view)
        {
            return FilesControllerHelperString.OpenEditAsync(fileId, version, doc, view);
        }

        [AllowAnonymous]
        [Read("file/{fileId:int}/openedit", Check = false)]
        public Task<Configuration<int>> OpenEditAsync(int fileId, int version, string doc, bool view)
        {
            return FilesControllerHelperInt.OpenEditAsync(fileId, version, doc, view);
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
        public Task<object> CreateUploadSessionFromBodyAsync(string folderId, [FromBody] SessionModel sessionModel)
        {
            return FilesControllerHelperString.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.LastModified, sessionModel.Encrypted);
        }

        [Create("{folderId}/upload/create_session")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<object> CreateUploadSessionFromFormAsync(string folderId, [FromForm] SessionModel sessionModel)
        {
            return FilesControllerHelperString.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.LastModified, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        public Task<object> CreateUploadSessionFromBodyAsync(int folderId, [FromBody] SessionModel sessionModel)
        {
            return FilesControllerHelperInt.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.LastModified, sessionModel.Encrypted);
        }

        [Create("{folderId:int}/upload/create_session")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<object> CreateUploadSessionFromFormAsync(int folderId, [FromForm] SessionModel sessionModel)
        {
            return FilesControllerHelperInt.CreateUploadSessionAsync(folderId, sessionModel.FileName, sessionModel.FileSize, sessionModel.RelativePath, sessionModel.LastModified, sessionModel.Encrypted);
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
        public Task<FileWrapper<int>> CreateTextFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFileAsync(GlobalFolderHelper.FolderMy, model);
        }

        [Create("@my/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> CreateTextFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFileAsync(GlobalFolderHelper.FolderMy, model);
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
            return await CreateTextFileAsync(await GlobalFolderHelper.FolderCommonAsync, model);
        }

        [Create("@common/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateTextFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateTextFileAsync(await GlobalFolderHelper.FolderCommonAsync, model);
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
        public Task<FileWrapper<string>> CreateTextFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFileAsync(folderId, model);
        }

        [Create("{folderId}/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<string>> CreateTextFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFileAsync(folderId, model);
        }

        private Task<FileWrapper<string>> CreateTextFileAsync(string folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperString.CreateTextFileAsync(folderId, model.Title, model.Content);
        }

        [Create("{folderId:int}/text")]
        public Task<FileWrapper<int>> CreateTextFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFileAsync(folderId, model);
        }

        [Create("{folderId:int}/text")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> CreateTextFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateTextFileAsync(folderId, model);
        }

        private Task<FileWrapper<int>> CreateTextFileAsync(int folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperInt.CreateTextFileAsync(folderId, model.Title, model.Content);
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
        public Task<FileWrapper<string>> CreateHtmlFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFileAsync(folderId, model);
        }

        [Create("{folderId}/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<string>> CreateHtmlFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFileAsync(folderId, model);
        }

        private Task<FileWrapper<string>> CreateHtmlFileAsync(string folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperString.CreateHtmlFileAsync(folderId, model.Title, model.Content);
        }

        [Create("{folderId:int}/html")]
        public Task<FileWrapper<int>> CreateHtmlFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFileAsync(folderId, model);
        }

        [Create("{folderId:int}/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> CreateHtmlFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFileAsync(folderId, model);
        }

        private Task<FileWrapper<int>> CreateHtmlFileAsync(int folderId, CreateTextOrHtmlFileModel model)
        {
            return FilesControllerHelperInt.CreateHtmlFileAsync(folderId, model.Title, model.Content);
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
        public Task<FileWrapper<int>> CreateHtmlFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFileAsync(GlobalFolderHelper.FolderMy, model);
        }

        [Create("@my/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> CreateHtmlFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return CreateHtmlFileAsync(GlobalFolderHelper.FolderMy, model);
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
            return await CreateHtmlFileAsync(await GlobalFolderHelper.FolderCommonAsync, model);
        }

        [Create("@common/html")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<FileWrapper<int>> CreateHtmlFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
        {
            return await CreateHtmlFileAsync(await GlobalFolderHelper.FolderCommonAsync, model);
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
        public Task<FolderWrapper<string>> CreateFolderFromBodyAsync(string folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.CreateFolderAsync(folderId, folderModel.Title);
        }

        [Create("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FolderWrapper<string>> CreateFolderFromFormAsync(string folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.CreateFolderAsync(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public Task<FolderWrapper<int>> CreateFolderFromBodyAsync(int folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.CreateFolderAsync(folderId, folderModel.Title);
        }

        [Create("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FolderWrapper<int>> CreateFolderFromFormAsync(int folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.CreateFolderAsync(folderId, folderModel.Title);
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
        public Task<FileWrapper<int>> CreateFileFromBodyAsync([FromBody] CreateFileModel<JsonElement> model)
        {
            return FilesControllerHelperInt.CreateFileAsync(GlobalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("@my/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> CreateFileFromFormAsync([FromForm] CreateFileModel<JsonElement> model)
        {
            return FilesControllerHelperInt.CreateFileAsync(GlobalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
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
        public Task<FileWrapper<string>> CreateFileFromBodyAsync(string folderId, [FromBody] CreateFileModel<JsonElement> model)
        {
            return FilesControllerHelperString.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId}/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<string>> CreateFileFromFormAsync(string folderId, [FromForm] CreateFileModel<JsonElement> model)
        {
            return FilesControllerHelperString.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId:int}/file")]
        public Task<FileWrapper<int>> CreateFileFromBodyAsync(int folderId, [FromBody] CreateFileModel<JsonElement> model)
        {
            return FilesControllerHelperInt.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
        }

        [Create("{folderId:int}/file")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> CreateFileFromFormAsync(int folderId, [FromForm] CreateFileModel<JsonElement> model)
        {
            return FilesControllerHelperInt.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
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
        public Task<FolderWrapper<string>> RenameFolderFromBodyAsync(string folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Update("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FolderWrapper<string>> RenameFolderFromFormAsync(string folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperString.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public Task<FolderWrapper<int>> RenameFolderFromBodyAsync(int folderId, [FromBody] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Update("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FolderWrapper<int>> RenameFolderFromFormAsync(int folderId, [FromForm] CreateFolderModel folderModel)
        {
            return FilesControllerHelperInt.RenameFolderAsync(folderId, folderModel.Title);
        }

        [Create("owner")]
        public IAsyncEnumerable<FileEntryWrapper> ChangeOwnerFromBodyAsync([FromBody] ChangeOwnerModel model)
        {
            return ChangeOwnerAsync(model);
        }

        [Create("owner")]
        [Consumes("application/x-www-form-urlencoded")]
        public IAsyncEnumerable<FileEntryWrapper> ChangeOwnerFromFormAsync([FromForm] ChangeOwnerModel model)
        {
            return ChangeOwnerAsync(model);
        }

        public async IAsyncEnumerable<FileEntryWrapper> ChangeOwnerAsync(ChangeOwnerModel model)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

            var result = AsyncEnumerable.Empty<FileEntry>();
            result.Concat(FileStorageServiceInt.ChangeOwnerAsync(folderIntIds, fileIntIds, model.UserId));
            result.Concat(FileStorageService.ChangeOwnerAsync(folderStringIds, fileStringIds, model.UserId));

            await foreach (var e in result)
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
        public Task<FolderWrapper<string>> GetFolderInfoAsync(string folderId)
        {
            return FilesControllerHelperString.GetFolderInfoAsync(folderId);
        }

        [Read("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public Task<FolderWrapper<int>> GetFolderInfoAsync(int folderId)
        {
            return FilesControllerHelperInt.GetFolderInfoAsync(folderId);
        }

        /// <summary>
        /// Returns parent folders
        /// </summary>
        /// <param name="folderId"></param>
        /// <category>Folders</category>
        /// <returns>Parent folders</returns>

        [Read("folder/{folderId}/path")]
        public IAsyncEnumerable<FileEntryWrapper> GetFolderPathAsync(string folderId)
        {
            return FilesControllerHelperString.GetFolderPathAsync(folderId);
        }


        [Read("folder/{folderId:int}/path")]
        public IAsyncEnumerable<FileEntryWrapper> GetFolderPathAsync(int folderId)
        {
            return FilesControllerHelperInt.GetFolderPathAsync(folderId);
        }

        /// <summary>
        /// Returns a detailed information about the file with the ID specified in the request
        /// </summary>
        /// <short>File information</short>
        /// <category>Files</category>
        /// <returns>File info</returns>

        [Read("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
        public Task<FileWrapper<string>> GetFileInfoAsync(string fileId, int version = -1)
        {
            return FilesControllerHelperString.GetFileInfoAsync(fileId, version);
        }

        [Read("file/{fileId:int}")]
        public Task<FileWrapper<int>> GetFileInfoAsync(int fileId, int version = -1)
        {
            return FilesControllerHelperInt.GetFileInfoAsync(fileId, version);
        }

        [Create("file/{fileId:int}/copyas", order: int.MaxValue - 1)]
        public object CopyFileAsFromBody(int fileId, [FromBody] CopyAsModel<JsonElement> model)
        {
            return CopyFile(fileId, model);
        }

        [Create("file/{fileId:int}/copyas", order: int.MaxValue - 1)]
        [Consumes("application/x-www-form-urlencoded")]
        public object CopyFileAsFromForm(int fileId, [FromForm] CopyAsModel<JsonElement> model)
        {
            return CopyFile(fileId, model);
        }

        [Create("file/{fileId}/copyas", order: int.MaxValue)]
        public object CopyFileAsFromBody(string fileId, [FromBody] CopyAsModel<JsonElement> model)
        {
            return CopyFile(fileId, model);
        }

        [Create("file/{fileId}/copyas", order: int.MaxValue)]
        [Consumes("application/x-www-form-urlencoded")]
        public object CopyFileAsFromForm(string fileId, [FromForm] CopyAsModel<JsonElement> model)
        {
            return CopyFile(fileId, model);
        }

        private object CopyFile<T>(T fileId, CopyAsModel<JsonElement> model)
        {
            var helper = ServiceProvider.GetService<FilesControllerHelper<T>>();
            if (model.DestFolderId.ValueKind == JsonValueKind.Number)
            {
                return helper.CopyFileAsAsync(fileId, model.DestFolderId.GetInt32(), model.DestTitle, model.Password);
            }
            else if (model.DestFolderId.ValueKind == JsonValueKind.String)
            {
                return helper.CopyFileAsAsync(fileId, model.DestFolderId.GetString(), model.DestTitle, model.Password);
            }

            return null;
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
        [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
        public Task<FileWrapper<string>> UpdateFileFromBodyAsync(string fileId, [FromBody] UpdateFileModel model)
        {
            return FilesControllerHelperString.UpdateFileAsync(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<string>> UpdateFileFromFormAsync(string fileId, [FromForm] UpdateFileModel model)
        {
            return FilesControllerHelperString.UpdateFileAsync(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public Task<FileWrapper<int>> UpdateFileFromBodyAsync(int fileId, [FromBody] UpdateFileModel model)
        {
            return FilesControllerHelperInt.UpdateFileAsync(fileId, model.Title, model.LastVersion);
        }

        [Update("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> UpdateFileFromFormAsync(int fileId, [FromForm] UpdateFileModel model)
        {
            return FilesControllerHelperInt.UpdateFileAsync(fileId, model.Title, model.LastVersion);
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
        public Task<IEnumerable<FileOperationWraper>> DeleteFile(string fileId, [FromBody] DeleteModel model)
        {
            return FilesControllerHelperString.DeleteFileAsync(fileId, model.DeleteAfter, model.Immediately);
        }

        [Delete("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
        public Task<IEnumerable<FileOperationWraper>> DeleteFile(int fileId, [FromBody] DeleteModel model)
        {
            return FilesControllerHelperInt.DeleteFileAsync(fileId, model.DeleteAfter, model.Immediately);
        }

        /// <summary>
        ///  Start conversion
        /// </summary>
        /// <short>Convert</short>
        /// <category>File operations</category>
        /// <param name="fileId"></param>
        /// <returns>Operation result</returns>

        [Update("file/{fileId}/checkconversion")]
        public IAsyncEnumerable<ConversationResult<string>> StartConversion(string fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel<string> model)
        {
            if (model == null)
            {
                model = new CheckConversionModel<string>();
            }
            model.FileId = fileId;
            return FilesControllerHelperString.StartConversionAsync(model);
        }

        [Update("file/{fileId:int}/checkconversion")]
        public IAsyncEnumerable<ConversationResult<int>> StartConversion(int fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel<int> model)
        {
            if (model == null)
            {
                model = new CheckConversionModel<int>();
            }
            model.FileId = fileId;
            return FilesControllerHelperInt.StartConversionAsync(model);
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
        public IAsyncEnumerable<ConversationResult<string>> CheckConversionAsync(string fileId, bool start)
        {
            return FilesControllerHelperString.CheckConversionAsync(new CheckConversionModel<string>()
            {
                FileId = fileId,
                StartConvert = start
            });
        }


        [Read("file/{fileId:int}/checkconversion")]
        public IAsyncEnumerable<ConversationResult<int>> CheckConversionAsync(int fileId, bool start)
        {
            return FilesControllerHelperInt.CheckConversionAsync(new CheckConversionModel<int>()
            {
                FileId = fileId,
                StartConvert = start
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
        [Delete("folder/{folderId}", order: int.MaxValue - 1, DisableFormat = true)]
        public Task<IEnumerable<FileOperationWraper>> DeleteFolder(string folderId, bool deleteAfter, bool immediately)
        {
            return FilesControllerHelperString.DeleteFolder(folderId, deleteAfter, immediately);
        }

        [Delete("folder/{folderId:int}")]
        public Task<IEnumerable<FileOperationWraper>> DeleteFolder(int folderId, bool deleteAfter, bool immediately)
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
        public IAsyncEnumerable<FileEntryWrapper> MoveOrCopyBatchCheckAsync([ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
        {
            return FilesControllerHelperString.MoveOrCopyBatchCheckAsync(batchModel);
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
        public Task<IEnumerable<FileOperationWraper>> MoveBatchItemsFromBody([FromBody] BatchModel batchModel)
        {
            return FilesControllerHelperString.MoveBatchItemsAsync(batchModel);
        }

        [Update("fileops/move")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileOperationWraper>> MoveBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
        {
            return FilesControllerHelperString.MoveBatchItemsAsync(batchModel);
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
        public Task<IEnumerable<FileOperationWraper>> CopyBatchItemsFromBody([FromBody] BatchModel batchModel)
        {
            return FilesControllerHelperString.CopyBatchItemsAsync(batchModel);
        }

        [Update("fileops/copy")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileOperationWraper>> CopyBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(BatchModelBinder))] BatchModel batchModel)
        {
            return FilesControllerHelperString.CopyBatchItemsAsync(batchModel);
        }

        /// <summary>
        ///   Marks all files and folders as read
        /// </summary>
        /// <short>Mark as read</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Update("fileops/markasread")]
        public Task<IEnumerable<FileOperationWraper>> MarkAsReadFromBody([FromBody] BaseBatchModel model)
        {
            return FilesControllerHelperString.MarkAsReadAsync(model);
        }

        [Update("fileops/markasread")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileOperationWraper>> MarkAsReadFromForm([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
        {
            return FilesControllerHelperString.MarkAsReadAsync(model);
        }

        /// <summary>
        ///  Finishes all the active file operations
        /// </summary>
        /// <short>Finish all</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>

        [Update("fileops/terminate")]
        public async IAsyncEnumerable<FileOperationWraper> TerminateTasks()
        {
            var tasks = FileStorageService.TerminateTasks();

            foreach (var e in tasks)
            {
                yield return await FileOperationWraperHelper.GetAsync(e);
            }
        }


        /// <summary>
        ///  Returns the list of all active file operations
        /// </summary>
        /// <short>Get file operations list</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Read("fileops")]
        public async Task<IEnumerable<FileOperationWraper>> GetOperationStatuses()
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.GetTasksStatuses())
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
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
        public Task<IEnumerable<FileOperationWraper>> BulkDownload([FromBody] DownloadModel model)
        {
            return FilesControllerHelperString.BulkDownloadAsync(model);
        }

        [Update("fileops/bulkdownload")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileOperationWraper>> BulkDownloadFromForm([FromForm] DownloadModel model)
        {
            return FilesControllerHelperString.BulkDownloadAsync(model);
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
        public async IAsyncEnumerable<FileOperationWraper> DeleteBatchItemsFromBody([FromBody] DeleteBatchModel batch)
        {
            var tasks = FileStorageService.DeleteItems("delete", batch.FileIds.ToList(), batch.FolderIds.ToList(), false, batch.DeleteAfter, batch.Immediately);

            foreach (var e in tasks)
            {
                yield return await FileOperationWraperHelper.GetAsync(e);
            }
        }

        [Update("fileops/delete")]
        [Consumes("application/x-www-form-urlencoded")]
        public async IAsyncEnumerable<FileOperationWraper> DeleteBatchItemsFromForm([FromForm][ModelBinder(BinderType = typeof(DeleteBatchModelBinder))] DeleteBatchModel batch)
        {
            var tasks = FileStorageService.DeleteItems("delete", batch.FileIds.ToList(), batch.FolderIds.ToList(), false, batch.DeleteAfter, batch.Immediately);

            foreach (var e in tasks)
            {
                yield return await FileOperationWraperHelper.GetAsync(e);
            }
        }

        /// <summary>
        ///   Deletes all files and folders from the recycle bin
        /// </summary>
        /// <short>Clear recycle bin</short>
        /// <category>File operations</category>
        /// <returns>Operation result</returns>
        [Update("fileops/emptytrash")]
        public Task<IEnumerable<FileOperationWraper>> EmptyTrashAsync()
        {
            return FilesControllerHelperInt.EmptyTrashAsync();
        }

        /// <summary>
        /// Returns the detailed information about all the available file versions with the ID specified in the request
        /// </summary>
        /// <short>File versions</short>
        /// <category>Files</category>
        /// <param name="fileId">File ID</param>
        /// <returns>File information</returns>
        [Read("file/{fileId}/history")]
        public Task<IEnumerable<FileWrapper<string>>> GetFileVersionInfoAsync(string fileId)
        {
            return FilesControllerHelperString.GetFileVersionInfoAsync(fileId);
        }

        [Read("file/{fileId:int}/history")]
        public Task<IEnumerable<FileWrapper<int>>> GetFileVersionInfoAsync(int fileId)
        {
            return FilesControllerHelperInt.GetFileVersionInfoAsync(fileId);
        }

        [Read("file/{fileId}/presigned")]
        public Task<DocumentService.FileLink> GetPresignedUriAsync(string fileId)
        {
            return FilesControllerHelperString.GetPresignedUriAsync(fileId);
        }

        [Read("file/{fileId:int}/presigned")]
        public Task<DocumentService.FileLink> GetPresignedUriAsync(int fileId)
        {
            return FilesControllerHelperInt.GetPresignedUriAsync(fileId);
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
        public Task<IEnumerable<FileWrapper<string>>> ChangeHistoryFromBodyAsync(string fileId, [FromBody] ChangeHistoryModel model)
        {
            return FilesControllerHelperString.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/history")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileWrapper<string>>> ChangeHistoryFromFormAsync(string fileId, [FromForm] ChangeHistoryModel model)
        {
            return FilesControllerHelperString.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        public Task<IEnumerable<FileWrapper<int>>> ChangeHistoryFromBodyAsync(int fileId, [FromBody] ChangeHistoryModel model)
        {
            return FilesControllerHelperInt.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId:int}/history")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileWrapper<int>>> ChangeHistoryFromFormAsync(int fileId, [FromForm] ChangeHistoryModel model)
        {
            return FilesControllerHelperInt.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
        }

        [Update("file/{fileId}/lock")]
        public Task<FileWrapper<string>> LockFileFromBodyAsync(string fileId, [FromBody] LockFileModel model)
        {
            return FilesControllerHelperString.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId}/lock")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<string>> LockFileFromFormAsync(string fileId, [FromForm] LockFileModel model)
        {
            return FilesControllerHelperString.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        public Task<FileWrapper<int>> LockFileFromBodyAsync(int fileId, [FromBody] LockFileModel model)
        {
            return FilesControllerHelperInt.LockFileAsync(fileId, model.LockFile);
        }

        [Update("file/{fileId:int}/lock")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FileWrapper<int>> LockFileFromFormAsync(int fileId, [FromForm] LockFileModel model)
        {
            return FilesControllerHelperInt.LockFileAsync(fileId, model.LockFile);
        }

        [AllowAnonymous]
        [Read("file/{fileId}/edit/history")]
        public Task<List<EditHistoryWrapper>> GetEditHistoryAsync(string fileId, string doc = null)
        {
            return FilesControllerHelperString.GetEditHistoryAsync(fileId, doc);
        }

        [AllowAnonymous]
        [Read("file/{fileId:int}/edit/history")]
        public Task<List<EditHistoryWrapper>> GetEditHistoryAsync(int fileId, string doc = null)
        {
            return FilesControllerHelperInt.GetEditHistoryAsync(fileId, doc);
        }

        [AllowAnonymous]
        [Read("file/{fileId}/edit/diff")]
        public Task<EditHistoryData> GetEditDiffUrlAsync(string fileId, int version = 0, string doc = null)
        {
            return FilesControllerHelperString.GetEditDiffUrlAsync(fileId, version, doc);
        }

        [AllowAnonymous]
        [Read("file/{fileId:int}/edit/diff")]
        public Task<EditHistoryData> GetEditDiffUrlAsync(int fileId, int version = 0, string doc = null)
        {
            return FilesControllerHelperInt.GetEditDiffUrlAsync(fileId, version, doc);
        }

        [AllowAnonymous]
        [Read("file/{fileId}/restoreversion")]
        public Task<List<EditHistoryWrapper>> RestoreVersionAsync(string fileId, int version = 0, string url = null, string doc = null)
        {
            return FilesControllerHelperString.RestoreVersionAsync(fileId, version, url, doc);
        }

        [AllowAnonymous]
        [Read("file/{fileId:int}/restoreversion")]
        public Task<List<EditHistoryWrapper>> RestoreVersionAsync(int fileId, int version = 0, string url = null, string doc = null)
        {
            return FilesControllerHelperInt.RestoreVersionAsync(fileId, version, url, doc);
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
        public Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(string fileId)
        {
            return FilesControllerHelperString.GetFileSecurityInfoAsync(fileId);
        }

        [Read("file/{fileId:int}/share")]
        public Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(int fileId)
        {
            return FilesControllerHelperInt.GetFileSecurityInfoAsync(fileId);
        }

        /// <summary>
        /// Returns the detailed information about shared folder with the ID specified in the request
        /// </summary>
        /// <short>Folder sharing</short>
        /// <param name="folderId">Folder ID</param>
        /// <category>Sharing</category>
        /// <returns>Shared folder information</returns>

        [Read("folder/{folderId}/share")]
        public Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(string folderId)
        {
            return FilesControllerHelperString.GetFolderSecurityInfoAsync(folderId);
        }

        [Read("folder/{folderId:int}/share")]
        public Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(int folderId)
        {
            return FilesControllerHelperInt.GetFolderSecurityInfoAsync(folderId);
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
        public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromBodyAsync(string fileId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromFormAsync(string fileId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromBodyAsync(int fileId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("file/{fileId:int}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromFormAsync(int fileId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("share")]
        public Task<IEnumerable<FileShareWrapper>> SetSecurityInfoFromBodyAsync([FromBody] SecurityInfoModel model)
        {
            return SetSecurityInfoAsync(model);
        }

        [Update("share")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileShareWrapper>> SetSecurityInfoFromFormAsync([FromForm] SecurityInfoModel model)
        {
            return SetSecurityInfoAsync(model);
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
        public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromBodyAsync(string folderId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromFormAsync(string folderId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperString.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId:int}/share")]
        public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromBodyAsync(int folderId, [FromBody] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
        }

        [Update("folder/{folderId:int}/share")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromFormAsync(int folderId, [FromForm] SecurityInfoModel model)
        {
            return FilesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
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
        public Task<bool> SetAceLinkAsync(int fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperInt.SetAceLinkAsync(fileId, model.Share);
        }

        [Update("{fileId}/setacelink")]
        public Task<bool> SetAceLinkAsync(string fileId, [FromBody] GenerateSharedLinkModel model)
        {
            return FilesControllerHelperString.SetAceLinkAsync(fileId, model.Share);
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
        public Task<FolderWrapper<string>> SaveThirdPartyFromBodyAsync([FromBody] ThirdPartyModel model)
        {
            return SaveThirdPartyAsync(model);
        }

        [Create("thirdparty")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<FolderWrapper<string>> SaveThirdPartyFromFormAsync([FromForm] ThirdPartyModel model)
        {
            return SaveThirdPartyAsync(model);
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
            var parent = await FileStorageServiceInt.GetFolderAsync(await GlobalFolderHelper.FolderCommonAsync);
            var thirdpartyFolders = await EntryManager.GetThirpartyFoldersAsync(parent);
            var result = new List<FolderWrapper<string>>();

            foreach (var r in thirdpartyFolders)
            {
                result.Add(await FolderWrapperHelper.GetAsync(r));
            }
            return result;
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
        public Task<object> DeleteThirdPartyAsync(int providerId)
        {
            return FileStorageService.DeleteThirdPartyAsync(providerId.ToString(CultureInfo.InvariantCulture));

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
        public Task<bool> AddFavoritesFromBodyAsync([FromBody] BaseBatchModel model)
        {
            return AddFavoritesAsync(model);
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
        public Task<bool> ToggleFileFavoriteAsync(string fileId, bool favorite)
        {
            return FileStorageService.ToggleFileFavoriteAsync(fileId, favorite);
        }

        [Read("favorites/{fileId:int}")]
        public Task<bool> ToggleFavoriteFromFormAsync(int fileId, bool favorite)
        {
            return FileStorageServiceInt.ToggleFileFavoriteAsync(fileId, favorite);
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
        public Task<bool> DeleteFavoritesFromBodyAsync([FromBody] BaseBatchModel model)
        {
            return DeleteFavoritesAsync(model);
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
        public ICompress ChangeDownloadZipFromBody([FromBody] DisplayModel model)
        {
            return FileStorageService.ChangeDownloadTarGz(model.Set);
        }

        [Update(@"settings/downloadtargz")]
        public ICompress ChangeDownloadZipFromForm([FromForm] DisplayModel model)
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
        public Task<IEnumerable<string>> CheckDocServiceUrlFromBodyAsync([FromBody] CheckDocServiceUrlModel model)
        {
            return CheckDocServiceUrlAsync(model);
        }

        [Update("docservice")]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<IEnumerable<string>> CheckDocServiceUrlFromFormAsync([FromForm] CheckDocServiceUrlModel model)
        {
            return CheckDocServiceUrlAsync(model);
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
        public Task<IEnumerable<JsonElement>> CreateThumbnailsFromBodyAsync([FromBody] BaseBatchModel model)
        {
            return FileStorageService.CreateThumbnailsAsync(model.FileIds.ToList());
        }

        [Create("thumbnails")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IEnumerable<JsonElement>> CreateThumbnailsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
        {
            return await FileStorageService.CreateThumbnailsAsync(model.FileIds.ToList());
        }

        [Create("masterform/{fileId}/checkfillformdraft")]
        public async Task<object> CheckFillFormDraftFromBodyAsync(string fileId, [FromBody] CheckFillFormDraftModel model)
        {
            return await FilesControllerHelperString.CheckFillFormDraftAsync(fileId, model.Version, model.Doc, !model.RequestEmbedded, model.RequestView);
        }

        [Create("masterform/{fileId}/checkfillformdraft")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> CheckFillFormDraftFromFormAsync(string fileId, [FromForm] CheckFillFormDraftModel model)
        {
            return await FilesControllerHelperString.CheckFillFormDraftAsync(fileId, model.Version, model.Doc, !model.RequestEmbedded, model.RequestView);
        }

        [Create("masterform/{fileId:int}/checkfillformdraft")]
        public async Task<object> CheckFillFormDraftFromBodyAsync(int fileId, [FromBody] CheckFillFormDraftModel model)
        {
            return await FilesControllerHelperInt.CheckFillFormDraftAsync(fileId, model.Version, model.Doc, !model.RequestEmbedded, model.RequestView);
        }

        [Create("masterform/{fileId:int}/checkfillformdraft")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> CheckFillFormDraftFromFormAsync(int fileId, [FromForm] CheckFillFormDraftModel model)
        {
            return await FilesControllerHelperInt.CheckFillFormDraftAsync(fileId, model.Version, model.Doc, !model.RequestEmbedded, model.RequestView);
        }

        public Task<IEnumerable<string>> CheckDocServiceUrlAsync(CheckDocServiceUrlModel model)
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

            return InternalCheckDocServiceUrlAsync();
        }

        private async Task<IEnumerable<string>> InternalCheckDocServiceUrlAsync()
        {
            await DocumentServiceConnector.CheckDocServiceUrlAsync();

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
        public Task<object> GetDocServiceUrlAsync(bool version)
        {
            var url = CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.DocServiceApiUrl);
            if (!version)
            {
                return Task.FromResult<object>(url);
            }

            return InternalGetDocServiceUrlAsync(url);
        }

        private async Task<object> InternalGetDocServiceUrlAsync(string url)
        {
            var dsVersion = await DocumentServiceConnector.GetVersionAsync();

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
            if (model.Code.Length == 0)
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
            public object File { get; set; }

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