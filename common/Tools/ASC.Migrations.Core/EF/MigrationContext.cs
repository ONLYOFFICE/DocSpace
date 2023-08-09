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

namespace ASC.Migrations.Core;

public class MigrationContext : DbContext
{
    public DbSet<AccountLinks> AccountLinks { get; set; }

    public DbSet<DbTariff> Tariffs { get; set; }
    public DbSet<DbTariffRow> TariffRows { get; set; }
    public DbSet<DbQuota> Quotas { get; set; }
    public DbSet<DbQuotaRow> QuotaRows { get; set; }

    public DbSet<MobileAppInstall> MobileAppInstall { get; set; }
    public DbSet<DbIPLookup> DbIPLookup { get; set; }
    public DbSet<Regions> Regions { get; set; }

    public DbSet<FireBaseUser> FireBaseUsers { get; set; }

    public DbSet<NotifyInfo> NotifyInfo { get; set; }
    public DbSet<NotifyQueue> NotifyQueue { get; set; }

    public DbSet<TelegramUser> TelegramUsers { get; set; }

    public DbSet<DbTenant> Tenants { get; set; }
    public DbSet<DbTenantVersion> TenantVersion { get; set; }
    public DbSet<DbTenantForbiden> TenantForbiden { get; set; }
    public DbSet<TenantIpRestrictions> TenantIpRestrictions { get; set; }
    public DbSet<DbCoreSettings> CoreSettings { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<UserSecurity> UserSecurity { get; set; }
    public DbSet<UserPhoto> Photos { get; set; }
    public DbSet<Acl> Acl { get; set; }
    public DbSet<DbGroup> Groups { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<DbSubscriptionMethod> SubscriptionMethods { get; set; }
    public DbSet<UserDav> UsersDav { get; set; }

    public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }
    public DbSet<DbWebstudioUserVisit> WebstudioUserVisit { get; set; }
    public DbSet<DbWebstudioIndex> WebstudioIndex { get; set; }

    public DbSet<InstanceRegistration> InstanceRegistrations { get; set; }

    public DbSet<AuditEvent> AuditEvents { get; set; }
    public DbSet<LoginEvent> LoginEvents { get; set; }

    public DbSet<BackupRecord> Backups { get; set; }
    public DbSet<BackupSchedule> Schedules { get; set; }

    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

    public DbSet<FeedLast> FeedLast { get; set; }
    public DbSet<FeedAggregate> FeedAggregates { get; set; }
    public DbSet<FeedUsers> FeedUsers { get; set; }
    public DbSet<FeedReaded> FeedReaded { get; set; }

    public DbSet<WebhooksConfig> WebhooksConfigs { get; set; }
    public DbSet<WebhooksLog> WebhooksLogs { get; set; }
    public DbSet<DbWebhook> Webhooks { get; set; }

    public DbSet<DbFile> Files { get; set; }
    public DbSet<DbFolder> Folders { get; set; }
    public DbSet<DbFolderTree> Tree { get; set; }
    public DbSet<DbFilesBunchObjects> BunchObjects { get; set; }
    public DbSet<DbFilesSecurity> Security { get; set; }
    public DbSet<DbFilesThirdpartyIdMapping> ThirdpartyIdMapping { get; set; }
    public DbSet<DbFilesThirdpartyAccount> ThirdpartyAccount { get; set; }
    public DbSet<DbFilesTagLink> TagLink { get; set; }
    public DbSet<DbFilesTag> Tag { get; set; }
    public DbSet<DbFilesThirdpartyApp> ThirdpartyApp { get; set; }
    public DbSet<DbFilesLink> FilesLink { get; set; }
    public DbSet<DbFilesProperties> FilesProperties { get; set; }
    public DbSet<FilesConverts> FilesConverts { get; set; }
    public DbSet<ShortLink> ShortLink { get; set; }
    public MigrationContext(DbContextOptions<MigrationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Database)
            .AddAccountLinks()
            .AddDbQuotaRow()
            .AddDbQuota()
            .AddDbTariff()
            .AddDbTariffRow()
            .AddMobileAppInstall()
            .AddDbIPLookup()
            .AddRegions()
            .AddFireBaseUsers()
            .AddNotifyInfo()
            .AddNotifyQueue()
            .AddTelegramUsers()
            .AddDbTenant()
            .AddCoreSettings()
            .AddDbTenantForbiden()
            .AddTenantIpRestrictions()
            .AddDbTenantVersion()
            .AddSubscriptionMethod()
            .AddUser()
            .AddAcl()
            .AddUserSecurity()
            .AddUserPhoto()
            .AddDbGroup()
            .AddUserGroup()
            .AddSubscription()
            .AddUserDav()
            .AddWebstudioSettings()
            .AddWebstudioUserVisit()
            .AddDbWebstudioIndex()
            .AddDbFiles()
            .AddDbFolder()
            .AddDbFolderTree()
            .AddDbFilesThirdpartyAccount()
            .AddDbFilesBunchObjects()
            .AddDbFilesSecurity()
            .AddDbFilesThirdpartyIdMapping()
            .AddDbFilesTagLink()
            .AddDbFilesTag()
            .AddDbDbFilesThirdpartyApp()
            .AddDbFilesLink()
            .AddDbFilesProperties()
            .AddFilesConverts()
            .AddInstanceRegistration()
            .AddAuditEvent()
            .AddLoginEvents()
            .AddBackupSchedule()
            .AddBackupRecord()
            .AddIntegrationEventLog()
            .AddFeedUsers()
            .AddFeedReaded()
            .AddFeedAggregate()
            .AddFeedLast()
            .AddDbWebhooks()
            .AddWebhooksConfig()
            .AddWebhooksLog()
            .AddShortLinks()
            .AddDbFunctions();
    }
}