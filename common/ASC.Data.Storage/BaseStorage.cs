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

namespace ASC.Data.Storage;

public abstract class BaseStorage : IDataStore
{
    public IQuotaController QuotaController { get; set; }
    public virtual bool IsSupportInternalUri => true;
    public virtual bool IsSupportedPreSignedUri => true;
    public virtual bool IsSupportChunking => false;
    internal string Modulename { get; set; }
    internal bool Cache { get; set; }
    internal DataList DataList { get; set; }
    internal string Tenant { get; set; }
    internal Dictionary<string, TimeSpan> DomainsExpires { get; set; }
        = new Dictionary<string, TimeSpan>();
    protected ILogger Logger { get; set; }

    protected readonly TempStream _tempStream;
    protected readonly TenantManager _tenantManager;
    protected readonly PathUtils _tpathUtils;
    protected readonly EmailValidationKeyProvider _temailValidationKeyProvider;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILoggerProvider _options;
    protected readonly IHttpClientFactory _clientFactory;

    private readonly TenantQuotaFeatureStatHelper _tenantQuotaFeatureStatHelper;
    private readonly QuotaSocketManager _quotaSocketManager;

    public BaseStorage(
        TempStream tempStream,
        TenantManager tenantManager,
        PathUtils pathUtils,
        EmailValidationKeyProvider emailValidationKeyProvider,
        IHttpContextAccessor httpContextAccessor,
        ILoggerProvider options,
        ILogger logger,
        IHttpClientFactory clientFactory,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper,
        QuotaSocketManager quotaSocketManager)
    {

        _tempStream = tempStream;
        _tenantManager = tenantManager;
        _tpathUtils = pathUtils;
        _temailValidationKeyProvider = emailValidationKeyProvider;
        _options = options;
        _clientFactory = clientFactory;
        Logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _tenantQuotaFeatureStatHelper = tenantQuotaFeatureStatHelper;
        _quotaSocketManager = quotaSocketManager;
    }

    public TimeSpan GetExpire(string domain)
    {
        return DomainsExpires.ContainsKey(domain) ? DomainsExpires[domain] : DomainsExpires[string.Empty];
    }

    public async Task<Uri> GetUriAsync(string path)
    {
        return await GetUriAsync(string.Empty, path);
    }

    public async Task<Uri> GetUriAsync(string domain, string path)
    {
        return await GetPreSignedUriAsync(domain, path, TimeSpan.MaxValue, null);
    }

    public async Task<Uri> GetPreSignedUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (string.IsNullOrEmpty(Tenant) && IsSupportInternalUri)
        {
            return await GetInternalUriAsync(domain, path, expire, headers);
        }

        var headerAttr = string.Empty;
        if (headers != null)
        {
            headerAttr = string.Join("&", headers.Select(HttpUtility.UrlEncode));
        }

        if (expire == TimeSpan.Zero || expire == TimeSpan.MinValue || expire == TimeSpan.MaxValue)
        {
            expire = GetExpire(domain);
        }

        var query = string.Empty;
        if (expire != TimeSpan.Zero && expire != TimeSpan.MinValue && expire != TimeSpan.MaxValue)
        {
            var expireString = expire.TotalMinutes.ToString(CultureInfo.InvariantCulture);

            int currentTenantId;
            var currentTenant = await _tenantManager.GetCurrentTenantAsync(false);
            if (currentTenant != null)
            {
                currentTenantId = currentTenant.Id;
            }
            else if (!TenantPath.TryGetTenant(Tenant, out currentTenantId))
            {
                currentTenantId = 0;
            }

            var auth = _temailValidationKeyProvider.GetEmailKey(currentTenantId, path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + "." + headerAttr + "." + expireString);
            query = $"{(path.IndexOf('?') >= 0 ? "&" : "?")}{Constants.QueryExpire}={expireString}&{Constants.QueryAuth}={auth}";
        }

        if (!string.IsNullOrEmpty(headerAttr))
        {
            query += $"{(query.IndexOf('?') >= 0 ? "&" : "?")}{Constants.QueryHeader}={HttpUtility.UrlEncode(headerAttr)}";
        }

