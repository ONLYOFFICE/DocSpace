
using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    public class DbFilesLink : BaseEntity, IDbFile
    {
        public int TenantId { get; set; }
        public string SourceId { get; set; }
        public string LinkedId { get; set; }
        public Guid LinkedFor { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, SourceId, LinkedId };
        }
    }

    public static class DbFilesLinkExtension
    {
        public static ModelBuilderWrapper AddDbFilesLink(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbFilesLink, Provider.MySql)
                .Add(PgSqlAddDbFilesLink, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesLink(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesLink>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SourceId, e.LinkedId })
                    .HasName("PRIMARY");

                entity.ToTable("files_link");

                entity.HasIndex(e => new { e.TenantId, e.SourceId, e.LinkedId, e.LinkedFor })
                    .HasDatabaseName("linked_for");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SourceId)
                    .HasColumnName("source_id")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LinkedId)
                    .HasColumnName("linked_id")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LinkedFor)
                    .HasColumnName("linked_for")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }

        public static void PgSqlAddDbFilesLink(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesLink>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SourceId, e.LinkedId })
                    .HasName("files_link_pkey");

                entity.ToTable("files_link", "onlyoffice");

                entity.HasIndex(e => new { e.TenantId, e.SourceId, e.LinkedId, e.LinkedFor })
                    .HasDatabaseName("linked_for_files_link");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.LinkedId)
                    .HasColumnName("linked_id")
                    .HasMaxLength(32);

                entity.Property(e => e.SourceId)
                    .HasColumnName("source_id")
                    .HasMaxLength(32);

                entity.Property(e => e.LinkedFor)
                    .HasColumnName("linked_for")
                    .HasMaxLength(38)
                    .IsFixedLength()
                    .HasDefaultValueSql("NULL::bpchar");
            });
        }
    }

}
