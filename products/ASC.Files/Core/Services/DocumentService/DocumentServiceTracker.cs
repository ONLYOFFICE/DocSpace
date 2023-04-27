// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Files.Services.DocumentService;

public class DocumentServiceTracker
{
    #region Class

    public enum TrackerStatus
    {
        NotFound = 0,
        Editing = 1,
        MustSave = 2,
        Corrupted = 3,
        Closed = 4,
        MailMerge = 5,
        ForceSave = 6,
        CorruptedForceSave = 7,
    }

    [DebuggerDisplay("{Status} - {Key}")]
    public class TrackerData
    {
        public List<Action> Actions { get; set; }
        public string ChangesUrl { get; set; }
        public string Filetype { get; set; }
        public ForceSaveInitiator ForceSaveType { get; set; }
        public object History { get; set; }
        public string Key { get; set; }
        public MailMergeData MailMerge { get; set; }
        public TrackerStatus Status { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public List<string> Users { get; set; }
        public string UserData { get; set; }
        public bool Encrypted { get; set; }

        [DebuggerDisplay("{Type} - {UserId}")]
        public class Action
        {
            public int Type { get; set; }
            public string UserId { get; set; }
        }

        public enum ForceSaveInitiator
        {
            Command = 0,
            User = 1,
            Timer = 2,
            UserSubmit = 3
        }
    }

    public enum MailMergeType
    {
        Html = 0,
        AttachDocx = 1,
        AttachPdf = 2,
    }

    [DebuggerDisplay("{From}")]
    public class MailMergeData
    {
        public int RecordCount { get; set; }
        public int RecordErrorCount { get; set; }
        public int RecordIndex { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public MailMergeType Type { get; set; }
        public string Title { get; set; } //attach
        public string Message { get; set; } //attach
    }

    [Serializable]
    public class TrackResponse
    {
        public int Error
        {
            get
            {
                return string.IsNullOrEmpty(Message)
                           ? 0 //error:0 - sended
                           : 1; //error:1 - some error
            }
        }

        public string Message { get; set; }

        public static string Serialize(TrackResponse response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            return JsonSerializer.Serialize(response, options);
        }
    }

    #endregion
}

[Scope]
public class DocumentServiceTrackerHelper
{
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly SocketManager _socketManager;
    private readonly GlobalStore _globalStore;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly IDaoFactory _daoFactory;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly EntryManager _entryManager;
    private readonly FileShareLink _fileShareLink;
    private readonly FilesMessageService _filesMessageService;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly NotifyClient _notifyClient;
    private readonly MailMergeTaskRunner _mailMergeTaskRunner;
    private readonly FileTrackerHelper _fileTracker;
    private readonly ILogger<DocumentServiceTrackerHelper> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ThirdPartySelector _thirdPartySelector;
    public DocumentServiceTrackerHelper(
        SecurityContext securityContext,
        UserManager userManager,
        TenantManager tenantManager,
        FilesLinkUtility filesLinkUtility,
        EmailValidationKeyProvider emailValidationKeyProvider,
        BaseCommonLinkUtility baseCommonLinkUtility,
        SocketManager socketManager,
        GlobalStore globalStore,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        IDaoFactory daoFactory,
        ILogger<DocumentServiceTrackerHelper> logger,
        DocumentServiceHelper documentServiceHelper,
        EntryManager entryManager,
        FileShareLink fileShareLink,
        FilesMessageService filesMessageService,
        DocumentServiceConnector documentServiceConnector,
        NotifyClient notifyClient,
        MailMergeTaskRunner mailMergeTaskRunner,
        FileTrackerHelper fileTracker,
            IHttpClientFactory clientFactory,
            ThirdPartySelector thirdPartySelector)
    {
        _securityContext = securityContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _filesLinkUtility = filesLinkUtility;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _socketManager = socketManager;
        _globalStore = globalStore;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _daoFactory = daoFactory;
        _documentServiceHelper = documentServiceHelper;
        _entryManager = entryManager;
        _fileShareLink = fileShareLink;
        _filesMessageService = filesMessageService;
        _documentServiceConnector = documentServiceConnector;
        _notifyClient = notifyClient;
        _mailMergeTaskRunner = mailMergeTaskRunner;
        _fileTracker = fileTracker;
        _logger = logger;
        _clientFactory = clientFactory;
        _thirdPartySelector = thirdPartySelector;
    }

