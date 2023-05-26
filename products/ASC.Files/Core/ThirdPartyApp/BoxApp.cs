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


namespace ASC.Web.Files.ThirdPartyApp;

[Scope]
public class BoxApp : Consumer, IThirdPartyApp, IOAuthProvider
{
    public const string AppAttr = "box";

    private const string _boxUrlUserInfo = "https://api.box.com/2.0/users/me";
    private const string _boxUrlFile = "https://api.box.com/2.0/files/{fileId}";
    private const string _boxUrlUpload = "https://upload.box.com/api/2.0/files/{fileId}/content";
    public string Scopes => string.Empty;
    public string CodeUrl => string.Empty;
    public string AccessTokenUrl => "https://www.box.com/api/oauth2/token";
    public string RedirectUri => string.Empty;
    public string ClientID => this["boxAppClientId"];
    public string ClientSecret => this["boxAppSecretKey"];
    public bool IsEnabled => !string.IsNullOrEmpty(ClientID) && !string.IsNullOrEmpty(ClientSecret);

    private readonly ILogger<BoxApp> _logger;
    private readonly PathProvider _pathProvider;
    private readonly TenantUtil _tenantUtil;
    private readonly AuthContext _authContext;
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly CookiesManager _cookiesManager;
    private readonly Global _global;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly SettingsManager _settingsManager;
    private readonly PersonalSettingsHelper _personalSettingsHelper;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly AccountLinker _accountLinker;
    private readonly SetupInfo _setupInfo;
    private readonly TokenHelper _tokenHelper;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly ThirdPartyAppHandlerService _thirdPartyAppHandlerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _clientFactory;
    private readonly RequestHelper _requestHelper;
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public BoxApp() { }

