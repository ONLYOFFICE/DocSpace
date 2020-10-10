using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_group")]
    public class DbGroup : BaseEntity
    {
        public int Tenant { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ParentId { get; set; }
        public string Sid { get; set; }
        public bool Removed { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
    public static class DbGroupExtension
    {
        public static ModelBuilderWrapper AddDbGroup(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbGroup, Provider.MySql)
                .Add(PgSqlAddDbGroup, Provider.Postgre);
            return modelBuilder;
        }
        private static void MySqlAddDbGroup(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbGroup>(entity =>
            {
                _ = entity.ToTable("core_group");

                _ = entity.HasIndex(e => e.LastModified)
                    .HasName("last_modified");

                _ = entity.HasIndex(e => new { e.Tenant, e.ParentId })
                    .HasName("parentid");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.ParentId)
                    .HasColumnName("parentid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Removed).HasColumnName("removed");

                _ = entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .HasColumnType("varchar(512)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
        private static void PgSqlAddDbGroup(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbGroup>(entity =>
            {
                _ = entity.ToTable("core_group");

                _ = entity.HasIndex(e => e.LastModified)
                    .HasName("last_modified");

                _ = entity.HasIndex(e => new { e.Tenant, e.ParentId })
                    .HasName("parentid");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(38);

                _ = entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryid")
                    .HasMaxLength(38)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(128);

                _ = entity.Property(e => e.ParentId)
                    .HasColumnName("parentid")
                    .HasMaxLength(38)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Removed).HasColumnName("removed");

                _ = entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .HasMaxLength(512)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
    }
}
