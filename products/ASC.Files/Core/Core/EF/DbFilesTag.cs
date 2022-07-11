using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
namespace ASC.Files.Core.EF
{
    public class DbFilesTag : IDbFile
    {
        public int TenantId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Owner { get; set; }
        public TagType Flag { get; set; }
    }
    public static class DbFilesTagExtension
    {
        public static ModelBuilderWrapper AddDbFilesTag(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbFilesTag, Provider.MySql)
                .Add(PgSqlAddDbFilesTag, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesTag(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesTag>(entity =>
            {
                entity.ToTable("files_tag");

                entity.HasIndex(e => new { e.TenantId, e.Owner, e.Name, e.Flag })
                    .HasDatabaseName("name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Flag).HasColumnName("flag");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddDbFilesTag(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesTag>(entity =>
            {
                entity.ToTable("files_tag", "onlyoffice");

                entity.HasIndex(e => new { e.TenantId, e.Owner, e.Name, e.Flag })
                    .HasDatabaseName("name_files_tag");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Flag).HasColumnName("flag");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasMaxLength(38);

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
    }
}
