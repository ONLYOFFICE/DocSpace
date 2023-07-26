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

public class TenantsModuleSpecifics : ModuleSpecificsBase
{
    public override string ConnectionStringName => "core";
    public override ModuleName ModuleName => ModuleName.Tenants;
    public override IEnumerable<TableInfo> Tables => _tables;
    public override IEnumerable<RelationInfo> TableRelations => _tableRelations;

    private readonly CoreSettings _coreSettings;

    private readonly TableInfo[] _tables = new[]
    {
            new TableInfo("tenants_quota", "tenant"),
            new TableInfo("tenants_tariff", "tenant", "id"),
            new TableInfo("tenants_tenants", "id", "id")
            {
                DateColumns = new Dictionary<string, bool> {{"creationdatetime", false}, {"statuschanged", false}, {"version_changed", false}}
            },
            new TableInfo("tenants_tariffrow", "tenant") {InsertMethod = InsertMethod.Replace},
            new TableInfo("tenants_quotarow", "tenant") {InsertMethod = InsertMethod.Replace},
            new TableInfo("core_user", "tenant", "id", IdType.Guid)
            {
                DateColumns = new Dictionary<string, bool> {{"workfromdate", false}, {"terminateddate", false}, {"last_modified", false}},
                UserIDColumns = new[] { "id" }
            },
            new TableInfo("core_group", "tenant", "id", IdType.Guid),
            new TableInfo("tenants_iprestrictions", "tenant", "id", IdType.Autoincrement)
        };

    private readonly RelationInfo[] _tableRelations = new[]
    {
            new RelationInfo("tenants_tenants", "id", "tenants_quota", "tenant"),
            new RelationInfo("tenants_tenants", "id", "tenants_tariff", "tenant"),
            new RelationInfo("tenants_tenants", "id", "tenants_tariff", "tariff"),
            new RelationInfo("tenants_tariff", "id", "tenants_tariffrow", "tariff_id"),
            new RelationInfo("core_user", "id", "tenants_tenants", "owner_id", null, null, RelationImportance.Low)
        };

    public TenantsModuleSpecifics(CoreSettings coreSettings, Helpers helpers)
        : base(helpers)
    {
        _coreSettings = coreSettings;
    }

    protected override bool TryPrepareRow(bool dump, DbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row, out Dictionary<string, object> preparedRow)
    {
        if (table.Name == "tenants_tenants" && string.IsNullOrEmpty(Convert.ToString(row["payment_id"])))
        {
            var oldTenantID = Convert.ToInt32(row["id"]);
            columnMapper.SetMapping("tenants_tenants", "payment_id", row["payment_id"], _coreSettings.GetKey(oldTenantID));
        }

        return base.TryPrepareRow(dump, connection, columnMapper, table, row, out preparedRow);
    }

    protected override bool TryPrepareValue(DbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, ref object value)
    {
        //we insert tenant as suspended so it can't be accessed before restore operation is finished
        if (table.Name.Equals("tenants_tenants", StringComparison.InvariantCultureIgnoreCase) &&
            columnName.Equals("status", StringComparison.InvariantCultureIgnoreCase))
        {
            value = (int)TenantStatus.Restoring;

            return true;
        }

        if (table.Name.Equals("tenants_tenants", StringComparison.InvariantCultureIgnoreCase) &&
            columnName.Equals("last_modified", StringComparison.InvariantCultureIgnoreCase))
        {
            value = DateTime.UtcNow;

            return true;
        }

        if (table.Name.Equals("tenants_quotarow", StringComparison.InvariantCultureIgnoreCase) &&
            columnName.Equals("last_modified", StringComparison.InvariantCultureIgnoreCase))
        {
            value = DateTime.UtcNow;

            return true;
        }

        if ((table.Name == "core_user" || table.Name == "core_group") && columnName == "last_modified")
        {
            value = DateTime.UtcNow.AddMinutes(2);

            return true;
        }

        return base.TryPrepareValue(connection, columnMapper, table, columnName, ref value);
    }
}
