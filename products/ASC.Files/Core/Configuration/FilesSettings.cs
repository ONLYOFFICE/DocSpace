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

namespace ASC.Web.Files.Classes;

[Serializable]
public class FilesSettings : ISettings<FilesSettings>
{
    [JsonPropertyName("EnableThirdpartySettings")]
    public bool EnableThirdpartySetting { get; set; }

    [JsonPropertyName("FastDelete")]
    public bool FastDeleteSetting { get; set; }

    [JsonPropertyName("StoreOriginalFiles")]
    public bool StoreOriginalFilesSetting { get; set; }

    [JsonPropertyName("KeepNewFileName")]
    public bool KeepNewFileName { get; set; }

    [JsonPropertyName("UpdateIfExist")]
    public bool UpdateIfExistSetting { get; set; }

    [JsonPropertyName("ConvertNotify")]
    public bool ConvertNotifySetting { get; set; }

    [JsonPropertyName("DefaultSortedBy")]
    public SortedByType DefaultSortedBySetting { get; set; }

    [JsonPropertyName("DefaultSortedAsc")]
    public bool DefaultSortedAscSetting { get; set; }

    [JsonPropertyName("HideConfirmConvertSave")]
    public bool HideConfirmConvertSaveSetting { get; set; }

    [JsonPropertyName("HideConfirmConvertOpen")]
    public bool HideConfirmConvertOpenSetting { get; set; }

    [JsonPropertyName("Forcesave")]
    public bool ForcesaveSetting { get; set; }

    [JsonPropertyName("StoreForcesave")]
    public bool StoreForcesaveSetting { get; set; }

    [JsonPropertyName("HideRecent")]
    public bool HideRecentSetting { get; set; }

    [JsonPropertyName("HideFavorites")]
    public bool HideFavoritesSetting { get; set; }

    [JsonPropertyName("HideTemplates")]
    public bool HideTemplatesSetting { get; set; }

    [JsonPropertyName("DownloadZip")]
    public bool DownloadTarGzSetting { get; set; }

    [JsonPropertyName("ShareLink")]
    public bool DisableShareLinkSetting { get; set; }

    [JsonPropertyName("ShareLinkSocialMedia")]
    public bool DisableShareSocialMediaSetting { get; set; }

    [JsonPropertyName("AutomaticallyCleanUp")]
    public AutoCleanUpData AutomaticallyCleanUpSetting { get; set; }

    [JsonPropertyName("DefaultSharingAccessRights")]
    public List<FileShare> DefaultSharingAccessRightsSetting { get; set; }

    public FilesSettings GetDefault()
    {
        return new FilesSettings
        {
            FastDeleteSetting = false,
            EnableThirdpartySetting = true,
            StoreOriginalFilesSetting = true,
            UpdateIfExistSetting = false,
            ConvertNotifySetting = true,
            DefaultSortedBySetting = SortedByType.DateAndTime,
            DefaultSortedAscSetting = false,
            HideConfirmConvertSaveSetting = false,
            HideConfirmConvertOpenSetting = false,
            ForcesaveSetting = true,
            StoreForcesaveSetting = false,
            HideRecentSetting = false,
            HideFavoritesSetting = false,
            HideTemplatesSetting = false,
            DownloadTarGzSetting = false,
            AutomaticallyCleanUpSetting = null,
            DefaultSharingAccessRightsSetting = null,
        };
    }

    [JsonIgnore]
    public Guid ID => new Guid("{03B382BD-3C20-4f03-8AB9-5A33F016316E}");
}

[Scope]
public class FilesSettingsHelper
{
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly SetupInfo _setupInfo;
    private readonly FileUtility _fileUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly SearchSettingsHelper _searchSettingsHelper;
    private readonly AuthContext _authContext;
    private static readonly FilesSettings _emptySettings = new();

    public FilesSettingsHelper(
        SettingsManager settingsManager,
        CoreBaseSettings coreBaseSettings,
        SetupInfo setupInfo,
        FileUtility fileUtility,
        FilesLinkUtility filesLinkUtility,
        SearchSettingsHelper searchSettingsHelper,
        AuthContext authContext)
    {
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
        _setupInfo = setupInfo;
        _fileUtility = fileUtility;
        _filesLinkUtility = filesLinkUtility;
        _searchSettingsHelper = searchSettingsHelper;
        _authContext = authContext;
    }

