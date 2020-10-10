using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.Files.Core.Security;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_security")]
    public class DbFilesSecurity : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("entry_id")]
        public string EntryId { get; set; }

        [Column("entry_type")]
        public FileEntryType EntryType { get; set; }

        public Guid Subject { get; set; }
        public Guid Owner { get; set; }
        public FileShare Security { get; set; }
        public DateTime TimeStamp { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, EntryId, EntryType, Subject };
        }
    }

    public static class DbFilesSecurityExtension
    {
       public static ModelBuilderWrapper AddDbFilesSecurity(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbFilesSecurity, Provider.MySql)
                .Add(PgSqlAddDbFilesSecurity, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesSecurity(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesSecurity>(entity =>
            {
                _ = entity.HasKey(e => new { e.TenantId, e.EntryId, e.EntryType, e.Subject })
                    .HasName("PRIMARY");

                _ = entity.ToTable("files_security");

                _ = entity.HasIndex(e => e.Owner)
                    .HasName("owner");

                _ = entity.HasIndex(e => new { e.TenantId, e.EntryType, e.EntryId, e.Owner })
                    .HasName("tenant_id");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.EntryType).HasColumnName("entry_type");

                _ = entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Security).HasColumnName("security");

                _ = entity.Property(e => e.TimeStamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });
    }
        public static void PgSqlAddDbFilesSecurity(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesSecurity>(entity =>
            {
                _ = entity.HasKey(e => new { e.TenantId, e.EntryId, e.EntryType, e.Subject })
                    .HasName("files_security_pkey");

                _ = entity.ToTable("files_security", "onlyoffice");

                _ = entity.HasIndex(e => e.Owner)
                    .HasName("owner");

                _ = entity.HasIndex(e => new { e.EntryId, e.TenantId, e.EntryType, e.Owner })
                    .HasName("tenant_id_files_security");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.EntryType).HasColumnName("entry_type");

                _ = entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasMaxLength(38)
                    .IsFixedLength();

                _ = entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasMaxLength(38)
                    .IsFixedLength();

                _ = entity.Property(e => e.Security).HasColumnName("security");

                _ = entity.Property(e => e.TimeStamp)
                    .HasColumnName("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }

    }
}
