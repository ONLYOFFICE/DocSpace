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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using ASC.Common.Web;
using ASC.Core;

using Newtonsoft.Json.Linq;

namespace ASC.Web.Core.Files
{
    /// <summary>
    /// Class service connector
    /// </summary>
    public static class DocumentService
    {
        /// <summary>
        /// Timeout to request conversion
        /// </summary>
        public static int Timeout = 120000;
        //public static int Timeout = Convert.ToInt32(ConfigurationManagerExtension.AppSettings["files.docservice.timeout"] ?? "120000");

        /// <summary>
        /// Number of tries request conversion
        /// </summary>
        public static int MaxTry = 3;

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
            if (expectedKey.Length > maxLength) expectedKey = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(expectedKey)));
            var key = Regex.Replace(expectedKey, "[^0-9a-zA-Z_]", "_");
            return key.Substring(key.Length - Math.Min(key.Length, maxLength));
        }

        /// <summary>
        /// The method is to convert the file to the required format
        /// </summary>
        /// <param name="documentConverterUrl">Url to the service of conversion</param>
        /// <param name="documentUri">Uri for the document to convert</param>
        /// <param name="fromExtension">Document extension</param>
        /// <param name="toExtension">Extension to which to convert</param>
        /// <param name="documentRevisionId">Key for caching on service</param>
        /// <param name="password">Password</param>
        /// <param name="thumbnail">Thumbnail settings</param>
        /// <param name="isAsync">Perform conversions asynchronously</param>
        /// <param name="signatureSecret">Secret key to generate the token</param>
        /// <param name="convertedDocumentUri">Uri to the converted document</param>
        /// <returns>The percentage of completion of conversion</returns>
        /// <example>
        /// string convertedDocumentUri;
        /// GetConvertedUri("http://helpcenter.teamlab.com/content/GettingStarted.pdf", ".pdf", ".docx", "469971047", false, out convertedDocumentUri);
        /// </example>
        /// <exception>
        /// </exception>
        public static int GetConvertedUri(
            FileUtility fileUtility,
            string documentConverterUrl,
            string documentUri,
            string fromExtension,
            string toExtension,
            string documentRevisionId,
            string password,
            ThumbnailData thumbnail,
            SpreadsheetLayout spreadsheetLayout,
            bool isAsync,
            string signatureSecret,
            out string convertedDocumentUri)
        {
            fromExtension = string.IsNullOrEmpty(fromExtension) ? Path.GetExtension(documentUri) : fromExtension;
            if (string.IsNullOrEmpty(fromExtension)) throw new ArgumentNullException("fromExtension", "Document's extension for conversion is not known");
            if (string.IsNullOrEmpty(toExtension)) throw new ArgumentNullException("toExtension", "Extension for conversion is not known");

            var title = Path.GetFileName(documentUri ?? "");
            title = string.IsNullOrEmpty(title) || title.Contains("?") ? Guid.NewGuid().ToString() : title;

            documentRevisionId = string.IsNullOrEmpty(documentRevisionId)
                                     ? documentUri
                                     : documentRevisionId;
            documentRevisionId = GenerateRevisionId(documentRevisionId);

            var request = (HttpWebRequest)WebRequest.Create(documentConverterUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Timeout = Timeout;

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

            var bodyString = System.Text.Json.JsonSerializer.Serialize(body, new System.Text.Json.JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var bytes = Encoding.UTF8.GetBytes(bodyString ?? "");
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            string dataResponse;
            WebResponse response = null;
            Stream responseStream = null;
            try
            {
                var countTry = 0;
                while (countTry < MaxTry)
                {
                    try
                    {
                        countTry++;
                        response = request.GetResponse();
                        responseStream = response.GetResponseStream();
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status != WebExceptionStatus.Timeout)
                        {
                            throw new HttpException((int)HttpStatusCode.BadRequest, ex.Message, ex);
                        }
                    }
                }
                if (countTry == MaxTry)
                {
                    throw new WebException("Timeout", WebExceptionStatus.Timeout);
                }

                if (responseStream == null) throw new WebException("Could not get an answer");
                using var reader = new StreamReader(responseStream);
                dataResponse = reader.ReadToEnd();
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Dispose();
                if (response != null)
                    response.Dispose();
            }

            return GetResponseUri(dataResponse, out convertedDocumentUri);
        }

        /// <summary>
        /// Request to Document Server with command
        /// </summary>
        /// <param name="documentTrackerUrl">Url to the command service</param>
        /// <param name="method">Name of method</param>
        /// <param name="documentRevisionId">Key for caching on service, whose used in editor</param>
        /// <param name="callbackUrl">Url to the callback handler</param>
        /// <param name="users">users id for drop</param>
        /// <param name="meta">file meta data for update</param>
        /// <param name="signatureSecret">Secret key to generate the token</param>
        /// <param name="version">server version</param>
        /// <returns>Response</returns>
        public static CommandResultTypes CommandRequest(FileUtility fileUtility,
            string documentTrackerUrl,
            CommandMethod method,
            string documentRevisionId,
            string callbackUrl,
            string[] users,
            MetaData meta,
            string signatureSecret,
            out string version)
        {
            var request = (HttpWebRequest)WebRequest.Create(documentTrackerUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = Timeout;

            var body = new CommandBody
            {
                Command = method,
                Key = documentRevisionId,
            };

            if (!string.IsNullOrEmpty(callbackUrl)) body.Callback = callbackUrl;
            if (users != null && users.Length > 0) body.Users = users;
            if (meta != null) body.Meta = meta;

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

            var bodyString = System.Text.Json.JsonSerializer.Serialize(body, new System.Text.Json.JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var bytes = Encoding.UTF8.GetBytes(bodyString ?? "");
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            string dataResponse;
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null) throw new Exception("Response is null");

                using var reader = new StreamReader(stream);
                dataResponse = reader.ReadToEnd();
            }

            var jResponse = JObject.Parse(dataResponse);

            try
            {
                version = jResponse.Value<string>("version");
            }
            catch (Exception)
            {
                version = "0";
            }

            return (CommandResultTypes)jResponse.Value<int>("error");
        }

        public static string DocbuilderRequest(
            FileUtility fileUtility,
            string docbuilderUrl,
            string requestKey,
            string scriptUrl,
            bool isAsync,
            string signatureSecret,
            out Dictionary<string, string> urls)
        {
            if (string.IsNullOrEmpty(docbuilderUrl))
                throw new ArgumentNullException("docbuilderUrl");

            if (string.IsNullOrEmpty(requestKey) && string.IsNullOrEmpty(scriptUrl))
                throw new ArgumentException("requestKey or inputScript is empty");

            var request = (HttpWebRequest)WebRequest.Create(docbuilderUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = Timeout;

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

            var bodyString = System.Text.Json.JsonSerializer.Serialize(body, new System.Text.Json.JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var bytes = Encoding.UTF8.GetBytes(bodyString ?? "");
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            string dataResponse = null;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using var reader = new StreamReader(responseStream);
                    dataResponse = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(dataResponse)) throw new Exception("Invalid response");

            var responseFromService = JObject.Parse(dataResponse);
            if (responseFromService == null) throw new Exception("Invalid answer format");

            var errorElement = responseFromService.Value<string>("error");
            if (!string.IsNullOrEmpty(errorElement))
            {
                DocumentServiceException.ProcessResponseError(errorElement);
            }

            var isEnd = responseFromService.Value<bool>("end");

            urls = null;
            if (isEnd)
            {
                IDictionary<string, JToken> rates = (JObject)responseFromService["urls"];

                urls = rates.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
            }

            return responseFromService.Value<string>("key");
        }

        public static bool HealthcheckRequest(string healthcheckUrl)
        {
            if (string.IsNullOrEmpty(healthcheckUrl))
                throw new ArgumentNullException("healthcheckUrl");

            var request = (HttpWebRequest)WebRequest.Create(healthcheckUrl);
            request.Timeout = Timeout;

            using var response = (HttpWebResponse)request.GetResponse();
            using var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new Exception("Empty response");
            }
            using var reader = new StreamReader(responseStream);
            var dataResponse = reader.ReadToEnd();
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
        }

        public enum CommandResultTypes
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
        [DebuggerDisplay("{Command} ({Key})")]
        private class CommandBody
        {
            [System.Text.Json.Serialization.JsonIgnore]
            public CommandMethod Command { get; set; }

            [JsonPropertyName("c")]
            public string C
            {
                get { return Command.ToString().ToLower(CultureInfo.InvariantCulture); }
            }

            [JsonPropertyName("callback")]
            public string Callback { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("meta")]
            public MetaData Meta { get; set; }

            [JsonPropertyName("users")]
            public string[] Users { get; set; }

            [JsonPropertyName("token")]
            public string Token { get; set; }

            //not used
            [JsonPropertyName("userdata")]
            public string UserData { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("{Title}")]
        public class MetaData
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("{Height}x{Width}")]
        public class ThumbnailData
        {
            [JsonPropertyName("aspect")]
            public int Aspect { get; set; }

            [JsonPropertyName("first")]
            public bool First { get; set; }

            [JsonPropertyName("height")]
            public int Height { get; set; }

            [JsonPropertyName("width")]
            public int Width { get; set; }
        }

        [Serializable]
        [DataContract(Name = "spreadsheetLayout", Namespace = "")]
        [DebuggerDisplay("SpreadsheetLayout {IgnorePrintArea} {Orientation} {FitToHeight} {FitToWidth} {Headings} {GridLines}")]
        public class SpreadsheetLayout
        {
            [JsonPropertyName("ignorePrintArea")]
            public bool IgnorePrintArea { get; set; }

            [JsonPropertyName("orientation")]
            public string Orientation { get; set; }

            [JsonPropertyName("fitToHeight")]
            public int FitToHeight { get; set; }

            [JsonPropertyName("fitToWidth")]
            public int FitToWidth { get; set; }

            [JsonPropertyName("headings")]
            public bool Headings { get; set; }

            [JsonPropertyName("gridLines")]
            public bool GridLines { get; set; }

            [JsonPropertyName("margins")]
            public Margins Margins { get; set; }

            [JsonPropertyName("pageSize")]
            public PageSize PageSize { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("Margins {Top} {Right} {Bottom} {Left}")]
        public class Margins
        {
            [JsonPropertyName("left")]
            public string Left { get; set; }

            [JsonPropertyName("right")]
            public string Right { get; set; }

            [JsonPropertyName("top")]
            public string Top { get; set; }

            [JsonPropertyName("bottom")]
            public string Bottom { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("PageSize {Width} {Height}")]
        public class PageSize
        {
            [JsonPropertyName("height")]
            public string Height { get; set; }

            [JsonPropertyName("width")]
            public string Width { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("{Title} from {FileType} to {OutputType} ({Key})")]
        private class ConvertionBody
        {
            [JsonPropertyName("async")]
            public bool Async { get; set; }

            [JsonPropertyName("filetype")]
            public string FileType { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("outputtype")]
            public string OutputType { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("thumbnail")]
            public ThumbnailData Thumbnail { get; set; }

            [JsonPropertyName("spreadsheetLayout")]
            public SpreadsheetLayout SpreadsheetLayout { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("token")]
            public string Token { get; set; }
        }

        [Serializable]
        [DebuggerDisplay("{Key}")]
        private class BuilderBody
        {
            [JsonPropertyName("async")]
            public bool Async { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("token")]
            public string Token { get; set; }
        }

        [Serializable]
        public class FileLink
        {
            [JsonPropertyName("filetype")]
            public string FileType { get; set; }

            [JsonPropertyName("token")]
            public string Token { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        [Serializable]
        public class DocumentServiceException : Exception
        {
            public ErrorCode Code;

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
                if (!Enum.TryParse(errorCode, true, out ErrorCode code))
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
        /// <param name="responseUri">Uri to the converted document</param>
        /// <returns>The percentage of completion of conversion</returns>
        private static int GetResponseUri(string jsonDocumentResponse, out string responseUri)
        {
            if (string.IsNullOrEmpty(jsonDocumentResponse)) throw new ArgumentException("Invalid param", "jsonDocumentResponse");

            var responseFromService = JObject.Parse(jsonDocumentResponse);
            if (responseFromService == null) throw new WebException("Invalid answer format");

            var errorElement = responseFromService.Value<string>("error");
            if (!string.IsNullOrEmpty(errorElement)) DocumentServiceException.ProcessResponseError(errorElement);

            var isEndConvert = responseFromService.Value<bool>("endConvert");

            int resultPercent;
            responseUri = string.Empty;
            if (isEndConvert)
            {
                responseUri = responseFromService.Value<string>("fileUrl");
                resultPercent = 100;
            }
            else
            {
                resultPercent = responseFromService.Value<int>("percent");
                if (resultPercent >= 100) resultPercent = 99;
            }

            return resultPercent;
        }
    }
}