    public List<string> ExtsImagePreviewed => _fileUtility.ExtsImagePreviewed;
    public List<string> ExtsMediaPreviewed => _fileUtility.ExtsMediaPreviewed;
    public List<string> ExtsWebPreviewed => _fileUtility.ExtsWebPreviewed;
    public List<string> ExtsWebEdited => _fileUtility.ExtsWebEdited;
    public List<string> ExtsWebEncrypt => _fileUtility.ExtsWebEncrypt;
    public List<string> ExtsWebReviewed => _fileUtility.ExtsWebReviewed;
    public List<string> ExtsWebCustomFilterEditing => _fileUtility.ExtsWebCustomFilterEditing;
    public List<string> ExtsWebRestrictedEditing => _fileUtility.ExtsWebRestrictedEditing;
    public List<string> ExtsWebCommented => _fileUtility.ExtsWebCommented;
    public List<string> ExtsWebTemplate => _fileUtility.ExtsWebTemplate;
    public List<string> ExtsCoAuthoring => _fileUtility.ExtsCoAuthoring;
    public List<string> ExtsMustConvert => _fileUtility.ExtsMustConvert;
    public Dictionary<string, List<string>> ExtsConvertible => _fileUtility.GetExtsConvertible();
    public List<string> ExtsUploadable => _fileUtility.ExtsUploadable;
    public List<string> ExtsArchive => FileUtility.ExtsArchive;
    public List<string> ExtsVideo => FileUtility.ExtsVideo;
    public List<string> ExtsAudio => FileUtility.ExtsAudio;
    public List<string> ExtsImage => FileUtility.ExtsImage;
    public List<string> ExtsSpreadsheet => FileUtility.ExtsSpreadsheet;
    public List<string> ExtsPresentation => FileUtility.ExtsPresentation;
    public List<string> ExtsDocument => FileUtility.ExtsDocument;
    public Dictionary<FileType, string> InternalFormats => _fileUtility.InternalExtension;
    public string MasterFormExtension => _fileUtility.MasterFormExtension;
    public string ParamVersion => FilesLinkUtility.Version;
    public string ParamOutType => FilesLinkUtility.OutType;
    public string FileDownloadUrlString => _filesLinkUtility.FileDownloadUrlString;
    public string FileWebViewerUrlString => _filesLinkUtility.FileWebViewerUrlString;
    public string FileWebViewerExternalUrlString => _filesLinkUtility.FileWebViewerExternalUrlString;
    public string FileWebEditorUrlString => _filesLinkUtility.FileWebEditorUrlString;
    public string FileWebEditorExternalUrlString => _filesLinkUtility.FileWebEditorExternalUrlString;
    public string FileRedirectPreviewUrlString => _filesLinkUtility.FileRedirectPreviewUrlString;
    public string FileThumbnailUrlString => _filesLinkUtility.FileThumbnailUrlString;

