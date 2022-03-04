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
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.HttpHandlers;
using ASC.Web.Files.ThirdPartyApp;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace ASC.Web.Files.Helpers
{
    [Scope]
    public class DocuSignToken
    {
        public ILog Log { get; set; }

        public const string AppAttr = "docusign";

        private TokenHelper TokenHelper { get; }
        private AuthContext AuthContext { get; }
        private ConsumerFactory ConsumerFactory { get; }

        public DocuSignToken(
            TokenHelper tokenHelper,
            IOptionsMonitor<ILog> options,
            AuthContext authContext,
            ConsumerFactory consumerFactory)
        {
            TokenHelper = tokenHelper;
            AuthContext = authContext;
            ConsumerFactory = consumerFactory;
            Log = options.CurrentValue;
        }

        public OAuth20Token GetToken()
        {
            return TokenHelper.GetToken(AppAttr);
        }

        public void DeleteToken(Guid? userId = null)
        {
            TokenHelper.DeleteToken(AppAttr, userId);
        }

        public void SaveToken(OAuth20Token token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            TokenHelper.SaveToken(new Token(token, AppAttr));
        }

        internal string GetRefreshedToken(OAuth20Token token)
        {
            if (token.IsExpired)
            {
                try
                {
                    Log.Info("DocuSign refresh token for user " + AuthContext.CurrentAccount.ID);

                    var refreshed = ConsumerFactory.Get<DocuSignLoginProvider>().RefreshToken(token.RefreshToken);

                    if (refreshed != null)
                    {
                        token.AccessToken = refreshed.AccessToken;
                        token.RefreshToken = refreshed.RefreshToken;
                        token.ExpiresIn = refreshed.ExpiresIn;
                        token.Timestamp = DateTime.UtcNow;

                        SaveToken(token);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("DocuSign refresh token for user " + AuthContext.CurrentAccount.ID, ex);
                }
            }
            return token.AccessToken;
        }
    }

    [Scope]
    public class DocuSignHelper
    {
        public ILog Log { get; set; }

        public const string UserField = "userId";

        public static readonly List<string> SupportedFormats = new List<string>
            {
                ".as", ".asl", ".doc", ".docm", ".docx", ".dot", ".dotm", ".dotx", ".htm", ".html", ".msg", ".pdf", ".pdx", ".rtf", ".txt", ".wpd", ".wps", ".wpt", ".xps",
                ".emz", ".svg", ".svgz", ".vdx", ".vss", ".vst",
                ".bmp", ".cdr", ".dcx", ".gif", ".ico", ".jpg", ".jpeg", ".pct", ".pic", ".png", ".rgb", ".sam", ".tga", ".tif", ".tiff", ".wpg",
                ".dps", ".dpt", ".pot", ".potx", ".pps", ".ppt", ".pptm", ".pptx",
                ".csv", ".et", ".ett", ".xls", ".xlsm", ".xlsx", ".xlt"
            };

        public static readonly long MaxFileSize = 25L * 1024L * 1024L;

        public static readonly int MaxEmailLength = 10000;

        private DocuSignToken DocuSignToken { get; }
        private FileSecurity FileSecurity { get; }
        private IDaoFactory DaoFactory { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private UserManager UserManager { get; }
        private AuthContext AuthContext { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private FileMarker FileMarker { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private FilesMessageService FilesMessageService { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private IServiceProvider ServiceProvider { get; }
        private ConsumerFactory ConsumerFactory { get; }

        public DocuSignHelper(
            DocuSignToken docuSignToken,
            FileSecurity fileSecurity,
            IDaoFactory daoFactory,
            IOptionsMonitor<ILog> options,
            BaseCommonLinkUtility baseCommonLinkUtility,
            UserManager userManager,
            AuthContext authContext,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            FileMarker fileMarker,
            GlobalFolderHelper globalFolderHelper,
            FilesMessageService filesMessageService,
            FilesLinkUtility filesLinkUtility,
            IServiceProvider serviceProvider,
            ConsumerFactory consumerFactory)
        {
            DocuSignToken = docuSignToken;
            FileSecurity = fileSecurity;
            DaoFactory = daoFactory;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            UserManager = userManager;
            AuthContext = authContext;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            FileMarker = fileMarker;
            GlobalFolderHelper = globalFolderHelper;
            FilesMessageService = filesMessageService;
            FilesLinkUtility = filesLinkUtility;
            ServiceProvider = serviceProvider;
            ConsumerFactory = consumerFactory;
            Log = options.CurrentValue;
        }

        public bool ValidateToken(OAuth20Token token)
        {
            GetDocuSignAccount(token);
            return true;
        }

        public async Task<string> SendDocuSignAsync<T>(T fileId, DocuSignData docuSignData, IDictionary<string, StringValues> requestHeaders)
        {
            if (docuSignData == null) throw new ArgumentNullException(nameof(docuSignData));
            var token = DocuSignToken.GetToken();
            var account = GetDocuSignAccount(token);

            var configuration = GetConfiguration(account, token);
            var (document, sourceFile) = await CreateDocumentAsync(fileId, docuSignData.Name, docuSignData.FolderId);

            var url = CreateEnvelope(account.AccountId, document, docuSignData, configuration);

            FilesMessageService.Send(sourceFile, requestHeaders, MessageAction.DocumentSendToSign, "DocuSign", sourceFile.Title);

            return url;
        }

        private DocuSignAccount GetDocuSignAccount(OAuth20Token token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var userInfoString = RequestHelper.PerformRequest(ConsumerFactory.Get<DocuSignLoginProvider>().DocuSignHost + "/oauth/userinfo",
                                                              headers: new Dictionary<string, string> { { "Authorization", "Bearer " + DocuSignToken.GetRefreshedToken(token) } });

            Log.Debug("DocuSing userInfo: " + userInfoString);

            var userInfo = (DocuSignUserInfo)JsonConvert.DeserializeObject(userInfoString, typeof(DocuSignUserInfo));

            if (userInfo.Accounts == null || userInfo.Accounts.Count == 0) throw new Exception("Account is null");

            var account = userInfo.Accounts[0];
            return account;
        }

        private DocuSign.eSign.Client.Configuration GetConfiguration(DocuSignAccount account, OAuth20Token token)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var apiClient = new ApiClient(account.BaseUri + "/restapi");

            var configuration = new DocuSign.eSign.Client.Configuration { ApiClient = apiClient };
            configuration.AddDefaultHeader("Authorization", "Bearer " + DocuSignToken.GetRefreshedToken(token));

            return configuration;
        }

        private async Task<(Document document, File<T> file)> CreateDocumentAsync<T>(T fileId, string documentName, string folderId)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var file = await fileDao.GetFileAsync(fileId);
            if (file == null) throw new Exception(FilesCommonResource.ErrorMassage_FileNotFound);
            if (!await FileSecurity.CanReadAsync(file)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            if (!SupportedFormats.Contains(FileUtility.GetFileExtension(file.Title))) throw new ArgumentException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
            if (file.ContentLength > MaxFileSize) throw new Exception(FileSizeComment.GetFileSizeExceptionString(MaxFileSize));

            byte[] fileBytes;
            using (var stream = await fileDao.GetFileStreamAsync(file))
            {
                var buffer = new byte[16 * 1024];
                using var ms = new MemoryStream();
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await ms.WriteAsync(buffer, 0, read);
                }
                fileBytes = ms.ToArray();
            }

            if (string.IsNullOrEmpty(documentName))
            {
                documentName = file.Title;
            }

            var document = new Document
            {
                DocumentBase64 = Convert.ToBase64String(fileBytes),
                DocumentFields = new List<NameValue>
                            {
                                new NameValue {Name = FilesLinkUtility.FolderId, Value = folderId},
                                new NameValue {Name = FilesLinkUtility.FileTitle, Value = file.Title},
                            },
                DocumentId = "1", //file.ID.ToString(),
                FileExtension = FileUtility.GetFileExtension(file.Title),
                Name = documentName,
            };

            return (document, file);
        }

        private string CreateEnvelope(string accountId, Document document, DocuSignData docuSignData, DocuSign.eSign.Client.Configuration configuration)
        {
            var eventNotification = new EventNotification
            {
                EnvelopeEvents = new List<EnvelopeEvent>
                        {
                            //new EnvelopeEvent {EnvelopeEventStatusCode = DocuSignStatus.Sent.ToString()},
                            //new EnvelopeEvent {EnvelopeEventStatusCode = DocuSignStatus.Delivered.ToString()},
                            new EnvelopeEvent {EnvelopeEventStatusCode = nameof(DocuSignStatus.Completed)},
                            new EnvelopeEvent {EnvelopeEventStatusCode = nameof(DocuSignStatus.Declined)},
                            new EnvelopeEvent {EnvelopeEventStatusCode = nameof(DocuSignStatus.Voided)},
                        },
                IncludeDocumentFields = "true",
                //RecipientEvents = new List<RecipientEvent>
                //    {
                //        new RecipientEvent {RecipientEventStatusCode = "Sent"},
                //        new RecipientEvent {RecipientEventStatusCode = "Delivered"},
                //        new RecipientEvent {RecipientEventStatusCode = "Completed"},
                //        new RecipientEvent {RecipientEventStatusCode = "Declined"},
                //        new RecipientEvent {RecipientEventStatusCode = "AuthenticationFailed"},
                //        new RecipientEvent {RecipientEventStatusCode = "AutoResponded"},
                //    },
                Url = BaseCommonLinkUtility.GetFullAbsolutePath(DocuSignHandlerService.Path(FilesLinkUtility) + "?" + FilesLinkUtility.Action + "=webhook"),
            };

            Log.Debug("DocuSign hook url: " + eventNotification.Url);

            var signers = new List<Signer>();
            docuSignData.Users.ForEach(uid =>
                {
                    try
                    {
                        var user = UserManager.GetUsers(uid);
                        signers.Add(new Signer
                        {
                            Email = user.Email,
                            Name = user.DisplayUserName(false, DisplayUserSettingsHelper),
                            RecipientId = user.ID.ToString(),
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Signer is undefined", ex);
                    }
                });

            var envelopeDefinition = new EnvelopeDefinition
            {
                CustomFields = new CustomFields
                {
                    TextCustomFields = new List<TextCustomField>
                                {
                                    new TextCustomField {Name = UserField, Value = AuthContext.CurrentAccount.ID.ToString()},
                                }
                },
                Documents = new List<Document> { document },
                EmailBlurb = docuSignData.Message,
                EmailSubject = docuSignData.Name,
                EventNotification = eventNotification,
                Recipients = new Recipients
                {
                    Signers = signers,
                },
                Status = "created",
            };

            var envelopesApi = new EnvelopesApi(configuration);
            var envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

            Log.Debug("DocuSign createdEnvelope: " + envelopeSummary.EnvelopeId);

            var envelopeId = envelopeSummary.EnvelopeId;
            var url = envelopesApi.CreateSenderView(accountId, envelopeId, new ReturnUrlRequest
            {
                ReturnUrl = BaseCommonLinkUtility.GetFullAbsolutePath(DocuSignHandlerService.Path(FilesLinkUtility) + "?" + FilesLinkUtility.Action + "=redirect")
            });
            Log.Debug("DocuSign senderView: " + url.Url);

            return url.Url;
        }

        public async Task<File<T>> SaveDocumentAsync<T>(string envelopeId, string documentId, string documentName, T folderId)
        {
            if (string.IsNullOrEmpty(envelopeId)) throw new ArgumentNullException(nameof(envelopeId));
            if (string.IsNullOrEmpty(documentId)) throw new ArgumentNullException(nameof(documentId));

            var token = DocuSignToken.GetToken();
            var account = GetDocuSignAccount(token);
            var configuration = GetConfiguration(account, token);

            var fileDao = DaoFactory.GetFileDao<T>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            if (string.IsNullOrEmpty(documentName))
            {
                documentName = "new.pdf";
            }

            Folder<T> folder;
            if (folderId == null
                || (folder = await folderDao.GetFolderAsync(folderId)) == null
                || folder.RootFolderType == FolderType.TRASH
                || !await FileSecurity.CanCreateAsync(folder))
            {
                if (GlobalFolderHelper.FolderMy != 0)
                {
                    folderId = GlobalFolderHelper.GetFolderMy<T>();
                }
                else
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
                }
            }

            var file = ServiceProvider.GetService<File<T>>();
            file.FolderID = folderId;
            file.Comment = FilesCommonResource.CommentCreateByDocuSign;
            file.Title = FileUtility.ReplaceFileExtension(documentName, ".pdf");

            var envelopesApi = new EnvelopesApi(configuration);
            Log.Info("DocuSign webhook get stream: " + documentId);
            using (var stream = await envelopesApi.GetDocumentAsync(account.AccountId, envelopeId, documentId))
            {
                file.ContentLength = stream.Length;
                file = await fileDao.SaveFileAsync(file, stream);
            }

            FilesMessageService.Send(file, MessageInitiator.ThirdPartyProvider, MessageAction.DocumentSignComplete, "DocuSign", file.Title);

            await FileMarker.MarkAsNewAsync(file);

            return file;
        }


        [DebuggerDisplay("{AccountId} {BaseUri}")]
        private class DocuSignAccount
        {
            [JsonPropertyName("account_id")]
            public string AccountId { get; set; }

            [JsonPropertyName("base_uri")]
            public string BaseUri { get; set; }
        }

        private class DocuSignUserInfo
        {
            public List<DocuSignAccount> Accounts { get; set; }
        }
    }

    [DebuggerDisplay("{Name}")]
    public class DocuSignData
    {
        public string FolderId { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public List<Guid> Users { get; set; }
    }

    public enum DocuSignStatus
    {
        Draft,
        Sent,
        Delivered,
        Completed,
        Declined,
        Voided,
    }
}