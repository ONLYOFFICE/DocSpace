namespace ASC.Data.Backup.Tasks.Data;

public enum RelationImportance
{
    Low,
    Normal
}

[DebuggerDisplay("({ParentTable},{ParentColumn})->({ChildTable},{ChildColumn})")]
public class RelationInfo
{
    public string ParentTable { get; private set; }
    public string ParentColumn { get; private set; }
    public string ChildTable { get; private set; }
    public string ChildColumn { get; private set; }
    public Type ParentModule { get; private set; }
    public RelationImportance Importance { get; private set; }
    public Func<DataRowInfo, bool> CollisionResolver { get; private set; }


    public RelationInfo(string parentTable, string parentColumn, string childTable, string childColumn)
        : this(parentTable, parentColumn, childTable, childColumn, null, null) { }

    public RelationInfo(string parentTable, string parentColumn, string childTable, string childColumn, Func<DataRowInfo, bool> collisionResolver)
        : this(parentTable, parentColumn, childTable, childColumn, null, collisionResolver) { }

    public RelationInfo(string parentTable, string parentColumn, string childTable, string childColumn, Type parentModule)
        : this(parentTable, parentColumn, childTable, childColumn, parentModule, null) { }

    public RelationInfo(string parentTable, string parentColumn, string childTable, string childColumn, Type parentModule, Func<DataRowInfo, bool> collisionResolver)
        : this(parentTable, parentColumn, childTable, childColumn, parentModule, collisionResolver, RelationImportance.Normal) { }

    public RelationInfo(string parentTable, string parentColumn, string childTable, string childColumn, Type parentModule, Func<DataRowInfo, bool> collisionResolver, RelationImportance importance)
    {
        ParentTable = parentTable;
        ParentColumn = parentColumn;
        ChildTable = childTable;
        ChildColumn = childColumn;
        ParentModule = parentModule;
        CollisionResolver = collisionResolver;
        Importance = importance;
    }

    public bool FitsForTable(string tableName)
    {
        return string.Equals(tableName, ChildTable, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool FitsForRow(DataRowInfo row)
    {
        return FitsForTable(row.TableName) && (CollisionResolver == null || CollisionResolver(row));
    }

    public bool IsExternal()
    {
        return ParentModule != null;
    }

    public bool IsSelfRelation()
    {
        return string.Equals(ParentTable, ChildTable, StringComparison.InvariantCultureIgnoreCase);
    }
}
