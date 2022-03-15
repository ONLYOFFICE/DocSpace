namespace ASC.Data.Backup.Tasks.Modules;

public class WebStudioModuleSpecifics : ModuleSpecificsBase
{
    public override ModuleName ModuleName => ModuleName.WebStudio;
    public override IEnumerable<TableInfo> Tables => _tables;
    public override IEnumerable<RelationInfo> TableRelations => _relations;

    private static readonly Guid _crmSettingsId = new Guid("fdf39b9a-ec96-4eb7-aeab-63f2c608eada");

    private readonly TableInfo[] _tables = new[]
    {
            new TableInfo("webstudio_fckuploads", "TenantID") {InsertMethod = InsertMethod.None},
            new TableInfo("webstudio_settings", "TenantID") {UserIDColumns = new[] {"UserID"}},
            new TableInfo("webstudio_uservisit", "tenantid") {InsertMethod = InsertMethod.None}
        };

    private readonly RelationInfo[] _relations = new[]
    {
            new RelationInfo("crm_organisation_logo", "id", "webstudio_settings", "Data", typeof(CrmInvoiceModuleSpecifics),
                x => _crmSettingsId == new Guid(Convert.ToString(x["ID"])))
        };

    public WebStudioModuleSpecifics(Helpers helpers) : base(helpers) { }

    protected override bool TryPrepareValue(DbConnection connection, ColumnMapper columnMapper, RelationInfo relation, ref object value)
    {
        if (relation.ParentTable == "crm_organisation_logo")
        {
            var success = true;
            value = Regex.Replace(
                Convert.ToString(value),
                @"(?<=""CompanyLogoID"":)\d+",
                match =>
                {
                    if (Convert.ToInt32(match.Value) == 0)
                    {
                        success = true;

                        return match.Value;
                    }

                    var mappedMessageId = Convert.ToString(columnMapper.GetMapping(relation.ParentTable, relation.ParentColumn, match.Value));
                    success = !string.IsNullOrEmpty(mappedMessageId);

                    return mappedMessageId;
                });

            return success;
        }
        return base.TryPrepareValue(connection, columnMapper, relation, ref value);
    }
}
