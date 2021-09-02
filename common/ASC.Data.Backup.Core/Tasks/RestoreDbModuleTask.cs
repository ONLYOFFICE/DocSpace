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
using System.Data.Common;
using System.IO;
using System.Linq;

using ASC.Common.Logging;
using ASC.Data.Backup.Exceptions;
using ASC.Data.Backup.Extensions;
using ASC.Data.Backup.Tasks.Data;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Tasks
{
    internal class RestoreDbModuleTask : PortalTaskBase
    {
        private const int TransactionLength = 10000;

        private IDataReadOperator Reader { get; set; }
        private IModuleSpecifics Module { get; set; }
        private ColumnMapper ColumnMapper { get; set; }
        private bool ReplaceDate { get; set; }
        private bool Dump { get; set; }

        public RestoreDbModuleTask(IOptionsMonitor<ILog> options, IModuleSpecifics module, IDataReadOperator reader, ColumnMapper columnMapper, DbFactory factory, bool replaceDate, bool dump, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig, ModuleProvider moduleProvider)
            : base(factory, options, storageFactory, storageFactoryConfig, moduleProvider)
        {
            Reader = reader ?? throw new ArgumentNullException("reader");
            ColumnMapper = columnMapper ?? throw new ArgumentNullException("columnMapper");
            DbFactory = factory ?? throw new ArgumentNullException("factory");
            Module = module;
            ReplaceDate = replaceDate;
            Dump = dump;
            Init(-1, null);
        }

        public override void RunJob()
        {
            Logger.DebugFormat("begin restore data for module {0}", Module.ModuleName);
            SetStepsCount(Module.Tables.Count(t => !IgnoredTables.Contains(t.Name)));

            using (var connection = DbFactory.OpenConnection())
            {
                foreach (var table in Module.GetTablesOrdered().Where(t => !IgnoredTables.Contains(t.Name) && t.InsertMethod != InsertMethod.None))
                {
                    Logger.DebugFormat("begin restore table {0}", table.Name);

                    var transactionsCommited = 0;
                    var rowsInserted = 0;
                    ActionInvoker.Try(
                        state =>
                            RestoreTable(connection.Fix(), (TableInfo)state, ref transactionsCommited,
                                ref rowsInserted), table, 5,
                        onAttemptFailure: error => ColumnMapper.Rollback(),
                        onFailure: error => { throw ThrowHelper.CantRestoreTable(table.Name, error); });

                    SetStepCompleted();
                    Logger.DebugFormat("{0} rows inserted for table {1}", rowsInserted, table.Name);
                }
            }

            Logger.DebugFormat("end restore data for module {0}", Module.ModuleName);
        }

        private void RestoreTable(DbConnection connection, TableInfo tableInfo, ref int transactionsCommited, ref int rowsInserted)
        {
            SetColumns(connection, tableInfo);

            using var stream = Reader.GetEntry(KeyHelper.GetTableZipKey(Module, tableInfo.Name));
            var lowImportanceRelations = Module
                .TableRelations
                .Where(
                    r =>
                        string.Equals(r.ParentTable, tableInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                .Where(r => r.Importance == RelationImportance.Low && !r.IsSelfRelation())
                .Select(r => Tuple.Create(r, Module.Tables.Single(t => t.Name == r.ChildTable)))
                .ToList();

            foreach (
                var rows in
                    GetRows(tableInfo, stream)
                        .Skip(transactionsCommited * TransactionLength)
                        .MakeParts(TransactionLength))
            {
                using var transaction = connection.BeginTransaction();
                var rowsSuccess = 0;
                foreach (var row in rows)
                {
                    if (ReplaceDate)
                    {
                        foreach (var column in tableInfo.DateColumns)
                        {
                            ColumnMapper.SetDateMapping(tableInfo.Name, column, row[column.Key]);
                        }
                    }

                    object oldIdValue = null;
                    object newIdValue = null;

                    if (tableInfo.HasIdColumn())
                    {
                        oldIdValue = row[tableInfo.IdColumn];
                        newIdValue = ColumnMapper.GetMapping(tableInfo.Name, tableInfo.IdColumn, oldIdValue);
                        if (newIdValue == null)
                        {
                            if (tableInfo.IdType == IdType.Guid)
                            {
                                newIdValue = Guid.NewGuid().ToString("D");
                            }
                            else if (tableInfo.IdType == IdType.Integer)
                            {
                                var command = connection.CreateCommand();
                                command.CommandText = string.Format("select max({0}) from {1};", tableInfo.IdColumn, tableInfo.Name);
                                newIdValue = (int)command.WithTimeout(120).ExecuteScalar() + 1;
                            }
                        }
                        if (newIdValue != null)
                        {
                            ColumnMapper.SetMapping(tableInfo.Name, tableInfo.IdColumn, oldIdValue,
                                newIdValue);
                        }
                    }

                    var insertCommand = Module.CreateInsertCommand(Dump, connection, ColumnMapper, tableInfo,
                        row);
                    if (insertCommand == null)
                    {
                        Logger.WarnFormat("Can't create command to insert row to {0} with values [{1}]", tableInfo,
                            row);
                        ColumnMapper.Rollback();
                        continue;
                    }
                    insertCommand.WithTimeout(120).ExecuteNonQuery();
                    rowsSuccess++;

                    if (tableInfo.HasIdColumn() && tableInfo.IdType == IdType.Autoincrement)
                    {
                        var lastIdCommand = DbFactory.CreateLastInsertIdCommand();
                        lastIdCommand.Connection = connection;
                        newIdValue = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                        ColumnMapper.SetMapping(tableInfo.Name, tableInfo.IdColumn, oldIdValue, newIdValue);
                    }

                    ColumnMapper.Commit();

                    foreach (var relation in lowImportanceRelations)
                    {
                        if (!relation.Item2.HasTenantColumn())
                        {
                            Logger.WarnFormat(
                                "Table {0} does not contain tenant id column. Can't apply low importance relations on such tables.",
                                relation.Item2.Name);
                            continue;
                        }

                        var oldValue = row[relation.Item1.ParentColumn];
                        var newValue = ColumnMapper.GetMapping(relation.Item1.ParentTable,
                            relation.Item1.ParentColumn, oldValue);
                        var command = connection.CreateCommand();
                        command.CommandText = string.Format("update {0} set {1} = {2} where {1} = {3} and {4} = {5}",
                                relation.Item1.ChildTable,
                                relation.Item1.ChildColumn,
                                newValue is string ? "'" + newValue + "'" : newValue,
                                oldValue is string ? "'" + oldValue + "'" : oldValue,
                                relation.Item2.TenantColumn,
                                ColumnMapper.GetTenantMapping());
                        command.WithTimeout(120).ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                transactionsCommited++;
                rowsInserted += rowsSuccess;
            }
        }

        private IEnumerable<DataRowInfo> GetRows(TableInfo table, Stream xmlStream)
        {
            if (xmlStream == null)
                return Enumerable.Empty<DataRowInfo>();

            var rows = DataRowInfoReader.ReadFromStream(xmlStream);

            var selfRelation = Module.TableRelations.SingleOrDefault(x => x.ChildTable == table.Name && x.IsSelfRelation());
            if (selfRelation != null)
            {
                rows = rows
                    .ToTree(x => x[selfRelation.ParentColumn], x => x[selfRelation.ChildColumn])
                    .SelectMany(x => OrderNode(x));
            }

            return rows;
        }

        private static IEnumerable<DataRowInfo> OrderNode(TreeNode<DataRowInfo> node)
        {
            var result = new List<DataRowInfo> { node.Entry };
            result.AddRange(node.Children.SelectMany(x => OrderNode(x)));
            return result;
        }

        private void SetColumns(DbConnection connection, TableInfo table)
        {
            var showColumnsCommand = DbFactory.CreateShowColumnsCommand(table.Name);
            showColumnsCommand.Connection = connection;

            table.Columns = ExecuteArray(showColumnsCommand);
        }

        public string[] ExecuteArray(DbCommand command)
        {
            var list = new List<string>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    list.Add(result.GetString(0));
                }
            }
            return list.ToArray();
        }
    }
}