    public async Task<string> GetCallbackUrlAsync<T>(T fileId)
    {
        var callbackUrl = _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath
                                                                + "?" + FilesLinkUtility.Action + "=track"
                                                                + "&" + FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(fileId.ToString())
                                                                + "&" + FilesLinkUtility.AuthKey + "=" + await _emailValidationKeyProvider.GetEmailKeyAsync(fileId.ToString()));
        callbackUrl = await _documentServiceConnector.ReplaceCommunityAdressAsync(callbackUrl);

        return callbackUrl;
    }

    public string GetCallbackUrl<T>(T fileId)
    {
        var callbackUrl = _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath
                                                                + "?" + FilesLinkUtility.Action + "=track"
                                                                + "&" + FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(fileId.ToString())
                                                                + "&" + FilesLinkUtility.AuthKey + "=" + _emailValidationKeyProvider.GetEmailKey(fileId.ToString()));
        callbackUrl = _documentServiceConnector.ReplaceCommunityAdress(callbackUrl);

        return callbackUrl;
    }

    public async Task<bool> StartTrackAsync<T>(T fileId, string docKeyForTrack)
    {
        var callbackUrl = await GetCallbackUrlAsync(fileId);

        return await _documentServiceConnector.CommandAsync(CommandMethod.Info, docKeyForTrack, fileId, callbackUrl);
    }

    public async Task<TrackResponse> ProcessDataAsync<T>(T fileId, TrackerData fileData)
    {
        switch (fileData.Status)
        {
            case TrackerStatus.NotFound:
            case TrackerStatus.Closed:
                _fileTracker.Remove(fileId);
                await _socketManager.StopEditAsync(fileId);

                break;

            case TrackerStatus.Editing:
                await ProcessEditAsync(fileId, fileData);
                break;

            case TrackerStatus.MustSave:
            case TrackerStatus.Corrupted:
            case TrackerStatus.ForceSave:
            case TrackerStatus.CorruptedForceSave:
                return await ProcessSaveAsync(fileId, fileData);

            case TrackerStatus.MailMerge:
                return await ProcessMailMergeAsync(fileId, fileData);
        }
        return null;
    }

    private async Task ProcessEditAsync<T>(T fileId, TrackerData fileData)
    {
        if (_thirdPartySelector.GetAppByFileId(fileId.ToString()) != null)
        {
            return;
        }

        var users = _fileTracker.GetEditingBy(fileId);
        var usersDrop = new List<string>();

        string docKey;
        var app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
        if (app == null)
        {
            File<T> fileStable;
            fileStable = await _daoFactory.GetFileDao<T>().GetFileStableAsync(fileId);

            docKey = await _documentServiceHelper.GetDocKeyAsync(fileStable);
        }
        else
        {
            docKey = fileData.Key;
        }

        if (!fileData.Key.Equals(docKey))
        {
            _logger.InformationDocServiceEditingFile(fileId.ToString(), docKey, fileData.Key, fileData.Users);
            usersDrop = fileData.Users;
        }
        else
        {
            foreach (var user in fileData.Users)
            {
                if (!Guid.TryParse(user, out var userId))
                {
                    if (!string.IsNullOrEmpty(user) && user.StartsWith("uid-"))
                    {
                        userId = Guid.Empty;
                    }
                    else
                    {
                        _logger.InformationDocServiceUserIdIsNotGuid(user);
                        continue;
                    }
                }
                users.Remove(userId);

                try
                {
                    var doc = await _fileShareLink.CreateKeyAsync(fileId);
                    await _entryManager.TrackEditingAsync(fileId, userId, userId, doc);
                }
                catch (Exception e)
                {
                    _logger.DebugDropCommand(fileId.ToString(), fileData.Key, user, e);
                    usersDrop.Add(userId.ToString());
                }
            }
        }

        if (usersDrop.Count > 0)
        {
            if (!await _documentServiceHelper.DropUserAsync(fileData.Key, usersDrop.ToArray(), fileId))
            {
                _logger.ErrorDocServiceDropFailed(usersDrop);
            }
        }

        foreach (var removeUserId in users)
        {
            _fileTracker.Remove(fileId, userId: removeUserId);
        }

        await _socketManager.StartEditAsync(fileId);
    }

