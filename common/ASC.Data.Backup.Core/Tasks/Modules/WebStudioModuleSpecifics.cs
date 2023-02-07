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

public class WebStudioModuleSpecifics : ModuleSpecificsBase
{
    public override ModuleName ModuleName => ModuleName.WebStudio;
    public override IEnumerable<TableInfo> Tables => _tables;
    public override IEnumerable<RelationInfo> TableRelations => _relations;

    private readonly TableInfo[] _tables = new[]
    {
            new TableInfo("webstudio_fckuploads", "TenantID") {InsertMethod = InsertMethod.None},
            new TableInfo("webstudio_settings", "TenantID") {UserIDColumns = new[] {"UserID"}},
            new TableInfo("webstudio_uservisit", "tenantid") {InsertMethod = InsertMethod.None}
        };

    private readonly RelationInfo[] _relations = new RelationInfo[0];

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
