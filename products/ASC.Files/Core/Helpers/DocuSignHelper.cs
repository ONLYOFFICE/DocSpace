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


using Document = DocuSign.eSign.Model.Document;

namespace ASC.Web.Files.Helpers;

[Scope]
public class DocuSignToken
{
    public ILog Logger { get; set; }

    public const string AppAttr = "docusign";

    private readonly TokenHelper _tokenHelper;
    private readonly AuthContext _authContext;
    private readonly ConsumerFactory _consumerFactory;

    public DocuSignToken(
        TokenHelper tokenHelper,
        IOptionsMonitor<ILog> options,
        AuthContext authContext,
        ConsumerFactory consumerFactory)
    {
        _tokenHelper = tokenHelper;
        _authContext = authContext;
        _consumerFactory = consumerFactory;
        Logger = options.CurrentValue;
    }

    public OAuth20Token GetToken()
    {
        return _tokenHelper.GetToken(AppAttr);
    }

    public void DeleteToken(Guid? userId = null)
    {
        _tokenHelper.DeleteToken(AppAttr, userId);
    }

    public void SaveToken(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        _tokenHelper.SaveToken(new Token(token, AppAttr));
    }

    internal string GetRefreshedToken(OAuth20Token token)
    {
        if (token.IsExpired)
        {
            try
            {
                Logger.Info("DocuSign refresh token for user " + _authContext.CurrentAccount.ID);

                var refreshed = _consumerFactory.Get<DocuSignLoginProvider>().RefreshToken(token.RefreshToken);

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
                Logger.Error("DocuSign refresh token for user " + _authContext.CurrentAccount.ID, ex);
            }
        }

        return token.AccessToken;
    }
}

[Scope]
public class DocuSignHelper
{
    public ILog Logger { get; set; }

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
        Logger = options.CurrentValue;
    }

    public bool ValidateToken(OAuth20Token token)
    {
        GetDocuSignAccount(token);

        return true;
    }

    public async Task<string> SendDocuSignAsync<T>(T fileId, DocuSignData docuSignData, IDictionary<string, StringValues> requestHeaders)
    {
        ArgumentNullException.ThrowIfNull(docuSignData);

        var token = _docuSignToken.GetToken();
        var account = GetDocuSignAccount(token);

        var configuration = GetConfiguration(account, token);
        var (document, sourceFile) = await CreateDocumentAsync(fileId, docuSignData.Name, docuSignData.FolderId);

        var url = CreateEnvelope(account.AccountId, document, docuSignData, configuration);

        _filesMessageService.Send(sourceFile, requestHeaders, MessageAction.DocumentSendToSign, "DocuSign", sourceFile.Title);

        return url;
    }

    private DocuSignAccount GetDocuSignAccount(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var userInfoString = RequestHelper.PerformRequest(_consumerFactory.Get<DocuSignLoginProvider>().DocuSignHost + "/oauth/userinfo",
                                                          headers: new Dictionary<string, string> { { "Authorization", "Bearer " + _docuSignToken.GetRefreshedToken(token) } });

        Logger.Debug("DocuSing userInfo: " + userInfoString);

        var userInfo = (DocuSignUserInfo)JsonConvert.DeserializeObject(userInfoString, typeof(DocuSignUserInfo));

        if (userInfo.Accounts == null || userInfo.Accounts.Count == 0)
        {
            throw new Exception("Account is null");
        }

        var account = userInfo.Accounts[0];

        return account;
    }

    private DocuSign.eSign.Client.Configuration GetConfiguration(DocuSignAccount account, OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(token);

        var apiClient = new ApiClient(account.BaseUri + "/restapi");

        var configuration = new DocuSign.eSign.Client.Configuration { ApiClient = apiClient };
        configuration.AddDefaultHeader("Authorization", "Bearer " + _docuSignToken.GetRefreshedToken(token));

        return configuration;
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
            Url = _baseCommonLinkUtility.GetFullAbsolutePath(DocuSignHandlerService.Path(_filesLinkUtility) + "?" + FilesLinkUtility.Action + "=webhook"),
        };

        Logger.Debug("DocuSign hook url: " + eventNotification.Url);

        var signers = new List<Signer>();
        docuSignData.Users.ForEach(uid =>
        {
            try
            {
                var user = _userManager.GetUsers(uid);
                signers.Add(new Signer
                {
                    Email = user.Email,
                    Name = user.DisplayUserName(false, _displayUserSettingsHelper),
                    RecipientId = user.Id.ToString(),
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Signer is undefined", ex);
            }
        });

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

        var envelopesApi = new EnvelopesApi(configuration);
        var envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

        Logger.Debug("DocuSign createdEnvelope: " + envelopeSummary.EnvelopeId);

        var envelopeId = envelopeSummary.EnvelopeId;
        var url = envelopesApi.CreateSenderView(accountId, envelopeId, new ReturnUrlRequest
        {
            ReturnUrl = _baseCommonLinkUtility.GetFullAbsolutePath(DocuSignHandlerService.Path(_filesLinkUtility) + "?" + FilesLinkUtility.Action + "=redirect")
        });
        Logger.Debug("DocuSign senderView: " + url.Url);

        return url.Url;
    }

    public async Task<File<T>> SaveDocumentAsync<T>(string envelopeId, string documentId, string documentName, T folderId)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(envelopeId);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(documentId);

        var token = _docuSignToken.GetToken();
        var account = GetDocuSignAccount(token);
        var configuration = GetConfiguration(account, token);

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
            if (_globalFolderHelper.FolderMy != 0)
            {
                folderId = _globalFolderHelper.GetFolderMy<T>();
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

        var envelopesApi = new EnvelopesApi(configuration);
        Logger.Info("DocuSign webhook get stream: " + documentId);
        using (var stream = await envelopesApi.GetDocumentAsync(account.AccountId, envelopeId, documentId))
        {
            file.ContentLength = stream.Length;
            file = await fileDao.SaveFileAsync(file, stream);
        }

        _filesMessageService.Send(file, MessageInitiator.ThirdPartyProvider, MessageAction.DocumentSignComplete, "DocuSign", file.Title);

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

public enum DocuSignStatus
{
    Draft,
    Sent,
    Delivered,
    Completed,
    Declined,
    Voided,
}
