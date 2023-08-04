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

/// <summary>
/// Class service connector
/// </summary>
public static class DocumentService
{
    /// <summary>
    /// Timeout to request conversion
    /// </summary>
    public static readonly int Timeout = 120000;
    //public static int Timeout = Convert.ToInt32(ConfigurationManagerExtension.AppSettings["files.docservice.timeout"] ?? "120000");

    /// <summary>
    /// Number of tries request conversion
    /// </summary>
    public static readonly int MaxTry = 3;

    /// <summary>
    /// Translation key to a supported form.
    /// </summary>
    /// <param name="expectedKey">Expected key</param>
    /// <returns>Supported key</returns>
    public static string GenerateRevisionId(string expectedKey)
    {
        expectedKey ??= "";
        const int maxLength = 128;
        using var sha256 = SHA256.Create();
        if (expectedKey.Length > maxLength)
        {
            expectedKey = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(expectedKey)));
        }

        var key = Regex.Replace(expectedKey, "[^0-9a-zA-Z_]", "_");
        return key.Substring(key.Length - Math.Min(key.Length, maxLength));
    }

    /// <summary>
    /// The method is to convert the file to the required format
    /// </summary>
    /// <param name="fileUtility"></param>
    /// <param name="documentConverterUrl">Url to the service of conversion</param>
    /// <param name="documentUri">Uri for the document to convert</param>
    /// <param name="fromExtension">Document extension</param>
    /// <param name="toExtension">Extension to which to convert</param>
    /// <param name="documentRevisionId">Key for caching on service</param>
    /// <param name="password">Password</param>
    /// <param name="region"></param>
    /// <param name="thumbnail">Thumbnail settings</param>
    /// <param name="spreadsheetLayout"></param>
    /// <param name="isAsync">Perform conversions asynchronously</param>
    /// <param name="signatureSecret">Secret key to generate the token</param>
    /// <param name="clientFactory"></param>
    /// <returns>The percentage of completion of conversion</returns>
    /// <example>
    /// string convertedDocumentUri;
    /// GetConvertedUri("http://helpcenter.teamlab.com/content/GettingStarted.pdf", ".pdf", ".docx", "469971047", false, out convertedDocumentUri);
    /// </example>
    /// <exception>
    /// </exception>

    public static Task<(int ResultPercent, string ConvertedDocumentUri, string convertedFileType)> GetConvertedUriAsync(
        FileUtility fileUtility,
        string documentConverterUrl,
        string documentUri,
        string fromExtension,
        string toExtension,
        string documentRevisionId,
        string password,
        string region,
        ThumbnailData thumbnail,
        SpreadsheetLayout spreadsheetLayout,
        bool isAsync,
        string signatureSecret,
       IHttpClientFactory clientFactory)
    {
        fromExtension = string.IsNullOrEmpty(fromExtension) ? Path.GetExtension(documentUri) : fromExtension;
        if (string.IsNullOrEmpty(fromExtension))
        {
            throw new ArgumentNullException(nameof(fromExtension), "Document's extension for conversion is not known");
        }

        if (string.IsNullOrEmpty(toExtension))
        {
            throw new ArgumentNullException(nameof(toExtension), "Extension for conversion is not known");
        }

        return InternalGetConvertedUriAsync(fileUtility, documentConverterUrl, documentUri, fromExtension, toExtension, documentRevisionId, password, region, thumbnail, spreadsheetLayout, isAsync, signatureSecret, clientFactory);
    }

    private static async Task<(int ResultPercent, string ConvertedDocumentUri, string convertedFileType)> InternalGetConvertedUriAsync(
       FileUtility fileUtility,
       string documentConverterUrl,
       string documentUri,
       string fromExtension,
       string toExtension,
       string documentRevisionId,
       string password,
       string region,
       ThumbnailData thumbnail,
       SpreadsheetLayout spreadsheetLayout,
       bool isAsync,
       string signatureSecret,
       IHttpClientFactory clientFactory)
    {
        var title = Path.GetFileName(documentUri ?? "");
        title = string.IsNullOrEmpty(title) || title.Contains('?') ? Guid.NewGuid().ToString() : title;

        documentRevisionId = string.IsNullOrEmpty(documentRevisionId)
                                 ? documentUri
                                 : documentRevisionId;
        documentRevisionId = GenerateRevisionId(documentRevisionId);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(documentConverterUrl),
            Method = HttpMethod.Post
        };
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        var httpClient = clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout);

        var body = new ConvertionBody
        {
            Async = isAsync,
            FileType = fromExtension.Trim('.'),
            Key = documentRevisionId,
            OutputType = toExtension.Trim('.'),
            Title = title,
            Thumbnail = thumbnail,
            SpreadsheetLayout = spreadsheetLayout,
            Url = documentUri,
            Region = region
        };

        if (!string.IsNullOrEmpty(password))
        {
            body.Password = password;
        }

        if (!string.IsNullOrEmpty(signatureSecret))
        {
            var payload = new Dictionary<string, object>
                    {
                        { "payload", body }
                    };

            var token = JsonWebToken.Encode(payload, signatureSecret);
            //todo: remove old scheme
            request.Headers.Add(fileUtility.SignatureHeader, "Bearer " + token);

            token = JsonWebToken.Encode(body, signatureSecret);
            body.Token = token;
        }

        var bodyString = JsonSerializer.Serialize(body, new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

        string dataResponse;
        HttpResponseMessage response = null;
        Stream responseStream = null;
        try
        {
            var countTry = 0;
            while (countTry < MaxTry)
            {
                try
                {
                    countTry++;
                    response = await httpClient.SendAsync(request);
                    responseStream = await response.Content.ReadAsStreamAsync();
                    break;
                }
                catch (HttpRequestException ex)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, ex.Message, ex);
                }
            }
            if (countTry == MaxTry)
            {
                throw new HttpRequestException("Timeout");
            }

            if (responseStream == null)
            {
                throw new WebException("Could not get an answer");
            }

            using var reader = new StreamReader(responseStream);
            dataResponse = await reader.ReadToEndAsync();
        }
        finally
        {
            if (responseStream != null)
            {
                responseStream.Dispose();
            }

            if (response != null)
            {
                response.Dispose();
            }
        }

        return GetResponseUri(dataResponse);
    }

    /// <summary>
    /// Request to Document Server with command
    /// </summary>
    /// <param name="fileUtility"></param>
    /// <param name="documentTrackerUrl">Url to the command service</param>
    /// <param name="method">Name of method</param>
    /// <param name="documentRevisionId">Key for caching on service, whose used in editor</param>
    /// <param name="callbackUrl">Url to the callback handler</param>
    /// <param name="users">users id for drop</param>
    /// <param name="meta">file meta data for update</param>
    /// <param name="signatureSecret">Secret key to generate the token</param>
    /// <param name="clientFactory"></param>
    /// <returns>Response</returns>

    public static async Task<CommandResponse> CommandRequestAsync(FileUtility fileUtility,
        string documentTrackerUrl,
        CommandMethod method,
        string documentRevisionId,
        string callbackUrl,
        string[] users,
        MetaData meta,
        string signatureSecret,
        IHttpClientFactory clientFactory)
    {
        var cancellationTokenSource = new CancellationTokenSource(Timeout);
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(documentTrackerUrl),
            Method = HttpMethod.Post
        };

        var httpClient = clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout);

        var body = new CommandBody
        {
            Command = method,
            Key = documentRevisionId,
        };

        if (!string.IsNullOrEmpty(callbackUrl))
        {
            body.Callback = callbackUrl;
        }

        if (users != null && users.Length > 0)
        {
            body.Users = users;
        }

        if (meta != null)
        {
            body.Meta = meta;
        }

        if (!string.IsNullOrEmpty(signatureSecret))
        {
            var payload = new Dictionary<string, object>
                {
                    { "payload", body }
                };

#pragma warning disable CS0618 // Type or member is obsolete
            var encoder = new JwtEncoder(new HMACSHA256Algorithm(),
                                                              new JsonNetSerializer(),
                                                              new JwtBase64UrlEncoder());
#pragma warning restore CS0618 // Type or member is obsolete

            var token = encoder.Encode(payload, signatureSecret);
            //todo: remove old scheme
            request.Headers.Add(fileUtility.SignatureHeader, "Bearer " + token);

            token = encoder.Encode(body, signatureSecret);
            body.Token = token;
        }

        var bodyString = JsonSerializer.Serialize(body, new System.Text.Json.JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

        string dataResponse;
        using (var response = await httpClient.SendAsync(request, cancellationTokenSource.Token))
        await using (var stream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token))
        {
            if (stream == null)
            {
                throw new Exception("Response is null");
            }

            using var reader = new StreamReader(stream);
            dataResponse = await reader.ReadToEndAsync();
        }


        try
        {
            var commandResponse = JsonSerializer.Deserialize<CommandResponse>(dataResponse, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            });
            return commandResponse;
        }
        catch (Exception ex)
        {
            return new CommandResponse
            {
                Error = ErrorTypes.ParseError,
                ErrorString = ex.Message
            };
        }
    }

    public static Task<(string DocBuilderKey, Dictionary<string, string> Urls)> DocbuilderRequestAsync(
        FileUtility fileUtility,
        string docbuilderUrl,
        string requestKey,
        string scriptUrl,
        bool isAsync,
        string signatureSecret,
       IHttpClientFactory clientFactory)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(docbuilderUrl);

        if (string.IsNullOrEmpty(requestKey) && string.IsNullOrEmpty(scriptUrl))
        {
            throw new ArgumentException("requestKey or inputScript is empty");
        }

        return InternalDocbuilderRequestAsync(fileUtility, docbuilderUrl, requestKey, scriptUrl, isAsync, signatureSecret, clientFactory);
    }

    private static async Task<(string DocBuilderKey, Dictionary<string, string> Urls)> InternalDocbuilderRequestAsync(
       FileUtility fileUtility,
       string docbuilderUrl,
       string requestKey,
       string scriptUrl,
       bool isAsync,
       string signatureSecret,
       IHttpClientFactory clientFactory)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(docbuilderUrl),
            Method = HttpMethod.Post
        };

        var httpClient = clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout);

        var body = new BuilderBody
        {
            Async = isAsync,
            Key = requestKey,
            Url = scriptUrl
        };

        if (!string.IsNullOrEmpty(signatureSecret))
        {
            var payload = new Dictionary<string, object>
                    {
                        { "payload", body }
                    };

            var token = JsonWebToken.Encode(payload, signatureSecret);
            //todo: remove old scheme
            request.Headers.Add(fileUtility.SignatureHeader, "Bearer " + token);

            token = JsonWebToken.Encode(body, signatureSecret);
            body.Token = token;
        }

        var bodyString = JsonSerializer.Serialize(body, new System.Text.Json.JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

        string dataResponse = null;

        using (var response = await httpClient.SendAsync(request))
        await using (var responseStream = await response.Content.ReadAsStreamAsync())
        {
            if (responseStream != null)
            {
                using var reader = new StreamReader(responseStream);
                dataResponse = await reader.ReadToEndAsync();
            }
        }

        if (string.IsNullOrEmpty(dataResponse))
        {
            throw new Exception("Invalid response");
        }

        var responseFromService = JObject.Parse(dataResponse);
        if (responseFromService == null)
        {
            throw new Exception("Invalid answer format");
        }

        var errorElement = responseFromService.Value<string>("error");
        if (!string.IsNullOrEmpty(errorElement))
        {
            DocumentServiceException.ProcessResponseError(errorElement);
        }

        var isEnd = responseFromService.Value<bool>("end");

        Dictionary<string, string> urls = null;
        if (isEnd)
        {
            IDictionary<string, JToken> rates = (JObject)responseFromService["urls"];

            urls = rates.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
        }

        return (responseFromService.Value<string>("key"), urls);
    }

    public static Task<bool> HealthcheckRequestAsync(string healthcheckUrl, IHttpClientFactory clientFactory)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(healthcheckUrl);

        return InternalHealthcheckRequestAsync(healthcheckUrl, clientFactory);
    }

    private static async Task<bool> InternalHealthcheckRequestAsync(string healthcheckUrl, IHttpClientFactory clientFactory)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(healthcheckUrl)
        };

        var httpClient = clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout);

        using var response = await httpClient.SendAsync(request);
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        if (responseStream == null)
        {
            throw new Exception("Empty response");
        }
        using var reader = new StreamReader(responseStream);
        var dataResponse = await reader.ReadToEndAsync();
        return dataResponse.Equals("true", StringComparison.InvariantCultureIgnoreCase);
    }

    public enum CommandMethod
    {
        Info,
        Drop,
        Saved, //not used
        Version,
        ForceSave, //not used
        Meta,
        License
    }

    [Serializable]
    [DebuggerDisplay("{Key}")]
    public class CommandResponse
    {
        [JsonPropertyName("error")]
        public ErrorTypes Error { get; set; }

        [JsonPropertyName("errorString")]
        public string ErrorString { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("license")]
        public License License { get; set; }

        [JsonPropertyName("server")]
        public ServerInfo Server { get; set; }

        [JsonPropertyName("quota")]
        public QuotaInfo Quota { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        public enum ErrorTypes
        {
            NoError = 0,
            DocumentIdError = 1,
            ParseError = 2,
            UnknownError = 3,
            NotModify = 4,
            UnknownCommand = 5,
            Token = 6,
            TokenExpire = 7,
        }

        [Serializable]
        [DebuggerDisplay("{BuildVersion}")]
        public class ServerInfo
        {
            [JsonPropertyName("buildDate")]
            public DateTime BuildDate { get; set; }

            [JsonPropertyName("buildNumber")]
            public int BuildNumber { get; set; }

            [JsonPropertyName("buildVersion")]
            public string BuildVersion { get; set; }

            [JsonPropertyName("packageType")]
            public PackageTypes PackageType { get; set; }

            [JsonPropertyName("resultType")]
            public ResultTypes ResultType { get; set; }

            [JsonPropertyName("workersCount")]
            public int WorkersCount { get; set; }

            public enum PackageTypes
            {
                OpenSource = 0,
                IntegrationEdition = 1,
                DeveloperEdition = 2
            }

            public enum ResultTypes
            {
                Error = 1,
                Expired = 2,
                Success = 3,
                UnknownUser = 4,
                Connections = 5,
                ExpiredTrial = 6,
                SuccessLimit = 7,
                UsersCount = 8,
                ConnectionsOS = 9,
                UsersCountOS = 10,
                ExpiredLimited = 11
            }
        }

        [Serializable]
        [DataContract(Name = "Quota", Namespace = "")]
        public class QuotaInfo
        {
            [JsonPropertyName("users")]
            public List<User> Users { get; set; }

            [Serializable]
            [DebuggerDisplay("{UserId} ({Expire})")]
            public class User
            {
                [JsonPropertyName("userid")]
                public string UserId { get; set; }

                [JsonPropertyName("expire")]
                public DateTime Expire { get; set; }
            }
        }
    }

    [Serializable]
    [DebuggerDisplay("{Command} ({Key})")]
    private class CommandBody
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public CommandMethod Command { get; set; }

        [JsonProperty(PropertyName = "c", Required = Required.Always)]
        [JsonPropertyName("c")]
        public string C
        {
            get { return Command.ToString().ToLower(CultureInfo.InvariantCulture); }
        }

        [JsonProperty(PropertyName = "callback", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("callback")]
        public string Callback { get; set; }

        [JsonProperty(PropertyName = "key", Required = Required.AllowNull)]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "meta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; }

        [JsonProperty(PropertyName = "users", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("users")]
        public string[] Users { get; set; }

        [JsonProperty(PropertyName = "token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        //not used
        [JsonProperty(PropertyName = "userdata", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("userdata")]
        public string UserData { get; set; }
    }

    [Serializable]
    [DebuggerDisplay("{Title}")]
    public class MetaData
    {
        [JsonProperty(PropertyName = "title")]
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

    [Serializable]
    [DebuggerDisplay("{Height}x{Width}")]
    public class ThumbnailData
    {
        [JsonProperty(PropertyName = "aspect")]
        [JsonPropertyName("aspect")]
        public int Aspect { get; set; }

        [JsonProperty(PropertyName = "first")]
        [JsonPropertyName("first")]
        public bool First { get; set; }

        [JsonProperty(PropertyName = "height")]
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "width")]
        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    [Serializable]
    [DataContract(Name = "spreadsheetLayout", Namespace = "")]
    [DebuggerDisplay("SpreadsheetLayout {IgnorePrintArea} {Orientation} {FitToHeight} {FitToWidth} {Headings} {GridLines}")]
    public class SpreadsheetLayout
    {
        [JsonProperty(PropertyName = "ignorePrintArea")]
        [JsonPropertyName("ignorePrintArea")]
        public bool IgnorePrintArea { get; set; }

        [JsonProperty(PropertyName = "orientation")]
        [JsonPropertyName("orientation")]
        public string Orientation { get; set; }

        [JsonProperty(PropertyName = "fitToHeight")]
        [JsonPropertyName("fitToHeight")]
        public int FitToHeight { get; set; }

        [JsonProperty(PropertyName = "fitToWidth")]
        [JsonPropertyName("fitToWidth")]
        public int FitToWidth { get; set; }

        [JsonProperty(PropertyName = "headings")]
        [JsonPropertyName("headings")]
        public bool Headings { get; set; }

        [JsonProperty(PropertyName = "gridLines")]
        [JsonPropertyName("gridLines")]
        public bool GridLines { get; set; }

        [JsonProperty(PropertyName = "margins")]
        [JsonPropertyName("margins")]
        public LayoutMargins Margins { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        [JsonPropertyName("pageSize")]
        public LayoutPageSize PageSize { get; set; }


        [Serializable]
        [DebuggerDisplay("Margins {Top} {Right} {Bottom} {Left}")]
        public class LayoutMargins
        {
            [JsonProperty(PropertyName = "left")]
            [JsonPropertyName("left")]
            public string Left { get; set; }

            [JsonProperty(PropertyName = "right")]
            [JsonPropertyName("right")]
            public string Right { get; set; }

            [JsonProperty(PropertyName = "top")]
            [JsonPropertyName("top")]
            public string Top { get; set; }

            [JsonProperty(PropertyName = "bottom")]
            [JsonPropertyName("bottom")]
            public string Bottom { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("PageSize {Width} {Height}")]
        public class LayoutPageSize
        {
            [JsonProperty(PropertyName = "height")]
            [JsonPropertyName("height")]
            public string Height { get; set; }

            [JsonProperty(PropertyName = "width")]
            [JsonPropertyName("width")]
            public string Width { get; set; }
        }
    }

    [Serializable]
    [DebuggerDisplay("{Title} from {FileType} to {OutputType} ({Key})")]
    private class ConvertionBody
    {
        [JsonProperty(PropertyName = "async")]
        [JsonPropertyName("async")]
        public bool Async { get; set; }

        [JsonProperty(PropertyName = "filetype", Required = Required.Always)]
        [JsonPropertyName("filetype")]
        public string FileType { get; set; }

        [JsonProperty(PropertyName = "key", Required = Required.Always)]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "outputtype", Required = Required.Always)]
        [JsonPropertyName("outputtype")]
        public string OutputType { get; set; }

        [JsonProperty(PropertyName = "password", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "title")]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "thumbnail", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("thumbnail")]
        public ThumbnailData Thumbnail { get; set; }

        [JsonProperty(PropertyName = "spreadsheetLayout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("spreadsheetLayout")]
        public SpreadsheetLayout SpreadsheetLayout { get; set; }

        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "region", Required = Required.Always)]
        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonProperty(PropertyName = "token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }

    [Serializable]
    [DebuggerDisplay("{Key}")]
    private class BuilderBody
    {
        [JsonProperty(PropertyName = "async")]
        [JsonPropertyName("async")]
        public bool Async { get; set; }

        [JsonProperty(PropertyName = "key", Required = Required.Always)]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }

    [Serializable]
    public class FileLink
    {
        [JsonProperty(PropertyName = "filetype")]
        [JsonPropertyName("filetype")]
        public string FileType { get; set; }

        [JsonProperty(PropertyName = "token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "url")]
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    [Serializable]
    public class DocumentServiceException : Exception
    {
        public ErrorCode Code { get; set; }

        public DocumentServiceException(ErrorCode errorCode, string message)
            : base(message)
        {
            Code = errorCode;
        }

        protected DocumentServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                Code = (ErrorCode)info.GetValue("Code", typeof(ErrorCode));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("Code", Code);
            }
        }

        public static void ProcessResponseError(string errorCode)
        {
            if (!ErrorCodeExtensions.TryParse(errorCode, true, out var code))
            {
                code = ErrorCode.Unknown;
            }
            var errorMessage = code switch
            {
                ErrorCode.VkeyUserCountExceed => "user count exceed",
                ErrorCode.VkeyKeyExpire => "signature expire",
                ErrorCode.VkeyEncrypt => "encrypt signature",
                ErrorCode.UploadCountFiles => "count files",
                ErrorCode.UploadExtension => "extension",
                ErrorCode.UploadContentLength => "upload length",
                ErrorCode.Vkey => "document signature",
                ErrorCode.TaskQueue => "database",
                ErrorCode.ConvertPassword => "password",
                ErrorCode.ConvertDownload => "download",
                ErrorCode.Convert => "convertation",
                ErrorCode.ConvertTimeout => "convertation timeout",
                ErrorCode.Unknown => "unknown error",
                _ => "errorCode = " + errorCode,
            };
            throw new DocumentServiceException(code, errorMessage);
        }

        [EnumExtensions]
        public enum ErrorCode
        {
            VkeyUserCountExceed = -22,
            VkeyKeyExpire = -21,
            VkeyEncrypt = -20,
            UploadCountFiles = -11,
            UploadExtension = -10,
            UploadContentLength = -9,
            Vkey = -8,
            TaskQueue = -6,
            ConvertPassword = -5,
            ConvertDownload = -4,
            Convert = -3,
            ConvertTimeout = -2,
            Unknown = -1
        }
    }

    /// <summary>
    /// Processing document received from the editing service
    /// </summary>
    /// <param name="jsonDocumentResponse">The resulting json from editing service</param>
    /// <returns>The percentage of completion of conversion and Uri to the converted document</returns>
    private static (int ResultPercent, string responseuri, string convertedFileType) GetResponseUri(string jsonDocumentResponse)
    {
        if (string.IsNullOrEmpty(jsonDocumentResponse))
        {
            throw new ArgumentException("Invalid param", nameof(jsonDocumentResponse));
        }

        var responseFromService = JObject.Parse(jsonDocumentResponse);
        if (responseFromService == null)
        {
            throw new WebException("Invalid answer format");
        }

        var errorElement = responseFromService.Value<string>("error");
        if (!string.IsNullOrEmpty(errorElement))
        {
            DocumentServiceException.ProcessResponseError(errorElement);
        }

        var isEndConvert = responseFromService.Value<bool>("endConvert");

        int resultPercent;
        var responseUri = string.Empty;
        var responseType = string.Empty;
        if (isEndConvert)
        {
            responseUri = responseFromService.Value<string>("fileUrl");
            responseType = responseFromService.Value<string>("fileType");
            resultPercent = 100;
        }
        else
        {
            resultPercent = responseFromService.Value<int>("percent");
            if (resultPercent >= 100)
            {
                resultPercent = 99;
            }
        }

        return (resultPercent, responseUri, responseType);
    }
}
