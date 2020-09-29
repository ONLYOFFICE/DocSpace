using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.TenantDbContextNpgsql
{
    public partial class TenantDbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "core_settings",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    id = table.Column<string>(maxLength: 128, nullable: false),
                    value = table.Column<byte[]>(nullable: false),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_settings_pkey", x => new { x.tenant, x.id });
                });

            migrationBuilder.CreateTable(
                name: "core_user",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<Guid>(maxLength: 38, nullable: false),
                    tenant = table.Column<int>(nullable: false),
                    username = table.Column<string>(maxLength: 255, nullable: false),
                    firstname = table.Column<string>(maxLength: 64, nullable: false),
                    lastname = table.Column<string>(maxLength: 64, nullable: false),
                    sex = table.Column<bool>(nullable: true),
                    bithdate = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    activation_status = table.Column<int>(nullable: false),
                    email = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    workfromdate = table.Column<DateTime>(nullable: true),
                    terminateddate = table.Column<DateTime>(nullable: true),
                    title = table.Column<string>(maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    culture = table.Column<string>(maxLength: 20, nullable: true, defaultValueSql: "NULL"),
                    contacts = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    phone = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    phone_activation = table.Column<int>(nullable: false),
                    location = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    notes = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sid = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sso_name_id = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sso_session_id = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    removed = table.Column<bool>(nullable: false),
                    create_on = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_user", x => x.id);
                });

            migrationBuilder.InsertData(
               schema: "onlyoffice",
               table: "core_user",
               columns: new[] { "id", "firstname", "lastname", "username", "tenant", "email", "workfromdate", "last_modified", "status", "activation_status", "phone_activation", "removed" },
               values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),"Administrator", "", "administrator", 1, " ", new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), 1, 0, 0, false });

            migrationBuilder.CreateTable(
                name: "tenants_forbiden",
                schema: "onlyoffice",
                columns: table => new
                {
                    address = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_forbiden_pkey", x => x.address);
                });

            migrationBuilder.CreateTable(
                name: "tenants_iprestrictions",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(nullable: false),
                    ip = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_iprestrictions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 255, nullable: false),
                    alias = table.Column<string>(maxLength: 100, nullable: false),
                    mappeddomain = table.Column<string>(maxLength: 100, nullable: true, defaultValueSql: "NULL"),
                    version = table.Column<int>(nullable: false, defaultValueSql: "2"),
                    Version_Changed = table.Column<DateTime>(nullable: true),
                    version_changed = table.Column<DateTime>(nullable: false),
                    language = table.Column<string>(fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                    timezone = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    trusteddomains = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    trusteddomainsenabled = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    status = table.Column<int>(nullable: false),
                    statuschanged = table.Column<DateTime>(nullable: true),
                    creationdatetime = table.Column<DateTime>(nullable: false),
                    owner_id = table.Column<Guid>(maxLength: 38, nullable: false, defaultValueSql: "NULL"),
                    @public = table.Column<bool>(name: "public", nullable: false),
                    publicvisibleproducts = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    payment_id = table.Column<string>(maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    industry = table.Column<int>(nullable: true),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_version",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    version = table.Column<string>(maxLength: 64, nullable: false),
                    url = table.Column<string>(maxLength: 64, nullable: false),
                    default_version = table.Column<int>(nullable: false),
                    visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_version", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "core_usergroup",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    userid = table.Column<Guid>(maxLength: 38, nullable: false),
                    groupid = table.Column<Guid>(maxLength: 38, nullable: false),
                    ref_type = table.Column<int>(nullable: false),
                    removed = table.Column<bool>(nullable: false),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usergroup_pkey", x => new { x.tenant, x.userid, x.groupid, x.ref_type });
                    
                });
            migrationBuilder.InsertData(
               schema: "onlyoffice",
               table: "core_usergroup",
               columns: new[] { "tenant", "userid", "groupid", "ref_type", "last_modified", "removed" },
               values: new object[] { 1, new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), "cd84e66b-b803-40fc-99f9-b2969a54a1de", 0, new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), false });

            migrationBuilder.CreateTable(
                name: "core_usersecurity",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(maxLength: 38, nullable: false),
                    tenant = table.Column<int>(nullable: false),
                    pwdhash = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    pwdhashsha512 = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usersecurity_pkey", x => x.userid);
                   
                });
            migrationBuilder.InsertData(
               schema: "onlyoffice",
               table: "core_usersecurity",
               columns: new[] { "userid", "tenant", "pwdhash", "pwdhashsha512" },
               values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 1, "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", "l/DFJ5yg4oh1F6Qp7uDhBw=="});

            
            migrationBuilder.CreateTable(
                name: "tenants_partners",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    partner_id = table.Column<string>(maxLength: 36, nullable: true, defaultValueSql: "NULL"),
                    affiliate_id = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    campaign = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_partners_pkey", x => x.tenant_id);
                    
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_forbiden",
                column: "address",
                values: new object[]
                {
                    "controlpanel",
                    "localhost"
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "name", "owner_id", "public", "status", "statuschanged", "version_changed", "Version_Changed" },
                values: new object[] { 1, "localhost", new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), null, "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), false, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.CreateIndex(
                name: "email",
                schema: "onlyoffice",
                table: "core_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "last_modified_core_user",
                schema: "onlyoffice",
                table: "core_user",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "username",
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "username", "tenant" });

            migrationBuilder.CreateIndex(
                name: "last_modified_core_usergroup",
                schema: "onlyoffice",
                table: "core_usergroup",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "IX_core_usergroup_userid",
                schema: "onlyoffice",
                table: "core_usergroup",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "pwdhash",
                schema: "onlyoffice",
                table: "core_usersecurity",
                column: "pwdhash");

            migrationBuilder.CreateIndex(
                name: "tenant_core_usersecurity",
                schema: "onlyoffice",
                table: "core_usersecurity",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "tenant_tenants_iprestrictions",
                schema: "onlyoffice",
                table: "tenants_iprestrictions",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "alias",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "last_modified_tenants_tenants",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "mappeddomain",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndex(
                name: "version",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usergroup",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usersecurity",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_forbiden",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_iprestrictions",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_partners",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_version",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_user",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
