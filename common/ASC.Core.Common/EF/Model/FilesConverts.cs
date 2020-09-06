using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("files_converts")]
    public class FilesConverts
    {
        public string Input { get; set; }
        public string Ouput { get; set; }
    }

    public static class FilesConvertsExtension
    {
        public static ModelBuilderWrapper AddFilesConverts(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddFilesConverts, Provider.MySql)
                .Add(PgSqlAddFilesConverts, Provider.Postrge);
            return modelBuilder;
        }
        public static void MySqlAddFilesConverts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilesConverts>(entity =>
            {
                entity.HasKey(e => new { e.Input, e.Ouput })
                    .HasName("PRIMARY");

                entity.ToTable("files_converts");

                entity.Property(e => e.Input)
                    .HasColumnName("input")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Ouput)
                    .HasColumnName("output")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddFilesConverts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilesConverts>(entity =>
            {
                entity.HasKey(e => new { e.Input, e.Ouput })
                    .HasName("files_converts_pkey");

                entity.ToTable("files_converts", "onlyoffice");

                entity.Property(e => e.Input)
                    .HasColumnName("input")
                    .HasMaxLength(50);

                entity.Property(e => e.Ouput)
                    .HasColumnName("output")
                    .HasMaxLength(50);
            });
        }
    }
}
