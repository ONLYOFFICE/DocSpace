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

using Timeout = System.Threading.Timeout;

namespace ASC.Web.Files.Utils;

[Singletone(Additional = typeof(FileConverterQueueExtension))]
internal class FileConverterQueue<T> : IDisposable
{
    private readonly object _singleThread = new object();
    private readonly IDictionary<File<T>, ConvertFileOperationResult> _conversionQueue;
    private readonly Timer _timer;
    private readonly object _locker;
    private readonly ICache _cache;
    private const int TimerPeriod = 500;

    private readonly IServiceProvider _serviceProvider;

    public FileConverterQueue(IServiceProvider ServiceProvider, ICache cache)
    {
        _conversionQueue = new Dictionary<File<T>, ConvertFileOperationResult>(new FileComparer<T>());
        _timer = new Timer(CheckConvertFilesStatus, null, 0, Timeout.Infinite);
        _locker = new object();
        _serviceProvider = ServiceProvider;
        _cache = cache;
    }

    public void Add(File<T> file, string password, int tenantId, IAccount account, bool deleteAfter, string url, string serverRootPath)
    {
        lock (_locker)
        {
            if (_conversionQueue.ContainsKey(file))
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
            _conversionQueue.Add(file, queueResult);
            _cache.Insert(GetKey(file), queueResult, TimeSpan.FromMinutes(10));

            _timer.Change(0, Timeout.Infinite);
        }
    }

    public async Task<ConvertFileOperationResult> GetStatusAsync(KeyValuePair<File<T>, bool> pair, FileSecurity fileSecurity)
    {
        var file = pair.Key;
        var key = GetKey(file);
        var operation = _cache.Get<ConvertFileOperationResult>(key);
        if (operation != null && (pair.Value || await fileSecurity.CanReadAsync(file)))
        {
            lock (_locker)
            {
                if (operation.Progress == 100)
                {
                    _conversionQueue.Remove(file);
                    _cache.Remove(key);
                }

                return operation;
            }
        }

        return null;
    }

    public bool IsConverting(File<T> file)
    {
        var result = _cache.Get<ConvertFileOperationResult>(GetKey(file));

        return result != null && result.Progress != 100 && string.IsNullOrEmpty(result.Error);
    }

    private void CheckConvertFilesStatus(object _)
    {
        if (Monitor.TryEnter(_singleThread))
        {
            using var scope = _serviceProvider.CreateScope();
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
                lock (_locker)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);

                    var queues = _conversionQueue.Where(x => !string.IsNullOrEmpty(x.Value.Processed)
                                               && (x.Value.Progress == 100 && DateTime.UtcNow - x.Value.StopDateTime > TimeSpan.FromMinutes(1) ||
                                                   DateTime.UtcNow - x.Value.StopDateTime > TimeSpan.FromMinutes(10)))
                        .ToList();

                    foreach (var q in queues)
                    {
                        _conversionQueue.Remove(q);
                        _cache.Remove(GetKey(q.Key));
                    }

                    logger.DebugFormat("Run CheckConvertFilesStatus: count {0}", _conversionQueue.Count);

                    if (_conversionQueue.Count == 0)
                    {
                        return;
                    }

                    filesIsConverting = _conversionQueue
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

                        lock (_locker)
                        {
                            if (!_conversionQueue.Keys.Contains(file))
                            {
                                continue;
                            }

                            var operationResult = _conversionQueue[file];
                            if (!string.IsNullOrEmpty(operationResult.Processed))
                            {
                                continue;
                            }

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

                            _cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
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
                        lock (_locker)
                        {
                            if (_conversionQueue.TryGetValue(file, out var operationResult))
                            {
                                if (operationResult.Delete)
                                {
                                    _conversionQueue.Remove(file);
                                    _cache.Remove(GetKey(file));
                                }
                                else
                                {
                                    operationResult.Progress = 100;
                                    operationResult.StopDateTime = DateTime.UtcNow;
                                    operationResult.Error = exception.Message;
                                    if (password) operationResult.Result = "password";
                                    _cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                                }
                            }
                        }

                        continue;
                    }

                    operationResultProgress = Math.Min(operationResultProgress, 100);
                    if (operationResultProgress < 100)
                    {
                        lock (_locker)
                        {
                            if (_conversionQueue.TryGetValue(file, out var operationResult))
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
                                _cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
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
                        lock (_locker)
                        {
                            if (_conversionQueue.TryGetValue(file, out var operationResult))
                            {
                                if (operationResult.Delete)
                                {
                                    _conversionQueue.Remove(file);
                                    _cache.Remove(GetKey(file));
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

                                    _cache.Insert(GetKey(file), operationResult, TimeSpan.FromMinutes(10));
                                }
                            }
                        }
                    }

                    logger.Debug("CheckConvertFilesStatus iteration end");
                }

                lock (_locker)
                {
                    _timer.Change(TimerPeriod, TimerPeriod);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message, exception);
                lock (_locker)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
            finally
            {
                Monitor.Exit(_singleThread);
            }
        }
    }

    private string GetKey(File<T> f)
    {
        return string.Format("fileConvertation-{0}", f.ID);
    }

    internal async Task<string> FileJsonSerializerAsync(EntryStatusManager EntryManager, File<T> file, string folderTitle)
    {
        if (file == null)
        {
            return string.Empty;
        }

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
        if (_timer != null)
        {
            _timer.Dispose();
        }
    }
}

