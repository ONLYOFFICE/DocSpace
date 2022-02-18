/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

namespace ASC.Data.Storage.DiscStorage;

[Scope]
public class DiscDataStore : BaseStorage
{
    public override bool IsSupportInternalUri => false;
    public override bool IsSupportedPreSignedUri => false;
    public override bool IsSupportChunking => true;

    private readonly Dictionary<string, MappedPath> _mappedPaths = new Dictionary<string, MappedPath>();
    private ICrypt _crypt;
    private EncryptionSettingsHelper _encryptionSettingsHelper;
    private EncryptionFactory _encryptionFactory;

    public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
    {
        Tenant = tenant;
        //Fill map path
        Modulename = moduleConfig.Name;
        DataList = new DataList(moduleConfig);

        foreach (var domain in moduleConfig.Domain)
        {
            _mappedPaths.Add(domain.Name, new MappedPath(TpathUtils, tenant, moduleConfig.AppendTenantId, domain.Path, handlerConfig.GetProperties()));
        }

        //Add default
        _mappedPaths.Add(string.Empty, new MappedPath(TpathUtils, tenant, moduleConfig.AppendTenantId, PathUtils.Normalize(moduleConfig.Path), handlerConfig.GetProperties()));

        //Make expires
        DomainsExpires =
            moduleConfig.Domain.Where(x => x.Expires != TimeSpan.Zero).
                ToDictionary(x => x.Name,
                             y => y.Expires);
        DomainsExpires.Add(string.Empty, moduleConfig.Expires);
        var settings = moduleConfig.DisabledEncryption ? new EncryptionSettings() : _encryptionSettingsHelper.Load();
        _crypt = _encryptionFactory.GetCrypt(moduleConfig.Name, settings);

        return this;
    }

