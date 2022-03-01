namespace ASC.Webhooks.Core.Migrations;

public partial class WebhooksDbContextMySql : Migration
{
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "webhooks_config");

        migrationBuilder.DropTable(
            name: "webhooks_logs");
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "webhooks_config",
            columns: table => new
            {
                config_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                tenant_id = table.Column<uint>(type: "int unsigned", nullable: false),
                uri = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                    .Annotation("MySql:CharSet", "utf8mb4"),
                secret_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.config_id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "webhooks_logs",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                config_id = table.Column<int>(type: "int", nullable: false),
                uid = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                tenant_id = table.Column<uint>(type: "int unsigned", nullable: false),
                request_payload = table.Column<string>(type: "json", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                request_headers = table.Column<string>(type: "json", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                response_payload = table.Column<string>(type: "json", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                response_headers = table.Column<string>(type: "json", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                creation_time = table.Column<DateTime>(type: "datetime", nullable: false),
                @event = table.Column<string>(name: "event", type: "varchar(100)", maxLength: 100, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}
