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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Security;
using ASC.Files.Resources;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.ThirdPartyApp;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.DependencyInjection;

using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Services.DocumentService
{
    public enum EditorType
    {
        Desktop,
        Mobile,
        Embedded,
        External,
    }

    [DataContract(Name = "editorConfig", Namespace = "")]
    public class Configuration<T>
    {
        public static readonly Dictionary<FileType, string> DocType = new Dictionary<FileType, string>
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
            Document.Info.File = file;
            EditorConfig = serviceProvider.GetService<EditorConfiguration<T>>();
            EditorConfig.SetConfiguration(this);
        }

        public EditorType Type
        {
            set { Document.Info.Type = value; }
            get { return Document.Info.Type; }
        }

        #region Property

        [DataMember(Name = "document")]
        public DocumentConfig<T> Document;

        [DataMember(Name = "documentType")]
        public string DocumentType
        {
            set { }
            get
            {
                DocType.TryGetValue(GetFileType, out var documentType);
                return documentType;
            }
        }

        [DataMember(Name = "editorConfig")]
        public EditorConfiguration<T> EditorConfig;

        [DataMember(Name = "token", EmitDefaultValue = false)]
        public string Token;

        [DataMember(Name = "type")]
        public string TypeString
        {
            set { Type = (EditorType)Enum.Parse(typeof(EditorType), value, true); }
            get { return Type.ToString().ToLower(); }
        }

        internal FileType GetFileType
        {
            set { }
            get
            {
                if (_fileTypeCache == FileType.Unknown)
                    _fileTypeCache = FileUtility.GetFileTypeByFileName(Document.Info.File.Title);
                return _fileTypeCache;
            }
        }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string ErrorMessage;

        #endregion

        public static string Serialize(Configuration<T> configuration)
        {
            using var ms = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(Configuration<T>));
            serializer.WriteObject(ms, configuration);
            ms.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }
    }
    #region Nested Classes

    [DataContract(Name = "document", Namespace = "")]
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


        [DataMember(Name = "fileType")]
        public string FileType
        {
            set { }
            get { return Info.File.ConvertedExtension.Trim('.'); }
        }

        [DataMember(Name = "info")]
        public InfoConfig<T> Info;

        [DataMember(Name = "key")]
        public string Key
        {
            set { _key = value; }
            get { return DocumentServiceConnector.GenerateRevisionId(_key); }
        }

        [DataMember(Name = "permissions")]
        public PermissionsConfig Permissions;

        [DataMember(Name = "title")]
        public string Title
        {
            set { _title = value; }
            get { return _title ?? Info.File.Title; }
        }

        [DataMember(Name = "url")]
        public string Url
        {
            set { _fileUri = DocumentServiceConnector.ReplaceCommunityAdress(value); }
            get
            {
                if (!string.IsNullOrEmpty(_fileUri))
                    return _fileUri;
                var last = Permissions.Edit || Permissions.Review || Permissions.Comment;
                _fileUri = DocumentServiceConnector.ReplaceCommunityAdress(PathProvider.GetFileStreamUrl(Info.File, SharedLinkKey, last));
                return _fileUri;
            }
        }

        public DocumentServiceConnector DocumentServiceConnector { get; }
        public PathProvider PathProvider { get; }
    }

    [DataContract(Name = "info", Namespace = "")]
    public class InfoConfig<T>
    {
        public File<T> File;

        public EditorType Type = EditorType.Desktop;
        private string _breadCrumbs;

        public InfoConfig(BreadCrumbsManager breadCrumbsManager, FileSharing fileSharing)
        {
            BreadCrumbsManager = breadCrumbsManager;
            FileSharing = fileSharing;
        }

        [Obsolete("Use owner (since v5.4)")]
        [DataMember(Name = "author")]
        public string Aouthor
        {
            set { }
            get { return File.CreateByString; }
        }

        [Obsolete("Use uploaded (since v5.4)")]
        [DataMember(Name = "created")]
        public string Created
        {
            set { }
            get { return File.CreateOnString; }
        }

        [DataMember(Name = "folder", EmitDefaultValue = false)]
        public string Folder
        {
            set { }
            get
            {
                if (Type == EditorType.Embedded || Type == EditorType.External) return null;
                if (string.IsNullOrEmpty(_breadCrumbs))
                {
                    const string crumbsSeporator = " \\ ";

                    var breadCrumbsList = BreadCrumbsManager.GetBreadCrumbs(File.FolderID);
                    _breadCrumbs = string.Join(crumbsSeporator, breadCrumbsList.Select(folder => folder.Title).ToArray());
                }

                return _breadCrumbs;
            }
        }

        [DataMember(Name = "owner")]
        public string Owner
        {
            set { }
            get { return File.CreateByString; }
        }

        [DataMember(Name = "uploaded")]
        public string Uploaded
        {
            set { }
            get { return File.CreateOnString; }
        }

        [DataMember(Name = "sharingSettings", EmitDefaultValue = false)]
        public ItemList<AceShortWrapper> SharingSettings
        {
            set { }
            get
            {
                if (Type == EditorType.Embedded
                    || Type == EditorType.External
                    || !FileSharing.CanSetAccess(File)) return null;

                try
                {
                    return FileSharing.GetSharedInfoShort<string>(File.UniqID);
                }
                catch
                {
                    return null;
                }
            }
        }

        public BreadCrumbsManager BreadCrumbsManager { get; }
        public FileSharing FileSharing { get; }
    }

    [DataContract(Name = "permissions", Namespace = "")]
    public class PermissionsConfig
    {
        [Obsolete("Since DS v5.5")]
        [DataMember(Name = "changeHistory")]
        public bool ChangeHistory = false;

        [DataMember(Name = "comment")]
        public bool Comment = true;

        [DataMember(Name = "download")]
        public bool Download = true;

        [DataMember(Name = "edit")]
        public bool Edit = true;

        [DataMember(Name = "fillForms")]
        public bool FillForms = true;

        [DataMember(Name = "print")]
        public bool Print = true;

        [DataMember(Name = "rename")]
        public bool Rename = false;

        [DataMember(Name = "review")]
        public bool Review = true;
    }

    [DataContract(Name = "editorConfig", Namespace = "")]
    public class EditorConfiguration<T>
    {
        public EditorConfiguration(
            UserManager userManager,
            AuthContext authContext,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            FilesLinkUtility filesLinkUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            PluginsConfig pluginsConfig,
            EmbeddedConfig embeddedConfig,
            CustomizationConfig<T> customizationConfig)
        {
            UserManager = userManager;
            AuthContext = authContext;
            FilesLinkUtility = filesLinkUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            Customization = customizationConfig;
            Plugins = pluginsConfig;
            Embedded = embeddedConfig;
            _userInfo = userManager.GetUsers(authContext.CurrentAccount.ID);

            User = _userInfo.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID)
                       ? new UserConfig
                       {
                           Id = Guid.NewGuid().ToString(),
                           Name = FilesCommonResource.Guest,
                       }
                       : new UserConfig
                       {
                           Id = _userInfo.ID.ToString(),
                           Name = _userInfo.DisplayUserName(false, displayUserSettingsHelper),
                       };
        }

        public bool ModeWrite = false;

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
            Customization.SetConfiguration(_configuration);
        }

        private readonly UserInfo _userInfo;
        private EmbeddedConfig _embeddedConfig;

        [DataMember(Name = "actionLink", EmitDefaultValue = false)]
        public ActionLinkConfig ActionLink;

        public string ActionLinkString
        {
            get { return null; }
            set
            {
                try
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(ActionLinkConfig));
                        ActionLink = (ActionLinkConfig)serializer.ReadObject(ms);
                    }
                }
                catch (Exception)
                {
                    ActionLink = null;
                }
            }
        }

        [DataMember(Name = "callbackUrl", EmitDefaultValue = false)]
        public string CallbackUrl;

        [DataMember(Name = "createUrl", EmitDefaultValue = false)]
        public string CreateUrl
        {
            set { }
            get
            {
                if (_configuration.Document.Info.Type != EditorType.Desktop) return null;
                if (!AuthContext.IsAuthenticated || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return null;

                return GetCreateUrl(_configuration.GetFileType);
            }
        }

        [DataMember(Name = "plugins", EmitDefaultValue = false)]
        public PluginsConfig Plugins;

        [DataMember(Name = "customization", EmitDefaultValue = false)]
        public CustomizationConfig<T> Customization;

        [DataMember(Name = "embedded", EmitDefaultValue = false)]
        public EmbeddedConfig Embedded
        {
            set { _embeddedConfig = value; }
            get { return _configuration.Document.Info.Type == EditorType.Embedded ? _embeddedConfig : null; }
        }

        [DataMember(Name = "fileChoiceUrl", EmitDefaultValue = false)]
        public string FileChoiceUrl;

        [DataMember(Name = "lang")]
        public string Lang
        {
            set { }
            get { return _userInfo.GetCulture().Name; }
        }

        //todo: remove old feild after release 5.2+
        [DataMember(Name = "mergeFolderUrl", EmitDefaultValue = false)]
        public string MergeFolderUrl;

        [DataMember(Name = "mode")]
        public string Mode
        {
            set { }
            get { return ModeWrite ? "edit" : "view"; }
        }

        public UserManager UserManager { get; }
        public AuthContext AuthContext { get; }
        public FilesLinkUtility FilesLinkUtility { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }

        [DataMember(Name = "saveAsUrl", EmitDefaultValue = false)]
        public string SaveAsUrl;

        [DataMember(Name = "sharingSettingsUrl", EmitDefaultValue = false)]
        public string SharingSettingsUrl;

        [DataMember(Name = "user")]
        public UserConfig User;

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

    [DataContract(Name = "actionLink", Namespace = "")]
    public class ActionLinkConfig
    {
        [DataMember(Name = "action", EmitDefaultValue = false)]
        public ActionConfig Action;


        [DataContract(Name = "action", Namespace = "")]
        public class ActionConfig
        {
            [DataMember(Name = "type", EmitDefaultValue = false)]
            public string Type;

            [DataMember(Name = "data", EmitDefaultValue = false)]
            public string Data;
        }


        public static string Serialize(ActionLinkConfig actionLinkConfig)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(ActionLinkConfig));
                serializer.WriteObject(ms, actionLinkConfig);
                ms.Seek(0, SeekOrigin.Begin);
                return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }
    }

    [DataContract(Name = "embedded", Namespace = "")]
    public class EmbeddedConfig
    {
        public string ShareLinkParam;

        [DataMember(Name = "embedUrl", EmitDefaultValue = false)]
        public string EmbedUrl
        {
            set { }
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FilesBaseAbsolutePath + FilesLinkUtility.EditorPage + "?" + FilesLinkUtility.Action + "=embedded" + ShareLinkParam); }
        }

        [DataMember(Name = "saveUrl", EmitDefaultValue = false)]
        public string SaveUrl
        {
            set { }
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FileHandlerPath + "?" + FilesLinkUtility.Action + "=download" + ShareLinkParam); }
        }

        [DataMember(Name = "shareUrl", EmitDefaultValue = false)]
        public string ShareUrl
        {
            set { }
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.FilesBaseAbsolutePath + FilesLinkUtility.EditorPage + "?" + FilesLinkUtility.Action + "=view" + ShareLinkParam); }
        }

        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        public FilesLinkUtility FilesLinkUtility { get; }

        [DataMember(Name = "toolbarDocked")]
        public string ToolbarDocked = "top";

        public EmbeddedConfig(BaseCommonLinkUtility baseCommonLinkUtility, FilesLinkUtility filesLinkUtility)
        {
            BaseCommonLinkUtility = baseCommonLinkUtility;
            FilesLinkUtility = filesLinkUtility;
        }
    }

    [DataContract(Name = "plugins", Namespace = "")]
    public class PluginsConfig
    {
        [DataMember(Name = "pluginsData", EmitDefaultValue = false)]
        public string[] PluginsData
        {
            set { }
            get
            {
                var plugins = new List<string>();

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

                return plugins.ToArray();
            }
        }

        public ConsumerFactory ConsumerFactory { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }

        public PluginsConfig(ConsumerFactory consumerFactory, BaseCommonLinkUtility baseCommonLinkUtility)
        {
            ConsumerFactory = consumerFactory;
            BaseCommonLinkUtility = baseCommonLinkUtility;
        }
    }

    [DataContract(Name = "customization", Namespace = "")]
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
            WebImageSupplier webImageSupplier,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CustomerConfig<T> customerConfig,
            LogoConfig<T> logoConfig)
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
            WebImageSupplier = webImageSupplier;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            Customer = customerConfig;
            Logo = logoConfig;
        }

        private Configuration<T> _configuration;

        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
            Customer.SetConfiguration(_configuration);
            Logo.SetConfiguration(_configuration);
        }

        public string GobackUrl;
        public bool IsRetina = false;


        [DataMember(Name = "about")]
        public bool About
        {
            set { }
            get { return !CoreBaseSettings.Standalone && !CoreBaseSettings.CustomMode; }
        }

        [DataMember(Name = "customer")]
        public CustomerConfig<T> Customer;

        [DataMember(Name = "feedback", EmitDefaultValue = false)]
        public FeedbackConfig Feedback
        {
            set { }
            get
            {
                if (CoreBaseSettings.Standalone) return null;
                if (!AdditionalWhiteLabelSettings.Instance(SettingsManager).FeedbackAndSupportEnabled) return null;

                return new FeedbackConfig
                {
                    Url = BaseCommonLinkUtility.GetRegionalUrl(
                            AdditionalWhiteLabelSettings.Instance(SettingsManager).FeedbackAndSupportUrl,
                            CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                };
            }
        }

        [DataMember(Name = "forcesave", EmitDefaultValue = false)]
        public bool Forcesave
        {
            set { }
            get
            {
                return FileUtility.CanForcesave
                       && !_configuration.Document.Info.File.ProviderEntry
                       && ThirdPartySelector.GetAppByFileId(_configuration.Document.Info.File.ID.ToString()) == null
                       && FilesSettingsHelper.Forcesave;
            }
        }

        [DataMember(Name = "goback", EmitDefaultValue = false)]
        public GobackConfig Goback
        {
            set { }
            get
            {
                if (_configuration.Type == EditorType.Embedded || _configuration.Type == EditorType.External) return null;
                if (!AuthContext.IsAuthenticated) return null;
                if (GobackUrl != null)
                {
                    return new GobackConfig
                    {
                        Url = GobackUrl,
                    };
                }

                var folderDao = DaoFactory.GetFolderDao<T>();
                try
                {
                    var parent = folderDao.GetFolder(_configuration.Document.Info.File.FolderID);
                    var fileSecurity = FileSecurity;
                    if (_configuration.Document.Info.File.RootFolderType == FolderType.USER
                        && !Equals(_configuration.Document.Info.File.RootFolderId, GlobalFolderHelper.FolderMy)
                        && !fileSecurity.CanRead(parent))
                    {
                        if (fileSecurity.CanRead(_configuration.Document.Info.File))
                        {
                            return new GobackConfig
                            {
                                Url = PathProvider.GetFolderUrl(GlobalFolderHelper.FolderShare),
                            };
                        }
                        return null;
                    }

                    return new GobackConfig
                    {
                        Url = PathProvider.GetFolderUrl(parent),
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        [DataMember(Name = "loaderLogo", EmitDefaultValue = false)]
        public string LoaderLogo
        {
            set { }
            get
            {
                return CoreBaseSettings.CustomMode
                           ? BaseCommonLinkUtility.GetFullAbsolutePath(WebImageSupplier.GetAbsoluteWebPath("loader.svg").ToLower())
                           : null;
            }
        }

        [DataMember(Name = "loaderName", EmitDefaultValue = false)]
        public string LoaderName
        {
            set { }
            get
            {
                return CoreBaseSettings.CustomMode
                           ? " "
                           : null;
            }
        }

        [DataMember(Name = "logo")]
        public LogoConfig<T> Logo;

        [DataMember(Name = "reviewDisplay", EmitDefaultValue = false)]
        public string ReviewDisplay
        {
            set { }
            get { return _configuration.EditorConfig.ModeWrite ? null : "markup"; }
        }

        public CoreBaseSettings CoreBaseSettings { get; }
        public SettingsManager SettingsManager { get; }
        public FileUtility FileUtility { get; }
        public FilesSettingsHelper FilesSettingsHelper { get; }
        public AuthContext AuthContext { get; }
        public FileSecurity FileSecurity { get; }
        public IDaoFactory DaoFactory { get; }
        public GlobalFolderHelper GlobalFolderHelper { get; }
        public PathProvider PathProvider { get; }
        public WebImageSupplier WebImageSupplier { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
    }

    [DataContract(Name = "customer", Namespace = "")]
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

        [DataMember(Name = "logo")]
        public string Logo
        {
            set { }
            get { return BaseCommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.Dark, !_configuration.EditorConfig.Customization.IsRetina)); }
        }

        [DataMember(Name = "name")]
        public string Name
        {
            set { }
            get
            {
                return (SettingsManager.Load<TenantWhiteLabelSettings>().GetLogoText(SettingsManager) ?? "")
                    .Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("/", "\\/");
            }
        }

        public SettingsManager SettingsManager { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        public TenantLogoHelper TenantLogoHelper { get; }
    }

    [DataContract(Name = "feedback", Namespace = "")]
    public class FeedbackConfig
    {
        [DataMember(Name = "url")]
        public string Url;

        [DataMember(Name = "visible")]
        public bool Visible = true;
    }

    [DataContract(Name = "goback", Namespace = "")]
    public class GobackConfig
    {
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string Url;
    }

    [DataContract(Name = "logo", Namespace = "")]
    public class LogoConfig<T>
    {
        public LogoConfig(
            SettingsManager settingsManager,
            BaseCommonLinkUtility baseCommonLinkUtility,
            TenantLogoHelper tenantLogoHelper)
        {
            BaseCommonLinkUtility = baseCommonLinkUtility;
            TenantLogoHelper = tenantLogoHelper;
            SettingsManager = settingsManager;
        }

        private Configuration<T> _configuration;
        internal void SetConfiguration(Configuration<T> configuration)
        {
            _configuration = configuration;
        }

        [DataMember(Name = "image")]
        public string Image
        {
            set { }
            get
            {
                return
                    _configuration.Type == EditorType.Embedded
                        ? null
                        : BaseCommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditor, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        [DataMember(Name = "imageEmbedded", EmitDefaultValue = false)]
        public string ImageEmbedded
        {
            set { }
            get
            {
                return
                    _configuration.Type != EditorType.Embedded
                        ? null
                        : BaseCommonLinkUtility.GetFullAbsolutePath(TenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.Dark, !_configuration.EditorConfig.Customization.IsRetina));
            }
        }

        [DataMember(Name = "url")]
        public string Url
        {
            set { }
            get { return CompanyWhiteLabelSettings.Instance(SettingsManager).Site; }
        }

        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        public TenantLogoHelper TenantLogoHelper { get; }
        public SettingsManager SettingsManager { get; }
    }

    [DataContract(Name = "user", Namespace = "")]
    public class UserConfig
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id;

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name;
    }

    public static class ConfigurationExtention
    {
        public static DIHelper AddConfigurationService(this DIHelper services)
        {
            return services
                .AddDocumentConfigService()
                .AddEditorConfigurationService();
        }

        public static DIHelper AddDocumentConfigService(this DIHelper services)
        {
            services.TryAddTransient<DocumentConfig<string>>();
            services.TryAddTransient<DocumentConfig<int>>();

            return services
                .AddDocumentServiceConnectorService()
                .AddPathProviderService()
                .AddInfoConfigService();
        }

        public static DIHelper AddInfoConfigService(this DIHelper services)
        {
            services.TryAddTransient<InfoConfig<string>>();
            services.TryAddTransient<InfoConfig<int>>();

            return services
                .AddBreadCrumbsManagerService()
                .AddFileSharingService();
        }

        public static DIHelper AddEditorConfigurationService(this DIHelper services)
        {
            services.TryAddTransient<EditorConfiguration<string>>();
            services.TryAddTransient<EditorConfiguration<int>>();

            return services
                .AddUserManagerService()
                .AddAuthContextService()
                .AddDisplayUserSettingsService()
                .AddFilesLinkUtilityService()
                .AddBaseCommonLinkUtilityService()
                .AddPluginsConfigService()
                .AddEmbeddedConfigService()
                .AddCustomizationConfigService();
        }

        public static DIHelper AddPluginsConfigService(this DIHelper services)
        {
            services.TryAddTransient<PluginsConfig>();

            return services
                .AddConsumerFactoryService()
                .AddBaseCommonLinkUtilityService();
        }

        public static DIHelper AddEmbeddedConfigService(this DIHelper services)
        {
            services.TryAddTransient<EmbeddedConfig>();

            return services
                .AddFilesLinkUtilityService()
                .AddBaseCommonLinkUtilityService();
        }

        public static DIHelper AddCustomizationConfigService(this DIHelper services)
        {
            services.TryAddTransient<CustomizationConfig<string>>();
            services.TryAddTransient<CustomizationConfig<int>>();

            return services
                .AddCoreBaseSettingsService()
                .AddSettingsManagerService()
                .AddFileUtilityService()
                .AddFilesSettingsHelperService()
                .AddAuthContextService()
                .AddFileSecurityService()
                .AddDaoFactoryService()
                .AddGlobalFolderHelperService()
                .AddPathProviderService()
                .AddWebImageSupplierService()
                .AddBaseCommonLinkUtilityService()
                .AddCustomerConfigService()
                .AddLogoConfigService();
        }

        public static DIHelper AddCustomerConfigService(this DIHelper services)
        {
            services.TryAddTransient<CustomerConfig<string>>();
            services.TryAddTransient<CustomerConfig<int>>();

            return services
                .AddSettingsManagerService()
                .AddBaseCommonLinkUtilityService()
                .AddTenantLogoHelperService();
        }

        public static DIHelper AddLogoConfigService(this DIHelper services)
        {
            services.TryAddTransient<LogoConfig<string>>();
            services.TryAddTransient<LogoConfig<int>>();

            return services
                .AddSettingsManagerService()
                .AddBaseCommonLinkUtilityService()
                .AddTenantLogoHelperService();
        }
    }
}