using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.MySql.Migrations.FilesDb
{
    /// <inheritdoc />
    public partial class FilesDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".dps", ".odp" },
                    { ".dps", ".otp" },
                    { ".dps", ".pdf" },
                    { ".dps", ".potm" },
                    { ".dps", ".potx" },
                    { ".dps", ".ppsm" },
                    { ".dps", ".ppsx" },
                    { ".dps", ".pptm" },
                    { ".dps", ".pptx" },
                    { ".dpt", ".odp" },
                    { ".dpt", ".otp" },
                    { ".dpt", ".pdf" },
                    { ".dpt", ".potm" },
                    { ".dpt", ".potx" },
                    { ".dpt", ".ppsm" },
                    { ".dpt", ".ppsx" },
                    { ".dpt", ".pptm" },
                    { ".dpt", ".pptx" },
                    { ".et", ".csv" },
                    { ".et", ".ods" },
                    { ".et", ".ots" },
                    { ".et", ".pdf" },
                    { ".et", ".xlsm" },
                    { ".et", ".xlsx" },
                    { ".et", ".xltm" },
                    { ".et", ".xltx" },
                    { ".ett", ".csv" },
                    { ".ett", ".ods" },
                    { ".ett", ".ots" },
                    { ".ett", ".pdf" },
                    { ".ett", ".xlsm" },
                    { ".ett", ".xlsx" },
                    { ".ett", ".xltm" },
                    { ".ett", ".xltx" },
                    { ".htm", ".docm" },
                    { ".htm", ".docx" },
                    { ".htm", ".dotm" },
                    { ".htm", ".dotx" },
                    { ".htm", ".epub" },
                    { ".htm", ".fb2" },
                    { ".htm", ".html" },
                    { ".htm", ".odt" },
                    { ".htm", ".ott" },
                    { ".htm", ".pdf" },
                    { ".htm", ".rtf" },
                    { ".htm", ".txt" },
                    { ".mht", ".html" },
                    { ".mhtml", ".docm" },
                    { ".mhtml", ".docx" },
                    { ".mhtml", ".dotm" },
                    { ".mhtml", ".dotx" },
                    { ".mhtml", ".epub" },
                    { ".mhtml", ".fb2" },
                    { ".mhtml", ".html" },
                    { ".mhtml", ".odt" },
                    { ".mhtml", ".ott" },
                    { ".mhtml", ".pdf" },
                    { ".mhtml", ".rtf" },
                    { ".mhtml", ".txt" },
                    { ".stw", ".docm" },
                    { ".stw", ".docx" },
                    { ".stw", ".dotm" },
                    { ".stw", ".dotx" },
                    { ".stw", ".epub" },
                    { ".stw", ".fb2" },
                    { ".stw", ".html" },
                    { ".stw", ".odt" },
                    { ".stw", ".ott" },
                    { ".stw", ".pdf" },
                    { ".stw", ".rtf" },
                    { ".stw", ".txt" },
                    { ".sxc", ".csv" },
                    { ".sxc", ".ods" },
                    { ".sxc", ".ots" },
                    { ".sxc", ".pdf" },
                    { ".sxc", ".xlsm" },
                    { ".sxc", ".xlsx" },
                    { ".sxc", ".xltm" },
                    { ".sxc", ".xltx" },
                    { ".sxi", ".odp" },
                    { ".sxi", ".otp" },
                    { ".sxi", ".pdf" },
                    { ".sxi", ".potm" },
                    { ".sxi", ".potx" },
                    { ".sxi", ".ppsm" },
                    { ".sxi", ".ppsx" },
                    { ".sxi", ".pptm" },
                    { ".sxi", ".pptx" },
                    { ".sxw", ".docm" },
                    { ".sxw", ".docx" },
                    { ".sxw", ".dotm" },
                    { ".sxw", ".dotx" },
                    { ".sxw", ".epub" },
                    { ".sxw", ".fb2" },
                    { ".sxw", ".html" },
                    { ".sxw", ".odt" },
                    { ".sxw", ".ott" },
                    { ".sxw", ".pdf" },
                    { ".sxw", ".rtf" },
                    { ".sxw", ".txt" },
                    { ".wps", ".docm" },
                    { ".wps", ".docx" },
                    { ".wps", ".dotm" },
                    { ".wps", ".dotx" },
                    { ".wps", ".epub" },
                    { ".wps", ".fb2" },
                    { ".wps", ".html" },
                    { ".wps", ".odt" },
                    { ".wps", ".ott" },
                    { ".wps", ".pdf" },
                    { ".wps", ".rtf" },
                    { ".wps", ".txt" },
                    { ".wpt", ".docm" },
                    { ".wpt", ".docx" },
                    { ".wpt", ".dotm" },
                    { ".wpt", ".dotx" },
                    { ".wpt", ".epub" },
                    { ".wpt", ".fb2" },
                    { ".wpt", ".html" },
                    { ".wpt", ".odt" },
                    { ".wpt", ".ott" },
                    { ".wpt", ".pdf" },
                    { ".wpt", ".rtf" },
                    { ".wpt", ".txt" },
                    { ".xlsb", ".csv" },
                    { ".xlsb", ".ods" },
                    { ".xlsb", ".ots" },
                    { ".xlsb", ".pdf" },
                    { ".xlsb", ".xlsm" },
                    { ".xlsb", ".xlsx" },
                    { ".xlsb", ".xltm" },
                    { ".xlsb", ".xltx" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".odp" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".otp" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".potm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".potx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".ppsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".ppsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".pptm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dps", ".pptx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".odp" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".otp" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".potm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".potx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".ppsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".ppsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".pptm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".dpt", ".pptx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".csv" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".ods" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".ots" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".xlsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".xlsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".xltm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".et", ".xltx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".csv" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".ods" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".ots" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".xlsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".xlsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".xltm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".ett", ".xltx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".docm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".docx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".dotm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".dotx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".epub" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".fb2" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".odt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".ott" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".rtf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".htm", ".txt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mht", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".docm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".docx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".dotm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".dotx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".epub" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".fb2" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".odt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".ott" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".rtf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".mhtml", ".txt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".docm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".docx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".dotm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".dotx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".epub" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".fb2" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".odt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".ott" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".rtf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".stw", ".txt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".csv" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".ods" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".ots" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".xlsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".xlsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".xltm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxc", ".xltx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".odp" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".otp" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".potm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".potx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".ppsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".ppsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".pptm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxi", ".pptx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".docm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".docx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".dotm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".dotx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".epub" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".fb2" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".odt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".ott" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".rtf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".sxw", ".txt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".docm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".docx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".dotm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".dotx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".epub" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".fb2" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".odt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".ott" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".rtf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wps", ".txt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".docm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".docx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".dotm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".dotx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".epub" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".fb2" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".html" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".odt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".ott" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".rtf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".wpt", ".txt" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".csv" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".ods" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".ots" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".pdf" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".xlsm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".xlsx" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".xltm" });

            migrationBuilder.DeleteData(
                table: "files_converts",
                keyColumns: new[] { "input", "output" },
                keyValues: new object[] { ".xlsb", ".xltx" });
        }
    }
}
