using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_reserve")]
    public class ResReserve
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string Title { get; set; }
        public string CultureTitle { get; set; }
        public string TextValue { get; set; }
        public int Flag { get; set; }
    }
    public static class ResReserveExtension
    {
        public static ModelBuilderWrapper AddResReserve(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddResReserve, Provider.MySql)
                .Add(PgSqlAddResReserve, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddResReserve(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResReserve>(entity =>
            {
                _ = entity.HasKey(e => new { e.FileId, e.Title, e.CultureTitle })
                    .HasName("PRIMARY");

                _ = entity.ToTable("res_reserve");

                _ = entity.HasIndex(e => e.CultureTitle)
                    .HasName("resources_FK2");

                _ = entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                _ = entity.Property(e => e.FileId).HasColumnName("fileid");

                _ = entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.CultureTitle)
                    .HasColumnName("cultureTitle")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Flag).HasColumnName("flag");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                _ = entity.Property(e => e.TextValue)
                    .HasColumnName("textValue")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResReserve(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResReserve>(entity =>
            {
                _ = entity.HasKey(e => new { e.FileId, e.Title, e.CultureTitle })
                    .HasName("res_reserve_pkey");

                _ = entity.ToTable("res_reserve", "onlyoffice");

                _ = entity.Property(e => e.FileId).HasColumnName("fileid");

                _ = entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(120);

                _ = entity.Property(e => e.CultureTitle)
                    .HasColumnName("cultureTitle")
                    .HasMaxLength(20);

                _ = entity.Property(e => e.Flag).HasColumnName("flag");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                _ = entity.Property(e => e.TextValue).HasColumnName("textValue");
            });
        }
    }
}
