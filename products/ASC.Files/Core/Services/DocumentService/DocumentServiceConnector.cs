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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Tenants;
using ASC.Files.Core.Resources;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using static ASC.Web.Core.Files.DocumentService;

using CommandMethod = ASC.Web.Core.Files.DocumentService.CommandMethod;

namespace ASC.Web.Files.Services.DocumentService
{
    [Scope]
    public class DocumentServiceConnector
    {
        public ILog Logger { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUtility FileUtility { get; }
        private IHttpClientFactory ClientFactory { get; }
        private GlobalStore GlobalStore { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private TenantManager TenantManager { get; }
        private TenantExtra TenantExtra { get; }
        private CoreSettings CoreSettings { get; }
        private PathProvider PathProvider { get; }

        public DocumentServiceConnector(
            IOptionsMonitor<ILog> optionsMonitor,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            PathProvider pathProvider,
            GlobalStore globalStore,
            BaseCommonLinkUtility baseCommonLinkUtility,
            TenantManager tenantManager,
            TenantExtra tenantExtra,
            CoreSettings coreSettings,
            IHttpClientFactory clientFactory)
        {
            Logger = optionsMonitor.CurrentValue;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            GlobalStore = globalStore;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            TenantManager = tenantManager;
            TenantExtra = tenantExtra;
            CoreSettings = coreSettings;
            PathProvider = pathProvider;
            ClientFactory = clientFactory;
        }

        public static string GenerateRevisionId(string expectedKey)
        {
            return Web.Core.Files.DocumentService.GenerateRevisionId(expectedKey);
        }

        public Task<(int ResultPercent, string ConvertedDocumentUri)> GetConvertedUriAsync(string documentUri,
                                          string fromExtension,
                                          string toExtension,
                                          string documentRevisionId,
                                          string password,
                                          ThumbnailData thumbnail,
                                          SpreadsheetLayout spreadsheetLayout,
                                          bool isAsync)
        {
            Logger.Debug($"DocService convert from {fromExtension} to {toExtension} - {documentUri}, DocServiceConverterUrl:{FilesLinkUtility.DocServiceConverterUrl}");
            try
            {
                return Web.Core.Files.DocumentService.GetConvertedUriAsync(
                    FileUtility,
                    FilesLinkUtility.DocServiceConverterUrl,
                    documentUri,
                    fromExtension,
                    toExtension,
                    GenerateRevisionId(documentRevisionId),
                    password,
                    thumbnail,
                    spreadsheetLayout,
                    isAsync,
                    FileUtility.SignatureSecret,
                    ClientFactory);
            }
            catch (Exception ex)
            {
                throw CustomizeError(ex);
            }
        }

        public async Task<bool> CommandAsync(CommandMethod method,
                                  string docKeyForTrack,
                                  object fileId = null,
                                  string callbackUrl = null,
                                  string[] users = null,
                                  MetaData meta = null)
        {
            Logger.DebugFormat("DocService command {0} fileId '{1}' docKey '{2}' callbackUrl '{3}' users '{4}' meta '{5}'", method, fileId, docKeyForTrack, callbackUrl, users != null ? string.Join(", ", users) : null, JsonConvert.SerializeObject(meta));
            try
            {
                var commandResponse = await CommandRequestAsync(
                    FileUtility,
                    FilesLinkUtility.DocServiceCommandUrl,
                    method,
                    GenerateRevisionId(docKeyForTrack),
                    callbackUrl,
                    users,
                    meta,
                    FileUtility.SignatureSecret,
                    ClientFactory);

                if (commandResponse.Error == CommandResponse.ErrorTypes.NoError)
                {
                    return true;
                }

                Logger.ErrorFormat("DocService command response: '{0}' {1}", commandResponse.Error, commandResponse.ErrorString);
            }
            catch (Exception e)
            {
                Logger.Error("DocService command error", e);
            }
            return false;
        }

        public async Task<(string BuilderKey,Dictionary<string, string> Urls)> DocbuilderRequestAsync(string requestKey,
                                               string inputScript,
                                               bool isAsync)
        {
            var urls = new Dictionary<string, string>();
            string scriptUrl = null;
            if (!string.IsNullOrEmpty(inputScript))
            {
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(inputScript);
                    await writer.FlushAsync();
                    stream.Position = 0;
                    scriptUrl = await PathProvider.GetTempUrlAsync(stream, ".docbuilder");
                }
                scriptUrl = ReplaceCommunityAdress(scriptUrl);
                requestKey = null;
            }

            Logger.DebugFormat("DocService builder requestKey {0} async {1}", requestKey, isAsync);
            try
            {
                return await Web.Core.Files.DocumentService.DocbuilderRequestAsync(
                    FileUtility,
                    FilesLinkUtility.DocServiceDocbuilderUrl,
                    GenerateRevisionId(requestKey),
                    scriptUrl,
                    isAsync,
                    FileUtility.SignatureSecret,
                    ClientFactory);
            }
            catch (Exception ex)
            {
                throw CustomizeError(ex);
            }
        }

        public async Task<string> GetVersionAsync()
        {
            Logger.DebugFormat("DocService request version");
            try
            {
                var commandResponse = await CommandRequestAsync(
                    FileUtility,
                    FilesLinkUtility.DocServiceCommandUrl,
                    CommandMethod.Version,
                    GenerateRevisionId(null),
                    null,
                    null,
                    null,
                    FileUtility.SignatureSecret,
                    ClientFactory);

                var version = commandResponse.Version;
                if (string.IsNullOrEmpty(version))
                {
                    version = "0";
                }

                if (commandResponse.Error == CommandResponse.ErrorTypes.NoError)
                {
                    return version;
                }

                Logger.ErrorFormat("DocService command response: '{0}' {1}", commandResponse.Error, commandResponse.ErrorString);
            }
            catch (Exception e)
            {
                Logger.Error("DocService command error", e);
            }
            return "4.1.5.1";
        }

        public async Task CheckDocServiceUrlAsync()
        {
            if (!string.IsNullOrEmpty(FilesLinkUtility.DocServiceHealthcheckUrl))
            {
                try
                {
                    if (!await HealthcheckRequestAsync(FilesLinkUtility.DocServiceHealthcheckUrl, ClientFactory))
                    {
                        throw new Exception("bad status");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Healthcheck DocService check error", ex);
                    throw new Exception("Healthcheck url: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(FilesLinkUtility.DocServiceConverterUrl))
            {
                string convertedFileUri = null;
                try
                {
                    const string fileExtension = ".docx";
                    var toExtension = FileUtility.GetInternalExtension(fileExtension);
                    var url = PathProvider.GetEmptyFileUrl(fileExtension);

                    var fileUri = ReplaceCommunityAdress(url);

                    var key = GenerateRevisionId(Guid.NewGuid().ToString());
                    var uriTuple = await Web.Core.Files.DocumentService.GetConvertedUriAsync(FileUtility, FilesLinkUtility.DocServiceConverterUrl, fileUri, fileExtension, toExtension, key, null, null, null, false, FileUtility.SignatureSecret, ClientFactory);
                    convertedFileUri = uriTuple.ConvertedDocumentUri;
                }
                catch (Exception ex)
                {
                    Logger.Error("Converter DocService check error", ex);
                    throw new Exception("Converter url: " + ex.Message);
                }

                try
                {
                    var request1 = new HttpRequestMessage();
                    request1.RequestUri = new Uri(convertedFileUri);

                    using var httpClient = ClientFactory.CreateClient();
                    using var response = await httpClient.SendAsync(request1);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Converted url is not available");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Document DocService check error", ex);
                    throw new Exception("Document server: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(FilesLinkUtility.DocServiceCommandUrl))
            {
                try
                {
                    var key = GenerateRevisionId(Guid.NewGuid().ToString());
                    await CommandRequestAsync(FileUtility, FilesLinkUtility.DocServiceCommandUrl, CommandMethod.Version, key, null, null, null, FileUtility.SignatureSecret, ClientFactory);
                }
                catch (Exception ex)
                {
                    Logger.Error("Command DocService check error", ex);
                    throw new Exception("Command url: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(FilesLinkUtility.DocServiceDocbuilderUrl))
            {
                try
                {
                    var storeTemplate = GlobalStore.GetStoreTemplate();
                    var scriptUri = await storeTemplate.GetUriAsync("", "test.docbuilder");
                    var scriptUrl = BaseCommonLinkUtility.GetFullAbsolutePath(scriptUri.ToString());
                    scriptUrl = ReplaceCommunityAdress(scriptUrl);

                    await Web.Core.Files.DocumentService.DocbuilderRequestAsync(FileUtility, FilesLinkUtility.DocServiceDocbuilderUrl, null, scriptUrl, false, FileUtility.SignatureSecret, ClientFactory);
                }
                catch (Exception ex)
                {
                    Logger.Error("DocService check error", ex);
                    throw new Exception("Docbuilder url: " + ex.Message);
                }
            }
        }

        public string ReplaceCommunityAdress(string url)
        {
            var docServicePortalUrl = FilesLinkUtility.DocServicePortalUrl;

            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            if (string.IsNullOrEmpty(docServicePortalUrl))
            {
                Tenant tenant = TenantManager.GetCurrentTenant();
                if (!TenantExtra.Saas
                    || string.IsNullOrEmpty(tenant.MappedDomain)
                    || !url.StartsWith("https://" + tenant.MappedDomain))
                {
                    return url;
                }

                docServicePortalUrl = "https://" + tenant.GetTenantDomain(CoreSettings, false);
            }

            var uri = new UriBuilder(url);
            if (new UriBuilder(BaseCommonLinkUtility.ServerRootPath).Host != uri.Host)
            {
                return url;
            }

            var urlRewriterQuery = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var query = HttpUtility.ParseQueryString(uri.Query);
            query[HttpRequestExtensions.UrlRewriterHeader] = urlRewriterQuery;
            uri.Query = query.ToString();

            var communityUrl = new UriBuilder(docServicePortalUrl);
            uri.Scheme = communityUrl.Scheme;
            uri.Host = communityUrl.Host;
            uri.Port = communityUrl.Port;

            return uri.ToString();
        }

        public string ReplaceDocumentAdress(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            var uri = new UriBuilder(url).ToString();
            var externalUri = new UriBuilder(BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.DocServiceUrl)).ToString();
            var internalUri = new UriBuilder(BaseCommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.DocServiceUrlInternal)).ToString();
            if (uri.StartsWith(internalUri, true, CultureInfo.InvariantCulture) || !uri.StartsWith(externalUri, true, CultureInfo.InvariantCulture))
            {
                return url;
            }

            uri = uri.Replace(externalUri, FilesLinkUtility.DocServiceUrlInternal);

            return uri;
        }

        private Exception CustomizeError(Exception ex)
        {
            var error = FilesCommonResource.ErrorMassage_DocServiceException;
            if (!string.IsNullOrEmpty(ex.Message))
                error += $" ({ex.Message})";

            Logger.Error("DocService error", ex);
            return new Exception(error, ex);
        }
    }
}