    public bool ConfirmDelete
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.FastDeleteSetting = !value;
            SaveForCurrentUser(setting);
        }
        get => !LoadForCurrentUser().FastDeleteSetting;
    }

    public bool EnableThirdParty
    {
        set
        {
            var setting = _settingsManager.Load<FilesSettings>();
            setting.EnableThirdpartySetting = value;
            _settingsManager.Save(setting);
        }
        get => _settingsManager.Load<FilesSettings>().EnableThirdpartySetting;
    }

    public bool ExternalShare
    {
        set
        {
            var settings = Load();
            settings.DisableShareLinkSetting = !value;
            Save(settings);
        }
        get { return !Load().DisableShareLinkSetting; }
    }

    public bool ExternalShareSocialMedia
    {
        set
        {
            var settings = Load();
            settings.DisableShareSocialMediaSetting = !value;
            Save(settings);
        }
        get
        {
            var setting = Load();
            return !setting.DisableShareLinkSetting && !setting.DisableShareSocialMediaSetting;
        }
    }

    public bool StoreOriginalFiles
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.StoreOriginalFilesSetting = value;
            SaveForCurrentUser(setting);
        }
        get => LoadForCurrentUser().StoreOriginalFilesSetting;
    }

    public bool KeepNewFileName
    {
        set => _settingsManager.ManageForCurrentUser<FilesSettings>(setting => setting.KeepNewFileName = value);
        get => LoadForCurrentUser().KeepNewFileName;
    }

    public bool UpdateIfExist
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.UpdateIfExistSetting = value;
            SaveForCurrentUser(setting);
        }
        get => LoadForCurrentUser().UpdateIfExistSetting;
    }

    public bool ConvertNotify
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.ConvertNotifySetting = value;
            SaveForCurrentUser(setting);
        }
        get => LoadForCurrentUser().ConvertNotifySetting;
    }

    public bool HideConfirmConvertSave
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.HideConfirmConvertSaveSetting = value;
            SaveForCurrentUser(setting);
        }
        get => LoadForCurrentUser().HideConfirmConvertSaveSetting;
    }

    public bool HideConfirmConvertOpen
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.HideConfirmConvertOpenSetting = value;
            SaveForCurrentUser(setting);
        }
        get => LoadForCurrentUser().HideConfirmConvertOpenSetting;
    }

    public OrderBy DefaultOrder
    {
        set
        {
            var setting = LoadForCurrentUser();
            if (setting.DefaultSortedBySetting != value.SortedBy || setting.DefaultSortedAscSetting != value.IsAsc)
            {
                setting.DefaultSortedBySetting = value.SortedBy;
                setting.DefaultSortedAscSetting = value.IsAsc;
                SaveForCurrentUser(setting);
            }
        }
        get
        {
            var setting = LoadForCurrentUser();

            return new OrderBy(setting.DefaultSortedBySetting, setting.DefaultSortedAscSetting);
        }
    }

    public bool Forcesave
    {
        set
        {
            //var setting = LoadForCurrentUser();
            //setting.ForcesaveSetting = value;
            //SaveForCurrentUser(setting);
        }
        get => true;//LoadForCurrentUser().ForcesaveSetting;
    }

    public bool StoreForcesave
    {
        set
        {
            //if (_coreBaseSettings.Personal)
            //{
            //    throw new NotSupportedException();
            //}

            //var setting = _settingsManager.Load<FilesSettings>();
            //setting.StoreForcesaveSetting = value;
            //_settingsManager.Save(setting);
        }
        get => false;//!_coreBaseSettings.Personal && _settingsManager.Load<FilesSettings>().StoreForcesaveSetting;
    }

    public bool RecentSection
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.HideRecentSetting = !value;
            SaveForCurrentUser(setting);
        }
        get => !LoadForCurrentUser().HideRecentSetting;
    }

    public bool FavoritesSection
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.HideFavoritesSetting = !value;
            SaveForCurrentUser(setting);
        }
        get => !LoadForCurrentUser().HideFavoritesSetting;
    }

    public bool TemplatesSection
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.HideTemplatesSetting = !value;
            SaveForCurrentUser(setting);
        }
        get => !LoadForCurrentUser().HideTemplatesSetting;
    }

    public bool DownloadTarGz
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.DownloadTarGzSetting = value;
            SaveForCurrentUser(setting);
        }
        get => LoadForCurrentUser().DownloadTarGzSetting;
    }
    public AutoCleanUpData AutomaticallyCleanUp
    {
        set
        {
            var setting = LoadForCurrentUser();
            setting.AutomaticallyCleanUpSetting = value;
            SaveForCurrentUser(setting);
        }
        get
        {
            var setting = LoadForCurrentUser().AutomaticallyCleanUpSetting;

            if (setting != null)
            {
                return setting;
            }

            setting = new AutoCleanUpData { IsAutoCleanUp = true, Gap = DateToAutoCleanUp.ThirtyDays };
            AutomaticallyCleanUp = setting;

            return setting;
        }
    }

    public bool CanSearchByContent
    {
        get
        {
            return _searchSettingsHelper.CanSearchByContentAsync<DbFile>().Result;
        }
    }

    public List<FileShare> DefaultSharingAccessRights
    {
        set
        {
            List<FileShare> GetNormalizedList(List<FileShare> src)
            {
                if (src == null || !src.Any())
                {
                    return null;
                }

                var res = new List<FileShare>();

                if (src.Contains(FileShare.FillForms))
                {
                    res.Add(FileShare.FillForms);
                }

                if (src.Contains(FileShare.CustomFilter))
                {
                    res.Add(FileShare.CustomFilter);
                }

                if (src.Contains(FileShare.Review))
                {
                    res.Add(FileShare.Review);
                }

                if (src.Contains(FileShare.ReadWrite))
                {
                    res.Add(FileShare.ReadWrite);
                    return res;
                }

                if (src.Contains(FileShare.Comment))
                {
                    res.Add(FileShare.Comment);
                    return res;
                }

                res.Add(FileShare.Read);
                return res;
            }

            var setting = LoadForCurrentUser();
            setting.DefaultSharingAccessRightsSetting = GetNormalizedList(value);
            SaveForCurrentUser(setting);
        }
        get
        {
            var setting = LoadForCurrentUser().DefaultSharingAccessRightsSetting;
            return setting ?? new List<FileShare>() { FileShare.Read };
        }
    }

    public long ChunkUploadSize
    {
        get => _setupInfo.ChunkUploadSize;
    }

    private FilesSettings Load()
    {
        return !_authContext.IsAuthenticated ? _emptySettings : _settingsManager.Load<FilesSettings>();
    }

    private void Save(FilesSettings settings)
    {
        if (!_authContext.IsAuthenticated)
        {
            return;
        }
        
        _settingsManager.Save(settings);
    }

    private FilesSettings LoadForCurrentUser()
    {
        return !_authContext.IsAuthenticated ? _emptySettings : _settingsManager.LoadForCurrentUser<FilesSettings>();
    }

    private void SaveForCurrentUser(FilesSettings settings)
    {
        if (!_authContext.IsAuthenticated)
        {
            return;
        }
        
        _settingsManager.SaveForCurrentUser(settings);
    }
}