    public DiscDataStore(
        TempStream tempStream,
        TenantManager tenantManager,
        PathUtils pathUtils,
        EmailValidationKeyProvider emailValidationKeyProvider,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<ILog> options,
        EncryptionSettingsHelper encryptionSettingsHelper,
            EncryptionFactory encryptionFactory,
            IHttpClientFactory clientFactory)
            : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, options, clientFactory)
    {
        _encryptionSettingsHelper = encryptionSettingsHelper;
        _encryptionFactory = encryptionFactory;
    }

    public string GetPhysicalPath(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var pathMap = GetPath(domain);

        return (pathMap.PhysicalPath + EnsureLeadingSlash(path)).Replace('\\', '/');
    }

    public override Stream GetReadStream(string domain, string path)
    {
        return GetReadStream(domain, path, true);
    }

    public Stream GetReadStream(string domain, string path, bool withDecription)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            return withDecription ? _crypt.GetReadStream(target) : File.OpenRead(target);
        }

        throw new FileNotFoundException("File not found", Path.GetFullPath(target));
    }

    public override Task<Stream> GetReadStreamAsync(string domain, string path, int offset)
    {
        return Task.FromResult(GetReadStream(domain, path, offset));
    }

    public override Stream GetReadStream(string domain, string path, int offset)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            var stream = _crypt.GetReadStream(target);
            if (0 < offset && stream.CanSeek)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }

            return stream;
        }

        throw new FileNotFoundException("File not found", Path.GetFullPath(target));
    }


    public override Uri Save(string domain, string path, Stream stream, string contentType, string contentDisposition)
    {
        return Save(domain, path, stream);
    }

    public override Uri Save(string domain, string path, Stream stream, string contentEncoding, int cacheDays)
    {
        return Save(domain, path, stream);

    }

    public override Uri Save(string domain, string path, Stream stream)
    {
        Logger.Debug("Save " + path);

        var buffered = TempStream.GetBuffered(stream);
        if (QuotaController != null)
        {
            QuotaController.QuotaUsedCheck(buffered.Length);
        }

        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (buffered == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        //Try seek to start
        if (buffered.CanSeek)
        {
            buffered.Seek(0, SeekOrigin.Begin);
        }

        //Lookup domain
        var target = GetTarget(domain, path);
        CreateDirectory(target);
        //Copy stream

        //optimaze disk file copy
        long fslen;
        if (buffered is FileStream fileStream)
        {
            File.Copy(fileStream.Name, target, true);
            fslen = fileStream.Length;
        }
        else
        {
            using var fs = File.Open(target, FileMode.Create);
            buffered.CopyTo(fs);
            fslen = fs.Length;
        }

        QuotaUsedAdd(domain, fslen);

        _crypt.EncryptFile(target);

        return GetUri(domain, path);
    }

    public override Uri Save(string domain, string path, Stream stream, ACL acl)
    {
        return Save(domain, path, stream);
    }

    #region chunking

    public override string InitiateChunkedUpload(string domain, string path)
    {
        var target = GetTarget(domain, path);
        CreateDirectory(target);

        return target;
    }

    public override string UploadChunk(string domain, string path, string uploadId, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
    {
        var target = GetTarget(domain, path);
        var mode = chunkNumber == 0 ? FileMode.Create : FileMode.Append;

        using (var fs = new FileStream(target, mode))
        {
            stream.CopyTo(fs);
        }

        return string.Format("{0}_{1}", chunkNumber, uploadId);
    }

    public override Uri FinalizeChunkedUpload(string domain, string path, string uploadId, Dictionary<int, string> eTags)
    {
        var target = GetTarget(domain, path);

        if (QuotaController != null)
        {
            if (!File.Exists(target))
            {
                throw new FileNotFoundException("file not found " + target);
            }

            var size = _crypt.GetFileSize(target);
            QuotaUsedAdd(domain, size);
        }

        _crypt.EncryptFile(target);

        return GetUri(domain, path);
    }

    public override void AbortChunkedUpload(string domain, string path, string uploadId)
    {
        var target = GetTarget(domain, path);
        if (File.Exists(target))
        {
            File.Delete(target);
        }
    }

    #endregion

    public override void Delete(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            var size = _crypt.GetFileSize(target);
            File.Delete(target);

            QuotaUsedDelete(domain, size);
        }
        else
        {
            throw new FileNotFoundException("file not found", target);
        }
    }

    public override void DeleteFiles(string domain, List<string> paths)
    {
        if (paths == null)
        {
            throw new ArgumentNullException(nameof(paths));
        }

        foreach (var path in paths)
        {
            var target = GetTarget(domain, path);

            if (!File.Exists(target))
            {
                continue;
            }

            var size = _crypt.GetFileSize(target);
            File.Delete(target);

            QuotaUsedDelete(domain, size);
        }
    }

    public override void DeleteFiles(string domain, string folderPath, string pattern, bool recursive)
    {
        if (folderPath == null)
        {
            throw new ArgumentNullException(nameof(folderPath));
        }

        //Return dirs
        var targetDir = GetTarget(domain, folderPath);
        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetFiles(targetDir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var entry in entries)
            {
                var size = _crypt.GetFileSize(entry);
                File.Delete(entry);
                QuotaUsedDelete(domain, size);
            }
        }
        else
        {
                throw new DirectoryNotFoundException($"Directory '{targetDir}' not found");
        }
    }

    public override void DeleteFiles(string domain, string folderPath, DateTime fromDate, DateTime toDate)
    {
        if (folderPath == null)
        {
            throw new ArgumentNullException(nameof(folderPath));
        }

        //Return dirs
        var targetDir = GetTarget(domain, folderPath);
        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);
            foreach (var entry in entries)
            {
                var fileInfo = new FileInfo(entry);
                if (fileInfo.LastWriteTime >= fromDate && fileInfo.LastWriteTime <= toDate)
                {
                    var size = _crypt.GetFileSize(entry);
                    File.Delete(entry);
                    QuotaUsedDelete(domain, size);
                }
            }
        }
        else
        {
                throw new DirectoryNotFoundException($"Directory '{targetDir}' not found");
        }
    }

    public override void MoveDirectory(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        var target = GetTarget(srcdomain, srcdir);
        var newtarget = GetTarget(newdomain, newdir);
        var newtargetSub = newtarget.Remove(newtarget.LastIndexOf(Path.DirectorySeparatorChar));

        if (!Directory.Exists(newtargetSub))
        {
            Directory.CreateDirectory(newtargetSub);
        }

        Directory.Move(target, newtarget);
    }

    public override Uri Move(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
    {
        if (srcpath == null)
        {
            throw new ArgumentNullException(nameof(srcpath));
        }

        if (newpath == null)
        {
            throw new ArgumentNullException(nameof(srcpath));
        }

        var target = GetTarget(srcdomain, srcpath);
        var newtarget = GetTarget(newdomain, newpath);

        if (File.Exists(target))
        {
            if (!Directory.Exists(Path.GetDirectoryName(newtarget)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newtarget));
            }

            var flength = _crypt.GetFileSize(target);

            //Delete file if exists
            if (File.Exists(newtarget))
            {
                File.Delete(newtarget);
            }

            File.Move(target, newtarget);

            QuotaUsedDelete(srcdomain, flength);
            QuotaUsedAdd(newdomain, flength, quotaCheckFileSize);
        }
        else
        {
            throw new FileNotFoundException("File not found", Path.GetFullPath(target));
        }

        return GetUri(newdomain, newpath);
    }

    public override bool IsDirectory(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        //Return dirs
        var targetDir = GetTarget(domain, path);
        if (!string.IsNullOrEmpty(targetDir) && !targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }

        return !string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir);
    }

    public override void DeleteDirectory(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        //Return dirs
        var targetDir = GetTarget(domain, path);

        if (string.IsNullOrEmpty(targetDir))
        {
            throw new Exception("targetDir is null");
        }

            if (!targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }

        if (!Directory.Exists(targetDir)) return;

        var entries = Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories);
        var size = entries.Select(entry => _crypt.GetFileSize(entry)).Sum();

        var subDirs = Directory.GetDirectories(targetDir, "*", SearchOption.AllDirectories).ToList();
        subDirs.Reverse();
        subDirs.ForEach(subdir => Directory.Delete(subdir, true));

        Directory.Delete(targetDir, true);

        QuotaUsedDelete(domain, size);
    }

    public override long GetFileSize(string domain, string path)
    {
        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            return _crypt.GetFileSize(target);
        }

        throw new FileNotFoundException("file not found " + target);
    }

    public override long GetDirectorySize(string domain, string path)
    {
        var target = GetTarget(domain, path);

        if (Directory.Exists(target))
        {
            return Directory.GetFiles(target, "*.*", SearchOption.AllDirectories)
                .Select(entry => _crypt.GetFileSize(entry))
                .Sum();
        }

        throw new FileNotFoundException("directory not found " + target);
    }

    public override Uri SaveTemp(string domain, out string assignedPath, Stream stream)
    {
        assignedPath = Guid.NewGuid().ToString();

        return Save(domain, assignedPath, stream);
    }

    public override string SavePrivate(string domain, string path, Stream stream, DateTime expires)
    {
        return Save(domain, path, stream).ToString();
    }

    public override void DeleteExpired(string domain, string folderPath, TimeSpan oldThreshold)
    {
        if (folderPath == null)
        {
            throw new ArgumentNullException(nameof(folderPath));
        }

        //Return dirs
        var targetDir = GetTarget(domain, folderPath);
        if (!Directory.Exists(targetDir))
        {
            return;
        }

        var entries = Directory.GetFiles(targetDir, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var entry in entries)
        {
            var finfo = new FileInfo(entry);
            if ((DateTime.UtcNow - finfo.CreationTimeUtc) > oldThreshold)
            {
                var size = _crypt.GetFileSize(entry);
                File.Delete(entry);

                QuotaUsedDelete(domain, size);
            }
        }
    }

    public override string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize, string contentType, string contentDisposition, string submitLabel)
    {
        throw new NotSupportedException("This operation supported only on s3 storage");
    }

    public override string GetUploadedUrl(string domain, string directoryPath)
    {
        throw new NotSupportedException("This operation supported only on s3 storage");
    }

    public override string GetUploadUrl()
    {
        throw new NotSupportedException("This operation supported only on s3 storage");
    }

    public override string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType, string contentDisposition)
    {
        throw new NotSupportedException("This operation supported only on s3 storage");
    }

    public override string[] ListDirectoriesRelative(string domain, string path, bool recursive)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        //Return dirs
        var targetDir = GetTarget(domain, path);
        if (!string.IsNullOrEmpty(targetDir) && !targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }

        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetDirectories(targetDir, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            return Array.ConvertAll(
                entries,
                x => x.Substring(targetDir.Length));
        }

        return Array.Empty<string>();
    }

    public override string[] ListFilesRelative(string domain, string path, string pattern, bool recursive)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        //Return dirs
        var targetDir = GetTarget(domain, path);
        if (!string.IsNullOrEmpty(targetDir) && !targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }

        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetFiles(targetDir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            return Array.ConvertAll(
                entries,
                x => x.Substring(targetDir.Length));
        }

        return Array.Empty<string>();
    }

    public override bool IsFile(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        //Return dirs
        var target = GetTarget(domain, path);
        var result = File.Exists(target);

        return result;
    }

    public override Task<bool> IsFileAsync(string domain, string path)
    {
        return Task.FromResult(IsFile(domain, path));
    }

    public override long ResetQuota(string domain)
    {
        if (QuotaController != null)
        {
            var size = GetUsedQuota(domain);
            QuotaController.QuotaUsedSet(Modulename, domain, DataList.GetData(domain), size);
        }

        return 0;
    }

    public override long GetUsedQuota(string domain)
    {
        var target = GetTarget(domain, string.Empty);
        long size = 0;

        if (Directory.Exists(target))
        {
            var entries = Directory.GetFiles(target, "*.*", SearchOption.AllDirectories);
            size = entries.Select(entry => _crypt.GetFileSize(entry)).Sum();
        }

        return size;
    }

    public override Uri Copy(string srcdomain, string srcpath, string newdomain, string newpath)
    {
        if (srcpath == null)
        {
            throw new ArgumentNullException(nameof(srcpath));
        }

        if (newpath == null)
        {
            throw new ArgumentNullException(nameof(srcpath));
        }

        var target = GetTarget(srcdomain, srcpath);
        var newtarget = GetTarget(newdomain, newpath);

        if (File.Exists(target))
        {
            if (!Directory.Exists(Path.GetDirectoryName(newtarget)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newtarget));
            }

            File.Copy(target, newtarget, true);

            var flength = _crypt.GetFileSize(target);
            QuotaUsedAdd(newdomain, flength);
        }
        else
        {
            throw new FileNotFoundException("File not found", Path.GetFullPath(target));
        }

        return GetUri(newdomain, newpath);
    }

    public override void CopyDirectory(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        var target = GetTarget(srcdomain, srcdir);
        var newtarget = GetTarget(newdomain, newdir);

        var diSource = new DirectoryInfo(target);
        var diTarget = new DirectoryInfo(newtarget);

        CopyAll(diSource, diTarget, newdomain);
    }


    public Stream GetWriteStream(string domain, string path)
    {
        return GetWriteStream(domain, path, FileMode.Create);
    }

    public Stream GetWriteStream(string domain, string path, FileMode fileMode)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var target = GetTarget(domain, path);
        CreateDirectory(target);

        return File.Open(target, fileMode);
    }

    public void Decrypt(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            _crypt.DecryptFile(target);
        }
        else
        {
            throw new FileNotFoundException("file not found", target);
        }
    }

    protected override Uri SaveWithAutoAttachment(string domain, string path, Stream stream, string attachmentFileName)
    {
        return Save(domain, path, stream);
    }

    private void CopyAll(DirectoryInfo source, DirectoryInfo target, string newdomain)
    {
        // Check if the target directory exists, if not, create it.
        if (!Directory.Exists(target.FullName))
        {
            Directory.CreateDirectory(target.FullName);
        }

        // Copy each file into it's new directory.
        foreach (var fi in source.GetFiles())
        {
            var fp = CrossPlatform.PathCombine(target.ToString(), fi.Name);
            fi.CopyTo(fp, true);
            var size = _crypt.GetFileSize(fp);
            QuotaUsedAdd(newdomain, size);
        }

        // Copy each subdirectory using recursion.
        foreach (var diSourceSubDir in source.GetDirectories())
        {
            var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir, newdomain);
        }
    }

    private MappedPath GetPath(string domain)
    {
        if (domain != null && _mappedPaths.TryGetValue(domain, out var value))
        {
                    return value;
        }

        return _mappedPaths[string.Empty].AppendDomain(domain);
    }

    private static void CreateDirectory(string target)
    {
        var targetDirectory = Path.GetDirectoryName(target);
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
    }

    private string GetTarget(string domain, string path)
    {
        var pathMap = GetPath(domain);
        //Build Dir
        var target = CrossPlatform.PathCombine(pathMap.PhysicalPath, PathUtils.Normalize(path));
        ValidatePath(target);

        return target;
    }

    private static void ValidatePath(string target)
    {
        if (Path.GetDirectoryName(target).IndexOfAny(Path.GetInvalidPathChars()) != -1 ||
            Path.GetFileName(target).IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            //Throw
            throw new ArgumentException("bad path");
        }
    }

    public void Encrypt(string domain, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            _crypt.EncryptFile(target);
        }
        else
        {
            throw new FileNotFoundException("file not found", target);
        }
    }
}
