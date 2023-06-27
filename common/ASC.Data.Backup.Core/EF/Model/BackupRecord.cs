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

public class BackupRecord : BaseEntity
{
    public Guid Id { get; set; }
    public int TenantId { get; set; }
    public bool IsScheduled { get; set; }
    public string Name { get; set; }
    public string Hash { get; set; }
    public BackupStorageType StorageType { get; set; }
    public string StorageBasePath { get; set; }
    public string StoragePath { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ExpiresOn { get; set; }
    public string StorageParams { get; set; }
    public bool Removed { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class BackupRecordExtension
{
    public static ModelBuilderWrapper AddBackupRecord(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<BackupRecord>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddBackupRecord, Provider.MySql)
            .Add(PgSqlAddBackupRecord, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddBackupRecord(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackupRecord>(entity =>
        {
            entity.ToTable("backup_backup")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_id");

            entity.HasIndex(e => e.ExpiresOn)
                .HasDatabaseName("expires_on");

            entity.HasIndex(e => e.IsScheduled)
                .HasDatabaseName("is_scheduled");

            entity.HasKey(e => new { e.Id })
                .HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasColumnName("tenant_id")
                .HasColumnType("int(10)");

            entity.Property(e => e.IsScheduled)
                .IsRequired()
                .HasColumnName("is_scheduled")
                .HasColumnType("tinyint(1)");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.StorageType)
                .IsRequired()
                .HasColumnName("storage_type")
                .HasColumnType("int(10)");

            entity.Property(e => e.StorageBasePath)
                .HasColumnName("storage_base_path")
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("NULL")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.StoragePath)
                .IsRequired()
                .HasColumnName("storage_path")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreatedOn)
                .IsRequired()
                .HasColumnName("created_on")
                .HasColumnType("datetime");

            entity.Property(e => e.ExpiresOn)
                .HasColumnName("expires_on")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'0001-01-01 00:00:00'");

            entity.Property(e => e.StorageParams)
                .HasColumnName("storage_params")
                .HasColumnType("text")
                .HasDefaultValueSql("NULL")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Hash)
                 .IsRequired()
                .HasColumnName("hash")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Removed)
                .HasColumnName("removed")
                .HasColumnType("tinyint(1)")
                .IsRequired();
        });
    }
    public static void PgSqlAddBackupRecord(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackupRecord>(entity =>
        {
            entity.ToTable("backup_backup");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_id");

            entity.HasIndex(e => e.ExpiresOn)
                .HasDatabaseName("expires_on");

            entity.HasIndex(e => e.IsScheduled)
                .HasDatabaseName("is_scheduled");

            entity.HasKey(e => new { e.Id })
                .HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("char")
                .HasMaxLength(38)
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasColumnName("tenant_id")
                .HasColumnType("int")
                .HasMaxLength(10);

            entity.Property(e => e.IsScheduled)
                .IsRequired()
                .HasColumnName("is_scheduled")
                .HasColumnType("int")
                .HasMaxLength(10);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255)
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.StorageType)
                .IsRequired()
                .HasColumnName("storage_type")
                .HasColumnType("int")
                .HasMaxLength(10);

            entity.Property(e => e.StorageBasePath)
                .HasColumnName("storage_base_path")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.StoragePath)
                .IsRequired()
                .HasColumnName("storage_path")
                .HasMaxLength(255)
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreatedOn)
                .IsRequired()
                .HasColumnName("created_on")
                .HasColumnType("datetime");

            entity.Property(e => e.ExpiresOn)
                .HasColumnName("expires_on")
                .HasColumnType("datetime")
                .HasDefaultValueSql("'0001-01-01 00:00:00'");

            entity.Property(e => e.StorageParams)
                .HasColumnName("storage_params")
                .HasColumnType("text")
                .HasDefaultValueSql("NULL")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Hash)
                 .IsRequired()
                .HasColumnName("hash")
                .HasMaxLength(64)
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Removed)
                .HasColumnName("removed")
                .HasColumnType("int")
                .HasMaxLength(10)
                .IsRequired();
        });
    }
}
