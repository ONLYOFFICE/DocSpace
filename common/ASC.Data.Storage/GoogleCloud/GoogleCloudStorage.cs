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

namespace ASC.Data.Storage.GoogleCloud;

[Scope]
public class GoogleCloudStorage : BaseStorage
{
    public override bool IsSupportChunking => true;

    private string _subDir = string.Empty;
    private Dictionary<string, PredefinedObjectAcl> _domainsAcl;
    private PredefinedObjectAcl _moduleAcl;
    private string _bucket = string.Empty;
    private string _json = string.Empty;
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
        Tenant = tenant;

        if (moduleConfig != null)
        {
            Modulename = moduleConfig.Name;
            DataList = new DataList(moduleConfig);

            DomainsExpires = moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).ToDictionary(x => x.Name, y => y.Expires);

            DomainsExpires.Add(string.Empty, moduleConfig.Expires);

            _domainsAcl = moduleConfig.Domain.ToDictionary(x => x.Name, y => GetGoogleCloudAcl(y.Acl));
            _moduleAcl = GetGoogleCloudAcl(moduleConfig.Acl);
        }
        else
        {
            Modulename = string.Empty;
            DataList = null;

            DomainsExpires = new Dictionary<string, TimeSpan> { { string.Empty, TimeSpan.Zero } };
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

    public static long DateToUnixTimestamp(DateTime date)
    {
        var ts = date - new DateTime(1970, 1, 1, 0, 0, 0);

        return (long)ts.TotalSeconds;
    }

    public override Uri GetInternalUri(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
    {
        if (expire == TimeSpan.Zero || expire == TimeSpan.MinValue || expire == TimeSpan.MaxValue)
        {
            expire = GetExpire(domain);
        }
        if (expire == TimeSpan.Zero || expire == TimeSpan.MinValue || expire == TimeSpan.MaxValue)
        {
            return GetUriShared(domain, path);
        }

        using var storage = GetStorage();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_json ?? ""));
        var preSignedURL = UrlSigner.FromServiceAccountData(stream).Sign(_bucket, MakePath(domain, path), expire, HttpMethod.Get);

        return MakeUri(preSignedURL);
    }

    public Uri GetUriShared(string domain, string path)
    {
        return new Uri(SecureHelper.IsSecure(HttpContextAccessor.HttpContext, Options) ? _bucketSSlRoot : _bucketRoot, MakePath(domain, path));
    }

    public override Stream GetReadStream(string domain, string path)
    {
        return GetReadStream(domain, path, 0);
    }

    public override Stream GetReadStream(string domain, string path, int offset)
    {
        var tempStream = TempStream.Create();

        using var storage = GetStorage();

        storage.DownloadObject(_bucket, MakePath(domain, path), tempStream, null, null);

        if (offset > 0)
        {
            tempStream.Seek(offset, SeekOrigin.Begin);
        }

        tempStream.Position = 0;

        return tempStream;
    }

    public override async Task<Stream> GetReadStreamAsync(string domain, string path, int offset)
    {
        var tempStream = TempStream.Create();

        var storage = GetStorage();

        await storage.DownloadObjectAsync(_bucket, MakePath(domain, path), tempStream);

        if (offset > 0)
        {
            tempStream.Seek(offset, SeekOrigin.Begin);
        }

        tempStream.Position = 0;

        return tempStream;
    }

    public override Uri Save(string domain, string path, System.IO.Stream stream)
    {
        return Save(domain, path, stream, string.Empty, string.Empty);
    }

    public override Uri Save(string domain, string path, System.IO.Stream stream, Configuration.ACL acl)
    {
        return Save(domain, path, stream, null, null, acl);
    }

    public override Uri Save(string domain, string path, System.IO.Stream stream, string contentType, string contentDisposition)
    {
        return Save(domain, path, stream, contentType, contentDisposition, ACL.Auto);
    }

    public override Uri Save(string domain, string path, System.IO.Stream stream, string contentEncoding, int cacheDays)
    {
        return Save(domain, path, stream, string.Empty, string.Empty, ACL.Auto, contentEncoding, cacheDays);
    }

    public Uri Save(string domain, string path, Stream stream, string contentType,
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

        var uploaded = storage.UploadObject(_bucket, MakePath(domain, path), mime, buffered, uploadObjectOptions, null);

        uploaded.ContentEncoding = contentEncoding;
        uploaded.CacheControl = string.Format("public, maxage={0}", (int)TimeSpan.FromDays(cacheDays).TotalSeconds);

        if (uploaded.Metadata == null)
        {
            uploaded.Metadata = new Dictionary<string, string>();
        }

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

        return GetUri(domain, path);
    }

    public override void Delete(string domain, string path)
    {
        using var storage = GetStorage();

        var key = MakePath(domain, path);
        var size = GetFileSize(domain, path);

        storage.DeleteObject(_bucket, key);

        QuotaUsedDelete(domain, size);
    }

    public override void DeleteFiles(string domain, string folderPath, string pattern, bool recursive)
    {
        using var storage = GetStorage();

        IEnumerable<Google.Apis.Storage.v1.Data.Object> objToDel;

        if (recursive)
        {
            objToDel = storage
                .ListObjects(_bucket, MakePath(domain, folderPath))
                .Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Name)));
        }
        else
        {
            objToDel = new List<Google.Apis.Storage.v1.Data.Object>();
        }

        foreach (var obj in objToDel)
        {
            storage.DeleteObject(_bucket, obj.Name);
            QuotaUsedDelete(domain, Convert.ToInt64(obj.Size));
        }
    }

    public override void DeleteFiles(string domain, List<string> paths)
    {
        if (paths.Count == 0) return;

        var keysToDel = new List<string>();

        long quotaUsed = 0;

        foreach (var path in paths)
        {
            try
            {

                var key = MakePath(domain, path);

                if (QuotaController != null)
                {
                    quotaUsed += GetFileSize(domain, path);
                }

                keysToDel.Add(key);
            }
            catch (FileNotFoundException)
            {

            }
        }

        if (keysToDel.Count == 0) return;

        using var storage = GetStorage();

        keysToDel.ForEach(x => storage.DeleteObject(_bucket, x));

        if (quotaUsed > 0)
        {
            QuotaUsedDelete(domain, quotaUsed);
        }
    }

    public override void DeleteFiles(string domain, string folderPath, DateTime fromDate, DateTime toDate)
    {
        using var storage = GetStorage();

        var objToDel = GetObjects(domain, folderPath, true)
                          .Where(x => x.Updated >= fromDate && x.Updated <= toDate);

        foreach (var obj in objToDel)
        {
            storage.DeleteObject(_bucket, obj.Name);
            QuotaUsedDelete(domain, Convert.ToInt64(obj.Size));
        }
    }

    public override void MoveDirectory(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        using var storage = GetStorage();
        var srckey = MakePath(srcdomain, srcdir);
        var dstkey = MakePath(newdomain, newdir);

        var objects = storage.ListObjects(_bucket, srckey);

        foreach (var obj in objects)
        {
            storage.CopyObject(_bucket, srckey, _bucket, dstkey, new CopyObjectOptions
            {
                DestinationPredefinedAcl = GetDomainACL(newdomain)
            });

            storage.DeleteObject(_bucket, srckey);

        }
    }

    public override Uri Move(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
    {
        using var storage = GetStorage();

        var srcKey = MakePath(srcdomain, srcpath);
        var dstKey = MakePath(newdomain, newpath);
        var size = GetFileSize(srcdomain, srcpath);

        storage.CopyObject(_bucket, srcKey, _bucket, dstKey, new CopyObjectOptions
        {
            DestinationPredefinedAcl = GetDomainACL(newdomain)
        });

        Delete(srcdomain, srcpath);

        QuotaUsedDelete(srcdomain, size);
        QuotaUsedAdd(newdomain, size, quotaCheckFileSize);

        return GetUri(newdomain, newpath);
    }

    public override Uri SaveTemp(string domain, out string assignedPath, System.IO.Stream stream)
    {
        assignedPath = Guid.NewGuid().ToString();

        return Save(domain, assignedPath, stream);
    }

    public override string[] ListDirectoriesRelative(string domain, string path, bool recursive)
    {
        return GetObjects(domain, path, recursive)
               .Select(x => x.Name.Substring(MakePath(domain, path + "/").Length))
               .ToArray();
    }

    private IEnumerable<Google.Apis.Storage.v1.Data.Object> GetObjects(string domain, string path, bool recursive)
    {
        using var storage = GetStorage();

        var items = storage.ListObjects(_bucket, MakePath(domain, path));

        if (recursive)
        {
            return items;
        }

        return items.Where(x => x.Name.IndexOf('/', MakePath(domain, path + "/").Length) == -1);
    }

    public override string[] ListFilesRelative(string domain, string path, string pattern, bool recursive)
    {
        return GetObjects(domain, path, recursive).Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Name)))
               .Select(x => x.Name.Substring(MakePath(domain, path + "/").Length).TrimStart('/')).ToArray();
    }

    public override bool IsFile(string domain, string path)
    {
        using var storage = GetStorage();

        var objects = storage.ListObjects(_bucket, MakePath(domain, path), null);

        return objects.Any();
    }

    public override async Task<bool> IsFileAsync(string domain, string path)
    {
        var storage = GetStorage();

        var objects = await storage.ListObjectsAsync(_bucket, MakePath(domain, path)).ReadPageAsync(1);

        return objects.Any();
    }

    public override bool IsDirectory(string domain, string path)
    {
        return IsFile(domain, path);
    }

    public override void DeleteDirectory(string domain, string path)
    {
        using var storage = GetStorage();

        var objToDel = storage
                      .ListObjects(_bucket, MakePath(domain, path));

        foreach (var obj in objToDel)
        {
            storage.DeleteObject(_bucket, obj.Name);
            QuotaUsedDelete(domain, Convert.ToInt64(obj.Size));
        }
    }

    public override long GetFileSize(string domain, string path)
    {
        using var storage = GetStorage();

        var obj = storage.GetObject(_bucket, MakePath(domain, path));

        return obj.Size.HasValue ? Convert.ToInt64(obj.Size.Value) : 0;
    }

    public override long GetDirectorySize(string domain, string path)
    {
        using var storage = GetStorage();

        var objToDel = storage
                      .ListObjects(_bucket, MakePath(domain, path));

        long result = 0;

        foreach (var obj in objToDel)
        {
            if (obj.Size.HasValue)
            {
                result += Convert.ToInt64(obj.Size.Value);
            }
        }

        return result;
    }

    public override long ResetQuota(string domain)
    {
        using var storage = GetStorage();

        var objects = storage
                      .ListObjects(_bucket, MakePath(domain, string.Empty));

        if (QuotaController != null)
        {
            long size = 0;

            foreach (var obj in objects)
            {
                if (obj.Size.HasValue)
                {
                    size += Convert.ToInt64(obj.Size.Value);
                }
            }

            QuotaController.QuotaUsedSet(Modulename, domain, DataList.GetData(domain), size);

            return size;
        }

        return 0;
    }

    public override long GetUsedQuota(string domain)
    {
        using var storage = GetStorage();

        var objects = storage
                      .ListObjects(_bucket, MakePath(domain, string.Empty));

        long result = 0;

        foreach (var obj in objects)
        {
            if (obj.Size.HasValue)
            {
                result += Convert.ToInt64(obj.Size.Value);
            }
        }

        return result;
    }

    public override Uri Copy(string srcdomain, string srcpath, string newdomain, string newpath)
    {
        using var storage = GetStorage();

        var size = GetFileSize(srcdomain, srcpath);

        var options = new CopyObjectOptions
        {
            DestinationPredefinedAcl = GetDomainACL(newdomain)
        };

        storage.CopyObject(_bucket, MakePath(srcdomain, srcpath), _bucket, MakePath(newdomain, newpath), options);

        QuotaUsedAdd(newdomain, size);

        return GetUri(newdomain, newpath);
    }

    public override void CopyDirectory(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        var srckey = MakePath(srcdomain, srcdir);
        var dstkey = MakePath(newdomain, newdir);
        //List files from src

        using var storage = GetStorage();

        var objects = storage.ListObjects(_bucket, srckey);

        foreach (var obj in objects)
        {
            storage.CopyObject(_bucket, srckey, _bucket, dstkey, new CopyObjectOptions
            {
                DestinationPredefinedAcl = GetDomainACL(newdomain)
            });

            QuotaUsedAdd(newdomain, Convert.ToInt64(obj.Size));
        }
    }

    public override string SavePrivate(string domain, string path, System.IO.Stream stream, DateTime expires)
    {
        using var storage = GetStorage();

        var buffered = TempStream.GetBuffered(stream);

        var uploadObjectOptions = new UploadObjectOptions
        {
            PredefinedAcl = PredefinedObjectAcl.BucketOwnerFullControl
        };

        buffered.Position = 0;

        var uploaded = storage.UploadObject(_bucket, MakePath(domain, path), "application/octet-stream", buffered, uploadObjectOptions, null);

        uploaded.CacheControl = string.Format("public, maxage={0}", (int)TimeSpan.FromDays(5).TotalSeconds);
        uploaded.ContentDisposition = "attachment";

        if (uploaded.Metadata == null)
        {
            uploaded.Metadata = new Dictionary<string, string>();
        }

        uploaded.Metadata["Expires"] = DateTime.UtcNow.Add(TimeSpan.FromDays(5)).ToString("R");
        uploaded.Metadata.Add("private-expire", expires.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));

        storage.UpdateObject(uploaded);

        using var mStream = new MemoryStream(Encoding.UTF8.GetBytes(_json ?? ""));
        var preSignedURL = FromServiceAccountData(mStream).Sign(RequestTemplate.FromBucket(_bucket).WithObjectName(MakePath(domain, path)), UrlSigner.Options.FromExpiration(expires));

        //TODO: CNAME!
        return preSignedURL;
    }

    public override void DeleteExpired(string domain, string path, TimeSpan oldThreshold)
    {
        using var storage = GetStorage();

        var objects = storage.ListObjects(_bucket, MakePath(domain, path));

        foreach (var obj in objects)
        {
            var objInfo = storage.GetObject(_bucket, MakePath(domain, path), null);

            var privateExpireKey = objInfo.Metadata["private-expire"];

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

            storage.DeleteObject(_bucket, MakePath(domain, path));

        }
    }

    #region chunking

    public override string InitiateChunkedUpload(string domain, string path)
    {
        using var storage = GetStorage();

        var tempUploader = storage.CreateObjectUploader(_bucket, MakePath(domain, path), null, new MemoryStream());

        var sessionUri = tempUploader.InitiateSessionAsync().Result;

        return sessionUri.ToString();
    }

    public override string UploadChunk(string domain,
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
        {
            totalBytes = Convert.ToString((chunkNumber - 1) * defaultChunkSize + chunkLength);
        }

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
                using var response = httpClient.Send(request);

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
                {
                    throw;
                }

                break;
            }
            catch
            {
                AbortChunkedUpload(domain, path, uploadUri);
                throw;
            }
        }

        return string.Empty;
    }

    public override Uri FinalizeChunkedUpload(string domain, string path, string uploadUri, Dictionary<int, string> eTags)
    {
        if (QuotaController != null)
        {
            var size = GetFileSize(domain, path);
            QuotaUsedAdd(domain, size);
        }

        return GetUri(domain, path);
    }

    public override void AbortChunkedUpload(string domain, string path, string uploadUri) { }

    #endregion

    public override string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize, string contentType, string contentDisposition, string submitLabel)
    {
        throw new NotImplementedException();
    }

    public override string GetUploadedUrl(string domain, string directoryPath)
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

    protected override Uri SaveWithAutoAttachment(string domain, string path, System.IO.Stream stream, string attachmentFileName)
    {
        var contentDisposition = $"attachment; filename={HttpUtility.UrlPathEncode(attachmentFileName)};";
        if (attachmentFileName.Any(c => c >= 0 && c <= 127))
        {
            contentDisposition = $"attachment; filename*=utf-8''{HttpUtility.UrlPathEncode(attachmentFileName)};";
        }
        return Save(domain, path, stream, null, contentDisposition);
    }

    private StorageClient GetStorage()
    {
        var credential = GoogleCredential.FromJson(_json);

        return StorageClient.Create(credential);
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
            result = $"{Tenant}/{Modulename}/{domain}/{path}";

        result = result.Replace("//", "/").TrimStart('/');
        if (_lowerCasing)
        {
            result = result.ToLowerInvariant();
        }

        return result;
    }

    private Uri MakeUri(string preSignedURL)
    {
        var uri = new Uri(preSignedURL);
        var signedPart = uri.PathAndQuery.TrimStart('/');

        return new Uri(uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? _bucketSSlRoot : _bucketRoot, signedPart);
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

}
