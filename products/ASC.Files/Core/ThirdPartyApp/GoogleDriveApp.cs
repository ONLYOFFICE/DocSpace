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

using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Web.Files.ThirdPartyApp;

[Scope]
public class GoogleDriveApp : Consumer, IThirdPartyApp, IOAuthProvider
{
    public const string AppAttr = "gdrive";

    public string Scopes => string.Empty;
    public string CodeUrl => string.Empty;
    public string AccessTokenUrl => _googleLoginProvider.Instance.AccessTokenUrl;
    public string RedirectUri => this["googleDriveAppRedirectUrl"];
    public string ClientID => this["googleDriveAppClientId"];
    public string ClientSecret => this["googleDriveAppSecretKey"];
    public bool IsEnabled => !string.IsNullOrEmpty(ClientID) && !string.IsNullOrEmpty(ClientSecret);

    private readonly ILogger<GoogleDriveApp> _logger;
    private readonly PathProvider _pathProvider;
    private readonly TenantUtil _tenantUtil;
    private readonly AuthContext _authContext;
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly CookiesManager _cookiesManager;
    private readonly Global _global;
    private readonly GlobalStore _globalStore;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly SettingsManager _settingsManager;
    private readonly PersonalSettingsHelper _personalSettingsHelper;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly AccountLinker _accountLinker;
    private readonly SetupInfo _setupInfo;
    private readonly GoogleLoginProvider _googleLoginProvider;
    private readonly TokenHelper _tokenHelper;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly ThirdPartyAppHandlerService _thirdPartyAppHandlerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _clientFactory;
    private readonly RequestHelper _requestHelper;
    private readonly ThirdPartySelector _thirdPartySelector;

    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public GoogleDriveApp() { }

