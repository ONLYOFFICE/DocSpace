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
using System.Net.Http;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Common;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.Utils
{
    [Singletone(Additional = typeof(FileConverterQueueExtension))]
    internal class FileConverterQueue<T> : IDisposable
    {
        private readonly object singleThread = new object();
        private readonly IDictionary<File<T>, ConvertFileOperationResult> conversionQueue;
        private readonly Timer timer;
        private readonly object locker;
        private readonly ICache cache;
        private const int TIMER_PERIOD = 500;

        private IServiceProvider ServiceProvider { get; }

        public FileConverterQueue(IServiceProvider ServiceProvider, ICache cache)
        {
            conversionQueue = new Dictionary<File<T>, ConvertFileOperationResult>(new FileComparer<T>());
            timer = new Timer(CheckConvertFilesStatus, null, 0, Timeout.Infinite);
            locker = new object();
            this.ServiceProvider = ServiceProvider;
            this.cache = cache;
        }

        public void Add(File<T> file, string password, int tenantId, IAccount account, bool deleteAfter, string url, string serverRootPath)
        {
            lock (locker)
            {
                if (conversionQueue.ContainsKey(file))
                {
                    return;
                }

                var queueResult = new ConvertFileOperationResult
                {
                    Source = string.Format("{{\"id\":\"{0}\", \"version\":\"{1}\"}}", file.ID, file.Version),
                    OperationType = FileOperationType.Convert,
                    Error = string.Empty,
                    Progress = 0,
                    Result = string.Empty,
                    Processed = "",
                    Id = string.Empty,
                    TenantId = tenantId,
                    Account = account,
                    Delete = deleteAfter,
                    StartDateTime = DateTime.Now,
                    Url = url,
                    Password = password,
                    ServerRootPath = serverRootPath
                };
                conversionQueue.Add(file, queueResult);
                cache.Insert(GetKey(file), queueResult, TimeSpan.FromMinutes(10));

                timer.Change(0, Timeout.Infinite);
            }
        }

        public async Task<ConvertFileOperationResult> GetStatusAsync(KeyValuePair<File<T>, bool> pair, FileSecurity fileSecurity)
        {
            var file = pair.Key;
            var key = GetKey(file);
            var operation = cache.Get<ConvertFileOperationResult>(key);
            if (operation != null && (pair.Value || await fileSecurity.CanReadAsync(file)))
            {
                lock (locker)
                {
                    if (operation.Progress == 100)
                    {
                        conversionQueue.Remove(file);
                        cache.Remove(key);
                    }
                    return operation;
                }
            }
            return null;
        }

        public bool IsConverting(File<T> file)
        {
            var result = cache.Get<ConvertFileOperationResult>(GetKey(file));
            return result != null && result.Progress != 100 && string.IsNullOrEmpty(result.Error);
        }

        private void CheckConvertFilesStatus(object _)
        {
            if (Monitor.TryEnter(singleThread))
            {
                using var scope = ServiceProvider.CreateScope();
                TenantManager tenantManager;
                UserManager userManager;
                SecurityContext securityContext;
                IDaoFactory daoFactory;
                FileSecurity fileSecurity;
                PathProvider pathProvider;
                SetupInfo setupInfo;
                FileUtility fileUtility;
                DocumentServiceHelper documentServiceHelper;
                DocumentServiceConnector documentServiceConnector;
                EntryStatusManager entryManager;
                FileConverter fileConverter;

                var logger = scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;

                try
                {
                    var filesIsConverting = new List<File<T>>();
                    lock (locker)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);

                        var queues = conversionQueue.Where(x => !string.IsNullOrEmpty(x.Value.Processed)
                                                   && (x.Value.Progress == 100 && DateTime.UtcNow - x.Value.StopDateTime > TimeSpan.FromMinutes(1) ||
                                                       DateTime.UtcNow - x.Value.StopDateTime > TimeSpan.FromMinutes(10)))
                            .ToList();

                        foreach (var q in queues)
                        {
                            conversionQueue.Remove(q);
                            cache.Remove(GetKey(q.Key));
                        }

                        logger.DebugFormat("Run CheckConvertFilesStatus: count {0}", conversionQueue.Count);

                        if (conversionQueue.Count == 0)
                        {
                            return;
                        }

                        filesIsConverting = conversionQueue
                            .Where(x => string.IsNullOrEmpty(x.Value.Processed))
                            .Select(x => x.Key)
                            .ToList();
                    }

                    string convertedFileUrl = null;

                    foreach (var file in filesIsConverting)
                    {
                        var fileUri = file.ID.ToString();
                        int operationResultProgress;

                        try
                        {
                            int tenantId;
                            IAccount account;
                            string password;
                            string serverRootPath;

                            lock (locker)
                            {
                                if (!conversionQueue.Keys.Contains(file)) continue;

                                var operationResult = conversionQueue[file];
                                if (!string.IsNullOrEmpty(operationResult.Processed)) continue;

                                operationResult.Processed = "1";
                                tenantId = operationResult.TenantId;
                                account = operationResult.Account;
                                password = operationResult.Password;
                                serverRootPath = operationResult.ServerRootPath;

                                //if (HttpContext.Current == null && !WorkContext.IsMono)
                                //{
                                //    HttpContext.Current = new HttpContext(
                                //        new HttpRequest("hack", operationResult.Url, string.Empty),
                                //        new HttpResponse(new StringWriter()));
                                //}

                                cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                            }

                            var commonLinkUtilitySettings = scope.ServiceProvider.GetService<CommonLinkUtilitySettings>();
                            commonLinkUtilitySettings.ServerUri = serverRootPath;

                            var scopeClass = scope.ServiceProvider.GetService<FileConverterQueueScope>();
                            (_, tenantManager, userManager, securityContext, daoFactory, fileSecurity, pathProvider, setupInfo, fileUtility, documentServiceHelper, documentServiceConnector, entryManager, fileConverter) = scopeClass;

                            tenantManager.SetCurrentTenant(tenantId);

                            securityContext.AuthenticateMeWithoutCookie(account);

                            var user = userManager.GetUsers(account.ID);
                            var culture = string.IsNullOrEmpty(user.CultureName) ? tenantManager.GetCurrentTenant().GetCulture() : CultureInfo.GetCultureInfo(user.CultureName);
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = culture;

                            if (!fileSecurity.CanReadAsync(file).Result && file.RootFolderType != FolderType.BUNCH)
                            {
                                //No rights in CRM after upload before attach
                                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
                            }
                            if (file.ContentLength > setupInfo.AvailableFileSize)
                            {
                                throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(setupInfo.AvailableFileSize)));
                            }

                            fileUri = pathProvider.GetFileStreamUrl(file);

                            var toExtension = fileUtility.GetInternalExtension(file.Title);
                            var fileExtension = file.ConvertedExtension;
                            var docKey = documentServiceHelper.GetDocKey(file);

                            fileUri = documentServiceConnector.ReplaceCommunityAdress(fileUri);
                            (operationResultProgress, convertedFileUrl) = documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, password, null, null, true).Result;
                        }
                        catch (Exception exception)
                        {
                            var password = exception.InnerException is DocumentService.DocumentServiceException documentServiceException
                                           && documentServiceException.Code == DocumentService.DocumentServiceException.ErrorCode.ConvertPassword;

                            logger.Error(string.Format("Error convert {0} with url {1}", file.ID, fileUri), exception);
                            lock (locker)
                            {
                                if (conversionQueue.TryGetValue(file, out var operationResult))
                                {
                                    if (operationResult.Delete)
                                    {
                                        conversionQueue.Remove(file);
                                        cache.Remove(GetKey(file));
                                    }
                                    else
                                    {
                                        operationResult.Progress = 100;
                                        operationResult.StopDateTime = DateTime.UtcNow;
                                        operationResult.Error = exception.Message;
                                        if (password) operationResult.Result = "password";
                                        cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                                    }
                                }
                            }
                            continue;
                        }

                        operationResultProgress = Math.Min(operationResultProgress, 100);
                        if (operationResultProgress < 100)
                        {
                            lock (locker)
                            {
                                if (conversionQueue.TryGetValue(file, out var operationResult))
                                {
                                    if (DateTime.Now - operationResult.StartDateTime > TimeSpan.FromMinutes(10))
                                    {
                                        operationResult.StopDateTime = DateTime.UtcNow;
                                        operationResult.Error = FilesCommonResource.ErrorMassage_ConvertTimeout;
                                        logger.ErrorFormat("CheckConvertFilesStatus timeout: {0} ({1})", file.ID, file.ContentLengthString);
                                    }
                                    else
                                    {
                                        operationResult.Processed = "";
                                    }
                                    operationResult.Progress = operationResultProgress;
                                    cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                                }
                            }

                            logger.Debug("CheckConvertFilesStatus iteration continue");
                            continue;
                        }

                        File<T> newFile = null;
                        var operationResultError = string.Empty;

                        try
                        {
                            newFile = fileConverter.SaveConvertedFileAsync(file, convertedFileUrl).Result;
                        }
                        catch (Exception e)
                        {
                            operationResultError = e.Message;

                            logger.ErrorFormat("{0} ConvertUrl: {1} fromUrl: {2}: {3}", operationResultError, convertedFileUrl, fileUri, e);
                            continue;
                        }
                        finally
                        {
                            lock (locker)
                            {
                                if (conversionQueue.TryGetValue(file, out var operationResult))
                                {
                                    if (operationResult.Delete)
                                    {
                                        conversionQueue.Remove(file);
                                        cache.Remove(GetKey(file));
                                    }
                                    else
                                    {
                                        if (newFile != null)
                                        {
                                            var folderDao = daoFactory.GetFolderDao<T>();
                                            var folder = folderDao.GetFolderAsync(newFile.FolderID).Result;
                                            var folderTitle = fileSecurity.CanReadAsync(folder).Result ? folder.Title : null;
                                            operationResult.Result = FileJsonSerializerAsync(entryManager, newFile, folderTitle).Result;
                                        }

                                        operationResult.Progress = 100;
                                        operationResult.StopDateTime = DateTime.UtcNow;
                                        operationResult.Processed = "1";
                                        if (!string.IsNullOrEmpty(operationResultError))
                                        {
                                            operationResult.Error = operationResultError;
                                        }
                                        cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                                    }
                                }
                            }
                        }

                        logger.Debug("CheckConvertFilesStatus iteration end");
                    }

                    lock (locker)
                    {
                        timer.Change(TIMER_PERIOD, TIMER_PERIOD);
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(exception.Message, exception);
                    lock (locker)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }
                finally
                {
                    Monitor.Exit(singleThread);
                }
            }
        }

        private string GetKey(File<T> f)
        {
            return string.Format("fileConvertation-{0}", f.ID);
        }

        internal async Task<string> FileJsonSerializerAsync(EntryStatusManager EntryManager, File<T> file, string folderTitle)
        {
            if (file == null) return string.Empty;

            await EntryManager.SetFileStatusAsync(file);

            var options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
                WriteIndented = false
            };

            return JsonSerializer.Serialize(
                                  new FileJsonSerializerData<T>()
                                  {
                                      Id = file.ID,
                                      Title = file.Title,
                                      Version = file.Version,
                                      FolderID = file.FolderID,
                                      FolderTitle = folderTitle ?? "",
                                      FileJson = JsonSerializer.Serialize(file, options)
                                  }, options);
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }

    [Scope]
    public class FileConverterQueueScope
    {
        private IOptionsMonitor<ILog> Options { get; }
        private TenantManager TenantManager { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private IDaoFactory DaoFactory { get; }
        private FileSecurity FileSecurity { get; }
        private PathProvider PathProvider { get; }
        private SetupInfo SetupInfo { get; }
        private FileUtility FileUtility { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private EntryStatusManager EntryManager { get; }
        private FileConverter FileConverter { get; }

        public FileConverterQueueScope(IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            UserManager userManager,
            SecurityContext securityContext,
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            PathProvider pathProvider,
            SetupInfo setupInfo,
            FileUtility fileUtility,
            DocumentServiceHelper documentServiceHelper,
            DocumentServiceConnector documentServiceConnector,
            EntryStatusManager entryManager,
            FileConverter fileConverter)
        {
            Options = options;
            TenantManager = tenantManager;
            UserManager = userManager;
            SecurityContext = securityContext;
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            PathProvider = pathProvider;
            SetupInfo = setupInfo;
            FileUtility = fileUtility;
            DocumentServiceHelper = documentServiceHelper;
            DocumentServiceConnector = documentServiceConnector;
            EntryManager = entryManager;
            FileConverter = fileConverter;
        }


        public void Deconstruct(out IOptionsMonitor<ILog> optionsMonitor,
            out TenantManager tenantManager,
            out UserManager userManager,
            out SecurityContext securityContext,
            out IDaoFactory daoFactory,
            out FileSecurity fileSecurity,
            out PathProvider pathProvider,
            out SetupInfo setupInfo,
            out FileUtility fileUtility,
            out DocumentServiceHelper documentServiceHelper,
            out DocumentServiceConnector documentServiceConnector,
            out EntryStatusManager entryManager,
            out FileConverter fileConverter)
        {
            optionsMonitor = Options;
            tenantManager = TenantManager;
            userManager = UserManager;
            securityContext = SecurityContext;
            daoFactory = DaoFactory;
            fileSecurity = FileSecurity;
            pathProvider = PathProvider;
            setupInfo = SetupInfo;
            fileUtility = FileUtility;
            documentServiceHelper = DocumentServiceHelper;
            documentServiceConnector = DocumentServiceConnector;
            entryManager = EntryManager;
            fileConverter = FileConverter;
        }

    }

    public class FileJsonSerializerData<T>
    {
        public T Id { get; set; }
        public string Title { get; set; }
        public int Version { get; set; }
        public T FolderID { get; set; }
        public string FolderTitle { get; set; }
        public string FileJson { get; set; }
    }

    [Scope(Additional = typeof(FileConverterExtension))]
    public class FileConverter
    {
        private FileUtility FileUtility { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private IDaoFactory DaoFactory { get; }
        private SetupInfo SetupInfo { get; }
        private PathProvider PathProvider { get; }
        private FileSecurity FileSecurity { get; }
        private FileMarker FileMarker { get; }
        private TenantManager TenantManager { get; }
        private AuthContext AuthContext { get; }
        private EntryManager EntryManager { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private FilesMessageService FilesMessageService { get; }
        private FileShareLink FileShareLink { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private FileTrackerHelper FileTracker { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private EntryStatusManager EntryStatusManager { get; }
        private IServiceProvider ServiceProvider { get; }
        private IHttpContextAccessor HttpContextAccesor { get; }
        private IHttpClientFactory ClientFactory { get; }

        public FileConverter(
            FileUtility fileUtility,
            FilesLinkUtility filesLinkUtility,
            IDaoFactory daoFactory,
            SetupInfo setupInfo,
            PathProvider pathProvider,
            FileSecurity fileSecurity,
            FileMarker fileMarker,
            TenantManager tenantManager,
            AuthContext authContext,
            EntryManager entryManager,
            FilesSettingsHelper filesSettingsHelper,
            GlobalFolderHelper globalFolderHelper,
            FilesMessageService filesMessageService,
            FileShareLink fileShareLink,
            DocumentServiceHelper documentServiceHelper,
            DocumentServiceConnector documentServiceConnector,
            FileTrackerHelper fileTracker,
            BaseCommonLinkUtility baseCommonLinkUtility,
            EntryStatusManager entryStatusManager,
            IServiceProvider serviceProvider,
            IHttpClientFactory clientFactory)
        {
            FileUtility = fileUtility;
            FilesLinkUtility = filesLinkUtility;
            DaoFactory = daoFactory;
            SetupInfo = setupInfo;
            PathProvider = pathProvider;
            FileSecurity = fileSecurity;
            FileMarker = fileMarker;
            TenantManager = tenantManager;
            AuthContext = authContext;
            EntryManager = entryManager;
            FilesSettingsHelper = filesSettingsHelper;
            GlobalFolderHelper = globalFolderHelper;
            FilesMessageService = filesMessageService;
            FileShareLink = fileShareLink;
            DocumentServiceHelper = documentServiceHelper;
            DocumentServiceConnector = documentServiceConnector;
            FileTracker = fileTracker;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            EntryStatusManager = entryStatusManager;
            ServiceProvider = serviceProvider;
            ClientFactory = clientFactory;
        }
        public FileConverter(
            FileUtility fileUtility,
            FilesLinkUtility filesLinkUtility,
            IDaoFactory daoFactory,
            SetupInfo setupInfo,
            PathProvider pathProvider,
            FileSecurity fileSecurity,
            FileMarker fileMarker,
            TenantManager tenantManager,
            AuthContext authContext,
            EntryManager entryManager,
            FilesSettingsHelper filesSettingsHelper,
            GlobalFolderHelper globalFolderHelper,
            FilesMessageService filesMessageService,
            FileShareLink fileShareLink,
            DocumentServiceHelper documentServiceHelper,
            DocumentServiceConnector documentServiceConnector,
            FileTrackerHelper fileTracker,
            BaseCommonLinkUtility baseCommonLinkUtility,
            EntryStatusManager entryStatusManager,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccesor,
            IHttpClientFactory clientFactory)
            : this(fileUtility, filesLinkUtility, daoFactory, setupInfo, pathProvider, fileSecurity,
                  fileMarker, tenantManager, authContext, entryManager, filesSettingsHelper,
                  globalFolderHelper, filesMessageService, fileShareLink, documentServiceHelper, documentServiceConnector, fileTracker,
                  baseCommonLinkUtility, entryStatusManager, serviceProvider, clientFactory)
        {
            HttpContextAccesor = httpContextAccesor;
        }

        public bool EnableAsUploaded
        {
            get { return FileUtility.ExtsMustConvert.Count > 0 && !string.IsNullOrEmpty(FilesLinkUtility.DocServiceConverterUrl); }
        }

        public bool MustConvert<T>(File<T> file)
        {
            if (file == null) return false;

            var ext = FileUtility.GetFileExtension(file.Title);
            return FileUtility.ExtsMustConvert.Contains(ext);
        }

        public bool EnableConvert<T>(File<T> file, string toExtension)
        {
            if (file == null || string.IsNullOrEmpty(toExtension))
            {
                return false;
            }

            if (file.Encrypted)
            {
                return false;
            }

            var fileExtension = file.ConvertedExtension;
            if (fileExtension.Trim('.').Equals(toExtension.Trim('.'), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            fileExtension = FileUtility.GetFileExtension(file.Title);
            if (FileUtility.InternalExtension.Values.Contains(toExtension))
            {
                return true;
            }

            return FileUtility.ExtsConvertible.Keys.Contains(fileExtension) && FileUtility.ExtsConvertible[fileExtension].Contains(toExtension);
        }

        public Task<Stream> ExecAsync<T>(File<T> file)
        {
            return ExecAsync(file, FileUtility.GetInternalExtension(file.Title));
        }      

        public async Task<Stream> ExecAsync<T>(File<T> file, string toExtension, string password = null)
        {
            if (!EnableConvert(file, toExtension))
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                return await fileDao.GetFileStreamAsync(file);
            }

            if (file.ContentLength > SetupInfo.AvailableFileSize)
            {
                throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize)));
            }

            var fileUri = PathProvider.GetFileStreamUrl(file);
            var docKey = DocumentServiceHelper.GetDocKey(file);
            fileUri = DocumentServiceConnector.ReplaceCommunityAdress(fileUri);

            var uriTuple = await DocumentServiceConnector.GetConvertedUriAsync(fileUri, file.ConvertedExtension, toExtension, docKey, password, null, null, false);
            var convertUri = uriTuple.ConvertedDocumentUri;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(convertUri);

            using var httpClient = ClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            return new ResponseStream(response);
        }

        public async Task<FileOperationResult> ExecSynchronouslyAsync<T>(File<T> file, string doc)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var fileSecurity = FileSecurity;
            if (!await fileSecurity.CanReadAsync(file))
            {
                (var readLink, file) = await FileShareLink.CheckAsync(doc, true, fileDao);
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
                }
                if (!readLink)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
                }
            }

            var fileUri = PathProvider.GetFileStreamUrl(file);
            var fileExtension = file.ConvertedExtension;
            var toExtension = FileUtility.GetInternalExtension(file.Title);
            var docKey = DocumentServiceHelper.GetDocKey(file);

            fileUri = DocumentServiceConnector.ReplaceCommunityAdress(fileUri);

            var uriTuple = await DocumentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, null, null, false);
            var convertUri = uriTuple.ConvertedDocumentUri;

            var operationResult = new ConvertFileOperationResult
            {
                Source = string.Format("{{\"id\":\"{0}\", \"version\":\"{1}\"}}", file.ID, file.Version),
                OperationType = FileOperationType.Convert,
                Error = string.Empty,
                Progress = 0,
                Result = string.Empty,
                Processed = "",
                Id = string.Empty,
                TenantId = TenantManager.GetCurrentTenant().TenantId,
                Account = AuthContext.CurrentAccount,
                Delete = false,
                StartDateTime = DateTime.Now,
                Url = HttpContextAccesor?.HttpContext != null ? HttpContextAccesor.HttpContext.Request.GetUrlRewriter().ToString() : null,
                Password = null,
                ServerRootPath = BaseCommonLinkUtility.ServerRootPath
            };

            var operationResultError = string.Empty;

            var newFile = await SaveConvertedFileAsync(file, convertUri);
            if (newFile != null)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var folder = await folderDao.GetFolderAsync(newFile.FolderID);
                var folderTitle = await fileSecurity.CanReadAsync(folder) ? folder.Title : null;
                operationResult.Result = await GetFileConverter<T>().FileJsonSerializerAsync(EntryStatusManager, newFile, folderTitle);
            }

            operationResult.Progress = 100;
            operationResult.StopDateTime = DateTime.UtcNow;
            operationResult.Processed = "1";

            if (!string.IsNullOrEmpty(operationResultError))
            {
                operationResult.Error = operationResultError;
            }

            return operationResult;
        }

        public async Task ExecAsynchronouslyAsync<T>(File<T> file, bool deleteAfter, string password = null)
        {
            if (!MustConvert(file))
            {
                throw new ArgumentException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
            }
            if (!string.IsNullOrEmpty(file.ConvertedType) || FileUtility.InternalExtension.Values.Contains(FileUtility.GetFileExtension(file.Title)))
            {
                return;
            }

            await FileMarker.RemoveMarkAsNewAsync(file);
            GetFileConverter<T>().Add(file, password, TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount, deleteAfter, HttpContextAccesor?.HttpContext != null ? HttpContextAccesor.HttpContext.Request.GetUrlRewriter().ToString() : null, BaseCommonLinkUtility.ServerRootPath);
        }

        public bool IsConverting<T>(File<T> file)
        {
            if (!MustConvert(file) || !string.IsNullOrEmpty(file.ConvertedType))
            {
                return false;
            }

            return GetFileConverter<T>().IsConverting(file);
        }

        public async IAsyncEnumerable<FileOperationResult> GetStatusAsync<T>(IEnumerable<KeyValuePair<File<T>, bool>> filesPair)
        {
            var result = new List<FileOperationResult>();
            foreach (var pair in filesPair)
            {
                var r = await GetFileConverter<T>().GetStatusAsync(pair, FileSecurity);

                if (r != null)
                {
                    yield return r;
                }
            }
        }

        public async Task<File<T>> SaveConvertedFileAsync<T>(File<T> file, string convertedFileUrl)
        {
            var fileSecurity = FileSecurity;
            var fileDao = DaoFactory.GetFileDao<T>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            File<T> newFile = null;
            var markAsTemplate = false;
            var newFileTitle = FileUtility.ReplaceFileExtension(file.Title, FileUtility.GetInternalExtension(file.Title));

            if (!FilesSettingsHelper.StoreOriginalFiles && await fileSecurity.CanEditAsync(file))
            {
                newFile = (File<T>)file.Clone();
                newFile.Version++;
                markAsTemplate = FileUtility.ExtsTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase)
                              && FileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(newFileTitle), StringComparer.CurrentCultureIgnoreCase);
            }
            else
            {
                var folderId = GlobalFolderHelper.GetFolderMy<T>();

                var parent = await folderDao.GetFolderAsync(file.FolderID);
                if (parent != null
                    && await fileSecurity.CanCreateAsync(parent))
                {
                    folderId = parent.ID;
                }

                if (Equals(folderId, 0)) throw new SecurityException(FilesCommonResource.ErrorMassage_FolderNotFound);

                if (FilesSettingsHelper.UpdateIfExist && (parent != null && !folderId.Equals(parent.ID) || !file.ProviderEntry))
                {
                    newFile = await fileDao.GetFileAsync(folderId, newFileTitle);
                    if (newFile != null && await fileSecurity.CanEditAsync(newFile) && !await EntryManager.FileLockedForMeAsync(newFile.ID) && !FileTracker.IsEditing(newFile.ID))
                    {
                        newFile.Version++;
                    }
                    else
                    {
                        newFile = null;
                    }
                }

                if (newFile == null)
                {
                    newFile = ServiceProvider.GetService<File<T>>();
                    newFile.FolderID = folderId;
                }
            }

            newFile.Title = newFileTitle;
            newFile.ConvertedType = null;
            newFile.Comment = string.Format(FilesCommonResource.CommentConvert, file.Title);
            newFile.ThumbnailStatus = Thumbnail.Waiting;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(convertedFileUrl);

            var httpClient = ClientFactory.CreateClient();

            try
            {
                using var response = await httpClient.SendAsync(request);
                using var convertedFileStream = new ResponseStream(response);
                newFile.ContentLength = convertedFileStream.Length;
                newFile = await fileDao.SaveFileAsync(newFile, convertedFileStream);
            }
            catch (HttpRequestException e)
            {
                var errorString = $"HttpRequestException: {e.StatusCode}";

                if (e.StatusCode != HttpStatusCode.NotFound)
                {
                    if (e.Message != null)
                    {
                        errorString += $" Error message: {e.Message}";
                    }
                }

                throw new Exception(errorString);
            }

            FilesMessageService.Send(newFile, MessageInitiator.DocsService, MessageAction.FileConverted, newFile.Title);

            var linkDao = DaoFactory.GetLinkDao();
            await linkDao.DeleteAllLinkAsync(file.ID.ToString());

            await FileMarker.MarkAsNewAsync(newFile);

            var tagDao = DaoFactory.GetTagDao<T>();
            var tags = await tagDao.GetTagsAsync(file.ID, FileEntryType.File, TagType.System).ToListAsync();
            if (tags.Count > 0)
            {
                tags.ForEach(r => r.EntryId = newFile.ID);
                tagDao.SaveTags(tags);
            }

            if (markAsTemplate)
            {
                tagDao.SaveTags(Tag.Template(AuthContext.CurrentAccount.ID, newFile));
            }

            return newFile;
        }

        private FileConverterQueue<T> GetFileConverter<T>()
        {
            return ServiceProvider.GetService<FileConverterQueue<T>>();
        }
    }

    internal class FileComparer<T> : IEqualityComparer<File<T>>
    {
        public bool Equals(File<T> x, File<T> y)
        {
            return x != null && y != null && Equals(x.ID, y.ID) && x.Version == y.Version;
        }

        public int GetHashCode(File<T> obj)
        {
            return obj.ID.GetHashCode() + obj.Version.GetHashCode();
        }
    }

    internal class ConvertFileOperationResult : FileOperationResult
    {
        public DateTime StartDateTime { get; set; }
        public DateTime StopDateTime { get; set; }
        public int TenantId { get; set; }
        public IAccount Account { get; set; }
        public bool Delete { get; set; }
        public string Url { get; set; }
        public string Password { get; set; }

        //hack for download
        public string ServerRootPath { get; set; }
    }

    public static class FileConverterQueueExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FileConverterQueueScope>();
        }
    }

    public static class FileConverterExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FileConverterQueue<int>>();
            services.TryAdd<FileConverterQueue<string>>();
        }
    }
}