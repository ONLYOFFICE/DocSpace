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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Data.Storage.Configuration;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Data.Storage.RackspaceCloud
{
    [Scope]
    public class RackspaceCloudStorage : BaseStorage
    {
        private string _region;
        private string _private_container;
        private string _public_container;
        private readonly List<string> _domains = new List<string>();
        private Dictionary<string, ACL> _domainsAcl;
        private ACL _moduleAcl;
        private string _subDir;
        private string _username;
        private string _apiKey;
        private bool _lowerCasing = true;
        private Uri _cname;
        private Uri _cnameSSL;

        private readonly ILog _logger;
            
        public RackspaceCloudStorage(
            TempPath tempPath,
            TempStream tempStream,
            TenantManager tenantManager,
            PathUtils pathUtils,
            EmailValidationKeyProvider emailValidationKeyProvider,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory httpClient)
            : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, options, httpClient)
        {
            _logger = options.Get("ASC.Data.Storage.Rackspace.RackspaceCloudStorage");
            TempPath = tempPath;
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

        private CloudFilesProvider GetClient()
        {
            var cloudIdentity = new CloudIdentity()
            {
                Username = _username,
                APIKey = _apiKey
            };

            return new CloudFilesProvider(cloudIdentity);
        }

        public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
        {
            _tenant = tenant;

            if (moduleConfig != null)
            {
                _modulename = moduleConfig.Name;
                _dataList = new DataList(moduleConfig);
                _domains.AddRange(moduleConfig.Domain.Select(x => $"{x.Name}/"));
                _domainsExpires = moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).ToDictionary(x => x.Name, y => y.Expires);
                _domainsExpires.Add(string.Empty, moduleConfig.Expires);
                _domainsAcl = moduleConfig.Domain.ToDictionary(x => x.Name, y => y.Acl);
                _moduleAcl = moduleConfig.Acl;
            }
            else
            {
                _modulename = string.Empty;
                _dataList = null;
                _domainsExpires = new Dictionary<string, TimeSpan> { { string.Empty, TimeSpan.Zero } };
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
                throw new ArgumentException("_public_container");

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

            var client = GetClient();

            var accounMetaData = client.GetAccountMetaData(_region);
            string secretKey;
            if (accounMetaData.TryGetValue("Temp-Url-Key", out secretKey))
            {

            }
            else
            {
                secretKey = Common.Utils.RandomString.Generate(64);
                accounMetaData.Add("Temp-Url-Key", secretKey);
                client.UpdateAccountMetadata(accounMetaData, _region);
            }

            return client.CreateTemporaryPublicUri(
                                                        JSIStudios.SimpleRESTServices.Client.HttpMethod.GET,
                                                        _private_container,
                                                        MakePath(domain, path),
                                                        secretKey,
                                                        DateTime.UtcNow.Add(expire),
                                                        _region);
        }

        private Uri GetUriShared(string domain, string path)
        {
            return new Uri(string.Format("{0}{1}", SecureHelper.IsSecure(HttpContextAccessor?.HttpContext, Options) ? _cnameSSL : _cname, MakePath(domain, path)));
        }

        public override Stream GetReadStream(string domain, string path)
        {
            return GetReadStream(domain, path, 0);
        }

        public override Stream GetReadStream(string domain, string path, int offset)
        {
            var outputStream = TempStream.Create();

            var client = GetClient();

            client.GetObject(_private_container, MakePath(domain, path), outputStream);

            outputStream.Position = 0;

            if (0 < offset) outputStream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

            return outputStream;
        }

        public override Task<Stream> GetReadStreamAsync(string domain, string path, int offset)
        {
            return Task.FromResult(GetReadStream(domain, path, offset));
        }

        public override Uri Save(string domain, string path, Stream stream)
        {
            return Save(domain, path, stream, string.Empty, string.Empty);
        }

        public override Uri Save(string domain, string path, Stream stream, ACL acl)
        {
            return Save(domain, path, stream, null, null, acl);
        }

        protected override Uri SaveWithAutoAttachment(string domain, string path, Stream stream, string attachmentFileName)
        {
            var contentDisposition = $"attachment; filename={HttpUtility.UrlPathEncode(attachmentFileName)};";
            if (attachmentFileName.Any(c => c >= 0 && c <= 127))
            {
                contentDisposition = $"attachment; filename*=utf-8''{HttpUtility.UrlPathEncode(attachmentFileName)};";
            }

            return Save(domain, path, stream, null, contentDisposition);
        }

        public override Uri Save(string domain, string path, Stream stream, string contentType, string contentDisposition)
        {
            return Save(domain, path, stream, contentType, contentDisposition, ACL.Auto);
        }

        public override Uri Save(string domain, string path, Stream stream, string contentEncoding, int cacheDays)
        {
            return Save(domain, path, stream, string.Empty, string.Empty, ACL.Auto, contentEncoding, cacheDays);
        }

        public Uri Save(string domain, string path, Stream stream, string contentType,
                              string contentDisposition, ACL acl, string contentEncoding = null, int cacheDays = 5,
            DateTime? deleteAt = null, long? deleteAfter = null)
        {
            var buffered = TempStream.GetBuffered(stream);

            if (QuotaController != null)
            {
                QuotaController.QuotaUsedCheck(buffered.Length);
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
                customHeaders.Add("Content-Encoding", contentEncoding);

            var cannedACL = acl == ACL.Auto ? GetDomainACL(domain) : ACL.Read;

            if (cannedACL == ACL.Read)
            {
                try
                {

                    using (var emptyStream = TempStream.Create())
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
                    _logger.InfoFormat("The invalidation {0} failed", _public_container + "/" + MakePath(domain, path));
                    _logger.Error(exp);
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

            QuotaUsedAdd(domain, buffered.Length);

            return GetUri(domain, path);

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

        public override void Delete(string domain, string path)
        {
            var client = GetClient();
            MakePath(domain, path);
            var size = GetFileSize(domain, path);

            client.DeleteObject(_private_container, MakePath(domain, path));

            QuotaUsedDelete(domain, size);

        }

        public override void DeleteFiles(string domain, string folderPath, string pattern, bool recursive)
        {
            var client = GetClient();

            var files = client.ListObjects(_private_container, null, null, null, MakePath(domain, folderPath), _region)
                              .Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x.Name)));

            if (!files.Any()) return;
            
            foreach(var file in files)
            {
                client.DeleteObject(_private_container, file.Name);
            }

            if (QuotaController != null)
            {
                QuotaUsedDelete(domain, files.Select(x => x.Bytes).Sum());
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

            var client = GetClient();

            keysToDel.ForEach(x => client.DeleteObject(_private_container, x));

            if (quotaUsed > 0)
            {
                QuotaUsedDelete(domain, quotaUsed);
            }
        }

        public override void DeleteFiles(string domain, string folderPath, DateTime fromDate, DateTime toDate)
        {
            var client = GetClient();

            var files = client.ListObjects(_private_container, null, null, null, MakePath(domain, folderPath), _region)
                               .Where(x => x.LastModified >= fromDate && x.LastModified <= toDate);

            if (!files.Any()) return;

            foreach(var file in files)
            {
                client.DeleteObject(_private_container, file.Name);
            }

            if (QuotaController != null)
            {
                QuotaUsedDelete(domain, files.Select(x => x.Bytes).Sum());
            }
        }

        public override void MoveDirectory(string srcdomain, string srcdir, string newdomain, string newdir)
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
        }

        public override Uri Move(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
        {
            var srcKey = MakePath(srcdomain, srcpath);
            var dstKey = MakePath(newdomain, newpath);
            var size = GetFileSize(srcdomain, srcpath);

            var client = GetClient();

            client.CopyObject(_private_container, srcKey, _private_container, dstKey);

            Delete(srcdomain, srcpath);

            QuotaUsedDelete(srcdomain, size);
            QuotaUsedAdd(newdomain, size, quotaCheckFileSize);

            return GetUri(newdomain, newpath);
        }

        public override Uri SaveTemp(string domain, out string assignedPath, Stream stream)
        {
            assignedPath = Guid.NewGuid().ToString();

            return Save(domain, assignedPath, stream);
        }

        public override string[] ListDirectoriesRelative(string domain, string path, bool recursive)
        {
            var client = GetClient();

            return client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region)
                  .Select(x => x.Name.Substring(MakePath(domain, path + "/").Length)).ToArray();
        }

        public override string[] ListFilesRelative(string domain, string path, string pattern, bool recursive)
        {
            var client = GetClient();

            var paths = client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region).Select(x => x.Name);

            return paths
                .Where(x => Wildcard.IsMatch(pattern, Path.GetFileName(x)))
                .Select(x => x.Substring(MakePath(domain, path + "/").Length).TrimStart('/')).ToArray();
        }

        public override bool IsFile(string domain, string path)
        {
            var client = GetClient();
            var objects = client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region);

            return objects.Any();
        }

        public override Task<bool> IsFileAsync(string domain, string path)
        {
            return Task.FromResult(IsFile(domain, path));
        }


        public override bool IsDirectory(string domain, string path)
        {
            return IsFile(domain, path);
        }

        public override void DeleteDirectory(string domain, string path)
        {
            var client = GetClient();

            var objToDel = client.ListObjects(_private_container, null, null, null, MakePath(domain, path), _region);

            foreach (var obj in objToDel)
            {
                client.DeleteObject(_private_container, obj.Name);
                QuotaUsedDelete(domain, obj.Bytes);
            }
        }

        public override long GetFileSize(string domain, string path)
        {
            var client = GetClient();

            var obj = client
                          .ListObjects(_private_container, null, null, null, MakePath(domain, path));

            if (obj.Any())
                return obj.Single().Bytes;

            return 0;
        }

        public override long GetDirectorySize(string domain, string path)
        {
            var client = GetClient();

            var objToDel = client
                          .ListObjects(_private_container, null, null, null, MakePath(domain, path));

            long result = 0;

            foreach (var obj in objToDel)
            {
                result += obj.Bytes;
            }

            return result;
        }

        public override long ResetQuota(string domain)
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

                QuotaController.QuotaUsedSet(_modulename, domain, _dataList.GetData(domain), size);

                return size;
            }

            return 0;
        }

        public override long GetUsedQuota(string domain)
        {
            var client = GetClient();

            var objects = client
                          .ListObjects(_private_container, null, null, null, MakePath(domain, string.Empty), _region);

            long result = 0;

            foreach (var obj in objects)
            {
                result += obj.Bytes;
            }

            return result;
        }

        public override Uri Copy(string srcdomain, string path, string newdomain, string newpath)
        {
            var srcKey = MakePath(srcdomain, path);
            var dstKey = MakePath(newdomain, newpath);
            var size = GetFileSize(srcdomain, path);
            var client = GetClient();

            client.CopyObject(_private_container, srcKey, _private_container, dstKey);

            QuotaUsedAdd(newdomain, size);

            return GetUri(newdomain, newpath);
        }

        public override void CopyDirectory(string srcdomain, string dir, string newdomain, string newdir)
        {
            var srckey = MakePath(srcdomain, dir);
            var dstkey = MakePath(newdomain, newdir);
            var client = GetClient();

            var files = client.ListObjects(_private_container, null, null, null, srckey, _region);

            foreach (var file in files)
            {
                client.CopyObject(_private_container, file.Name, _private_container, file.Name.Replace(srckey, dstkey));

                QuotaUsedAdd(newdomain, file.Bytes);
            }
        }

        public override string SavePrivate(string domain, string path, Stream stream, DateTime expires)
        {
            var uri = Save(domain, path, stream, "application/octet-stream", "attachment", ACL.Auto, null, 5, expires);

            return uri.ToString();
        }

        public override void DeleteExpired(string domain, string path, TimeSpan oldThreshold)
        {
            // When the file is saved is specified life time
        }

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

        #region chunking

        public override string InitiateChunkedUpload(string domain, string path)
        {
            return TempPath.GetTempFileName();
        }

        public override string UploadChunk(string domain, string path, string filePath, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
        {
            const int BufferSize = 4096;

            var mode = chunkNumber == 0 ? FileMode.Create : FileMode.Append;

            using (var fs = new FileStream(filePath, mode))
            {
                var buffer = new byte[BufferSize];
                int readed;
                while ((readed = stream.Read(buffer, 0, BufferSize)) != 0)
                {
                    fs.Write(buffer, 0, readed);
                }
            }

            return string.Format("{0}_{1}", chunkNumber, filePath);
        }

        public override void AbortChunkedUpload(string domain, string path, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public override Uri FinalizeChunkedUpload(string domain, string path, string filePath, Dictionary<int, string> eTags)
        {
            var client = GetClient();

            client.CreateObjectFromFile(_private_container, filePath, MakePath(domain, path));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (QuotaController != null)
            {
                var size = GetFileSize(domain, path);

                QuotaUsedAdd(domain, size);
            }

            return GetUri(domain, path);
        }

        public override bool IsSupportChunking { get { return true; } }

        public TempPath TempPath { get; }

        #endregion
    }
}
