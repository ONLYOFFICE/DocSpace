using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("webstudio_index")]
    public class DbWebstudioIndex : BaseEntity
    {
        [Key]
        [Column("index_name")]
        public string IndexName { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new[] { IndexName };
        }
    }
    public static class DbWebstudioIndexExtension
    {
        public static ModelBuilderWrapper AddDbWebstudioIndex(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbWebstudioIndex, Provider.MySql)
                .Add(PgSqlAddDbWebstudioIndex, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbWebstudioIndex(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioIndex>(entity =>
            {
                entity.HasKey(e => e.IndexName)
                    .HasName("PRIMARY");

                entity.ToTable("webstudio_index");

                entity.Property(e => e.IndexName)
                    .HasColumnName("index_name")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });
        }
        public static void PgSqlAddDbWebstudioIndex(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioIndex>(entity =>
            {
                entity.HasKey(e => e.IndexName)
                    .HasName("webstudio_index_pkey");

                entity.ToTable("webstudio_index", "onlyoffice");

                entity.Property(e => e.IndexName)
                    .HasColumnName("index_name")
                    .HasMaxLength(50);

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
