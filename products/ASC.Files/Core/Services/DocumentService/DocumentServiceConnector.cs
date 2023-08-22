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

namespace ASC.Web.Files.Services.DocumentService;

[Scope]
public class DocumentServiceConnector
{
    private readonly ILogger<DocumentServiceConnector> _logger;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly IHttpClientFactory _clientFactory;
    private readonly GlobalStore _globalStore;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly CoreSettings _coreSettings;
    private readonly PathProvider _pathProvider;

    public DocumentServiceConnector(
        ILogger<DocumentServiceConnector> logger,
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
        _logger = logger;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _globalStore = globalStore;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _coreSettings = coreSettings;
        _pathProvider = pathProvider;
        _clientFactory = clientFactory;
    }

    public static string GenerateRevisionId(string expectedKey)
    {
        return ASC.Files.Core.Helpers.DocumentService.GenerateRevisionId(expectedKey);
    }

    public async Task<(int ResultPercent, string ConvertedDocumentUri, string convertedFileType)> GetConvertedUriAsync(string documentUri,
                                      string fromExtension,
                                      string toExtension,
                                      string documentRevisionId,
                                      string password,
                                      string region,
                                      ThumbnailData thumbnail,
                                      SpreadsheetLayout spreadsheetLayout,
                                      bool isAsync)
    {
        _logger.DebugDocServiceConvert(fromExtension, toExtension, documentUri, _filesLinkUtility.DocServiceConverterUrl);
        try
        {
            return await ASC.Files.Core.Helpers.DocumentService.GetConvertedUriAsync(
                _fileUtility,
                _filesLinkUtility.DocServiceConverterUrl,
                documentUri,
                fromExtension,
                toExtension,
                GenerateRevisionId(documentRevisionId),
                password,
                region,
                thumbnail,
                spreadsheetLayout,
                isAsync,
                _fileUtility.SignatureSecret,
                _clientFactory);
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
        _logger.DebugDocServiceCommand(method, fileId.ToString(), docKeyForTrack, callbackUrl, users != null ? string.Join(", ", users) : null, JsonConvert.SerializeObject(meta));
        try
        {
            var commandResponse = await CommandRequestAsync(
                _fileUtility,
                _filesLinkUtility.DocServiceCommandUrl,
                method,
                GenerateRevisionId(docKeyForTrack),
                callbackUrl,
                users,
                meta,
                _fileUtility.SignatureSecret,
                _clientFactory);

            if (commandResponse.Error == ErrorTypes.NoError)
            {
                return true;
            }

            _logger.ErrorDocServiceCommandResponse(commandResponse.Error, commandResponse.ErrorString);
        }
        catch (Exception e)
        {
            _logger.ErrorDocServiceCommandError(e);
        }

        return false;
    }

    public async Task<(string BuilderKey, Dictionary<string, string> Urls)> DocbuilderRequestAsync(string requestKey,
                                           string inputScript,
                                           bool isAsync)
    {
        var urls = new Dictionary<string, string>();
        string scriptUrl = null;
        if (!string.IsNullOrEmpty(inputScript))
        {
            using (var stream = new MemoryStream())
            await using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(inputScript);
                await writer.FlushAsync();
                stream.Position = 0;
                scriptUrl = await _pathProvider.GetTempUrlAsync(stream, ".docbuilder");
            }
            scriptUrl = await ReplaceCommunityAdressAsync(scriptUrl);
            requestKey = null;
        }

        _logger.DebugDocServiceBuilderRequestKey(requestKey, isAsync);
        try
        {
            return await ASC.Files.Core.Helpers.DocumentService.DocbuilderRequestAsync(
                _fileUtility,
                _filesLinkUtility.DocServiceDocbuilderUrl,
                GenerateRevisionId(requestKey),
                scriptUrl,
                isAsync,
                _fileUtility.SignatureSecret,
                _clientFactory);
        }
        catch (Exception ex)
        {
            throw CustomizeError(ex);
        }
    }

    public async Task<string> GetVersionAsync()
    {
        _logger.DebugDocServiceRequestVersion();
        try
        {
            var commandResponse = await CommandRequestAsync(
                _fileUtility,
                _filesLinkUtility.DocServiceCommandUrl,
                CommandMethod.Version,
                GenerateRevisionId(null),
                null,
                null,
                null,
                _fileUtility.SignatureSecret,
                _clientFactory);

            var version = commandResponse.Version;
            if (string.IsNullOrEmpty(version))
            {
                version = "0";
            }

            if (commandResponse.Error == ErrorTypes.NoError)
            {
                return version;
            }

            _logger.ErrorDocServiceCommandResponse(commandResponse.Error, commandResponse.ErrorString);
        }
        catch (Exception e)
        {
            _logger.ErrorDocServiceCommandError(e);
        }

        return "4.1.5.1";
    }

