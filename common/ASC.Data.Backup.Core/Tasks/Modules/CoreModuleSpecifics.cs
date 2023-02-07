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

public class CoreModuleSpecifics : ModuleSpecificsBase
{
    public override ModuleName ModuleName => ModuleName.Core;
    public override IEnumerable<TableInfo> Tables => _tables;
    public override IEnumerable<RelationInfo> TableRelations => _tableRelations;

    private readonly RelationInfo[] _tableRelations;
    private readonly Helpers _helpers;
    private readonly TableInfo[] _tables = new[]
    {
            new TableInfo("core_acl", "tenant") {InsertMethod = InsertMethod.Ignore},
            new TableInfo("core_subscription", "tenant"),
            new TableInfo("core_subscriptionmethod", "tenant"),
            new TableInfo("core_userphoto", "tenant") {UserIDColumns = new[] {"userid"}},
            new TableInfo("core_usersecurity", "tenant") {UserIDColumns = new[] {"userid"}},
            new TableInfo("core_usergroup", "tenant") {UserIDColumns = new[] {"userid"}},
            new TableInfo("feed_aggregate", "tenant")
            {
                InsertMethod = InsertMethod.None,
                DateColumns = new Dictionary<string, bool> {{"created_date", false}, {"aggregated_date", false}}
            },
            new TableInfo("feed_readed", "tenant_id")
            {
                InsertMethod = InsertMethod.None,
                DateColumns = new Dictionary<string, bool> {{"timestamp", false}}
            },
            new TableInfo("feed_users") {InsertMethod = InsertMethod.None},
            new TableInfo("backup_schedule", "tenant_id"),
            new TableInfo("core_settings", "tenant")
        };

    public CoreModuleSpecifics(Helpers helpers) : base(helpers)
    {
        _helpers = helpers;
        _tableRelations = new[]
        {
                new RelationInfo("core_user", "id", "core_acl", "subject", typeof(TenantsModuleSpecifics)),
                new RelationInfo("core_group", "id", "core_acl", "subject", typeof(TenantsModuleSpecifics)),
                new RelationInfo("core_user", "id", "core_subscription", "recipient", typeof(TenantsModuleSpecifics)),
                new RelationInfo("core_group", "id", "core_subscription", "recipient", typeof(TenantsModuleSpecifics)),
                new RelationInfo("core_user", "id", "core_subscriptionmethod", "recipient", typeof(TenantsModuleSpecifics)),
                new RelationInfo("core_group", "id", "core_subscriptionmethod", "recipient", typeof(TenantsModuleSpecifics)),
                new RelationInfo("core_group", "id", "core_usergroup", "groupid", typeof(TenantsModuleSpecifics),
                                 x => !helpers.IsEmptyOrSystemGroup(Convert.ToString(x["groupid"]))),

                new RelationInfo("core_user", "id", "feed_users", "user_id", typeof(CoreModuleSpecifics)),

                new RelationInfo("files_folder", "id", "backup_schedule", "storage_base_path", typeof(FilesModuleSpecifics),
                                 x => IsDocumentsStorageType(Convert.ToString(x["storage_type"]))),
            };
    }

    protected override string GetSelectCommandConditionText(int tenantId, TableInfo table)
    {
        if (table.Name == "feed_users")
        {
            return "inner join core_user t1 on t1.id = t.user_id where t1.tenant = " + tenantId;
        }

        if (table.Name == "core_settings")
        {
            return string.Format("where t.{0} = {1} and id not in ('{2}')", table.TenantColumn, tenantId, LicenseReader.CustomerIdKey);
        }

        return base.GetSelectCommandConditionText(tenantId, table);
    }

    protected override bool TryPrepareValue(DbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, ref object value)
    {
        if (table.Name == "core_usergroup" && columnName == "last_modified")
        {
            value = DateTime.UtcNow;

            return true;
        }

        return base.TryPrepareValue(connection, columnMapper, table, columnName, ref value);
    }

    protected override bool TryPrepareRow(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row, out Dictionary<string, object> preparedRow)
    {
        if (table.Name == "core_acl")
        {
            if (int.Parse((string)row["tenant"]) == -1)
            {
                preparedRow = null;

                return false;
            }
        }

        return base.TryPrepareRow(dump, connection, columnMapper, table, row, out preparedRow);
    }

    protected override bool TryPrepareValue(DbConnection connection, ColumnMapper columnMapper, RelationInfo relation, ref object value)
    {
        if (relation.ChildTable == "core_acl" && relation.ChildColumn == "object")
        {
            var valParts = Convert.ToString(value).Split('|');

            var entityId = columnMapper.GetMapping(relation.ParentTable, relation.ParentColumn, valParts[1]);
            if (entityId == null)
            {
                return false;
            }

            value = string.Format("{0}|{1}", valParts[0], entityId);

            return true;
        }

        return base.TryPrepareValue(connection, columnMapper, relation, ref value);
    }

    protected override bool TryPrepareValue(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, IEnumerable<RelationInfo> relations, ref object value)
    {
        var relationList = relations.ToList();

        if (relationList.All(x => x.ChildTable == "core_subscription" && x.ChildColumn == "object" && x.ParentTable.StartsWith("projects_")))
        {
            var valParts = Convert.ToString(value).Split('_');

            var projectId = columnMapper.GetMapping("projects_projects", "id", valParts[2]);
            if (projectId == null)
            {
                return false;
            }

            var firstRelation = relationList.First(x => x.ParentTable != "projects_projects");
            var entityId = columnMapper.GetMapping(firstRelation.ParentTable, firstRelation.ParentColumn, valParts[1]);
            if (entityId == null)
            {
                return false;
            }

            value = string.Format("{0}_{1}_{2}", valParts[0], entityId, projectId);

            return true;
        }

        if (relationList.All(x => x.ChildTable == "core_subscription" && x.ChildColumn == "recipient")
            || relationList.All(x => x.ChildTable == "core_subscriptionmethod" && x.ChildColumn == "recipient")
            || relationList.All(x => x.ChildTable == "core_acl" && x.ChildColumn == "subject"))
        {
            var strVal = Convert.ToString(value);
            if (_helpers.IsEmptyOrSystemUser(strVal) || _helpers.IsEmptyOrSystemGroup(strVal))
            {
                return true;
            }

            foreach (var relation in relationList)
            {
                var mapping = columnMapper.GetMapping(relation.ParentTable, relation.ParentColumn, value);
                if (mapping != null)
                {
                    value = mapping;

                    return true;
                }
            }

            return false;
        }

        return base.TryPrepareValue(dump, connection, columnMapper, table, columnName, relationList, ref value);
    }

    private static bool ValidateSource(Guid expectedValue, DataRowInfo row)
    {
        var source = Convert.ToString(row["source"]);
        try
        {
            return expectedValue == new Guid(source);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsDocumentsStorageType(string strStorageType)
    {
        var storageType = int.Parse(strStorageType);

        return storageType == 0 || storageType == 1;
    }
}
