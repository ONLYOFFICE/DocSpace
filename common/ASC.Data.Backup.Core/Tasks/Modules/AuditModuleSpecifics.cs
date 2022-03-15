namespace ASC.Data.Backup.Tasks.Modules;

public class AuditModuleSpecifics : ModuleSpecificsBase
{
    public override string ConnectionStringName => "core";
    public override ModuleName ModuleName => ModuleName.Audit;
    public override IEnumerable<TableInfo> Tables => _tables;
    public override IEnumerable<RelationInfo> TableRelations => Enumerable.Empty<RelationInfo>();

    private readonly TableInfo[] _tables = new[]
    {
            new TableInfo("audit_events", "tenant_id", "id")
            {
                UserIDColumns = new[] {"user_id"}
            },
            new TableInfo("login_events", "tenant_id", "id")
            {
                UserIDColumns = new[] {"user_id"}
            }
        };

    public AuditModuleSpecifics(Helpers helpers)
    : base(helpers) { }
}
