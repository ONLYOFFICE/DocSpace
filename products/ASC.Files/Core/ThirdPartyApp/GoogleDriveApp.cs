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
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Web;

using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.FederatedLogin.Profile;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core;
using ASC.Web.Files.HttpHandlers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using MimeMapping = ASC.Common.Web.MimeMapping;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.ThirdPartyApp
{
    public class GoogleDriveApp : Consumer, IThirdPartyApp, IOAuthProvider
    {
        public const string AppAttr = "gdrive";

        public string Scopes { get { return ""; } }
        public string CodeUrl { get { return ""; } }
        public string AccessTokenUrl { get { return GoogleLoginProvider.Instance.AccessTokenUrl; } }
        public string RedirectUri { get { return this["googleDriveAppRedirectUrl"]; } }
        public string ClientID { get { return this["googleDriveAppClientId"]; } }
        public string ClientSecret { get { return this["googleDriveAppSecretKey"]; } }

        public bool IsEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(ClientID) &&
                       !string.IsNullOrEmpty(ClientSecret);
            }
        }

        public ILog Logger { get; }
        private PathProvider PathProvider { get; }
        private TenantUtil TenantUtil { get; }
        private AuthContext AuthContext { get; }
        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
        private UserManagerWrapper UserManagerWrapper { get; }
        private CookiesManager CookiesManager { get; }
        private MessageService MessageService { get; }
        private Global Global { get; }
        private GlobalStore GlobalStore { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private SettingsManager SettingsManager { get; }
        private PersonalSettingsHelper PersonalSettingsHelper { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private FileUtility FileUtility { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private IOptionsSnapshot<AccountLinker> Snapshot { get; }
        private SetupInfo SetupInfo { get; }
        private GoogleLoginProvider GoogleLoginProvider { get; }
        private TokenHelper TokenHelper { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private ThirdPartyAppHandlerService ThirdPartyAppHandlerService { get; }
        private IServiceProvider ServiceProvider { get; }

        public GoogleDriveApp()
        {
        }

        public GoogleDriveApp(
            PathProvider pathProvider,
            TenantUtil tenantUtil,
            AuthContext authContext,
            SecurityContext securityContext,
            UserManager userManager,
            UserManagerWrapper userManagerWrapper,
            CookiesManager cookiesManager,
            MessageService messageService,
            Global global,
            GlobalStore globalStore,
            EmailValidationKeyProvider emailValidationKeyProvider,
            FilesLinkUtility filesLinkUtility,
            SettingsManager settingsManager,
            PersonalSettingsHelper personalSettingsHelper,
            BaseCommonLinkUtility baseCommonLinkUtility,
            IOptionsMonitor<ILog> option,
            FileUtility fileUtility,
            FilesSettingsHelper filesSettingsHelper,
            IOptionsSnapshot<AccountLinker> snapshot,
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
            string name, int order, Dictionary<string, string> additional)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, additional)
        {
            Logger = option.CurrentValue;
            PathProvider = pathProvider;
            TenantUtil = tenantUtil;
            AuthContext = authContext;
            SecurityContext = securityContext;
            UserManager = userManager;
            UserManagerWrapper = userManagerWrapper;
            CookiesManager = cookiesManager;
            MessageService = messageService;
            Global = global;
            GlobalStore = globalStore;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            FilesLinkUtility = filesLinkUtility;
            SettingsManager = settingsManager;
            PersonalSettingsHelper = personalSettingsHelper;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            FileUtility = fileUtility;
            FilesSettingsHelper = filesSettingsHelper;
            Snapshot = snapshot;
            SetupInfo = setupInfo;
            GoogleLoginProvider = googleLoginProvider;
            TokenHelper = tokenHelper;
            DocumentServiceConnector = documentServiceConnector;
            ThirdPartyAppHandlerService = thirdPartyAppHandlerService;
            ServiceProvider = serviceProvider;
        }

        public bool Request(HttpContext context)
        {
            switch ((context.Request.Query[FilesLinkUtility.Action].FirstOrDefault() ?? "").ToLower())
            {
                case "stream":
                    StreamFile(context);
                    return true;
                case "convert":
                    ConfirmConvertFile(context);
                    return true;
                case "create":
                    CreateFile(context);
                    return true;
            }

            if (!string.IsNullOrEmpty(context.Request.Query["code"]))
            {
                RequestCode(context);
                return true;
            }

            return false;
        }

        public string GetRefreshUrl()
        {
            return AccessTokenUrl;
        }

        public File<string> GetFile(string fileId, out bool editable)
        {
            Logger.Debug("GoogleDriveApp: get file " + fileId);
            fileId = ThirdPartySelector.GetFileId(fileId);

            var token = TokenHelper.GetToken(AppAttr);
            var driveFile = GetDriveFile(fileId, token);
            editable = false;

            if (driveFile == null) return null;

            var jsonFile = JObject.Parse(driveFile);

            var file = ServiceProvider.GetService<File<string>>();
            file.ID = ThirdPartySelector.BuildAppFileId(AppAttr, jsonFile.Value<string>("id"));
            file.Title = Global.ReplaceInvalidCharsAndTruncate(GetCorrectTitle(jsonFile));
            file.CreateOn = TenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("createdTime"));
            file.ModifiedOn = TenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("modifiedTime"));
            file.ContentLength = Convert.ToInt64(jsonFile.Value<string>("size"));
            file._modifiedByString = jsonFile["lastModifyingUser"]["displayName"].Value<string>();
            file.ProviderKey = "Google";

            var owners = jsonFile["owners"];
            if (owners != null)
            {
                file._createByString = owners[0]["displayName"].Value<string>();
            }

            editable = jsonFile["capabilities"]["canEdit"].Value<bool>();
            return file;
        }

        public string GetFileStreamUrl(File<string> file)
        {
            if (file == null) return string.Empty;

            var fileId = ThirdPartySelector.GetFileId(file.ID.ToString());
            return GetFileStreamUrl(fileId);
        }

        private string GetFileStreamUrl(string fileId)
        {
            Logger.Debug("GoogleDriveApp: get file stream url " + fileId);

            var uriBuilder = new UriBuilder(BaseCommonLinkUtility.GetFullAbsolutePath(ThirdPartyAppHandlerService.HandlerPath));
            if (uriBuilder.Uri.IsLoopback)
            {
                uriBuilder.Host = Dns.GetHostName();
            }
            var query = uriBuilder.Query;
            query += FilesLinkUtility.Action + "=stream&";
            query += FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(fileId) + "&";
            query += CommonLinkUtility.ParamName_UserUserID + "=" + HttpUtility.UrlEncode(AuthContext.CurrentAccount.ID.ToString()) + "&";
            query += FilesLinkUtility.AuthKey + "=" + EmailValidationKeyProvider.GetEmailKey(fileId + AuthContext.CurrentAccount.ID) + "&";
            query += ThirdPartySelector.AppAttr + "=" + AppAttr;

            return uriBuilder.Uri + "?" + query;
        }

        public void SaveFile(string fileId, string fileType, string downloadUrl, Stream stream)
        {
            Logger.Debug("GoogleDriveApp: save file stream " + fileId +
                                (stream == null
                                     ? " from - " + downloadUrl
                                     : " from stream"));
            fileId = ThirdPartySelector.GetFileId(fileId);

            var token = TokenHelper.GetToken(AppAttr);

            var driveFile = GetDriveFile(fileId, token);
            if (driveFile == null)
            {
                Logger.Error("GoogleDriveApp: file is null");
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
                        downloadUrl = PathProvider.GetTempUrl(stream, fileType);
                        downloadUrl = DocumentServiceConnector.ReplaceCommunityAdress(downloadUrl);
                    }

                    Logger.Debug("GoogleDriveApp: GetConvertedUri from " + fileType + " to " + currentType + " - " + downloadUrl);

                    var key = DocumentServiceConnector.GenerateRevisionId(downloadUrl);
                    DocumentServiceConnector.GetConvertedUri(downloadUrl, fileType, currentType, key, null, null, null, false, out downloadUrl);
                    stream = null;
                }
                catch (Exception e)
                {
                    Logger.Error("GoogleDriveApp: Error convert", e);
                }
            }

            var request = (HttpWebRequest)WebRequest.Create(GoogleLoginProvider.GoogleUrlFileUpload + "/{fileId}?uploadType=media".Replace("{fileId}", fileId));
            request.Method = "PATCH";
            request.Headers.Add("Authorization", "Bearer " + token);
            request.ContentType = MimeMapping.GetMimeMapping(currentType);

            if (stream != null)
            {
                request.ContentLength = stream.Length;

                const int bufferSize = 2048;
                var buffer = new byte[bufferSize];
                int readed;
                while ((readed = stream.Read(buffer, 0, bufferSize)) > 0)
                {
                    request.GetRequestStream().Write(buffer, 0, readed);
                }
            }
            else
            {
                var downloadRequest = (HttpWebRequest)WebRequest.Create(downloadUrl);
                using var downloadStream = new ResponseStream(downloadRequest.GetResponse());
                request.ContentLength = downloadStream.Length;

                const int bufferSize = 2048;
                var buffer = new byte[bufferSize];
                int readed;
                while ((readed = downloadStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    request.GetRequestStream().Write(buffer, 0, readed);
                }
            }

            try
            {
                using var response = request.GetResponse();
                using var responseStream = response.GetResponseStream();
                string result = null;
                if (responseStream != null)
                {
                    using var readStream = new StreamReader(responseStream);
                    result = readStream.ReadToEnd();
                }

                Logger.Debug("GoogleDriveApp: save file stream response - " + result);
            }
            catch (WebException e)
            {
                Logger.Error("GoogleDriveApp: Error save file stream", e);
                request.Abort();
                var httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse.StatusCode == HttpStatusCode.Forbidden || httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
                }
                throw;
            }
        }


        private void RequestCode(HttpContext context)
        {
            var state = context.Request.Query["state"];
            Logger.Debug("GoogleDriveApp: state - " + state);
            if (string.IsNullOrEmpty(state))
            {
                Logger.Error("GoogleDriveApp: empty state");
                throw new Exception("Empty state");
            }

            var token = GetToken(context.Request.Query["code"]);
            if (token == null)
            {
                Logger.Error("GoogleDriveApp: token is null");
                throw new SecurityException("Access token is null");
            }

            var stateJson = JObject.Parse(state);

            var googleUserId = stateJson.Value<string>("userId");

            if (AuthContext.IsAuthenticated)
            {
                if (!CurrentUser(googleUserId))
                {
                    Logger.Debug("GoogleDriveApp: logout for " + googleUserId);
                    CookiesManager.ClearCookies(CookiesType.AuthKey);
                    AuthContext.Logout();
                }
            }

            if (!AuthContext.IsAuthenticated)
            {
                var userInfo = GetUserInfo(token, out var isNew);

                if (userInfo == null)
                {
                    Logger.Error("GoogleDriveApp: UserInfo is null");
                    throw new Exception("Profile is null");
                }

                var cookiesKey = SecurityContext.AuthenticateMe(userInfo.ID);
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                MessageService.Send(MessageAction.LoginSuccessViaSocialApp);

                if (isNew)
                {
                    var userHelpTourSettings = SettingsManager.LoadForCurrentUser<UserHelpTourSettings>();
                    userHelpTourSettings.IsNewUser = true;
                    SettingsManager.SaveForCurrentUser(userHelpTourSettings);

                    PersonalSettingsHelper.IsNewUser = true;
                    PersonalSettingsHelper.IsNotActivated = true;
                }

                if (!string.IsNullOrEmpty(googleUserId) && !CurrentUser(googleUserId))
                {
                    AddLinker(googleUserId);
                }
            }

            TokenHelper.SaveToken(token);

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
                        Logger.Error("GoogleDriveApp: ids is empty");
                        throw new Exception("File id is null");
                    }
                    var fileId = idsArray.ToObject<List<string>>().FirstOrDefault();

                    var driveFile = GetDriveFile(fileId, token);
                    if (driveFile == null)
                    {
                        Logger.Error("GoogleDriveApp: file is null");
                        throw new Exception("File not found");
                    }

                    var jsonFile = JObject.Parse(driveFile);
                    var ext = GetCorrectExt(jsonFile);
                    if (FileUtility.ExtsMustConvert.Contains(ext)
                        || GoogleLoginProvider.GoogleDriveExt.Contains(ext))
                    {
                        Logger.Debug("GoogleDriveApp: file must be converted");
                        if (FilesSettingsHelper.ConvertNotify)
                        {
                            //context.Response.Redirect(App.Location + "?" + FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(fileId), true);
                            return;
                        }

                        fileId = CreateConvertedFile(driveFile, token);
                    }

                    context.Response.Redirect(FilesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
                    return;
            }
            Logger.Error("GoogleDriveApp: Action not identified");
            throw new Exception("Action not identified");
        }

        private void StreamFile(HttpContext context)
        {
            try
            {
                var fileId = context.Request.Query[FilesLinkUtility.FileId];
                var auth = context.Request.Query[FilesLinkUtility.AuthKey];
                var userId = context.Request.Query[CommonLinkUtility.ParamName_UserUserID];

                Logger.Debug("GoogleDriveApp: get file stream " + fileId);

                var validateResult = EmailValidationKeyProvider.ValidateEmailKey(fileId + userId, auth, Global.StreamUrlExpire);
                if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                    Logger.Error(string.Format("GoogleDriveApp: validate error {0} {1}: {2}", FilesLinkUtility.AuthKey, validateResult, context.Request.Url()), exc);

                    throw exc;
                }

                Token token = null;

                if (Guid.TryParse(userId, out var userIdGuid))
                {
                    token = TokenHelper.GetToken(AppAttr, userIdGuid);
                }

                if (token == null)
                {
                    Logger.Error("BoxApp: token is null");
                    throw new SecurityException("Access token is null");
                }

                var driveFile = GetDriveFile(fileId, token);

                var jsonFile = JObject.Parse(driveFile);

                var downloadUrl = GoogleLoginProvider.GoogleUrlFile + fileId + "?alt=media";

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    Logger.Error("GoogleDriveApp: downloadUrl is null");
                    throw new Exception("downloadUrl is null");
                }

                Logger.Debug("GoogleDriveApp: get file stream downloadUrl - " + downloadUrl);

                var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);

                using var response = request.GetResponse();
                using var stream = new ResponseStream(response);
                stream.CopyTo(context.Response.Body);

                var contentLength = jsonFile.Value<string>("size");
                Logger.Debug("GoogleDriveApp: get file stream contentLength - " + contentLength);
                context.Response.Headers.Add("Content-Length", contentLength);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.WriteAsync(ex.Message).Wait();
                Logger.Error("GoogleDriveApp: Error request " + context.Request.Url(), ex);
            }
            try
            {
                context.Response.Body.Flush();
                //TODO
                //context.Response.SuppressContent = true;
                //context.ApplicationInstance.CompleteRequest();
            }
            catch (HttpException ex)
            {
                Logger.Error("GoogleDriveApp StreamFile", ex);
            }
        }

        private void ConfirmConvertFile(HttpContext context)
        {
            var fileId = context.Request.Query[FilesLinkUtility.FileId];
            Logger.Debug("GoogleDriveApp: ConfirmConvertFile - " + fileId);

            var token = TokenHelper.GetToken(AppAttr);

            var driveFile = GetDriveFile(fileId, token);
            if (driveFile == null)
            {
                Logger.Error("GoogleDriveApp: file is null");
                throw new Exception("File not found");
            }

            fileId = CreateConvertedFile(driveFile, token);

            context.Response.Redirect(FilesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
        }

        private void CreateFile(HttpContext context)
        {
            var folderId = context.Request.Query[FilesLinkUtility.FolderId];
            var fileName = context.Request.Query[FilesLinkUtility.FileTitle];
            Logger.Debug("GoogleDriveApp: CreateFile folderId - " + folderId + " fileName - " + fileName);

            var token = TokenHelper.GetToken(AppAttr);

            var culture = UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture();
            var storeTemplate = GlobalStore.GetStoreTemplate();

            var path = FileConstant.NewDocPath + culture + "/";
            if (!storeTemplate.IsDirectory(path))
            {
                path = FileConstant.NewDocPath + "default/";
            }
            var ext = FileUtility.InternalExtension[FileUtility.GetFileTypeByFileName(fileName)];
            path += "new" + ext;
            fileName = FileUtility.ReplaceFileExtension(fileName, ext);

            string driveFile;
            using (var content = storeTemplate.GetReadStream("", path))
            {
                driveFile = CreateFile(content, fileName, folderId, token);
            }
            if (driveFile == null)
            {
                Logger.Error("GoogleDriveApp: file is null");
                throw new Exception("File not created");
            }

            var jsonFile = JObject.Parse(driveFile);
            var fileId = jsonFile.Value<string>("id");

            context.Response.Redirect(FilesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
        }

        private Token GetToken(string code)
        {
            try
            {
                Logger.Debug("GoogleDriveApp: GetAccessToken by code " + code);
                var token = OAuth20TokenHelper.GetAccessToken<GoogleDriveApp>(ConsumerFactory, code);
                return new Token(token, AppAttr);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        private bool CurrentUser(string googleId)
        {
            var linker = Snapshot.Get("webstudio");
            var linkedProfiles = linker.GetLinkedObjectsByHashId(HashHelper.MD5(string.Format("{0}/{1}", ProviderConstants.Google, googleId)));
            linkedProfiles = linkedProfiles.Concat(linker.GetLinkedObjectsByHashId(HashHelper.MD5(string.Format("{0}/{1}", ProviderConstants.OpenId, googleId))));
            return linkedProfiles.Any(profileId => Guid.TryParse(profileId, out var tmp) && tmp == AuthContext.CurrentAccount.ID);
        }

        private void AddLinker(string googleUserId)
        {
            Logger.Debug("GoogleDriveApp: AddLinker " + googleUserId);
            var linker = Snapshot.Get("webstudio");
            linker.AddLink(AuthContext.CurrentAccount.ID.ToString(), googleUserId, ProviderConstants.Google);
        }

        private UserInfo GetUserInfo(Token token, out bool isNew)
        {
            isNew = false;
            if (token == null)
            {
                Logger.Error("GoogleDriveApp: token is null");
                throw new SecurityException("Access token is null");
            }

            LoginProfile loginProfile = null;
            try
            {
                loginProfile = GoogleLoginProvider.Instance.GetLoginProfile(token.GetRefreshedToken(TokenHelper));
            }
            catch (Exception ex)
            {
                Logger.Error("GoogleDriveApp: userinfo request", ex);
            }

            if (loginProfile == null)
            {
                Logger.Error("Error in userinfo request");
                return null;
            }

            var userInfo = UserManager.GetUserByEmail(loginProfile.EMail);
            if (Equals(userInfo, Constants.LostUser))
            {
                userInfo = loginProfile.ProfileToUserInfo(CoreBaseSettings);

                var cultureName = loginProfile.Locale;
                if (string.IsNullOrEmpty(cultureName))
                    cultureName = Thread.CurrentThread.CurrentUICulture.Name;

                var cultureInfo = SetupInfo.EnabledCultures.Find(c => string.Equals(c.Name, cultureName, StringComparison.InvariantCultureIgnoreCase));
                if (cultureInfo != null)
                {
                    userInfo.CultureName = cultureInfo.Name;
                }
                else
                {
                    Logger.DebugFormat("From google app new personal user '{0}' without culture {1}", userInfo.Email, cultureName);
                }

                try
                {
                    SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                    userInfo = UserManagerWrapper.AddUser(userInfo, UserManagerWrapper.GeneratePassword());
                }
                finally
                {
                    SecurityContext.Logout();
                }

                isNew = true;

                Logger.Debug("GoogleDriveApp: new user " + userInfo.ID);
            }

            return userInfo;
        }

        private string GetDriveFile(string googleFileId, Token token)
        {
            if (token == null)
            {
                Logger.Error("GoogleDriveApp: token is null");
                throw new SecurityException("Access token is null");
            }
            try
            {
                var requestUrl = GoogleLoginProvider.GoogleUrlFile + googleFileId + "?fields=" + HttpUtility.UrlEncode(GoogleLoginProvider.FilesFields);
                var resultResponse = RequestHelper.PerformRequest(requestUrl,
                                                                  headers: new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
                Logger.Debug("GoogleDriveApp: file response - " + resultResponse);
                return resultResponse;
            }
            catch (Exception ex)
            {
                Logger.Error("GoogleDriveApp: file request", ex);
            }
            return null;
        }

        private string CreateFile(string contentUrl, string fileName, string folderId, Token token)
        {
            if (string.IsNullOrEmpty(contentUrl))
            {
                Logger.Error("GoogleDriveApp: downloadUrl is null");
                throw new Exception("downloadUrl is null");
            }
            Logger.Debug("GoogleDriveApp: create from - " + contentUrl);

            var request = (HttpWebRequest)WebRequest.Create(contentUrl);

            using var content = new ResponseStream(request.GetResponse());
            return CreateFile(content, fileName, folderId, token);
        }

        private string CreateFile(Stream content, string fileName, string folderId, Token token)
        {
            Logger.Debug("GoogleDriveApp: create file");

            var request = (HttpWebRequest)WebRequest.Create(GoogleLoginProvider.GoogleUrlFileUpload + "?uploadType=multipart");

            using (var tmpStream = new MemoryStream())
            {
                var boundary = DateTime.UtcNow.Ticks.ToString("x");

                var folderdata = string.IsNullOrEmpty(folderId) ? "" : string.Format(",\"parents\":[\"{0}\"]", folderId);
                var metadata = string.Format("{{\"name\":\"{0}\"{1}}}", fileName, folderdata);
                var metadataPart = string.Format("\r\n--{0}\r\nContent-Type: application/json; charset=UTF-8\r\n\r\n{1}", boundary, metadata);
                var bytes = Encoding.UTF8.GetBytes(metadataPart);
                tmpStream.Write(bytes, 0, bytes.Length);

                var mediaPartStart = string.Format("\r\n--{0}\r\nContent-Type: {1}\r\n\r\n", boundary, MimeMapping.GetMimeMapping(fileName));
                bytes = Encoding.UTF8.GetBytes(mediaPartStart);
                tmpStream.Write(bytes, 0, bytes.Length);

                content.CopyTo(tmpStream);

                var mediaPartEnd = string.Format("\r\n--{0}--\r\n", boundary);
                bytes = Encoding.UTF8.GetBytes(mediaPartEnd);
                tmpStream.Write(bytes, 0, bytes.Length);

                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + token);
                request.ContentType = "multipart/related; boundary=" + boundary;
                request.ContentLength = tmpStream.Length;
                Logger.Debug("GoogleDriveApp: create file totalSize - " + tmpStream.Length);

                const int bufferSize = 2048;
                var buffer = new byte[bufferSize];
                int readed;
                tmpStream.Seek(0, SeekOrigin.Begin);
                while ((readed = tmpStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    request.GetRequestStream().Write(buffer, 0, readed);
                }
            }

            try
            {
                using var response = request.GetResponse();
                using var responseStream = response.GetResponseStream();
                string result = null;
                if (responseStream != null)
                {
                    using var readStream = new StreamReader(responseStream);
                    result = readStream.ReadToEnd();
                }

                Logger.Debug("GoogleDriveApp: create file response - " + result);
                return result;
            }
            catch (WebException e)
            {
                Logger.Error("GoogleDriveApp: Error create file", e);
                request.Abort();

                var httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse.StatusCode == HttpStatusCode.Forbidden || httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
                }
            }
            return null;
        }

        private string ConvertFile(string fileId, string fromExt)
        {
            Logger.Debug("GoogleDriveApp: convert file");

            var downloadUrl = GetFileStreamUrl(fileId);

            var toExt = FileUtility.GetInternalExtension(fromExt);
            try
            {
                Logger.Debug("GoogleDriveApp: GetConvertedUri- " + downloadUrl);

                var key = DocumentServiceConnector.GenerateRevisionId(downloadUrl);
                DocumentServiceConnector.GetConvertedUri(downloadUrl, fromExt, toExt, key, null, null, null, false, out downloadUrl);
            }
            catch (Exception e)
            {
                Logger.Error("GoogleDriveApp: Error GetConvertedUri", e);
            }

            return downloadUrl;
        }

        private string CreateConvertedFile(string driveFile, Token token)
        {
            var jsonFile = JObject.Parse(driveFile);
            var fileName = GetCorrectTitle(jsonFile);

            var folderId = (string)jsonFile.SelectToken("parents[0]");

            Logger.Info("GoogleDriveApp: create copy - " + fileName);

            var ext = GetCorrectExt(jsonFile);
            var fileId = jsonFile.Value<string>("id");

            if (GoogleLoginProvider.GoogleDriveExt.Contains(ext))
            {
                var internalExt = FileUtility.GetGoogleDownloadableExtension(ext);
                fileName = FileUtility.ReplaceFileExtension(fileName, internalExt);
                var requiredMimeType = MimeMapping.GetMimeMapping(internalExt);

                var downloadUrl = GoogleLoginProvider.GoogleUrlFile
                                  + string.Format("{0}/export?mimeType={1}",
                                                  fileId,
                                                  HttpUtility.UrlEncode(requiredMimeType));

                var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);

                Logger.Debug("GoogleDriveApp: download exportLink - " + downloadUrl);
                try
                {
                    using var fileStream = new ResponseStream(request.GetResponse());
                    driveFile = CreateFile(fileStream, fileName, folderId, token);
                }
                catch (WebException e)
                {
                    Logger.Error("GoogleDriveApp: Error download exportLink", e);
                    request.Abort();

                    var httpResponse = (HttpWebResponse)e.Response;
                    if (httpResponse.StatusCode == HttpStatusCode.Forbidden || httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
                    }
                }
            }
            else
            {
                var convertedUrl = ConvertFile(fileId, ext);

                if (string.IsNullOrEmpty(convertedUrl))
                {
                    Logger.ErrorFormat("GoogleDriveApp: Error convertUrl. size {0}", FileSizeComment.FilesSizeToString(jsonFile.Value<int>("size")));
                    throw new Exception(FilesCommonResource.ErrorMassage_DocServiceException + " (convert)");
                }

                var toExt = FileUtility.GetInternalExtension(fileName);
                fileName = FileUtility.ReplaceFileExtension(fileName, toExt);
                driveFile = CreateFile(convertedUrl, fileName, folderId, token);
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

                        Logger.Debug("GoogleDriveApp: Try GetCorrectExt - " + ext + " for - " + mimeType);
                    }
                }
            }
            return ext;
        }
    }
}