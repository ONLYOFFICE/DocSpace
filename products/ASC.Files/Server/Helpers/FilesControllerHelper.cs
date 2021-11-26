using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Entries;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Http;
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
        private ILog Logger { get; set; }

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
            IHttpContextAccessor httpContextAccessor)
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
            HttpContextAccessor = httpContextAccessor;
            Logger = optionMonitor.Get("ASC.Files");
        }

        public async Task<FolderContentWrapper<T>> GetFolderAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withSubFolders)
        {
            var folderContentWrapper = ToFolderContentWrapperAsync(folderId, userIdOrGroupId, filterType, withSubFolders);
            return await folderContentWrapper.NotFoundIfNull();
        }

        public object UploadFile(T folderId, UploadModel uploadModel)
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

                return new List<FileWrapper<T>>
                {
                    InsertFile(folderId, uploadModel.File.OpenReadStream(), fileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)
                };
            }

            throw new InvalidOperationException("No input files");
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
                return uploadModel.Files.Select(postedFile => InsertFile(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)).ToList();
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

        public FileWrapper<T> InsertFile(T folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
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

        public async Task<FileWrapper<T>> InsertFileAsync(T folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
        {
            try
            {
                var resultFile = await FileUploader.ExecAsync(folderId, title, file.Length, file, createNewIfExist ?? !FilesSettingsHelper.UpdateIfExist, !keepConvertStatus);
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

        public FileWrapper<T> UpdateFileStream(Stream file, T fileId, bool encrypted = false, bool forcesave = false)
        {
            try
            {
                var resultFile = FileStorageService.UpdateFileStream(fileId, file, encrypted, forcesave);
                return FileWrapperHelper.Get(resultFile);
            }
            catch (FileNotFoundException e)
            {
                throw new ItemNotFoundException("File not found", e);
            }
        }

        public FileWrapper<T> SaveEditing(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
        {
            return FileWrapperHelper.Get(FileStorageService.SaveEditing(fileId, fileExtension, downloadUri, stream, doc, forcesave));
        }

        public async Task<FileWrapper<T>> SaveEditingAsync(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
        {
            return FileWrapperHelper.Get(await FileStorageService.SaveEditingAsync(fileId, fileExtension, downloadUri, stream, doc, forcesave));
        }

        public string StartEdit(T fileId, bool editingAlone, string doc)
        {
            return FileStorageService.StartEdit(fileId, editingAlone, doc);
        }

        public async Task<string> StartEditAsync(T fileId, bool editingAlone, string doc)
        {
            return await FileStorageService.StartEditAsync(fileId, editingAlone, doc);
        }

        public KeyValuePair<bool, string> TrackEditFile(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return FileStorageService.TrackEditFile(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        public async Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
        {
            return await FileStorageService.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
        }

        public Configuration<T> OpenEdit(T fileId, int version, string doc, bool view)
        {
            DocumentServiceHelper.GetParams(fileId, version, doc, true, !view, true, out var configuration);
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

        public object CreateUploadSession(T folderId, string fileName, long fileSize, string relativePath, bool encrypted)
        {
            var file = FileUploader.VerifyChunkedUpload(folderId, fileName, fileSize, FilesSettingsHelper.UpdateIfExist, relativePath);

            if (FilesLinkUtility.IsLocalFileUploader)
            {
                var session = FileUploader.InitiateUpload(file.FolderID, (file.ID ?? default), file.Title, file.ContentLength, encrypted);

                var responseObject = ChunkedUploadSessionHelper.ToResponseObject(session, true);
                return new
                {
                    success = true,
                    data = responseObject
                };
            }

            var createSessionUrl = FilesLinkUtility.GetInitiateUploadSessionUrl(TenantManager.GetCurrentTenant().TenantId, file.FolderID, file.ID, file.Title, file.ContentLength, encrypted, SecurityContext);
            var request = (HttpWebRequest)WebRequest.Create(createSessionUrl);
            request.Method = "POST";
            request.ContentLength = 0;

            // hack for uploader.onlyoffice.com in api requests
            var rewriterHeader = ApiContext.HttpContextAccessor.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
            if (!string.IsNullOrEmpty(rewriterHeader))
            {
                request.Headers[HttpRequestExtensions.UrlRewriterHeader] = rewriterHeader;
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            using var response = request.GetResponse();
            using var responseStream = response.GetResponseStream();
            using var streamReader = new StreamReader(responseStream);
            return JObject.Parse(streamReader.ReadToEnd()); //result is json string
        }

        public async Task<object> CreateUploadSessionAsync(T folderId, string fileName, long fileSize, string relativePath, bool encrypted)
        {
            var file = await FileUploader.VerifyChunkedUploadAsync(folderId, fileName, fileSize, FilesSettingsHelper.UpdateIfExist, relativePath);

            if (FilesLinkUtility.IsLocalFileUploader)
            {
                var session = await FileUploader.InitiateUploadAsync(file.FolderID, (file.ID ?? default), file.Title, file.ContentLength, encrypted);

                var responseObject = await ChunkedUploadSessionHelper.ToResponseObjectAsync(session, true);
                return new
                {
                    success = true,
                    data = responseObject
                };
            }

            var createSessionUrl = FilesLinkUtility.GetInitiateUploadSessionUrl(TenantManager.GetCurrentTenant().TenantId, file.FolderID, file.ID, file.Title, file.ContentLength, encrypted, SecurityContext);
            var request = (HttpWebRequest)WebRequest.Create(createSessionUrl);
            request.Method = "POST";
            request.ContentLength = 0;

            // hack for uploader.onlyoffice.com in api requests
            var rewriterHeader = ApiContext.HttpContextAccessor.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
            if (!string.IsNullOrEmpty(rewriterHeader))
            {
                request.Headers[HttpRequestExtensions.UrlRewriterHeader] = rewriterHeader;
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            using var response = request.GetResponse();
            using var responseStream = response.GetResponseStream();
            using var streamReader = new StreamReader(responseStream);
            return JObject.Parse(streamReader.ReadToEnd()); //result is json string
        }

        public FileWrapper<T> CreateTextFile(T folderId, string title, string content)
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

        public async Task<FileWrapper<T>> CreateTextFileAsync(T folderId, string title, string content)
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
            return await CreateFileAsync(folderId, title, content, extension);
        }

        private FileWrapper<T> CreateFile(T folderId, string title, string content, string extension)
        {
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var file = FileUploader.Exec(folderId,
                              title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension),
                              memStream.Length, memStream);
            return FileWrapperHelper.Get(file);
        }

        private async Task<FileWrapper<T>> CreateFileAsync(T folderId, string title, string content, string extension)
        {
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var file = await FileUploader.ExecAsync(folderId,
                              title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension),
                              memStream.Length, memStream);
            return await FileWrapperHelper.GetAsync(file);
        }

        public FileWrapper<T> CreateHtmlFile(T folderId, string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            return CreateFile(folderId, title, content, ".html");
        }

        public async Task<FileWrapper<T>> CreateHtmlFileAsync(T folderId, string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            return await CreateFileAsync(folderId, title, content, ".html");
        }

        public FolderWrapper<T> CreateFolder(T folderId, string title)
        {
            var folder = FileStorageService.CreateNewFolder(folderId, title);
            return FolderWrapperHelper.Get(folder);
        }

        public async Task<FolderWrapper<T>> CreateFolderAsync(T folderId, string title)
        {
            var folder = await FileStorageService.CreateNewFolderAsync(folderId, title);
            return FolderWrapperHelper.Get(folder);
        }

        public FileWrapper<T> CreateFile(T folderId, string title, T templateId, bool enableExternalExt = false)
        {
            var file = FileStorageService.CreateNewFile(new FileModel<T> { ParentId = folderId, Title = title, TemplateId = templateId }, enableExternalExt);
            return FileWrapperHelper.Get(file);
        }

        public async Task<FileWrapper<T>> CreateFileAsync(T folderId, string title, T templateId, bool enableExternalExt = false)
        {
            var file = await FileStorageService.CreateNewFileAsync(new FileModel<T> { ParentId = folderId, Title = title, TemplateId = templateId }, enableExternalExt);
            return FileWrapperHelper.Get(file);
        }

        public FolderWrapper<T> RenameFolder(T folderId, string title)
        {
            var folder = FileStorageService.FolderRename(folderId, title);
            return FolderWrapperHelper.Get(folder);
        }

        public async Task<FolderWrapper<T>> RenameFolderAsync(T folderId, string title)
        {
            var folder = await FileStorageService.FolderRenameAsync(folderId, title);
            return FolderWrapperHelper.Get(folder);
        }

        public FolderWrapper<T> GetFolderInfo(T folderId)
        {
            var folder = FileStorageService.GetFolder(folderId).NotFoundIfNull("Folder not found");

            return FolderWrapperHelper.Get(folder);
        }

        public async Task<FolderWrapper<T>> GetFolderInfoAsync(T folderId)
        {
            var folder = await FileStorageService.GetFolderAsync(folderId).NotFoundIfNull("Folder not found");

            return await FolderWrapperHelper.GetAsync(folder);
        }

        public IEnumerable<FileEntryWrapper> GetFolderPath(T folderId)
        {
            return EntryManager.GetBreadCrumbs(folderId).Select(GetFileEntryWrapper);
        }

        public async IAsyncEnumerable<FileEntryWrapper> GetFolderPathAsync(T folderId)
        {
            var breadCrumbs = await EntryManager.GetBreadCrumbsAsync(folderId);

            foreach (var e in breadCrumbs)
            {
                yield return await GetFileEntryWrapperAsync(e);
            }
        }

        public FileWrapper<T> GetFileInfo(T fileId, int version = -1)
        {
            var file = FileStorageService.GetFile(fileId, version).NotFoundIfNull("File not found");
            return FileWrapperHelper.Get(file);
        }

        public async Task<FileWrapper<T>> GetFileInfoAsync(T fileId, int version = -1)
        {
            var file = await FileStorageService.GetFileAsync(fileId, version);
            file = file.NotFoundIfNull("File not found");
            return await FileWrapperHelper.GetAsync(file);
        }

        public async Task<FileWrapper<T>> AddToRecentAsync(T fileId, int version = -1)
        {
            var file = await FileStorageService.GetFileAsync(fileId, version).NotFoundIfNull("File not found");
            EntryManager.MarkAsRecent(file);
            return await FileWrapperHelper.GetAsync(file);
        }

        public List<FileEntryWrapper> GetNewItems(T folderId)
        {
            return FileStorageService.GetNewItems(folderId)
                .Select(GetFileEntryWrapper)
                .ToList();
        }

        public async Task<List<FileEntryWrapper>> GetNewItemsAsync(T folderId)
        {
            var newItems = await FileStorageService.GetNewItemsAsync(folderId);
            return newItems.Select(GetFileEntryWrapper).ToList();
        }

        public FileWrapper<T> UpdateFile(T fileId, string title, int lastVersion)
        {
            File<T> newFile = null;
            if (!string.IsNullOrEmpty(title))
            {
                newFile = FileStorageService.FileRename(fileId, title);
            }

            if (lastVersion > 0)
            {
                var pair = FileStorageService.UpdateToVersion(fileId, lastVersion);
                newFile = pair.Key;
            }

            return newFile != null ? FileWrapperHelper.Get(newFile) : GetFileInfo(fileId);
        }

        public async Task<FileWrapper<T>> UpdateFileAsync(T fileId, string title, int lastVersion)
        {
            if (!string.IsNullOrEmpty(title))
                await FileStorageService.FileRenameAsync(fileId, title);

            if (lastVersion > 0)
                await FileStorageService.UpdateToVersionAsync(fileId, lastVersion);

            return await GetFileInfoAsync(fileId);
        }

        public IEnumerable<FileOperationWraper> DeleteFile(T fileId, bool deleteAfter, bool immediately)
        {
            return FileStorageService.DeleteFile("delete", fileId, false, deleteAfter, immediately)
                .Select(FileOperationWraperHelper.Get);
        }

        public IEnumerable<ConversationResult<T>> StartConversion(T fileId, bool sync = false)
        {
            return CheckConversion(fileId, true, sync);
        }

        public IAsyncEnumerable<ConversationResult<T>> StartConversionAsync(T fileId, bool sync = false)
        {
            return CheckConversionAsync(fileId, true, sync);
        }

        public IEnumerable<ConversationResult<T>> CheckConversion(T fileId, bool start, bool sync = false)
        {
            return FileStorageService.CheckConversion(new List<List<string>>
            {
                new List<string> { fileId.ToString(), "0", start.ToString() }
            }, sync)
            .Select(r =>
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
                        o.File = GetFileInfo(jResult.Id, jResult.Version);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
                return o;
            });
        }

        public async IAsyncEnumerable<ConversationResult<T>> CheckConversionAsync(T fileId, bool start, bool sync = false)
        {
            var checkConversaion = await FileStorageService.CheckConversionAsync(new List<List<string>>
            {
                new List<string> { fileId.ToString(), "0", start.ToString() }
            }, sync);

            foreach(var r in checkConversaion)
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
                        Logger.Error(e);
                    }
                }
                yield return o;
            }
        }

        public IEnumerable<FileOperationWraper> DeleteFolder(T folderId, bool deleteAfter, bool immediately)
        {
            return FileStorageService.DeleteFolder("delete", folderId, false, deleteAfter, immediately)
                    .Select(FileOperationWraperHelper.Get)
                    .ToList();
        }

        public IEnumerable<FileEntryWrapper> MoveOrCopyBatchCheck(BatchModel batchModel)
        {
            var checkedFiles = new List<object>();
            var checkedFolders = new List<object>();

            if (batchModel.DestFolderId.ValueKind == JsonValueKind.Number)
            {
                (checkedFiles, checkedFolders) = FileStorageService.MoveOrCopyFilesCheck(batchModel.FileIds.ToList(), batchModel.FolderIds.ToList(), batchModel.DestFolderId.GetInt32());
            }
            else
            {
                (checkedFiles, checkedFolders) = FileStorageService.MoveOrCopyFilesCheck(batchModel.FileIds.ToList(), batchModel.FolderIds.ToList(), batchModel.DestFolderId.GetString());
            }

            var entries = FileStorageService.GetItems(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");

            entries.AddRange(FileStorageService.GetItems(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

            return entries.Select(GetFileEntryWrapper).ToList();
        }

        public async IAsyncEnumerable<FileEntryWrapper> MoveOrCopyBatchCheckAsync(BatchModel batchModel)
        {
            var checkedFiles = new List<object>();
            var checkedFolders = new List<object>();

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

            foreach(var e in entries)
            {
                yield return await GetFileEntryWrapperAsync(e);
            }
        }

        public IEnumerable<FileOperationWraper> MoveBatchItems(BatchModel batchModel)
        {
            return FileStorageService.MoveOrCopyItems(batchModel.FolderIds.ToList(), batchModel.FileIds.ToList(), batchModel.DestFolderId, batchModel.ConflictResolveType, false, batchModel.DeleteAfter)
                .Select(FileOperationWraperHelper.Get)
                .ToList();
        }

        public IEnumerable<FileOperationWraper> CopyBatchItems(BatchModel batchModel)
        {
            return FileStorageService.MoveOrCopyItems(batchModel.FolderIds.ToList(), batchModel.FileIds.ToList(), batchModel.DestFolderId, batchModel.ConflictResolveType, true, batchModel.DeleteAfter)
                .Select(FileOperationWraperHelper.Get)
                .ToList();
        }

        public IEnumerable<FileOperationWraper> MarkAsRead(BaseBatchModel model)
        {
            return FileStorageService.MarkAsRead(model.FolderIds.ToList(), model.FileIds.ToList()).Select(FileOperationWraperHelper.Get).ToList();
        }

        public IEnumerable<FileOperationWraper> TerminateTasks()
        {
            return FileStorageService.TerminateTasks().Select(FileOperationWraperHelper.Get);
        }

        public IEnumerable<FileOperationWraper> GetOperationStatuses()
        {
            return FileStorageService.GetTasksStatuses().Select(FileOperationWraperHelper.Get);
        }

        public IEnumerable<FileOperationWraper> BulkDownload(DownloadModel model)
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

            return FileStorageService.BulkDownload(folders, files).Select(FileOperationWraperHelper.Get).ToList();
        }

        public IEnumerable<FileOperationWraper> EmptyTrash()
        {
            return FileStorageService.EmptyTrash().Select(FileOperationWraperHelper.Get).ToList();
        }

        public IEnumerable<FileWrapper<T>> GetFileVersionInfo(T fileId)
        {
            var files = FileStorageService.GetFileHistory(fileId);
            return files.Select(r => FileWrapperHelper.Get(r)).ToList();
        }

        public async Task<IEnumerable<FileWrapper<T>>> GetFileVersionInfoAsync(T fileId)
        {
            var files = await FileStorageService.GetFileHistoryAsync(fileId);
            return files.Select(r => FileWrapperHelper.Get(r)).ToList();
        }

        public IEnumerable<FileWrapper<T>> ChangeHistory(T fileId, int version, bool continueVersion)
        {
            var history = FileStorageService.CompleteVersion(fileId, version, continueVersion).Value;
            return history.Select(r => FileWrapperHelper.Get(r)).ToList();
        }

        public async Task<IEnumerable<FileWrapper<T>>> ChangeHistoryAsync(T fileId, int version, bool continueVersion)
        {
            var pair = await FileStorageService.CompleteVersionAsync(fileId, version, continueVersion);
            var history = pair.Value;
            return history.Select(r => FileWrapperHelper.Get(r)).ToList();
        }

        public FileWrapper<T> LockFile(T fileId, bool lockFile)
        {
            var result = FileStorageService.LockFile(fileId, lockFile);
            return FileWrapperHelper.Get(result);
        }

        public async Task<FileWrapper<T>> LockFileAsync(T fileId, bool lockFile)
        {
            var result = await FileStorageService.LockFileAsync(fileId, lockFile);
            return await FileWrapperHelper.GetAsync(result);
        }

        public DocumentService.FileLink GetPresignedUri(T fileId)
        {
            return FileStorageService.GetPresignedUri(fileId);
        }

        public async Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
        {
            return await FileStorageService.GetPresignedUriAsync(fileId);
        }

        public string UpdateComment(T fileId, int version, string comment)
        {
            return FileStorageService.UpdateComment(fileId, version, comment);
        }

        public async Task<string> UpdateCommentAsync(T fileId, int version, string comment)
        {
            return await FileStorageService.UpdateCommentAsync(fileId, version, comment);
        }

        public IEnumerable<FileShareWrapper> GetFileSecurityInfo(T fileId)
        {
            return GetSecurityInfo(new List<T> { fileId }, new List<T> { });
        }

        public async Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(T fileId)
        {
            return await GetSecurityInfoAsync(new List<T> { fileId }, new List<T> { });
        }

        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(T folderId)
        {
            return GetSecurityInfo(new List<T> { }, new List<T> { folderId });
        }

        public async Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(T folderId)
        {
            return await GetSecurityInfoAsync(new List<T> { }, new List<T> { folderId });
        }

        public IEnumerable<FileEntryWrapper> GetFolders(T folderId)
        {
            return FileStorageService.GetFolders(folderId)
                                .Select(GetFileEntryWrapper)
                                .ToList();
        }

        public async IAsyncEnumerable<FileEntryWrapper> GetFoldersAsync(T folderId)
        {
            var folders = await FileStorageService.GetFoldersAsync(folderId);
            foreach (var folder in folders)
            {
                yield return await GetFileEntryWrapperAsync(folder);
            }
        }

        public IEnumerable<FileShareWrapper> GetSecurityInfo(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
        {
            var fileShares = FileStorageService.GetSharedInfo(fileIds, folderIds);
            return fileShares.Select(FileShareWrapperHelper.Get).ToList();
        }

        public async Task<IEnumerable<FileShareWrapper>> GetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
        {
            var fileShares = await FileStorageService.GetSharedInfoAsync(fileIds, folderIds);
            return fileShares.Select(FileShareWrapperHelper.Get).ToList();
        }

        public IEnumerable<FileShareWrapper> SetFileSecurityInfo(T fileId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            return SetSecurityInfo(new List<T> { fileId }, new List<T>(), share, notify, sharingMessage);
        }

        public async Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoAsync(T fileId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            return await SetSecurityInfoAsync(new List<T> { fileId }, new List<T>(), share, notify, sharingMessage);
        }

        public IEnumerable<FileShareWrapper> SetFolderSecurityInfo(T folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            return SetSecurityInfo(new List<T>(), new List<T> { folderId }, share, notify, sharingMessage);
        }

        public async Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoAsync(T folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
        {
            return await SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, share, notify, sharingMessage);
        }

        public IEnumerable<FileShareWrapper> SetSecurityInfo(IEnumerable<T> fileIds, IEnumerable<T> folderIds, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
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
                FileStorageService.SetAceObject(aceCollection, notify);
            }

            return GetSecurityInfo(fileIds, folderIds);
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

        public bool RemoveSecurityInfo(List<T> fileIds, List<T> folderIds)
        {
            FileStorageService.RemoveAce(fileIds, folderIds);

            return true;
        }

        public async Task<bool> RemoveSecurityInfoAsync(List<T> fileIds, List<T> folderIds)
        {
            await FileStorageService.RemoveAceAsync(fileIds, folderIds);

            return true;
        }

        public string GenerateSharedLink(T fileId, FileShare share)
        {
            var file = GetFileInfo(fileId);

            var sharedInfo = FileStorageService.GetSharedInfo(new List<T> { fileId }, new List<T> { }).Find(r => r.SubjectId == FileConstant.ShareLinkId);
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
                    Aces = list
                };
                FileStorageService.SetAceObject(aceCollection, false);
                sharedInfo = FileStorageService.GetSharedInfo(new List<T> { fileId }, new List<T> { }).Find(r => r.SubjectId == FileConstant.ShareLinkId);
            }

            return sharedInfo.Link;
        }

        public async Task<string> GenerateSharedLinkAsync(T fileId, FileShare share)
        {
            var file = await GetFileInfoAsync(fileId);

            var sharedInfo = FileStorageService.GetSharedInfo(new List<T> { fileId }, new List<T> { }).Find(r => r.SubjectId == FileConstant.ShareLinkId);
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
                    Aces = list
                };
                FileStorageService.SetAceObject(aceCollection, false);
                sharedInfo = FileStorageService.GetSharedInfo(new List<T> { fileId }, new List<T> { }).Find(r => r.SubjectId == FileConstant.ShareLinkId);
            }

            return sharedInfo.Link;
        }

        public bool SetAceLink(T fileId, FileShare share)
        {
            return FileStorageService.SetAceLink(fileId, share);
        }

        public async Task<bool> SetAceLinkAsync(T fileId, FileShare share)
        {
            return await FileStorageService.SetAceLinkAsync(fileId, share);
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
            return await FolderContentWrapperHelper.GetAsync(await FileStorageService.GetFolderItemsAsync(folderId,
                                                                               startIndex,
                                                                               Convert.ToInt32(ApiContext.Count),
                                                                               filterType,
                                                                               filterType == FilterType.ByUser,
                                                                               userIdOrGroupId.ToString(),
                                                                               ApiContext.FilterValue,
                                                                               false,
                                                                               withSubFolders,
                                                                               orderBy),
                                            startIndex);
        }

        internal FileEntryWrapper GetFileEntryWrapper(FileEntry r)
        {
            FileEntryWrapper wrapper = null;
            if (r is Folder<int> fol1)
            {
                wrapper = FolderWrapperHelper.Get(fol1);
            }
            else if (r is Folder<string> fol2)
            {
                wrapper = FolderWrapperHelper.Get(fol2);
            }
            else if (r is File<int> file1)
            {
                wrapper = FileWrapperHelper.Get(file1);
            }
            else if (r is File<string> file2)
            {
                wrapper = FileWrapperHelper.Get(file2);
            }

            return wrapper;
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
