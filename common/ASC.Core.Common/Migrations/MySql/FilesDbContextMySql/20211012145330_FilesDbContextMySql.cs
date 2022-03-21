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

namespace ASC.Core.Common.Migrations.MySql.FilesDbContextMySql;

public partial class FilesDbContextMySql : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "files_converts",
            columns: table => new
            {
                input = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                output = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.input, x.output });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.InsertData(
            table: "files_converts",
            columns: new[] { "input", "output" },
            values: new object[,]
            {
                    { ".csv", ".ods" },
                    { ".pps", ".odp" },
                    { ".pps", ".pdf" },
                    { ".pps", ".pptx" },
                    { ".ppsm", ".odp" },
                    { ".ppsm", ".pdf" },
                    { ".ppsm", ".pptx" },
                    { ".potx", ".pptx" },
                    { ".ppsx", ".odp" },
                    { ".ppsx", ".pptx" },
                    { ".ppt", ".odp" },
                    { ".ppt", ".pdf" },
                    { ".ppt", ".pptx" },
                    { ".pptm", ".odp" },
                    { ".pptm", ".pdf" },
                    { ".ppsx", ".pdf" },
                    { ".potx", ".pdf" },
                    { ".potx", ".odp" },
                    { ".potm", ".pptx" },
                    { ".ots", ".xlsx" },
                    { ".odt", ".docx" },
                    { ".odt", ".pdf" },
                    { ".odt", ".rtf" },
                    { ".odt", ".txt" },
                    { ".ott", ".docx" },
                    { ".ott", ".odt" },
                    { ".ott", ".pdf" },
                    { ".ott", ".rtf" },
                    { ".ott", ".txt" },
                    { ".pot", ".odp" },
                    { ".pot", ".pdf" },
                    { ".pot", ".pptx" },
                    { ".potm", ".odp" },
                    { ".potm", ".pdf" },
                    { ".pptm", ".pptx" },
                    { ".ots", ".pdf" },
                    { ".pptt", ".odp" },
                    { ".pptt", ".pptx" },
                    { ".xlst", ".xlsx" },
                    { ".xlst", ".csv" },
                    { ".xlst", ".ods" },
                    { ".xlt", ".csv" },
                    { ".xlt", ".ods" },
                    { ".xlt", ".pdf" },
                    { ".xlst", ".pdf" },
                    { ".xlt", ".xlsx" },
                    { ".xltm", ".ods" },
                    { ".xltm", ".pdf" },
                    { ".xltm", ".xlsx" },
                    { ".xltx", ".pdf" },
                    { ".xltx", ".csv" },
                    { ".xltx", ".ods" },
                    { ".xltm", ".csv" },
                    { ".xlsm", ".xlsx" },
                    { ".xlsm", ".ods" },
                    { ".xlsm", ".pdf" },
                    { ".pptx", ".odp" },
                    { ".pptx", ".pdf" },
                    { ".rtf", ".odp" },
                    { ".rtf", ".pdf" },
                    { ".rtf", ".docx" },
                    { ".rtf", ".txt" },
                    { ".txt", ".pdf" },
                    { ".txt", ".docx" },
                    { ".txt", ".odp" },
                    { ".txt", ".rtx" },
                    { ".xls", ".csv" },
                    { ".xls", ".ods" },
                    { ".xls", ".pdf" },
                    { ".xls", ".xlsx" },
                    { ".xlsm", ".csv" },
                    { ".pptt", ".pdf" },
                    { ".ots", ".ods" },
                    { ".ots", ".csv" },
                    { ".ods", ".xlsx" },
                    { ".dot", ".pdf" },
                    { ".dot", ".rtf" },
                    { ".dot", ".txt" },
                    { ".dotm", ".docx" },
                    { ".dotm", ".odt" },
                    { ".dotm", ".pdf" },
                    { ".dot", ".odt" },
                    { ".dotm", ".rtf" },
                    { ".dotx", ".docx" },
                    { ".dotx", ".odt" },
                    { ".dotx", ".pdf" },
                    { ".dotx", ".rtf" },
                    { ".dotx", ".txt" },
                    { ".epub", ".docx" },
                    { ".dotm", ".txt" },
                    { ".dot", ".docx" },
                    { ".docx", ".txt" },
                    { ".docx", ".rtf" },
                    { ".docx", ".docxf" },
                    { ".docxf",".docx" },
                    { ".docxf",".dotx" },
                    { ".docxf",".epub" },
                    { ".docxf",".fb2" },
                    { ".docxf",".html" },
                    { ".docxf",".odt" },
                    { ".docxf",".oform" },
                    { ".docxf",".ott" },
                    { ".docxf",".pdf" },
                    { ".docxf",".rtf" },
                    { ".docxf",".txt" },
                    { ".csv", ".pdf" },
                    { ".csv", ".xlsx" },
                    { ".doc", ".docx" },
                    { ".doc", ".odt" },
                    { ".doc", ".pdf" },
                    { ".doc", ".rtf" },
                    { ".doc", ".txt" },
                    { ".docm", ".docx" },
                    { ".docm", ".odt" },
                    { ".docm", ".pdf" },
                    { ".docm", ".rtf" },
                    { ".docm", ".txt" },
                    { ".doct", ".docx" },
                    { ".docx", ".odt" },
                    { ".docx", ".pdf" },
                    { ".epub", ".odt" },
                    { ".epub", ".pdf" },
                    { ".epub", ".rtf" },
                    { ".epub", ".txt" },
                    { ".html", ".pdf" },
                    { ".html", ".rtf" },
                    { ".html", ".txt" },
                    { ".mht", ".docx" },
                    { ".mht", ".odt" },
                    { ".mht", ".pdf" },
                    { ".mht", ".rtf" },
                    { ".mht", ".txt" },
                    { ".odp", ".pdf" },
                    { ".odp", ".pptx" },
                    { ".otp", ".odp" },
                    { ".otp", ".pdf" },
                    { ".otp", ".pptx" },
                    { ".ods", ".csv" },
                    { ".ods", ".pdf" },
                    { ".html", ".odt" },
                    { ".xltx", ".xlsx" },
                    { ".html", ".docx" },
                    { ".fodt", ".rtf" },
                    { ".fb2", ".docx" },
                    { ".fb2", ".odt" },
                    { ".fb2", ".pdf" },
                    { ".fb2", ".rtf" },
                    { ".fb2", ".txt" },
                    { ".fodp", ".odp" },
                    { ".fodp", ".pdf" },
                    { ".fodp", ".pptx" },
                    { ".fods", ".csv" },
                    { ".fods", ".ods" },
                    { ".fods", ".pdf" },
                    { ".fods", ".xlsx" },
                    { ".fodt", ".docx" },
                    { ".fodt", ".odt" },
                    { ".fodt", ".pdf" },
                    { ".fodt", ".txt" },
                    { ".xps", ".pdf" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "files_converts");
    }
}
