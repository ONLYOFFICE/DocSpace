using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.Migration
{
    /// <inheritdoc />
    public partial class MigrationContextUpgrade1 : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE tenants_quotarow SET tenant = -1 WHERE tenant = 0;");
            
            migrationBuilder.AddForeignKey(
                name: "FK_audit_events_tenants_tenants_tenant_id",
                table: "audit_events",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_backup_backup_tenants_tenants_tenant_id",
                table: "backup_backup",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_backup_schedule_tenants_tenants_tenant_id",
                table: "backup_schedule",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_acl_tenants_tenants_tenant",
                table: "core_acl",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_group_tenants_tenants_tenant",
                table: "core_group",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_settings_tenants_tenants_tenant",
                table: "core_settings",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_subscription_tenants_tenants_tenant",
                table: "core_subscription",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                table: "core_subscriptionmethod",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_user_tenants_tenants_tenant",
                table: "core_user",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_userdav_tenants_tenants_tenant_id",
                table: "core_userdav",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                table: "core_usergroup",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_userphoto_tenants_tenants_tenant",
                table: "core_userphoto",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usersecurity_tenants_tenants_tenant",
                table: "core_usersecurity",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_bus_integration_event_log_tenants_tenants_tenant_id",
                table: "event_bus_integration_event_log",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feed_aggregate_tenants_tenants_tenant",
                table: "feed_aggregate",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feed_readed_tenants_tenants_tenant_id",
                table: "feed_readed",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feed_users_feed_aggregate_feed_id",
                table: "feed_users",
                column: "feed_id",
                principalTable: "feed_aggregate",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_bunch_objects_tenants_tenants_tenant_id",
                table: "files_bunch_objects",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_file_tenants_tenants_tenant_id",
                table: "files_file",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_folder_tenants_tenants_tenant_id",
                table: "files_folder",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_folder_tree_files_folder_folder_id",
                table: "files_folder_tree",
                column: "folder_id",
                principalTable: "files_folder",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_link_tenants_tenants_tenant_id",
                table: "files_link",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_properties_tenants_tenants_tenant_id",
                table: "files_properties",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_security_tenants_tenants_tenant_id",
                table: "files_security",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_tag_tenants_tenants_tenant_id",
                table: "files_tag",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_tag_link_tenants_tenants_tenant_id",
                table: "files_tag_link",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_thirdparty_account_tenants_tenants_tenant_id",
                table: "files_thirdparty_account",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_thirdparty_app_tenants_tenants_tenant_id",
                table: "files_thirdparty_app",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_thirdparty_id_mapping_tenants_tenants_tenant_id",
                table: "files_thirdparty_id_mapping",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_firebase_users_tenants_tenants_tenant_id",
                table: "firebase_users",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_login_events_tenants_tenants_tenant_id",
                table: "login_events",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notify_queue_tenants_tenants_tenant_id",
                table: "notify_queue",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_telegram_users_tenants_tenants_tenant_id",
                table: "telegram_users",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_iprestrictions_tenants_tenants_tenant",
                table: "tenants_iprestrictions",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_quotarow_tenants_tenants_tenant",
                table: "tenants_quotarow",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_tariff_tenants_tenants_tenant",
                table: "tenants_tariff",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_tariffrow_tenants_tenants_tenant",
                table: "tenants_tariffrow",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webhooks_config_tenants_tenants_tenant_id",
                table: "webhooks_config",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webhooks_logs_tenants_tenants_tenant_id",
                table: "webhooks_logs",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webhooks_logs_webhooks_config_config_id",
                table: "webhooks_logs",
                column: "config_id",
                principalTable: "webhooks_config",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webstudio_settings_tenants_tenants_TenantID",
                table: "webstudio_settings",
                column: "TenantID",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webstudio_uservisit_tenants_tenants_tenantid",
                table: "webstudio_uservisit",
                column: "tenantid",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_events_tenants_tenants_tenant_id",
                table: "audit_events");

            migrationBuilder.DropForeignKey(
                name: "FK_backup_backup_tenants_tenants_tenant_id",
                table: "backup_backup");

            migrationBuilder.DropForeignKey(
                name: "FK_backup_schedule_tenants_tenants_tenant_id",
                table: "backup_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_core_acl_tenants_tenants_tenant",
                table: "core_acl");

            migrationBuilder.DropForeignKey(
                name: "FK_core_group_tenants_tenants_tenant",
                table: "core_group");

            migrationBuilder.DropForeignKey(
                name: "FK_core_settings_tenants_tenants_tenant",
                table: "core_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_core_subscription_tenants_tenants_tenant",
                table: "core_subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                table: "core_subscriptionmethod");

            migrationBuilder.DropForeignKey(
                name: "FK_core_user_tenants_tenants_tenant",
                table: "core_user");

            migrationBuilder.DropForeignKey(
                name: "FK_core_userdav_tenants_tenants_tenant_id",
                table: "core_userdav");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                table: "core_usergroup");

            migrationBuilder.DropForeignKey(
                name: "FK_core_userphoto_tenants_tenants_tenant",
                table: "core_userphoto");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usersecurity_tenants_tenants_tenant",
                table: "core_usersecurity");

            migrationBuilder.DropForeignKey(
                name: "FK_event_bus_integration_event_log_tenants_tenants_tenant_id",
                table: "event_bus_integration_event_log");

            migrationBuilder.DropForeignKey(
                name: "FK_feed_aggregate_tenants_tenants_tenant",
                table: "feed_aggregate");

            migrationBuilder.DropForeignKey(
                name: "FK_feed_readed_tenants_tenants_tenant_id",
                table: "feed_readed");

            migrationBuilder.DropForeignKey(
                name: "FK_feed_users_feed_aggregate_feed_id",
                table: "feed_users");

            migrationBuilder.DropForeignKey(
                name: "FK_files_bunch_objects_tenants_tenants_tenant_id",
                table: "files_bunch_objects");

            migrationBuilder.DropForeignKey(
                name: "FK_files_file_tenants_tenants_tenant_id",
                table: "files_file");

            migrationBuilder.DropForeignKey(
                name: "FK_files_folder_tenants_tenants_tenant_id",
                table: "files_folder");

            migrationBuilder.DropForeignKey(
                name: "FK_files_folder_tree_files_folder_folder_id",
                table: "files_folder_tree");

            migrationBuilder.DropForeignKey(
                name: "FK_files_link_tenants_tenants_tenant_id",
                table: "files_link");

            migrationBuilder.DropForeignKey(
                name: "FK_files_properties_tenants_tenants_tenant_id",
                table: "files_properties");

            migrationBuilder.DropForeignKey(
                name: "FK_files_security_tenants_tenants_tenant_id",
                table: "files_security");

            migrationBuilder.DropForeignKey(
                name: "FK_files_tag_tenants_tenants_tenant_id",
                table: "files_tag");

            migrationBuilder.DropForeignKey(
                name: "FK_files_tag_link_tenants_tenants_tenant_id",
                table: "files_tag_link");

            migrationBuilder.DropForeignKey(
                name: "FK_files_thirdparty_account_tenants_tenants_tenant_id",
                table: "files_thirdparty_account");

            migrationBuilder.DropForeignKey(
                name: "FK_files_thirdparty_app_tenants_tenants_tenant_id",
                table: "files_thirdparty_app");

            migrationBuilder.DropForeignKey(
                name: "FK_files_thirdparty_id_mapping_tenants_tenants_tenant_id",
                table: "files_thirdparty_id_mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_firebase_users_tenants_tenants_tenant_id",
                table: "firebase_users");

            migrationBuilder.DropForeignKey(
                name: "FK_login_events_tenants_tenants_tenant_id",
                table: "login_events");

            migrationBuilder.DropForeignKey(
                name: "FK_notify_queue_tenants_tenants_tenant_id",
                table: "notify_queue");

            migrationBuilder.DropForeignKey(
                name: "FK_telegram_users_tenants_tenants_tenant_id",
                table: "telegram_users");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_iprestrictions_tenants_tenants_tenant",
                table: "tenants_iprestrictions");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_quotarow_tenants_tenants_tenant",
                table: "tenants_quotarow");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_tariff_tenants_tenants_tenant",
                table: "tenants_tariff");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_tariffrow_tenants_tenants_tenant",
                table: "tenants_tariffrow");

            migrationBuilder.DropForeignKey(
                name: "FK_webhooks_config_tenants_tenants_tenant_id",
                table: "webhooks_config");

            migrationBuilder.DropForeignKey(
                name: "FK_webhooks_logs_tenants_tenants_tenant_id",
                table: "webhooks_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_webhooks_logs_webhooks_config_config_id",
                table: "webhooks_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_webstudio_settings_tenants_tenants_TenantID",
                table: "webstudio_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_webstudio_uservisit_tenants_tenants_tenantid",
                table: "webstudio_uservisit");
        }
    }
}