        var tenant = Tenant.Trim('/');
        var vpath = _tpathUtils.ResolveVirtualPath(Modulename, domain);
        vpath = _tpathUtils.ResolveVirtualPath(vpath, false);
        vpath = string.Format(vpath, tenant);
        var virtualPath = new Uri(vpath + "/", UriKind.RelativeOrAbsolute);

        var uri = virtualPath.IsAbsoluteUri ?
                      new MonoUri(virtualPath, virtualPath.LocalPath.TrimEnd('/') + EnsureLeadingSlash(path.Replace('\\', '/')) + query) :
                      new MonoUri(virtualPath.ToString().TrimEnd('/') + EnsureLeadingSlash(path.Replace('\\', '/')) + query, UriKind.Relative);

        return uri;
    }

    public virtual Task<Uri> GetInternalUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
    {
        return Task.FromResult<Uri>(null);
    }

    public abstract Task<Stream> GetReadStreamAsync(string domain, string path);

    public abstract Task<Stream> GetReadStreamAsync(string domain, string path, long offset);

    public abstract Task<Uri> SaveAsync(string domain, string path, Stream stream);
    public abstract Task<Uri> SaveAsync(string domain, string path, Stream stream, ACL acl);

    public async Task<Uri> SaveAsync(string domain, string path, Stream stream, string attachmentFileName)
    {
        if (!string.IsNullOrEmpty(attachmentFileName))
        {
            return await SaveWithAutoAttachmentAsync(domain, path, stream, attachmentFileName);
        }
        return await SaveAsync(domain, path, stream);
    }

    protected abstract Task<Uri> SaveWithAutoAttachmentAsync(string domain, string path, Stream stream, string attachmentFileName);


    public abstract Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType,
                            string contentDisposition);
    public abstract Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentEncoding, int cacheDays);

    #region chunking

    public virtual Task<string> InitiateChunkedUploadAsync(string domain, string path)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string> UploadChunkAsync(string domain, string path, string uploadId, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
    {
        throw new NotImplementedException();
    }

    public virtual Task<Uri> FinalizeChunkedUploadAsync(string domain, string path, string uploadId, Dictionary<int, string> eTags)
    {
        throw new NotImplementedException();
    }

    public virtual Task AbortChunkedUploadAsync(string domain, string path, string uploadId)
    {
        throw new NotImplementedException();
    }

    public virtual IDataWriteOperator CreateDataWriteOperator(
            CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder)
    {
        return new ChunkZipWriteOperator(_tempStream, chunkedUploadSession, sessionHolder);
    }

    #endregion

    public abstract Task DeleteAsync(string domain, string path);
    public abstract Task DeleteFilesAsync(string domain, string folderPath, string pattern, bool recursive);
    public abstract Task DeleteFilesAsync(string domain, List<string> paths);
    public abstract Task DeleteFilesAsync(string domain, string folderPath, DateTime fromDate, DateTime toDate);
    public abstract Task MoveDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir);
    public abstract Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true);
    public abstract Task<(Uri, string)> SaveTempAsync(string domain, Stream stream);
    public abstract IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string domain, string path, bool recursive);
    public abstract IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive);

    public abstract Task<bool> IsFileAsync(string domain, string path);
    public abstract Task<bool> IsDirectoryAsync(string domain, string path);
    public abstract Task DeleteDirectoryAsync(string domain, string path);
    public abstract Task<long> GetFileSizeAsync(string domain, string path);
    public abstract Task<long> GetDirectorySizeAsync(string domain, string path);
    public abstract Task<long> ResetQuotaAsync(string domain);
    public abstract Task<long> GetUsedQuotaAsync(string domain);
    public abstract Task<Uri> CopyAsync(string srcdomain, string path, string newdomain, string newpath);
    public abstract Task CopyDirectoryAsync(string srcdomain, string dir, string newdomain, string newdir);

    public async Task<Stream> GetReadStreamAsync(string path)
    {
        return await GetReadStreamAsync(string.Empty, path);
    }

    public async Task<Uri> SaveAsync(string path, Stream stream, string attachmentFileName)
    {
        return await SaveAsync(string.Empty, path, stream, attachmentFileName);
    }

    public async Task<Uri> SaveAsync(string path, Stream stream)
    {
        return await SaveAsync(string.Empty, path, stream);
    }

    public async Task DeleteAsync(string path)
    {
        await DeleteAsync(string.Empty, path);
    }

    public async Task DeleteFilesAsync(string folderPath, string pattern, bool recursive)
    {
        await DeleteFilesAsync(string.Empty, folderPath, pattern, recursive);
    }

    public async Task<Uri> MoveAsync(string srcpath, string newdomain, string newpath)
    {
        return await MoveAsync(string.Empty, srcpath, newdomain, newpath);
    }

    public async Task<(Uri, string)> SaveTempAsync(Stream stream)
    {
        return await SaveTempAsync(string.Empty, stream);
    }

    public IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string path, bool recursive)
    {
        return ListDirectoriesRelativeAsync(string.Empty, path, recursive);
    }

    public IAsyncEnumerable<Uri> ListFilesAsync(string path, string pattern, bool recursive)
    {
        return ListFilesAsync(string.Empty, path, pattern, recursive);
    }

    public async IAsyncEnumerable<Uri> ListFilesAsync(string domain, string path, string pattern, bool recursive)
    {
        var filePaths = ListFilesRelativeAsync(domain, path, pattern, recursive);

        await foreach (var paths in filePaths)
        {
            yield return await GetUriAsync(domain, CrossPlatform.PathCombine(PathUtils.Normalize(path), paths));
        }
    }

    public async Task<bool> IsFileAsync(string path)
    {
        return await IsFileAsync(string.Empty, path);
    }

    public async Task<bool> IsDirectoryAsync(string path)
    {
        return await IsDirectoryAsync(string.Empty, path);
    }

    public async Task DeleteDirectoryAsync(string path)
    {
        await DeleteDirectoryAsync(string.Empty, path);
    }

    public async Task<long> GetFileSizeAsync(string path)
    {
        return await GetFileSizeAsync(string.Empty, path);
    }

    public async Task<long> GetDirectorySizeAsync(string path)
    {
        return await GetDirectorySizeAsync(string.Empty, path);
    }

    public async Task<Uri> CopyAsync(string path, string newdomain, string newpath)
    {
        return await CopyAsync(string.Empty, path, newdomain, newpath);
    }

    public async Task CopyDirectoryAsync(string dir, string newdomain, string newdir)
    {
        await CopyDirectoryAsync(string.Empty, dir, newdomain, newdir);
    }

    public virtual IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
    {
        return this;
    }

    public IDataStore SetQuotaController(IQuotaController controller)
    {
        QuotaController = controller;

        return this;
    }

    public abstract Task<string> SavePrivateAsync(string domain, string path, Stream stream, DateTime expires);
    public abstract Task DeleteExpiredAsync(string domain, string path, TimeSpan oldThreshold);

    public abstract string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize,
                                         string contentType, string contentDisposition, string submitLabel);

    public abstract string GetUploadUrl();

    public abstract string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType,
                                         string contentDisposition);

    internal async Task QuotaUsedAddAsync(string domain, long size, bool quotaCheckFileSize = true)
    {
        if (QuotaController != null)
        {
            await QuotaController.QuotaUsedAddAsync(Modulename, domain, DataList.GetData(domain), size, quotaCheckFileSize);
            var(name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<MaxTotalSizeFeature, long>();
            _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
        }
    }

    internal async Task QuotaUsedDeleteAsync(string domain, long size)
    {
        if (QuotaController != null)
        {
            await QuotaController.QuotaUsedDeleteAsync(Modulename, domain, DataList.GetData(domain), size);
            var (name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<MaxTotalSizeFeature, long>();
            _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
        }
    }

    internal static string EnsureLeadingSlash(string str)
    {
        return "/" + str.TrimStart('/');
    }

    public abstract Task<string> GetFileEtagAsync(string domain, string path);
    
    internal class MonoUri : Uri
    {
        public MonoUri(Uri baseUri, string relativeUri)
            : base(baseUri, relativeUri) { }

        public MonoUri(string uriString, UriKind uriKind)
            : base(uriString, uriKind) { }

        public override string ToString()
        {
            var s = base.ToString();
            if (WorkContext.IsMono && s.StartsWith(UriSchemeFile + SchemeDelimiter))
            {
                return s.Substring(7);
            }

            return s;
        }
    }
}
