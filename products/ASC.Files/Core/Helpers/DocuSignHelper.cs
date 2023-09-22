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

using Document = DocuSign.eSign.Model.Document;

namespace ASC.Web.Files.Helpers;

[Scope]
public class DocuSignToken
{
    public const string AppAttr = "docusign";

    private readonly ILogger<DocuSignHelper> _logger;
    private readonly TokenHelper _tokenHelper;
    private readonly AuthContext _authContext;
    private readonly ConsumerFactory _consumerFactory;

    public DocuSignToken(
        TokenHelper tokenHelper,
        ILogger<DocuSignHelper> logger,
        AuthContext authContext,
        ConsumerFactory consumerFactory)
    {
        _tokenHelper = tokenHelper;
        _authContext = authContext;
        _consumerFactory = consumerFactory;
        _logger = logger;
    }

    public async Task<OAuth20Token> GetTokenAsync()
    {
        return await _tokenHelper.GetTokenAsync(AppAttr);
    }

    public async Task DeleteTokenAsync(Guid? userId = null)
    {
        await _tokenHelper.DeleteTokenAsync(AppAttr, userId);
    }

    public async Task SaveTokenAsync(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        await _tokenHelper.SaveTokenAsync(new Token(token, AppAttr));
    }

    internal async Task<string> GetRefreshedTokenAsync(OAuth20Token token)
    {
        if (token.IsExpired)
        {
            try
            {
                _logger.InformationDocuSignRefreshToken(_authContext.CurrentAccount.ID);

                var refreshed = _consumerFactory.Get<DocuSignLoginProvider>().RefreshToken(token.RefreshToken);

                if (refreshed != null)
                {
                    token.AccessToken = refreshed.AccessToken;
                    token.RefreshToken = refreshed.RefreshToken;
                    token.ExpiresIn = refreshed.ExpiresIn;
                    token.Timestamp = DateTime.UtcNow;

                    await SaveTokenAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorDocuSignRefreshToken(_authContext.CurrentAccount.ID, ex);
            }
        }

        return token.AccessToken;
    }
}

[Scope]
public class DocuSignHelper
{
    private readonly ILogger<DocuSignHelper> _logger;

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

    private readonly DocuSignToken _docuSignToken;
    private readonly FileSecurity _fileSecurity;
    private readonly IDaoFactory _daoFactory;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly FileMarker _fileMarker;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesMessageService _filesMessageService;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerFactory _consumerFactory;
    private readonly RequestHelper _requestHelper;

    public DocuSignHelper(
        DocuSignToken docuSignToken,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory,
        ILogger<DocuSignHelper> logger,
        BaseCommonLinkUtility baseCommonLinkUtility,
        UserManager userManager,
        AuthContext authContext,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        FileMarker fileMarker,
        GlobalFolderHelper globalFolderHelper,
        FilesMessageService filesMessageService,
        FilesLinkUtility filesLinkUtility,
        IServiceProvider serviceProvider,
        ConsumerFactory consumerFactory,
        RequestHelper requestHelper)
    {
        _docuSignToken = docuSignToken;
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _userManager = userManager;
        _authContext = authContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _fileMarker = fileMarker;
        _globalFolderHelper = globalFolderHelper;
        _filesMessageService = filesMessageService;
        _filesLinkUtility = filesLinkUtility;
        _serviceProvider = serviceProvider;
        _consumerFactory = consumerFactory;
        _logger = logger;
        _requestHelper = requestHelper;
    }

    public async Task<bool> ValidateTokenAsync(OAuth20Token token)
    {
        await GetDocuSignAccountAsync(token);

        return true;
    }

    public async Task<string> SendDocuSignAsync<T>(T fileId, DocuSignData docuSignData)
    {
        ArgumentNullException.ThrowIfNull(docuSignData);

        var token = await _docuSignToken.GetTokenAsync();
        var account = await GetDocuSignAccountAsync(token);

        var apiClient = await GetApiClientAsync(account, token);
        var (document, sourceFile) = await CreateDocumentAsync(fileId, docuSignData.Name, docuSignData.FolderId);

        var url = await CreateEnvelopeAsync(account.AccountId, document, docuSignData, apiClient);

        _ = _filesMessageService.SendAsync(MessageAction.DocumentSendToSign, sourceFile, "DocuSign", sourceFile.Title);

        return url;
    }

    private async Task<DocuSignAccount> GetDocuSignAccountAsync(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var userInfoString = _requestHelper.PerformRequest(_consumerFactory.Get<DocuSignLoginProvider>().DocuSignHost + "/oauth/userinfo",
                                                          headers: new Dictionary<string, string> { { "Authorization", "Bearer " + await _docuSignToken.GetRefreshedTokenAsync(token) } });

        _logger.DebugDocuSingUserInfo(userInfoString);

        var userInfo = (DocuSignUserInfo)JsonConvert.DeserializeObject(userInfoString, typeof(DocuSignUserInfo));

        if (userInfo.Accounts == null || userInfo.Accounts.Count == 0)
        {
            throw new Exception("Account is null");
        }

        var account = userInfo.Accounts[0];

        return account;
    }

