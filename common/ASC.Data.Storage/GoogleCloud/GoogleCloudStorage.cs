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
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Data.Storage.Configuration;
using ASC.Security.Cryptography;

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using static Google.Cloud.Storage.V1.UrlSigner;

using MimeMapping = ASC.Common.Web.MimeMapping;


namespace ASC.Data.Storage.GoogleCloud
{
    [Scope]
    public class GoogleCloudStorage : BaseStorage
    {
        private string _subDir = string.Empty;
        private Dictionary<string, PredefinedObjectAcl> _domainsAcl;
        private PredefinedObjectAcl _moduleAcl;

        private string _bucket = "";
        private string _json = "";

        private Uri _bucketRoot;
        private Uri _bucketSSlRoot;

        private bool _lowerCasing = true;

        public GoogleCloudStorage(
            TempStream tempStream,
            TenantManager tenantManager,
            PathUtils pathUtils,
            EmailValidationKeyProvider emailValidationKeyProvider,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory) : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, options, clientFactory)
        {
        }

        public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
        {
            _tenant = tenant;

            if (moduleConfig != null)
            {
                _modulename = moduleConfig.Name;
                _dataList = new DataList(moduleConfig);

                _domainsExpires = moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).ToDictionary(x => x.Name, y => y.Expires);

                _domainsExpires.Add(string.Empty, moduleConfig.Expires);

                _domainsAcl = moduleConfig.Domain.ToDictionary(x => x.Name, y => GetGoogleCloudAcl(y.Acl));
                _moduleAcl = GetGoogleCloudAcl(moduleConfig.Acl);
            }
            else
            {
                _modulename = string.Empty;
                _dataList = null;

                _domainsExpires = new Dictionary<string, TimeSpan> { { string.Empty, TimeSpan.Zero } };
                _domainsAcl = new Dictionary<string, PredefinedObjectAcl>();
                _moduleAcl = PredefinedObjectAcl.PublicRead;
            }

            _bucket = props["bucket"];

            _bucketRoot = props.ContainsKey("cname") && Uri.IsWellFormedUriString(props["cname"], UriKind.Absolute)
                              ? new Uri(props["cname"], UriKind.Absolute)
                              : new Uri("https://storage.googleapis.com/" + _bucket + "/", UriKind.Absolute);

            _bucketSSlRoot = props.ContainsKey("cnamessl") &&
                             Uri.IsWellFormedUriString(props["cnamessl"], UriKind.Absolute)
                                 ? new Uri(props["cnamessl"], UriKind.Absolute)
                                 : new Uri("https://storage.googleapis.com/" + _bucket + "/", UriKind.Absolute);

            if (props.TryGetValue("lower", out var value))
            {
                bool.TryParse(value, out _lowerCasing);
            }

            _json = props["json"];

            props.TryGetValue("subdir", out _subDir);

