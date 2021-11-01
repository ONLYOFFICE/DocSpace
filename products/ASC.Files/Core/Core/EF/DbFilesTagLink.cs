using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    public class DbFilesTagLink : BaseEntity, IDbFile
    {
        public int TenantId { get; set; }
        public int TagId { get; set; }
        public FileEntryType EntryType { get; set; }
        public string EntryId { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateOn { get; set; }
        public int TagCount { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, TagId, EntryId, EntryType };
        }
    }

    public static class DbFilesTagLinkExtension
    {
        public static ModelBuilderWrapper AddDbFilesTagLink(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbFilesTagLink, Provider.MySql)
                .Add(PgSqlAddDbFilesTagLink, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesTagLink(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesTagLink>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.TagId, e.EntryId, e.EntryType })
                    .HasName("PRIMARY");

                entity.ToTable("files_tag_link");

                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on");

                entity.HasIndex(e => new { e.TenantId, e.EntryId, e.EntryType })
                    .HasDatabaseName("entry_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.TagId).HasColumnName("tag_id");

                entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EntryType).HasColumnName("entry_type");

                entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.TagCount).HasColumnName("tag_count");
            });
        }
        public static void PgSqlAddDbFilesTagLink(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesTagLink>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.TagId, e.EntryType, e.EntryId })
                    .HasName("files_tag_link_pkey");

                entity.ToTable("files_tag_link", "onlyoffice");

                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on_files_tag_link");

                entity.HasIndex(e => new { e.TenantId, e.EntryType, e.EntryId })
                    .HasDatabaseName("entry_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.TagId).HasColumnName("tag_id");

                entity.Property(e => e.EntryType).HasColumnName("entry_type");

                entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasMaxLength(32);

                entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasMaxLength(38)
                    .IsFixedLength()
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.CreateOn).HasColumnName("create_on");

                entity.Property(e => e.TagCount).HasColumnName("tag_count");
            });
        }
    }

}
