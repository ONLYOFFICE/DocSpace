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

namespace ASC.Web.Files.Services.DocumentService
{
    public enum EditorType
    {
        Desktop,
        Mobile,
        Embedded,
        External,
    }

    public class Configuration<T>
    {
        internal static readonly Dictionary<FileType, string> DocType = new Dictionary<FileType, string>
        {
            { FileType.Document, "text" },
            { FileType.Spreadsheet, "spreadsheet" },
            { FileType.Presentation, "presentation" }
        };

        private FileType _fileTypeCache = FileType.Unknown;

        public Configuration(
            File<T> file,
            IServiceProvider serviceProvider)
        {
            Document = serviceProvider.GetService<DocumentConfig<T>>();
            Document.Info.SetFile(file);
            EditorConfig = serviceProvider.GetService<EditorConfiguration<T>>();
            EditorConfig.SetConfiguration(this);
        }

        public EditorType EditorType
        {
            set => Document.Info.Type = value;
            get => Document.Info.Type;
        }

        public DocumentConfig<T> Document { get; set; }
        public string DocumentType
        {
            get
            {
                DocType.TryGetValue(GetFileType, out var documentType);

                return documentType;
            }
        }
        public EditorConfiguration<T> EditorConfig { get; set; }
        public string Token { get; set; }
        public string Type
        {
            set => EditorType = (EditorType)Enum.Parse(typeof(EditorType), value, true);
            get => EditorType.ToString().ToLower();
        }

        internal FileType GetFileType
        {
            get
            {
                if (_fileTypeCache == FileType.Unknown)
                {
                    _fileTypeCache = FileUtility.GetFileTypeByFileName(Document.Info.GetFile().Title);
                }

                return _fileTypeCache;
            }
        }

        [JsonPropertyName("Error")]
        public string ErrorMessage { get; set; }

        public static string Serialize(Configuration<T> configuration)
        {
            return JsonSerializer.Serialize(configuration);
        }
    }

    #region Nested Classes

    [Transient]
    public class DocumentConfig<T>
    {
        public string SharedLinkKey { get; set; }

        public DocumentConfig(DocumentServiceConnector documentServiceConnector, PathProvider pathProvider, InfoConfig<T> infoConfig)
        {
            Info = infoConfig;
            Permissions = new PermissionsConfig();
            _documentServiceConnector = documentServiceConnector;
            _pathProvider = pathProvider;
        }

        private string _key = string.Empty;
        private string _fileUri;
        private string _title = null;


        public string FileType => Info.GetFile().ConvertedExtension.Trim('.');
        public InfoConfig<T> Info { get; set; }
        public string Key
        {
            set => _key = value;
            get => DocumentServiceConnector.GenerateRevisionId(_key);
        }

        public PermissionsConfig Permissions { get; set; }

        public string Title
        {
            set => _title = value;
            get => _title ?? Info.GetFile().Title;
        }

        public string Url
        {
            set => _fileUri = _documentServiceConnector.ReplaceCommunityAdress(value);
            get
            {
                if (!string.IsNullOrEmpty(_fileUri))
                {
                    return _fileUri;
                }

                var last = Permissions.Edit || Permissions.Review || Permissions.Comment;
                _fileUri = _documentServiceConnector.ReplaceCommunityAdress(_pathProvider.GetFileStreamUrl(Info.GetFile(), SharedLinkKey, last));

                return _fileUri;
            }
        }

        private readonly DocumentServiceConnector _documentServiceConnector;
        private readonly PathProvider _pathProvider;
    }

    [Transient]
    public class InfoConfig<T>
    {
        private File<T> _file;

        public File<T> GetFile()
        {
            return _file;
        }

        public void SetFile(File<T> file)
        {
            _file = file;
        }

        public EditorType Type { get; set; } = EditorType.Desktop;
        private string _breadCrumbs;

        public InfoConfig(BreadCrumbsManager breadCrumbsManager, FileSharing fileSharing, SecurityContext securityContext, UserManager userManager)
        {
            _breadCrumbsManager = breadCrumbsManager;
            _fileSharing = fileSharing;
            _securityContext = securityContext;
            _userManager = userManager;
        }

