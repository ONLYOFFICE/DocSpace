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

namespace ASC.Files.Core.Helpers;

[Scope]
public class FilesLinkUtility
{
    public const string FilesBaseVirtualPath = "~/";
    public const string EditorPage = "doceditor";
    private readonly string _filesUploaderURL;

    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
    private readonly IConfiguration _configuration;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly ExternalShare _externalShare;

    public FilesLinkUtility(
        CommonLinkUtility commonLinkUtility,
        BaseCommonLinkUtility baseCommonLinkUtility,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        InstanceCrypto instanceCrypto, 
        ExternalShare externalShare)
    {
        _commonLinkUtility = commonLinkUtility;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
        _configuration = configuration;
        _instanceCrypto = instanceCrypto;
        _externalShare = externalShare;
        _filesUploaderURL = _configuration["files:uploader:url"] ?? "~";
    }

    public string FilesBaseAbsolutePath
    {
        get { return _baseCommonLinkUtility.ToAbsolute(FilesBaseVirtualPath); }
    }

    public const string FileId = "fileid";
    public const string FolderId = "folderid";
    public const string Version = "version";
    public const string FileUri = "fileuri";
    public const string FileTitle = "title";
    public const string Action = "action";
    public const string DocShareKey = "doc";
    public const string TryParam = "try";
    public const string FolderUrl = "folderurl";
    public const string OutType = "outputtype";
    public const string AuthKey = "stream_auth";
    public const string Anchor = "anchor";
    public const string Size = "size";
    public const string FolderShareKey = "share";

    public string FileHandlerPath
    {
        get { return FilesBaseAbsolutePath + "filehandler.ashx"; }
    }