    public GoogleDriveApp(
        PathProvider pathProvider,
        TenantUtil tenantUtil,
        AuthContext authContext,
        SecurityContext securityContext,
        UserManager userManager,
        UserManagerWrapper userManagerWrapper,
        CookiesManager cookiesManager,
        Global global,
        GlobalStore globalStore,
        EmailValidationKeyProvider emailValidationKeyProvider,
        FilesLinkUtility filesLinkUtility,
        SettingsManager settingsManager,
        PersonalSettingsHelper personalSettingsHelper,
        BaseCommonLinkUtility baseCommonLinkUtility,
        ILogger<GoogleDriveApp> logger,
        FileUtility fileUtility,
        FilesSettingsHelper filesSettingsHelper,
        AccountLinker accountLinker,
        SetupInfo setupInfo,
        GoogleLoginProvider googleLoginProvider,
        TokenHelper tokenHelper,
        DocumentServiceConnector documentServiceConnector,
        ThirdPartyAppHandlerService thirdPartyAppHandlerService,
        IServiceProvider serviceProvider,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        IHttpClientFactory clientFactory,
        OAuth20TokenHelper oAuth20TokenHelper,
        RequestHelper requestHelper,
        ThirdPartySelector thirdPartySelector,
        string name, int order, Dictionary<string, string> additional)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, additional)
    {
        _logger = logger;
        _pathProvider = pathProvider;
        _tenantUtil = tenantUtil;
        _authContext = authContext;
        _securityContext = securityContext;
        _userManager = userManager;
        _userManagerWrapper = userManagerWrapper;
        _cookiesManager = cookiesManager;
        _global = global;
        _globalStore = globalStore;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _filesLinkUtility = filesLinkUtility;
        _settingsManager = settingsManager;
        _personalSettingsHelper = personalSettingsHelper;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _fileUtility = fileUtility;
        _filesSettingsHelper = filesSettingsHelper;
        _accountLinker = accountLinker;
        _setupInfo = setupInfo;
        _googleLoginProvider = googleLoginProvider;
        _tokenHelper = tokenHelper;
        _documentServiceConnector = documentServiceConnector;
        _thirdPartyAppHandlerService = thirdPartyAppHandlerService;
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
        _oAuth20TokenHelper = oAuth20TokenHelper;
        _requestHelper = requestHelper;
        _thirdPartySelector = thirdPartySelector;
    }

    public async Task<bool> RequestAsync(HttpContext context)
    {
        switch ((context.Request.Query[FilesLinkUtility.Action].FirstOrDefault() ?? "").ToLower())
        {
            case "stream":
                await StreamFileAsync(context);
                return true;
            case "convert":
                await ConfirmConvertFileAsync(context);
                return true;
            case "create":
                await CreateFileAsync(context);
                return true;
        }

        if (!string.IsNullOrEmpty(context.Request.Query["code"]))
        {
            await RequestCodeAsync(context);

            return true;
        }

        return false;
    }

    public string GetRefreshUrl()
    {
        return AccessTokenUrl;
    }

    public async Task<(File<string>, bool)> GetFileAsync(string fileId)
    {
        _logger.DebugGoogleDriveAppGetFile(fileId);
        fileId = ThirdPartySelector.GetFileId(fileId);

        var token = await _tokenHelper.GetTokenAsync(AppAttr);
        var driveFile = GetDriveFile(fileId, token);
        var editable = false;

        if (driveFile == null)
        {
            return (null, editable);
        }

        var jsonFile = JObject.Parse(driveFile);

        var file = _serviceProvider.GetService<File<string>>();
        file.Id = ThirdPartySelector.BuildAppFileId(AppAttr, jsonFile.Value<string>("id"));
        file.Title = Global.ReplaceInvalidCharsAndTruncate(GetCorrectTitle(jsonFile));
        file.CreateOn = _tenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("createdTime"));
        file.ModifiedOn = _tenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("modifiedTime"));
        file.ContentLength = Convert.ToInt64(jsonFile.Value<string>("size"));
        file.ModifiedByString = jsonFile["lastModifyingUser"]["displayName"].Value<string>();
        file.ProviderKey = "Google";

        var owners = jsonFile["owners"];
        if (owners != null)
        {
            file.CreateByString = owners[0]["displayName"].Value<string>();
        }

        editable = jsonFile["capabilities"]["canEdit"].Value<bool>();

        return (file, editable);
    }

    public string GetFileStreamUrl(File<string> file)
    {
        if (file == null)
        {
            return string.Empty;
        }

        var fileId = ThirdPartySelector.GetFileId(file.Id);

        return GetFileStreamUrl(fileId);
    }

    private string GetFileStreamUrl(string fileId)
    {
        _logger.DebugGoogleDriveAppGetFileStreamUrl(fileId);

        var uriBuilder = new UriBuilder(_baseCommonLinkUtility.GetFullAbsolutePath(_thirdPartyAppHandlerService.HandlerPath));
        if (uriBuilder.Uri.IsLoopback)
        {
            uriBuilder.Host = Dns.GetHostName();
        }

        var query = uriBuilder.Query;
        query += FilesLinkUtility.Action + "=stream&";
        query += FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(fileId) + "&";
        query += CommonLinkUtility.ParamName_UserUserID + "=" + HttpUtility.UrlEncode(_authContext.CurrentAccount.ID.ToString()) + "&";
        query += FilesLinkUtility.AuthKey + "=" + _emailValidationKeyProvider.GetEmailKeyAsync(fileId + _authContext.CurrentAccount.ID) + "&";
        query += ThirdPartySelector.AppAttr + "=" + AppAttr;

        return uriBuilder.Uri + "?" + query;
    }

    public async Task SaveFileAsync(string fileId, string fileType, string downloadUrl, Stream stream)
    {
        _logger.DebugGoogleDriveAppSaveFileStream(fileId, stream == null ? downloadUrl : "stream");
        fileId = ThirdPartySelector.GetFileId(fileId);

        var token = await _tokenHelper.GetTokenAsync(AppAttr);

        var driveFile = GetDriveFile(fileId, token);
        if (driveFile == null)
        {
            _logger.ErrorGoogleDriveAppFileIsNull();

            throw new Exception("File not found");
        }

        var jsonFile = JObject.Parse(driveFile);
        var currentType = GetCorrectExt(jsonFile);
        if (!fileType.Equals(currentType))
        {
            try
            {
                if (stream != null)
                {
                    downloadUrl = await _pathProvider.GetTempUrlAsync(stream, fileType);
                    downloadUrl = await _documentServiceConnector.ReplaceCommunityAdressAsync(downloadUrl);
                }

                _logger.DebugGoogleDriveAppGetConvertedUri(fileType, currentType, downloadUrl);

                var key = DocumentServiceConnector.GenerateRevisionId(downloadUrl);

                var resultTuple = await _documentServiceConnector.GetConvertedUriAsync(downloadUrl, fileType, currentType, key, null, CultureInfo.CurrentUICulture.Name, null, null, false);
                downloadUrl = resultTuple.ConvertedDocumentUri;

                stream = null;
            }
            catch (Exception e)
            {
                _logger.ErrorGoogleDriveAppConvert(e);
            }
        }

        var httpClient = _clientFactory.CreateClient();

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(GoogleLoginProvider.GoogleUrlFileUpload + "/{fileId}?uploadType=media".Replace("{fileId}", fileId)),
            Method = HttpMethod.Patch
        };
        request.Headers.Add("Authorization", "Bearer " + token);


        if (stream != null)
        {
            request.Content = new StreamContent(stream);
        }
        else
        {
            var response = await httpClient.GetAsync(downloadUrl);
            var downloadStream = new ResponseStream(response);
            request.Content = new StreamContent(downloadStream);
        }

        request.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(currentType));

        try
        {
            httpClient = _clientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            using var responseStream = await response.Content.ReadAsStreamAsync();
            string result = null;
            if (responseStream != null)
            {
                using var readStream = new StreamReader(responseStream);
                result = await readStream.ReadToEndAsync();
            }

            _logger.DebugGoogleDriveAppSaveFileStream2(result);
        }
        catch (HttpRequestException e)
        {
            _logger.ErrorGoogleDriveAppSaveFileStream(e);
            if (e.StatusCode == HttpStatusCode.Forbidden || e.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
            }

            throw;
        }
    }

    private async Task RequestCodeAsync(HttpContext context)
    {
        var state = context.Request.Query["state"];
        _logger.DebugGoogleDriveAppState(state);
        if (string.IsNullOrEmpty(state))
        {
            _logger.ErrorGoogleDriveAppEmptyIsNull();

            throw new Exception("Empty state");
        }

        var token = GetToken(context.Request.Query["code"]);
        if (token == null)
        {
            _logger.ErrorGoogleDriveAppTokenIsNull();

            throw new SecurityException("Access token is null");
        }

        var stateJson = JObject.Parse(state);

        var googleUserId = stateJson.Value<string>("userId");

        if (_authContext.IsAuthenticated)
        {
            if (!(await CurrentUserAsync(googleUserId)))
            {
                _logger.DebugGoogleDriveAppLogout(googleUserId);
                _cookiesManager.ClearCookies(CookiesType.AuthKey);
                _authContext.Logout();
            }
        }

        if (!_authContext.IsAuthenticated)
        {
            var wrapper = await GetUserInfoAsync(token);
            var userInfo = wrapper.UserInfo;
            var isNew = wrapper.IsNew;

            if (userInfo == null)
            {
                _logger.ErrorGoogleDriveAppUserInfoIsNull();

                throw new Exception("Profile is null");
            }

            await _cookiesManager.AuthenticateMeAndSetCookiesAsync(userInfo.Tenant, userInfo.Id, MessageAction.LoginSuccessViaSocialApp);

            if (isNew)
            {
                var userHelpTourSettings = await _settingsManager.LoadForCurrentUserAsync<UserHelpTourSettings>();
                userHelpTourSettings.IsNewUser = true;
                await _settingsManager.SaveForCurrentUserAsync(userHelpTourSettings);

                _personalSettingsHelper.IsNewUser = true;
                _personalSettingsHelper.IsNotActivated = true;
            }

            if (!string.IsNullOrEmpty(googleUserId) && !(await CurrentUserAsync(googleUserId)))
            {
                await AddLinkerAsync(googleUserId);
            }
        }

        await _tokenHelper.SaveTokenAsync(token);

        var action = stateJson.Value<string>("action");
        switch (action)
        {
            case "create":
                //var folderId = stateJson.Value<string>("folderId");

                //context.Response.Redirect(App.Location + "?" + FilesLinkUtility.FolderId + "=" + HttpUtility.UrlEncode(folderId), true);
                return;
            case "open":
                var idsArray = stateJson.Value<JArray>("ids") ?? stateJson.Value<JArray>("exportIds");
                if (idsArray == null)
                {
                    _logger.ErrorGoogleDriveAppIdsIsNull();

                    throw new Exception("File id is null");
                }
                var fileId = idsArray.ToObject<List<string>>().FirstOrDefault();

                var driveFile = GetDriveFile(fileId, token);
                if (driveFile == null)
                {
                    _logger.ErrorGoogleDriveAppFileIsNull();

                    throw new Exception("File not found");
                }

                var jsonFile = JObject.Parse(driveFile);
                var ext = GetCorrectExt(jsonFile);
                if (_fileUtility.ExtsMustConvert.Contains(ext)
                    || GoogleLoginProvider.GoogleDriveExt.Contains(ext))
                {
                    _logger.DebugGoogleDriveAppFileMustBeConverted();
                    if (_filesSettingsHelper.ConvertNotify)
                    {
                        context.Response.Redirect(
                            _baseCommonLinkUtility.ToAbsolute(_thirdPartyAppHandlerService.HandlerPath)
                            + "?" + FilesLinkUtility.Action + "=convert"
                            + "&" + FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(fileId)
                            + "&" + ThirdPartySelector.AppAttr + "=" + AppAttr,
                            false);
                        return;
                    }

                    fileId = await CreateConvertedFileAsync(driveFile, token);
                }

                context.Response.Redirect(_filesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
                await context.Response.CompleteAsync();
                return;
        }

        _logger.ErrorGoogleDriveAppActionNotIdentified();

        throw new Exception("Action not identified");
    }

    private async Task StreamFileAsync(HttpContext context)
    {
        try
        {
            var fileId = context.Request.Query[FilesLinkUtility.FileId];
            var auth = context.Request.Query[FilesLinkUtility.AuthKey];
            var userId = context.Request.Query[CommonLinkUtility.ParamName_UserUserID];

            _logger.DebugGoogleDriveAppGetFileStream(fileId);

            var validateResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(fileId + userId, auth, _global.StreamUrlExpire);
            if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
            {
                var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                _logger.ErrorGoogleDriveAppValidate(FilesLinkUtility.AuthKey, validateResult, context.Request.Url(), exc);

                throw exc;
            }

            Token token = null;

            if (Guid.TryParse(userId, out var userIdGuid))
            {
                token = await _tokenHelper.GetTokenAsync(AppAttr, userIdGuid);
            }

            if (token == null)
            {
                _logger.ErrorGoogleDriveAppTokenIsNull();

                throw new SecurityException("Access token is null");
            }

            var driveFile = GetDriveFile(fileId, token);

            var jsonFile = JObject.Parse(driveFile);

            var downloadUrl = GoogleLoginProvider.GoogleUrlFile + fileId + "?alt=media";

            if (string.IsNullOrEmpty(downloadUrl))
            {
                _logger.ErrorGoogleDriveAppDownloadUrlIsNull();

                throw new Exception("downloadUrl is null");
            }

            var contentLength = jsonFile.Value<string>("size");
            _logger.DebugGoogleDriveAppGetFileStreamcontentLength(contentLength);
            context.Response.Headers.Add("Content-Length", contentLength);

            _logger.DebugGoogleDriveAppGetFileStreamDownloadUrl(downloadUrl);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(downloadUrl),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", "Bearer " + token);

            var httpClient = _clientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            using var stream = new ResponseStream(response);
            await stream.CopyToAsync(context.Response.Body);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(ex.Message);
            _logger.ErrorGoogleDriveAppRequest(context.Request.Url(), ex);
        }
        try
        {
            await context.Response.Body.FlushAsync();
            //TODO
            //context.Response.SuppressContent = true;
            //context.ApplicationInstance.CompleteRequest();
        }
        catch (HttpException ex)
        {
            _logger.ErrorGoogleDriveAppStreamFile(ex);
        }
    }

    private async Task ConfirmConvertFileAsync(HttpContext context)
    {
        var fileId = context.Request.Query[FilesLinkUtility.FileId];
        _logger.DebugGoogleDriveAppConfirmConvertFile(fileId);

        var token = await _tokenHelper.GetTokenAsync(AppAttr);

        var driveFile = GetDriveFile(fileId, token);
        if (driveFile == null)
        {
            _logger.ErrorGoogleDriveAppFileIsNull();

            throw new Exception("File not found");
        }

        fileId = await CreateConvertedFileAsync(driveFile, token);

        context.Response.Redirect(_filesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
    }

    private async Task CreateFileAsync(HttpContext context)
    {
        var folderId = context.Request.Query[FilesLinkUtility.FolderId];
        var fileName = context.Request.Query[FilesLinkUtility.FileTitle];
        _logger.DebugGoogleDriveAppCreateFile(folderId, fileName);

        var token = await _tokenHelper.GetTokenAsync(AppAttr);

        var culture = (await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID)).GetCulture();
        var storeTemplate = await _globalStore.GetStoreTemplateAsync();

        var path = FileConstant.NewDocPath + culture + "/";
        if (!await storeTemplate.IsDirectoryAsync(path))
        {
            path = FileConstant.NewDocPath + "default/";
        }

        var ext = _fileUtility.InternalExtension[FileUtility.GetFileTypeByFileName(fileName)];
        path += "new" + ext;
        fileName = FileUtility.ReplaceFileExtension(fileName, ext);

        string driveFile;
        using (var content = await storeTemplate.GetReadStreamAsync("", path))
        {
            driveFile = await CreateFileAsync(content, fileName, folderId, token);
        }
        if (driveFile == null)
        {
            _logger.ErrorGoogleDriveAppFileIsNull();

            throw new Exception("File not created");
        }

        var jsonFile = JObject.Parse(driveFile);
        var fileId = jsonFile.Value<string>("id");

        context.Response.Redirect(_filesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
    }

    private Token GetToken(string code)
    {
        try
        {
            _logger.DebugGoogleDriveAppGetAccessTokenByCode(code);
            var token = _oAuth20TokenHelper.GetAccessToken<GoogleDriveApp>(ConsumerFactory, code);

            return new Token(token, AppAttr);
        }
        catch (Exception ex)
        {
            _logger.ErrorGetToken(ex);
        }

        return null;
    }

    private async Task<bool> CurrentUserAsync(string googleId)
    {
        var linkedProfiles = await _accountLinker.GetLinkedObjectsByHashIdAsync(HashHelper.MD5($"{ProviderConstants.Google}/{googleId}"));

        return linkedProfiles.Any(profileId => Guid.TryParse(profileId, out var tmp) && tmp == _authContext.CurrentAccount.ID);
    }

    private async Task AddLinkerAsync(string googleUserId)
    {
        _logger.DebugGoogleDriveApAddLinker(googleUserId);

        await _accountLinker.AddLinkAsync(_authContext.CurrentAccount.ID.ToString(), googleUserId, ProviderConstants.Google);
    }

    private async Task<UserInfoWrapper> GetUserInfoAsync(Token token)
    {
        var wrapper = new UserInfoWrapper();
        if (token == null)
        {
            _logger.ErrorGoogleDriveAppTokenIsNull();

            throw new SecurityException("Access token is null");
        }

        LoginProfile loginProfile = null;
        try
        {
            loginProfile = _googleLoginProvider.Instance.GetLoginProfile(await token.GetRefreshedTokenAsync(_tokenHelper, _oAuth20TokenHelper, _thirdPartySelector));
        }
        catch (Exception ex)
        {
            _logger.ErrorGoogleDriveAppUserInfoRequest(ex);
        }

        if (loginProfile == null)
        {
            _logger.ErrorInUserInfoRequest();

            return null;
        }

        var userInfo = await _userManager.GetUserByEmailAsync(loginProfile.EMail);
        if (Equals(userInfo, Constants.LostUser))
        {
            userInfo = loginProfile.ProfileToUserInfo(CoreBaseSettings);

            var cultureName = loginProfile.Locale;
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = CultureInfo.CurrentUICulture.Name;
            }

            var cultureInfo = _setupInfo.EnabledCultures.Find(c => string.Equals(c.Name, cultureName, StringComparison.InvariantCultureIgnoreCase));
            if (cultureInfo != null)
            {
                userInfo.CultureName = cultureInfo.Name;
            }
            else
            {
                _logger.DebugFromGoogleAppNewPersonalUser(userInfo.Email, cultureName);
            }

            try
            {
                await _securityContext.AuthenticateMeWithoutCookieAsync(ASC.Core.Configuration.Constants.CoreSystem);
                userInfo = await _userManagerWrapper.AddUserAsync(userInfo, UserManagerWrapper.GeneratePassword());
            }
            finally
            {
                _securityContext.Logout();
            }

            wrapper.IsNew = true;

            _logger.DebugGoogleDriveAppNewUser(userInfo.Id);
        }
        wrapper.UserInfo = userInfo;
        return wrapper;
    }

    private string GetDriveFile(string googleFileId, Token token)
    {
        if (token == null)
        {
            _logger.ErrorGoogleDriveAppTokenIsNull();

            throw new SecurityException("Access token is null");
        }
        try
        {
            var requestUrl = GoogleLoginProvider.GoogleUrlFile + googleFileId + "?fields=" + HttpUtility.UrlEncode(GoogleLoginProvider.FilesFields);
            var resultResponse = _requestHelper.PerformRequest(requestUrl,
                                                          headers: new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
            _logger.DebugGoogleDriveAppFileResponse(resultResponse);

            return resultResponse;
        }
        catch (Exception ex)
        {
            _logger.ErrorGoogleDriveAppFileRequest(ex);
        }
        return null;
    }

    private async Task<string> CreateFileAsync(string contentUrl, string fileName, string folderId, Token token)
    {
        if (string.IsNullOrEmpty(contentUrl))
        {
            _logger.ErrorGoogleDriveAppDownloadUrlIsNull();

            throw new Exception("downloadUrl is null");
        }

        _logger.DebugGoogleDriveAppCreateFrom(contentUrl);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(contentUrl)
        };

        var httpClient = _clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);
        using var content = new ResponseStream(response);

        return await CreateFileAsync(content, fileName, folderId, token);
    }

    private async Task<string> CreateFileAsync(Stream content, string fileName, string folderId, Token token)
    {
        _logger.DebugGoogleDriveAppCreateFile2();

        var httpClient = _clientFactory.CreateClient();

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(GoogleLoginProvider.GoogleUrlFileUpload + "?uploadType=multipart")
        };

        var boundary = DateTime.UtcNow.Ticks.ToString("x");
        request.Method = HttpMethod.Post;
        request.Headers.Add("Authorization", "Bearer " + token);

        var stringContent = new { name = fileName, parents = new List<string>() };

        if (!string.IsNullOrEmpty(folderId))
        {
            stringContent.parents.Add(folderId);
        }

        var streamContent = new StreamContent(content);
        streamContent.Headers.TryAddWithoutValidation("Content-Type", MimeMapping.GetMimeMapping(fileName));

        var multipartContent = new MultipartContent("related", boundary);
        multipartContent.Add(JsonContent.Create(stringContent));
        multipartContent.Add(streamContent);
        request.Content = multipartContent;

        //Logger.Debug("GoogleDriveApp: create file totalSize - " + tmpStream.Length);

        try
        {
            using var response = await httpClient.SendAsync(request);
            using var responseStream = await response.Content.ReadAsStreamAsync();
            string result = null;
            if (responseStream != null)
            {
                using var readStream = new StreamReader(responseStream);
                result = await readStream.ReadToEndAsync();
            }

            _logger.DebugGoogleDriveAppCreateFileResponse(result);

            return result;
        }
        catch (HttpRequestException e)
        {
            _logger.ErrorGoogleDriveAppCreateFile(e);

            if (e.StatusCode == HttpStatusCode.Forbidden || e.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
            }
        }

        return null;
    }

    private async Task<string> ConvertFileAsync(string fileId, string fromExt)
    {
        _logger.DebugGoogleDriveAppConvertFile();

        var downloadUrl = GetFileStreamUrl(fileId);

        var toExt = _fileUtility.GetInternalExtension(fromExt);
        try
        {
            _logger.DebugGoogleDriveAppGetConvertedUri2(downloadUrl);

            var key = DocumentServiceConnector.GenerateRevisionId(downloadUrl);

            var resultTuple = await _documentServiceConnector.GetConvertedUriAsync(downloadUrl, fromExt, toExt, key, null, CultureInfo.CurrentUICulture.Name, null, null, false);
            downloadUrl = resultTuple.ConvertedDocumentUri;

        }
        catch (Exception e)
        {
            _logger.ErrorGoogleDriveAppGetConvertedUri(e);
        }

        return downloadUrl;
    }

    private async Task<string> CreateConvertedFileAsync(string driveFile, Token token)
    {
        var jsonFile = JObject.Parse(driveFile);
        var fileName = GetCorrectTitle(jsonFile);

        var folderId = (string)jsonFile.SelectToken("parents[0]");

        _logger.InformationGoogleDriveAppCreateCopy(fileName);

        var ext = GetCorrectExt(jsonFile);
        var fileId = jsonFile.Value<string>("id");

        if (GoogleLoginProvider.GoogleDriveExt.Contains(ext))
        {
            var internalExt = _fileUtility.GetGoogleDownloadableExtension(ext);
            fileName = FileUtility.ReplaceFileExtension(fileName, internalExt);
            var requiredMimeType = MimeMapping.GetMimeMapping(internalExt);

            var downloadUrl = GoogleLoginProvider.GoogleUrlFile + $"{fileId}/export?mimeType={HttpUtility.UrlEncode(requiredMimeType)}";

            var httpClient = _clientFactory.CreateClient();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(downloadUrl),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", "Bearer " + token);

            _logger.DebugGoogleDriveAppDownloadExportLink(downloadUrl);
            try
            {
                using var response = await httpClient.SendAsync(request);
                using var fileStream = new ResponseStream(response);
                driveFile = await CreateFileAsync(fileStream, fileName, folderId, token);
            }
            catch (HttpRequestException e)
            {
                _logger.ErrorGoogleDriveAppDownLoadExportLink(e);

                if (e.StatusCode == HttpStatusCode.Forbidden || e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
                }
            }
        }
        else
        {
            var convertedUrl = await ConvertFileAsync(fileId, ext);

            if (string.IsNullOrEmpty(convertedUrl))
            {
                _logger.ErrorGoogleDriveAppConvertUrl(FileSizeComment.FilesSizeToString(jsonFile.Value<int>("size")));

                throw new Exception(FilesCommonResource.ErrorMassage_DocServiceException + " (convert)");
            }

            var toExt = _fileUtility.GetInternalExtension(fileName);
            fileName = FileUtility.ReplaceFileExtension(fileName, toExt);
            driveFile = await CreateFileAsync(convertedUrl, fileName, folderId, token);
        }

        jsonFile = JObject.Parse(driveFile);

        return jsonFile.Value<string>("id");
    }


    private string GetCorrectTitle(JToken jsonFile)
    {
        var title = jsonFile.Value<string>("name") ?? "";
        var extTitle = FileUtility.GetFileExtension(title);
        var correctExt = GetCorrectExt(jsonFile);

        if (extTitle != correctExt)
        {
            title += correctExt;
        }

        return title;
    }

    private string GetCorrectExt(JToken jsonFile)
    {
        var mimeType = (jsonFile.Value<string>("mimeType") ?? "").ToLower();

        var ext = MimeMapping.GetExtention(mimeType);
        if (!GoogleLoginProvider.GoogleDriveExt.Contains(ext))
        {
            var title = (jsonFile.Value<string>("name") ?? "").ToLower();
            ext = FileUtility.GetFileExtension(title);

            if (MimeMapping.GetMimeMapping(ext) != mimeType)
            {
                var originalFilename = (jsonFile.Value<string>("originalFilename") ?? "").ToLower();
                ext = FileUtility.GetFileExtension(originalFilename);

                if (MimeMapping.GetMimeMapping(ext) != mimeType)
                {
                    ext = MimeMapping.GetExtention(mimeType);

                    _logger.DebugGoogleDriveAppTryGetCorrectExt(ext, mimeType);
                }
            }
        }

        return ext;
    }
}
