using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.Files.Core.Security;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    public class DbFilesSecurity : BaseEntity, IDbFile
    {
        public int TenantId { get; set; }
        public string EntryId { get; set; }
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
            modelBuilder
                .Add(MySqlAddDbFilesSecurity, Provider.MySql)
                .Add(PgSqlAddDbFilesSecurity, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesSecurity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesSecurity>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.EntryId, e.EntryType, e.Subject })
                    .HasName("PRIMARY");

                entity.ToTable("files_security");

                entity.HasIndex(e => e.Owner)
                    .HasDatabaseName("owner");

                entity.HasIndex(e => new { e.TenantId, e.EntryType, e.EntryId, e.Owner })
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EntryType).HasColumnName("entry_type");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Security).HasColumnName("security");

                entity.Property(e => e.TimeStamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });
        }
        public static void PgSqlAddDbFilesSecurity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesSecurity>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.EntryId, e.EntryType, e.Subject })
                    .HasName("files_security_pkey");

                entity.ToTable("files_security", "onlyoffice");

                entity.HasIndex(e => e.Owner)
                    .HasDatabaseName("owner");

                entity.HasIndex(e => new { e.EntryId, e.TenantId, e.EntryType, e.Owner })
                    .HasDatabaseName("tenant_id_files_security");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasMaxLength(50);

                entity.Property(e => e.EntryType).HasColumnName("entry_type");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasMaxLength(38)
                    .IsFixedLength();

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasMaxLength(38)
                    .IsFixedLength();

                entity.Property(e => e.Security).HasColumnName("security");

                entity.Property(e => e.TimeStamp)
                    .HasColumnName("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }

    }
}
