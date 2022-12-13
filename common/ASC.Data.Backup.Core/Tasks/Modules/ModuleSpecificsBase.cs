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

namespace ASC.Data.Backup.Tasks.Modules;

public abstract class ModuleSpecificsBase : IModuleSpecifics
{
    public abstract ModuleName ModuleName { get; }
    public abstract IEnumerable<TableInfo> Tables { get; }
    public abstract IEnumerable<RelationInfo> TableRelations { get; }
    public virtual string ConnectionStringName
        => _connectionStringName ??= ModuleName.ToString().ToLower();

    private string _connectionStringName;
    private readonly Helpers _helpers;

    protected ModuleSpecificsBase(Helpers helpers)
    {
        _helpers = helpers;
    }

    public IEnumerable<TableInfo> GetTablesOrdered()
    {
        var notOrderedTables = new List<TableInfo>(Tables);

        var totalTablesCount = notOrderedTables.Count;
        var orderedTablesCount = 0;
        while (orderedTablesCount < totalTablesCount)
        {
            var orderedTablesCountBeforeIter = orderedTablesCount; // ensure we not in infinite loop...

            var i = 0;
            while (i < notOrderedTables.Count)
            {
                var table = notOrderedTables[i];

                var parentTables = TableRelations
                    .Where(x => x.FitsForTable(table.Name) && !x.IsExternal() && !x.IsSelfRelation() && x.Importance != RelationImportance.Low)
                    .Select(x => x.ParentTable);

                if (parentTables.All(x => notOrderedTables.All(y => !string.Equals(y.Name, x, StringComparison.InvariantCultureIgnoreCase))))
                {
                    notOrderedTables.RemoveAt(i);
                    orderedTablesCount++;

                    yield return table;
                }
                else
                {
                    i++;
                }
            }

            if (orderedTablesCountBeforeIter == orderedTablesCount) // ensure we not in infinite loop...
            {
                throw ThrowHelper.CantOrderTables(notOrderedTables.Select(x => x.Name));
            }
        }
    }

    public DbCommand CreateSelectCommand(DbConnection connection, int tenantId, TableInfo table, int limit, int offset)
    {
        var command = connection.CreateCommand();
        command.CommandText = string.Format("select t.* from {0} as t {1} limit {2},{3};", table.Name, GetSelectCommandConditionText(tenantId, table), offset, limit);

        return command;
    }

    public DbCommand CreateSelectCommand(DbConnection connection, int tenantId, TableInfo table, int limit, int offset, Guid id)
    {
        var command = connection.CreateCommand();

        var conditionUserText = GetContitionUserText(table, id);

        command.CommandText = string.Format("select t.* from {0} as t {1} {2} limit {3},{4};", table.Name, GetSelectCommandConditionText(tenantId, table, id), conditionUserText, offset, limit);

        return command; 
    }

    private string GetSelectCommandConditionText(int tenantId, TableInfo table, Guid id)
    {
        var conditionText = GetSelectCommandConditionText(tenantId, table);
        if (table.Name == "files_folder_tree")
        {
            conditionText += $" and t1.create_by = '{id}'";
        }
        if (table.Name == "files_bunch_objects")
        {
            conditionText = $"inner join files_folder as t1 on t1.id = t.left_node {conditionText} and t1.create_by = '{id}'";
        }
        return conditionText;
    }

    private string GetContitionUserText(TableInfo table, Guid id)
    {
        if (table.UserIDColumns.Any())
        {
            return "and t." + table.UserIDColumns[0] + " = '" + id + "' ";
        }
        else
        {
            return "";
        }
    }

    public DbCommand CreateDeleteCommand(DbConnection connection, int tenantId, TableInfo table)
    {
        var command = connection.CreateCommand();
        command.CommandText = $"delete t.* from {table.Name} as t {GetDeleteCommandConditionText(tenantId, table)};";

        return command;
    }

    public DbCommand CreateInsertCommand(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row)
    {
        if (table.InsertMethod == InsertMethod.None)
        {
            return null;
        }

        if (!TryPrepareRow(dump, connection, columnMapper, table, row, out var valuesForInsert))
        {
            return null;
        }

        var columns = valuesForInsert.Keys.Intersect(table.Columns).ToArray();
        var insert = table.InsertMethod != InsertMethod.Ignore
                                              ? table.InsertMethod.ToString().ToLower()
                                                  : "insert ignore";
        var insertCommantText = $"{insert} into {table.Name}({string.Join(",", columns)}) values({string.Join(",", columns.Select(c => "@" + c))});";

        var command = connection.CreateCommand();
        command.CommandText = insertCommantText;

        foreach (var parameter in valuesForInsert)
        {
            AddParameter(command, parameter.Key, parameter.Value);
        }

        return command;
    }