[Scope]
public class FileConverterQueueScope
{
    private readonly IOptionsMonitor<ILog> _options;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly PathProvider _pathProvider;
    private readonly SetupInfo _setupInfo;
    private readonly FileUtility _fileUtility;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly EntryStatusManager _entryManager;
    private readonly FileConverter _fileConverter;

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
        _options = options;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _securityContext = securityContext;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _pathProvider = pathProvider;
        _setupInfo = setupInfo;
        _fileUtility = fileUtility;
        _documentServiceHelper = documentServiceHelper;
        _documentServiceConnector = documentServiceConnector;
        _entryManager = entryManager;
        _fileConverter = fileConverter;
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
        optionsMonitor = _options;
        tenantManager = _tenantManager;
        userManager = _userManager;
        securityContext = _securityContext;
        daoFactory = _daoFactory;
        fileSecurity = _fileSecurity;
        pathProvider = _pathProvider;
        setupInfo = _setupInfo;
        fileUtility = _fileUtility;
        documentServiceHelper = _documentServiceHelper;
        documentServiceConnector = _documentServiceConnector;
        entryManager = _entryManager;
        fileConverter = _fileConverter;
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
    private readonly FileUtility _fileUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IDaoFactory _daoFactory;
    private readonly SetupInfo _setupInfo;
    private readonly PathProvider _pathProvider;
    private readonly FileSecurity _fileSecurity;
    private readonly FileMarker _fileMarker;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly EntryManager _entryManager;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesMessageService _filesMessageService;
    private readonly FileShareLink _fileShareLink;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly FileTrackerHelper _fileTracker;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly EntryStatusManager _entryStatusManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccesor;
    private readonly IHttpClientFactory _clientFactory;

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
        _fileUtility = fileUtility;
        _filesLinkUtility = filesLinkUtility;
        _daoFactory = daoFactory;
        _setupInfo = setupInfo;
        _pathProvider = pathProvider;
        _fileSecurity = fileSecurity;
        _fileMarker = fileMarker;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _entryManager = entryManager;
        _filesSettingsHelper = filesSettingsHelper;
        _globalFolderHelper = globalFolderHelper;
        _filesMessageService = filesMessageService;
        _fileShareLink = fileShareLink;
        _documentServiceHelper = documentServiceHelper;
        _documentServiceConnector = documentServiceConnector;
        _fileTracker = fileTracker;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _entryStatusManager = entryStatusManager;
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
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
        _httpContextAccesor = httpContextAccesor;
    }

    public bool EnableAsUploaded => _fileUtility.ExtsMustConvert.Count > 0 && !string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl);

    public bool MustConvert<T>(File<T> file)
    {
        if (file == null)
        {
            return false;
        }

        var ext = FileUtility.GetFileExtension(file.Title);

        return _fileUtility.ExtsMustConvert.Contains(ext);
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
        if (_fileUtility.InternalExtension.Values.Contains(toExtension))
        {
            return true;
        }

        return _fileUtility.ExtsConvertible.Keys.Contains(fileExtension) && _fileUtility.ExtsConvertible[fileExtension].Contains(toExtension);
    }

    public Task<Stream> ExecAsync<T>(File<T> file)
    {
        return ExecAsync(file, _fileUtility.GetInternalExtension(file.Title));
    }

    public async Task<Stream> ExecAsync<T>(File<T> file, string toExtension, string password = null)
    {
        if (!EnableConvert(file, toExtension))
        {
            var fileDao = _daoFactory.GetFileDao<T>();

            return await fileDao.GetFileStreamAsync(file);
        }

        if (file.ContentLength > _setupInfo.AvailableFileSize)
        {
            throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(_setupInfo.AvailableFileSize)));
        }

        var fileUri = _pathProvider.GetFileStreamUrl(file);
        var docKey = _documentServiceHelper.GetDocKey(file);
        fileUri = _documentServiceConnector.ReplaceCommunityAdress(fileUri);

        var uriTuple = await _documentServiceConnector.GetConvertedUriAsync(fileUri, file.ConvertedExtension, toExtension, docKey, password, null, null, false);
        var convertUri = uriTuple.ConvertedDocumentUri;
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(convertUri);

        using var httpClient = _clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);

        return new ResponseStream(response);
    }

    public async Task<FileOperationResult> ExecSynchronouslyAsync<T>(File<T> file, string doc)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var fileSecurity = _fileSecurity;
        if (!await fileSecurity.CanReadAsync(file))
        {
            (var readLink, file) = await _fileShareLink.CheckAsync(doc, true, fileDao);
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            }
            if (!readLink)
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            }
        }

        var fileUri = _pathProvider.GetFileStreamUrl(file);
        var fileExtension = file.ConvertedExtension;
        var toExtension = _fileUtility.GetInternalExtension(file.Title);
        var docKey = _documentServiceHelper.GetDocKey(file);

        fileUri = _documentServiceConnector.ReplaceCommunityAdress(fileUri);

        var uriTuple = await _documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, null, null, false);
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
            TenantId = _tenantManager.GetCurrentTenant().TenantId,
            Account = _authContext.CurrentAccount,
            Delete = false,
            StartDateTime = DateTime.Now,
            Url = _httpContextAccesor?.HttpContext != null ? _httpContextAccesor.HttpContext.Request.GetUrlRewriter().ToString() : null,
            Password = null,
            ServerRootPath = _baseCommonLinkUtility.ServerRootPath
        };

        var operationResultError = string.Empty;

        var newFile = await SaveConvertedFileAsync(file, convertUri);
        if (newFile != null)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var folder = await folderDao.GetFolderAsync(newFile.FolderID);
            var folderTitle = await fileSecurity.CanReadAsync(folder) ? folder.Title : null;
            operationResult.Result = await GetFileConverter<T>().FileJsonSerializerAsync(_entryStatusManager, newFile, folderTitle);
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
        if (!string.IsNullOrEmpty(file.ConvertedType) || _fileUtility.InternalExtension.Values.Contains(FileUtility.GetFileExtension(file.Title)))
        {
            return;
        }

        await _fileMarker.RemoveMarkAsNewAsync(file);
        GetFileConverter<T>().Add(file, password, _tenantManager.GetCurrentTenant().TenantId, _authContext.CurrentAccount, deleteAfter, _httpContextAccesor?.HttpContext != null ? _httpContextAccesor.HttpContext.Request.GetUrlRewriter().ToString() : null, _baseCommonLinkUtility.ServerRootPath);
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
            var r = await GetFileConverter<T>().GetStatusAsync(pair, _fileSecurity);

            if (r != null)
            {
                yield return r;
            }
        }
    }

    public async Task<File<T>> SaveConvertedFileAsync<T>(File<T> file, string convertedFileUrl)
    {
        var fileSecurity = _fileSecurity;
        var fileDao = _daoFactory.GetFileDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        File<T> newFile = null;
        var markAsTemplate = false;
        var newFileTitle = FileUtility.ReplaceFileExtension(file.Title, _fileUtility.GetInternalExtension(file.Title));

        if (!_filesSettingsHelper.StoreOriginalFiles && await fileSecurity.CanEditAsync(file))
        {
            newFile = (File<T>)file.Clone();
            newFile.Version++;
            markAsTemplate = FileUtility.ExtsTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase)
                          && _fileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(newFileTitle), StringComparer.CurrentCultureIgnoreCase);
        }
        else
        {
            var folderId = _globalFolderHelper.GetFolderMy<T>();

            var parent = await folderDao.GetFolderAsync(file.FolderID);
            if (parent != null
                && await fileSecurity.CanCreateAsync(parent))
            {
                folderId = parent.ID;
            }

            if (Equals(folderId, 0))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_FolderNotFound);
            }

            if (_filesSettingsHelper.UpdateIfExist && (parent != null && !folderId.Equals(parent.ID) || !file.ProviderEntry))
            {
                newFile = await fileDao.GetFileAsync(folderId, newFileTitle);
                if (newFile != null && await fileSecurity.CanEditAsync(newFile) && !await _entryManager.FileLockedForMeAsync(newFile.ID) && !_fileTracker.IsEditing(newFile.ID))
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
                newFile = _serviceProvider.GetService<File<T>>();
                newFile.FolderID = folderId;
            }
        }

        newFile.Title = newFileTitle;
        newFile.ConvertedType = null;
        newFile.Comment = string.Format(FilesCommonResource.CommentConvert, file.Title);
        newFile.ThumbnailStatus = Thumbnail.Waiting;

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(convertedFileUrl);

        var httpClient = _clientFactory.CreateClient();

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

        _filesMessageService.Send(newFile, MessageInitiator.DocsService, MessageAction.FileConverted, newFile.Title);

        var linkDao = _daoFactory.GetLinkDao();
        await linkDao.DeleteAllLinkAsync(file.ID.ToString());

        await _fileMarker.MarkAsNewAsync(newFile);

        var tagDao = _daoFactory.GetTagDao<T>();
        var tags = await tagDao.GetTagsAsync(file.ID, FileEntryType.File, TagType.System).ToListAsync();
        if (tags.Count > 0)
        {
            tags.ForEach(r => r.EntryId = newFile.ID);
            tagDao.SaveTags(tags);
        }

        if (markAsTemplate)
        {
            tagDao.SaveTags(Tag.Template(_authContext.CurrentAccount.ID, newFile));
        }

        return newFile;
    }

    private FileConverterQueue<T> GetFileConverter<T>()
    {
        return _serviceProvider.GetService<FileConverterQueue<T>>();
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