    public async Task CheckDocServiceUrlAsync()
    {
        if (!string.IsNullOrEmpty(_filesLinkUtility.DocServiceHealthcheckUrl))
        {
            try
            {
                if (!await HealthcheckRequestAsync(_filesLinkUtility.DocServiceHealthcheckUrl, _clientFactory))
                {
                    throw new Exception("bad status");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorDocServiceHealthcheck(ex);

                throw new Exception("Healthcheck url: " + ex.Message);
            }
        }

        if (!string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl))
        {
            string convertedFileUri = null;
            try
            {
                const string fileExtension = ".docx";
                var toExtension = _fileUtility.GetInternalExtension(fileExtension);
                var url = _pathProvider.GetEmptyFileUrl(fileExtension);

                var fileUri = await ReplaceCommunityAdressAsync(url);

                var key = GenerateRevisionId(Guid.NewGuid().ToString());
                var uriTuple = await ASC.Files.Core.Helpers.DocumentService.GetConvertedUriAsync(_fileUtility, _filesLinkUtility.DocServiceConverterUrl, fileUri, fileExtension, toExtension, key, null, null, null, null, false, _fileUtility.SignatureSecret, _clientFactory);
                convertedFileUri = uriTuple.ConvertedDocumentUri;
            }
            catch (Exception ex)
            {
                _logger.ErrorConverterDocServiceCheckError(ex);

                throw new Exception("Converter url: " + ex.Message);
            }

            try
            {
                var request1 = new HttpRequestMessage
                {
                    RequestUri = new Uri(convertedFileUri)
                };

                using var httpClient = _clientFactory.CreateClient();
                using var response = await httpClient.SendAsync(request1);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Converted url is not available");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorDocumentDocServiceCheckError(ex);

                throw new Exception("Document server: " + ex.Message);
            }
        }

        if (!string.IsNullOrEmpty(_filesLinkUtility.DocServiceCommandUrl))
        {
            try
            {
                var key = GenerateRevisionId(Guid.NewGuid().ToString());
                await CommandRequestAsync(_fileUtility, _filesLinkUtility.DocServiceCommandUrl, CommandMethod.Version, key, null, null, null, _fileUtility.SignatureSecret, _clientFactory);
            }
            catch (Exception ex)
            {
                _logger.ErrorCommandDocServiceCheckError(ex);

                throw new Exception("Command url: " + ex.Message);
            }
        }

        if (!string.IsNullOrEmpty(_filesLinkUtility.DocServiceDocbuilderUrl))
        {
            try
            {
                var storeTemplate = await _globalStore.GetStoreTemplateAsync();
                var scriptUri = await storeTemplate.GetUriAsync("", "test.docbuilder");
                var scriptUrl = _baseCommonLinkUtility.GetFullAbsolutePath(scriptUri.ToString());
                scriptUrl = await ReplaceCommunityAdressAsync(scriptUrl);

                await ASC.Files.Core.Helpers.DocumentService.DocbuilderRequestAsync(_fileUtility, _filesLinkUtility.DocServiceDocbuilderUrl, null, scriptUrl, false, _fileUtility.SignatureSecret, _clientFactory);
            }
            catch (Exception ex)
            {
                _logger.ErrorDocServiceCheck(ex);

                throw new Exception("Docbuilder url: " + ex.Message);
            }
        }
    }

    public string ReplaceCommunityAdress(string url)
    {
        var docServicePortalUrl = _filesLinkUtility.DocServicePortalUrl;

        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        if (string.IsNullOrEmpty(docServicePortalUrl))
        {
            var tenant = _tenantManager.GetCurrentTenant();
            if (!_tenantExtra.Saas
                || string.IsNullOrEmpty(tenant.MappedDomain)
                || !url.StartsWith("https://" + tenant.MappedDomain))
            {
                return url;
            }

            docServicePortalUrl = "https://" + tenant.GetTenantDomain(_coreSettings, false);
        }

        var uri = new UriBuilder(url);
        if (new UriBuilder(_baseCommonLinkUtility.ServerRootPath).Host != uri.Host)
        {
            return url;
        }

        var urlRewriterQuery = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
        var query = HttpUtility.ParseQueryString(uri.Query);
        //        query[HttpRequestExtensions.UrlRewriterHeader] = urlRewriterQuery;
        uri.Query = query.ToString();

        var communityUrl = new UriBuilder(docServicePortalUrl);
        uri.Scheme = communityUrl.Scheme;
        uri.Host = communityUrl.Host;
        uri.Port = communityUrl.Port;

        return uri.ToString();
    }

    public async Task<string> ReplaceCommunityAdressAsync(string url)
    {
        var docServicePortalUrl = _filesLinkUtility.DocServicePortalUrl;

        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        if (string.IsNullOrEmpty(docServicePortalUrl))
        {
            var tenant = await _tenantManager.GetCurrentTenantAsync();
            if (!_tenantExtra.Saas
                || string.IsNullOrEmpty(tenant.MappedDomain)
                || !url.StartsWith("https://" + tenant.MappedDomain))
            {
                return url;
            }

            docServicePortalUrl = "https://" + tenant.GetTenantDomain(_coreSettings, false);
        }

        var uri = new UriBuilder(url);
        if (new UriBuilder(_baseCommonLinkUtility.ServerRootPath).Host != uri.Host)
        {
            return url;
        }

        var urlRewriterQuery = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
        var query = HttpUtility.ParseQueryString(uri.Query);
        //query[HttpRequestExtensions.UrlRewriterHeader] = urlRewriterQuery;
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
        var externalUri = new UriBuilder(_baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.DocServiceUrl)).ToString();
        var internalUri = new UriBuilder(_baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.DocServiceUrlInternal)).ToString();
        if (uri.StartsWith(internalUri, true, CultureInfo.InvariantCulture) || !uri.StartsWith(externalUri, true, CultureInfo.InvariantCulture))
        {
            return url;
        }

        uri = uri.Replace(externalUri, _filesLinkUtility.DocServiceUrlInternal);

        return uri;
    }

    private Exception CustomizeError(Exception ex)
    {
        var error = FilesCommonResource.ErrorMassage_DocServiceException;
        if (!string.IsNullOrEmpty(ex.Message))
        {
            error += $" ({ex.Message})";
        }

        _logger.ErrorDocServiceError(ex);

        return new Exception(error, ex);
    }
}
