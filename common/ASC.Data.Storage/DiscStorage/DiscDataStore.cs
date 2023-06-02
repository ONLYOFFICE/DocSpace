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

namespace ASC.Data.Storage.DiscStorage;

[Scope]
public class DiscDataStore : BaseStorage
{
    public override bool IsSupportInternalUri => false;
    public override bool IsSupportedPreSignedUri => false;
    public override bool IsSupportChunking => true;

    private readonly Dictionary<string, MappedPath> _mappedPaths = new Dictionary<string, MappedPath>();
    private ICrypt _crypt;
    private readonly EncryptionSettingsHelper _encryptionSettingsHelper;
    private readonly EncryptionFactory _encryptionFactory;

    public override IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props)
    {
        Tenant = tenant;
        //Fill map path
        Modulename = moduleConfig.Name;
        DataList = new DataList(moduleConfig);

        foreach (var domain in moduleConfig.Domain)
        {
            _mappedPaths.Add(domain.Name, new MappedPath(_tpathUtils, tenant, moduleConfig.AppendTenantId, domain.Path, handlerConfig.GetProperties()));
        }

        //Add default
        _mappedPaths.Add(string.Empty, new MappedPath(_tpathUtils, tenant, moduleConfig.AppendTenantId, PathUtils.Normalize(moduleConfig.Path), handlerConfig.GetProperties()));

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
        ILoggerProvider options,
        ILogger<DiscDataStore> logger,
        EncryptionSettingsHelper encryptionSettingsHelper,
        EncryptionFactory encryptionFactory,
        IHttpClientFactory clientFactory,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper,
        QuotaSocketManager quotaSocketManager)
        : base(tempStream, tenantManager, pathUtils, emailValidationKeyProvider, httpContextAccessor, options, logger, clientFactory, tenantQuotaFeatureStatHelper, quotaSocketManager)
    {
        _encryptionSettingsHelper = encryptionSettingsHelper;
        _encryptionFactory = encryptionFactory;
    }

    public string GetPhysicalPath(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var pathMap = GetPath(domain);

        return (pathMap.PhysicalPath + EnsureLeadingSlash(path)).Replace('\\', '/');
    }

    public override Task<Stream> GetReadStreamAsync(string domain, string path)
    {
        return Task.FromResult(GetReadStream(domain, path, true));
    }

    public Stream GetReadStream(string domain, string path, bool withDecription)
    {
        ArgumentNullException.ThrowIfNull(path);

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            return withDecription ? _crypt.GetReadStream(target) : File.OpenRead(target);
        }

        throw new FileNotFoundException("File not found", Path.GetFullPath(target));
    }

    public override Task<Stream> GetReadStreamAsync(string domain, string path, long offset)
    {
        ArgumentNullException.ThrowIfNull(path);

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            var stream = _crypt.GetReadStream(target);
            if (0 < offset && stream.CanSeek)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }
            else if (0 < offset)
            {
                throw new InvalidOperationException("Seek stream is not impossible");
            }

            return Task.FromResult(stream);
        }

        throw new FileNotFoundException("File not found", Path.GetFullPath(target));
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType, string contentDisposition)
    {
        return SaveAsync(domain, path, stream);
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentEncoding, int cacheDays)
    {
        return SaveAsync(domain, path, stream);
    }
    private bool EnableQuotaCheck(string domain)
    {
        return (QuotaController != null) && !domain.EndsWith("_temp");
    }

    public override async Task<Uri> SaveAsync(string domain, string path, Stream stream)
    {
        Logger.DebugSavePath(path);

        var buffered = _tempStream.GetBuffered(stream);
            
        if (EnableQuotaCheck(domain))
        {
            await QuotaController.QuotaUsedCheckAsync(buffered.Length);
        }

        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(stream);

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
            await buffered.CopyToAsync(fs);
            fslen = fs.Length;
        }

        await QuotaUsedAddAsync(domain, fslen);

        _crypt.EncryptFile(target);

        return await GetUriAsync(domain, path);
    }

    public override Task<Uri> SaveAsync(string domain, string path, Stream stream, ACL acl)
    {
        return SaveAsync(domain, path, stream);
    }

    #region chunking
    public override Task<string> InitiateChunkedUploadAsync(string domain, string path)
    {
        var target = GetTarget(domain, path);
        CreateDirectory(target);
        return Task.FromResult(target);
    }

    public override async Task<string> UploadChunkAsync(string domain, string path, string uploadId, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength)
    {
        var target = GetTarget(domain, path);
        var mode = chunkNumber == 0 ? FileMode.Create : FileMode.Append;

        using (var fs = new FileStream(target, mode))
        {
            await stream.CopyToAsync(fs);
        }

        return string.Format("{0}_{1}", chunkNumber, uploadId);
    }

    public override async Task<Uri> FinalizeChunkedUploadAsync(string domain, string path, string uploadId, Dictionary<int, string> eTags)
    {
        var target = GetTarget(domain, path);

        if (QuotaController != null)
        {
            if (!File.Exists(target))
            {
                throw new FileNotFoundException("file not found " + target);
            }

            var size = _crypt.GetFileSize(target);
            await QuotaUsedAddAsync(domain, size);
        }

        _crypt.EncryptFile(target);

        return await GetUriAsync(domain, path);
    }

    public override Task AbortChunkedUploadAsync(string domain, string path, string uploadId)
    {
        var target = GetTarget(domain, path);
        if (File.Exists(target))
        {
            File.Delete(target);
        }
        return Task.CompletedTask;
    }

    #endregion

    public override async Task DeleteAsync(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            var size = _crypt.GetFileSize(target);
            File.Delete(target);

            await QuotaUsedDeleteAsync(domain, size);
        }
        else
        {
            throw new FileNotFoundException("file not found", target);
        }
    }

    public override async Task DeleteFilesAsync(string domain, List<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        foreach (var path in paths)
        {
            var target = GetTarget(domain, path);

            if (!File.Exists(target))
            {
                continue;
            }

            var size = _crypt.GetFileSize(target);
            File.Delete(target);

            await QuotaUsedDeleteAsync(domain, size);
        }
    }

    public override async Task DeleteFilesAsync(string domain, string folderPath, string pattern, bool recursive)
    {
        ArgumentNullException.ThrowIfNull(folderPath);

        //Return dirs
        var targetDir = GetTarget(domain, folderPath);
        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetFiles(targetDir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var entry in entries)
            {
                var size = _crypt.GetFileSize(entry);
                File.Delete(entry);
                await QuotaUsedDeleteAsync(domain, size);
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory '{targetDir}' not found");
        }
    }

    public override async Task DeleteFilesAsync(string domain, string folderPath, DateTime fromDate, DateTime toDate)
    {
        ArgumentNullException.ThrowIfNull(folderPath);

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
                    await QuotaUsedDeleteAsync(domain, size);
                }
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory '{targetDir}' not found");
        }
    }

    public override Task MoveDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        var target = GetTarget(srcdomain, srcdir);
        var newtarget = GetTarget(newdomain, newdir);
        var newtargetSub = newtarget.Remove(newtarget.LastIndexOf(Path.DirectorySeparatorChar));

        if (!Directory.Exists(newtargetSub))
        {
            Directory.CreateDirectory(newtargetSub);
        }

        Directory.Move(target, newtarget);

        return Task.CompletedTask;
    }

    public override async Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true)
    {
        ArgumentNullException.ThrowIfNull(srcpath);
        ArgumentNullException.ThrowIfNull(newpath);

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

            await QuotaUsedDeleteAsync(srcdomain, flength);
            await QuotaUsedAddAsync(newdomain, flength, quotaCheckFileSize);
        }
        else
        {
            throw new FileNotFoundException("File not found", Path.GetFullPath(target));
        }
        return await GetUriAsync(newdomain, newpath);
    }

    public override Task<bool> IsDirectoryAsync(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        //Return dirs
        var targetDir = GetTarget(domain, path);
        if (!string.IsNullOrEmpty(targetDir) && !targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }
        return Task.FromResult(!string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir));
    }

    public override async Task DeleteDirectoryAsync(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

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

        if (!Directory.Exists(targetDir))
        {
            return;
        }

        var entries = Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories);
        var size = entries.Select(entry => _crypt.GetFileSize(entry)).Sum();

        var subDirs = Directory.GetDirectories(targetDir, "*", SearchOption.AllDirectories).ToList();
        subDirs.Reverse();
        subDirs.ForEach(subdir => Directory.Delete(subdir, true));

        Directory.Delete(targetDir, true);

        await QuotaUsedDeleteAsync(domain, size);
    }

    public override Task<long> GetFileSizeAsync(string domain, string path)
    {
        var target = GetTarget(domain, path);

        if (File.Exists(target))
        {
            return Task.FromResult(_crypt.GetFileSize(target));
        }

        throw new FileNotFoundException("file not found " + target);
    }

    public override Task<long> GetDirectorySizeAsync(string domain, string path)
    {
        var target = GetTarget(domain, path);

        if (Directory.Exists(target))
        {
            return Task.FromResult(Directory.GetFiles(target, "*.*", SearchOption.AllDirectories)
            .Select(entry => _crypt.GetFileSize(entry))
                .Sum());
        }

        throw new FileNotFoundException("directory not found " + target);
    }

    public override async Task<(Uri, string)> SaveTempAsync(string domain, Stream stream)
    {
        var assignedPath = Guid.NewGuid().ToString();
        return (await SaveAsync(domain, assignedPath, stream), assignedPath);
    }

    public override async Task<string> SavePrivateAsync(string domain, string path, Stream stream, DateTime expires)
    {
        var result = await SaveAsync(domain, path, stream);
        return result.ToString();
    }

    public override async Task DeleteExpiredAsync(string domain, string folderPath, TimeSpan oldThreshold)
    {
        ArgumentNullException.ThrowIfNull(folderPath);

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

                await QuotaUsedDeleteAsync(domain, size);
            }
        }
    }

    public override string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize, string contentType, string contentDisposition, string submitLabel)
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

    public override IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string domain, string path, bool recursive)
    {
        ArgumentNullException.ThrowIfNull(path);

        //Return dirs
        var targetDir = GetTarget(domain, path);
        if (!string.IsNullOrEmpty(targetDir) && !targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }

        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetDirectories(targetDir, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var tmp = Array.ConvertAll(
            entries,
            x => x.Substring(targetDir.Length));
            return tmp.ToAsyncEnumerable();
        }
        return AsyncEnumerable.Empty<string>();
    }

    public override IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive)
    {
        ArgumentNullException.ThrowIfNull(path);

        //Return dirs
        var targetDir = GetTarget(domain, path);
        if (!string.IsNullOrEmpty(targetDir) && !targetDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            targetDir += Path.DirectorySeparatorChar;
        }

        if (Directory.Exists(targetDir))
        {
            var entries = Directory.GetFiles(targetDir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var tmp = Array.ConvertAll(
            entries,
            x => x.Substring(targetDir.Length));
            return tmp.ToAsyncEnumerable();
        }
        return AsyncEnumerable.Empty<string>();
    }

    public override Task<bool> IsFileAsync(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        //Return dirs
        var target = GetTarget(domain, path);
        var result = File.Exists(target);
        return Task.FromResult(result);
    }

    public override async Task<long> ResetQuotaAsync(string domain)
    {
        if (QuotaController != null)
        {
            var size = await GetUsedQuotaAsync(domain);
            await QuotaController.QuotaUsedSetAsync(Modulename, domain, DataList.GetData(domain), size);
        }

        return 0;
    }

    public override Task<long> GetUsedQuotaAsync(string domain)
    {
        var target = GetTarget(domain, string.Empty);
        long size = 0;

        if (Directory.Exists(target))
        {
            var entries = Directory.GetFiles(target, "*.*", SearchOption.AllDirectories);
            size = entries.Select(entry => _crypt.GetFileSize(entry)).Sum();
        }
        return Task.FromResult(size);
    }

    public override async Task<Uri> CopyAsync(string srcdomain, string srcpath, string newdomain, string newpath)
    {
        ArgumentNullException.ThrowIfNull(srcpath);
        ArgumentNullException.ThrowIfNull(newpath);

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
            await QuotaUsedAddAsync(newdomain, flength);
        }
        else
        {
            throw new FileNotFoundException("File not found", Path.GetFullPath(target));
        }
        return await GetUriAsync(newdomain, newpath);
    }

    public override async Task CopyDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir)
    {
        var target = GetTarget(srcdomain, srcdir);
        var newtarget = GetTarget(newdomain, newdir);

        var diSource = new DirectoryInfo(target);
        var diTarget = new DirectoryInfo(newtarget);

        await CopyAllAsync(diSource, diTarget, newdomain);
    }


    public Stream GetWriteStream(string domain, string path)
    {
        return GetWriteStream(domain, path, FileMode.Create);
    }

    public Stream GetWriteStream(string domain, string path, FileMode fileMode)
    {
        ArgumentNullException.ThrowIfNull(path);

        var target = GetTarget(domain, path);
        CreateDirectory(target);

        return File.Open(target, fileMode);
    }

    public void Decrypt(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

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

    protected override Task<Uri> SaveWithAutoAttachmentAsync(string domain, string path, Stream stream, string attachmentFileName)
    {
        return SaveAsync(domain, path, stream);
    }

    private async Task CopyAllAsync(DirectoryInfo source, DirectoryInfo target, string newdomain)
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
            await QuotaUsedAddAsync(newdomain, size);
        }

        // Copy each subdirectory using recursion.
        foreach (var diSourceSubDir in source.GetDirectories())
        {
            var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            await CopyAllAsync(diSourceSubDir, nextTargetSubDir, newdomain);
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
        ArgumentNullException.ThrowIfNull(path);

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

    public override Task<string> GetFileEtagAsync(string domain, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var target = GetTarget(domain, path);
        var lastModificationDate = File.Exists(target) ? File.GetLastWriteTimeUtc(target) : throw new FileNotFoundException("File not found" + target);
        var etag = '"' + lastModificationDate.Ticks.ToString("X8", CultureInfo.InvariantCulture) + '"';

        return Task.FromResult(etag);
    }
}
