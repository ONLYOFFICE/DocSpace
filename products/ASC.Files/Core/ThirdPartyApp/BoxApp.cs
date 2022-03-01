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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.ThirdPartyApp
{
    public class BoxApp : Consumer, IThirdPartyApp, IOAuthProvider
    {
        public const string AppAttr = "box";

        private const string BoxUrlUserInfo = "https://api.box.com/2.0/users/me";
        private const string BoxUrlFile = "https://api.box.com/2.0/files/{fileId}";
        private const string BoxUrlUpload = "https://upload.box.com/api/2.0/files/{fileId}/content";

        public string Scopes { get { return ""; } }
        public string CodeUrl { get { return ""; } }
        public string AccessTokenUrl { get { return "https://www.box.com/api/oauth2/token"; } }
        public string RedirectUri { get { return ""; } }
        public string ClientID { get { return this["boxAppClientId"]; } }
        public string ClientSecret { get { return this["boxAppSecretKey"]; } }

        public bool IsEnabled
        {
            get { return !string.IsNullOrEmpty(ClientID) && !string.IsNullOrEmpty(ClientSecret); }
        }

        private PathProvider PathProvider { get; }
        private TenantUtil TenantUtil { get; }
        private AuthContext AuthContext { get; }
        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
        private UserManagerWrapper UserManagerWrapper { get; }
        private CookiesManager CookiesManager { get; }
        private MessageService MessageService { get; }
        private Global Global { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private SettingsManager SettingsManager { get; }
        private PersonalSettingsHelper PersonalSettingsHelper { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private IOptionsSnapshot<AccountLinker> Snapshot { get; }
        private SetupInfo SetupInfo { get; }
        private TokenHelper TokenHelper { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private ThirdPartyAppHandlerService ThirdPartyAppHandlerService { get; }
        private IServiceProvider ServiceProvider { get; }
        public ILog Logger { get; }
        public IHttpClientFactory ClientFactory { get; }

        public BoxApp()
        {
        }

        public BoxApp(
            PathProvider pathProvider,
            TenantUtil tenantUtil,
            IOptionsMonitor<ILog> option,
            AuthContext authContext,
            SecurityContext securityContext,
            UserManager userManager,
            UserManagerWrapper userManagerWrapper,
            CookiesManager cookiesManager,
            MessageService messageService,
            Global global,
            EmailValidationKeyProvider emailValidationKeyProvider,
            FilesLinkUtility filesLinkUtility,
            SettingsManager settingsManager,
            PersonalSettingsHelper personalSettingsHelper,
            BaseCommonLinkUtility baseCommonLinkUtility,
            IOptionsSnapshot<AccountLinker> snapshot,
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
            string name, int order, Dictionary<string, string> additional)
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, additional)
        {
            PathProvider = pathProvider;
            TenantUtil = tenantUtil;
            AuthContext = authContext;
            SecurityContext = securityContext;
            UserManager = userManager;
            UserManagerWrapper = userManagerWrapper;
            CookiesManager = cookiesManager;
            MessageService = messageService;
            Global = global;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            FilesLinkUtility = filesLinkUtility;
            SettingsManager = settingsManager;
            PersonalSettingsHelper = personalSettingsHelper;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            Snapshot = snapshot;
            SetupInfo = setupInfo;
            TokenHelper = tokenHelper;
            DocumentServiceConnector = documentServiceConnector;
            ThirdPartyAppHandlerService = thirdPartyAppHandlerService;
            ServiceProvider = serviceProvider;
            Logger = option.CurrentValue;
            ClientFactory = clientFactory;
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
            Logger.Debug("BoxApp: get file " + fileId);
            fileId = ThirdPartySelector.GetFileId(fileId);

            var token = TokenHelper.GetToken(AppAttr);

            var boxFile = GetBoxFile(fileId, token);
            editable = true;

            if (boxFile == null) return null;

            var jsonFile = JObject.Parse(boxFile);

            var file = ServiceProvider.GetService<File<string>>();
            file.ID = ThirdPartySelector.BuildAppFileId(AppAttr, jsonFile.Value<string>("id"));
            file.Title = Global.ReplaceInvalidCharsAndTruncate(jsonFile.Value<string>("name"));
            file.CreateOn = TenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("created_at"));
            file.ModifiedOn = TenantUtil.DateTimeFromUtc(jsonFile.Value<DateTime>("modified_at"));
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
                    Logger.Debug("BoxApp: locked by " + lockedUserId);

                    editable = CurrentUser(lockedUserId);
                }
            }

            return file;
        }

        public string GetFileStreamUrl(File<string> file)
        {
            if (file == null) return string.Empty;

            var fileId = ThirdPartySelector.GetFileId(file.ID);

            Logger.Debug("BoxApp: get file stream url " + fileId);

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

        public async Task SaveFileAsync(string fileId, string fileType, string downloadUrl, Stream stream)
        {
            Logger.Debug("BoxApp: save file stream " + fileId +
                                (stream == null
                                     ? " from - " + downloadUrl
                                     : " from stream"));
            fileId = ThirdPartySelector.GetFileId(fileId);

            var token = TokenHelper.GetToken(AppAttr);

            var boxFile = GetBoxFile(fileId, token);
            if (boxFile == null)
            {
                Logger.Error("BoxApp: file is null");
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
                        downloadUrl = await PathProvider.GetTempUrlAsync(stream, fileType);
                        downloadUrl = DocumentServiceConnector.ReplaceCommunityAdress(downloadUrl);
                    }

                    Logger.Debug("BoxApp: GetConvertedUri from " + fileType + " to " + currentType + " - " + downloadUrl);

                    var key = DocumentServiceConnector.GenerateRevisionId(downloadUrl);

                    var resultTuple = await DocumentServiceConnector.GetConvertedUriAsync(downloadUrl, fileType, currentType, key, null, null, null, false);
                    downloadUrl = resultTuple.ConvertedDocumentUri;

                    stream = null;
                }
                catch (Exception e)
                {
                    Logger.Error("BoxApp: Error convert", e);
                }
            }

            var httpClient = ClientFactory.CreateClient();

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(BoxUrlUpload.Replace("{fileId}", fileId));

            using (var tmpStream = new MemoryStream())
            {
                var boundary = DateTime.UtcNow.Ticks.ToString("x");

                var metadata = $"Content-Disposition: form-data; name=\"filename\"; filename=\"{title}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                var metadataPart = $"--{boundary}\r\n{metadata}";
                var bytes = Encoding.UTF8.GetBytes(metadataPart);
                await tmpStream.WriteAsync(bytes, 0, bytes.Length);

                if (stream != null)
                {
                    await stream.CopyToAsync(tmpStream);
                }
                else
                {
                    var downloadRequest = new HttpRequestMessage();
                    downloadRequest.RequestUri = new Uri(downloadUrl);
                    using var response = await httpClient.SendAsync(request);
                    using var downloadStream = new ResponseStream(response);
                    await downloadStream.CopyToAsync(tmpStream);
                }

                var mediaPartEnd = $"\r\n--{boundary}--\r\n";
                bytes = Encoding.UTF8.GetBytes(mediaPartEnd);
                await tmpStream.WriteAsync(bytes, 0, bytes.Length);

                request.Method = HttpMethod.Post;
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data; boundary=" + boundary);
                Logger.Debug("BoxApp: save file totalSize - " + tmpStream.Length);

                tmpStream.Seek(0, SeekOrigin.Begin);
                request.Content = new StreamContent(tmpStream);
            }

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

                Logger.Debug("BoxApp: save file response - " + result);
            }
            catch (HttpRequestException e)
            {
                Logger.Error("BoxApp: Error save file", e);
                if (e.StatusCode == HttpStatusCode.Forbidden || e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException, e);
                }
                throw;
            }
        }


        private void RequestCode(HttpContext context)
        {
            var token = GetToken(context.Request.Query["code"]);
            if (token == null)
            {
                Logger.Error("BoxApp: token is null");
                throw new SecurityException("Access token is null");
            }

            var boxUserId = context.Request.Query["userId"];

            if (AuthContext.IsAuthenticated)
            {
                if (!CurrentUser(boxUserId))
                {
                    Logger.Debug("BoxApp: logout for " + boxUserId);
                    CookiesManager.ClearCookies(CookiesType.AuthKey);
                    AuthContext.Logout();
                }
            }

            if (!AuthContext.IsAuthenticated)
            {
                var userInfo = GetUserInfo(token, out var isNew);

                if (userInfo == null)
                {
                    Logger.Error("BoxApp: UserInfo is null");
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

                if (!string.IsNullOrEmpty(boxUserId) && !CurrentUser(boxUserId))
                {
                    AddLinker(boxUserId);
                }
            }

            TokenHelper.SaveToken(token);

            var fileId = context.Request.Query["id"];

            context.Response.Redirect(FilesLinkUtility.GetFileWebEditorUrl(ThirdPartySelector.BuildAppFileId(AppAttr, fileId)), true);
        }      

        private async Task StreamFileAsync(HttpContext context)
        {
            try
            {
                var fileId = context.Request.Query[FilesLinkUtility.FileId];
                var auth = context.Request.Query[FilesLinkUtility.AuthKey];
                var userId = context.Request.Query[CommonLinkUtility.ParamName_UserUserID];

                Logger.Debug("BoxApp: get file stream " + fileId);

                var validateResult = EmailValidationKeyProvider.ValidateEmailKey(fileId + userId, auth, Global.StreamUrlExpire);
                if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                    Logger.Error(string.Format("BoxApp: validate error {0} {1}: {2}", FilesLinkUtility.AuthKey, validateResult, context.Request.Url()), exc);

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

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(BoxUrlFile.Replace("{fileId}", fileId) + "/content");
                request.Method = HttpMethod.Get;
                request.Headers.Add("Authorization", "Bearer " + token);

                var httpClient = ClientFactory.CreateClient();
                using var response = await httpClient.SendAsync(request);
                using var stream = new ResponseStream(response);
                await stream.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(ex.Message);
                Logger.Error("BoxApp: Error request " + context.Request.Url(), ex);
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
                Logger.Error("BoxApp StreamFile", ex);
            }
        }

        private bool CurrentUser(string boxUserId)
        {
            var linkedProfiles = Snapshot.Get("webstudio")
                .GetLinkedObjectsByHashId(HashHelper.MD5($"{ProviderConstants.Box}/{boxUserId}"));
            return linkedProfiles.Any(profileId => Guid.TryParse(profileId, out var tmp) && tmp == AuthContext.CurrentAccount.ID);
        }

        private void AddLinker(string boxUserId)
        {
            Logger.Debug("BoxApp: AddLinker " + boxUserId);
            var linker = Snapshot.Get("webstudio");
            linker.AddLink(AuthContext.CurrentAccount.ID.ToString(), boxUserId, ProviderConstants.Box);
        }

        private UserInfo GetUserInfo(Token token, out bool isNew)
        {
            isNew = false;
            if (token == null)
            {
                Logger.Error("BoxApp: token is null");
                throw new SecurityException("Access token is null");
            }

            var resultResponse = string.Empty;
            try
            {
                resultResponse = RequestHelper.PerformRequest(BoxUrlUserInfo,
                                                              headers: new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
                Logger.Debug("BoxApp: userinfo response - " + resultResponse);
            }
            catch (Exception ex)
            {
                Logger.Error("BoxApp: userinfo request", ex);
            }

            var boxUserInfo = JObject.Parse(resultResponse);
            if (boxUserInfo == null)
            {
                Logger.Error("Error in userinfo request");
                return null;
            }

            var email = boxUserInfo.Value<string>("login");
            var userInfo = UserManager.GetUserByEmail(email);
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
                    cultureName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                var cultureInfo = SetupInfo.EnabledCultures.Find(c => string.Equals(c.TwoLetterISOLanguageName, cultureName, StringComparison.InvariantCultureIgnoreCase));
                if (cultureInfo != null)
                {
                    userInfo.CultureName = cultureInfo.Name;
                }
                else
                {
                    Logger.DebugFormat("From box app new personal user '{0}' without culture {1}", userInfo.Email, cultureName);
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
                    SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                    userInfo = UserManagerWrapper.AddUser(userInfo, UserManagerWrapper.GeneratePassword());
                }
                finally
                {
                    AuthContext.Logout();
                }

                isNew = true;

                Logger.Debug("BoxApp: new user " + userInfo.ID);
            }

            return userInfo;
        }

        private string GetBoxFile(string boxFileId, Token token)
        {
            if (token == null)
            {
                Logger.Error("BoxApp: token is null");
                throw new SecurityException("Access token is null");
            }

            try
            {
                var resultResponse = RequestHelper.PerformRequest(BoxUrlFile.Replace("{fileId}", boxFileId),
                                                                  headers: new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
                Logger.Debug("BoxApp: file response - " + resultResponse);
                return resultResponse;
            }
            catch (Exception ex)
            {
                Logger.Error("BoxApp: file request", ex);
            }
            return null;
        }

        private Token GetToken(string code)
        {
            try
            {
                Logger.Debug("BoxApp: GetAccessToken by code " + code);
                var token = OAuth20TokenHelper.GetAccessToken<BoxApp>(ConsumerFactory, code);
                return new Token(token, AppAttr);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
    }
}