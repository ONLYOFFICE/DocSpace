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


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Data.Backup.EF.Context;
using ASC.Data.Backup.Exceptions;
using ASC.Data.Backup.Extensions;
using ASC.Data.Backup.Tasks.Data;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ASC.Data.Backup.Tasks
{
    [Scope]
    public class BackupPortalTask : PortalTaskBase
    {
        private const int MaxLength = 250;

        private const int BatchLimit = 5000;
        public string BackupFilePath { get; private set; }
        public int Limit { get; private set; }
        private bool Dump { get; set; }
        private TenantManager TenantManager { get; set; }
        private TempStream TempStream { get; }
        private Lazy<BackupsContext> LazyBackupsContext { get; }
        private BackupsContext BackupRecordContext { get => LazyBackupsContext.Value; }

        public BackupPortalTask(DbFactory dbFactory, DbContextManager<BackupsContext> dbContextManager, IOptionsMonitor<ILog> options, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig, ModuleProvider moduleProvider, TempStream tempStream)
            : base(dbFactory, options, storageFactory, storageFactoryConfig, moduleProvider)
        {
            Dump = coreBaseSettings.Standalone;
            TenantManager = tenantManager;
            TempStream = tempStream;
            LazyBackupsContext = new Lazy<BackupsContext>(() => dbContextManager.Get(DbFactory.ConnectionStringSettings.ConnectionString));
        }

        public void Init(int tenantId, string fromConfigPath, string toFilePath, int limit)
        {
            if (string.IsNullOrEmpty(toFilePath))
                throw new ArgumentNullException(nameof(toFilePath));
            BackupFilePath = toFilePath;
            Limit = limit;
            Init(tenantId, fromConfigPath);

        }
        public override void RunJob()
        {
            Logger.DebugFormat("begin backup {0}", TenantId);
            TenantManager.SetCurrentTenant(TenantId);


            using (var writer = new ZipWriteOperator(TempStream, BackupFilePath))
            {
                if (Dump)
                {
                    DoDump(writer);
                }
                else
                {

                    var modulesToProcess = GetModulesToProcess().ToList();
                    var fileGroups = GetFilesGroup();

                    var stepscount = ProcessStorage ? fileGroups.Count : 0;
                    SetStepsCount(modulesToProcess.Count + stepscount);

                    foreach (var module in modulesToProcess)
                    {
                        DoBackupModule(writer, module);
                    }
                    if (ProcessStorage)
                    {
                        DoBackupStorage(writer, fileGroups);
                    }
                }
            }
            Logger.DebugFormat("end backup {0}", TenantId);
        }

        private void DoDump(IDataWriteOperator writer)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(true.ToString())))
            {
                writer.WriteEntry(KeyHelper.GetDumpKey(), stream);
            }

            List<string> tables;
            var files = new List<BackupFileInfo>();
            using (var connection = DbFactory.OpenConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "show tables";
                tables = ExecuteList(command).Select(r => Convert.ToString(r[0])).ToList();
            }
            /*  using (var dbManager = new DbManager("default", 100000))
              {
                  tables = dbManager.ExecuteList("show tables;").Select(r => Convert.ToString(r[0])).ToList();
              }*/

            var stepscount = tables.Count * 4; // (schema + data) * (dump + zip)
            if (ProcessStorage)
            {
                var tenants = TenantManager.GetTenants(false).Select(r => r.TenantId);
                foreach (var t in tenants)
                {
                    files.AddRange(GetFiles(t));
                }
                stepscount += files.Count * 2 + 1;
                Logger.Debug("files:" + files.Count);
            }

            SetStepsCount(stepscount);

            var excluded = ModuleProvider.AllModules.Where(r => IgnoredModules.Contains(r.ModuleName)).SelectMany(r => r.Tables).Select(r => r.Name).ToList();
            excluded.AddRange(IgnoredTables);
            excluded.Add("res_");

            var dir = Path.GetDirectoryName(BackupFilePath);
            var subDir = CrossPlatform.PathCombine(dir, Path.GetFileNameWithoutExtension(BackupFilePath));
            var schemeDir = CrossPlatform.PathCombine(subDir, KeyHelper.GetDatabaseSchema());
            var dataDir = CrossPlatform.PathCombine(subDir, KeyHelper.GetDatabaseData());

            if (!Directory.Exists(schemeDir))
            {
                Directory.CreateDirectory(schemeDir);
            }
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            var dict = tables.ToDictionary(t => t, SelectCount);
            tables.Sort((pair1, pair2) => dict[pair1].CompareTo(dict[pair2]));

            for (var i = 0; i < tables.Count; i += TasksLimit)
            {
                var tasks = new List<Task>(TasksLimit * 2);
                for (var j = 0; j < TasksLimit && i + j < tables.Count; j++)
                {
                    var t = tables[i + j];
                    tasks.Add(Task.Run(() => DumpTableScheme(t, schemeDir)));
                    if (!excluded.Any(t.StartsWith))
                    {
                        tasks.Add(Task.Run(() => DumpTableData(t, dataDir, dict[t])));
                    }
                    else
                    {
                        SetStepCompleted(2);
                    }
                }

                Task.WaitAll(tasks.ToArray());

                ArchiveDir(writer, subDir);
            }

            Logger.DebugFormat("dir remove start {0}", subDir);
            Directory.Delete(subDir, true);
            Logger.DebugFormat("dir remove end {0}", subDir);

            if (ProcessStorage)
            {
                DoDumpStorage(writer, files);
            }
        }

        private IEnumerable<BackupFileInfo> GetFiles(int tenantId)
        {
            var files = GetFilesToProcess(tenantId).ToList();
            var exclude = BackupRecordContext.Backups.AsQueryable().Where(b => b.TenantId == tenantId && b.StorageType == 0 && b.StoragePath != null).ToList();
            files = files.Where(f => !exclude.Any(e => f.Path.Replace('\\', '/').Contains($"/file_{e.StoragePath}/"))).ToList();
            return files;

        }

        private void DumpTableScheme(string t, string dir)
        {
            try
            {
                Logger.DebugFormat("dump table scheme start {0}", t);
                using (var connection = DbFactory.OpenConnection())
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

                Logger.DebugFormat("dump table scheme stop {0}", t);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }

        }

        private int SelectCount(string t)
        {
            try
            {
                using var connection = DbFactory.OpenConnection();
                using var analyzeCommand = connection.CreateCommand();
                analyzeCommand.CommandText = $"analyze table {t}";
                analyzeCommand.ExecuteNonQuery();
                using var command = connection.CreateCommand();
                command.CommandText = $"select TABLE_ROWS from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{t}' and TABLE_SCHEMA = '{connection.Database}'";
                return int.Parse(command.ExecuteScalar().ToString());
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }

        }

        private void DumpTableData(string t, string dir, int count)
        {
            try
            {
                if (count == 0)
                {
                    Logger.DebugFormat("dump table data stop {0}", t);
                    SetStepCompleted(2);
                    return;
                }

                Logger.DebugFormat("dump table data start {0}", t);
                bool searchWithPrimary;
                string primaryIndex;
                var primaryIndexStep = 0;
                var primaryIndexStart = 0;

                List<string> columns;
                using (var connection = DbFactory.OpenConnection())
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
                        result = GetDataWithPrimary(t, columns, primaryIndex, primaryIndexStart, primaryIndexStep);
                        primaryIndexStart += primaryIndexStep;
                    }
                    else
                    {
                        result = GetData(t, columns, offset);
                    }

                    offset += Limit;

                    var resultCount = result.Count;

                    if (resultCount == 0) break;

                    SaveToFile(path, t, columns, result);

                    if (resultCount < Limit) break;

                } while (true);


                SetStepCompleted();
                Logger.DebugFormat("dump table data stop {0}", t);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        private List<object[]> GetData(string t, List<string> columns, int offset)
        {
            using var connection = DbFactory.OpenConnection();
            var command = connection.CreateCommand();
            var selects = string.Join(',', columns);
            command.CommandText = $"select {selects} from {t} LIMIT {offset}, {Limit}";
            return ExecuteList(command);
        }
        private List<object[]> GetDataWithPrimary(string t, List<string> columns, string primary, int start, int step)
        {
            using var connection = DbFactory.OpenConnection();
            var command = connection.CreateCommand();
            var selects = string.Join(',', columns);
            command.CommandText = $"select {selects} from {t} where {primary} BETWEEN  {start} and {start + step} ";
            return ExecuteList(command);
        }

        private void SaveToFile(string path, string t, IReadOnlyCollection<string> columns, List<object[]> data)
        {
            Logger.DebugFormat("save to file {0}", t);
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
                            if (value is byte[] byteArray)
                            {
                                sw.Write("0x");
                                foreach (var b in byteArray)
                                    sw.Write("{0:x2}", b);
                            }
                            else
                            {
                                var ser = new JsonSerializer();
                                ser.Serialize(writer, value);
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

        private void DoDumpStorage(IDataWriteOperator writer, IReadOnlyList<BackupFileInfo> files)
        {
            Logger.Debug("begin backup storage");

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
                    tasks.Add(Task.Run(() => DoDumpFile(t, storageDir)));
                }

                Task.WaitAll(tasks.ToArray());

                ArchiveDir(writer, subDir);

                Directory.Delete(storageDir, true);
            }

            var restoreInfoXml = new XElement("storage_restore", files.Select(file => (object)file.ToXElement()).ToArray());

            var tmpPath = CrossPlatform.PathCombine(subDir, KeyHelper.GetStorageRestoreInfoZipKey());
            Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));

            using (var tmpFile = new FileStream(tmpPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose))
            {
                restoreInfoXml.WriteTo(tmpFile);
                writer.WriteEntry(KeyHelper.GetStorageRestoreInfoZipKey(), tmpFile);
            }

            SetStepCompleted();

            Directory.Delete(subDir, true);

            Logger.Debug("end backup storage");
        }

        private async Task DoDumpFile(BackupFileInfo file, string dir)
        {
            var storage = StorageFactory.GetStorage(ConfigPath, file.Tenant.ToString(), file.Module);
            var filePath = CrossPlatform.PathCombine(dir, file.GetZipKey());
            var dirName = Path.GetDirectoryName(filePath);

            Logger.DebugFormat("backup file {0}", filePath);

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

        private void ArchiveDir(IDataWriteOperator writer, string subDir)
        {
            Logger.DebugFormat("archive dir start {0}", subDir);
            foreach (var enumerateFile in Directory.EnumerateFiles(subDir, "*", SearchOption.AllDirectories))
            {
                var f = enumerateFile;
                if (!WorkContext.IsMono && enumerateFile.Length > MaxLength)
                {
                    f = @"\\?\" + f;
                }
                using (var tmpFile = new FileStream(f, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose))
                {
                    writer.WriteEntry(enumerateFile.Substring(subDir.Length), tmpFile);
                }
                SetStepCompleted();
            }
            Logger.DebugFormat("archive dir end {0}", subDir);
        }

        private List<IGrouping<string, BackupFileInfo>> GetFilesGroup()
        {
            var files = GetFilesToProcess(TenantId).ToList();
            var exclude = BackupRecordContext.Backups.AsQueryable().Where(b => b.TenantId == TenantId && b.StorageType == 0 && b.StoragePath != null).ToList();

            files = files.Where(f => !exclude.Any(e => f.Path.Replace('\\', '/').Contains($"/file_{e.StoragePath}/"))).ToList();

            return files.GroupBy(file => file.Module).ToList();
        }

        private void DoBackupModule(IDataWriteOperator writer, IModuleSpecifics module)
        {
            Logger.DebugFormat("begin saving data for module {0}", module.ModuleName);
            var tablesToProcess = module.Tables.Where(t => !IgnoredTables.Contains(t.Name) && t.InsertMethod != InsertMethod.None).ToList();
            var tablesCount = tablesToProcess.Count;
            var tablesProcessed = 0;

            using (var connection = DbFactory.OpenConnection())
            {
                foreach (var table in tablesToProcess)
                {
                    Logger.DebugFormat("begin load table {0}", table.Name);
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
                            onAttemptFailure: error => Logger.Warn("backup attempt failure: {0}", error));

                        foreach (var col in data.Columns.Cast<DataColumn>().Where(col => col.DataType == typeof(DateTime)))
                        {
                            col.DateTimeMode = DataSetDateTime.Unspecified;
                        }

                        module.PrepareData(data);

                        Logger.DebugFormat("end load table {0}", table.Name);

                        Logger.DebugFormat("begin saving table {0}", table.Name);

                        using (var file = TempStream.Create())
                        {
                            data.WriteXml(file, XmlWriteMode.WriteSchema);
                            data.Clear();

                            writer.WriteEntry(KeyHelper.GetTableZipKey(module, data.TableName), file);
                        }

                        Logger.DebugFormat("end saving table {0}", table.Name);
                    }

                    SetCurrentStepProgress((int)(++tablesProcessed * 100 / (double)tablesCount));
                }
            }
            Logger.DebugFormat("end saving data for module {0}", module.ModuleName);
        }

        private void DoBackupStorage(IDataWriteOperator writer, List<IGrouping<string, BackupFileInfo>> fileGroups)
        {
            Logger.Debug("begin backup storage");

            foreach (var group in fileGroups)
            {
                var filesProcessed = 0;
                var filesCount = group.Count();

                foreach (var file in group)
                {
                    var storage = StorageFactory.GetStorage(ConfigPath, TenantId.ToString(), group.Key);
                    var file1 = file;
                    ActionInvoker.Try(state =>
                    {
                        var f = (BackupFileInfo)state;
                        using var fileStream = storage.GetReadStreamAsync(f.Domain, f.Path).Result;
                        writer.WriteEntry(file1.GetZipKey(), fileStream);
                    }, file, 5, error => Logger.WarnFormat("can't backup file ({0}:{1}): {2}", file1.Module, file1.Path, error));

                    SetCurrentStepProgress((int)(++filesProcessed * 100 / (double)filesCount));
                }
            }

            var restoreInfoXml = new XElement(
                "storage_restore",
                fileGroups
                    .SelectMany(group => group.Select(file => (object)file.ToXElement()))
                    .ToArray());

            using (var tmpFile = TempStream.Create())
            {
                restoreInfoXml.WriteTo(tmpFile);
                writer.WriteEntry(KeyHelper.GetStorageRestoreInfoZipKey(), tmpFile);
            }

            Logger.Debug("end backup storage");
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
    }
}
