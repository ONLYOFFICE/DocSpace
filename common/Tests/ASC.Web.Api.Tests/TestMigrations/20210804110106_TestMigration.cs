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

using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations;

public partial class TestMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "core_user",
            keyColumn: "id",
            keyValue: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
            columns: new[] { "last_modified", "workfromdate" },
            values: new object[] { new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), new DateTime(2021, 8, 4, 11, 1, 4, 512, DateTimeKind.Utc).AddTicks(9355) });

        migrationBuilder.InsertData(
            table: "core_user",
            columns: new[] { "id", "activation_status", "bithdate", "contacts", "culture", "email", "firstname", "last_modified", "lastname", "location", "notes", "phone", "phone_activation", "removed", "sex", "sid", "sso_name_id", "sso_session_id", "status", "tenant", "terminateddate", "title", "username", "workfromdate", "create_on" },
            values: new object[] { "99223c7b-e3c9-11eb-9063-982cbc0ea1e5", 0, null, null, null, "test@gmail.com", "Test", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(2928), "User", null, null, null, 0, false, null, null, null, null, 1, 1, null, null, "TestUser", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(2940), DateTime.UtcNow });

        migrationBuilder.InsertData(
            table: "core_usersecurity",
            columns: new[] { "userid", "pwdhash", "tenant" },
            values: new object[] { "99223c7b-e3c9-11eb-9063-982cbc0ea1e5", "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", 1 });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "core_usersecurity",
            keyColumn: "userid",
            keyValue: "99223c7b-e3c9-11eb-9063-982cbc0ea1e5");

        migrationBuilder.DeleteData(
            table: "core_user",
            keyColumn: "id",
            keyValue: "99223c7b-e3c9-11eb-9063-982cbc0ea1e5");

        migrationBuilder.UpdateData(
            table: "core_user",
            keyColumn: "id",
            keyValue: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
            columns: new[] { "last_modified", "workfromdate" },
            values: new object[] { new DateTime(2021, 8, 3, 21, 35, 0, 522, DateTimeKind.Utc).AddTicks(6893), new DateTime(2021, 8, 3, 21, 35, 0, 522, DateTimeKind.Utc).AddTicks(5587) });
    }
}