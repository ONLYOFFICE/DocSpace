namespace ASC.Data.Backup.Tasks.Modules;

public class CalendarModuleSpecifics : ModuleSpecificsBase
{
    public override ModuleName ModuleName => ModuleName.Calendar;
    public override IEnumerable<TableInfo> Tables => _tables;
    public override IEnumerable<RelationInfo> TableRelations => _tableRelations;

    private readonly RelationInfo[] _tableRelations;
    private readonly TableInfo[] _tables = new[]
    {
            new TableInfo("calendar_calendars", "tenant", "id") {UserIDColumns = new[] {"owner_id"}},
            new TableInfo("calendar_calendar_item"),
            new TableInfo("calendar_calendar_user") {UserIDColumns = new[] {"user_id"}},
            new TableInfo("calendar_events", "tenant", "id")
            {
                UserIDColumns = new[] {"owner_id"},
                DateColumns = new Dictionary<string, bool> {{"start_date", true}, {"end_date", true}}
            },
            new TableInfo("calendar_event_history", "tenant"),
            new TableInfo("calendar_event_item"),
            new TableInfo("calendar_event_user") {UserIDColumns = new[] {"user_id"}},
            new TableInfo("calendar_notifications", "tenant")
            {
                UserIDColumns = new[] {"user_id"},
                DateColumns = new Dictionary<string, bool> {{"notify_date", true}}
            }
        };

    public CalendarModuleSpecifics(Helpers helpers)
        : base(helpers)
    {
        _tableRelations = new[]
        {
                new RelationInfo("calendar_calendars", "id", "calendar_calendar_item", "calendar_id"),
                new RelationInfo("calendar_calendars", "id", "calendar_calendar_user", "calendar_id"),
                new RelationInfo("calendar_calendars", "id", "calendar_events", "calendar_id"),
                new RelationInfo("calendar_calendars", "id", "calendar_event_history", "calendar_id"),
                new RelationInfo("calendar_events", "id", "calendar_event_history", "event_id"),
                new RelationInfo("calendar_events", "id", "calendar_event_item", "event_id"),
                new RelationInfo("calendar_events", "id", "calendar_event_user", "event_id"),
                new RelationInfo("calendar_events", "id", "calendar_notifications", "event_id"),
                new RelationInfo("core_user", "id", "calendar_calendar_item", "item_id", typeof(TenantsModuleSpecifics),
                    x => Convert.ToInt32(x["is_group"]) == 0),
                new RelationInfo("core_group", "id", "calendar_calendar_item", "item_id", typeof(TenantsModuleSpecifics),
                    x => Convert.ToInt32(x["is_group"]) == 1 && !helpers.IsEmptyOrSystemGroup(Convert.ToString(x["item_id"]))),
                new RelationInfo("core_user", "id", "calendar_event_item", "item_id", typeof(TenantsModuleSpecifics),
                    x => Convert.ToInt32(x["is_group"]) == 0),
                new RelationInfo("core_group", "id", "calendar_event_item", "item_id", typeof(TenantsModuleSpecifics),
                    x => Convert.ToInt32(x["is_group"]) == 1 && !helpers.IsEmptyOrSystemGroup(Convert.ToString(x["item_id"])))
            };
    }

    protected override string GetSelectCommandConditionText(int tenantId, TableInfo table)
    {
        if (table.Name == "calendar_calendar_item" || table.Name == "calendar_calendar_user")
        {
            return "inner join calendar_calendars as t1 on t1.id = t.calendar_id where t1.tenant = " + tenantId;
        }

        if (table.Name == "calendar_event_item" || table.Name == "calendar_event_user")
        {
            return "inner join calendar_events as t1 on t1.id = t.event_id where t1.tenant = " + tenantId;
        }

        if (table.Name == "calendar_event_history")
        {
            return string.Format(
                "inner join calendar_calendars as t1 on t1.id = t.calendar_id  and t1.tenant = t.tenant  inner join calendar_events as t2 on t2.id = t.event_id where t1.tenant = {0} and t2.tenant = {0}",
                tenantId);
        }

        return base.GetSelectCommandConditionText(tenantId, table);
    }
}