    public DbCommand AddParameter(DbCommand command, string name, object value)
    {
        var p = command.CreateParameter();
        if (!string.IsNullOrEmpty(name))
        {
            p.ParameterName = name.StartsWith('@') ? name : "@" + name;
        }

        p.Value = GetParameterValue(value);

        command.Parameters.Add(p);

        return command;
    }

    public object GetParameterValue(object value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }

        if (value is Enum @enum)
        {
            return @enum.ToString("d");
        }

        if (value is DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, DateTimeKind.Unspecified);
        }

        return value;
    }

    public virtual bool TryAdjustFilePath(bool dump, ColumnMapper columnMapper, ref string filePath)
    {
        return true;
    }

    public virtual void PrepareData(DataTable data)
    {
        // nothing to do
    }

    public virtual Stream PrepareData(string key, Stream stream, ColumnMapper columnMapper)
    {
        return stream;
    }

    protected virtual string GetSelectCommandConditionText(int tenantId, TableInfo table)
    {
        if (!table.HasTenantColumn())
        {
            throw ThrowHelper.CantDetectTenant(table.Name);
        }

        return string.Format("where t.{0} = {1}", table.TenantColumn, tenantId);
    }

    protected virtual string GetDeleteCommandConditionText(int tenantId, TableInfo table)
    {
        return GetSelectCommandConditionText(tenantId, table);
    }

    protected virtual bool TryPrepareRow(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row, out Dictionary<string, object> preparedRow)
    {
        preparedRow = new Dictionary<string, object>();

        var parentRelations = TableRelations
            .Where(x => x.FitsForRow(row) && x.Importance != RelationImportance.Low)
            .GroupBy(x => x.ChildColumn)
            .ToDictionary(x => x.Key);

        foreach (var columnName in row.ColumnNames)
        {
            if (table.IdType == IdType.Autoincrement && columnName.Equals(table.IdColumn, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var val = row[columnName];
            if (!parentRelations.ContainsKey(columnName))
            {
                if (!TryPrepareValue(connection, columnMapper, table, columnName, ref val))
                {
                    return false;
                }
            }
            else
            {
                if (!TryPrepareValue(dump, connection, columnMapper, table, columnName, parentRelations[columnName], ref val))
                {
                    return false;
                }

                if (!table.HasIdColumn() && !table.HasTenantColumn() && val == row[columnName])
                {
                    return false;
                }
            }

            preparedRow.Add(columnName, val);
        }

        return true;
    }

    protected virtual bool TryPrepareValue(DbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, ref object value)
    {
        if (columnName.Equals(table.TenantColumn, StringComparison.OrdinalIgnoreCase))
        {
            var tenantMapping = columnMapper.GetTenantMapping();
            if (tenantMapping < 1)
            {
                return false;
            }

            value = tenantMapping;

            return true;
        }

        if (table.UserIDColumns.Any(x => columnName.Equals(x, StringComparison.OrdinalIgnoreCase)))
        {
            var strVal = Convert.ToString(value);
            var userMapping = columnMapper.GetUserMapping(strVal);
            if (userMapping == null)
            {
                return _helpers.IsEmptyOrSystemUser(strVal);
            }

            value = userMapping;

            return true;
        }

        var mapping = columnMapper.GetMapping(table.Name, columnName, value);
        if (mapping != null)
        {
            value = mapping;
        }

        return true;
    }

    protected virtual bool TryPrepareValue(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, IEnumerable<RelationInfo> relations, ref object value)
    {
        return TryPrepareValue(connection, columnMapper, relations.Single(), ref value);
    }

    protected virtual bool TryPrepareValue(DbConnection connection, ColumnMapper columnMapper, RelationInfo relation, ref object value)
    {
        var mappedValue = columnMapper.GetMapping(relation.ParentTable, relation.ParentColumn, value);
        if (mappedValue != null)
        {
            value = mappedValue;

            return true;
        }

        return value == null ||
            Guid.TryParse(Convert.ToString(value), out _) ||
            int.TryParse(Convert.ToString(value), out _);
    }
}
