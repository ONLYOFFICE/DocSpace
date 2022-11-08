using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class FirebaseDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "firebase_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    firebase_device_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    application = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_subscribed = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("firebase_users_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "user_id",
                schema: "onlyoffice",
                table: "firebase_users",
                columns: new[] { "tenant_id", "user_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "firebase_users",
                schema: "onlyoffice");
        }
    }
}