    private async Task<DocuSignClient> GetApiClientAsync(DocuSignAccount account, OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(token);

        var apiClient = new DocuSignClient(account.BaseUri + "/restapi");

        apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + await _docuSignToken.GetRefreshedTokenAsync(token));

        return apiClient;
    }

    private async Task<(Document document, File<T> file)> CreateDocumentAsync<T>(T fileId, string documentName, string folderId)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_FileNotFound);
        }
        if (!await _fileSecurity.CanReadAsync(file))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
        }
        if (!SupportedFormats.Contains(FileUtility.GetFileExtension(file.Title)))
        {
            throw new ArgumentException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
        }
        if (file.ContentLength > MaxFileSize)
        {
            throw new Exception(FileSizeComment.GetFileSizeExceptionString(MaxFileSize));
        }

        byte[] fileBytes;
        await using (var stream = await fileDao.GetFileStreamAsync(file))
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

    private async Task<string> CreateEnvelopeAsync(string accountId, Document document, DocuSignData docuSignData, DocuSignClient apiClient)
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
            Url = _baseCommonLinkUtility.GetFullAbsolutePath(DocuSignHandlerService.Path(_filesLinkUtility) + "?" + FilesLinkUtility.Action + "=webhook"),
        };

        _logger.DebugDocuSingHookUrl(eventNotification.Url);

        var signers = new List<Signer>();

        foreach (var uid in docuSignData.Users)
        {
            try
            {
                var user = await _userManager.GetUsersAsync(uid);
                signers.Add(new Signer
                {
                    Email = user.Email,
                    Name = user.DisplayUserName(false, _displayUserSettingsHelper),
                    RecipientId = user.Id.ToString(),
                });
            }
            catch (Exception ex)
            {
                _logger.ErrorSignerIsUndefined(ex);
            }
        }

        var envelopeDefinition = new EnvelopeDefinition
        {
            CustomFields = new CustomFields
            {
                TextCustomFields = new List<TextCustomField>
                    {
                        new TextCustomField {Name = UserField, Value = _authContext.CurrentAccount.ID.ToString()},
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

        var envelopesApi = new EnvelopesApi(apiClient);
        var envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

        _logger.DebugDocuSingCreatedEnvelope(envelopeSummary.EnvelopeId);

        var envelopeId = envelopeSummary.EnvelopeId;
        var url = envelopesApi.CreateSenderView(accountId, envelopeId, new ReturnUrlRequest
        {
            ReturnUrl = _baseCommonLinkUtility.GetFullAbsolutePath(DocuSignHandlerService.Path(_filesLinkUtility) + "?" + FilesLinkUtility.Action + "=redirect")
        });
        _logger.DebugDocuSingSenderView(url.Url);

        return url.Url;
    }

    public async Task<File<T>> SaveDocumentAsync<T>(string envelopeId, string documentId, string documentName, T folderId)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(envelopeId);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(documentId);

        var token = await _docuSignToken.GetTokenAsync();
        var account = await GetDocuSignAccountAsync(token);
        var apiClient = await GetApiClientAsync(account, token);

        var fileDao = _daoFactory.GetFileDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        if (string.IsNullOrEmpty(documentName))
        {
            documentName = "new.pdf";
        }

        Folder<T> folder;
        if (folderId == null
            || (folder = await folderDao.GetFolderAsync(folderId)) == null
            || folder.RootFolderType == FolderType.TRASH
            || !await _fileSecurity.CanCreateAsync(folder))
        {
            if (await _globalFolderHelper.FolderMyAsync != 0)
            {
                folderId = await _globalFolderHelper.GetFolderMyAsync<T>();
            }
            else
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
            }
        }

        var file = _serviceProvider.GetService<File<T>>();
        file.ParentId = folderId;
        file.Comment = FilesCommonResource.CommentCreateByDocuSign;
        file.Title = FileUtility.ReplaceFileExtension(documentName, ".pdf");

        var envelopesApi = new EnvelopesApi(apiClient);
        _logger.InformationDocuSignWebhookGetStream(documentId);
        await using (var stream = await envelopesApi.GetDocumentAsync(account.AccountId, envelopeId, documentId))
        {
            file.ContentLength = stream.Length;
            file = await fileDao.SaveFileAsync(file, stream);
        }

        _ = _filesMessageService.SendAsync(MessageAction.DocumentSignComplete, file, MessageInitiator.ThirdPartyProvider, "DocuSign", file.Title);

        await _fileMarker.MarkAsNewAsync(file);

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

[EnumExtensions]
public enum DocuSignStatus
{
    Draft,
    Sent,
    Delivered,
    Completed,
    Declined,
    Voided,
}
