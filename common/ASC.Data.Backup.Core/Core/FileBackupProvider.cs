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

namespace ASC.Data.Backup;

[Scope]
public class FileBackupProvider : IBackupProvider
{
    public string Name => "Files";

    private readonly IEnumerable<string> _allowedModules;
    private readonly ILogger<FileBackupProvider> _logger;
    private readonly StorageFactory _storageFactory;
    private readonly StorageFactoryConfig _storageFactoryConfig;

    public FileBackupProvider(ILogger<FileBackupProvider> logger, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig)
    {
        _storageFactory = storageFactory;
        _storageFactoryConfig = storageFactoryConfig;
        _logger = logger;
    }

    public FileBackupProvider()
    {
        _allowedModules = new List<string>() { "forum", "photo", "bookmarking", "wiki", "files", "crm", "projects", "logo", "fckuploaders", "talk" };
    }

    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public async Task<IEnumerable<XElement>> GetElements(int tenant, string[] configs, IDataWriteOperator writer)
    {
        InvokeProgressChanged("Saving files...", 0);

        var config = GetWebConfig(configs);
        var files = await ComposeFiles(tenant, config);

        var elements = new List<XElement>();
        var backupKeys = new List<string>();

        var counter = 0;
        var totalCount = (double)files.Count();

        foreach (var file in files)
        {
            var backupPath = GetBackupPath(file);
            if (!backupKeys.Contains(backupPath))
            {
                var storage = _storageFactory.GetStorage(config, tenant.ToString(), file.Module);
                var errors = 0;
                while (true)
                {
                    try
                    {
                        using var stream = await storage.GetReadStreamAsync(file.Domain, file.Path);
                        writer.WriteEntry(backupPath, stream);
                        break;
                    }
                    catch (Exception error)
                    {
                        errors++;
                        if (20 < errors)
                        {
                            _logger.ErrorCanNotBackupFile(file.Path, error);
                            break;
                        }
                    }
                }
                elements.Add(file.ToXElement());
                backupKeys.Add(backupPath);
                _logger.DebugBackupFile(file.Path);
            }
            InvokeProgressChanged("Saving file " + file.Path, counter++ / totalCount * 100);
        }
        return elements;
    }

    public async Task LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator dataOperator)
    {
        InvokeProgressChanged("Restoring files...", 0);

        var config = GetWebConfig(configs);
        var files = elements.Where(e => e.Name == "file");
        double current = 0;
        foreach (var file in files)
        {
            var backupInfo = new FileBackupInfo(file);
            if (_allowedModules.Contains(backupInfo.Module))
            {
                using (var entry = dataOperator.GetEntry(GetBackupPath(backupInfo)))
                {
                    var storage = _storageFactory.GetStorage(config, tenant.ToString(), backupInfo.Module, null);
                    try
                    {
                        await storage.SaveAsync(backupInfo.Domain, backupInfo.Path, entry);
                    }
                    catch (Exception error)
                    {
                        _logger.ErrorCanNotRestoreFile(file, error);
                    }
                }
                InvokeProgressChanged("Restoring file " + backupInfo.Path, current++ / files.Count() * 100);
            }
        }
    }

    private async Task<IEnumerable<FileBackupInfo>> ComposeFiles(int tenant, string config)
    {
        var files = new List<FileBackupInfo>();
        foreach (var module in _storageFactoryConfig.GetModuleList(config))
        {
            if (_allowedModules.Contains(module))
            {
                var store = _storageFactory.GetStorage(config, tenant.ToString(), module);
                var domainList = _storageFactoryConfig.GetDomainList(config, module);

                foreach (var domain in domainList)
                {
                    files.AddRange((await store
                            .ListFilesRelativeAsync(domain, "\\", "*.*", true).ToArrayAsync())
                        .Select(x => new FileBackupInfo(domain, module, x)));
                }

                files.AddRange((await store
                        .ListFilesRelativeAsync(string.Empty, "\\", "*.*", true).ToArrayAsync())
                        .Where(x => domainList.All(domain => x.IndexOf($"{domain}/") == -1))
                    .Select(x => new FileBackupInfo(string.Empty, module, x)));
            }
        }

        return files.Distinct();
    }

    private string GetBackupPath(FileBackupInfo backupInfo)
    {
        return CrossPlatform.PathCombine(backupInfo.Module, CrossPlatform.PathCombine(backupInfo.Domain, backupInfo.Path.Replace('/', '\\')));
    }

    private string GetWebConfig(string[] configs)
    {
        return configs.Where(c => "Web.config".Equals(Path.GetFileName(c), StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
    }

    private void InvokeProgressChanged(string status, double currentProgress)
    {
        try
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(status, (int)currentProgress));
        }
        catch (Exception error)
        {
            _logger.ErrorInvokeProgressChanged(error);
        }
    }

    private class FileBackupInfo
    {
        public string Module { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public int Errors { get; set; }

        public FileBackupInfo(string domain, string module, string path)
        {
            Domain = domain;
            Module = module;
            Path = path;
        }

        public FileBackupInfo(XElement element)
        {
            Domain = element.Attribute("domain").Value;
            Module = element.Attribute("module").Value;
            Path = element.Attribute("path").Value;
        }

        public XElement ToXElement()
        {
            return new XElement("file",
                new XAttribute("module", Module),
                new XAttribute("domain", Domain),
                new XAttribute("path", Path)
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(FileBackupInfo))
            {
                return false;
            }

            return Equals((FileBackupInfo)obj);
        }

        public bool Equals(FileBackupInfo other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Module, Module) && Equals(other.Domain, Domain) && Equals(other.Path, Path);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Module, Domain, Path);
        }
    }
}
