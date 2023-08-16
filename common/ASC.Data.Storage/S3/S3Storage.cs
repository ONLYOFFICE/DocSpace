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

using Amazon.Extensions.S3.Encryption;
using Amazon.Extensions.S3.Encryption.Primitives;
using Amazon.S3.Internal;

namespace ASC.Data.Storage.S3;

[Scope]
public class S3Storage : BaseStorage
{
    public override bool IsSupportCdnUri => true;
    public override bool IsSupportChunking => true;

    private readonly List<string> _domains = new List<string>();
    private Dictionary<string, S3CannedACL> _domainsAcl;
    private S3CannedACL _moduleAcl;
    private string _accessKeyId = string.Empty;
    private string _bucket = string.Empty;
    private string _recycleDir = string.Empty;
    private bool _recycleUse;
    private Uri _bucketRoot;
    private Uri _bucketSSlRoot;
    private string _region = "";
    private string _serviceurl;
    private bool _forcepathstyle;
    private string _secretAccessKeyId = string.Empty;
    private readonly ServerSideEncryptionMethod _sse = ServerSideEncryptionMethod.AES256;
    private bool _useHttp = true;
    private bool _lowerCasing = true;
    private bool _cdnEnabled;
    private string _cdnKeyPairId;
    private string _cdnPrivateKeyPath;
    private string _cdnDistributionDomain;
    private string _subDir = "";

    private EncryptionMethod _encryptionMethod = EncryptionMethod.None;
    private string _encryptionKey;
    private readonly IConfiguration _configuration;