    public string DocServiceUrl
    {
        get
        {
            var url = GetUrlSetting("public");
            if (!string.IsNullOrEmpty(url) && url != "/")
            {
                url = url.TrimEnd('/') + "/";
            }
            return url;
        }
        set
        {
            SetUrlSetting("api", null);

            value = (value ?? "").Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(value))
            {
                value = value.TrimEnd('/') + "/";
                if (!new Regex(@"(^https?:\/\/)|^\/", RegexOptions.CultureInvariant).IsMatch(value))
                {
                    value = "http://" + value;
                }
            }

            SetUrlSetting("public", value);
        }
    }

    public string DocServiceUrlInternal
    {
        get
        {
            var url = GetUrlSetting("internal");
            if (string.IsNullOrEmpty(url))
            {
                url = DocServiceUrl;
            }
            else
            {
                url = url.TrimEnd('/') + "/";
            }
            return url;
        }
        set
        {
            SetUrlSetting("converter", null);
            SetUrlSetting("storage", null);
            SetUrlSetting("command", null);
            SetUrlSetting("docbuilder", null);

            value = (value ?? "").Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(value))
            {
                value = value.TrimEnd('/') + "/";
                if (!new Regex(@"(^https?:\/\/)", RegexOptions.CultureInvariant).IsMatch(value))
                {
                    value = "http://" + value;
                }
            }

            SetUrlSetting("internal", value);
        }
    }

    public string DocServiceApiUrl
    {
        get
        {
            var url = GetUrlSetting("api");
            if (string.IsNullOrEmpty(url))
            {
                url = DocServiceUrl;
                if (!string.IsNullOrEmpty(url))
                {
                    url += "web-apps/apps/api/documents/api.js";
                }
            }
            return url;
        }
    }

    public string DocServiceConverterUrl
    {
        get
        {
            var url = GetUrlSetting("converter");
            if (string.IsNullOrEmpty(url))
            {
                url = DocServiceUrlInternal;
                if (!string.IsNullOrEmpty(url))
                {
                    url += "ConvertService.ashx";
                }
            }
            return url;
        }
    }

    public string DocServiceCommandUrl
    {
        get
        {
            var url = GetUrlSetting("command");
            if (string.IsNullOrEmpty(url))
            {
                url = DocServiceUrlInternal;
                if (!string.IsNullOrEmpty(url))
                {
                    url += "coauthoring/CommandService.ashx";
                }
            }
            return url;
        }
    }

    public string DocServiceDocbuilderUrl
    {
        get
        {
            var url = GetUrlSetting("docbuilder");
            if (string.IsNullOrEmpty(url))
            {
                url = DocServiceUrlInternal;
                if (!string.IsNullOrEmpty(url))
                {
                    url += "docbuilder";
                }
            }
            return url;
        }
    }

    public string DocServiceHealthcheckUrl
    {
        get
        {
            var url = GetUrlSetting("healthcheck");
            if (string.IsNullOrEmpty(url))
            {
                url = DocServiceUrlInternal;
                if (!string.IsNullOrEmpty(url))
                {
                    url += "healthcheck";
                }
            }
            return url;
        }
    }

    public string DocServicePortalUrl
    {
        get { return GetUrlSetting("portal"); }
        set
        {
            value = (value ?? "").Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(value))
            {
                value = value.TrimEnd('/') + "/";
                if (!new Regex(@"(^https?:\/\/)", RegexOptions.CultureInvariant).IsMatch(value))
                {
                    value = "http://" + value;
                }
            }

            SetUrlSetting("portal", value);
        }
    }

    public string FileDownloadUrlString
    {
        get { return FileHandlerPath + "?" + Action + "=download&" + FileId + "={0}"; }
    }

    public string GetFileDownloadUrl(object fileId)
    {
        return GetFileDownloadUrl(fileId, 0, string.Empty);
    }

    public string GetFileDownloadUrl(object fileId, int fileVersion, string convertToExtension)
    {
        var url = string.Format(FileDownloadUrlString, HttpUtility.UrlEncode(fileId.ToString()))
               + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty)
               + (string.IsNullOrEmpty(convertToExtension) ? string.Empty : "&" + OutType + "=" + convertToExtension);

        return GetUrlWithShare(url);
    }

    public string GetFileWebMediaViewUrl(object fileId)
    {
        var url = FilesBaseAbsolutePath + "#preview/" + HttpUtility.UrlEncode(fileId.ToString());

        return GetUrlWithShare(url);
    }

    public string FileWebViewerUrlString
    {
        get { return $"{FileWebEditorUrlString}&{Action}=view"; }
    }

    public string FileWebViewerExternalUrlString
    {
        get { return FilesBaseAbsolutePath + EditorPage + "?" + FileUri + "={0}&" + FileTitle + "={1}&" + FolderUrl + "={2}"; }
    }

    public string FileWebEditorUrlString
    {
        get { return $"/{EditorPage}?{FileId}={{0}}"; }
    }

    public string GetFileWebEditorUrl<T>(T fileId, int fileVersion = 0)
    {
        var url = string.Format(FileWebEditorUrlString, HttpUtility.UrlEncode(fileId.ToString()))
            + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty);

        return GetUrlWithShare(url);
    }

    public string GetFileWebEditorTryUrl(FileType fileType)
    {
        return FilesBaseAbsolutePath + EditorPage + "?" + TryParam + "=" + fileType;
    }

    public string FileWebEditorExternalUrlString
    {
        get { return FileHandlerPath + "?" + Action + "=create&" + FileUri + "={0}&" + FileTitle + "={1}"; }
    }

    public string GetFileWebEditorExternalUrl(string fileUri, string fileTitle)
    {
        return GetFileWebEditorExternalUrl(fileUri, fileTitle, false);
    }

    public string GetFileWebEditorExternalUrl(string fileUri, string fileTitle, bool openFolder)
    {
        var url = string.Format(FileWebEditorExternalUrlString, HttpUtility.UrlEncode(fileUri), HttpUtility.UrlEncode(fileTitle));
        if (openFolder)
        {
            url += "&openfolder=true";
        }

        return url;
    }

    public string GetFileWebPreviewUrl(FileUtility fileUtility, string fileTitle, object fileId, int fileVersion = 0)
    {
        if (fileUtility.CanImageView(fileTitle) || fileUtility.CanMediaView(fileTitle))
        {
            return GetFileWebMediaViewUrl(fileId);
        }

        if (fileUtility.CanWebView(fileTitle))
        {
            if (fileUtility.ExtsMustConvert.Contains(FileUtility.GetFileExtension(fileTitle)))
            {
                var url = string.Format(FileWebViewerUrlString, HttpUtility.UrlEncode(fileId.ToString()));
                return GetUrlWithShare(url);
            }

            return GetFileWebEditorUrl(fileId, fileVersion);
        }

        return GetFileDownloadUrl(fileId);
    }

    public string FileRedirectPreviewUrlString
    {
        get { return FileHandlerPath + "?" + Action + "=redirect"; }
    }

    public string GetFileRedirectPreviewUrl(object enrtyId, bool isFile)
    {
        return FileRedirectPreviewUrlString + "&" + (isFile ? FileId : FolderId) + "=" + HttpUtility.UrlEncode(enrtyId.ToString());
    }

    public string FileThumbnailUrlString
    {
        get { return FileHandlerPath + "?" + Action + "=thumb&" + FileId + "={0}"; }
    }

    public string GetFileThumbnailUrl(object fileId, int fileVersion)
    {
        var url = string.Format(FileThumbnailUrlString, HttpUtility.UrlEncode(fileId.ToString()))
               + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty);

        return GetUrlWithShare(url);
    }


    public string GetInitiateUploadSessionUrl(int tenantId, object folderId, object fileId, string fileName, long contentLength, bool encrypted, SecurityContext securityContext)
    {
        var queryString = string.Format("?initiate=true&{0}={1}&fileSize={2}&tid={3}&userid={4}&culture={5}&encrypted={6}",
                                        FileTitle,
                                        HttpUtility.UrlEncode(fileName),
                                        contentLength,
                                        tenantId,
                                        HttpUtility.UrlEncode(_instanceCrypto.Encrypt(securityContext.CurrentAccount.ID.ToString())),
                                        CultureInfo.CurrentUICulture.Name,
                                        encrypted.ToString().ToLower());

        if (fileId != null)
        {
            queryString = queryString + "&" + FileId + "=" + HttpUtility.UrlEncode(fileId.ToString());
        }

        if (folderId != null)
        {
            queryString = queryString + "&" + FolderId + "=" + HttpUtility.UrlEncode(folderId.ToString());
        }

        return _commonLinkUtility.GetFullAbsolutePath(GetFileUploaderHandlerVirtualPath() + queryString);
    }

    public string GetUploadChunkLocationUrl(string uploadId)
    {
        var queryString = "?uid=" + uploadId;
        return _commonLinkUtility.GetFullAbsolutePath(GetFileUploaderHandlerVirtualPath() + queryString);
    }

    public bool IsLocalFileUploader
    {
        get { return !Regex.IsMatch(_filesUploaderURL, "^http(s)?://\\.*"); }
    }

    private string GetFileUploaderHandlerVirtualPath()
    {
        var virtualPath = _filesUploaderURL;
        return virtualPath.EndsWith(".ashx") ? virtualPath : virtualPath.TrimEnd('/') + "/ChunkedUploader.ashx";
    }

    private string GetUrlSetting(string key, string appSettingsKey = null)
    {
        var value = string.Empty;
        if (_coreBaseSettings.Standalone)
        {
            value = _coreSettings.GetSetting(GetSettingsKey(key));
        }
        if (string.IsNullOrEmpty(value))
        {
            value = _configuration["files:docservice:url:" + (appSettingsKey ?? key)];
        }
        return value;
    }

    private void SetUrlSetting(string key, string value)
    {
        if (!_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException("Method for server edition only.");
        }
        value = (value ?? "").Trim();
        if (string.IsNullOrEmpty(value))
        {
            value = null;
        }

        if (GetUrlSetting(key) != value)
        {
             _coreSettings.SaveSetting(GetSettingsKey(key), value);
        }
    }

    private string GetSettingsKey(string key)
    {
        return "DocKey_" + key;
    }
    
    private string GetUrlWithShare(string url)
    {
        if (_externalShare.GetLinkId() == default)
        {
            return url;
        }

        var key = _externalShare.GetKey();

        if (!string.IsNullOrEmpty(key))
        {
            url += $"&{FolderShareKey}={key}";
        }

        return url;
    }
}