            return this;
        }

        private StorageClient GetStorage()
        {
            var credential = GoogleCredential.FromJson(_json);

            return StorageClient.Create(credential);
        }

        private Task<StorageClient> GetStorageAsync()
        {
            var credential = GoogleCredential.FromJson(_json);

            return StorageClient.CreateAsync(credential);
        }

        private string MakePath(string domain, string path)
        {
            string result;

            path = path.TrimStart('\\', '/').TrimEnd('/').Replace('\\', '/');

            if (!string.IsNullOrEmpty(_subDir))
            {
                if (_subDir.Length == 1 && (_subDir[0] == '/' || _subDir[0] == '\\'))
                    result = path;
                else
                    result = $"{_subDir}/{path}"; // Ignory all, if _subDir is not null
            }
            else//Key combined from module+domain+filename
                result = $"{_tenant}/{_modulename}/{domain}/{path}";

            result = result.Replace("//", "/").TrimStart('/');
            if (_lowerCasing)
            {
                result = result.ToLowerInvariant();
            }

            return result;
        }


        public static long DateToUnixTimestamp(DateTime date)
        {
            var ts = date - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }

        public override Task<Uri> GetInternalUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
        {
            if (expire == TimeSpan.Zero || expire == TimeSpan.MinValue || expire == TimeSpan.MaxValue)
            {
                expire = GetExpire(domain);
            }
            if (expire == TimeSpan.Zero || expire == TimeSpan.MinValue || expire == TimeSpan.MaxValue)
            {
                return Task.FromResult(GetUriShared(domain, path));
            }

            return InternalGetInternalUriAsync(domain, path, expire);
        }

        private async Task<Uri> InternalGetInternalUriAsync(string domain, string path, TimeSpan expire)
        {
            using var storage = GetStorage();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_json ?? ""));
            var preSignedURL = await UrlSigner.FromServiceAccountData(stream).SignAsync(_bucket, MakePath(domain, path), expire, HttpMethod.Get);

            return MakeUri(preSignedURL);
        }

        public Uri GetUriShared(string domain, string path)
        {
            return new Uri(SecureHelper.IsSecure(HttpContextAccessor.HttpContext, Options) ? _bucketSSlRoot : _bucketRoot, MakePath(domain, path));
        }

        private Uri MakeUri(string preSignedURL)
        {
            var uri = new Uri(preSignedURL);
            var signedPart = uri.PathAndQuery.TrimStart('/');

            return new Uri(uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? _bucketSSlRoot : _bucketRoot, signedPart);

        }

        public override Task<System.IO.Stream> GetReadStreamAsync(string domain, string path)
        {
            return GetReadStreamAsync(domain, path, 0);
        }

        public override async Task<System.IO.Stream> GetReadStreamAsync(string domain, string path, int offset)
        {
            var tempStream = TempStream.Create();

            var storage = GetStorage();

            await storage.DownloadObjectAsync(_bucket, MakePath(domain, path), tempStream);

            if (offset > 0)
                tempStream.Seek(offset, SeekOrigin.Begin);

            tempStream.Position = 0;

            return tempStream;
        }

        public override Task<Uri> SaveAsync(string domain, string path, System.IO.Stream stream)
        {
            return SaveAsync(domain, path, stream, string.Empty, string.Empty);
        }

        public override Task<Uri> SaveAsync(string domain, string path, System.IO.Stream stream, Configuration.ACL acl)
        {
            return SaveAsync(domain, path, stream, null, null, acl);
        }

        protected override Task<Uri> SaveWithAutoAttachmentAsync(string domain, string path, System.IO.Stream stream, string attachmentFileName)
        {
            var contentDisposition = $"attachment; filename={HttpUtility.UrlPathEncode(attachmentFileName)};";
            if (attachmentFileName.Any(c => c >= 0 && c <= 127))
            {
                contentDisposition = $"attachment; filename*=utf-8''{HttpUtility.UrlPathEncode(attachmentFileName)};";
            }
            return SaveAsync(domain, path, stream, null, contentDisposition);
        }

        public override Task<Uri> SaveAsync(string domain, string path, System.IO.Stream stream, string contentType, string contentDisposition)
        {
            return SaveAsync(domain, path, stream, contentType, contentDisposition, ACL.Auto);
        }

        public override Task<Uri> SaveAsync(string domain, string path, System.IO.Stream stream, string contentEncoding, int cacheDays)
        {
            return SaveAsync(domain, path, stream, string.Empty, string.Empty, ACL.Auto, contentEncoding, cacheDays);
        }

        public async Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                          string contentDisposition, ACL acl, string contentEncoding = null, int cacheDays = 5)
        {

            var buffered = TempStream.GetBuffered(stream);

            if (QuotaController != null)
            {
                QuotaController.QuotaUsedCheck(buffered.Length);
            }

            var mime = string.IsNullOrEmpty(contentType)
                        ? MimeMapping.GetMimeMapping(Path.GetFileName(path))
                        : contentType;

            using var storage = GetStorage();

            var uploadObjectOptions = new UploadObjectOptions
            {
                PredefinedAcl = acl == ACL.Auto ? GetDomainACL(domain) : GetGoogleCloudAcl(acl)
            };

            buffered.Position = 0;

            var uploaded = await storage.UploadObjectAsync(_bucket, MakePath(domain, path), mime, buffered, uploadObjectOptions);

            uploaded.ContentEncoding = contentEncoding;
            uploaded.CacheControl = string.Format("public, maxage={0}", (int)TimeSpan.FromDays(cacheDays).TotalSeconds);

            if (uploaded.Metadata == null)
                uploaded.Metadata = new Dictionary<string, string>();

            uploaded.Metadata["Expires"] = DateTime.UtcNow.Add(TimeSpan.FromDays(cacheDays)).ToString("R");

            if (!string.IsNullOrEmpty(contentDisposition))
            {
                uploaded.ContentDisposition = contentDisposition;
            }
            else if (mime == "application/octet-stream")
            {
                uploaded.ContentDisposition = "attachment";
            }

            storage.UpdateObject(uploaded);

            //           InvalidateCloudFront(MakePath(domain, path));

            QuotaUsedAdd(domain, buffered.Length);

            return await GetUriAsync(domain, path);
        }

        private void InvalidateCloudFront(params string[] paths)
        {
            throw new NotImplementedException();
        }

        private PredefinedObjectAcl GetGoogleCloudAcl(ACL acl)
        {
            return PredefinedObjectAcl.PublicRead;
            //return acl switch
            //{
            //    ACL.Read => PredefinedObjectAcl.PublicRead,
            //    _ => PredefinedObjectAcl.PublicRead,
            //};
        }

        private PredefinedObjectAcl GetDomainACL(string domain)
        {
            if (GetExpire(domain) != TimeSpan.Zero)
            {
                return PredefinedObjectAcl.Private;
            }

            if (_domainsAcl.TryGetValue(domain, out var value))
            {
                return value;
            }
            return _moduleAcl;
        }

        public override async Task DeleteAsync(string domain, string path)
        {
            using var storage = GetStorage();

            var key = MakePath(domain, path);
            var size = await GetFileSizeAsync(domain, path);

            await storage.DeleteObjectAsync(_bucket, key);

            QuotaUsedDelete(domain, size);
        }

        public override async Task DeleteFilesAsync(string domain, string folderPath, string pattern, bool recursive)
        {
            using var storage = GetStorage();

            IAsyncEnumerable<Google.Apis.Storage.v1.Data.Object> objToDel;

            if (recursive)
                objToDel = storage
                           .ListObjectsAsync(_bucket, MakePath(domain, folderPath))
                           .Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Name)));
            else
                objToDel = AsyncEnumerable.Empty<Google.Apis.Storage.v1.Data.Object>();

            await foreach (var obj in objToDel)
            {
                await storage.DeleteObjectAsync(_bucket, obj.Name);
                QuotaUsedDelete(domain, Convert.ToInt64(obj.Size));
            }
        }       

        public override Task DeleteFilesAsync(string domain, List<string> paths)
        {
            if (paths.Count == 0) return Task.CompletedTask;

            return InternalDeleteFilesAsync(domain, paths);
        }

        private async Task InternalDeleteFilesAsync(string domain, List<string> paths)
        {
            var keysToDel = new List<string>();

            long quotaUsed = 0;

            foreach (var path in paths)
            {
                try
                {

                    var key = MakePath(domain, path);

                    if (QuotaController != null)
                    {
                        quotaUsed += await GetFileSizeAsync(domain, path);
                    }

                    keysToDel.Add(key);
                }
                catch (FileNotFoundException)
                {

                }
            }

            if (keysToDel.Count == 0) return;

            using var storage = GetStorage();

            foreach (var e in keysToDel)
            {
                await storage.DeleteObjectAsync(_bucket, e);
            }

            if (quotaUsed > 0)
            {
                QuotaUsedDelete(domain, quotaUsed);
            }
        }

        public override async Task DeleteFilesAsync(string domain, string folderPath, DateTime fromDate, DateTime toDate)
        {
            using var storage = GetStorage();

            var objToDel = GetObjectsAsync(domain, folderPath, true)
                              .Where(x => x.Updated >= fromDate && x.Updated <= toDate);

            await foreach (var obj in objToDel)
            {
                await storage.DeleteObjectAsync(_bucket, obj.Name);
                QuotaUsedDelete(domain, Convert.ToInt64(obj.Size));
            }
        }

        public override async Task MoveDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
        {
            using var storage = GetStorage();
            var srckey = MakePath(srcdomain, srcdir);
            var dstkey = MakePath(newdomain, newdir);

            var objects = storage.ListObjects(_bucket, srckey);

            foreach (var obj in objects)
            {
                await storage.CopyObjectAsync(_bucket, srckey, _bucket, dstkey, new CopyObjectOptions
                {
                    DestinationPredefinedAcl = GetDomainACL(newdomain)
                });

                await storage.DeleteObjectAsync(_bucket, srckey);

            }
        }

        public override async Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
        {
            using var storage = GetStorage();

            var srcKey = MakePath(srcdomain, srcpath);
            var dstKey = MakePath(newdomain, newpath);
            var size = await GetFileSizeAsync(srcdomain, srcpath);

            storage.CopyObject(_bucket, srcKey, _bucket, dstKey, new CopyObjectOptions
            {
                DestinationPredefinedAcl = GetDomainACL(newdomain)
            });

            await DeleteAsync(srcdomain, srcpath);

            QuotaUsedDelete(srcdomain, size);
            QuotaUsedAdd(newdomain, size, quotaCheckFileSize);

            return await GetUriAsync(newdomain, newpath);

        }

        public override Task<Uri> SaveTempAsync(string domain, out string assignedPath, System.IO.Stream stream)
        {
            assignedPath = Guid.NewGuid().ToString();

            return SaveAsync(domain, assignedPath, stream);
        }

        public override IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string domain, string path, bool recursive)
        {
            return GetObjectsAsync(domain, path, recursive)
                   .Select(x => x.Name.Substring(MakePath(domain, path + "/").Length));
        }

        private IEnumerable<Google.Apis.Storage.v1.Data.Object> GetObjects(string domain, string path, bool recursive)
        {
            using var storage = GetStorage();

            var items = storage.ListObjects(_bucket, MakePath(domain, path));

            if (recursive) return items;

            return items.Where(x => x.Name.IndexOf('/', MakePath(domain, path + "/").Length) == -1);
        }

        private IAsyncEnumerable<Google.Apis.Storage.v1.Data.Object> GetObjectsAsync(string domain, string path, bool recursive)
        {
            using var storage = GetStorage();

            var items = storage.ListObjectsAsync(_bucket, MakePath(domain, path));

            if (recursive) return items;

            return items.Where(x => x.Name.IndexOf('/', MakePath(domain, path + "/").Length) == -1);
        }

        public override IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive)
        {
            return GetObjectsAsync(domain, path, recursive).Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Name)))
                   .Select(x => x.Name.Substring(MakePath(domain, path + "/").Length).TrimStart('/'));
        }

        public override async Task<bool> IsFileAsync(string domain, string path)
        {
            var storage = GetStorage();

            var objects = await storage.ListObjectsAsync(_bucket, MakePath(domain, path)).ReadPageAsync(1);

            return objects.Any();
        }

        public override Task<bool> IsDirectoryAsync(string domain, string path)
        {
            return IsFileAsync(domain, path);
        }

        public override async Task DeleteDirectoryAsync(string domain, string path)
        {
            using var storage = GetStorage();

            var objToDel = storage
                          .ListObjectsAsync(_bucket, MakePath(domain, path));

            await foreach (var obj in objToDel)
            {
                await storage.DeleteObjectAsync(_bucket, obj.Name);
                QuotaUsedDelete(domain, Convert.ToInt64(obj.Size));
            }
        }

        public override async Task<long> GetFileSizeAsync(string domain, string path)
        {
            using var storage = GetStorage();

            var obj = await storage.GetObjectAsync(_bucket, MakePath(domain, path));

            if (obj.Size.HasValue)
                return Convert.ToInt64(obj.Size.Value);

            return 0;
        }

        public override async Task<long> GetDirectorySizeAsync(string domain, string path)
        {
            using var storage = GetStorage();

            var objToDel = storage
                          .ListObjectsAsync(_bucket, MakePath(domain, path));

            long result = 0;

            await foreach (var obj in objToDel)
            {
                if (obj.Size.HasValue)
                    result += Convert.ToInt64(obj.Size.Value);
            }

            return result;
        }

        public override async Task<long> ResetQuotaAsync(string domain)
        {
            using var storage = GetStorage();

            var objects = storage
                          .ListObjectsAsync(_bucket, MakePath(domain, string.Empty));

            if (QuotaController != null)
            {
                long size = 0;

                await foreach (var obj in objects)
                {
                    if (obj.Size.HasValue)
                        size += Convert.ToInt64(obj.Size.Value);
                }

                QuotaController.QuotaUsedSet(_modulename, domain, _dataList.GetData(domain), size);

                return size;
            }

            return 0;
        }


        public override async Task<long> GetUsedQuotaAsync(string domain)
        {
            using var storage = GetStorage();

            var objects = storage
                          .ListObjectsAsync(_bucket, MakePath(domain, string.Empty));

            long result = 0;

            await foreach (var obj in objects)
            {
                if (obj.Size.HasValue)
                    result += Convert.ToInt64(obj.Size.Value);
            }

            return result;
        }

        public override async Task<Uri> CopyAsync(string srcdomain, string srcpath, string newdomain, string newpath)
        {
            using var storage = GetStorage();

            var size = await GetFileSizeAsync(srcdomain, srcpath);

            var options = new CopyObjectOptions
            {
                DestinationPredefinedAcl = GetDomainACL(newdomain)
            };

            await storage.CopyObjectAsync(_bucket, MakePath(srcdomain, srcpath), _bucket, MakePath(newdomain, newpath), options);

            QuotaUsedAdd(newdomain, size);

            return await GetUriAsync(newdomain, newpath);
        }

        public override async Task CopyDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
        {
            var srckey = MakePath(srcdomain, srcdir);
            var dstkey = MakePath(newdomain, newdir);
            //List files from src

            using var storage = GetStorage();

            var objects = storage.ListObjectsAsync(_bucket, srckey);

            await foreach (var obj in objects)
            {
                await storage.CopyObjectAsync(_bucket, srckey, _bucket, dstkey, new CopyObjectOptions
                {
                    DestinationPredefinedAcl = GetDomainACL(newdomain)
                });


                QuotaUsedAdd(newdomain, Convert.ToInt64(obj.Size));
            }
        }

        public override async Task<string> SavePrivateAsync(string domain, string path, System.IO.Stream stream, DateTime expires)
        {
            using var storage = GetStorage();

            var buffered = TempStream.GetBuffered(stream);

            var uploadObjectOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.BucketOwnerFullControl
            };

            buffered.Position = 0;

            var uploaded = await storage.UploadObjectAsync(_bucket, MakePath(domain, path), "application/octet-stream", buffered, uploadObjectOptions);

            uploaded.CacheControl = string.Format("public, maxage={0}", (int)TimeSpan.FromDays(5).TotalSeconds);
            uploaded.ContentDisposition = "attachment";

            if (uploaded.Metadata == null)
                uploaded.Metadata = new Dictionary<string, string>();

            uploaded.Metadata["Expires"] = DateTime.UtcNow.Add(TimeSpan.FromDays(5)).ToString("R");
            uploaded.Metadata.Add("private-expire", expires.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));

            await storage.UpdateObjectAsync(uploaded);

            using var mStream = new MemoryStream(Encoding.UTF8.GetBytes(_json ?? ""));
            var preSignedURL = await FromServiceAccountData(mStream).SignAsync(RequestTemplate.FromBucket(_bucket).WithObjectName(MakePath(domain, path)), UrlSigner.Options.FromExpiration(expires));

            //TODO: CNAME!
            return preSignedURL;
        }

        public override async Task DeleteExpiredAsync(string domain, string path, TimeSpan oldThreshold)
        {
            using var storage = GetStorage();

            var objects = storage.ListObjectsAsync(_bucket, MakePath(domain, path));

            await foreach (var obj in objects)
            {
                var objInfo = await storage.GetObjectAsync(_bucket, MakePath(domain, path), null);

                var privateExpireKey = objInfo.Metadata["private-expire"];

                if (string.IsNullOrEmpty(privateExpireKey)) continue;


                if (!long.TryParse(privateExpireKey, out var fileTime)) continue;
                if (DateTime.UtcNow <= DateTime.FromFileTimeUtc(fileTime)) continue;

                await storage.DeleteObjectAsync(_bucket, MakePath(domain, path));

            }
        }

        #region chunking

        public override async Task<string> InitiateChunkedUploadAsync(string domain, string path)
        {
            using var storage = GetStorage();

            var tempUploader = storage.CreateObjectUploader(_bucket, MakePath(domain, path), null, new MemoryStream());

            var sessionUri = await tempUploader.InitiateSessionAsync();

            return sessionUri.ToString();
        }



        public override async Task<string> UploadChunkAsync(string domain,
                                          string path,
                                          string uploadUri,
                                          Stream stream,
                                          long defaultChunkSize,
                                          int chunkNumber,
                                          long chunkLength)
        {

            var bytesRangeStart = Convert.ToString((chunkNumber - 1) * defaultChunkSize);
            var bytesRangeEnd = Convert.ToString((chunkNumber - 1) * defaultChunkSize + chunkLength - 1);

            var totalBytes = "*";

            if (chunkLength != defaultChunkSize)
                totalBytes = Convert.ToString((chunkNumber - 1) * defaultChunkSize + chunkLength);

            var contentRangeHeader = $"bytes {bytesRangeStart}-{bytesRangeEnd}/{totalBytes}";

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(uploadUri);
            request.Method = HttpMethod.Put;
            request.Headers.Add("Content-Range", contentRangeHeader);
            request.Content = new StreamContent(stream);


            const int MAX_RETRIES = 100;
            int millisecondsTimeout;

            for (var i = 0; i < MAX_RETRIES; i++)
            {
                millisecondsTimeout = Math.Min(Convert.ToInt32(Math.Pow(2, i)) + RandomNumberGenerator.GetInt32(1000), 32 * 1000);

                try
                {
                    var httpClient = ClientFactory.CreateClient();
                    using var response = await httpClient.SendAsync(request);

                    break;
                }
                catch (HttpRequestException ex)
                {
                    var status = (int)ex.StatusCode;

                    if (status == 408 || status == 500 || status == 502 || status == 503 || status == 504)
                    {
                        Thread.Sleep(millisecondsTimeout);
                        continue;
                    }

                    if (status != 308)
                        throw;

                    break;
                }
                catch
                {
                    await AbortChunkedUploadAsync(domain, path, uploadUri);
                    throw;
                }
            }

            return string.Empty;
        }

        public override async Task<Uri> FinalizeChunkedUploadAsync(string domain, string path, string uploadUri, Dictionary<int, string> eTags)
        {
            if (QuotaController != null)
            {
                var size = await GetFileSizeAsync(domain, path);
                QuotaUsedAdd(domain, size);
            }

            return await GetUriAsync(domain, path);
        }

        public override Task AbortChunkedUploadAsync(string domain, string path, string uploadUri)
        {
            return Task.CompletedTask;
        }

        public override bool IsSupportChunking { get { return true; } }

        #endregion

        public override string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize, string contentType, string contentDisposition, string submitLabel)
        {
            throw new NotImplementedException();
        }
        public override Task<string> GetUploadedUrlAsync(string domain, string directoryPath)
        {
            throw new NotImplementedException();
        }

        public override string GetUploadUrl()
        {
            throw new NotImplementedException();
        }

        public override string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType, string contentDisposition)
        {
            throw new NotImplementedException();
        }
    }
}