    public BoxApp(
        PathProvider pathProvider,
        TenantUtil tenantUtil,
        ILogger<BoxApp> logger,
        AuthContext authContext,
        SecurityContext securityContext,
        UserManager userManager,
        UserManagerWrapper userManagerWrapper,
        CookiesManager cookiesManager,
        Global global,
        EmailValidationKeyProvider emailValidationKeyProvider,
        FilesLinkUtility filesLinkUtility,
        SettingsManager settingsManager,
        PersonalSettingsHelper personalSettingsHelper,
        BaseCommonLinkUtility baseCommonLinkUtility,
        AccountLinker accountLinker,
        SetupInfo setupInfo,
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
        RequestHelper requestHelper,
        OAuth20TokenHelper oAuth20TokenHelper,
        string name, int order, Dictionary<string, string> additional)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, additional)
    {
        _pathProvider = pathProvider;
        _tenantUtil = tenantUtil;
        _authContext = authContext;
        _securityContext = securityContext;
        _userManager = userManager;
        _userManagerWrapper = userManagerWrapper;
        _cookiesManager = cookiesManager;
        _global = global;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _filesLinkUtility = filesLinkUtility;
        _settingsManager = settingsManager;
        _personalSettingsHelper = personalSettingsHelper;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _accountLinker = accountLinker;
        _setupInfo = setupInfo;
        _tokenHelper = tokenHelper;
        _documentServiceConnector = documentServiceConnector;
        _thirdPartyAppHandlerService = thirdPartyAppHandlerService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _clientFactory = clientFactory;
        _requestHelper = requestHelper;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public async Task<bool> RequestAsync(HttpContext context)
    {
        if ((context.Request.Query[FilesLinkUtility.Action].FirstOrDefault() ?? "").Equals("stream", StringComparison.InvariantCultureIgnoreCase))
        {
            await StreamFileAsync(context);

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
        _logger.DebugBoxAppGetFile(fileId);
        fileId = ThirdPartySelector.GetFileId(fileId);

        var token = await _tokenHelper.GetTokenAsync(AppAttr);

        var boxFile = GetBoxFile(fileId, token);
        var editable = true;

        if (boxFile == null)
        {
            return (null, editable);
        }

        var jsonFile = JObject.Parse(boxFile);

        var file = _serviceProvider.GetService<File<string>>();
        file.Id = ThirdPartySelector.BuildAppFileId(AppAttr, jsonFile.Value<string>("id"));
        file.Title = Global.ReplaceInvalidCharsAndTruncate(jsonFile.Value<string>("name"));
        file.CreateOn = _tenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("created_at"));
        file.ModifiedOn = _tenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("modified_at"));
        file.ContentLength = Convert.ToInt64(jsonFile.Value<string>("size"));
        file.ProviderKey = "Box";

        var modifiedBy = jsonFile.Value<JObject>("modified_by");
        if (modifiedBy != null)
        {
            file.ModifiedByString = modifiedBy.Value<string>("name");
        }

        var createdBy = jsonFile.Value<JObject>("created_by");
        if (createdBy != null)
        {
            file.CreateByString = createdBy.Value<string>("name");
        }


        var locked = jsonFile.Value<JObject>("lock");
        if (locked != null)
        {
            var lockedBy = locked.Value<JObject>("created_by");
            if (lockedBy != null)
            {
                var lockedUserId = lockedBy.Value<string>("id");
                _logger.DebugBoxAppLockedBy(lockedUserId);

                editable = await CurrentUserAsync(lockedUserId);
            }
        }

        return (file, editable);
    }

    public string GetFileStreamUrl(File<string> file)
    {
        if (file == null)
        {
            return string.Empty;
        }

        var fileId = ThirdPartySelector.GetFileId(file.Id);

        _logger.DebugBoxAppGetFileStreamUrl(fileId);

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
        _logger.DebugBoxAppSaveFileStream(fileId, stream == null ? downloadUrl : "stream");
        fileId = ThirdPartySelector.GetFileId(fileId);

        var token = await _tokenHelper.GetTokenAsync(AppAttr);

        var boxFile = GetBoxFile(fileId, token);
        if (boxFile == null)
        {
            _logger.ErrorBoxAppFileIsNull();

            throw new Exception("File not found");
        }

        var jsonFile = JObject.Parse(boxFile);
        var title = Global.ReplaceInvalidCharsAndTruncate(jsonFile.Value<string>("name"));
        var currentType = FileUtility.GetFileExtension(title);
        if (!fileType.Equals(currentType))
        {
            try
            {
                if (stream != null)
                {
                    downloadUrl = await _pathProvider.GetTempUrlAsync(stream, fileType);
                    downloadUrl = await _documentServiceConnector.ReplaceCommunityAdressAsync(downloadUrl);
                }

                _logger.DebugBoxAppGetConvertedUri(fileType, currentType, downloadUrl);

                var key = DocumentServiceConnector.GenerateRevisionId(downloadUrl);

                var resultTuple = await _documentServiceConnector.GetConvertedUriAsync(downloadUrl, fileType, currentType, key, null, CultureInfo.CurrentUICulture.Name, null, null, false);
                downloadUrl = resultTuple.ConvertedDocumentUri;

                stream = null;
            }
            catch (Exception e)
            {
                _logger.ErrorBoxAppConvert(e);
            }
        }

        var httpClient = _clientFactory.CreateClient();

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_boxUrlUpload.Replace("{fileId}", fileId))
        };

        StreamContent streamContent;

        using var multipartFormContent = new MultipartFormDataContent();

        if (stream != null)
        {
            streamContent = new StreamContent(stream);
        }
        else
        {
            var downloadRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(downloadUrl)
            };
            var response = await httpClient.SendAsync(downloadRequest);
            var downloadStream = new ResponseStream(response);

            streamContent = new StreamContent(downloadStream);
        }

        streamContent.Headers.TryAddWithoutValidation("Content-Type", MimeMapping.GetMimeMapping(title));
        multipartFormContent.Add(streamContent, name: "filename", fileName: title);

        request.Content = multipartFormContent;
        request.Method = HttpMethod.Post;
        request.Headers.Add("Authorization", "Bearer " + token);
        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data; boundary=" + boundary);
        //_logger.DebugBoxAppSaveFileTotalSize(tmpStream.Length);

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

            _logger.DebugBoxAppSaveFileResponse(result);
        }
        catch (HttpRequestException e)
        {
            _logger.ErrorBoxAppSaveFile(e);
            if (e.StatusCode == HttpStatusCode.Forbidden || e.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
            }

            throw;
        }
    }


    private async Task RequestCodeAsync(HttpContext context)
    {
        var token = GetToken(context.Request.Query["code"]);
        if (token == null)
        {
            _logger.ErrorBoxAppTokenIsNull();

            throw new SecurityException("Access token is null");
        }

        var boxUserId = context.Request.Query["userId"];

        if (_authContext.IsAuthenticated)
        {
            if (!(await CurrentUserAsync(boxUserId)))
            {
                _logger.DebugBoxAppLogout(boxUserId);
                _cookiesManager.ClearCookies(CookiesType.AuthKey);
                _authContext.Logout();
            }
        }

        if (!_authContext.IsAuthenticated)
        {
            var wrapper = await GetUserInfo(token);
            var userInfo = wrapper.UserInfo;
            var isNew = wrapper.IsNew;

            if (userInfo == null)
            {
                _logger.ErrorBoxAppUserInfoIsNull();

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

            if (!string.IsNullOrEmpty(boxUserId) && !(await CurrentUserAsync(boxUserId)))
            {
                await AddLinkerAsync(boxUserId);
            }
        }

        await _tokenHelper.SaveTokenAsync(token);

        var fileId = context.Request.Query["id"];

        context.Response.Redirect(_filesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
    }

    private async Task StreamFileAsync(HttpContext context)
    {
        try
        {
            var fileId = context.Request.Query[FilesLinkUtility.FileId];
            var auth = context.Request.Query[FilesLinkUtility.AuthKey];
            var userId = context.Request.Query[CommonLinkUtility.ParamName_UserUserID];

            _logger.DebugBoxAppGetFileStream(fileId);

            var validateResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(fileId + userId, auth, _global.StreamUrlExpire);
            if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
            {
                var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                _logger.ErrorBoxAppValidateError(FilesLinkUtility.AuthKey, validateResult, context.Request.Url(), exc);

                throw exc;
            }

            Token token = null;

            if (Guid.TryParse(userId, out var userIdGuid))
            {
                token = await _tokenHelper.GetTokenAsync(AppAttr, userIdGuid);
            }

            if (token == null)
            {
                _logger.ErrorBoxAppTokenIsNull();

                throw new SecurityException("Access token is null");
            }

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_boxUrlFile.Replace("{fileId}", fileId) + "/content"),
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
            _logger.ErrorBoxAppErrorRequest(context.Request.Url(), ex);
        }

        try
        {
            await context.Response.Body.FlushAsync();
            //TODO
            //context.Response.Body.SuppressContent = true;
            //context.ApplicationInstance.CompleteRequest();
        }
        catch (HttpException ex)
        {
            _logger.ErrorBoxAppStreamFile(ex);
        }
    }

    private async Task<bool> CurrentUserAsync(string boxUserId)
    {
        var linkedProfiles = await _accountLinker.GetLinkedObjectsByHashIdAsync(HashHelper.MD5($"{ProviderConstants.Box}/{boxUserId}"));

        return linkedProfiles.Any(profileId => Guid.TryParse(profileId, out var tmp) && tmp == _authContext.CurrentAccount.ID);
    }

    private async Task AddLinkerAsync(string boxUserId)
    {
        _logger.DebugBoxAppAddLinker(boxUserId);

        await _accountLinker.AddLinkAsync(_authContext.CurrentAccount.ID.ToString(), boxUserId, ProviderConstants.Box);
    }

    private async Task<UserInfoWrapper> GetUserInfo(Token token)
    {
        var wrapper = new UserInfoWrapper();
        if (token == null)
        {
            _logger.ErrorBoxAppTokenIsNull();

            throw new SecurityException("Access token is null");
        }

        var resultResponse = string.Empty;
        try
        {
            resultResponse = _requestHelper.PerformRequest(_boxUrlUserInfo,
                                                          headers: new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
            _logger.DebugBoxAppUserInfoResponse(resultResponse);
        }
        catch (Exception ex)
        {
            _logger.ErrorBoxAppUserinfoRequest(ex);
        }

        var boxUserInfo = JObject.Parse(resultResponse);
        if (boxUserInfo == null)
        {
            _logger.ErrorInUserInfoRequest();

            return null;
        }

        var email = boxUserInfo.Value<string>("login");
        var userInfo = await _userManager.GetUserByEmailAsync(email);
        if (Equals(userInfo, Constants.LostUser))
        {
            userInfo = new UserInfo
            {
                FirstName = boxUserInfo.Value<string>("name"),
                Email = email,
                MobilePhone = boxUserInfo.Value<string>("phone"),
            };

            var cultureName = boxUserInfo.Value<string>("language");
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }

            var cultureInfo = _setupInfo.EnabledCultures.Find(c => string.Equals(c.TwoLetterISOLanguageName, cultureName, StringComparison.InvariantCultureIgnoreCase));
            if (cultureInfo != null)
            {
                userInfo.CultureName = cultureInfo.Name;
            }
            else
            {
                _logger.DebugBoxAppFromBoxAppNewPersonalUser(userInfo.Email, cultureName);
            }

            if (string.IsNullOrEmpty(userInfo.FirstName))
            {
                userInfo.FirstName = FilesCommonResource.UnknownFirstName;
            }
            if (string.IsNullOrEmpty(userInfo.LastName))
            {
                userInfo.LastName = FilesCommonResource.UnknownLastName;
            }

            try
            {
                await _securityContext.AuthenticateMeWithoutCookieAsync(ASC.Core.Configuration.Constants.CoreSystem);
                userInfo = await _userManagerWrapper.AddUserAsync(userInfo, UserManagerWrapper.GeneratePassword());
            }
            finally
            {
                _authContext.Logout();
            }

            wrapper.IsNew = true;

            _logger.DebugBoxAppNewUser(userInfo.Id);
        }

        wrapper.UserInfo = userInfo;
        return wrapper;
    }

    private string GetBoxFile(string boxFileId, Token token)
    {
        if (token == null)
        {
            _logger.ErrorBoxAppTokenIsNull();

            throw new SecurityException("Access token is null");
        }

        try
        {
            var resultResponse = _requestHelper.PerformRequest(_boxUrlFile.Replace("{fileId}", boxFileId),
                                                              headers: new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
            _logger.DebugBoxAppFileResponse(resultResponse);

            return resultResponse;
        }
        catch (Exception ex)
        {
            _logger.ErrorBoxAppFileRequest(ex);
        }
        return null;
    }

    private Token GetToken(string code)
    {
        try
        {
            _logger.DebugBoxAppGetAccessTokenByCode(code);
            var token = _oAuth20TokenHelper.GetAccessToken<BoxApp>(ConsumerFactory, code);

            return new Token(token, AppAttr);
        }
        catch (Exception ex)
        {
            _logger.ErrorGetToken(ex);
        }

        return null;
    }
}
