using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.UserDbContextMySql
{
    public partial class UserDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "core_group",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant = table.Column<int>(nullable: false),
                    name = table.Column<string>(type: "varchar(128)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    categoryid = table.Column<string>(type: "varchar(38)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    parentid = table.Column<string>(type: "varchar(38)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sid = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    removed = table.Column<bool>(nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "core_subscription",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    source = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    action = table.Column<string>(type: "varchar(128)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    recipient = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    @object = table.Column<string>(name: "object", type: "varchar(128)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    unsubscribed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.source, x.action, x.recipient, x.@object });
                });

            migrationBuilder.CreateTable(
                name: "core_subscriptionmethod",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    source = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    action = table.Column<string>(type: "varchar(128)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    recipient = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sender = table.Column<string>(type: "varchar(1024)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.source, x.action, x.recipient });
                });



            migrationBuilder.CreateTable(
                name: "core_userphoto",
                columns: table => new
                {
                    userid = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant = table.Column<int>(nullable: false),
                    photo = table.Column<byte[]>(type: "mediumblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.userid);
                });


            migrationBuilder.InsertData(
                table: "core_subscription",
                columns: new[] { "tenant", "source", "action", "recipient", "object", "unsubscribed" },
                values: new object[,]
                {
                    { -1, "asc.web.studio", "send_whats_new", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "6fe286a4-479e-4c25-a8d9-0156e332b0c0", "sharedocument", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "6a598c74-91ae-437d-a5f4-ad339bd11bb2", "new post", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "853b6eb9-73ee-438d-9b09-8ffeedf36234", "new topic in forum", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "9d51954f-db9b-4aed-94e3-ed70b914e10", "new photo uploaded", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "28b10049-dd20-4f54-b986-873bc14ccfc7", "new bookmark created", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "742cf945-cbbc-4a57-82d6-1600a12cf8ca", "new wiki page", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "37620ae5-c40b-45ce-855a-39dd7d76a1fa", "BirthdayReminderd", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "6504977c-75af-4691-9099-084d3ddeea04", "new feed", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "SetAccess", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "40650da3-f7c1-424c-8c89-b9c115472e08", "calendar_sharing", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "40650da3-f7c1-424c-8c89-b9c115472e08", "event_alert", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "asc.web.studio", "admin_notify", "cd84e66b-b803-40fc-99f9-b2969a54a1de", "", false },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "ResponsibleForTask", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "AddRelationshipEvent", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "ExportCompleted", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "CreateNewContact", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "ResponsibleForOpportunity", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "", false },
                    { -1, "asc.web.studio", "periodic_notify", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false },
                    { -1, "6fe286a4-479e-4c25-a8d9-0156e332b0c0", "sharefolder", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "", false }
                });

            migrationBuilder.InsertData(
                table: "core_subscriptionmethod",
                columns: new[] { "tenant", "source", "action", "recipient", "sender" },
                values: new object[,]
                {
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "AddRelationshipEvent", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender|messanger.sender" },
                    { -1, "asc.web.studio", "periodic_notify", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender" },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "ResponsibleForOpportunity", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender|messanger.sender" },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "CreateNewContact", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender" },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "ExportCompleted", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender|messanger.sender" },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "ResponsibleForTask", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender|messanger.sender" },
                    { -1, "6fe286a4-479e-4c25-a8d9-0156e332b0c0", "updatedocument", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "asc.web.studio", "admin_notify", "cd84e66b-b803-40fc-99f9-b2969a54a1de", "email.sender" },
                    { -1, "asc.web.studio", "send_whats_new", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender" },
                    { -1, "6504977c-75af-4691-9099-084d3ddeea04", "new feed", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6a598c74-91ae-437d-a5f4-ad339bd11bb2", "new post", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "853b6eb9-73ee-438d-9b09-8ffeedf36234", "new topic in forum", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "9d51954f-db9b-4aed-94e3-ed70b914e101", "new photo uploaded", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "28b10049-dd20-4f54-b986-873bc14ccfc7", "new bookmark created", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "742cf945-cbbc-4a57-82d6-1600a12cf8ca", "new wiki page", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "37620ae5-c40b-45ce-855a-39dd7d76a1fa", "BirthdayReminder", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender|messanger.sender" },
                    { -1, "6fe286a4-479e-4c25-a8d9-0156e332b0c0", "sharedocument", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6fe286a4-479e-4c25-a8d9-0156e332b0c0", "sharefolder", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "invitetoproject", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "milestonedeadline", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "newcommentformessage", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "newcommentformilestone", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "newcommentfortask", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "projectcreaterequest", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "projecteditrequest", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "removefromproject", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "responsibleforproject", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "responsiblefortask", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "6045b68c-2c2e-42db-9e53-c272e814c4ad", "taskclosed", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "40650da3-f7c1-424c-8c89-b9c115472e08", "calendar_sharing", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "40650da3-f7c1-424c-8c89-b9c115472e08", "event_alert", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "email.sender|messanger.sender" },
                    { -1, "13ff36fb-0272-4887-b416-74f52b0d0b02", "SetAccess", "abef62db-11a8-4673-9d32-ef1d8af19dc0", "email.sender|messanger.sender" }
                });

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "core_group",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "parentid",
                table: "core_group",
                columns: new[] { "tenant", "parentid" });


            migrationBuilder.CreateIndex(
                name: "tenant",
                table: "core_userphoto",
                column: "tenant");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_acl");

            migrationBuilder.DropTable(
                name: "core_group");

            migrationBuilder.DropTable(
                name: "core_subscription");

            migrationBuilder.DropTable(
                name: "core_subscriptionmethod");

            migrationBuilder.DropTable(
                name: "core_usergroup");

            migrationBuilder.DropTable(
                name: "core_userphoto");

            migrationBuilder.DropTable(
                name: "core_usersecurity");

            migrationBuilder.DropTable(
                name: "core_user");
        }
    }
}
