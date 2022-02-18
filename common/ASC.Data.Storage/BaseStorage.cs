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

namespace ASC.Data.Storage;

public abstract class BaseStorage : IDataStore
{
    public IQuotaController QuotaController { get; set; }
    public virtual bool IsSupportInternalUri => true;
    public virtual bool IsSupportedPreSignedUri => true;
    public virtual bool IsSupportChunking => false;
    internal string Modulename { get; set; }
    internal DataList DataList { get; set; }
    internal string Tenant { get; set; }
    internal Dictionary<string, TimeSpan> DomainsExpires { get; set; }
        = new Dictionary<string, TimeSpan>();
    protected ILog Logger { get; set; }

    protected readonly TempStream TempStream;
    protected readonly TenantManager TenantManager;
    protected readonly PathUtils TpathUtils;
    protected readonly EmailValidationKeyProvider TemailValidationKeyProvider;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly IOptionsMonitor<ILog> Options;
    protected readonly IHttpClientFactory ClientFactory;

    public BaseStorage(
        TempStream tempStream,
        TenantManager tenantManager,
        PathUtils pathUtils,
        EmailValidationKeyProvider emailValidationKeyProvider,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<ILog> options,
        IHttpClientFactory clientFactory)
    {

        TempStream = tempStream;
        TenantManager = tenantManager;
        TpathUtils = pathUtils;
        TemailValidationKeyProvider = emailValidationKeyProvider;
        Options = options;
        ClientFactory = clientFactory;
        Logger = options.CurrentValue;
        HttpContextAccessor = httpContextAccessor;
    }

    public TimeSpan GetExpire(string domain)
    {
        return DomainsExpires.ContainsKey(domain) ? DomainsExpires[domain] : DomainsExpires[string.Empty];
    }

    public Uri GetUri(string path)
    {
        return GetUri(string.Empty, path);
    }

    public Uri GetUri(string domain, string path)
    {
        return GetPreSignedUri(domain, path, TimeSpan.MaxValue, null);
    }

    public Uri GetPreSignedUri(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (string.IsNullOrEmpty(Tenant) && IsSupportInternalUri)
        {
            return GetInternalUri(domain, path, expire, headers);
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
            var currentTenant = TenantManager.GetCurrentTenant(false);
            if (currentTenant != null)
            {
                currentTenantId = currentTenant.TenantId;
            }
            else if (!TenantPath.TryGetTenant(Tenant, out currentTenantId))
            {
                currentTenantId = 0;
            }

            var auth = TemailValidationKeyProvider.GetEmailKey(currentTenantId, path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + "." + headerAttr + "." + expireString);
            query = $"{(path.IndexOf('?') >= 0 ? "&" : "?")}{Constants.QueryExpire}={expireString}&{Constants.QueryAuth}={auth}";
        }

        if (!string.IsNullOrEmpty(headerAttr))
        {
            query += $"{(query.IndexOf('?') >= 0 ? "&" : "?")}{Constants.QueryHeader}={HttpUtility.UrlEncode(headerAttr)}";
        }

        var tenant = Tenant.Trim('/');
        var vpath = TpathUtils.ResolveVirtualPath(Modulename, domain);
        vpath = TpathUtils.ResolveVirtualPath(vpath, false);
        vpath = string.Format(vpath, tenant);
        var virtualPath = new Uri(vpath + "/", UriKind.RelativeOrAbsolute);

        var uri = virtualPath.IsAbsoluteUri ?
                      new MonoUri(virtualPath, virtualPath.LocalPath.TrimEnd('/') + EnsureLeadingSlash(path.Replace('\\', '/')) + query) :
                      new MonoUri(virtualPath.ToString().TrimEnd('/') + EnsureLeadingSlash(path.Replace('\\', '/')) + query, UriKind.Relative);

        return uri;
    }

    public virtual Uri GetInternalUri(string domain, string path, TimeSpan expire, IEnumerable<string> headers)
    {
        return null;
    }

    public abstract Stream GetReadStream(string domain, string path);

    public abstract Stream GetReadStream(string domain, string path, int offset);

    public abstract Task<Stream> GetReadStreamAsync(string domain, string path, int offset);

    public abstract Uri Save(string domain, string path, Stream stream);

    public abstract Uri Save(string domain, string path, Stream stream, ACL acl);

    public Uri Save(string domain, string path, Stream stream, string attachmentFileName)
    {
        if (!string.IsNullOrEmpty(attachmentFileName))
        {
            return SaveWithAutoAttachment(domain, path, stream, attachmentFileName);
        }

        return Save(domain, path, stream);
    }

    protected abstract Uri SaveWithAutoAttachment(string domain, string path, Stream stream, string attachmentFileName);

    public abstract Uri Save(string domain, string path, Stream stream, string contentType,
                             string contentDisposition);

    public abstract Uri Save(string domain, string path, Stream stream, string contentEncoding, int cacheDays);

    #region chunking

    public virtual string InitiateChunkedUpload(string domain, string path)
    {
        throw new NotImplementedException();
    }

