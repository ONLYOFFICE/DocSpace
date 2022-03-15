namespace ASC.Data.Backup.Tasks.Data;

public enum InsertMethod
{
    None,
    Insert,
    Replace,
    Ignore
}

public enum IdType
{
    Autoincrement,
    Guid,
    Integer
}

[DebuggerDisplay("{Name}")]
public class TableInfo
{
    public string[] Columns { get; set; }
    public string[] UserIDColumns { get; set; }
    public Dictionary<string, bool> DateColumns { get; set; }
    public InsertMethod InsertMethod { get; set; }
    public string Name { get; private set; }
    public string IdColumn { get; private set; }
    public IdType IdType { get; private set; }
    public string TenantColumn { get; private set; }

    public TableInfo(string name, string tenantColumn = null, string idColumn = null, IdType idType = IdType.Autoincrement)
    {
        Name = name;
        IdColumn = idColumn;
        IdType = idType;
        TenantColumn = tenantColumn;
        DateColumns = new Dictionary<string, bool>();
        InsertMethod = InsertMethod.Insert;
    }

    public bool HasIdColumn()
    {
        return !string.IsNullOrEmpty(IdColumn);
    }

    public bool HasDateColumns()
    {
            return DateColumns.Count > 0;
    }

    public bool HasTenantColumn()
    {
        return !string.IsNullOrEmpty(TenantColumn);
    }

    public override string ToString()
    {
        return string.Format("{0} {1} [{2} ({3}), {4}]", InsertMethod, Name, IdColumn, IdType, TenantColumn);
    }
}
