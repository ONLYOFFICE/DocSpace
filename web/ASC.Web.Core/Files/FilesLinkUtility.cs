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
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;

namespace ASC.Web.Core.Files
{
    [Scope]
    public class FilesLinkUtility
    {
        public const string FilesBaseVirtualPath = "~/products/files/";
        public const string EditorPage = "doceditor";
        private readonly string FilesUploaderURL;
        private CommonLinkUtility CommonLinkUtility { get; set; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private CoreSettings CoreSettings { get; set; }
        private IConfiguration Configuration { get; }
        private InstanceCrypto InstanceCrypto { get; }

        public FilesLinkUtility(
            CommonLinkUtility commonLinkUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            InstanceCrypto instanceCrypto)
        {
            CommonLinkUtility = commonLinkUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            CoreBaseSettings = coreBaseSettings;
            CoreSettings = coreSettings;
            Configuration = configuration;
            InstanceCrypto = instanceCrypto;
            FilesUploaderURL = Configuration["files:uploader:url"] ?? "~";
        }

        public string FilesBaseAbsolutePath
        {
            get { return BaseCommonLinkUtility.ToAbsolute(FilesBaseVirtualPath); }
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

        public string FileHandlerPath
        {
            get { return FilesBaseAbsolutePath + "httphandlers/filehandler.ashx"; }
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
            return string.Format(FileDownloadUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty)
                   + (string.IsNullOrEmpty(convertToExtension) ? string.Empty : "&" + OutType + "=" + convertToExtension);
        }

        public string GetFileWebMediaViewUrl(object fileId)
        {
            return FilesBaseAbsolutePath + "#preview/" + HttpUtility.UrlEncode(fileId.ToString());
        }

        public string FileWebViewerUrlString
        {
            get { return FileWebEditorUrlString + "&" + Action + "=view"; }
        }

        public string GetFileWebViewerUrlForMobile(object fileId, int fileVersion)
        {
            var viewerUrl = CommonLinkUtility.ToAbsolute("~/../products/files/") + EditorPage + "?" + FileId + "={0}";

            return string.Format(viewerUrl, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty);
        }

        public string FileWebViewerExternalUrlString
        {
            get { return FilesBaseAbsolutePath + EditorPage + "?" + FileUri + "={0}&" + FileTitle + "={1}&" + FolderUrl + "={2}"; }
        }

        public string GetFileWebViewerExternalUrl(string fileUri, string fileTitle, string refererUrl = "")
        {
            return string.Format(FileWebViewerExternalUrlString, HttpUtility.UrlEncode(fileUri), HttpUtility.UrlEncode(fileTitle), HttpUtility.UrlEncode(refererUrl));
        }

        public string FileWebEditorUrlString
        {
            get { return FilesBaseAbsolutePath + EditorPage + "?" + FileId + "={0}"; }
        }

        public string GetFileWebEditorUrl(object fileId, int fileVersion = 0)
        {
            return string.Format(FileWebEditorUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty);
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
                url += "&openfolder=true";
            return url;
        }

        public string GetFileWebPreviewUrl(FileUtility fileUtility, string fileTitle, object fileId, int fileVersion = 0)
        {
            if (fileUtility.CanImageView(fileTitle) || fileUtility.CanMediaView(fileTitle))
                return GetFileWebMediaViewUrl(fileId);

            if (fileUtility.CanWebView(fileTitle))
            {
                if (fileUtility.ExtsMustConvert.Contains(FileUtility.GetFileExtension(fileTitle)))
                    return string.Format(FileWebViewerUrlString, HttpUtility.UrlEncode(fileId.ToString()));
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
            return string.Format(FileThumbnailUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty);
        }


        public string GetInitiateUploadSessionUrl(int tenantId, object folderId, object fileId, string fileName, long contentLength, bool encrypted, SecurityContext securityContext)
        {
            var queryString = string.Format("?initiate=true&{0}={1}&fileSize={2}&tid={3}&userid={4}&culture={5}&encrypted={6}",
                                            FileTitle,
                                            HttpUtility.UrlEncode(fileName),
                                            contentLength,
                                            tenantId,
                                            HttpUtility.UrlEncode(InstanceCrypto.Encrypt(securityContext.CurrentAccount.ID.ToString())),
                                            Thread.CurrentThread.CurrentUICulture.Name,
                                            encrypted.ToString().ToLower());

            if (fileId != null)
                queryString = queryString + "&" + FileId + "=" + HttpUtility.UrlEncode(fileId.ToString());

            if (folderId != null)
                queryString = queryString + "&" + FolderId + "=" + HttpUtility.UrlEncode(folderId.ToString());

            return CommonLinkUtility.GetFullAbsolutePath(GetFileUploaderHandlerVirtualPath() + queryString);
        }

        public string GetUploadChunkLocationUrl(string uploadId)
        {
            var queryString = "?uid=" + uploadId;
            return CommonLinkUtility.GetFullAbsolutePath(GetFileUploaderHandlerVirtualPath() + queryString);
        }

        public bool IsLocalFileUploader
        {
            get { return !Regex.IsMatch(FilesUploaderURL, "^http(s)?://\\.*"); }
        }

        private string GetFileUploaderHandlerVirtualPath()
        {
            var virtualPath = FilesUploaderURL;
            return virtualPath.EndsWith(".ashx") ? virtualPath : virtualPath.TrimEnd('/') + "/ChunkedUploader.ashx";
        }

        private string GetUrlSetting(string key, string appSettingsKey = null)
        {
            var value = string.Empty;
            if (CoreBaseSettings.Standalone)
            {
                value = CoreSettings.GetSetting(GetSettingsKey(key));
            }
            if (string.IsNullOrEmpty(value))
            {
                value = Configuration["files:docservice:url:" + (appSettingsKey ?? key)];
            }
            return value;
        }

        private void SetUrlSetting(string key, string value)
        {
            if (!CoreBaseSettings.Standalone)
            {
                throw new NotSupportedException("Method for server edition only.");
            }
            value = (value ?? "").Trim();
            if (string.IsNullOrEmpty(value)) value = null;
            if (GetUrlSetting(key) != value)
                CoreSettings.SaveSetting(GetSettingsKey(key), value);
        }

        private string GetSettingsKey(string key)
        {
            return "DocKey_" + key;
        }
    }
}