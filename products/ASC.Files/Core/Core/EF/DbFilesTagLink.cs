using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_tag_link")]
    public class DbFilesTagLink : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("tag_id")]
        public int TagId { get; set; }

        [Column("entry_type")]
        public FileEntryType EntryType { get; set; }

        [Column("entry_id")]
        public string EntryId { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("tag_count")]
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
            _ = modelBuilder
                .Add(MySqlAddDbFilesTagLink, Provider.MySql)
                .Add(PgSqlAddDbFilesTagLink, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesTagLink(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesTagLink>(entity =>
            {
                _ = entity.HasKey(e => new { e.TenantId, e.TagId, e.EntryId, e.EntryType })
                    .HasName("PRIMARY");

                _ = entity.ToTable("files_tag_link");

                _ = entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                _ = entity.HasIndex(e => new { e.TenantId, e.EntryId, e.EntryType })
                    .HasName("entry_id");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.TagId).HasColumnName("tag_id");

                _ = entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.EntryType).HasColumnName("entry_type");

                _ = entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.TagCount).HasColumnName("tag_count");
            });
        }
        public static void PgSqlAddDbFilesTagLink(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesTagLink>(entity =>
            {
                _ = entity.HasKey(e => new { e.TenantId, e.TagId, e.EntryType, e.EntryId })
                    .HasName("files_tag_link_pkey");

                _ = entity.ToTable("files_tag_link", "onlyoffice");

                _ = entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on_files_tag_link");

                _ = entity.HasIndex(e => new { e.TenantId, e.EntryType, e.EntryId })
                    .HasName("entry_id");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.TagId).HasColumnName("tag_id");

                _ = entity.Property(e => e.EntryType).HasColumnName("entry_type");

                _ = entity.Property(e => e.EntryId)
                    .HasColumnName("entry_id")
                    .HasMaxLength(32);

                _ = entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasMaxLength(38)
                    .IsFixedLength()
                    .HasDefaultValueSql("NULL::bpchar");

                _ = entity.Property(e => e.CreateOn).HasColumnName("create_on");

                _ = entity.Property(e => e.TagCount).HasColumnName("tag_count");
            });
        }
    }

}
