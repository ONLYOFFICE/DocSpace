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

namespace ASC.Data.Backup.EF.Model;

public class BackupSchedule : BaseEntity
{
    public int TenantId { get; set; }
    public string Cron { get; set; }
    public int BackupsStored { get; set; }
    public BackupStorageType StorageType { get; set; }
    public string StorageBasePath { get; set; }
    public DateTime LastBackupTime { get; set; }
    public string StorageParams { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId };
    }
}

public static class BackupScheduleExtension
{
    public static ModelBuilderWrapper AddBackupSchedule(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<BackupSchedule>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddBackupSchedule, Provider.MySql)
            .Add(PgSqlAddBackupSchedule, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddBackupSchedule(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackupSchedule>(entity =>
        {
            entity.HasKey(e => new { e.TenantId })
                .HasName("PRIMARY");

            entity.ToTable("backup_schedule")
                .HasCharSet("utf8");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasColumnType("int(10)")
                .ValueGeneratedNever();

            entity.Property(e => e.Cron)
                .IsRequired()
                .HasColumnName("cron")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci"); ;

            entity.Property(e => e.BackupsStored)
                .IsRequired()
                .HasColumnName("backups_stored")
                .HasColumnType("int(10)");

            entity.Property(e => e.StorageType)
                .IsRequired()
                .HasColumnName("storage_type")
                .HasColumnType("int(10)");

            entity.Property(e => e.StorageBasePath)
                .HasColumnName("storage_base_path")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci")
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.LastBackupTime)
                .IsRequired()
                .HasColumnName("last_backup_time")
                .HasColumnType("datetime");

            entity.Property(e => e.StorageParams)
                .HasColumnName("storage_params")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci")
                .HasDefaultValueSql("NULL");

            entity.HasOne(e => e.Tenant)
                   .WithOne()
                   .HasForeignKey<BackupSchedule>(b => b.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
    public static void PgSqlAddBackupSchedule(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackupSchedule>(entity =>
        {
            entity.HasKey(e => new { e.TenantId })
                .HasName("PRIMARY");

            entity.ToTable("backup_schedule");

            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasColumnName("tenant_id")
                .HasMaxLength(10);

            entity.Property(e => e.Cron)
                .IsRequired()
                .HasColumnName("cron")
                .HasMaxLength(255)
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci"); ;

            entity.Property(e => e.BackupsStored)
                .IsRequired()
                .HasColumnName("backups_stored")
                .HasMaxLength(10);

            entity.Property(e => e.StorageType)
                .IsRequired()
                .HasColumnName("storage_type")
                .HasMaxLength(10);

            entity.Property(e => e.StorageBasePath)
                .HasColumnName("storage_base_path")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci")
                .HasDefaultValueSql("NULL")
                .HasMaxLength(255);

            entity.Property(e => e.LastBackupTime)
                .IsRequired()
                .HasColumnName("last_backup_time")
                .HasColumnType("datetime");

            entity.Property(e => e.StorageParams)
                .HasColumnName("storage_params")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci")
                .HasDefaultValueSql("NULL");

            entity.HasOne(e => e.Tenant)
                   .WithOne()
                   .HasForeignKey<BackupSchedule>(b => b.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