        public bool? Favorite
        {
            get
            {
                if (!_securityContext.IsAuthenticated || _userManager.GetUsers(_securityContext.CurrentAccount.ID).IsVisitor(_userManager))
                {
                    return null;
                }

                if (_file.Encrypted)
                {
                    return null;
                }

                return _file.IsFavorite;
            }
        }

        public string Folder
        {
            get
            {
                if (Type == EditorType.Embedded || Type == EditorType.External)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(_breadCrumbs))
                {
                    const string crumbsSeporator = " \\ ";

                    var breadCrumbsList = _breadCrumbsManager.GetBreadCrumbsAsync(_file.FolderID).Result;
                    _breadCrumbs = string.Join(crumbsSeporator, breadCrumbsList.Select(folder => folder.Title).ToArray());
                }

                return _breadCrumbs;
            }
        }

        public string Owner => _file.CreateByString;

        public string Uploaded => _file.CreateOnString;

        public List<AceShortWrapper> SharingSettings
        {
            get
            {
                if (Type == EditorType.Embedded
                    || Type == EditorType.External
                    || !_fileSharing.CanSetAccessAsync(_file).Result)
                {
                    return null;
                }

                try
                {
                    return _fileSharing.GetSharedInfoShortFileAsync(_file.ID).Result;
                }
                catch
                {
                    return null;
                }
            }
        }