    private async Task<TrackResponse> ProcessSaveAsync<T>(T fileId, TrackerData fileData)
    {
        var comments = new List<string>();
        if (fileData.Status == TrackerStatus.Corrupted
            || fileData.Status == TrackerStatus.CorruptedForceSave)
        {
            comments.Add(FilesCommonResource.ErrorMassage_SaveCorrupted);
        }

        var forcesave = fileData.Status == TrackerStatus.ForceSave || fileData.Status == TrackerStatus.CorruptedForceSave;

        if (fileData.Users == null || fileData.Users.Count == 0 || !Guid.TryParse(fileData.Users[0], out var userId))
        {
            userId = Guid.Empty;
        }

        var app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
        if (app == null)
        {
            File<T> fileStable;
            fileStable = await _daoFactory.GetFileDao<T>().GetFileStableAsync(fileId);

            var docKey = await _documentServiceHelper.GetDocKeyAsync(fileStable);
            if (!fileData.Key.Equals(docKey))
            {
                _logger.ErrorDocServiceSavingFile(fileId.ToString(), docKey, fileData.Key);

                await StoringFileAfterErrorAsync(fileId, userId.ToString(), _documentServiceConnector.ReplaceDocumentAdress(fileData.Url), fileData.Filetype);

                return new TrackResponse { Message = "Expected key " + docKey };
            }
        }

        UserInfo user = null;
        try
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(userId);

            user = await _userManager.GetUsersAsync(userId);
            var culture = string.IsNullOrEmpty(user.CultureName) ? (await _tenantManager.GetCurrentTenantAsync()).GetCulture() : CultureInfo.GetCultureInfo(user.CultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        catch (Exception ex)
        {
            _logger.InformationDocServiceSaveError(userId, ex);
            if (!userId.Equals(ASC.Core.Configuration.Constants.Guest.ID))
            {
                comments.Add(FilesCommonResource.ErrorMassage_SaveAnonymous);
            }
        }

        File<T> file = null;
        var saveMessage = "Not saved";

        if (string.IsNullOrEmpty(fileData.Url))
        {
            try
            {
                comments.Add(FilesCommonResource.ErrorMassage_SaveUrlLost);

                file = await _entryManager.CompleteVersionFileAsync(fileId, 0, false, false);

                await _daoFactory.GetFileDao<T>().UpdateCommentAsync(file.Id, file.Version, string.Join("; ", comments));

                file = null;
                _logger.ErrorDocServiceSave2(fileId.ToString(), userId, fileData.Key);
            }
            catch (Exception ex)
            {
                _logger.ErrorDocServiceSaveVersionUpdate(fileId.ToString(), userId, fileData.Key, ex);
            }
        }
        else
        {
            if (fileData.Encrypted)
            {
                comments.Add(FilesCommonResource.CommentEditEncrypt);
            }

            var forcesaveType = ForcesaveType.None;
            if (forcesave)
            {
                switch (fileData.ForceSaveType)
                {
                    case TrackerData.ForceSaveInitiator.Command:
                        forcesaveType = ForcesaveType.Command;
                        comments.Add(FilesCommonResource.CommentAutosave);
                        break;
                    case TrackerData.ForceSaveInitiator.Timer:
                        forcesaveType = ForcesaveType.Timer;
                        comments.Add(FilesCommonResource.CommentAutosave);
                        break;
                    case TrackerData.ForceSaveInitiator.User:
                        forcesaveType = ForcesaveType.User;
                        comments.Add(FilesCommonResource.CommentForcesave);
                        break;
                    case TrackerData.ForceSaveInitiator.UserSubmit:
                        forcesaveType = ForcesaveType.UserSubmit;
                        comments.Add(FilesCommonResource.CommentSubmitFillForm);
                        break;
                }
            }

            try
            {
                file = await _entryManager.SaveEditingAsync(fileId, fileData.Filetype, _documentServiceConnector.ReplaceDocumentAdress(fileData.Url), null, string.Empty, string.Join("; ", comments), false, fileData.Encrypted, forcesaveType, true);
                saveMessage = fileData.Status == TrackerStatus.MustSave || fileData.Status == TrackerStatus.ForceSave ? null : "Status " + fileData.Status;
            }
            catch (Exception ex)
            {
                _logger.ErrorDocServiceSave(fileId.ToString(), userId, fileData.Key, fileData.Url, ex);
                saveMessage = ex.Message;

                await StoringFileAfterErrorAsync(fileId, userId.ToString(), _documentServiceConnector.ReplaceDocumentAdress(fileData.Url), fileData.Filetype);
            }
        }

        if (!forcesave)
        {
            _fileTracker.Remove(fileId);
            await _socketManager.StopEditAsync(fileId);
        }

        if (file != null)
        {
            if (user != null)
            {
                 _ = _filesMessageService.SendAsync(file, MessageInitiator.DocsService, MessageAction.UserFileUpdated, user.DisplayUserName(false, _displayUserSettingsHelper), file.Title);
            }

            if (!forcesave)
            {
                await SaveHistoryAsync(file, (fileData.History ?? "").ToString(), _documentServiceConnector.ReplaceDocumentAdress(fileData.ChangesUrl));
            }

            if (fileData.Status == TrackerStatus.ForceSave && fileData.ForceSaveType == TrackerData.ForceSaveInitiator.UserSubmit)
            {
                await _entryManager.SubmitFillForm(file);
            }
        }

        return new TrackResponse { Message = saveMessage };
    }

    private async Task<TrackResponse> ProcessMailMergeAsync<T>(T fileId, TrackerData fileData)
    {
        if (fileData.Users == null || fileData.Users.Count == 0 || !Guid.TryParse(fileData.Users[0], out var userId))
        {
            userId = _fileTracker.GetEditingBy(fileId).FirstOrDefault();
        }

        string saveMessage;

        try
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(userId);

            var user = await _userManager.GetUsersAsync(userId);
            var culture = string.IsNullOrEmpty(user.CultureName) ? (await _tenantManager.GetCurrentTenantAsync()).GetCulture() : CultureInfo.GetCultureInfo(user.CultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            if (string.IsNullOrEmpty(fileData.Url))
            {
                throw new ArgumentException("emptry url");
            }

            if (fileData.MailMerge == null)
            {
                throw new ArgumentException("MailMerge is null");
            }

            var message = fileData.MailMerge.Message;
            Stream attach = null;
            var httpClient = _clientFactory.CreateClient();
            switch (fileData.MailMerge.Type)
            {
                case MailMergeType.AttachDocx:
                case MailMergeType.AttachPdf:
                    var requestDownload = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_documentServiceConnector.ReplaceDocumentAdress(fileData.Url))
                    };

                    using (var responseDownload = await httpClient.SendAsync(requestDownload))
                    using (var streamDownload = await responseDownload.Content.ReadAsStreamAsync())
                    using (var downloadStream = new ResponseStream(streamDownload, streamDownload.Length))
                    {
                        const int bufferSize = 2048;
                        var buffer = new byte[bufferSize];
                        int readed;
                        attach = new MemoryStream();
                        while ((readed = await downloadStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                        {
                            await attach.WriteAsync(buffer, 0, readed);
                        }

                        attach.Position = 0;
                    }

                    if (string.IsNullOrEmpty(fileData.MailMerge.Title))
                    {
                        fileData.MailMerge.Title = "Attach";
                    }

                    var attachExt = fileData.MailMerge.Type == MailMergeType.AttachDocx ? ".docx" : ".pdf";
                    var curExt = FileUtility.GetFileExtension(fileData.MailMerge.Title);
                    if (curExt != attachExt)
                    {
                        fileData.MailMerge.Title += attachExt;
                    }

                    break;

                case MailMergeType.Html:
                    var httpRequest = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_documentServiceConnector.ReplaceDocumentAdress(fileData.Url))
                    };

                    using (var httpResponse = await httpClient.SendAsync(httpRequest))
                    using (var stream = await httpResponse.Content.ReadAsStreamAsync())
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream, Encoding.GetEncoding(Encoding.UTF8.WebName)))
                            {
                                message = await reader.ReadToEndAsync();
                            }
                        }
                    }

                    break;
            }

            using (var mailMergeTask =
                new MailMergeTask
                {
                    From = fileData.MailMerge.From,
                    Subject = fileData.MailMerge.Subject,
                    To = fileData.MailMerge.To,
                    Message = message,
                    AttachTitle = fileData.MailMerge.Title,
                    Attach = attach
                })
            {
                var response = await _mailMergeTaskRunner.RunAsync(mailMergeTask, _clientFactory);
                _logger.InformationDocServiceMailMerge(fileData.MailMerge.RecordIndex + 1, fileData.MailMerge.RecordCount, response);
            }
            saveMessage = null;
        }
        catch (Exception ex)
        {
            _logger.ErrorDocServiceMailMerge(fileData.MailMerge == null ? "" : " " + fileData.MailMerge.RecordIndex + "/" + fileData.MailMerge.RecordCount,
                              userId, fileData.Url, ex);
            saveMessage = ex.Message;
        }

        if (fileData.MailMerge != null &&
            fileData.MailMerge.RecordIndex == fileData.MailMerge.RecordCount - 1)
        {
            var errorCount = fileData.MailMerge.RecordErrorCount;
            if (!string.IsNullOrEmpty(saveMessage))
            {
                errorCount++;
            }

            await _notifyClient.SendMailMergeEndAsync(userId, fileData.MailMerge.RecordCount, errorCount);
        }

        return new TrackResponse { Message = saveMessage };
    }

    private async Task StoringFileAfterErrorAsync<T>(T fileId, string userId, string downloadUri, string downloadType)
    {
        if (string.IsNullOrEmpty(downloadUri))
        {
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(downloadType))
            {
                downloadType = FileUtility.GetFileExtension(downloadUri).Trim('.');
            }

            var fileName = Global.ReplaceInvalidCharsAndTruncate(fileId + "." + downloadType);

            var path = $@"save_crash\{DateTime.UtcNow:yyyy_MM_dd}\{userId}_{fileName}";

            var store = await _globalStore.GetStoreAsync();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(downloadUri)
            };

            var httpClient = _clientFactory.CreateClient();
            using (var response = await httpClient.SendAsync(request))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new ResponseStream(stream, stream.Length))
            {
                await store.SaveAsync(FileConstant.StorageDomainTmp, path, fileStream);
            }
            _logger.DebugDocServiceStoring(path);
        }
        catch (Exception ex)
        {
            _logger.ErrorDocServiceSaveFileToTempStore(ex);
        }
    }

    private async Task SaveHistoryAsync<T>(File<T> file, string changes, string differenceUrl)
    {
        if (file == null)
        {
            return;
        }

        if (file.ProviderEntry)
        {
            return;
        }

        if (string.IsNullOrEmpty(changes) || string.IsNullOrEmpty(differenceUrl))
        {
            return;
        }

        try
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(differenceUrl)
            };

            var httpClient = _clientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();

            using var differenceStream = new ResponseStream(stream, stream.Length);
            await fileDao.SaveEditHistoryAsync(file, changes, differenceStream);
        }
        catch (Exception ex)
        {
            _logger.ErrorDocServiceSavehistory(ex);
        }
    }
}