    public S3Storage(
        TempStream tempStream,
        TenantManager tenantManager,
        PathUtils pathUtils,
        EmailValidationKeyProvider emailValidationKeyProvider,
        IHttpContextAccessor httpContextAccessor,
        ILoggerProvider factory,
        ILogger<S3Storage> options,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper,
        QuotaSocketManager quotaSocketManager)
        : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, factory, options, clientFactory, tenantQuotaFeatureStatHelper, quotaSocketManager)
    {
        _configuration = configuration;
    }

    public Uri GetUriInternal(string path)
    {
        return new Uri(SecureHelper.IsSecure(_httpContextAccessor?.HttpContext, _options) ? _bucketSSlRoot : _bucketRoot, path);
    }

    public Uri GetUriShared(string domain, string path)
    {
        return new Uri(SecureHelper.IsSecure(_httpContextAccessor?.HttpContext, _options) ? _bucketSSlRoot : _bucketRoot, MakePath(domain, path));
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
            Protocol = SecureHelper.IsSecure(_httpContextAccessor?.HttpContext, _options) ? Protocol.HTTPS : Protocol.HTTP,
            Verb = HttpVerb.GET
        };

        if (headers != null && headers.Any())
        {
            var headersOverrides = new ResponseHeaderOverrides();

            foreach (var h in headers)
            {
                if (h.StartsWith("Content-Disposition"))
                {
                    headersOverrides.ContentDisposition = (h.Substring("Content-Disposition".Length + 1));
                }
                else if (h.StartsWith("Cache-Control"))
                {
                    headersOverrides.CacheControl = (h.Substring("Cache-Control".Length + 1));
                }
                else if (h.StartsWith("Content-Encoding"))
                {
                    headersOverrides.ContentEncoding = (h.Substring("Content-Encoding".Length + 1));
                }
                else if (h.StartsWith("Content-Language"))
                {
                    headersOverrides.ContentLanguage = (h.Substring("Content-Language".Length + 1));
                }
                else if (h.StartsWith("Content-Type"))
                {
                    headersOverrides.ContentType = (h.Substring("Content-Type".Length + 1));
                }
                else if (h.StartsWith("Expires"))
                {
                    headersOverrides.Expires = (h.Substring("Expires".Length + 1));
                }
                else
                {
                    throw new FormatException(string.Format("Invalid header: {0}", h));
                }
            }

            pUrlRequest.ResponseHeaderOverrides = headersOverrides;
        }

        using var client = GetClient();

        return Task.FromResult(MakeUri(client.GetPreSignedURL(pUrlRequest)));
    }

    public override Task<Uri> GetCdnPreSignedUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
    {
        if (!_cdnEnabled) return GetInternalUriAsync(domain, path, expire, headers);

        var proto = SecureHelper.IsSecure(_httpContextAccessor?.HttpContext, _options) ? "https" : "http";

        var baseUrl = $"{proto}://{_cdnDistributionDomain}/{MakePath(domain, path)}";

        var uriBuilder = new UriBuilder(baseUrl)
        {
            Port = -1
        };

        var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (headers != null && headers.Any())
        {
            foreach (var h in headers)
            {
                if (h.StartsWith("Content-Disposition"))
                {
                    queryParams["response-content-disposition"] = h.Substring("Content-Disposition".Length + 1);
                }
                else if (h.StartsWith("Cache-Control"))
                {
                    queryParams["response-cache-control"] = h.Substring("Cache-Control".Length + 1);
                }
                else if (h.StartsWith("Content-Encoding"))
                {
                    queryParams["response-content-encoding"] = h.Substring("Content-Encoding".Length + 1);
                }
                else if (h.StartsWith("Content-Language"))
                {
                    queryParams["response-content-language"] = h.Substring("Content-Language".Length + 1);
                }
                else if (h.StartsWith("Content-Type"))
                {
                    queryParams["response-content-type"] = h.Substring("Content-Type".Length + 1);
                }
                else if (h.StartsWith("Expires"))
                {
                    queryParams["response-expires"] = h.Substring("Expires".Length + 1);
                }
                else if (h.StartsWith("Custom-Cache-Key"))
                {
                    queryParams["custom-cache-key"] = h.Substring("Custom-Cache-Key".Length + 1);
                }
                else
                {
                    throw new FormatException(string.Format("Invalid header: {0}", h));
                }
            }
        }

        uriBuilder.Query = queryParams.ToString();

        var signedUrl = "";

        using (TextReader textReader = File.OpenText(_cdnPrivateKeyPath))
        {
            signedUrl = AmazonCloudFrontUrlSigner.GetCannedSignedURL(
                      uriBuilder.ToString(),
                      textReader,
                      _cdnKeyPairId,
                      DateTime.UtcNow.Add(expire));
        }

        return Task.FromResult(new Uri(signedUrl));
    }

    public override Task<Stream> GetReadStreamAsync(string domain, string path)
    {
        return GetReadStreamAsync(domain, path, 0);
    }

    public override async Task<Stream> GetReadStreamAsync(string domain, string path, long offset)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucket,
            Key = MakePath(domain, path)
        };

        if (0 < offset)
        {
            request.ByteRange = new ByteRange(offset, int.MaxValue);
        }

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

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                string contentDisposition)
    {
        return SaveAsync(domain, path, stream, contentType, contentDisposition, ACL.Auto);
    }

    private bool EnableQuotaCheck(string domain)
    {
        return (QuotaController != null) && !domain.EndsWith("_temp");
    }

    public async Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                         string contentDisposition, ACL acl, string contentEncoding = null, int cacheDays = 5)
    {
        var buffered = _tempStream.GetBuffered(stream);

        if (EnableQuotaCheck(domain))
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
            AutoCloseStream = false
        };

        if (!(client is IAmazonS3Encryption))
        {
            string kmsKeyId;
            request.ServerSideEncryptionMethod = GetServerSideEncryptionMethod(out kmsKeyId);
            request.ServerSideEncryptionKeyManagementServiceKeyId = kmsKeyId;
        }

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

        //await InvalidateCloudFrontAsync(MakePath(domain, path));

        await QuotaUsedAdd(domain, buffered.Length);

        return await GetUriAsync(domain, path);
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
            Key = MakePath(domain, path)
        };

        using var s3 = GetClient();
        if (!(s3 is IAmazonS3Encryption))
        {
            string kmsKeyId;
            request.ServerSideEncryptionMethod = GetServerSideEncryptionMethod(out kmsKeyId);
            request.ServerSideEncryptionKeyManagementServiceKeyId = kmsKeyId;
        }
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
            var response = await s3.UploadPartAsync(request);

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
                //    await InvalidateCloudFrontAsync(MakePath(domain, path));
            }

            if (QuotaController != null)
            {
                var size = await GetFileSizeAsync(domain, path);
                await QuotaUsedAdd(domain, size);
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

    public override IDataWriteOperator CreateDataWriteOperator(CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder)
    {
        return new S3ZipWriteOperator(_tempStream, chunkedUploadSession, sessionHolder);
    }

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

        await QuotaUsedDelete(domain, size);
    }

    public override Task DeleteFilesAsync(string domain, List<string> paths)
    {
        if (paths.Count == 0)
        {
            return Task.CompletedTask;
        }

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
        {
            return;
        }

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
            await QuotaUsedDelete(domain, quotaUsed);
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

            if (QuotaController != null)
            {
                if (string.IsNullOrEmpty(QuotaController.ExcludePattern) ||
                    !Path.GetFileName(s3Object.Key).StartsWith(QuotaController.ExcludePattern))
                {
                    await QuotaUsedDelete(domain, s3Object.Size);
                }
            }
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

            await QuotaUsedDelete(domain, s3Object.Size);
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
            await CopyFileAsync(client, s3Object.Key, s3Object.Key.Replace(srckey, dstkey), newdomain);

            await client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = s3Object.Key
            });
        }
    }

    public override async Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
    {
        var srcKey = MakePath(srcdomain, srcpath);
        var dstKey = MakePath(newdomain, newpath);
        var size = await GetFileSizeAsync(srcdomain, srcpath);

        using var client = GetClient();
        await CopyFileAsync(client, srcKey, dstKey, newdomain, S3MetadataDirective.REPLACE);
        await DeleteAsync(srcdomain, srcpath);

        await QuotaUsedDelete(srcdomain, size);
        await QuotaUsedAdd(newdomain, size, quotaCheckFileSize);

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
        var buffered = _tempStream.GetBuffered(stream);
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

            var metadata = await client.GetObjectMetadataAsync(request);
            var privateExpireKey = metadata.Metadata["private-expire"];
            if (string.IsNullOrEmpty(privateExpireKey))
            {
                continue;
            }

            if (!long.TryParse(privateExpireKey, out var fileTime))
            {
                continue;
            }

            if (DateTime.UtcNow <= DateTime.FromFileTimeUtc(fileTime))
            {
                continue;
            }
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
        {
            postBuilder.Append($"\"Content-Type\":\"{contentType}\",");
        }

        if (!string.IsNullOrEmpty(contentDisposition))
        {
            postBuilder.Append($"\"Content-Disposition\":\"{contentDisposition}\",");
        }

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
        {
            formBuilder.Append($"<input type=\"hidden\" name=\"success_action_redirect\" value=\"{redirectTo}\" />");
        }

        formBuilder.AppendFormat("<input type=\"hidden\" name=\"success_action_status\" value=\"{0}\" />", 201);

        if (!string.IsNullOrEmpty(contentType))
        {
            formBuilder.Append($"<input type=\"hidden\" name=\"Content-Type\" value=\"{contentType}\" />");
        }

        if (!string.IsNullOrEmpty(contentDisposition))
        {
            formBuilder.Append($"<input type=\"hidden\" name=\"Content-Disposition\" value=\"{contentDisposition}\" />");
        }

        formBuilder.Append($"<input type=\"hidden\" name=\"AWSAccessKeyId\" value=\"{_accessKeyId}\"/>");
        formBuilder.Append($"<input type=\"hidden\" name=\"Policy\" value=\"{policyBase64}\" />");
        formBuilder.Append($"<input type=\"hidden\" name=\"Signature\" value=\"{sign}\" />");
        formBuilder.Append("<input type=\"hidden\" name=\"SignatureVersion\" value=\"2\" />");
        formBuilder.Append("<input type=\"hidden\" name=\"SignatureMethod\" value=\"HmacSHA1{0}\" />");
        formBuilder.Append("<input type=\"file\" name=\"file\" />");
        formBuilder.Append($"<input type=\"submit\" name=\"submit\" value=\"{submitLabel}\" /></form>");

        return formBuilder.ToString();
    }

    public override async IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive)
    {
        var tmp = await GetS3ObjectsAsync(domain, path);
        var obj = tmp.Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Key)))
            .Select(x => x.Key.Substring((MakePath(domain, path) + "/").Length).TrimStart('/'));

        foreach (var e in obj)
        {
            yield return e;
        }
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
        {
            throw new FileNotFoundException("directory not found", path);
        }

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
            QuotaController.QuotaUsedSet(Modulename, domain, DataList.GetData(domain), size);

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
        var srcKey = MakePath(srcdomain, srcpath);
        var dstKey = MakePath(newdomain, newpath);
        var size = await GetFileSizeAsync(srcdomain, srcpath);
        using var client = GetClient();
        await CopyFileAsync(client, srcKey, dstKey, newdomain, S3MetadataDirective.REPLACE);

        await QuotaUsedAdd(newdomain, size);

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
            await CopyFileAsync(client, s3Object.Key, s3Object.Key.Replace(srckey, dstkey), newdomain);

            await QuotaUsedAdd(newdomain, s3Object.Size);
        }
    }

    public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
    {
        Tenant = tenant;

        if (moduleConfig != null)
        {
            Modulename = moduleConfig.Name;
            DataList = new DataList(moduleConfig);
            _domains.AddRange(moduleConfig.Domain.Select(x => $"{x.Name}/"));

            //Make expires
            DomainsExpires = moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).ToDictionary(x => x.Name, y => y.Expires);
            DomainsExpires.Add(string.Empty, moduleConfig.Expires);

            _domainsAcl = moduleConfig.Domain.ToDictionary(x => x.Name, y => GetS3Acl(y.Acl));
            _moduleAcl = GetS3Acl(moduleConfig.Acl);
        }
        else
        {
            Modulename = string.Empty;
            DataList = null;

            //Make expires
            DomainsExpires = new Dictionary<string, TimeSpan> { { string.Empty, TimeSpan.Zero } };

            _domainsAcl = new Dictionary<string, S3CannedACL>();
            _moduleAcl = S3CannedACL.PublicRead;
        }

        _accessKeyId = props["acesskey"];
        _secretAccessKeyId = props["secretaccesskey"];
        _bucket = props["bucket"];

        props.TryGetValue("recycleDir", out _recycleDir);

        if (props.TryGetValue("recycleUse", out var recycleUseProp) && bool.TryParse(recycleUseProp, out var recycleUse))
        {
            _recycleUse = recycleUse;
        }

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
            _encryptionMethod = sse.ToLower() switch
            {
                "none" => EncryptionMethod.None,
                "aes256" => EncryptionMethod.ServerS3,
                "awskms" => EncryptionMethod.ServerKms,
                "clientawskms" => EncryptionMethod.ClientKms,
                _ => EncryptionMethod.None,
            };
        }

        if (props.ContainsKey("ssekey") && !string.IsNullOrEmpty(props["ssekey"]))
        {
            _encryptionKey = props["ssekey"];
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

        if (props.TryGetValue("cdn_enabled", out var cdnEnabled))
        {
            if (bool.TryParse(cdnEnabled, out _cdnEnabled))
            {
                _cdnKeyPairId = props["cdn_keyPairId"];
                _cdnPrivateKeyPath = props["cdn_privateKeyPath"];
                _cdnDistributionDomain = props["cdn_distributionDomain"];
            }
        }

        props.TryGetValue("subdir", out _subDir);

        return this;
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

    private Uri MakeUri(string preSignedURL)
    {
        var uri = new Uri(preSignedURL);
        var signedPart = uri.PathAndQuery.TrimStart('/');

        var baseUri = uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? _bucketSSlRoot : _bucketRoot;

        if (preSignedURL.StartsWith(baseUri.ToString())) return uri;

        return new UnencodedUri(baseUri, signedPart);
    }

    private Task InvalidateCloudFrontAsync(params string[] paths)
    {
        if (!_cdnEnabled || string.IsNullOrEmpty(_cdnDistributionDomain))
        {
            return Task.CompletedTask;
        }

        return InternalInvalidateCloudFrontAsync(paths);
    }

    private async Task InternalInvalidateCloudFrontAsync(params string[] paths)
    {
        using var cfClient = GetCloudFrontClient();
        var invalidationRequest = new CreateInvalidationRequest
        {
            DistributionId = _cdnDistributionDomain,
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


    private bool CheckKey(string domain, string key)
    {
        return !string.IsNullOrEmpty(domain) ||
               _domains.All(configuredDomains => !key.StartsWith(MakePath(configuredDomains, "")));
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
        if (string.IsNullOrEmpty(_recycleDir) || !recycle)
        {
            return s30Objects;
        }

        s30Objects.Concat(await GetS3ObjectsByPathAsync(domain, GetRecyclePath(path)));
        return s30Objects;
    }

    private string MakePath(string domain, string path)
    {
        string result;

        path = path.TrimStart('\\', '/').TrimEnd('/').Replace('\\', '/');

        if (!string.IsNullOrEmpty(_subDir))
        {
            if (_subDir.Length == 1 && (_subDir[0] == '/' || _subDir[0] == '\\'))
            {
                result = path;
            }
            else
            {
                result = $"{_subDir}/{path}"; // Ignory all, if _subDir is not null
            }
        }
        else//Key combined from module+domain+filename
        {
            result = $"{Tenant}/{Modulename}/{domain}/{path}";
        }


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
        if (string.IsNullOrEmpty(_recycleDir) || string.IsNullOrEmpty(domain) || domain.EndsWith("_temp") || !_recycleUse)
        {
            return Task.CompletedTask;
        }

        return InternalRecycleAsync(client, domain, key);
    }

    private async Task InternalRecycleAsync(IAmazonS3 client, string domain, string key)
    {
        await CopyFileAsync(client, key, GetRecyclePath(key), domain, S3MetadataDirective.REPLACE, S3StorageClass.Glacier);
    }

    private async Task CopyFileAsync(IAmazonS3 client, string sourceKey, string destinationKey, string newdomain, S3MetadataDirective metadataDirective = S3MetadataDirective.COPY, S3StorageClass storageClass = null)
    {
        var metadataRequest = new GetObjectMetadataRequest
        {
            BucketName = _bucket,
            Key = sourceKey
        };

        var metadataResponse = await client.GetObjectMetadataAsync(metadataRequest);
        var objectSize = metadataResponse.ContentLength;

        if (objectSize >= 100 * 1024 * 1024L) //100 megabytes
        {
            var copyResponses = new List<CopyPartResponse>();

            var initiateRequest =
                new InitiateMultipartUploadRequest
                {
                    BucketName = _bucket,
                    Key = destinationKey,
                    CannedACL = GetDomainACL(newdomain)
                };

            if (!(client is IAmazonS3Encryption))
            {
                string kmsKeyId;
                initiateRequest.ServerSideEncryptionMethod = GetServerSideEncryptionMethod(out kmsKeyId);
                initiateRequest.ServerSideEncryptionKeyManagementServiceKeyId = kmsKeyId;
            }

            if (storageClass != null)
            {
                initiateRequest.StorageClass = storageClass;
            }

            var initResponse = await client.InitiateMultipartUploadAsync(initiateRequest);

            var uploadId = initResponse.UploadId;

            var partSize = GetChunkSize();

            var uploadTasks = new List<Task<CopyPartResponse>>();

            long bytePosition = 0;
            for (var i = 1; bytePosition < objectSize; i++)
            {
                var copyRequest = new CopyPartRequest
                {
                    DestinationBucket = _bucket,
                    DestinationKey = destinationKey,
                    SourceBucket = _bucket,
                    SourceKey = sourceKey,
                    UploadId = uploadId,
                    FirstByte = bytePosition,
                    LastByte = bytePosition + partSize - 1 >= objectSize ? objectSize - 1 : bytePosition + partSize - 1,
                    PartNumber = i
                };

                uploadTasks.Add(client.CopyPartAsync(copyRequest));

                bytePosition += partSize;
            }

            copyResponses.AddRange(await Task.WhenAll(uploadTasks));

            var completeRequest =
                new CompleteMultipartUploadRequest
                {
                    BucketName = _bucket,
                    Key = destinationKey,
                    UploadId = initResponse.UploadId
                };
            completeRequest.AddPartETags(copyResponses);

            await client.CompleteMultipartUploadAsync(completeRequest);
        }
        else
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = _bucket,
                SourceKey = sourceKey,
                DestinationBucket = _bucket,
                DestinationKey = destinationKey,
                CannedACL = GetDomainACL(newdomain),
                MetadataDirective = metadataDirective,
            };

            if (!(client is IAmazonS3Encryption))
            {
                string kmsKeyId;
                request.ServerSideEncryptionMethod = GetServerSideEncryptionMethod(out kmsKeyId);
                request.ServerSideEncryptionKeyManagementServiceKeyId = kmsKeyId;
            }

            if (storageClass != null)
            {
                request.StorageClass = storageClass;
            }

            await client.CopyObjectAsync(request);
        }
    }

    private IAmazonCloudFront GetCloudFrontClient()
    {
        var cfg = new AmazonCloudFrontConfig { MaxErrorRetry = 3 };

        return new AmazonCloudFrontClient(_accessKeyId, _secretAccessKeyId, cfg);
    }

    private IAmazonS3 GetClient()
    {
        var encryptionClient = GetEncryptionClient();

        if (encryptionClient != null)
        {
            return encryptionClient;
        }

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
        public override bool CanRead => _response.ResponseStream.CanRead;
        public override bool CanSeek => _response.ResponseStream.CanSeek;
        public override bool CanWrite => _response.ResponseStream.CanWrite;
        public override long Length => _response.ContentLength;
        public override long Position
        {
            get => _response.ResponseStream.Position;
            set => _response.ResponseStream.Position = value;
        }

        private readonly GetObjectResponse _response;

        public ResponseStreamWrapper(GetObjectResponse response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
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
            if (disposing)
            {
                _response.Dispose();
            }
        }
    }

    private IAmazonS3 GetEncryptionClient()
    {
        if (!string.IsNullOrEmpty(_encryptionKey))
        {
            return null;
        }

        EncryptionMaterialsV2 encryptionMaterials = null;

        switch (_encryptionMethod)
        {
            case EncryptionMethod.ClientKms:
                var encryptionContext = new Dictionary<string, string>();
                encryptionMaterials = new EncryptionMaterialsV2(_encryptionKey, KmsType.KmsContext, encryptionContext);
                break;
                //case EncryptionMethod.ClientAes:
                //    var symmetricAlgorithm = Aes.Create();
                //    symmetricAlgorithm.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                //    encryptionMaterials = new EncryptionMaterialsV2(symmetricAlgorithm, SymmetricAlgorithmType.AesGcm);
                //    break;
                //case EncryptionMethod.ClientRsa:
                //    var asymmetricAlgorithm = RSA.Create();
                //    asymmetricAlgorithm.FromXmlString(_encryptionKey);
                //    encryptionMaterials = new EncryptionMaterialsV2(asymmetricAlgorithm, AsymmetricAlgorithmType.RsaOaepSha1);
                //    break;
        }

        if (encryptionMaterials == null)
        {
            return null;
        }

        var cfg = new AmazonS3CryptoConfigurationV2(SecurityProfile.V2AndLegacy)
        {
            StorageMode = CryptoStorageMode.ObjectMetadata,
            MaxErrorRetry = 3
        };

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

        return new AmazonS3EncryptionClientV2(_accessKeyId, _secretAccessKeyId, cfg, encryptionMaterials);
    }
    private ServerSideEncryptionMethod GetServerSideEncryptionMethod(out string kmsKeyId)
    {
        kmsKeyId = null;

        var method = ServerSideEncryptionMethod.None;

        switch (_encryptionMethod)
        {
            case EncryptionMethod.ServerS3:
                method = ServerSideEncryptionMethod.AES256;
                break;
            case EncryptionMethod.ServerKms:
                method = ServerSideEncryptionMethod.AWSKMS;
                if (!string.IsNullOrEmpty(_encryptionKey))
                {
                    kmsKeyId = _encryptionKey;
                }
                break;
        }

        return method;
    }

    public override async Task<string> GetFileEtagAsync(string domain, string path)
    {
        using var client = GetClient();

        var getObjectMetadataRequest = new GetObjectMetadataRequest
        {
            BucketName = _bucket,
            Key = MakePath(domain, path)
        };

        var el = await client.GetObjectMetadataAsync(getObjectMetadataRequest);

        return el.ETag;
    }

    private long GetChunkSize()
    {
        var configSetting = _configuration["files:uploader:chunk-size"];
        if (!string.IsNullOrEmpty(configSetting))
        {
            configSetting = configSetting.Trim();
            return long.Parse(configSetting);
        }
        long defaultValue = 10 * 1024 * 1024;
        return defaultValue;
    }

    private enum EncryptionMethod
    {
        None,
        ServerS3,
        ServerKms,
        ClientKms,
        //ClientAes,
        //ClientRsa
    }
}
