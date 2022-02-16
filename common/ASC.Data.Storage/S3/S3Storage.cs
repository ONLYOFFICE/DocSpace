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
using System.Threading.Tasks;
using System.Web;

using Amazon;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.Util;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Data.Storage.Configuration;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Data.Storage.S3
{
    [Scope]
    public class S3Storage : BaseStorage
    {
        private readonly List<string> _domains = new List<string>();
        private Dictionary<string, S3CannedACL> _domainsAcl;
        private S3CannedACL _moduleAcl;
        private string _accessKeyId = "";
        private string _bucket = "";
        private string _recycleDir = "";
        private Uri _bucketRoot;
        private Uri _bucketSSlRoot;
        private string _region = string.Empty;
        private string _serviceurl;
        private bool _forcepathstyle;
        private string _secretAccessKeyId = "";
        private ServerSideEncryptionMethod _sse = ServerSideEncryptionMethod.AES256;
        private bool _useHttp = true;

        private bool _lowerCasing = true;
        private bool _revalidateCloudFront;
        private string _distributionId = string.Empty;
        private string _subDir = string.Empty;

        public S3Storage(
            TempStream tempStream,
            TenantManager tenantManager,
            PathUtils pathUtils,
            EmailValidationKeyProvider emailValidationKeyProvider,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory)
            : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, options, clientFactory)
        {
        }

        private S3CannedACL GetDomainACL(string domain)
        {
            if (GetExpire(domain) != TimeSpan.Zero)
            {
                return S3CannedACL.Private;
            }

            if (_domainsAcl.TryGetValue(domain, out var value))
            {
                return value;
            }
            return _moduleAcl;
        }

        private S3CannedACL GetS3Acl(ACL acl)
        {
            return acl switch
            {
                ACL.Read => S3CannedACL.PublicRead,
                ACL.Private => S3CannedACL.Private,
                _ => S3CannedACL.PublicRead,
            };
        }

        public Uri GetUriInternal(string path)
        {
            return new Uri(SecureHelper.IsSecure(HttpContextAccessor?.HttpContext, Options) ? _bucketSSlRoot : _bucketRoot, path);
        }

        public Uri GetUriShared(string domain, string path)
        {
            return new Uri(SecureHelper.IsSecure(HttpContextAccessor?.HttpContext, Options) ? _bucketSSlRoot : _bucketRoot, MakePath(domain, path));
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

            var pUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = _bucket,
                Expires = DateTime.UtcNow.Add(expire),
                Key = MakePath(domain, path),
                Protocol = SecureHelper.IsSecure(HttpContextAccessor?.HttpContext, Options) ? Protocol.HTTPS : Protocol.HTTP,
                Verb = HttpVerb.GET
            };

            if (headers != null && headers.Any())
            {
                var headersOverrides = new ResponseHeaderOverrides();

                foreach (var h in headers)
                {
                    if (h.StartsWith("Content-Disposition")) headersOverrides.ContentDisposition = h.Substring("Content-Disposition".Length + 1);
                    else if (h.StartsWith("Cache-Control")) headersOverrides.CacheControl = h.Substring("Cache-Control".Length + 1);
                    else if (h.StartsWith("Content-Encoding")) headersOverrides.ContentEncoding = h.Substring("Content-Encoding".Length + 1);
                    else if (h.StartsWith("Content-Language")) headersOverrides.ContentLanguage = h.Substring("Content-Language".Length + 1);
                    else if (h.StartsWith("Content-Type")) headersOverrides.ContentType = h.Substring("Content-Type".Length + 1);
                    else if (h.StartsWith("Expires")) headersOverrides.Expires = h.Substring("Expires".Length + 1);
                    else throw new FormatException($"Invalid header: {h}");
                }
                pUrlRequest.ResponseHeaderOverrides = headersOverrides;
            }
            using var client = GetClient();
            return Task.FromResult(MakeUri(client.GetPreSignedURL(pUrlRequest)));
        }

        private Uri MakeUri(string preSignedURL)
        {
            var uri = new Uri(preSignedURL);
            var signedPart = uri.PathAndQuery.TrimStart('/');
            return new UnencodedUri(uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? _bucketSSlRoot : _bucketRoot, signedPart);
        }

        public override Task<Stream> GetReadStreamAsync(string domain, string path)
        {
            return GetReadStreamAsync(domain, path, 0);
        }

        public override async Task<Stream> GetReadStreamAsync(string domain, string path, int offset)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucket,
                Key = MakePath(domain, path)
            };

            if (0 < offset) request.ByteRange = new ByteRange(offset, int.MaxValue);

            try
            {
                using var client = GetClient();
                return new ResponseStreamWrapper(await client.GetObjectAsync(request));
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.ErrorCode == "NoSuchKey")
                {
                    throw new FileNotFoundException("File not found", path);
                }

                throw;
            }
        }

        protected override Task<Uri> SaveWithAutoAttachmentAsync(string domain, string path, Stream stream, string attachmentFileName)
        {
            var contentDisposition = $"attachment; filename={HttpUtility.UrlPathEncode(attachmentFileName)};";
            if (attachmentFileName.Any(c => c >= 0 && c <= 127))
            {
                contentDisposition = $"attachment; filename*=utf-8''{HttpUtility.UrlPathEncode(attachmentFileName)};";
            }
            return SaveAsync(domain, path, stream, null, contentDisposition);
        }

        public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                        string contentDisposition)
        {
            return SaveAsync(domain, path, stream, contentType, contentDisposition, ACL.Auto);
        }

        public async Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                                 string contentDisposition, ACL acl, string contentEncoding = null, int cacheDays = 5)
        {
            var buffered = TempStream.GetBuffered(stream);
            if (QuotaController != null)
            {
                QuotaController.QuotaUsedCheck(buffered.Length);
            }

            using var client = GetClient();
            using var uploader = new TransferUtility(client);
            var mime = string.IsNullOrEmpty(contentType)
? MimeMapping.GetMimeMapping(Path.GetFileName(path))
: contentType;

            var request = new TransferUtilityUploadRequest
            {
                BucketName = _bucket,
                Key = MakePath(domain, path),
                ContentType = mime,
                ServerSideEncryptionMethod = _sse,
                InputStream = buffered,
                AutoCloseStream = false,
                Headers =
                    {
                        CacheControl = string.Format("public, maxage={0}", (int)TimeSpan.FromDays(cacheDays).TotalSeconds),
                        ExpiresUtc = DateTime.UtcNow.Add(TimeSpan.FromDays(cacheDays))
                    }
            };

            if (!WorkContext.IsMono) //  System.Net.Sockets.SocketException: Connection reset by peer
            {
                switch (acl)
                {
                    case ACL.Auto:
                        request.CannedACL = GetDomainACL(domain);
                        break;
                    case ACL.Read:
                    case ACL.Private:
                        request.CannedACL = GetS3Acl(acl);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(contentDisposition))
            {
                request.Headers.ContentDisposition = contentDisposition;
            }
            else if (mime == "application/octet-stream")
            {
                request.Headers.ContentDisposition = "attachment";
            }

            if (!string.IsNullOrEmpty(contentEncoding))
            {
                request.Headers.ContentEncoding = contentEncoding;
            }

            await uploader.UploadAsync(request);

            await InvalidateCloudFrontAsync(MakePath(domain, path));

            QuotaUsedAdd(domain, buffered.Length);

            return await GetUriAsync(domain, path);
        }

        private Task InvalidateCloudFrontAsync(params string[] paths)
        {
            if (!_revalidateCloudFront || string.IsNullOrEmpty(_distributionId)) return Task.CompletedTask;

            return InternalInvalidateCloudFrontAsync(paths);
        }

        private async Task InternalInvalidateCloudFrontAsync(params string[] paths)
        {
            using var cfClient = GetCloudFrontClient();
            var invalidationRequest = new CreateInvalidationRequest
            {
                DistributionId = _distributionId,
                InvalidationBatch = new InvalidationBatch
                {
                    CallerReference = Guid.NewGuid().ToString(),

                    Paths = new Paths
                    {
                        Items = paths.ToList(),
                        Quantity = paths.Count()
                    }
                }
            };

            await cfClient.CreateInvalidationAsync(invalidationRequest);
        }


        public override Task<Uri> SaveAsync(string domain, string path, Stream stream)
        {
            return SaveAsync(domain, path, stream, string.Empty, string.Empty);
        }

        public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentEncoding, int cacheDays)
        {
            return SaveAsync(domain, path, stream, string.Empty, string.Empty, ACL.Auto, contentEncoding, cacheDays);
        }

        public override Task<Uri> SaveAsync(string domain, string path, Stream stream, ACL acl)
        {
            return SaveAsync(domain, path, stream, null, null, acl);
        }

        #region chunking

        public override async Task<string> InitiateChunkedUploadAsync(string domain, string path)
        {
            var request = new InitiateMultipartUploadRequest
            {
                BucketName = _bucket,
                Key = MakePath(domain, path),
                ServerSideEncryptionMethod = _sse
            };

            using var s3 = GetClient();
            var response = await s3.InitiateMultipartUploadAsync(request);
            return response.UploadId;
        }

        public override async Task<string> UploadChunkAsync(string domain, string path, string uploadId, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
        {
            var request = new UploadPartRequest
            {
                BucketName = _bucket,
                Key = MakePath(domain, path),
                UploadId = uploadId,
                PartNumber = chunkNumber,
                InputStream = stream
            };

            try
            {
                using var s3 = GetClient();
                var response = s3.UploadPartAsync(request).Result;
                return response.ETag;
            }
            catch (AmazonS3Exception error)
            {
                if (error.ErrorCode == "NoSuchUpload")
                {
                    await AbortChunkedUploadAsync(domain, path, uploadId);
                }

                throw;
            }
        }

        public override async Task<Uri> FinalizeChunkedUploadAsync(string domain, string path, string uploadId, Dictionary<int, string> eTags)
        {
            var request = new CompleteMultipartUploadRequest
            {
                BucketName = _bucket,
                Key = MakePath(domain, path),
                UploadId = uploadId,
                PartETags = eTags.Select(x => new PartETag(x.Key, x.Value)).ToList()
            };

            try
            {
                using (var s3 = GetClient())
                {
                    await s3.CompleteMultipartUploadAsync(request);
                    await InvalidateCloudFrontAsync(MakePath(domain, path));
                }

                if (QuotaController != null)
                {
                    var size = await GetFileSizeAsync(domain, path);
                    QuotaUsedAdd(domain, size);
                }

                return await GetUriAsync(domain, path);
            }
            catch (AmazonS3Exception error)
            {
                if (error.ErrorCode == "NoSuchUpload")
                {
                    await AbortChunkedUploadAsync(domain, path, uploadId);
                }

                throw;
            }
        }

        public override async Task AbortChunkedUploadAsync(string domain, string path, string uploadId)
        {
            var key = MakePath(domain, path);

            var request = new AbortMultipartUploadRequest
            {
                BucketName = _bucket,
                Key = key,
                UploadId = uploadId
            };

            using var s3 = GetClient();
            await s3.AbortMultipartUploadAsync(request);
        }

        public override bool IsSupportChunking { get { return true; } }

        #endregion

        public override async Task DeleteAsync(string domain, string path)
        {
            using var client = GetClient();
            var key = MakePath(domain, path);
            var size = await GetFileSizeAsync(domain, path);

            await RecycleAsync(client, domain, key);

            var request = new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = key
            };

            await client.DeleteObjectAsync(request);

            QuotaUsedDelete(domain, size);
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
                    //var obj = GetS3Objects(domain, path).FirstOrDefault();

                    var key = MakePath(domain, path);

                    if (QuotaController != null)
                    {
                        quotaUsed += await GetFileSizeAsync(domain, path);
                    }

                    keysToDel.Add(key);

                    //objsToDel.Add(obj);
                }
                catch (FileNotFoundException)
                {

                }
            }

            if (keysToDel.Count == 0)
                return;

            using (var client = GetClient())
            {
                var deleteRequest = new DeleteObjectsRequest
                {
                    BucketName = _bucket,
                    Objects = keysToDel.Select(key => new KeyVersion { Key = key }).ToList()
                };

                await client.DeleteObjectsAsync(deleteRequest);
            }

            if (quotaUsed > 0)
            {
                QuotaUsedDelete(domain, quotaUsed);
            }
        }

        public override async Task DeleteFilesAsync(string domain, string path, string pattern, bool recursive)
        {
            var makedPath = MakePath(domain, path) + '/';
            var obj = await GetS3ObjectsAsync(domain, path);
            var objToDel = obj.Where(x =>
                    Wildcard.IsMatch(pattern, Path.GetFileName(x.Key))
                    && (recursive || !x.Key.Remove(0, makedPath.Length).Contains('/'))
                    );

            using var client = GetClient();
            foreach (var s3Object in objToDel)
            {
                await RecycleAsync(client, domain, s3Object.Key);

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucket,
                    Key = s3Object.Key
                };

                await client.DeleteObjectAsync(deleteRequest);

                QuotaUsedDelete(domain, s3Object.Size);
            }
        }

        public override async Task DeleteFilesAsync(string domain, string path, DateTime fromDate, DateTime toDate)
        {
            var obj = await GetS3ObjectsAsync(domain, path);
            var objToDel = obj.Where(x => x.LastModified >= fromDate && x.LastModified <= toDate);

            using var client = GetClient();
            foreach (var s3Object in objToDel)
            {
                await RecycleAsync(client, domain, s3Object.Key);

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucket,
                    Key = s3Object.Key
                };

                await client.DeleteObjectAsync(deleteRequest);

                QuotaUsedDelete(domain, s3Object.Size);
            }
        }

        public override async Task MoveDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
        {
            var srckey = MakePath(srcdomain, srcdir);
            var dstkey = MakePath(newdomain, newdir);
            //List files from src
            using var client = GetClient();
            var request = new ListObjectsRequest
            {
                BucketName = _bucket,
                Prefix = srckey
            };

            var response = await client.ListObjectsAsync(request);
            foreach (var s3Object in response.S3Objects)
            {
                await client.CopyObjectAsync(new CopyObjectRequest
                {
                    SourceBucket = _bucket,
                    SourceKey = s3Object.Key,
                    DestinationBucket = _bucket,
                    DestinationKey = s3Object.Key.Replace(srckey, dstkey),
                    CannedACL = GetDomainACL(newdomain),
                    ServerSideEncryptionMethod = _sse
                });

                await client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _bucket,
                    Key = s3Object.Key
                });
            }
        }

        public override async Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
        {
            using var client = GetClient();
            var srcKey = MakePath(srcdomain, srcpath);
            var dstKey = MakePath(newdomain, newpath);
            var size = await GetFileSizeAsync(srcdomain, srcpath);

            var request = new CopyObjectRequest
            {
                SourceBucket = _bucket,
                SourceKey = srcKey,
                DestinationBucket = _bucket,
                DestinationKey = dstKey,
                CannedACL = GetDomainACL(newdomain),
                MetadataDirective = S3MetadataDirective.REPLACE,
                ServerSideEncryptionMethod = _sse
            };

            await client.CopyObjectAsync(request);
            await DeleteAsync(srcdomain, srcpath);

            QuotaUsedDelete(srcdomain, size);
            QuotaUsedAdd(newdomain, size, quotaCheckFileSize);

            return await GetUriAsync(newdomain, newpath);
        }
        public override Task<Uri> SaveTempAsync(string domain, out string assignedPath, Stream stream)
        {
            assignedPath = Guid.NewGuid().ToString();
            return SaveAsync(domain, assignedPath, stream);
        }

        public override async IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string domain, string path, bool recursive)
        {
            var tmp = await GetS3ObjectsAsync(domain, path);
            var obj = tmp.Select(x => x.Key.Substring((MakePath(domain, path) + "/").Length));
            foreach (var e in obj)
            {
                yield return e;
            }
        }

        public override async Task<string> SavePrivateAsync(string domain, string path, Stream stream, DateTime expires)
        {
            using var client = GetClient();
            using var uploader = new TransferUtility(client);
            var objectKey = MakePath(domain, path);
            var buffered = TempStream.GetBuffered(stream);
            var request = new TransferUtilityUploadRequest
            {
                BucketName = _bucket,
                Key = objectKey,
                CannedACL = S3CannedACL.BucketOwnerFullControl,
                ContentType = "application/octet-stream",
                InputStream = buffered,
                Headers =
                    {
                        CacheControl = string.Format("public, maxage={0}", (int)TimeSpan.FromDays(5).TotalSeconds),
                        ExpiresUtc = DateTime.UtcNow.Add(TimeSpan.FromDays(5)),
                        ContentDisposition = "attachment",
                    }
            };


            request.Metadata.Add("private-expire", expires.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));

            await uploader.UploadAsync(request);

            //Get presigned url                
            var pUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = _bucket,
                Expires = expires,
                Key = objectKey,
                Protocol = Protocol.HTTP,
                Verb = HttpVerb.GET
            };

            var url = client.GetPreSignedURL(pUrlRequest);
            //TODO: CNAME!
            return url;
        }

        public override async Task DeleteExpiredAsync(string domain, string path, TimeSpan oldThreshold)
        {
            using var client = GetClient();
            var s3Obj = await GetS3ObjectsAsync(domain, path);
            foreach (var s3Object in s3Obj)
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucket,
                    Key = s3Object.Key
                };

                var metadata = await  client.GetObjectMetadataAsync(request);
                var privateExpireKey = metadata.Metadata["private-expire"];
                if (string.IsNullOrEmpty(privateExpireKey)) continue;

                if (!long.TryParse(privateExpireKey, out var fileTime)) continue;
                if (DateTime.UtcNow <= DateTime.FromFileTimeUtc(fileTime)) continue;
                //Delete it
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucket,
                    Key = s3Object.Key
                };

                await client.DeleteObjectAsync(deleteObjectRequest);
            }
        }

        public override string GetUploadUrl()
        {
            return GetUriInternal(string.Empty).ToString();
        }

        public override string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType,
                                             string contentDisposition)
        {
            var key = MakePath(domain, directoryPath) + "/";
            //Generate policy
            var policyBase64 = GetPolicyBase64(key, string.Empty, contentType, contentDisposition, maxUploadSize,
                                                  out var sign);
            var postBuilder = new StringBuilder();
            postBuilder.Append('{');
            postBuilder.Append("\"key\":\"").Append(key).Append("${{filename}}\",");
            postBuilder.Append("\"acl\":\"public-read\",");
            postBuilder.Append($"\"key\":\"{key}\",");
            postBuilder.Append("\"success_action_status\":\"201\",");

            if (!string.IsNullOrEmpty(contentType))
                postBuilder.Append($"\"Content-Type\":\"{contentType}\",");
            if (!string.IsNullOrEmpty(contentDisposition))
                postBuilder.Append($"\"Content-Disposition\":\"{contentDisposition}\",");

            postBuilder.Append($"\"AWSAccessKeyId\":\"{_accessKeyId}\",");
            postBuilder.Append($"\"Policy\":\"{policyBase64}\",");
            postBuilder.Append($"\"Signature\":\"{sign}\"");
            postBuilder.Append("\"SignatureVersion\":\"2\"");
            postBuilder.Append("\"SignatureMethod\":\"HmacSHA1\"");
            postBuilder.Append('}');
            return postBuilder.ToString();
        }

        public override string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize,
                                             string contentType, string contentDisposition, string submitLabel)
        {
            var destBucket = GetUploadUrl();
            var key = MakePath(domain, directoryPath) + "/";
            //Generate policy
            var policyBase64 = GetPolicyBase64(key, redirectTo, contentType, contentDisposition, maxUploadSize,
                                                  out var sign);

            var formBuilder = new StringBuilder();
            formBuilder.Append($"<form action=\"{destBucket}\" method=\"post\" enctype=\"multipart/form-data\">");
            formBuilder.Append($"<input type=\"hidden\" name=\"key\" value=\"{key}${{filename}}\" />");
            formBuilder.Append("<input type=\"hidden\" name=\"acl\" value=\"public-read\" />");
            if (!string.IsNullOrEmpty(redirectTo))
                formBuilder.Append($"<input type=\"hidden\" name=\"success_action_redirect\" value=\"{redirectTo}\" />");

            formBuilder.AppendFormat("<input type=\"hidden\" name=\"success_action_status\" value=\"{0}\" />", 201);

            if (!string.IsNullOrEmpty(contentType))
                formBuilder.Append($"<input type=\"hidden\" name=\"Content-Type\" value=\"{contentType}\" />");
            if (!string.IsNullOrEmpty(contentDisposition))
                formBuilder.Append($"<input type=\"hidden\" name=\"Content-Disposition\" value=\"{contentDisposition}\" />");
            formBuilder.Append($"<input type=\"hidden\" name=\"AWSAccessKeyId\" value=\"{_accessKeyId}\"/>");
            formBuilder.Append($"<input type=\"hidden\" name=\"Policy\" value=\"{policyBase64}\" />");
            formBuilder.Append($"<input type=\"hidden\" name=\"Signature\" value=\"{sign}\" />");
            formBuilder.Append("<input type=\"hidden\" name=\"SignatureVersion\" value=\"2\" />");
            formBuilder.Append("<input type=\"hidden\" name=\"SignatureMethod\" value=\"HmacSHA1{0}\" />");
            formBuilder.Append("<input type=\"file\" name=\"file\" />");
            formBuilder.Append($"<input type=\"submit\" name=\"submit\" value=\"{submitLabel}\" /></form>");
            return formBuilder.ToString();
        }

        private string GetPolicyBase64(string key, string redirectTo, string contentType, string contentDisposition,
                                       long maxUploadSize, out string sign)
        {
            var policyBuilder = new StringBuilder();

            var minutes = DateTime.UtcNow.AddMinutes(15).ToString(AWSSDKUtils.ISO8601DateFormat,
                                                                               CultureInfo.InvariantCulture);

            policyBuilder.Append($"{{\"expiration\": \"{minutes}\",\"conditions\":[");
            policyBuilder.Append($"{{\"bucket\": \"{_bucket}\"}},");
            policyBuilder.Append($"[\"starts-with\", \"$key\", \"{key}\"],");
            policyBuilder.Append("{\"acl\": \"public-read\"},");
            if (!string.IsNullOrEmpty(redirectTo))
            {
                policyBuilder.Append($"{{\"success_action_redirect\": \"{redirectTo}\"}},");
            }
            policyBuilder.Append("{{\"success_action_status\": \"201\"}},");
            if (!string.IsNullOrEmpty(contentType))
            {
                policyBuilder.Append($"[\"eq\", \"$Content-Type\", \"{contentType}\"],");
            }
            if (!string.IsNullOrEmpty(contentDisposition))
            {
                policyBuilder.Append($"[\"eq\", \"$Content-Disposition\", \"{contentDisposition}\"],");
            }
            policyBuilder.Append($"[\"content-length-range\", 0, {maxUploadSize}]");
            policyBuilder.Append("]}");

            var policyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(policyBuilder.ToString()));
            //sign = AWSSDKUtils.HMACSign(policyBase64, _secretAccessKeyId, new HMACSHA1());
            using var algorithm = new HMACSHA1 { Key = Encoding.UTF8.GetBytes(_secretAccessKeyId) };
            try
            {
                algorithm.Key = Encoding.UTF8.GetBytes(key);
                sign = Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(policyBase64)));
            }
            finally
            {
                algorithm.Clear();
            }

            return policyBase64;
        }     

        public override async Task<string> GetUploadedUrlAsync(string domain, string directoryPath)
        {
            if (HttpContextAccessor?.HttpContext != null)
            {
                var buket = HttpContextAccessor?.HttpContext.Request.Query["bucket"].FirstOrDefault();
                var key = HttpContextAccessor?.HttpContext.Request.Query["key"].FirstOrDefault();
                var etag = HttpContextAccessor?.HttpContext.Request.Query["etag"].FirstOrDefault();
                var destkey = MakePath(domain, directoryPath) + "/";

                if (!string.IsNullOrEmpty(buket) && !string.IsNullOrEmpty(key) && string.Equals(buket, _bucket) &&
                    key.StartsWith(destkey))
                {
                    var domainpath = key.Substring(MakePath(domain, string.Empty).Length);
                    var skipQuota = false;
                    if (HttpContextAccessor?.HttpContext.Session != null)
                    {
                        HttpContextAccessor.HttpContext.Session.TryGetValue(etag, out var isCounted);
                        skipQuota = isCounted != null;
                    }
                    //Add to quota controller
                    if (QuotaController != null && !skipQuota)
                    {
                        try
                        {
                            var size = await GetFileSizeAsync(domain, domainpath);
                            QuotaUsedAdd(domain, size);

                            if (HttpContextAccessor?.HttpContext.Session != null)
                            {
                                //TODO:
                                //HttpContext.Current.Session.Add(etag, size); 
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    return GetUriInternal(key).ToString();
                }
            }
            return string.Empty;
        }

        public override async IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive)
        {
            var tmp = await GetS3ObjectsAsync(domain, path);
            var obj = tmp.Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Key)))
                .Select(x => x.Key.Substring((MakePath(domain, path) + "/").Length).TrimStart('/'));

            foreach (var e in obj)
                yield return e;
        }

        private bool CheckKey(string domain, string key)
        {
            return !string.IsNullOrEmpty(domain) ||
                   _domains.All(configuredDomains => !key.StartsWith(MakePath(configuredDomains, "")));
        }

        public override async Task<bool> IsFileAsync(string domain, string path)
        {
            using var client = GetClient();
            try
            {
                var getObjectMetadataRequest = new GetObjectMetadataRequest
                {
                    BucketName = _bucket,
                    Key = MakePath(domain, path)
                };

                await client.GetObjectMetadataAsync(getObjectMetadataRequest);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (string.Equals(ex.ErrorCode, "NoSuchBucket"))
                {
                    return false;
                }

                if (string.Equals(ex.ErrorCode, "NotFound"))
                {
                    return false;
                }

                throw;
            }
        }

        public override async Task<bool> IsDirectoryAsync(string domain, string path)
        {
            using (var client = GetClient())
            {
                var request = new ListObjectsRequest { BucketName = _bucket, Prefix = MakePath(domain, path) };
                var response = await client.ListObjectsAsync(request);
                return response.S3Objects.Count > 0;
            }
        }

        public override async Task DeleteDirectoryAsync(string domain, string path)
        {
            await DeleteFilesAsync(domain, path, "*", true);
        }

        public override async Task<long> GetFileSizeAsync(string domain, string path)
        {
            using var client = GetClient();
            var request = new ListObjectsRequest { BucketName = _bucket, Prefix = MakePath(domain, path) };
            var response = await client.ListObjectsAsync(request);
            if (response.S3Objects.Count > 0)
            {
                return response.S3Objects[0].Size;
            }
            throw new FileNotFoundException("file not found", path);
        }

        public override async Task<long> GetDirectorySizeAsync(string domain, string path)
        {
            if (!await IsDirectoryAsync(domain, path))
                throw new FileNotFoundException("directory not found", path);

            var tmp = await GetS3ObjectsAsync(domain, path);
            return tmp.Where(x => Wildcard.IsMatch("*.*", Path.GetFileName(x.Key)))
                .Sum(x => x.Size);
        }

        public override async Task<long> ResetQuotaAsync(string domain)
        {
            if (QuotaController != null)
            {
                var objects = await GetS3ObjectsAsync(domain);
                var size = objects.Sum(s3Object => s3Object.Size);
                QuotaController.QuotaUsedSet(_modulename, domain, _dataList.GetData(domain), size);
                return size;
            }
            return 0;
        }

        public override async Task<long> GetUsedQuotaAsync(string domain)
        {
            var objects = await GetS3ObjectsAsync(domain);
            return objects.Sum(s3Object => s3Object.Size);
        }

        public override async Task<Uri> CopyAsync(string srcdomain, string srcpath, string newdomain, string newpath)
        {
            using var client = GetClient();
            var srcKey = MakePath(srcdomain, srcpath);
            var dstKey = MakePath(newdomain, newpath);
            var size = await GetFileSizeAsync(srcdomain, srcpath);

            var request = new CopyObjectRequest
            {
                SourceBucket = _bucket,
                SourceKey = srcKey,
                DestinationBucket = _bucket,
                DestinationKey = dstKey,
                CannedACL = GetDomainACL(newdomain),
                MetadataDirective = S3MetadataDirective.REPLACE,
                ServerSideEncryptionMethod = _sse
            };

            await client.CopyObjectAsync(request);

            QuotaUsedAdd(newdomain, size);

            return await GetUriAsync(newdomain, newpath);
        }

        public override async Task CopyDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
        {
            var srckey = MakePath(srcdomain, srcdir);
            var dstkey = MakePath(newdomain, newdir);
            //List files from src
            using var client = GetClient();
            var request = new ListObjectsRequest { BucketName = _bucket, Prefix = srckey };

            var response = await client.ListObjectsAsync(request);
            foreach (var s3Object in response.S3Objects)
            {
                await client.CopyObjectAsync(new CopyObjectRequest
                {
                    SourceBucket = _bucket,
                    SourceKey = s3Object.Key,
                    DestinationBucket = _bucket,
                    DestinationKey = s3Object.Key.Replace(srckey, dstkey),
                    CannedACL = GetDomainACL(newdomain),
                    ServerSideEncryptionMethod = _sse
                });

                QuotaUsedAdd(newdomain, s3Object.Size);
            }
        }

        private async Task<IEnumerable<S3Object>> GetS3ObjectsByPathAsync(string domain, string path)
        {
            using var client = GetClient();
            var request = new ListObjectsRequest
            {
                BucketName = _bucket,
                Prefix = path,
                MaxKeys = 1000
            };

            var objects = new List<S3Object>();
            ListObjectsResponse response;
            do
            {
                response = await client.ListObjectsAsync(request);
                objects.AddRange(response.S3Objects.Where(entry => CheckKey(domain, entry.Key)));
                request.Marker = response.NextMarker;
            } while (response.IsTruncated);
            return objects;
        }

        private async Task<IEnumerable<S3Object>> GetS3ObjectsAsync(string domain, string path = "", bool recycle = false)
        {
            path = MakePath(domain, path) + '/';
            var s30Objects = await GetS3ObjectsByPathAsync(domain, path);
            if (string.IsNullOrEmpty(_recycleDir) || !recycle) return s30Objects;
            s30Objects.Concat(await GetS3ObjectsByPathAsync(domain, GetRecyclePath(path)));
            return s30Objects;
        }


        public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
        {
            _tenant = tenant;

            if (moduleConfig != null)
            {
                _modulename = moduleConfig.Name;
                _dataList = new DataList(moduleConfig);
                _domains.AddRange(moduleConfig.Domain.Select(x => $"{x.Name}/"));

                //Make expires
                _domainsExpires = moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).ToDictionary(x => x.Name, y => y.Expires);
                _domainsExpires.Add(string.Empty, moduleConfig.Expires);

                _domainsAcl = moduleConfig.Domain.ToDictionary(x => x.Name, y => GetS3Acl(y.Acl));
                _moduleAcl = GetS3Acl(moduleConfig.Acl);
            }
            else
            {
                _modulename = string.Empty;
                _dataList = null;

                //Make expires
                _domainsExpires = new Dictionary<string, TimeSpan> { { string.Empty, TimeSpan.Zero } };

                _domainsAcl = new Dictionary<string, S3CannedACL>();
                _moduleAcl = S3CannedACL.PublicRead;
            }

            _accessKeyId = props["acesskey"];
            _secretAccessKeyId = props["secretaccesskey"];
            _bucket = props["bucket"];

            props.TryGetValue("recycleDir", out _recycleDir);

            if (props.TryGetValue("region", out var region) && !string.IsNullOrEmpty(region))
            {
                _region = region;
            }

            if (props.TryGetValue("serviceurl", out var url) && !string.IsNullOrEmpty(url))
            {
                _serviceurl = url;
            }

            if (props.TryGetValue("forcepathstyle", out var style))
            {
                if (bool.TryParse(style, out var fps))
                {
                    _forcepathstyle = fps;
                }
            }

            if (props.TryGetValue("usehttp", out var use))
            {
                if (bool.TryParse(use, out var uh))
                {
                    _useHttp = uh;
                }
            }

            if (props.TryGetValue("sse", out var sse) && !string.IsNullOrEmpty(sse))
            {
                _sse = sse.ToLower() switch
                {
                    "none" => ServerSideEncryptionMethod.None,
                    "aes256" => ServerSideEncryptionMethod.AES256,
                    "awskms" => ServerSideEncryptionMethod.AWSKMS,
                    _ => ServerSideEncryptionMethod.None,
                };
            }

            _bucketRoot = props.ContainsKey("cname") && Uri.IsWellFormedUriString(props["cname"], UriKind.Absolute)
                              ? new Uri(props["cname"], UriKind.Absolute)
                              : new Uri($"http://s3.{_region}.amazonaws.com/{_bucket}/", UriKind.Absolute);
            _bucketSSlRoot = props.ContainsKey("cnamessl") &&
                             Uri.IsWellFormedUriString(props["cnamessl"], UriKind.Absolute)
                                 ? new Uri(props["cnamessl"], UriKind.Absolute)
                                 : new Uri($"https://s3.{_region}.amazonaws.com/{_bucket}/", UriKind.Absolute);

            if (props.TryGetValue("lower", out var lower))
            {
                bool.TryParse(lower, out _lowerCasing);
            }
            if (props.TryGetValue("cloudfront", out var front))
            {
                bool.TryParse(front, out _revalidateCloudFront);
            }

            props.TryGetValue("distribution", out _distributionId);
            props.TryGetValue("subdir", out _subDir);

            return this;
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

            result = result.Replace("//", "/").TrimStart('/').TrimEnd('/');
            if (_lowerCasing)
            {
                result = result.ToLowerInvariant();
            }

            return result;
        }

        private string GetRecyclePath(string path)
        {
            return string.IsNullOrEmpty(_recycleDir) ? "" : $"{_recycleDir}/{path.TrimStart('/')}";
        }

        private Task RecycleAsync(IAmazonS3 client, string domain, string key)
        {
            if (string.IsNullOrEmpty(_recycleDir)) return Task.CompletedTask;

            return InternalRecycleAsync(client, domain, key);
        }

        private async Task InternalRecycleAsync(IAmazonS3 client, string domain, string key)
        {
            var copyObjectRequest = new CopyObjectRequest
            {
                SourceBucket = _bucket,
                SourceKey = key,
                DestinationBucket = _bucket,
                DestinationKey = GetRecyclePath(key),
                CannedACL = GetDomainACL(domain),
                MetadataDirective = S3MetadataDirective.REPLACE,
                ServerSideEncryptionMethod = _sse,
                StorageClass = S3StorageClass.Glacier
            };

            await client.CopyObjectAsync(copyObjectRequest);
        }

        private IAmazonCloudFront GetCloudFrontClient()
        {
            var cfg = new AmazonCloudFrontConfig { MaxErrorRetry = 3 };
            return new AmazonCloudFrontClient(_accessKeyId, _secretAccessKeyId, cfg);
        }

        private IAmazonS3 GetClient()
        {
            var cfg = new AmazonS3Config { MaxErrorRetry = 3 };

            if (!string.IsNullOrEmpty(_serviceurl))
            {
                cfg.ServiceURL = _serviceurl;

                cfg.ForcePathStyle = _forcepathstyle;
            }
            else
            {
                cfg.RegionEndpoint = RegionEndpoint.GetBySystemName(_region);
            }

            cfg.UseHttp = _useHttp;

            return new AmazonS3Client(_accessKeyId, _secretAccessKeyId, cfg);
        }



        private class ResponseStreamWrapper : Stream
        {
            private readonly GetObjectResponse _response;


            public ResponseStreamWrapper(GetObjectResponse response)
            {
                _response = response ?? throw new ArgumentNullException(nameof(response));
            }


            public override bool CanRead
            {
                get { return _response.ResponseStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _response.ResponseStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _response.ResponseStream.CanWrite; }
            }

            public override long Length
            {
                get { return _response.ContentLength; }
            }

            public override long Position
            {
                get { return _response.ResponseStream.Position; }
                set { _response.ResponseStream.Position = value; }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _response.ResponseStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _response.ResponseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _response.ResponseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _response.ResponseStream.Write(buffer, offset, count);
            }

            public override void Flush()
            {
                _response.ResponseStream.Flush();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing) _response.Dispose();
            }
        }
    }
}