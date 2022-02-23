using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.FederatedLogin.Helpers;
using ASC.Files.Core;
using ASC.Files.Core.Model;
using ASC.Files.Model;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Entries;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using static ASC.Api.Documents.FilesController;

using FileShare = ASC.Files.Core.Security.FileShare;
using MimeMapping = ASC.Common.Web.MimeMapping;
using SortedByType = ASC.Files.Core.SortedByType;

namespace ASC.Files.Helpers
{
    [Scope]
    public class FilesControllerHelper<T>
    {
        private readonly ApiContext ApiContext;
        private readonly FileStorageService<T> FileStorageService;

        private FileWrapperHelper FileWrapperHelper { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUploader FileUploader { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private TenantManager TenantManager { get; }
        private SecurityContext SecurityContext { get; }
        private FolderWrapperHelper FolderWrapperHelper { get; }
        private FileOperationWraperHelper FileOperationWraperHelper { get; }
        private FileShareWrapperHelper FileShareWrapperHelper { get; }
        private FileShareParamsHelper FileShareParamsHelper { get; }
        private EntryManager EntryManager { get; }
        private FolderContentWrapperHelper FolderContentWrapperHelper { get; }
        private ChunkedUploadSessionHelper ChunkedUploadSessionHelper { get; }
        private DocumentServiceTrackerHelper DocumentServiceTracker { get; }
        private SettingsManager SettingsManager { get; }
        private EncryptionKeyPairHelper EncryptionKeyPairHelper { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private FileConverter FileConverter { get; }
        private ApiDateTimeHelper ApiDateTimeHelper { get; }
        private UserManager UserManager { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public SocketManager SocketManager { get; }
        public IServiceProvider ServiceProvider { get; }
        private ILog Logger { get; set; }
        private IHttpClientFactory ClientFactory { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileStorageService"></param>
        public FilesControllerHelper(
            ApiContext context,
            FileStorageService<T> fileStorageService,
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
            FolderContentWrapperHelper folderContentWrapperHelper,
            ChunkedUploadSessionHelper chunkedUploadSessionHelper,
            DocumentServiceTrackerHelper documentServiceTracker,
            IOptionsMonitor<ILog> optionMonitor,
            SettingsManager settingsManager,
            EncryptionKeyPairHelper encryptionKeyPairHelper,
            IHttpContextAccessor httpContextAccessor,
            FileConverter fileConverter,
            ApiDateTimeHelper apiDateTimeHelper,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            IServiceProvider serviceProvider,
            SocketManager socketManager,
            IHttpClientFactory clientFactory)
        {
            ApiContext = context;
            FileStorageService = fileStorageService;
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
            FolderContentWrapperHelper = folderContentWrapperHelper;
            ChunkedUploadSessionHelper = chunkedUploadSessionHelper;
            DocumentServiceTracker = documentServiceTracker;
            SettingsManager = settingsManager;
            EncryptionKeyPairHelper = encryptionKeyPairHelper;
            ApiDateTimeHelper = apiDateTimeHelper;
            UserManager = userManager;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            ServiceProvider = serviceProvider;
            SocketManager = socketManager;
            HttpContextAccessor = httpContextAccessor;
            FileConverter = fileConverter;
            Logger = optionMonitor.Get("ASC.Files");
            ClientFactory = clientFactory;
        }

        public async Task<FolderContentWrapper<T>> GetFolderAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withSubFolders)
        {
            var folderContentWrapper = await ToFolderContentWrapperAsync(folderId, userIdOrGroupId, filterType, withSubFolders);
            return folderContentWrapper.NotFoundIfNull();
        }

        public async Task<object> UploadFileAsync(T folderId, UploadModel uploadModel)
        {
            if (uploadModel.StoreOriginalFileFlag.HasValue)
            {
                FilesSettingsHelper.StoreOriginalFiles = uploadModel.StoreOriginalFileFlag.Value;
            }

            IEnumerable<IFormFile> files = HttpContextAccessor.HttpContext.Request.Form.Files;
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

                return new List<FileWrapper<T>>
                {
                    await InsertFileAsync(folderId, uploadModel.File.OpenReadStream(), fileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)
                };
            }

            throw new InvalidOperationException("No input files");
        }

        public async Task<FileWrapper<T>> InsertFileAsync(T folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            try
            {
                var resultFile = await FileUploader.ExecAsync(folderId, title, file.Length, file, createNewIfExist ?? !FilesSettingsHelper.UpdateIfExist, !keepConvertStatus);

                await SocketManager.CreateFileAsync(resultFile);

                return await FileWrapperHelper.GetAsync(resultFile);
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

        public async Task<FileWrapper<T>> UpdateFileStreamAsync(Stream file, T fileId, string fileExtension, bool encrypted = false, bool forcesave = false)
        {
            try
            {
                var resultFile = await FileStorageService.UpdateFileStreamAsync(fileId, file, fileExtension, encrypted, forcesave);
                return await FileWrapperHelper.GetAsync(resultFile);
            }
            catch (FileNotFoundException e)
            {
                throw new ItemNotFoundException("File not found", e);
            }
        }

        public async Task<FileWrapper<T>> SaveEditingAsync(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
        {
            return await FileWrapperHelper.GetAsync(await FileStorageService.SaveEditingAsync(fileId, fileExtension, downloadUri, stream, doc, forcesave));
        }

        public Task<string> StartEditAsync(T fileId, bool editingAlone, string doc)
        {
            return FileStorageService.StartEditAsync(fileId, editingAlone, doc);
        }

        public Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FileStorageService.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        public async Task<Configuration<T>> OpenEditAsync(T fileId, int version, string doc, bool view)
        {
            var docParams = await DocumentServiceHelper.GetParamsAsync(fileId, version, doc, true, !view, true);
            var configuration = docParams.Configuration;

            configuration.EditorType = EditorType.External;
            if (configuration.EditorConfig.ModeWrite)
            {
                configuration.EditorConfig.CallbackUrl = DocumentServiceTracker.GetCallbackUrl(configuration.Document.Info.GetFile().ID.ToString());
            }

            if (configuration.Document.Info.GetFile().RootFolderType == FolderType.Privacy && PrivacyRoomSettings.GetEnabled(SettingsManager))
            {
                var keyPair = EncryptionKeyPairHelper.GetKeyPair();
                if (keyPair != null)
                {
                    configuration.EditorConfig.EncryptionKeys = new EncryptionKeysConfig
                    {
                        PrivateKeyEnc = keyPair.PrivateKeyEnc,
                        PublicKey = keyPair.PublicKey,
                    };
                }
            }

            if (!configuration.Document.Info.GetFile().Encrypted && !configuration.Document.Info.GetFile().ProviderEntry) EntryManager.MarkAsRecent(configuration.Document.Info.GetFile());

            configuration.Token = DocumentServiceHelper.GetSignature(configuration);
            return configuration;
        }

        public async Task<object> CreateUploadSessionAsync(T folderId, string fileName, long fileSize, string relativePath, ApiDateTime lastModified, bool encrypted)
        {
            var file = await FileUploader.VerifyChunkedUploadAsync(folderId, fileName, fileSize, FilesSettingsHelper.UpdateIfExist, lastModified, relativePath);

            if (FilesLinkUtility.IsLocalFileUploader)
            {
                var session = await FileUploader.InitiateUploadAsync(file.FolderID, file.ID ?? default, file.Title, file.ContentLength, encrypted);

                var responseObject = await ChunkedUploadSessionHelper.ToResponseObjectAsync(session, true);
                return new
                {
                    success = true,
                    data = responseObject
                };
            }

            var createSessionUrl = FilesLinkUtility.GetInitiateUploadSessionUrl(TenantManager.GetCurrentTenant().TenantId, file.FolderID, file.ID, file.Title, file.ContentLength, encrypted, SecurityContext);

            var httpClient = ClientFactory.CreateClient();

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(createSessionUrl);
            request.Method = HttpMethod.Post;

            // hack for uploader.onlyoffice.com in api requests
            var rewriterHeader = ApiContext.HttpContextAccessor.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
            if (!string.IsNullOrEmpty(rewriterHeader))
            {
                request.Headers.Add(HttpRequestExtensions.UrlRewriterHeader, rewriterHeader.ToString());
            }

            using var response = await httpClient.SendAsync(request);
            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            return JObject.Parse(await streamReader.ReadToEndAsync()); //result is json string
        }

        public Task<FileWrapper<T>> CreateTextFileAsync(T folderId, string title, string content)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            //Try detect content
            var extension = ".txt";
            if (!string.IsNullOrEmpty(content))
            {
                if (Regex.IsMatch(content, @"<([^\s>]*)(\s[^<]*)>"))
                {
                    extension = ".html";
                }
            }
            return CreateFileAsync(folderId, title, content, extension);
        }

        private async Task<FileWrapper<T>> CreateFileAsync(T folderId, string title, string content, string extension)
        {
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var file = await FileUploader.ExecAsync(folderId,
                              title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension),
                              memStream.Length, memStream);
            return await FileWrapperHelper.GetAsync(file);
        }

        public Task<FileWrapper<T>> CreateHtmlFileAsync(T folderId, string title, string content)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            return CreateFileAsync(folderId, title, content, ".html");
        }

        public async Task<FolderWrapper<T>> CreateFolderAsync(T folderId, string title)
        {
            var folder = await FileStorageService.CreateNewFolderAsync(folderId, title);

            return await FolderWrapperHelper.GetAsync(folder);
        }

        public async Task<FileWrapper<T>> CreateFileAsync(T folderId, string title, JsonElement templateId, bool enableExternalExt = false)
        {
            File<T> file;

            if (templateId.ValueKind == JsonValueKind.Number)
            {
                file = await FileStorageService.CreateNewFileAsync(new FileModel<T, int> { ParentId = folderId, Title = title, TemplateId = templateId.GetInt32() }, enableExternalExt);
            }
            else if (templateId.ValueKind == JsonValueKind.String)
            {
                file = await FileStorageService.CreateNewFileAsync(new FileModel<T, string> { ParentId = folderId, Title = title, TemplateId = templateId.GetString() }, enableExternalExt);
            }
            else
            {
                file = await FileStorageService.CreateNewFileAsync(new FileModel<T, int> { ParentId = folderId, Title = title, TemplateId = 0 }, enableExternalExt);
            }

            return await FileWrapperHelper.GetAsync(file);
        }

        public async Task<FolderWrapper<T>> RenameFolderAsync(T folderId, string title)
        {
            var folder = await FileStorageService.FolderRenameAsync(folderId, title);
            return await FolderWrapperHelper.GetAsync(folder);
        }

        public async Task<FolderWrapper<T>> GetFolderInfoAsync(T folderId)
        {
            var folder = await FileStorageService.GetFolderAsync(folderId).NotFoundIfNull("Folder not found");

            return await FolderWrapperHelper.GetAsync(folder);
        }

        public async IAsyncEnumerable<FileEntryWrapper> GetFolderPathAsync(T folderId)
        {
            var breadCrumbs = await EntryManager.GetBreadCrumbsAsync(folderId);

            foreach (var e in breadCrumbs)
            {
                yield return await GetFileEntryWrapperAsync(e);
            }
        }

        public async Task<FileWrapper<T>> GetFileInfoAsync(T fileId, int version = -1)
        {
            var file = await FileStorageService.GetFileAsync(fileId, version);
            file = file.NotFoundIfNull("File not found");
            return await FileWrapperHelper.GetAsync(file);
        }

        public async Task<FileWrapper<TTemplate>> CopyFileAsAsync<TTemplate>(T fileId, TTemplate destFolderId, string destTitle, string password = null)
        {
            var service = ServiceProvider.GetService<FileStorageService<TTemplate>>();
            var controller = ServiceProvider.GetService<FilesControllerHelper<TTemplate>>();
            var file = await FileStorageService.GetFileAsync(fileId, -1);
            var ext = FileUtility.GetFileExtension(file.Title);
            var destExt = FileUtility.GetFileExtension(destTitle);

            if (ext == destExt)
            {
                var newFile = await service.CreateNewFileAsync(new FileModel<TTemplate, T> { ParentId = destFolderId, Title = destTitle, TemplateId = fileId }, false);
                return await FileWrapperHelper.GetAsync(newFile);
            }

            using (var fileStream = await FileConverter.ExecAsync(file, destExt, password))
            {
                return await controller.InsertFileAsync(destFolderId, fileStream, destTitle, true);
            }
        }

        public async Task<FileWrapper<T>> AddToRecentAsync(T fileId, int version = -1)
        {
            var file = await FileStorageService.GetFileAsync(fileId, version).NotFoundIfNull("File not found");
            EntryManager.MarkAsRecent(file);
            return await FileWrapperHelper.GetAsync(file);
        }

        public async Task<List<FileEntryWrapper>> GetNewItemsAsync(T folderId)
        {
            var newItems = await FileStorageService.GetNewItemsAsync(folderId);
            var result = new List<FileEntryWrapper>();

            foreach (var e in newItems)
            {
                result.Add(await GetFileEntryWrapperAsync(e));
            }

            return result;
        }

        public async Task<FileWrapper<T>> UpdateFileAsync(T fileId, string title, int lastVersion)
        {
            if (!string.IsNullOrEmpty(title))
                await FileStorageService.FileRenameAsync(fileId, title);

            if (lastVersion > 0)
                await FileStorageService.UpdateToVersionAsync(fileId, lastVersion);

            return await GetFileInfoAsync(fileId);
        }

        public async Task<IEnumerable<FileOperationWraper>> DeleteFileAsync(T fileId, bool deleteAfter, bool immediately)
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.DeleteFile("delete", fileId, false, deleteAfter, immediately))
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }
        public IAsyncEnumerable<ConversationResult<T>> StartConversionAsync(CheckConversionModel<T> model)
        {
            model.StartConvert = true;
            return CheckConversionAsync(model);
        }

        public async IAsyncEnumerable<ConversationResult<T>> CheckConversionAsync(CheckConversionModel<T> model)
        {
            var checkConversaion = FileStorageService.CheckConversionAsync(new List<CheckConversionModel<T>>() { model }, model.Sync);

            await foreach (var r in checkConversaion)
            {
                var o = new ConversationResult<T>
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
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            AllowTrailingCommas = true,
                            PropertyNameCaseInsensitive = true
                        };
                        var jResult = JsonSerializer.Deserialize<FileJsonSerializerData<T>>(r.Result, options);
                        o.File = await GetFileInfoAsync(jResult.Id, jResult.Version);
                    }
                    catch (Exception e)
                    {
                        o.File = r.Result;
                        Logger.Error(e);
                    }
                }
                yield return o;
            }
        }

        public Task<string> CheckFillFormDraftAsync(T fileId, int version, string doc, bool editPossible, bool view)
        {
            return FileStorageService.CheckFillFormDraftAsync(fileId, version, doc, editPossible, view);
        }

        public async Task<IEnumerable<FileOperationWraper>> DeleteFolder(T folderId, bool deleteAfter, bool immediately)
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.DeleteFolder("delete", folderId, false, deleteAfter, immediately))
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async IAsyncEnumerable<FileEntryWrapper> MoveOrCopyBatchCheckAsync(BatchModel batchModel)
        {
            List<object> checkedFiles;
            List<object> checkedFolders;

            if (batchModel.DestFolderId.ValueKind == JsonValueKind.Number)
            {
                (checkedFiles, checkedFolders) = await FileStorageService.MoveOrCopyFilesCheckAsync(batchModel.FileIds.ToList(), batchModel.FolderIds.ToList(), batchModel.DestFolderId.GetInt32());
            }
            else
            {
                (checkedFiles, checkedFolders) = await FileStorageService.MoveOrCopyFilesCheckAsync(batchModel.FileIds.ToList(), batchModel.FolderIds.ToList(), batchModel.DestFolderId.GetString());
            }

            var entries = await FileStorageService.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");

            entries.AddRange(await FileStorageService.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

            foreach (var e in entries)
            {
                yield return await GetFileEntryWrapperAsync(e);
            }
        }

        public async Task<IEnumerable<FileOperationWraper>> MoveBatchItemsAsync(BatchModel batchModel)
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.MoveOrCopyItems(batchModel.FolderIds.ToList(), batchModel.FileIds.ToList(), batchModel.DestFolderId, batchModel.ConflictResolveType, false, batchModel.DeleteAfter))
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileOperationWraper>> CopyBatchItemsAsync(BatchModel batchModel)
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.MoveOrCopyItems(batchModel.FolderIds.ToList(), batchModel.FileIds.ToList(), batchModel.DestFolderId, batchModel.ConflictResolveType, true, batchModel.DeleteAfter))
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileOperationWraper>> MarkAsReadAsync(BaseBatchModel model)
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.MarkAsRead(model.FolderIds.ToList(), model.FileIds.ToList()))
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileOperationWraper>> TerminateTasksAsync()
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.TerminateTasks())
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileOperationWraper>> GetOperationStatusesAsync()
        {
            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.GetTasksStatuses())
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileOperationWraper>> BulkDownloadAsync(DownloadModel model)
        {
            var folders = new Dictionary<JsonElement, string>();
            var files = new Dictionary<JsonElement, string>();

            foreach (var fileId in model.FileConvertIds.Where(fileId => !files.ContainsKey(fileId.Key)))
            {
                files.Add(fileId.Key, fileId.Value);
            }

            foreach (var fileId in model.FileIds.Where(fileId => !files.ContainsKey(fileId)))
            {
                files.Add(fileId, string.Empty);
            }

            foreach (var folderId in model.FolderIds.Where(folderId => !folders.ContainsKey(folderId)))
            {
                folders.Add(folderId, string.Empty);
            }

            var result = new List<FileOperationWraper>();

            foreach (var e in FileStorageService.BulkDownload(folders, files))
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileOperationWraper>> EmptyTrashAsync()
        {
            var emptyTrash = await FileStorageService.EmptyTrashAsync();
            var result = new List<FileOperationWraper>();

            foreach (var e in emptyTrash)
            {
                result.Add(await FileOperationWraperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileWrapper<T>>> GetFileVersionInfoAsync(T fileId)
        {
            var files = await FileStorageService.GetFileHistoryAsync(fileId);
            var result = new List<FileWrapper<T>>();

            foreach (var e in files)
            {
                result.Add(await FileWrapperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<IEnumerable<FileWrapper<T>>> ChangeHistoryAsync(T fileId, int version, bool continueVersion)
        {
            var pair = await FileStorageService.CompleteVersionAsync(fileId, version, continueVersion);
            var history = pair.Value;

            var result = new List<FileWrapper<T>>();

            foreach (var e in history)
            {
                result.Add(await FileWrapperHelper.GetAsync(e));
            }

            return result;
        }

        public async Task<FileWrapper<T>> LockFileAsync(T fileId, bool lockFile)
        {
            var result = await FileStorageService.LockFileAsync(fileId, lockFile);
            return await FileWrapperHelper.GetAsync(result);
        }

        public Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
        {
            return FileStorageService.GetPresignedUriAsync(fileId);
        }

        public async Task<List<EditHistoryWrapper>> GetEditHistoryAsync(T fileId, string doc = null)
        {
            var result = await FileStorageService.GetEditHistoryAsync(fileId, doc);
            return result.Select(r => new EditHistoryWrapper(r, ApiDateTimeHelper, UserManager, DisplayUserSettingsHelper)).ToList();
        }

        public Task<EditHistoryData> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
        {
            return FileStorageService.GetEditDiffUrlAsync(fileId, version, doc);
        }

        public async Task<List<EditHistoryWrapper>> RestoreVersionAsync(T fileId, int version = 0, string url = null, string doc = null)
        {
            var result = await FileStorageService.RestoreVersionAsync(fileId, version, url, doc);
            return result.Select(r => new EditHistoryWrapper(r, ApiDateTimeHelper, UserManager, DisplayUserSettingsHelper)).ToList();
        }

        public Task<string> UpdateCommentAsync(T fileId, int version, string comment)
        {
            return FileStorageService.UpdateCommentAsync(fileId, version, comment);
        }

        public Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(T fileId)
        {
            return GetSecurityInfoAsync(new List<T> { fileId }, new List<T> { });
        }

        public Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(T folderId)
        {
            return GetSecurityInfoAsync(new List<T> { }, new List<T> { folderId });
        }

        public async IAsyncEnumerable<FileEntryWrapper> GetFoldersAsync(T folderId)
        {
            var folders = await FileStorageService.GetFoldersAsync(folderId);
            foreach (var folder in folders)
            {
                yield return await GetFileEntryWrapperAsync(folder);
            }
        }

        public async Task<IEnumerable<FileShareWrapper>> GetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
        {
            var fileShares = await FileStorageService.GetSharedInfoAsync(fileIds, folderIds);
            return fileShares.Select(FileShareWrapperHelper.Get).ToList();
        }

        public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoAsync(T fileId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            return SetSecurityInfoAsync(new List<T> { fileId }, new List<T>(), share, notify, sharingMessage);
        }

        public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoAsync(T folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            return SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, share, notify, sharingMessage);
        }

        public async Task<IEnumerable<FileShareWrapper>> SetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            if (share != null && share.Any())
            {
                var list = new List<AceWrapper>(share.Select(FileShareParamsHelper.ToAceObject));
                var aceCollection = new AceCollection<T>
                {
                    Files = fileIds,
                    Folders = folderIds,
                    Aces = list,
                    Message = sharingMessage
                };
                await FileStorageService.SetAceObjectAsync(aceCollection, notify);
            }

            return await GetSecurityInfoAsync(fileIds, folderIds);
        }

        public async Task<bool> RemoveSecurityInfoAsync(List<T> fileIds, List<T> folderIds)
        {
            await FileStorageService.RemoveAceAsync(fileIds, folderIds);

            return true;
        }

        public async Task<string> GenerateSharedLinkAsync(T fileId, FileShare share)
        {
            var file = await GetFileInfoAsync(fileId);

            var tmpInfo = await FileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
            var sharedInfo = tmpInfo.Find(r => r.SubjectId == FileConstant.ShareLinkId);

            if (sharedInfo == null || sharedInfo.Share != share)
            {
                var list = new List<AceWrapper>
                    {
                        new AceWrapper
                            {
                                SubjectId = FileConstant.ShareLinkId,
                                SubjectGroup = true,
                                Share = share
                            }
                    };
                var aceCollection = new AceCollection<T>
                {
                    Files = new List<T> { fileId },
                    Folders = new List<T>(0),
                    Aces = list
                };
                await FileStorageService.SetAceObjectAsync(aceCollection, false);

                tmpInfo = await FileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
                sharedInfo = tmpInfo.Find(r => r.SubjectId == FileConstant.ShareLinkId);
            }

            return sharedInfo.Link;
        }

        public Task<bool> SetAceLinkAsync(T fileId, FileShare share)
        {
            return FileStorageService.SetAceLinkAsync(fileId, share);
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

        private async Task<FolderContentWrapper<T>> ToFolderContentWrapperAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withSubFolders)
        {
            OrderBy orderBy = null;
            if (Enum.TryParse(ApiContext.SortBy, true, out SortedByType sortBy))
            {
                orderBy = new OrderBy(sortBy, !ApiContext.SortDescending);
            }

            var startIndex = Convert.ToInt32(ApiContext.StartIndex);
            var items = await FileStorageService.GetFolderItemsAsync(folderId,
                                                                               startIndex,
                                                                               Convert.ToInt32(ApiContext.Count),
                                                                               filterType,
                                                                               filterType == FilterType.ByUser,
                                                                               userIdOrGroupId.ToString(),
                                                                               ApiContext.FilterValue,
                                                                               false,
                                                                               withSubFolders,
                                                                               orderBy);
            return await FolderContentWrapperHelper.GetAsync(items, startIndex);
        }

        internal async Task<FileEntryWrapper> GetFileEntryWrapperAsync(FileEntry r)
        {
            FileEntryWrapper wrapper = null;
            if (r is Folder<int> fol1)
            {
                wrapper = await FolderWrapperHelper.GetAsync(fol1);
            }
            else if (r is Folder<string> fol2)
            {
                wrapper = await FolderWrapperHelper.GetAsync(fol2);
            }
            else if (r is File<int> file1)
            {
                wrapper = await FileWrapperHelper.GetAsync(file1);
            }
            else if (r is File<string> file2)
            {
                wrapper = await FileWrapperHelper.GetAsync(file2);
            }

            return wrapper;
        }

        internal IFormFile GetFileFromRequest(IModelWithFile model)
        {
            IEnumerable<IFormFile> files = HttpContextAccessor.HttpContext.Request.Form.Files;
            if (files != null && files.Any())
            {
                return files.First();
            }

            return model.File;
        }
    }
}
