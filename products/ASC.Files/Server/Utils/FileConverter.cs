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
using System.Security;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Security;
using ASC.Files.Resources;
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
    internal class FileConverterQueue<T>
    {
        private readonly object singleThread = new object();
        private readonly IDictionary<File<T>, ConvertFileOperationResult> conversionQueue;
        private readonly Timer timer;
        private readonly object locker;
        private readonly ICache cache;
        private const int TIMER_PERIOD = 500;

        public IServiceProvider ServiceProvider { get; }

        public FileConverterQueue(IServiceProvider ServiceProvider)
        {
            conversionQueue = new Dictionary<File<T>, ConvertFileOperationResult>(new FileComparer<T>());
            timer = new Timer(CheckConvertFilesStatus, null, 0, Timeout.Infinite);
            locker = new object();
            this.ServiceProvider = ServiceProvider;
            cache = AscCache.Memory;
        }

        public void Add(File<T> file, string password, int tenantId, IAccount account, bool deleteAfter, string url)
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
                    Password = password
                };
                conversionQueue.Add(file, queueResult);
                cache.Insert(GetKey(file), queueResult, TimeSpan.FromMinutes(10));

                timer.Change(0, Timeout.Infinite);
            }
        }

        public ConvertFileOperationResult GetStatus(KeyValuePair<File<T>, bool> pair, FileSecurity fileSecurity)
        {
            var file = pair.Key;
            var key = GetKey(file);
            var operation = cache.Get<ConvertFileOperationResult>(key);
            if (operation != null && (pair.Value || fileSecurity.CanRead(file)))
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
                var logger = scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                var userManager = scope.ServiceProvider.GetService<UserManager>();
                var securityContext = scope.ServiceProvider.GetService<SecurityContext>();
                var daoFactory = scope.ServiceProvider.GetService<IDaoFactory>();
                var FileSecurity = scope.ServiceProvider.GetService<FileSecurity>();
                var PathProvider = scope.ServiceProvider.GetService<PathProvider>();
                var SetupInfo = scope.ServiceProvider.GetService<SetupInfo>();
                var FileUtility = scope.ServiceProvider.GetService<FileUtility>();
                var DocumentServiceHelper = scope.ServiceProvider.GetService<DocumentServiceHelper>();
                var DocumentServiceConnector = scope.ServiceProvider.GetService<DocumentServiceConnector>();
                var EntryManager = scope.ServiceProvider.GetService<EntryManager>();
                var FileConverter = scope.ServiceProvider.GetService<FileConverter>();

                try
                {
                    var filesIsConverting = new List<File<T>>();
                    lock (locker)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);

                        conversionQueue.Where(x => !string.IsNullOrEmpty(x.Value.Processed)
                                                   && (x.Value.Progress == 100 && DateTime.UtcNow - x.Value.StopDateTime > TimeSpan.FromMinutes(1) ||
                                                       DateTime.UtcNow - x.Value.StopDateTime > TimeSpan.FromMinutes(10)))
                                       .ToList()
                                       .ForEach(x =>
                                       {
                                           conversionQueue.Remove(x);
                                           cache.Remove(GetKey(x.Key));
                                       });

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

                    var fileSecurity = FileSecurity;
                    foreach (var file in filesIsConverting)
                    {
                        var fileUri = file.ID.ToString();
                        string convertedFileUrl;
                        int operationResultProgress;

                        try
                        {
                            int tenantId;
                            IAccount account;
                            string password;

                            lock (locker)
                            {
                                if (!conversionQueue.Keys.Contains(file)) continue;

                                var operationResult = conversionQueue[file];
                                if (!string.IsNullOrEmpty(operationResult.Processed)) continue;

                                operationResult.Processed = "1";
                                tenantId = operationResult.TenantId;
                                account = operationResult.Account;
                                password = operationResult.Password;

                                //if (HttpContext.Current == null && !WorkContext.IsMono)
                                //{
                                //    HttpContext.Current = new HttpContext(
                                //        new HttpRequest("hack", operationResult.Url, string.Empty),
                                //        new HttpResponse(new StringWriter()));
                                //}

                                cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                            }

                            tenantManager.SetCurrentTenant(tenantId);
                            securityContext.AuthenticateMe(account);

                            var user = userManager.GetUsers(account.ID);
                            var culture = string.IsNullOrEmpty(user.CultureName) ? tenantManager.GetCurrentTenant().GetCulture() : CultureInfo.GetCultureInfo(user.CultureName);
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = culture;

                            if (!fileSecurity.CanRead(file) && file.RootFolderType != FolderType.BUNCH)
                            {
                                //No rights in CRM after upload before attach
                                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
                            }
                            if (file.ContentLength > SetupInfo.AvailableFileSize)
                            {
                                throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize)));
                            }

                            fileUri = PathProvider.GetFileStreamUrl(file);

                            var toExtension = FileUtility.GetInternalExtension(file.Title);
                            var fileExtension = file.ConvertedExtension;
                            var docKey = DocumentServiceHelper.GetDocKey(file);

                            fileUri = DocumentServiceConnector.ReplaceCommunityAdress(fileUri);
                            operationResultProgress = DocumentServiceConnector.GetConvertedUri(fileUri, fileExtension, toExtension, docKey, password, true, out convertedFileUrl);
                        }
                        catch (Exception exception)
                        {
                            var password = exception.InnerException != null
                                           && (exception.InnerException is DocumentService.DocumentServiceException documentServiceException)
                                           && documentServiceException.Code == DocumentService.DocumentServiceException.ErrorCode.ConvertPassword;

                            logger.Error(string.Format("Error convert {0} with url {1}", file.ID, fileUri), exception);
                            lock (locker)
                            {
                                if (conversionQueue.Keys.Contains(file))
                                {
                                    var operationResult = conversionQueue[file];
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
                                if (conversionQueue.Keys.Contains(file))
                                {
                                    var operationResult = conversionQueue[file];

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
                            newFile = FileConverter.SaveConvertedFile(file, convertedFileUrl);
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
                                if (conversionQueue.Keys.Contains(file))
                                {
                                    var operationResult = conversionQueue[file];
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
                                            var folder = folderDao.GetFolder(newFile.FolderID);
                                            var folderTitle = fileSecurity.CanRead(folder) ? folder.Title : null;
                                            operationResult.Result = FileJsonSerializer(EntryManager, newFile, folderTitle);
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

        private string FileJsonSerializer(EntryManager EntryManager, File<T> file, string folderTitle)
        {
            if (file == null) return string.Empty;

            EntryManager.SetFileStatus(file);
            return
                string.Format("{{ \"id\": \"{0}\"," +
                              " \"title\": \"{1}\"," +
                              " \"version\": \"{2}\"," +
                              " \"folderId\": \"{3}\"," +
                              " \"folderTitle\": \"{4}\"," +
                              " \"fileXml\": \"{5}\" }}",
                              file.ID,
                              file.Title,
                              file.Version,
                              file.FolderID,
                              folderTitle ?? "",
                              File<T>.Serialize(file).Replace('"', '\''));
        }
    }

    public class FileConverter
    {
        public FileUtility FileUtility { get; }
        public FilesLinkUtility FilesLinkUtility { get; }
        public IDaoFactory DaoFactory { get; }
        public SetupInfo SetupInfo { get; }
        public PathProvider PathProvider { get; }
        public FileSecurity FileSecurity { get; }
        public FileMarker FileMarker { get; }
        public TenantManager TenantManager { get; }
        public AuthContext AuthContext { get; }
        public EntryManager EntryManager { get; }
        public FilesSettingsHelper FilesSettingsHelper { get; }
        public GlobalFolderHelper GlobalFolderHelper { get; }
        public FilesMessageService FilesMessageService { get; }
        public FileShareLink FileShareLink { get; }
        public DocumentServiceHelper DocumentServiceHelper { get; }
        public DocumentServiceConnector DocumentServiceConnector { get; }
        public IServiceProvider ServiceProvider { get; }
        public IHttpContextAccessor HttpContextAccesor { get; }

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
            IOptionsMonitor<ILog> options,
            FilesSettingsHelper filesSettingsHelper,
            GlobalFolderHelper globalFolderHelper,
            FilesMessageService filesMessageService,
            FileShareLink fileShareLink,
            DocumentServiceHelper documentServiceHelper,
            DocumentServiceConnector documentServiceConnector,
            IServiceProvider serviceProvider)
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
            ServiceProvider = serviceProvider;
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
            IOptionsMonitor<ILog> options,
            FilesSettingsHelper filesSettingsHelper,
            GlobalFolderHelper globalFolderHelper,
            FilesMessageService filesMessageService,
            FileShareLink fileShareLink,
            DocumentServiceHelper documentServiceHelper,
            DocumentServiceConnector documentServiceConnector,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccesor)
            : this(fileUtility, filesLinkUtility, daoFactory, setupInfo, pathProvider, fileSecurity,
                  fileMarker, tenantManager, authContext, entryManager, options, filesSettingsHelper,
                  globalFolderHelper, filesMessageService, fileShareLink, documentServiceHelper, documentServiceConnector,
                  serviceProvider)
        {
            HttpContextAccesor = httpContextAccesor;
        }

        public bool EnableAsUploaded
        {
            get { return FileUtility.ExtsMustConvert.Any() && !string.IsNullOrEmpty(FilesLinkUtility.DocServiceConverterUrl); }
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

        public Stream Exec<T>(File<T> file)
        {
            return Exec(file, FileUtility.GetInternalExtension(file.Title));
        }

        public Stream Exec<T>(File<T> file, string toExtension)
        {
            if (!EnableConvert(file, toExtension))
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                return fileDao.GetFileStream(file);
            }

            if (file.ContentLength > SetupInfo.AvailableFileSize)
            {
                throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize)));
            }

            var fileUri = PathProvider.GetFileStreamUrl(file);
            var docKey = DocumentServiceHelper.GetDocKey(file);
            fileUri = DocumentServiceConnector.ReplaceCommunityAdress(fileUri);
            DocumentServiceConnector.GetConvertedUri(fileUri, file.ConvertedExtension, toExtension, docKey, null, false, out var convertUri);

            if (WorkContext.IsMono && ServicePointManager.ServerCertificateValidationCallback == null)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, c, n, p) => true; //HACK: http://ubuntuforums.org/showthread.php?t=1841740
            }
            return new ResponseStream(((HttpWebRequest)WebRequest.Create(convertUri)).GetResponse());
        }

        public File<T> ExecSync<T>(File<T> file, string doc)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var fileSecurity = FileSecurity;
            if (!fileSecurity.CanRead(file))
            {
                var readLink = FileShareLink.Check(doc, true, fileDao, out file);
                if (file == null)
                {
                    throw new ArgumentNullException("file", FilesCommonResource.ErrorMassage_FileNotFound);
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
            DocumentServiceConnector.GetConvertedUri(fileUri, fileExtension, toExtension, docKey, null, false, out var convertUri);

            return SaveConvertedFile(file, convertUri);
        }

        public void ExecAsync<T>(File<T> file, bool deleteAfter, string password = null)
        {
            if (!MustConvert(file))
            {
                throw new ArgumentException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
            }
            if (!string.IsNullOrEmpty(file.ConvertedType) || FileUtility.InternalExtension.Values.Contains(FileUtility.GetFileExtension(file.Title)))
            {
                return;
            }

            FileMarker.RemoveMarkAsNew(file);
            GetFileConverter<T>().Add(file, password, TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount, deleteAfter, HttpContextAccesor?.HttpContext != null ? HttpContextAccesor.HttpContext.Request.GetUrlRewriter().ToString() : null);
        }

        public bool IsConverting<T>(File<T> file)
        {
            if (!MustConvert(file) || !string.IsNullOrEmpty(file.ConvertedType))
            {
                return false;
            }

            return GetFileConverter<T>().IsConverting(file);
        }

        public IEnumerable<FileOperationResult> GetStatus<T>(IEnumerable<KeyValuePair<File<T>, bool>> filesPair)
        {
            var result = new List<FileOperationResult>();
            foreach (var pair in filesPair)
            {
                var r = GetFileConverter<T>().GetStatus(pair, FileSecurity);

                if (r != null)
                {
                    result.Add(r);
                }
            }
            return result;
        }

        public File<T> SaveConvertedFile<T>(File<T> file, string convertedFileUrl)
        {
            var fileSecurity = FileSecurity;
            var fileDao = DaoFactory.GetFileDao<T>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            File<T> newFile = null;
            var newFileTitle = FileUtility.ReplaceFileExtension(file.Title, FileUtility.GetInternalExtension(file.Title));

            if (!FilesSettingsHelper.StoreOriginalFiles && fileSecurity.CanEdit(file))
            {
                newFile = (File<T>)file.Clone();
                newFile.Version++;
            }
            else
            {
                var folderId = GlobalFolderHelper.GetFolderMy<T>();

                var parent = folderDao.GetFolder(file.FolderID);
                if (parent != null
                    && fileSecurity.CanCreate(parent))
                {
                    folderId = parent.ID;
                }

                if (Equals(folderId, 0)) throw new SecurityException(FilesCommonResource.ErrorMassage_FolderNotFound);

                if (FilesSettingsHelper.UpdateIfExist && (parent != null && !folderId.Equals(parent.ID) || !file.ProviderEntry))
                {
                    newFile = fileDao.GetFile(folderId, newFileTitle);
                    if (newFile != null && fileSecurity.CanEdit(newFile) && !EntryManager.FileLockedForMe(newFile.ID) && !FileTracker.IsEditing(newFile.ID))
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

            var req = (HttpWebRequest)WebRequest.Create(convertedFileUrl);

            if (WorkContext.IsMono && ServicePointManager.ServerCertificateValidationCallback == null)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, c, n, p) => true; //HACK: http://ubuntuforums.org/showthread.php?t=1841740
            }

            try
            {
                using (var convertedFileStream = new ResponseStream(req.GetResponse()))
                {
                    newFile.ContentLength = convertedFileStream.Length;
                    newFile = fileDao.SaveFile(newFile, convertedFileStream);
                }
            }
            catch (WebException e)
            {
                using var response = e.Response;
                var httpResponse = (HttpWebResponse)response;
                var errorString = string.Format("WebException: {0}", httpResponse.StatusCode);

                if (httpResponse.StatusCode != HttpStatusCode.NotFound)
                {
                    using var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        using var readStream = new StreamReader(responseStream);
                        var text = readStream.ReadToEnd();
                        errorString += string.Format(" Error message: {0}", text);
                    }
                }

                throw new Exception(errorString);
            }

            FilesMessageService.Send(newFile, MessageInitiator.DocsService, MessageAction.FileConverted, newFile.Title);
            FileMarker.MarkAsNew(newFile);

            var tagDao = DaoFactory.GetTagDao<T>();
            var tags = tagDao.GetTags(file.ID, FileEntryType.File, TagType.System).ToList();
            if (tags.Any())
            {
                tags.ForEach(r => r.EntryId = newFile.ID);
                tagDao.SaveTags(tags);
            }

            return newFile;
        }

        private FileConverterQueue<T> GetFileConverter<T>() => ServiceProvider.GetService<FileConverterQueue<T>>();
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
    }

    public static class FileConverterExtension
    {
        public static DIHelper AddFileConverterService(this DIHelper services)
        {
            services.TryAddScoped<FileConverter>();
            services.TryAddSingleton<FileConverterQueue<string>>();
            services.TryAddSingleton<FileConverterQueue<int>>();
            return services
                .AddFilesLinkUtilityService()
                .AddFileUtilityService()
                .AddDaoFactoryService()
                .AddSetupInfo()
                .AddPathProviderService()
                .AddFileSecurityService()
                .AddFileMarkerService()
                .AddTenantManagerService()
                .AddAuthContextService()
                .AddEntryManagerService()
                .AddFilesSettingsHelperService()
                .AddGlobalFolderHelperService()
                .AddFilesMessageService()
                .AddFileShareLinkService()
                .AddDocumentServiceHelperService()
                .AddDocumentServiceConnectorService();
        }
    }
}