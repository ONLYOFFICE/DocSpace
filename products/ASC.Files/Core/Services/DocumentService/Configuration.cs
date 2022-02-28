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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.ThirdPartyApp;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.DependencyInjection;

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
            IServiceProvider serviceProvider
            )
        {
            Document = serviceProvider.GetService<DocumentConfig<T>>();
            Document.Info.SetFile(file);
            EditorConfig = serviceProvider.GetService<EditorConfiguration<T>>();
            EditorConfig.SetConfiguration(this);
        }

        public EditorType EditorType
        {
            set { Document.Info.Type = value; }
            get { return Document.Info.Type; }
        }

        #region Property

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
            set { EditorType = (EditorType)Enum.Parse(typeof(EditorType), value, true); }
            get { return EditorType.ToString().ToLower(); }
        }

        internal FileType GetFileType
        {
            get
            {
                if (_fileTypeCache == FileType.Unknown)
                    _fileTypeCache = FileUtility.GetFileTypeByFileName(Document.Info.GetFile().Title);
                return _fileTypeCache;
            }
        }

        [JsonPropertyName("Error")]
        public string ErrorMessage { get; set; }

        #endregion

        public static string Serialize(Configuration<T> configuration)
        {
            return JsonSerializer.Serialize(configuration);
        }
    }
    #region Nested Classes

    [Transient]
    public class DocumentConfig<T>
    {
        public string SharedLinkKey;

        public DocumentConfig(DocumentServiceConnector documentServiceConnector, PathProvider pathProvider, InfoConfig<T> infoConfig)
        {
            Info = infoConfig;
            Permissions = new PermissionsConfig();
            DocumentServiceConnector = documentServiceConnector;
            PathProvider = pathProvider;
        }

        private string _key = string.Empty;
        private string _fileUri;
        private string _title = null;


        public string FileType
        {
            get { return Info.GetFile().ConvertedExtension.Trim('.'); }
        }

        public InfoConfig<T> Info { get; set; }

        public string Key
        {
            set { _key = value; }
            get { return DocumentServiceConnector.GenerateRevisionId(_key); }
        }

        public PermissionsConfig Permissions { get; set; }

        public string Title
        {
            set { _title = value; }
            get { return _title ?? Info.GetFile().Title; }
        }

        public string Url
        {
            set { _fileUri = DocumentServiceConnector.ReplaceCommunityAdress(value); }
            get
            {
                if (!string.IsNullOrEmpty(_fileUri))
                    return _fileUri;
                var last = Permissions.Edit || Permissions.Review || Permissions.Comment;
                _fileUri = DocumentServiceConnector.ReplaceCommunityAdress(PathProvider.GetFileStreamUrl(Info.GetFile(), SharedLinkKey, last));
                return _fileUri;
            }
        }

        private DocumentServiceConnector DocumentServiceConnector { get; }
        private PathProvider PathProvider { get; }
    }

    [Transient]
    public class InfoConfig<T>
    {
        private File<T> File;

        public File<T> GetFile() => File;
        public void SetFile(File<T> file) => File = file;

        public EditorType Type { get; set; } = EditorType.Desktop;
        private string _breadCrumbs;

        public InfoConfig(BreadCrumbsManager breadCrumbsManager, FileSharing fileSharing, SecurityContext securityContext, UserManager userManager)
        {
            BreadCrumbsManager = breadCrumbsManager;
            FileSharing = fileSharing;
            SecurityContext = securityContext;
            UserManager = userManager;
        }

        public bool? Favorite
        {
            get
            {
                if (!SecurityContext.IsAuthenticated || UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor(UserManager)) return null;
                if (File.Encrypted) return null;
                return File.IsFavorite;
            }
        }

        public string Folder
        {
            get
            {
                if (Type == EditorType.Embedded || Type == EditorType.External) return null;
                if (string.IsNullOrEmpty(_breadCrumbs))
                {
                    const string crumbsSeporator = " \\ ";

                    var breadCrumbsList = BreadCrumbsManager.GetBreadCrumbsAsync(File.FolderID).Result;
                    _breadCrumbs = string.Join(crumbsSeporator, breadCrumbsList.Select(folder => folder.Title).ToArray());
                }

                return _breadCrumbs;
            }
        }

        public string Owner
        {
            get { return File.CreateByString; }
        }

        public string Uploaded
        {
            get { return File.CreateOnString; }
        }

        public List<AceShortWrapper> SharingSettings
        {
            get
            {
                if (Type == EditorType.Embedded
                    || Type == EditorType.External
                    || !FileSharing.CanSetAccessAsync(File).Result) return null;

                try
                {
                    return FileSharing.GetSharedInfoShortFileAsync(File.ID).Result;
                }
                catch
                {
                    return null;
                }
            }
        }

        private BreadCrumbsManager BreadCrumbsManager { get; }
        private FileSharing FileSharing { get; }
        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
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
            UserManager = userManager;
            AuthContext = authContext;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            Customization = customizationConfig;
            FilesSettingsHelper = filesSettingsHelper;
            DaoFactory = daoFactory;
            EntryManager = entryManager;
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
            get { return null; }
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
                if (!AuthContext.IsAuthenticated || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return null;
                if (!FilesSettingsHelper.TemplatesSection) return null;

                var extension = FileUtility.GetInternalExtension(_configuration.Document.Title).TrimStart('.');
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

                var folderDao = DaoFactory.GetFolderDao<int>();
                var fileDao = DaoFactory.GetFileDao<int>();
                var files = EntryManager.GetTemplatesAsync(folderDao, fileDao, filter, false, Guid.Empty, string.Empty, false).Result;
                var listTemplates = from file in files
                                    select
                                        new TemplatesConfig
                                        {
                                            Image = BaseCommonLinkUtility.GetFullAbsolutePath("skins/default/images/filetype/thumb/" + extension + ".png"),
                                            Name = file.Title,
                                            Title = file.Title,
                                            Url = BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileWebEditorUrl(file.ID))
                                        };
                return listTemplates.ToList();
            }
        }

        public string CallbackUrl { get; set; }

        public string CreateUrl
        {
            get
            {
                if (_configuration.Document.Info.Type != EditorType.Desktop) return null;
                if (!AuthContext.IsAuthenticated || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return null;

                return GetCreateUrl(_configuration.GetFileType);
            }
        }

        public PluginsConfig Plugins { get; set; }

        public CustomizationConfig<T> Customization { get; set; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private IDaoFactory DaoFactory { get; }
        private EntryManager EntryManager { get; }

        public EmbeddedConfig Embedded
        {
            set { _embeddedConfig = value; }
            get { return _configuration.Document.Info.Type == EditorType.Embedded ? _embeddedConfig : null; }
        }

        public EncryptionKeysConfig EncryptionKeys { get; set; }

        public string FileChoiceUrl { get; set; }

        public string Lang
        {
            get { return _userInfo.GetCulture().Name; }
        }

        public string Mode
        {
            get { return ModeWrite ? "edit" : "view"; }
        }

        private UserManager UserManager { get; }
        private AuthContext AuthContext { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUtility FileUtility { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }

        public string SaveAsUrl { get; set; }
        public List<RecentConfig> Recent
        {
            get
            {
                if (!AuthContext.IsAuthenticated || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return null;
                if (!FilesSettingsHelper.RecentSection) return null;

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

                var folderDao = DaoFactory.GetFolderDao<int>();
                var files = EntryManager.GetRecentAsync(filter, false, Guid.Empty, string.Empty, false).Result.Cast<File<int>>();

                var listRecent = from file in files
                                 where !Equals(_configuration.Document.Info.GetFile().ID, file.ID)
                                 select
                                     new RecentConfig
                                     {
                                         Folder = folderDao.GetFolderAsync(file.FolderID).Result.Title,
                                         Title = file.Title,
                                         Url = BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileWebEditorUrl(file.ID))
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

            return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FileHandlerPath)
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

        public string EmbedUrl
        {
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FilesBaseAbsolutePath + FilesLinkUtility.EditorPage + "?" + FilesLinkUtility.Action + "=embedded" + ShareLinkParam); }
        }

        public string SaveUrl
        {
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FileHandlerPath + "?" + FilesLinkUtility.Action + "=download" + ShareLinkParam); }
        }

        public string ShareUrl
        {
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FilesBaseAbsolutePath + FilesLinkUtility.EditorPage + "?" + FilesLinkUtility.Action + "=view" + ShareLinkParam); }
        }

        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private FilesLinkUtility FilesLinkUtility { get; }

        public string ToolbarDocked { get => "top"; }

        public EmbeddedConfig(BaseCommonLinkUtility baseCommonLinkUtility, FilesLinkUtility filesLinkUtility)
        {
            BaseCommonLinkUtility = baseCommonLinkUtility;
            FilesLinkUtility = filesLinkUtility;
        }
    }

    public class EncryptionKeysConfig
    {
        public string CryptoEngineId { get => "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}"; }

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

                if (CoreBaseSettings.Standalone
    || !TenantExtra.GetTenantQuota().Free)
                {
                    var easyBibHelper = ConsumerFactory.Get<EasyBibHelper>();
                    if (!string.IsNullOrEmpty(easyBibHelper.AppKey))
                    {
                        plugins.Add(BaseCommonLinkUtility.GetFullAbsolutePath("ThirdParty/plugin/easybib/config.json"));
                    }

                    var wordpressLoginProvider = ConsumerFactory.Get<WordpressLoginProvider>();
                    if (!string.IsNullOrEmpty(wordpressLoginProvider.ClientID) &&
                        !string.IsNullOrEmpty(wordpressLoginProvider.ClientSecret) &&
                        !string.IsNullOrEmpty(wordpressLoginProvider.RedirectUri))
                    {
                        plugins.Add(BaseCommonLinkUtility.GetFullAbsolutePath("ThirdParty/plugin/wordpress/config.json"));
                    }
                }

                return plugins.ToArray();
            }
        }

        private ConsumerFactory ConsumerFactory { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private TenantExtra TenantExtra { get; }

        public PluginsConfig(
            ConsumerFactory consumerFactory,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CoreBaseSettings coreBaseSettings,
            TenantExtra tenantExtra)
        {
            ConsumerFactory = consumerFactory;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            CoreBaseSettings = coreBaseSettings;
            TenantExtra = tenantExtra;
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
            CoreBaseSettings = coreBaseSettings;
            SettingsManager = settingsManager;
            FileUtility = fileUtility;
            FilesSettingsHelper = filesSettingsHelper;
            AuthContext = authContext;
            FileSecurity = fileSecurity;
            DaoFactory = daoFactory;
            GlobalFolderHelper = globalFolderHelper;
            PathProvider = pathProvider;
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


        public bool About
        {
            get { return !CoreBaseSettings.Standalone && !CoreBaseSettings.CustomMode; }
        }

        public CustomerConfig<T> Customer { get; set; }

        public FeedbackConfig Feedback
        {
            get
            {
                if (CoreBaseSettings.Standalone) return null;
                var settings = SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
                if (!settings.FeedbackAndSupportEnabled) return null;

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
                return FileUtility.CanForcesave
                       && !_configuration.Document.Info.GetFile().ProviderEntry
                       && ThirdPartySelector.GetAppByFileId(_configuration.Document.Info.GetFile().ID.ToString()) == null
                       && FilesSettingsHelper.Forcesave;
            }
        }

        public GobackConfig Goback
        {
            get
            {
                if (_configuration.EditorType == EditorType.Embedded || _configuration.EditorType == EditorType.External) return null;
                if (!AuthContext.IsAuthenticated) return null;
                //if (_gobackUrl != null)
                //{
                //    return new GobackConfig
                //    {
                //        Url = _gobackUrl,
                //    };
                //}

                var folderDao = DaoFactory.GetFolderDao<T>();
                try
                {
                    var parent = folderDao.GetFolderAsync(_configuration.Document.Info.GetFile().FolderID).Result;
                    var fileSecurity = FileSecurity;
                    if (_configuration.Document.Info.GetFile().RootFolderType == FolderType.USER
                        && !Equals(_configuration.Document.Info.GetFile().RootFolderId, GlobalFolderHelper.FolderMy)
                        && !fileSecurity.CanReadAsync(parent).Result)
                    {
                        if (fileSecurity.CanReadAsync(_configuration.Document.Info.GetFile()).Result)
                        {
                            return new GobackConfig
                            {
                                Url = PathProvider.GetFolderUrlByIdAsync(GlobalFolderHelper.FolderShareAsync.Result).Result,
                            };
                        }
                        return null;
                    }

                    if (_configuration.Document.Info.GetFile().Encrypted
                        && _configuration.Document.Info.GetFile().RootFolderType == FolderType.Privacy
                        && !fileSecurity.CanReadAsync(parent).Result)
                    {
                        parent = folderDao.GetFolderAsync(GlobalFolderHelper.GetFolderPrivacyAsync<T>().Result).Result;
                    }

                    return new GobackConfig
                    {
                        Url = PathProvider.GetFolderUrlAsync(parent).Result,
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
                return AuthContext.IsAuthenticated
                       && !_configuration.Document.Info.GetFile().Encrypted
                       && FileSharing.CanSetAccessAsync(_configuration.Document.Info.GetFile()).Result;
            }
        }

        public string ReviewDisplay
        {
            get { return _configuration.EditorConfig.ModeWrite ? null : "markup"; }
        }

        private CoreBaseSettings CoreBaseSettings { get; }
        private SettingsManager SettingsManager { get; }
        private FileUtility FileUtility { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private AuthContext AuthContext { get; }
        private FileSecurity FileSecurity { get; }
        private IDaoFactory DaoFactory { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private PathProvider PathProvider { get; }
    }

    [Transient]
    public class CustomerConfig<T>
    {
        public CustomerConfig(
            SettingsManager settingsManager,
            BaseCommonLinkUtility baseCommonLinkUtility,
            TenantLogoHelper tenantLogoHelper)
        {
            SettingsManager = settingsManager;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            TenantLogoHelper = tenantLogoHelper;
        }

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
        }

        public string Logo
        {
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.Dark, !_configuration.EditorConfig.Customization.IsRetina)); }
        }

        public string Name
        {
            get
            {
                return (SettingsManager.Load<TenantWhiteLabelSettings>().GetLogoText(SettingsManager) ?? "")
                    .Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("/", "\\/");
            }
        }

        private SettingsManager SettingsManager { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private TenantLogoHelper TenantLogoHelper { get; }
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
            CommonLinkUtility = commonLinkUtility;
            TenantLogoHelper = tenantLogoHelper;
            FileUtility = fileUtility;
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
                var fillingForm = FileUtility.CanWebRestrictedEditing(_configuration.Document.Title);

                return
                    _configuration.EditorType == EditorType.Embedded
                    || fillingForm
                        ? CommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditorEmbed, !_configuration.EditorConfig.Customization.IsRetina))
                        : CommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditor, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        public string ImageDark
        {
            set { }
            get
            {
                return CommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditor, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        public string ImageEmbedded
        {
            get
            {
                return
                    _configuration.EditorType != EditorType.Embedded
                        ? null
                        : CommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditorEmbed, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        public string Url
        {
            set { }
            get { return CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetDefault()); }
        }

        private CommonLinkUtility CommonLinkUtility { get; }
        private TenantLogoHelper TenantLogoHelper { get; }
        private FileUtility FileUtility { get; }
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