        private readonly BreadCrumbsManager _breadCrumbsManager;
        private readonly FileSharing _fileSharing;
        private readonly SecurityContext _securityContext;
        private readonly UserManager _userManager;
    }

    public class PermissionsConfig
    {
        //todo: obsolete since DS v5.5
        public bool ChangeHistory { get; set; } = false;
        public bool Comment { get; set; } = true;
        public bool Download { get; set; } = true;
        public bool Edit { get; set; } = true;
        public bool FillForms { get; set; } = true;
        public bool Print { get; set; } = true;
        public bool ModifyFilter { get; set; } = true;

        //todo: obsolete since DS v6.0
        public bool Rename { get; set; } = false;
        public bool Review { get; set; } = true;
    }

    [Transient]
    public class EditorConfiguration<T>
    {
        public EditorConfiguration(
            UserManager userManager,
            AuthContext authContext,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            PluginsConfig pluginsConfig,
            EmbeddedConfig embeddedConfig,
            CustomizationConfig<T> customizationConfig,
            FilesSettingsHelper filesSettingsHelper,
            IDaoFactory daoFactory,
            EntryManager entryManager)
        {
            _userManager = userManager;
            _authContext = authContext;
            _filesLinkUtility = filesLinkUtility;
            _fileUtility = fileUtility;
            _baseCommonLinkUtility = baseCommonLinkUtility;
            Customization = customizationConfig;
            _filesSettingsHelper = filesSettingsHelper;
            _daoFactory = daoFactory;
            _entryManager = entryManager;
            Plugins = pluginsConfig;
            Embedded = embeddedConfig;
            _userInfo = userManager.GetUsers(authContext.CurrentAccount.ID);

            if (!_userInfo.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID))
            {
                User = new UserConfig
                {
                    Id = _userInfo.ID.ToString(),
                    Name = _userInfo.DisplayUserName(false, displayUserSettingsHelper),
                };
            }
        }

        public bool ModeWrite { get; set; } = false;

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
            Customization.SetConfiguration(_configuration);
        }

        private readonly UserInfo _userInfo;
        private EmbeddedConfig _embeddedConfig;

        public ActionLinkConfig ActionLink { get; set; }

        public string ActionLinkString
        {
            get => null;
            set
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    };

                    JsonSerializer.Deserialize<ActionLinkConfig>(value, options);
                }
                catch (Exception)
                {
                    ActionLink = null;
                }
            }
        }


        public List<TemplatesConfig> Templates
        {
            set { }
            get
            {
                if (!_authContext.IsAuthenticated || _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
                {
                    return null;
                }

                if (!_filesSettingsHelper.TemplatesSection)
                {
                    return null;
                }

                var extension = _fileUtility.GetInternalExtension(_configuration.Document.Title).TrimStart('.');
                var filter = FilterType.FilesOnly;
                switch (_configuration.GetFileType)
                {
                    case FileType.Document:
                        filter = FilterType.DocumentsOnly;
                        break;
                    case FileType.Spreadsheet:
                        filter = FilterType.SpreadsheetsOnly;
                        break;
                    case FileType.Presentation:
                        filter = FilterType.PresentationsOnly;
                        break;
                }

                var folderDao = _daoFactory.GetFolderDao<int>();
                var fileDao = _daoFactory.GetFileDao<int>();
                var files = _entryManager.GetTemplatesAsync(folderDao, fileDao, filter, false, Guid.Empty, string.Empty, false).Result;
                var listTemplates = from file in files
                                    select
                                        new TemplatesConfig
                                        {
                                            Image = _baseCommonLinkUtility.GetFullAbsolutePath("skins/default/images/filetype/thumb/" + extension + ".png"),
                                            Name = file.Title,
                                            Title = file.Title,
                                            Url = _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebEditorUrl(file.ID))
                                        };
                return listTemplates.ToList();
            }
        }

        public string CallbackUrl { get; set; }

        public string CreateUrl
        {
            get
            {
                if (_configuration.Document.Info.Type != EditorType.Desktop)
                {
                    return null;
                }

                if (!_authContext.IsAuthenticated || _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
                {
                    return null;
                }

                return GetCreateUrl(_configuration.GetFileType);
            }
        }

        public PluginsConfig Plugins { get; set; }

        public CustomizationConfig<T> Customization { get; set; }
        private readonly FilesSettingsHelper _filesSettingsHelper;
        private readonly IDaoFactory _daoFactory;
        private readonly EntryManager _entryManager;

        public EmbeddedConfig Embedded
        {
            set => _embeddedConfig = value;
            get => _configuration.Document.Info.Type == EditorType.Embedded ? _embeddedConfig : null;
        }

        public EncryptionKeysConfig EncryptionKeys { get; set; }

        public string FileChoiceUrl { get; set; }

        public string Lang => _userInfo.GetCulture().Name;

        public string Mode => ModeWrite ? "edit" : "view";

        private readonly UserManager _userManager;
        private readonly AuthContext _authContext;
        private readonly FilesLinkUtility _filesLinkUtility;
        private readonly FileUtility _fileUtility;
        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;

        public string SaveAsUrl { get; set; }
        public List<RecentConfig> Recent
        {
            get
            {
                if (!_authContext.IsAuthenticated || _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
                {
                    return null;
                }

                if (!_filesSettingsHelper.RecentSection)
                {
                    return null;
                }

                var filter = FilterType.FilesOnly;
                switch (_configuration.GetFileType)
                {
                    case FileType.Document:
                        filter = FilterType.DocumentsOnly;
                        break;
                    case FileType.Spreadsheet:
                        filter = FilterType.SpreadsheetsOnly;
                        break;
                    case FileType.Presentation:
                        filter = FilterType.PresentationsOnly;
                        break;
                }

                var folderDao = _daoFactory.GetFolderDao<int>();
                var files = _entryManager.GetRecentAsync(filter, false, Guid.Empty, string.Empty, false).Result.Cast<File<int>>();

                var listRecent = from file in files
                                 where !Equals(_configuration.Document.Info.GetFile().ID, file.ID)
                                 select
                                     new RecentConfig
                                     {
                                         Folder = folderDao.GetFolderAsync(file.FolderID).Result.Title,
                                         Title = file.Title,
                                         Url = _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebEditorUrl(file.ID))
                                     };

                return listRecent.ToList();
            }
        }

        public string SharingSettingsUrl { get; set; }

        public UserConfig User { get; set; }

        private string GetCreateUrl(FileType fileType)
        {
            string title;
            switch (fileType)
            {
                case FileType.Document:
                    title = FilesJSResource.TitleNewFileText;
                    break;
                case FileType.Spreadsheet:
                    title = FilesJSResource.TitleNewFileSpreadsheet;
                    break;
                case FileType.Presentation:
                    title = FilesJSResource.TitleNewFilePresentation;
                    break;
                default:
                    return null;
            }

            Configuration<T>.DocType.TryGetValue(fileType, out var documentType);

            return _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath)
                   + "?" + FilesLinkUtility.Action + "=create"
                   + "&doctype=" + documentType
                   + "&" + FilesLinkUtility.FileTitle + "=" + HttpUtility.UrlEncode(title);
        }
    }

    #endregion

    public class ActionLinkConfig
    {
        public ActionConfig Action { get; set; }

        public class ActionConfig
        {
            public string Type { get; set; }
            public string Data { get; set; }
        }

        public static string Serialize(ActionLinkConfig actionLinkConfig)
        {
            return JsonSerializer.Serialize(actionLinkConfig);
        }
    }

    [Transient]
    public class EmbeddedConfig
    {
        public string ShareLinkParam { get; set; }

        public string EmbedUrl => _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FilesBaseAbsolutePath 
            + FilesLinkUtility.EditorPage + "?" + FilesLinkUtility.Action + "=embedded" + ShareLinkParam);

        public string SaveUrl => _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath + "?" 
            + FilesLinkUtility.Action + "=download" + ShareLinkParam);

        public string ShareUrl => _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FilesBaseAbsolutePath 
            + FilesLinkUtility.EditorPage + "?" + FilesLinkUtility.Action + "=view" + ShareLinkParam);

        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
        private readonly FilesLinkUtility _filesLinkUtility;

        public string ToolbarDocked => "top";

        public EmbeddedConfig(BaseCommonLinkUtility baseCommonLinkUtility, FilesLinkUtility filesLinkUtility)
        {
            _baseCommonLinkUtility = baseCommonLinkUtility;
            _filesLinkUtility = filesLinkUtility;
        }
    }

    public class EncryptionKeysConfig
    {
        public string CryptoEngineId => "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
        public string PrivateKeyEnc { get; set; }
        public string PublicKey { get; set; }
    }

    [Transient]
    public class PluginsConfig
    {
        public string[] PluginsData
        {
            get
            {
                var plugins = new List<string>();

                if (_coreBaseSettings.Standalone || !_tenantExtra.GetTenantQuota().Free)
                {
                    var easyBibHelper = _consumerFactory.Get<EasyBibHelper>();
                    if (!string.IsNullOrEmpty(easyBibHelper.AppKey))
                    {
                        plugins.Add(_baseCommonLinkUtility.GetFullAbsolutePath("ThirdParty/plugin/easybib/config.json"));
                    }

                    var wordpressLoginProvider = _consumerFactory.Get<WordpressLoginProvider>();
                    if (!string.IsNullOrEmpty(wordpressLoginProvider.ClientID) &&
                        !string.IsNullOrEmpty(wordpressLoginProvider.ClientSecret) &&
                        !string.IsNullOrEmpty(wordpressLoginProvider.RedirectUri))
                    {
                        plugins.Add(_baseCommonLinkUtility.GetFullAbsolutePath("ThirdParty/plugin/wordpress/config.json"));
                    }
                }

                return plugins.ToArray();
            }
        }

        private readonly ConsumerFactory _consumerFactory;
        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
        private readonly CoreBaseSettings _coreBaseSettings;
        private readonly TenantExtra _tenantExtra;

        public PluginsConfig(
            ConsumerFactory consumerFactory,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CoreBaseSettings coreBaseSettings,
            TenantExtra tenantExtra)
        {
            _consumerFactory = consumerFactory;
            _baseCommonLinkUtility = baseCommonLinkUtility;
            _coreBaseSettings = coreBaseSettings;
            _tenantExtra = tenantExtra;
        }
    }

    [Transient]
    public class CustomizationConfig<T>
    {
        public CustomizationConfig(
            CoreBaseSettings coreBaseSettings,
            SettingsManager settingsManager,
            FileUtility fileUtility,
            FilesSettingsHelper filesSettingsHelper,
            AuthContext authContext,
            FileSecurity fileSecurity,
            IDaoFactory daoFactory,
            GlobalFolderHelper globalFolderHelper,
            PathProvider pathProvider,
            CustomerConfig<T> customerConfig,
            LogoConfig<T> logoConfig,
            FileSharing fileSharing)
        {
            _coreBaseSettings = coreBaseSettings;
            _settingsManager = settingsManager;
            _fileUtility = fileUtility;
            _filesSettingsHelper = filesSettingsHelper;
            _authContext = authContext;
            _fileSecurity = fileSecurity;
            _daoFactory = daoFactory;
            _globalFolderHelper = globalFolderHelper;
            _pathProvider = pathProvider;
            Customer = customerConfig;
            Logo = logoConfig;
            FileSharing = fileSharing;
        }

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
            Customer.SetConfiguration(_configuration);
            Logo.SetConfiguration(_configuration);
        }

        //private string _gobackUrl;
        public bool IsRetina { get; set; } = false;

        public bool About => !_coreBaseSettings.Standalone && !_coreBaseSettings.CustomMode;

        public CustomerConfig<T> Customer { get; set; }

        public FeedbackConfig Feedback
        {
            get
            {
                if (_coreBaseSettings.Standalone)
                {
                    return null;
                }

                var settings = _settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
                if (!settings.FeedbackAndSupportEnabled)
                {
                    return null;
                }

                return new FeedbackConfig
                {
                    Url = BaseCommonLinkUtility.GetRegionalUrl(
                            settings.FeedbackAndSupportUrl,
                            CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                };
            }
        }

        public bool? Forcesave
        {
            get
            {
                return _fileUtility.CanForcesave
                       && !_configuration.Document.Info.GetFile().ProviderEntry
                       && ThirdPartySelector.GetAppByFileId(_configuration.Document.Info.GetFile().ID.ToString()) == null
                       && _filesSettingsHelper.Forcesave;
            }
        }

        public GobackConfig Goback
        {
            get
            {
                if (_configuration.EditorType == EditorType.Embedded || _configuration.EditorType == EditorType.External)
                {
                    return null;
                }

                if (!_authContext.IsAuthenticated)
                {
                    return null;
                }
                //if (_gobackUrl != null)
                //{
                //    return new GobackConfig
                //    {
                //        Url = _gobackUrl,
                //    };
                //}

                var folderDao = _daoFactory.GetFolderDao<T>();
                try
                {
                    var parent = folderDao.GetFolderAsync(_configuration.Document.Info.GetFile().FolderID).Result;
                    var fileSecurity = _fileSecurity;
                    if (_configuration.Document.Info.GetFile().RootFolderType == FolderType.USER
                        && !Equals(_configuration.Document.Info.GetFile().RootFolderId, _globalFolderHelper.FolderMy)
                        && !fileSecurity.CanReadAsync(parent).Result)
                    {
                        if (fileSecurity.CanReadAsync(_configuration.Document.Info.GetFile()).Result)
                        {
                            return new GobackConfig
                            {
                                Url = _pathProvider.GetFolderUrlByIdAsync(_globalFolderHelper.FolderShareAsync.Result).Result,
                            };
                        }

                        return null;
                    }

                    if (_configuration.Document.Info.GetFile().Encrypted
                        && _configuration.Document.Info.GetFile().RootFolderType == FolderType.Privacy
                        && !fileSecurity.CanReadAsync(parent).Result)
                    {
                        parent = folderDao.GetFolderAsync(_globalFolderHelper.GetFolderPrivacyAsync<T>().Result).Result;
                    }

                    return new GobackConfig
                    {
                        Url = _pathProvider.GetFolderUrlAsync(parent).Result,
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public LogoConfig<T> Logo { get; set; }
        private FileSharing FileSharing { get; }

        public bool MentionShare
        {
            get
            {
                return _authContext.IsAuthenticated
                       && !_configuration.Document.Info.GetFile().Encrypted
                       && FileSharing.CanSetAccessAsync(_configuration.Document.Info.GetFile()).Result;
            }
        }

        public string ReviewDisplay
        {
            get { return _configuration.EditorConfig.ModeWrite ? null : "markup"; }
        }

        private readonly CoreBaseSettings _coreBaseSettings;
        private readonly SettingsManager _settingsManager;
        private readonly FileUtility _fileUtility;
        private readonly FilesSettingsHelper _filesSettingsHelper;
        private readonly AuthContext _authContext;
        private readonly FileSecurity _fileSecurity;
        private readonly IDaoFactory _daoFactory;
        private readonly GlobalFolderHelper _globalFolderHelper;
        private readonly PathProvider _pathProvider;
    }

    [Transient]
    public class CustomerConfig<T>
    {
        public CustomerConfig(
            SettingsManager settingsManager,
            BaseCommonLinkUtility baseCommonLinkUtility,
            TenantLogoHelper tenantLogoHelper)
        {
            _settingsManager = settingsManager;
            _baseCommonLinkUtility = baseCommonLinkUtility;
            _tenantLogoHelper = tenantLogoHelper;
        }

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
        }

        public string Logo => _baseCommonLinkUtility.GetFullAbsolutePath(
            _tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.Dark, !_configuration.EditorConfig.Customization.IsRetina));

        public string Name => (_settingsManager.Load<TenantWhiteLabelSettings>().GetLogoText(_settingsManager) ?? "")
                    .Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("/", "\\/");

        private readonly SettingsManager _settingsManager;
        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
        private readonly TenantLogoHelper _tenantLogoHelper;
    }

    public class FeedbackConfig
    {
        public string Url { get; set; }
        public bool Visible { get => true; }
    }

    public class GobackConfig
    {
        public string Url { get; set; }
    }

    [Transient]
    public class LogoConfig<T>
    {
        public LogoConfig(
            CommonLinkUtility commonLinkUtility,
            TenantLogoHelper tenantLogoHelper,
            FileUtility fileUtility)
        {
            _commonLinkUtility = commonLinkUtility;
            _tenantLogoHelper = tenantLogoHelper;
            _fileUtility = fileUtility;
        }

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
        }

        public string Image
        {
            get
            {
                var fillingForm = _fileUtility.CanWebRestrictedEditing(_configuration.Document.Title);

                return _configuration.EditorType == EditorType.Embedded 
                    || fillingForm
                        ? _commonLinkUtility.GetFullAbsolutePath(_tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditorEmbed, !_configuration.EditorConfig.Customization.IsRetina))
                        : _commonLinkUtility.GetFullAbsolutePath(_tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditor, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        public string ImageDark
        {
            set { }
            get => _commonLinkUtility.GetFullAbsolutePath(
                _tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditor, !_configuration.EditorConfig.Customization.IsRetina));
        }

        public string ImageEmbedded
        {
            get
            {
                return _configuration.EditorType != EditorType.Embedded
                        ? null
                        : _commonLinkUtility.GetFullAbsolutePath(_tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditorEmbed, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        public string Url
        {
            set { }
            get => _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetDefault());
        }

        private readonly CommonLinkUtility _commonLinkUtility;
        private readonly TenantLogoHelper _tenantLogoHelper;
        private readonly FileUtility _fileUtility;
    }

    public class RecentConfig
    {
        public string Folder { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class TemplatesConfig
    {
        public string Image { get; set; }

        //todo: obsolete since DS v6.0
        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class UserConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public static class ConfigurationExtention
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<DocumentConfig<string>>();
            services.TryAdd<DocumentConfig<int>>();

            services.TryAdd<InfoConfig<string>>();
            services.TryAdd<InfoConfig<int>>();

            services.TryAdd<EditorConfiguration<string>>();
            services.TryAdd<EditorConfiguration<int>>();

            services.TryAdd<PluginsConfig>();
            services.TryAdd<EmbeddedConfig>();

            services.TryAdd<CustomizationConfig<string>>();
            services.TryAdd<CustomizationConfig<int>>();

            services.TryAdd<CustomerConfig<string>>();
            services.TryAdd<CustomerConfig<int>>();

            services.TryAdd<LogoConfig<string>>();
            services.TryAdd<LogoConfig<int>>();

        }
    }
}