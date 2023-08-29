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

namespace ASC.Data.Storage.RackspaceCloud;

[Scope]
public class RackspaceCloudStorage : BaseStorage
{
    public override bool IsSupportChunking => true;
    public TempPath TempPath { get; }

    private string _region;
    private string _private_container;
    private string _public_container;
    private Dictionary<string, ACL> _domainsAcl;
    private ACL _moduleAcl;
    private string _subDir;
    private string _username;
    private string _apiKey;
    private bool _lowerCasing = true;
    private Uri _cname;
    private Uri _cnameSSL;
    private readonly List<string> _domains = new List<string>();
    private readonly ILogger<RackspaceCloudStorage> _logger;

    public RackspaceCloudStorage(
        TempPath tempPath,
        TempStream tempStream,
        TenantManager tenantManager,
        PathUtils pathUtils,
        EmailValidationKeyProvider emailValidationKeyProvider,
        IHttpContextAccessor httpContextAccessor,
        ILoggerProvider options,
        ILogger<RackspaceCloudStorage> logger,
        IHttpClientFactory httpClient,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper,
        QuotaSocketManager quotaSocketManager)
        : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, options, logger, httpClient, tenantQuotaFeatureStatHelper, quotaSocketManager)
    {
        _logger = logger;
        TempPath = tempPath;
    }

    public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
    {
        Tenant = tenant;

        if (moduleConfig != null)
        {
            Modulename = moduleConfig.Name;
            DataList = new DataList(moduleConfig);
            _domains.AddRange(moduleConfig.Domain.Select(x => $"{x.Name}/"));
            DomainsExpires = moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).ToDictionary(x => x.Name, y => y.Expires);
            DomainsExpires.Add(string.Empty, moduleConfig.Expires);
            _domainsAcl = moduleConfig.Domain.ToDictionary(x => x.Name, y => y.Acl);
            _moduleAcl = moduleConfig.Acl;
        }
        else
        {
            Modulename = string.Empty;
            DataList = null;
            DomainsExpires = new Dictionary<string, TimeSpan> { { string.Empty, TimeSpan.Zero } };
            _domainsAcl = new Dictionary<string, ACL>();
            _moduleAcl = ACL.Auto;
        }


        _private_container = props["private_container"];
        _region = props["region"];
        _apiKey = props["apiKey"];
        _username = props["username"];

        if (props.TryGetValue("lower", out var value))
        {
            bool.TryParse(value, out _lowerCasing);
        }

        props.TryGetValue("subdir", out _subDir);

        _public_container = props["public_container"];

        if (string.IsNullOrEmpty(_public_container))
        {
            throw new ArgumentException("_public_container");
        }

        var client = GetClient();

        var cdnHeaders = client.GetContainerCDNHeader(_public_container, _region);

        _cname = props.ContainsKey("cname") && Uri.IsWellFormedUriString(props["cname"], UriKind.Absolute)
                     ? new Uri(props["cname"], UriKind.Absolute)
                     : new Uri(cdnHeaders.CDNUri);

        _cnameSSL = props.ContainsKey("cnamessl") &&
                         Uri.IsWellFormedUriString(props["cnamessl"], UriKind.Absolute)
                             ? new Uri(props["cnamessl"], UriKind.Absolute)
                             : new Uri(cdnHeaders.CDNSslUri);

        return this;
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

        var client = GetClient();

        var accounMetaData = client.GetAccountMetaData(_region);
        string secretKey;
        if (accounMetaData.TryGetValue("Temp-Url-Key", out secretKey))
        {

        }
        else
        {
            secretKey = RandomString.Generate(64);
            accounMetaData.Add("Temp-Url-Key", secretKey);
            client.UpdateAccountMetadata(accounMetaData, _region);
        }

        return Task.FromResult(client.CreateTemporaryPublicUri(
                                                JSIStudios.SimpleRESTServices.Client.HttpMethod.GET,
                                                _private_container,
                                                MakePath(domain, path),
                                                secretKey,
                                                DateTime.UtcNow.Add(expire),
                                                    _region));
    }

    public override Task<Stream> GetReadStreamAsync(string domain, string path)
    {
        return GetReadStreamAsync(domain, path, 0);
    }

    public override Task<Stream> GetReadStreamAsync(string domain, string path, long offset)
    {
        return GetReadStreamAsync(domain, path, offset);
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream)
    {
        return SaveAsync(domain, path, stream, string.Empty, string.Empty);
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, ACL acl)
    {
        return SaveAsync(domain, path, stream, null, null, acl);
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType, string contentDisposition)
    {
        return SaveAsync(domain, path, stream, contentType, contentDisposition, ACL.Auto);
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentEncoding, int cacheDays)
    {
        return SaveAsync(domain, path, stream, string.Empty, string.Empty, ACL.Auto, contentEncoding, cacheDays);
    }

    private bool EnableQuotaCheck(string domain)
    {
        return (QuotaController != null) && !domain.EndsWith("_temp");
    }

    public async Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                      string contentDisposition, ACL acl, string contentEncoding = null, int cacheDays = 5,
    DateTime? deleteAt = null, long? deleteAfter = null)
    {
        var buffered = _tempStream.GetBuffered(stream);

        if (EnableQuotaCheck(domain))
        {
            await QuotaController.QuotaUsedCheckAsync(buffered.Length);
        }

        var client = GetClient();

        var mime = string.IsNullOrEmpty(contentType)
                             ? MimeMapping.GetMimeMapping(Path.GetFileName(path))
                             : contentType;


        var customHeaders = new Dictionary<string, string>();

        if (cacheDays > 0)
        {
            customHeaders.Add("Cache-Control", string.Format("public, maxage={0}", (int)TimeSpan.FromDays(cacheDays).TotalSeconds));
            customHeaders.Add("Expires", DateTime.UtcNow.Add(TimeSpan.FromDays(cacheDays)).ToString());
        }

        if (deleteAt.HasValue)
        {
            var ts = deleteAt.Value - new DateTime(1970, 1, 1, 0, 0, 0);
            var unixTimestamp = (long)ts.TotalSeconds;

            customHeaders.Add("X-Delete-At", unixTimestamp.ToString());
        }

        if (deleteAfter.HasValue)
        {
            customHeaders.Add("X-Delete-After", deleteAfter.ToString());
        }

        if (!string.IsNullOrEmpty(contentEncoding))
        {
            customHeaders.Add("Content-Encoding", contentEncoding);
        }

        var cannedACL = acl == ACL.Auto ? GetDomainACL(domain) : ACL.Read;

        if (cannedACL == ACL.Read)
        {
            try
            {
                await using (var emptyStream = _tempStream.Create())
                {

                    var headers = new Dictionary<string, string>
                        {
                            { "X-Object-Manifest", $"{_private_container}/{MakePath(domain, path)}" }
                        };
                    // create symlink
                    client.CreateObject(_public_container,
                               emptyStream,
                               MakePath(domain, path),
                               mime,
                               4096,
                               headers,
                               _region
                              );

                    emptyStream.Close();
                }

                client.PurgeObjectFromCDN(_public_container, MakePath(domain, path));
            }
            catch (Exception exp)
            {
                _logger.ErrorInvalidationFailed(_public_container + "/" + MakePath(domain, path), exp);
            }
        }

        stream.Position = 0;

        client.CreateObject(_private_container,
                            stream,
                            MakePath(domain, path),
                            mime,
                            4096,
                            customHeaders,
                            _region
                           );

        await QuotaUsedAddAsync(domain, buffered.Length);

        return await GetUriAsync(domain, path);

    }

    public override async Task DeleteAsync(string domain, string path)
    {
        var client = GetClient();
        MakePath(domain, path);
        var size = await GetFileSizeAsync(domain, path);

        client.DeleteObject(_private_container, MakePath(domain, path));

        await QuotaUsedDeleteAsync(domain, size);

    }

    public override async Task DeleteFilesAsync(string domain, string folderPath, string pattern, bool recursive)
    {
        var client = GetClient();

        var files = client.ListObjects(_private_container, null, null, null, MakePath(domain, folderPath), _region)
                          .Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Name)));

        if (!files.Any())
        {
            return;
        }

        foreach (var file in files)
        {
            client.DeleteObject(_private_container, file.Name);
        }

        if (QuotaController != null)
        {
            await QuotaUsedDeleteAsync(domain, files.Select(x => x.Bytes).Sum());
        }
    }

    public override async Task DeleteFilesAsync(string domain, List<string> paths)
    {
        if (paths.Count == 0)
        {
            return;
        }

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

        if (keysToDel.Count == 0)
        {
            return;
        }

        var client = GetClient();

        keysToDel.ForEach(x => client.DeleteObject(_private_container, x));

        if (quotaUsed > 0)
        {
            await QuotaUsedDeleteAsync(domain, quotaUsed);
        }
    }

    public override async Task DeleteFilesAsync(string domain, string folderPath, DateTime fromDate, DateTime toDate)
    {
        var client = GetClient();

        var files = client.ListObjects(_private_container, null, null, null, MakePath(domain, folderPath), _region)
                           .Where(x => x.LastModified >= fromDate && x.LastModified <= toDate);

        if (!files.Any())
        {
            return;
        }

        foreach (var file in files)
        {
            client.DeleteObject(_private_container, file.Name);
        }

        if (QuotaController != null)
        {
            await QuotaUsedDeleteAsync(domain, files.Select(x => x.Bytes).Sum());
        }
    }

    public override Task MoveDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        var client = GetClient();
        var srckey = MakePath(srcdomain, srcdir);
        var dstkey = MakePath(newdomain, newdir);

        var paths = client.ListObjects(_private_container, null, null, srckey, _region).Select(x => x.Name);

        foreach (var path in paths)
        {
            client.CopyObject(_private_container, path, _private_container, path.Replace(srckey, dstkey));
            client.DeleteObject(_private_container, path);
        }

        return Task.CompletedTask;
    }

    public override async Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
    {
        var srcKey = MakePath(srcdomain, srcpath);
        var dstKey = MakePath(newdomain, newpath);
        var size = await GetFileSizeAsync(srcdomain, srcpath);

        var client = GetClient();

        client.CopyObject(_private_container, srcKey, _private_container, dstKey);

        await DeleteAsync(srcdomain, srcpath);

        await QuotaUsedDeleteAsync(srcdomain, size);
        await QuotaUsedAddAsync(newdomain, size, quotaCheckFileSize);

        return await GetUriAsync(newdomain, newpath);
    }

    public override async Task<(Uri, string)> SaveTempAsync(string domain, Stream stream)
    {
       var assignedPath = Guid.NewGuid().ToString();

        return (await SaveAsync(domain, assignedPath, stream), assignedPath);
    }

    public override IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string domain, string path, bool recursive)
    {
        var client = GetClient();

        return client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region)
                  .Select(x => x.Name.Substring(MakePath(domain, path + "/").Length)).ToAsyncEnumerable();
    }

    public override IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive)
    {
        var client = GetClient();

        var paths = client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region).Select(x => x.Name);

        return paths
            .Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x)))
                .Select(x => x.Substring(MakePath(domain, path + "/").Length).TrimStart('/')).ToAsyncEnumerable();
    }

    public override Task<bool> IsFileAsync(string domain, string path)
    {
        var client = GetClient();
        var objects = client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region);

        var result = objects.Any();
        return Task.FromResult(result);
    }

    public override Task<bool> IsDirectoryAsync(string domain, string path)
    {
        return IsFileAsync(domain, path);
    }

    public override async Task DeleteDirectoryAsync(string domain, string path)
    {
        var client = GetClient();

        var objToDel = client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region);

        foreach (var obj in objToDel)
        {
            client.DeleteObject(_private_container, obj.Name);

            if (QuotaController != null)
            {
                if (string.IsNullOrEmpty(QuotaController.ExcludePattern) ||
                    !Path.GetFileName(obj.Name).StartsWith(QuotaController.ExcludePattern))
                {
                    await QuotaUsedDeleteAsync(domain, obj.Bytes);
                }
            }
        }
    }

    public override Task<long> GetFileSizeAsync(string domain, string path)
    {
        var client = GetClient();

        var obj = client
                      .ListObjects(_private_container, null, null, null, MakePath(domain, path));

        if (obj.Any())
        {
            return Task.FromResult(obj.Single().Bytes);
        }

        return Task.FromResult<long>(0);
    }

    public override Task<long> GetDirectorySizeAsync(string domain, string path)
    {
        var client = GetClient();

        var objToDel = client
                      .ListObjects(_private_container, null, null, null, MakePath(domain, path));

        long result = 0;

        foreach (var obj in objToDel)
        {
            result += obj.Bytes;
        }

        return Task.FromResult(result);
    }

    public override async Task<long> ResetQuotaAsync(string domain)
    {
        var client = GetClient();

        var objects = client
                      .ListObjects(_private_container, null, null, null, MakePath(domain, string.Empty), _region);

        if (QuotaController != null)
        {
            long size = 0;

            foreach (var obj in objects)
            {
                size += obj.Bytes;
            }

            await QuotaController.QuotaUsedSetAsync(Modulename, domain, DataList.GetData(domain), size);

            return size;
        }

        return 0;
    }

    public override Task<long> GetUsedQuotaAsync(string domain)
    {
        var client = GetClient();

        var objects = client
                      .ListObjects(_private_container, null, null, null, MakePath(domain, string.Empty), _region);

        long result = 0;

        foreach (var obj in objects)
        {
            result += obj.Bytes;
        }

        return Task.FromResult(result);
    }

    public override async Task<Uri> CopyAsync(string srcdomain, string path, string newdomain, string newpath)
    {
        var srcKey = MakePath(srcdomain, path);
        var dstKey = MakePath(newdomain, newpath);
        var size = await GetFileSizeAsync(srcdomain, path);
        var client = GetClient();

        client.CopyObject(_private_container, srcKey, _private_container, dstKey);

        await QuotaUsedAddAsync(newdomain, size);

        return await GetUriAsync(newdomain, newpath);
    }

    public override async Task CopyDirectoryAsync(string srcdomain, string dir, string newdomain, string newdir)
    {
        var srckey = MakePath(srcdomain, dir);
        var dstkey = MakePath(newdomain, newdir);
        var client = GetClient();

        var files = client.ListObjects(_private_container, null, null, null, srckey, _region);

        foreach (var file in files)
        {
            client.CopyObject(_private_container, file.Name, _private_container, file.Name.Replace(srckey, dstkey));

            await QuotaUsedAddAsync(newdomain, file.Bytes);
        }
    }

    public override async Task<string> SavePrivateAsync(string domain, string path, Stream stream, DateTime expires)
    {
        var uri = await SaveAsync(domain, path, stream, "application/octet-stream", "attachment", ACL.Auto, null, 5, expires);

        return uri.ToString();
    }

    public override Task DeleteExpiredAsync(string domain, string path, TimeSpan oldThreshold)
    {
        return Task.CompletedTask;
        // When the file is saved is specified life time
    }

    public override string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize, string contentType, string contentDisposition, string submitLabel)
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

    #region chunking

    public override Task<string> InitiateChunkedUploadAsync(string domain, string path)
    {
        return Task.FromResult(TempPath.GetTempFileName());
    }

    public override async Task<string> UploadChunkAsync(string domain, string path, string filePath, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
    {
        const int BufferSize = 4096;

        var mode = chunkNumber == 0 ? FileMode.Create : FileMode.Append;

        await using (var fs = new FileStream(filePath, mode))
        {
            var buffer = new byte[BufferSize];
            int readed;
            while ((readed = await stream.ReadAsync(buffer, 0, BufferSize)) != 0)
            {
                await fs.WriteAsync(buffer, 0, readed);
            }
        }

        return string.Format("{0}_{1}", chunkNumber, filePath);
    }

    public override Task AbortChunkedUploadAsync(string domain, string path, string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    public override async Task<Uri> FinalizeChunkedUploadAsync(string domain, string path, string filePath, Dictionary<int, string> eTags)
    {
        var client = GetClient();

        client.CreateObjectFromFile(_private_container, filePath, MakePath(domain, path));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        if (QuotaController != null)
        {
            var size = await GetFileSizeAsync(domain, path);

            await QuotaUsedAddAsync(domain, size);
        }

        return await GetUriAsync(domain, path);
    }

    #endregion

    protected override Task<Uri> SaveWithAutoAttachmentAsync(string domain, string path, Stream stream, string attachmentFileName)
    {
        var contentDisposition = $"attachment; filename={HttpUtility.UrlPathEncode(attachmentFileName)};";
        if (attachmentFileName.Any(c => c >= 0 && c <= 127))
        {
            contentDisposition = $"attachment; filename*=utf-8''{HttpUtility.UrlPathEncode(attachmentFileName)};";
        }

        return SaveAsync(domain, path, stream, null, contentDisposition);
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

        result = result.Replace("//", "/").TrimStart('/');
        if (_lowerCasing)
        {
            result = result.ToLowerInvariant();
        }

        return result;
    }

    private CloudFilesProvider GetClient()
    {
        var cloudIdentity = new CloudIdentity()
        {
            Username = _username,
            APIKey = _apiKey
        };

        return new CloudFilesProvider(cloudIdentity);
    }


    private Uri GetUriShared(string domain, string path)
    {
        return new Uri(string.Format("{0}{1}", SecureHelper.IsSecure(_httpContextAccessor?.HttpContext, _options) ? _cnameSSL : _cname, MakePath(domain, path)));
    }

    private ACL GetDomainACL(string domain)
    {
        if (GetExpire(domain) != TimeSpan.Zero)
        {
            return ACL.Auto;
        }

        if (_domainsAcl.TryGetValue(domain, out var value))
        {
            return value;
        }
        return _moduleAcl;
    }

    public override Task<string> GetFileEtagAsync(string domain, string path)
    {
        var client = GetClient();
        var prefix = MakePath(domain, path);

        var obj = client.ListObjects(_private_container, null, null, null, prefix, _region).FirstOrDefault();

        var lastModificationDate = obj == null ? throw new FileNotFoundException("File not found" + prefix) : obj.LastModified.UtcDateTime;

        var etag = '"' + lastModificationDate.Ticks.ToString("X8", CultureInfo.InvariantCulture) + '"';

        return Task.FromResult(etag);
    }
}