    public virtual string UploadChunk(string domain, string path, string uploadId, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
    {
        throw new NotImplementedException();
    }

    public virtual Uri FinalizeChunkedUpload(string domain, string path, string uploadId, Dictionary<int, string> eTags)
    {
        throw new NotImplementedException();
    }

    public virtual void AbortChunkedUpload(string domain, string path, string uploadId)
    {
        throw new NotImplementedException();
    }

    #endregion

    public abstract void Delete(string domain, string path);

    public abstract void DeleteFiles(string domain, string folderPath, string pattern, bool recursive);

    public abstract void DeleteFiles(string domain, List<string> paths);

    public abstract void DeleteFiles(string domain, string folderPath, DateTime fromDate, DateTime toDate);

    public abstract void MoveDirectory(string srcdomain, string srcdir, string newdomain, string newdir);

    public abstract Uri Move(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true);

    public abstract Uri SaveTemp(string domain, out string assignedPath, Stream stream);

    public abstract string[] ListDirectoriesRelative(string domain, string path, bool recursive);

    public abstract string[] ListFilesRelative(string domain, string path, string pattern, bool recursive);

    public abstract bool IsFile(string domain, string path);

    public abstract Task<bool> IsFileAsync(string domain, string path);

    public abstract bool IsDirectory(string domain, string path);

    public abstract void DeleteDirectory(string domain, string path);

    public abstract long GetFileSize(string domain, string path);

    public abstract long GetDirectorySize(string domain, string path);

    public abstract long ResetQuota(string domain);

    public abstract long GetUsedQuota(string domain);

    public abstract Uri Copy(string srcdomain, string path, string newdomain, string newpath);

    public abstract void CopyDirectory(string srcdomain, string dir, string newdomain, string newdir);

    public Stream GetReadStream(string path)
    {
        return GetReadStream(string.Empty, path);
    }

    public Uri Save(string path, Stream stream, string attachmentFileName)
    {
        return Save(string.Empty, path, stream, attachmentFileName);
    }

    public Uri Save(string path, Stream stream)
    {
        return Save(string.Empty, path, stream);
    }

    public void Delete(string path)
    {
        Delete(string.Empty, path);
    }

    public void DeleteFiles(string folderPath, string pattern, bool recursive)
    {
        DeleteFiles(string.Empty, folderPath, pattern, recursive);
    }

    public Uri Move(string srcpath, string newdomain, string newpath)
    {
        return Move(string.Empty, srcpath, newdomain, newpath);
    }

    public Uri SaveTemp(out string assignedPath, Stream stream)
    {
        return SaveTemp(string.Empty, out assignedPath, stream);
    }

    public string[] ListDirectoriesRelative(string path, bool recursive)
    {
        return ListDirectoriesRelative(string.Empty, path, recursive);
    }

    public Uri[] ListFiles(string path, string pattern, bool recursive)
    {
        return ListFiles(string.Empty, path, pattern, recursive);
    }

    public Uri[] ListFiles(string domain, string path, string pattern, bool recursive)
    {
        var filePaths = ListFilesRelative(domain, path, pattern, recursive);
        return Array.ConvertAll(
            filePaths,
            x => GetUri(domain, CrossPlatform.PathCombine(PathUtils.Normalize(path), x)));
    }

    public bool IsFile(string path)
    {
        return IsFile(string.Empty, path);
    }

    public bool IsDirectory(string path)
    {
        return IsDirectory(string.Empty, path);
    }

    public void DeleteDirectory(string path)
    {
        DeleteDirectory(string.Empty, path);
    }

    public long GetFileSize(string path)
    {
        return GetFileSize(string.Empty, path);
    }

    public long GetDirectorySize(string path)
    {
        return GetDirectorySize(string.Empty, path);
    }


    public Uri Copy(string path, string newdomain, string newpath)
    {
        return Copy(string.Empty, path, newdomain, newpath);
    }

    public void CopyDirectory(string dir, string newdomain, string newdir)
    {
        CopyDirectory(string.Empty, dir, newdomain, newdir);
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

    public abstract string SavePrivate(string domain, string path, Stream stream, DateTime expires);

    public abstract void DeleteExpired(string domain, string path, TimeSpan oldThreshold);

    public abstract string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize,
                                         string contentType, string contentDisposition, string submitLabel);

    public abstract string GetUploadedUrl(string domain, string directoryPath);
    public abstract string GetUploadUrl();

    public abstract string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType,
                                         string contentDisposition);

    internal void QuotaUsedAdd(string domain, long size, bool quotaCheckFileSize = true)
    {
        if (QuotaController != null)
        {
            QuotaController.QuotaUsedAdd(Modulename, domain, DataList.GetData(domain), size, quotaCheckFileSize);
        }
    }

    internal void QuotaUsedDelete(string domain, long size)
    {
        if (QuotaController != null)
        {
            QuotaController.QuotaUsedDelete(Modulename, domain, DataList.GetData(domain), size);
        }
    }

    internal static string EnsureLeadingSlash(string str)
    {
        return "/" + str.TrimStart('/');
    }

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
