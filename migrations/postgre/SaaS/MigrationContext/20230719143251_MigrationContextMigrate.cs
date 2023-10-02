using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "account_links",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    uid = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    provider = table.Column<string>(type: "character(60)", fixedLength: true, maxLength: 60, nullable: true, defaultValueSql: "NULL"),
                    profile = table.Column<string>(type: "text", nullable: false),
                    linked = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_links_pkey", x => new { x.id, x.uid });
                });

            migrationBuilder.CreateTable(
                name: "dbip_lookup",
                columns: table => new
                {
                    addrtype = table.Column<string>(name: "addr_type", type: "enum('ipv4','ipv6')", nullable: false),
                    ipstart = table.Column<byte[]>(name: "ip_start", type: "varbinary(16)", nullable: false),
                    ipend = table.Column<byte[]>(name: "ip_end", type: "varbinary(16)", nullable: false),
                    continent = table.Column<string>(type: "char(2)", nullable: false),
                    country = table.Column<string>(type: "char(2)", nullable: false),
                    stateprovcode = table.Column<string>(name: "stateprov_code", type: "varchar(15)", nullable: true),
                    stateprov = table.Column<string>(type: "varchar(80)", nullable: false),
                    district = table.Column<string>(type: "varchar(80)", nullable: false),
                    city = table.Column<string>(type: "varchar(80)", nullable: false),
                    zipcode = table.Column<string>(type: "varchar(20)", nullable: true),
                    latitude = table.Column<float>(type: "float", nullable: false),
                    longitude = table.Column<float>(type: "float", nullable: false),
                    geonameid = table.Column<int>(name: "geoname_id", type: "int(10)", nullable: true),
                    timezoneoffset = table.Column<float>(name: "timezone_offset", type: "float", nullable: false),
                    timezonename = table.Column<string>(name: "timezone_name", type: "varchar(64)", nullable: false),
                    weathercode = table.Column<string>(name: "weather_code", type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbip_lookup", x => new { x.addrtype, x.ipstart });
                });

            migrationBuilder.CreateTable(
                name: "feed_last",
                schema: "onlyoffice",
                columns: table => new
                {
                    lastkey = table.Column<string>(name: "last_key", type: "character varying(128)", maxLength: 128, nullable: false),
                    lastdate = table.Column<DateTime>(name: "last_date", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_last_pkey", x => x.lastkey);
                });

            migrationBuilder.CreateTable(
                name: "files_converts",
                schema: "onlyoffice",
                columns: table => new
                {
                    input = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    output = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_converts_pkey", x => new { x.input, x.output });
                });

            migrationBuilder.CreateTable(
                name: "hosting_instance_registration",
                columns: table => new
                {
                    instanceregistrationid = table.Column<string>(name: "instance_registration_id", type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    lastupdated = table.Column<DateTime>(name: "last_updated", type: "datetime", nullable: true),
                    workertypename = table.Column<string>(name: "worker_type_name", type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    isactive = table.Column<bool>(name: "is_active", type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.instanceregistrationid);
                });

            migrationBuilder.CreateTable(
                name: "mobile_app_install",
                schema: "onlyoffice",
                columns: table => new
                {
                    useremail = table.Column<string>(name: "user_email", type: "character varying(255)", maxLength: 255, nullable: false),
                    apptype = table.Column<int>(name: "app_type", type: "integer", nullable: false),
                    registeredon = table.Column<DateTime>(name: "registered_on", type: "timestamp with time zone", nullable: false),
                    lastsign = table.Column<DateTime>(name: "last_sign", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mobile_app_install_pkey", x => new { x.useremail, x.apptype });
                });

            migrationBuilder.CreateTable(
                name: "notify_info",
                schema: "onlyoffice",
                columns: table => new
                {
                    notifyid = table.Column<int>(name: "notify_id", type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    modifydate = table.Column<DateTime>(name: "modify_date", type: "timestamp with time zone", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notify_info_pkey", x => x.notifyid);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Region = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    ConnectionString = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Region);
                });

            migrationBuilder.CreateTable(
                name: "tenants_forbiden",
                schema: "onlyoffice",
                columns: table => new
                {
                    address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_forbiden_pkey", x => x.address);
                });

            migrationBuilder.CreateTable(
                name: "tenants_quota",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    features = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValueSql: "0.00"),
                    productid = table.Column<string>(name: "product_id", type: "character varying(128)", maxLength: 128, nullable: true, defaultValueSql: "NULL"),
                    visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_quota_pkey", x => x.tenant);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mappeddomain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValueSql: "NULL"),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "2"),
                    versionchanged = table.Column<DateTime>(name: "version_changed", type: "timestamp with time zone", nullable: true),
                    language = table.Column<string>(type: "character(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    trusteddomains = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    trusteddomainsenabled = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    status = table.Column<int>(type: "integer", nullable: false),
                    statuschanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ownerid = table.Column<Guid>(name: "owner_id", type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    paymentid = table.Column<string>(name: "payment_id", type: "character varying(38)", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    industry = table.Column<int>(type: "integer", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
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
                    id = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    url = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    defaultversion = table.Column<int>(name: "default_version", type: "integer", nullable: false),
                    visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_version", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "webhooks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "''"),
                    method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "webstudio_index",
                schema: "onlyoffice",
                columns: table => new
                {
                    indexname = table.Column<string>(name: "index_name", type: "character varying(50)", maxLength: 50, nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_index_pkey", x => x.indexname);
                });

            migrationBuilder.CreateTable(
                name: "audit_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    initiator = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    target = table.Column<string>(type: "text", nullable: true),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    platform = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", fixedLength: true, maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_events_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "backup_backup",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char", maxLength: 38, nullable: false, collation: "utf8_general_ci"),
                    tenantid = table.Column<int>(name: "tenant_id", type: "int", maxLength: 10, nullable: false),
                    isscheduled = table.Column<int>(name: "is_scheduled", type: "int", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, collation: "utf8_general_ci"),
                    storagetype = table.Column<int>(name: "storage_type", type: "int", maxLength: 10, nullable: false),
                    storagebasepath = table.Column<string>(name: "storage_base_path", type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci"),
                    storagepath = table.Column<string>(name: "storage_path", type: "character varying(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    createdon = table.Column<DateTime>(name: "created_on", type: "datetime", nullable: false),
                    expireson = table.Column<DateTime>(name: "expires_on", type: "datetime", nullable: false, defaultValueSql: "'0001-01-01 00:00:00'"),
                    storageparams = table.Column<string>(name: "storage_params", type: "text", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci"),
                    removed = table.Column<int>(type: "int", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_backup_backup_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "backup_schedule",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", maxLength: 10, nullable: false),
                    cron = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    backupsstored = table.Column<int>(name: "backups_stored", type: "integer", maxLength: 10, nullable: false),
                    storagetype = table.Column<int>(name: "storage_type", type: "integer", maxLength: 10, nullable: false),
                    storagebasepath = table.Column<string>(name: "storage_base_path", type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci"),
                    lastbackuptime = table.Column<DateTime>(name: "last_backup_time", type: "datetime", nullable: false),
                    storageparams = table.Column<string>(name: "storage_params", type: "text", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenantid);
                    table.ForeignKey(
                        name: "FK_backup_schedule_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_acl",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    subject = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    action = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    @object = table.Column<string>(name: "object", type: "character varying(255)", maxLength: 255, nullable: false, defaultValueSql: "''"),
                    acetype = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_acl_pkey", x => new { x.tenant, x.subject, x.action, x.@object });
                    table.ForeignKey(
                        name: "FK_core_acl_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    categoryid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    parentid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    sid = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    removed = table.Column<bool>(type: "boolean", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_core_group_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_settings",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<byte[]>(type: "bytea", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_settings_pkey", x => new { x.tenant, x.id });
                    table.ForeignKey(
                        name: "FK_core_settings_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_subscription",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<string>(type: "character varying(38)", maxLength: 38, nullable: false),
                    action = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    recipient = table.Column<string>(type: "character varying(38)", maxLength: 38, nullable: false),
                    @object = table.Column<string>(name: "object", type: "character varying(128)", maxLength: 128, nullable: false),
                    unsubscribed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_subscription_pkey", x => new { x.tenant, x.source, x.action, x.recipient, x.@object });
                    table.ForeignKey(
                        name: "FK_core_subscription_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_subscriptionmethod",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<string>(type: "character varying(38)", maxLength: 38, nullable: false),
                    action = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    recipient = table.Column<string>(type: "character varying(38)", maxLength: 38, nullable: false),
                    sender = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_subscriptionmethod_pkey", x => new { x.tenant, x.source, x.action, x.recipient });
                    table.ForeignKey(
                        name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_user",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    firstname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    lastname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    sex = table.Column<bool>(type: "boolean", nullable: true),
                    bithdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    activationstatus = table.Column<int>(name: "activation_status", type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    workfromdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    terminateddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    culture = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "NULL"),
                    contacts = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    phone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    phoneactivation = table.Column<int>(name: "phone_activation", type: "integer", nullable: false),
                    location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    notes = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sid = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    ssonameid = table.Column<string>(name: "sso_name_id", type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    ssosessionid = table.Column<string>(name: "sso_session_id", type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    removed = table.Column<bool>(type: "boolean", nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_core_user_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_userdav",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_userdav_pkey", x => new { x.tenantid, x.userid });
                    table.ForeignKey(
                        name: "FK_core_userdav_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_usergroup",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    groupid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    reftype = table.Column<int>(name: "ref_type", type: "integer", nullable: false),
                    removed = table.Column<bool>(type: "boolean", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usergroup_pkey", x => new { x.tenant, x.userid, x.groupid, x.reftype });
                    table.ForeignKey(
                        name: "FK_core_usergroup_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_userphoto",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    photo = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_userphoto_pkey", x => x.userid);
                    table.ForeignKey(
                        name: "FK_core_userphoto_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_usersecurity",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    pwdhash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usersecurity_pkey", x => x.userid);
                    table.ForeignKey(
                        name: "FK_core_usersecurity_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_bus_integration_event_log",
                columns: table => new
                {
                    eventid = table.Column<Guid>(name: "event_id", type: "char(38)", nullable: false, collation: "utf8_general_ci"),
                    eventtypename = table.Column<string>(name: "event_type_name", type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    state = table.Column<int>(type: "int(11)", nullable: false),
                    timessent = table.Column<int>(name: "times_sent", type: "int(11)", nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "datetime", nullable: false),
                    createby = table.Column<Guid>(name: "create_by", type: "char(38)", nullable: false, collation: "utf8_general_ci"),
                    content = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci"),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    tenantid = table.Column<int>(name: "tenant_id", type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.eventid);
                    table.ForeignKey(
                        name: "FK_event_bus_integration_event_log_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feed_aggregate",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    product = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    author = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    modifiedby = table.Column<Guid>(name: "modified_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false),
                    modifieddate = table.Column<DateTime>(name: "modified_date", type: "timestamp with time zone", nullable: false),
                    groupid = table.Column<string>(name: "group_id", type: "character varying(70)", maxLength: 70, nullable: true, defaultValueSql: "NULL"),
                    aggregateddate = table.Column<DateTime>(name: "aggregated_date", type: "timestamp with time zone", nullable: false),
                    json = table.Column<string>(type: "text", nullable: false),
                    keywords = table.Column<string>(type: "text", nullable: true),
                    contextid = table.Column<string>(name: "context_id", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_aggregate", x => x.id);
                    table.ForeignKey(
                        name: "FK_feed_aggregate_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feed_readed",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_readed_pkey", x => new { x.userid, x.tenantid, x.module });
                    table.ForeignKey(
                        name: "FK_feed_readed_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_bunch_objects",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    rightnode = table.Column<string>(name: "right_node", type: "character varying(255)", maxLength: 255, nullable: false),
                    leftnode = table.Column<string>(name: "left_node", type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_bunch_objects_pkey", x => new { x.tenantid, x.rightnode });
                    table.ForeignKey(
                        name: "FK_files_bunch_objects_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_file",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    versiongroup = table.Column<int>(name: "version_group", type: "integer", nullable: false, defaultValueSql: "1"),
                    currentversion = table.Column<bool>(name: "current_version", type: "boolean", nullable: false),
                    folderid = table.Column<int>(name: "folder_id", type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    contentlength = table.Column<long>(name: "content_length", type: "bigint", nullable: false, defaultValueSql: "'0'::bigint"),
                    filestatus = table.Column<int>(name: "file_status", type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                    modifiedby = table.Column<Guid>(name: "modified_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp with time zone", nullable: false),
                    convertedtype = table.Column<string>(name: "converted_type", type: "character varying(10)", maxLength: 10, nullable: true, defaultValueSql: "NULL::character varying"),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    changes = table.Column<string>(type: "text", nullable: true),
                    encrypted = table.Column<bool>(type: "boolean", nullable: false),
                    forcesave = table.Column<int>(type: "integer", nullable: false),
                    thumb = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_file_pkey", x => new { x.id, x.tenantid, x.version });
                    table.ForeignKey(
                        name: "FK_files_file_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_folder",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parentid = table.Column<int>(name: "parent_id", type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    foldertype = table.Column<int>(name: "folder_type", type: "integer", nullable: false),
                    createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                    modifiedby = table.Column<Guid>(name: "modified_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp with time zone", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    foldersCount = table.Column<int>(type: "integer", nullable: false),
                    filesCount = table.Column<int>(type: "integer", nullable: false),
                    @private = table.Column<bool>(name: "private", type: "boolean", nullable: false),
                    haslogo = table.Column<bool>(name: "has_logo", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_folder", x => x.id);
                    table.ForeignKey(
                        name: "FK_files_folder_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_link",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    sourceid = table.Column<string>(name: "source_id", type: "character varying(32)", maxLength: 32, nullable: false),
                    linkedid = table.Column<string>(name: "linked_id", type: "character varying(32)", maxLength: 32, nullable: false),
                    linkedfor = table.Column<Guid>(name: "linked_for", type: "uuid", fixedLength: true, maxLength: 38, nullable: false, defaultValueSql: "NULL::bpchar")
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_link_pkey", x => new { x.tenantid, x.sourceid, x.linkedid });
                    table.ForeignKey(
                        name: "FK_files_link_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_properties",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    entryid = table.Column<string>(name: "entry_id", type: "character varying(50)", maxLength: 50, nullable: false),
                    data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_properties_pkey", x => new { x.tenantid, x.entryid });
                    table.ForeignKey(
                        name: "FK_files_properties_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_security",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    entryid = table.Column<string>(name: "entry_id", type: "character varying(50)", maxLength: 50, nullable: false),
                    entrytype = table.Column<int>(name: "entry_type", type: "integer", nullable: false),
                    subject = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    subjecttype = table.Column<int>(name: "subject_type", type: "integer", nullable: false),
                    owner = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    security = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    options = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_security_pkey", x => new { x.tenantid, x.entryid, x.entrytype, x.subject });
                    table.ForeignKey(
                        name: "FK_files_security_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_tag",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    owner = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    flag = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_tag", x => x.id);
                    table.ForeignKey(
                        name: "FK_files_tag_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_tag_link",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    tagid = table.Column<int>(name: "tag_id", type: "integer", nullable: false),
                    entrytype = table.Column<int>(name: "entry_type", type: "integer", nullable: false),
                    entryid = table.Column<string>(name: "entry_id", type: "character varying(32)", maxLength: 32, nullable: false),
                    createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: true, defaultValueSql: "NULL::bpchar"),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: true),
                    tagcount = table.Column<int>(name: "tag_count", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_tag_link_pkey", x => new { x.tenantid, x.tagid, x.entrytype, x.entryid });
                    table.ForeignKey(
                        name: "FK_files_tag_link_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_account",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "'0'::character varying"),
                    customertitle = table.Column<string>(name: "customer_title", type: "character varying(400)", maxLength: 400, nullable: false),
                    username = table.Column<string>(name: "user_name", type: "character varying(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    token = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false),
                    foldertype = table.Column<int>(name: "folder_type", type: "integer", nullable: false),
                    roomtype = table.Column<int>(name: "room_type", type: "integer", nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    folderid = table.Column<string>(name: "folder_id", type: "text", nullable: true),
                    @private = table.Column<bool>(name: "private", type: "boolean", nullable: false),
                    haslogo = table.Column<bool>(name: "has_logo", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_thirdparty_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_files_thirdparty_account_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_app",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false),
                    app = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    token = table.Column<string>(type: "text", nullable: true),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_thirdparty_app_pkey", x => new { x.userid, x.app });
                    table.ForeignKey(
                        name: "FK_files_thirdparty_app_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_id_mapping",
                schema: "onlyoffice",
                columns: table => new
                {
                    hashid = table.Column<string>(name: "hash_id", type: "character(32)", fixedLength: true, maxLength: 32, nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_thirdparty_id_mapping_pkey", x => x.hashid);
                    table.ForeignKey(
                        name: "FK_files_thirdparty_id_mapping_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "firebase_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 36, nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    firebasedevicetoken = table.Column<string>(name: "firebase_device_token", type: "character varying(255)", maxLength: 255, nullable: true),
                    application = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    issubscribed = table.Column<bool>(name: "is_subscribed", type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("firebase_users_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_firebase_users_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL::character varying"),
                    platform = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_login_events_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notify_queue",
                schema: "onlyoffice",
                columns: table => new
                {
                    notifyid = table.Column<int>(name: "notify_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    sender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    reciever = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    subject = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    contenttype = table.Column<string>(name: "content_type", type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    content = table.Column<string>(type: "text", nullable: true),
                    sendertype = table.Column<string>(name: "sender_type", type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    replyto = table.Column<string>(name: "reply_to", type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    creationdate = table.Column<DateTime>(name: "creation_date", type: "timestamp with time zone", nullable: false),
                    attachments = table.Column<string>(type: "text", nullable: true),
                    autosubmitted = table.Column<string>(name: "auto_submitted", type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notify_queue_pkey", x => x.notifyid);
                    table.ForeignKey(
                        name: "FK_notify_queue_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telegram_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    portaluserid = table.Column<Guid>(name: "portal_user_id", type: "uuid", maxLength: 38, nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    telegramuserid = table.Column<long>(name: "telegram_user_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("telegram_users_pkey", x => new { x.tenantid, x.portaluserid });
                    table.ForeignKey(
                        name: "FK_telegram_users_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenants_iprestrictions",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    foradmin = table.Column<bool>(name: "for_admin", type: "TINYINT(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_iprestrictions", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenants_iprestrictions_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenants_quotarow",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    tag = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "'0'"),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 36, nullable: false, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_quotarow_pkey", x => new { x.tenant, x.path });
                    table.ForeignKey(
                        name: "FK_tenants_quotarow_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tariff",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    customerid = table.Column<string>(name: "customer_id", type: "character varying(255)", maxLength: 255, nullable: false, defaultValueSql: "NULL"),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenants_tariff_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tariffrow",
                schema: "onlyoffice",
                columns: table => new
                {
                    tariffid = table.Column<int>(name: "tariff_id", type: "int", nullable: false),
                    quota = table.Column<int>(type: "int", nullable: false),
                    tenant = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.tariffid, x.quota });
                    table.ForeignKey(
                        name: "FK_tenants_tariffrow_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webhooks_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    secretkey = table.Column<string>(name: "secret_key", type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    uri = table.Column<string>(type: "text", nullable: true, defaultValueSql: "''"),
                    enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    ssl = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_webhooks_config_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webstudio_settings",
                schema: "onlyoffice",
                columns: table => new
                {
                    TenantID = table.Column<int>(type: "integer", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false),
                    UserID = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_settings_pkey", x => new { x.TenantID, x.ID, x.UserID });
                    table.ForeignKey(
                        name: "FK_webstudio_settings_tenants_tenants_TenantID",
                        column: x => x.TenantID,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(type: "integer", nullable: false),
                    visitdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    productid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    userid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    visitcount = table.Column<int>(type: "integer", nullable: false),
                    firstvisittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lastvisittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_uservisit_pkey", x => new { x.tenantid, x.visitdate, x.productid, x.userid });
                    table.ForeignKey(
                        name: "FK_webstudio_uservisit_tenants_tenants_tenantid",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feed_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    feedid = table.Column<string>(name: "feed_id", type: "character varying(88)", maxLength: 88, nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", fixedLength: true, maxLength: 38, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_users_pkey", x => new { x.feedid, x.userid });
                    table.ForeignKey(
                        name: "FK_feed_users_feed_aggregate_feed_id",
                        column: x => x.feedid,
                        principalSchema: "onlyoffice",
                        principalTable: "feed_aggregate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "files_folder_tree",
                schema: "onlyoffice",
                columns: table => new
                {
                    folderid = table.Column<int>(name: "folder_id", type: "integer", nullable: false),
                    parentid = table.Column<int>(name: "parent_id", type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_folder_tree_pkey", x => new { x.parentid, x.folderid });
                    table.ForeignKey(
                        name: "FK_files_folder_tree_files_folder_folder_id",
                        column: x => x.folderid,
                        principalSchema: "onlyoffice",
                        principalTable: "files_folder",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webhooks_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    configid = table.Column<int>(name: "config_id", type: "int", nullable: false),
                    creationtime = table.Column<DateTime>(name: "creation_time", type: "datetime", nullable: false),
                    webhookid = table.Column<int>(name: "webhook_id", type: "int", nullable: false),
                    requestheaders = table.Column<string>(name: "request_headers", type: "json", nullable: true),
                    requestpayload = table.Column<string>(name: "request_payload", type: "text", nullable: false),
                    responseheaders = table.Column<string>(name: "response_headers", type: "json", nullable: true),
                    responsepayload = table.Column<string>(name: "response_payload", type: "text", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    uid = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    delivery = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_webhooks_logs_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_webhooks_logs_webhooks_config_config_id",
                        column: x => x.configid,
                        principalTable: "webhooks_config",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webplugins",
                schema: "onlyoffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    license = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    author = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    homepage = table.Column<string>(name: "home_page", type: "character varying(255)", maxLength: 255, nullable: true),
                    pluginname = table.Column<string>(name: "plugin_name", type: "character varying(255)", maxLength: 255, nullable: false),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 36, nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    system = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webplugins_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_webplugins_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".csv", ".ods" },
                    { ".csv", ".ots" },
                    { ".csv", ".pdf" },
                    { ".csv", ".xlsm" },
                    { ".csv", ".xlsx" },
                    { ".csv", ".xltm" },
                    { ".csv", ".xltx" },
                    { ".doc", ".docm" },
                    { ".doc", ".docx" },
                    { ".doc", ".dotm" },
                    { ".doc", ".dotx" },
                    { ".doc", ".epub" },
                    { ".doc", ".fb2" },
                    { ".doc", ".html" },
                    { ".doc", ".odt" },
                    { ".doc", ".ott" },
                    { ".doc", ".pdf" },
                    { ".doc", ".rtf" },
                    { ".doc", ".txt" },
                    { ".docm", ".docx" },
                    { ".docm", ".dotm" },
                    { ".docm", ".dotx" },
                    { ".docm", ".epub" },
                    { ".docm", ".fb2" },
                    { ".docm", ".html" },
                    { ".docm", ".odt" },
                    { ".docm", ".ott" },
                    { ".docm", ".pdf" },
                    { ".docm", ".rtf" },
                    { ".docm", ".txt" },
                    { ".doct", ".docx" },
                    { ".docx", ".docm" },
                    { ".docx", ".docxf" },
                    { ".docx", ".dotm" },
                    { ".docx", ".dotx" },
                    { ".docx", ".epub" },
                    { ".docx", ".fb2" },
                    { ".docx", ".html" },
                    { ".docx", ".odt" },
                    { ".docx", ".ott" },
                    { ".docx", ".pdf" },
                    { ".docx", ".rtf" },
                    { ".docx", ".txt" },
                    { ".docxf", ".docm" },
                    { ".docxf", ".docx" },
                    { ".docxf", ".dotm" },
                    { ".docxf", ".dotx" },
                    { ".docxf", ".epub" },
                    { ".docxf", ".fb2" },
                    { ".docxf", ".html" },
                    { ".docxf", ".odt" },
                    { ".docxf", ".oform" },
                    { ".docxf", ".ott" },
                    { ".docxf", ".pdf" },
                    { ".docxf", ".rtf" },
                    { ".docxf", ".txt" },
                    { ".dot", ".docm" },
                    { ".dot", ".docx" },
                    { ".dot", ".dotm" },
                    { ".dot", ".dotx" },
                    { ".dot", ".epub" },
                    { ".dot", ".fb2" },
                    { ".dot", ".html" },
                    { ".dot", ".odt" },
                    { ".dot", ".ott" },
                    { ".dot", ".pdf" },
                    { ".dot", ".rtf" },
                    { ".dot", ".txt" },
                    { ".dotm", ".docm" },
                    { ".dotm", ".docx" },
                    { ".dotm", ".dotx" },
                    { ".dotm", ".epub" },
                    { ".dotm", ".fb2" },
                    { ".dotm", ".html" },
                    { ".dotm", ".odt" },
                    { ".dotm", ".ott" },
                    { ".dotm", ".pdf" },
                    { ".dotm", ".rtf" },
                    { ".dotm", ".txt" },
                    { ".dotx", ".docm" },
                    { ".dotx", ".docx" },
                    { ".dotx", ".dotm" },
                    { ".dotx", ".epub" },
                    { ".dotx", ".fb2" },
                    { ".dotx", ".html" },
                    { ".dotx", ".odt" },
                    { ".dotx", ".ott" },
                    { ".dotx", ".pdf" },
                    { ".dotx", ".rtf" },
                    { ".dotx", ".txt" },
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
                    { ".epub", ".docm" },
                    { ".epub", ".docx" },
                    { ".epub", ".dotm" },
                    { ".epub", ".dotx" },
                    { ".epub", ".fb2" },
                    { ".epub", ".html" },
                    { ".epub", ".odt" },
                    { ".epub", ".ott" },
                    { ".epub", ".pdf" },
                    { ".epub", ".rtf" },
                    { ".epub", ".txt" },
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
                    { ".fb2", ".docm" },
                    { ".fb2", ".docx" },
                    { ".fb2", ".dotm" },
                    { ".fb2", ".dotx" },
                    { ".fb2", ".epub" },
                    { ".fb2", ".html" },
                    { ".fb2", ".odt" },
                    { ".fb2", ".ott" },
                    { ".fb2", ".pdf" },
                    { ".fb2", ".rtf" },
                    { ".fb2", ".txt" },
                    { ".fodp", ".odp" },
                    { ".fodp", ".otp" },
                    { ".fodp", ".pdf" },
                    { ".fodp", ".potm" },
                    { ".fodp", ".potx" },
                    { ".fodp", ".ppsm" },
                    { ".fodp", ".ppsx" },
                    { ".fodp", ".pptm" },
                    { ".fodp", ".pptx" },
                    { ".fods", ".csv" },
                    { ".fods", ".ods" },
                    { ".fods", ".ots" },
                    { ".fods", ".pdf" },
                    { ".fods", ".xlsm" },
                    { ".fods", ".xlsx" },
                    { ".fods", ".xltm" },
                    { ".fods", ".xltx" },
                    { ".fodt", ".docm" },
                    { ".fodt", ".docx" },
                    { ".fodt", ".dotm" },
                    { ".fodt", ".dotx" },
                    { ".fodt", ".epub" },
                    { ".fodt", ".fb2" },
                    { ".fodt", ".html" },
                    { ".fodt", ".odt" },
                    { ".fodt", ".ott" },
                    { ".fodt", ".pdf" },
                    { ".fodt", ".rtf" },
                    { ".fodt", ".txt" },
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
                    { ".html", ".docm" },
                    { ".html", ".docx" },
                    { ".html", ".dotm" },
                    { ".html", ".dotx" },
                    { ".html", ".epub" },
                    { ".html", ".fb2" },
                    { ".html", ".odt" },
                    { ".html", ".ott" },
                    { ".html", ".pdf" },
                    { ".html", ".rtf" },
                    { ".html", ".txt" },
                    { ".mht", ".docm" },
                    { ".mht", ".docx" },
                    { ".mht", ".dotm" },
                    { ".mht", ".dotx" },
                    { ".mht", ".epub" },
                    { ".mht", ".fb2" },
                    { ".mht", ".html" },
                    { ".mht", ".odt" },
                    { ".mht", ".ott" },
                    { ".mht", ".pdf" },
                    { ".mht", ".rtf" },
                    { ".mht", ".txt" },
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
                    { ".odp", ".otp" },
                    { ".odp", ".pdf" },
                    { ".odp", ".potm" },
                    { ".odp", ".potx" },
                    { ".odp", ".ppsm" },
                    { ".odp", ".ppsx" },
                    { ".odp", ".pptm" },
                    { ".odp", ".pptx" },
                    { ".ods", ".csv" },
                    { ".ods", ".ots" },
                    { ".ods", ".pdf" },
                    { ".ods", ".xlsm" },
                    { ".ods", ".xlsx" },
                    { ".ods", ".xltm" },
                    { ".ods", ".xltx" },
                    { ".odt", ".docm" },
                    { ".odt", ".docx" },
                    { ".odt", ".dotm" },
                    { ".odt", ".dotx" },
                    { ".odt", ".epub" },
                    { ".odt", ".fb2" },
                    { ".odt", ".html" },
                    { ".odt", ".ott" },
                    { ".odt", ".pdf" },
                    { ".odt", ".rtf" },
                    { ".odt", ".txt" },
                    { ".otp", ".odp" },
                    { ".otp", ".pdf" },
                    { ".otp", ".potm" },
                    { ".otp", ".potx" },
                    { ".otp", ".ppsm" },
                    { ".otp", ".ppsx" },
                    { ".otp", ".pptm" },
                    { ".otp", ".pptx" },
                    { ".ots", ".csv" },
                    { ".ots", ".ods" },
                    { ".ots", ".pdf" },
                    { ".ots", ".xlsm" },
                    { ".ots", ".xlsx" },
                    { ".ots", ".xltm" },
                    { ".ots", ".xltx" },
                    { ".ott", ".docm" },
                    { ".ott", ".docx" },
                    { ".ott", ".dotm" },
                    { ".ott", ".dotx" },
                    { ".ott", ".epub" },
                    { ".ott", ".fb2" },
                    { ".ott", ".html" },
                    { ".ott", ".odt" },
                    { ".ott", ".pdf" },
                    { ".ott", ".rtf" },
                    { ".ott", ".txt" },
                    { ".oxps", ".docm" },
                    { ".oxps", ".docx" },
                    { ".oxps", ".dotm" },
                    { ".oxps", ".dotx" },
                    { ".oxps", ".epub" },
                    { ".oxps", ".fb2" },
                    { ".oxps", ".html" },
                    { ".oxps", ".odt" },
                    { ".oxps", ".ott" },
                    { ".oxps", ".pdf" },
                    { ".oxps", ".rtf" },
                    { ".oxps", ".txt" },
                    { ".pdf", ".docm" },
                    { ".pdf", ".docx" },
                    { ".pdf", ".dotm" },
                    { ".pdf", ".dotx" },
                    { ".pdf", ".epub" },
                    { ".pdf", ".fb2" },
                    { ".pdf", ".html" },
                    { ".pdf", ".odt" },
                    { ".pdf", ".ott" },
                    { ".pdf", ".rtf" },
                    { ".pdf", ".txt" },
                    { ".pot", ".odp" },
                    { ".pot", ".otp" },
                    { ".pot", ".pdf" },
                    { ".pot", ".potm" },
                    { ".pot", ".potx" },
                    { ".pot", ".ppsm" },
                    { ".pot", ".ppsx" },
                    { ".pot", ".pptm" },
                    { ".pot", ".pptx" },
                    { ".potm", ".odp" },
                    { ".potm", ".otp" },
                    { ".potm", ".pdf" },
                    { ".potm", ".potx" },
                    { ".potm", ".ppsm" },
                    { ".potm", ".ppsx" },
                    { ".potm", ".pptm" },
                    { ".potm", ".pptx" },
                    { ".potx", ".odp" },
                    { ".potx", ".otp" },
                    { ".potx", ".pdf" },
                    { ".potx", ".potm" },
                    { ".potx", ".ppsm" },
                    { ".potx", ".ppsx" },
                    { ".potx", ".pptm" },
                    { ".potx", ".pptx" },
                    { ".pps", ".odp" },
                    { ".pps", ".otp" },
                    { ".pps", ".pdf" },
                    { ".pps", ".potm" },
                    { ".pps", ".potx" },
                    { ".pps", ".ppsm" },
                    { ".pps", ".ppsx" },
                    { ".pps", ".pptm" },
                    { ".pps", ".pptx" },
                    { ".ppsm", ".odp" },
                    { ".ppsm", ".otp" },
                    { ".ppsm", ".pdf" },
                    { ".ppsm", ".potm" },
                    { ".ppsm", ".potx" },
                    { ".ppsm", ".ppsx" },
                    { ".ppsm", ".pptm" },
                    { ".ppsm", ".pptx" },
                    { ".ppsx", ".odp" },
                    { ".ppsx", ".otp" },
                    { ".ppsx", ".pdf" },
                    { ".ppsx", ".potm" },
                    { ".ppsx", ".potx" },
                    { ".ppsx", ".ppsm" },
                    { ".ppsx", ".pptm" },
                    { ".ppsx", ".pptx" },
                    { ".ppt", ".odp" },
                    { ".ppt", ".otp" },
                    { ".ppt", ".pdf" },
                    { ".ppt", ".potm" },
                    { ".ppt", ".potx" },
                    { ".ppt", ".ppsm" },
                    { ".ppt", ".ppsx" },
                    { ".ppt", ".pptm" },
                    { ".ppt", ".pptx" },
                    { ".pptm", ".odp" },
                    { ".pptm", ".otp" },
                    { ".pptm", ".pdf" },
                    { ".pptm", ".potm" },
                    { ".pptm", ".potx" },
                    { ".pptm", ".ppsm" },
                    { ".pptm", ".ppsx" },
                    { ".pptm", ".pptx" },
                    { ".pptt", ".pptx" },
                    { ".pptx", ".odp" },
                    { ".pptx", ".otp" },
                    { ".pptx", ".pdf" },
                    { ".pptx", ".potm" },
                    { ".pptx", ".potx" },
                    { ".pptx", ".ppsm" },
                    { ".pptx", ".ppsx" },
                    { ".pptx", ".pptm" },
                    { ".rtf", ".docm" },
                    { ".rtf", ".docx" },
                    { ".rtf", ".dotm" },
                    { ".rtf", ".dotx" },
                    { ".rtf", ".epub" },
                    { ".rtf", ".fb2" },
                    { ".rtf", ".html" },
                    { ".rtf", ".odt" },
                    { ".rtf", ".ott" },
                    { ".rtf", ".pdf" },
                    { ".rtf", ".txt" },
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
                    { ".txt", ".docm" },
                    { ".txt", ".docx" },
                    { ".txt", ".dotm" },
                    { ".txt", ".dotx" },
                    { ".txt", ".epub" },
                    { ".txt", ".fb2" },
                    { ".txt", ".html" },
                    { ".txt", ".odt" },
                    { ".txt", ".ott" },
                    { ".txt", ".pdf" },
                    { ".txt", ".rtf" },
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
                    { ".xls", ".csv" },
                    { ".xls", ".ods" },
                    { ".xls", ".ots" },
                    { ".xls", ".pdf" },
                    { ".xls", ".xlsm" },
                    { ".xls", ".xlsx" },
                    { ".xls", ".xltm" },
                    { ".xls", ".xltx" },
                    { ".xlsb", ".csv" },
                    { ".xlsb", ".ods" },
                    { ".xlsb", ".ots" },
                    { ".xlsb", ".pdf" },
                    { ".xlsb", ".xlsm" },
                    { ".xlsb", ".xlsx" },
                    { ".xlsb", ".xltm" },
                    { ".xlsb", ".xltx" },
                    { ".xlsm", ".csv" },
                    { ".xlsm", ".ods" },
                    { ".xlsm", ".ots" },
                    { ".xlsm", ".pdf" },
                    { ".xlsm", ".xlsx" },
                    { ".xlsm", ".xltm" },
                    { ".xlsm", ".xltx" },
                    { ".xlst", ".xlsx" },
                    { ".xlsx", ".csv" },
                    { ".xlsx", ".ods" },
                    { ".xlsx", ".ots" },
                    { ".xlsx", ".pdf" },
                    { ".xlsx", ".xlsm" },
                    { ".xlsx", ".xltm" },
                    { ".xlsx", ".xltx" },
                    { ".xlt", ".csv" },
                    { ".xlt", ".ods" },
                    { ".xlt", ".ots" },
                    { ".xlt", ".pdf" },
                    { ".xlt", ".xlsm" },
                    { ".xlt", ".xlsx" },
                    { ".xlt", ".xltm" },
                    { ".xlt", ".xltx" },
                    { ".xltm", ".csv" },
                    { ".xltm", ".ods" },
                    { ".xltm", ".ots" },
                    { ".xltm", ".pdf" },
                    { ".xltm", ".xlsm" },
                    { ".xltm", ".xlsx" },
                    { ".xltm", ".xltx" },
                    { ".xltx", ".csv" },
                    { ".xltx", ".ods" },
                    { ".xltx", ".ots" },
                    { ".xltx", ".pdf" },
                    { ".xltx", ".xlsm" },
                    { ".xltx", ".xlsx" },
                    { ".xltx", ".xltm" },
                    { ".xml", ".docm" },
                    { ".xml", ".docx" },
                    { ".xml", ".dotm" },
                    { ".xml", ".dotx" },
                    { ".xml", ".epub" },
                    { ".xml", ".fb2" },
                    { ".xml", ".html" },
                    { ".xml", ".odt" },
                    { ".xml", ".ott" },
                    { ".xml", ".pdf" },
                    { ".xml", ".rtf" },
                    { ".xml", ".txt" },
                    { ".xps", ".docm" },
                    { ".xps", ".docx" },
                    { ".xps", ".dotm" },
                    { ".xps", ".dotx" },
                    { ".xps", ".epub" },
                    { ".xps", ".fb2" },
                    { ".xps", ".html" },
                    { ".xps", ".odt" },
                    { ".xps", ".ott" },
                    { ".xps", ".pdf" },
                    { ".xps", ".rtf" },
                    { ".xps", ".txt" }
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
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "visible" },
                values: new object[] { -3, null, "free,total_size:2147483648,manager:3,room:12", "startup", false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "price", "product_id", "visible" },
                values: new object[] { -2, null, "audit,ldap,sso,whitelabel,thirdparty,restore,oauth,contentsearch,total_size:107374182400,file_size:1024,manager:1", "admin", 30m, "1002", true });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "visible" },
                values: new object[] { -1, null, "trial,audit,ldap,sso,whitelabel,thirdparty,restore,oauth,total_size:107374182400,file_size:100,manager:1", "trial", false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[,]
                {
                    { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null },
                    { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_acl",
                columns: new[] { "action", "object", "subject", "tenant", "acetype" },
                values: new object[,]
                {
                    { new Guid("ef5e6790-f346-4b6e-b662-722bc28cb0db"), "", new Guid("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), -1, 0 },
                    { new Guid("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"), "", new Guid("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), -1, 0 },
                    { new Guid("e0759a42-47f0-4763-a26a-d5aa665bec35"), "", new Guid("712d9ec3-5d2b-4b13-824f-71f00191dcca"), -1, 0 },
                    { new Guid("08d75c97-cf3f-494b-90d1-751c941fe2dd"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("0d1f72a8-63da-47ea-ae42-0900e4ac72a9"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("13e30b51-5b4d-40a5-8575-cb561899eeb1"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("19f658ae-722b-4cd8-8236-3ad150801d96"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("2c6552b3-b2e0-4a00-b8fd-13c161e337b1"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("40bf31f4-3132-4e76-8d5c-9828a89501a3"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("49ae8915-2b30-4348-ab74-b152279364fb"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("948ad738-434b-4a88-8e38-7569d332910a"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("9d75a568-52aa-49d8-ad43-473756cd8903"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("d49f4e30-da10-4b39-bc6d-b41ef6e039d3"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("d852b66f-6719-45e1-8657-18f0bb791690"), "", new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), -1, 0 },
                    { new Guid("13e30b51-5b4d-40a5-8575-cb561899eeb1"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("49ae8915-2b30-4348-ab74-b152279364fb"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("9018c001-24c2-44bf-a1db-d1121a570e74"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("d1f3b53d-d9e2-4259-80e7-d24380978395"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("e0759a42-47f0-4763-a26a-d5aa665bec35"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("f11e88d7-f185-4372-927c-d88008d2c483"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"), "", new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), -1, 0 },
                    { new Guid("00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("0d68b142-e20a-446e-a832-0d6b0b65a164"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("14be970f-7af5-4590-8e81-ea32b5f7866d"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("18ecc94d-6afa-4994-8406-aee9dff12ce2"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("298530eb-435e-4dc6-a776-9abcd95c70e9"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("430eaf70-1886-483c-a746-1a18e3e6bb63"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("557d6503-633b-4490-a14c-6473147ce2b3"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("662f3db7-9bc8-42cf-84da-2765f563e9b0"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("724cbb75-d1c9-451e-bae0-4de0db96b1f7"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("7cb5c0d1-d254-433f-abe3-ff23373ec631"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("91b29dcd-9430-4403-b17a-27d09189be88"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("a18480a4-6d18-4c71-84fa-789888791f45"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("b630d29b-1844-4bda-bbbe-cf5542df3559"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("d7cdb020-288b-41e5-a857-597347618533"), "", new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), -1, 0 },
                    { new Guid("088d5940-a80f-4403-9741-d610718ce95c"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("08d66144-e1c9-4065-9aa1-aa4bba0a7bc8"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("388c29d3-c662-4a61-bf47-fc2f7094224a"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("6f05c382-8bca-4469-9424-c807a98c40d7"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|28b10049dd204f54b986873bc14ccfc7", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 1 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|37620ae5c40b45ce855a39dd7d76a1fa", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|3cfd481b46f24a4ab55cb8c0c9def02c", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 1 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|46cfa73af32046cf8d5bcd82e1d67f26", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6a598c7491ae437da5f4ad339bd11bb2", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 1 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|742cf945cbbc4a5782d61600a12cf8ca", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 1 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|853b6eb973ee438d9b098ffeedf36234", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 1 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 1 },
                    { new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("9018c001-24c2-44bf-a1db-d1121a570e74"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("a362fe79-684e-4d43-a599-65bc1f4e167f"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("c426c349-9ad4-47cd-9b8f-99fc30675951"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("d11ebcb9-0e6e-45e6-a6d0-99c41d687598"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("d1f3b53d-d9e2-4259-80e7-d24380978395"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("e0759a42-47f0-4763-a26a-d5aa665bec35"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("fbc37705-a04c-40ad-a68c-ce2f0423f397"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 },
                    { new Guid("fcac42b8-9386-48eb-a938-d19b3c576912"), "", new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), -1, 0 }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_settings",
                columns: new[] { "id", "tenant", "last_modified", "value" },
                values: new object[,]
                {
                    { "CompanyWhiteLabelSettings", -1, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new byte[] { 245, 71, 4, 138, 72, 101, 23, 21, 135, 217, 206, 188, 138, 73, 108, 96, 29, 150, 3, 31, 44, 28, 62, 145, 96, 53, 57, 66, 238, 118, 93, 172, 211, 22, 244, 181, 244, 40, 146, 67, 111, 196, 162, 27, 154, 109, 248, 255, 181, 17, 253, 127, 42, 65, 19, 90, 26, 206, 203, 145, 159, 159, 243, 105, 24, 71, 188, 165, 53, 85, 57, 37, 186, 251, 57, 96, 18, 162, 218, 80, 0, 101, 250, 100, 66, 97, 24, 51, 240, 215, 216, 169, 105, 100, 15, 253, 29, 83, 182, 236, 203, 53, 68, 251, 2, 150, 149, 148, 58, 136, 84, 37, 151, 82, 92, 227, 30, 52, 111, 40, 154, 155, 7, 126, 149, 100, 169, 87, 10, 129, 228, 138, 177, 101, 77, 67, 177, 216, 189, 201, 1, 213, 136, 216, 107, 198, 253, 221, 106, 255, 198, 17, 68, 14, 110, 90, 174, 182, 68, 222, 188, 77, 157, 19, 26, 68, 86, 97, 15, 81, 24, 171, 214, 114, 191, 175, 56, 56, 48, 52, 125, 82, 253, 113, 71, 41, 201, 5, 8, 118, 162, 191, 99, 196, 48, 198, 223, 79, 204, 174, 31, 97, 236, 20, 213, 218, 85, 34, 16, 74, 196, 209, 235, 14, 71, 209, 32, 131, 195, 84, 11, 66, 74, 19, 115, 255, 99, 69, 235, 210, 204, 15, 13, 4, 143, 127, 152, 125, 212, 91 } },
                    { "FullTextSearchSettings", -1, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new byte[] { 8, 120, 207, 5, 153, 181, 23, 202, 162, 211, 218, 237, 157, 6, 76, 62, 220, 238, 175, 67, 31, 53, 166, 246, 66, 220, 173, 160, 72, 23, 227, 81, 50, 39, 187, 177, 222, 110, 43, 171, 235, 158, 16, 119, 178, 207, 49, 140, 72, 152, 20, 84, 94, 135, 117, 1, 246, 51, 251, 190, 148, 2, 44, 252, 221, 2, 91, 83, 149, 151, 58, 245, 16, 148, 52, 8, 187, 86, 150, 46, 227, 93, 163, 95, 47, 131, 116, 207, 95, 209, 38, 149, 53, 148, 73, 215, 206, 251, 194, 199, 189, 17, 42, 229, 135, 82, 23, 154, 162, 165, 158, 94, 23, 128, 30, 88, 12, 204, 96, 250, 236, 142, 189, 211, 214, 18, 196, 136, 102, 102, 217, 109, 108, 240, 96, 96, 94, 100, 201, 10, 31, 170, 128, 192 } },
                    { "SmtpSettings", -1, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new byte[] { 240, 82, 224, 144, 161, 163, 117, 13, 173, 205, 78, 153, 97, 218, 4, 170, 81, 239, 1, 151, 226, 192, 98, 60, 241, 44, 88, 56, 191, 164, 10, 155, 72, 186, 239, 203, 227, 113, 88, 119, 49, 215, 227, 220, 158, 124, 96, 9, 116, 47, 158, 65, 93, 86, 219, 15, 10, 224, 142, 50, 248, 144, 75, 44, 68, 28, 198, 87, 198, 69, 67, 234, 238, 38, 32, 68, 162, 139, 67, 53, 220, 176, 240, 196, 233, 64, 29, 137, 31, 160, 99, 105, 249, 132, 202, 45, 71, 92, 134, 194, 55, 145, 121, 97, 197, 130, 119, 105, 131, 21, 133, 35, 10, 102, 172, 119, 135, 230, 251, 86, 253, 62, 55, 56, 146, 103, 164, 106 } }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_subscription",
                columns: new[] { "action", "object", "recipient", "source", "tenant", "unsubscribed" },
                values: new object[,]
                {
                    { "AddRelationshipEvent", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, false },
                    { "CreateNewContact", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, false },
                    { "ExportCompleted", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, false },
                    { "ResponsibleForOpportunity", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, false },
                    { "ResponsibleForTask", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, false },
                    { "SetAccess", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, false },
                    { "new bookmark created", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "28b10049-dd20-4f54-b986-873bc14ccfc7", -1, false },
                    { "BirthdayReminder", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "37620ae5-c40b-45ce-855a-39dd7d76a1fa", -1, false },
                    { "calendar_sharing", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "40650da3-f7c1-424c-8c89-b9c115472e08", -1, false },
                    { "event_alert", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "40650da3-f7c1-424c-8c89-b9c115472e08", -1, false },
                    { "new feed", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6504977c-75af-4691-9099-084d3ddeea04", -1, false },
                    { "new post", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6a598c74-91ae-437d-a5f4-ad339bd11bb2", -1, false },
                    { "sharedocument", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6fe286a4-479e-4c25-a8d9-0156e332b0c0", -1, false },
                    { "sharefolder", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6fe286a4-479e-4c25-a8d9-0156e332b0c0", -1, false },
                    { "new wiki page", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "742cf945-cbbc-4a57-82d6-1600a12cf8ca", -1, false },
                    { "new topic in forum", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "853b6eb9-73ee-438d-9b09-8ffeedf36234", -1, false },
                    { "new photo uploaded", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "9d51954f-db9b-4aed-94e3-ed70b914e101", -1, false },
                    { "admin_notify", "", "cd84e66b-b803-40fc-99f9-b2969a54a1de", "asc.web.studio", -1, false },
                    { "periodic_notify", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1, false },
                    { "rooms_activity", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1, false },
                    { "send_whats_new", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1, false }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_subscriptionmethod",
                columns: new[] { "action", "recipient", "source", "tenant", "sender" },
                values: new object[,]
                {
                    { "AddRelationshipEvent", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, "email.sender|messanger.sender" },
                    { "CreateNewContact", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, "email.sender|messanger.sender" },
                    { "ExportCompleted", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, "email.sender|messanger.sender" },
                    { "ResponsibleForOpportunity", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, "email.sender|messanger.sender" },
                    { "ResponsibleForTask", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, "email.sender|messanger.sender" },
                    { "SetAccess", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13ff36fb-0272-4887-b416-74f52b0d0b02", -1, "email.sender|messanger.sender" },
                    { "new bookmark created", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "28b10049-dd20-4f54-b986-873bc14ccfc7", -1, "email.sender|messanger.sender" },
                    { "BirthdayReminder", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "37620ae5-c40b-45ce-855a-39dd7d76a1fa", -1, "email.sender|messanger.sender" },
                    { "calendar_sharing", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "40650da3-f7c1-424c-8c89-b9c115472e08", -1, "email.sender|messanger.sender" },
                    { "event_alert", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "40650da3-f7c1-424c-8c89-b9c115472e08", -1, "email.sender|messanger.sender" },
                    { "invitetoproject", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "milestonedeadline", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "newcommentformessage", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "newcommentformilestone", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "newcommentfortask", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "projectcreaterequest", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "projecteditrequest", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "removefromproject", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "responsibleforproject", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "responsiblefortask", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "taskclosed", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6045b68c-2c2e-42db-9e53-c272e814c4ad", -1, "email.sender|messanger.sender" },
                    { "new feed", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6504977c-75af-4691-9099-084d3ddeea04", -1, "email.sender|messanger.sender" },
                    { "new post", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6a598c74-91ae-437d-a5f4-ad339bd11bb2", -1, "email.sender|messanger.sender" },
                    { "sharedocument", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6fe286a4-479e-4c25-a8d9-0156e332b0c0", -1, "email.sender|messanger.sender" },
                    { "sharefolder", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6fe286a4-479e-4c25-a8d9-0156e332b0c0", -1, "email.sender|messanger.sender" },
                    { "updatedocument", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6fe286a4-479e-4c25-a8d9-0156e332b0c0", -1, "email.sender|messanger.sender" },
                    { "new wiki page", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "742cf945-cbbc-4a57-82d6-1600a12cf8ca", -1, "email.sender|messanger.sender" },
                    { "new topic in forum", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "853b6eb9-73ee-438d-9b09-8ffeedf36234", -1, "email.sender|messanger.sender" },
                    { "new photo uploaded", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "9d51954f-db9b-4aed-94e3-ed70b914e101", -1, "email.sender|messanger.sender" },
                    { "admin_notify", "cd84e66b-b803-40fc-99f9-b2969a54a1de", "asc.web.studio", -1, "email.sender" },
                    { "periodic_notify", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1, "email.sender" },
                    { "send_whats_new", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1, "email.sender" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "id", "activation_status", "bithdate", "create_on", "email", "firstname", "last_modified", "lastname", "phone_activation", "removed", "sex", "status", "tenant", "terminateddate", "username", "workfromdate" },
                values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Administrator", new DateTime(2021, 3, 9, 9, 52, 55, 765, DateTimeKind.Utc).AddTicks(1420), "", 0, false, null, 1, 1, null, "administrator", new DateTime(2021, 3, 9, 9, 52, 55, 764, DateTimeKind.Utc).AddTicks(9157) });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_usergroup",
                columns: new[] { "ref_type", "tenant", "groupid", "userid", "last_modified", "removed" },
                values: new object[] { 0, 1, new Guid("cd84e66b-b803-40fc-99f9-b2969a54a1de"), new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_usersecurity",
                columns: new[] { "userid", "LastModified", "pwdhash", "tenant" },
                values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", 1 });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "webstudio_settings",
                columns: new[] { "ID", "TenantID", "UserID", "Data" },
                values: new object[] { new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"), 1, new Guid("00000000-0000-0000-0000-000000000000"), "{\"Completed\":false}" });

            migrationBuilder.CreateIndex(
                name: "uid",
                schema: "onlyoffice",
                table: "account_links",
                column: "uid");

            migrationBuilder.CreateIndex(
                name: "date",
                schema: "onlyoffice",
                table: "audit_events",
                columns: new[] { "tenant_id", "date" });

            migrationBuilder.CreateIndex(
                name: "expires_on",
                table: "backup_backup",
                column: "expires_on");

            migrationBuilder.CreateIndex(
                name: "is_scheduled",
                table: "backup_backup",
                column: "is_scheduled");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "backup_backup",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "core_group",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "parentid",
                table: "core_group",
                columns: new[] { "tenant", "parentid" });

            migrationBuilder.CreateIndex(
                name: "email",
                schema: "onlyoffice",
                table: "core_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user",
                column: "tenant");

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
                name: "tenant_core_userphoto",
                schema: "onlyoffice",
                table: "core_userphoto",
                column: "tenant");

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
                name: "tenant_id",
                table: "event_bus_integration_event_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "aggregated_date",
                schema: "onlyoffice",
                table: "feed_aggregate",
                columns: new[] { "tenant", "aggregated_date" });

            migrationBuilder.CreateIndex(
                name: "modified_date",
                schema: "onlyoffice",
                table: "feed_aggregate",
                columns: new[] { "tenant", "modified_date" });

            migrationBuilder.CreateIndex(
                name: "product",
                schema: "onlyoffice",
                table: "feed_aggregate",
                columns: new[] { "tenant", "product" });

            migrationBuilder.CreateIndex(
                name: "IX_feed_readed_tenant_id",
                schema: "onlyoffice",
                table: "feed_readed",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "user_id_feed_users",
                schema: "onlyoffice",
                table: "feed_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "left_node",
                schema: "onlyoffice",
                table: "files_bunch_objects",
                column: "left_node");

            migrationBuilder.CreateIndex(
                name: "folder_id",
                schema: "onlyoffice",
                table: "files_file",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "id",
                schema: "onlyoffice",
                table: "files_file",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "modified_on_files_file",
                schema: "onlyoffice",
                table: "files_file",
                column: "modified_on");

            migrationBuilder.CreateIndex(
                name: "tenant_id_folder_id_content_length",
                schema: "onlyoffice",
                table: "files_file",
                columns: new[] { "tenant_id", "folder_id", "content_length" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_folder_id_modified_on",
                schema: "onlyoffice",
                table: "files_file",
                columns: new[] { "tenant_id", "folder_id", "modified_on" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_folder_id_title",
                schema: "onlyoffice",
                table: "files_file",
                columns: new[] { "tenant_id", "folder_id", "title" });

            migrationBuilder.CreateIndex(
                name: "modified_on_files_folder",
                schema: "onlyoffice",
                table: "files_folder",
                column: "modified_on");

            migrationBuilder.CreateIndex(
                name: "parent_id",
                schema: "onlyoffice",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_parent_id_modified_on",
                schema: "onlyoffice",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id", "modified_on" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_parent_id_title",
                schema: "onlyoffice",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id", "title" });

            migrationBuilder.CreateIndex(
                name: "folder_id_files_folder_tree",
                schema: "onlyoffice",
                table: "files_folder_tree",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "linked_for_files_link",
                schema: "onlyoffice",
                table: "files_link",
                columns: new[] { "tenant_id", "source_id", "linked_id", "linked_for" });

            migrationBuilder.CreateIndex(
                name: "owner",
                schema: "onlyoffice",
                table: "files_security",
                column: "owner");

            migrationBuilder.CreateIndex(
                name: "tenant_id_files_security",
                schema: "onlyoffice",
                table: "files_security",
                columns: new[] { "entry_id", "tenant_id", "entry_type", "owner" });

            migrationBuilder.CreateIndex(
                name: "name_files_tag",
                schema: "onlyoffice",
                table: "files_tag",
                columns: new[] { "tenant_id", "owner", "name", "flag" });

            migrationBuilder.CreateIndex(
                name: "create_on_files_tag_link",
                schema: "onlyoffice",
                table: "files_tag_link",
                column: "create_on");

            migrationBuilder.CreateIndex(
                name: "entry_id",
                schema: "onlyoffice",
                table: "files_tag_link",
                columns: new[] { "tenant_id", "entry_type", "entry_id" });

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_files_thirdparty_app_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_app",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "index_1",
                schema: "onlyoffice",
                table: "files_thirdparty_id_mapping",
                columns: new[] { "tenant_id", "hash_id" });

            migrationBuilder.CreateIndex(
                name: "user_id",
                schema: "onlyoffice",
                table: "firebase_users",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "worker_type_name",
                table: "hosting_instance_registration",
                column: "worker_type_name");

            migrationBuilder.CreateIndex(
                name: "date_login_events",
                schema: "onlyoffice",
                table: "login_events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_login_events_tenant_id",
                schema: "onlyoffice",
                table: "login_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "tenant_id_login_events",
                schema: "onlyoffice",
                table: "login_events",
                columns: new[] { "user_id", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "state",
                schema: "onlyoffice",
                table: "notify_info",
                column: "state");

            migrationBuilder.CreateIndex(
                name: "IX_notify_queue_tenant_id",
                schema: "onlyoffice",
                table: "notify_queue",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "tgId",
                schema: "onlyoffice",
                table: "telegram_users",
                column: "telegram_user_id");

            migrationBuilder.CreateIndex(
                name: "tenant_tenants_iprestrictions",
                schema: "onlyoffice",
                table: "tenants_iprestrictions",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "last_modified_tenants_quotarow",
                schema: "onlyoffice",
                table: "tenants_quotarow",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "tenant_tenants_tariff",
                schema: "onlyoffice",
                table: "tenants_tariff",
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

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "webhooks_config",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_webhooks_logs_config_id",
                table: "webhooks_logs",
                column: "config_id");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "webhooks_logs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ID",
                schema: "onlyoffice",
                table: "webstudio_settings",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "visitdate",
                schema: "onlyoffice",
                table: "webstudio_uservisit",
                column: "visitdate");

            migrationBuilder.CreateIndex(
                name: "tenant_webplugins",
                schema: "onlyoffice",
                table: "webplugins",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_links",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "audit_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "backup_backup");

            migrationBuilder.DropTable(
                name: "backup_schedule");

            migrationBuilder.DropTable(
                name: "core_acl",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_group");

            migrationBuilder.DropTable(
                name: "core_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_subscription",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_subscriptionmethod",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_user",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_userdav",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usergroup",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_userphoto",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usersecurity",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "dbip_lookup");

            migrationBuilder.DropTable(
                name: "event_bus_integration_event_log");

            migrationBuilder.DropTable(
                name: "feed_last",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "feed_readed",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "feed_users",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_bunch_objects",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_converts",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_file",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_folder_tree",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_link",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_properties",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_security",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_tag",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_tag_link",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_thirdparty_account",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_thirdparty_app",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_thirdparty_id_mapping",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "firebase_users",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "hosting_instance_registration");

            migrationBuilder.DropTable(
                name: "login_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mobile_app_install",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "notify_info",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "notify_queue",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "telegram_users",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_forbiden",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_iprestrictions",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_quota",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_quotarow",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tariff",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tariffrow",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_version",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webhooks");

            migrationBuilder.DropTable(
                name: "webhooks_logs");

            migrationBuilder.DropTable(
                name: "webstudio_index",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "feed_aggregate",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_folder",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webhooks_config");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webplugins",
                schema: "onlyoffice");
        }
    }
}
