namespace ASC.Data.Backup.Tasks;

internal class RestoreDbModuleTask : PortalTaskBase
{
    private const int TransactionLength = 10000;

    private readonly IDataReadOperator _reader;
    private readonly IModuleSpecifics _module;
    private readonly ColumnMapper _columnMapper;
    private readonly bool _replaceDate;
    private readonly bool _dump;

    public RestoreDbModuleTask(IOptionsMonitor<ILog> options, IModuleSpecifics module, IDataReadOperator reader, ColumnMapper columnMapper, DbFactory factory, bool replaceDate, bool dump, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig, ModuleProvider moduleProvider)
        : base(factory, options, storageFactory, storageFactoryConfig, moduleProvider)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _columnMapper = columnMapper ?? throw new ArgumentNullException(nameof(columnMapper));
        DbFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        _module = module;
        _replaceDate = replaceDate;
        _dump = dump;
        Init(-1, null);
    }

    public override void RunJob()
    {
        Logger.DebugFormat("begin restore data for module {0}", _module.ModuleName);
        SetStepsCount(_module.Tables.Count(t => !IgnoredTables.Contains(t.Name)));

        using (var connection = DbFactory.OpenConnection())
        {
            foreach (var table in _module.GetTablesOrdered().Where(t => !IgnoredTables.Contains(t.Name) && t.InsertMethod != InsertMethod.None))
            {
                Logger.DebugFormat("begin restore table {0}", table.Name);

                var transactionsCommited = 0;
                var rowsInserted = 0;
                ActionInvoker.Try(
                    state =>
                        RestoreTable(connection.Fix(), (TableInfo)state, ref transactionsCommited,
                            ref rowsInserted), table, 5,
                    onAttemptFailure: error => _columnMapper.Rollback(),
                    onFailure: error => { throw ThrowHelper.CantRestoreTable(table.Name, error); });

                SetStepCompleted();
                Logger.DebugFormat("{0} rows inserted for table {1}", rowsInserted, table.Name);
            }
        }

        Logger.DebugFormat("end restore data for module {0}", _module.ModuleName);
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

    private void RestoreTable(DbConnection connection, TableInfo tableInfo, ref int transactionsCommited, ref int rowsInserted)
    {
        SetColumns(connection, tableInfo);

        using var stream = _reader.GetEntry(KeyHelper.GetTableZipKey(_module, tableInfo.Name));
        var lowImportanceRelations = _module
            .TableRelations
            .Where(
                r =>
                    string.Equals(r.ParentTable, tableInfo.Name, StringComparison.InvariantCultureIgnoreCase))
            .Where(r => r.Importance == RelationImportance.Low && !r.IsSelfRelation())
            .Select(r => Tuple.Create(r, _module.Tables.Single(t => t.Name == r.ChildTable)))
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
                if (_replaceDate)
                {
                    foreach (var column in tableInfo.DateColumns)
                    {
                        _columnMapper.SetDateMapping(tableInfo.Name, column, row[column.Key]);
                    }
                }

                object oldIdValue = null;
                object newIdValue = null;

                if (tableInfo.HasIdColumn())
                {
                    oldIdValue = row[tableInfo.IdColumn];
                    newIdValue = _columnMapper.GetMapping(tableInfo.Name, tableInfo.IdColumn, oldIdValue);
                    if (newIdValue == null)
                    {
                        if (tableInfo.IdType == IdType.Guid)
                        {
                            newIdValue = Guid.NewGuid().ToString("D");
                        }
                        else if (tableInfo.IdType == IdType.Integer)
                        {
                            var command = connection.CreateCommand();
                                command.CommandText = $"select max({tableInfo.IdColumn}) from {tableInfo.Name};";
                            newIdValue = (int)command.WithTimeout(120).ExecuteScalar() + 1;
                        }
                    }
                    if (newIdValue != null)
                    {
                        _columnMapper.SetMapping(tableInfo.Name, tableInfo.IdColumn, oldIdValue,
                            newIdValue);
                    }
                }

                var insertCommand = _module.CreateInsertCommand(_dump, connection, _columnMapper, tableInfo,
                    row);
                if (insertCommand == null)
                {
                    Logger.WarnFormat("Can't create command to insert row to {0} with values [{1}]", tableInfo,
                        row);
                    _columnMapper.Rollback();

                    continue;
                }
                insertCommand.WithTimeout(120).ExecuteNonQuery();
                rowsSuccess++;

                if (tableInfo.HasIdColumn() && tableInfo.IdType == IdType.Autoincrement)
                {
                    var lastIdCommand = DbFactory.CreateLastInsertIdCommand();
                    lastIdCommand.Connection = connection;
                    newIdValue = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    _columnMapper.SetMapping(tableInfo.Name, tableInfo.IdColumn, oldIdValue, newIdValue);
                }

                _columnMapper.Commit();

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
                    var newValue = _columnMapper.GetMapping(relation.Item1.ParentTable,
                        relation.Item1.ParentColumn, oldValue);
                    var command = connection.CreateCommand();
                    command.CommandText = string.Format("update {0} set {1} = {2} where {1} = {3} and {4} = {5}",
                            relation.Item1.ChildTable,
                            relation.Item1.ChildColumn,
                            newValue is string ? "'" + newValue + "'" : newValue,
                            oldValue is string ? "'" + oldValue + "'" : oldValue,
                            relation.Item2.TenantColumn,
                            _columnMapper.GetTenantMapping());
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
        {
            return Enumerable.Empty<DataRowInfo>();
        }

        var rows = DataRowInfoReader.ReadFromStream(xmlStream);

        var selfRelation = _module.TableRelations.SingleOrDefault(x => x.ChildTable == table.Name && x.IsSelfRelation());
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
}
