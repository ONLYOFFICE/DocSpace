namespace ASC.Data.Backup.Tasks.Modules;

public enum ModuleName
{
    Audit,
    Calendar,
    Community,
    Core,
    Crm,
    Crm2,
    CrmInvoice,
    Files,
    Files2,
    Mail,
    Projects,
    Tenants,
    WebStudio
}

public interface IModuleSpecifics
{
    IEnumerable<RelationInfo> TableRelations { get; }
    IEnumerable<TableInfo> Tables { get; }
    ModuleName ModuleName { get; }
    string ConnectionStringName { get; }

    bool TryAdjustFilePath(bool dump, ColumnMapper columnMapper, ref string filePath);
    DbCommand CreateDeleteCommand(DbConnection connection, int tenantId, TableInfo table);
    DbCommand CreateInsertCommand(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row);
    DbCommand CreateSelectCommand(DbConnection connection, int tenantId, TableInfo table, int limit, int offset);
    IEnumerable<TableInfo> GetTablesOrdered();
    Stream PrepareData(string key, Stream data, ColumnMapper columnMapper);
    void PrepareData(DataTable data);
}
