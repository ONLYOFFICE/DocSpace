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

namespace ASC.Data.Backup.Tasks;

[Scope]
public class BackupPortalTask : PortalTaskBase
{
    public string BackupFilePath { get; private set; }
    public int Limit { get; private set; }

    private const int MaxLength = 250;
    private const int BatchLimit = 5000;

    private readonly bool _dump;
    private readonly IDbContextFactory<BackupsContext> _dbContextFactory;
    private readonly ILogger<BackupPortalTask> _logger;
    private readonly TenantManager _tenantManager;
    private readonly TempStream _tempStream;

    public BackupPortalTask(
        DbFactory dbFactory,
        IDbContextFactory<BackupsContext> dbContextFactory,
        ILogger<BackupPortalTask> logger,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        StorageFactory storageFactory,
        StorageFactoryConfig storageFactoryConfig,
        ModuleProvider moduleProvider,
        TempStream tempStream)
        : base(dbFactory, logger, storageFactory, storageFactoryConfig, moduleProvider)
    {
        _dump = coreBaseSettings.Standalone;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _tenantManager = tenantManager;
        _tempStream = tempStream;
    }

    public void Init(int tenantId, string toFilePath, int limit, IDataWriteOperator writeOperator)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(toFilePath);

        BackupFilePath = toFilePath;
        Limit = limit;
        WriteOperator = writeOperator;
        Init(tenantId);

    }

    public override async Task RunJob()
    {
        _logger.DebugBeginBackup(TenantId);
        await _tenantManager.SetCurrentTenantAsync(TenantId);

        await using (WriteOperator)
        {
            if (_dump)
            {
                await DoDump(WriteOperator);
            }
            else
            {

                var modulesToProcess = GetModulesToProcess().ToList();
                var fileGroups = await GetFilesGroup();

                var stepscount = ProcessStorage ? fileGroups.Count : 0;
                SetStepsCount(modulesToProcess.Count + stepscount);

                foreach (var module in modulesToProcess)
                {
                    await DoBackupModule(WriteOperator, module);
                }
                if (ProcessStorage)
                {
                    await DoBackupStorageAsync(WriteOperator, fileGroups);
                }
            }
        }

        _logger.DebugEndBackup(TenantId);
    }

    public List<object[]> ExecuteList(DbCommand command)
    {
        var list = new List<object[]>();
        using (var result = command.ExecuteReader())
        {
            while (result.Read())
            {
                var objects = new object[result.FieldCount];
                result.GetValues(objects);
                list.Add(objects);
            }
        }

        return list;
    }

    private async Task DoDump(IDataWriteOperator writer)
    {
        var databases = new Dictionary<Tuple<string, string>, List<string>>();

        try
        {
            using (var connection = DbFactory.OpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "select id, connection_string from mail_server_server";
                ExecuteList(command).ForEach(r =>
                {
                    var connectionString = GetConnectionString((int)r[0], JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(r[1]))["DbConnection"].ToString());

                    var command = connection.CreateCommand();
                    command.CommandText = "show tables";
                    var tables = ExecuteList(command).Select(r => Convert.ToString(r[0])).ToList();
                    databases.Add(new Tuple<string, string>(connectionString.Name, connectionString.ConnectionString), tables);
                });
            }
        }
        catch (Exception e)
        {
            _logger.ErrorWithException(e);
        }

        using (var connection = DbFactory.OpenConnection())
        {
            var command = connection.CreateCommand();
            command.CommandText = "show tables";
            var tables = ExecuteList(command).Select(r => Convert.ToString(r[0])).ToList();
            databases.Add(new Tuple<string, string>("default", DbFactory.ConnectionStringSettings()), tables);
        }

        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(true.ToString())))
        {
            await writer.WriteEntryAsync(KeyHelper.GetDumpKey(), stream);
        }

        var files = new List<BackupFileInfo>();

        var stepscount = 0;
        foreach (var db in databases)
        {
            stepscount += db.Value.Count * 4;// (schema + data) * (dump + zip)
        }

        if (ProcessStorage)
        {
            var tenants = (await _tenantManager.GetTenantsAsync(false)).Select(r => r.Id);
            foreach (var t in tenants)
            {
                files.AddRange(await GetFiles(t));
            }

            stepscount += files.Count * 2 + 1;
            _logger.DebugFilesCount(files.Count);
        }

        SetStepsCount(stepscount);

        foreach (var db in databases)
        {
            await DoDump(writer, db.Key.Item1, db.Key.Item2, db.Value);
        }
        var dir = Path.GetDirectoryName(BackupFilePath);
        var subDir = Path.Combine(dir, Path.GetFileNameWithoutExtension(BackupFilePath));
        _logger.DebugDirRemoveStart(subDir);
        Directory.Delete(subDir, true);
        _logger.DebugDirRemoveEnd(subDir);

        if (ProcessStorage)
        {
            await DoDumpStorage(writer, files);
        }
    }

    private async Task DoDump(IDataWriteOperator writer, string dbName, string connectionString, List<string> tables)
    {
        var excluded = ModuleProvider.AllModules.Where(r => _ignoredModules.Contains(r.ModuleName)).SelectMany(r => r.Tables).Select(r => r.Name).ToList();
        excluded.AddRange(_ignoredTables);
        excluded.Add("res_");

        var dir = Path.GetDirectoryName(BackupFilePath);
        var subDir = CrossPlatform.PathCombine(dir, Path.GetFileNameWithoutExtension(BackupFilePath));
        var schemeDir = "";
        var dataDir = "";
        if (dbName == "default")
        {
            schemeDir = Path.Combine(subDir, KeyHelper.GetDatabaseSchema());
            dataDir = Path.Combine(subDir, KeyHelper.GetDatabaseData());
        }
        else
        {
            schemeDir = Path.Combine(subDir, dbName, KeyHelper.GetDatabaseSchema());
            dataDir = Path.Combine(subDir, dbName, KeyHelper.GetDatabaseData());
        }

        if (!Directory.Exists(schemeDir))
        {
            Directory.CreateDirectory(schemeDir);
        }
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        var dict = new Dictionary<string, int>();
        foreach (var table in tables)
        {
            dict.Add(table, SelectCount(table, connectionString));
        }
        tables.Sort((pair1, pair2) => dict[pair1].CompareTo(dict[pair2]));

        for (var i = 0; i < tables.Count; i += TasksLimit)
        {
            var tasks = new List<Task>(TasksLimit * 2);
            for (var j = 0; j < TasksLimit && i + j < tables.Count; j++)
            {
                var t = tables[i + j];
                tasks.Add(Task.Run(() => DumpTableScheme(t, schemeDir, connectionString)));
                if (!excluded.Any(t.StartsWith))
                {
                    tasks.Add(Task.Run(() => DumpTableData(t, dataDir, dict[t], connectionString)));
                }
                else
                {
                    SetStepCompleted(2);
                }
            }

            Task.WaitAll(tasks.ToArray());

            await ArchiveDir(writer, subDir);
        }
    }

    private async Task<IEnumerable<BackupFileInfo>> GetFiles(int tenantId)
    {
        var files = (await GetFilesToProcess(tenantId)).ToList();
        using var backupRecordContext = _dbContextFactory.CreateDbContext();
        var exclude = await backupRecordContext.Backups.Where(b => b.TenantId == tenantId && b.StorageType == 0 && b.StoragePath != null).ToListAsync();
        files = files.Where(f => !exclude.Any(e => f.Path.Replace('\\', '/').Contains($"/file_{e.StoragePath}/"))).ToList();
        return files;

    }

    private void DumpTableScheme(string t, string dir, string connectionString)
    {
        try
        {
            _logger.DebugDumpTableSchemeStart(t);
            using (var connection = DbFactory.OpenConnection(connectionString: connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SHOW CREATE TABLE `{t}`";
                var createScheme = ExecuteList(command);
                var creates = new StringBuilder();
                creates.Append($"DROP TABLE IF EXISTS `{t}`;");
                creates.AppendLine();
                creates.Append(createScheme
                        .Select(r => Convert.ToString(r[1]))
                        .FirstOrDefault());
                creates.Append(';');

                var path = CrossPlatform.PathCombine(dir, t);
                using (var stream = File.OpenWrite(path))
                {
                    var bytes = Encoding.UTF8.GetBytes(creates.ToString());
                    stream.Write(bytes, 0, bytes.Length);
                }

                SetStepCompleted();
            }

            _logger.DebugDumpTableSchemeStop(t);
        }
        catch (Exception e)
        {
            _logger.ErrorDumpTableScheme(e);
        }

    }

    private int SelectCount(string t, string dbName)
    {
        try
        {
            using var connection = DbFactory.OpenConnection(connectionString: dbName);
            using var analyzeCommand = connection.CreateCommand();
            analyzeCommand.CommandText = $"analyze table {t}";
            analyzeCommand.ExecuteNonQuery();
            using var command = connection.CreateCommand();
            command.CommandText = $"select TABLE_ROWS from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{t}' and TABLE_SCHEMA = '{connection.Database}'";

            return int.Parse(command.ExecuteScalar().ToString());
        }
        catch (Exception e)
        {
            _logger.ErrorSelectCount(e);
            throw;
        }

    }

    private void DumpTableData(string t, string dir, int count, string connectionString)
    {
        try
        {
            if (count == 0)
            {
                _logger.DebugDumpTableDataStop(t);
                SetStepCompleted(2);

                return;
            }

            _logger.DebugDumpTableDataStart(t);
            bool searchWithPrimary;
            string primaryIndex;
            var primaryIndexStep = 0;
            var primaryIndexStart = 0;

            List<string> columns;

            using (var connection = DbFactory.OpenConnection(connectionString: connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = string.Format($"SHOW COLUMNS FROM `{t}`");
                columns = ExecuteList(command).Select(r => "`" + Convert.ToString(r[0]) + "`").ToList();
                if (command.CommandText.Contains("tenants_quota") || command.CommandText.Contains("webstudio_settings"))
                {

                }
            }

            using (var connection = DbFactory.OpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select COLUMN_NAME from information_schema.`COLUMNS` where TABLE_SCHEMA = '{connection.Database}' and TABLE_NAME = '{t}' and COLUMN_KEY = 'PRI' and DATA_TYPE = 'int'";
                primaryIndex = ExecuteList(command).ConvertAll(r => Convert.ToString(r[0])).FirstOrDefault();

            }

            using (var connection = DbFactory.OpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SHOW INDEXES FROM {t} WHERE COLUMN_NAME='{primaryIndex}' AND seq_in_index=1";
                var isLeft = ExecuteList(command);
                searchWithPrimary = isLeft.Count == 1;
            }

            if (searchWithPrimary)
            {
                using var connection = DbFactory.OpenConnection();
                var command = connection.CreateCommand();
                command.CommandText = $"select max({primaryIndex}), min({primaryIndex}) from {t}";
                var minMax = ExecuteList(command).ConvertAll(r => new Tuple<int, int>(Convert.ToInt32(r[0]), Convert.ToInt32(r[1]))).FirstOrDefault();
                primaryIndexStart = minMax.Item2;
                primaryIndexStep = (minMax.Item1 - minMax.Item2) / count;

                if (primaryIndexStep < Limit)
                {
                    primaryIndexStep = Limit;
                }
            }

            var path = CrossPlatform.PathCombine(dir, t);

            var offset = 0;

            do
            {
                List<object[]> result;

                if (searchWithPrimary)
                {
                    result = GetDataWithPrimary(t, columns, primaryIndex, primaryIndexStart, primaryIndexStep, connectionString);
                    primaryIndexStart += primaryIndexStep;
                }
                else
                {
                    result = GetData(t, columns, offset, connectionString);
                }

                offset += Limit;

                var resultCount = result.Count;

                if (resultCount == 0)
                {
                    break;
                }

                SaveToFile(path, t, columns, result);

            } while (true);


            SetStepCompleted();
            _logger.DebugDumpTableDataStop(t);
        }
        catch (Exception e)
        {
            _logger.ErrorDumpTableData(e);
            throw;
        }
    }

    private List<object[]> GetData(string t, List<string> columns, int offset, string connectionString)
    {
        using var connection = DbFactory.OpenConnection(connectionString: connectionString);
        var command = connection.CreateCommand();
        var selects = string.Join(',', columns);
        command.CommandText = $"select {selects} from {t} LIMIT {offset}, {Limit}";

        return ExecuteList(command);
    }

    private List<object[]> GetDataWithPrimary(string t, List<string> columns, string primary, int start, int step, string connectionString)
    {
        using var connection = DbFactory.OpenConnection(connectionString: connectionString);
        var command = connection.CreateCommand();
        var selects = string.Join(',', columns);
        command.CommandText = $"select {selects} from {t} where {primary} BETWEEN  {start} and {start + step} ";

        return ExecuteList(command);
    }

    private ConnectionStringSettings GetConnectionString(int id, string connectionString)
    {
        connectionString = connectionString + ";convert zero datetime=True";
        return new ConnectionStringSettings("mailservice-" + id, connectionString, "MySql.Data.MySqlClient");
    }

    private void SaveToFile(string path, string t, IReadOnlyCollection<string> columns, List<object[]> data)
    {
        _logger.DebugSaveTable(t);
        List<object[]> portion;
        while ((portion = data.Take(BatchLimit).ToList()).Count > 0)
        {
            using (var sw = new StreamWriter(path, true))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.QuoteChar = '\'';
                writer.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                sw.Write("REPLACE INTO `{0}` ({1}) VALUES ", t, string.Join(",", columns));
                sw.WriteLine();

                for (var j = 0; j < portion.Count; j++)
                {
                    var obj = portion[j];
                    sw.Write("(");

                    for (var i = 0; i < obj.Length; i++)
                    {
                        var value = obj[i];
                        if (value is byte[] byteArray && byteArray.Length != 0)
                        {
                            sw.Write("0x");
                            foreach (var b in byteArray)
                            {
                                sw.Write("{0:x2}", b);
                            }
                        }
                        else
                        {
                            var s = obj[i] as string;
                            if (s != null)
                            {
                                sw.Write("'" + s.Replace("\r", "\\r").Replace("\n", "\\n") + "'");
                            }
                            else
                            {
                                var ser = new JsonSerializer();
                                ser.Serialize(writer, value);
                            }
                        }
                        if (i != obj.Length - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(")");

                    if (j != portion.Count - 1)
                    {
                        sw.Write(",");
                    }
                    else
                    {
                        sw.Write(";");
                    }

                    sw.WriteLine();
                }
            }

            data = data.Skip(BatchLimit).ToList();
        }
    }

    private async Task DoDumpStorage(IDataWriteOperator writer, IReadOnlyList<BackupFileInfo> files)
    {
        _logger.DebugBeginBackupStorage();

        var dir = Path.GetDirectoryName(BackupFilePath);
        var subDir = CrossPlatform.PathCombine(dir, Path.GetFileNameWithoutExtension(BackupFilePath));

        for (var i = 0; i < files.Count; i += TasksLimit)
        {
            var storageDir = CrossPlatform.PathCombine(subDir, KeyHelper.GetStorage());

            if (!Directory.Exists(storageDir))
            {
                Directory.CreateDirectory(storageDir);
            }

            var tasks = new List<Task>(TasksLimit);
            for (var j = 0; j < TasksLimit && i + j < files.Count; j++)
            {
                var t = files[i + j];
                tasks.Add(Task.Run(() => DoDumpFileAsync(t, storageDir)));
            }

            Task.WaitAll(tasks.ToArray());

            await ArchiveDir(writer, subDir);

            Directory.Delete(storageDir, true);
        }

        var restoreInfoXml = new XElement("storage_restore", files.Select(file => (object)file.ToXElement()).ToArray());

        var tmpPath = CrossPlatform.PathCombine(subDir, KeyHelper.GetStorageRestoreInfoZipKey());
        Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));

        using (var tmpFile = new FileStream(tmpPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose))
        {
            restoreInfoXml.WriteTo(tmpFile);
            await writer.WriteEntryAsync(KeyHelper.GetStorageRestoreInfoZipKey(), tmpFile);
        }

        SetStepCompleted();

        Directory.Delete(subDir, true);

        _logger.DebugEndBackupStorage();
    }

    private async Task DoDumpFileAsync(BackupFileInfo file, string dir)
    {
        var storage = await StorageFactory.GetStorageAsync(file.Tenant, file.Module);
        var filePath = CrossPlatform.PathCombine(dir, file.GetZipKey());
        var dirName = Path.GetDirectoryName(filePath);

        _logger.DebugBackupFile(filePath);

        if (!Directory.Exists(dirName) && !string.IsNullOrEmpty(dirName))
        {
            Directory.CreateDirectory(dirName);
        }

        if (!WorkContext.IsMono && filePath.Length > MaxLength)
        {
            filePath = @"\\?\" + filePath;
        }

        using (var fileStream = await storage.GetReadStreamAsync(file.Domain, file.Path))
        using (var tmpFile = File.OpenWrite(filePath))
        {
            await fileStream.CopyToAsync(tmpFile);
        }

        SetStepCompleted();
    }

    private async Task ArchiveDir(IDataWriteOperator writer, string subDir)
    {
        _logger.DebugArchiveDirStart(subDir);
        foreach (var enumerateFile in Directory.EnumerateFiles(subDir, "*", SearchOption.AllDirectories))
        {
            var f = enumerateFile;
            if (!WorkContext.IsMono && enumerateFile.Length > MaxLength)
            {
                f = @"\\?\" + f;
            }

            using (var tmpFile = new FileStream(f, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose))
            {
                await writer.WriteEntryAsync(enumerateFile.Substring(subDir.Length), tmpFile);
            }

            SetStepCompleted();
        }

        _logger.DebugArchiveDirEnd(subDir);
    }

    private async Task<List<IGrouping<string, BackupFileInfo>>> GetFilesGroup()
    {
        var files = (await GetFilesToProcess(TenantId)).ToList();

        using var backupRecordContext = _dbContextFactory.CreateDbContext();
        var exclude = await backupRecordContext.Backups.Where(b => b.TenantId == TenantId && b.StorageType == 0 && b.StoragePath != null).ToListAsync();

        files = files.Where(f => !exclude.Any(e => f.Path.Replace('\\', '/').Contains($"/file_{e.StoragePath}/"))).ToList();

        return files.GroupBy(file => file.Module).ToList();
    }

    private async Task DoBackupModule(IDataWriteOperator writer, IModuleSpecifics module)
    {
        _logger.DebugBeginSavingDataForModule(module.ModuleName);
        var tablesToProcess = module.Tables.Where(t => !_ignoredTables.Contains(t.Name) && t.InsertMethod != InsertMethod.None).ToList();
        var tablesCount = tablesToProcess.Count;
        var tablesProcessed = 0;

        using (var connection = DbFactory.OpenConnection())
        {
            foreach (var table in tablesToProcess)
            {
                _logger.DebugBeginLoadTable(table.Name);
                using (var data = new DataTable(table.Name))
                {
                    ActionInvoker.Try(
                        state =>
                        {
                            data.Clear();
                            int counts;
                            var offset = 0;
                            do
                            {
                                var t = (TableInfo)state;
                                var dataAdapter = DbFactory.CreateDataAdapter();
                                dataAdapter.SelectCommand = module.CreateSelectCommand(connection.Fix(), TenantId, t, Limit, offset).WithTimeout(600);
                                counts = ((DbDataAdapter)dataAdapter).Fill(data);
                                offset += Limit;
                            } while (counts == Limit);

                        },
                        table,
                        maxAttempts: 5,
                        onFailure: error => { throw ThrowHelper.CantBackupTable(table.Name, error); },
                        onAttemptFailure: error => _logger.WarningBackupAttemptFailure(error));

                    foreach (var col in data.Columns.Cast<DataColumn>().Where(col => col.DataType == typeof(DateTime)))
                    {
                        col.DateTimeMode = DataSetDateTime.Unspecified;
                    }

                    module.PrepareData(data);

                    _logger.DebugEndLoadTable(table.Name);

                    _logger.DebugBeginSavingTable(table.Name);

                    using (var file = _tempStream.Create())
                    {
                        data.WriteXml(file, XmlWriteMode.WriteSchema);
                        data.Clear();

                        await writer.WriteEntryAsync(KeyHelper.GetTableZipKey(module, data.TableName), file);
                    }

                    _logger.DebugEndSavingTable(table.Name);
                }

                SetCurrentStepProgress((int)(++tablesProcessed * 100 / (double)tablesCount));
            }
        }

        _logger.DebugEndSavingDataForModule(module.ModuleName);
    }

    private async Task DoBackupStorageAsync(IDataWriteOperator writer, List<IGrouping<string, BackupFileInfo>> fileGroups)
    {
        _logger.DebugBeginBackupStorage();

        foreach (var group in fileGroups)
        {
            var filesProcessed = 0;
            var filesCount = group.Count();

            foreach (var file in group)
            {
                var storage = await StorageFactory.GetStorageAsync(TenantId, group.Key);
                var file1 = file;
                Stream fileStream = null;
                await ActionInvoker.Try(async state =>
                {
                    var f = (BackupFileInfo)state;
                    fileStream = await storage.GetReadStreamAsync(f.Domain, f.Path);
                }, file, 5, error => _logger.WarningCanNotBackupFile(file1.Module, file1.Path, error));
                if(fileStream != null)
                {
                    await writer.WriteEntryAsync(file1.GetZipKey(), fileStream);
                    fileStream.Dispose();
                }
                SetCurrentStepProgress((int)(++filesProcessed * 100 / (double)filesCount));
            }
        }

        var restoreInfoXml = new XElement(
            "storage_restore",
            fileGroups
                .SelectMany(group => group.Select(file => (object)file.ToXElement()))
                .ToArray());

        using (var tmpFile = _tempStream.Create())
        {
            restoreInfoXml.WriteTo(tmpFile);
            await writer.WriteEntryAsync(KeyHelper.GetStorageRestoreInfoZipKey(), tmpFile);
        }

        _logger.DebugEndBackupStorage();
    }

}
